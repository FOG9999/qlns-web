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
    public class ComDataChungTuController : AppController
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
            if (PhienLamViec.iID_MaPhongBan == "11" && _sharedService.CheckLock(PhienLamViec.NamLamViec, "1", "SKT_ComData_ChungTu")) 
            {
                var donvis = PhienLamViec.DonViList;
                var _dvis = new List<KeyViewModel>();
                _dvis.Add(new KeyViewModel(null, "--Tất cả đơn vị--"));
                foreach (var dt in donvis)
                {
                    var nk = new KeyViewModel(dt.Key, dt.Value);
                    _dvis.Add(nk);
                }
                var vm = new ComDataChungTuListViewModel()
                {
                    Items = getChungTuList(loai, orderby)
                    .Where(p => search == null || p.Id_DonVi == search)
                    .OrderBy(p => p.Id_DonVi),
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
                        _sKTService.GetNganhAll(PhienLamViec.NamLamViec, Username, id_phongban_dich)
                        .AsEnumerable().ToDictionary(x => x.Field<string>("MaNganh"), x => x.Field<string>("TenNganh"))
                        .ToSelectList());

            return ToDropdownList(vm);
        }
        public JsonResult Ds_DonVi(string Id_PhongBanDich)
        {
            var data = _ngansachService.GetDonviByPhongBanId(PhienLamViec.iNamLamViec, Id_PhongBanDich)
                .ToDictionary(x => x.iID_MaDonVi, x => x.sMoTa);

            var vm = new ChecklistModel("DonVi", data.ToSelectList());
            return ToDropdownList(vm);
        }
        #endregion

        #region edit

        public ActionResult Edit(Guid id)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var entity = conn.Get<SKT_ComData_ChungTu>(id);
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
                var entity = conn.Get<SKT_ComData_ChungTu>(vm.Id);

                entity.MapFrom(vm);

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
                var entity = conn.Get<SKT_ComData_ChungTu>(id);
                if (entity != null && (entity.UserCreator == Username &&
                !_sKTService.CheckLock(PhienLamViec.iNamLamViec.ToValue<int>(), entity.Id_DonVi, entity.Id_PhongBan, Username)))
                {
                    try
                    {                        
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
        public ActionResult Create(ComDataChungTuEditViewModel vm)
        {
            var isvalid = ModelState.IsValid;

            if (vm.Id_DonVi == "-1")
            {
                ModelState.AddModelError("Id_DonVi", (vm.iLoai == 1) ? "- Chưa chọn đơn vị cần nhập số liệu!" : "Bạn chưa chọn ngành bảo đảm cần nhập số liệu!");
            }           

            if (checkCreate(vm.iLoai, Username, PhienLamViec.iNamLamViec, vm.Id_DonVi, vm.Id_PhongBan))
                ModelState.AddModelError("Id_DonVi", (vm.iLoai == 1) ? "- Đơn vị này đã có chứng từ nhập trước rồi! Đề nghị đ/c điều chỉnh số liệu ở chứng từ cũ!" : "Ngành bảo đảm này đã có chứng từ nhập trước rồi! Đề nghị đ/c điều chỉnh số liệu ở chứng từ cũ!");     

            if (ModelState.Any(x => x.Value.Errors.Count > 0))
            {
                vm.DonViList = PhienLamViec.DonViList.ToSelectList();
                vm.PhongBanList = _ngansachService.GetPhongBansQuanLyNS().AsEnumerable().ToDictionary(x => x.Field<string>("sKyHieu"), x => x.Field<string>("sMoTa")).ToSelectList();
                return View(vm);
            }

            vm.Id = Guid.NewGuid();
            if (vm.iLoai == 1) { 
                var donvi = _ngansachService.GetDonVi(PhienLamViec.iNamLamViec, vm.Id_DonVi);
                vm.TenDonVi = $"{donvi.sMoTa}";
            } else if (vm.iLoai == 2)
            {
                var nganh = _ngansachService.Nganh_Get(PhienLamViec.iNamLamViec, vm.Id_DonVi);
                vm.TenDonVi = $"{nganh.iID_MaNganh} - Ngành {nganh.sTenNganh}";
            }

            var entity = vm.MapTo<SKT_ComData_ChungTu>();
            entity.NamLamViec = int.Parse(PhienLamViec.iNamLamViec);
            entity.Id_PhongBan = vm.Id_PhongBan;
            var phongban = _ngansachService.GetPhongBanById(entity.Id_PhongBan);
            entity.TenPhongBan = $"{phongban.sKyHieu} - {phongban.sMoTa}";   

            entity.NgayChungTu = DateTime.Now;
            entity.DateCreated = DateTime.Now;
            entity.UserCreator = Username;

            using (var conn = _connectionFactory.GetConnection())
            {
                entity.SoChungTu = getSoChungTuLast(conn);
                entity.iLan = getSoLanLast(entity, conn);
                conn.Insert(entity);
            }

            return RedirectToAction("Index", "ComDataChungTuChiTiet", new { vm.Id });
        }

        #endregion

        #region private methods

        private ComDataChungTuEditViewModel createViewModel(int loai = 1, SKT_ComData_ChungTu entity = null)
        {
            if (entity == null)
            {
                entity = new SKT_ComData_ChungTu()
                {
                    NgayChungTu = DateTime.Now.Date,
                    iLoai = loai,
                };
            }
            var vm = entity.MapTo<ComDataChungTuEditViewModel>();

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
                    var phongbanList = _ngansachService.GetPhongBansQuanLyNS("02,05,06,07,08,10,17").AsEnumerable().ToDictionary(x => x.Field<string>("sKyHieu"), x => x.Field<string>("sMoTa")).ToSelectList();
                    vm.PhongBanList = phongbanList;
                    var donviList = PhienLamViec.DonViList.ToSelectList();
                    vm.DonViList = donviList;
                }

            }

            return vm;
        }

        private IEnumerable<ComDataChungTuDetailsViewModel> getChungTuList(int loai, string orderby = null)
        {
            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                var sql = @"
                            select  * 
                            from    SKT_ComData_ChungTu 
                            where   NamLamViec=@iNamLamViec
                                    and iLoai = @loai
                                    and Id_DonVi in (select * from f_split(@donvis))
                            order by @orderby

                            ";
                #region orderby

                orderby = string.IsNullOrWhiteSpace(orderby) ?
                    _orderBys.First().Key :
                    _orderBys.FirstOrDefault(x => x.Value == orderby).Key;
                var donvis = _ngansachService.GetDonvisByUser(Username, PhienLamViec.NamLamViec);

                sql = sql.Replace("@orderby", orderby);

                #endregion

                var items = conn.Query<SKT_ComData_ChungTu>(sql,
                    param: new
                    {
                        PhienLamViec.iNamLamViec,
                        donvis = donvis.ToParamString(),
                        loai,
                    },
                    commandType: System.Data.CommandType.Text);

                return items.Select(x => x.MapTo<ComDataChungTuDetailsViewModel>()).ToList();
            }

        }
        private int getSoChungTuLast(IDbConnection conn = null)
        {
            var result = 1;
            var dispose = conn == null;

            if (dispose)
                conn = _connectionFactory.GetConnection();

            var sql = "select max(SoChungTu) from SKT_ComData_ChungTu";
            var max = conn.QueryFirstOrDefault<int?>(sql);
            if (max.HasValue)
                result = max.GetValueOrDefault() + 1;

            if (dispose)
                conn.Dispose();

            return result;
        }
        private bool checkCreate(int loai, string user, string nam, string id_donvi, string id_phongban)
        {
            var result = 0;
            var conn = _connectionFactory.GetConnection();

            var sql = @"select  max(iLan)
                        from    SKT_ComData_ChungTu 
                        where   NamLamViec = @nam
                                and iLoai = @loai
                                and Id_PhongBan = @id_phongban
                                and Id_DonVi = @id_donvi";


            var max = conn.QueryFirstOrDefault<int?>(sql,
                    param: new
                    {
                        nam = nam,
                        id_phongban = id_phongban,
                        id_donvi = id_donvi,
                        user = user,
                        loai,
                    },
                    commandType: System.Data.CommandType.Text);

            if (max.HasValue)
                result = max.GetValueOrDefault();

            conn.Dispose();
            if (result == 0)
                return false;
            else
                return true;
        }
        private int getSoLanLast(SKT_ComData_ChungTu chungTu, IDbConnection conn = null)
        {
            var result = 1;
            var dispose = conn == null;

            if (dispose)
                conn = _connectionFactory.GetConnection();

            var sql = @"select  max(iLan)
                        from    SKT_ComData_ChungTu 
                        where   NamLamViec = @nam
                                and iLoai = @loai
                                and Id_PhongBan = @id_phongban
                                and Id_DonVi = @id_donvi
                                and UserCreator = @user";


            var max = conn.QueryFirstOrDefault<int?>(sql,
                    param: new
                    {
                        nam = chungTu.NamLamViec,
                        loai = chungTu.iLoai,
                        id_phongban = chungTu.Id_PhongBan,
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
        #endregion



    }
}
