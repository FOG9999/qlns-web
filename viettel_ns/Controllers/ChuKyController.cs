using AutoMapper.Extensions;
using Dapper;
using DapperExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Viettel.Data;
using Viettel.Domain.DomainModel;
using Viettel.Services;
using VIETTEL.Helpers;
using VIETTEL.Models;

namespace VIETTEL.Controllers
{
    public class ChuKyCopyViewModel
    {
        public int PhanHe { get; set; }

        [Required]
        public string UsernameCopy { get; set; }
    }
    public class ChuKyController : AppController
    {
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;

        //public ChuKyController(IConnectionFactory connectionFactory)
        //{
        //    _connectionFactory = connectionFactory;
        //}

        // GET: ChuKy
        public ActionResult Sheet()
        {
            var vm = new SheetViewModel()
            {
                Table = new ChuKySheetTable(),
            };
            return View(vm);
        }

        #region edit


        public ActionResult Edit(string c, string tren = "", string duoi = "1,2,3,4,5", int popup = 0)
        {
            if ((string)TempData["isClosed"] == "1")
            {
                ViewBag.isClosed = 1;
            }
            var entity = getChuKyByController(c, Username);
            if (entity == null)
            {
                entity = new NS_DanhMuc_BaoCao_ChuKy()
                {
                    sController = c,
                    sTenBaoCao = c,
                };
            }
            var vm = entity.MapTo<ChuKyViewModel>();

            setViewBag(popup);
            return View(vm);
        }

        [HttpPost]
        public ActionResult Edit(ChuKyViewModel vm, int popup = 0)
        {
            if (!ModelState.IsValid)
            {
                setViewBag();
                return View(vm);
            }

            #region create or update chu ky
            NS_DanhMuc_BaoCao_ChuKy entity = null;
            var isNew = vm.iID_MaBaoCao_ChuKy == 0;
            if (isNew)
            {
                entity = vm.MapTo<NS_DanhMuc_BaoCao_ChuKy>();
                entity.sID_MaNguoiDungTao = Username;
                entity.iTrangThai = 1;
                entity.dNgayTao = DateTime.Now;
            }
            else
            {
                entity = getChuKyByController(vm.sController, Username);
                entity.sID_MaNguoiDungSua = Username;
                entity.dNgaySua = DateTime.Now;

                entity.MapFrom(vm);
            }


            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                if (isNew)
                {
                    conn.Insert(entity);
                }
                else
                {
                    conn.Update(entity);
                }
            }
            #endregion

            setViewBag(popup);
            TempData["isClosed"] = "1";
            return RedirectToAction("Edit", new { c = vm.sController, popup });
        }

        private void setViewBag(int popup = 0)
        {
            ViewBag.ChuKyList = getChuKyAll().ToSelectList("iID_MaChuKy", "sMoTa", "-1", "");
            ViewBag.PhanHeList = getAll_PhanHe().ToSelectList("iID_MaPhanHe", "sMoTa", "-1", "<-- Tất cả -->");
            ViewBag.IsPopup = popup;
        }


        #endregion

        #region delete

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var success = false;
            var msg = string.Empty;


            using (var conn = _connectionFactory.GetConnection())
            {
                var sql = "delete NS_DanhMuc_BaoCao_ChuKy where iID_MaBaoCao_ChuKy=@id";

                conn.Open();
                var trans = conn.BeginTransaction();
                try
                {
                    //var entity = conn.QueryFirstOrDefault<NS_DanhMuc_BaoCao_ChuKy>(sql, new { id }, trans);
                    //entity.iTrangThai = 0;
                    //conn.Update(entity, trans);
                    //conn.Delete(sql, new { id }, trans);

                    conn.Execute(sql, new { id }, trans);
                    trans.Commit();

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
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePhanHe(int id)
        {
            var success = false;
            var msg = string.Empty;

            using (var conn = _connectionFactory.GetConnection())
            {
                var sql = "delete NS_DanhMuc_BaoCao_ChuKy where iID_MaPhanHe=@id and sID_MaNguoiDungTao=@username";

                //                var sql = @"
                //UPDATE  NS_DanhMuc_BaoCao_ChuKy
                //SET iTrangThai=0, iSoLanSua = iSoLanSua+1
                //where iID_MaPhanHe=@id and sID_MaNguoiDungTao=@username";


                conn.Open();
                var trans = conn.BeginTransaction();
                try
                {
                    conn.Execute(sql, new { id, Username }, trans);
                    trans.Commit();
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
        }


        #endregion

        #region chu ky table

        public ActionResult SheetFrame(string filters = null)
        {

            var sheet = string.IsNullOrWhiteSpace(filters) ?
                    new ChuKySheetTable(Request.QueryString) :
                    new ChuKySheetTable(Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(filters));

            sheet.FillSheet(getChuKyAll(sheet.Filters));

            var vm = new SheetViewModel(
                    bang: sheet,
                    filters: sheet.Filters,
                    //urlPost: isReadonly ? "" : Url.Action("Save", "ChungTuChiTiet", new { area = "DuToanKT" }),
                    urlPost: Url.Action("Save", this.ControllerName(), new { area = "" }),
                    urlGet: Url.Action("SheetFrame", this.ControllerName(), new { area = "" }));

            return View("_sheetFrame", vm);
        }


        [HttpPost]
        public ActionResult Save(SheetEditViewModel vm)
        {
            var rows = vm.Rows.Where(x => !x.IsParent).ToList();
            if (rows.Count > 0)
            {
                var columns = new ChuKySheetTable().Columns.Where(x => !x.IsReadonly);

                using (var conn = ConnectionFactory.Default.GetConnection())
                {
                    rows.ForEach(r =>
                    {
                        var id = r.Id.ToValue<int>();

                        if (r.IsDeleted && id > 0)
                        {
                            #region delete

                            var entity = conn.Get<NS_DanhMucChuKy>(id);
                            if (entity != null)
                            {
                                //entity.iTrangThai = 0;
                                //entity.dNgaySua = DateTime.Now;
                                //entity.sID_MaNguoiDungSua = Username;

                                //conn.Update(entity);

                                conn.Delete(entity);
                            }

                            #endregion
                        }
                        else
                        {
                            var changes = r.Columns.Where(c => columns.Any(x => x.ColumnName == c.Key));
                            var isNew = id == 0;
                            if (isNew)
                            {
                                #region create

                                var entity = new NS_DanhMucChuKy()
                                {
                                    iTrangThai = 1,
                                    dNgayTao = DateTime.Now,
                                    sID_MaNguoiDungTao = Username,
                                };

                                entity.MapFrom(changes);

                                conn.Insert(entity);

                                #endregion
                            }
                            else
                            {
                                #region edit

                                var entity = conn.Get<NS_DanhMucChuKy>(id);
                                entity.MapFrom(changes);

                                entity.dNgaySua = DateTime.Now;
                                entity.sID_MaNguoiDungSua = Username;

                                conn.Update(entity);

                                #endregion
                            }
                        }

                    });
                }
            }

            return RedirectToAction("SheetFrame", new { filters = vm.FiltersString });
        }
        #endregion

        #region Danh sach chu ky

        public ActionResult Index(int? phanhe)
        {
            var vm = getAll_ChuKy(phanhe);
            ViewBag.PhanHeList = getAll_PhanHe().ToSelectList("iID_MaPhanHe", "sMoTa", "-1", "<-- Tất cả -->");
            return View(vm);
        }

        #endregion

        #region Copy

        public ActionResult Copy(int phanhe = -1)
        {
            var vm = new ChuKyCopyViewModel()
            {
                PhanHe = phanhe,
            };
            ViewBag.PhanHeList = getAll_PhanHe().ToSelectList("iID_MaPhanHe", "sMoTa", "-1", "<-- Tất cả -->");
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Copy(ChuKyCopyViewModel vm)
        {
            var username = vm.UsernameCopy;
            if (!ModelState.IsValid)
            {
                ViewBag.PhanHeList = getAll_PhanHe().ToSelectList("iID_MaPhanHe", "sMoTa", "-1", "<-- Tất cả -->");
                return View(vm);
            }

            #region clone chuky

            var items = getAll_ChuKy(vm.PhanHe, vm.UsernameCopy);
            if (items == null || items.Count() == 0)
            {
                ViewBag.PhanHeList = getAll_PhanHe().ToSelectList("iID_MaPhanHe", "sMoTa", "-1", "<-- Tất cả -->");
                return View(vm);
            }
            using (var conn = _connectionFactory.GetConnection())
            {
                items.ToList()
               .ForEach(item =>
               {
                   //var entity = conn.Get<NS_DanhMuc_BaoCao_ChuKy>(item.iID_MaBaoCao_ChuKy);
                   var entity = getChuKyByController(item.sController, Username);
                   var exist = false;
                   if (entity == null)
                   {
                       entity = new NS_DanhMuc_BaoCao_ChuKy();
                   }
                   else
                   {
                       exist = true;
                   }


                   // clone values
                   entity.MapFrom(item, "iID_MaBaoCao_ChuKy");
                   entity.sID_MaNguoiDungTao = Username;
                   entity.sID_MaNguoiDungSua = Username;
                   entity.dNgaySua = DateTime.Now;
                   if (exist)
                   {
                       conn.Update(entity);
                   }
                   else
                   {
                       entity.dNgayTao = DateTime.Now;
                       conn.Insert(entity);
                   }

               });

            }


            #endregion

            return RedirectToAction("Index", new { vm.PhanHe });
        }

        #endregion

        #region data

        private DataTable getChuKyAll()
        {
            var sql = FileHelpers.GetSqlQuery("ds_chuky.sql");

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                return conn.GetTable(sql);
            }
        }

        private DataTable getChuKyAll(Dictionary<string, string> filters)
        {
            var sql = FileHelpers.GetSqlQuery("ds_chuky_sheet.sql");

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                return cmd.AddParams(filters)
                             .GetTable();
            }
        }

        private NS_DanhMuc_BaoCao_ChuKy getChuKyByController(string controller, string username)
        {
            var sql = @"
select * from NS_DanhMuc_BaoCao_ChuKy
where   iTrangThai=1
        and sController=@controller
        and sID_MaNguoiDungTao=@username

";
            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                return conn.QueryFirstOrDefault<NS_DanhMuc_BaoCao_ChuKy>(sql, new { controller, username });
            }
        }

        private DataTable getAll_PhanHe()
        {
            var sql = @"

select iID_MaPhanHe, CONVERT(nvarchar,iID_MaPhanHe) + ' - ' + sTen as sMoTa from NS_PhanHe
where   iTrangThai=1
order by iID_MaPhanHe

";

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                var dt = conn.GetTable(sql);
                return dt;
            }
        }

        public IEnumerable<NS_DanhMuc_BaoCao_ChuKy> getAll_ChuKy(int? phanhe, string username = null)
        {
            var sql = @"

select * from NS_DanhMuc_BaoCao_ChuKy
where   iTrangThai=1 
        and sID_MaNguoiDungTao=@username
        and (@iID_MaPhanHe is null or iID_MaPhanHe=@iID_MaPhanHe)
order by iID_MaPhanHe, sController

";

            var iID_MaPhanHe = string.Empty;
            if (phanhe.HasValue && phanhe > 0)
            {
                iID_MaPhanHe = phanhe.ToString();
            }

            username = username ?? Username;

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                var query = conn.Query<NS_DanhMuc_BaoCao_ChuKy>(
                    sql,
                    new
                    {
                        username,
                        iID_MaPhanHe = iID_MaPhanHe.ToParamString(),
                    });

                return query;
            }

        }

        #endregion
    }
}
