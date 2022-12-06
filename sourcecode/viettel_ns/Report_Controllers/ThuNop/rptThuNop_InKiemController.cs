using System;
using System.Web.Mvc;
using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System.Data;
using System.Data.SqlClient;
using VIETTEL.Models;
using VIETTEL.Controllers;
using Viettel.Services;
using VIETTEL.Helpers;
using Viettel.Data;
using VIETTEL.Flexcel;

namespace VIETTEL.Report_Controllers.ThuNop {
    public class rptThuNop_InKiemController : FlexcelReportController 
    {
        //
        // GET: /rptThuNop_InKiem/
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private const string sFilePath = "/Report_ExcelFrom/ThuNop/rptThuNop_InKiem.xls";

        #region view pdf and download excel

        /// <summary>
        /// Hàm view pdf hoặc xuất file excel
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        /// 
        public ActionResult Print(string ext,
            string maND, 
            string iID_MaPhongBan = null,
            string iID_MaDonVi = null) {

            HamChung.Language();
            String sDuongDan = sFilePath;
            string nam = PhienLamViec.iNamLamViec;

            var xls = createReport(nam, sDuongDan, maND, iID_MaPhongBan, iID_MaDonVi);

            return Print(xls, ext);
        }

        #endregion

        #region methods
        /// < mary>
        /// Hàm lấy dữ liệu ruột data 
        /// </ mary>
        /// <param name="nam"></param>
        /// <param name="maND"></param>
        /// <param name="iID_MaPhongBan"></param>
        /// <param name="iID_MaDonVi"></param>
        /// <returns></returns>
        private DataTable getThuNop_InKiem(string nam, string iID_MaPhongBan, string iID_MaDonVi)
        {
            #region definition input
            var sql = FileHelpers.GetSqlQuery("rptThuNop_InKiem.sql"); ;            
            #endregion

            #region get data

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn)) {
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan.ToParamString());
                cmd.Parameters.AddWithValue("@iNamLamViec", nam);
                cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi.ToParamString());

                var dt = cmd.GetTable();
                return dt;
            }

            #endregion      
        }

        /// < mary>
        /// Tạo báo cáo
        /// </ mary>
        /// <param name="nam"></param>
        /// <param name="path"></param>
        /// <param name="maND"></param>
        /// <param name="iID_MaPhongBan"></param>
        /// <param name="iID_MaDonVi"></param>
        /// <returns></returns>
        private ExcelFile createReport(string nam, string path, string maND, string iID_MaPhongBan, string iID_MaDonVi)
        {
            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(sFilePath));
            var fr = new FlexCelReport();

            var data = getThuNop_InKiem(nam, iID_MaPhongBan, iID_MaDonVi);

            FillDataTableLH(fr, data);

            String DonVi = iID_MaDonVi + " - " + DonViModels.Get_TenDonVi(iID_MaDonVi,maND);
            
            fr.SetValue("BoQuocPhong", ReportModels.CauHinhTenDonViSuDung(1,maND));
            fr.SetValue("QuanKhu", ReportModels.CauHinhTenDonViSuDung(2,maND));
            fr.SetValue("NgayThang", ReportModels.Ngay_Thang_Nam_HienTai());
            fr.SetValue("Nam", nam);
            fr.SetValue("PB", iID_MaPhongBan);
            fr.SetValue("DonVi", DonVi);
            fr.UseCommonValue()
              .UseForm(this)
              .Run(xls);

            return xls;
        }

        /// < mary>
        /// Đổ dư liệu xuống báo cáo
        /// </ mary>
        /// <param name="fr"></param>
        /// <param name="data"></param>
        protected virtual FlexCelReport FillDataTableLH(FlexCelReport fr, DataTable data) {            

            fr.AddTable("ChiTiet", data, TDisposeMode.DisposeAfterRun);            

            DataTable dtLoaiHinh = HamChung.SelectDistinct("LoaiHinh", data, "sKyHieuCap2", "sKyHieuCap2,sTenLoaiHinhCap2,sKyHieuCap3,sTenLoaiHinhCap3");
            fr.AddTable("LoaiHinh", dtLoaiHinh);            

            return fr;
        }
        #endregion
    }
}

