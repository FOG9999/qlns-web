using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Viettel;
using Viettel.Domain.DomainModel;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Models;

namespace VIETTEL.Areas.CapPhat.Models
{
    public class rptCapPhat_ThongTriViewModel
    {
        public CP_CapPhat Entity { get; set; }
        public string iID_MaPhongBan { get; set; }
        public SelectList LoaiCapPhatList { get; set; }
        public SelectList LoaiThongTriList { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
    }
}

namespace VIETTEL.Areas.CapPhat.Controllers.Reports
{
    public class rptCapPhatReportController : FlexcelReportController
    {
        protected override string ViewFolder()
        {
            return @"~/Areas\CapPhat\Views\Reports\";
        }
    }

    public class rptCapPhat_ThongTriController : rptCapPhatReportController
    {
        private readonly ICapPhatService _capPhatService = CapPhatService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private string _filePath_ng = "/Report_ExcelFrom/CapPhat/rptCapPhat_ThongTri_ChiTiet_Ng.xls";
        private string _filePath_m = "/Report_ExcelFrom/CapPhat/rptCapPhat_ThongTri_ChiTiet_M.xls";
        private string _filePath_tm = "/Report_ExcelFrom/CapPhat/rptCapPhat_ThongTri_ChiTiet_TM.xls";

        // GET: CapPhat/rptCapPhat_ThongTri
        public ActionResult Index(string id)
        {
            var entity = _capPhatService.GetChungTu(Guid.Parse(id));
            var vm = new Models.rptCapPhat_ThongTriViewModel()
            {
                Entity = entity,
                Date = DateTime.Now.Date,

                LoaiCapPhatList = _capPhatService.GetLoaiCapPhat().ToSelectList(),
                LoaiThongTriList = _capPhatService.GetLoaiThongTri(entity.iLoai.GetValueOrDefault(), entity.sDSLNS).ToSelectList("loaiThongTri", "TenThongTri"),
            };

            return View(ViewFileName(), vm);
        }

        #region public methods

        public JsonResult Ds_LNS(
           string id_chungtu,
           string loaiThongTri)
        {
            var data = _capPhatService.GetLnsThongTri(id_chungtu, loaiThongTri);
            var vm = new ChecklistModel("LNS", data.ToSelectList("sLNS", "TenHT"));

            return ToCheckboxList(vm);
        }

        public JsonResult Ds_DonVi(
            string id_chungtu,
            string loaiThongTri,
            string lns)
        {
            var data = _capPhatService.GetDonViThongTri(id_chungtu, loaiThongTri, lns);
            var vm = new ChecklistModel("DonVi", data.ToSelectList("iID_MaDonVi", "TenHT"));

            return ToCheckboxList(vm);
        }

        public JsonResult GhiChu(string id_chungtu, string id_donvi, string loaiThongTri)
        {
            var sTen = $"thongtri_cp_{PhienLamViec.iNamLamViec}_{id_chungtu}_{id_donvi}_{loaiThongTri}";
            var ghichu = _capPhatService.GetGhiChuThongTri(Username, sTen, id_donvi);

            // lay noi dung cua chung tu thanh ghi chu neu chua sua.
            if (ghichu.IsEmpty())
            {
                ghichu = _capPhatService.GetChungTu(new Guid(id_chungtu)).sNoiDung;
            }
            return Json(ghichu, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GhiChu_Update(string id_chungtu, string id_donvi, string loaiThongTri, string ghichu)
        {
            var sTen = $"thongtri_cp_{PhienLamViec.iNamLamViec}_{id_chungtu}_{id_donvi}_{loaiThongTri}";
            var success = _capPhatService.UpdateGhiChuThongTri(Username, sTen, id_donvi, ghichu);
            return Json(success, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Print


        private CP_CapPhat _entity;
        private int loaiCapPhat;
        private int loaiThongTri;
        private int iIn;
        private int iChuKy;
        private string lns;
        private string id_donvi;
        private DateTime date;

        public ActionResult Print(
            string id_chungtu,
            int loaiCapPhat,
            int loaiThongTri,
            string lns,
            string id_donvi,
            //string loai,
            int iChuky,
            int iIn,
            string ghiChu,
            string date,
            string ext)
        {
            this._entity = _capPhatService.GetChungTu(new Guid(id_chungtu));
            this.loaiCapPhat = loaiCapPhat;
            this.loaiThongTri = loaiThongTri;
            this.lns = lns;
            this.id_donvi = id_donvi;
            //this.loai = loai;
            this.iChuKy = iChuky;
            //this.thualenh = thualenh;
            this.date = Convert.ToDateTime(date);
            this.iIn = iIn;

            var xls = createReport();

            return Print(xls, ext);
        }

        /// <summary>
        /// Tạo báo cáo
        /// </summary>
        /// <returns></returns>
        private ExcelFile createReport()
        {
            FlexCelReport fr = new FlexCelReport();
            loadData(fr);

            //fr.SetValue("BoQuocPhong", ReportModels.CauHinhTenDonViSuDung(1, Username));
            //fr.SetValue("QuanKhu", ReportModels.CauHinhTenDonViSuDung(2, Username));
            //fr.SetValue("NgayThang", "Ngày " + date.Day + " tháng " + date.Month + " năm " + date.Year);


            //fr.SetValue("Nam", PhienLamViec.iNamLamViec);
            //fr.SetValue("NgayLap", "tháng " + Thang + " năm " + PhienLamViec.iNamLamViec);
            //fr.SetValue("LoaiCapPhat", capPhat.Trim());
            //fr.SetValue("soChuky", chuky);
            //fr.SetValue("soTL", thualenh);

            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(getFilePath(_entity.sLoai)));

            fr.SetValue
                (new
                {
                    sTenDonVi = _nganSachService.GetDonVi(PhienLamViec.iNamLamViec, id_donvi).sTen,
                    NgayThang = $"Ngày {date.Day} tháng {date.Month} năm {date.Year}",
                    LoaiCapPhat = loaiCapPhat == -1 ?
                        $"Cấp {_capPhatService.GetLoaiThongTri()[loaiThongTri.ToString()].ToLower()}" :
                        $"{_capPhatService.GetLoaiCapPhat()[loaiCapPhat.ToString()]} {_capPhatService.GetLoaiThongTri()[loaiThongTri.ToString()].ToLower()}",
                    ngayLap = $"tháng {(date.Year > PhienLamViec.NamLamViec ? "12" : date.Month.ToString())} năm {_entity.iNamLamViec}",
                    iIn,
                    MTT = L("ThongTri"),
                    iChuKy,
                })
                .UseChuKy()
                .UseChuKyForController(this.ControllerName())
                .UseForm(this)
                .Run(xls);

            return xls;
        }

        private void loadData(FlexCelReport fr)
        {
            var data = _capPhatService.CapPhatThongTriChiTiet(PhienLamViec.iNamLamViec, lns, id_donvi, _entity.iID_MaCapPhat.ToString());
            // giảm cấp
            var isGiamCap = loaiCapPhat == 2 || loaiCapPhat == 3;
            if (isGiamCap)
            {
                data.AsEnumerable()
                       .ToList()
                       .ForEach(row =>
                       {
                           row["rTuChi"] = -Convert.ToDouble(row["rTuChi"]);
                       });
            }


            FillDataTable(fr, data);

            //Lấy giá trị ghi chú của đơn vị     
            var sGhiChu = getGhiChu(id_donvi);
            //fr.AddTable("dtEmpty", getGhiChuDatatableUp(sGhiChu, data.Rows.Count > 8 ? 0 : 8 - data.Rows.Count));
            fr.AddTable("dtEmpty", getGhiChuDatatableUp(sGhiChu));
            fr.SetValue(new
            {
                Tien = isGiamCap ? $"Giảm {data.Sum("rTuChi").ToStringMoney().ToLower()}" : data.Sum("rTuChi").ToStringMoney(),
                sGhiChu = iIn == 1 ? "" : sGhiChu,
            });
        }

        private string getFilePath(string loai)
        {
            if (loai == "sM") return _filePath_m;
            if (loai == "sTM") return _filePath_tm;

            return _filePath_ng;
        }


        #region ghichu

        private DataTable getGhiChuDatatableUp(string ghichu, int maxLine = 0)
        {
            var dt = new DataTable();
            dt.Columns.Add("sGhiChu");

            var words = ghichu.Split(new string[] { "\n", "&#10;" }, StringSplitOptions.None)
                .Where(x => !string.IsNullOrWhiteSpace(x));

            words.ToList().ForEach(w =>
            {
                var lines = w.Replace("  ", " ").Split(new char[] { ' ' })
                .ToList()
                .ChunkByChar();

                lines.ForEach(x =>
                {
                    var row = dt.NewRow();
                    row[0] = x.Join(" ");
                    dt.Rows.Add(row);
                });
            });

            if (dt.Rows.Count < maxLine)
            {
                for (int i = dt.Rows.Count; i < maxLine; i++)
                {
                    var row = dt.NewRow();
                    dt.Rows.Add(row);
                }
            }

            return dt;
        }

        private string getGhiChu(string iID_MaDonVi)
        {
            var sTen = $"thongtri_cp_{PhienLamViec.iNamLamViec}_{_entity.iID_MaCapPhat.ToString()}_{iID_MaDonVi}_{loaiThongTri}";
            var ghichu = _capPhatService.GetGhiChuThongTri(Username, sTen, iID_MaDonVi);

            return ghichu;
        }

        #endregion
        #endregion

    }
}
