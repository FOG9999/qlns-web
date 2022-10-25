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

namespace VIETTEL.Report_Controllers.DuToan
{
    public class rptDuToan_ChiTiet_TongHopController : Controller
    {

        public string sViewPath = "~/Report_Views/DuToan/";
        private const String sFilePath = "/Report_ExcelFrom/DuToan/rptDuToan_ChiTiet_TongHop.xls";


        public ActionResult Index()
        {
            if (HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath, User.Identity.Name))
            {
                ViewData["path"] = "~/Report_Views/DuToan/rptDuToan_ChiTiet_TongHop.aspx";
                ViewData["PageLoad"] = "0";
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
        public ActionResult EditSubmit(String ParentID)
        {
            String iCapTongHop = Request.Form[ParentID + "_iCapTongHop"];
            String iID_MaDonVi = Request.Form["iID_MaDonVi"];
            ViewData["PageLoad"] = "1";
            ViewData["iCapTongHop"] = iCapTongHop;
            ViewData["iID_MaDonVi"] = iID_MaDonVi;
            ViewData["path"] = "~/Report_Views/DuToan/rptDuToan_ChiTiet_TongHop.aspx";
            return View(sViewPath + "ReportView.aspx");
        }
        /// <summary>
        /// hàm khởi tạo báo cáo
        /// </summary>
        /// <param name="path"></param>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>

        public ExcelFile CreateReport(String path, String MaND, String iID_MaPhongBan, String iCapTongHop, String iID_MaDonVi)
        {
            DataTable dtCauHinh = NguoiDungCauHinhModels.LayCauHinh(MaND);
            String iNamLamViec = Convert.ToString(dtCauHinh.Rows[0]["iNamLamViec"]);
            XlsFile Result = new XlsFile(true);
            Result.Open(path);
            FlexCelReport fr = new FlexCelReport();
            LoadData(fr, MaND, iID_MaPhongBan, iCapTongHop, iID_MaDonVi);
            fr = ReportModels.LayThongTinChuKy(fr, "rptDuToan_ChiTiet_TongHop");
            fr.SetValue("Nam", iNamLamViec);
            String sTenDonVi = DonViModels.Get_TenDonVi(iID_MaDonVi, MaND);
            if (iCapTongHop != "1")
                fr.SetValue("sTenDonVi", "Đơn vị: " + sTenDonVi);
            else
            {
                fr.SetValue("sTenDonVi", "");
            }
            fr.SetValue("Cap1", ReportModels.CauHinhTenDonViSuDung(1, MaND));
            fr.SetValue("Cap2", ReportModels.CauHinhTenDonViSuDung(2, MaND));
            fr.SetValue("Ngay", ReportModels.Ngay_Thang_Nam_HienTai());
            fr.Run(Result);
            return Result;

        }
        
        //1020000
        /// <summary>
        /// Phụ lục 2c-c
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public static DataTable DT_rptDuToan_ChiTiet_TongHop(String MaND, String iID_MaPhongBan, String iCapTongHop, String iID_MaDonVi)
        {
            String DKDonVi = "", DKPhongBan = "", DK = "", DKBaoDam = ""; ;
            SqlCommand cmd = new SqlCommand();
            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan(MaND, cmd, "");
            int DVT = 1000;
            if (iCapTongHop == "1")
            {
                iID_MaDonVi = "-1";
            }
            else
            {
                string sql = String.Format(@"SELECT * FROM NS_MucLucNganSach_Nganh WHERE iNamLamViec=@iNamLamViec AND iTrangThai=1 AND iID_MaNganh = '" + iID_MaDonVi + "'");
                SqlCommand cmdNganh = new SqlCommand(sql);
                cmdNganh.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
                DataTable dtNganhChon = Connection.GetDataTable(cmdNganh);

                string iID_MaNganhMLNS = "";
                if (dtNganhChon.Rows.Count > 0)
                {
                    iID_MaNganhMLNS = Convert.ToString(dtNganhChon.Rows[0]["iID_MaNganhMLNS"]);
                }

                if (String.IsNullOrEmpty(iID_MaNganhMLNS))
                    DKBaoDam = " AND sNG IN (-1)";
                else
                {
                    DKBaoDam = " AND sNG IN (" + iID_MaNganhMLNS + ")";
                }
                DK += " AND iID_MaDonVi=@iID_MaDonVia";
                cmd.Parameters.AddWithValue("@iID_MaDonVia", iID_MaDonVi);
            }
            String SQL = String.Format(@"SELECT Cap1='A',sTenCap1=N'PHẦN THU',
	    Cap2='I',sTenCap2=N'THU NỘP NSQP',
	    Cap3='1',sTenCap3=N'Thu cân đối',
	    Cap4='1',sTenCap4=N'',
	    Cap5='1',sTenCap5=N'',
	    Cap6='1',sTenCap6=N'',
sLNS,iID_MaPhongBan,SUM(rTuChi)/1000 as rTien 
FROM DT_ChungTuChiTiet
WHERE iNamLamViec=@iNamLamViec {0} AND sLNS like '80101%' AND iTrangThai=1 
GROUP BY sLNS,iID_MaPhongBan
HAVING SUM(rTuChi)<>0
UNION
--thu quản lý
SELECT Cap1='A',sTenCap1=N'PHẦN THU',
		Cap2='I',sTenCap2=N'THU NỘP NSQP',
	    Cap3='2',sTenCap3=N'Thu quản lý',
	    Cap4='1',sTenCap4=N'',
	    Cap5='1',sTenCap5=N'',
	    Cap6='1',sTenCap6=N'',
	    sLNS,iID_MaPhongBan,SUM(rTuChi)/1000 as rTien 
FROM DT_ChungTuChiTiet
WHERE iNamLamViec=@iNamLamViec {0} AND sLNS like '80102%' AND iTrangThai=1
GROUP BY sLNS,iID_MaPhongBan
HAVING SUM(rTuChi)<>0
UNION
--thu nhà nước
SELECT Cap1='A',sTenCap1=N'PHẦN THU',
		Cap2='II',sTenCap2=N'THU NỘP NS NHÀ NƯỚC',
	    Cap3='1',sTenCap3=N'',
	    Cap4='1',sTenCap4=N'',
	    Cap5='1',sTenCap5=N'',
	    Cap6='1',sTenCap6=N'',
sLNS,iID_MaPhongBan,SUM(rTuChi)/1000 as rTien 
FROM DT_ChungTuChiTiet
WHERE iNamLamViec=@iNamLamViec {0} AND sLNS like '802%' AND iTrangThai=1
GROUP BY sLNS,iID_MaPhongBan
HAVING SUM(rTuChi)<>0


UNION
-- bao dam hang nhap
SELECT Cap1='B',sTenCap1=N'PHẦN CHI',
		Cap2='I',sTenCap2=N'NGÂN SÁCH QUỐC PHÒNG',
	    Cap3='1',sTenCap3=N'Phần ngân sách',
	    Cap4='A1',sTenCap4=N'Số chi bằng tiền- NSBĐ',
	    Cap5='1',sTenCap5=N'Ngoại tệ - NSBĐ',
	    Cap6='1',sTenCap6=N'',
sLNS,iID_MaPhongBan,SUM(rHangNhap)/1000 as rTien 
FROM DT_ChungTuChiTiet
WHERE iNamLamViec=@iNamLamViec {1} AND iTrangThai=1  AND( sLNS LIKE '104%') 
GROUP BY sLNS,iID_MaPhongBan 
HAVING SUM(rHangNhap)<>0
UNION
-- bao dam tu chi 
SELECT Cap1='B',sTenCap1=N'PHẦN CHI',
		Cap2='I',sTenCap2=N'NGÂN SÁCH QUỐC PHÒNG',
	    Cap3='1',sTenCap3=N'Phần ngân sách',
	    Cap4='A1',sTenCap4=N'Số chi bằng tiền- NSBĐ',
	    Cap5='2',sTenCap5=N'Tiền trong nước- NSBĐ',
	    Cap6='1',sTenCap6=N'Tự chi - NSBĐ',
sLNS,iID_MaPhongBan,SUM(rTuChi+rHangMua)/1000 as rTien 
FROM DT_ChungTuChiTiet
WHERE iNamLamViec=@iNamLamViec {1} AND iTrangThai=1  AND( sLNS LIKE '104%') 
GROUP BY sLNS,iID_MaPhongBan 
HAVING SUM(rTuChi+rHangMua)<>0
UNION
-- bao dam du phong
SELECT Cap1='B',sTenCap1=N'PHẦN CHI',
		Cap2='I',sTenCap2=N'NGÂN SÁCH QUỐC PHÒNG',
	    Cap3='1',sTenCap3=N'Phần ngân sách',
	    Cap4='A1',sTenCap4=N'Số chi bằng tiền- NSBĐ',
	    Cap5='2',sTenCap5=N'Tiền trong nước- NSBĐ',
	    Cap6='2',sTenCap6=N'Số chờ phân cấp- NSBĐ',
sLNS,iID_MaPhongBan,SUM(rDuPhong)/1000 as rTien 
FROM DT_ChungTuChiTiet
WHERE iNamLamViec=@iNamLamViec {1} AND iTrangThai=1  AND( sLNS LIKE '104%') 
GROUP BY sLNS,iID_MaPhongBan 
HAVING SUM(rDuPhong)<>0
UNION
-- bao dam phân cấp
SELECT Cap1='B',sTenCap1=N'PHẦN CHI',
		Cap2='I',sTenCap2=N'NGÂN SÁCH QUỐC PHÒNG',
	    Cap3='1',sTenCap3=N'Phần ngân sách',
	    Cap4='B1',sTenCap4=N'Số đã phân cấp cho đơn vị- NSBĐ',
	    Cap5='2',sTenCap5=N'',
	    Cap6='2',sTenCap6=N'',
sLNS,iID_MaPhongBan,SUM(rPhanCap)/1000 as rTien 
FROM DT_ChungTuChiTiet
WHERE iNamLamViec=@iNamLamViec {1} AND iTrangThai=1  AND( sLNS LIKE '104%') 
GROUP BY sLNS,iID_MaPhongBan 
HAVING SUM(rPhanCap)<>0
UNION
--tx
SELECT Cap1='B',sTenCap1=N'PHẦN CHI',
		Cap2='I',sTenCap2=N'NGÂN SÁCH QUỐC PHÒNG',
	    Cap3='1',sTenCap3=N'Phần ngân sách',
	    Cap4='A2',sTenCap4=N'Số chi bằng tiền- Lương phụ cấp tiền ăn',
	    Cap5='2',sTenCap5=N'Tiền trong nước- Lương phụ cấp tiền ăn',
	    Cap6='1',sTenCap6=N'',
sLNS,iID_MaPhongBan,SUM(rTuChi/1000) as rTien 
FROM DT_ChungTuChiTiet
WHERE iNamLamViec=@iNamLamViec {0} AND sLNS='1010000' AND iTrangThai=1
GROUP BY sLNS,iID_MaPhongBan
HAVING SUM(rTuChi)<>0
UNION
--nv tự chi
 SELECT Cap1='B',sTenCap1=N'PHẦN CHI',
		Cap2='I',sTenCap2=N'NGÂN SÁCH QUỐC PHÒNG',
	    Cap3='1',sTenCap3=N'Phần ngân sách',
	    Cap4='A3',sTenCap4=N'Số chi bằng tiền- Nghiệp vụ',
	    Cap5='2',sTenCap5=N'Tiền trong nước- Nghiệp vụ',
	    Cap6='1',sTenCap6=N'',
sLNS,iID_MaPhongBan,SUM(rTuChi)/1000 as rTien
FROM(
SELECT sLNS,iID_MaPhongBan,
rTuChi=SUM(rTuChi)
 FROM DT_ChungTuChiTiet
 WHERE iTrangThai=1  AND( sLNS like '102%') 
 AND iNamLamViec=@iNamLamViec {0} 
 GROUP BY sLNS,iID_MaPhongBan
UNION
SELECT sLNS,iID_MaPhongBan,
rTuChi=SUM(rTuChi)
 FROM DT_ChungTuChiTiet_PhanCap
 WHERE iTrangThai=1  AND( sLNS='1020000'  OR sLNS='1020100') 
 AND iNamLamViec=@iNamLamViec {0}
 GROUP BY sLNS,iID_MaPhongBan) a
 GROUP BY sLNS,iID_MaPhongBan
HAVING SUM(rTuChi)<>0
UNION

--nv hien vat 

SELECT Cap1='B',sTenCap1=N'PHẦN CHI',
		Cap2='I',sTenCap2=N'NGÂN SÁCH QUỐC PHÒNG',
	    Cap3='3',sTenCap3=N'HIỆN VẬT MUA NGÀNH',
	    Cap4='C3',sTenCap4=N'HIỆN VẬT MUA NGÀNH-Nghiệp vụ',
	    Cap5='1',sTenCap5=N'',
	    Cap6='1',sTenCap6=N'',
sLNS,iID_MaPhongBan,SUM(rHienVat)/1000 as rTien
FROM(
SELECT sLNS,iID_MaPhongBan,
rHienVat=SUM(rHienVat)
 FROM DT_ChungTuChiTiet
 WHERE iTrangThai=1  AND( sLNS like '102%') 
 AND iNamLamViec=@iNamLamViec {0} 
 GROUP BY sLNS,iID_MaPhongBan
UNION
SELECT sLNS,iID_MaPhongBan,
rHienVat=SUM(rHienVat)
 FROM DT_ChungTuChiTiet_PhanCap
 WHERE iTrangThai=1  AND( sLNS='1020000'  OR sLNS='1020100') 
 AND iNamLamViec=@iNamLamViec {0}
 GROUP BY sLNS,iID_MaPhongBan) a
 GROUP BY sLNS,iID_MaPhongBan
 HAVING SUM(rHienVat)<>0
 UNION
 --xdcb
SELECT Cap1='B',sTenCap1=N'PHẦN CHI',
		Cap2='I',sTenCap2=N'NGÂN SÁCH QUỐC PHÒNG',
	    Cap3='1',sTenCap3=N'Phần ngân sách',
	    Cap4='A4',sTenCap4=N'Số chi bằng tiền- XDCB',
	    Cap5='2',sTenCap5=N'Tiền trong nước- XDCB',
	    Cap6='1',sTenCap6=N'',
sLNS,iID_MaPhongBan,SUM(rTuChi)/1000 as rTien 
FROM DT_ChungTuChiTiet
WHERE iNamLamViec=@iNamLamViec {0} AND sLNS='1030100' AND iTrangThai=1
GROUP BY sLNS,iID_MaPhongBan 
 HAVING SUM(rTuChi)<>0
UNION
 --DN
SELECT Cap1='B',sTenCap1=N'PHẦN CHI',
		Cap2='I',sTenCap2=N'NGÂN SÁCH QUỐC PHÒNG',
	    Cap3='1',sTenCap3=N'Phần ngân sách',
	    Cap4='A5',sTenCap4=N'Số chi bằng tiền- doanh nghiệp',
	    Cap5='2',sTenCap5=N'Tiền trong nước- doanh nghiệp',
	    Cap6='1',sTenCap6=N'',
	    sLNS,iID_MaPhongBan,SUM(rTuChi/1000) as rTien 
FROM DT_ChungTuChiTiet
WHERE iNamLamViec=@iNamLamViec {0} AND sLNS LIKE '105%' AND iTrangThai=1
GROUP BY sLNS,iID_MaPhongBan 
HAVING SUM(rTuChi)<>0
UNION
 --Khac
SELECT Cap1='B',sTenCap1=N'PHẦN CHI',
		Cap2='I',sTenCap2=N'NGÂN SÁCH QUỐC PHÒNG',
	    Cap3='1',sTenCap3=N'Phần ngân sách',
	    Cap4='A6',sTenCap4=N'Số chi bằng tiền- ngân sách khác',
	    Cap5='2',sTenCap5=N'Tiền trong nước- ngân sách khác',
	    Cap6='1',sTenCap6=N'',
sLNS,iID_MaPhongBan,SUM(rTuChi/1000) as rTien 
FROM DT_ChungTuChiTiet
WHERE iNamLamViec=@iNamLamViec {0} AND sLNS LIKE '109%' AND iTrangThai=1
GROUP BY sLNS,iID_MaPhongBan 
HAVING SUM(rTuChi)<>0
UNION
 --Ton Kho
SELECT Cap1='B',sTenCap1=N'PHẦN CHI',
		Cap2='I',sTenCap2=N'NGÂN SÁCH QUỐC PHÒNG',
	    Cap3='2',sTenCap3=N'Phần sử dụng tồn kho',
	    Cap4='1',sTenCap4=N'',
	    Cap5='1',sTenCap5=N'',
	    Cap6='1',sTenCap6=N'',
sLNS,iID_MaPhongBan,SUM(rTonKho/1000) as rTien 
FROM DT_ChungTuChiTiet
WHERE iNamLamViec=@iNamLamViec {0} AND iTrangThai=1 AND rTonKho<>0
GROUP BY sLNS,iID_MaPhongBan 
HAVING SUM(rTonKho)<>0
UNION
--Nha nuoc

SELECT Cap1='B',sTenCap1=N'PHẦN CHI',
		Cap2='II',sTenCap2=N'NGÂN SÁCH NHÀ NƯỚC GIAO',
	    Cap3='1',sTenCap3=N'',
	    Cap4='1',sTenCap4=N'',
	    Cap5='1',sTenCap5=N'',
	    Cap6='1',sTenCap6=N'',
sLNS,iID_MaPhongBan,SUM(rTuChi)/1000 as rTien
FROM(
SELECT sLNS,iID_MaPhongBan,SUM(rTuChi) as rTuChi 
FROM DT_ChungTuChiTiet
WHERE iNamLamViec=@iNamLamViec {0} AND sLNS LIKE '2%' AND iTrangThai=1
GROUP BY sLNS,iID_MaPhongBan 

UNION

SELECT sLNS,iID_MaPhongBan,SUM(rTuChi) as rTuChi 
FROM DT_ChungTuChiTiet_PhanCap
WHERE iNamLamViec=@iNamLamViec {0} AND sLNS LIKE '2%' AND iTrangThai=1
GROUP BY sLNS,iID_MaPhongBan ) a
GROUP BY sLNS,iID_MaPhongBan
HAVING SUM(rTuChi)<>0
UNION
-- KH khac
SELECT Cap1='B',sTenCap1=N'PHẦN CHI',
		Cap2='III',sTenCap2=N'KINH PHI KHÁC',
	    Cap3='1',sTenCap3=N'',
	    Cap4='1',sTenCap4=N'',
	    Cap5='1',sTenCap5=N'',
	    Cap6='1',sTenCap6=N'',
sLNS,iID_MaPhongBan,SUM(rTuChi/1000) as rTien 
FROM DT_ChungTuChiTiet
WHERE iNamLamViec=@iNamLamViec {0} AND iTrangThai=1  AND( sLNS LIKE '4%') 
GROUP BY sLNS,iID_MaPhongBan 
HAVING SUM(rTuChi)<>0

ORDER BY Cap1,Cap2,Cap3,Cap4,Cap5,Cap6,iID_MaPhongBan,sLNS", DK, DKBaoDam, DKDonVi, DVT);
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }

        public JsonResult Ds_DonVi(String ParentID, String iID_MaDonVi)
        {
            String MaND = User.Identity.Name;
            DataTable dt = DuToan_ReportModels.dtDonViAll(MaND, "ALL");
            if (String.IsNullOrEmpty(iID_MaDonVi))
            {
                iID_MaDonVi = Guid.Empty.ToString();
            }
            String ViewNam = "~/Views/DungChung/DonVi/DonVi_DanhSach.ascx";
            DanhSachDonViModels Model = new DanhSachDonViModels(MaND, iID_MaDonVi, dt, ParentID);
            String strDonVi = HamChung.RenderPartialViewToStringLoad(ViewNam, Model, this);
            return Json(strDonVi, JsonRequestBehavior.AllowGet);
        }

        public static String LayMoTa(String sLNS)
        {
            String sMoTa = "";

            String SQL = String.Format(@"SELECT sMoTa FROM NS_MucLucNganSach WHERE iTrangThai=1 AND sLNS={0} AND sL='' AND LEN(sLNS)=7", sLNS);
            sMoTa = Connection.GetValueString(SQL, "");
            return sMoTa;
        }
        /// <summary>
        /// hàm hiển thị dữ liệu ra báo cáo
        /// </summary>
        /// <param name="fr"></param>
        /// <param name="NamLamViec"></param>
        private void LoadData(FlexCelReport fr, String MaND, String iID_MaPhongBan, String iCapTongHop, String iID_MaDonVi)
        {
            DataRow r;
            DataTable data = DT_rptDuToan_ChiTiet_TongHop(MaND, iID_MaPhongBan, iCapTongHop, iID_MaDonVi);
            String sMoTa = "";
            for (int i = 0; i < data.Rows.Count; i++)
            {
                sMoTa = data.Rows[i]["sLNS"] + " - " + LayMoTa(Convert.ToString(data.Rows[i]["sLNS"]));
                data.Rows[i]["sLNS"] = sMoTa;
            }
            data.TableName = "ChiTiet";
            fr.AddTable("ChiTiet", data);
            DataTable dtPhongBan = HamChung.SelectDistinct("dtPhongBan", data, "Cap1,Cap2,Cap3,Cap4,Cap5,Cap6,iID_MaPhongBan", "Cap1,sTenCap1,Cap2,sTenCap2,Cap3,sTenCap3,Cap4,sTenCap4,Cap5,sTenCap5,Cap6,sTenCap6,iID_MaPhongBan", "");
            DataTable dtCap6 = HamChung.SelectDistinct("dtCap6", dtPhongBan, "Cap1,Cap2,Cap3,Cap4,Cap5,Cap6", "Cap1,sTenCap1,Cap2,sTenCap2,Cap3,sTenCap3,Cap4,sTenCap4,Cap5,sTenCap5,Cap6,sTenCap6", "");
            DataTable dtCap5 = HamChung.SelectDistinct("dtCap5", dtCap6, "Cap1,Cap2,Cap3,Cap4,Cap5", "Cap1,sTenCap1,Cap2,sTenCap2,Cap3,sTenCap3,Cap4,sTenCap4,Cap5,sTenCap5", "");
            DataTable dtCap4 = HamChung.SelectDistinct("dtCap4", dtCap5, "Cap1,Cap2,Cap3,Cap4", "Cap1,sTenCap1,Cap2,sTenCap2,Cap3,sTenCap3,Cap4,sTenCap4", "");
            DataTable dtCap3 = HamChung.SelectDistinct("dtCap3", dtCap4, "Cap1,Cap2,Cap3", "Cap1,sTenCap1,Cap2,sTenCap2,Cap3,sTenCap3", "");
            DataTable dtCap2 = HamChung.SelectDistinct("dtCap2", dtCap3, "Cap1,Cap2", "Cap1,sTenCap1,Cap2,sTenCap2", "");
            DataTable dtCap1 = HamChung.SelectDistinct("dtCap1", dtCap2, "Cap1", "Cap1,sTenCap1", "");


            fr.AddTable("dtPhongBan", dtPhongBan);
            fr.AddTable("Cap6", dtCap6);
            fr.AddTable("Cap5", dtCap5);
            fr.AddTable("Cap4", dtCap4);
            fr.AddTable("Cap3", dtCap3);
            fr.AddTable("Cap2", dtCap2);
            fr.AddTable("Cap1", dtCap1);




            data.Dispose();


        }

        /// <summary>
        /// Hàm xuất dữ liệu ra file excel
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public clsExcelResult ExportToExcel(String MaND, String iID_MaPhongBan, String iCapTongHop, String iID_MaDonVi)
        {
            clsExcelResult clsResult = new clsExcelResult();
            ExcelFile xls = CreateReport(Server.MapPath(sFilePath), MaND, iID_MaPhongBan, iCapTongHop, iID_MaDonVi);

            using (MemoryStream ms = new MemoryStream())
            {
                xls.Save(ms);
                ms.Position = 0;
                clsResult.ms = ms;
                clsResult.FileName = "DuToan_1010000_TongHop.xls";
                clsResult.type = "xls";
                return clsResult;
            }

        }
        /// <summary>
        /// hàm view PDF
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <returns></returns>
        public ActionResult ViewPDF(String MaND, String iID_MaPhongBan, String iCapTongHop, String iID_MaDonVi)
        {
            HamChung.Language();
            ExcelFile xls = CreateReport(Server.MapPath(sFilePath), MaND, iID_MaPhongBan, iCapTongHop, iID_MaDonVi);
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
    }
}

