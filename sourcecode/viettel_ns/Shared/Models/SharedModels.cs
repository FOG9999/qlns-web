using System;
using DomainModel;
using System.Data;
using System.Data.SqlClient;

namespace VIETTEL.Models
{
    public class SharedModels
    {
        #region hieppm
        public static int iID_MaPhanHe = PhanHeModels.iID_MaPhanHeThuNopNganSach;
        public static DataTable GetLoaiBaoCao()
        {
            var dt = new DataTable();
            dt.Columns.Add("Loai", typeof(string));
            dt.Columns.Add("Ten", typeof(string));
            DataRow dr = dt.NewRow();
            dr["Loai"] = "1";
            dr["Ten"] = "Tờ bìa";
            dt.Rows.InsertAt(dr, 0);

            dr = dt.NewRow();
            dr["Loai"] = "2";
            dr["Ten"] = "Chi tiết đơn vị";
            dt.Rows.InsertAt(dr, 1);

            return dt;
        }
        public static DataTable GetMucChiTietNS()
        {
            var dt = new DataTable();
            dt.Columns.Add("Loai", typeof(string));
            dt.Columns.Add("Ten", typeof(string));
            DataRow dr = dt.NewRow();
            dr["Loai"] = "1";
            dr["Ten"] = "Đến LNS";
            dt.Rows.InsertAt(dr, 0);

            dr = dt.NewRow();
            dr["Loai"] = "2";
            dr["Ten"] = "Đến Mục";
            dt.Rows.InsertAt(dr, 1);

            dr = dt.NewRow();
            dr["Loai"] = "3";
            dr["Ten"] = "Đến Tiểu Mục";
            dt.Rows.InsertAt(dr, 1);

            dr = dt.NewRow();
            dr["Loai"] = "4";
            dr["Ten"] = "Đến Ngành";
            dt.Rows.InsertAt(dr, 1);

            return dt;
        }
        public static DataTable GetLoaiHinhSKT()
        {
            var dt = new DataTable();
            dt.Columns.Add("Loai", typeof(string));
            dt.Columns.Add("Ten", typeof(string));
            DataRow dr = dt.NewRow();
            dr["Loai"] = "1";
            dr["Ten"] = "Phòng ban đề nghị";
            dt.Rows.InsertAt(dr, 0);

            dr = dt.NewRow();
            dr["Loai"] = "2";
            dr["Ten"] = "Số tăng/giảm nhiệm vụ";
            dt.Rows.InsertAt(dr, 1);

            dr = dt.NewRow();
            dr["Loai"] = "3";
            dr["Ten"] = "Số giảm bệnh viện tự chủ";
            dt.Rows.InsertAt(dr, 2);

            dr = dt.NewRow();
            dr["Loai"] = "4";
            dr["Ten"] = "Tổng hợp số đề nghị (bao gồm tăng/giảm)";
            dt.Rows.InsertAt(dr, 3);

            dr = dt.NewRow();
            dr["Loai"] = "5";
            dr["Ten"] = "Tổng hợp số kiểm tra";
            dt.Rows.InsertAt(dr, 4);
            return dt;
        }
        public static DataTable GetPBDN()
        {
            var dt = new DataTable();
            dt.Columns.Add("Loai", typeof(string));
            dt.Columns.Add("Ten", typeof(string));
            DataRow dr = dt.NewRow();
            dr["Loai"] = "1";
            dr["Ten"] = "Phòng (b6,b7,b8,b10)";
            dt.Rows.InsertAt(dr, 0);

            dr = dt.NewRow();
            dr["Loai"] = "2";
            dr["Ten"] = "Phòng KHNS";
            dt.Rows.InsertAt(dr, 1);

            return dt;
        }
        public static DataTable getThongTinCotBaoCao(String sLoaiNS)
        {
            DataTable dt;
            String SQL = "SELECT * FROM TN_CauHinhBaoCao WHERE sLoaiNS=@sLoaiNS Order by iID_MaCotBaoCao";
            SqlCommand cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@sLoaiNS", sLoaiNS);
            dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }

        public static DataTable getThongTinBaoCao(String sLoaiNS, String iID_MaCotBaoCao)
        {
            DataTable dt;
            String SQL = "SELECT * FROM TN_CauHinhBaoCao WHERE sLoaiNS=@sLoaiNS AND iID_MaCotBaoCao=@iID_MaCotBaoCao";
            SqlCommand cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@sLoaiNS", sLoaiNS);
            cmd.Parameters.AddWithValue("@iID_MaCotBaoCao", iID_MaCotBaoCao);
            dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }

        public static String DKDonVi(String MaND, SqlCommand cmd)
        {
            String DKDonVi = "";
            DataTable dtNĐonVi = NganSach_HamChungModels.DSDonViCuaNguoiDung(MaND);
            for (int i = 0; i < dtNĐonVi.Rows.Count; i++)
            {
                DKDonVi += "iID_MaDonVi=@iID_MaDonVi" + i;
                if (i < dtNĐonVi.Rows.Count - 1)
                    DKDonVi += " OR ";
                cmd.Parameters.AddWithValue("@iID_MaDonVi" + i, dtNĐonVi.Rows[i]["iID_MaDonVi"]);
            }
            if (String.IsNullOrEmpty(DKDonVi)) DKDonVi = " AND 0=1";
            else
            {
                DKDonVi = "AND (" + DKDonVi + ")";
            }
            return DKDonVi;
        }

        public static String DKPhongBan(String MaND, SqlCommand cmd, String iID_MaPhongBan)
        {
            String DKPhongBan = "";
            String sTenPB = NguoiDung_PhongBanModels.getTenPhongBan_NguoiDung(MaND);
            if (iID_MaPhongBan == "-1" || iID_MaPhongBan == "" || iID_MaPhongBan == "02" || iID_MaPhongBan == "11")
            {
                if (sTenPB == "02" || sTenPB == "2" || sTenPB == "11")
                    DKPhongBan = " AND 1=1";
                else
                {
                    DKPhongBan = " AND iID_MaPhongBan=@iID_MaPhongBan";
                    cmd.Parameters.AddWithValue("@iID_MaPhongBan", sTenPB);
                }
            }
            else
            {
                DKPhongBan = " AND iID_MaPhongBan=@iID_MaPhongBan";
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan);
            }
            return DKPhongBan;
        }
        public static String DKPhongBan_Dich(String MaND, SqlCommand cmd, String iID_MaPhongBan)
        {
            String DKPhongBan = "";
            String sTenPB = NguoiDung_PhongBanModels.getTenPhongBan_NguoiDung(MaND);
            if (iID_MaPhongBan == "-1" || iID_MaPhongBan == "" || iID_MaPhongBan == "02" || iID_MaPhongBan == "11")
            {
                if (sTenPB == "02" || sTenPB == "2" || sTenPB == "11")
                    DKPhongBan = " AND 1=1";
                else
                {
                    DKPhongBan = " AND iID_MaPhongBanDich=@iID_MaPhongBan";
                    cmd.Parameters.AddWithValue("@iID_MaPhongBan", sTenPB);
                }
            }
            else
            {
                DKPhongBan = " AND iID_MaPhongBanDich=@iID_MaPhongBan";
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan);
            }
            return DKPhongBan;
        }
        public static String DKPhongBan_QuyetToan(String MaND, SqlCommand cmd)
        {
            String DKPhongBan = "";
            String sTenPB = NguoiDung_PhongBanModels.getTenPhongBan_NguoiDung(MaND);
            if (sTenPB != "02")
            {
                DKPhongBan = " AND iID_MaPhongBan=@iID_MaPhongBan";
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", sTenPB);
            }
            return DKPhongBan;
        }

        public static String DKLNS(String MaND, SqlCommand cmd, String iID_MaPhongBan)
        {
            String DKLNS = "";
            DataTable dtLNS = DanhMucModels.NS_LoaiNganSachFull(iID_MaPhongBan);
            for (int i = 0; i < dtLNS.Rows.Count; i++)
            {
                DKLNS += "sLNS=@sLNS" + i;
                if (i < dtLNS.Rows.Count - 1)
                    DKLNS += " OR ";
                cmd.Parameters.AddWithValue("@sLNS" + i, dtLNS.Rows[i]["sLNS"]);
            }
            if (String.IsNullOrEmpty(DKLNS)) DKLNS = " AND 0=1";
            else
            {
                DKLNS = "AND (" + DKLNS + ")";
            }
            return DKLNS;
        }

        public static DataTable listPhongBan(String exceptPB)
        {
            String SQL = "SELECT * FROM TN_ListPhongBan(@exceptPB)";

            SqlCommand cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@exceptPB", exceptPB);

            return Connection.GetDataTable(cmd);
        }

        public static DataTable getDSPhongBan(String iNamLamViec, String MaND)
        {
            String sTenPB = NguoiDung_PhongBanModels.getTenPhongBan_NguoiDung(MaND);
            String DK = "";


            if (sTenPB == "02" || sTenPB == "2" || sTenPB == "11")
            {
                DK = " AND 1=1";
            }
            else
            {
                DK = " AND iID_MaPhongBan=@iID_MaPhongBan";
            }
            if (MaND.StartsWith("trolyphongbanb"))
            {
                DK = " AND sID_MaNguoiDungTao like 'trolyphongbanb%'";
            }

            String SQL = String.Format(@"SELECT DISTINCT iID_MaPhongBan,sTenPhongBan
FROM TN_ChungTuChiTiet
WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec {0}
", DK);
            SqlCommand cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
            cmd.Parameters.AddWithValue("@iID_MaPhongBan", sTenPB);
            DataTable dt = Connection.GetDataTable(cmd);
            return dt;
        }

        public static DataTable getKeHoach(String MaND, String iID_MaPhongBan)
        {
            String DKDonVi = "", DKPhongBan = "";
            SqlCommand cmd = new SqlCommand();
            DKDonVi = SharedModels.DKDonVi(MaND, cmd);
            DKPhongBan = SharedModels.DKPhongBan(MaND, cmd, iID_MaPhongBan);
            String SQL = "";
            if (iID_MaPhongBan == "-1")
            {
                SQL = String.Format(@"SELECT iID_MaPhongBan,
KeHoachNSQP=SUM(CASE WHEN sLNS like ('801%') THEN rTuChi ELSE 0 END)
,KeHoachNSNN=SUM(CASE WHEN sLNS like ('802%') THEN rTuChi ELSE 0 END)
FROM DT_ChungTuChiTiet
WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec {0} {1}
GROUP BY iID_MaPhongBan", DKDonVi, DKPhongBan);
            }
            else
            {
                SQL =
                    String.Format(@"SELECT iID_MaPhongBan,iID_MaDonVi,REPLACE(sTenDonVi,iID_MaDonVi+' - ','') as sTenDonVi,
KeHoachNSQP=SUM(CASE WHEN sLNS like ('801%') THEN rTuChi ELSE 0 END)
,KeHoachNSNN=SUM(CASE WHEN sLNS like ('802%') THEN rTuChi ELSE 0 END)
FROM DT_ChungTuChiTiet
WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec {0} {1}
GROUP BY iID_MaPhongBan,iID_MaDonVi,sTenDonVi", DKDonVi, DKPhongBan);
            }

            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }

        public static DataTable getdtLoaiHinh()
        {
            String SQL = "";
            SQL =
                String.Format(
                    @"SELECT DC_DanhMuc.sTen as  sMoTa,DC_DanhMuc.sTenKhoa as sNG FROM DC_LoaiDanhMuc INNER JOIN DC_DanhMuc ON DC_DanhMuc.iID_MaLoaiDanhMuc = DC_LoaiDanhMuc.iID_MaLoaiDanhMuc WHERE DC_DanhMuc.bHoatDong=1 AND DC_LoaiDanhMuc.sTenBang=N'TN_LoaiHinh' ORDER BY iSTT");
            DataTable dt = Connection.GetDataTable(SQL);
            return dt;
        }
        public static DataTable getdtLNSThuNop()
        {
            String SQL = "";
            SQL =
                String.Format(
                    @"SELECT DISTINCT sLNS,sLNS+' - ' +sMoTa as sMoTa FROM NS_MucLucNganSach WHERE iTrangThai=1 AND sLNS LIKE '8%' AND sL='' AND LEN(sLNS)=7 ORDER BY sLNS");
            DataTable dt = Connection.GetDataTable(SQL);
            return dt;
        }

        public static bool checkXoaThuNop(String iID_MaChungTu)
        {
            String SQL =
                "SELECT COUNT(iID_MaChungTu) FROM TN_ChungTuChiTiet WHERE iTrangThai=1 AND iID_MaChungTu=@iID_MaChungTu";
            SqlCommand cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@iID_MaChungTu", iID_MaChungTu);
            int a = Convert.ToInt32(Connection.GetValue(cmd, 0));
            if (a > 0) return false;
            else
            {
                return true;
            }
        }

        public static bool checkXoaDuToan(String iID_MaChungTu)
        {
            String SQL =
                "SELECT COUNT(iID_MaChungTu) FROM DT_ChungTuChiTiet WHERE iTrangThai=1 AND iID_MaChungTu=@iID_MaChungTu";
            SqlCommand cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@iID_MaChungTu", iID_MaChungTu);
            int a = Convert.ToInt32(Connection.GetValue(cmd, 0));
            if (a > 0) return false;
            else
            {
                return true;
            }
        }

        public static bool checkXoaDuToanBS(String iID_MaChungTu)
        {
            String SQL =
                "SELECT COUNT(iID_MaChungTu) FROM DTBS_ChungTuChiTiet WHERE iTrangThai=1 AND iID_MaChungTu=@iID_MaChungTu";
            SqlCommand cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@iID_MaChungTu", iID_MaChungTu);
            int a = Convert.ToInt32(Connection.GetValue(cmd, 0));
            if (a > 0) return false;
            else
            {
                return true;
            }
        }

        public static bool checkXoaCapPhat(String iID_MaChungTu)
        {
            String SQL =
                "SELECT COUNT(iID_MaCapPhat) FROM CP_CapPhatChiTiet WHERE iTrangThai=1 AND iID_MaCapPhat=@iID_MaChungTu";
            SqlCommand cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@iID_MaChungTu", iID_MaChungTu);
            int a = Convert.ToInt32(Connection.GetValue(cmd, 0));
            if (a > 0) return false;
            else
            {
                return true;
            }
        }

        public static bool checkXoaQuyetToan(String iID_MaChungTu)
        {
            String SQL =
                "SELECT COUNT(iID_MaChungTu) FROM QTA_ChungTuChiTiet WHERE iTrangThai=1 AND iID_MaChungTu=@iID_MaChungTu";
            SqlCommand cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@iID_MaChungTu", iID_MaChungTu);
            int a = Convert.ToInt32(Connection.GetValue(cmd, 0));
            if (a > 0) return false;
            else
            {
                return true;
            }
        }

        public static DataTable getDSLoaiThongTri()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaLoai", typeof(string));
            dt.Columns.Add("sTen", typeof(string));
            DataRow dr = dt.NewRow();
            dr["MaLoai"] = "1";
            dr["sTen"] = "Nộp NSQP";
            dt.Rows.InsertAt(dr, 0);

            dr = dt.NewRow();
            dr["MaLoai"] = "2";
            dr["sTen"] = "Thuế TNDN qua BQP";
            dt.Rows.InsertAt(dr, 1);

            dr = dt.NewRow();
            dr["MaLoai"] = "3";
            dr["sTen"] = "Nộp NSNN Khác";
            dt.Rows.InsertAt(dr, 2);
            return dt;
        }

        public static DataTable getDSIN()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("LoaiIn", typeof(string));
            dt.Columns.Add("sTen", typeof(string));
            DataRow dr = dt.NewRow();
            dr["LoaiIn"] = "All";
            dr["sTen"] = "Tất cả";
            dt.Rows.InsertAt(dr, 0);

            dr = dt.NewRow();
            dr["LoaiIn"] = "Short";
            dr["sTen"] = "Rút gọn";
            dt.Rows.InsertAt(dr, 1);

            dr = dt.NewRow();
            dr["LoaiIn"] = "LoaiKhoan";
            dr["sTen"] = "Đến loại khoản";
            dt.Rows.InsertAt(dr, 2);
            return dt;
        }

        public static DataTable getDsLoaiChuKy()
        {
            DataTable dtILoaiTG = new DataTable();
            dtILoaiTG.Columns.Add("MaLoaiCK", typeof(String));
            dtILoaiTG.Columns.Add("TenLoaiCK", typeof(String));
            DataRow Row;
            Row = dtILoaiTG.NewRow();
            Row[0] = Convert.ToString("0");
            Row[1] = Convert.ToString("Không");
            dtILoaiTG.Rows.Add(Row);
            Row = dtILoaiTG.NewRow();
            Row[0] = Convert.ToString("1");
            Row[1] = Convert.ToString("Có");
            dtILoaiTG.Rows.Add(Row);
            return dtILoaiTG;
        }
        #endregion

        #region duonglh3
        public static String DKDonVi(String MaND, SqlCommand cmd, String tblAlias)
        {
            String DKDonVi = "";
            DataTable dtNĐonVi = NganSach_HamChungModels.DSDonViCuaNguoiDung(MaND);
            for (int i = 0; i < dtNĐonVi.Rows.Count; i++)
            {
                DKDonVi += tblAlias + ".iID_MaDonVi=@iID_MaDonVi" + i;
                if (i < dtNĐonVi.Rows.Count - 1)
                    DKDonVi += " OR ";
                cmd.Parameters.AddWithValue("@iID_MaDonVi" + i, dtNĐonVi.Rows[i]["iID_MaDonVi"]);
            }
            if (String.IsNullOrEmpty(DKDonVi)) DKDonVi = " AND 0=1";
            else
            {
                DKDonVi = "AND (" + DKDonVi + ")";
            }
            return DKDonVi;
        }
        public static String DKPhongBan(String MaND, SqlCommand cmd, String iID_MaPhongBan, String tblAlias)
        {
            String DKPhongBan = "";
            String sTenPB = NguoiDung_PhongBanModels.getTenPhongBan_NguoiDung(MaND);
            if (iID_MaPhongBan == "-1" || iID_MaPhongBan == "")
            {
                if (sTenPB == "02" || sTenPB == "2")
                    DKPhongBan = " AND 1=1";
                else
                {
                    DKPhongBan = string.Format(" AND {0}.iID_MaPhongBan=@iID_MaPhongBan", tblAlias);
                    cmd.Parameters.AddWithValue("@iID_MaPhongBan", sTenPB);
                }
            }
            else
            {
                DKPhongBan = string.Format(" AND {0}.iID_MaPhongBan=@iID_MaPhongBan", tblAlias);
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan);
            }
            return DKPhongBan;
        }
        public static String DKPhongBan_QuyetToan(String MaND, SqlCommand cmd, string tblAlias)
        {
            String DKPhongBan = "";
            String sTenPB = NguoiDung_PhongBanModels.getTenPhongBan_NguoiDung(MaND);
            if (sTenPB != "02")
                DKPhongBan = string.Format(" AND {0}.iID_MaPhongBan=@iID_MaPhongBan", tblAlias);
            cmd.Parameters.AddWithValue("@iID_MaPhongBan", sTenPB);

            return DKPhongBan;
        }
        #endregion

    }
}