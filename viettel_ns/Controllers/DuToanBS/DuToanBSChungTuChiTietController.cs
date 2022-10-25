using System;
using System.Web.Mvc;
using DomainModel;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Data;
using VIETTEL.Models;
using System.IO;
using VIETTEL.Models.DuToanBS;
using System.Collections.Generic;
using Viettel.Data;
using Viettel.Services;
using Aspose.Cells;
using System.Globalization;

namespace VIETTEL.Controllers.DuToanBS
{
    public class DuToanBSChungTuChiTietController : Controller
    {
        //
        // GET: /DuToanBS_ChungTuChiTiet/
        private string sViewPath = "~/Views/DuToanBS/ChungTuChiTiet/";
        private const string DETAIL = "Detail";
        private const string EDIT = "Edit";
        private const string CREATE = "Create";
        private const string BANG_CHUNGTU = "";
        private const string BANG_CHUNGTU_CHITIET = "DTBS_ChungTuChiTiet";
        private const string PERMITION_MESSAGE = "PermitionMessage";
        private const string VIEW_CHUNGTU_CHITIET_INDEX = "ChungTuChiTiet_Index.aspx";
        private const string VIEW_CHUNGTU_CHITIET_FRAME = "ChungTuChiTiet_Index_DanhSach_Frame.aspx";
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;

        #region Index
        /// <summary>
        /// Index
        /// </summary>
        /// <param name="iID_MaChungTu"></param>
        /// <param name="sLNS"></param>
        /// <param name="iID_MaDonVi"></param>
        /// <param name="iLoai"></param>
        /// <param name="iChiTapTrung"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult Index(string iID_MaChungTu, string sLNS, string iID_MaDonVi, string iLoai, string iChiTapTrung)
        {
            //Kiểm tra quyền của người dùng với chức năng
            if (BaoMat.ChoPhepLamViec(User.Identity.Name, BANG_CHUNGTU_CHITIET, DETAIL) == false)
            {
                return RedirectToAction("Index", PERMITION_MESSAGE);
            }
            ViewData["iID_MaChungTu"] = iID_MaChungTu;
            ViewData["sLNS"] = sLNS;
            ViewData["iLoai"] = iLoai;
            ViewData["iID_MaDonVi"] = iID_MaDonVi;
            ViewData["iChiTapTrung"] = iChiTapTrung;

            return View(sViewPath + VIEW_CHUNGTU_CHITIET_INDEX);
        }
        #endregion

        #region Lưới chứng từ chi tiết
        /// <summary>
        /// Lưới chứng từ chi tiết
        /// </summary>
        /// <param name="sLNS"></param>
        /// <param name="MaLoai"></param>
        /// <param name="iLoai"></param>
        /// <param name="iChiTapTrung"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult ChungTuChiTietFrame(string sLNS, string sL, string sK, string sM, string sTM, string sTTM, string sNG, string sTNG, string iID_MaDonVi, string MaLoai, string iLoai, string iChiTapTrung)
        {
            //Kiểm tra quyền của người dùng với chức năng
            if (BaoMat.ChoPhepLamViec(User.Identity.Name, BANG_CHUNGTU_CHITIET, DETAIL) == false)
            {
                return RedirectToAction("Index", PERMITION_MESSAGE);
            }
            string iID_MaChungTu = Request.Form["DuToan_iID_MaChungTu"];
            if (String.IsNullOrEmpty(iLoai))
                iLoai = Request.Form["DuToan_iLoai"];
            MaLoai = Request.Form["DuToan_MaLoai"];
            ViewData["iID_MaChungTu"] = iID_MaChungTu;
            ViewData["MaLoai"] = MaLoai;
            ViewData["iLoai"] = iLoai;
            ViewData["iChiTapTrung"] = iChiTapTrung;
            ViewData["sLNS"] = sLNS;
            ViewData["sL"] = sL;
            ViewData["sK"] = sK;
            ViewData["sM"] = sM;
            ViewData["sTM"] = sTM;
            ViewData["sTTM"] = sTTM;
            ViewData["sNG"] = sNG;
            ViewData["sTNG"] = sTNG;
            ViewData["iID_MaDonVi"] = iID_MaDonVi;
            return View(sViewPath + VIEW_CHUNGTU_CHITIET_FRAME);
        }
        public ActionResult ChuyenSoLieu()
        {
            //Kiểm tra quyền của người dùng với chức năng
            if (BaoMat.ChoPhepLamViec(User.Identity.Name, BANG_CHUNGTU_CHITIET, DETAIL) == false)
            {
                return RedirectToAction("Index", PERMITION_MESSAGE);
            }
            string iID_MaChungTu = Request.Form["DuToanBS_iID_MaChungTu"];
            string iLoai = Request.Form["DuToanBS_iLoai"];
            string MaLoai = Request.Form["DuToanBS_MaLoai"];
            string iID_MaChungTuOld = Request.Form["DuToanBS_iID_MaChungTuOld"];
            string len = Request.Form["DuToanBS_len"];
            string dvt = Request.Form["DuToanBS_dvt"];
            ViewData["iID_MaChungTu"] = iID_MaChungTu;
            ViewData["MaLoai"] = MaLoai;
            ViewData["iLoai"] = iLoai;
            using (var conn = _connectionFactory.GetConnection())
            using (var cmd = new SqlCommand("sp_chuyendulieu_dtbs_old_new", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParams(new
                {
                    idold = iID_MaChungTuOld,
                    idnew = iID_MaChungTu,
                    lendv = len,
                    dvt = dvt,
                });
                conn.Open();
                var result = cmd.ExecuteNonQuery();
                conn.Close();
            }
            return View(sViewPath + VIEW_CHUNGTU_CHITIET_FRAME);
        }
    #endregion
    #region Cập nhật chứng từ chi tiết
    /// <summary>
    /// Cập nhật chứng từ chi tiết
    /// </summary>
    /// <param name="ChiNganSach">Chi ngân sách</param>
    /// <param name="iID_MaChungTu">Mã chứng từ</param>
    /// <param name="sLNS">Loại ngân sách</param>
    /// <param name="iLoai">Loại</param>
    /// <param name="iChiTapTrung">Chi tập trung</param>
    /// <returns></returns>
    [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CapNhatChungTuChiTiet(string ChiNganSach, string iID_MaChungTu, string sLNS, string iLoai, string iChiTapTrung)
        {
            NameValueCollection data;
            if (iLoai == "4")
            {
                data = DuToanBSChungTuModels.LayThongTinChungTuKyThuatLan2(iID_MaChungTu);
            }
            else
            {
                data = DuToanBSChungTuModels.LayThongTinChungTu(iID_MaChungTu);
            }

            string MaND = User.Identity.Name;
            string ipAddress = Request.UserHostAddress;
            //Lấy thông tin của Bang
            string idXauMaCacHang = Request.Form["idXauMaCacHang"];
            string idXauMaCacCot = Request.Form["idXauMaCacCot"];
            string idXauGiaTriChiTiet = Request.Form["idXauGiaTriChiTiet"];
            string idXauCacHangDaXoa = Request.Form["idXauCacHangDaXoa"];
            string idXauDuLieuThayDoi = Request.Form["idXauDuLieuThayDoi"];
            string idMaMucLucNganSach = Request.Form["idMaMucLucNganSach"];
            String DSTruong = "iID_MaDonVi," + MucLucNganSachModels.strDSTruong;
            String[] arrDSTruong = DSTruong.Split(',');
            Dictionary<String, String> arrGiaTriTimKiem = new Dictionary<string, string>();
            for (int i = 0; i < arrDSTruong.Length; i++)
            {
                arrGiaTriTimKiem.Add(arrDSTruong[i], Request.Form[arrDSTruong[i]]);
            }
            //Cập nhật vào database
            DuToanBSChungTuChiTietModels.CapNhatChungTuChiTiet(iID_MaChungTu, iLoai, iChiTapTrung, MaND, idXauDuLieuThayDoi, ipAddress, idMaMucLucNganSach, idXauMaCacHang, idXauCacHangDaXoa, idXauMaCacCot, idXauGiaTriChiTiet);

            //Kiểm tra trình duyệt/từ chối
            string idAction = Request.Form["idAction"];
            //Từ chối chứng từ.
            string sLyDo = Request.Form["sLyDo"];
            if (idAction == "1")
            {
                return RedirectToAction("TuChoiChungTu", "DuToanBSChungTu", new { maChungTu = iID_MaChungTu, sLNS = data["sDSLNS"], iKyThuat = data["iKyThuat"], sLyDo = sLyDo, chiTiet = "1" });
            }
            //Trình duyệt chứng từ.
            else if (idAction == "2")
            {
                return RedirectToAction("TrinhDuyetChungTu", "DuToanBSChungTu", new { maChungTu = iID_MaChungTu, sLNS = data["sDSLNS"], iKyThuat = data["iKyThuat"], sLyDo = sLyDo, chiTiet = "1" });
            }
            return RedirectToAction("ChungTuChiTietFrame", new { iID_MaChungTu = iID_MaChungTu, sLNS = sLNS, sL = arrGiaTriTimKiem["sL"], sK = arrGiaTriTimKiem["sK"], sM = arrGiaTriTimKiem["sM"], sTM = arrGiaTriTimKiem["sTM"], sTTM = arrGiaTriTimKiem["sTTM"], sNG = arrGiaTriTimKiem["sNG"], sTNG = arrGiaTriTimKiem["sTNG"], iID_MaDonVi = arrGiaTriTimKiem["iID_MaDonVi"], iLoai = iLoai, iChiTapTrung = iChiTapTrung });
        }
        #endregion

        #region Upload excel
        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UpLoadExcel(String iID_MaChungTu, String Loai, String iLoai)
        {
            string path = string.Empty;
            string sTenKhoa = "TMTL";
            path = TuLieuLichSuModels.ThuMucLuu(sTenKhoa);
            String sFileName = "";
            string newPath = AppDomain.CurrentDomain.BaseDirectory + path;
            if (Directory.Exists(newPath) == false)
            {
                Directory.CreateDirectory(newPath);
            }
            sFileName = Path.Combine(newPath, "IDFE__" + $"{DateTime.Now.ToBinary()}.xls");
            Request.Files["uploadFile"].SaveAs(sFileName);
            FileStream fstream = new FileStream(sFileName, FileMode.Open);
            Workbook workbook = new Workbook(fstream);
            var worksheet = workbook.Worksheets["Sheet1"];
            DataTable dt = null;
            // data from excel file
            int columnCount = worksheet.Cells.MaxDataColumn + 1;

            int rowCount = worksheet.Cells.MaxDataRow + 1;
            Range range = worksheet.Cells.CreateRange(0, 0, rowCount, columnCount);
           
            dt = range.ExportDataTable();
            fstream.Close();
            System.IO.File.Delete(sFileName);

            var data = new DataTable();
            data.Columns.Add("sLns", typeof(string));
            data.Columns.Add("sL", typeof(string));
            data.Columns.Add("sK", typeof(string));
            data.Columns.Add("sM", typeof(string));
            data.Columns.Add("sTM", typeof(string));
            data.Columns.Add("sTTM", typeof(string));
            data.Columns.Add("sNg", typeof(string));
            data.Columns.Add("iID_MaPhongBan", typeof(string));
            data.Columns.Add("iID_MaDonVi", typeof(string));
            data.Columns.Add("rTuChi", typeof(decimal));                       
            
            Dictionary<string,string> dsDv = new Dictionary<string, string>();
            for (int i = 9; i < dt.Columns.Count; i++)
            {
                dsDv.Add(i.ToString(), dt.Rows[0][i].ToString().Trim());
            }
            dt.Rows.RemoveAt(0);

            var items = dt.AsEnumerable().Where(x => x.Field<string>(0) == "x");
            for (int i = 9; i < dt.Columns.Count; i++)
            {
                var dvi = dsDv[i.ToString()];
                foreach (DataRow dtr in items)
                {
                    var xau = $"{dtr[1].ToString()}-{dtr[2].ToString()}-{dtr[3].ToString()}-{dtr[4].ToString()}-{dtr[5].ToString()}-{dtr[6].ToString()}-{dtr[7].ToString()}";
                    var value = Convert.ToDecimal(dtr[i].ToString(),CultureInfo.CurrentUICulture);
                    if (xau.Length == 31 && value != 0)
                    { 
                        var nr = data.NewRow();
                        nr["sLns"] = dtr[1];
                        nr["sL"] = dtr[2];
                        nr["sK"] = dtr[3];
                        nr["sM"] = dtr[4];
                        nr["sTM"] = dtr[5];
                        nr["sTTM"] = dtr[6];
                        nr["sNg"] = dtr[7];
                        nr["iID_MaPhongBan"] = dtr[8];
                        nr["iID_MaDonVi"] = dvi;
                        nr["rTuChi"] = value;
                        data.Rows.Add(nr);
                    }         
                }
            }

            if (data.Rows.Count > 0)
            {
                var rChungTu = DuToanBSChungTuModels.LayChungTu(iID_MaChungTu).Rows[0];

                int dvt = 1;

                var iID_MaNguonNganSach = rChungTu["iID_MaNguonNganSach"];
                var iID_MaNamNganSach = rChungTu["iID_MaNamNganSach"];
                var iID_MaPhongBan = rChungTu["iID_MaPhongBan"];
                var sTenPhongBan = rChungTu["sTenPhongBan"];
                var iNamLamViec = rChungTu["iNamLamViec"];
                var MaND = rChungTu["sID_MaNguoiDungTao"];
                var sLNS = rChungTu["sDSLNS"].ToString();
                SqlCommand cmd1;
                // Xoa du lieu cu
                string SQL = string.Format(@"DELETE DTBS_ChungTuChiTiet WHERE iID_MaChungTu=@iID_MaChungTu");
                cmd1 = new SqlCommand(SQL);
                cmd1.Parameters.AddWithValue("@iID_MaChungTu", rChungTu["iID_MaChungTu"]);
                Connection.UpdateDatabase(cmd1);

                //themdt
                foreach (DataRow dtr in data.Rows)
                {
                    var check = (sLNS.Contains(dtr["sLNS"].ToString()));
                    var value = Convert.ToDecimal(dtr["rTuChi"]) * dvt;
                    if (check)
                    { 
                        
                        SQL = String.Format(@"INSERT INTO DTBS_ChungTuChiTiet(iID_MaChungTu,iID_MaPhongBan,sTenPhongBan,iID_MaPhongBanDich,iID_MaTrangThaiDuyet,
                            iNamLamViec,iID_MaNguonNganSach,iID_MaNamNganSach,iID_MaDonVi,sTenDonVi,sXauNoiMa,sLNS,sL,sK,sM,sTM,sTTM,sNG,
                            sMoTa,rTonKho,rTuChi,rHangNhap,rHangMua,rHienVat,rPhanCap,rDuPhong,iID_MaNhomNguoiDung_DuocGiao,sID_MaNguoiDung_DuocGiao,sID_MaNguoiDungTao,sIPSua,sID_MaNguoiDungSua,iKyThuat) VALUES (@iID_MaChungTu,@iID_MaPhongBan,@sTenPhongBan,@iID_MaPhongBanDich,@iID_MaTrangThaiDuyet,@iNamLamViec,@iID_MaNguonNganSach,@iID_MaNamNganSach,
                            @iID_MaDonVi,@sTenDonVi,@sXauNoiMa,@sLNS,@sL,@sK,@sM,@sTM,@sTTM,@sNG,@sMoTa,@rTonKho,@rTuChi,@rHangNhap,@rHangMua,@rHienVat,@rPhanCap,@rDuPhong,@iID_MaNhomNguoiDung_DuocGiao,@sID_MaNguoiDung_DuocGiao,@sID_MaNguoiDungTao,@sIPSua,@sID_MaNguoiDungSua,@iKyThuat)");
                        cmd1 = new SqlCommand(SQL);
                        cmd1.Parameters.AddWithValue("@iID_MaChungTu", rChungTu["iID_MaChungTu"]);
                        cmd1.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan);
                        cmd1.Parameters.AddWithValue("@sTenPhongBan", sTenPhongBan);
                        string iID_MaDonVi = Convert.ToString(dtr["iID_MaDonVi"]);
                        string iID_MaPhongBan_Dich = Convert.ToString(dtr["iID_MaPhongBan"]);
                        cmd1.Parameters.AddWithValue("@iID_MaPhongBanDich", iID_MaPhongBan_Dich);
                        cmd1.Parameters.AddWithValue("@iID_MaTrangThaiDuyet", rChungTu["iID_MaTrangThaiDuyet"]);
                        cmd1.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                        cmd1.Parameters.AddWithValue("@iID_MaNguonNganSach", iID_MaNguonNganSach);
                        cmd1.Parameters.AddWithValue("@iID_MaNamNganSach", iID_MaNamNganSach);
                        cmd1.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
                        cmd1.Parameters.AddWithValue("@sTenDonVi", iID_MaDonVi + " - " + DonViModels.Get_TenDonVi(iID_MaDonVi));
                        cmd1.Parameters.AddWithValue("@sXauNoiMa", dtr["sLNS"] + "-" + dtr["sL"] + "-" + dtr["sK"] + "-" + dtr["sM"] + "-" + dtr["sTM"] + "-" + dtr["sTTM"] + "-" + dtr["sNG"]);
                        cmd1.Parameters.AddWithValue("@sLNS", dtr["sLNS"]);
                        cmd1.Parameters.AddWithValue("@sL", dtr["sL"]);
                        cmd1.Parameters.AddWithValue("@sK", dtr["sK"]);
                        cmd1.Parameters.AddWithValue("@sM", dtr["sM"]);
                        cmd1.Parameters.AddWithValue("@sTM", dtr["sTM"]);
                        cmd1.Parameters.AddWithValue("@sTTM", dtr["sTTM"]);
                        cmd1.Parameters.AddWithValue("@sNG", dtr["sNG"]);
                        cmd1.Parameters.AddWithValue("@sMoTa", "");                    
                        cmd1.Parameters.AddWithValue("@rTonKho", 0);                   
                        cmd1.Parameters.AddWithValue("@rTuChi", value);
                        cmd1.Parameters.AddWithValue("@rHangNhap", 0);                    
                        cmd1.Parameters.AddWithValue("@rHangMua", 0);                   
                        cmd1.Parameters.AddWithValue("@rHienVat", 0);                    
                        cmd1.Parameters.AddWithValue("@rPhanCap", 0);                    
                        cmd1.Parameters.AddWithValue("@rDuPhong", 0);
                        cmd1.Parameters.AddWithValue("@iID_MaNhomNguoiDung_DuocGiao", rChungTu["iID_MaNhomNguoiDung_DuocGiao"]);
                        cmd1.Parameters.AddWithValue("@sID_MaNguoiDung_DuocGiao", MaND);
                        cmd1.Parameters.AddWithValue("@sID_MaNguoiDungTao", MaND);
                        cmd1.Parameters.AddWithValue("@sIPSua", Request.UserHostAddress);
                        cmd1.Parameters.AddWithValue("@sID_MaNguoiDungSua", MaND);
                        cmd1.Parameters.AddWithValue("@iKyThuat", 0);
                        Connection.UpdateDatabase(cmd1);
                    }
                }

                // UPDATE MLNS, Ten don vi, 
                SQL = String.Format(@"UPDATE DTBS_ChungTuChiTiet
                    SET sTenPhongBan=iID_MaPhongBan+ ' - B'+ iID_MaPhongBan                    
                        ,sTenDonVi=(SELECT iID_MaDonVi + ' - ' + sTen FROM NS_DonVi WHERE iNamLamViec_Donvi=@iNamLamViec AND NS_DonVi.iID_MaDonVi=DTBS_ChungTuChiTiet.iID_MaDonVi)                    
                    WHERE iID_MaChungTu=@iID_MaChungTu;
                    UPDATE DTBS_ChungTuChiTiet
                    SET iID_MaMucLucNganSach_Cha=(SELECT iID_MaMucLucNganSach_Cha FROM NS_MucLucNganSach WHERE NS_MucLucNganSach.iTrangThai=1 AND iNamLamViec=@iNamLamViec AND DTBS_ChungTuChiTiet.sXauNoiMa=NS_MucLucNganSach.sXauNoiMa)
                        ,iID_MaMucLucNganSach=(SELECT iID_MaMucLucNganSach FROM NS_MucLucNganSach WHERE NS_MucLucNganSach.iTrangThai=1 AND iNamLamViec=@iNamLamViec AND  DTBS_ChungTuChiTiet.sXauNoiMa=NS_MucLucNganSach.sXauNoiMa)
                        ,sMoTa=(SELECT sMoTa FROM NS_MucLucNganSach WHERE NS_MucLucNganSach.iTrangThai=1 AND iNamLamViec=@iNamLamViec AND DTBS_ChungTuChiTiet.sXauNoiMa=NS_MucLucNganSach.sXauNoiMa)
                    WHERE iID_MaChungTu=@iID_MaChungTu AND EXISTS (SELECT * FROM NS_MucLucNganSach WHERE NS_MucLucNganSach.iTrangThai=1 AND iNamLamViec=@iNamLamViec AND DTBS_ChungTuChiTiet.sXauNoiMa=NS_MucLucNganSach.sXauNoiMa)");
                cmd1 = new SqlCommand(SQL);
                cmd1.Parameters.AddWithValue("@iID_MaChungTu", rChungTu["iID_MaChungTu"]);
                cmd1.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                Connection.UpdateDatabase(cmd1);
                
                cmd1.Dispose();                
            }
            return RedirectToAction("ChungTuChiTietFrame", "DuToanBSChungTuChiTiet", new { iID_MaChungTu = iID_MaChungTu });
        }
        #endregion

        #region Lấy 1 hang AJAX: rTongSoNamTruoc
        [Authorize]
        public JsonResult get_GiaTri(String Truong, String GiaTri, String DSGiaTri)
        {
            if (Truong == "rTongSoNamTruoc")
            {
                return get_GiaTriTongSoNamTruoc(GiaTri, DSGiaTri);
            }
            return null;
        }

        private JsonResult get_GiaTriTongSoNamTruoc(String GiaTri, String DSGiaTri)
        {
            String iID_MaChungTu = GiaTri;
            String strDSTruong = "iID_MaDonVi,sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG";
            String[] arrDSTruong = strDSTruong.Split(',');
            String[] arrDSGiaTri = DSGiaTri.Split(',');

            SqlCommand cmd = new SqlCommand();
            String DK = "";

            NameValueCollection data = DuToan_ChungTuModels.LayThongTin(iID_MaChungTu);
            //DK = String.Format("iNamLamViec={0} AND iID_MaNguonNganSach={1} AND iID_MaNamNganSach={2}", Convert.ToInt32(data["iNamLamViec"]) - 1, data["iID_MaNguonNganSach"], data["iID_MaNamNganSach"]);
            DK = "iNamLamViec=@iNamLamViec AND iID_MaNguonNganSach=@iID_MaNguonNganSach AND iID_MaNamNganSach=@iID_MaNamNganSach";
            cmd.Parameters.AddWithValue("@iNamLamViec", Convert.ToInt32(data["iNamLamViec"]) - 1);
            cmd.Parameters.AddWithValue("@iID_MaNguonNganSach", data["iID_MaNguonNganSach"]);
            cmd.Parameters.AddWithValue("@iID_MaNamNganSach", data["iID_MaNamNganSach"]);
            int i = 0;

            for (i = 0; i < arrDSGiaTri.Length; i++)
            {
                DK += String.Format(" AND {0}=@{0}", arrDSTruong[i]);
                cmd.Parameters.AddWithValue("@" + arrDSTruong[i], arrDSGiaTri[i]);
                //DK += String.Format(" AND {0}='{1}'", arrDSTruong[i], arrDSGiaTri[i]);
            }

            String SQL = String.Format("SELECT SUM(rTongSo) FROM DT_ChungTuChiTiet WHERE bLaHangCha=0 AND {0}", DK);
            cmd.CommandText = SQL;
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();

            Object item = new
            {
                value = "0",
                label = "0"
            };

            if (dt.Rows.Count > 0 && dt.Rows[0][0] != DBNull.Value)
            {
                item = new
                {
                    value = dt.Rows[0][0],
                    label = dt.Rows[0][0]
                };
            }
            dt.Dispose();

            return Json(item, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Lấy 1 hang AJAX: LayPhongBan
        [Authorize]
        public JsonResult GetPhongBanCuaDonVi(String iID_MaDonVi)
        {
            String MaDonVi = iID_MaDonVi.Split('-')[0];
            String iID_MaPhongBan = DonViModels.getPhongBanCuaDonVi(MaDonVi, User.Identity.Name);
            Object item = new
            {
                value = iID_MaPhongBan,
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
