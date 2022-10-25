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
using Viettel.Services;
using VIETTEL.Areas.DuToanKT.Models;
using VIETTEL.Controllers;
using VIETTEL.Models;

namespace VIETTEL.Areas.DuToanKT.Controllers
{
    public class ChungTuController : AppController
    {
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _ngansachService = NganSachService.Default;
        private readonly IQuyetToanService _quyetToanService = QuyetToanService.Default;
        private readonly IDuToanKTService _duToanKTService = DuToanKTService.Default;

        private readonly IEnumerable<KeyViewModel> _orderBys = new List<KeyViewModel>
        {
            new KeyViewModel("donvi", "Id_DonVi", "NS.DonVi"),
            new KeyViewModel("ngay_desc", "NgayChungTu desc", "NS.NgayChungTu.Desc"),
            new KeyViewModel("ngay_asc", "NgayChungTu asc", "NS.NgayChungTu.Asc"),
            new KeyViewModel("sochungtu", "SoChungTu", "NS.SoChungTu"),
        };


        #region public methods

        public ActionResult Index(int loai = 1, string ireq = null, string orderby = null)
        {
            var vm = new ChungTuListViewModel()
            {
                Items = getChungTuList(loai, ireq, orderby),
                OrderBys = _orderBys,
                Loai = loai,
            };

            ViewBag.Loai = loai;
            return View(vm);
        }

        public JsonResult Ds_Nganh(string id_phongban_dich = null)
        {
            ChecklistModel vm;

            if (id_phongban_dich == null || id_phongban_dich == "undefined")
            {
                vm = new ChecklistModel("Id_DonVi", _ngansachService.Nganh_GetAll(Username, PhienLamViec.PhongBan.sKyHieu).ToSelectList("iID_MaNganh", "sTenNganh", "-1", "-- Chọn ngành --"));
            }
            else
            {
                if (PhienLamViec.PhongBan.sKyHieu == "02" || PhienLamViec.PhongBan.sKyHieu == "11")
                    vm = new ChecklistModel("Id_DonVi",
                        _duToanKTService.GetNganhBD(PhienLamViec.iNamLamViec, id_phongban_dich, id_phongban_dich)
                        .ToSelectList("MaNganh", "TenNganh"));
                else
                    vm = new ChecklistModel("Id_DonVi",
                        _duToanKTService.GetNganhBD(PhienLamViec.iNamLamViec, PhienLamViec.PhongBan.sKyHieu, PhienLamViec.PhongBan.sKyHieu)
                        .ToSelectList("MaNganh", "TenNganh"));
            }
            return ToDropdownList(vm);
        }

        #endregion

        #region edit

        public ActionResult Edit(Guid id)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var entity = conn.Get<DTKT_ChungTu>(id);
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
                var entity = conn.Get<DTKT_ChungTu>(vm.Id);

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
                var entity = conn.Get<DTKT_ChungTu>(id);
                if (entity.UserCreator == Username ||
                !_duToanKTService.CheckLock(PhienLamViec.iNamLamViec.ToValue<int>(), entity.Id_DonVi, entity.Id_PhongBan, Username, entity.iRequest.ToString()))
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();
                    try
                    {

                        entity = conn.Get<DTKT_ChungTu>(id, trans);
                        entity.iTrangThai = 0;
                        entity.DateModified = DateTime.Now;
                        entity.UserCreator = Username;
                        conn.Update(entity, trans);

                        // delete chung tu chi tiet
                        var sql = @"update DTKT_ChungTuChiTiet set iTrangThai=0, DateModified=@DateModified, UserCreator=@UserCreator where Id_ChungTu=@Id_ChungTu";
                        using (var cmd = new SqlCommand(sql, conn, trans))
                        {
                            cmd.AddParams(new
                            {
                                DateModified = DateTime.Now,
                                UserCreator = Username,
                                Id_ChungTu = id,
                            });

                            cmd.ExecuteNonQuery();
                            trans.Commit();
                        }

                        success = true;
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            trans.Rollback();
                        }
                        catch
                        {

                        }

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

        //[AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ChungTuEditViewModel vm)
        {
            var isvalid = ModelState.IsValid;

            if (vm.Id_DonVi == "-1")
            {
                ModelState.AddModelError("Id_DonVi", vm.iLoai == 1 ? "- Chưa chọn đơn vị cần nhập số liệu!" : "Bạn chưa chọn ngành bảo đảm cần nhập số liệu!");
            }

            if (vm.iRequest == -1)
            {
                ModelState.AddModelError("iRequest", "- Chưa chọn phòng ban đề nghị!");
            }

            // kiem tra xem b2 da khoa so lieu hay chua
            if (_duToanKTService.CheckLock(PhienLamViec.iNamLamViec.ToValue<int>(), vm.Id_DonVi, PhienLamViec.iID_MaPhongBan, Username, vm.iRequest.ToString()))
            {
                ModelState.AddModelError("lock", $"- Không được nhập số liệu cho {vm.TenDonVi} vì đã khóa số liệu của phòng ban hoặc của đ/c.!");
            }

            if (ModelState.Any(x => x.Value.Errors.Count > 0))
            {
                vm.DonViList = PhienLamViec.DonViList.ToSelectList("-1", "-- Chọn đơn vị --");
                vm.RequestList = _duToanKTService.GetRequestList().ToSelectList();

                return View(vm);
            }

            vm.Id = Guid.NewGuid();
            //vm.TenDonVi = PhienLamViec.DonViList.First(x => x.Key == vm.Id_DonVi).Value;
            var donvi = _ngansachService.GetDonVi(PhienLamViec.iNamLamViec, vm.Id_DonVi);
            vm.TenDonVi = $"{donvi.iID_MaDonVi} - {donvi.sMoTa}";

            vm.Request = _duToanKTService.GetRequestList()[vm.iRequest.ToString()];
            //vm.TenDonVi = vm.Id_DonVi;

            var entity = vm.MapTo<DTKT_ChungTu>();
            entity.iID_MaNamNganSach = PhienLamViec.iID_MaNamNganSach;
            entity.iID_MaNguonNganSach = PhienLamViec.iID_MaNguonNganSach;
            entity.NamLamViec = int.Parse(PhienLamViec.iNamLamViec);
            entity.Id_PhongBan = PhienLamViec.iID_MaPhongBan;
            if (PhienLamViec.iID_MaPhongBan == "02" || PhienLamViec.iID_MaPhongBan == "11")
            {
                entity.Id_PhongBanDich = vm.Id_PhongBanDich;
            }
            else
            {
                entity.Id_PhongBanDich = PhienLamViec.iID_MaPhongBan;
            }
            entity.iLoai = vm.iLoai;
            entity.iTrangThai = 1;

            entity.DateCreated = DateTime.Now;
            entity.UserCreator = Username;

            using (var conn = _connectionFactory.GetConnection())
            {
                entity.SoChungTu = getSoChungTuLast(conn);
                entity.iLan = getSoLanLast(entity, conn);
                conn.Insert(entity);
            }

            return RedirectToAction("Index", "ChungTuChiTiet", new { vm.Id });
        }

        #endregion

        #region private methods

        private ChungTuEditViewModel createViewModel(int loai = 1, DTKT_ChungTu entity = null)
        {
            if (entity == null)
            {
                entity = new DTKT_ChungTu()
                {
                    NgayChungTu = DateTime.Now.Date,
                    iLoai = loai,
                };
            }
            var vm = entity.MapTo<ChungTuEditViewModel>();


            // them moi
            if (entity.Id == Guid.Empty)
            {
                var requestList = _duToanKTService.GetRequestList().ToSelectList();
                vm.RequestList = requestList;

                if (PhienLamViec.PhongBan.sKyHieu != "02" || PhienLamViec.PhongBan.sKyHieu != "11")
                {
                    var donviList = PhienLamViec.DonViList.ToSelectList("-1", "-- Chọn đơn vị --");
                    vm.DonViList = donviList;
                }
                else
                {
                    vm.PhongBanList = _ngansachService.GetPhongBanQuanLyNS().ToSelectList("iID_MaPhongBan", "sMoTa", "-1", "-- Chọn phòng ban --");
                }

            }

            return vm;
        }

        private IEnumerable<ChungTuDetailsViewModel> getChungTuList(int loai, string ireq = null, string orderby = null)
        {
            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                var sql = @"

select * from DTKT_ChungTu 
where   iTrangThai=1
        and NamLamViec=@iNamLamViec
        and Id_PhongBan=@Id_PhongBan
        and iLoai=@loai
        and (@username is null or UserCreator=@username)
        and (@ireq is null or iRequest = @ireq)
order by @orderby

";
                #region orderby

                orderby = string.IsNullOrWhiteSpace(orderby) ?
                    _orderBys.First().Key :
                    _orderBys.FirstOrDefault(x => x.Value == orderby).Key;

                sql = sql.Replace("@orderby", orderby);

                #endregion

                var items = conn.Query<DTKT_ChungTu>(sql,
                    param: new
                    {
                        PhienLamViec.iNamLamViec,
                        loai,
                        ireq = ireq.ToParamString(),
                        Id_PhongBan = PhienLamViec.iID_MaPhongBan,
                        username = (_ngansachService.GetUserRoleType(Username) == (int)UserRoleType.TroLyTongHop ? string.Empty : Username).ToParamString(),
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

            var sql = "select max(SoChungTu) from DTKT_ChungTu";
            var max = conn.QueryFirstOrDefault<int?>(sql);
            if (max.HasValue)
                result = max.GetValueOrDefault() + 1;

            if (dispose)
                conn.Dispose();

            return result;
        }

        private int getSoLanLast(DTKT_ChungTu chungTu, IDbConnection conn = null)
        {
            var result = 1;
            var dispose = conn == null;

            if (dispose)
                conn = _connectionFactory.GetConnection();

            var sql = @"select  max(iLan)
                        from    DTKT_ChungTu 
                        where   iTrangThai = 1
                                and NamLamViec = @nam
                                and iLoai = @loai
                                and iRequest = @ireq
                                and Id_PhongBan = @id_phongban
                                and Id_DonVi = @id_donvi
                                and UserCreator = @user";


            var max = conn.QueryFirstOrDefault<int?>(sql,
                    param: new
                    {
                        nam = chungTu.NamLamViec,
                        loai = chungTu.iLoai,
                        ireq = chungTu.iRequest,
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

        public JsonResult Ds_DonVi(string Id_PhongBanDich)
        {
            var data = _duToanKTService.Get_DonVi_TheoBql_ExistData(
                namLamViec: PhienLamViec.iNamLamViec,
                iID_MaPhongBan: Id_PhongBanDich,
                request: 0);

            var vm = new ChecklistModel("DonVi", data.ToSelectList("Id", "Ten", "-1", "-- Chọn đơn vị --"));
            return ToDropdownList(vm);
        }

        #endregion

    }
}
