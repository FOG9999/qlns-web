using System;
using System.Collections;
using System.Web.Mvc;
using FlexCel.Core;
using FlexCel.Report;
using FlexCel.Render;
using FlexCel.XlsAdapter;
using System.Data;
using System.Data.SqlClient;
using DomainModel;
using DomainModel.Abstract;
using DomainModel.Controls;
using VIETTEL.Models;
using VIETTEL.Models.ThuNop;
using VIETTEL.Controllers;
using System.IO;
using System.Text.RegularExpressions;

namespace VIETTEL.Report_Controllers.ThuNop {
    public class rptThuNopThongTriController : Controller
    {

        // GET: /rptThuNop_ThongTri/     

        public string sViewPath = "~/Report_Views/";
        private const String sFilePath = "~/Report_ExcelForm/ThuNop/rptThuNopThongTri.xls";        
        public int count = 0;
     
        public String LoaiTT = "";
        public String LoaiNS = "";
        public ActionResult Index()
        {
            if (HamChung.CoQuyenXemTheoMenu(Request.Url.AbsolutePath, User.Identity.Name))
            {
                HamRiengModels.UserDefault(User.Identity.Name);
                ViewData["path"] = "~/Report_Views/ThuNop/ThongTri/rptThuNop_ThongTri.aspx";
                ViewData["FilePath"] = Server.MapPath(sFilePath);
                return View(sViewPath + "ReportView.aspx");
            }
            else
            {
                return RedirectToAction("Index", "PermitionMessage");
            }
        }

        public ActionResult EditSubmit(String ParentID)
        {
            String iID_MaChungTu = Convert.ToString(Request.Form[ParentID + "_iID_MaChungTu"]).Trim();
            String iID_MaDonVi = Convert.ToString(Request.Form[ParentID + "_iID_MaDonVi"]).Trim();
            String sSoCT = Convert.ToString(Request.Form[ParentID + "_sSoCT"]).Trim();
            String iLoai = Convert.ToString(Request.Form[ParentID + "_iLoai"]).Trim();
            String sGhiChu = Convert.ToString(Request.Form[ParentID + "_sGhiChu"]).Trim();
            String iID_MaThongTri = Convert.ToString(Request.Form["ThongTri" + "_iID_MaThongTri"]);
            String chkThemMoi = Convert.ToString(Request.Form[ParentID + "_chkThemMoi"]);
            ViewData["iID_MaChungTu"] = iID_MaChungTu;
            ViewData["sSoCT"] = sSoCT;
            ViewData["iID_MaDonVi"] = iID_MaDonVi;
            ViewData["iID_MaThongTri"] = iID_MaThongTri;
            ViewData["sGhiChu"] = sGhiChu;
            ViewData["PageLoad"] = 1;
            ViewData["iLoai"] = iLoai;
                     
            sGhiChu=sGhiChu.Replace("\r\n", "&#10;");
            Bang bangTN = new Bang("TN_ChungTu");
            bangTN.MaNguoiDungSua = User.Identity.Name;
            bangTN.IPSua = Request.UserHostAddress;
            bangTN.GiaTriKhoa = iID_MaChungTu;
            bangTN.DuLieuMoi = false;
            bangTN.CmdParams.Parameters.AddWithValue("@sGhiChu", sGhiChu);
            bangTN.Save();
            
            if (iID_MaDonVi.Length < 2)
                return RedirectToAction("Index", "ThuNop_ChungTuChiTiet", new { iID_MaChungTu = iID_MaChungTu });
            else
            {
               
                if (chkThemMoi == "on")
                {           
                   
                    String sTen = Convert.ToString(Request.Form[ParentID + "_sTen"]);
                    String sNoiDung = Convert.ToString(Request.Form[ParentID + "_sNoiDung"]);
                    Bang bang = new Bang("KT_LoaiThongTri");
                    bang.MaNguoiDungSua = User.Identity.Name;
                    bang.IPSua = Request.UserHostAddress;
                  
                    bang.CmdParams.Parameters.AddWithValue("@iID_MaPhanHe", SharedModels.iID_MaPhanHe);
                    bang.CmdParams.Parameters.AddWithValue("@sLoaiKhoan", "");
                    bang.CmdParams.Parameters.AddWithValue("@sLoaiThongTri", sTen);
                    bang.CmdParams.Parameters.AddWithValue("@sTenLoaiNS", sNoiDung);
                    iID_MaThongTri = Convert.ToString(bang.Save());
                    ViewData["iID_MaThongTri"] = iID_MaThongTri;
                }
                ViewData["path"] = "~/Report_Views/ThuNop/ThongTri/rptThuNop_InThongTri.aspx";
                return View(sViewPath + "ReportView_NoMaster.aspx");                
               
            }
        }

        //Lấy dữ liệu
        public DataTable rptThuNop_ThongTri_DuLieu(String iID_MaChungTu, String iID_MaDonVi, String sSoCT,String iLoai)
        {
            SqlDataAdapter da;
            DataSet ds = new DataSet();
            //Khoi tao DataAdapter

            da = new SqlDataAdapter("[dbo].[TN_GetDataThongTri]", Connection.ConnectionString);

            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            da.SelectCommand.Parameters.Add("@sSoCT", SqlDbType.Char, 10);
            da.SelectCommand.Parameters["@sSoCT"].Value = sSoCT;
            da.SelectCommand.Parameters.Add("@iLoai", SqlDbType.Char, 10);
            da.SelectCommand.Parameters["@iLoai"].Value = iLoai;
            da.SelectCommand.Parameters.Add("@iID_MaDonVi", SqlDbType.NChar, 10);
            da.SelectCommand.Parameters["@iID_MaDonVi"].Value = iID_MaDonVi;
            da.SelectCommand.Parameters.Add("@iID_MaChungTu", SqlDbType.NVarChar, 50);
            da.SelectCommand.Parameters["@iID_MaChungTu"].Value = iID_MaChungTu;

            //Do du lieu vao dataset
            da.Fill(ds);

            return ds.Tables[0];              
           
        }
       
        public ExcelFile CreateReport(String path, String iID_MaChungTu, String iID_MaDonVi, String iID_MaThongTri, String sSoCT, String iLoai, String noiDung)
        {

            String MaND = User.Identity.Name;            
            XlsFile Result = new XlsFile(true);
            Result.Open(path);
            FlexCelReport fr = new FlexCelReport();
            fr = ReportModels.LayThongTinChuKy(fr, "rptThuNopThongTri",MaND);
            LoadData(fr, iID_MaChungTu, iID_MaDonVi, sSoCT, iLoai);
          
            ////lay ten thong tri
            if (!String.IsNullOrEmpty(iID_MaThongTri))
            {
                DataTable dtLoaiThongTri;
                String SQL = "SELECT sLoaiThongTri,sTenLoaiNS FROM KT_LoaiThongTri WHERE iID_MaThongTri=@iID_MaThongTri AND sID_MaNguoiDungTao=@sID_MaNguoiDungTao";
                SqlCommand cmd = new SqlCommand(SQL);
                cmd.Parameters.AddWithValue("@iID_MaThongTri", iID_MaThongTri);
                cmd.Parameters.AddWithValue("@sID_MaNguoiDungTao", MaND);
                dtLoaiThongTri = Connection.GetDataTable(cmd);
                cmd.Dispose();
                if (dtLoaiThongTri != null && dtLoaiThongTri.Rows.Count > 0)
                {
                    LoaiTT = Convert.ToString(dtLoaiThongTri.Rows[0]["sLoaiThongTri"]);
                    LoaiNS = Convert.ToString(dtLoaiThongTri.Rows[0]["sTenLoaiNS"]);
                    dtLoaiThongTri.Dispose();
                }
            }
            else
            {
                LoaiTT = "Thu nộp Ngân sách";
                LoaiNS = "Ngân sách...";
            }
            String sNgayLap = "", Nam = "", dNgayChungTu="";
            DataTable dt = ThuNop_ChungTuModels.GetChungTu(iID_MaChungTu);
           
            if (dt.Rows.Count > 0)
            {                
                dNgayChungTu = dt.Rows[0]["dNgayChungTu"].ToString();
                Nam = dt.Rows[0]["iNamLamViec"].ToString();
                dt.Dispose();
               
            }
            if (!String.IsNullOrEmpty(dNgayChungTu))
            {
                sNgayLap = " tháng  " + dNgayChungTu.Substring(3, 2) + " năm  " + Nam;
            }            
            fr.SetValue("NgayThangNam", "Ngày "+dNgayChungTu.Substring(0, 2)+ " tháng  " + dNgayChungTu.Substring(3, 2) + "  năm  " + dNgayChungTu.Substring(6, 4));
            fr.SetValue("NoiDung", noiDung);
            fr.SetValue("Loai", LoaiTT);
            fr.SetValue("LNS", LoaiNS);
            fr.SetValue("DonVi", DonViModels.Get_TenDonVi(iID_MaDonVi,MaND));
            fr.SetValue("dNgayLap", sNgayLap);                       
            fr.SetValue("Nam", Nam);
            fr.Run(Result);
            return Result;
        }

        private void LoadData(FlexCelReport fr, String iID_MaChungTu, String iID_MaDonVi, string sSoCT, String iLoai)
        {
            DataTable data = rptThuNop_ThongTri_DuLieu(iID_MaChungTu, iID_MaDonVi, sSoCT, iLoai);
            data.TableName = "ChiTiet";
            fr.AddTable("ChiTiet", data);    
                       
            int a = data.Rows.Count;
            
            int count = 15 - (a );
            DataTable dt = new DataTable();
            dt.Columns.Add("sGhiChu", typeof (String));
            int soChu1Trang = 48;            
            ArrayList   arrDongTong = new ArrayList();
            String sGhiChu = Convert.ToString(CommonFunction.LayTruong("TN_ChungTu", "iID_MaChungTu", iID_MaChungTu, "sGhiChu"));
            String[] arrDong = Regex.Split(sGhiChu, "&#10;");
            
            for (int i = 0; i < arrDong.Length; i++)
            {
                if (arrDong[i] != "")
                {
                    int tg = 0;
                    String s = "";
                    String[] arrDongCon = arrDong[i].Split(' ');
                    for (int j = 0; j < arrDongCon.Length; j++)
                    {
                         if (arrDongCon[j] != "")
                         {
                             int x = arrDongCon[j].Length;
                                 tg =tg+ x+1;
                             if(tg>soChu1Trang)
                             {
                                 arrDongTong.Add(s);
                                 j--;
                                 tg = 0;
                                 s = "";
                                 continue;
                             }
                             s += arrDongCon[j].Trim() + " ";
                         }
                    }
                    if (tg <= soChu1Trang) arrDongTong.Add(s);
                   
                }
            }
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    DataRow r = dt.NewRow();
                   
                        for (int j = count - i; j < count; j++)
                        {
                            if (j <= arrDongTong.Count)
                            {
                                r["sGhiChu"] = arrDongTong[arrDongTong.Count-j];
                            }
                            break;
                        }
                    dt.Rows.Add(r);
                }
            }
            fr.AddTable("dtDongTrang", dt);
            long TongTien = 0;
            for (int i = 0; i < data.Rows.Count; i++)
            {
                if (data.Rows[i]["rSoTien"].ToString() != "0")
                {
                    TongTien += long.Parse(data.Rows[i]["rSoTien"].ToString());
                }
            }
            fr.SetValue("Tien", CommonFunction.TienRaChu(TongTien));
            data.Dispose();
            
            dt.Dispose();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="iID_MaChungTu"></param>
        /// <param name="iID_MaDonVi"></param>
        /// <param name="LoaiTK"></param>
        /// <param name="iThang"></param>
        /// <param name="iNam"></param>
        /// <returns></returns>
        public ActionResult ViewPDF(String iID_MaChungTu, String iID_MaDonVi, String iID_MaThongTri, String sSoCT, String iLoai, String noiDung)
        {
            HamChung.Language();
            ExcelFile xls = CreateReport(Server.MapPath(sFilePath), iID_MaChungTu, iID_MaDonVi, iID_MaThongTri, sSoCT, iLoai, noiDung);
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

        public clsExcelResult ExportToExcel(String iID_MaChungTu, String iID_MaDonVi, String iID_MaThongTri, String sSoCT, String iLoai, String noiDung)
        {
            HamChung.Language();
            clsExcelResult clsResult = new clsExcelResult();
            ExcelFile xls = CreateReport(Server.MapPath(sFilePath), iID_MaChungTu, iID_MaDonVi, iID_MaThongTri, sSoCT, iLoai, noiDung);

            using (MemoryStream ms = new MemoryStream())
            {
                xls.Save(ms);
                ms.Position = 0;
                clsResult.ms = ms;
                clsResult.FileName = "ThuNop_DTNS_Na.xls";
                clsResult.type = "xls";
                return clsResult;
            }
        }

        public static DataTable getDSLoaiThongTri(String MaND)
        {
            String SQL = "SELECT * FROM KT_LoaiThongTri WHERE iTrangThai=1 AND sID_MaNguoiDungTao=@sID_MaNguoiDungTao AND iID_MaPhanHe=@iID_MaPhanHe ORDER BY dNgayTao";
            SqlCommand cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@iID_MaPhanHe", SharedModels.iID_MaPhanHe);
            cmd.Parameters.AddWithValue("@sID_MaNguoiDungTao", MaND);
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }
        public JsonResult Ds_DonVi(String ParentID, String iID_MaChungTu, String sSoCT, String iID_MaDonVi)
        {
            return Json(obj_DonVi(ParentID, iID_MaChungTu, sSoCT, iID_MaDonVi), JsonRequestBehavior.AllowGet);
        }
        public String obj_DonVi(String ParentID, String iID_MaChungTu, String sSoCT, String iID_MaDonVi)
        {
            String input = "";
            DataTable dt = DonViModels.DanhSach_DonVi_ThuNop(iID_MaChungTu, sSoCT);
            SelectOptionList slDonvi= new SelectOptionList(dt,"iID_MaDonVi","sTen");
            input = MyHtmlHelper.DropDownList(ParentID, slDonvi, iID_MaDonVi, "iID_MaDonVi", "", "class=\"input1_2\" style=\"width: 100%\"");
            return input;
        }

        [Authorize]
        public ActionResult Delete(String iID_MaThongTri, String iID_MaChungTu, String iLoai)
        {
            Bang bang = new Bang("KT_LoaiThongTri");
            bang.MaNguoiDungSua = User.Identity.Name;
            bang.IPSua = Request.UserHostAddress;
            bang.GiaTriKhoa = iID_MaThongTri;
            bang.Delete();

            return RedirectToAction("ThuNopChiTiet_Frame", "ThuNop_ChungTuChiTiet", new { iID_MaChungTu = iID_MaChungTu, iLoai = iLoai });
        }
    }
}
