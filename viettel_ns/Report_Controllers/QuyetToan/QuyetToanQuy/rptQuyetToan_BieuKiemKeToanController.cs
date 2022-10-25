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

namespace VIETTEL.Report_Controllers.ThuNop
{
    public class rptQuyetToan_BieuKiemKeToanController : Controller
    {
       
        public string sViewPath = "~/Report_Views/";
        private const String sFilePath = "/Report_ExcelFrom/QuyetToan/QuyetToanQuy/rptQuyetToan_BieuKiemKeToan.xls";

        public static String iID_MaPhongBan, iID_MaDonVi, iThang_Quy, iID_MaNamNganSach, MaND;
        public ActionResult Index()
        {
            ViewData["path"] = "~/Report_Views/QuyetToan/QuyetToanQuy/rptQuyetToan_BieuKiemKeToan.aspx";
            return View(sViewPath + "ReportView.aspx");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <param name="iID_MaDonVi"></param>
        /// <returns></returns>
        public static DataTable rptQuyetToan_BieuKiemKeToan()
        {
            String DKDonVi = "", DKPhongBan = "", DK = "";
            SqlCommand cmd = new SqlCommand();
            if (!String.IsNullOrEmpty(iID_MaDonVi) && iID_MaDonVi != "-1")
            {
                DK += " AND iID_MaDonVi=@iID_MaDonVi";
                cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
            }
            if (!String.IsNullOrEmpty(iID_MaNamNganSach) && iID_MaNamNganSach != "0")
            {
                DK += " AND iID_MaNamNganSach=@iID_MaNamNganSach";
                cmd.Parameters.AddWithValue("@iID_MaNamNganSach", iID_MaNamNganSach);
            }
            if (iThang_Quy != "-1")
            {
                DK += " AND iThang_Quy<=@iThang_Quy";
                cmd.Parameters.AddWithValue("@iThang_Quy", iThang_Quy);
            }
            String DKTrongKy = "";
            if (iThang_Quy != "-1")
            {
                DKTrongKy += " AND iThang_Quy=@iThang_Quy";
            }
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan(MaND, cmd, iID_MaPhongBan);
            String SQL =
                String.Format(@"


SELECT sLoai='A', sTenLoai=N'Ngân sách quốc phòng',
sLNS='1',sTenLNS=N'Kinh phí lương, phụ cấp, tiền ăn',
TK=ISNULL(SUM(CASE WHEN iID_MaNamNganSach=1 {3} THEN rTuChi ELSE 0 END),0),
LK=ISNULL(SUM(CASE WHEN iID_MaNamNganSach=1 THEN rTuChi ELSE 0 END),0)
,iID_MaDonVi,sTenDonVi
,iID_MaNamNganSach=1,sTenNamNganSach=N'Năm trước'
FROM QTA_ChungTuChiTiet 
WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach=1  {0} {1} {2}
AND sLNS='1010000'
GROUP BY iID_MaDonVi,sTenDonVi

UNION

SELECT sLoai='A', sTenLoai=N'Ngân sách quốc phòng',
sLNS='1',sTenLNS=N'Kinh phí lương, phụ cấp, tiền ăn',
TK=ISNULL(SUM(CASE WHEN iID_MaNamNganSach=2 {3} THEN rTuChi ELSE 0 END),0),
LK=ISNULL(SUM(CASE WHEN iID_MaNamNganSach=2 THEN rTuChi ELSE 0 END),0)
,iID_MaDonVi,sTenDonVi
,iID_MaNamNganSach=2,sTenNamNganSach=N'Năm nay'
FROM QTA_ChungTuChiTiet 
WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach=2  {0} {1} {2}
AND sLNS='1010000'
GROUP BY iID_MaDonVi,sTenDonVi

UNION


SELECT sLoai='A', sTenLoai=N'Ngân sách quốc phòng',
sLNS='2',sTenLNS=N'Kinh phí nghiệp vụ',
TK=ISNULL(SUM(CASE WHEN iID_MaNamNganSach=1 {3} THEN rTuChi ELSE 0 END),0),
LK=ISNULL(SUM(CASE WHEN iID_MaNamNganSach=1 THEN rTuChi ELSE 0 END),0)
,iID_MaDonVi,sTenDonVi
,iID_MaNamNganSach=1,sTenNamNganSach=N'Năm trước'
FROM QTA_ChungTuChiTiet 
WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach=1  {0} {1} {2}
AND sLNS IN ('1020100','1020200','1020300','1020500')
GROUP BY iID_MaDonVi,sTenDonVi


UNION


SELECT sLoai='A', sTenLoai=N'Ngân sách quốc phòng',
sLNS='2',sTenLNS=N'Kinh phí nghiệp vụ',
TK=ISNULL(SUM(CASE WHEN iID_MaNamNganSach=2 {3} THEN rTuChi ELSE 0 END),0),
LK=ISNULL(SUM(CASE WHEN iID_MaNamNganSach=2 THEN rTuChi ELSE 0 END),0)
,iID_MaDonVi,sTenDonVi
,iID_MaNamNganSach=2,sTenNamNganSach=N'Năm nay'
FROM QTA_ChungTuChiTiet 
WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach=2  {0} {1} {2}
AND sLNS IN ('1020100','1020200','1020300','1020500')
GROUP BY iID_MaDonVi,sTenDonVi

UNION


SELECT sLoai='A', sTenLoai=N'Ngân sách quốc phòng',
sLNS='3',sTenLNS=N'Kinh phí đầu tư XDCB-NSQP',
TK=ISNULL(SUM(CASE WHEN iID_MaNamNganSach=1 {3} THEN rTuChi ELSE 0 END),0),
LK=ISNULL(SUM(CASE WHEN iID_MaNamNganSach=1 THEN rTuChi ELSE 0 END),0)
,iID_MaDonVi,sTenDonVi
,iID_MaNamNganSach=1,sTenNamNganSach=N'Năm trước'
FROM QTA_ChungTuChiTiet 
WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach=1  {0} {1} {2}
AND sLNS IN ('1030100','2010300')
GROUP BY iID_MaDonVi,sTenDonVi


UNION


SELECT sLoai='A', sTenLoai=N'Ngân sách quốc phòng',
sLNS='3',sTenLNS=N'Kinh phí đầu tư XDCB-NSQP',
TK=ISNULL(SUM(CASE WHEN iID_MaNamNganSach=2 {3} THEN rTuChi ELSE 0 END),0),
LK=ISNULL(SUM(CASE WHEN iID_MaNamNganSach=2 THEN rTuChi ELSE 0 END),0)
,iID_MaDonVi,sTenDonVi
,iID_MaNamNganSach=2,sTenNamNganSach=N'Năm nay'
FROM QTA_ChungTuChiTiet 
WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach=2  {0} {1} {2}
AND sLNS IN ('1030100','2010300')
GROUP BY iID_MaDonVi,sTenDonVi

UNION

SELECT sLoai='A', sTenLoai=N'Ngân sách quốc phòng',
sLNS='4',sTenLNS=N'Kinh phí Bảo đảm',
TK=ISNULL(SUM(CASE WHEN iID_MaNamNganSach=1 {3} THEN rTuChi ELSE 0 END),0),
LK=ISNULL(SUM(CASE WHEN iID_MaNamNganSach=1 THEN rTuChi ELSE 0 END),0)
,iID_MaDonVi,sTenDonVi
,iID_MaNamNganSach=1,sTenNamNganSach=N'Năm trước'
FROM QTA_ChungTuChiTiet 
WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach=1  {0} {1} {2}
AND sLNS IN ('1040100','1040200','1040300')
GROUP BY iID_MaDonVi,sTenDonVi

UNION

SELECT sLoai='A', sTenLoai=N'Ngân sách quốc phòng',
sLNS='4',sTenLNS=N'Kinh phí Bảo đảm',
TK=ISNULL(SUM(CASE WHEN iID_MaNamNganSach=2 {3} THEN rTuChi ELSE 0 END),0),
LK=ISNULL(SUM(CASE WHEN iID_MaNamNganSach=2 THEN rTuChi ELSE 0 END),0)
,iID_MaDonVi,sTenDonVi
,iID_MaNamNganSach=2,sTenNamNganSach=N'Năm nay'
FROM QTA_ChungTuChiTiet 
WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach=2  {0} {1} {2}
AND sLNS IN ('1040100','1040200','1040300')
GROUP BY iID_MaDonVi,sTenDonVi

UNION

SELECT sLoai='A', sTenLoai=N'Ngân sách quốc phòng',
sLNS='5',sTenLNS=N'Chi cho doanh nghiệp',
TK=ISNULL(SUM(CASE WHEN iID_MaNamNganSach=1 {3} THEN rTuChi ELSE 0 END),0),
LK=ISNULL(SUM(CASE WHEN iID_MaNamNganSach=1 THEN rTuChi ELSE 0 END),0)
,iID_MaDonVi,sTenDonVi
,iID_MaNamNganSach=1,sTenNamNganSach=N'Năm trước'
FROM QTA_ChungTuChiTiet 
WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach=1  {0} {1} {2}
AND sLNS IN ('1050100','1050000')
GROUP BY iID_MaDonVi,sTenDonVi

UNION

SELECT sLoai='A', sTenLoai=N'Ngân sách quốc phòng',
sLNS='5',sTenLNS=N'Chi cho doanh nghiệp',
TK=ISNULL(SUM(CASE WHEN iID_MaNamNganSach=2 {3} THEN rTuChi ELSE 0 END),0),
LK=ISNULL(SUM(CASE WHEN iID_MaNamNganSach=2 THEN rTuChi ELSE 0 END),0)
,iID_MaDonVi,sTenDonVi
,iID_MaNamNganSach=2,sTenNamNganSach=N'Năm nay'
FROM QTA_ChungTuChiTiet 
WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach=2  {0} {1} {2}
AND sLNS IN ('1050100','1050000')
GROUP BY iID_MaDonVi,sTenDonVi

UNION

SELECT sLoai='A', sTenLoai=N'Ngân sách quốc phòng',
sLNS='6',sTenLNS=N'Ngân sách QP khác (NN giao)',
TK=ISNULL(SUM(CASE WHEN iID_MaNamNganSach=1 {3} THEN rTuChi ELSE 0 END),0),
LK=ISNULL(SUM(CASE WHEN iID_MaNamNganSach=1 THEN rTuChi ELSE 0 END),0)
,iID_MaDonVi,sTenDonVi
,iID_MaNamNganSach=1,sTenNamNganSach=N'Năm trước'
FROM QTA_ChungTuChiTiet 
WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach=1  {0} {1} {2}
AND sLNS LIKE '109%'
GROUP BY iID_MaDonVi,sTenDonVi

UNION

SELECT sLoai='A', sTenLoai=N'Ngân sách quốc phòng',
sLNS='6',sTenLNS=N'Ngân sách QP khác (NN giao)',
TK=ISNULL(SUM(CASE WHEN iID_MaNamNganSach=2 {3} THEN rTuChi ELSE 0 END),0),
LK=ISNULL(SUM(CASE WHEN iID_MaNamNganSach=2 THEN rTuChi ELSE 0 END),0)
,iID_MaDonVi,sTenDonVi
,iID_MaNamNganSach=2,sTenNamNganSach=N'Năm nay'
FROM QTA_ChungTuChiTiet 
WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach=2  {0} {1} {2}
AND sLNS LIKE '109%'
GROUP BY iID_MaDonVi,sTenDonVi

UNION

SELECT sLoai='B', sTenLoai=N'Ngân sách nhà nước',
sLNS='1',sTenLNS=N'Chi biển đông hải đảo',
TK=ISNULL(SUM(CASE WHEN iID_MaNamNganSach=1 {3} THEN rTuChi ELSE 0 END),0),
LK=ISNULL(SUM(CASE WHEN iID_MaNamNganSach=1 THEN rTuChi ELSE 0 END),0)
,iID_MaDonVi,sTenDonVi
,iID_MaNamNganSach=1,sTenNamNganSach=N'Năm trước'
FROM QTA_ChungTuChiTiet 
WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach=1  {0} {1} {2}
AND sLNS IN ('2120100')
GROUP BY iID_MaDonVi,sTenDonVi

UNION

SELECT sLoai='B', sTenLoai=N'Ngân sách nhà nước',
sLNS='1',sTenLNS=N'Chi biển đông hải đảo',
TK=ISNULL(SUM(CASE WHEN iID_MaNamNganSach=2 {3} THEN rTuChi ELSE 0 END),0),
LK=ISNULL(SUM(CASE WHEN iID_MaNamNganSach=2 THEN rTuChi ELSE 0 END),0)
,iID_MaDonVi,sTenDonVi
,iID_MaNamNganSach=2,sTenNamNganSach=N'Năm nay'
FROM QTA_ChungTuChiTiet 
WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach=2  {0} {1} {2}
AND sLNS IN ('2120100')
GROUP BY iID_MaDonVi,sTenDonVi


UNION

SELECT sLoai='B', sTenLoai=N'Ngân sách nhà nước',
sLNS='2',sTenLNS=N'Chi ĐT TT (XDCB, Nguồn TPCP)',
TK=ISNULL(SUM(CASE WHEN iID_MaNamNganSach=1 {3} THEN rTuChi ELSE 0 END),0),
LK=ISNULL(SUM(CASE WHEN iID_MaNamNganSach=1 THEN rTuChi ELSE 0 END),0)
,iID_MaDonVi,sTenDonVi
,iID_MaNamNganSach=1,sTenNamNganSach=N'Năm trước'
FROM QTA_ChungTuChiTiet 
WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach=1  {0} {1} {2}
AND sLNS IN ('2010100','2200000')
GROUP BY iID_MaDonVi,sTenDonVi


UNION

SELECT sLoai='B', sTenLoai=N'Ngân sách nhà nước',
sLNS='2',sTenLNS=N'Chi ĐT TT (XDCB, Nguồn TPCP)',
TK=ISNULL(SUM(CASE WHEN iID_MaNamNganSach=2 {3} THEN rTuChi ELSE 0 END),0),
LK=ISNULL(SUM(CASE WHEN iID_MaNamNganSach=2 THEN rTuChi ELSE 0 END),0)
,iID_MaDonVi,sTenDonVi
,iID_MaNamNganSach=2,sTenNamNganSach=N'Năm nay'
FROM QTA_ChungTuChiTiet 
WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach=2  {0} {1} {2}
AND sLNS IN ('2010100','2200000')
GROUP BY iID_MaDonVi,sTenDonVi

UNION

SELECT sLoai='B', sTenLoai=N'Ngân sách nhà nước',
sLNS='3',sTenLNS=N'Chi dự trữ nhà nước',
TK=ISNULL(SUM(CASE WHEN iID_MaNamNganSach=1 {3} THEN rTuChi ELSE 0 END),0),
LK=ISNULL(SUM(CASE WHEN iID_MaNamNganSach=1 THEN rTuChi ELSE 0 END),0)
,iID_MaDonVi,sTenDonVi
,iID_MaNamNganSach=1,sTenNamNganSach=N'Năm trước'
FROM QTA_ChungTuChiTiet 
WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach=1  {0} {1} {2}
AND sLNS IN ('2100000')
GROUP BY iID_MaDonVi,sTenDonVi

UNION

SELECT sLoai='B', sTenLoai=N'Ngân sách nhà nước',
sLNS='3',sTenLNS=N'Chi dự trữ nhà nước',
TK=ISNULL(SUM(CASE WHEN iID_MaNamNganSach=2 {3} THEN rTuChi ELSE 0 END),0),
LK=ISNULL(SUM(CASE WHEN iID_MaNamNganSach=2 THEN rTuChi ELSE 0 END),0)
,iID_MaDonVi,sTenDonVi
,iID_MaNamNganSach=2,sTenNamNganSach=N'Năm nay'
FROM QTA_ChungTuChiTiet 
WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach=2  {0} {1} {2}
AND sLNS IN ('2100000')
GROUP BY iID_MaDonVi,sTenDonVi

UNION

SELECT sLoai='B', sTenLoai=N'Ngân sách nhà nước',
sLNS='4',sTenLNS=N'Chi KP nhà nước khác',
TK=ISNULL(SUM(CASE WHEN iID_MaNamNganSach=1 {3} THEN rTuChi ELSE 0 END),0),
LK=ISNULL(SUM(CASE WHEN iID_MaNamNganSach=1 THEN rTuChi ELSE 0 END),0)
,iID_MaDonVi,sTenDonVi
,iID_MaNamNganSach=1,sTenNamNganSach=N'Năm trước'
FROM QTA_ChungTuChiTiet 
WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach=1  {0} {1} {2}
AND sLNS LIKE '2%' AND sLNS NOT IN ('2120100','2010100','2200000','2100000','2010300')
GROUP BY iID_MaDonVi,sTenDonVi


UNION

SELECT sLoai='B', sTenLoai=N'Ngân sách nhà nước',
sLNS='4',sTenLNS=N'Chi KP nhà nước khác',
TK=ISNULL(SUM(CASE WHEN iID_MaNamNganSach=2 {3} THEN rTuChi ELSE 0 END),0),
LK=ISNULL(SUM(CASE WHEN iID_MaNamNganSach=2 THEN rTuChi ELSE 0 END),0)
,iID_MaDonVi,sTenDonVi
,iID_MaNamNganSach=2,sTenNamNganSach=N'Năm nay'
FROM QTA_ChungTuChiTiet 
WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach=2  {0} {1} {2}
AND sLNS LIKE '2%' AND sLNS NOT IN ('2120100','2010100','2200000','2100000','2010300')
GROUP BY iID_MaDonVi,sTenDonVi

UNION

SELECT sLoai='C', sTenLoai=N'Ngân sách đặc biệt',
sLNS='1',sTenLNS=N'Ngân sách đặc biệt',
TK=ISNULL(SUM(CASE WHEN iID_MaNamNganSach=1 {3} THEN rTuChi ELSE 0 END),0),
LK=ISNULL(SUM(CASE WHEN iID_MaNamNganSach=1 THEN rTuChi ELSE 0 END),0)
,iID_MaDonVi,sTenDonVi
,iID_MaNamNganSach=1,sTenNamNganSach=N'Năm trước'
FROM QTA_ChungTuChiTiet 
WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach=1  {0} {1} {2}
AND sLNS LIKE '3%' 
GROUP BY iID_MaDonVi,sTenDonVi

UNION

SELECT sLoai='C', sTenLoai=N'Ngân sách đặc biệt',
sLNS='1',sTenLNS=N'Ngân sách đặc biệt',
TK=ISNULL(SUM(CASE WHEN iID_MaNamNganSach=2 {3} THEN rTuChi ELSE 0 END),0),
LK=ISNULL(SUM(CASE WHEN iID_MaNamNganSach=2 THEN rTuChi ELSE 0 END),0)
,iID_MaDonVi,sTenDonVi
,iID_MaNamNganSach=2,sTenNamNganSach=N'Năm nay'
FROM QTA_ChungTuChiTiet 
WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach=2  {0} {1} {2}
AND sLNS LIKE '3%' 
GROUP BY iID_MaDonVi,sTenDonVi

UNION

SELECT sLoai='D', sTenLoai=N'Kinh phí khác',
sLNS='1',sTenLNS=N'Kinh phí khác',
TK=ISNULL(SUM(CASE WHEN iID_MaNamNganSach=1 {3} THEN rTuChi ELSE 0 END),0),
LK=ISNULL(SUM(CASE WHEN iID_MaNamNganSach=1 THEN rTuChi ELSE 0 END),0)
,iID_MaDonVi,sTenDonVi
,iID_MaNamNganSach=1,sTenNamNganSach=N'Năm trước'
FROM QTA_ChungTuChiTiet 
WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach=1  {0} {1} {2}
AND sLNS LIKE '4%'
GROUP BY iID_MaDonVi,sTenDonVi

UNION

SELECT sLoai='D', sTenLoai=N'Kinh phí khác',
sLNS='1',sTenLNS=N'Kinh phí khác',
TK=ISNULL(SUM(CASE WHEN iID_MaNamNganSach=2 {3} THEN rTuChi ELSE 0 END),0),
LK=ISNULL(SUM(CASE WHEN iID_MaNamNganSach=2 THEN rTuChi ELSE 0 END),0)
,iID_MaDonVi,sTenDonVi
,iID_MaNamNganSach=2,sTenNamNganSach=N'Năm nay'
FROM QTA_ChungTuChiTiet 
WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach=2  {0} {1} {2}
AND sLNS LIKE '4%'
GROUP BY iID_MaDonVi,sTenDonVi
ORDER BY iID_MaDonVi
", DKDonVi, DKPhongBan, DK, DKTrongKy);
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }

        /// <summary>
        /// Lấy dữ liệu
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <param name="iID_MaDonVi"></param>
        /// <returns></returns>
        public ActionResult EditSubmit(String ParentID)
        {
            iID_MaPhongBan = Request.Form[ParentID + "_iID_MaPhongBan"];
            iID_MaDonVi = Request.Form[ParentID + "_iID_MaDonVi"];
            iThang_Quy = Request.Form[ParentID + "_iThang_Quy"];
            iID_MaNamNganSach = Request.Form[ParentID + "_iID_MaNamNganSach"];
            ViewData["path"] = "~/Report_Views/QuyetToan/QuyetToanQuy/rptQuyetToan_BieuKiemKeToan.aspx";
            ViewData["iID_MaPhongBan"] = iID_MaPhongBan;
            ViewData["iID_MaDonVi"] = iID_MaDonVi;
            ViewData["iThang_Quy"] = iThang_Quy;
            ViewData["iID_MaNamNganSach"] = iID_MaNamNganSach;
            ViewData["PageLoad"] = "1";
            return View(sViewPath + "ReportView.aspx");
        }

        /// <summary>
        /// Tạo báo cáo
        /// </summary>
        /// <param name="path"></param>
        /// <param name="NamLamViec"></param>
        /// <param name="iID_MaDonVi"></param>
        /// <returns></returns>
        public ExcelFile CreateReport()
        {
            XlsFile Result = new XlsFile(true);
            Result.Open(Server.MapPath(sFilePath));
            FlexCelReport fr = new FlexCelReport();
            fr = ReportModels.LayThongTinChuKy(fr, "rptQuyetToan_BieuKiemKeToan");
            LoadData(fr);
            String Nam = ReportModels.LayNamLamViec(MaND);
           
            fr.SetValue("BoQuocPhong", ReportModels.CauHinhTenDonViSuDung(1,MaND));
            fr.SetValue("QuanKhu", ReportModels.CauHinhTenDonViSuDung(2, MaND));
            fr.SetValue("NgayThang", ReportModels.Ngay_Thang_Nam_HienTai());
            fr.SetValue("Nam","Năm "+ Nam);
            if (iThang_Quy != "-1")
            {
                fr.SetValue("Quy","Quý "+ iThang_Quy+" - ");
            }
            else
                fr.SetValue("Quy", "");
            if (iID_MaDonVi != "-1")
            {
                String sTenDonVi = DonViModels.Get_TenDonVi(iID_MaDonVi,MaND);
                fr.SetValue("DonVi", sTenDonVi);
            }
            else
                fr.SetValue("DonVi", "");


            if (iID_MaPhongBan != "-1")
                fr.SetValue("PhongBan", "B " + iID_MaPhongBan);
            else
            {
                fr.SetValue("PhongBan", "");
            }

           

            if (iID_MaNamNganSach == "1")
                fr.SetValue("NamNganSach","Năm trước");
            else if (iID_MaNamNganSach == "2")
                fr.SetValue("NamNganSach", "Năm nay");
            else
                fr.SetValue("NamNganSach", "");
            fr.Run(Result);
            return Result;
        }

        /// <summary>
        /// Đổ dư liệu xuống báo cáo
        /// </summary>
        /// <param name="fr"></param>
        /// <param name="NamLamViec"></param>
        /// <param name="iID_MaDonVi"></param>
        private void LoadData(FlexCelReport fr)
        {
           
            DataTable dtDonVi = rptQuyetToan_BieuKiemKeToan();
            fr.AddTable("DonVi",dtDonVi);

            DataTable dtNamNganSach = HamChung.SelectDistinct("ChiTiet", dtDonVi, "sLoai,sLNS,iID_MaNamNganSach", "sLoai,sTenLoai,sLNS,sTenLNS,iID_MaNamNganSach,sTenNamNganSach");
            fr.AddTable("NamNS", dtNamNganSach);

            DataTable data = HamChung.SelectDistinct("ChiTiet", dtNamNganSach, "sLoai,sLNS", "sLoai,sTenLoai,sLNS,sTenLNS");
            data.TableName = "ChiTiet";
            fr.AddTable("ChiTiet", data);
            dtDonVi.Dispose();
            data.Dispose();
            dtNamNganSach.Dispose();
            DataTable dtLoai = HamChung.SelectDistinct("Loai",data,"sLoai","sLoai,sTenLoai");
            fr.AddTable("Loai", dtLoai);
            dtLoai.Dispose();
          
        }
       

        /// <summary>
        /// Hiển thị báo cáo theo định dạng PDF
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <param name="iID_MaDonVi"></param>
        /// <returns></returns>
        public ActionResult ViewPDF(String viID_MaPhongBan, String viID_MaDonVi, String viThang_Quy, String viID_MaNamNganSach)
        {
            HamChung.Language();
            MaND = User.Identity.Name;
            iID_MaPhongBan = viID_MaPhongBan;
            iID_MaDonVi = viID_MaDonVi;
            iThang_Quy = viThang_Quy;
            iID_MaNamNganSach = viID_MaNamNganSach;
            ExcelFile xls = CreateReport();
            using (FlexCelPdfExport pdf = new FlexCelPdfExport())
            {
                pdf.Workbook = xls;
                using (MemoryStream ms = new MemoryStream())
                {
                    pdf.BeginExport(ms);
                    pdf.ExportAllVisibleSheets(false, "BaoCao");
                    pdf.EndExport();
                    ms.Position = 0;
                    return File(ms.ToArray(), "application/pdf");
                }
            }
        }

        public clsExcelResult ExportToExcel(String viID_MaPhongBan, String viID_MaDonVi, String viThang_Quy, String viID_MaNamNganSach)
        {
            HamChung.Language();
            MaND = User.Identity.Name;
            iID_MaPhongBan = viID_MaPhongBan;
            iID_MaDonVi = viID_MaDonVi;
            iThang_Quy = viThang_Quy;
            iID_MaNamNganSach = viID_MaNamNganSach;
            clsExcelResult clsResult = new clsExcelResult();
            ExcelFile xls = CreateReport();

            using (MemoryStream ms = new MemoryStream())
            {
                xls.Save(ms);
                ms.Position = 0;
                clsResult.ms = ms;
                clsResult.FileName = "QuyetToan_BieuKiem.xls";
                clsResult.type = "xls";
                return clsResult;
            }
        }
        public JsonResult Ds_DonVi(String ParentID, String oiID_MaPhongBan, String oiThang_Quy, String oiID_MaNamNganSach, String oiID_MaDonVi, String oMaND)
        {
            string s = User.Identity.Name;
            return Json(obj_DonVi(ParentID, oiID_MaPhongBan, oiThang_Quy, oiID_MaNamNganSach, oiID_MaDonVi, oMaND), JsonRequestBehavior.AllowGet);
        }
        public String obj_DonVi(String ParentID, String oiID_MaPhongBan,String oiThang_Quy,String oiID_MaNamNganSach, String oiID_MaDonVi,String oMaND)
        {
            String input = "";
            DataTable dt = DonViModels.DanhSach_DonVi_QuyetToan_PhongBan(oiID_MaPhongBan, oiThang_Quy, oiID_MaNamNganSach,oMaND);
            SelectOptionList slDonvi = new SelectOptionList(dt, "iID_MaDonVi", "sTen");
            input = MyHtmlHelper.DropDownList(ParentID, slDonvi, iID_MaDonVi, "iID_MaDonVi", "", "class=\"input1_2\" style=\"width: 100%\"");
            return input;
        }
    }
}

