using System;
using System.Data;
using System.Data.SqlClient;
using DomainModel;
using DomainModel.Abstract;
using System.Web;

namespace VIETTEL.Models.ThuNop {
    public class ThuNop_LoaiHinhModels
    {
        public static String LayXauLoaiHinh(String sTenLoaiHinh, String sKyHieu, String Path, String XauThemMoi, String XauHanhDong,
            String XauSapXep, String MaLoaiHinhCha, int Cap, ref int ThuTu)
        {
            String vR = "";
            String SQL = "";
            SqlCommand cmd1 = new SqlCommand();
            String strSQL = "SELECT Top 1 * FROM TN_DanhMucLoaiHinh WHERE iTrangThai=1 ";
            Boolean Ok = false;
            if (String.IsNullOrEmpty(sTenLoaiHinh) == false)
            {
                Ok = true;
                strSQL += " AND sTen LIKE @sTen";
                cmd1.Parameters.AddWithValue("@sTen", sTenLoaiHinh + "%");
            }
            if (String.IsNullOrEmpty(sKyHieu) == false && sKyHieu != "")
            {
                Ok = true;
                strSQL += " AND sKyHieu= @sKyHieu";
                cmd1.Parameters.AddWithValue("@sKyHieu", sKyHieu);
            }
            cmd1.CommandText = strSQL;
            if (Ok)
            {
                DataTable dt1 = Connection.GetDataTable(cmd1);
                cmd1.Dispose();

                if (dt1.Rows.Count > 0)
                {
                    MaLoaiHinhCha = Convert.ToString(dt1.Rows[0]["iID_MaLoaiHinh_Cha"]);
                    sKyHieu = "";
                    sTenLoaiHinh = "";
                }
            }

            SqlCommand cmd = new SqlCommand();
            if (MaLoaiHinhCha != null && MaLoaiHinhCha != "")
            {
                SQL = string.Format("SELECT * FROM TN_DanhMucLoaiHinh WHERE iTrangThai=1 AND iID_MaLoaiHinh_Cha = '{0}' Order by iSTT", MaLoaiHinhCha);
            }
            else
            {
                SQL = "SELECT * FROM TN_DanhMucLoaiHinh WHERE iTrangThai=1 AND iID_MaLoaiHinh_Cha = 0";
            }
            cmd.CommandText = SQL;
            DataTable dt = Connection.GetDataTable(cmd);
            if (dt.Rows.Count > 0)
            {
                int i, tgThuTu;

                string strPG = "", strXauMucLucLoaiHinhCon, strDoanTrang = "";

                for (i = 1; i <= Cap; i++)
                {
                    strDoanTrang += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                }
                for (i = 0; i < dt.Rows.Count; i++)
                {
                    ThuTu++;
                    tgThuTu = ThuTu;
                    DataRow Row = dt.Rows[i];
                    String strHanhDong = XauHanhDong.Replace("%23%23", Row["iID_MaLoaiHinh"].ToString());
                    strXauMucLucLoaiHinhCon = LayXauLoaiHinh(sTenLoaiHinh, sKyHieu, Path, XauThemMoi, XauHanhDong, XauSapXep, Convert.ToString(Row["iID_MaLoaiHinh"]), Cap + 1, ref ThuTu);

                    if (Convert.ToBoolean(Row["bLaHangCha"]))
                    {
                        strHanhDong = XauThemMoi.Replace("%23%23", Row["iID_MaLoaiHinh"].ToString()) + strHanhDong;
                    }
                    if (strXauMucLucLoaiHinhCon != "")
                    {                        
                        strHanhDong += XauSapXep.Replace("%23%23", Row["iID_MaLoaiHinh"].ToString());
                    }
                    strPG += string.Format("<tr>");
                    if (Cap == 0)
                    {
                        strPG += string.Format("<td style=\"background-color:#f4f9fd;padding: 3px 3px;\">{0}<b>{1}</b></td>", strDoanTrang, HttpUtility.HtmlEncode(Row["sTT"]));
                        strPG += string.Format("<td style=\"background-color:#f4f9fd;padding: 3px 3px;\">{0}<b>{1}</b></td>", strDoanTrang, HttpUtility.HtmlEncode(Row["sKyHieu"]));
                        strPG += string.Format("<td style=\"background-color:#f4f9fd;padding: 3px 3px;\">{0}<b>{1}</b></td>", strDoanTrang, HttpUtility.HtmlEncode(Row["sTen"]));
                    }
                    else
                    {
                        if (tgThuTu % 2 == 0)
                        {
                            strPG += string.Format("<td style=\"background-color:#dff0fb;padding: 3px 3px;\">{0}{1}</td>", strDoanTrang, HttpUtility.HtmlEncode(Row["sTT"]));
                            strPG += string.Format("<td style=\"background-color:#dff0fb;padding: 3px 3px;\">{0}{1}</td>", strDoanTrang, HttpUtility.HtmlEncode(Row["sKyHieu"]));
                            strPG += string.Format("<td style=\"background-color:#dff0fb;padding: 3px 3px;\">{0}{1}</td>", strDoanTrang, HttpUtility.HtmlEncode(Row["sTen"]));
                        }
                        else
                        {
                            strPG += string.Format("<td style=\"padding: 3px 3px;\">{0}{1}</td>", strDoanTrang, HttpUtility.HtmlEncode(Row["sTT"]));
                            strPG += string.Format("<td style=\"padding: 3px 3px;\">{0}{1}</td>", strDoanTrang, HttpUtility.HtmlEncode(Row["sKyHieu"]));
                            strPG += string.Format("<td style=\"padding: 3px 3px;\">{0}{1}</td>", strDoanTrang, HttpUtility.HtmlEncode(Row["sTen"]));
                        }
                    }
                    if (tgThuTu % 2 == 0)
                    {
                        strPG += string.Format("<td style=\"background-color:#dff0fb;padding: 3px 3px; text-align: center\">{0}</td>", strHanhDong);
                    }
                    else
                    {
                        strPG += string.Format("<td style=\"padding: 3px 3px; text-align: center\">{0}</td>", strHanhDong);
                    }

                    strPG += string.Format("</tr>");
                    strPG += strXauMucLucLoaiHinhCon;
                }
                vR = String.Format("{0}", strPG);
            }
            dt.Dispose();
            return vR;
        }

        public static DataTable Get_ChiTietLoaiHinh_Row(String iID_MaLoaiHinh)
        {
            DataTable vR;
            SqlCommand cmd = new SqlCommand("SELECT * FROM TN_DanhMucLoaiHinh WHERE iTrangThai=1 AND iID_MaLoaiHinh=@iID_MaLoaiHinh");
            cmd.Parameters.AddWithValue("@iID_MaLoaiHinh", iID_MaLoaiHinh);
            vR = Connection.GetDataTable(cmd);
            cmd.Dispose();

            return vR;
        }

        public static int Get_So_KyHieuLoaiHinh(String sKyHieu)
        {
            int vR = 0;
            SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM TN_DanhMucLoaiHinh WHERE iTrangThai=1 AND sKyHieu=@sKyHieu");
            cmd.Parameters.AddWithValue("@sKyHieu", sKyHieu);
            vR = Convert.ToInt32(Connection.GetValue(cmd, 0));
            cmd.Dispose();

            return vR;
        }

        public static Boolean Delete(String iID_MaLoaiHinh)
        {
            //Check co so lieu
            String SQL = "";
            SQL = String.Format(@"SELECT COUNT(iID_MaLoaiHinh) FROM TN_ChungTuChiTiet WHERE iID_MaLoaiHinh=@iID_MaLoaiHinh AND iTrangThai=1");
            SqlCommand cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@iID_MaLoaiHinh", iID_MaLoaiHinh);
            int count = (int)Connection.GetValue(cmd, 1);
            if (count > 0)
            {
                return false;
            }

            //Khong phai la hang cha
            SQL = String.Format(@"SELECT COUNT(iID_MaLoaiHinh) FROM TN_DanhMucLoaiHinh WHERE iID_MaLoaiHinh=@iID_MaLoaiHinh AND iTrangThai=1");
            cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@iID_MaLoaiHinh", iID_MaLoaiHinh);
            count = (int)Connection.GetValue(cmd, 1);
            if (count > 0)
            {
                return false;
            }

            Boolean vR = false;
            Bang bang = new Bang("TN_DanhMucLoaiHinh");
            bang.GiaTriKhoa = iID_MaLoaiHinh;
            bang.Delete();
            vR = true;
            return vR;
        }

        public static DataTable Get_MucLucLoaiHinh()
        {
            DataTable vR;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "SELECT * FROM TN_DanhMucLoaiHinh WHERE iTrangThai = 1 ORDER BY iSTT";
            vR = Connection.GetDataTable(cmd);
            cmd.Dispose();

            return vR;
        }

        public static DataTable Get_LoaiHinh_Theo_KyHieu(String sKyHieu)
        {
            DataTable vR;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "SELECT TOP 1 TN_DanhMucLOaiHinh.* FROM TN_DanhMucLoaiHinh WHERE iTrangThai = 1 AND sKyHieu=@sKyHieu ORDER BY iSTT";
            cmd.Parameters.AddWithValue("@sKyHieu", sKyHieu);
            vR = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return vR;
        }
    }
}