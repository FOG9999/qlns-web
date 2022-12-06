using FlexCel.Core;
using FlexCel.Render;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Viettel.Domain.DomainModel;
using Viettel.Models.BaoHiemXaHoi;
using Viettel.Services;
using VIETTEL.Areas.BaoHiemXaHoi.Models;
using VIETTEL.Common;
using VIETTEL.Controllers;
using VIETTEL.Helpers;

namespace VIETTEL.Areas.BaoHiemXaHoi.Controllers
{
    public class QLBenhNhanBHXHController : AppController
    {
        private readonly IBaoHiemXaHoiService _bHXHService = BaoHiemXaHoiService.Default;
        private readonly INganSachService _iNganSachService = NganSachService.Default;
        private const string sTemplateName = "DanhSachBenhNhan_{0}";
        private const string sFilePath = "/Report_ExcelFrom/BHXH/rpt_LoiImportBenhNhanDieuTriNoiTru_Danhsach.xls";

        #region QL Bệnh nhân BHXH
        // GET: BaoHiemXaHoi/QLBenhNhanBHXH
        public ActionResult Index()
        {
            QLBenhNhanBHXHPagingViewModel vm = new QLBenhNhanBHXHPagingViewModel();
            vm._paging.CurrentPage = 1;
            vm.iThangFrom = DateTime.Now.Month.ToString();
            vm.iThangTo = DateTime.Now.Month.ToString();

            vm.Items = _bHXHService.GetAllBenhNhanDieuTriBHXH(ref vm._paging, int.Parse(PhienLamViec.iNamLamViec), DateTime.Now.Month, DateTime.Now.Month, string.Empty, string.Empty, null, "", "");
            return View(vm);
        }

        [HttpPost]
        public ActionResult QLBenhNhanBHXHListView(PagingInfo _paging, string sMaDonViBHXH, string sMaDonViNS, int? iThangFrom, int? iThangTo, int? iSoNgayDieuTri, string sHoTen, string sMaThe)
        {
            QLBenhNhanBHXHPagingViewModel vm = new QLBenhNhanBHXHPagingViewModel();
            vm._paging = _paging;
            vm.Items = _bHXHService.GetAllBenhNhanDieuTriBHXH(ref vm._paging, int.Parse(PhienLamViec.iNamLamViec), iThangFrom, iThangTo, sMaDonViBHXH, sMaDonViNS, iSoNgayDieuTri, sHoTen, sMaThe);
            vm.iThangFrom = iThangFrom.ToString();
            vm.iThangTo = iThangTo.ToString();
            vm.sMaDonViNS = sMaDonViNS;
            vm.sMaDonViBHXH = sMaDonViBHXH;
            vm.iSoNgayDieuTri = iSoNgayDieuTri.ToString();
            vm.sHoTen = sHoTen;
            vm.sMaThe = sMaThe;
            return PartialView("_list", vm);
        }

        public JsonResult GetDonviNS()
        {
            var result = new List<dynamic>();
            //var listDonViNS = _bHXHService.GetAllDonViNS(int.Parse(PhienLamViec.iNamLamViec)).ToList();
            var listDonViNS = _iNganSachService.GetDonviListByUser(Username, PhienLamViec.NamLamViec).ToList();
            
            result.Insert(0, new { id = "", text = "--Chọn đơn vị NS--" });
            if (listDonViNS != null && listDonViNS.Any())
            {
                foreach (var item in listDonViNS)
                {
                    result.Add(new { id = item.iID_MaDonVi, text = item.iID_MaDonVi + " - " + item.sTen });
                }
            }

            return Json(new { status = true, data = result });
        }

        public JsonResult GetDataComboBoxDonViBHParent(string sMaDonViNS)
        {
            var result = new List<dynamic>();
            var listDonViBHParent = _bHXHService.GetListDonViBHXHByDonViNS(sMaDonViNS).ToList();
            result.Insert(0, new { id = "", text = "--Chọn đơn vị BHXH--" });
            if (listDonViBHParent != null && listDonViBHParent.Any())
            {
                foreach (var item in listDonViBHParent)
                {
                    result.Add(new { id = item.iID_MaDonViBHXH, text = item.iID_MaDonViBHXH + " - " + item.sTen });
                }
            }

            return Json(new { status = true, data = result });
        }

        #endregion

        #region Import file bệnh nhân BHXH
        public ActionResult ImportBenhNhanBHXH()
        {
            TepBenhNhanBHXHPagingViewModel vm = new TepBenhNhanBHXHPagingViewModel();
            vm._paging.CurrentPage = 1;

            vm.Items = _bHXHService.GetAllFileBenhNhanBHXH(ref vm._paging, int.Parse(PhienLamViec.iNamLamViec), null, string.Empty, string.Empty);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ImportFileBHXHListView(PagingInfo _paging, string sTenFile, string sMoTa, int? iThang)
        {
            TepBenhNhanBHXHPagingViewModel vm = new TepBenhNhanBHXHPagingViewModel();
            vm._paging = _paging;
            vm.Items = _bHXHService.GetAllFileBenhNhanBHXH(ref vm._paging, int.Parse(PhienLamViec.iNamLamViec), iThang, sMoTa, sTenFile);

            return PartialView("_listFile", vm);
        }

        [HttpPost]
        public ActionResult LoadDataXML()
        {
            List<QLBenhNhanBHXHViewModel> lstBenhNhan = new List<QLBenhNhanBHXHViewModel>();
            List<TepBenhNhanBHXHViewModel> lstTepBenhNhan = new List<TepBenhNhanBHXHViewModel>();
            List<ImportBHXHModel> lstResult = new List<ImportBHXHModel>();
            ImportBhxhProcessModel modelImport = new ImportBhxhProcessModel();
            List<BenhNhanBHXHTempViewModel> lstBenhNhanTemp = new List<BenhNhanBHXHTempViewModel>();
            bool bIsErrorDonVi = false;
            string sFilePath = HttpContext.Server.MapPath("~/App_Data/ExportXml");
            if (!Directory.Exists(sFilePath))
            {
                Directory.CreateDirectory(sFilePath);
            }
            if (Request.Files.Count > 0)
            {
                try
                {
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        HttpPostedFileBase file = Request.Files[i];
                        string sKey = Request.Files.Keys[i];
                        string iThangTep = sKey.Substring(0, sKey.IndexOf("|"));
                        string sMoTaTep = sKey.Substring(sKey.IndexOf("|") + 1);
                        
                        string sFileName = Request.Files[i].FileName.Substring(0, Request.Files[i].FileName.LastIndexOf('.')) + "_" + DateTime.Now.ToString("ddMMyyyy_HHmmss") + ".xml";
                        string sUploadName = Path.Combine(sFilePath, sFileName);
                        Request.Files[i].SaveAs(sUploadName);

                        TepBenhNhanBHXHViewModel benhNhan = new TepBenhNhanBHXHViewModel();
                        benhNhan.iID_TepBenhNhanID = Guid.NewGuid();
                        benhNhan.sTenFile = file.FileName;
                        benhNhan.iThang = int.Parse(iThangTep);
                        benhNhan.sMoTa = sMoTaTep;
                        benhNhan.iNamLamViec = PhienLamViec.NamLamViec;
                        benhNhan.sID_MaNguoiDungTao = Username;
                        benhNhan.dNgayTao = DateTime.Now;
                        benhNhan.sFilePath = sFileName;
                        lstTepBenhNhan.Add(benhNhan);

                        ImportBHXHModel importModel = new ImportBHXHModel();
                        importModel.dNgayTao = DateTime.Now;
                        importModel.iID_ImportID = Guid.NewGuid();
                        importModel.iNamLamViec = int.Parse(PhienLamViec.iNamLamViec);
                        importModel.iThang = benhNhan.iThang;
                        importModel.sFileName = file.FileName;
                        importModel.sFileNameServer = sFileName;
                        importModel.sID_MaNguoiDungTao = Username;
                        lstResult.Add(importModel);
                        
                    }

                    // - chay process
                    foreach(var importItem in lstResult)
                    {
                        modelImport.ProcessImport(int.Parse(PhienLamViec.iNamLamViec), importItem, sFilePath, ref bIsErrorDonVi);
                    }

                    var result = _bHXHService.SaveFileAndImportBenhNhan(int.Parse(PhienLamViec.iNamLamViec), lstTepBenhNhan, lstResult);

                    //lstBenhNhanTemp = _bHXHService.GetBenhNhanImportTemp(lstResult.Select(x => x.iID_ImportID).ToList()).ToList();
                    //TempData["DanhSachBenhNhanDung"] = lstBenhNhan.Where(x => x.bIsFalse == true).ToList();
                    //TempData["DanhSachBenhNhanSai"] = lstBenhNhan.Where(x => x.bIsFalse == false).ToList();
                    //TempData["TepBenhNhan"] = lstTepBenhNhan;
                    //TempData["File"] = Request.Files;
                }
                catch (Exception ex)
                {
                    AppLog.LogError(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                }
            }
            string lstMaDonViBHXH = string.Join(",", _bHXHService.GetListDonViBHXH(int.Parse(PhienLamViec.iNamLamViec), Username).Select(x => x.iID_MaDonViBHXH));
            ViewBag.LstMaDonViBHHXH = lstMaDonViBHXH;
            ViewBag.lstIdImport = string.Join(",", lstResult.Select(n => n.iID_ImportID.ToString()));
            ViewBag.bIsErrorDonVi = bIsErrorDonVi ? 1 : 0;
            return PartialView("_listDanhSach", lstResult);
        }
        
        [HttpPost]
        public JsonResult LoadDataTable(List<Guid> lstImportId, int iPage)
        {
            var lstDataTable = _bHXHService.GetBenhNhanImportTemp(lstImportId, iPage);
            var lstError = _bHXHService.GetErrorByLine(lstDataTable.Select(n => new TTblImportLine() { iLine = n.iLine ?? 0, iThang = n.iThangBN }).ToList());
            return Json(new { bIsComplete = true, lstDataTable = lstDataTable, lstError = lstError }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult LoadDataImportError(List<Guid> lstImportId, int iPage)
        {
            List<BenhNhanBaoHiemQuery> lstUpdate = (List<BenhNhanBaoHiemQuery>)TempData["DanhSachBenhNhanSai"];
            TempData.Keep("DanhSachBenhNhanSai");
            if (TempData["lstIdImport"] != null)
            {
                TempData.Keep("lstIdImport");
            }
            var lstDataTable = _bHXHService.GetBenhNhanUpdateImportTemp(lstImportId, lstUpdate, iPage);
            var lstError = _bHXHService.GetErrorByLine(lstDataTable.Select(n => new TTblImportLine() { iLine = n.iLine ?? 0, iThang = n.iThangBN }).ToList());
            return Json(new { bIsComplete = true, lstDataTable = lstDataTable, lstError = lstError }, JsonRequestBehavior.AllowGet);
        }

        private bool CheckDateTime(string dateTime)
        {
            if (string.IsNullOrEmpty(dateTime) || dateTime.Length != 8)
                return false;
            DateTime datePare = new DateTime();
            string year = dateTime.Substring(0, 4);
            string month = dateTime.Substring(4, 2);
            string day = dateTime.Substring(6, 2);
            if (DateTime.TryParse(string.Format("{0}/{1}/{2}", year, month, day), out datePare))
                return true;
            return false;
        }

        private bool CheckNumber(string number)
        {
            if (string.IsNullOrEmpty(number))
                return false;
            try
            {
                int num = Convert.ToInt32(number);
            }
            catch
            {
                return false;
            }
            return true;
        }

        private bool CheckExistBenhNhan(string maBH, string ngayVaoVien, string ngayRaVien)
        {
            return _bHXHService.CheckExistBenhNhan(maBH, ngayVaoVien, ngayRaVien);
        }

        [HttpPost]
        public JsonResult BenhNhanBHXHSave(List<BenhNhanBaoHiemQuery> lstBenhNhanUpDate, string lstIdImport/*, string sMaDonViBHXH*//*, int iThang, string sMoTa*/)
        {
            int countError = 0;
            TempData["DanhSachBenhNhanSai"] = lstBenhNhanUpDate;
            TempData["lstIdImport"] = lstIdImport;
            if (!_bHXHService.SaveBenhNhanBHXH(lstBenhNhanUpDate, lstIdImport, Username, ref countError))
                return Json(new { bIsComplete = false, sMessError = "Không cập nhật được dữ liệu !" }, JsonRequestBehavior.AllowGet);

            return Json(new { bIsComplete = true, numberBNLoi = countError }, JsonRequestBehavior.AllowGet); ;
        }

        private void AddListFileDownload(Dictionary<int, List<string>> lstFileDownload, ref List<TepBenhNhanBHXHViewModel> lstTepBenhNhan)
        {
            foreach(var item in lstTepBenhNhan)
            {
                if (!lstFileDownload.ContainsKey(item.iThang ?? 0)) continue;
                item.sFilePath = string.Join(",", lstFileDownload[item.iThang ?? 0]);
            }
        }

        private void UploadFileGoc(ref Dictionary<int, List<string>> lstFileDownload)
        {
            string sFilePath = HttpContext.Server.MapPath("~/App_Data/ExportXml");
            if (!Directory.Exists(sFilePath))
            {
                Directory.CreateDirectory(sFilePath);
            }
            if (TempData["File"] != null)
            {
                var files = (System.Web.HttpFileCollectionWrapper)TempData["File"];
                for (int i = 0; i < files.Count; ++i)
                {
                    string sFileName = files[i].FileName.Substring(0, files[i].FileName.LastIndexOf('.')) + DateTime.Now.ToString("ddMMyyyy_HHmmss") + ".xml";
                    string sUploadName = Path.Combine(sFilePath, sFileName);

                    int iThang = int.Parse(files.GetKey(i).Split('|')[0].ToString());
                    if (!lstFileDownload.ContainsKey(iThang))
                        lstFileDownload.Add(iThang, new List<string>());
                    lstFileDownload[iThang].Add(sFileName);

                    files[i].SaveAs(sUploadName);
                }
            }
        }

        private void WriteDataXml(List<QLBenhNhanBHXHViewModel> data, ref Dictionary<int, List<string>> lstFileDownload)
        {
            var iThangs = data.GroupBy(n => (n.iThang ?? 0)).Select(n => n.Key);
            string sFilePath = HttpContext.Server.MapPath("~/App_Data/ExportXml");
            if (!Directory.Exists(sFilePath))
            {
                Directory.CreateDirectory(sFilePath);
            }

            foreach(int iThang in iThangs)
            {
                var lstDataCheck = data.Where(n => n.iThang == iThang);
                if (lstDataCheck == null || lstDataCheck.Count() == 0) continue;
                string sFileName = string.Format("FileLoiBenhNhan_{0}_{1}.xml", iThang, DateTime.Now.ToString("ddMMyyyy_HHmmss"));
                string sFileUpload = Path.Combine(sFilePath, sFileName);
                if (!lstFileDownload.ContainsKey(iThang))
                    lstFileDownload.Add(iThang, new List<string>());
                lstFileDownload[iThang].Add(sFileName);

                XmlWriter writer = XmlWriter.Create(sFileUpload);
                writer.WriteStartDocument();
                writer.WriteStartElement("NewDataSet");
                foreach (var item in data)
                {
                    writer.WriteStartElement("DLKCB");
                    writer.WriteElementString("stt", item.sSoTTBN);
                    writer.WriteElementString("ho_ten", item.sTen);
                    writer.WriteElementString("NGAYSINH", item.sNgaySinh);
                    writer.WriteElementString("GIOITINH", item.sGioiTinh);
                    writer.WriteElementString("CAPBAC", item.sCapBac);
                    writer.WriteElementString("MATHE", item.sMaThe);
                    writer.WriteElementString("NGAY_VAO_VIEN", item.sNgayVaoVien);
                    writer.WriteElementString("NGAY_RA_VIEN", item.sNgayRaVien);
                    writer.WriteElementString("SO_NGAY_DTRI", item.sSoNgayDieuTriBN);
                    writer.WriteElementString("MABV", item.sMaBenhVien);
                    writer.WriteElementString("TENBV", item.sTenBenhVien);
                    writer.WriteElementString("MADV", item.iID_MaDonVi);
                    writer.WriteElementString("TENDV", item.sTenDonVi);
                    writer.WriteElementString("GHICHU", item.sGhiChu);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
                writer.Close();
            }

        }

        public FileResult Download(string sFileName)
        {
            if (string.IsNullOrEmpty(sFileName)) return null;
            string sFilePath = HttpContext.Server.MapPath("~/App_Data/ExportXml");
            string sFileDownload = Path.Combine(sFilePath, sFileName);
            if (!System.IO.File.Exists(sFileDownload)) return null;

            byte[] fileBytes = System.IO.File.ReadAllBytes(sFileDownload);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, sFileName);
        }

        public ActionResult DanhSachBenhNhanLoi()
        {
            List<QLBenhNhanBHXHViewModel> lstBenhNhanSai = new List<QLBenhNhanBHXHViewModel>();
            if (TempData["DanhSachBenhNhanSai"] != null)
            {
                TempData.Keep("DanhSachBenhNhanSai");
            }
            if(TempData["lstIdImport"] != null)
            {
                ViewBag.lstImportID = TempData["lstIdImport"];
                TempData.Keep("lstIdImport");
            }
            return View(lstBenhNhanSai);
        }

        [HttpPost]
        public ActionResult XuatFileExcel()
        {
            ExcelFile excel = TaoFileExel();
            using (MemoryStream stream = new MemoryStream())
            {
                excel.Save(stream);
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Danh sach import loi benh nhan dieu tri noi tru.xls");
            }
        }

        public ExcelFile TaoFileExel()
        {
            var datas = new List<QLBenhNhanBHXHViewModel>();

            if (TempData["DanhSachBenhNhanSai"] != null)
            {
                datas = (List<QLBenhNhanBHXHViewModel>)TempData["DanhSachBenhNhanSai"];
                TempData.Keep("DanhSachBenhNhanSai");
            }


            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath(sFilePath));
            FlexCelReport fr = new FlexCelReport();
            fr.SetValue("iNamLamViec", PhienLamViec.NamLamViec);
            fr.AddTable<QLBenhNhanBHXHViewModel>("dt", datas);
            fr.Run(Result);
            return Result;
        }

        public ActionResult XuatFilePDF()
        {
            var datas = new List<QLBenhNhanBHXHViewModel>();

            if (TempData["DanhSachBenhNhanSai"] != null)
            {
                datas = (List<QLBenhNhanBHXHViewModel>)TempData["DanhSachBenhNhanSai"];
                TempData.Keep("DanhSachBenhNhanSai");
            }


            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath(sFilePath));
            FlexCelReport fr = new FlexCelReport();
            fr.SetValue("iNamLamViec", PhienLamViec.NamLamViec);
            fr.AddTable<QLBenhNhanBHXHViewModel>("dt", datas);
            fr.Run(Result);

            FlexCelPdfExport pdf = new FlexCelPdfExport(Result, true);
            var bufferPdf = new MemoryStream();

            pdf.Export(bufferPdf);

            string sContentType = "application/pdf";
            string sFileName = "Danh sach import loi benh nhan dieu tri noi tru.pdf";

            Response.ContentType = sContentType;
            Response.AddHeader("Content-Disposition", "attachment; filename=" + sFileName);
            Response.BinaryWrite(bufferPdf.ToArray());
            Response.Flush();
            Response.End();
            return RedirectToAction("Index");
        }
        #endregion
    }
}