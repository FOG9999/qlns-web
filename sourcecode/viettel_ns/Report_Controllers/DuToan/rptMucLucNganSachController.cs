using System;
using System.Web.Mvc;
using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System.Data;
using System.Data.SqlClient;
using DomainModel;
using VIETTEL.Models;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Helpers;
using Viettel.Data;
using Viettel.Services;
using System.Web.UI.WebControls;

namespace VIETTEL.Models
{
    public class rptMucLucNganSachViewModel
    {

        public string iNamLamViec { get; set; }
        public string iID_MaPhongBan { get; set; }
        public SelectList LoaiInList { get; set; }
        public ChecklistModel loaiNSList { get; set; }

    }
}

namespace VIETTEL.Report_Controllers.DuToan
{
    public class rptMucLucNganSachController : FlexcelReportController
    {

        #region var def
        public string _viewPath = "~/Views/Report_Views/MucLucNganSach/";
        private const String sFilePath = "/Report_ExcelFrom/MucLucNganSach/rptMucLucNganSach.xls";
        private const String sFilePathLK = "/Report_ExcelFrom/MucLucNganSach/rptMucLucNganSachLK.xls";
        private const String sFilePathFull = "/Report_ExcelFrom/MucLucNganSach/rptMucLucNganSachFull.xls";

        private string _filePath;
        private string iID_MaPhongBan;
        private string option;
        private string dsLNS;

        public ActionResult Index()
        {
            var view = _viewPath + this.ControllerName() + ".cshtml";
            var iNamLamViec = PhienLamViec.iNamLamViec;
            var iID_MaPhongBan = PhienLamViec.iID_MaPhongBan;
            var dtLoaiIn = SharedModels.getDSIN();
            var dtLNS = Ds_LNS();

            var vm = new rptMucLucNganSachViewModel
            {
                iNamLamViec = iNamLamViec,
                iID_MaPhongBan = iID_MaPhongBan,
                loaiNSList = new ChecklistModel("LNS", dtLNS.ToSelectList("sLNS", "TenLNS")),
                LoaiInList = dtLoaiIn.ToSelectList("LoaiIn", "sTen")
            };

            return View(view, vm);
        }
        #endregion

        #region public methods

        public ActionResult Print(
            string iID_MaPhongBan,
            string dsLNS,
            string option,
            string ext)
        {

            switch (option)
            {
                case "All":
                    this._filePath = sFilePathFull;
                    break;
                case "Short":
                    this._filePath = sFilePath;
                    break;
                default:
                    this._filePath = sFilePathLK;
                    break;
            }

            this.option = option;
            this.iID_MaPhongBan = iID_MaPhongBan;
            this.dsLNS = dsLNS;

            var xls = createReport();
            return Print(xls, ext);
        }

        /// <summary>
        /// Tạo báo cáo
        /// </summary>
        /// <returns></returns>
        public ExcelFile createReport()
        {
            FlexCelReport fr = new FlexCelReport();
            LoadData(fr);

            fr.SetValue("Cap1", ReportModels.CauHinhTenDonViSuDung(1, Username));
            fr.SetValue("Cap2", ReportModels.CauHinhTenDonViSuDung(2, Username));
            fr.SetValue("Ngay", ReportModels.Ngay_Thang_Nam_HienTai());
            fr.SetValue("Nam", PhienLamViec.iNamLamViec);
            if (iID_MaPhongBan != "-1")
                fr.SetValue("sTenDonVi", "B " + iID_MaPhongBan);
            else
            {
                fr.SetValue("sTenDonVi", "");
            }

            fr.UseChuKy(Username)
              .UseChuKyForController(this.ControllerName());

            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(this._filePath));

            fr.Run(xls);

            return xls;
        }

        /// <summary>
        /// Lấy dữ liệu data
        /// </summary>
        /// <returns></returns>
        public DataTable dt_MucLucNganSach()
        {
            DataTable dt = new DataTable();

            #region definition input

            var sql = FileHelpers.GetSqlQuery("rptMucLucNganSach.sql");

            #endregion

            #region dieu kien  

            if (option == "LoaiKhoan")
            {
                sql = sql.Replace("@@select", " sK");
            }
            else
            {
                sql = sql.Replace("@@select", @" sK
                                                , sM
                                                , sTM
                                                , sTTM
                                                , sNG
                                                , sMoTa");
            }

            var lnsArr = dsLNS.Split(',');

            sql = sql.Replace("@@sLNS", dsLNS.ToParamLikeStartWith("sLNS"));

            int iNamLamViec = Convert.ToInt32(PhienLamViec.iNamLamViec);
            if (iNamLamViec < 2017) iNamLamViec = 2017;

            #endregion

            #region get data

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan.ToParamString());
                cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                if (option == "LoaiKhoan")
                {
                    cmd.Parameters.AddWithValue("@sM", "");
                }
                else
                {
                    cmd.Parameters.AddWithValue("@sM", DBNull.Value);
                }

                dt = cmd.GetTable();
            }
            #endregion

            if (option == "LoaiKhoan")
            {
                dt.Columns.Add("sMoTa", typeof(String));
                foreach (DataRow dtr in dt.Rows)
                {
                    dtr["sMoTa"] = LayMoTa(dtr["sLNS"].ToString(), dtr["sL"].ToString(), dtr["sK"].ToString());
                }
            }
            else
            {
                dt.Columns.Add("sLNS_temp", typeof(String));
                dt.Columns.Add("sL_temp", typeof(String));
                dt.Columns.Add("sK_temp", typeof(String));
                dt.Columns.Add("sM_temp", typeof(String));
                dt.Columns.Add("sTM_temp", typeof(String));
                for (int i = 0; i < dt.Rows.Count - 1; i++)
                {
                    if (i == 0)
                    {
                        dt.Rows[i]["sLNS_temp"] = dt.Rows[i]["sLNS"];
                        dt.Rows[i]["sL_temp"] = dt.Rows[i]["sL"];
                        dt.Rows[i]["sK_temp"] = dt.Rows[i]["sK"];
                        dt.Rows[i]["sM_temp"] = dt.Rows[i]["sM"];
                        dt.Rows[i]["sTM_temp"] = dt.Rows[i]["sTM"];
                    }
                    if (((i - 41) % 46) == 0)
                    {
                        dt.Rows[i + 1]["sLNS_temp"] = dt.Rows[i + 1]["sLNS"];
                        dt.Rows[i + 1]["sL_temp"] = dt.Rows[i + 1]["sL"];
                        dt.Rows[i + 1]["sK_temp"] = dt.Rows[i + 1]["sK"];
                        dt.Rows[i + 1]["sM_temp"] = dt.Rows[i + 1]["sM"];
                        dt.Rows[i + 1]["sTM_temp"] = dt.Rows[i + 1]["sTM"];
                    }
                    else
                    {
                        if (dt.Rows[i]["sLNS"].ToString() == dt.Rows[i + 1]["sLNS"].ToString())
                        {
                            dt.Rows[i + 1]["sLNS_temp"] = "";
                        }
                        else
                        {
                            dt.Rows[i + 1]["sLNS_temp"] = dt.Rows[i + 1]["sLNS"];

                        }
                        if (dt.Rows[i]["sL"].ToString() == dt.Rows[i + 1]["sL"].ToString())
                        {

                            dt.Rows[i + 1]["sL_temp"] = "";

                        }
                        else
                        {

                            dt.Rows[i + 1]["sL_temp"] = dt.Rows[i + 1]["sL"];

                        }
                        if (dt.Rows[i]["sK"].ToString() == dt.Rows[i + 1]["sK"].ToString())
                        {
                            dt.Rows[i + 1]["sK_temp"] = "";
                        }
                        else
                        {
                            dt.Rows[i + 1]["sK_temp"] = dt.Rows[i + 1]["sK"];
                        }
                        if (dt.Rows[i]["sM"].ToString() == dt.Rows[i + 1]["sM"].ToString())
                        {
                            dt.Rows[i + 1]["sM_temp"] = "";
                        }
                        else
                        {
                            dt.Rows[i + 1]["sM_temp"] = dt.Rows[i + 1]["sM"];
                        }
                        if (dt.Rows[i]["sTM"].ToString() == dt.Rows[i + 1]["sTM"].ToString())
                        {
                            dt.Rows[i + 1]["sTM_temp"] = "";
                        }
                        else
                        {
                            dt.Rows[i + 1]["sTM_temp"] = dt.Rows[i + 1]["sTM"];
                        }
                    }
                }
            }
            return dt;
        }

        public String LayMoTa(string sLNS, string sL = null, string sK = null)
        {
            #region definition input

            var sql = FileHelpers.GetSqlQuery("rptMucLucNganSach_LayMoTa.sql");


            #endregion

            #region dieu kien

            int iNamLamViec = Convert.ToInt32(PhienLamViec.iNamLamViec);
            if (iNamLamViec < 2017) iNamLamViec = 2017;

            #endregion

            #region get data

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@iNamLamViec", PhienLamViec.iNamLamViec);
                cmd.Parameters.AddWithValue("@sLNS", sLNS);
                cmd.Parameters.AddWithValue("@sL", sL);
                cmd.Parameters.AddWithValue("@sK", sK);
                if (option == "LoaiKhoan")
                {
                    cmd.Parameters.AddWithValue("@sM", "");
                }
                else
                {
                    cmd.Parameters.AddWithValue("@sM", DBNull.Value);
                }

                return cmd.GetValue();
            }
            #endregion
        }

        public DataTable Ds_LNS()
        {
            DataTable dt = new DataTable();

            #region definition input

            var sql = FileHelpers.GetSqlQuery("rptMucLucNganSach_PB_LNS.sql");


            #endregion

            #region dieu kien

            int iNamLamViec = Convert.ToInt32(PhienLamViec.iNamLamViec);
            if (iNamLamViec < 2017) iNamLamViec = 2017;

            #endregion

            #region get data

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", PhienLamViec.iID_MaPhongBan);
                cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);

                dt = cmd.GetTable();
            }

            if (PhienLamViec.iID_MaPhongBan == "02" || PhienLamViec.iID_MaPhongBan == "11")
            {
                DataRow dtr = dt.NewRow();
                dtr["sLNS"] = "1";
                dtr["TenLNS"] = "1 - Ngân sách quốc phòng";
                dt.Rows.Add(dtr);
                dtr = dt.NewRow();
                dtr["sLNS"] = "2";
                dtr["TenLNS"] = "2 - Ngân sách nhà nước";
                dt.Rows.Add(dtr);
                dtr = dt.NewRow();
                dtr["sLNS"] = "3";
                dtr["TenLNS"] = "3 - Chi đặc biệt";
                dt.Rows.Add(dtr);
                dtr = dt.NewRow();
                dtr["sLNS"] = "4";
                dtr["TenLNS"] = "4 - Chi bằng nguồn kinh phí khác";
                dt.Rows.Add(dtr);
                dtr = dt.NewRow();
                dtr["sLNS"] = "8";
                dtr["TenLNS"] = "8 - Thu nộp";
                dt.Rows.Add(dtr);
                DataView dv = dt.DefaultView;
                dv.Sort = "sLNS";

                return dv.ToTable();
            }
            else
            {
                return dt;
            }
            #endregion
        }

        #endregion

        #region private methods
        /// <summary>
        /// Đổ dư liệu xuống báo cáo
        /// </summary>
        /// <param name="fr"></param>
        private void LoadData(FlexCelReport fr)
        {
            DataTable data = dt_MucLucNganSach();
            if (option == "LoaiKhoan")
            {
                data.TableName = "ChiTietLK";
                fr.AddTable("ChiTietLK", data);
            }
            else
            {
                data.TableName = "ChiTiet";
                fr.AddTable("ChiTiet", data);
            }
            data.Dispose();
        }


        #endregion            
    }
}