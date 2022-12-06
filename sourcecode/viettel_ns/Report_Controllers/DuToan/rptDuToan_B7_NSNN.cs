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
    public class rptDuToan_B7_NSNNController : AppController
    {
        private readonly INganSachService _nganSachService = NganSachService.Default;

        private const string sFilePath = "/Report_ExcelFrom/DuToan/rptDuToan_B7_NSNN.xls";

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

        private ExcelFile createReport(string nam, string iID_MaPhongBanDich = null)
        {
            var xls = new XlsFile(true);
            xls.Open(Server.MapPath(sFilePath));
            var fr = new FlexCelReport();

            fillDataTable(fr, nam, iID_MaPhongBanDich);

            fr.SetValue("Nam", nam);
            fr.UseCommandValue()
              .Run(xls);

            return xls;
        }

        private DataTable getNSQP(string nam, string iID_MaPhongBanDich = null)
        {
            #region sql

            var sql = @"


SELECT  sLNS1,
        'sMoTa' as sMoTa,
		DT.iID_MaDonVi, 
		DV.sTen as sTenDonVi,
		sum(CT_rTuChi) as CT_rTuChi,
		sum(CT_rBS) as CT_rBS,
		sum(QT_rTuChi) as QT_rTuChi,
		sum(CP_rTuChi) as CP_rTuChi
from
(

SELECT  sLNS1,
		CT.iID_MaDonVi, 
		SUM(rTuChi) as CT_rTuChi, 
		SUM(rBS) as CT_rBS,
		0 as QT_rTuChi,
		0 as CP_rTuChi
FROM (

	-- DU TOAN
	SELECT		LEFT(sLNS,1) as sLNS1,
				iID_MaDonVi,
                SUM(rTuChi) as rTuChi,
				0 as rBS
    FROM	DT_ChungTuChiTiet 
    WHERE	iTrangThai=1 
            AND (@iID_MaPhongBanDich is NULL OR iID_MaPhongBanDich=@iID_MaPhongBanDich)  
            AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach = 2
			AND  (MaLoai='' OR MaLoai='2') 
            AND (sLNS LIKE '2%' OR sLNS LIKE '4%') 
    GROUP BY LEFT(sLNS,1),iID_MaDonVi
    HAVING SUM(rTuChi)<>0 
	--order by sLNS, iID_MaDonVi
            
	UNION ALL
	-- DU TOAN - PHAN CAP
	SELECT	LEFT(sLNS,1) as sLNS1,
			iID_MaDonVi,
            SUM(rTuChi) as rTuChi,
			0 as rBS
    FROM	DT_ChungTuChiTiet_PhanCap 
    WHERE	iTrangThai=1 
            AND (@iID_MaPhongBanDich is NULL OR iID_MaPhongBanDich=@iID_MaPhongBanDich)  
            AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach = 2
			AND (sLNS LIKE '2%' OR sLNS LIKE '4%') 
    GROUP BY LEFT(sLNS,1),iID_MaDonVi
    HAVING SUM(rTuChi)<>0
	--order by sLNS, iID_MaDonVi
                        
						
	UNION ALL
	-- BO SUNG
    SELECT	LEFT(sLNS,1) as sLNS1,
			iID_MaDonVi,
			0 as rTuChi,
            SUM(rTuChi) as rBS
        FROM DTBS_ChungTuChiTiet 
        WHERE  iTrangThai=1 
            AND (@iID_MaPhongBanDich is NULL OR iID_MaPhongBanDich=@iID_MaPhongBanDich)  
            AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach = 2
			AND (MaLoai='' OR MaLoai='2')
			AND (sLNS LIKE '2%' OR sLNS LIKE '4%') 
        GROUP BY LEFT(sLNS,1),iID_MaDonVi
        HAVING SUM(rTuChi)<>0
	--order by sLNS, iID_MaDonVi

    UNION ALL

	SELECT	LEFT(sLNS,1) as sLNS1,
			iID_MaDonVi,
            SUM(rTuChi) as rTuChi,
			0 as rBS
    FROM	DTBS_ChungTuChiTiet_PhanCap 
    WHERE	iTrangThai=1 
            AND (@iID_MaPhongBanDich is NULL OR iID_MaPhongBanDich=@iID_MaPhongBanDich)  
            AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach = 2
			AND (sLNS LIKE '2%' OR sLNS LIKE '4%') 
    GROUP BY LEFT(sLNS,1),iID_MaDonVi
    HAVING SUM(rTuChi)<>0 

) as ct


GROUP BY sLNS1, CT.iID_MaDonVi
--ORDER BY sLNS,iID_MaDonVi

UNION ALL

-- QUYET TOAN                   
SELECT	LEFT(sLNS,1) as sLNS1,
        iID_MaDonVi,
		0 AS CT_rTuChi,
		0 as CT_rBS,
        SUM(rTuChi) as QT_rTuChi,
		0 as CP_rTuChi
FROM	QTA_ChungTuChiTiet
WHERE	iTrangThai=1 
        AND (@iID_MaPhongBanDich is NULL OR iID_MaPhongBan=@iID_MaPhongBanDich)  
        AND iNamLamViec=@iNamLamViec
		--AND iThang_Quy IN (1,2)
		AND (sLNS LIKE '2%' OR sLNS LIKE '4%') 
GROUP BY LEFT(sLNS,1),iID_MaDonVi
HAVING SUM(rTuChi)<>0 
--order by iID_MaDonVi

UNION ALL
-- CAP PHAT
SELECT	LEFT(sLNS,1) as sLNS1,
        iID_MaDonVi,
		0 AS CT_rTuChi,
		0 as CT_rBS,
		0 as QT_rTuChi,
        SUM(rTuChi) as CP_rTuChi

FROM	CP_CapPhatChiTiet
WHERE	iTrangThai=1 
        AND (@iID_MaPhongBanDich is NULL OR iID_MaPhongBan=@iID_MaPhongBanDich)  
        AND iNamLamViec=@iNamLamViec
		AND (sLNS LIKE '2%' OR sLNS LIKE '4%') 
GROUP BY LEFT(sLNS,1),iID_MaDonVi
HAVING SUM(rTuChi)<>0 
 
) as DT

-- LAY TEN DON VI
JOIN NS_DonVi AS DV
ON DV.iID_MaDonVi = DT.iID_MaDonVi AND DV.iNamLamViec_DonVi = @iNamLamViec

--where sLns3 in (207)
GROUP BY sLNS1, DT.iID_MaDonVi, DV.sTen, sMoTa
ORDER BY sLNS1, DT.iID_MaDonVi



";

            #endregion

            #region get data

            //using (var conn = new SqlConnection(ConnectionFactory.Default.ConnectionString))
            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("iNamLamViec", nam);
                cmd.Parameters.AddWithValue("iID_MaPhongBanDich", string.IsNullOrWhiteSpace(iID_MaPhongBanDich) ? DBNull.Value : (object)iID_MaPhongBanDich);
                var dt = Connection.GetDataTable(cmd);
                return dt;
            }

            #endregion
        }

        private void fillDataTable(FlexCelReport fr, string nam, string iID_MaPhongBanDich = null)
        {
            DataTable data = getNSQP(nam, iID_MaPhongBanDich);

            data.TableName = "ChiTiet";
            fr.AddTable("ChiTiet", data);
            DataTable dtsLNS1 = HamChung.SelectDistinct("dtsLNS1", data, "sLNS1", "sLNS1,sMoTa", "");
           
            dtsLNS1.AsEnumerable().ToList()
                .ForEach(r =>
                {
                    r["sMoTa"] = _nganSachService.GetMLNS_MoTa(r.Field<string>("sLNS1"));
                });
  
            fr.AddTable("dtsLNS1", dtsLNS1);

            data.Dispose(); 
            dtsLNS1.Dispose();
        }
    }
}