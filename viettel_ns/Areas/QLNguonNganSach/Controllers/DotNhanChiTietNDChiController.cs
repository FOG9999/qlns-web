using DapperExtensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Services;
using VIETTEL.Areas.QLNguonNganSach.Models;
using VIETTEL.Controllers;
using VIETTEL.Models;

namespace VIETTEL.Areas.QLNguonNganSach.Controllers
{
    public class DotNhanChiTietNDChiController : AppController
    {
        private readonly IQLNguonNganSachService _qLNguonNSService = QLNguonNganSachService.Default;
        // GET: QLNguonNganSach/DotNhanChiTietNDChi
        public ActionResult Index()
        {
            return View();
        }

        #region data grid moi
        public ActionResult ViewDotNhanChiTietNDChi(Guid iID_DotNhan, string sNoiDung, string sMaLoaiDuToan, bool isClone, double fSoTienNhanTuBTC, double SoTienDaPhanNDC, double fSoTienConLai, Guid iID_DotNhanChiTiet)
        {
            var vm = new DotNhanChiTietNDChiViewModel
            {
                iID_DotNhan = iID_DotNhan,
                iID_DotNhanChiTiet = iID_DotNhanChiTiet,
                sNoiDung = sNoiDung,
                sMaLoaiDuToan = sMaLoaiDuToan,
                isClone = isClone,
                fSoTienNhanTuBTC = fSoTienNhanTuBTC,
                SoTienDaPhanNDC = SoTienDaPhanNDC,
                fSoTienConLai = fSoTienConLai
            };

            TempData["sMaLoaiDuToan"] = sMaLoaiDuToan;
            return View(vm);
        }

        public ActionResult SheetFrame(string id, string filters = null)
        {
            var filtersJson = filters == null ? Request.QueryString.ToDictionary() : JsonConvert.DeserializeObject<Dictionary<string, string>>(filters);

            var sMaLoaiDuToan = TempData["sMaLoaiDuToan"];
            var sheet = new QLDotNhanChiTiet_NDChi_SheetTable(Guid.Parse(id), sMaLoaiDuToan.ToString(), Username, filtersJson);
            var vm = new DotNhanChiTietNDChiViewModel
            {
                Sheet = new SheetViewModel(
                    bang: sheet,
                    id: id,
                    filters: sheet.Filters,
                    urlPost: Url.Action("Save", "DotNhanChiTietNDChi", new { area = "QLNguonNganSach" }),
                    urlGet: Url.Action("SheetFrame", "DotNhanChiTietNDChi", new { area = "QLNguonNganSach" })
                    ),
            };
            vm.Sheet.AvaiableKeys = new Dictionary<string, string>();
            TempData["sMaLoaiDuToan"] = sMaLoaiDuToan;
            return View("_sheetFrame", vm);
        }

        [HttpPost]
        public ActionResult Save(SheetEditViewModel vm)
        {
            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                int iNamLamViec = int.Parse(PhienLamViec.iNamLamViec);

                conn.Open();
                var rows = vm.Rows.Where(x => !x.IsParent).ToList();
                if (rows.Count > 0)
                {
                    rows.ForEach(r =>
                    {
                        var trans = conn.BeginTransaction();
                        var valuesId = r.Id.ToList("_", true);
                        Guid iID_NoiDungChi = Guid.Parse(valuesId[0]);
                        Guid iID_DotNhanChiTiet = Guid.Parse(valuesId[1]);
                        var nnsDotNhanChiTietNDChi = _qLNguonNSService.GetDotNhanChiTietNDChi(iID_NoiDungChi, iID_DotNhanChiTiet);
                        if (nnsDotNhanChiTietNDChi != null)
                        {
                            var entity = conn.Get<NNS_DotNhanChiTiet_NDChi>(nnsDotNhanChiTietNDChi.iID_DotNhanChiTiet_NDChi, trans);
                            if (r.Columns.ContainsKey("GhiChu"))
                            {
                                entity.GhiChu = r.Columns["GhiChu"];
                            }
                            if (r.Columns.ContainsKey("SoTien"))
                            {
                                entity.SoTien = Decimal.Parse(r.Columns["SoTien"]);
                            }
                            if(entity.SoTien == 0)
                            {
                                conn.Delete(entity, trans);
                            } else
                            {
                                entity.iNamLamViec = int.Parse(PhienLamViec.iNamLamViec);
                                entity.sID_MaNguoiDungSua = Username;
                                entity.dNgaySua = DateTime.Now;
                                entity.sIPSua = Request.UserHostAddress;
                                conn.Update(entity, trans);
                            }
                        }
                        else
                        {
                            var newEntity = new NNS_DotNhanChiTiet_NDChi();
                            var columns = new QLDotNhanChiTiet_NDChi_SheetTable().Columns;
                            string GhiChu = "";
                            decimal SoTien = 0;
                            if (r.Columns.ContainsKey("GhiChu"))
                            {
                                GhiChu = r.Columns["GhiChu"];
                            }
                            
                            if (r.Columns.ContainsKey("SoTien"))
                            {
                                SoTien = Decimal.Parse(r.Columns["SoTien"]);
                            }
                            newEntity.SoTien = SoTien;
                            newEntity.GhiChu = GhiChu;
                            newEntity.iID_NoiDungChi = iID_NoiDungChi;
                            newEntity.iID_DotNhanChiTiet = iID_DotNhanChiTiet;
                            newEntity.iNamLamViec = int.Parse(PhienLamViec.iNamLamViec);
                            newEntity.sID_MaNguoiDungTao = Username;
                            newEntity.dNgayTao = DateTime.Now;

                            conn.Insert(newEntity, trans);
                        }

                        // commit to db
                        trans.Commit();
                    });
                }
            }

            var sMaLoaiDuToan = TempData["sMaLoaiDuToan"];
            TempData["sMaLoaiDuToan"] = sMaLoaiDuToan;
            return RedirectToAction("SheetFrame", new { vm.Id, vm.Option, filter = vm.FiltersString });
        }
        #endregion

        [HttpPost]
        public JsonResult createDNChiTiet(Guid iID_DotNhan, Guid iID_Nguon)
        {
            //check exist
            var entityDNCT = _qLNguonNSService.GetDotNhanChiTietByIdDotNhanIdNguon(iID_DotNhan, iID_Nguon, PhienLamViec.NamLamViec);
            if(entityDNCT == null)
            {
                entityDNCT = new NNS_DotNhanChiTiet();
                using (var conn = ConnectionFactory.Default.GetConnection())
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();

                    #region Them moi NNS_DotNhanChiTiet
                    entityDNCT.iID_DotNhanChiTiet = Guid.NewGuid();
                    entityDNCT.iID_DotNhan = iID_DotNhan;
                    entityDNCT.iID_Nguon = iID_Nguon;
                    entityDNCT.sID_MaNguoiDungTao = Username;
                    entityDNCT.dNgayTao = DateTime.Now;
                    entityDNCT.iNamLamViec = int.Parse(PhienLamViec.iNamLamViec);
                    conn.Insert(entityDNCT, trans);
                    #endregion

                    // commit to db
                    trans.Commit();
                }
            }

            return Json(new { data = entityDNCT.iID_DotNhanChiTiet, bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetSoKiemTra()
        {
            IEnumerable<SoKiemTraDotNhanChiTietNDChiModel> soKiemTraDnctNDC = _qLNguonNSService.GetSoKiemTraDnctNDC(int.Parse(PhienLamViec.iNamLamViec));
            return Json(new { data = soKiemTraDnctNDC, bIsComplete = true }, JsonRequestBehavior.AllowGet);
        }
    }
}