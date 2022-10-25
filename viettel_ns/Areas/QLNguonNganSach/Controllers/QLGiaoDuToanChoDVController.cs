using Dapper;
using DapperExtensions;
using DomainModel;
using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using Newtonsoft.Json;
using OfficeOpenXml;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLNguonNganSach;
using Viettel.Services;
using VIETTEL.Areas.QLNguonNganSach.Models;
using VIETTEL.Common;
using VIETTEL.Controllers;
using VIETTEL.Models;

namespace VIETTEL.Areas.QLNguonNganSach.Controllers
{
    public class QLGiaoDuToanChoDVController : AppController
    {
        private readonly IQLNguonNganSachService _qLNguonNSService = QLNguonNganSachService.Default;

        #region Quan ly giao du toan cho don vi
        // GET: QLNguonNganSach/QLGiaoDuToanChoDV
        public ActionResult Index()
        {
            NNSGiaoDuToanViewModel vm = new NNSGiaoDuToanViewModel();
            vm._paging.CurrentPage = 1;
            vm.Items = _qLNguonNSService.GetAllNNSGiaoDuToanChoDV(ref vm._paging, "", "", "", "", null, null, "", null, null, Username);
            List<DM_LoaiDuToan> lstLoaiDuToan = _qLNguonNSService.GetAllLoaiDuToan("", Username);
            lstLoaiDuToan.Insert(0, new DM_LoaiDuToan { sMaLoaiDuToan = string.Empty, sTenLoaiDuToan = Constants.TAT_CA });

            //vm.Items = _qLNguonNSService.GetAllNNSGiaoDuToanChoDV(ref vm._paging,"","","",Username);
            //List<DM_LoaiDuToan> lstLoaiDuToan = _qLNguonNSService.GetAllLoaiDuToan("",Username);
            ////lstLoaiDuToan.Insert(0, new DM_LoaiDuToan { sMaLoaiDuToan = string.Empty, sTenLoaiDuToan = Constants.TAT_CA });

            ViewBag.ListLoaiDuToan = lstLoaiDuToan.ToSelectList("sMaLoaiDuToan", "sTenLoaiDuToan");
            return View(vm);
        }

        [HttpPost]
        public ActionResult NNSGiaoDuToanListView(PagingInfo _paging, string sSoChungTu, string sNoiDung, string sLoaiDuToan, string sSoQuyetDinh, DateTime? dNgayQuyetDinhTu, DateTime? dNgayQuyetDinhDen, string sSoCongVan, DateTime? dNgayCongVanTu, DateTime? dNgayCongVanDen)
        {
            NNSGiaoDuToanViewModel vm = new NNSGiaoDuToanViewModel();
            vm._paging = _paging;

            //vm.Items = _qLNguonNSService.GetAllNNSGiaoDuToanChoDV(ref vm._paging, sSoChungTu, sNoiDung, sLoaiDuToan, Username);


            vm.Items = _qLNguonNSService.GetAllNNSGiaoDuToanChoDV(ref vm._paging, sSoChungTu, sNoiDung, sLoaiDuToan, sSoQuyetDinh, dNgayQuyetDinhTu, dNgayQuyetDinhDen, sSoCongVan, dNgayCongVanTu, dNgayCongVanDen, Username);
            List<DM_LoaiDuToan> lstLoaiDuToan = _qLNguonNSService.GetAllLoaiDuToan("", Username);
            lstLoaiDuToan.Insert(0, new DM_LoaiDuToan { sMaLoaiDuToan = string.Empty, sTenLoaiDuToan = Constants.TAT_CA });
            ViewBag.ListLoaiDuToan = lstLoaiDuToan.ToSelectList("sMaLoaiDuToan", "sTenLoaiDuToan");

            return PartialView("_list", vm);
        }

        [HttpPost]
        public ActionResult GetModal(Guid? id)
        {
            NNS_DuToan data = new NNS_DuToan();
            if (id.HasValue)
            {
                data = _qLNguonNSService.GetNNSGiaoDuToanByID(id.Value);
            }
            else
            {
                int indexMax = _qLNguonNSService.GetMaxIndexNNSGiaoDuToan(Username);
                string sSochungTu = GenerateSoChungTu(indexMax + 1);
                data.sSoChungTu = sSochungTu;
                // set default dNgayChungTu, dNgayQuyetDinh, dNgayCongVan = today
                data.dNgayChungTu = DateTime.Now;
                //data.dNgayQuyetDinh = DateTime.Now;
                data.dNgayCongVan = DateTime.Now;
            }
            List<DM_LoaiDuToan> lstLoaiDuToan = _qLNguonNSService.GetAllLoaiDuToan("", Username);
            lstLoaiDuToan.Insert(0, new DM_LoaiDuToan { sMaLoaiDuToan = string.Empty, sTenLoaiDuToan = Constants.TAT_CA });
            ViewBag.ListLoaiDuToan = lstLoaiDuToan.ToSelectList("sMaLoaiDuToan", "sTenLoaiDuToan");

            return PartialView("_modalUpdate", data);
        }

        [HttpPost]
        public JsonResult GetValueDotNhanById(Guid id)
        {
            if (id == null && id == Guid.Empty)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
            var model = _qLNguonNSService.Get_NNS_DuToan_DotNhan_ById(id);
            if (model == null)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { status = true, data = model.sNoiDung }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetModalDetail(Guid id)
        {
            NNS_DuToan data = _qLNguonNSService.GetNNSGiaoDuToanByID(id);
            return PartialView("_modalDetail", data);
        }

        [HttpPost]
        public JsonResult NNSGiaoDuToanSave(NNSDuToanSaveModel data)
        {
            if (data == null)
            {
                return Json(new { status = false, sMessage = "Không lấy được dữ liệu truyền lên" }, JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrEmpty(data.sMaLoaiDuToan))
            {
                return Json(new { status = false, sMessage = "Chọn loại dự toán" }, JsonRequestBehavior.AllowGet);
            }

            if (data.iID_DuToan == null || data.iID_DuToan == Guid.Empty)
            {
                if (data.sMaLoaiDuToan == "001")
                {
                    var checkDuToan = _qLNguonNSService.CheckCreateLoaiDuToanDauNam(Username, data.sMaLoaiDuToan);
                    if (checkDuToan == false)
                    {
                        return Json(new { status = false, sMessage = "Dự toán đầu năm đã được tạo. Vui lòng kiểm tra lại" });
                    }
                }

                if (data.sMaLoaiDuToan == "002")
                {
                    var checkDuToan = _qLNguonNSService.CheckCreateLoaiDuToanDauNam(Username, data.sMaLoaiDuToan);
                    if (checkDuToan == false)
                    {
                        return Json(new { status = false, sMessage = "Dự toán năm trước chuyển sang đã được tạo. Vui lòng kiểm tra lại" });
                    }
                }
            }

            if (!data.dNgayChungTu.HasValue)
            {
                return Json(new { status = false, sMessage = "Chọn ngày chứng từ" }, JsonRequestBehavior.AllowGet);
            }

            var model = new NNS_DuToan();
            model.sMaLoaiDuToan = data.sMaLoaiDuToan;
            model.sTenLoaiDuToan = data.sTenLoaiDuToan;
            model.sNoiDung = data.sNoiDung;
            model.iID_DuToan = data.iID_DuToan;
            model.sSoChungTu = data.sSoChungTu;
            model.dNgayChungTu = data.dNgayChungTu;
            model.sSoQuyetDinh = data.sSoQuyetDinh;
            model.dNgayQuyetDinh = data.dNgayQuyetDinh;
            model.sSoCongVan = data.sSoCongVan;
            model.dNgayCongVan = data.dNgayCongVan;
            if (!_qLNguonNSService.InsertNNSDuToanGiaoDV(ref model, data.ListChungTuDotGom, data.ListChungTu, Request.UserHostAddress, Username))
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            return Json(new { status = true, iID_DuToan = model.iID_DuToan }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public bool NNSGiaoDuToanDelete(Guid idDuToan)
        {
            if (!_qLNguonNSService.DeleteNNSGiaoDuToan(idDuToan)) return false;

            return true;
        }

        private string GenerateSoChungTu(int indexMax)
        {
            if (indexMax < 10)
            {
                return "DTNNS-000" + indexMax;
            }
            else if (indexMax < 100)
            {
                return "DTNNS-00" + indexMax;
            }
            else if (indexMax < 1000)
            {
                return "DTNNS-0" + indexMax;
            }
            return "DTNNS-" + indexMax;
        }

        [HttpPost]
        public ActionResult ExportData(string sSoChungTu = "", string sNoiDung = "", string sMaLoaiDuToan = "", string sSoQuyetDinh = "", DateTime? dNgayQuyetDinhTu = null, DateTime? dNgayQuyetDinhDen = null, string sSoCongVan = "", DateTime? dNgayCongVanTu = null, DateTime? dNgayCongVanDen = null, string sLoaiTep = "")
        {
            var listData = _qLNguonNSService.ExportData(PhienLamViec.NamLamViec, sSoChungTu, sNoiDung, sMaLoaiDuToan, sSoQuyetDinh, dNgayQuyetDinhTu, dNgayQuyetDinhDen, sSoCongVan, dNgayCongVanTu, dNgayCongVanDen);

            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath("~/Areas/QLNguonNganSach/ReportExcelForm/rpt_NNS_DuToan.xlsx"));
            FlexCelReport fr = new FlexCelReport();
            fr.AddTable<NNSDuToanExportDataModel>("DuToan", listData.AsEnumerable());
            fr.SetValue("iNamLamViec", PhienLamViec.iNamLamViec);
            fr.Run(Result);

            using (MemoryStream stream = new MemoryStream())
            {
                Result.Save(stream);

                if (sLoaiTep == "EXCEL")
                {
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Danh sách dự toán năm {PhienLamViec.iNamLamViec}.xlsx");
                }
            }

            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Chi tiet du toan
        public ActionResult ChiTietDuToan(string id)
        {
            var ids = id.Split('_');
            var idDuToan = "";
            if (ids.Length >= 1)
            {
                idDuToan = ids[0];
            }
            var idMaChungTu = "";
            if (ids.Length >= 2)
            {
                idMaChungTu = ids[1];
            }
            var idNhiemVu = "";
            if (ids.Length >= 3)
            {
                idNhiemVu = ids[2];
            }
            var vm = new NNSDuToanChiTietViewModel
            {
                Id_DuToan = idDuToan,
                Entity = _qLNguonNSService.GetNNSGiaoDuToanByID(Guid.Parse(idDuToan)),
            };
            List<DM_LoaiDuToan> lstLoaiDuToan = _qLNguonNSService.GetAllLoaiDuToan("", Username);
            lstLoaiDuToan.Insert(0, new DM_LoaiDuToan { sMaLoaiDuToan = string.Empty, sTenLoaiDuToan = Constants.TAT_CA });
            ViewBag.ListLoaiDuToan = lstLoaiDuToan.ToSelectList("sMaLoaiDuToan", "sTenLoaiDuToan");
            if (!string.IsNullOrEmpty(idNhiemVu))
            {
                var nhiemVu = _qLNguonNSService.GetNhiemVuById(idNhiemVu);
                if (nhiemVu != null)
                {
                    ViewBag.Title = $"Nhiệm vụ {nhiemVu.sNhiemVu}";
                }
                else
                {
                    ViewBag.Title = "Nhiệm vụ";
                }
                vm.Id_MaChungTu = idMaChungTu;
                vm.Id_NhiemVu = idNhiemVu;
                return View(vm);
            }
            else
            {
                if (vm.Entity.sMaLoaiDuToan == "001" || vm.Entity.sMaLoaiDuToan == "002")
                {
                    return View(vm);
                }

                return Redirect($"/QLNguonNganSach/NNSDuToanNhiemVu?id={idDuToan}&&filter=null");
            }
        }

        public ActionResult ChiTietDuToanNhiemVu(string idDuToan, string idMaChungTu, string idNhiemVu)
        {
            var nhiemVu = _qLNguonNSService.GetNhiemVuById(idNhiemVu);
            if (nhiemVu != null)
            {
                ViewBag.Title = $"Nhiệm vụ {nhiemVu.sNhiemVu}";
            }
            else
            {
                ViewBag.Title = "Nhiệm vụ";
            }

            var vm = new NNSDuToanChiTietViewModel
            {
                Id_DuToan = idDuToan,
                Id_MaChungTu = idMaChungTu,
                Id_NhiemVu = idNhiemVu,
                Entity = _qLNguonNSService.GetNNSGiaoDuToanByID(Guid.Parse(idDuToan)),
            };
            List<DM_LoaiDuToan> lstLoaiDuToan = _qLNguonNSService.GetAllLoaiDuToan("", Username);
            lstLoaiDuToan.Insert(0, new DM_LoaiDuToan { sMaLoaiDuToan = string.Empty, sTenLoaiDuToan = Constants.TAT_CA });
            ViewBag.ListLoaiDuToan = lstLoaiDuToan.ToSelectList("sMaLoaiDuToan", "sTenLoaiDuToan");
            return View(vm);
        }

        public ActionResult SheetFrameNhiemVu(string id, string filter, string idMaChungTu, string idNhiemVu)
        {
            var filters = filter == null ? Request.QueryString.ToDictionary() : JsonConvert.DeserializeObject<Dictionary<string, string>>(filter);
            var listModel = new List<NNSDuToanChiTietGetSoTienByDonViModel>();
            if (!string.IsNullOrEmpty(idMaChungTu))
            {
                listModel = _qLNguonNSService.GetDuLieuSoTienNoiDungChiTheoIdMaChungTu(Username, idNhiemVu);
            }
            var dutoan = _qLNguonNSService.GetNNSGiaoDuToanByID(Guid.Parse(id));
            var listNoiDungChiTongTien = _qLNguonNSService.GetTongTienDuToanChiTiet(id, Username, dutoan.sMaLoaiDuToan);
            var sheet = new NNSDuToan_SheetTable(id, filters, listModel, listNoiDungChiTongTien, true, Username, idNhiemVu);
            var vm = new NNSDuToanChiTietViewModel
            {
                Sheet = new SheetViewModel(
                    bang: sheet,
                    id: id,
                    filters: sheet.Filters,
                    urlPost: Url.Action("Save", "QLGiaoDuToanChoDV", new { area = "QLNguonNganSach" })
                    ),
            };
            //HttpContext.Session.Add("SESSION_IDNhiemVu", idNhiemVu);
            return View("_sheetFrame", vm);
        }

        [Authorize]
        public JsonResult get_DanhSach(String Truong, String GiaTri, String DSGiaTri)
        {
            if (Truong == "TenDonVi")
            {
                return get_DanhSachDonViGiaoDuToan(GiaTri);
            }
            else if (Truong == "sTenPhongBan")
            {
                return get_DanhSachPhongBan(GiaTri);
            }
            return null;
        }

        private JsonResult get_DanhSachPhongBan(String GiaTri)
        {
            List<Object> list = new List<Object>();
            String SQL = String.Format("SELECT TOP 10 sKyHieu, sTen, sMoTa FROM NS_PhongBan WHERE iTrangThai=1 AND (sKyHieu LIKE @sKyHieu OR sTen LIKE @sTen OR sMoTa LIKE @sMoTa) ORDER BY sKyHieu");
            SqlCommand cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@sKyHieu", "%" + GiaTri + "%");
            cmd.Parameters.AddWithValue("@sTen", "%" + GiaTri + "%");
            cmd.Parameters.AddWithValue("@sMoTa", "%" + GiaTri + "%");
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            if (dt.Rows.Count == 0)
            {
                //Trong trường hợp không tìm ra dữ liệu sẽ hiển thị tất cả
                dt.Dispose();
                SQL = String.Format("SELECT sKyHieu, sTen, sMoTa FROM NS_PhongBan WHERE iTrangThai=1 ORDER BY sKyHieu");
                cmd = new SqlCommand(SQL);
                dt = Connection.GetDataTable(cmd);
                cmd.Dispose();
            }
            int i;
            for (i = 0; i < dt.Rows.Count; i++)
            {
                Object item = new
                {
                    value = String.Format("{0}", dt.Rows[i]["sKyHieu"]),
                    label = String.Format("{0} - {1}", dt.Rows[i]["sTen"], dt.Rows[i]["sMoTa"])
                };
                list.Add(item);
            }
            dt.Dispose();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult get_DanhSachDonViGiaoDuToan(String GiaTri)
        {
            String MaND = User.Identity.Name;
            List<Object> list = new List<Object>();

            String DK = String.Format("iTrangThai=1 AND iNamLamViec_DonVi=@iNamLamViec ");
            String SQL = String.Format("SELECT TOP 10 iID_MaDonVi, sTen FROM NS_DonVi WHERE {0} AND (iID_MaDonVi LIKE @iID_MaDonVi OR sTen like @sTen) ORDER BY iID_MaDonVi", DK);
            SqlCommand cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@iID_MaDonVi", "%" + GiaTri + "%");
            cmd.Parameters.AddWithValue("@sTen", "%" + "%" + GiaTri + "%");
            //cmd.Parameters.AddWithValue("@sSoTaiKhoan", "%" + GiaTri + "%");
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            if (dt.Rows.Count == 0)
            {
                //Trong trường hợp không tìm ra dữ liệu sẽ hiển thị tất cả
                dt.Dispose();
                DK = String.Format("iTrangThai=1 AND iNamLamViec_DonVi=@iNamLamViec ");
                SQL = String.Format("SELECT TOP 10 iID_MaDonVi, sTen FROM NS_DonVi WHERE {0}  ORDER BY iID_MaDonVi", DK);
                cmd = new SqlCommand(SQL);
                cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
                dt = Connection.GetDataTable(cmd);
                cmd.Dispose();
            }
            int i;
            for (i = 0; i < dt.Rows.Count; i++)
            {
                Object item = new
                {
                    value = String.Format("{0}", dt.Rows[i]["iID_MaDonVi"]),
                    label = String.Format("{0} - {1}", dt.Rows[i]["iID_MaDonVi"], dt.Rows[i]["sTen"])
                };
                list.Add(item);
            }
            dt.Dispose();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult get_GiaTri(String Truong, String GiaTri, String DSGiaTri)
        {
            if (Truong == "TenDonVi")
            {
                return get_GiaTriDonViCoTen(GiaTri);
            }

            else if (Truong == "sTenPhongBan")
            {
                return get_GiaTriPhongBan(GiaTri);
            }

            return null;
        }

        private JsonResult get_GiaTriPhongBan(String GiaTri)
        {
            Object item = new
            {
                value = "",
                label = ""
            };
            if (String.IsNullOrEmpty(GiaTri) == false)
            {
                List<Object> list = new List<Object>();
                String SQL = String.Format("SELECT TOP 10 sKyHieu, sTen FROM NS_PhongBan WHERE iTrangThai=1 AND sKyHieu LIKE @sKyHieu ORDER BY sKyHieu");
                SqlCommand cmd = new SqlCommand(SQL);
                cmd.Parameters.AddWithValue("@sKyHieu", GiaTri + "%");
                DataTable dt = Connection.GetDataTable(cmd);
                cmd.Dispose();
                if (dt.Rows.Count > 0)
                {
                    item = new
                    {
                        value = String.Format("{0}", dt.Rows[0]["sKyHieu"]),
                        label = String.Format("{0}", string.Format("{0} - {1}", dt.Rows[0]["sKyHieu"], dt.Rows[0]["sTen"]))
                    };
                }
                dt.Dispose();
            }
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        private JsonResult get_GiaTriDonViCoTen(String GiaTri, bool isOnlyName = false)
        {
            Object item = new
            {
                value = "",
                label = ""
            };
            if (String.IsNullOrEmpty(GiaTri) == false)
            {
                String MaND = User.Identity.Name;
                List<Object> list = new List<Object>();
                String DK = "iTrangThai=1 AND iNamLamViec_DonVi=@iNamLamViec AND ";
                String SQL = String.Format("SELECT TOP 2 iID_MaDonVi, sTen FROM NS_DonVi WHERE {0} iID_MaDonVi LIKE @iID_MaDonVi AND iNamLamViec_DonVi=@iNamLamViec ORDER BY iID_MaDonVi", DK);
                SqlCommand cmd = new SqlCommand(SQL);
                cmd.Parameters.AddWithValue("@iID_MaDonVi", GiaTri + "%");
                //loi cho chuc tc
                // cmd.Parameters.AddWithValue("@sMaNguoiDung", MaND);
                cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
                DataTable dt = Connection.GetDataTable(cmd);
                cmd.Dispose();
                if (dt.Rows.Count > 0)
                {
                    item = new
                    {
                        value = String.Format("{0}", dt.Rows[0]["iID_MaDonVi"]),
                        label = isOnlyName ? String.Format("{0}", dt.Rows[0]["sTen"]) : String.Format("{0} - {1}", dt.Rows[0]["iID_MaDonVi"], dt.Rows[0]["sTen"])
                    };
                }
                dt.Dispose();
            }
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SheetFrame(string id, string filter = null, bool isSaoChep = false)
        {
            var filters = filter == null ? Request.QueryString.ToDictionary() : JsonConvert.DeserializeObject<Dictionary<string, string>>(filter);
            var listModel = new List<NNSDuToanChiTietGetSoTienByDonViModel>();
            var ids = id.Split('_');
            var idDuToan = "";
            var idMaChungTu = "";
            var idNhiemVu = "";
            if (ids.Length >= 1)
            {
                idDuToan = ids[0];
            }
            if (ids.Length >= 2)
            {
                idMaChungTu = ids[1];
            }
            if (ids.Length >= 3)
            {
                idNhiemVu = ids[2];
            }
            if (ids.Length >= 4)
            {
                id = id.Remove(id.Length - 2, 2);
            }
            var dutoan = _qLNguonNSService.GetNNSGiaoDuToanByID(Guid.Parse(idDuToan));
            if (dutoan != null)
            {
                if (!string.IsNullOrEmpty(dutoan.sMaLoaiDuToan))
                {
                    //if (!string.IsNullOrEmpty(idNhiemVu) && !string.IsNullOrEmpty(idMaChungTu))
                    //if (!string.IsNullOrEmpty(idNhiemVu))
                    //{
                    //    isSaoChep = true;
                    //    listModel = _qLNguonNSService.GetDuLieuSoTienNoiDungChiTheoIdMaChungTu(Username, idNhiemVu);
                    //}
                    //else
                    //{
                    if (isSaoChep == true)
                    {
                        if (dutoan.sMaLoaiDuToan == "001")
                        {
                            listModel = _qLNguonNSService.GetDuLieuTinhTongTienDuToanDV(Username);
                        }
                        if (dutoan.sMaLoaiDuToan == "002")
                        {
                            listModel = _qLNguonNSService.GetDuLieuTinhTongTienDuToanDauNamTruocChuyenSang(Username);
                        }
                        if (dutoan.sMaLoaiDuToan == "003" && dutoan.sMaLoaiDuToan == "004")
                        {
                            listModel = _qLNguonNSService.GetDuLieuTinhTongTienDuToanDVTruocVaSauBaMuoiThangChin(Username, idDuToan);
                        }
                    }
                    //}
                }
            }

            var listNoiDungChiTongTien = _qLNguonNSService.GetTongTienDuToanChiTiet(idDuToan, Username, dutoan.sMaLoaiDuToan, idNhiemVu);
            var sheet = new NNSDuToan_SheetTable(idDuToan, filters, listModel, listNoiDungChiTongTien, isSaoChep, Username, idNhiemVu);
            var vm = new NNSDuToanChiTietViewModel();
            vm.Sheet = new SheetViewModel(
                    bang: sheet,
                    id: id,
                    filters: sheet.Filters,
                    urlPost: Url.Action("Save", "QLGiaoDuToanChoDV", new { area = "QLNguonNganSach" })
                    );
            vm.Sheet.AvaiableKeys = new Dictionary<string, string>();
            return View("_sheetFrame", vm);
        }

        [HttpPost]
        public JsonResult SaoChepDuToan(string id)
        {
            var duToan = _qLNguonNSService.GetNNSGiaoDuToanByID(Guid.Parse(id));
            var listModel = new List<NNSDuToanChiTietGetSoTienByDonViModel>();
            if (duToan != null)
            {
                if (!string.IsNullOrEmpty(duToan.sMaLoaiDuToan))
                {
                    if (duToan.sMaLoaiDuToan != "003" && duToan.sMaLoaiDuToan != "004")
                    {
                        listModel = _qLNguonNSService.GetDuLieuTinhTongTienDuToanDV(Username);
                    }
                    else
                    {
                        listModel = _qLNguonNSService.GetDuLieuTinhTongTienDuToanDVTruocVaSauBaMuoiThangChin(Username, id);
                    }
                }
            }
            return Json(new { listData = listModel });
        }

        [HttpPost]
        public ActionResult Save(SheetEditViewModel vm)
        {
            var ids = vm.Id.Split('_');
            var idDuToan = "";
            var idNhiemVu = "";
            bool isSaoChep = false;
            if (ids.Length >= 1)
            {
                idDuToan = ids[0];
            }
            if (ids.Length >= 3)
            {
                idNhiemVu = ids[2];
            }
            if (ids.Length >= 4)
            {
                isSaoChep = int.Parse(ids[3]) != 0;
                vm.Id = vm.Id.Remove(vm.Id.Length - 2, 2);
            }

            var listIdNhiemVuDelete = new List<NNS_DuToanChiTiet>();
            var rows = vm.Rows.Where(x => !x.IsParent).ToList();

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();

                #region crud
                if (isSaoChep)
                {
                    _qLNguonNSService.DeleteDuToanChiTietDuToanID(idDuToan);
                }

                if (rows.Count > 0)
                {
                    var columns = new NNSDuToan_SheetTable().Columns.Where(x => !x.IsReadonly);
                    var dutoan = _qLNguonNSService.GetNNSGiaoDuToanByID(Guid.Parse(idDuToan));
                    rows.ForEach(r =>
                    {
                        if (r.Columns.ContainsKey("iID_NhiemVu"))
                        {
                            if (r.Columns["iID_NhiemVu"] != null)
                            {
                                idNhiemVu = r.Columns["iID_NhiemVu"].ToString();
                            }
                        }

                        var values = new List<string>();
                        if (r.Id.IsNotEmpty() && r.Id != "_undefined")
                        {
                            if (r.Id.ToList("_", true) != null && r.Id.ToList("_", true).Any())
                            {
                                values.AddRange(r.Id.ToList("_", true));
                            }
                        }
                        var id_dutoanchitiet = values.FirstOrDefault();
                        var dictValue = new Dictionary<string, string>();
                        if (columns != null && columns.Any())
                        {
                            foreach (var item in columns)
                            {
                                if (r.Columns.ContainsKey(item.ColumnName))
                                {
                                    dictValue.Add(item.ColumnName, r.Columns[item.ColumnName]);
                                }
                            }
                        }
                        //var changes = r.Columns.Where(c => columns.Any(x => x.ColumnName == c.Key));

                        if (!string.IsNullOrWhiteSpace(id_dutoanchitiet) && r.IsDeleted)
                        {
                            #region delete

                            var entity = conn.Get<NNS_DuToanChiTiet>(id_dutoanchitiet, trans);
                            if (entity != null)
                            {
                                listIdNhiemVuDelete.Add(entity);
                                conn.Delete(entity, trans);
                            }

                            #endregion
                        }
                        else if (dictValue.Any())
                        {
                            var isNew = string.IsNullOrWhiteSpace(id_dutoanchitiet);
                            if (isNew)
                            {
                                #region create
                                if (!string.IsNullOrEmpty(dictValue["iID_MaDonVi"]) || !string.IsNullOrEmpty(dictValue["sMaPhongBan"])
                                || (!string.IsNullOrEmpty(dictValue["SoTien"]) && dictValue["SoTien"] != "0"))
                                {
                                    var entity = new NNS_DuToanChiTiet()
                                    {
                                        //iID_DuToanChiTiet = Guid.NewGuid(),
                                        iID_DuToan = Guid.Parse(idDuToan),
                                        iNamLamViec = dutoan.iNamLamViec,
                                        iID_MaNamNganSach = dutoan.iID_MaNamNganSach,
                                        iID_MaNguonNganSach = dutoan.iID_MaNguonNganSach,

                                        dNgayTao = DateTime.Now,
                                        sID_MaNguoiDungTao = Username,

                                    };
                                    entity.Mapper<NNS_DuToanChiTiet>(dictValue);
                                    if(entity.TenDonVi.IndexOf(entity.iID_MaDonVi) > -1)
                                {
                                    string sTenDonVi = entity.TenDonVi.Substring(entity.iID_MaDonVi.Length + 2, entity.TenDonVi.Length - (entity.iID_MaDonVi.Length + 2)).Trim();
                                    entity.TenDonVi = sTenDonVi;
                                }
                                
                                if(entity.sTenPhongBan.IndexOf(entity.sMaPhongBan) > -1)
                                {
                                    string sTenPhongBan = entity.sTenPhongBan.Substring(entity.sMaPhongBan.Length + 2, entity.sTenPhongBan.Length - (entity.sMaPhongBan.Length + 2)).Trim();
                                    entity.sTenPhongBan = sTenPhongBan;
                                }

                                    conn.Insert(entity, trans);
                                }
                                #endregion
                            }
                            else
                            {
                                #region edit
                                var entity = conn.Get<NNS_DuToanChiTiet>(id_dutoanchitiet, trans);
                                entity.dNgaySua = DateTime.Now;
                                entity.sID_MaNguoiDungSua = Username;
                                entity.iSoLanSua = (!entity.iSoLanSua.HasValue ? 0 : entity.iSoLanSua) + 1;
                                entity.sIPSua = Request.UserHostAddress;

                                entity.Mapper<NNS_DuToanChiTiet>(dictValue);

                                entity.Mapper<NNS_DuToanChiTiet>(dictValue);
                                if(entity.TenDonVi.IndexOf(entity.iID_MaDonVi + " -") > -1)
                                {
                                    string sTenDonVi = entity.TenDonVi.Substring(entity.iID_MaDonVi.Length + 2, entity.TenDonVi.Length - (entity.iID_MaDonVi.Length + 2)).Trim();
                                    entity.TenDonVi = sTenDonVi;
                                }
                                
                                if(entity.sTenPhongBan.IndexOf(entity.sMaPhongBan + " -") > -1)
                                {
                                    string sTenPhongBan = entity.sTenPhongBan.Substring(entity.sMaPhongBan.Length + 2, entity.sTenPhongBan.Length - (entity.sMaPhongBan.Length + 2)).Trim();
                                    entity.sTenPhongBan = sTenPhongBan;
                                }

                                conn.Update(entity, trans);

                                #endregion
                            }
                        }
                    });

                    #endregion

                    // commit to db
                    trans.Commit();
                }

                // update tong so tien du toan
                _qLNguonNSService.UpdateSumDuToan(idDuToan);
                if (listIdNhiemVuDelete != null && listIdNhiemVuDelete.Any())
                {
                    //_qLNguonNSService.DeleteIDNhiemVu(listIdNhiemVuDelete);
                }

            }

            return RedirectToAction("SheetFrame", new { vm.Id, vm.Option, filter = vm.FiltersString });
        }
        #endregion
    }
}