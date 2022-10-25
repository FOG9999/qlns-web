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
using VIETTEL.Areas.SKT.Models;
using VIETTEL.Controllers;
using VIETTEL.Models;

namespace VIETTEL.Areas.SKT.Controllers
{
    public class DacThuChungTuController : AppController
    {
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _ngansachService = NganSachService.Default;
        private readonly ISKTService _sKTService = SKTService.Default;       

        #region public methods

        public ActionResult Index()
        {
            if (_ngansachService.GetUserRoleType(Username) == (int)UserRoleType.TroLyTongHop)
            {
                var vm = new DacThuChungTuListViewModel()
                {
                    Items = getChungTuList(),
                };
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
                        .ToSelectList("-1", "-- Chọn ngành bảo đảm --"));

            return ToDropdownList(vm);
        }

        #endregion

        #region edit

        public ActionResult Edit(Guid id)
        {
            using (var conn = _connectionFactory.GetConnection())
            {
                var entity = conn.Get<SKT_DacThuChungTu>(id);
                if (entity == null)
                    return RedirectToAction("Create");

                var vm = createViewModel(entity: entity);
                return View(vm);
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Edit(DacThuChungTuEditViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            using (var conn = _connectionFactory.GetConnection())
            {
                var entity = conn.Get<SKT_DacThuChungTu>(vm.Id);

                entity.MapFrom(vm);

                entity.DateModified = DateTime.Now;
                entity.UserModifier = Username;
                entity.AuditCount = entity.AuditCount.GetValueOrDefault() + 1;

                conn.Update(entity);
            }

            return RedirectToAction("Index");
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
                var entity = conn.Get<SKT_DacThuChungTu>(id);
                if (entity != null&&(entity.UserCreator == Username &&
                !_sKTService.CheckLock(PhienLamViec.iNamLamViec.ToValue<int>(), entity.Id_Nganh, PhienLamViec.iID_MaPhongBan, Username)))
                {
                    try
                    {                        
                        _sKTService.DeleteChungTuCT(id, "SKT_DacThuChiTiet");
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
        public ActionResult Create()
        {
            var vm = createViewModel();
            return View(vm);
        }

        //[AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DacThuChungTuEditViewModel vm)
        {
            var isvalid = ModelState.IsValid;

            if (vm.Id_Nganh == "-1")
            {
                ModelState.AddModelError("Id_Nganh", "Bạn chưa chọn ngành bảo đảm cần nhập số liệu!");
            }

            if (checkCreate(Username, PhienLamViec.iNamLamViec, vm.Id_Nganh, PhienLamViec.iID_MaPhongBan))
                ModelState.AddModelError("Id_Nganh", "Ngành bảo đảm này đã có chứng từ nhập trước rồi! Đề nghị đ/c điều chỉnh số liệu ở chứng từ cũ!");
            // kiem tra xem b2 da khoa so lieu hay chua
            if (_sKTService.CheckLock(PhienLamViec.iNamLamViec.ToValue<int>(), vm.Id_Nganh, PhienLamViec.iID_MaPhongBan, Username))
            {
                ModelState.AddModelError("lock", $"- Không được nhập số liệu cho {vm.TenNganh} vì đã khóa số liệu của phòng ban hoặc của đ/c.!");
            }

            if (ModelState.Any(x => x.Value.Errors.Count > 0))
            {
                vm.NganhList = _sKTService.GetNganhAll(PhienLamViec.NamLamViec, Username)
                        .AsEnumerable().ToDictionary(x => x.Field<string>("MaNganh"), x => x.Field<string>("TenNganh"))
                        .ToSelectList("-1", "-- Chọn ngành bảo đảm --");
                return View(vm);
            }

            vm.Id = Guid.NewGuid();
            
            var nganh = _ngansachService.Nganh_Get(PhienLamViec.iNamLamViec, vm.Id_Nganh);
            vm.TenNganh = $"{nganh.iID_MaNganh} - Ngành {nganh.sTenNganh}";            

            var entity = vm.MapTo<SKT_DacThuChungTu>();
            entity.NamLamViec = int.Parse(PhienLamViec.iNamLamViec);
            entity.Id_PhongBan = PhienLamViec.iID_MaPhongBan;

            entity.NgayChungTu = DateTime.Now;
            entity.DateCreated = DateTime.Now;
            entity.UserCreator = Username;

            using (var conn = _connectionFactory.GetConnection())
            {
                entity.SoChungTu = getSoChungTuLast(conn);
                entity.iLan = getSoLanLast(entity, conn);
                conn.Insert(entity);
            }

            return RedirectToAction("Index", "DacThuChiTiet", new { vm.Id });
        }

        #endregion

        #region private methods

        private DacThuChungTuEditViewModel createViewModel(SKT_DacThuChungTu entity = null)
        {
            if (entity == null)
            {
                entity = new SKT_DacThuChungTu()
                {
                    NgayChungTu = DateTime.Now.Date,
                };
            }
            var vm = entity.MapTo<DacThuChungTuEditViewModel>();

            // them moi
            if (entity.Id == Guid.Empty)
            {
                vm.NganhList = _sKTService.GetNganhAll(PhienLamViec.NamLamViec, Username)
                        .AsEnumerable().ToDictionary(x => x.Field<string>("MaNganh"), x => x.Field<string>("TenNganh"))
                        .ToSelectList("-1", "-- Chọn ngành bảo đảm --");
            }

            return vm;
        }

        private IEnumerable<DacThuChungTuDetailsViewModel> getChungTuList()
        {
            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                var sql = @"
                            select * from SKT_DacThuChungTu 
                            where   NamLamViec=@iNamLamViec
                                    and Id_PhongBan = @id_Phongban
                                    and (@username is null or UserCreator=@username)
                            order by Id_Nganh

                            ";               

                var items = conn.Query<SKT_DacThuChungTu>(sql,
                    param: new
                    {
                        PhienLamViec.iNamLamViec,
                        id_Phongban = PhienLamViec.iID_MaPhongBan,
                        username = (_ngansachService.GetUserRoleType(Username) == (int)UserRoleType.TroLyTongHop ? string.Empty : Username).ToParamString(),
                    },
                    commandType: System.Data.CommandType.Text);

                return items.Select(x => x.MapTo<DacThuChungTuDetailsViewModel>()).ToList();
            }

        }

        private int getSoChungTuLast(IDbConnection conn = null)
        {
            var result = 1;
            var dispose = conn == null;

            if (dispose)
                conn = _connectionFactory.GetConnection();

            var sql = "select max(SoChungTu) from SKT_DacThuChungTu";
            var max = conn.QueryFirstOrDefault<int?>(sql);
            if (max.HasValue)
                result = max.GetValueOrDefault() + 1;

            if (dispose)
                conn.Dispose();

            return result;
        }
        private bool checkCreate(string user, string nam, string id_nganh, string id_phongban)
        {
            var result = 0;
            var conn = _connectionFactory.GetConnection();

            var sql = @"select  max(iLan)
                        from    SKT_DacThuChungTu 
                        where   NamLamViec = @nam
                                and Id_PhongBan = @id_phongban
                                and Id_Nganh = @id_nganh
                                and UserCreator = @user";


            var max = conn.QueryFirstOrDefault<int?>(sql,
                    param: new
                    {
                        nam = nam,
                        id_phongban = id_phongban,
                        id_nganh = id_nganh,
                        user = user,
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
        private int getSoLanLast(SKT_DacThuChungTu chungTu, IDbConnection conn = null)
        {
            var result = 1;
            var dispose = conn == null;

            if (dispose)
                conn = _connectionFactory.GetConnection();

            var sql = @"select  max(iLan)
                        from    SKT_DacThuChungTu 
                        where   NamLamViec = @nam
                                and Id_Nganh = @id_Nganh
                                and UserCreator = @user";


            var max = conn.QueryFirstOrDefault<int?>(sql,
                    param: new
                    {
                        nam = chungTu.NamLamViec,
                        id_Nganh = chungTu.Id_Nganh,
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
