using DomainModel;
using DomainModel.Abstract;
using System;
using System.Data;
using System.Data.SqlClient;
namespace VIETTEL.Models
{
    public class CapPhatModels
    {
        public static int iID_MaPhanHe = PhanHeModels.iID_MaPhanHeCapPhat;
        public static void ChuyenNamSau(String sMaND, String sTenBangChungTu = "CP_CapPhat", String sTenBangChiTiet = "CP_CapPhatChiTiet")
        {
            DataTable dtCauHinh = NguoiDungCauHinhModels.LayCauHinh(sMaND);
            int iNamLamViec = Convert.ToInt16(dtCauHinh.Rows[0]["iNamLamViec"]);

            String SQL = String.Format("SELECT * FROM {0} WHERE iTrangThai=1 AND iID_MaTrangThaiDuyet={1} AND  iID_MaNamNganSach=3 AND iNamLamViec=@iNamLamViec", sTenBangChungTu, LuongCongViecModel.Get_iID_MaTrangThaiDuyet_DaDuyet(PhanHeModels.iID_MaPhanHeCapPhat));
            DataTable dtChungTu;
            SqlCommand cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
            dtChungTu = Connection.GetDataTable(cmd);
            cmd.Dispose();

            Bang bang = new Bang(sTenBangChungTu);
            Bang bangChiTiet = new Bang(sTenBangChiTiet);
            String iID_MaCapPhat, iID_MaCapPhatatGoc;
            DataTable dtChiTiet;
            for (int i = 0; i < dtChungTu.Rows.Count; i++)
            {

                iID_MaCapPhatatGoc = Convert.ToString(dtChungTu.Rows[i]["iID_MaCapPhat"]);
                SQL = String.Format("SELECT * FROM {0} WHERE iTrangThai=1 AND iID_MaTrangThaiDuyet={1} AND  iID_MaCapPhat=@iID_MaCapPhat", sTenBangChiTiet, LuongCongViecModel.Get_iID_MaTrangThaiDuyet_DaDuyet(PhanHeModels.iID_MaPhanHeCapPhat));
                cmd = new SqlCommand(SQL);
                cmd.Parameters.AddWithValue("@iID_MaCapPhat", iID_MaCapPhatatGoc);
                dtChiTiet = Connection.GetDataTable(cmd);
                cmd.Dispose();
                //add du liệu các cột vào param     
                for (int j = 0; j < dtChungTu.Columns.Count; j++)
                {
                    String TenCot = dtChungTu.Columns[j].ColumnName;
                    String GiaTri = Convert.ToString(dtChungTu.Rows[i][j]);
                    if (TenCot == "iID_MaNamNganSach")
                    {
                        GiaTri = "1";//chuyển mã năm ngân sách thành 1 (Năm nay)
                    }
                    if (TenCot == "iNamLamViec")
                    {
                        GiaTri = Convert.ToString(iNamLamViec + 1);
                    }
                    bang.CmdParams.Parameters.AddWithValue("@" + TenCot, GiaTri);
                }
                iID_MaCapPhat = Guid.NewGuid().ToString();
                //Remove trường tự tăng iSoCapPhat
                bang.CmdParams.Parameters.RemoveAt(bang.CmdParams.Parameters.IndexOf("@iSoCapPhat"));
                bang.CmdParams.Parameters["@" + bang.TruongKhoa].Value = iID_MaCapPhat;
                bang.Save();

                for (int h = 0; h < dtChiTiet.Rows.Count; h++)
                {
                    for (int j = 1; j < dtChiTiet.Columns.Count; j++)
                    {
                        Type _type = dtChiTiet.Columns[j].DataType;
                        String TenCot = dtChiTiet.Columns[j].ColumnName;
                        Object GiaTri = dtChiTiet.Rows[h][j];

                        if (TenCot == "iID_MaNamNganSach")
                        {
                            GiaTri = "1";//chuyển mã năm ngân sách thành 1 (Năm nay)
                        }
                        if (TenCot == "iNamLamViec")
                        {
                            GiaTri = iNamLamViec + 1;
                        }
                        if (TenCot == "iID_MaCapPhat")
                        {
                            GiaTri = iID_MaCapPhat;
                        }
                        if (h == 0)
                        {
                            bangChiTiet.CmdParams.Parameters.AddWithValue("@" + TenCot, GiaTri);
                        }
                        else
                        {
                            bangChiTiet.CmdParams.Parameters["@" + TenCot].Value = GiaTri;
                        }

                    }
                    bangChiTiet.Save();
                }
            }
        }

        public static DataTable getDSPhongBan(String iNamLamViec, String MaND)
        {
            String sTenPB = NguoiDung_PhongBanModels.getTenPhongBan_NguoiDung(MaND);
            String DK = "";

            if (sTenPB == "02" || sTenPB == "2")
            {
            }
            else
            {
                DK = " AND iID_MaPhongBan=@iID_MaPhongBan";
            }

            String SQL = String.Format(@"SELECT DISTINCT iID_MaPhongBan,sTenPhongBan
FROM CP_CapPhatChiTiet
WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec {0} AND iID_MaPhongBan NOT IN (02)
", DK);
            SqlCommand cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
            cmd.Parameters.AddWithValue("@iID_MaPhongBan", sTenPB);
            DataTable dt = Connection.GetDataTable(cmd);
            DataRow dr = dt.NewRow();
            dr["iID_MaPhongBan"] = "-1";
            dr["sTenPhongBan"] = "--Chọn tất cả các B--";
            dt.Rows.InsertAt(dr, 0);
            return dt;
        }
        public static DataTable GetDanhSachLoaiNSCapPhat_ThongTri()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaLoai", typeof(String));
            dt.Columns.Add("sTen", typeof(String));
            DataRow dr = dt.NewRow();
            dr["MaLoai"] = "QuocPhong";
            dr["sTen"] = "kinh phí Quốc phòng";
            dt.Rows.InsertAt(dr, 0);

            dr = dt.NewRow();
            dr["MaLoai"] = "BaoDam";
            dr["sTen"] = "kinh phí bảo đảm";
            dt.Rows.InsertAt(dr, 1);


            dr = dt.NewRow();
            dr["MaLoai"] = "DoanhNghiep";
            dr["sTen"] = "kinh phí doanh nghiệp";
            dt.Rows.InsertAt(dr, 2);


            dr = dt.NewRow();
            dr["MaLoai"] = "NhaNuoc";
            dr["sTen"] = "kinh phí nhà nước";
            dt.Rows.InsertAt(dr, 3);

            dr = dt.NewRow();
            dr["MaLoai"] = "Khac";
            dr["sTen"] = "kinh phí khác";
            dt.Rows.InsertAt(dr, 4);


            dr = dt.NewRow();
            dr["MaLoai"] = "DacBiet";
            dr["sTen"] = "kinh phí đặc biệt";
            dt.Rows.InsertAt(dr, 5);

            dr = dt.NewRow();
            dr["MaLoai"] = "QPKhac";
            dr["sTen"] = "kinh phí quốc phòng khác";
            dt.Rows.InsertAt(dr, 6);

            dt.Dispose();
            return dt;
        }

        public static DataTable getDanhSachTuyChinh()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaLoai", typeof(String));
            dt.Columns.Add("sTen", typeof(String));
            DataRow dr = dt.NewRow();
            dr["MaLoai"] = "TatCa";
            dr["sTen"] = "Tất cả mục lục ngân sách";
            dt.Rows.InsertAt(dr, 0);

            dr = dt.NewRow();
            dr["MaLoai"] = "ChiTieu";
            dr["sTen"] = "Chỉ hiện mục có chỉ tiêu";
            dt.Rows.InsertAt(dr, 1);

            dr = dt.NewRow();
            dr["MaLoai"] = "ConChiTieu";
            dr["sTen"] = "Chỉ hiện mục vẫn còn chỉ tiêu";
            dt.Rows.InsertAt(dr, 2);

            dr = dt.NewRow();
            dr["MaLoai"] = "CapPhat";
            dr["sTen"] = "Hiện dữ liệu cấp phát đã nhập";
            dt.Rows.InsertAt(dr, 3);

            dr = dt.NewRow();
            dr["MaLoai"] = "QuaChiTieu";
            dr["sTen"] = "Hiện dữ liệu cấp phát quá chỉ tiêu";
            dt.Rows.InsertAt(dr, 4);
            dt.Dispose();
            return dt;
        }

        public static DataTable getDanhSachNganh(String MaND)
        {
            DataTable dtCauHinh = NguoiDungCauHinhModels.LayCauHinh(MaND);
            int iNamLamViec = Convert.ToInt16(dtCauHinh.Rows[0]["iNamLamViec"]);
            var dsDV = NguoiDung_DonViModels.DS_NguoiDung_DonVi(MaND).AsEnumerable().Select(x => x.Field<string>("iID_MaDonVi")).Join();

            String SQL = String.Format(@"select DISTInCT iID_MaNganh, sTenNganh from

(select DISTINCT sNG from f_ns_chitieu_full5(@iNamLamViec,@dsDV,@IdPhongBan,'2,4',null,GETDATE(),1,null)
where rHienVat <> 0) chuyenNganh

LEFT JOIN 

(SELECT DISTINCT iID_MaNganh, iID_MaNganhMLNS, 'Ngành - ' + sTenNganh AS sTenNganh FROM NS_MucLucNganSach_Nganh
WHERE iTrangThai = 1 AND iNamLamViec=@iNamLamViec and iID_MaNganh <> '00') nganhBD

ON nganhBD.iID_MaNganhMLNS like '%' + chuyenNganh.sNG + '%'");

            SqlCommand cmd = new SqlCommand(SQL);
            string iID_MaPhongBan = NguoiDung_PhongBanModels.getTenPhongBan_NguoiDung(MaND);            
            cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
            cmd.Parameters.AddWithValue("@IdPhongBan", iID_MaPhongBan);
            cmd.Parameters.AddWithValue("@dsDV", dsDV);
            DataTable dt = Connection.GetDataTable(cmd);
            return dt;
        }

        public static DataTable getDanhSachChiTieu()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaLoai", typeof(String));
            dt.Columns.Add("sTen", typeof(String));
            DataRow dr = dt.NewRow();
            dr["MaLoai"] = "-1";
            dr["sTen"] = "Dự toán đầu năm + bổ sung (năm nay + năm trước chuyển sang)";
            dt.Rows.InsertAt(dr, 0);

            dr = dt.NewRow();
            dr["MaLoai"] = "1";
            dr["sTen"] = "Dự toán đầu năm";
            dt.Rows.InsertAt(dr, 1);

            dr = dt.NewRow();
            dr["MaLoai"] = "2";
            dr["sTen"] = "Dự toán bổ sung";
            dt.Rows.InsertAt(dr, 2);

            dt.Dispose();
            return dt;
        }

        public static DataTable Get_dtTinhChatCapThu()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("iID_MaTinhChatCapThu", typeof(String));
            dt.Columns.Add("sTen", typeof(String));
            DataRow dr = dt.NewRow();
            dr["iID_MaTinhChatCapThu"] = "-1";
            dr["sTen"] = "--Chọn tính chất cấp thu--";
            dt.Rows.InsertAt(dr, 0);

            dr = dt.NewRow();
            dr["iID_MaTinhChatCapThu"] = "0";
            dr["sTen"] = "Cấp có chỉ tiêu";
            dt.Rows.InsertAt(dr, 1);

            dr = dt.NewRow();
            dr["iID_MaTinhChatCapThu"] = "1";
            dr["sTen"] = "Cấp không có chỉ tiêu";
            dt.Rows.InsertAt(dr, 2);

            dr = dt.NewRow();
            dr["iID_MaTinhChatCapThu"] = "2";
            dr["sTen"] = "Cấp thu";
            dt.Rows.InsertAt(dr, 2);

            dr = dt.NewRow();
            dr["iID_MaTinhChatCapThu"] = "3";
            dr["sTen"] = "Giảm cấp";
            dt.Rows.InsertAt(dr, 2);

            dt.Dispose();
            return dt;
        }

        /*
         * <summary>
         * lấy index của chuỗi loại ngân sách: LNS, L, K, M, TM, NG 
         * index cha lớn hơn index con
         * </summary>
         * */
        public static int getIndex(string val)
        {
            String[] arrDSTruong = MucLucNganSachModels.arrDSTruong;
            int index = -1;
            for (int i = 0; i < arrDSTruong.Length; i++)
                if (val == arrDSTruong[i])
                {
                    index = i;
                    break;
                }
            return index;
        }
    }
}
