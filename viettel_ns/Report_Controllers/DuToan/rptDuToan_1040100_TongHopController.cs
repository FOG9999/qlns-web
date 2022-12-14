using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FlexCel.Core;
using FlexCel.Report;
using FlexCel.Render;
using FlexCel.XlsAdapter;
using System.Data;
using System.Data.SqlClient;
using DomainModel;
using VIETTEL.Models;
using VIETTEL.Controllers;
using System.IO;
using Viettel.Services;
using VIETTEL.Flexcel;
using Viettel.Extensions;

namespace VIETTEL.Report_Controllers.DuToan
{
    public class rptDuToan_1040100_TongHopController : AppController
    {
        private readonly INganSachService _nganSachService = NganSachService.Default;

        public string sViewPath = "~/Report_Views/DuToan/";
        private  String sFilePath = "/Report_ExcelFrom/DuToan/rptDuToan_1040100_TongHop.xls";
        private  String sFilePath_TrinhKy = "/Report_ExcelFrom/DuToan/rptDuToan_1040100_TongHop_TrinhKy.xls";
        private String sFilePath_Muc = "/Report_ExcelFrom/DuToan/rptDuToan_1040100_TongHop_Muc.xls";
        String iCapTongHop = "";
        [Authorize]
        public ActionResult Index(int trinhky = 1)
        {
            if(HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath,User.Identity.Name))
            {
            ViewData["path"] = "~/Report_Views/DuToan/rptDuToan_1040100_TongHop.aspx";
            ViewData["PageLoad"] = "0";
            ViewData["trinhky"] = trinhky;

            return View(sViewPath + "ReportView.aspx");
            }
            else
            {
                return RedirectToAction("Index", "PermitionMessage");
            }
        }
        /// <summary>
        /// hàm lấy các giá trên form
        /// </summary>
        /// <param name="ParentID"></param>
        /// <returns></returns>
        public ActionResult EditSubmit(string ParentID, int trinhky = 0)
        {
            String iID_MaPhongBan = Request.Form[ParentID + "_iID_MaPhongBan"];
            String iCapTongHop = Request.Form[ParentID + "_iCapTongHop"];


            ViewData["PageLoad"] = "1";
            ViewData["iID_MaPhongBan"] = iID_MaPhongBan;
            ViewData["trinhky"] = trinhky;
            ViewData["iCapTongHop"] = iCapTongHop;
            ViewData["path"] = "~/Report_Views/DuToan/rptDuToan_1040100_TongHop.aspx";

            return View(sViewPath + "ReportView.aspx");
        }

        /// <summary>
        /// hàm khởi tạo báo cáo
        /// </summary>
        /// <param name="path"></param>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>

        private ExcelFile CreateReport(string iID_MaPhongBan, string iCapTongHop, int trinhky = 0)
        {
            
            var file = sFilePath;
            if(iCapTongHop == "Muc"){
                file = sFilePath_Muc;
            }
            else
            {
                file = iID_MaPhongBan != null && iID_MaPhongBan != "-1" && trinhky == 1 ?
                    sFilePath_TrinhKy :
                    sFilePath;
            }
            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(file));

            FlexCelReport fr = new FlexCelReport();
            LoadData(fr, Username, iID_MaPhongBan);

            var loaiKhoan = HamChung.GetLoaiKhoanText("1040100");
            fr.SetValue("LoaiKhoan", loaiKhoan);

            fr.UseCommonValue()
              .UseChuKy(Username, iID_MaPhongBan)
              .UseChuKyForController(this.ControllerName())
              .Run(xls);

            return xls;
        }

        //1020200
        /// <summary>
        /// Phụ lục 2c-c
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public static DataTable DT_rptDuToan_1040100_TongHop(String MaND,String iID_MaPhongBan)
        {
            if (string.IsNullOrWhiteSpace(iID_MaPhongBan))
                iID_MaPhongBan = "-1";

            String DKDonVi = "", DKPhongBan = "", DK = "";
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan_Dich(MaND, cmd, iID_MaPhongBan);
            int DVT = 1000;


            String SQL = "";
            String sTenB10 = "", sTenB6 = "", sTenB = "";

            sTenB10 = "TC,BTTM";

            sTenB6 = "DN";

            sTenB = "QBC";
            SQL = String.Format(
                @"SELECT sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,
                        iID_MaPhongBanDich,sTenPB=CASE WHEN iID_MaPhongBanDich='10' THEN N'{4}' 
							                        WHEN iID_MaPhongBanDich='06' THEN N'{5}'
							                        WHEN iID_MaPhongBanDich NOT IN ('06','10')  THEN N'{6}' END
                        ,rTuChi=SUM(rTuChi/{3})
                        ,rTonKho=SUM(rTonKho/{3})
                        ,rHangNhap=SUM(rHangNhap/{3})
                        ,rHangMua=SUM(rHangMua/{3})
                        ,rPhanCap=SUM(rPhanCap/{3})
                        ,rDuPhong=SUM(rDuPhong/{3})
                 FROM DT_ChungTuChiTiet
                 WHERE iTrangThai=1  AND sLNS LIKE '1040100%' AND (MaLoai='' OR MaLoai='2') AND iNamLamViec=@iNamLamViec {0} {1} {2}
                 GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaPhongBanDich
                 HAVING SUM(rTuChi)<>0 OR SUM(rTonKho)<>0 OR SUM(rHangNhap)<>0 OR SUM(rHangMua)<>0 OR SUM(rPhanCap)<>0 OR SUM(rDuPhong)<>0
                 ORDER BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sTenPB"
                 , "", DKPhongBan, DKDonVi, DVT, sTenB10,sTenB6,sTenB);
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
//            SQL = String.Format(@"SELECT sLNS,sL,sK,sM,sTM,sTTM
//,(SUM(rTuChi)+SUM(rhangNhap)+SUM(rPhanCap)+SUM(rDuPhong) + SUM(rHangMua))/{3} as rPhanCap
//FROM DT_ChungTuChiTiet
//WHERE iKyThuat=1 AND MaLoai=2 {0} {1} {2}
//AND iTrangThai=1  AND iNamLamViec=@iNamLamViec
//AND (rTuChi<>0 OR rHangNhap<>0 OR rPhanCap<>0 OR rDuPhong<>0 OR rHangMua<>0)
//AND sMoTa<>''
//GROUP BY sLNS,sL,sK,sM,sTM,sTTM", "",DKPhongBan,DKDonVi,DVT);
//            cmd.CommandText = SQL;
//            DataTable dtPhanCap = Connection.GetDataTable(cmd);
            //for (int i = 0; i < dtPhanCap.Rows.Count; i++)
            //{
                //String TruyVan = String.Format(@"sLNS='{0}' AND sL='{1}' AND sK='{2}' AND sM='{3}' AND sTM='{4}' AND sTTM='{5}'", dtPhanCap.Rows[i]["sLNS"], dtPhanCap.Rows[i]["sL"], dtPhanCap.Rows[i]["sK"], dtPhanCap.Rows[i]["sM"], dtPhanCap.Rows[i]["sTM"], dtPhanCap.Rows[i]["sTTM"]);
                //DataRow[] dr = dt.Select(TruyVan);
                //if (dr.Length > 0)
                //    dr[0]["rPhanCap"] = Convert.ToDecimal(dr[0]["rPhanCap"]) - Convert.ToDecimal(dtPhanCap.Rows[i]["rPhanCap"]);
            //}
            return dt;

        }
         
        /// <summary>
        /// hàm hiển thị dữ liệu ra báo cáo
        /// </summary>
        /// <param name="fr"></param>
        /// <param name="NamLamViec"></param>
        private void LoadData(FlexCelReport fr, String MaND, String iID_MaPhongBan)
        {
            DataTable data = DT_rptDuToan_1040100_TongHop(MaND, iID_MaPhongBan);
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

            data.Dispose();
            dtsTM.Dispose();
            dtsM.Dispose();
            dtsL.Dispose();
            dtsLNS.Dispose();
        }
      
        /// <summary>
        /// hàm view PDF
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public ActionResult ViewPDF(string iID_MaPhongBan, string iCapTongHop, int trinhky = 0)
        {
            HamChung.Language();

            ExcelFile xls = CreateReport(iID_MaPhongBan, iCapTongHop, trinhky);
            return xls.ToPdfContentResult(string.Format("{0}_{1}.pdf", this.ControllerName(), DateTime.Now.GetTimeStamp()));
        }

        public ActionResult Download(string iID_MaPhongBan, string iCapTongHop, int trinhky = 0)
        {
            HamChung.Language();

            ExcelFile xls = CreateReport(iID_MaPhongBan, iCapTongHop, trinhky);
            return xls.ToFileResult(string.Format("{0}_{1}.xls", this.ControllerName(), DateTime.Now.GetTimeStamp()));
        }
    }
}

