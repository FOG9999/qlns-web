using FlexCel.Core;
using FlexCel.Render;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using Viettel.Data;
using Viettel.Services;
using VIETTEL.Areas.SKT.Models;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Models;

namespace VIETTEL.Areas.SKT.Controllers
{
    public class rptNCSKT_TH04ViewModel
    {
        public SelectList NganhList { get; set; }
        public SelectList PhongBanList { get; set; }
        public SelectList DonViList { get; set; }
    }
    public class rptNCSKT_TH04Controller : FlexcelReportController
    {
        public string _viewPath = "~/Areas/SKT/Views/Reports/TongHop/";
        private const string _filePath_bc = "~/Areas/SKT/FlexcelForm/TongHop/rptNCSKT_TH04/rptNCSKT_TH04_bc.xls";
        private const string _filePath = "~/Areas/SKT/FlexcelForm/TongHop/rptNCSKT_TH04/rptNCSKT_TH04.xls";
        private const string _filePath_dv = "~/Areas/SKT/FlexcelForm/TongHop/rptNCSKT_TH04/rptNCSKT_TH04_dv.xls";

        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly ISKTService _SKTService = SKTService.Default;
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private int _dvt;
        private int _loai;
        private int _loaiBC;
        private string _nganh;
        private string _donvi;
        private string _phongban;

        public ActionResult Index()
        {
            if (User.Identity.Name.EndsWith("b2"))
            {
                var dNganh = _nganSachService.ChuyenNganh_GetAll(PhienLamViec.iNamLamViec);
                var phongbanList = (PhienLamViec.iID_MaPhongBan != "02" && PhienLamViec.iID_MaPhongBan != "11") ? new SelectList(_nganSachService.GetBql(Username), "sKyHieu", "sMoTa") : _nganSachService.GetPhongBanQuanLyNS("06,07,08,10,17").ToSelectList("iID_MaPhongBan", "sMoTa", "-1", "-- Tất cả phòng ban --"); ;
                var donviList = _nganSachService.GetDonviListByUser(Username, PhienLamViec.iNamLamViec);

                var vm = new rptNCSKT_TH04ViewModel()
                {
                    PhongBanList = phongbanList,
                    NganhList = dNganh.ToSelectList("Id","Nganh", "-1","-- Chọn chuyên ngành --"),
                    DonViList = donviList.ToSelectList("iID_MaDonVi", "sMoTa", "-1", "-- Chọn đơn vị --"),
                };

                var view = _viewPath + "rptNCSKT_TH04.cshtml";

                return View(view, vm);
            }
            else
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }
        }
        public JsonResult Ds_DonVi(string phongban)
        {
            var data = _SKTService.Get_DonVi_ExistData(PhienLamViec.iNamLamViec, 4, PhienLamViec.iID_MaDonVi, phongban != "-1" ? phongban : PhienLamViec.iID_MaPhongBan, phongban);
            var vm = new ChecklistModel("Id_DonVi", data.ToSelectList("Id", "Ten", "-1", "-- Chọn đơn vị --"));

            return ToDropdownList(vm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id_phongban"></param>
        /// <param name="id_donvi"></param>
        /// <param name="loai">1: NSSD, 2: NSBĐ ngành</param>
        /// <param name="ext"></param>
        /// <param name="dvt"></param>
        /// <returns></returns>
        public ActionResult Print(
            string nganh,
            string donvi,
            string phongban,
            int loai = 1,
            int loaiBC = 1,
            string ext = "pdf",
            int dvt = 1000)
        {
            // chi cho phep in donvi duoc quan ly           
            this._dvt = dvt;
            this._loai = loai;
            this._loaiBC = loaiBC;
            this._nganh = nganh;
            this._donvi = donvi;
            this._phongban = phongban;
            var xls = createReport();

            if (xls.ContainsKey(false))
            {
                return new EmptyResult();
            }
            else
            {
                var filename = $"Báo_cáo_TH04_{DateTime.Now.GetTimeStamp()}.{ext}";
                return Print(xls[true], ext, filename);
            }

        }
        
        #region private methods

        /// <summary>
        /// Tạo báo cáo
        /// </summary>
        /// <returns></returns>
        private Dictionary<bool, ExcelFile> createReport()
        {
            var fr = new FlexCelReport();
            var xls = new XlsFile(true);

            var check = loadData(fr);

            if (check.ContainsKey(false))
            {
                return new Dictionary<bool, ExcelFile>
                {
                    {false, xls}
                };
            }
            else
            {

                xls.Open(Server.MapPath(_loai == 2 ? (_loaiBC == 200 ? _filePath_bc : _filePath) : _filePath_dv));

                var dictieude = _SKTService.GetTieuDeDuLieuNamTruoc(PhienLamViec.NamLamViec);

                fr.SetValue(new
                {
                    headerr = $"Đơn vị tính: {_dvt.ToHeaderMoney()}",
                    namn = PhienLamViec.NamLamViec - 1,
                    nam = PhienLamViec.NamLamViec,
                    tieude1 = dictieude[1],
                    tieude2 = dictieude[2],
                    tieude3 = dictieude[3],
                    phongban = _phongban == "-1" ? "" : _nganSachService.GetPhongBanById(_phongban).sMoTa,
                });

                fr.UseCommonValue()
                  .UseChuKyForController(this.ControllerName())
                  .UseForm(this).Run(xls);
              
                if (!_loaiBC.ToString().StartsWith("20"))
                {
                    if (_loaiBC == 22)
                    {
                        xls.SetCellValue(1, 2, $"TỔNG HỢP SỐ KIỂM TRA NSSD NĂM {PhienLamViec.iNamLamViec}");
                    }
                    else if (_loaiBC == 23)
                    {
                        xls.SetCellValue(1, 2, $"TỔNG HỢP SỐ KIỂM TRA NSBĐ NĂM {PhienLamViec.iNamLamViec}");
                    }                    
                    xls.SetCellValue(5, 3, "SỐ CỤC ĐỀ NGHỊ");
                    xls.SetCellValue(5, 4, "SỐ NGÀNH THẨM ĐỊNH");
                }
                else if (_loaiBC == 201)
                {
                    xls.SetCellValue(1, 2, $"CHI TIẾT SỐ KIỂM TRA NĂM {PhienLamViec.iNamLamViec}");
                }
                else if (_loaiBC == 202)
                {
                    xls.SetCellValue(1, 2, $"CHI TIẾT TĂNG GIẢM SỐ KIỂM TRA NĂM {PhienLamViec.iNamLamViec}");
                }

                var file = xls.TotalPageCount();
                if (file > 1)
                {
                    xls.ClearDiffFirstPage();
                }
                return new Dictionary<bool, ExcelFile>
                {
                    {true, xls}
                };
            }
        }

        private Dictionary<bool, FlexCelReport> loadData(FlexCelReport fr)
        {
            var DonVi = getDataSet().Tables[0];
            if (DonVi.Rows.Count == 0)
            {
                return new Dictionary<bool, FlexCelReport>
                {
                    { false, fr }
                };
            }
            else
            {
                var Muc = DonVi.SelectDistinct("ChiTiet", "X1,X2,X3,X4,KyHieu,MoTa,STTBC");
                var view = Muc.AsDataView();
                view.Sort = "STTBC";
                Muc = view.ToTable();
                var PhongBan = DonVi.SelectDistinct("PhongBan", "X1,X2,X3,X4,KyHieu,Id_PhongBan,TenPhongBan");          
                _SKTService.FillDataTable_NC(fr, Muc);
                fr.AddTable("Muc", DonVi);
                fr.AddTable("PhongBan", PhongBan);
                fr.AddRelationship("ChiTiet", "PhongBan", "X1,X2,X3,X4,KyHieu".Split(','), "X1,X2,X3,X4,KyHieu".Split(','));
                fr.AddRelationship("PhongBan", "Muc", "X1,X2,X3,X4,KyHieu,Id_PhongBan".Split(','), "X1,X2,X3,X4,KyHieu,Id_PhongBan".Split(','));

                var DoanhNghiep = getDataSet().Tables[1];
                fr.AddTable("DN", DoanhNghiep);
                return new Dictionary<bool, FlexCelReport>
                {
                    { true, fr }
                };
            }
        }

        private DataSet getDataSet()
        {
            using (var conn = _connectionFactory.GetConnection())
            { 
                var cmd = new SqlCommand("sp_ncskt_report_th04", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                return cmd.GetDataset(new
                {
                    nganh = _nganh.ToParamString(),
                    phongban = _phongban.ToParamString(),
                    donvi = _donvi.ToParamString(),
                    nam = PhienLamViec.NamLamViec,
                    loai = _loaiBC.ToParamString(),
                    dvt = _dvt.ToParamString(),
                });
            }
        }        
        #endregion
    }
}
