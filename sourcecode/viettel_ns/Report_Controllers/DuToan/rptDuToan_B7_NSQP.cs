using DomainModel;
using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VIETTEL.Controllers;
using VIETTEL.Flexcel;
using VIETTEL.Models;
using VIETTEL.Services;

namespace VIETTEL.Report_Controllers.DuToan
{
    public class rptDuToan_B7_NSQPController : AppController
    {
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private const string sFilePath = "/Report_ExcelFrom/DuToan/rptDuToan_B7_NSQP.xls";
        //private const string sFilePath_NSNN = "/Report_ExcelFrom/DuToan/rptDuToan_B7_NSNN.xls";

        #region view pdf and download excel

        /// <summary>
        /// hàm view PDF
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        /// 
        public ActionResult ViewPDF(string nam, string iID_MaPhongBan = null)
        {
            HamChung.Language();
            var xls = createReport(nam, iID_MaPhongBan);
            return xls.ToPdfContentResult(string.Format("{0}_{1}.pdf", this.ControllerName(), DateTime.Now.GetTimeStamp()));
        }

        public ActionResult Download(string nam)
        {
            HamChung.Language();
            var xls = createReport(nam);
            return xls.ToFileResult(string.Format("{0}_{1}.xls", this.ControllerName(), DateTime.Now.GetTimeStamp()));
        }

        #endregion

        private ExcelFile createReport(string nam, string iID_MaPhongBan = null)
        {
            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(sFilePath));
            var fr = new FlexCelReport();

            fillDataTable(fr, nam,iID_MaPhongBan);

            fr.SetValue("Nam", nam);
            fr.UseCommandValue()
              .Run(xls);

            return xls;
        }

        private DataTable getNSQP(string nam, string iID_MaPhongBan = null)
        {
            #region sql

            var sql = @"

select  sLNS1, sLNS3, sLNS5, sLNS,
        'smota' as sMoTa,
		DT.iID_MaDonVi, 
		DV.sTen as sTenDonVi,
		sum(CT_rTuChi) as CT_rTuChi,
		sum(CT_rBS) as CT_rBS,
		sum(QT_rTuChi) as QT_rTuChi
from
(

SELECT  sLNS1,sLNS3,sLNS5,sLNS,
        CT.iID_MaDonVi, 
		SUM(rTuChi) as CT_rTuChi, 
		SUM(rBS) as CT_rBS,
		0 as QT_rTuChi
FROM (

	SELECT		SUBSTRING(sLNS,1,1) as sLNS1,
                SUBSTRING(sLNS,1,3) as sLNS3,
                sLNS5=CASE WHEN sLNS=1020000 then 10201 else  SUBSTRING(sLNS,1,5) END ,
                sLNS =CASE WHEN sLNS=1020000 then 1020100 else sLNS END,
                SUM(rTuChi) as rTuChi,rBS=0,iID_MaDonVi
    FROM	DT_ChungTuChiTiet 
    WHERE	iTrangThai=1 AND iID_MaPhongBanDich=07 AND iNamLamViec=@iNamLamViec AND  (MaLoai='' OR MaLoai='2') 
    GROUP BY sLNS,iID_MaDonVi
    HAVING SUM(rTuChi)<>0 
            
	UNION ALL
            
	SELECT	SUBSTRING(sLNS,1,1) as sLNS1,
            SUBSTRING(sLNS,1,3) as sLNS3,
            sLNS5=CASE WHEN sLNS=1020000 then 10201 else  SUBSTRING(sLNS,1,5) END ,
            sLNS =CASE WHEN sLNS=1020000 then 1020100 else sLNS END ,
            SUM(rTuChi) as rTuChi,0,iID_MaDonVi
    FROM DT_ChungTuChiTiet_PhanCap 
    WHERE  iTrangThai=1 AND iID_MaPhongBanDich=07  AND iNamLamViec=@iNamLamViec AND (sLNS='1020100' OR sLNS='1020000' OR sLNS LIKE '207%') 
    GROUP BY sLNS,iID_MaDonVi
    HAVING SUM(rTuChi)<>0 OR SUM(rHienVat)<>0
                        
						
	UNION ALL

    SELECT SUBSTRING(sLNS,1,1) as sLNS1,
                SUBSTRING(sLNS,1,3) as sLNS3,
                sLNS5=CASE WHEN sLNS=1020000 then 10201 else  SUBSTRING(sLNS,1,5) END ,
                sLNS =CASE WHEN sLNS=1020000 then 1020100 else sLNS END ,
                0,SUM(rTuChi) as rBS,iID_MaDonVi
        FROM DTBS_ChungTuChiTiet 
        WHERE  iTrangThai=1 AND iID_MaPhongBanDich=07  AND iNamLamViec=@iNamLamViec AND (MaLoai='' OR MaLoai='2')
        GROUP BY sLNS,iID_MaDonVi
        HAVING SUM(rTuChi)<>0 OR SUM(rHienVat)<>0

    UNION ALL
    SELECT SUBSTRING(sLNS,1,1) as sLNS1,
            SUBSTRING(sLNS,1,3) as sLNS3,
            sLNS5=CASE WHEN sLNS=1020000 then 10201 else  SUBSTRING(sLNS,1,5) END ,
            sLNS =CASE WHEN sLNS=1020000 then 1020100 else sLNS END ,
            0,SUM(rTuChi) as rTuChi,iID_MaDonVi
    FROM DTBS_ChungTuChiTiet_PhanCap 
    WHERE iTrangThai=1 AND iID_MaPhongBanDich=07  AND iNamLamViec=@iNamLamViec AND (sLNS='1020100' OR sLNS='1020000' OR sLNS LIKE '207%') 
    GROUP BY sLNS,iID_MaDonVi
    HAVING SUM(rTuChi)<>0 OR SUM(rHienVat)<>0

) as ct

WHERE sLNS IN (1010000,1020100,1020000)
GROUP BY sLNS1,sLNS3, sLNS5,sLNS, CT.iID_MaDonVi
--ORDER BY sLNS,iID_MaDonVi



UNION ALL

-- QUYET TOAN

                   
SELECT	SUBSTRING(sLNS,1,1) as sLNS1,
        SUBSTRING(sLNS,1,3) as sLNS3,
        sLNS5=CASE WHEN sLNS=1020000 then 10201 else  SUBSTRING(sLNS,1,5) END ,
        sLNS =CASE WHEN sLNS=1020000 then 1020100 else sLNS END ,
        iID_MaDonVi,
		0 AS CT_rTuChi,
		0 as CT_rBS,
        SUM(rTuChi) as QT_rTuChi
FROM	QTA_ChungTuChiTiet
WHERE	iTrangThai=1 AND iID_MaPhongBan=07  AND iNamLamViec=@iNamLamViec AND 
	sLNS IN (1010000,1020100,1020000) AND 
	iTrangThai=1 AND 
	iThang_Quy IN (1,2)
GROUP BY sLNS,iID_MaDonVi
HAVING SUM(rTuChi)<>0 OR SUM(rHienVat)<>0
--order by iID_MaDonVi
-- END 101

UNION ALL

-- BEGIN 104,109
SELECT  sLNS1,sLNS3,sLNS5,sLNS,
        iID_MaDonVi, 
		SUM(rTuChi) as CT_rTuChi, 
		SUM(rBS) as CT_rBS,
		0 as QT_rTuChi
FROM (

	SELECT	SUBSTRING(sLNS,1,1) as sLNS1,
            SUBSTRING(sLNS,1,3) as sLNS3,
			SUBSTRING(sLNS,1,5) as sLNS5,
            sLNS,				
			iID_MaDonVi,
            SUM(rTuChi) as rTuChi,
			0 as rBS
    FROM	DT_ChungTuChiTiet 
    WHERE	iTrangThai=1 AND iID_MaPhongBanDich=07 AND iNamLamViec=@iNamLamViec 
            AND  (MaLoai='' OR MaLoai='2' OR sLNS = '1090500') 
            AND  sLNS IN (1040100,1090200, 1090400, 1090500, 1091300)
    GROUP BY sLNS,  iID_MaDonVi
    HAVING SUM(rTuChi)<>0 
            
	UNION ALL
            
	SELECT	SUBSTRING(sLNS,1,1) as sLNS1,
            SUBSTRING(sLNS,1,3) as sLNS3,
            SUBSTRING(sLNS,1,5) as sLNS5,
            sLNS,				
			iID_MaDonVi,
            SUM(rTuChi) as rTuChi,
			0 as rBS 
    FROM    DT_ChungTuChiTiet_PhanCap 
    WHERE  iTrangThai=1 AND iID_MaPhongBanDich=07  AND iNamLamViec=@iNamLamViec AND 
            sLNS IN (1040100,1090200, 1090400, 1090500, 1091300)
    GROUP BY sLNS,iID_MaDonVi
    HAVING SUM(rTuChi )<>0

	
	UNION ALL

    SELECT	SUBSTRING(sLNS,1,1) as sLNS1,
			SUBSTRING(sLNS,1,3) as sLNS3,
            SUBSTRING(sLNS,1,5) as sLNS5,
            sLNS,	
			iID_MaDonVi,
            0 as rTuChi,
			SUM(rTuChi) as rBS
    FROM DTBS_ChungTuChiTiet 
    WHERE  iTrangThai=1 AND iID_MaPhongBanDich=07  AND iNamLamViec=@iNamLamViec AND 
            (MaLoai='' OR MaLoai='2') AND 
            sLNS IN (1040100,1090200, 1090400, 1090500, 1091300)
    GROUP BY sLNS, iID_MaDonVi
    HAVING SUM(rTuChi)<>0 OR SUM(rHienVat)<>0

    UNION ALL
    SELECT	SUBSTRING(sLNS,1,1) as sLNS1,
            SUBSTRING(sLNS,1,3) as sLNS3,
            SUBSTRING(sLNS,1,5) as sLNS5,
            sLNS,	
			iID_MaDonVi,
            0 as rTuChi,
			SUM(rTuChi) as rTuChi
    FROM    DTBS_ChungTuChiTiet_PhanCap 
    WHERE   iTrangThai=1 AND iID_MaPhongBanDich=07  AND iNamLamViec=@iNamLamViec AND 
            sLNS IN (1040100,1090200, 1090400, 1090500, 1091300)
    GROUP BY sLNS, iID_MaDonVi
    HAVING SUM(rTuChi)<>0

) AS ct_104
--WHERE sLNS IN (1040100,1090200, 1090400, 1091300)
GROUP BY sLNS1,sLNS3, sLNS5,sLNS, iID_MaDonVi
--ORDER BY sLNS,iID_MaDonVi



UNION ALL

-- QUYET TOAN

                   
SELECT	SUBSTRING(sLNS,1,1) as sLNS1,
        SUBSTRING(sLNS,1,3) as sLNS3,
        SUBSTRING(sLNS,1,5) as sLNS5,
        sLNS,
        iID_MaDonVi,
		0 AS CT_rTuChi,
		0 as CT_rBS,
        SUM(rTuChi) as QT_rTuChi
FROM	QTA_ChungTuChiTiet
WHERE	iTrangThai=1 AND iID_MaPhongBan=07  AND iNamLamViec=@iNamLamViec AND 
	sLNS IN (1040100,1090200, 1090400, 1090500, 1091300) AND 
	iTrangThai=1 AND 
	iThang_Quy IN (1,2)
GROUP BY sLNS,iID_MaDonVi
HAVING SUM(rTuChi)<>0 OR SUM(rHienVat)<>0
--order by iID_MaDonVi


-- 1040200

UNION ALL
SELECT	SUBSTRING('1040200',1,1) as sLNS1,
			SUBSTRING('1040200',1,3) as sLNS3,
			SUBSTRING('1040200',1,5) as sLNS5,
			1040200 as sLNS,				
			iID_MaDonVi,
			SUM(rHangNhap) as rTuChi,
			0 as rBS,
            0 as QT_rTuChi 
FROM DT_ChungTuChiTiet
WHERE  iTrangThai=1 AND iID_MaPhongBanDich=07  AND iNamLamViec=2017 AND sLNS IN (1040100)
GROUP by sLNS, iID_MaDonVi

UNION ALL
SELECT	SUBSTRING('1040200',1,1) as sLNS1,
		SUBSTRING('1040200',1,3) as sLNS3,
		SUBSTRING('1040200',1,5) as sLNS5,
		1040200 as sLNS,				
		iID_MaDonVi,
		0 as rTuChi,
		SUM(rHangNhap) as rBS,
        0 as QT_rTuChi 
FROM DTBS_ChungTuChiTiet
WHERE  iTrangThai=1 AND iID_MaPhongBanDich=07  AND iNamLamViec=2017 AND sLNS IN (1040100)
GROUP by sLNS, iID_MaDonVi

-- END 1040200




) as DT

-- LAY TEN DON VI
JOIN NS_DonVi AS DV
ON DV.iID_MaDonVi = DT.iID_MaDonVi AND DV.iNamLamViec_DonVi = @iNamLamViec

group by sLNS, sLNS1, sLNS3, sLNS5, DT.iID_MaDonVi, DV.sTen
ORDER BY sLNS, DT.iID_MaDonVi


";

            #endregion

            #region sql2

            sql = @"

SELECT  sLNS1, 
		sLNS3, 
		sLNS5,
		sLNS,
        'smota' as sMoTa,
		DT.iID_MaDonVi, 
		DV.sTen as sTenDonVi,
		sum(CT_rTuChi) as CT_rTuChi,
		sum(CT_rBS) as CT_rBS,
		sum(QT_rTuChi) as QT_rTuChi
from
(

SELECT  sLNS1,sLNS3,sLNS5,sLNS,
        CT.iID_MaDonVi, 
		SUM(rTuChi) as CT_rTuChi, 
		SUM(rBS) as CT_rBS,
		0 as QT_rTuChi
FROM (

	-- DU TOAN
	SELECT		LEFT(sLNS,1) as sLNS1,
                LEFT(sLNS,3) as sLNS3,
                sLNS5=	CASE	WHEN sLNS=1020000 then 10201
							else  LEFT(sLNS,5) END, 
				sLNS=	CASE	WHEN sLNS=1020000 then 1020100
							else sLNS END,
				iID_MaDonVi,
                SUM(rTuChi) as rTuChi,
				0 as rBS
    FROM	DT_ChungTuChiTiet 
    WHERE	iTrangThai=1 AND iID_MaPhongBanDich=07 AND iNamLamViec=@iNamLamViec
			AND  (MaLoai='' OR MaLoai='2' OR sLNS = '1090500') 
    GROUP BY sLNS,iID_MaDonVi
    HAVING SUM(rTuChi)<>0 
	--order by sLNS, iID_MaDonVi
            
	UNION ALL
	-- DU TOAN - PHAN CAP
	SELECT	LEFT(sLNS,1) as sLNS1,
            LEFT(sLNS,3) as sLNS3,
            LEFT(sLNS,5) as sLNS5,
            sLNS,
			iID_MaDonVi,
            SUM(rTuChi) as rTuChi,
			0 as rBS
    FROM	DT_ChungTuChiTiet_PhanCap 
    WHERE	iTrangThai=1 AND iID_MaPhongBanDich=07  AND iNamLamViec=@iNamLamViec
			AND (sLNS IN (101000,1020100,1020000,1040100,1090200,1090400,1090500,1091300) OR sLNS LIKE '207%') 
    GROUP BY sLNS,iID_MaDonVi
    HAVING SUM(rTuChi)<>0
	--order by sLNS, iID_MaDonVi
                        
						
	UNION ALL
	-- BO SUNG
    SELECT	LEFT(sLNS,1) as sLNS1,
            LEFT(sLNS,3) as sLNS3,
            LEFT(sLNS,5) as sLNS5,
            sLNS,
			iID_MaDonVi,
			0 as rTuChi,
            SUM(rTuChi) as rBS
        FROM DTBS_ChungTuChiTiet 
        WHERE  iTrangThai=1 AND iID_MaPhongBanDich=07  AND iNamLamViec=@iNamLamViec
			AND (MaLoai='' OR MaLoai='2')
        GROUP BY sLNS,iID_MaDonVi
        HAVING SUM(rTuChi)<>0
	--order by sLNS, iID_MaDonVi

    UNION ALL

	SELECT	LEFT(sLNS,1) as sLNS1,
            LEFT(sLNS,3) as sLNS3,
            LEFT(sLNS,5) as sLNS5,
            sLNS,
			iID_MaDonVi,
            SUM(rTuChi) as rTuChi,
			0 as rBS
    FROM	DTBS_ChungTuChiTiet_PhanCap 
    WHERE	iTrangThai=1 AND iID_MaPhongBanDich=07  AND iNamLamViec=@iNamLamViec
			AND (sLNS IN (101000,1020100,1020000,1040100,1090200,1090400,1090500,1091300) OR sLNS LIKE '207%') 
    GROUP BY sLNS,iID_MaDonVi
    HAVING SUM(rTuChi)<>0 

) as ct


GROUP BY sLNS1,sLNS3, sLNS5,sLNS, CT.iID_MaDonVi
--ORDER BY sLNS,iID_MaDonVi



UNION ALL

-- QUYET TOAN                   
SELECT	LEFT(sLNS,1) as sLNS1,
        LEFT(sLNS,3) as sLNS3,
        LEFT(sLNS,5) as sLNS5,
        sLNS,
        iID_MaDonVi,
		0 AS CT_rTuChi,
		0 as CT_rBS,
        SUM(rTuChi) as QT_rTuChi
FROM	QTA_ChungTuChiTiet
WHERE	iTrangThai=1 AND iID_MaPhongBan=07  AND iNamLamViec=@iNamLamViec
		AND iThang_Quy IN (1,2)
GROUP BY sLNS,iID_MaDonVi
HAVING SUM(rTuChi)<>0 
--order by iID_MaDonVi
-- END 101
 
UNION ALL

-- QUYET TOAN
SELECT	LEFT(1040200,1) as sLNS1,
		LEFT(1040200,3) as sLNS3,
		LEFT(1040200,5) as sLNS5,
		1040200 as sLNS,				
		iID_MaDonVi,
		SUM(rHangNhap) as rTuChi,
		0 as rBS,
        0 as QT_rTuChi 
FROM DT_ChungTuChiTiet
WHERE  iTrangThai=1 AND iID_MaPhongBanDich=07  AND iNamLamViec=@iNamLamViec AND sLNS IN (1040100)
GROUP by sLNS, iID_MaDonVi
HAVING SUM(rHangNHap) <> 0

UNION ALL
SELECT	LEFT(1040200,1) as sLNS1,
		LEFT(1040200,3) as sLNS3,
		LEFT(1040200,5) as sLNS5,
		1040200 as sLNS,							
		iID_MaDonVi,
		0 as rTuChi,
		SUM(rHangNhap) as rBS,
        0 as QT_rTuChi 
FROM DTBS_ChungTuChiTiet
WHERE  iTrangThai=1 AND iID_MaPhongBanDich=07  AND iNamLamViec=@iNamLamViec AND sLNS IN (1040100)
GROUP by sLNS, iID_MaDonVi
HAVING SUM(rHangNHap) <> 0

-- END 1040200

) as DT

-- LAY TEN DON VI
JOIN NS_DonVi AS DV
ON DV.iID_MaDonVi = DT.iID_MaDonVi AND DV.iNamLamViec_DonVi = @iNamLamViec

where sLns3 in (101,102,104,109)
group by sLNS1, sLNS3, sLNS5,sLNS, DT.iID_MaDonVi, DV.sTen, sMoTa
ORDER BY sLNS, DT.iID_MaDonVi

";
            #endregion

            #region sql2

            sql = @"

SELECT  sLNS1, 
		sLNS3, 
		sLNS5,
		sLNS,
        'smota' as sMoTa,
		DT.iID_MaDonVi, 
		DV.sTen as sTenDonVi,
		sum(CT_rTuChi) as CT_rTuChi,
		sum(CT_rBS) as CT_rBS,
		sum(CP_rTuChi) as CP_rTuChi,
		sum(QT_rTuChi) as QT_rTuChi
from
(

SELECT  sLNS1,sLNS3,sLNS5,sLNS,
        CT.iID_MaDonVi, 
		SUM(rTuChi) as CT_rTuChi, 
		SUM(rBS) as CT_rBS,
		0 as CP_rTuChi,
		0 as QT_rTuChi
FROM (

	-- DU TOAN
	SELECT		LEFT(sLNS,1) as sLNS1,
                LEFT(sLNS,3) as sLNS3,
                sLNS5=	CASE	WHEN sLNS=1020000 then 10201
							else  LEFT(sLNS,5) END, 
				sLNS=	CASE	WHEN sLNS=1020000 then 1020100
							else sLNS END,
				iID_MaDonVi,
                SUM(rTuChi) as rTuChi,
				0 as rBS
    FROM	DT_ChungTuChiTiet 
    WHERE	iTrangThai=1 AND iID_MaPhongBanDich=07 AND iNamLamViec=@iNamLamViec
			AND  (MaLoai='' OR MaLoai='2' OR sLNS = '1090500') 
    GROUP BY sLNS,iID_MaDonVi
    HAVING SUM(rTuChi)<>0 
	--order by sLNS, iID_MaDonVi
            
	UNION ALL
	-- DU TOAN - PHAN CAP
	SELECT	LEFT(sLNS,1) as sLNS1,
            LEFT(sLNS,3) as sLNS3,
            LEFT(sLNS,5) as sLNS5,
            sLNS,
			iID_MaDonVi,
            SUM(rTuChi) as rTuChi,
			0 as rBS
    FROM	DT_ChungTuChiTiet_PhanCap 
    WHERE	iTrangThai=1 AND iID_MaPhongBanDich=07  AND iNamLamViec=@iNamLamViec
			AND (sLNS IN (101000,1020100,1020000,1040100,1090200,1090400,1090500,1091300) OR sLNS LIKE '207%') 
    GROUP BY sLNS,iID_MaDonVi
    HAVING SUM(rTuChi)<>0
	--order by sLNS, iID_MaDonVi
                        
						
	UNION ALL
	-- BO SUNG
    SELECT	LEFT(sLNS,1) as sLNS1,
            LEFT(sLNS,3) as sLNS3,
            LEFT(sLNS,5) as sLNS5,
            sLNS,
			iID_MaDonVi,
			0 as rTuChi,
            SUM(rTuChi) as rBS
        FROM DTBS_ChungTuChiTiet 
        WHERE  iTrangThai=1 AND iID_MaPhongBanDich=07  AND iNamLamViec=@iNamLamViec
			AND (MaLoai='' OR MaLoai='2')
        GROUP BY sLNS,iID_MaDonVi
        HAVING SUM(rTuChi)<>0
	--order by sLNS, iID_MaDonVi

    UNION ALL

	SELECT	LEFT(sLNS,1) as sLNS1,
            LEFT(sLNS,3) as sLNS3,
            LEFT(sLNS,5) as sLNS5,
            sLNS,
			iID_MaDonVi,
            SUM(rTuChi) as rTuChi,
			0 as rBS
    FROM	DTBS_ChungTuChiTiet_PhanCap 
    WHERE	iTrangThai=1 AND iID_MaPhongBanDich=07  AND iNamLamViec=@iNamLamViec
			AND (sLNS IN (101000,1020100,1020000,1040100,1090200,1090400,1090500,1091300) OR sLNS LIKE '207%') 
    GROUP BY sLNS,iID_MaDonVi
    HAVING SUM(rTuChi)<>0 

) as ct


GROUP BY sLNS1,sLNS3, sLNS5,sLNS, CT.iID_MaDonVi
--ORDER BY sLNS,iID_MaDonVi



UNION ALL

-- QUYET TOAN                   
SELECT	LEFT(sLNS,1) as sLNS1,
        LEFT(sLNS,3) as sLNS3,
        LEFT(sLNS,5) as sLNS5,
        sLNS,
        iID_MaDonVi,
		0 AS CT_rTuChi,
		0 as CT_rBS,
		0 as CP_rTuChi,
        SUM(rTuChi) as QT_rTuChi

FROM	QTA_ChungTuChiTiet
WHERE	iTrangThai=1 AND iID_MaPhongBan=07  AND iNamLamViec=@iNamLamViec
		AND iThang_Quy IN (1,2)
GROUP BY sLNS,iID_MaDonVi
HAVING SUM(rTuChi)<>0 
--order by iID_MaDonVi
-- END 101
 
UNION ALL

-- QUYET TOAN
SELECT	LEFT(1040200,1) as sLNS1,
		LEFT(1040200,3) as sLNS3,
		LEFT(1040200,5) as sLNS5,
		1040200 as sLNS,				
		iID_MaDonVi,
		SUM(rHangNhap) as rTuChi,
		0 as rBS,
		0 as CP_rTuChi,
        0 as QT_rTuChi

FROM DT_ChungTuChiTiet
WHERE  iTrangThai=1 AND iID_MaPhongBanDich=07  AND iNamLamViec=@iNamLamViec AND sLNS IN (1040100)
GROUP by sLNS, iID_MaDonVi
HAVING SUM(rHangNHap) <> 0

UNION ALL
SELECT	LEFT(1040200,1) as sLNS1,
		LEFT(1040200,3) as sLNS3,
		LEFT(1040200,5) as sLNS5,
		1040200 as sLNS,							
		iID_MaDonVi,
		0 as rTuChi,
		SUM(rHangNhap) as rBS,
		0 as CP_rTuChi,
        0 as QT_rTuChi

FROM DTBS_ChungTuChiTiet
WHERE  iTrangThai=1 AND iID_MaPhongBanDich=07  AND iNamLamViec=@iNamLamViec AND sLNS IN (1040100)
GROUP by sLNS, iID_MaDonVi
HAVING SUM(rHangNHap) <> 0

-- END 1040200


UNION ALL
-- CAP PHAT              
SELECT	LEFT(sLNS,1) as sLNS1,
        LEFT(sLNS,3) as sLNS3,
        LEFT(sLNS,5) as sLNS5,
        sLNS,
        iID_MaDonVi,
		0 AS CT_rTuChi,
		0 as CT_rBS,
		SUM(rTuChi) AS CP_rTuChi,
        0 as QT_rTuChi
FROM	CP_CapPhatChiTiet
WHERE	iTrangThai=1 AND iID_MaPhongBan=07  AND iNamLamViec=@iNamLamViec
GROUP BY sLNS,iID_MaDonVi
HAVING SUM(rTuChi)<>0 

) as DT

-- LAY TEN DON VI
JOIN NS_DonVi AS DV
ON DV.iID_MaDonVi = DT.iID_MaDonVi AND DV.iNamLamViec_DonVi = @iNamLamViec

where sLns3 in (101,102,104,109)
group by sLNS1, sLNS3, sLNS5,sLNS, DT.iID_MaDonVi, DV.sTen, sMoTa
ORDER BY sLNS, DT.iID_MaDonVi

";
            #endregion

            sql = @"



SELECT  sLNS1, 
		sLNS3, 
		sLNS5,
		sLNS,

	    'smota' as sMoTa,
		DT.iID_MaDonVi, 
		DV.sTen as sTenDonVi,
		sum(HT_Cap) as HT_Cap,
		sum(HT_QuyetToan) as HT_QuyetToan,
		sum(CT_NamTruoc) as CT_NamTruoc,
		sum(CT_DauNam) as CT_DauNam,
		sum(CT_BoSung) as CT_BoSung,
		sum(CP_Cap) as CP_Cap,
		sum(CP_Thu) as CP_Thu,
		sum(QT_rTuChi) as QT_rTuChi
from
(

-- HOP THUC NAM TRUOC
	-- CAP HOP THUC       
SELECT	LEFT(sLNS,1) as sLNS1,
        LEFT(sLNS,3) as sLNS3,
        LEFT(sLNS,5) as sLNS5,
        sLNS,
        iID_MaDonVi,
		sum(rTuChi) as HT_Cap,
		0 as HT_QuyetToan,
		0 as CT_NamTruoc,
		0 as CT_DauNam,
		0 as CT_BoSung,
		0 as CP_Cap,
		0 as CP_Thu,
		0 as QT_rTuChi
FROM	CP_CapPhatChiTiet
WHERE	iTrangThai=1 
        AND (@iID_MaPhongBanDich is NULL OR iID_MaPhongBan=@iID_MaPhongBanDich)  
        AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach = 1
GROUP BY sLNS,iID_MaDonVi
HAVING SUM(rTuChi)<>0 

UNION ALL

-- QUYET TOAN HOP THUC
SELECT	LEFT(sLNS,1) as sLNS1,
        LEFT(sLNS,3) as sLNS3,
        LEFT(sLNS,5) as sLNS5,
        sLNS,
        iID_MaDonVi,
		0 as HT_Cap,
		sum(rTuChi) as HT_QuyetToan,
		0 as CT_NamTruoc,
		0 as CT_DauNam,
		0 as CT_BoSung,
		0 as CP_Cap,
		0 as CP_Thu,
		0 as QT_rTuChi

FROM	QTA_ChungTuChiTiet
WHERE	iTrangThai=1 
        AND (@iID_MaPhongBanDich is NULL OR iID_MaPhongBan=@iID_MaPhongBanDich)  
        AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach = 1
		--AND iThang_Quy IN (1,2)
GROUP BY sLNS,iID_MaDonVi
HAVING SUM(rTuChi)<>0 

UNION ALL

-- CHI TIEU NAM TRUOC
-- du toan
SELECT  sLNS1,sLNS3,sLNS5,sLNS,
        CT_NAMTRUOC.iID_MaDonVi, 
		0 as HT_Cap,
		0 as HT_QuyetToan,
		sum(rTuChi) as CT_NamTruoc,
		0 as CT_DauNam,
		0 as CT_BoSung,
		0 as CP_Cap,
		0 as CP_Thu,
		0 as QT_rTuChi
FROM (

	

	-- DU TOAN
	SELECT	LEFT(sLNS,1) as sLNS1,
            sLNS3=	CASE	
					WHEN sLNS=1050100 then '101'
					else  LEFT(sLNS,3) END, 
            sLNS5=	CASE	
					WHEN sLNS=1050100 then '10100'
                    WHEN sLNS=1020000 then 10201
					else  LEFT(sLNS,5) END, 
			sLNS=	CASE	
					WHEN sLNS=1050100 then '1010000'
                    WHEN sLNS=1020000 then 1020100
					else sLNS END,
			iID_MaDonVi,
            SUM(rTuChi) as rTuChi

    FROM	DT_ChungTuChiTiet 
    WHERE	iTrangThai=1 
            AND (@iID_MaPhongBanDich is NULL OR iID_MaPhongBanDich=@iID_MaPhongBanDich)  
            AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach = 4
			AND  (MaLoai='' OR MaLoai='2') 
    GROUP BY sLNS,iID_MaDonVi
    HAVING SUM(rTuChi)<>0 
	--order by sLNS, iID_MaDonVi
            
	UNION ALL
	-- DU TOAN - PHAN CAP
	SELECT	LEFT(sLNS,1) as sLNS1,
            LEFT(sLNS,3) as sLNS3,
            LEFT(sLNS,5) as sLNS5,
            sLNS,

			iID_MaDonVi,
            SUM(rTuChi) as rTuChi
    FROM	DT_ChungTuChiTiet_PhanCap 
    WHERE	iTrangThai=1 
            AND (@iID_MaPhongBanDich is NULL OR iID_MaPhongBanDich=@iID_MaPhongBanDich)  
            AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach = 4
			--AND (sLNS IN (101000,1020100,1020000,1040100,1090200,1090400,1090500,1091300) OR sLNS LIKE '207%') 
    GROUP BY sLNS,iID_MaDonVi
    HAVING SUM(rTuChi)<>0
	--order by sLNS, iID_MaDonVi
   

) as CT_NAMTRUOC
GROUP BY sLNS1,sLNS3, sLNS5,sLNS, CT_NAMTRUOC.iID_MaDonVi

UNION ALL

-- du toan
SELECT  sLNS1,sLNS3,sLNS5,sLNS,
        CT.iID_MaDonVi, 
		0 as HT_Cap,
		0 as HT_QuyetToan,
		0 as CT_NamTruoc,
		sum(rTuChi) as CT_DauNam,
		sum(rBS) as CT_BoSung,
		0 as CP_Cap,
		0 as CP_Thu,
		0 as QT_rTuChi
FROM (

	-- DU TOAN
	SELECT		
			LEFT(sLNS,1) as sLNS1,
			--LEFT(sLNS,3) as sLNS3,
			--LEFT(sLNS,5) as sLNS5,
			--sLNS,
                
			sLNS3=	CASE	
					WHEN sLNS=1050100 then '101'
					else  LEFT(sLNS,3) END, 
            sLNS5=	CASE	
					WHEN sLNS=1050100 then '10100'
					else  LEFT(sLNS,5) END, 
			sLNS=	CASE	
					WHEN sLNS=1050100 then '1010000'
					else sLNS END,
			iID_MaDonVi,
            SUM(rTuChi) as rTuChi,
			0 as rBS
    FROM	DT_ChungTuChiTiet 
    WHERE	iTrangThai=1 
            AND (@iID_MaPhongBanDich is NULL OR iID_MaPhongBanDich=@iID_MaPhongBanDich)  
            AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach = 2
			AND  (MaLoai='' OR MaLoai='2') 
    GROUP BY sLNS,iID_MaDonVi
    HAVING SUM(rTuChi)<>0 
	--order by sLNS, iID_MaDonVi
            
	UNION ALL
	-- DU TOAN - PHAN CAP
	SELECT	LEFT(sLNS,1) as sLNS1,
            LEFT(sLNS,3) as sLNS3,
            LEFT(sLNS,5) as sLNS5,
            sLNS,
			iID_MaDonVi,
            SUM(rTuChi) as rTuChi,
			0 as rBS
    FROM	DT_ChungTuChiTiet_PhanCap 
    WHERE	iTrangThai=1 
            AND (@iID_MaPhongBanDich is NULL OR iID_MaPhongBanDich=@iID_MaPhongBanDich)  
            AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach = 2
			--AND (sLNS IN (101000,1020100,1020000,1040100,1090200,1090400,1090500,1091300) OR sLNS LIKE '207%' OR sLNS like '2%') 
    GROUP BY sLNS,iID_MaDonVi
    HAVING SUM(rTuChi)<>0
	--order by sLNS, iID_MaDonVi
                        
						
	UNION ALL
	-- BO SUNG
    SELECT	LEFT(sLNS,1) as sLNS1,
            sLNS3=	CASE	
					WHEN sLNS=1050100 then '101'
					else  LEFT(sLNS,3) END, 
            sLNS5=	CASE	
					WHEN sLNS=1050100 then '10100'
                    WHEN sLNS=1020000 then 10201
					else  LEFT(sLNS,5) END, 
			sLNS=	CASE	
					WHEN sLNS=1050100 then '1010000'
                    WHEN sLNS=1020000 then 1020100
					else sLNS END,
			iID_MaDonVi,
			0 as rTuChi,
            SUM(rTuChi) as rBS
        FROM DTBS_ChungTuChiTiet 
        WHERE  iTrangThai=1 
            AND (@iID_MaPhongBanDich is NULL OR iID_MaPhongBanDich=@iID_MaPhongBanDich)  
            AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach = 2
			AND (MaLoai='' OR MaLoai='2')
        GROUP BY sLNS,iID_MaDonVi
        HAVING SUM(rTuChi)<>0
	--order by sLNS, iID_MaDonVi

    UNION ALL

	SELECT	LEFT(sLNS,1) as sLNS1,
            LEFT(sLNS,3) as sLNS3,
            LEFT(sLNS,5) as sLNS5,
            sLNS,
			iID_MaDonVi,
            SUM(rTuChi) as rTuChi,
			0 as rBS
    FROM	DTBS_ChungTuChiTiet_PhanCap 
    WHERE	iTrangThai=1 
            AND (@iID_MaPhongBanDich is NULL OR iID_MaPhongBanDich=@iID_MaPhongBanDich)  
            AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach = 2
			--AND (sLNS IN (101000,1020100,1020000,1040100,1090200,1090400,1090500,1091300) OR sLNS LIKE '207%') 
    GROUP BY sLNS,iID_MaDonVi
    HAVING SUM(rTuChi)<>0 

) as ct


GROUP BY sLNS1,sLNS3, sLNS5,sLNS, CT.iID_MaDonVi
--ORDER BY sLNS,iID_MaDonVi



UNION ALL

-- QUYET TOAN                   
SELECT	LEFT(sLNS,1) as sLNS1,
        --LEFT(sLNS,3) as sLNS3,
        --LEFT(sLNS,5) as sLNS5,
        --sLNS,
        sLNS3=	CASE	
				WHEN sLNS=1050100 then '101'
				else  LEFT(sLNS,3) END, 
        sLNS5=	CASE	
				WHEN sLNS=1050100 then '10100'
                WHEN sLNS=1020000 then 10201
				else  LEFT(sLNS,5) END, 
		sLNS=	CASE	
				WHEN sLNS=1050100 then '1010000'
                WHEN sLNS=1020000 then 1020100
				else sLNS END,
        iID_MaDonVi,
		0 as HT_Cap,
		0 as HT_QuyetToan,
		0 as CT_NamTruoc,
		0 as CT_DauNam,
		0 as CT_BoSung,
		0 as CP_Cap,
		0 as CP_Thu,
		sum(rTuChi) as QT_rTuChi

FROM	QTA_ChungTuChiTiet
WHERE	iTrangThai=1 
        AND (@iID_MaPhongBanDich is NULL OR iID_MaPhongBan=@iID_MaPhongBanDich)  
        AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach = 2
		--AND iThang_Quy IN (1,2)
GROUP BY sLNS,iID_MaDonVi
HAVING SUM(rTuChi)<>0 
--order by iID_MaDonVi
-- END 101
 



UNION ALL
-- CAP PHAT              
SELECT	LEFT(sLNS,1) as sLNS1,
        --LEFT(sLNS,3) as sLNS3,
        --LEFT(sLNS,5) as sLNS5,
        --sLNS,
        sLNS3=	CASE	
				WHEN sLNS=1050100 then '101'
				else  LEFT(sLNS,3) END, 
        sLNS5=	CASE	
				WHEN sLNS=1050100 then '10100'
                WHEN sLNS=1020000 then 10201
				else  LEFT(sLNS,5) END, 
		sLNS=	CASE	
				WHEN sLNS=1050100 then '1010000'
                WHEN sLNS=1020000 then 1020100
				else sLNS END,
        iID_MaDonVi,
		0 as HT_Cap,
		0 as HT_QuyetToan,
		0 as CT_NamTruoc,
		0 as CT_DauNam,
		0 as CT_BoSung,
		sum(rTuChi) as CP_Cap,
		0 as CP_Thu,
		0 as QT_rTuChi
FROM	CP_CapPhatChiTiet
WHERE	iTrangThai=1 
        AND (@iID_MaPhongBanDich is NULL OR iID_MaPhongBan=@iID_MaPhongBanDich)  
        AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach = 2
GROUP BY sLNS,iID_MaDonVi
HAVING SUM(rTuChi)<>0 

) as DT

-- LAY TEN DON VI
JOIN NS_DonVi AS DV
ON DV.iID_MaDonVi = DT.iID_MaDonVi AND DV.iNamLamViec_DonVi = @iNamLamViec

where --sLns3 in (101,102,104,109) 
		(sLNS3 LIKE '1%'
		OR sLNS3 LIKE '2%' 
		OR sLNS3 LIKE '4%')
		and sLNS not in(1050000)
group by sLNS1, sLNS3, sLNS5,sLNS, DT.iID_MaDonVi, DV.sTen, sMoTa
ORDER BY sLNS, DT.iID_MaDonVi


";

            #region get data

            //using (var conn = new SqlConnection(ConnectionFactory.Default.ConnectionString))
            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("iID_MaPhongBanDich", string.IsNullOrWhiteSpace(iID_MaPhongBan) ? DBNull.Value : (object)iID_MaPhongBan);
                cmd.Parameters.AddWithValue("iNamLamViec", nam);
                var dt = Connection.GetDataTable(cmd);
                return dt;
            }

            #endregion
        }

        private void fillDataTable(FlexCelReport fr, string nam, string iID_MaPhongBan = null)
        {
            DataTable data = getNSQP(nam,iID_MaPhongBan);

            data.TableName = "ChiTiet";
            fr.AddTable("ChiTiet", data);
            DataTable dtsLNS = HamChung.SelectDistinct("dtsLNS", data, "sLNS1,sLNS3,sLNS5,sLNS", "sLNS1,sLNS3,sLNS5,sLNS,sMoTa", "sLNS");
            DataTable dtsLNS5 = HamChung.SelectDistinct("dtsLNS5", dtsLNS, "sLNS1,sLNS3,sLNS5", "sLNS1,sLNS3,sLNS5,sMoTa", "sLNS");
            DataTable dtsLNS3 = HamChung.SelectDistinct("dtsLNS3", dtsLNS5, "sLNS1,sLNS3", "sLNS1,sLNS3,sMoTa", "");
            DataTable dtsLNS1 = HamChung.SelectDistinct("dtsLNS1", dtsLNS5, "sLNS1", "sLNS1,sMoTa", "");

            //DataRow r;
            //for (int i = 0; i < dtsLNS3.Rows.Count; i++)
            //{
            //    r = dtsLNS3.Rows[i];
            //    //r["sMoTa"] = LayMoTa(Convert.ToString(r["sLNS3"]));
            //    r["sMoTa"] = _nganSachService.GetMLNS(r.Field<string>("sLNS3"));

            //}
            //for (int i = 0; i < dtsLNS5.Rows.Count; i++)
            //{
            //    r = dtsLNS5.Rows[i];
            //    r["sMoTa"] = _nganSachService.GetMLNS(r.Field<string>("sLNS5"));
            //}

            dtsLNS.AsEnumerable().ToList()
                .ForEach(r =>
                {
                    r["sMoTa"] = _nganSachService.GetMLNS_MoTa(r.Field<int>("sLNS").ToString());
                });

            //dtsLNS5.AsEnumerable().ToList()
            //    .ForEach(r =>
            //    {
            //        r["sMoTa"] = _nganSachService.GetMLNS_MoTa(r.Field<int>("sLNS5"));
            //    });
            dtsLNS3.AsEnumerable().ToList()
                 .ForEach(r =>
                 {
                     r["sMoTa"] = _nganSachService.GetMLNS_MoTa(r.Field<string>("sLNS3"));
                 });

            dtsLNS1.AsEnumerable().ToList()
                 .ForEach(r =>
                 {
                     r["sMoTa"] = _nganSachService.GetMLNS_MoTa(r.Field<string>("sLNS1"));
                 });

            fr.AddTable("dtsLNS", dtsLNS);
            fr.AddTable("dtsLNS5", dtsLNS5);
            fr.AddTable("dtsLNS3", dtsLNS3);
            fr.AddTable("dtsLNS1", dtsLNS1);

            data.Dispose(); 
            dtsLNS.Dispose();
            dtsLNS3.Dispose();
            dtsLNS5.Dispose();
            dtsLNS1.Dispose();

        }
    }
}