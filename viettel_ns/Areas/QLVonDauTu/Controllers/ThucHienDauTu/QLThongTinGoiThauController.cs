using DapperExtensions;
using FlexCel.Core;
using FlexCel.Render;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLVonDauTu;
using Viettel.Services;
using VIETTEL.Areas.QLVonDauTu.Model.NganSachQuocPhong;
using VIETTEL.Areas.z.Models;
using VIETTEL.Common;
using VIETTEL.Controllers;
using VIETTEL.Helpers;
using static VTS.QLNS.CTC.App.Service.UserFunction.FormatNumber;

namespace VIETTEL.Areas.QLVonDauTu.Controllers.ThucHienDauTu
{
    public class QLThongTinGoiThauController : FlexcelReportController
    {
        private readonly IQLVonDauTuService _vonDauTuService = QLVonDauTuService.Default;
        INganSachService _iNganSachService = NganSachService.Default;
        // GET: QLVonDauTu/QLThongTinGoiThau
        public ActionResult Index()
        {
            IEnumerable<ThongTinGoiThauViewModel> listData = _vonDauTuService.GetAllThongTinGoiThau();
            return View(listData);
        }

        [HttpPost]
        public ActionResult GoiThauListView(string tenDuAn, string tenGoiThau, int giaTriMin = 0, int giaTriMax = 0)
        {
            IEnumerable<ThongTinGoiThauViewModel> lstData = _vonDauTuService.GetAllThongTinGoiThau(tenDuAn, tenGoiThau, giaTriMin, giaTriMax);

            return PartialView("_list", lstData);
        }

        public ActionResult Update(Guid id)
        {
            ViewBag.ListNhaThau = _vonDauTuService.GetAllNhaThau().ToSelectList("iID_NhaThauID", "sTenNhaThau");
            ThongTinGoiThauViewModel objDotnhan = _vonDauTuService.GetThongTinGoiThau(id);
            ViewBag.ChiTietDuAn = _vonDauTuService.GetThongTinDuAnByDuAnId(objDotnhan.iID_DuAnID.Value);
            objDotnhan.dNgayQuyetDinh = DateTime.Now;
            ViewBag.dBatDauNhaThau = objDotnhan.dBatDauChonNhaThau.HasValue ? objDotnhan.dBatDauChonNhaThau.Value.ToString("dd/MM/yyyy"): string.Empty;
            ViewBag.dKetThucNhaThau = objDotnhan.dKetThucChonNhaThau.HasValue ? objDotnhan.dKetThucChonNhaThau.Value.ToString("dd/MM/yyyy") : string.Empty;

            //ViewBag.ListChiPhi = _vonDauTuService.LayChiPhi().ToSelectList("iID_ChiPhi", "sTenChiPhi");
            //ViewBag.ListNguonVon = _vonDauTuService.LayNguonVon().ToSelectList("iID_MaNguonNganSach", "sTen");

            //List<VDT_DA_TT_HopDong> lstHangMuc = _vonDauTuService.GetListHTHopDong(objDotnhan.iID_DuAnID.Value).ToList();
            //lstHangMuc.Insert(0, new VDT_DA_TT_HopDong { iID_LoaiHopDongID = Guid.Empty, sHinhThucHopDong = "--Chọn--" });
            //ViewBag.listHTHopDong = lstHangMuc.ToSelectList("iID_DuAnID", "sHinhThucHopDong");
            return View(objDotnhan);
        }
        
        public ActionResult Import()
        {
            return View();
        }

        public ActionResult Add()
        {
            ThongTinGoiThauViewModel objDotnhan = new ThongTinGoiThauViewModel();
            var listDonvi = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
            ViewBag.ListDonViQuanLy = listDonvi.ToSelectList("iID_Ma", "sTen");
            ViewBag.ListNhaThau = _vonDauTuService.GetAllNhaThau().ToSelectList("iID_NhaThauID", "sTenNhaThau");
            return View(objDotnhan);
        }

        public JsonResult LayThongTinChiTietDuAn(string iID)
        {
            var d_DuAn = _vonDauTuService.LayThongTinChiTietDuAn(Guid.Parse(iID));
            var d_DauTu = _vonDauTuService.LayThongTinQDDauTu(Guid.Parse(iID));
            return Json(new { d_DuAn = d_DuAn, d_DauTu = d_DauTu }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Save(ThongTinGoiThauViewModel model)
        {

            var sMessage = string.Format(Constants.THEM_BAN_GHI, model.sSoQuyetDinh);
            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                if (model.iID_GoiThauID != null || model.iID_GoiThauID != Guid.Empty)
                {
                    sMessage = sMessage.Replace("Thêm mới", "Cập nhật");

                } 

                try
                {
                    conn.Open();
                    var trans = conn.BeginTransaction();
                    var entity = new VDT_DA_GoiThau();
                    if (model.goiThau.iID_GoiThauID != Guid.Empty)
                    {
                        #region Sua
                        entity = conn.Get<VDT_DA_GoiThau>(model.goiThau.iID_GoiThauID, trans);
                        entity.sSoQuyetDinh = model.goiThau.sSoQuyetDinh;
                        entity.dNgayQuyetDinh = model.goiThau.dNgayQuyetDinh;
                        entity.sTenGoiThau = model.goiThau.sTenGoiThau;
                        entity.sHinhThucChonNhaThau = model.goiThau.sHinhThucChonNhaThau;
                        entity.sPhuongThucDauThau = model.goiThau.sPhuongThucDauThau;
                        entity.sHinhThucHopDong = model.goiThau.sHinhThucHopDong;
                        //entity.iThoiGianThucHien = model.goiThau.iThoiGianThucHien;
                        entity.dBatDauChonNhaThau = model.goiThau.dBatDauChonNhaThau;
                        entity.dKetThucChonNhaThau = model.goiThau.dKetThucChonNhaThau;
                        entity.iID_NhaThauID = model.goiThau.iID_NhaThauID;
                        entity.dDateUpdate = DateTime.Now;
                        entity.sUserUpdate = Username;
                        conn.Update(entity, trans);
                        #endregion

                    }

                    // commit to db
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    return Json(new { status = false, sMessage = sMessage }, JsonRequestBehavior.AllowGet);

                }

            }
            return Json(new { status = true,sMessage = sMessage }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(Guid id)
        {
            var entity = _vonDauTuService.GetThongTinGoiThauById(id);
            var sMessage = Constants.XOA_BAN_GHI;
            if (entity != null)
            {
                sMessage = string.Format(sMessage, entity.sSoQuyetDinh);
            }
            bool status = _vonDauTuService.DeleteGoiThau(id, Username);
            return Json(new { bIsComplete = status, sMessage = sMessage }, JsonRequestBehavior.AllowGet);
             
        }
        public ActionResult DieuChinh(Guid id)
        {
            ThongTinGoiThauViewModel objGoiThau = _vonDauTuService.GetThongTinGoiThau(id);
            ViewBag.ListChiPhi = _vonDauTuService.LayChiPhi().ToSelectList("iID_ChiPhi", "sTenChiPhi");
            ViewBag.ListNguonVon = _vonDauTuService.LayNguonVon().ToSelectList("iID_MaNguonNganSach", "sTen");
            ViewBag.ListHangMucDauTu = _vonDauTuService.ListHangMucByDuAn(objGoiThau.iID_DuAnID.Value,objGoiThau.dNgayQuyetDinh.Value).ToSelectList("iID_DuAn_HangMucID", "sTenHangMuc");
            ViewBag.ThongTinDuAn = _vonDauTuService.GetThongTinDuAnByGoiThauId(id);

            return View(objGoiThau);
        }

        [HttpPost]
        public JsonResult AddGoiThauDieuChinh(ThongTinGoiThauViewModel model,string iID_GoiThauID)
        {
            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();
                var entity = new VDT_DA_GoiThau();
                if (model.goiThau.iID_GoiThauID == Guid.Empty)
                {
                    #region add goi thau bo sung
                    entity.MapFrom(model.goiThau);
                    entity.iID_ParentID = Guid.Parse(iID_GoiThauID);
                    entity.sUserCreate = Username;
                    entity.dDateCreate = DateTime.Now;
                    entity.bActive = true;
                    entity.bIsGoc = false;
                    entity.sUserCreate = Username;
                    entity.dDateCreate = DateTime.Now;
                    conn.Insert(entity, trans);
                    #endregion
                    // sua goi thau cha
                    VDT_DA_GoiThau objGoiThauCha = conn.Get<VDT_DA_GoiThau>(Guid.Parse(iID_GoiThauID), trans);
                    objGoiThauCha.bActive = false;
                    conn.Update(objGoiThauCha,trans);
                }
                
                //them moi data vao cac bang phu lien quan
                if (model.listChiPhi.Count() > 0)
                {
                    for (int i = 0; i < model.listChiPhi.Count(); i++)
                    {
                        var entityChiPhi = new VDT_DA_GoiThau_ChiPhi();
                        entityChiPhi.MapFrom(model.listChiPhi.ToList()[i]);
                        entityChiPhi.iID_GoiThauID = entity.iID_GoiThauID;
                        conn.Insert(entityChiPhi, trans);
                    }
                }

                #region Them moi nguon von
                if (model.listNguonVon.Count() > 0)
                {
                    for (int i = 0; i < model.listNguonVon.Count(); i++)
                    {
                        var entityNguonVon = new VDT_DA_GoiThau_NguonVon();
                        entityNguonVon.MapFrom(model.listNguonVon.ToList()[i]);
                        entityNguonVon.iID_GoiThauID = entity.iID_GoiThauID;
                        conn.Insert(entityNguonVon, trans);
                    }
                }
                #endregion
                #region Them moi hang muc
                if (model.listHangMuc.Count() > 0)
                {
                    for (int i = 0; i < model.listHangMuc.Count(); i++)
                    {
                        var entityHangMuc = new VDT_DA_GoiThau_HangMuc();
                        entityHangMuc.MapFrom(model.listHangMuc.ToList()[i]);
                        entityHangMuc.iID_GoiThauID = entity.iID_GoiThauID;
                        conn.Insert(entityHangMuc, trans);
                    }
                }
                #endregion
                // commit to db
                trans.Commit();
            }
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Detail(Guid id)
        {
            ViewBag.ListNhaThau = _vonDauTuService.GetAllNhaThau().ToSelectList("iID_NhaThauID", "sTenNhaThau");
            ThongTinGoiThauViewModel objDotnhan = _vonDauTuService.GetThongTinGoiThau(id);
            ViewBag.ChiTietDuAn = _vonDauTuService.GetThongTinDuAnByDuAnId(objDotnhan.iID_DuAnID.Value);

            return View(objDotnhan);
        }

        public JsonResult ListDuAnTheoDonViQuanLy(string iID_DonViQuanLyID)
        {
            List<VDT_DA_DuAn> lstDuAn = _vonDauTuService.ListDuAnTheoDonViQuanLy(Guid.Parse(iID_DonViQuanLyID)).ToList();
            StringBuilder htmlString = new StringBuilder();
            if (lstDuAn != null && lstDuAn.Count > 0)
            {
                htmlString.AppendFormat("<option value='{0}'>{1}</option>", Guid.Empty, Constants.CHON);
                for (int i = 0; i < lstDuAn.Count; i++)
                {
                    htmlString.AppendFormat("<option value='{0}' data-sMaDuAn='{1}'>{2}</option>", lstDuAn[i].iID_DuAnID, lstDuAn[i].sMaDuAn, lstDuAn[i].sTenDuAn);
                }
            }
            return Json(htmlString.ToString(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult LayDataTheoDuAn(string iID_DuAnID,DateTime dNgayLap)
        {
            VDT_DA_DuAn d_DuAn = _vonDauTuService.LayThongTinChiTietDuAn(Guid.Parse(iID_DuAnID));
            List<VDT_DM_ChiPhi> listChiPhiByDuAn = _vonDauTuService.ListChiPhiByDuAn(Guid.Parse(iID_DuAnID),dNgayLap).ToList();
            List<NS_NguonNganSach> listNguonVonByDuAn = _vonDauTuService.ListNguonVonByDuAn(Guid.Parse(iID_DuAnID),dNgayLap).ToList();
            List<VDT_DA_DuAn_HangMuc> listHangMucByDuAn = _vonDauTuService.ListHangMucByDuAn(Guid.Parse(iID_DuAnID),dNgayLap).ToList();
            StringBuilder htmlStringCP = new StringBuilder();
            StringBuilder htmlStringNV = new StringBuilder();
            StringBuilder htmlStringHM = new StringBuilder();
            if (listChiPhiByDuAn != null && listChiPhiByDuAn.Count > 0)
            {
                htmlStringCP.AppendFormat("<option value='{0}'>{1}</option>", Guid.Empty, Constants.CHON);
                for (int i = 0; i < listChiPhiByDuAn.Count; i++)
                {
                    htmlStringCP.AppendFormat("<option value='{0}' data-sMaChiPhi='{1}'>{2}</option>", listChiPhiByDuAn[i].iID_ChiPhi, listChiPhiByDuAn[i].sMaChiPhi, listChiPhiByDuAn[i].sTenChiPhi);
                }
            }
            if (listNguonVonByDuAn != null && listNguonVonByDuAn.Count > 0)
            {
                htmlStringNV.AppendFormat("<option value='{0}'>{1}</option>", Guid.Empty, Constants.CHON);
                for (int i = 0; i < listNguonVonByDuAn.Count; i++)
                {
                    htmlStringNV.AppendFormat("<option value='{0}' >{1}</option>", listNguonVonByDuAn[i].iID_MaNguonNganSach,  listNguonVonByDuAn[i].sTen);
                }
            }
            if (listHangMucByDuAn != null && listHangMucByDuAn.Count > 0)
            {
                htmlStringHM.AppendFormat("<option value='{0}'>{1}</option>", Guid.Empty, Constants.CHON);
                for (int i = 0; i < listHangMucByDuAn.Count; i++)
                {
                    htmlStringHM.AppendFormat("<option value='{0}' >{1}</option>", listHangMucByDuAn[i].iID_DuAn_HangMucID, listHangMucByDuAn[i].sTenHangMuc);
                }
            }
            return Json(new {listChiPhi = htmlStringCP.ToString(),listNguonVon = htmlStringNV.ToString(), listHangMuc = htmlStringHM.ToString(),duAn = d_DuAn }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTongMucDauTuChiPhi(Guid iID,Guid iID_DuAnID, DateTime dNgayLap)
        {
            var tongMucDauTuChiPhi = _vonDauTuService.GetTongMucDauTuChiPhi(iID, iID_DuAnID, dNgayLap);
            return Json(new { tongMucDauTuChiPhi }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTongMucDauTuNV(int iID, Guid iID_DuAnID, DateTime dNgayLap)
        {
            var tongMucDauTuNV = _vonDauTuService.GetTongMucDauTuNguonVon(iID, iID_DuAnID, dNgayLap);
            return Json(new { tongMucDauTuNV }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTongMucDauTuHM(Guid iID, Guid iID_DuAnID, DateTime dNgayLap)
        {
            var tongMucDauTuHM = _vonDauTuService.GetTongMucDauTuHangMuc(iID, iID_DuAnID, dNgayLap);
            return Json(new { tongMucDauTuHM }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LayThongTinChiPhiDieuChinh(string iID_GoiThauGoc, DateTime dNgayLap)
        {
            IEnumerable<GoiThauChiPhiViewModel> lstChiPhi = _vonDauTuService.GetListChiPhiDieuChinh(Guid.Parse(iID_GoiThauGoc), dNgayLap);
            IEnumerable<GoiThauNguonVonViewModel> lstNguonVon = _vonDauTuService.GetListNguonVonDieuChinh(Guid.Parse(iID_GoiThauGoc), dNgayLap);
            IEnumerable<GoiThauHangMucViewModel> lstHangMuc = _vonDauTuService.GetListHangMucDieuChinh(Guid.Parse(iID_GoiThauGoc), dNgayLap);
            return Json(new { lstCp = lstChiPhi , lstNguonVon  = lstNguonVon , lstHangMuc = lstHangMuc }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetListChiTietGoiThau(Guid id)
        {
            var listGoiThauNguonVon = _vonDauTuService.GetListNguonVonChuaDieuChinhByGoiThau(id);
            var listGoiThauChiPhi = _vonDauTuService.GetListChiPhiChuaDieuChinhByGoiThau(id);
            var listGoiThauHangMuc = _vonDauTuService.GetListHangMucChuaDieuChinhByGoiThau(id);
            var listHopDong = _vonDauTuService.GetThongTinHopDong(id);
            return Json(new { status = true, nguonvon = listGoiThauNguonVon,chiphi = listGoiThauChiPhi, hangmuc = listGoiThauHangMuc, hopdong = listHopDong });
        }

        [HttpPost]
        public JsonResult OnExport(List<Guid> ids)
        {
            try
            {
                List<ThongTinGoiThauViewModel> lstGoiThau = _vonDauTuService.GetThongTinGoiThauByListGoiThauId(ids);
                if (lstGoiThau != null)
                {
                    int i = 1;
                    foreach (var item in lstGoiThau)
                    {
                        item.IStt = i;
                        i++;
                    }
                }
                var lstNhaThau = _vonDauTuService.GetAllNhaThau();
                XlsFile Result = new XlsFile(true);
                FlexCelReport fr = new FlexCelReport();
                fr.AddTable("Items", lstGoiThau);
                fr.AddTable("ItemsNhaThau", lstNhaThau);
                Result.Open(Server.MapPath("~/Areas/QLVonDauTu/ReportExcelForm/Import-Goithau-DuAn.xlsx"));
                fr.Run(Result);
                TempData["DataExportGoiThauXls"] = Result;
                FlexCelPdfExport pdf = new FlexCelPdfExport(Result, true);
                var bufferPdf = new MemoryStream();
                pdf.Export(bufferPdf);
            }
            catch(Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return Json(new { status = true}, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ExportReport()
        {
            ExcelFile xls = (ExcelFile)TempData["DataExportGoiThauXls"];
            return Print(xls, "xls", string.Format("Import-Goithau-DuAn_{0}.xlsx", DateTime.Now.ToString("ddMMyyyy_HHmm")));
        }

        [HttpPost]
        public JsonResult LoadDataExcel(HttpPostedFileBase file)
        {
            try
            {
                List<string> lstError = new List<string>();
                if (file == null) return Json(new { bIsComplete = false, sMessError = "Chưa chọn file import !" }, JsonRequestBehavior.AllowGet);
                var file_data = getBytes(file);
                var dt = ExcelHelpers.LoadExcelDataTable(file_data);
                IEnumerable<VdtDaGoiThauImportModel> dataImport = excel_result(dt, ref lstError);
                return Json(new { bIsComplete = true, data = dataImport, ListError = lstError }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return Json(new { bIsComplete = false, sMessError = "Có lỗi xảy ra trong file import !" }, JsonRequestBehavior.AllowGet);
        }

        private byte[] getBytes(HttpPostedFileBase file)
        {
            using (BinaryReader b = new BinaryReader(file.InputStream))
            {
                byte[] xls = b.ReadBytes(file.ContentLength);
                return xls;
            }
        }

        private IEnumerable<VdtDaGoiThauImportModel> excel_result(System.Data.DataTable dt, ref List<string> lstError)
        {
            Dictionary<string, Guid> dicMaDuAn = new Dictionary<string, Guid>();
            Dictionary<string, Guid> dicNhaThau = new Dictionary<string, Guid>();

            var lstAllDuAn = _vonDauTuService.GetVDTDADuAn();
            if (lstAllDuAn != null)
            {
                foreach (var item in lstAllDuAn)
                {
                    if (!dicMaDuAn.ContainsKey(item.sMaDuAn)) dicMaDuAn.Add(item.sMaDuAn, item.iID_DuAnID);
                }
            }
            var lstAllNhaThau = _vonDauTuService.GetAllNhaThau();
            if (lstAllNhaThau != null)
            {
                foreach (var item in lstAllNhaThau)
                {
                    if (!dicNhaThau.ContainsKey(item.sMaNhaThau)) dicNhaThau.Add(item.sMaNhaThau, item.iID_NhaThauID);
                }
            }

            List<VdtDaGoiThauImportModel> dataImport = new List<VdtDaGoiThauImportModel>();

            var items = dt.AsEnumerable().Where(x => x.Field<string>(0) != "" || x.Field<string>(0) != null || x.Field<string>(0) != "null");
            int index = 1;
            for (var i = 2; i < items.Count(); i++)
            {
                DataRow r = items.ToList()[i];

                var IStt = r.Field<string>(0);
                var sMaDuAn = r.Field<string>(1);
                var sSoQuyetDinh = r.Field<string>(2);
                var sNgayQuyetDinh = r.Field<string>(3);
                var sTenGoiThau = r.Field<string>(4);
                var fTienTrungThau = r.Field<string>(5);
                var iThoiGianThucHien = r.Field<string>(6);
                var sMaNhaThau = r.Field<string>(7);

                var e = new VdtDaGoiThauImportModel
                {
                    IStt = string.IsNullOrEmpty(IStt) ? string.Empty : IStt,
                    sMaDuAn = string.IsNullOrEmpty(sMaDuAn) ? string.Empty : sMaDuAn,
                    sSoQuyetDinh = string.IsNullOrEmpty(sSoQuyetDinh) ? string.Empty : sSoQuyetDinh,
                    sNgayQuyetDinh = string.IsNullOrEmpty(sNgayQuyetDinh) ? string.Empty : sNgayQuyetDinh,
                    sTenGoiThau = string.IsNullOrEmpty(sTenGoiThau) ? string.Empty : sTenGoiThau,
                    fTienTrungThau = string.IsNullOrEmpty(fTienTrungThau) ? string.Empty : fTienTrungThau,
                    iThoiGianThucHien = string.IsNullOrEmpty(iThoiGianThucHien) ? string.Empty : iThoiGianThucHien,
                    sMaNhaThau = string.IsNullOrEmpty(sMaNhaThau) ? string.Empty : sMaNhaThau,
                };
                e.bIsError = ValidateChungTuChiTietImport(index, e, dicMaDuAn, dicNhaThau, ref lstError);
                dataImport.Add(e);
                index++;
            }
            return dataImport.AsEnumerable();
        }

        public bool ValidateChungTuChiTietImport(int index, VdtDaGoiThauImportModel item, Dictionary<string, Guid> dicMaDuAn, Dictionary<string, Guid> dicNhaThau, ref List<string> lstError)
        {
            double dDoublePare = 0;
            int iIntPare = 0;
            DateTime dDatePare = DateTime.Now;
            bool bIsError = false;

            if (string.IsNullOrEmpty(item.sMaDuAn))
            {
                lstError.Add(string.Format("Dòng {0} - Chưa nhập dữ liệu {1} !", index, "mã dự án"));
                bIsError = true;
            }
            else if (!dicMaDuAn.ContainsKey(item.sMaDuAn))
            {
                lstError.Add(string.Format("Dòng {0} - Không tìm thấy dự án [{1}] !", index, item.sMaDuAn));
                bIsError = true;
            }

            if(!string.IsNullOrEmpty(item.sNgayQuyetDinh) && !DateTime.TryParseExact(item.sNgayQuyetDinh, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dDatePare))
            {
                lstError.Add(string.Format("Dòng {0} - Ngày quyết định không đúng định dạng !", index));
                bIsError = true;
            }

            if (string.IsNullOrEmpty(item.sTenGoiThau))
            {
                lstError.Add(string.Format("Dòng {0} - Chưa nhập dữ liệu {1} !", index, "tên gói thầu"));
                bIsError = true;
            }

            if (string.IsNullOrEmpty(item.fTienTrungThau))
            {
                lstError.Add(string.Format("Dòng {0} - Chưa nhập dữ liệu {1} !", index, "giá gói thầu"));
                bIsError = true;
            }else if(!double.TryParse(item.fTienTrungThau,out dDoublePare))
            {
                lstError.Add(string.Format("Dòng {0} - Giá gói thầu không đúng định dạng !", index));
                bIsError = true;
            }

            if (string.IsNullOrEmpty(item.iThoiGianThucHien))
            {
                lstError.Add(string.Format("Dòng {0} - Chưa nhập dữ liệu {1} !", index, "thời gian thực hiện"));
                bIsError = true;
            }
            else if(!int.TryParse(item.iThoiGianThucHien, out iIntPare))
            {
                lstError.Add(string.Format("Dòng {0} - Thời gian thực hiện không đúng định dạng !", index));
                bIsError = true;
            }

            if(!string.IsNullOrEmpty(item.sMaNhaThau) && !dicNhaThau.ContainsKey(item.sMaNhaThau))
            {
                lstError.Add(string.Format("Dòng {0} - Không tìm thấy nhà thầu [{1}] !", index, item.sMaNhaThau));
            }

            return bIsError;
        }

        public JsonResult OnSaveDataImport(List<VdtDaGoiThauImportModel> lstData)
        {
            try
            {
                Dictionary<string, Guid> dicMaDuAn = new Dictionary<string, Guid>();
                Dictionary<string, Guid> dicNhaThau = new Dictionary<string, Guid>();
                var lstAllDuAn = _vonDauTuService.GetVDTDADuAn();
                if (lstAllDuAn != null)
                {
                    foreach (var item in lstAllDuAn)
                    {
                        if (!dicMaDuAn.ContainsKey(item.sMaDuAn)) dicMaDuAn.Add(item.sMaDuAn, item.iID_DuAnID);
                    }
                }
                var lstAllNhaThau = _vonDauTuService.GetAllNhaThau();
                if (lstAllNhaThau != null)
                {
                    foreach (var item in lstAllNhaThau)
                    {
                        if (!dicNhaThau.ContainsKey(item.sMaNhaThau)) dicNhaThau.Add(item.sMaNhaThau, item.iID_NhaThauID);
                    }
                }

                List<VDT_DA_GoiThau> lstGoiThau = new List<VDT_DA_GoiThau>();
                foreach (var item in lstData)
                {
                    Guid iId = Guid.NewGuid();
                    VDT_DA_GoiThau obj = new VDT_DA_GoiThau();
                    obj.iID_DuAnID = dicMaDuAn[item.sMaDuAn];
                    obj.sSoQuyetDinh = item.sSoQuyetDinh;
                    if (!string.IsNullOrEmpty(item.sNgayQuyetDinh))
                        obj.dNgayQuyetDinh = DateTime.ParseExact(item.sNgayQuyetDinh, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    obj.sTenGoiThau = item.sTenGoiThau;
                    obj.fTienTrungThau = double.Parse(item.fTienTrungThau);
                    obj.iThoiGianThucHien = int.Parse(item.iThoiGianThucHien);
                    if (!string.IsNullOrEmpty(item.sMaNhaThau))
                        obj.iID_NhaThauID = dicNhaThau[item.sMaNhaThau];
                    obj.iID_GoiThauID = iId;
                    obj.iID_GoiThauGocID = iId;
                    obj.bActive = true;
                    obj.dDateCreate = DateTime.Now;
                    obj.sUserCreate = Username;
                    lstGoiThau.Add(obj);
                }
                _vonDauTuService.AddRangerVdtDaGoiThau(lstGoiThau);
                return Json(new { bIsSuccess = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

            }
            return Json(new { bIsSuccess = false }, JsonRequestBehavior.AllowGet);
        }
    }
}