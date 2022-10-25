using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Data;
using System.Web.Mvc;
using Viettel.Data;
using Viettel.Services;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Models;

namespace VIETTEL.Report_Controllers.DuToan
{
    public class rptDuToan_1040100_ChiTiet_TrinhKyController : AppController
    {
        private readonly INganSachService _nganSachService = NganSachService.Default;

        public string sViewPath = "~/Report_Views/DuToan/";
        private String sFilePath = "/Report_ExcelFrom/DuToan/rptDuToan_1040100_TungDonVi_TrinhKy.xls";

        /// <summary>
        /// hàm khởi tạo báo cáo
        /// </summary>
        /// <param name="path"></param>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>


        /// <summary>
        /// hàm view PDF
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        /// 
        public ActionResult ViewPDF(string iID_MaChungTu)
        {
            HamChung.Language();
            var xls = createReport(iID_MaChungTu);
            return xls.ToPdfContentResult(string.Format("{0}_{1}.pdf", this.ControllerName(), DateTime.Now.GetTimeStamp()));
        }

        public ActionResult Download(string iID_MaChungTu)
        {
            HamChung.Language();
            var xls = createReport(iID_MaChungTu);
            return xls.ToFileResult(string.Format("{0}_{1}.xls", this.ControllerName(), DateTime.Now.GetTimeStamp()));
        }

        private ExcelFile createReport(string iID_MaChungTu)
        {
            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(sFilePath));

            var fr = new FlexCelReport();

            var dvt = Request.GetQueryStringValue("dvt", 1);
            if (dvt == 0)
                dvt = 1000;
            LoadData(fr, iID_MaChungTu, dvt);


            //var sTenDonVi = Convert.ToString(CommonFunction.LayTruong("NS_MucLucNganSach_Nganh", "iID_MaNganh", iID_MaDonVi, "sTenNganh"));
            fr.SetValue("sTenDonVi", "");
            var loaiKhoan = HamChung.GetLoaiKhoanText("1040100");
            fr.SetValue("LoaiKhoan", loaiKhoan);

            fr.UseCommonValue(null, dvt)
                .UseChuKy()
                .UseChuKyForController(this.ControllerName())
                .Run(xls);

            return xls;
        }




        private DataTable DT_rptDuToan_1040100_ChiTiet_TrinhKy(string iID_MaChungTu, int dvt = 1000)
        {
            #region sql

            var sql =
@"SELECT    sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sGhiChu,
            rTuChi      =SUM(rTuChi/{0}),
            rHienVat    =SUM(rHienVat/{0}),
            rTonKho     =SUM(rTonKho/{0}),
            rHangNhap   =SUM(rHangNhap/{0}),
            rHangMua    =SUM(rHangMua/{0}),
            rPhanCap    =SUM(rPhanCap/{0}),
            rDuPhong    =SUM(rDuPhong/{0})
    FROM DT_ChungTuChiTiet
    WHERE   
            iTrangThai=1  AND 
            --iNamLamViec=@iNamLamViec AND
            iID_MaChungTu = @iID_MaChungTu
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sGhiChu
    HAVING  SUM(rTuChi)<>0 OR 
            SUM(rHienVat)<>0 OR 
            SUM(rHangNhap)<>0 OR 
            SUM(rTonKho)<>0  OR 
            SUM(rHangMua)<>0  OR 
            SUM(rPhanCap)<>0 OR 
            SUM(rDuPhong)<>0";

            #endregion

            #region dieu kien

            sql = string.Format(sql, dvt);

            #endregion

            var config = _nganSachService.GetCauHinh(Username);
            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                var dt = conn.ExecuteDataTable(
                     commandText: sql,
                     parameters: new
                     {
                         iID_MaChungTu,
                     },
                     commandType: CommandType.Text
                 );

                return dt;
            }
        }


        /// <summary>
        /// hàm hiển thị dữ liệu ra báo cáo
        /// </summary>
        /// <param name="fr"></param>
        /// <param name="NamLamViec"></param>
        private void LoadData(FlexCelReport fr, string iID_MaChungTu, int dvt = 1000)
        {
            DataRow r;
            DataTable data = DT_rptDuToan_1040100_ChiTiet_TrinhKy(iID_MaChungTu, dvt);
            DataTable dataGhiChu = data;


            data.TableName = "ChiTiet";
            fr.AddTable("ChiTiet", data);
            DataTable dtsTM = HamChung.SelectDistinct("dtsTM", data, "sLNS,sL,sK,sM,sTM", "sLNS,sL,sK,sM,sTM,sMoTa", "sLNS,sL,sK,sM,sTM,sTTM");
            DataTable dtsM = HamChung.SelectDistinct("dtsM", dtsTM, "sLNS,sL,sK,sM", "sLNS,sL,sK,sM,sMoTa", "sLNS,sL,sK,sM,sTM");
            DataTable dtsL = HamChung.SelectDistinct("dtsL", dtsM, "sLNS,sL,sK", "sLNS,sL,sK,sMoTa", "sLNS,sL,sK,sM");
            DataTable dtsLNS = HamChung.SelectDistinct("dtsLNS", dtsL, "sLNS", "sLNS,sMoTa", "sLNS,sL");



            fr.AddTable("dtsTM", dtsTM);
            fr.AddTable("dtsM", dtsM);
            fr.AddTable("dtsL", dtsL);
            fr.AddTable("dtsLNS", dtsLNS);

            fr.AddTable("GhiChu", dataGhiChu);
            DataTable dtMoTaGhiChu = HamChung.SelectDistinct("dtMoTaGhiChu", dataGhiChu, "sLNS,sL,sK,sM,sTM,sTTM,sNG", "sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sGhiChu", "");
            fr.AddTable("dtMoTaGhiChu", dtMoTaGhiChu);
            bool checkCoGhiChu = false;
            for (int i = 0; i < dtMoTaGhiChu.Rows.Count; i++)
            {
                r = dtMoTaGhiChu.Rows[i];
                if (!String.IsNullOrEmpty(r["sGhiChu"].ToString()))
                {
                    checkCoGhiChu = true;
                }
            }
            if (checkCoGhiChu)
                fr.SetValue("sGhiChu", " * Ghi chú: ");
            else
                fr.SetValue("sGhiChu", "");
            data.Dispose();
            dtsTM.Dispose();
            dtsM.Dispose();
            dtsL.Dispose();
            dtsLNS.Dispose();

        }
    }
}
