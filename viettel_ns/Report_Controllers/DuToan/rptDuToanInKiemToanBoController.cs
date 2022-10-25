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
    public class rptDuToanInKiemToanBoController : Controller
    {
        //
        // GET: /rptThuNop_4CC/
        public string sViewPath = "~/Report_Views/";
        private const String sFilePath = "/Report_ExcelFrom/DuToan/rptDuToanInKiemToanBo.xls";
        public  String sLNS, sL, sK, sM, sTM, sTTM, sNG, MaPhongBan, iID_MaDonVi, MaND;
        public ActionResult Index()
        {
            FlexCelReport fr = new FlexCelReport();
            ViewData["path"] = "~/Report_Views/DuToan/rptDuToanInKiemToanBo.aspx";
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
        /// VungNV: add new params MaPhongBan
        public  DataTable rptDuToanInKiemToanBo()
        {
            String DKDonVi = "", DKPhongBan = "", DK = "";
            SqlCommand cmd = new SqlCommand();

            //Begin: VungNV: 2015/09/21: add new condition of sLNS array
            if (String.IsNullOrEmpty(sLNS))
            {
                sLNS = "-100";
                DK += "sLNS=@sLNS";
                cmd.Parameters.AddWithValue("@sLNS", sLNS);
            }
            else
            {
                String[] arrSLN = sLNS.Split(',');
                for (int i = 0; i < arrSLN.Length; i++)
                {
                    DK += "sLNS=@sLNS" + i;
                    cmd.Parameters.AddWithValue("@sLNS" + i, arrSLN[i]);
                    if (i < arrSLN.Length - 1)
                    {
                        DK += " OR ";
                    }
                }
            }

            if (!String.IsNullOrEmpty(DK))
            {
                DK = " AND (" + DK + ")";
            }
            //End: VungNV: 2015/09/21: add new condition of sLNS array

            String DKDonViChon = "";
            if (!String.IsNullOrEmpty(iID_MaDonVi) && iID_MaDonVi != "-1")
            {
                String[] arrDonVi = iID_MaDonVi.Split(',');

                for (int i = 0; i < arrDonVi.Length; i++)
                {
                    DKDonViChon += "iID_MaDonVi=@iID_MaDonVix" + i;
                    cmd.Parameters.AddWithValue("@iID_MaDonVix" + i, arrDonVi[i]);
                    if (i < arrDonVi.Length - 1)
                    {
                        DKDonViChon += " OR ";
                    }
                }
                if (arrDonVi.Length>=1)
                {
                    DKDonViChon = " AND (" + DKDonViChon + ")";
                }
            }
            else
                DK = "1=0";
            //VungNV: add condition based on the MaPhongBan
            if (MaPhongBan != "-1")
            {
                DK += "AND iID_MaPhongBan=@MaPhongBan";
                cmd.Parameters.AddWithValue("@MaPhongBan", MaPhongBan);
            }
            if (!String.IsNullOrEmpty(sL))
            {
                DK += " AND sL=@sL";
                cmd.Parameters.AddWithValue("@sL", sL);
            }
            if (!String.IsNullOrEmpty(sK))
            {
                DK += " AND sK=@sK";
                cmd.Parameters.AddWithValue("@sK", sK);
            }
            if (!String.IsNullOrEmpty(sM))
            {
                DK += " AND sM=@sM";
                cmd.Parameters.AddWithValue("@sM", sM);
            }
            if (!String.IsNullOrEmpty(sTM))
            {
                DK += " AND sTM=@sTM";
                cmd.Parameters.AddWithValue("@sTM", sTM);
            }
            if (!String.IsNullOrEmpty(sTTM))
            {
                DK += " AND sTTM=@sTTM";
                cmd.Parameters.AddWithValue("@sTTM", sTTM);
            }
            if (!String.IsNullOrEmpty(sNG))
            {
                DK += " AND sNG=@sNG";
                cmd.Parameters.AddWithValue("@sNG", sNG);
            }

            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);

            String SQL =
                String.Format(@"
                            SELECT sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi,sTenDonVi
                            ,rTuChi=SUM(rTuChi/{3})
                            ,rHienVat=SUM(rHienVat/{3})
                            ,rPhanCap=SUM(rPhanCap/{3})
                            ,rDuPhong=SUM(rDuPhong/{3})
                            ,rHangNhap=SUM(rHangNhap/{3})
                            ,rHangMua=SUM(rHangMua/{3})
                            ,rTonKho=SUM(rTonKho/{3})    
                             ,rChiTapTrung=SUM(rChiTapTrung/{3})    
                             FROM DT_ChungTuChiTiet
                             WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec  AND (MaLoai='' OR MaLoai='2') {0} {1} {2} {4}
                             GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi,sTenDonVi
                             HAVING SUM(rTuChi)<>0 
                             OR SUM(rHienVat) <>0
                             OR SUM(rPhanCap)<>0
                             OR SUM(rDuPhong)<>0
                             OR SUM(rHangNhap)<>0
                             OR SUM(rHangMua)<>0
                             OR SUM(rTonKho)<>0 
                             OR SUM(rChiTapTrung)<>0 

                                UNION

                             SELECT sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi,sTenDonVi
                            ,rTuChi=SUM(rTuChi/{3})
                            ,rHienVat=SUM(rHienVat/{3})
                            ,rPhanCap=SUM(rPhanCap/{3})
                            ,rDuPhong=SUM(rDuPhong/{3})
                            ,rHangNhap=SUM(rHangNhap/{3})
                            ,rHangMua=SUM(rHangMua/{3})
                            ,rTonKho=SUM(rTonKho/{3})  
                             ,rChiTapTrung=SUM(rChiTapTrung/{3})      
                             FROM DT_ChungTuChiTiet_PhanCap
                             WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec  AND (MaLoai='' OR MaLoai='2') {0} {1} {2}  {4}
                             GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi,sTenDonVi
                             HAVING SUM(rTuChi)<>0 
                             OR SUM(rHienVat) <>0
                             OR SUM(rPhanCap)<>0
                             OR SUM(rDuPhong)<>0
                             OR SUM(rHangNhap)<>0
                             OR SUM(rHangMua)<>0
                             OR SUM(rTonKho)<>0
                              OR SUM(rChiTapTrung)<>0          
        
                            ", DK, DKDonVi, DKPhongBan,"1000",DKDonViChon);

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
        /// 
        public ActionResult EditSubmit(String ParentID)
        {
            String sLNS = Request.Form["sLNS"];
            String iID_MaDonVi = Request.Form["iID_MaDonVi"];
            String sL = Request.Form[ParentID + "_sL"];
            String sK = Request.Form[ParentID + "_sK"];
            String sM = Request.Form[ParentID + "_sM"];
            String sTM = Request.Form[ParentID + "_sTM"];
            String sTTM = Request.Form[ParentID + "_sTTM"];
            String sNG = Request.Form[ParentID + "_sNG"];

            //VungNV: Add new params MaPhongBan
            String MaPhongBan = Request.Form[ParentID + "_iID_MaPhongBan"];

            ViewData["PageLoad"] = "1";
            ViewData["sLNS"] = sLNS;
            ViewData["iID_MaDonVi"] = iID_MaDonVi;
            ViewData["sL"] = sL;
            ViewData["sK"] = sK;
            ViewData["sK"] = sK;
            ViewData["sM"] = sM;
            ViewData["sTM"] = sTM;
            ViewData["sTTM"] = sTTM;
            ViewData["sNG"] = sNG;
            //VungNV: Add new ViewData of MaPhongBan
            ViewData["MaPhongBan"] = MaPhongBan;
            ViewData["path"] = "~/Report_Views/DuToan/rptDuToanInKiemToanBo.aspx";
            return View(sViewPath + "ReportView.aspx");
        }

        /// <summary>
        /// Tạo báo cáo
        /// </summary>
        /// <param name="path"></param>
        /// <param name="NamLamViec"></param>
        /// <param name="iID_MaDonVi"></param>
        /// <returns></returns>
        /// VungNV: add new params MaPhongBan
        public ExcelFile CreateReport(String path)
        {
            XlsFile Result = new XlsFile(true);
            Result.Open(path);
            FlexCelReport fr = new FlexCelReport();
            fr = ReportModels.LayThongTinChuKy(fr, "rptDuToanInKiemToanBo");

            //VungNV: add new params MaPhongBan
            LoadData(fr);
            String Nam = ReportModels.LayNamLamViec(MaND);

          
            String sTenDonVi = DonViModels.Get_TenDonVi(iID_MaDonVi, MaND);
            fr.SetValue("BoQuocPhong", ReportModels.CauHinhTenDonViSuDung(1, MaND));
            fr.SetValue("QuanKhu", ReportModels.CauHinhTenDonViSuDung(2, MaND));
            fr.SetValue("NgayThang", ReportModels.Ngay_Thang_Nam_HienTai());
            fr.SetValue("Nam", Nam);
            fr.SetValue("sTenDonVi", sTenDonVi);
            fr.Run(Result);
            return Result;
        }

        /// <summary>
        /// Đổ dư liệu xuống báo cáo
        /// </summary>
        /// <param name="fr"></param>
        /// <param name="NamLamViec"></param>
        /// <param name="iID_MaDonVi"></param>
        /// VungNV: add new params MaPhongBan
        private void LoadData(FlexCelReport fr)
        {
            DataRow r;
            // VungNV: add new params MaPhongBan
            DataTable data = rptDuToanInKiemToanBo();
            data.TableName = "ChiTiet";
            fr.AddTable("ChiTiet", data);

          DataTable  dtsTM = HamChung.SelectDistinct("dtsTM", data, "sLNS,sL,sK,sM,sTM", "iID_MaDonVi,sTenDonVi,sLNS,sL,sK,sM,sTM,sMoTa", "sLNS,sL,sK,sM,sTM,sTTM");
          DataTable dtsM = HamChung.SelectDistinct("dtsM", dtsTM, "sLNS,sL,sK,sM", "iID_MaDonVi,sTenDonVi,sLNS,sL,sK,sM,sMoTa", "sLNS,sL,sK,sM,sTM");
          DataTable dtsL = HamChung.SelectDistinct("dtsL", dtsM, "sLNS,sL,sK", "iID_MaDonVi,sTenDonVi,sLNS,sL,sK,sMoTa", "sLNS,sL,sK,sM");
          DataTable dtsLNS = HamChung.SelectDistinct("dtsLNS", dtsL, "sLNS", "iID_MaDonVi,sTenDonVi,sLNS,sMoTa", "sLNS,sL");

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
        public static String LayMoTa(String sLNS)
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
        /// VungNV: add new params MaPhongBan
        public ActionResult ViewPDF(String iID_MaDonVi, String sLNS, String sL, String sK, String sM,String sTTM,String sNG,String MaPhongBan)
        {
            HamChung.Language();
            String sDuongDan = "";
            sDuongDan = sFilePath;
            this.iID_MaDonVi=iID_MaDonVi;
            this.sLNS = sLNS;
            this.sL = sL;
            this.sK = sK;
            this.sM = sM;
            this.sTTM = sTTM;
            this.sNG = sNG;
            this.MaPhongBan = MaPhongBan;
            this.MaND = User.Identity.Name;
            //VungNV: add new params MaPhongBan
            ExcelFile xls = CreateReport(Server.MapPath(sDuongDan));
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

        public clsExcelResult ExportToExcel(String MaND, String sLNS, String iThang_Quy, String iID_MaDonVi, String iID_MaNamNganSach, String MaPhongBan)
        {
            HamChung.Language();
            String sDuongDan = "";
            sDuongDan = sFilePath;
            clsExcelResult clsResult = new clsExcelResult();
            ExcelFile xls = CreateReport(Server.MapPath(sDuongDan));

            using (MemoryStream ms = new MemoryStream())
            {
                xls.Save(ms);
                ms.Position = 0;
                clsResult.ms = ms;
                clsResult.FileName = "SoSanhChiTieuQuyetToan.xls";
                clsResult.type = "xls";
                return clsResult;
            }
        }

        public JsonResult Ds_DonVi(String ParentID, String Thang_Quy, String iID_MaDonVi, String sLNS, String iID_MaNamNganSach, String iID_MaPhongBan)
        {
            String MaND = User.Identity.Name;
            DataTable dt = DuToan_ReportModels.dtDonVi(MaND,sLNS);
            if (String.IsNullOrEmpty(iID_MaDonVi))
            {
                iID_MaDonVi = Guid.Empty.ToString();
            }
            String ViewNam = "~/Views/DungChung/DonVi/DonVi_DanhSach.ascx";
            DanhSachDonViModels Model = new DanhSachDonViModels(MaND, iID_MaDonVi, dt, ParentID);
            String strDonVi = HamChung.RenderPartialViewToStringLoad(ViewNam, Model, this);
            return Json(strDonVi, JsonRequestBehavior.AllowGet);
        }

    }
}

