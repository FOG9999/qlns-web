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
using DomainModel.Controls;
using VIETTEL.Models;
using VIETTEL.Controllers;
using System.IO;
using VIETTEL.Flexcel;
using Viettel.Services;
using Viettel.Data;

namespace VIETTEL.Report_Controllers.ThuNop
{
    public class rptQuyetToan_TongHop_Nam_QuyController : AppController
    {
        //
        public string sViewPath = "~/Report_Views/";
        private const String sFilePath = "/Report_ExcelFrom/QuyetToan/QuyetToanQuy/rptQuyetToan_TongHop_Nam_Quy.xls";
        private const String sFilePath_TongHop = "/Report_ExcelFrom/QuyetToan/QuyetToanQuy/rptQuyetToan_TongHop_Nam_Quy_TongHop.xls";
        private const String sFilePath_TongHop_denLNS = "/Report_ExcelFrom/QuyetToan/QuyetToanQuy/rptQuyetToan_TongHop_Nam_Quy_TongHop_denLNS.xls";
        private const String sFilePath_TongHop_denM = "/Report_ExcelFrom/QuyetToan/QuyetToanQuy/rptQuyetToan_TongHop_Nam_Quy_TongHop_denM.xls";
        private const String sFilePath_TongHop_denTM = "/Report_ExcelFrom/QuyetToan/QuyetToanQuy/rptQuyetToan_TongHop_Nam_Quy_TongHop_denTM.xls";
        public ActionResult Index()
        {
            FlexCelReport fr = new FlexCelReport();
            ViewData["path"] = "~/Report_Views/QuyetToan/QuyetToanQuy/rptQuyetToan_TongHop_Nam_Quy.aspx";
            return View(sViewPath + "ReportView.aspx");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="MaND"></param>
        /// <param name="iID_MaPhongBan"></param>
        /// <param name="iID_MaDonVi"></param>
        /// <param name="iID_MaNamNganSach">2:Nam nay 1.Nam Truoc</param>
        /// <returns></returns>
        public static DataTable rptQuyetToan_TongHop_Nam_Quy(String MaND, String sLNS, String iThang_Quy, String iID_MaDonVi, String iID_MaNamNganSach, String LoaiBaoCao, String iID_MaPhongBan)
        {
            String DKDonVi = "", DKPhongBan = "", DK = "";
            SqlCommand cmd = new SqlCommand();

            ////Báo cáo chi tiết từng đơn vị
            //if (LoaiBaoCao == "ChiTiet")
            //{
            //    if (!String.IsNullOrEmpty(iID_MaDonVi) && iID_MaDonVi != "-1")
            //    {
            //        DK += " AND iID_MaDonVi=@iID_MaDonVi";
            //        cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
            //    }
            //}
            ////Báo cáo tổng hợp
            //else
            //{
            //    if (String.IsNullOrEmpty(iID_MaDonVi))
            //        iID_MaDonVi = Guid.Empty.ToString();
            //    String[] arrDonVi = iID_MaDonVi.Split(',');
            //    for (int i = 0; i < arrDonVi.Length; i++)
            //    {
            //        DK += "iID_MaDonVi=@MaDonVi" + i;
            //        cmd.Parameters.AddWithValue("@MaDonVi" + i, arrDonVi[i]);
            //        if (i < arrDonVi.Length - 1)
            //            DK += " OR ";
            //    }
            //    if (String.IsNullOrEmpty(DK) == false)
            //        DK = " AND (" + DK + ")";
            //}

            //if (!String.IsNullOrEmpty(sLNS))
            //{
            //    DK += " AND sLNS IN (" + sLNS + ")";
            //}

            //if (iID_MaNamNganSach == "2")
            //{
            //    DK += " AND iID_MaNamNganSach IN (2) ";
            //}
            //else if (iID_MaNamNganSach == "1")
            //{
            //    DK += " AND iID_MaNamNganSach IN (1) ";
            //}
            //else
            //{
            //    DK += " AND iID_MaNamNganSach IN (1,2) ";
            //}

            //if (iID_MaPhongBan != "-1")
            //{
            //    DK += " AND iID_MaPhongBan = @MaPhongBan ";
            //    cmd.Parameters.AddWithValue("@MaPhongBan", iID_MaPhongBan);
            //}
            //else
            //{
            //    cmd.Parameters.AddWithValue("@MaPhongBan", DBNull.Value);
            //}
            //DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            //DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);


            //String SQL = "";
            //            if (LoaiBaoCao == "ChiTiet")
            //            {
            //                SQL = String.Format(@"
            //                    SELECT 
            //                        SUBSTRING(sLNS,1,1) as sLNS1
            //                        ,SUBSTRING(sLNS,1,3) as sLNS3
            //                        ,SUBSTRING(sLNS,1,5) as sLNS5
            //                        ,sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa
            //                        ,SUM(rTuChi) as rTuChi
            //                        ,SUM(Quy1) as Quy1
            //                        ,SUM(Quy2) as Quy2
            //                        ,SUM(Quy3) as Quy3
            //                        ,SUM(Quy4) as Quy4
            //                    FROM
            //                        (
            //
            //                        SELECT 
            //                            sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa
            //                            ,rTuChi=0
            //                            ,Quy1=SUM(CASE WHEN (iThang_Quy=1) THEN rTuChi ELSE 0 END)
            //                            ,Quy2=SUM(CASE WHEN (iThang_Quy=2) THEN rTuChi ELSE 0 END)
            //                            ,Quy3=SUM(CASE WHEN (iThang_Quy=3) THEN rTuChi ELSE 0 END)
            //                            ,Quy4=SUM(CASE WHEN (iThang_Quy=4) THEN rTuChi ELSE 0 END)
            //                        FROM 
            //                            QTA_ChungTuChiTiet
            //                        WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec AND iThang_Quy<=@iThang_Quy {0} {1} {2}
            //                        GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa
            //                        ) a
            //                    GROUP BY  sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa
            //                    HAVING 
            //                        SUM(rTuChi)<>0 
            //                        OR SUM(Quy1) <>0
            //                        OR SUM(Quy2)<>0
            //                        OR SUM(Quy3)<>0
            //                        OR SUM(Quy4)<>0 ", DK, DKDonVi, DKPhongBan);
            //            }
            //            else
            //{
            //    SQL = String.Format(@"
            //        SELECT 
            //            SUBSTRING(sLNS,1,1) as sLNS1
            //            ,SUBSTRING(sLNS,1,3) as sLNS3
            //            ,SUBSTRING(sLNS,1,5) as sLNS5
            //            ,sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,
            //            sXauNoiMa,
            //            iID_MaDonVi,sTenDonVi
            //            ,SUM(rTuChi) as rTuChi
            //            ,SUM(Quy1) as Quy1
            //            ,SUM(Quy2) as Quy2
            //            ,SUM(Quy3) as Quy3
            //            ,SUM(Quy4) as Quy4
            //        FROM
            //            (

            //            SELECT 
            //                sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sXauNoiMa,iID_MaDonVi,sTenDonVi
            //                ,rTuChi=0
            //                ,Quy1=SUM(CASE WHEN (iThang_Quy=1) THEN rTuChi ELSE 0 END)
            //                ,Quy2=SUM(CASE WHEN (iThang_Quy=2) THEN rTuChi ELSE 0 END)
            //                ,Quy3=SUM(CASE WHEN (iThang_Quy=3) THEN rTuChi ELSE 0 END)
            //                ,Quy4=SUM(CASE WHEN (iThang_Quy=4) THEN rTuChi ELSE 0 END)
            //            FROM 
            //                QTA_ChungTuChiTiet
            //             WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec AND iThang_Quy<=@iThang_Quy {0} {1} {2}
            //             GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sXauNoiMa,iID_MaDonVi,sTenDonVi) a
            //             GROUP BY  sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sXauNoiMa,iID_MaDonVi,sTenDonVi
            //             HAVING 
            //                SUM(rTuChi)<>0  
            //                OR SUM(Quy1) <>0
            //                OR SUM(Quy2)<>0
            //                OR SUM(Quy3)<>0
            //                OR SUM(Quy4)<>0 ", DK, DKDonVi, DKPhongBan);
            //}

            #region sql

            var sql =
@"

SELECT 
    LEFT(a.sLNS,1) as sLNS1,
    LEFT(a.sLNS,3) as sLNS3,
    LEFT(a.sLNS,5) as sLNS5,
    a.sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,
	iID_MaDonVi,
	sTenDonVi,
    SUM(rTuChi) as rTuChi,
    SUM(Quy1) as Quy1,
    SUM(Quy2) as Quy2,
    SUM(Quy3) as Quy3,
    SUM(Quy4) as Quy4
FROM
(
    -- quyet toan
    SELECT 
        sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,
		iID_MaDonVi,
		(iID_MaDonVi + ' - ' + sTenDonVi) as sTenDonVi,
        rTuChi=0,
        Quy1=SUM(CASE WHEN (iThang_Quy=1) THEN rTuChi ELSE 0 END),
        Quy2=SUM(CASE WHEN (iThang_Quy=2) THEN rTuChi ELSE 0 END),
        Quy3=SUM(CASE WHEN (iThang_Quy=3) THEN rTuChi ELSE 0 END),
        Quy4=SUM(CASE WHEN (iThang_Quy=4) THEN rTuChi ELSE 0 END)
    FROM	QTA_ChungTuChiTiet
    WHERE	iTrangThai=1 
			AND iNamLamViec=@iNamLamViec 
			AND iThang_Quy<=@iThang_Quy
            AND iID_MaNamNganSach IN (SELECT * FROM F_SplitString2(@iID_MaNamNganSach))
			AND (@iID_MaDonVi IS NULL OR iID_MaDonVi = @iID_MaDonVi)
			
			
	GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi,sTenDonVi

	UNION ALL

    -- lay chi tieu
	SELECT
		sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,
		iID_MaDonVi,
		sTenDonVi,
        CT as rTuChi,
        0 as Quy1,
        0 as Quy2,
        0 as Quy3,
        0 as Quy4
	FROM    F_NS_ChiTieu_TheoDonVi(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, @iID_MaNamNganSach)
	WHERE   sLNS IN (SELECT * FROM F_SplitString2(@sLNS)) 
) a

 GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi,sTenDonVi
";

            var sqlTongHop =
@"



SELECT 
    LEFT(a.sLNS,1) as sLNS1,
    LEFT(a.sLNS,3) as sLNS3,
    LEFT(a.sLNS,5) as sLNS5,
    a.sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,
	iID_MaDonVi, sTenDonVi,
	SUM(rTuChi) as rTuChi,
    SUM(Quy1) as Quy1,
    SUM(Quy2) as Quy2,
    SUM(Quy3) as Quy3,
    SUM(Quy4) as Quy4
FROM
(
    -- quyet toan
    SELECT 
        sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,
		iID_MaDonVi, (iID_MaDonVi +' - ' + sTenDonVi) as sTenDonVi,
		rTuChi=0,
        Quy1=SUM(CASE WHEN (iThang_Quy=1) THEN rTuChi ELSE 0 END),
        Quy2=SUM(CASE WHEN (iThang_Quy=2) THEN rTuChi ELSE 0 END),
        Quy3=SUM(CASE WHEN (iThang_Quy=3) THEN rTuChi ELSE 0 END),
        Quy4=SUM(CASE WHEN (iThang_Quy=4) THEN rTuChi ELSE 0 END)
    FROM	QTA_ChungTuChiTiet
    WHERE	iTrangThai=1 
			AND iNamLamViec=@iNamLamViec 
			AND iThang_Quy<=@iThang_Quy
            AND iID_MaNamNganSach IN (SELECT * FROM F_SplitString2(@iID_MaNamNganSach))
			AND sLNS IN (SELECT * FROM F_SplitString2(@sLNS)) 
            AND iID_MaDonVi IN (SELECT * FROM F_SplitString2(@iID_MaDonVi))
	GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi,sTenDonVi

	UNION ALL

    -- lay chi tieu
	SELECT
		sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,
		iID_MaDonVi, sTenDonVi,
		CT as rTuChi,
        0 as Quy1,
        0 as Quy2,
        0 as Quy3,
        0 as Quy4
	FROM    F_NS_ChiTieu_TheoDonVi(@iNamLamviec, @iID_MaDonVi,  @iID_MaPhongBan, @iID_MaNamNganSach)
	WHERE   sLNS IN (SELECT * FROM F_SplitString2(@sLNS)) 

) a


GROUP BY a.sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi,sTenDonVi
HAVING SUM(rTuChi + Quy1 + Quy2 + Quy3 + Quy4) > 0
ORDER BY a.sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi


";
            #endregion


            if (LoaiBaoCao != "ChiTiet")
                sql = sqlTongHop;

            //sql = string.Format(sql, DK, DKDonVi, DKPhongBan)
            //    .Replace("@@sLNS", sLNS);

            var namLamViec = int.Parse(ReportModels.LayNamLamViec(MaND));

            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("@iNamLamViec", namLamViec);
            cmd.Parameters.AddWithValue("@iThang_Quy", iThang_Quy);
            cmd.Parameters.AddWithValue("@sLNS", sLNS);
            cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
            cmd.Parameters.AddWithValue("@iID_MaNamNganSach", string.IsNullOrWhiteSpace(iID_MaNamNganSach) || iID_MaNamNganSach == "0" ? "1,2" : iID_MaNamNganSach);
            cmd.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan.ToParamString());
            

            DataTable dtAll = Connection.GetDataTable(cmd);
            DataView view = new DataView(dtAll);
            DataTable dtChiTiet = view.ToTable(false, "sLNS1", "sLNS3", "sLNS5", "sLNS", "sL",
                                        "sK", "sM", "sTM", "sTTM", "sNG", "sMoTa", "rTuChi", "Quy1", "Quy2", "Quy3", "Quy4");
            cmd.Dispose();


            if (LoaiBaoCao == "chitiet")
                return dtChiTiet;

            return dtAll;
        }

        /// <summary>
        /// Lấy dữ liệu
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <param name="iID_MaDonVi"></param>
        /// <returns></returns>
        public ActionResult EditSubmit(String ParentID)
        {
            String sLNS = Request.Form["sLNS"];
            String iID_MaDonVi = Request.Form["iID_MaDonVi"];
            String iThang_Quy = Request.Form["QuyetToanNganSach" + "_iThang_Quy"];
            String iID_MaNamNganSach = Request.Form["QuyetToanNganSach" + "_iID_MaNamNganSach"];
            String iID_MaPhongBan = Request.Form[ParentID + "_iID_MaPhongBan"];
            String LoaiTongHop = Request.Form["QuyetToanNganSach" + "_LoaiTongHop"];
            String iID_TuyChon = Request.Form["QuyetToanNganSach" + "_iID_TuyChon"];
            ViewData["PageLoad"] = "1";
            ViewData["sLNS"] = sLNS;
            ViewData["iID_MaDonVi"] = iID_MaDonVi;
            ViewData["iThang_Quy"] = iThang_Quy;
            ViewData["iID_MaNamNganSach"] = iID_MaNamNganSach;
            ViewData["iID_MaPhongBan"] = iID_MaPhongBan;
            ViewData["LoaiTongHop"] = LoaiTongHop;
            ViewData["iID_TuyChon"] = iID_TuyChon;
            ViewData["path"] = "~/Report_Views/QuyetToan/QuyetToanQuy/rptQuyetToan_TongHop_Nam_Quy.aspx";
            return View(sViewPath + "ReportView.aspx");
        }

        /// <summary>
        /// Tạo báo cáo
        /// </summary>
        /// <param name="path"></param>
        /// <param name="NamLamViec"></param>
        /// <param name="iID_MaDonVi"></param>
        /// <returns></returns>
        public ExcelFile CreateReport(String path, String MaND, String sLNS, String iThang_Quy, String iID_MaDonVi, String iID_MaNamNganSach, String LoaiBaoCao, String iID_MaPhongBan)
        {
            XlsFile Result = new XlsFile(true);
            Result.Open(path);
            FlexCelReport fr = new FlexCelReport();
            fr = ReportModels.LayThongTinChuKy(fr, "rptQuyetToan_TongHop_Nam_Quy");

            LoadData(fr, MaND, sLNS, iThang_Quy, iID_MaDonVi, iID_MaNamNganSach, LoaiBaoCao, iID_MaPhongBan);
            String Nam = ReportModels.LayNamLamViec(MaND);

            //lay ten nam ngan sach
            String NamNganSach = "";
            if (iID_MaNamNganSach == "1")
                NamNganSach = "QUYẾT TOÁN NĂM TRƯỚC";
            else if (iID_MaNamNganSach == "2")
                NamNganSach = "QUYẾT TOÁN NĂM NAY";
            else
            {
                NamNganSach = "TỔNG HỢP";
            }

            String sTenDonVi = DonViModels.Get_TenDonVi(iID_MaDonVi,MaND);
            String sTenPhongBan = "";
            if (iID_MaPhongBan != "-1")
            {
                //sTenPhongBan = "B" + iID_MaPhongBan;
                sTenPhongBan = PhongBanModels.Get_TenPhongBan(iID_MaPhongBan);
            }
            //fr.SetValue("BoQuocPhong", ReportModels.CauHinhTenDonViSuDung(1,MaND));
            //fr.SetValue("QuanKhu", ReportModels.CauHinhTenDonViSuDung(2,MaND));
            fr.SetValue("NgayThang", ReportModels.Ngay_Thang_Nam_HienTai());
            fr.SetValue("Nam", Nam);
            fr.SetValue("Quy", iThang_Quy);
            fr.SetValue("NamNganSach", NamNganSach);
            fr.SetValue("sTenDonVi", sTenDonVi);
            fr.SetValue("sTenPhongBan", sTenPhongBan);
            fr.UseCommonValue()
                .UseChuKy(Username, iID_MaPhongBan)
                .Run(Result);
            return Result;
        }

        /// <summary>
        /// Đổ dư liệu xuống báo cáo
        /// </summary>
        /// <param name="fr"></param>
        /// <param name="NamLamViec"></param>
        /// <param name="iID_MaDonVi"></param>
        private void LoadData(FlexCelReport fr, String MaND, String sLNS, String iThang_Quy, String iID_MaDonVi, String iID_MaNamNganSach, String LoaiBaoCao, String iID_MaPhongBan)
        {
            DataRow r;
            DataTable data = new DataTable();
            DataTable dtDonVi = new DataTable();
            if (LoaiBaoCao == "ChiTiet")
            {
                data = rptQuyetToan_TongHop_Nam_Quy(MaND, sLNS, iThang_Quy, iID_MaDonVi, iID_MaNamNganSach, LoaiBaoCao, iID_MaPhongBan);
            }
            else
            {
                dtDonVi = rptQuyetToan_TongHop_Nam_Quy(MaND, sLNS, iThang_Quy, iID_MaDonVi, iID_MaNamNganSach, LoaiBaoCao, iID_MaPhongBan);
                data = HamChung.SelectDistinct("ChiTiet", dtDonVi, "sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM,sTTM,sNG", "sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa");
                fr.AddTable("dtDonVi", dtDonVi);
                dtDonVi.Dispose();

            }

            data.TableName = "ChiTiet";
            fr.AddTable("ChiTiet", data);
            DataTable dtsTM = HamChung.SelectDistinct("dtsTM", data, "sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM", "sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM,sMoTa", "sLNS,sL,sK,sM,sTM,sTTM");
            DataTable dtsM = HamChung.SelectDistinct("dtsM", dtsTM, "sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM", "sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sMoTa", "sLNS,sL,sK,sM,sTM");
            DataTable dtsL = HamChung.SelectDistinct("dtsL", dtsM, "sLNS1,sLNS3,sLNS5,sLNS,sL,sK", "sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sMoTa", "sLNS,sL,sK,sM");
            DataTable dtsLNS = HamChung.SelectDistinct("dtsLNS", dtsL, "sLNS1,sLNS3,sLNS5,sLNS", "sLNS1,sLNS3,sLNS5,sLNS,sMoTa", "sLNS,sL");

            DataTable dtsLNS5 = HamChung.SelectDistinct("dtsLNS5", dtsLNS, "sLNS1,sLNS3,sLNS5", "sLNS1,sLNS3,sLNS5,sMoTa");
            for (int i = 0; i < dtsLNS5.Rows.Count; i++)
            {
                r = dtsLNS5.Rows[i];
                r["sMoTa"] = LayMoTa(Convert.ToString(r["sLNS5"]));
            }
            DataTable dtsLNS3 = HamChung.SelectDistinct("dtsLNS3", dtsLNS5, "sLNS1,sLNS3", "sLNS1,sLNS3,sMoTa");

            for (int i = 0; i < dtsLNS3.Rows.Count; i++)
            {
                r = dtsLNS3.Rows[i];
                r["sMoTa"] = LayMoTa(Convert.ToString(r["sLNS3"]));
            }
            DataTable dtsLNS1 = HamChung.SelectDistinct("dtsLNS1", dtsLNS3, "sLNS1", "sLNS1,sMoTa");
            for (int i = 0; i < dtsLNS1.Rows.Count; i++)
            {
                r = dtsLNS1.Rows[i];
                r["sMoTa"] = LayMoTa(Convert.ToString(r["sLNS1"]));
            }

            fr.AddTable("dtsTM", dtsTM);
            fr.AddTable("dtsM", dtsM);
            fr.AddTable("dtsL", dtsL);
            fr.AddTable("dtsLNS", dtsLNS);
            fr.AddTable("dtsLNS1", dtsLNS1);
            fr.AddTable("dtsLNS3", dtsLNS3);
            fr.AddTable("dtsLNS5", dtsLNS5);

            data.Dispose();
            dtsTM.Dispose();
            dtsM.Dispose();
            dtsL.Dispose();
            dtsLNS.Dispose();
            dtsLNS1.Dispose();
            dtsLNS3.Dispose();
            dtsLNS5.Dispose();

        }
        private string LayMoTa(String sLNS)
        {
            String sMoTa = "";

            String SQL = String.Format(@"SELECT sMoTa FROM NS_MucLucNganSach WHERE iTrangThai=1 AND sLNS={0}", sLNS);
            sMoTa = Connection.GetValueString(SQL, "");
            return sMoTa;
        }
        /// <summary>
        /// Hiển thị báo cáo theo định dạng PDF
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <param name="iID_MaDonVi"></param>
        /// <returns></returns>
        public ActionResult ViewPDF(String MaND, String iThang_Quy, String sLNS, String iID_MaDonVi, String iID_MaNamNganSach, String LoaiBaoCao, String iID_MaPhongBan, String iID_TuyChon)
        {
            HamChung.Language();
            String sDuongDan = "";
            if (LoaiBaoCao == "ChiTiet")
            {
                sDuongDan = sFilePath;
            }
            else
            {
                if (iID_TuyChon == "1")
                {
                    sDuongDan = sFilePath_TongHop;
                }
                else if(iID_TuyChon == "2")
                {
                    sDuongDan = sFilePath_TongHop_denLNS;
                }
                else if (iID_TuyChon == "3")
                {
                    sDuongDan = sFilePath_TongHop_denM;
                }
                else if (iID_TuyChon == "4")
                {
                    sDuongDan = sFilePath_TongHop_denTM;
                }
            }

            ExcelFile xls = CreateReport(Server.MapPath(sDuongDan), MaND, sLNS, iThang_Quy, iID_MaDonVi, iID_MaNamNganSach, LoaiBaoCao, iID_MaPhongBan);
            return xls.ToPdfContentResult(string.Format("{0}_{1}.pdf", this.ControllerName(), DateTime.Now.GetTimeStamp()));
        }

        public ActionResult Excel(String MaND, String sLNS, String iThang_Quy, String iID_MaDonVi, String iID_MaNamNganSach, String LoaiBaoCao, String iID_MaPhongBan, String iID_TuyChon)
        {
            String sDuongDan = "";
            if (LoaiBaoCao == "ChiTiet")
                sDuongDan = sFilePath;
            else
            {
                if (iID_TuyChon == "1")
                {
                    sDuongDan = sFilePath_TongHop;
                }
                else if (iID_TuyChon == "2")
                {
                    sDuongDan = sFilePath_TongHop_denLNS;
                }
                else if (iID_TuyChon == "3")
                {
                    sDuongDan = sFilePath_TongHop_denM;
                }
                else if (iID_TuyChon == "4")
                {
                    sDuongDan = sFilePath_TongHop_denTM;
                }
            }

            HamChung.Language();
            var xls = CreateReport(Server.MapPath(sDuongDan), Username, sLNS, iThang_Quy, iID_MaDonVi, iID_MaNamNganSach, LoaiBaoCao, iID_MaPhongBan);
            return xls.ToFileResult(string.Format("{0}_{1}.xls", this.ControllerName(), DateTime.Now.GetTimeStamp()));
 
        }
        public JsonResult Ds_DonVi(String ParentID, String Thang_Quy, String iID_MaDonVi, String sLNS, String iID_MaNamNganSach, String iID_MaPhongBan)
        {
            String MaND = User.Identity.Name;
            DataTable dt = QuyetToan_ReportModels.dtDonVi_LNS_PhongBan(Thang_Quy, iID_MaNamNganSach, MaND, iID_MaDonVi, iID_MaPhongBan);

            if (String.IsNullOrEmpty(sLNS))
            {
                sLNS = Guid.Empty.ToString();
            }
            String ViewNam = "~/Views/DungChung/DonVi/LNS_DanhSach.ascx";
            DanhSachDonViModels Model = new DanhSachDonViModels(MaND, sLNS, dt, ParentID);
            String strDonVi = HamChung.RenderPartialViewToStringLoad(ViewNam, Model, this);
            return Json(strDonVi, JsonRequestBehavior.AllowGet);
        }

    }
}

