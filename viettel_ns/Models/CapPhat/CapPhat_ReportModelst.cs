using DomainModel;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Viettel.Data;
using Viettel.Services;
using VIETTEL.Helpers;

namespace VIETTEL.Models.CapPhat
{
    public class CapPhat_ReportModelst
    {
        public static int iID_MaPhanHe = PhanHeModels.iID_MaPhanHeCapPhat;

        /// <summary>
        /// Lấy ghi chú của đơn vị
        /// </summary>
        /// <param name="MaND">Mã người dùng</param>
        /// <param name="iID_MaDonVi">Mã đơn vị</param>
        /// <returns></returns>
        /// HiepPM: 2018/05/23
        public static string LayGhiChu(String MaND, String iID_MaDonVi, String iID_MaCapPhat)
        {
            #region definition input

            var sql = FileHelpers.GetSqlQuery("rptCapPhat_LayGhiChu.sql");

            #endregion

            #region dieu kien            

            #endregion

            #region get data

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@sTen", iID_MaCapPhat);
                cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
                cmd.Parameters.AddWithValue("@sID_MaNguoiDung", MaND);

                return cmd.GetValue();
            }
            #endregion              

        }

        /// <summary>
        /// Lấy danh sách LNS theo loại thông tri
        /// </summary>
        /// <param name="iID_MaCapPhat">Mã chứng từ cấp phát</param>
        /// <param name="LoaiThongTri">Loại thông tri</param>
        /// VungNV: 2015/11/11
        public static DataTable LayDtThongTri_LNS(string iID_MaCapPhat, string LoaiThongTri = null)
        {
            var chungTu = CapPhat_ChungTuModels.LayThongTin(iID_MaCapPhat);
            DataTable dt = new DataTable();

            #region definition input chitieu

            var sql = FileHelpers.GetSqlQuery("rptCapPhat_ThongTri_LNS.sql");

            #endregion

            #region dieu kien chitieu  

            string sLNS = "";
            if (LoaiThongTri == "0")
                sLNS = "101,102";
            else if (LoaiThongTri == "1")
                sLNS = "104";
            else if (LoaiThongTri == "2")
                sLNS = "105";
            else if (LoaiThongTri == "3")
                sLNS = "2";
            else if (LoaiThongTri == "4")
                sLNS = "4";
            else if (LoaiThongTri == "5")
                sLNS = "3";
            else
                sLNS = "109";

            if (String.IsNullOrEmpty(sLNS))
                sLNS = "-100";

            sql = sql.Replace("@@sLNS", sLNS.ToParamLikeStartWith("sLNS"));

            #endregion

            #region get data chitieu

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@iID_MaCapPhat", chungTu["iID_MaCapPhat"]);
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", chungTu["iID_MaPhongBan"]);
                cmd.Parameters.AddWithValue("@iNamLamViec", chungTu["iNamLamViec"]);

                return cmd.GetTable();
            }
            #endregion   
        }
        public static DataTable LayDtThongTriLNS_DonVi(String iID_MaCapPhat, String LoaiThongTri = null, String sLNS = null)
        {
            var chungTu = CapPhat_ChungTuModels.LayThongTin(iID_MaCapPhat);

            #region definition input chitieu

            var sql = FileHelpers.GetSqlQuery("rptCapPhat_ThongTri_DonVi.sql");

            #endregion

            #region dieu kien chitieu  

            var dkDonVi = new NganSachService().GetDonviListByUser(System.Web.HttpContext.Current.User.Identity.Name, chungTu["iNamLamViec"])
                        .ToDictionary(x => x.iID_MaDonVi, x => x.iID_MaDonVi + " - " + x.sTen).Select(x => x.Key).Join();
            #endregion

            #region get data chitieu

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@iID_MaCapPhat", iID_MaCapPhat);
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", chungTu["iID_MaPhongBan"]);
                cmd.Parameters.AddWithValue("@iNamLamViec", chungTu["iNamLamViec"]);
                cmd.Parameters.AddWithValue("@sLNS", sLNS);
                cmd.Parameters.AddWithValue("@iID_MaDonVi", dkDonVi);

                return cmd.GetTable();
            }
            #endregion
        }
        public static DataTable LayDtLoaiThongTri_NG(string iID_MaCapPhat, string MaND, string iID_MaPhongBan)
        {
            #region definition input

            var sql = FileHelpers.GetSqlQuery("rptCapPhat_ThongTriSec_Nganh.sql");
            string MaPhongBan = NguoiDung_PhongBanModels.getTenPhongBan_NguoiDung(MaND);

            #endregion

            #region dieu kien            

            #endregion

            #region get data

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", MaPhongBan);
                cmd.Parameters.AddWithValue("@iID_MaCapPhat", iID_MaCapPhat);
                cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
                if (MaPhongBan == "07")
                {
                    cmd.Parameters.AddWithValue("@MaND", "%b7%");
                }
                else
                {
                    cmd.Parameters.AddWithValue("@MaND", "%" + MaND + "%");
                }

                return cmd.GetTable();
            }
            #endregion             
        }
        public static DataTable LayDtNG_DonVi(string iID_MaCapPhat, string LoaiThongTri, string MaND, string NG, string iID_MaPhongBan)
        {
            #region definition input

            var sql = FileHelpers.GetSqlQuery("rptCapPhat_ThongTriSec_Nganh_DonVi.sql");
            string MaPhongBan = NguoiDung_PhongBanModels.getTenPhongBan_NguoiDung(MaND);

            #endregion

            #region dieu kien     

            if (String.IsNullOrEmpty(NG))
                NG = "-100";

            #endregion

            #region get data

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", MaPhongBan);
                cmd.Parameters.AddWithValue("@iID_MaCapPhat", iID_MaCapPhat);
                cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
                cmd.Parameters.AddWithValue("@Nganh", NG);
                return cmd.GetTable();

            }
            #endregion

        }
        public static DataTable getLoaiNS()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaLoai", typeof(string));
            dt.Columns.Add("sTen", typeof(string));
            DataRow dr = dt.NewRow();
            dr["MaLoai"] = "1";
            dr["sTen"] = "Ngân sách QP";
            dt.Rows.InsertAt(dr, 0);

            dr = dt.NewRow();
            dr["MaLoai"] = "2";
            dr["sTen"] = "Ngân sách nhà nước";
            dt.Rows.InsertAt(dr, 1);
            return dt;
        }
        public static DataTable GetDanhSachLoaiCapPhat_ThongTri()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaLoai", typeof(String));
            dt.Columns.Add("sTen", typeof(String));
            DataRow dr = dt.NewRow();
            dr["MaLoai"] = "0";
            dr["sTen"] = "Kinh phí Quốc phòng";
            dt.Rows.InsertAt(dr, 0);

            dr = dt.NewRow();
            dr["MaLoai"] = "1";
            dr["sTen"] = "Kinh phí Bảo đảm";
            dt.Rows.InsertAt(dr, 1);


            dr = dt.NewRow();
            dr["MaLoai"] = "2";
            dr["sTen"] = "Kinh phí Doanh nghiệp";
            dt.Rows.InsertAt(dr, 2);


            dr = dt.NewRow();
            dr["MaLoai"] = "3";
            dr["sTen"] = "Kinh phí Nhà nước";
            dt.Rows.InsertAt(dr, 3);

            dr = dt.NewRow();
            dr["MaLoai"] = "4";
            dr["sTen"] = "Kinh phí Khác";
            dt.Rows.InsertAt(dr, 4);


            dr = dt.NewRow();
            dr["MaLoai"] = "5";
            dr["sTen"] = "Kinh phí Đặc biệt";
            dt.Rows.InsertAt(dr, 5);

            dr = dt.NewRow();
            dr["MaLoai"] = "6";
            dr["sTen"] = "Kinh phí Quốc phòng khác";
            dt.Rows.InsertAt(dr, 6);

            dt.Dispose();
            return dt;
        }

        /// <summary>
        /// Lấy dữ liệu danh sách chứng từ thông tri cấp phát
        /// </summary>
        /// <param name="MaND">Mã người dùng</param>
        /// <param name="sLNS">Mã loại ngân sách</param>
        /// <param name="iNamCapPhat">Năm cấp phát</param>
        /// <param name="iID_MaDonVi">Mã đơn vị</param>
        /// <param name="LoaiTongHop">Loại báo cáo: chi tiết hay tổng hợp</param>
        /// <returns></returns>
        /// VungNV: 2015/11/11
        public static DataTable rptCapPhat_ThongTri(String MaND, String sLNS, String iDotCapPhat, String iID_MaDonVi, String LoaiTongHop)
        {
            String DKDonVi = "";
            String DKPhongBan = "";
            String DK = "";
            SqlCommand cmd = new SqlCommand();
            String SQL = "";

            DKDonVi = ThuNopModels.DKDonVi(MaND, cmd);
            DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);

            //Báo cáo chi tiết từng đơn vị
            if (LoaiTongHop == "ChiTiet")
            {
                if (!String.IsNullOrEmpty(iID_MaDonVi) && iID_MaDonVi != "-1")
                {
                    DK += " AND iID_MaDonVi=@iID_MaDonVi";
                    cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
                }
            }
            //Báo cáo tổng hợp các đơn vị
            else
            {
                if (String.IsNullOrEmpty(iID_MaDonVi))
                    iID_MaDonVi = Guid.Empty.ToString();

                String[] arrDonVi = iID_MaDonVi.Split(',');

                for (int i = 0; i < arrDonVi.Length; i++)
                {
                    DK += "iID_MaDonVi=@MaDonVi" + i;
                    cmd.Parameters.AddWithValue("@MaDonVi" + i, arrDonVi[i]);
                    if (i < arrDonVi.Length - 1)
                        DK += " OR ";
                }

                if (!String.IsNullOrEmpty(DK))
                    DK = " AND (" + DK + ")";
            }

            if (!String.IsNullOrEmpty(sLNS))
            {
                DK += " AND sLNS IN (" + sLNS + ")";
            }

            //Nếu là báo cáo tổng hợp thì lấy thêm mã đơn vị và tên đơn vị
            string strDonVi = "";

            //Báo cáo tổng hợp các đơn vị
            if (LoaiTongHop == "TongHop")
            {
                strDonVi = " ,iID_MaDonVi, sTenDonVi";
            }

            SQL = String.Format(@"
                    SELECT SUBSTRING(sLNS,1,1) as sLNS1,
                            SUBSTRING(sLNS,1,3) as sLNS3,
                            SUBSTRING(sLNS,1,5) as sLNS5,
                            sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa {3}
                            ,SUM(rTuChi) as rTuChi
                     FROM CP_CapPhatChiTiet
                     WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec {0} {1} {2}
                            AND iID_MaCapPhat = @iID_MaCapPhat 
                     GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa {3}
                     HAVING SUM(rTuChi)<>0 ", DK, DKDonVi, DKPhongBan, strDonVi);

            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            cmd.Parameters.AddWithValue("@iID_MaCapPhat", iDotCapPhat);
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();

            return dt;
        }
        public static DataTable rptCapPhat_ThongTriSec(string MaND, string NG, string iDotCapPhat, string iID_MaDonVi, string LoaiTongHop)
        {

            #region definition input

            var sql = FileHelpers.GetSqlQuery("rptCapPhat_ThongTriSec.sql");
            string iID_MaPhongBan = NguoiDung_PhongBanModels.getTenPhongBan_NguoiDung(MaND);

            #endregion

            #region dieu kien

            if (LoaiTongHop == "TongHop")
            {
                sql = sql.Replace("@@strSelect", @"SUM(rTuChi) AS rTuChi,
                                                     , iID_MaDonVi
                                                     , sTenDonVi");
                sql = sql.Replace("@@strDonVi", @"iID_MaDonVi, sTenDonVi, sNG");
            }
            else
            {
                sql = sql.Replace("@@strSelect", @"SUM(rTuChi) AS rTuChi");
                sql = sql.Replace("@@strDonVi", @"sNG");
            }

            #endregion

            #region get data

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan);
                cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
                cmd.Parameters.AddWithValue("@iID_MaCapPhat", iDotCapPhat);
                cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
                cmd.Parameters.AddWithValue("@Nganh", NG);

                return cmd.GetTable();
            }
            #endregion
        }
        public static DataTable rptCapPhat_THChiTieu_Sec(string MaND, string sLNS)
        {
            String sql, DK;
            SqlCommand cmd, cmd1 = new SqlCommand();
            DataTable vR;
            String iNamLamViec = ReportModels.LayNamLamViec(System.Web.HttpContext.Current.User.Identity.Name);
            String iID_MaPhongBan = NguoiDung_PhongBanModels.getTenPhongBan_NguoiDung(MaND);
            String sTruongTien = "rTuChi,iID_MaCapPhatChiTiet,iID_MaDonVi,sTenDonVi,sMaCongTrinh,rTuChi_PhanBo,rTuChi_DaCap";
            String[] arrDSTruong = (MucLucNganSachModels.strDSTruong + ",iID_MaDonVi").Split(',');
            String[] arrDSTruongTien = sTruongTien.Split(',');

            //<--Lay toan bo Muc luc ngan sach
            cmd = new SqlCommand();
            DK = "iTrangThai=1";

            String DKLoai = "";
            if (!string.IsNullOrEmpty(sLNS))
            {
                DataTable dtNGChiTieu = new DataTable();
                #region definition input

                sql = FileHelpers.GetSqlQuery("dtCapPhat_HienVat_Nganh.sql");

                #endregion

                #region dieu kien

                string iID_MaDonVi = NganSach_HamChungModels.DSDonViCuaNguoiDung(MaND)
                            .AsEnumerable()
                            .Select(x => x.Field<string>("iID_MaDonVi"))
                            .Join();

                #endregion

                #region get data

                using (var conn = ConnectionFactory.Default.GetConnection())
                using (cmd1 = new SqlCommand(sql, conn))
                {
                    cmd1.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                    cmd1.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan);
                    cmd1.Parameters.AddWithValue("@Nganh", DBNull.Value);
                    cmd1.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
                    cmd1.Parameters.AddWithValue("@iID_MaNguonNganSach", DBNull.Value);
                    cmd1.Parameters.AddWithValue("@iID_MaNamNganSach", DBNull.Value);

                    dtNGChiTieu = cmd1.GetTable();
                }
                #endregion

                String DKNGChiTieu = "";

                if (dtNGChiTieu.Rows.Count > 0)
                {
                    for (int i = 0; i < dtNGChiTieu.Rows.Count; i++)
                    {
                        DKNGChiTieu += "sNG=@sNGCT" + i;
                        if (i < dtNGChiTieu.Rows.Count - 1)
                        {
                            DKNGChiTieu += " OR ";
                        }
                        cmd.Parameters.AddWithValue("@sNGCT" + i, dtNGChiTieu.Rows[i]["sNG"]);
                    }
                    DKLoai = " AND sLNS = '' AND (" + DKNGChiTieu + ")";
                }
            }
            else
                DKLoai += String.Format("and 1=1");

            sql = String.Format("SELECT iID_MaMucLucNganSach,iID_MaMucLucNganSach_Cha,bLaHangCha,sXauNoiMa,{0},{1} FROM NS_MucLucNganSach WHERE iNamLamViec=@iNamLamViec AND  {2} {4} ORDER BY {3}", MucLucNganSachModels.strDSTruong, MucLucNganSachModels.strDSDuocNhapTruongTien, DK, MucLucNganSachModels.strDSTruongSapXep, DKLoai);
            cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
            cmd.CommandText = sql;
            vR = Connection.GetDataTable(cmd);
            cmd.Dispose();
            //Lay toan bo Muc luc ngan sach-->
            DataColumn column;

            for (int j = 0; j < arrDSTruongTien.Length; j++)
            {

                if (arrDSTruongTien[j] == "rTuChi_PhanBo" || arrDSTruongTien[j] == "rTuChi_DaCap" || arrDSTruongTien[j] == "rTuChi")
                {
                    column = new DataColumn(arrDSTruongTien[j], typeof(decimal));
                    column.DefaultValue = 0;
                }
                else
                {
                    column = new DataColumn(arrDSTruongTien[j], typeof(String));
                }
                column.AllowDBNull = true;
                vR.Columns.Add(column);
            }


            //HungPX: Lấy thông tin cột tiền đã cấp
            //DataRow dr = dtChungTu.Rows[0];

            //Dien don vi
            DataTable arrDV = NganSach_HamChungModels.DSDonViCuaNguoiDung(MaND);

            int count1 = vR.Rows.Count;
            for (int j = 0; j < count1; j++)
            {
                DataRow dr = vR.Rows[j];
                if (dr["sNG"] != "")
                {
                    for (int i = 0; i < arrDV.Rows.Count; i++)
                    {
                        if (i == -1)
                        {
                            dr["iID_MaDonVi"] = arrDV.Rows[i]["iID_MaDonVi"];
                            dr["sTenDonVi"] = arrDV.Rows[i]["sTen"];
                        }
                        else
                        {
                            DataRow temp = vR.NewRow();
                            for (int c = 0; c < vR.Columns.Count; c++)
                            {
                                temp[c] = dr[c];
                            }
                            temp["iID_MaDonVi"] = arrDV.Rows[i]["iID_MaDonVi"];
                            temp["sTenDonVi"] = arrDV.Rows[i]["sTen"];
                            vR.Rows.Add(temp);
                        }

                    }
                }
            }
            DataView dv = vR.DefaultView;
            dv.Sort = "sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi";
            vR = dv.ToTable();

            DataTable dtChungTuChiTiet = new DataTable();

            #region definition input

            sql = FileHelpers.GetSqlQuery("dtCapPhat_ChungTu_DaCap.sql");

            #endregion

            #region dieu kien            

            #endregion

            #region get data

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                cmd.Parameters.AddWithValue("@iID_MaCapPhat", DBNull.Value);
                dtChungTuChiTiet = cmd.GetTable();
            }
            #endregion

            int cs0 = 0;

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
                                vR.Rows[i][k] = dtChungTuChiTiet.Rows[j][vR.Columns[k].ColumnName];
                            }
                            count++;
                        }
                        else
                        {
                            DataRow row = vR.NewRow();
                            for (int k = 0; k < vR.Columns.Count - 2; k++)
                            {
                                row[k] = dtChungTuChiTiet.Rows[j][vR.Columns[k].ColumnName];
                            }
                            vR.Rows.InsertAt(row, i + 1);
                            i++;
                            vRCount++;
                        }
                    }
                }

            }
            for (int i = vR.Rows.Count - 1; i >= 0; i--)
            {
                String iID_MaDonVi = Convert.ToString(vR.Rows[i]["iID_MaDonVi"]);
                if (iID_MaDonVi == "")
                {
                    vR.Rows.RemoveAt(i);
                }
            }
            LayChiTieuDMNB(vR);
            LayCapPhatDaCapDMNB(vR);

            dtChungTuChiTiet.Dispose();
            cmd.Dispose();
            return vR;
        }
        private static void LayChiTieuDMNB(DataTable vR)
        {

            String MaND = System.Web.HttpContext.Current.User.Identity.Name;
            DataTable dtCauHinh = NguoiDungCauHinhModels.LayCauHinh(MaND);
            String iID_MaPhongBan = NguoiDung_PhongBanModels.getTenPhongBan_NguoiDung(MaND);
            DataTable dtChiTieu = new DataTable();

            #region definition input chitieu

            var sql = FileHelpers.GetSqlQuery("dtCapPhat_HienVat.sql");

            #endregion

            #region dieu kien chitieu  

            #endregion

            #region get data chitieu

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan);
                cmd.Parameters.AddWithValue("@iNamLamViec", dtCauHinh.Rows[0]["iNamLamViec"]);
                cmd.Parameters.AddWithValue("@iID_MaNguonNganSach", dtCauHinh.Rows[0]["iID_MaNguonNganSach"]);
                cmd.Parameters.AddWithValue("@iID_MaNamNganSach", dtCauHinh.Rows[0]["iID_MaNamNganSach"]);

                dtChiTieu = cmd.GetTable();
            }
            #endregion
            int count = vR.Rows.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                String sNG = Convert.ToString(vR.Rows[i]["sNG"]);
                String iID_MaDonVi = Convert.ToString(vR.Rows[i]["iID_MaDonVi"]);

                DataRow[] dr, dr1;

                dr = dtChiTieu.Select("sNG='" + sNG + "' AND iID_MaDonVi='" + iID_MaDonVi + "'");
                dr1 = dtChiTieu.Select("sNG='" + sNG + "'");

                if (dr.Length > 0)
                {
                    //gan vao bang chung tu chi tiet VR
                    DataRow row1 = vR.Rows[i];

                    row1["rTuChi_PhanBo"] = Convert.ToDouble(dr[0]["rHienVat"]);
                }
                else if (dr1.Length == 0)
                {
                    if (Convert.ToString(vR.Rows[i]["sNG"]) != "")
                    {
                        vR.Rows.RemoveAt(i);
                    }
                }
                else
                {
                    if (Convert.ToString(vR.Rows[i]["sNG"]) != "" && Convert.ToString(vR.Rows[i]["iID_MaDonVi"]) != "")
                    {
                        vR.Rows.RemoveAt(i);
                    }
                }
            }
        }
        private static void LayCapPhatDaCapDMNB(DataTable vR)
        {
            String MaND = System.Web.HttpContext.Current.User.Identity.Name;
            DataTable dtCauHinh = NguoiDungCauHinhModels.LayCauHinh(MaND);
            String iID_MaPhongBan = NguoiDung_PhongBanModels.getTenPhongBan_NguoiDung(MaND);

            DataTable dtCapPhat = new DataTable();

            #region definition input dacap

            var sql = FileHelpers.GetSqlQuery("dtCapPhat_DaCap.sql");

            #endregion

            #region dieu kien dacap 

            sql = sql.Replace("@@Tien_HienVat", "sTTM = '' AND 1");
            sql = sql.Replace("@@select", "sNG");
            sql = sql.Replace("@@MucCap", "sNG");

            #endregion

            #region get data dacap

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan);
                cmd.Parameters.AddWithValue("@iNamLamViec", dtCauHinh.Rows[0]["iNamLamViec"]);
                cmd.Parameters.AddWithValue("@iID_MaNguonNganSach", dtCauHinh.Rows[0]["iID_MaNguonNganSach"]);
                cmd.Parameters.AddWithValue("@iID_MaNamNganSach", dtCauHinh.Rows[0]["iID_MaNamNganSach"]);
                cmd.Parameters.AddWithValue("@dNgayCapPhat", DateTime.Now);
                cmd.Parameters.AddWithValue("@iSoCapPhat", DBNull.Value);

                dtCapPhat = cmd.GetTable();
            }
            #endregion

            for (int i = 0; i < vR.Rows.Count; i++)
            {
                String sNG = Convert.ToString(vR.Rows[i]["sNG"]);
                String iID_MaDonVi = Convert.ToString(vR.Rows[i]["iID_MaDonVi"]);
                DataRow[] dr;
                dr = dtCapPhat.Select("sNG='" + sNG + "' AND iID_MaDonVi='" + iID_MaDonVi + "'");

                if (dr.Length > 0)
                {
                    //gan vao bang chung tu chi tiet VR
                    DataRow row1 = vR.Rows[i];
                    row1["rTuChi_DaCap"] = dr[0]["rTuChi"];
                }
            }
        }
        public static DataTable getDanhSachNganh(string MaND)
        {
            #region definition input

            var sql = FileHelpers.GetSqlQuery("dtCapPhat_HienVat_Nganh.sql");

            #endregion

            #region dieu kien

            string iNamLamViec = ReportModels.LayNamLamViec(System.Web.HttpContext.Current.User.Identity.Name);
            string iID_MaPhongBan = NguoiDung_PhongBanModels.getTenPhongBan_NguoiDung(MaND);

            #endregion

            #region get data

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan);
                cmd.Parameters.AddWithValue("@Nganh", DBNull.Value);
                cmd.Parameters.AddWithValue("@iID_MaDonVi", DBNull.Value);
                cmd.Parameters.AddWithValue("@iID_MaNguonNganSach", DBNull.Value);
                cmd.Parameters.AddWithValue("@iID_MaNamNganSach", DBNull.Value);

                var dt = cmd.GetTable();
                return dt;
            }
            #endregion
        }
        public static DataTable rptCapPhat_THChiTieu(String MaND, String loaiNS)
        {

            //DataTable vR;
            //String iNamLamViec = ReportModels.LayNamLamViec(System.Web.HttpContext.Current.User.Identity.Name);
            //String iID_MaPhongBan = NguoiDung_PhongBanModels.getTenPhongBan_NguoiDung(MaND);
            //DataTable dt = NganSach_HamChungModels.DSLNSCuaPhongBan(MaND);
            //String sTruongTien = "rTuChi,iID_MaCapPhatChiTiet,iID_MaDonVi,sTenDonVi,sMaCongTrinh,rTuChi_PhanBo,rTuChi_DaCap";
            //String[] arrDSTruong = (MucLucNganSachModels.strDSTruong + ",iID_MaDonVi").Split(',');
            //String[] arrDSTruongTien = sTruongTien.Split(',');

            //String SQL, DK;
            //SqlCommand cmd;

            ////<--Lay toan bo Muc luc ngan sach
            //cmd = new SqlCommand();
            //DK = "iTrangThai=1";

            //if (arrGiaTriTimKiem != null) {
            //    for (int i = 0; i < arrDSTruong.Length - 1; i++) {
            //        if (String.IsNullOrEmpty(arrGiaTriTimKiem[arrDSTruong[i]]) == false) {
            //            DK += String.Format(" AND {0} LIKE @{0}", arrDSTruong[i]);
            //            cmd.Parameters.AddWithValue("@" + arrDSTruong[i], arrGiaTriTimKiem[arrDSTruong[i]] + "%");
            //        }
            //    }
            //}
            //if (dt.Rows.Count > 0)
            //    DK += " AND( ";
            //for (int i = 0; i < dt.Rows.Count; i++) {
            //    String Nguon = Convert.ToString(dt.Rows[i]["sLNS"]).Substring(0, 1);
            //    String LoaiNS = Convert.ToString(dt.Rows[i]["sLNS"]).Substring(0, 3);
            //    DK += " (sLNS=@sLNS" + i + " OR (SUBSTRING(sLNS,1,1)=" + Nguon + " AND LEN(sLNS)=1) OR  (SUBSTRING(sLNS,1,3)=" + LoaiNS + " AND LEN(sLNS)=3))";
            //    if (i < dt.Rows.Count - 1)
            //        DK += " OR ";
            //    cmd.Parameters.AddWithValue("@sLNS" + i, dt.Rows[i]["sLNS"]);
            //}
            //if (dt.Rows.Count > 0)
            //    DK += " ) ";

            //String DKLoai = "";
            ////ngan sach quoc phong tung loai ns 


            //if (!string.IsNullOrEmpty(sDSLNS)) {
            //    String[] arrLNS = sDSLNS.Split(',');
            //    for (int i = 0; i < arrLNS.Length; i++) {
            //        DKLoai += "sLNS=@sDSLNS" + i;
            //        if (i < arrLNS.Length - 1) {
            //            DKLoai += " OR ";
            //        }
            //        cmd.Parameters.AddWithValue("@sDSLNS" + i, arrLNS[i]);
            //    }
            //    DKLoai = String.Format("and ({0})", DKLoai);
            //    //lay danh sach LNS co chi tieu
            //    SQL = String.Format(@"SELECT DISTINCT sLNS
            //FROM DT_ChungTuChiTiet
            //WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec
            //AND iID_MaPhongBanDich=@iID_MaPhongBan
            //{0}

            //UNION 
            //SELECT DISTINCT sLNS
            //FROM DT_ChungTuChiTiet_PhanCap
            //WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec
            //AND iID_MaPhongBanDich=@iID_MaPhongBan
            //{0}

            //UNION 
            //SELECT DISTINCT sLNS
            //FROM DTBS_ChungTuChiTiet 
            //WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec
            //AND iID_MaPhongBanDich=@iID_MaPhongBan
            //AND iID_MaChungTu IN (SELECT maChungTu FROM F_NS_DSCtuBS_TheoNgay(@iNamLamViec,@iID_MaPhongBan,@cNgay))
            //{0}

            //UNION 
            //SELECT DISTINCT pc.sLNS
            //FROM DTBS_ChungTuChiTiet_PhanCap as pc INNER JOIN (SELECT iID_MaChungTuChiTiet, iID_MaChungTu FROM DTBS_ChungTuChiTiet) as ctct ON ctct.iID_MaChungTuChiTiet = pc.iID_MaChungTu 
            //WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec
            //AND iID_MaPhongBanDich=@iID_MaPhongBan
            //AND (ctct.iID_MaChungTu IN (SELECT maChungTu FROM F_NS_DSCtuBS_TheoNgay(@iNamLamViec,@iID_MaPhongBan,@cNgay))
            //     OR ctct.iID_MaChungTu in (   select iID_MaChungTuChiTiet 
            //                                                                                            from DTBS_ChungTuChiTiet 
            //                                                                                            where iID_MaChungTu in (
            //                                                                                                        select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
            //                                                                                                        where iID_MaChungTu in (   
            //                                                                                                                                select iID_MaChungTu from DTBS_ChungTu
            //                                                                                                                                where iID_MaChungTu in (
            //                                                                                                                                                        select iID_MaDuyetDuToanCuoiCung from DTBS_ChungTu
            //                                                                                                                                                        where iID_MaChungTu in (SELECT maChungTu FROM F_NS_DSCtuBS_TheoNgay(@iNamLamViec,@iID_MaPhongBan,@cNgay)))))) )
            //{0}
            //", DKLoai);
            //    SqlCommand cmd1 = new SqlCommand(SQL);
            //    cmd1.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
            //    cmd1.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan);
            //    cmd1.Parameters.AddWithValue("@cNgay", ngayCT);

            //    for (int i = 0; i < arrLNS.Length; i++) {
            //        cmd1.Parameters.AddWithValue("@sDSLNS" + i, arrLNS[i]);
            //    }
            //    DataTable dtLNSChiTieu = Connection.GetDataTable(cmd1);
            //    //if (arrLNS.Contains("3010000"))
            //    //{
            //    //    DataRow dr = dtLNSChiTieu.NewRow();
            //    //    dr["sLNS"] = "3010000";
            //    //    dtLNSChiTieu.Rows.Add(dr);
            //    //}
            //    String DKLNSChiTieu = "";
            //    if (dtLNSChiTieu.Rows.Count > 0) {
            //        for (int i = 0; i < dtLNSChiTieu.Rows.Count; i++) {
            //            DKLNSChiTieu += "sLNS=@sDSLNSCT" + i;
            //            if (i < dtLNSChiTieu.Rows.Count - 1) {
            //                DKLNSChiTieu += " OR ";
            //            }
            //            cmd.Parameters.AddWithValue("@sDSLNSCT" + i, dtLNSChiTieu.Rows[i]["sLNS"]);
            //        }
            //        DKLoai = DKLoai + " AND (" + DKLNSChiTieu + ")";
            //    }
            //} else
            //    DKLoai += String.Format("and sLNS like '---%'");


            //SQL = String.Format("SELECT iID_MaMucLucNganSach,iID_MaMucLucNganSach_Cha,bLaHangCha,sXauNoiMa,{0},{1} FROM NS_MucLucNganSach WHERE iNamLamViec=2018 AND {2} {4} {5} ORDER BY {3}", MucLucNganSachModels.strDSTruong, MucLucNganSachModels.strDSDuocNhapTruongTien, DK, MucLucNganSachModels.strDSTruongSapXep, DKLoai, DKsLoai);
            //cmd.CommandText = SQL;
            //vR = Connection.GetDataTable(cmd);
            //cmd.Dispose();
            ////Lay toan bo Muc luc ngan sach-->
            //DataColumn column;

            //for (int j = 0; j < arrDSTruongTien.Length; j++) {
            //    column = new DataColumn(arrDSTruongTien[j], typeof(String));
            //    if (arrDSTruongTien[j] == "rTuChi_PhanBo" || arrDSTruongTien[j] == "rTuChi_DaCap" || arrDSTruongTien[j] == "rTuChi") {
            //        column.DefaultValue = 0;
            //    }
            //    column.AllowDBNull = true;
            //    vR.Columns.Add(column);
            //}


            ////HungPX: Lấy thông tin cột tiền đã cấp
            ////DataRow dr = dtChungTu.Rows[0];

            ////Dien don vi
            //DataTable arrDV = NganSach_HamChungModels.DSDonViCuaNguoiDung(MaND);
            //String iID_MaDonVi_TK = arrGiaTriTimKiem["iID_MaDonVi"];
            //if (iID_MaDonVi_TK != null && iID_MaDonVi_TK != "") {
            //    for (int i = arrDV.Rows.Count - 1; i >= 0; i--) {
            //        if (Convert.ToString(arrDV.Rows[i]["iID_MaDonVi"]) != iID_MaDonVi_TK)
            //            arrDV.Rows.RemoveAt(i);
            //    }
            //}
            //int count1 = vR.Rows.Count;
            //for (int j = 0; j < count1; j++) {
            //    DataRow dr = vR.Rows[j];
            //    if (sMucCap == "sM" && dr["sM"] != "") {
            //        for (int i = 0; i < arrDV.Rows.Count; i++) {
            //            if (i == -1) {
            //                dr["iID_MaDonVi"] = arrDV.Rows[i]["iID_MaDonVi"];
            //                dr["sTenDonVi"] = arrDV.Rows[i]["sTen"];
            //            } else {
            //                DataRow temp = vR.NewRow();
            //                for (int c = 0; c < vR.Columns.Count; c++) {
            //                    temp[c] = dr[c];
            //                }
            //                temp["iID_MaDonVi"] = arrDV.Rows[i]["iID_MaDonVi"];
            //                temp["sTenDonVi"] = arrDV.Rows[i]["sTen"];
            //                vR.Rows.Add(temp);
            //            }

            //        }
            //    }
            //    if (sMucCap == "sTM" && dr["sTM"] != "") {
            //        for (int i = 0; i < arrDV.Rows.Count; i++) {
            //            if (i == -1) {
            //                dr["iID_MaDonVi"] = arrDV.Rows[i]["iID_MaDonVi"];
            //                dr["sTenDonVi"] = arrDV.Rows[i]["sTen"];
            //            } else {
            //                DataRow temp = vR.NewRow();
            //                for (int c = 0; c < vR.Columns.Count; c++) {
            //                    temp[c] = dr[c];
            //                }
            //                temp["iID_MaDonVi"] = arrDV.Rows[i]["iID_MaDonVi"];
            //                temp["sTenDonVi"] = arrDV.Rows[i]["sTen"];
            //                vR.Rows.Add(temp);
            //            }

            //        }
            //    }
            //    if (sMucCap == "sNG" && dr["sNG"] != "") {
            //        for (int i = 0; i < arrDV.Rows.Count; i++) {
            //            if (i == -1) {
            //                dr["iID_MaDonVi"] = arrDV.Rows[i]["iID_MaDonVi"];
            //                dr["sTenDonVi"] = arrDV.Rows[i]["sTen"];
            //            } else {
            //                DataRow temp = vR.NewRow();
            //                for (int c = 0; c < vR.Columns.Count; c++) {
            //                    temp[c] = dr[c];
            //                }
            //                temp["iID_MaDonVi"] = arrDV.Rows[i]["iID_MaDonVi"];
            //                temp["sTenDonVi"] = arrDV.Rows[i]["sTen"];
            //                vR.Rows.Add(temp);
            //            }

            //        }
            //    }
            //}
            //DataView dv = vR.DefaultView;
            //if (sMucCap == "sM") {
            //    dv.Sort = "sLNS,sL,sK,sM,iID_MaDonVi";
            //} else {
            //    dv.Sort = "sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi";
            //}
            //vR = dv.ToTable();

            //cmd = new SqlCommand();
            ////Lay Du Lieu Trong Bang QTA_ChungTuChiTiet
            //DK = "iTrangThai=1 AND iID_MaCapPhat=@iID_MaCapPhat AND rTuChi<>0";
            //cmd.Parameters.AddWithValue("@iID_MaCapPhat", iID_MaCapPhat);
            //SQL = String.Format("SELECT * FROM CP_CapPhatChiTiet WHERE {0} ORDER BY sXauNoiMa", DK);
            //cmd.CommandText = SQL;

            //DataTable dtChungTuChiTiet = Connection.GetDataTable(cmd);
            //int cs0 = 0;

            //int vRCount = vR.Rows.Count;
            //for (int i = 0; i < vRCount; i++) {
            //    int count = 0;
            //    for (int j = cs0; j < dtChungTuChiTiet.Rows.Count; j++) {

            //        Boolean ok = true;
            //        for (int k = 0; k < arrDSTruong.Length; k++) {
            //            if (Convert.ToString(vR.Rows[i][arrDSTruong[k]]) != Convert.ToString(dtChungTuChiTiet.Rows[j][arrDSTruong[k]])) {
            //                ok = false;
            //                break;
            //            }
            //        }
            //        if (ok) {
            //            if (count == 0) {
            //                for (int k = 0; k < vR.Columns.Count - 2; k++) {
            //                    vR.Rows[i][k] = dtChungTuChiTiet.Rows[j][vR.Columns[k].ColumnName];
            //                }
            //                count++;
            //            } else {
            //                DataRow row = vR.NewRow();
            //                for (int k = 0; k < vR.Columns.Count - 2; k++) {
            //                    row[k] = dtChungTuChiTiet.Rows[j][vR.Columns[k].ColumnName];
            //                }
            //                vR.Rows.InsertAt(row, i + 1);
            //                i++;
            //                vRCount++;
            //            }
            //        }
            //    }

            //}

            //LayChiTieu(vR);
            //LayCapPhatDaCap(vR);

            //dtChungTu.Dispose();
            //dtChungTuChiTiet.Dispose();
            //cmd.Dispose();
            return new DataTable();
        }
        private static void LayChiTieu(DataTable vR)
        {
            //NameValueCollection data = CapPhat_ChungTuModels.LayThongTin(iID_MaCapPhat);
            //DateTime ngayCapPhat = Convert.ToDateTime(data["dNgayCapPhat"]);
            //DataTable dtChiTieu = new DataTable();

            //#region definition input chitieu

            //var sql = FileHelpers.GetSqlQuery("dtCapPhat_Tien.sql");

            //#endregion

            //#region dieu kien chitieu  

            //String sLNSct = data["sDSLNS"];
            //if (data["iLoai"] == "1") {
            //    if (!String.IsNullOrEmpty(data["sDSLNS"])) {
            //        if (sLNSct == "1040200" || sLNSct == "1040300")
            //            sLNSct = "1040100";
            //        if (sLNSct.Contains("1020100")) {
            //            sLNSct += ",1020100,1020000";
            //        }
            //    }
            //}

            //if (data["sLoai"] == "sTM") {
            //    sql = sql.Replace("@@MucCap", "sM,sTM");
            //} else if (data["sLoai"] == "sM") {
            //    sql = sql.Replace("@@MucCap", "sM");
            //} else {
            //    sql = sql.Replace("@@MucCap", "sM,sTM,sTTM,sNG");
            //}
            //#endregion

            //#region get data chitieu

            //using (var conn = ConnectionFactory.Default.GetConnection())
            //using (var cmd = new SqlCommand(sql, conn)) {
            //    cmd.Parameters.AddWithValue("@iID_MaPhongBan", data["iID_MaPhongBan"]);
            //    cmd.Parameters.AddWithValue("@iNamLamViec", data["iNamLamViec"]);
            //    cmd.Parameters.AddWithValue("@iID_MaNguonNganSach", data["iID_MaNguonNganSach"]);
            //    cmd.Parameters.AddWithValue("@iID_MaNamNganSach", data["iID_MaNamNganSach"]);
            //    cmd.Parameters.AddWithValue("@sLNS", sLNSct);
            //    cmd.Parameters.AddWithValue("@dNgayCapPhat", ngayCapPhat);

            //    dtChiTieu = cmd.GetTable();
            //}
            //#endregion

            //int count = vR.Rows.Count;
            //for (int i = count - 1; i >= 0; i--) {
            //    String sLNS = Convert.ToString(vR.Rows[i]["sLNS"]);
            //    String sL = Convert.ToString(vR.Rows[i]["sL"]);
            //    String sK = Convert.ToString(vR.Rows[i]["sK"]);
            //    String sM = Convert.ToString(vR.Rows[i]["sM"]);
            //    String sTM = Convert.ToString(vR.Rows[i]["sTM"]);
            //    String sTTM = Convert.ToString(vR.Rows[i]["sTTM"]);
            //    String sNG = Convert.ToString(vR.Rows[i]["sNG"]);
            //    String iID_MaDonVi = Convert.ToString(vR.Rows[i]["iID_MaDonVi"]);
            //    if (sLNS == "1040200" || sLNS == "1040300")
            //        sLNS = "1040100";
            //    DataRow[] dr, dr1;
            //    if (data["sLoai"] == "sM") {
            //        dr = dtChiTieu.Select("sLNS='" + sLNS + "' AND sL='" + sL + "' AND sK='" + sK + "' AND sM='" + sM + "' AND iID_MaDonVi='" + iID_MaDonVi + "'");
            //        dr1 = dtChiTieu.Select("sLNS='" + sLNS + "' AND sL='" + sL + "' AND sK='" + sK + "' AND sM='" + sM + "'");
            //    } else if (data["sLoai"] == "sTM") {
            //        dr = dtChiTieu.Select("sLNS='" + sLNS + "' AND sL='" + sL + "' AND sK='" + sK + "' AND sM='" + sM + "' AND sTM='" + sTM + "' AND iID_MaDonVi='" + iID_MaDonVi + "'");
            //        dr1 = dtChiTieu.Select("sLNS='" + sLNS + "' AND sL='" + sL + "' AND sK='" + sK + "' AND sM='" + sM + "' AND sTM='" + sTM + "'");
            //    } else {
            //        dr = dtChiTieu.Select("sLNS='" + sLNS + "' AND sL='" + sL + "' AND sK='" + sK + "' AND sM='" + sM + "' AND sTM='" + sTM + "' AND sTTM='" + sTTM + "' AND sNG='" + sNG + "' AND iID_MaDonVi='" + iID_MaDonVi + "'");
            //        dr1 = dtChiTieu.Select("sLNS='" + sLNS + "' AND sL='" + sL + "' AND sK='" + sK + "' AND sM='" + sM + "' AND sTM='" + sTM + "' AND sTTM='" + sTTM + "' AND sNG='" + sNG + "'");
            //    }
            //    if (dr.Length > 0) {
            //        //gan vao bang chung tu chi tiet VR
            //        DataRow row1 = vR.Rows[i];
            //        if (Convert.ToString(row1["sLNS"]) == "1040200") {
            //            row1["rTuChi_PhanBo"] = dr[0]["rHangNhap"];
            //        } else if (Convert.ToString(row1["sLNS"]) == "1040300") {
            //            row1["rTuChi_PhanBo"] = dr[0]["rHangMua"];
            //        } else {
            //            row1["rTuChi_PhanBo"] = Convert.ToDouble(dr[0]["rTuChi"]);
            //        }
            //    }

            //    //else if (dr1.Length == 0)
            //    //{                    
            //    //    if (data["sLoai"] == "sM")
            //    //    {
            //    //        if (Convert.ToString(vR.Rows[i]["sM"]) != "")
            //    //        {
            //    //            vR.Rows.RemoveAt(i);
            //    //        }
            //    //    }
            //    //    if (data["sLoai"] == "sTM")
            //    //    {
            //    //        if (Convert.ToString(vR.Rows[i]["sTM"]) != "")
            //    //        {
            //    //            vR.Rows.RemoveAt(i);
            //    //        }
            //    //    }
            //    //    if (data["sLoai"] == "sNG")
            //    //    {
            //    //        if (Convert.ToString(vR.Rows[i]["sNG"]) != "")
            //    //        {
            //    //            vR.Rows.RemoveAt(i);
            //    //        }
            //    //    }
            //    //}
            //    else {
            //        //if (data["sLoai"] == "sM")
            //        //{
            //        //    if (Convert.ToString(vR.Rows[i]["sM"]) != "" && Convert.ToString(vR.Rows[i]["iID_MaDonVi"]) != "")
            //        //    {
            //        //        vR.Rows.RemoveAt(i);
            //        //    }
            //        //}
            //        //if (data["sLoai"] == "sTM")
            //        //{
            //        //    if (Convert.ToString(vR.Rows[i]["sTM"]) != "" && Convert.ToString(vR.Rows[i]["iID_MaDonVi"]) != "")
            //        //    {
            //        //        vR.Rows.RemoveAt(i);
            //        //    }
            //        //}
            //        //if (data["sLoai"] == "sNG")
            //        //{
            //        //    if (Convert.ToString(vR.Rows[i]["sNG"]) != "" && Convert.ToString(vR.Rows[i]["iID_MaDonVi"]) != "")
            //        //    {
            //        //        vR.Rows.RemoveAt(i);
            //        //    }
            //        //}
            //    }
            //}

        }
        private static void LayCapPhatDaCap(DataTable vR)
        {
            //NameValueCollection data = CapPhat_ChungTuModels.LayThongTin(iID_MaCapPhat);
            //DateTime ngayCT = Convert.ToDateTime(data["dNgayCapPhat"]);
            //DataTable dtCapPhat = new DataTable();

            //#region definition input dacap

            //var sql = FileHelpers.GetSqlQuery("dtCapPhat_DaCap.sql");

            //#endregion

            //#region dieu kien dacap  

            //String sLNSct = data["sDSLNS"];
            //if (data["iLoai"] == "1") {
            //    if (!String.IsNullOrEmpty(data["sDSLNS"])) {
            //        if (sLNSct == "1040200" || sLNSct == "1040300")
            //            sLNSct = "1040100";
            //        if (sLNSct.Contains("1020100")) {
            //            sLNSct += ",1020100,1020000";
            //        }
            //    }
            //}

            //if (data["sLoai"] == "sTM") {
            //    sql = sql.Replace("@@select", @"SUBSTRING(sLNS,1,1) as sLNS1
            //                                , SUBSTRING(sLNS, 1, 3) as sLNS3
            //                                , sLNS5 = CASE WHEN sLNS = 1020000 then 10201 else SUBSTRING(sLNS, 1, 5) END
            //                                , sLNS = CASE WHEN sLNS = 1020000 then 1020100 else sLNS END
            //                                , sL
            //                                , sK
            //                                , sM
            //                                , sTM");
            //    sql = sql.Replace("@@MucCap", @"sLNS
            //                                    , sL
            //                                    , sK
            //                                    , sM
            //                                    , sTM");
            //} else if (data["sLoai"] == "sM") {
            //    sql = sql.Replace("@@select", @"SUBSTRING(sLNS,1,1) as sLNS1
            //                                , SUBSTRING(sLNS, 1, 3) as sLNS3
            //                                , sLNS5 = CASE WHEN sLNS = 1020000 then 10201 else SUBSTRING(sLNS, 1, 5) END
            //                                , sLNS = CASE WHEN sLNS = 1020000 then 1020100 else sLNS END
            //                                , sL
            //                                , sK
            //                                , sM
            //                                ");
            //    sql = sql.Replace("@@MucCap", @"sLNS
            //                                    , sL
            //                                    , sK
            //                                    , sM");
            //} else {
            //    sql = sql.Replace("@@select", @"SUBSTRING(sLNS,1,1) as sLNS1
            //                                , SUBSTRING(sLNS, 1, 3) as sLNS3
            //                                , sLNS5 = CASE WHEN sLNS = 1020000 then 10201 else SUBSTRING(sLNS, 1, 5) END
            //                                , sLNS = CASE WHEN sLNS = 1020000 then 1020100 else sLNS END
            //                                , sL
            //                                , sK
            //                                , sM
            //                                , sTM
            //                                , sTTM
            //                                , sNG");
            //    sql = sql.Replace("@@MucCap", @"sLNS
            //                                    , sL
            //                                    , sK
            //                                    , sM
            //                                    , sTM
            //                                    , sTTM
            //                                    , sNG");
            //}
            //sql = sql.Replace("@@Tien_HienVat", @"( @sLNS IS NULL OR sLNS IN (SELECT * FROM F_Split(@sLNS))) AND 1 ");

            //#endregion      

            //#region get data dacap

            //using (var conn = ConnectionFactory.Default.GetConnection())
            //using (var cmd = new SqlCommand(sql, conn)) {
            //    cmd.Parameters.AddWithValue("@iID_MaPhongBan", data["iID_MaPhongBan"]);
            //    cmd.Parameters.AddWithValue("@iNamLamViec", data["iNamLamViec"]);
            //    cmd.Parameters.AddWithValue("@iID_MaNguonNganSach", data["iID_MaNguonNganSach"]);
            //    cmd.Parameters.AddWithValue("@iID_MaNamNganSach", data["iID_MaNamNganSach"]);
            //    cmd.Parameters.AddWithValue("@sLNS", sLNSct);
            //    cmd.Parameters.AddWithValue("@dNgayCapPhat", ngayCT);
            //    cmd.Parameters.AddWithValue("@iSoCapPhat", data["iSoCapPhat"]);

            //    dtCapPhat = cmd.GetTable();
            //}
            //#endregion 

            //for (int i = 0; i < vR.Rows.Count; i++) {
            //    String sLNS = Convert.ToString(vR.Rows[i]["sLNS"]);
            //    String sL = Convert.ToString(vR.Rows[i]["sL"]);
            //    String sK = Convert.ToString(vR.Rows[i]["sK"]);
            //    String sM = Convert.ToString(vR.Rows[i]["sM"]);
            //    String sTM = Convert.ToString(vR.Rows[i]["sTM"]);
            //    String sTTM = Convert.ToString(vR.Rows[i]["sTTM"]);
            //    String sNG = Convert.ToString(vR.Rows[i]["sNG"]);
            //    String iID_MaDonVi = Convert.ToString(vR.Rows[i]["iID_MaDonVi"]);
            //    DataRow[] dr;
            //    if (data["sLoai"] == "sM") {
            //        dr = dtCapPhat.Select("sLNS='" + sLNS + "' AND sL='" + sL + "' AND sK='" + sK + "' AND sM='" + sM + "' AND iID_MaDonVi='" + iID_MaDonVi + "'");
            //    } else if (data["sLoai"] == "sTM") {
            //        dr = dtCapPhat.Select("sLNS='" + sLNS + "' AND sL='" + sL + "' AND sK='" + sK + "' AND sM='" + sM + "' AND sTM='" + sTM + "' AND iID_MaDonVi='" + iID_MaDonVi + "'");
            //    } else {
            //        dr = dtCapPhat.Select("sLNS='" + sLNS + "' AND sL='" + sL + "' AND sK='" + sK + "' AND sM='" + sM + "' AND sTM='" + sTM + "' AND sTTM='" + sTTM + "' AND sNG='" + sNG + "' AND iID_MaDonVi='" + iID_MaDonVi + "'");
            //    }
            //    if (dr.Length > 0) {
            //        //gan vao bang chung tu chi tiet VR
            //        DataRow row1 = vR.Rows[i];
            //        row1["rTuChi_DaCap"] = dr[0]["rTuChi"];
            //    }
            //}

        }
    }
}
