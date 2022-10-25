using DomainModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Viettel.Services;

namespace VIETTEL.Models
{
    public class MucLucNganSachModels
    {
        //Danh sách trường của mục lục ngân sách
        public static String strDSTruongTienTieuDe = "Tên công trình,Ngày,Người,Chi tại kho bạc,Tồn kho,Tự chi,Chi tập trung,Hàng nhập,Hàng mua,Hiện vật,Phân cấp,Dự phòng";
        public static String strDSTruongTienTieuDe_So = "Ngày,Người,Chi tại kho bạc,Tồn kho,Tự chi,Chi tại ngành,Chi tập trung,Hàng nhập,Hàng mua,Hiện vật,Phân cấp,Dự phòng";
        public static String strDSTruongTien_So = "rNgay,rSoNguoi,rChiTaiKhoBac,rTonKho,rTuChi,rChiTaiNganh,rChiTapTrung,rHangNhap,rHangMua,rHienVat,rPhanCap,rDuPhong";
        public static String strDSTruongTien_Xau = "sTenCongTrinh";
        public static String strDSTruongTien = strDSTruongTien_Xau + "," + strDSTruongTien_So;

        public static String strDSTruongTienDoRong_So = "100,100,100,100,100,100,100,100,100,100,100,100";
        public static String strDSTruongTienDoRong_Xau = "150";
        public static String strDSTruongTienDoRong = strDSTruongTienDoRong_Xau + "," + strDSTruongTienDoRong_So;

        public static String strDSTruongTien_Full = strDSTruongTien + ",rTongSo";
        public static String strDSTruongTienDoRong_Full = strDSTruongTienDoRong + ",100";
        public static String strDSTruongTienTieuDe_Full = strDSTruongTienTieuDe + ",Tổng số";

        public static String strDSDuocNhapTruongTien = "b" + strDSTruongTien.Replace(",", ",b");

        public static String strDSTruongSapXep = "sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG";

        public static String strDSTruongTieuDe = "LNS,L,K,M,TM,TTM,NG,TNG,Nội dung";
        public static String strDSTruong = "sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,sMoTa";

        public static String strDSTruongTieuDeQuyetToan = "LNS,L,K,M,TM,TTM,NG,TNG,Nội dung,Giá trị";
        public static String strDSTruongQuyetToan = "sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,sMoTa,fGiaTriUSD";

        public static String strDSTruongDoRong = "60,30,30,40,40,30,30,30,280";

        public static String strDSTruongDoRongQuyetToan = "60,30,30,40,40,30,30,30,280,100";

        public static String strDSTruong_LNS_New = "sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa";
        public static String strDSTruongDoRong_LNS_New = "60,40,40,40,40,40,40,350";

        public static String strDSTruong_LNS = "sM,sTM,sTTM,sNG,sMoTa";
        public static String strDSTruongDoRong_LNS = "40,40,40,40,350";
        public static String strDSTruong_80 = "sLNS,sNG,sMoTa";
        public static String strDSTruongDoRong_80 = "60,40,350";
        public static String[] arrDSTruongTieuDe = strDSTruongTieuDe.Split(',');
        public static String[] arrDSTruong = strDSTruong.Split(',');
        public static String[] arrDSTruongQuyetToan = strDSTruongQuyetToan.Split(',');
        public static String[] arrDSTruongTieuDeQuyetToan = strDSTruongTieuDeQuyetToan.Split(',');

        public static String[] arrDSTruongDoRong = strDSTruongDoRong.Split(',');

        public static String[] arrDSTruongDoRongQuyetToan = strDSTruongDoRongQuyetToan.Split(',');


        public static String B6 = "30,34,35,50,69,70,71,72,73,78,80,88,89,90,91,92,93,94";
        public static String B7 = "10,11,12,13,14,15,17,19,21,22,23,24,29,31,33,41,42,43,44,45,46,47,81,82,83,84,87,95,96,97,98";
        public static String B10 = "02,03,51,52,53,55,56,57,61,65,75,76,77,79,99";

        //Danh sách trường tiền của phần Thu Nộp Ngân Sách        
        public static String strDSTruongTien_ThuNopTieuDe = "Tổng số thu, Tổng số chi phí,Tổng số QTNS, Khấu hao TSCĐ, Tiền lương, QTNS Khác, Chi phí khác, Tổng số nộp NSNN,Thuế GTGT, Tổng nộp thuế TNDN, Trong đó: nộp qua BQP, Phí/lệ phí, Tổng số nộp NSNN - Khác, Trong đó: nộp qua BQP, Chênh lệch, Nộp NSQP, Bổ sung kinh phí, Trích các quỹ, Số chưa phân phối, Ghi chú";
        public static String strDSTruongTien_ThuNop_So = "rTongThu,rTongChiPhi,rTongQTNS,rKhauHaoTSCD,rTienLuong,rQTNSKhac,rChiPhiKhac,rTongNopNSNN,rNopThueGTGT,rTongNopThueTNDN,rNopThueTNDNBQP,rPhiLePhi,rTongNopNSNNKhac,rNopNSNNKhacBQP,rChenhLech,rNopNSQP,rBoSungKinhPhi,rTrichQuyDonVi,rSoChuaPhanPhoi";
        public static String strDSTruongTien_ThuNop_So_Loai1 = "rKeHoach,rThu,rThoaiThu,rTongThu";

        public static String strDSTruongTien_ThuNop_Xau = "sGhiChu";
        public static String strDSTruongTien_ThuNop = strDSTruongTien_ThuNop_So + "," + strDSTruongTien_ThuNop_Xau;
        public static String strDSTruongTien_ThuNop_Loai1 = strDSTruongTien_ThuNop_So_Loai1 + "," + strDSTruongTien_ThuNop_Xau;

        public static String strDSTruongTien_ThuNopDoRong = "150,120,120,120,120,120,120,120,120,120,150,120,150,150,120,120,120,120,120,250";


        public static DataTable NS_LoaiNganSach()
        {
            SqlCommand cmd = new SqlCommand(String.Format("SELECT distinct(sLNS),sMoTa,{0} FROM NS_MucLucNganSach WHERE iNamLamViec = @iNamLamViec AND LEN(sLNS)=7 AND sL='' ORDER BY sLNS", strDSDuocNhapTruongTien + ",sNhapTheoTruong"));
            int iNamLamViec = Convert.ToInt32(ReportModels.LayNamLamViec(System.Web.HttpContext.Current.User.Identity.Name));
            cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
            DataTable vR = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return vR;
        }

        public static DataTable dt_ChiTietMucLucNganSach(String iID_MaMucLucNganSach)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM NS_MucLucNganSach WHERE iID_MaMucLucNganSach=@iID_MaMucLucNganSach");
            cmd.Parameters.AddWithValue("@iID_MaMucLucNganSach", iID_MaMucLucNganSach);
            DataTable vR = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return vR;
        }

        public static DataTable dt_ChiTietMucLucNganSach_XauNoiMa(String sXauNoiMa)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM NS_MucLucNganSach WHERE sXauNoiMa=@sXauNoiMa");
            cmd.Parameters.AddWithValue("@sXauNoiMa", sXauNoiMa);
            DataTable vR = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return vR;
        }

        public static DataTable dt_ChiTietMucLucNganSach_XauNoiMa_KeCan(String sXauNoiMa)
        {
            SqlCommand cmd = new SqlCommand("SELECT TOP 1 * FROM NS_MucLucNganSach WHERE sXauNoiMa LIKE @sXauNoiMa");
            cmd.Parameters.AddWithValue("@sXauNoiMa", sXauNoiMa + "%");
            DataTable vR = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return vR;
        }

        public static DataTable dt_ChiTiet_TheoDSTruong(String[] arrDSGiaTri)
        {
            SqlCommand cmd = new SqlCommand();
            String DK = "";
            int i = 0;
            for (i = 0; i < arrDSTruong.Length - 1; i++)
            {
                DK += String.Format(" AND {0}=@{0}", arrDSTruong[i]);
                cmd.Parameters.AddWithValue("@" + arrDSTruong[i], arrDSGiaTri[i]);
            }

            String SQL = String.Format("SELECT TOP 2 * FROM NS_MucLucNganSach WHERE bLaHangCha=0 {0}", DK);
            cmd.CommandText = SQL;
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }
        public static DataTable dt_ChiTiet_TheoDSTruong_DuToan(String[] arrDSGiaTri)
        {
            SqlCommand cmd = new SqlCommand();
            String DK = "";
            int i = 0;
            for (i = 0; i < arrDSTruong.Length - 2; i++)
            {
                DK += String.Format(" AND {0}=@{0}", arrDSTruong[i]);
                cmd.Parameters.AddWithValue("@" + arrDSTruong[i], arrDSGiaTri[i]);
            }

            String SQL = String.Format("SELECT TOP 2 * FROM NS_MucLucNganSach WHERE bLaHangCha=0 {0}", DK);
            cmd.CommandText = SQL;
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }

        public static void CapNhapLai()
        {
            SqlCommand cmd;
            String[] arrDSTruong = MucLucNganSachModels.strDSTruong.Split(',');
            //Cap nhap lai xau ma
            String SQL = "SELECT * FROM NS_MucLucNganSach WHERE iNamLamViec=@iNamLamViec AND iTrangThai=1 ORDER BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG";
            cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@iNamLamViec", NguoiDungCauHinhModels.iNamLamViec);
            DataTable dt = Connection.GetDataTable(cmd);
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                DataRow RMucLucNganSach = dt.Rows[i];
                string sXauNoiMa = "";
                Boolean bLaHangCha = false;
                for (int j = 0; j < arrDSTruong.Length - 1; j++)
                {
                    if (String.IsNullOrEmpty(Convert.ToString(RMucLucNganSach[arrDSTruong[j]])) == false)
                    {
                        if (sXauNoiMa != "") sXauNoiMa += "-";
                        sXauNoiMa += Convert.ToString(RMucLucNganSach[arrDSTruong[j]]);
                    }
                    else
                    {
                        if (j < arrDSTruong.Length - 2)
                        {
                            bLaHangCha = true;
                        }
                    }
                }
                cmd = new SqlCommand("UPDATE NS_MucLucNganSach SET sXauNoiMa=@sXauNoiMa, bLaHangCha=@bLaHangCha WHERE iID_MaMucLucNganSach=@iID_MaMucLucNganSach");
                cmd.Parameters.AddWithValue("@sXauNoiMa", sXauNoiMa);
                cmd.Parameters.AddWithValue("@bLaHangCha", bLaHangCha);
                cmd.Parameters.AddWithValue("@iID_MaMucLucNganSach", RMucLucNganSach["iID_MaMucLucNganSach"]);
                Connection.UpdateDatabase(cmd);
                cmd.Dispose();
            }
            dt.Dispose();
            //Cap nhap lai ma cha
            SQL = "SELECT * FROM NS_MucLucNganSach WHERE iNamLamViec=@iNamLamViec AND iTrangThai=1 ORDER BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG";
            cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@iNamLamViec", NguoiDungCauHinhModels.iNamLamViec);
            dt = Connection.GetDataTable(cmd);
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                DataRow RMucLucNganSach = dt.Rows[i];
                Object iID_MaMucLucNganSachCha = DBNull.Value;

                string sXauNoiMa = Convert.ToString(RMucLucNganSach["sXauNoiMa"]);

                for (int j = i - 1; j >= 0; j--)
                {
                    String tg_sXauNoiMa = Convert.ToString(dt.Rows[j]["sXauNoiMa"]);
                    if (sXauNoiMa.StartsWith(tg_sXauNoiMa.Trim()))
                    {
                        cmd = new SqlCommand("UPDATE NS_MucLucNganSach SET iID_MaMucLucNganSach_Cha=@iID_MaMucLucNganSach_Cha WHERE iID_MaMucLucNganSach=@iID_MaMucLucNganSach");
                        cmd.Parameters.AddWithValue("@iID_MaMucLucNganSach", RMucLucNganSach["iID_MaMucLucNganSach"]);
                        cmd.Parameters.AddWithValue("@iID_MaMucLucNganSach_Cha", dt.Rows[j]["iID_MaMucLucNganSach"]);
                        Connection.UpdateDatabase(cmd);
                        cmd.Dispose();
                        break;
                    }
                }
            }
            dt.Dispose();
        }

        //Chuc nang sua: luu data muc luc ngan sach
        //Ngay sua: 6/5/2021
        //Nguoi sua: anhht
        //Noi dung: cai thien toc do khi luu man muc luc ngan sach
        public static void CapNhapLai(String sLNS, string MaND = "")
        {
            IConnectionFactory _connectionFactory = ConnectionFactory.Default;
            SqlCommand cmd;

            if (sLNS == "0")
                sLNS = "";

            String[] arrDSTruong = MucLucNganSachModels.strDSTruong.Split(',');
            var nam = MucLucNganSachModels.LayNamLamViec(MaND);
            //Cap nhap lai xau ma
            String DK = "";
            //if (String.IsNullOrEmpty(sLNS) == false) DK = " WHERE iNamLamViec=@iNamLamViec AND sLNS LIKE '" + sLNS + "%'";
            DK = " WHERE iNamLamViec=@iNamLamViec AND sLNS LIKE '" + sLNS + "%'";


            String SQL = String.Format("SELECT * FROM NS_MucLucNganSach {0} AND iTrangThai=1 ORDER BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG", DK);
            cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@iNamLamViec", nam);
            DataTable dt = Connection.GetDataTable(cmd);
            String sXauNoiMa1 = "";
            // anhht: tao datatable chua data de chuyen xuong procedure xu ly
            DataTable dtDataUpdate = new DataTable();
            dtDataUpdate.Columns.Add("iID_MaMucLucNganSach");
            dtDataUpdate.Columns.Add("iID_MaMucLucNganSach_Cha");
            dtDataUpdate.Columns.Add("sXauNoiMa");
            dtDataUpdate.Columns.Add("bLaHangCha");
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                DataRow RMucLucNganSach = dt.Rows[i];
                string sXauNoiMa = "";

                Boolean bLaHangCha = false;
                for (int j = 0; j < arrDSTruong.Length - 1; j++)
                {
                    if (String.IsNullOrEmpty(Convert.ToString(RMucLucNganSach[arrDSTruong[j]])) == false)
                    {
                        if (sXauNoiMa != "") sXauNoiMa += "-";
                        sXauNoiMa += Convert.ToString(RMucLucNganSach[arrDSTruong[j]]);
                    }
                    else
                    {
                        if (j < arrDSTruong.Length - 2)
                        {
                            bLaHangCha = true;
                        }
                    }
                }
                if (i == dt.Rows.Count - 1)
                {
                    sXauNoiMa1 = sXauNoiMa;
                }
                if (sXauNoiMa1.StartsWith(sXauNoiMa) && sXauNoiMa1.Equals(sXauNoiMa) == false)
                {
                    bLaHangCha = true;
                }
                sXauNoiMa1 = sXauNoiMa;
                // anhht: them row moi, add vao datatable o tren
                DataRow dr = dtDataUpdate.NewRow();
                dr["iID_MaMucLucNganSach"] = Convert.ToString(RMucLucNganSach["iID_MaMucLucNganSach"]);
                dr["sXauNoiMa"] = sXauNoiMa;
                dr["bLaHangCha"] = bLaHangCha;
                dtDataUpdate.Rows.Add(dr);
            }

            // anhht: su dung procedure de update lai data, tranh viec tao nhieu connection dan den toc do cham
            if (dtDataUpdate.Rows.Count > 0)
            {
                using (var conn = _connectionFactory.GetConnection())
                {

                    using (var cmd1 = new SqlCommand("u_sXauNoiMa_bLaHangCha_NS_MucLucNganSach", conn))
                    {
                        cmd1.CommandType = CommandType.StoredProcedure;
                        SqlParameter tvpParam = cmd1.Parameters.AddWithValue("@Values", dtDataUpdate);
                        tvpParam.SqlDbType = SqlDbType.Structured;
                        tvpParam.TypeName = "dbo.TempNSMLNS";

                        conn.Open();
                        cmd1.ExecuteNonQuery();
                        conn.Close();
                    }

                }
            }

            dt.Dispose();
            //Cap nhap lai ma cha
            SQL = String.Format("SELECT * FROM NS_MucLucNganSach {0} AND iTrangThai = 1 ORDER BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG", DK);
            cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@iNamLamViec", nam);
            dt = Connection.GetDataTable(cmd);
            dtDataUpdate.Clear();

            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                DataRow RMucLucNganSach = dt.Rows[i];
                Object iID_MaMucLucNganSachCha = DBNull.Value;

                string sXauNoiMa = Convert.ToString(RMucLucNganSach["sXauNoiMa"]);

                for (int j = i - 1; j >= 0; j--)
                {
                    String tg_sXauNoiMa = Convert.ToString(dt.Rows[j]["sXauNoiMa"]);
                    if (sXauNoiMa.StartsWith(tg_sXauNoiMa))
                    {
                        // anhht: them row moi, add vao datatable o tren
                        DataRow dr = dtDataUpdate.NewRow();
                        dr["iID_MaMucLucNganSach"] = RMucLucNganSach["iID_MaMucLucNganSach"];
                        dr["iID_MaMucLucNganSach_Cha"] = dt.Rows[j]["iID_MaMucLucNganSach"];
                        dtDataUpdate.Rows.Add(dr);
                        break;
                    }
                }
            }

            // anhht: su dung procedure de update lai data, tranh viec tao nhieu connection dan den toc do cham
            if (dtDataUpdate.Rows.Count > 0)
            {
                using (var conn = _connectionFactory.GetConnection())
                {

                    using (var cmd1 = new SqlCommand("u_iID_MaMucLucNganSach_Cha_NS_MucLucNganSach", conn))
                    {
                        cmd1.CommandType = CommandType.StoredProcedure;
                        SqlParameter tvpParam = cmd1.Parameters.AddWithValue("@Values", dtDataUpdate);
                        tvpParam.SqlDbType = SqlDbType.Structured;
                        tvpParam.TypeName = "dbo.TempNSMLNS";

                        conn.Open();
                        cmd1.ExecuteNonQuery();
                        conn.Close();
                    }

                }
            }
            dt.Dispose();
        }

        public static DataTable Get_dtDanhSachMucLucNganSach(Dictionary<String, String> arrGiaTriTimKiem, String MaND)
        {
            String SQL = "SELECT * FROM NS_MucLucNganSach WHERE iNamLamViec=@iNamLamViec   AND {0} ORDER BY sXauNoiMa";
            int iNamLamViec = Convert.ToInt32(MucLucNganSachModels.LayNamLamViec(MaND));
            String DK = "";
            SqlCommand cmd = new SqlCommand();
            #region Điều kiện
            if (arrGiaTriTimKiem != null)
            {
                //DK = DK + " (";
                for (int i = 0; i < arrDSTruong.Length; i++)
                {
                    if (String.IsNullOrEmpty(arrGiaTriTimKiem[arrDSTruong[i]]) == false)
                    {
                        if (!String.IsNullOrEmpty(DK)) DK += " AND ";
                        if (arrGiaTriTimKiem[arrDSTruong[i]].ToString() == "0" && arrDSTruong[i] == "sLNS")
                        {
                            DK += "1=1";
                        }
                        else if (arrGiaTriTimKiem[arrDSTruong[i]].Length < 7 && arrDSTruong[i] == "sLNS")
                        {
                            DK += String.Format("{0} LIKE @{0}", arrDSTruong[i]);
                            cmd.Parameters.AddWithValue("@" + arrDSTruong[i], arrGiaTriTimKiem[arrDSTruong[i]] + "%");
                        }
                        else
                        {
                            DK += String.Format("{0}=@{0}", arrDSTruong[i]);
                            cmd.Parameters.AddWithValue("@" + arrDSTruong[i], arrGiaTriTimKiem[arrDSTruong[i]]);
                        }
                    }
                }
                if (String.IsNullOrEmpty(DK) == false)
                {
                    DK = "AND ( " + DK + ")";
                }
                else
                {
                    DK = "AND (1 = 1)";
                }

            }
            #endregion Điều kiện
            DK = "iTrangThai=1 " + DK;
            if (MaND != "hieppm")
            {
                DK += "AND sLNS <> ''";
            }
            SQL = String.Format(SQL, DK);
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
            DataTable vR = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return vR;
        }

        public static DataTable Get_dtDanhSachMucLucNganSachQuyetToan(Dictionary<String, String> arrGiaTriTimKiem, String MaND, String idChuyenQuyetToan)
        {
            String SQL = "SELECT * FROM NH_QT_ChuyenQuyetToan_ChiTiet WHERE iNamLamViec=@iNamLamViec  AND (@idChuyenQuyetToan IS NULL OR iID_ChuyenQuyetToanID = @idChuyenQuyetToan)  AND {0} ORDER BY sXauNoiMa";
            int iNamLamViec = Convert.ToInt32(MucLucNganSachModels.LayNamLamViec(MaND));
            String DK = "";
            SqlCommand cmd = new SqlCommand();
            #region Điều kiện
            if (arrGiaTriTimKiem != null)
            {
                //DK = DK + " (";
                for (int i = 0; i < arrDSTruongQuyetToan.Length; i++)
                {
                    if (String.IsNullOrEmpty(arrGiaTriTimKiem[arrDSTruongQuyetToan[i]]) == false)
                    {
                        if (!String.IsNullOrEmpty(DK)) DK += " AND ";
                        if (arrGiaTriTimKiem[arrDSTruongQuyetToan[i]].ToString() == "0" && arrDSTruongQuyetToan[i] == "sLNS")
                        {
                            DK += "1=1";
                        }
                        else if (arrGiaTriTimKiem[arrDSTruongQuyetToan[i]].Length < 7 && arrDSTruongQuyetToan[i] == "sLNS")
                        {
                            DK += String.Format("{0} LIKE @{0}", arrDSTruongQuyetToan[i]);
                            cmd.Parameters.AddWithValue("@" + arrDSTruongQuyetToan[i], arrGiaTriTimKiem[arrDSTruongQuyetToan[i]] + "%");
                        }
                        else if (arrGiaTriTimKiem[arrDSTruongQuyetToan[i]].Length > 0 && arrDSTruongQuyetToan[i] == "sMoTa")
                        {
                            DK += String.Format("{0} LIKE CONCAT(N'%', @{0}, N'%')", arrDSTruongQuyetToan[i]);
                            cmd.Parameters.AddWithValue("@" + arrDSTruongQuyetToan[i], arrGiaTriTimKiem[arrDSTruongQuyetToan[i]]);
                        }
                        else
                        {
                            DK += String.Format("{0}=@{0}", arrDSTruongQuyetToan[i]);
                            cmd.Parameters.AddWithValue("@" + arrDSTruongQuyetToan[i], arrGiaTriTimKiem[arrDSTruongQuyetToan[i]]);
                        }
                    }
                }
                if (String.IsNullOrEmpty(DK) == false)
                {
                    DK = "AND ( " + DK + ")";
                }
                else
                {
                    DK = "AND (1 = 1)";
                }

            }
            #endregion Điều kiện
            DK = "iTrangThai=1 " + DK;
            if (MaND != "hieppm")
            {
                DK += "AND sLNS <> ''";
            }
            SQL = String.Format(SQL, DK);
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
            cmd.Parameters.AddWithValue("@idChuyenQuyetToan", idChuyenQuyetToan);
            DataTable vR = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return vR;
        }

        public static DataTable Get_dtDanhSachMucLucNganSachQuyetToanFistTime(Dictionary<String, String> arrGiaTriTimKiem, String MaND)
        {
            String SQL = "SELECT  mlns.*,cqt.fGiaTriUSD,cqt.iID_ChuyenQuyetToanChiTietID FROM NS_MucLucNganSach mlns left join NH_QT_ChuyenQuyetToan_ChiTiet cqt on mlns.iID_MaMucLucNganSach = cqt.iID_MaMucLucNganSach" +
                " WHERE mlns.iNamLamViec=@iNamLamViec  AND mlns.sLNS like '3%'  AND {0} ORDER BY sXauNoiMa";
            int iNamLamViec = Convert.ToInt32(MucLucNganSachModels.LayNamLamViec(MaND));
            String DK = "";
            SqlCommand cmd = new SqlCommand();
            #region Điều kiện
            if (arrGiaTriTimKiem != null)
            {
                //DK = DK + " (";
                for (int i = 0; i < arrDSTruongQuyetToan.Length; i++)
                {
                    if (String.IsNullOrEmpty(arrGiaTriTimKiem[arrDSTruongQuyetToan[i]]) == false)
                    {
                        if (!String.IsNullOrEmpty(DK)) DK += " AND ";
                        if (arrGiaTriTimKiem[arrDSTruongQuyetToan[i]].ToString() == "0" && arrDSTruongQuyetToan[i] == "sLNS")
                        {
                            DK += "1=1";
                        }
                        else if (arrGiaTriTimKiem[arrDSTruongQuyetToan[i]].Length < 7 && arrDSTruongQuyetToan[i] == "sLNS")
                        {
                            DK += String.Format("{0} LIKE @{0}", arrDSTruongQuyetToan[i]);
                            cmd.Parameters.AddWithValue("@" + arrDSTruongQuyetToan[i], arrGiaTriTimKiem[arrDSTruongQuyetToan[i]] + "%");
                        }
                        else
                        {
                            DK += String.Format("{0}=@{0}", arrDSTruongQuyetToan[i]);
                            cmd.Parameters.AddWithValue("@" + arrDSTruongQuyetToan[i], arrGiaTriTimKiem[arrDSTruongQuyetToan[i]]);
                        }
                    }
                }
                if (String.IsNullOrEmpty(DK) == false)
                {
                    DK = "AND ( " + DK + ")";
                }
                else
                {
                    DK = "AND (1 = 1)";
                }

            }
            #endregion Điều kiện
            DK = "mlns.iTrangThai=1 " + DK;
            if (MaND != "hieppm")
            {
                DK += "AND mlns.sLNS <> ''";
            }
            SQL = String.Format(SQL, DK);
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
            DataTable vR = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return vR;
        }

        public static DataTable Get_dtDanhSachMucLucNganSach_Nhom()
        {
            String SQL = "SELECT DISTINCT sLNS,sL + '-'+ sK + '-'+ sM as sM FROM NS_MucLucNganSach WHERE iTrangThai=1 AND sTM='' ORDER BY sLNS,sL,sK";
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = SQL;
            DataTable vR = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return vR;
        }



        public static DataTable Get_dtMucLucNganSach(int Trang = 1, int SoBanGhi = 0, String sLNS = "", String sL = "", String sK = "", String sM = "", String sTM = "", String sTTM = "", String sNG = "", String sTNG = "")
        {
            String SQL = "SELECT * FROM NS_MucLucNganSach {0} ";
            String DK = "";
            SqlCommand cmd = new SqlCommand();
            #region Điều kiện
            if (String.IsNullOrEmpty(sLNS) == false)
            {
                if (String.IsNullOrEmpty(DK) == false) DK += " AND ";
                DK = "sLNS=@sLNS";
                cmd.Parameters.AddWithValue("@sLNS", sLNS);
            }
            if (String.IsNullOrEmpty(sL) == false)
            {
                if (String.IsNullOrEmpty(DK) == false) DK += " AND ";
                DK = "sL=@sL";
                cmd.Parameters.AddWithValue("@sL", sL);
            }
            if (String.IsNullOrEmpty(sK) == false)
            {
                if (String.IsNullOrEmpty(DK) == false) DK += " AND ";
                DK = "sK=@sK";
                cmd.Parameters.AddWithValue("@sK", sK);
            }
            if (String.IsNullOrEmpty(sM) == false)
            {
                if (String.IsNullOrEmpty(DK) == false) DK += " AND ";
                DK = "sM=@sM";
                cmd.Parameters.AddWithValue("@sM", sM);
            }
            if (String.IsNullOrEmpty(sTM) == false)
            {
                if (String.IsNullOrEmpty(DK) == false) DK += " AND ";
                DK = "sTM=@sTM";
                cmd.Parameters.AddWithValue("@sTM", sTM);
            }
            if (String.IsNullOrEmpty(sTTM) == false)
            {
                if (String.IsNullOrEmpty(DK) == false) DK += " AND ";
                DK = "sTTM=@sTTM";
                cmd.Parameters.AddWithValue("@sTTM", sTTM);
            }
            if (String.IsNullOrEmpty(sNG) == false)
            {
                if (String.IsNullOrEmpty(DK) == false) DK += " AND ";
                DK = "sNG=@sNG";
                cmd.Parameters.AddWithValue("@sNG", sNG);
            }
            if (String.IsNullOrEmpty(sTNG) == false)
            {
                if (String.IsNullOrEmpty(DK) == false) DK += " AND ";
                DK = "sTNG=@sTNG";
                cmd.Parameters.AddWithValue("@sTNG", sTNG);
            }

            #endregion Điều kiện

            if (String.IsNullOrEmpty(DK) == false) DK = " WHERE " + DK;
            SQL = String.Format(SQL, DK);
            cmd.CommandText = SQL;
            DataTable vR = CommonFunction.dtData(cmd, "iSTT", Trang, SoBanGhi);
            cmd.Dispose();
            return vR;
        }

        public static int Get_MucLucNganSach_Count(String sLNS = "", String sL = "", String sK = "", String sM = "", String sTM = "", String sTTM = "", String sNG = "", String sTNG = "")
        {
            String SQL = "SELECT COUNT(*) FROM NS_MucLucNganSach {0} ";
            String DK = "";
            SqlCommand cmd = new SqlCommand();
            #region Điều kiện
            if (String.IsNullOrEmpty(sLNS) == false)
            {
                if (String.IsNullOrEmpty(DK) == false) DK += " AND ";
                DK = "sLNS=@sLNS";
                cmd.Parameters.AddWithValue("@sLNS", sLNS);
            }
            if (String.IsNullOrEmpty(sL) == false)
            {
                if (String.IsNullOrEmpty(DK) == false) DK += " AND ";
                DK = "sL=@sL";
                cmd.Parameters.AddWithValue("@sL", sL);
            }
            if (String.IsNullOrEmpty(sK) == false)
            {
                if (String.IsNullOrEmpty(DK) == false) DK += " AND ";
                DK = "sK=@sK";
                cmd.Parameters.AddWithValue("@sK", sK);
            }
            if (String.IsNullOrEmpty(sM) == false)
            {
                if (String.IsNullOrEmpty(DK) == false) DK += " AND ";
                DK = "sM=@sM";
                cmd.Parameters.AddWithValue("@sM", sM);
            }
            if (String.IsNullOrEmpty(sTM) == false)
            {
                if (String.IsNullOrEmpty(DK) == false) DK += " AND ";
                DK = "sTM=@sTM";
                cmd.Parameters.AddWithValue("@sTM", sTM);
            }
            if (String.IsNullOrEmpty(sTTM) == false)
            {
                if (String.IsNullOrEmpty(DK) == false) DK += " AND ";
                DK = "sTTM=@sTTM";
                cmd.Parameters.AddWithValue("@sTTM", sTTM);
            }
            if (String.IsNullOrEmpty(sNG) == false)
            {
                if (String.IsNullOrEmpty(DK) == false) DK += " AND ";
                DK = "sNG=@sNG";
                cmd.Parameters.AddWithValue("@sNG", sNG);
            }
            if (String.IsNullOrEmpty(sTNG) == false)
            {
                if (String.IsNullOrEmpty(DK) == false) DK += " AND ";
                DK = "sTNG=@sTNG";
                cmd.Parameters.AddWithValue("@sTNG", sTNG);
            }

            #endregion Điều kiện

            if (String.IsNullOrEmpty(DK) == false) DK = " WHERE " + DK;
            SQL = String.Format(SQL, DK);
            cmd.CommandText = SQL;
            int vR = Convert.ToInt32(Connection.GetValue(cmd, 0));
            cmd.Dispose();

            return vR;
        }
        public static String LayNamLamViec(String MaND)
        {
            DataTable dt = NguoiDungCauHinhModels.LayCauHinh(MaND);
            string iNamLamViec = DateTime.Now.Year.ToString();
            if (dt.Rows.Count > 0)
            {
                iNamLamViec = Convert.ToString(dt.Rows[0]["iNamLamViec"]);
            }

            return iNamLamViec;
        }
    }
}
