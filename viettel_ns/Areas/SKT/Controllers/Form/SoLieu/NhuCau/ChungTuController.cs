using AutoMapper.Extensions;
using Dapper;
using DapperExtensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;
using Viettel.Data;
using Viettel.Domain.DomainModel;
using Viettel.Domain.Json;
using Viettel.Services;
using VIETTEL.Areas.SKT.Models;
using VIETTEL.Controllers;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Areas.SKT.Controllers
{
    public class ChungTuController : AppController
    {
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly ISharedService _sharedService = SharedService.Default;
        private readonly INganSachService _ngansachService = NganSachService.Default;
        private readonly ISKTService _sKTService = SKTService.Default;
        public int PageSize = 15;

        private readonly IEnumerable<KeyViewModel> _orderBys = new List<KeyViewModel>
        {
            new KeyViewModel("donvi", "Id_DonVi", "NS.DonVi"),
            new KeyViewModel("ngay_desc", "NgayChungTu desc", "NS.NgayChungTu.Desc"),
            new KeyViewModel("ngay_asc", "NgayChungTu asc", "NS.NgayChungTu.Asc"),
            new KeyViewModel("sochungtu", "SoChungTu", "NS.SoChungTu"),
        };

        #region public methods
        public ActionResult Index(string search = null, int page = 1, int loai = 1, string orderby = null)
        {
            if (_sharedService.CheckLock(PhienLamViec.NamLamViec, "1", "SKT_ChungTu"))
            {
                var donvis = PhienLamViec.DonViList;
                var _dvis = new List<KeyViewModel>();
                _dvis.Add(new KeyViewModel(null, "--Tất cả đơn vị--"));
                foreach (var dt in donvis)
                {
                    var nk = new KeyViewModel(dt.Key, dt.Value);
                    _dvis.Add(nk);
                }
                var vm = new ChungTuListViewModel()
                {
                    Items = getChungTuList(loai, orderby)
                    .Where(p => search == null || p.Id_DonVi == search)
                    .OrderByDescending(p => p.iLan)
                    .ThenBy(p => p.Id_PhongBanDich)
                    .ThenBy(p => p.Id_DonVi),
                    OrderBys = _orderBys,
                    Dvis = _dvis,
                    Loai = loai,
                    PagingInfo = new PagingInfo
                    {
                        CurrentPage = page,
                        ItemsPerPage = PageSize,
                        TotalItems = search == null ?
                            getChungTuList(loai, orderby).Count() :
                            getChungTuList(loai, orderby).Where(e => e.Id_DonVi == search).Count()
                    },
                };

                ViewBag.Loai = loai;
                return View(vm);
            }
            else
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }
        }
        public JsonResult Ds_Nganh(string id_phongban_dich = null)
        {
            ChecklistModel vm = new ChecklistModel("Id_DonVi",
                        _sKTService.GetNganhAll(PhienLamViec.NamLamViec, Username, id_phongban_dich == "02" ? "" : id_phongban_dich, id_phongban_dich == "02" ? "00" : PhienLamViec.iID_MaDonVi)
                        .AsEnumerable().ToDictionary(x => x.Field<string>("MaNganh"), x => x.Field<string>("TenNganh"))
                        .ToSelectList());

            return ToDropdownList(vm);
        }
        public JsonResult Ds_DonVi(string Id_PhongBanDich)
        {
            var data = _sKTService.GetDonViAll(PhienLamViec.NamLamViec, Id_PhongBanDich, PhienLamViec.iID_MaDonVi).AsEnumerable()
                .ToDictionary(x => x.Field<string>("iID_MaDonVi"), x => x.Field<string>("sMoTa"));

            //if (PhienLamViec.iID_MaPhongBan == "02")
            //{
            //    data = _sKTService.Get_DonVi_ExistData(PhienLamViec.iNamLamViec, 4, PhienLamViec.iID_MaDonVi, Id_PhongBanDich, Id_PhongBanDich).AsEnumerable()
            //    .ToDictionary(x => x.Field<string>("Id"), x => x.Field<string>("Ten"));
            //} else if (PhienLamViec.iID_MaPhongBan == "11")
            //{
            //    data = _sKTService.Get_DonVi_ExistData(PhienLamViec.iNamLamViec, 4, PhienLamViec.iID_MaDonVi, "02", Id_PhongBanDich).AsEnumerable()
            //    .ToDictionary(x => x.Field<string>("Id"), x => x.Field<string>("Ten"));
            //}

            var vm = new ChecklistModel("DonVi", data.ToSelectList());
            return ToDropdownList(vm);
        }
        #endregion

        #region edit

        public ActionResult Edit(Guid id)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var entity = conn.Get<SKT_ChungTu>(id);
                if (entity == null)
                    return RedirectToAction("Create");

                var vm = createViewModel(entity: entity);
                return View(vm);
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Edit(ChungTuEditViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            using (var conn = _connectionFactory.GetConnection())
            {
                var entity = conn.Get<SKT_ChungTu>(vm.Id);

                entity.MapFrom(vm);

                if (string.IsNullOrWhiteSpace(entity.Id_PhongBanDich))
                    entity.Id_PhongBanDich = entity.Id_PhongBan;

                entity.DateModified = DateTime.Now;
                entity.UserModifier = Username;
                entity.AuditCount = entity.AuditCount.GetValueOrDefault() + 1;

                conn.Update(entity);
            }

            return RedirectToAction("Index", new { loai = vm.iLoai });
        }

        #endregion

        #region delete

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Guid id)
        {
            var success = false;
            var msg = string.Empty;

            using (var conn = _connectionFactory.GetConnection())
            {
                var entity = conn.Get<SKT_ChungTu>(id);
                if (entity != null && (entity.UserCreator == Username &&
                !_sKTService.CheckLock(PhienLamViec.iNamLamViec.ToValue<int>(), entity.Id_DonVi, entity.Id_PhongBan, Username)))
                {
                    try
                    {                        
                        _sKTService.DeleteChungTuCT(id, "SKT_ChungTuChiTiet");
                        conn.Open();
                        conn.Delete(entity);
                        success = true;
                        
                    }
                    catch (Exception ex)
                    {                        
                        #if DEBUG
                        msg = ex.Message;
                        #endif
                    }
                    return Json(new { success, msg }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    msg = L("Error.DeleteFail");

                    return Json(new { success, msg }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        #endregion

        #region create

        [HttpGet]
        public ActionResult Create(int Loai = 1)
        {
            var vm = createViewModel(Loai);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ChungTuEditViewModel vm)
        {
            var isvalid = ModelState.IsValid;

            if (vm.Id_DonVi == "-1")
            {
                ModelState.AddModelError("Id_DonVi", (vm.iLoai == 1 || vm.iLoai == 3 || vm.iLoai == 4) ? "- Chưa chọn đơn vị cần nhập số liệu!" : "Bạn chưa chọn ngành bảo đảm cần nhập số liệu!");
            }           

            if (checkCreate(vm.iLoai, Username, PhienLamViec.iNamLamViec, vm.Id_DonVi, PhienLamViec.iID_MaPhongBan, PhienLamViec.iID_MaPhongBan == "02" ? vm.Id_PhongBanDich : PhienLamViec.iID_MaPhongBan))
                ModelState.AddModelError("Id_DonVi", (vm.iLoai == 1 || vm.iLoai == 2 || vm.iLoai == 3 || vm.iLoai == 4) ? "- Đơn vị này đã có chứng từ nhập trước rồi! Đề nghị đ/c điều chỉnh số liệu ở chứng từ cũ!" : "Ngành bảo đảm này đã có chứng từ nhập trước rồi! Đề nghị đ/c điều chỉnh số liệu ở chứng từ cũ!");

            // kiem tra xem b2 da khoa so lieu hay chua
            if (_sKTService.CheckLock(PhienLamViec.iNamLamViec.ToValue<int>(), vm.Id_DonVi, PhienLamViec.iID_MaPhongBan, Username))
            {
                ModelState.AddModelError("lock", $"- Không được nhập số liệu cho {vm.TenDonVi} vì đã khóa số liệu của phòng ban hoặc của đ/c.!");
            }

            if (ModelState.Any(x => x.Value.Errors.Count > 0))
            {
                vm.DonViList = PhienLamViec.DonViList.ToSelectList("-1", "-- Chọn đơn vị --");
                vm.PhongBanList = _ngansachService.GetPhongBansQuanLyNS().AsEnumerable().ToDictionary(x => x.Field<string>("sKyHieu"), x => x.Field<string>("sMoTa")).ToSelectList("-1", "-- Chọn phòng ban --");
                return View(vm);
            }

            vm.Id = Guid.NewGuid();
            if (vm.iLoai == 1 || vm.iLoai == 3 || vm.iLoai == 4) { 
                var donvi = _ngansachService.GetDonVi(PhienLamViec.iNamLamViec, vm.Id_DonVi);
                vm.TenDonVi = $"{donvi.sMoTa}";
            }
            else if (vm.iLoai == 2 || vm.iLoai == 5 || vm.iLoai == 6)
            {
                var nganh = _ngansachService.Nganh_Get(PhienLamViec.iNamLamViec, vm.Id_DonVi);
                vm.TenDonVi = $"{nganh.iID_MaNganh} - Ngành {nganh.sTenNganh}";
            }

            var entity = vm.MapTo<SKT_ChungTu>();
            entity.NamLamViec = int.Parse(PhienLamViec.iNamLamViec);
            entity.Id_PhongBan = PhienLamViec.iID_MaPhongBan;
            var phongban = _ngansachService.GetPhongBanById(PhienLamViec.iID_MaPhongBan);
            entity.TenPhongBan = $"{phongban.sKyHieu} - {phongban.sMoTa}";            

            if (PhienLamViec.iID_MaPhongBan == "02" || PhienLamViec.iID_MaPhongBan == "11")
            {
                entity.Id_PhongBanDich = vm.Id_PhongBanDich;
            }
            else
            {
                entity.Id_PhongBanDich = PhienLamViec.iID_MaPhongBan;
            }
            var phongbandich = _ngansachService.GetPhongBanById(entity.Id_PhongBanDich);
            entity.TenPhongBanDich = $"{phongbandich.sKyHieu} - {phongbandich.sMoTa}";
            entity.iLoai = vm.iLoai;

            entity.NgayChungTu = DateTime.Now;
            entity.DateCreated = DateTime.Now;
            entity.UserCreator = Username;

            using (var conn = _connectionFactory.GetConnection())
            {
                entity.SoChungTu = getSoChungTuLast(conn);
                entity.iLan = getSoLanLast(entity, conn);
                conn.Insert(entity);
            }

            if (vm.iLoai != 5 && vm.iLoai != 6)
            {
                return RedirectToAction("Index", "ChungTuChiTiet", new { vm.Id });
            }
            else
            {
                return RedirectToAction("Index", "NganhChungTuChiTiet", new { vm.Id });
            }
        }

        #endregion

        #region block   
        [HttpPost]
        public ActionResult OpenOrLock(Guid id)
        {
            var success = false;
            var msg = string.Empty;

            using (var conn = _connectionFactory.GetConnection())
            {
                var entity = conn.Get<SKT_ChungTu>(id);                                
                try
                {
                    _sKTService.LockChungTu(id, "SKT_ChungTu");
                    success = true;                    
                }
                catch (Exception ex)
                {
#if DEBUG
                    msg = ex.Message;
#endif
                }
                return Json(new { success, msg }, JsonRequestBehavior.AllowGet);               
            }
        }
        public PartialViewResult LockOrOpen()
        {
            var vm = getLockViewModel();
            return PartialView("_editor_id", vm);
        }
        [HttpPost]
        public ActionResult UpdateLock(
            int nam,
            int id_lock,
            string iloais = null,
            string donvis = null)
        {

            var success = false;
            var msg = "";
            if (!ModelState.IsValid)
            {
            }

            if (id_lock != -1)
            {
                try
                {
                    using (var conn = ConnectionFactory.Default.GetConnection())
                    {
                        conn.Open();
                        var sql_update = FileHelpers.GetSqlQuery("skt_lockoropen_bupdate.sql");
                        using (var cmd = new SqlCommand(sql_update, conn))
                        {
                            cmd.AddParams(new
                            {
                                nam,
                                id_lock,
                                iloais = iloais.ToParamString(),
                                phongban = PhienLamViec.iID_MaPhongBan.ToParamString(),
                                donvis = donvis.ToParamString(),
                            });

                            var result = cmd.ExecuteNonQuery();
                            success = result > 0;
                        }
                        conn.Close();
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    msg = ex.Message;
#else
                            msg = L("Msg.Save.Failed");
#endif
                }
            }

            if (success)
            {
                msg = "Đang thực hiện, xin chờ trong giây lát!";

            }

            return Json(new js_msg()
            {
                success = success,
                text = msg,
                title = success ? (id_lock == 1 ? "Đã khóa thành công!" : "Đã mở thành công!") : "Không thực hiện thành công",
            });
        }
        public JsonResult Ds_Lock(
            int nam,
            int id_lock,
            string phongban,
            string iloais = null,
            string group = null
            )
        {
            var data = getTable(nam, id_lock, phongban, iloais);
            if (string.IsNullOrEmpty(group) || group == "undefined")
            {
                var dt = data.SelectDistinct("Loai", "iLoai,sLoai");
                return ToCheckboxList(new ChecklistModel("iLoai", dt.ToSelectList("iLoai", "sLoai")));
            }
            else if (group == "iLoai")
            {
                var dt = data.SelectDistinct("Donvi", "Id_DonVi,TenDonVi");
                return ToCheckboxList(new ChecklistModel("DonVi", dt.ToSelectList("Id_DonVi", "TenDonVi")));
            }
            else
            {
                return null;
            }
        }        
        #endregion

        #region
        public PartialViewResult ChuyenT()
        {
            var vm = getChuyenViewModel();
            return PartialView("_chuyen_id", vm);
        }
        [HttpPost]
        public ActionResult ChuyenT(
            string id_donvi)
        {
            var success = false;
            var msg = ""; 
           
            try
            {
                using (var conn = ConnectionFactory.Default.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("skt_chuyen", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.AddParams(new
                        {
                            nam = PhienLamViec.NamLamViec,
                            phongban = PhienLamViec.iID_MaPhongBan,
                            donvi = id_donvi,
                            user = Username,
                        });
                        var result = cmd.ExecuteNonQuery();
                        success = result > 0;
                    }                       
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                msg = ex.Message;
#else
                        msg = L("Msg.Save.Failed");
#endif
            }

            if (success)
            {
                msg = "Đang thực hiện, xin chờ trong giây lát!";
            }

            return Json(new js_msg()
            {
                success = success,
                text = msg,
                title = success ? "Đã thực hiện thành công" : "Không thành công",
            });
        }
        #endregion

        #region private methods

        private ChungTuEditViewModel createViewModel(int loai = 1, SKT_ChungTu entity = null)
        {
            if (entity == null)
            {
                entity = new SKT_ChungTu()
                {
                    NgayChungTu = DateTime.Now.Date,
                    iLoai = loai,
                };
            }
            var vm = entity.MapTo<ChungTuEditViewModel>();

            // them moi
            if (entity.Id == Guid.Empty)
            {                

                if (PhienLamViec.PhongBan.sKyHieu != "02" && PhienLamViec.PhongBan.sKyHieu != "11")
                {
                    var donviList = PhienLamViec.DonViList.ToSelectList();
                    vm.DonViList = donviList;
                }
                else
                {
                    var phongbanList = new DataTable();
                    if (loai == 1 || loai == 3) { 
                        phongbanList = _ngansachService.GetPhongBansQuanLyNS("02,05,06,07,08,10,17");
                    }
                    else
                    {
                        if (loai == 5) { 
                            phongbanList = _ngansachService.GetPhongBanQuanLyNS("02,07,10");
                        }
                        else
                        {
                            phongbanList = _ngansachService.GetPhongBanQuanLyNS("07,10");
                        }
                    }
                    vm.PhongBanList = phongbanList.AsEnumerable().ToDictionary(x => x.Field<string>("sKyHieu"), x => x.Field<string>("sMoTa")).ToSelectList();
                    var donviList = PhienLamViec.DonViList.ToSelectList();
                    vm.DonViList = donviList;
                }

            }

            return vm;
        }

        private IEnumerable<ChungTuDetailsViewModel> getChungTuList(int loai, string orderby = null)
        {
            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                var sql = @"
                            select  * 
                            from    SKT_ChungTu 
                            where   NamLamViec=@iNamLamViec
                                    and Id_PhongBan=@Id_PhongBan
                                    and iLoai=@loai
                                    and Id_DonVi in (select * from f_split(@donvis))
                            order by  iLan DESC, @orderby

                            ";
                #region orderby

                orderby = string.IsNullOrWhiteSpace(orderby) ?
                    _orderBys.First().Key :
                    _orderBys.FirstOrDefault(x => x.Value == orderby).Key;
                var donvis = _ngansachService.GetDonvisByUser(Username, PhienLamViec.NamLamViec);

                sql = sql.Replace("@orderby", orderby);

                #endregion

                var items = conn.Query<SKT_ChungTu>(sql,
                    param: new
                    {
                        PhienLamViec.iNamLamViec,
                        loai,
                        Id_PhongBan = PhienLamViec.iID_MaPhongBan,
                        donvis = donvis.ToParamString(),
                    },
                    commandType: System.Data.CommandType.Text);

                return items.Select(x => x.MapTo<ChungTuDetailsViewModel>()).ToList();
            }

        }
        private int getSoChungTuLast(IDbConnection conn = null)
        {
            var result = 1;
            var dispose = conn == null;

            if (dispose)
                conn = _connectionFactory.GetConnection();

            var sql = "select max(SoChungTu) from SKT_ChungTu";
            var max = conn.QueryFirstOrDefault<int?>(sql);
            if (max.HasValue)
                result = max.GetValueOrDefault() + 1;

            if (dispose)
                conn.Dispose();

            return result;
        }
        private bool checkCreate(int loai, string user, string nam, string id_donvi, string id_phongban, string id_phongbandich)
        {
            var result = 0;
            var conn = _connectionFactory.GetConnection();

            var sql = @"select  max(iLan)
                        from    SKT_ChungTu 
                        where   NamLamViec = @nam
                                and (@loai in (1,2) and iLoai = @loai)
                                and Id_PhongBan = @id_phongban
                                and Id_PhongBanDich = @id_phongbandich
                                and Id_DonVi = @id_donvi";


            var max = conn.QueryFirstOrDefault<int?>(sql,
                    param: new
                    {
                        nam = nam,
                        loai = loai,
                        id_phongban = id_phongban,
                        id_phongbandich = id_phongbandich,
                        id_donvi = id_donvi,
                        user = user,
                    },
                    commandType: System.Data.CommandType.Text);

            if (max.HasValue)
                result = max.GetValueOrDefault();

            conn.Dispose();
            if (result == 0 || (result != 0 && loai != 1 && loai != 2))
                return false;
            else
                return true;
        }
        private int getSoLanLast(SKT_ChungTu chungTu, IDbConnection conn = null)
        {
            var result = 1;
            var dispose = conn == null;

            if (dispose)
                conn = _connectionFactory.GetConnection();

            var sql = @"select  max(iLan)
                        from    SKT_ChungTu 
                        where   NamLamViec = @nam
                                and iLoai = @loai
                                and Id_PhongBan = @id_phongban
                                and Id_PhongBanDich = @id_phongbandich
                                and Id_DonVi = @id_donvi
                                and UserCreator = @user";


            var max = conn.QueryFirstOrDefault<int?>(sql,
                    param: new
                    {
                        nam = chungTu.NamLamViec,
                        loai = chungTu.iLoai,
                        id_phongban = chungTu.Id_PhongBan,
                        id_phongbandich = chungTu.Id_PhongBanDich,
                        id_donvi = chungTu.Id_DonVi,
                        user = chungTu.UserCreator,
                    },
                    commandType: System.Data.CommandType.Text);

            if (max.HasValue)
                result = max.GetValueOrDefault() + 1;

            if (dispose)
                conn.Dispose();

            return result;
        }
        private SKTLockIdViewModel getLockViewModel()
        {
            var lockList = new Dictionary<string, string>
            {
                { "0", "Mở chứng từ"},
                { "1", "Khóa chứng từ" },
            };
            var vm = new SKTLockIdViewModel
            {
                LockList = lockList.ToSelectList("-1", "----Chọn mở hay khóa----")
            };


            return vm;
        }
        private SKTChuyenViewModel getChuyenViewModel()
        {
            var donviList = PhienLamViec.DonViList.ToSelectList("-1", "-- Chọn đơn vị --");
            var vm = new SKTChuyenViewModel
            {
                DonViList = donviList
            };
            return vm;
        }

        private DataTable getTable(
           int nam,
           int id_lock,
           string phongban,
           string iloais)
        {
            return _sKTService.GetChungTu_BLock(nam, id_lock, phongban, iloais);
        }
        
        #endregion



    }
}
