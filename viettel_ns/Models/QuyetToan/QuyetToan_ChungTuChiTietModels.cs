using DomainModel;
using DomainModel.Abstract;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using Viettel.Data;
using Viettel.Services;
using VIETTEL.Helpers;

namespace VIETTEL.Models
{
    public class QuyetToan_ChungTuChiTietModels
    {
        /// <summary>
        /// ThemChiTiet
        /// </summary>
        /// <param name="iID_MaChungTu"></param>
        /// <param name="MaND"></param>
        /// <param name="IPSua"></param>
        public static void ThemChiTiet(String iID_MaChungTu, String MaND, String IPSua)
        {
            String[] arrDSTruongTien = MucLucNganSachModels.strDSTruongTien.Split(',');
            DataTable dtChungTu = QuyetToan_ChungTuModels.GetChungTu(iID_MaChungTu);

            String iID_MaChiTieu = Convert.ToString(dtChungTu.Rows[0]["iID_MaChungTu"]);
            int iNamLamViec = Convert.ToInt32(dtChungTu.Rows[0]["iNamLamViec"]);
            int iID_MaNguonNganSach = Convert.ToInt32(dtChungTu.Rows[0]["iID_MaNguonNganSach"]);
            int iID_MaNamNganSach = Convert.ToInt32(dtChungTu.Rows[0]["iID_MaNamNganSach"]);
            Boolean bChiNganSach = Convert.ToBoolean(dtChungTu.Rows[0]["bChiNganSach"]);
            String sLNS = "";

            sLNS = Convert.ToString(dtChungTu.Rows[0]["sDSLNS"]);

            if (sLNS != "")
            {
                String[] arrsLNS = sLNS.Split(',');
                int i;
                for (i = 0; i < arrsLNS.Length; i++)
                {
                    if (arrsLNS[i] != "")
                    {
                        DataTable dt = NganSach_HamChungModels.DT_MucLucNganSach_sLNS(arrsLNS[i]);

                        Bang bang = new Bang("QTA_ChungTuChiTiet");
                        bang.GiaTriKhoa = null;
                        bang.DuLieuMoi = true;
                        bang.MaNguoiDungSua = MaND;
                        bang.IPSua = IPSua;
                        bang.CmdParams.Parameters.AddWithValue("@iID_MaChungTu", iID_MaChungTu);
                        bang.CmdParams.Parameters.AddWithValue("@iID_MaPhongBan", dtChungTu.Rows[0]["iID_MaPhongBan"]);
                        bang.CmdParams.Parameters.AddWithValue("@iID_MaTrangThaiDuyet", dtChungTu.Rows[0]["iID_MaTrangThaiDuyet"]);
                        bang.CmdParams.Parameters.AddWithValue("@iNamLamViec", dtChungTu.Rows[0]["iNamLamViec"]);
                        bang.CmdParams.Parameters.AddWithValue("@iID_MaNguonNganSach", dtChungTu.Rows[0]["iID_MaNguonNganSach"]);
                        bang.CmdParams.Parameters.AddWithValue("@iID_MaNamNganSach", dtChungTu.Rows[0]["iID_MaNamNganSach"]);
                        bang.CmdParams.Parameters.AddWithValue("@bChiNganSach", dtChungTu.Rows[0]["bChiNganSach"]);
                        bang.CmdParams.Parameters.AddWithValue("@iThang_Quy", dtChungTu.Rows[0]["iThang_Quy"]);
                        bang.CmdParams.Parameters.AddWithValue("@bLoaiThang_Quy", dtChungTu.Rows[0]["bLoaiThang_Quy"]);
                        bang.CmdParams.Parameters.AddWithValue("@iID_MaDonVi", dtChungTu.Rows[0]["iID_MaDonVi"]);

                        for (int j = 0; j < dt.Rows.Count; j++)
                        {
                            //Dien thong tin cua Muc luc ngan sach
                            NganSach_HamChungModels.ThemThongTinCuaMucLucNganSach(dt.Rows[j], bang.CmdParams.Parameters);
                            //Xet rieng ngan sach thuong xuyen
                            if (Convert.ToString(dt.Rows[j]["sLNS"]) == "1010000")
                            {
                                bang.CmdParams.Parameters["@brNgay"].Value = true;
                                bang.CmdParams.Parameters["@brSoNguoi"].Value = true;
                            }

                            //Lấy số liệu tự động từ Lương sang
                            bang.Save();
                        }
                        dt.Dispose();
                        dtChungTu.Dispose();

                        DataTable dtChungTuChiTietVuaTao = Get_dtChungTuChiTiet(iID_MaChungTu);

                        //Update lại các giá trị Chỉ tiêu cho bảng vừa tạo                        
                        Update_TruongChiTieu(dtChungTuChiTietVuaTao);

                        //Update lại các giá trị Đã quyết toán cho bảng vừa tạo
                        Update_TruongDaQuyetToan(dtChungTuChiTietVuaTao);

                        //Update dữ liệu cho trường rTuChi từ Lương chuyển sang
                        Update_TuChiTuLuong(dtChungTuChiTietVuaTao);
                    }
                }
            }
        }
        /// <summary>
        /// Update_TruongDuToan
        /// </summary>
        /// <param name="dt"></param>
        public static void Update_TruongDuToan(DataTable dt)
        {
            SqlCommand cmd;
            DataRow R;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                R = dt.Rows[i];
                String iThang = "";
                switch (Convert.ToInt32(R["bLoaiThang_Quy"]))
                {
                    case 0:
                        iThang = Convert.ToString(R["iThang_Quy"]);
                        break;
                    case 1:
                        Int32 iQuy = Convert.ToInt32(R["iThang_Quy"]);
                        switch (iQuy)
                        {
                            case 1:
                                iThang = "3";
                                break;
                            case 2:
                                iThang = "6";
                                break;
                            case 3:
                                iThang = "9";
                                break;
                            case 4:
                                iThang = "12";
                                break;
                        }
                        break;
                    case 2:
                        iThang = "12";
                        break;
                }
                DataTable dtDuToan = Get_dtTongDuToanChoDonVi(Convert.ToString(R["iID_MaMucLucNganSach"]), Convert.ToString(R["iID_MaDonVi"]), Convert.ToString(R["iNamLamViec"]), Convert.ToString(R["iID_MaNguonNganSach"]), Convert.ToString(R["iID_MaNamNganSach"]), iThang);
                cmd = new SqlCommand();
                String DK = "iTrangThai = 1";
                DK += " AND iNamLamViec=@iNamLamViec";
                cmd.Parameters.AddWithValue("@iNamLamViec", Convert.ToString(R["iNamLamViec"]));
                DK += " AND iThang_Quy=@iThang_Quy";
                cmd.Parameters.AddWithValue("@iThang_Quy", Convert.ToString(R["iThang_Quy"]));
                DK += " AND bLoaiThang_Quy=@bLoaiThang_Quy";
                cmd.Parameters.AddWithValue("@bLoaiThang_Quy", Convert.ToString(R["bLoaiThang_Quy"]));
                DK += " AND iID_MaMucLucNganSach=@iID_MaMucLucNganSach";
                cmd.Parameters.AddWithValue("@iID_MaMucLucNganSach", Convert.ToString(R["iID_MaMucLucNganSach"]));
                DK += " AND iID_MaDonVi=@iID_MaDonVi";
                cmd.Parameters.AddWithValue("@iID_MaDonVi", Convert.ToString(R["iID_MaDonVi"]));
                DK += " AND iID_MaChungTu=@iID_MaChungTu";
                cmd.Parameters.AddWithValue("@iID_MaChungTu", Convert.ToString(R["iID_MaChungTu"]));

                String[] arrDSTruongTien_So = MucLucNganSachModels.strDSTruongTien_So.Split(',');
                String strTruong = "";
                for (int j = 0; j < arrDSTruongTien_So.Length; j++)
                {
                    if (j > 0) strTruong += ",";
                    Double rValues = 0;
                    if (dtDuToan != null)
                    {
                        if (Convert.ToString(dtDuToan.Rows[0][arrDSTruongTien_So[j]]) != null && Convert.ToString(dtDuToan.Rows[0][arrDSTruongTien_So[j]]) != "")
                        {
                            rValues = Convert.ToDouble(dtDuToan.Rows[0][arrDSTruongTien_So[j]]);
                        }
                    }
                    strTruong += String.Format("{0}=@{1}", arrDSTruongTien_So[j] + "_DonVi", arrDSTruongTien_So[j] + "_DonVi");
                    cmd.Parameters.AddWithValue(String.Format("@{0}", arrDSTruongTien_So[j] + "_DonVi"), rValues);
                }
                cmd.CommandText = String.Format("UPDATE QTA_ChungTuChiTiet SET {0} WHERE {1}", strTruong, DK);
                Connection.UpdateDatabase(cmd);
                cmd.Dispose();
                dtDuToan.Dispose();
            }
        }
        /// <summary>
        /// Update  trường chỉ tiêu 
        /// </summary>
        /// <param name="dt"></param>
        public static void Update_TruongChiTieu(DataTable dt)
        {
            SqlCommand cmd;
            DataRow R;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                R = dt.Rows[i];
                String iThang = "";
                switch (Convert.ToInt32(R["bLoaiThang_Quy"]))
                {
                    case 0:
                        iThang = Convert.ToString(R["iThang_Quy"]);
                        break;
                    case 1:
                        Int32 iQuy = Convert.ToInt32(R["iThang_Quy"]);
                        switch (iQuy)
                        {
                            case 1:
                                iThang = "3";
                                break;
                            case 2:
                                iThang = "6";
                                break;
                            case 3:
                                iThang = "9";
                                break;
                            case 4:
                                iThang = "12";
                                break;
                        }
                        break;
                    case 2:
                        iThang = "12";
                        break;
                }
                DataTable dtChiTieu = Get_dtTongCapPhatChoDonVi(Convert.ToString(R["iID_MaMucLucNganSach"]), Convert.ToString(R["iID_MaDonVi"]), Convert.ToString(R["iNamLamViec"]), Convert.ToString(R["iID_MaNguonNganSach"]), Convert.ToString(R["iID_MaNamNganSach"]), iThang);
                cmd = new SqlCommand();
                String DK = "iTrangThai = 1";
                DK += " AND iNamLamViec=@iNamLamViec";
                cmd.Parameters.AddWithValue("@iNamLamViec", Convert.ToString(R["iNamLamViec"]));
                DK += " AND iThang_Quy=@iThang_Quy";
                cmd.Parameters.AddWithValue("@iThang_Quy", Convert.ToString(R["iThang_Quy"]));
                DK += " AND bLoaiThang_Quy=@bLoaiThang_Quy";
                cmd.Parameters.AddWithValue("@bLoaiThang_Quy", Convert.ToString(R["bLoaiThang_Quy"]));
                DK += " AND iID_MaMucLucNganSach=@iID_MaMucLucNganSach";
                cmd.Parameters.AddWithValue("@iID_MaMucLucNganSach", Convert.ToString(R["iID_MaMucLucNganSach"]));
                DK += " AND iID_MaDonVi=@iID_MaDonVi";
                cmd.Parameters.AddWithValue("@iID_MaDonVi", Convert.ToString(R["iID_MaDonVi"]));
                DK += " AND iID_MaChungTu=@iID_MaChungTu";
                cmd.Parameters.AddWithValue("@iID_MaChungTu", Convert.ToString(R["iID_MaChungTu"]));

                String[] arrDSTruongTien_So = MucLucNganSachModels.strDSTruongTien_So.Split(',');
                String strTruong = "";
                for (int j = 0; j < arrDSTruongTien_So.Length; j++)
                {
                    if (j > 0) strTruong += ",";
                    Double rValues = 0;
                    if (dtChiTieu != null)
                    {
                        if (Convert.ToString(dtChiTieu.Rows[0][arrDSTruongTien_So[j]]) != null && Convert.ToString(dtChiTieu.Rows[0][arrDSTruongTien_So[j]]) != "")
                        {
                            rValues = Convert.ToDouble(dtChiTieu.Rows[0][arrDSTruongTien_So[j]]);
                        }
                    }
                    strTruong += String.Format("{0}=@{1}", arrDSTruongTien_So[j] + "_ChiTieu", arrDSTruongTien_So[j] + "_ChiTieu");
                    cmd.Parameters.AddWithValue(String.Format("@{0}", arrDSTruongTien_So[j] + "_ChiTieu"), rValues);
                }
                cmd.CommandText = String.Format("UPDATE QTA_ChungTuChiTiet SET {0} WHERE {1}", strTruong, DK);
                Connection.UpdateDatabase(cmd);
                cmd.Dispose();
                dtChiTieu.Dispose();
            }
        }
        /// <summary>
        /// Update_TruongDaQuyetToan
        /// </summary>
        /// <param name="dt"></param>
        public static void Update_TruongDaQuyetToan(DataTable dt)
        {
            //SqlCommand cmd;
            //DataRow R;
            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    R = dt.Rows[i];
            //    DataTable dtChiTieu = Get_dtTongDaQuyetToanChoDonVi(Convert.ToString(R["iID_MaMucLucNganSach"]), Convert.ToString(R["iID_MaDonVi"]), Convert.ToString(R["iNamLamViec"]), Convert.ToString(R["iID_MaNguonNganSach"]), Convert.ToString(R["iID_MaNamNganSach"]), Convert.ToInt32(R["iThang_Quy"]), Convert.ToInt32(R["bLoaiThang_Quy"]), Convert.ToString(R["dNgayChungTu"]));
            //    cmd = new SqlCommand();
            //    String DK = "iTrangThai = 1";
            //    DK += " AND iNamLamViec=@iNamLamViec";
            //    cmd.Parameters.AddWithValue("@iNamLamViec", Convert.ToString(R["iNamLamViec"]));
            //    DK += " AND iThang_Quy=@iThang_Quy";
            //    cmd.Parameters.AddWithValue("@iThang_Quy", Convert.ToString(R["iThang_Quy"]));
            //    DK += " AND bLoaiThang_Quy=@bLoaiThang_Quy";
            //    cmd.Parameters.AddWithValue("@bLoaiThang_Quy", Convert.ToString(R["bLoaiThang_Quy"]));
            //    DK += " AND iID_MaMucLucNganSach=@iID_MaMucLucNganSach";
            //    cmd.Parameters.AddWithValue("@iID_MaMucLucNganSach", Convert.ToString(R["iID_MaMucLucNganSach"]));
            //    DK += " AND iID_MaDonVi=@iID_MaDonVi";
            //    cmd.Parameters.AddWithValue("@iID_MaDonVi", Convert.ToString(R["iID_MaDonVi"]));
            //    DK += " AND iID_MaChungTu=@iID_MaChungTu";
            //    cmd.Parameters.AddWithValue("@iID_MaChungTu", Convert.ToString(R["iID_MaChungTu"]));

            //    String[] arrDSTruongTien_So = MucLucNganSachModels.strDSTruongTien_So.Split(',');
            //    String strTruong = "";
            //    for (int j = 0; j < arrDSTruongTien_So.Length; j++)
            //    {
            //        if (j > 0) strTruong += ",";
            //        Double rValues = 0;
            //        if (Convert.ToString(dtChiTieu.Rows[0][arrDSTruongTien_So[j]]) != null && Convert.ToString(dtChiTieu.Rows[0][arrDSTruongTien_So[j]]) != "")
            //        {
            //            rValues = Convert.ToDouble(dtChiTieu.Rows[0][arrDSTruongTien_So[j]]);
            //        }
            //        strTruong += String.Format("{0}=@{1}", arrDSTruongTien_So[j] + "_DaQuyetToan", arrDSTruongTien_So[j] + "_DaQuyetToan");
            //        cmd.Parameters.AddWithValue(String.Format("@{0}", arrDSTruongTien_So[j] + "_DaQuyetToan"), rValues);
            //    }
            //    cmd.CommandText = String.Format("UPDATE QTA_ChungTuChiTiet SET {0} WHERE {1}", strTruong, DK);
            //    Connection.UpdateDatabase(cmd);
            //    cmd.Dispose();
            //    dtChiTieu.Dispose();
            //}
        }
        /// <summary>
        /// Update_TuChiTuLuong
        /// </summary>
        /// <param name="dt"></param>
        public static void Update_TuChiTuLuong(DataTable dt)
        {
            SqlCommand cmd;
            DataRow R;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                R = dt.Rows[i];
                DataTable dtChiTieu = Get_dtTongLuongChoDonVi(Convert.ToString(R["iID_MaMucLucNganSach"]), Convert.ToString(R["iID_MaDonVi"]), Convert.ToString(R["iNamLamViec"]), Convert.ToInt32(R["iThang_Quy"]));
                cmd = new SqlCommand();
                String DK = "iTrangThai = 1";
                DK += " AND iNamLamViec=@iNamLamViec";
                cmd.Parameters.AddWithValue("@iNamLamViec", Convert.ToString(R["iNamLamViec"]));
                DK += " AND iThang_Quy=@iThang_Quy";
                cmd.Parameters.AddWithValue("@iThang_Quy", Convert.ToString(R["iThang_Quy"]));
                DK += " AND bLoaiThang_Quy=@bLoaiThang_Quy";
                cmd.Parameters.AddWithValue("@bLoaiThang_Quy", Convert.ToString(R["bLoaiThang_Quy"]));
                DK += " AND iID_MaMucLucNganSach=@iID_MaMucLucNganSach";
                cmd.Parameters.AddWithValue("@iID_MaMucLucNganSach", Convert.ToString(R["iID_MaMucLucNganSach"]));
                DK += " AND iID_MaDonVi=@iID_MaDonVi";
                cmd.Parameters.AddWithValue("@iID_MaDonVi", Convert.ToString(R["iID_MaDonVi"]));
                DK += " AND iID_MaChungTu=@iID_MaChungTu";
                cmd.Parameters.AddWithValue("@iID_MaChungTu", Convert.ToString(R["iID_MaChungTu"]));

                String strTruong = "";
                Double rvNguoi = 0, rvTien = 0;
                if (Convert.ToString(dtChiTieu.Rows[0]["rNguoi"]) != null && Convert.ToString(dtChiTieu.Rows[0]["rNguoi"]) != "")
                {
                    rvNguoi = Convert.ToDouble(dtChiTieu.Rows[0]["rNguoi"]);
                }
                if (Convert.ToString(dtChiTieu.Rows[0]["rTien"]) != null && Convert.ToString(dtChiTieu.Rows[0]["rTien"]) != "")
                {
                    rvTien = Convert.ToDouble(dtChiTieu.Rows[0]["rTien"]);
                }

                strTruong += String.Format("{0}=@{1}", "rSoNguoi", "rSoNguoi");
                cmd.Parameters.AddWithValue(String.Format("@{0}", "rSoNguoi"), rvNguoi);
                strTruong += "," + String.Format("{0}=@{1}", "rTuChi", "rTuChi");
                cmd.Parameters.AddWithValue(String.Format("@{0}", "rTuChi"), rvTien);

                cmd.CommandText = String.Format("UPDATE QTA_ChungTuChiTiet SET {0} WHERE {1}", strTruong, DK);
                Connection.UpdateDatabase(cmd);
                cmd.Dispose();
                dtChiTieu.Dispose();
            }
        }
        /// <summary>
        /// Get_dtChungTuChiTiet
        /// </summary>
        /// <param name="iID_MaChungTu"></param>
        /// <returns></returns>
        public static DataTable Get_dtChungTuChiTiet(String iID_MaChungTu)
        {
            DataTable vR;
            SqlCommand cmd = new SqlCommand("SELECT * FROM QTA_ChungTuChiTiet WHERE iID_MaChungTu=@iID_MaChungTu AND iTrangThai = 1");
            cmd.Parameters.AddWithValue("@iID_MaChungTu", iID_MaChungTu);
            vR = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return vR;
        }
        /// <summary>
        /// Get_dtTongDuToanChoDonVi
        /// </summary>
        /// <param name="iID_MaMucLucNganSach"></param>
        /// <param name="iID_MaDonVi"></param>
        /// <param name="iNamLamViec"></param>
        /// <param name="iID_MaNguonNganSach"></param>
        /// <param name="iID_MaNamNganSach"></param>
        /// <param name="iThang"></param>
        /// <returns></returns>
        public static DataTable Get_dtTongDuToanChoDonVi(String iID_MaMucLucNganSach,
                                                         String iID_MaDonVi,
                                                         String iNamLamViec,
                                                         String iID_MaNguonNganSach,
                                                         String iID_MaNamNganSach,
                                                         String iThang)
        {
            if (iID_MaMucLucNganSach == null) iID_MaMucLucNganSach = "";
            if (iID_MaDonVi == null) iID_MaDonVi = "";
            if (iNamLamViec == null) iNamLamViec = "";
            if (iID_MaNguonNganSach == null) iID_MaNguonNganSach = "";
            if (iID_MaNamNganSach == null) iID_MaNamNganSach = "";
            if (iThang == null) iThang = "";

            SqlCommand cmd = new SqlCommand();
            String DK = "iTrangThai = 1";
            DK += " AND iID_MaTrangThaiDuyet=@iID_MaTrangThaiDuyet";
            cmd.Parameters.AddWithValue("@iID_MaTrangThaiDuyet", LuongCongViecModel.Get_iID_MaTrangThaiDuyet_DaDuyet(DuToanModels.iID_MaPhanHe));
            DK += " AND iNamLamViec=@iNamLamViec";
            cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
            DK += " AND iID_MaNguonNganSach=@iID_MaNguonNganSach";
            cmd.Parameters.AddWithValue("@iID_MaNguonNganSach", iID_MaNguonNganSach);
            DK += " AND iID_MaNamNganSach=@iID_MaNamNganSach";
            cmd.Parameters.AddWithValue("@iID_MaNamNganSach", iID_MaNamNganSach);
            DK += " AND iID_MaMucLucNganSach=@iID_MaMucLucNganSach";
            cmd.Parameters.AddWithValue("@iID_MaMucLucNganSach", iID_MaMucLucNganSach);
            DK += " AND iID_MaDonVi=@iID_MaDonVi";
            cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
            DK += " AND iID_MaDotNganSach IN (SELECT iID_MaDotNganSach FROM DT_DotNganSach WHERE iNamLamViec=@iNamLamViec AND MONTH(dNgayDotNganSach) <= @iThang)";
            cmd.Parameters.AddWithValue("@iThang", iThang);

            String[] arrDSTruongTien_So = MucLucNganSachModels.strDSTruongTien_So.Split(',');
            String strTruong = "";
            for (int i = 0; i < arrDSTruongTien_So.Length; i++)
            {
                if (i > 0) strTruong += ",";
                strTruong += String.Format("SUM({0}) AS {0}", arrDSTruongTien_So[i]);
            }

            cmd.CommandText = String.Format("SELECT {0} FROM DT_ChungTuChiTiet WHERE {1}", strTruong, DK);
            DataTable vR = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return vR;
        }
        /// <summary>
        /// Get_dtTongCapPhatChoDonVi
        /// </summary>
        /// <param name="iID_MaMucLucNganSach"></param>
        /// <param name="iID_MaDonVi"></param>
        /// <param name="iNamLamViec"></param>
        /// <param name="iID_MaNguonNganSach"></param>
        /// <param name="iID_MaNamNganSach"></param>
        /// <param name="iThang"></param>
        /// <returns></returns>
        public static DataTable Get_dtTongCapPhatChoDonVi(String iID_MaMucLucNganSach,
                                                         String iID_MaDonVi,
                                                         String iNamLamViec,
                                                         String iID_MaNguonNganSach,
                                                         String iID_MaNamNganSach,
                                                         String iThang)
        {
            if (iID_MaMucLucNganSach == null) iID_MaMucLucNganSach = "";
            if (iID_MaDonVi == null) iID_MaDonVi = "";
            if (iNamLamViec == null) iNamLamViec = "";
            if (iID_MaNguonNganSach == null) iID_MaNguonNganSach = "";
            if (iID_MaNamNganSach == null) iID_MaNamNganSach = "";
            if (iThang == null) iThang = "";

            SqlCommand cmd = new SqlCommand();
            String DK = "iTrangThai = 1";
            DK += " AND iID_MaTrangThaiDuyet=@iID_MaTrangThaiDuyet";
            cmd.Parameters.AddWithValue("@iID_MaTrangThaiDuyet", LuongCongViecModel.Get_iID_MaTrangThaiDuyet_DaDuyet(PhanHeModels.iID_MaPhanHeQuyetToan));
            DK += " AND iNamLamViec=@iNamLamViec";
            cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
            DK += " AND iID_MaNguonNganSach=@iID_MaNguonNganSach";
            cmd.Parameters.AddWithValue("@iID_MaNguonNganSach", iID_MaNguonNganSach);
            DK += " AND iID_MaNamNganSach=@iID_MaNamNganSach";
            cmd.Parameters.AddWithValue("@iID_MaNamNganSach", iID_MaNamNganSach);
            DK += " AND iID_MaMucLucNganSach=@iID_MaMucLucNganSach";
            cmd.Parameters.AddWithValue("@iID_MaMucLucNganSach", iID_MaMucLucNganSach);
            DK += " AND iID_MaDonVi=@iID_MaDonVi";
            cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
            DK += " AND iID_MaDotPhanBo IN (SELECT iID_MaDotPhanBo FROM PB_DotPhanBo WHERE MONTH(dNgayDotPhanBo) <= @iThang)";
            cmd.Parameters.AddWithValue("@iThang", iThang);

            String[] arrDSTruongTien_So = MucLucNganSachModels.strDSTruongTien_So.Split(',');
            String strTruong = "";
            for (int i = 0; i < arrDSTruongTien_So.Length; i++)
            {
                if (i > 0) strTruong += ",";
                strTruong += String.Format("SUM({0}) AS {0}", arrDSTruongTien_So[i]);
            }

            cmd.CommandText = String.Format("SELECT {0} FROM PB_PhanBoChiTiet WHERE {1}", strTruong, DK);
            DataTable vR = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return vR;
        }
        /// <summary>
        /// Get_dtTongDaQuyetToanChoDonVi
        /// </summary>
        /// <param name="iID_MaMucLucNganSach"></param>
        /// <param name="iID_MaDonVi"></param>
        /// <param name="iNamLamViec"></param>
        /// <param name="iID_MaNguonNganSach"></param>
        /// <param name="iID_MaNamNganSach"></param>
        /// <param name="iThang_Quy"></param>
        /// <param name="bLoaiThang_Quy"></param>
        /// <returns></returns>
        public static DataTable Get_dtTongDaQuyetToanChoDonVi(
            String iID_MaMucLucNganSach,
            String iID_MaDonVi,
            String iNamLamViec,
            String iID_MaNguonNganSach,
            String iID_MaNamNganSach,
            int iSoChungTu,
            int iThang_Quy,
            int bLoaiThang_Quy,
            DateTime dNgayChungTu,
            String iID_MaPhongBan)
        {
            #region definition input chitieu

            var sql = FileHelpers.GetSqlQuery("qt_table_daquyettoan.sql");

            #endregion

            #region dieu kien chitieu              

            #endregion

            #region get data chitieu

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                cmd.Parameters.AddWithValue("@iID_MaNamNganSach", iID_MaNamNganSach);
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan);
                cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
                cmd.Parameters.AddWithValue("@bLoaiThang_Quy", @bLoaiThang_Quy);
                cmd.Parameters.AddWithValue("@iThang_Quy", @iThang_Quy);
                cmd.Parameters.AddWithValue("@dNgayChungTu", dNgayChungTu);
                cmd.Parameters.AddWithValue("@iSoChungTu", iSoChungTu);

                var dt = cmd.GetTable();

                return dt;
            }
            #endregion
        }
        public static DataTable Get_dtTongChiTieuChoDonVi(
                String iID_MaDonVi,
                String iNamLamViec,
                String iID_MaNguonNganSach,
                String iID_MaNamNganSach,
                String iID_MaPhongBan,
                String iThang_Quy,
                DateTime dNgayChungTu,
                String sLNS,
                String iLoai)
        {
            #region definition input chitieu

            var sql = FileHelpers.GetSqlQuery("QuyetToan_ChiTieu.sql");

            #endregion

            #region dieu kien chitieu  

            String sLNSct = sLNS;
            if (iLoai == "1")
            {
                if (!String.IsNullOrEmpty(sLNS))
                {
                    if (sLNSct.StartsWith("104"))
                        sLNSct = "1040100,1040200,1040300";
                    if (sLNSct.Contains("1020100"))
                    {
                        sLNSct += ",1020100,1020000";
                    }
                }
                sql = sql.Replace("@@sLNS", "'1%'");
                sql = sql.Replace("@@DKLNSDB", "");
            }
            else if (iLoai == "0")
            {
                sql = sql.Replace("@@sLNS", "'4%'");
                sql = sql.Replace("@@DKLNSDB", "");
            }
            else
            {
                sql = sql.Replace("@@sLNS", "'2%'");
                sql = sql.Replace("@@DKLNSDB", "OR sLNS LIKE '3%'");
            }
            if (iID_MaNamNganSach == "2")
            {
                iID_MaNamNganSach = "2,4";
            }
            else if (iID_MaNamNganSach == "1")
            {
                iID_MaNamNganSach = "1,5";
            }

            #endregion

            #region get data chitieu

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan);
                cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                cmd.Parameters.AddWithValue("@iID_MaNguonNganSach", DBNull.Value);
                cmd.Parameters.AddWithValue("@iID_MaNamNganSach", iID_MaNamNganSach);
                cmd.Parameters.AddWithValue("@sLNS", sLNSct);
                cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
                cmd.Parameters.AddWithValue("@dMaDot", dNgayChungTu);
                cmd.Parameters.AddWithValue("@dvt", 1);

                var dt = cmd.GetTable();
                return dt;
            }
            #endregion

        }

        public static DataTable Get_dtTongChiTieuChoDonVi_v2(
                String iID_MaDonVi,
                String iNamLamViec,
                String iID_MaNguonNganSach,
                String iID_MaNamNganSach,
                String iID_MaPhongBan,
                String iThang_Quy,
                DateTime dNgayChungTu,
                String sLNS,
                String iLoai)
        {
            #region definition input chitieu

            var sql = FileHelpers.GetSqlQuery("qt_Data_ChiTieu.sql");

            #endregion

            #region dieu kien chitieu  

            String sLNSct = sLNS;
            if (iLoai == "1")
            {
                if (!String.IsNullOrEmpty(sLNS))
                {
                    if (sLNSct.StartsWith("104"))
                        sLNSct = "1040100,1040200,1040300";
                    if (sLNSct.Contains("1020100"))
                    {
                        sLNSct += ",1020100,1020000";
                    }
                }
                sql = sql.Replace("@@sLNS", "'1%'");
                sql = sql.Replace("@@DKLNSDB", "");
            }
            else if (iLoai == "0")
            {
                sql = sql.Replace("@@sLNS", "'4%'");
                sql = sql.Replace("@@DKLNSDB", "");
            }
            else
            {
                sql = sql.Replace("@@sLNS", "'2%'");
                sql = sql.Replace("@@DKLNSDB", "OR sLNS LIKE '3%'");
            }
            if (iID_MaNamNganSach == "2")
            {
                iID_MaNamNganSach = "2,4";
            }
            else if (iID_MaNamNganSach == "1")
            {
                iID_MaNamNganSach = "1,5";
            }

            #endregion

            #region get data chitieu

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan);
                cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                cmd.Parameters.AddWithValue("@iID_MaNguonNganSach", iID_MaNguonNganSach);
                cmd.Parameters.AddWithValue("@iID_MaNamNganSach", iID_MaNamNganSach);
                cmd.Parameters.AddWithValue("@sLNS", sLNSct);
                cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);

                return cmd.GetTable();
            }
            #endregion

        }
        /// <summary>
        /// Get_dtTongLuongChoDonVi
        /// </summary>
        /// <param name="iID_MaMucLucNganSach"></param>
        /// <param name="iID_MaDonVi"></param>
        /// <param name="iNamLamViec"></param>
        /// <param name="iThang_Quy"></param>
        /// <returns></returns>
        public static DataTable Get_dtTongLuongChoDonVi(String iID_MaMucLucNganSach,
                                                        String iID_MaDonVi,
                                                        String iNamLamViec,
                                                        int iThang_Quy)
        {
            SqlCommand cmd = new SqlCommand();
            String DK = "iTrangThai = 1";
            DK += " AND iID_MaMucLucNganSach=@iID_MaMucLucNganSach";
            cmd.Parameters.AddWithValue("@iID_MaMucLucNganSach", iID_MaMucLucNganSach);
            DK += " AND iID_MaDonVi=@iID_MaDonVi";
            cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
            DK += " AND iNamBangLuong=@iNamBangLuong";
            cmd.Parameters.AddWithValue("@iNamBangLuong", iNamLamViec);
            DK += " AND iThangBangLuong = @iThangBangLuong";
            cmd.Parameters.AddWithValue("@iThangBangLuong", iThang_Quy);

            String[] arrDSTruongTien_So = "rNguoi,rTien".Split(',');
            String strTruong = "";
            for (int i = 0; i < arrDSTruongTien_So.Length; i++)
            {
                if (i > 0) strTruong += ",";
                strTruong += String.Format("SUM({0}) AS {0}", arrDSTruongTien_So[i]);
            }

            cmd.CommandText = String.Format("SELECT {0} FROM L_BangLuong_QuyetToan WHERE {1}", strTruong, DK);
            DataTable vR = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return vR;
        }
        /// <summary>
        /// InsertDuyetQuyetToan
        /// </summary>
        /// <param name="iID_MaChungTu"></param>
        /// <param name="NoiDung"></param>
        /// <param name="MaND"></param>
        /// <param name="IPSua"></param>
        /// <returns></returns>
        public static String InsertDuyetQuyetToan(String iID_MaChungTu, String NoiDung, String MaND, String IPSua)
        {
            String MaDuyetChungTu;
            Bang bang = new Bang("QTA_DuyetChungTu");
            bang.MaNguoiDungSua = MaND;
            bang.IPSua = IPSua;
            bang.DuLieuMoi = true;
            bang.CmdParams.Parameters.AddWithValue("@iID_MaChungTu", iID_MaChungTu);
            bang.CmdParams.Parameters.AddWithValue("@sNoiDung", NoiDung);
            MaDuyetChungTu = Convert.ToString(bang.Save());
            return MaDuyetChungTu;
        }
        /// <summary>
        /// Get_iID_MaTrangThaiDuyet_TuChoi
        /// </summary>
        /// <param name="MaND"></param>
        /// <param name="iID_MaChungTu"></param>
        /// <returns></returns>
        public static int Get_iID_MaTrangThaiDuyet_TuChoi(String MaND, String iID_MaChungTu)
        {
            int vR = -1;
            DataTable dt = QuyetToan_ChungTuModels.GetChungTu(iID_MaChungTu);
            int iID_MaTrangThaiDuyet = Convert.ToInt32(dt.Rows[0]["iID_MaTrangThaiDuyet"]);
            dt.Dispose();
            if (LuongCongViecModel.NguoiDung_DuocSuaChungTu(PhanHeModels.iID_MaPhanHeQuyetToan, MaND, iID_MaTrangThaiDuyet))
            {
                int iID_MaTrangThaiDuyet_TuChoi = LuongCongViecModel.Luong_iID_MaTrangThaiDuyet_TuChoi(iID_MaTrangThaiDuyet);
                if (iID_MaTrangThaiDuyet_TuChoi > 0)
                {
                    SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM QTA_ChungTuChiTiet WHERE iID_MaChungTu=@iID_MaChungTu AND bDongY=0");
                    cmd.Parameters.AddWithValue("@iID_MaChungTu", iID_MaChungTu);
                    if (Convert.ToInt32(Connection.GetValue(cmd, 0)) > 0)
                    {
                        vR = Convert.ToInt32(iID_MaTrangThaiDuyet_TuChoi);
                    }
                    cmd.Dispose();
                }
            }
            return vR;
        }
        /// <summary>
        /// Get_iID_MaTrangThaiDuyet_TrinhDuyet
        /// </summary>
        /// <param name="MaND"></param>
        /// <param name="iID_MaChungTu"></param>
        /// <returns></returns>
        public static int Get_iID_MaTrangThaiDuyet_TrinhDuyet(String MaND, String iID_MaChungTu)
        {
            int vR = -1;
            DataTable dt = QuyetToan_ChungTuModels.GetChungTu(iID_MaChungTu);
            int iID_MaTrangThaiDuyet = Convert.ToInt32(dt.Rows[0]["iID_MaTrangThaiDuyet"]);
            dt.Dispose();
            if (LuongCongViecModel.NguoiDung_DuocSuaChungTu(PhanHeModels.iID_MaPhanHeQuyetToan, MaND, iID_MaTrangThaiDuyet))
            {
                int iID_MaTrangThaiDuyet_TrinhDuyet = LuongCongViecModel.Luong_iID_MaTrangThaiDuyet_TrinhDuyet(iID_MaTrangThaiDuyet);
                if (iID_MaTrangThaiDuyet_TrinhDuyet > 0)
                {
                    vR = Convert.ToInt32(iID_MaTrangThaiDuyet_TrinhDuyet);
                }
            }
            return vR;
        }

        //public static DataTable Get_dtQuyetToanChiTiet(String iID_MaChungTu, Dictionary<String, String> arrGiaTriTimKiem)
        //{
        //    DataTable vR;
        //    String SQL, DK;
        //    SqlCommand cmd = new SqlCommand();

        //    DK = "iTrangThai=1 AND iID_MaChungTu=@iID_MaChungTu";
        //    cmd.Parameters.AddWithValue("@iID_MaChungTu", iID_MaChungTu);

        //    if (arrGiaTriTimKiem != null)
        //    {
        //        String DSTruong = "iID_MaDonVi," + MucLucNganSachModels.strDSTruong;
        //        String[] arrDSTruong = DSTruong.Split(',');
        //        for (int i = 0; i < arrDSTruong.Length; i++)
        //        {
        //            if (String.IsNullOrEmpty(arrGiaTriTimKiem[arrDSTruong[i]]) == false)
        //            {
        //                DK += String.Format(" AND {0}=@{0}", arrDSTruong[i]);
        //                cmd.Parameters.AddWithValue("@" + arrDSTruong[i], arrGiaTriTimKiem[arrDSTruong[i]]);
        //            }
        //        }
        //    }

        //    SQL = String.Format("SELECT * FROM QTA_ChungTuChiTiet WHERE {0} ORDER BY sXauNoiMa", DK);
        //    cmd.CommandText = SQL;

        //    vR = Connection.GetDataTable(cmd);
        //    cmd.Dispose();

        //    return vR;
        //}
        /// <summary>
        /// Dien_TruongDuToan
        /// </summary>
        /// <param name="RChungTu"></param>
        /// <param name="dt"></param>
        /// <param name="bLoaiThang_Quy"></param>
        /// <param name="iThang_Quy"></param>
        private static void Dien_TruongDuToan(DataRow RChungTu, DataTable dt, int bLoaiThang_Quy, int iThang_Quy)
        {
            String[] arrDSTruongTien_So = MucLucNganSachModels.strDSTruongTien_So.Split(',');
            DataRow R;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                R = dt.Rows[i];
                String iThang = "";
                switch (bLoaiThang_Quy)
                {
                    case 0:
                        iThang = Convert.ToString(iThang_Quy);
                        break;
                    case 1:
                        Int32 iQuy = iThang_Quy;
                        switch (iQuy)
                        {
                            case 1:
                                iThang = "3";
                                break;
                            case 2:
                                iThang = "6";
                                break;
                            case 3:
                                iThang = "9";
                                break;
                            case 4:
                                iThang = "12";
                                break;
                        }
                        break;
                    case 2:
                        iThang = "12";
                        break;
                }
                DataTable dtDuToan = Get_dtTongDuToanChoDonVi(Convert.ToString(R["iID_MaMucLucNganSach"]), Convert.ToString(RChungTu["iID_MaDonVi"]), Convert.ToString(RChungTu["iNamLamViec"]), Convert.ToString(RChungTu["iID_MaNguonNganSach"]), Convert.ToString(RChungTu["iID_MaNamNganSach"]), iThang);
                for (int j = 0; j < arrDSTruongTien_So.Length; j++)
                {
                    if (dt.Columns.IndexOf(arrDSTruongTien_So[j] + "_DonVi") >= 0)
                    {
                        Double rValues = 0;
                        if (dtDuToan != null)
                        {
                            if (Convert.ToString(dtDuToan.Rows[0][arrDSTruongTien_So[j]]) != null && Convert.ToString(dtDuToan.Rows[0][arrDSTruongTien_So[j]]) != "")
                            {
                                rValues = Convert.ToDouble(dtDuToan.Rows[0][arrDSTruongTien_So[j]]);
                            }
                        }
                        R[arrDSTruongTien_So[j] + "_DonVi"] = rValues;
                    }
                }
                dtDuToan.Dispose();
            }
        }
        /// <summary>
        /// Dien_TruongChiTieu
        /// </summary>
        /// <param name="RChungTu"></param>
        /// <param name="dt"></param>
        /// <param name="bLoaiThang_Quy"></param>
        /// <param name="iThang_Quy"></param>
        private static void Dien_TruongChiTieu(DataRow RChungTu, DataTable dt, int bLoaiThang_Quy, int iThang_Quy)
        {
            String[] arrDSTruongTien_So = MucLucNganSachModels.strDSTruongTien_So.Split(',');
            DataRow R;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                R = dt.Rows[i];
                String iThang = "";
                switch (bLoaiThang_Quy)
                {
                    case 0:
                        iThang = Convert.ToString(iThang_Quy);
                        break;
                    case 1:
                        Int32 iQuy = iThang_Quy;
                        switch (iQuy)
                        {
                            case 1:
                                iThang = "3";
                                break;
                            case 2:
                                iThang = "6";
                                break;
                            case 3:
                                iThang = "9";
                                break;
                            case 4:
                                iThang = "12";
                                break;
                        }
                        break;
                    case 2:
                        iThang = "12";
                        break;
                }
                DataTable dtChiTieu = Get_dtTongCapPhatChoDonVi(Convert.ToString(R["iID_MaMucLucNganSach"]), Convert.ToString(RChungTu["iID_MaDonVi"]), Convert.ToString(RChungTu["iNamLamViec"]), Convert.ToString(RChungTu["iID_MaNguonNganSach"]), Convert.ToString(RChungTu["iID_MaNamNganSach"]), iThang);
                for (int j = 0; j < arrDSTruongTien_So.Length; j++)
                {
                    if (dt.Columns.IndexOf(arrDSTruongTien_So[j] + "_ChiTieu") >= 0)
                    {
                        Double rValues = 0;
                        if (dtChiTieu != null)
                        {
                            if (Convert.ToString(dtChiTieu.Rows[0][arrDSTruongTien_So[j]]) != null && Convert.ToString(dtChiTieu.Rows[0][arrDSTruongTien_So[j]]) != "")
                            {
                                rValues = Convert.ToDouble(dtChiTieu.Rows[0][arrDSTruongTien_So[j]]);
                            }
                        }
                        R[arrDSTruongTien_So[j] + "_ChiTieu"] = rValues;
                    }
                }
                dtChiTieu.Dispose();
            }
        }
        /// <summary>
        /// Dien_TruongDaQuyetToan
        /// </summary>
        /// <param name="row"></param>
        /// <param name="dt"></param>
        private static void Dien_TruongDaQuyetToan(DataRow row, DataTable dt)
        {
            DataRow R, R_QT;
            String[] arrDSTruongTien_So = MucLucNganSachModels.strDSTruongTien_So.Split(',');
            DataTable dtQuyetToan = Get_dtTongDaQuyetToanChoDonVi("",
                Convert.ToString(row["iID_MaDonVi"]),
                Convert.ToString(row["iNamLamViec"]),
                Convert.ToString(row["iID_MaNguonNganSach"]),
                Convert.ToString(row["iID_MaNamNganSach"]),
                Convert.ToInt32(row["iSoChungTu"]),
                Convert.ToInt32(row["iThang_Quy"]),
                Convert.ToInt32(row["bLoaiThang_Quy"]),
                row.Field<DateTime>("dNgayChungTu"),
                Convert.ToString(row["iID_MaPhongBan"]));

            int tg = 0;
            for (int i = 0; i < dtQuyetToan.Rows.Count; i++)
            {
                R_QT = dtQuyetToan.Rows[i];
                for (int j = tg; j < dt.Rows.Count; j++)
                {
                    R = dt.Rows[j];
                    String sXauNoiMa = Convert.ToString(R["sLNS"]) + "-" + Convert.ToString(R["sL"]) + "-" +
                        Convert.ToString(R["sK"]) + "-" + Convert.ToString(R["sM"]) + "-" +
                        Convert.ToString(R["sTM"]) + "-" + Convert.ToString(R["sTTM"]) + "-" + Convert.ToString(R["sNG"]);

                    String sXauNoiMaQT = Convert.ToString(R_QT["sLNS"]) + "-" + Convert.ToString(R_QT["sL"]) + "-" +
                        Convert.ToString(R_QT["sK"]) + "-" + Convert.ToString(R_QT["sM"]) + "-" +
                        Convert.ToString(R_QT["sTM"]) + "-" + Convert.ToString(R_QT["sTTM"]) + "-" + Convert.ToString(R_QT["sNG"]);
                    if (sXauNoiMa == sXauNoiMaQT)
                    {
                        Double rValues = Convert.IsDBNull(R["rDaQuyetToan"]) ? 0 : Convert.ToDouble(R["rDaQuyetToan"]);
                        rValues += Convert.ToDouble(R_QT["rDaQuyetToan"]);
                        R["rDaQuyetToan"] = rValues;
                        break;
                    }
                }
            }

            dtQuyetToan.Dispose();
        }
        private static void Dien_TruongChiTieu(DataRow row, DataTable dt)
        {
            DataRow R, R_QT;
            String[] arrDSTruongTien_So = MucLucNganSachModels.strDSTruongTien_So.Split(',');
            DataTable dtChiTieu = Get_dtTongChiTieuChoDonVi(Convert.ToString(row["iID_MaDonVi"]), Convert.ToString(row["iNamLamViec"]),
                Convert.ToString(row["iID_MaNguonNganSach"]),
                Convert.ToString(row["iID_MaNamNganSach"]),
                Convert.ToString(row["iID_MaPhongBan"]),
                Convert.ToString(row["iThang_Quy"]),
                row.Field<DateTime>("dNgayChungTu"),
                Convert.ToString(row["sDSLNS"]), Convert.ToString(row["iLoai"]));

            int tg = 0;
            for (int i = 0; i < dtChiTieu.Rows.Count; i++)
            {
                R_QT = dtChiTieu.Rows[i];
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    R = dt.Rows[j];
                    String sXauNoiMa = Convert.ToString(R["sLNS"]) + "-" + Convert.ToString(R["sL"]) + "-" +
                        Convert.ToString(R["sK"]) + "-" + Convert.ToString(R["sM"]) + "-" +
                        Convert.ToString(R["sTM"]) + "-" + Convert.ToString(R["sTTM"]) + "-" + Convert.ToString(R["sNG"]);

                    String sXauNoiMaQT = Convert.ToString(R_QT["sLNS"]) + "-" + Convert.ToString(R_QT["sL"]) + "-" +
                        Convert.ToString(R_QT["sK"]) + "-" + Convert.ToString(R_QT["sM"]) + "-" +
                        Convert.ToString(R_QT["sTM"]) + "-" + Convert.ToString(R_QT["sTTM"]) + "-" + Convert.ToString(R_QT["sNG"]);

                    if (sXauNoiMa == sXauNoiMaQT)
                    {
                        Double rValues = 0;
                        rValues = Convert.ToDouble(R_QT["rTuChi"]);
                        R["rChiTieu"] = rValues;
                        // tg = j++;
                        break;
                    }
                }
            }

            dtChiTieu.Dispose();
        }

        public static void DienChiTieuQuyetToan(DataTable dt, String iID_MaDonVi,
                                                       String iNamLamViec,
                                                       String iID_MaNguonNganSach,
                                                       String iID_MaNamNganSach,
                                                       String iID_MaPhongBan,
                                                       String iThang_Quy,
                                                       DateTime dNgayChungTu,
                                                       String sLNS,
                                                       String MaND)
        {
            DataRow R, R_QT;
            String[] arrDSTruongTien_So = MucLucNganSachModels.strDSTruongTien_So.Split(',');
            DataTable dtChiTieu = Get_dtTongChiTieuChoDonVi(iID_MaDonVi, iNamLamViec, iID_MaNguonNganSach, iID_MaNamNganSach, iID_MaPhongBan, iThang_Quy, dNgayChungTu, sLNS, MaND);

            int tg = 0;
            DataRow addR;
            String[] arrCol = "sLNS,sL,sK,sM,sTM,sTTM,sNG,rChiTieu".Split(',');
            for (int i = 0; i < dtChiTieu.Rows.Count; i++)
            {

                R_QT = dtChiTieu.Rows[i];
                String sXauTruyVan = "";
                sXauTruyVan = String.Format(@"sLNS='{0}' AND sL='{1}' AND sK='{2}' AND sM='{3}' AND sTM='{4}'
                                                       AND sTTM='{5}' AND sNG='{6}'",
                                                      R_QT["sLNS"], R_QT["sL"],
                                                      R_QT["sK"],
                                                      R_QT["sM"], R_QT["sTM"],
                                                      R_QT["sTTM"], R_QT["sNG"]);
                DataRow[] RCheck = dt.Select(sXauTruyVan);

                if (RCheck == null || RCheck.Length == 0)
                {
                    addR = dt.NewRow();
                    addR["sLNS1"] = dtChiTieu.Rows[i]["sLNS1"];
                    addR["sLNS3"] = dtChiTieu.Rows[i]["sLNS3"];
                    addR["sLNS5"] = dtChiTieu.Rows[i]["sLNS5"];
                    addR["sLNS"] = dtChiTieu.Rows[i]["sLNS"];
                    addR["sL"] = dtChiTieu.Rows[i]["sL"];
                    addR["sK"] = dtChiTieu.Rows[i]["sK"];
                    addR["sM"] = dtChiTieu.Rows[i]["sM"];
                    addR["sTM"] = dtChiTieu.Rows[i]["sTM"];
                    addR["sTTM"] = dtChiTieu.Rows[i]["sTTM"];
                    addR["sNG"] = dtChiTieu.Rows[i]["sNG"];
                    //addR["sMoTa"] = dtChiTieu.Rows[i]["sMoTa"];
                    addR["rChiTieu"] = Convert.ToDecimal(dtChiTieu.Rows[i]["rTuChi"]) + Convert.ToDecimal(dtChiTieu.Rows[i]["rHienVat"]);

                    dt.Rows.Add(addR);
                }
                else
                {
                    for (int j = tg; j < dt.Rows.Count; j++)
                    {
                        R = dt.Rows[j];
                        //Chuyen hang thanh cot hang nhap
                        if (Convert.ToString(R["sLNS"]) == "1040200")
                        {
                            String sXauNoiMaQT = Convert.ToString(R["sXauNoiMa"]).Replace("1040200", "1040100");
                            if (sXauNoiMaQT == Convert.ToString(R_QT["sXauNoiMa"]))
                            {
                                Double rValues = 0;
                                rValues = Convert.ToDouble(R_QT["rHangNhap"]);
                                R["rChiTieu"] = rValues;
                                tg = j++;
                                break;
                            }
                        }
                        else if (Convert.ToString(R["sLNS"]) == "1040300")
                        {
                            String sXauNoiMaQT = Convert.ToString(R["sXauNoiMa"]).Replace("1040300", "1040100");
                            if (sXauNoiMaQT == Convert.ToString(R_QT["sXauNoiMa"]))
                            {
                                Double rValues = 0;
                                rValues = Convert.ToDouble(R_QT["rHangMua"]);
                                R["rChiTieu"] = rValues;
                                tg = j++;
                                break;
                            }
                        }
                        //Với các loại ngân sách còn lại
                        else
                        {
                            String sXauNoiMa = Convert.ToString(R["sLNS"]) + "-" + Convert.ToString(R["sL"]) + "-" +
                                Convert.ToString(R["sK"]) + "-" + Convert.ToString(R["sM"]) + "-" +
                                Convert.ToString(R["sTM"]) + "-" + Convert.ToString(R["sTTM"]) + "-" + Convert.ToString(R["sNG"]);

                            String sXauNoiMaQT = Convert.ToString(R_QT["sLNS"]) + "-" + Convert.ToString(R_QT["sL"]) + "-" +
                               Convert.ToString(R_QT["sK"]) + "-" + Convert.ToString(R_QT["sM"]) + "-" +
                               Convert.ToString(R_QT["sTM"]) + "-" + Convert.ToString(R_QT["sTTM"]) + "-" + Convert.ToString(R_QT["sNG"]);

                            if (sXauNoiMa == sXauNoiMaQT)
                            {

                                Double rValues = 0;
                                rValues = Convert.ToDouble(R_QT["rTuChi"]) + Convert.ToDouble(R_QT["rHienVat"]);
                                R["rChiTieu"] = rValues;
                                tg = j++;
                                break;
                            }
                        }
                    }
                }
            }
            DataView dv = dt.DefaultView;
            dv.Sort = "sLNS,sL,sK,sM,sTM,sTTM,sNG";
            dt = dv.ToTable();


            dtChiTieu.Dispose();
        }
        /// <summary>
        /// Dien_TuChiTuLuong
        /// </summary>
        /// <param name="RChungTu"></param>
        /// <param name="dt"></param>
        private static void Dien_TuChiTuLuong(DataRow RChungTu, DataTable dt)
        {
            DataRow R;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                R = dt.Rows[i];
                DataTable dtTG = Get_dtTongLuongChoDonVi(Convert.ToString(R["iID_MaMucLucNganSach"]), Convert.ToString(RChungTu["iID_MaDonVi"]), Convert.ToString(RChungTu["iNamLamViec"]), Convert.ToInt32(RChungTu["iThang_Quy"]));
                Double rvNguoi = 0, rvTien = 0;
                if (Convert.ToString(dtTG.Rows[0]["rNguoi"]) != null && Convert.ToString(dtTG.Rows[0]["rNguoi"]) != "")
                {
                    rvNguoi = Convert.ToDouble(dtTG.Rows[0]["rNguoi"]);
                }
                if (Convert.ToString(dtTG.Rows[0]["rTien"]) != null && Convert.ToString(dtTG.Rows[0]["rTien"]) != "")
                {
                    rvTien = Convert.ToDouble(dtTG.Rows[0]["rTien"]);
                }

                R["rSoNguoi"] = rvNguoi;
                R["rTuChi"] = rvTien;
                dtTG.Dispose();
            }
        }
        /// <summary>
        /// Dien_HangDuLieuBaoHiem
        /// </summary>
        /// <param name="RChungTu"></param>
        /// <param name="dt"></param>
        private static void Dien_HangDuLieuBaoHiem(DataRow RChungTu, DataTable dt)
        {
            DataRow R;
            String[] arrDSTruongTien_So = MucLucNganSachModels.strDSTruongTien_So.Split(',');
            DataTable dtBaoHiem = Get_dtBaoHiem(Convert.ToInt32(RChungTu["iID_MaNamNganSach"]), Convert.ToInt32(RChungTu["iID_MaNguonNganSach"]), Convert.ToInt32(RChungTu["iNamLamViec"]), Convert.ToInt32(RChungTu["iThang_Quy"]), Convert.ToInt32(RChungTu["bLoaiThang_Quy"]), Convert.ToString(RChungTu["iID_MaDonVi"]));
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                R = dt.Rows[i];
                if (Convert.ToString(dt.Rows[i]["sXauNoiMa"]) == "1010000-460-468-6300-6301-00-00-00")
                {
                    if (String.IsNullOrEmpty(dtBaoHiem.Rows[0]["SoTienXH"].ToString()) == false)
                    {
                        R["rTuChi"] = Convert.ToDouble(dtBaoHiem.Rows[0]["SoTienXH"]);
                    }
                }
                else if (Convert.ToString(dt.Rows[i]["sXauNoiMa"]) == "1010000-460-468-6300-6302-00-00-00")
                {
                    if (String.IsNullOrEmpty(dtBaoHiem.Rows[0]["SoTienYT"].ToString()) == false)
                    {
                        R["rTuChi"] = Convert.ToDouble(dtBaoHiem.Rows[0]["SoTienYT"]);
                    }
                }
                else if (Convert.ToString(dt.Rows[i]["sXauNoiMa"]) == "1010000-460-468-6300-6303-00-00-00")
                {
                    if (String.IsNullOrEmpty(dtBaoHiem.Rows[0]["SoTienTN"].ToString()) == false)
                    {
                        R["rTuChi"] = Convert.ToDouble(dtBaoHiem.Rows[0]["SoTienTN"]);
                    }
                }
                dtBaoHiem.Dispose();
            }
        }
        /// <summary>
        /// Get_dtQuyetToanChiTiet
        /// </summary>
        /// <param name="iID_MaChungTu"></param>
        /// <param name="arrGiaTriTimKiem"></param>
        /// <returns></returns>
        //public static DataTable Get_dtQuyetToanChiTiet(String iID_MaChungTu, String MaND, Dictionary<String, String> arrGiaTriTimKiem)
        //{
        //    DataTable vR;

        //    DataTable dtChungTu = QuyetToan_ChungTuModels.GetChungTu(iID_MaChungTu);
        //    String sLNS = Convert.ToString(dtChungTu.Rows[0]["sDSLNS"]);
        //    String iLoai = Convert.ToString(dtChungTu.Rows[0]["iLoai"]);

        //    int iThang_Quy = Convert.ToInt32(dtChungTu.Rows[0]["iThang_Quy"]);
        //    int bLoaiThang_Quy = Convert.ToInt32(dtChungTu.Rows[0]["bLoaiThang_Quy"]);
        //    DataRow RChungTu = dtChungTu.Rows[0];
        //    String iID_MaPhongBan = NganSach_HamChungModels.MaPhongBanCuaMaND(MaND);
        //    String sTruongTien = MucLucNganSachModels.strDSTruongTien + ",iID_MaChungTuChiTiet,rDonViDeNghi,rVuotChiTieu,rTonThatTonDong,rDaCapTien,rChuaCapTien,sGhiChu";
        //    String[] arrDSTruong = MucLucNganSachModels.strDSTruong.Split(',');
        //    String[] arrDSTruongTien = sTruongTien.Split(',');

        //    String SQL, DK;
        //    SqlCommand cmd;

        //    //<--Lay toan bo Muc luc ngan sach
        //    cmd = new SqlCommand();
        //    DK = "iTrangThai=1";

        //    if (arrGiaTriTimKiem != null)
        //    {
        //        for (int i = 0; i < arrDSTruong.Length - 1; i++)
        //        {
        //            if (String.IsNullOrEmpty(arrGiaTriTimKiem[arrDSTruong[i]]) == false)
        //            {
        //                DK += String.Format(" AND {0}=@{0}", arrDSTruong[i]);
        //                cmd.Parameters.AddWithValue("@" + arrDSTruong[i], arrGiaTriTimKiem[arrDSTruong[i]]);
        //            }
        //        }
        //    }


        //    String[] arrsLNS = sLNS.Split(',');
        //    String DKLNS = "";
        //    //ngan sach quoc phong tung loai ns 
        //    if (iLoai == "1")
        //    {
        //        for (int i = 0; i < arrsLNS.Length; i++)
        //        {
        //            if (arrsLNS[i] != "" && arrsLNS[i] != "104")
        //            {
        //                if (String.IsNullOrEmpty(DKLNS) == false)
        //                {
        //                    DKLNS += " OR ";
        //                }
        //                DKLNS += String.Format("(sLNS=@sLNS{0} or sLNS in (select * from f_split3(@sLNS{0})))", i);
        //                cmd.Parameters.AddWithValue(String.Format("@sLNS{0}", i), arrsLNS[i]);
        //            }
        //            else if (arrsLNS[i] == "104")
        //            {
        //                if (String.IsNullOrEmpty(DKLNS) == false)
        //                {
        //                    DKLNS += " OR ";
        //                }
        //                DKLNS += String.Format("sLNS LIKE @sLNS{0}", i);
        //                cmd.Parameters.AddWithValue(String.Format("@sLNS{0}", i), "104%");
        //            }
        //        }
        //    }
        //    //toan bo ngan sach nha nuoc dau 2,3
        //    else if (iLoai == "2")
        //    {

        //        DataTable dtPB = DanhMucModels.NS_LoaiNganSachNhaNuoc_PhongBan(iID_MaPhongBan);
        //        for (int i = 0; i < dtPB.Rows.Count; i++)
        //        {
        //            DKLNS += " sLNS=@sLNS" + i;
        //            if (i < dtPB.Rows.Count - 1)
        //                DKLNS += " OR ";
        //            cmd.Parameters.AddWithValue("@sLNS" + i, dtPB.Rows[i]["sLNS"]);

        //        }
        //        if (String.IsNullOrEmpty(DKLNS)) DKLNS = " sLNS=-1";
        //    }
        //    //cac loai con lai
        //    else
        //    {

        //        DataTable dtPB = DanhMucModels.NS_LoaiNganSachKhac_PhongBan(iID_MaPhongBan);
        //        for (int i = 0; i < dtPB.Rows.Count; i++)
        //        {
        //            DKLNS += " sLNS=@sLNS" + i;
        //            if (i < dtPB.Rows.Count - 1)
        //                DKLNS += " OR ";
        //            cmd.Parameters.AddWithValue("@sLNS" + i, dtPB.Rows[i]["sLNS"]);

        //        }
        //        if (String.IsNullOrEmpty(DKLNS)) DKLNS = " sLNS=-1";
        //    }

        //    DK += String.Format(" AND ({0})", DKLNS);

        //    SQL = String.Format("SELECT iID_MaMucLucNganSach,iID_MaMucLucNganSach_Cha,bLaHangCha,sXauNoiMa,{0},{1} FROM NS_MucLucNganSach WHERE iNamLamViec=@iNamLamViec AND {2}  ORDER BY {3}", MucLucNganSachModels.strDSTruong, MucLucNganSachModels.strDSDuocNhapTruongTien, DK, MucLucNganSachModels.strDSTruongSapXep);
        //    int iNamLamViec = Convert.ToInt32(ReportModels.LayNamLamViec(MaND));
        //    cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
        //    cmd.CommandText = SQL;
        //    vR = Connection.GetDataTable(cmd);
        //    cmd.Dispose();

        //    DataColumn column;

        //    //Them Tiêu đề tiền
        //    for (int j = 0; j < arrDSTruongTien.Length; j++)
        //    {
        //        if (arrDSTruongTien[j] == "sTenCongTrinh" || arrDSTruongTien[j] == "iID_MaChungTuChiTiet")
        //        {
        //            column = new DataColumn(arrDSTruongTien[j], typeof(String));
        //            column.AllowDBNull = true;
        //            vR.Columns.Add(column);
        //        }
        //    }

        //    for (int j = 0; j < arrDSTruongTien.Length; j++)
        //    {
        //        if (arrDSTruongTien[j] == "rTongSo" ||
        //            (arrDSTruongTien[j] != "sTenCongTrinh" && arrDSTruongTien[j] != "iID_MaChungTuChiTiet"))
        //        {

        //            if (arrDSTruongTien[j] == "sGhiChu")
        //            {
        //                column = new DataColumn(arrDSTruongTien[j], typeof(String));
        //            }
        //            else
        //            {
        //                column = new DataColumn(arrDSTruongTien[j], typeof(Double));
        //                column.DefaultValue = 0;
        //            }
        //            vR.Columns.Add(column);
        //        }
        //    }

        //    //Them cot duyet
        //    column = new DataColumn("bDongY", typeof(Boolean));
        //    column.AllowDBNull = true;
        //    vR.Columns.Add(column);
        //    column = new DataColumn("sLyDo", typeof(String));
        //    column.AllowDBNull = true;
        //    vR.Columns.Add(column);
        //    cmd = new SqlCommand();
        //    DK = "iTrangThai=1 AND iID_MaChungTu=@iID_MaChungTu";
        //    cmd.Parameters.AddWithValue("@iID_MaChungTu", iID_MaChungTu);
        //    SQL = String.Format("SELECT * FROM QTA_ChungTuChiTiet WHERE {0} ORDER BY sXauNoiMa", DK);
        //    cmd.CommandText = SQL;

        //    DataTable dtChungTuChiTiet = Connection.GetDataTable(cmd);
        //    int cs0 = 0;
        //    column = new DataColumn("rDaQuyetToan", typeof(decimal));
        //    column.AllowDBNull = true;
        //    vR.Columns.Add(column);
        //    column = new DataColumn("rChiTieu", typeof(decimal));
        //    column.AllowDBNull = true;
        //    vR.Columns.Add(column);
        //    Dien_TruongChiTieu(RChungTu, vR);
        //    Dien_TruongDaQuyetToan(RChungTu, vR);

        //    int vRCount = vR.Rows.Count;
        //    for (int i = 0; i < vRCount; i++)
        //    {
        //        int count = 0;
        //        for (int j = cs0; j < dtChungTuChiTiet.Rows.Count; j++)
        //        {

        //            Boolean ok = true;
        //            for (int k = 0; k < arrDSTruong.Length; k++)
        //            {
        //                if (Convert.ToString(vR.Rows[i][arrDSTruong[k]]) != Convert.ToString(dtChungTuChiTiet.Rows[j][arrDSTruong[k]]))
        //                {
        //                    ok = false;
        //                    break;
        //                }
        //            }
        //            if (ok)
        //            {
        //                if (count == 0)
        //                {
        //                    for (int k = 0; k < vR.Columns.Count - 2; k++)
        //                    {
        //                        if ((vR.Columns[k].ColumnName.StartsWith("b") == false && vR.Columns[k].ColumnName != "iID_MaMucLucNganSach_Cha") || vR.Columns[k].ColumnName == "bLaHangCha")
        //                            vR.Rows[i][k] = dtChungTuChiTiet.Rows[j][vR.Columns[k].ColumnName];
        //                        else
        //                            vR.Rows[i][k] = vR.Rows[i][k];
        //                    }
        //                    count++;
        //                }
        //                else
        //                {
        //                    DataRow row = vR.NewRow();
        //                    for (int k = 0; k < vR.Columns.Count - 2; k++)
        //                    {
        //                        if ((vR.Columns[k].ColumnName.StartsWith("b") == false && vR.Columns[k].ColumnName != "iID_MaMucLucNganSach_Cha") || vR.Columns[k].ColumnName == "bLaHangCha")
        //                            row[k] = dtChungTuChiTiet.Rows[j][vR.Columns[k].ColumnName];
        //                        else
        //                        {
        //                            row[k] = vR.Rows[i][k];
        //                        }
        //                    }
        //                    vR.Rows.InsertAt(row, i + 1);
        //                    i++;
        //                    vRCount++;
        //                }
        //            }
        //        }

        //    }
        //    cmd.Dispose();
        //    dtChungTuChiTiet.Dispose();
        //    dtChungTu.Dispose();


        //    return vR;
        //}

        #region Get_dtQuyetToanChiTiet_v1
        // Chức năng sửa: Quyết toán 
        // Người sửa: anhht
        // Nội dung: Chuyển rDaQuyetToan và rChiTieu vào cùng sql lấy data chi tiết
        public static DataTable Get_dtQuyetToanChiTiet(String iID_MaChungTu, String MaND, Dictionary<String, String> arrGiaTriTimKiem)
        {
            DataTable vR;

            DataTable dtChungTu = QuyetToan_ChungTuModels.GetChungTu(iID_MaChungTu);
            String sLNS = Convert.ToString(dtChungTu.Rows[0]["sDSLNS"]);
            String iLoai = Convert.ToString(dtChungTu.Rows[0]["iLoai"]);

            int iThang_Quy = Convert.ToInt32(dtChungTu.Rows[0]["iThang_Quy"]);
            int bLoaiThang_Quy = Convert.ToInt32(dtChungTu.Rows[0]["bLoaiThang_Quy"]);
            DataRow RChungTu = dtChungTu.Rows[0];
            String iID_MaPhongBan = NganSach_HamChungModels.MaPhongBanCuaMaND(MaND);
            String sTruongTien = MucLucNganSachModels.strDSTruongTien + ",iID_MaChungTuChiTiet,rDonViDeNghi,rVuotChiTieu,rTonThatTonDong,rDaCapTien,rChuaCapTien,sGhiChu";
            String[] arrDSTruong = MucLucNganSachModels.strDSTruong.Split(',');
            String[] arrDSTruongTien = sTruongTien.Split(',');

            try
            {
                String SQL, DK;
                SqlCommand cmd;

                //<--Lay toan bo Muc luc ngan sach
                cmd = new SqlCommand();
                DK = "iTrangThai=1";

                if (arrGiaTriTimKiem != null)
                {
                    for (int i = 0; i < arrDSTruong.Length - 1; i++)
                    {
                        if (String.IsNullOrEmpty(arrGiaTriTimKiem[arrDSTruong[i]]) == false)
                        {
                            DK += String.Format(" AND {0}=@{0}", arrDSTruong[i]);
                            cmd.Parameters.AddWithValue("@" + arrDSTruong[i], arrGiaTriTimKiem[arrDSTruong[i]]);
                        }
                    }
                }

                String[] arrsLNS = sLNS.Split(',');
                String DKLNS = "";
                //ngan sach quoc phong tung loai ns 
                if (iLoai == "1")
                {
                    for (int i = 0; i < arrsLNS.Length; i++)
                    {
                        if (arrsLNS[i] != "" && arrsLNS[i] != "104")
                        {
                            if (String.IsNullOrEmpty(DKLNS) == false)
                            {
                                DKLNS += " OR ";
                            }
                            DKLNS += String.Format("(mlns.sLNS=@sLNS{0} or mlns.sLNS in (select * from f_split3(@sLNS{0})))", i);
                            cmd.Parameters.AddWithValue(String.Format("@sLNS{0}", i), arrsLNS[i]);
                        }
                        else if (arrsLNS[i] == "104")
                        {
                            if (String.IsNullOrEmpty(DKLNS) == false)
                            {
                                DKLNS += " OR ";
                            }
                            DKLNS += String.Format("mlns.sLNS LIKE @sLNS{0}", i);
                            cmd.Parameters.AddWithValue(String.Format("@sLNS{0}", i), "104%");
                        }
                    }
                }
                //toan bo ngan sach nha nuoc dau 2,3
                else if (iLoai == "2")
                {

                    DataTable dtPB = DanhMucModels.NS_LoaiNganSachNhaNuoc_PhongBan(iID_MaPhongBan);
                    for (int i = 0; i < dtPB.Rows.Count; i++)
                    {
                        DKLNS += " mlns.sLNS=@sLNS" + i;
                        if (i < dtPB.Rows.Count - 1)
                            DKLNS += " OR ";
                        cmd.Parameters.AddWithValue("@sLNS" + i, dtPB.Rows[i]["sLNS"]);

                    }
                    if (String.IsNullOrEmpty(DKLNS)) DKLNS = " mlns.sLNS=-1";
                }
                //cac loai con lai
                else
                {

                    DataTable dtPB = DanhMucModels.NS_LoaiNganSachKhac_PhongBan(iID_MaPhongBan);
                    for (int i = 0; i < dtPB.Rows.Count; i++)
                    {
                        DKLNS += " mlns.sLNS=@sLNS" + i;
                        if (i < dtPB.Rows.Count - 1)
                            DKLNS += " OR ";
                        cmd.Parameters.AddWithValue("@sLNS" + i, dtPB.Rows[i]["sLNS"]);

                    }
                    if (String.IsNullOrEmpty(DKLNS)) DKLNS = " mlns.sLNS=-1";
                }

                DK += String.Format(" AND ({0})", DKLNS);

                var sql = FileHelpers.GetSqlQuery("get_dtquyettoanchitiet.sql");

                //SQL = String.Format("SELECT iID_MaMucLucNganSach,iID_MaMucLucNganSach_Cha,bLaHangCha,sXauNoiMa,{0},{1} FROM NS_MucLucNganSach WHERE iNamLamViec=@iNamLamViec AND {2}  ORDER BY {3}", MucLucNganSachModels.strDSTruong, MucLucNganSachModels.strDSDuocNhapTruongTien, DK, MucLucNganSachModels.strDSTruongSapXep);
                int iNamLamViec = Convert.ToInt32(ReportModels.LayNamLamViec(MaND));
                cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", Convert.ToString(RChungTu["iID_MaPhongBan"]));
                cmd.Parameters.AddWithValue("@iID_MaNamNganSach", Convert.ToString(RChungTu["iID_MaNamNganSach"]));
                cmd.Parameters.AddWithValue("@iID_MaNguonNganSach", Convert.ToString(RChungTu["iID_MaNguonNganSach"]));

                cmd.Parameters.AddWithValue("@iID_MaDonVi", Convert.ToString(RChungTu["iID_MaDonVi"]));
                cmd.Parameters.AddWithValue("@iThang_Quy", iThang_Quy);
                cmd.Parameters.AddWithValue("@dNgayChungTu", RChungTu.Field<DateTime>("dNgayChungTu"));
                cmd.Parameters.AddWithValue("@iSoChungTu", Convert.ToInt32(RChungTu["iSoChungTu"]));

                cmd.Parameters.AddWithValue("@loai", Convert.ToInt32(iLoai));
                cmd.Parameters.AddWithValue("@dvt", 1);

                sql = sql.Replace("@@dsDuocNhapTruongTien", MucLucNganSachModels.strDSDuocNhapTruongTien);
                sql = sql.Replace("@@dk", DK);
                sql = sql.Replace("@@dsTruongSapXep", MucLucNganSachModels.strDSTruongSapXep);

                cmd.CommandText = sql;
                vR = Connection.GetDataTable(cmd);

                cmd.Dispose();

                DataColumn column;

                //Them Tiêu đề tiền
                for (int j = 0; j < arrDSTruongTien.Length; j++)
                {
                    if (arrDSTruongTien[j] == "sTenCongTrinh" || arrDSTruongTien[j] == "iID_MaChungTuChiTiet")
                    {
                        column = new DataColumn(arrDSTruongTien[j], typeof(String));
                        column.AllowDBNull = true;
                        vR.Columns.Add(column);
                        column.SetOrdinal(vR.Columns.Count - 3);
                    }
                }

                for (int j = 0; j < arrDSTruongTien.Length; j++)
                {
                    if (arrDSTruongTien[j] == "rTongSo" ||
                        (arrDSTruongTien[j] != "sTenCongTrinh" && arrDSTruongTien[j] != "iID_MaChungTuChiTiet"))
                    {

                        if (arrDSTruongTien[j] == "sGhiChu")
                        {
                            column = new DataColumn(arrDSTruongTien[j], typeof(String));
                        }
                        else
                        {
                            column = new DataColumn(arrDSTruongTien[j], typeof(Double));
                            column.DefaultValue = 0;
                        }
                        vR.Columns.Add(column);
                        column.SetOrdinal(vR.Columns.Count - 3);
                    }
                }

                // anhht comment: chuyển cột bDongY, sLyDo vào sql
                //Them cot duyet
                //column = new DataColumn("bDongY", typeof(Boolean));
                //column.AllowDBNull = true;
                //vR.Columns.Add(column);
                //column = new DataColumn("sLyDo", typeof(String));
                //column.AllowDBNull = true;
                //vR.Columns.Add(column);
                cmd = new SqlCommand();
                DK = "iTrangThai=1 AND iID_MaChungTu=@iID_MaChungTu";
                cmd.Parameters.AddWithValue("@iID_MaChungTu", iID_MaChungTu);
                SQL = String.Format("SELECT * FROM QTA_ChungTuChiTiet WHERE {0} ORDER BY sXauNoiMa", DK);
                cmd.CommandText = SQL;

                DataTable dtChungTuChiTiet = Connection.GetDataTable(cmd);
                int cs0 = 0;

                // anhht comment: chuyển cột rDaQuyetToan, rChiTieu vào sql
                //column = new DataColumn("rDaQuyetToan", typeof(decimal));
                //column.AllowDBNull = true;
                //vR.Columns.Add(column);
                //column = new DataColumn("rChiTieu", typeof(decimal));
                //column.AllowDBNull = true;
                //vR.Columns.Add(column);
                //Dien_TruongChiTieu(RChungTu, vR);
                //Dien_TruongDaQuyetToan(RChungTu, vR);

                int vRCount = vR.Rows.Count;
                for (int i = 0; i < vRCount; i++)
                {
                    int count = 0;
                    for (int j = cs0; j < dtChungTuChiTiet.Rows.Count; j++)
                    {

                        Boolean ok = true;
                        for (int k = 0; k < arrDSTruong.Length; k++)
                        {
                            if (Convert.ToString(vR.Rows[i][arrDSTruong[k]]) != Convert.ToString(dtChungTuChiTiet.Rows[j][arrDSTruong[k]]))
                            {
                                ok = false;
                                break;
                            }
                        }
                        if (ok)
                        {
                            if (count == 0)
                            {
                                for (int k = 0; k < vR.Columns.Count - 2; k++)
                                {
                                    if ((vR.Columns[k].ColumnName.StartsWith("b") == false && vR.Columns[k].ColumnName != "iID_MaMucLucNganSach_Cha") || vR.Columns[k].ColumnName == "bLaHangCha")
                                        vR.Rows[i][k] = dtChungTuChiTiet.Rows[j][vR.Columns[k].ColumnName];
                                    else
                                        vR.Rows[i][k] = vR.Rows[i][k];
                                }
                                count++;
                            }
                            else
                            {
                                DataRow row = vR.NewRow();
                                for (int k = 0; k < vR.Columns.Count - 2; k++)
                                {
                                    if ((vR.Columns[k].ColumnName.StartsWith("b") == false && vR.Columns[k].ColumnName != "iID_MaMucLucNganSach_Cha") || vR.Columns[k].ColumnName == "bLaHangCha")
                                        row[k] = dtChungTuChiTiet.Rows[j][vR.Columns[k].ColumnName];
                                    else
                                    {
                                        row[k] = vR.Rows[i][k];
                                    }
                                }
                                vR.Rows.InsertAt(row, i + 1);
                                i++;
                                vRCount++;
                            }
                        }
                    }

                }
                cmd.Dispose();
                dtChungTuChiTiet.Dispose();
                dtChungTu.Dispose();


                return vR;
            } catch(Exception ex)
            {
                return null;
            }
            
        }
        #endregion


        /// <summary>
        /// Lay tong hop bao hiem
        /// </summary>
        /// <param name="iID_MaNamNganSach"></param>
        /// <param name="iID_MaNguonNganSach"></param>
        /// <param name="NamLamViec"></param>
        /// <param name="iThang"></param>
        /// <param name="iThangQuy"></param>
        /// <param name="iID_MaDonVi"></param>
        /// <returns></returns>
        public static DataTable Get_dtBaoHiem(int iID_MaNamNganSach, int iID_MaNguonNganSach, int NamLamViec, int iThang
            , int iThangQuy, String iID_MaDonVi)
        {
            DataTable vR;
            SqlCommand cmd;
            cmd = new SqlCommand();
            cmd.CommandText = "BH_TINHSOBAOHIEM_QUYETTOAN";
            cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.AddWithValue("@iID_MaNamNganSach", iID_MaNamNganSach);
            //cmd.Parameters.AddWithValue("@iID_MaNguonNganSach", iID_MaNguonNganSach);
            cmd.Parameters.AddWithValue("@NamLamViec", NamLamViec);
            cmd.Parameters.AddWithValue("@iThang", iThang);
            cmd.Parameters.AddWithValue("@iThangQuy", iThangQuy);
            cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
            cmd.Parameters.AddWithValue("@iID_MaTrangThaiDuyet_QTQS", LuongCongViecModel.Get_iID_MaTrangThaiDuyet_DaDuyet(PhanHeModels.iID_MaPhanHeQuyetToan));
            cmd.Parameters.AddWithValue("@iID_MaTrangThaiDuyetPTCTCT", LuongCongViecModel.Get_iID_MaTrangThaiDuyet_DaDuyet(PhanHeModels.iID_MaPhanHeBaoHiem));
            vR = Connection.GetDataTable(cmd);
            cmd.Dispose();


            return vR;
        }

        #region Giải thích số tiền
        public static NameValueCollection LayThongTin_GiaiThichSoTien(String iID_MaChungTu)
        {
            NameValueCollection data = new NameValueCollection();
            DataTable dt = Get_dtGiaiThichSoTien(iID_MaChungTu);
            String sColumName = "";
            String[] arrsKyHieu = "310,320,330".Split(',');
            String sTruongTien = "iID_MaGiaiThichSoTien,iSiQuan,iQNCN,iCNVCQP,iHSQCS,rSiQuan,rQNCN,rCNVCQP,rHSQCS";
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        sColumName = dt.Columns[j].ColumnName;
                        if (sTruongTien.IndexOf(sColumName) >= 0)
                        {
                            sColumName = sColumName + "_" + arrsKyHieu[i];
                        }

                        if (String.IsNullOrEmpty(data.Get(sColumName)))
                        {
                            data.Add(sColumName, Convert.ToString(dt.Rows[i][j]));
                        }
                    }
                }
                data.Add("DuLieuMoi", "0");
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        sColumName = dt.Columns[j].ColumnName;
                        String GiaTri = null;
                        if (sTruongTien.IndexOf(sColumName) >= 0)
                        {
                            if (sColumName != "iID_MaGiaiThichSoTien")
                            {
                                GiaTri = "0";
                            }
                            sColumName = sColumName + "_" + arrsKyHieu[i];

                        }
                        if (String.IsNullOrEmpty(data.Get(sColumName)))
                        {
                            data.Add(sColumName, GiaTri);
                        }

                    }
                }
                data.Add("DuLieuMoi", "1");
            }
            //Add dữ liệu từ quyết toán và F4
            DataTable dtTL = Get_dtTienLuongXinQuyetToan(iID_MaChungTu);
            String[] arrTruongTien = "rSiQuan,rQNCN,rCNVCQP,rHopDong".Split(',');
            for (int i = 0; i < dtTL.Rows.Count; i++)
            {
                for (int j = 0; j < arrTruongTien.Length; j++)
                {
                    String colName = arrTruongTien[j] + Convert.ToString(dtTL.Rows[i]["iHang"]);
                    data.Add(colName, dtTL.Rows[i][arrTruongTien[j]].ToString());
                }
            }
            dtTL.Dispose();
            return data;
        }
        public static DataTable Get_dtGiaiThichSoTien(String iID_MaChungTu)
        {
            DataTable dt = QuyetToan_ChungTuModels.GetChungTu(iID_MaChungTu);
            DataTable vR;
            SqlCommand cmd = new SqlCommand("SELECT * FROM QTA_GiaiThichSoTien WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec AND iThang_Quy=@iThang_Quy  AND iID_MaDonVi=@iID_MaDonVi ORDER BY sKyHieuDoiTuong");
            cmd.Parameters.AddWithValue("@iID_MaDonVi", dt.Rows[0]["iID_MaDonVi"]);
            cmd.Parameters.AddWithValue("@iThang_Quy", dt.Rows[0]["iThang_Quy"]);
            cmd.Parameters.AddWithValue("@iNamLamViec", dt.Rows[0]["iNamLamViec"]);
            vR = Connection.GetDataTable(cmd);
            cmd.Dispose();
            dt.Dispose();
            return vR;
        }

        public static DataTable Get_dtTienLuongXinQuyetToan(String iID_MaChungTu)
        {
            String SQL = @"SELECT ISNULL(SUM(rTuChi),0) as rTuChi FROM QTA_ChungTuChiTiet 
                        WHERE iID_MaChungTu=@iID_MaChungTu
                        AND sXauNoiMa IN (SELECT sXauNoiMa FROM BH_CauHinh_DoiTuong_NganSach WHERE sMaTX=@sMaTX)";
            SqlCommand cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@iID_MaChungTu", iID_MaChungTu);
            cmd.Parameters.AddWithValue("@sMaTX", "A1");
            Double A1 = Convert.ToDouble(Connection.GetValue(cmd, 0));

            cmd.Parameters["@sMaTX"].Value = "A2";
            Double A2 = Convert.ToDouble(Connection.GetValue(cmd, 0));

            cmd.Parameters["@sMaTX"].Value = "A3";
            Double A3 = Convert.ToDouble(Connection.GetValue(cmd, 0));

            cmd.Parameters["@sMaTX"].Value = "A4";
            Double A4 = Convert.ToDouble(Connection.GetValue(cmd, 0));

            cmd.Parameters["@sMaTX"].Value = "B1";
            Double B1 = Convert.ToDouble(Connection.GetValue(cmd, 0));

            cmd.Parameters["@sMaTX"].Value = "B2";
            Double B2 = Convert.ToDouble(Connection.GetValue(cmd, 0));

            cmd.Parameters["@sMaTX"].Value = "B3";
            Double B3 = Convert.ToDouble(Connection.GetValue(cmd, 0));

            cmd.Parameters["@sMaTX"].Value = "B4";
            Double B4 = Convert.ToDouble(Connection.GetValue(cmd, 0));
            cmd.Dispose();

            DataTable dt = QuyetToan_ChungTuModels.GetChungTu(iID_MaChungTu);

            SQL = @"SELECT SUM(rLuongCoBan) as rLuongCoBan FROM QTA_QuyetToanBaoHiem 
                    WHERE iID_MaDonVi=@iID_MaDonVi AND iThang_Quy=@iThang_Quy AND iNamLamViec=@iNamLamViec AND SUBSTRING(sKyHieuDoiTuong,2,1)=@sKyHieuDoiTuong";
            cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@iID_MaDonVi", dt.Rows[0]["iID_MaDonVi"]);
            cmd.Parameters.AddWithValue("@iThang_Quy", dt.Rows[0]["iThang_Quy"]);
            cmd.Parameters.AddWithValue("@iNamLamViec", dt.Rows[0]["iNamLamViec"]);
            cmd.Parameters.AddWithValue("@sKyHieuDoiTuong", "1");

            Double LCB1 = Convert.ToDouble(Connection.GetValue(cmd, 0));

            cmd.Parameters["@sKyHieuDoiTuong"].Value = "2";
            Double LCB2 = Convert.ToDouble(Connection.GetValue(cmd, 0));

            cmd.Parameters["@sKyHieuDoiTuong"].Value = "3";
            Double LCB3 = Convert.ToDouble(Connection.GetValue(cmd, 0));

            cmd.Parameters["@sKyHieuDoiTuong"].Value = "4";
            Double LCB4 = Convert.ToDouble(Connection.GetValue(cmd, 0));

            SQL = @"SELECT SUM(rTongSo) - SUM(rLuongCoBan) as rPhuCap FROM QTA_QuyetToanBaoHiem 
                    WHERE iID_MaDonVi=@iID_MaDonVi AND iThang_Quy=@iThang_Quy AND iNamLamViec=@iNamLamViec AND SUBSTRING(sKyHieuDoiTuong,2,1)=@sKyHieuDoiTuong";
            cmd.CommandText = SQL;
            cmd.Parameters["@sKyHieuDoiTuong"].Value = "1";
            Double PC1 = Convert.ToDouble(Connection.GetValue(cmd, 0));
            cmd.Parameters["@sKyHieuDoiTuong"].Value = "2";
            Double PC2 = Convert.ToDouble(Connection.GetValue(cmd, 0));
            cmd.Parameters["@sKyHieuDoiTuong"].Value = "3";
            Double PC3 = Convert.ToDouble(Connection.GetValue(cmd, 0));
            cmd.Parameters["@sKyHieuDoiTuong"].Value = "4";
            Double PC4 = Convert.ToDouble(Connection.GetValue(cmd, 0));
            cmd.Dispose();
            dt.Dispose();

            DataTable dtKQ = new DataTable();
            dtKQ.Columns.Add("iHang", typeof(Int16));
            dtKQ.Columns.Add("rSiQuan", typeof(Decimal));
            dtKQ.Columns.Add("rQNCN", typeof(Decimal));
            dtKQ.Columns.Add("rCNVCQP", typeof(Decimal));
            dtKQ.Columns.Add("rHopDong", typeof(Decimal));
            DataRow R = dtKQ.NewRow();
            R[0] = 1;
            R[1] = LCB1 + A1;
            R[2] = LCB2 + A2;
            R[3] = LCB3 + A3;
            R[4] = LCB4 + A4;
            dtKQ.Rows.Add(R);

            R = dtKQ.NewRow();
            R[0] = 2;
            R[1] = PC1 + B1;
            R[2] = PC2 + B2;
            R[3] = PC3 + B3;
            R[4] = PC4 + B4;
            dtKQ.Rows.Add(R);

            R = dtKQ.NewRow();
            R[0] = 3;
            R[1] = LCB1;
            R[2] = LCB2;
            R[3] = LCB3;
            R[4] = LCB4;
            dtKQ.Rows.Add(R);

            R = dtKQ.NewRow();
            R[0] = 4;
            R[1] = PC1;
            R[2] = PC2;
            R[3] = PC3;
            R[4] = PC4;
            dtKQ.Rows.Add(R);

            R = dtKQ.NewRow();
            R[0] = 5;
            R[1] = A1;
            R[2] = A2;
            R[3] = A3;
            R[4] = A4;
            dtKQ.Rows.Add(R);

            R = dtKQ.NewRow();
            R[0] = 6;
            R[1] = B1;
            R[2] = B2;
            R[3] = B3;
            R[4] = B4;
            dtKQ.Rows.Add(R);
            return dtKQ;
        }
        #endregion

        #region Giải thích bằng lời
        public static DataTable Get_dtGiaiThichBangLoi(String iID_MaChungTu)
        {

            DataTable dt = QuyetToan_ChungTuModels.GetChungTu(iID_MaChungTu);
            DataTable vR;
            SqlCommand cmd = new SqlCommand("SELECT * FROM QTA_GiaiThichBangLoi WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec AND iThang_Quy=@iThang_Quy  AND iID_MaDonVi=@iID_MaDonVi ORDER BY iSTT");
            cmd.Parameters.AddWithValue("@iID_MaDonVi", dt.Rows[0]["iID_MaDonVi"]);
            cmd.Parameters.AddWithValue("@iThang_Quy", dt.Rows[0]["iThang_Quy"]);
            cmd.Parameters.AddWithValue("@iNamLamViec", dt.Rows[0]["iNamLamViec"]);
            vR = Connection.GetDataTable(cmd);
            cmd.Dispose();
            dt.Dispose();
            int H = vR.Rows.Count;
            if (H < 10)
            {
                DataRow R;
                for (int i = 0; i < 10 - H; i++)
                {
                    R = vR.NewRow();
                    vR.Rows.Add(R);
                }
            }
            return vR;
        }
        #endregion

    }
}
