using System;
using System.Data;
using System.Data.SqlClient;
using DomainModel;
using DomainModel.Abstract;
using System.Web;

namespace VIETTEL.Models.ThuNop {
    public class ThuNop_CauHinhBaoCaoModels
    {
        public static String LayXauBaoCao(String iLoai, String sTenBaoCao, String sLoaiHinh, String Path, String XauHanhDong,
            String MaBaoCaoCha, int Cap, ref int ThuTu)
        {
            String vR = "";
            String SQL = "";
            SqlCommand cmd1 = new SqlCommand();
            String strSQL = "SELECT Top 1 * FROM TN_CauHinhBaoCao WHERE iTrangThai=1 AND iID_MaLoaiBaoCao = @iLoai";
            Boolean Ok = false;
            cmd1.Parameters.AddWithValue("@iLoai", iLoai);
            if (String.IsNullOrEmpty(sTenBaoCao) == false)
            {
                Ok = true;
                strSQL += " AND sTen LIKE @sTen";
                cmd1.Parameters.AddWithValue("@sTen", sTenBaoCao + "%");
            }
            if (String.IsNullOrEmpty(sLoaiHinh) == false && sLoaiHinh != "")
            {
                Ok = true;
                strSQL += " AND sLoaiHinh= @sLoaiHinh";
                cmd1.Parameters.AddWithValue("@sLoaiHinh", sLoaiHinh);
            }
            cmd1.CommandText = strSQL;
            if (Ok)
            {
                DataTable dt1 = Connection.GetDataTable(cmd1);
                cmd1.Dispose();

                if (dt1.Rows.Count > 0)
                {
                    MaBaoCaoCha = Convert.ToString(dt1.Rows[0]["iID_MaBaoCao_Cha"]);
                    sLoaiHinh = "";
                    sTenBaoCao = "";
                }
            }

            SqlCommand cmd = new SqlCommand();
            if (MaBaoCaoCha != null && MaBaoCaoCha != "")
            {
                SQL = string.Format("SELECT * FROM TN_CauHinhBaoCao WHERE iTrangThai = 1 AND iID_MaBaoCao_Cha = '{0}' AND iID_MaLoaiBaoCao = @iLoai ORDER BY sLoaiNS,iID_MaCotBaoCao", MaBaoCaoCha);
            }
            else
            {
                SQL = "SELECT * FROM TN_CauHinhBaoCao WHERE iTrangThai = 1 AND iID_MaBaoCao_Cha = 0 AND iID_MaLoaiBaoCao = @iLoai ORDER BY sLoaiNS,iID_MaCotBaoCao";
            }
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iLoai", iLoai);
            DataTable dt = Connection.GetDataTable(cmd);
            if (dt.Rows.Count > 0)
            {
                int i, tgThuTu;

                string strPG = "";

                string strDoanTrang = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                
                for (i = 0; i < dt.Rows.Count; i++)
                {
                    ThuTu++;
                    tgThuTu = ThuTu;
                    DataRow Row = dt.Rows[i];
                    String strHanhDong = XauHanhDong.Replace("%23%23", Row["iID_MaBaoCao"].ToString());
                    
                    strPG += string.Format("<tr>");
                    if (iLoai == "2")
                    {
                        if (tgThuTu % 2 == 0)
                        {
                            strPG += string.Format("<td style=\"background-color:#dff0fb;padding: 3px 3px;\">{0}{1}</td>", strDoanTrang, tgThuTu);
                            strPG += string.Format("<td style=\"background-color:#dff0fb;padding: 3px 3px;\">{0}<b>{1}</b></td>", strDoanTrang, HttpUtility.HtmlEncode(Row["sLoaiNS"]));
                            strPG += string.Format("<td style=\"background-color:#dff0fb;padding: 3px 3px;\">{0}<b>{1}</b></td>", strDoanTrang, HttpUtility.HtmlEncode(Row["iID_MaCotBaoCao"]));
                            strPG += string.Format("<td style=\"background-color:#dff0fb;padding: 3px 3px;\"><div style=\"word-wrap: break-word; width: 650px;\">{0}{1}</div></td>", strDoanTrang, HttpUtility.HtmlEncode(Row["sLoaiHinh"]));
                            strPG += string.Format("<td style=\"background-color:#dff0fb;padding: 3px 3px;\">{0}{1}</td>", strDoanTrang, HttpUtility.HtmlEncode(Row["sTen"]));
                            strPG += string.Format("<td style=\"background-color:#dff0fb;padding: 3px 3px; text-align: center\">{0}</td>", strHanhDong);
                        }
                        else
                        {
                            strPG += string.Format("<td style=\"padding: 3px 3px;\">{0}{1}</td>", strDoanTrang, tgThuTu);
                            strPG += string.Format("<td style=\"padding: 3px 3px;\">{0}<b>{1}</b></td>", strDoanTrang, HttpUtility.HtmlEncode(Row["sLoaiNS"]));
                            strPG += string.Format("<td style=\"padding: 3px 3px;\">{0}<b>{1}</b></td>", strDoanTrang, HttpUtility.HtmlEncode(Row["iID_MaCotBaoCao"]));
                            strPG += string.Format("<td style=\"padding: 3px 3px;\"><div style=\"word-wrap: break-word; width: 650px;\">{0}{1}</div></td>", strDoanTrang, HttpUtility.HtmlEncode(Row["sLoaiHinh"]));
                            strPG += string.Format("<td style=\"padding: 3px 3px;\">{0}{1}</td>", strDoanTrang, HttpUtility.HtmlEncode(Row["sTen"]));
                            strPG += string.Format("<td style=\"padding: 3px 3px; text-align: center\">{0}</td>", strHanhDong);
                        }
                    }
                    else if (iLoai == "3")
                    {
                        if (tgThuTu % 2 == 0)
                        {
                            strPG += string.Format("<td style=\"background-color:#dff0fb;padding: 3px 3px;\">{0}{1}</td>", strDoanTrang, tgThuTu);                            
                            strPG += string.Format("<td style=\"background-color:#dff0fb;padding: 3px 3px;\">{0}<b>{1}</b></td>", strDoanTrang, HttpUtility.HtmlEncode(Row["iID_MaCotBaoCao"]));
                            strPG += string.Format("<td style=\"background-color:#dff0fb;padding: 3px 3px;\"><div style=\"word-wrap: break-word; width: 650px;\">{0}{1}</div></td>", strDoanTrang, HttpUtility.HtmlEncode(Row["sLoaiHinh"]));
                            strPG += string.Format("<td style=\"background-color:#dff0fb;padding: 3px 3px;\">{0}{1}</td>", strDoanTrang, HttpUtility.HtmlEncode(Row["sTen"]));
                            strPG += string.Format("<td style=\"background-color:#dff0fb;padding: 3px 3px;\">{0}{1}</td>", strDoanTrang, HttpUtility.HtmlEncode(Row["sTenVietTat"]));
                            strPG += string.Format("<td style=\"background-color:#dff0fb;padding: 3px 3px; text-align: center\">{0}</td>", strHanhDong);
                        }
                        else
                        {
                            strPG += string.Format("<td style=\"padding: 3px 3px;\">{0}{1}</td>", strDoanTrang, tgThuTu);                            
                            strPG += string.Format("<td style=\"padding: 3px 3px;\">{0}<b>{1}</b></td>", strDoanTrang, HttpUtility.HtmlEncode(Row["iID_MaCotBaoCao"]));
                            strPG += string.Format("<td style=\"padding: 3px 3px;\"><div style=\"word-wrap: break-word; width: 650px;\">{0}{1}</div></td>", strDoanTrang, HttpUtility.HtmlEncode(Row["sLoaiHinh"]));
                            strPG += string.Format("<td style=\"padding: 3px 3px;\">{0}{1}</td>", strDoanTrang, HttpUtility.HtmlEncode(Row["sTen"]));
                            strPG += string.Format("<td style=\"padding: 3px 3px;\">{0}{1}</td>", strDoanTrang, HttpUtility.HtmlEncode(Row["sTenVietTat"]));
                            strPG += string.Format("<td style=\"padding: 3px 3px; text-align: center\">{0}</td>", strHanhDong);
                        }
                    }
                    
                    strPG += string.Format("</tr>");
                }
                vR = String.Format("{0}", strPG);
            }
            dt.Dispose();
            return vR;
        }

        public static DataTable Get_ChiTietBaoCao_Row(String iID_MaBaoCao)
        {
            DataTable vR;
            SqlCommand cmd = new SqlCommand("SELECT * FROM TN_CauHinhBaoCao WHERE iID_MaBaoCao=@iID_MaBaoCao");
            cmd.Parameters.AddWithValue("@iID_MaBaoCao", iID_MaBaoCao);
            vR = Connection.GetDataTable(cmd);
            cmd.Dispose();

            return vR;
        }

        public static int Get_So_KyHieuBaoCao(String sLoaiHinh)
        {
            int vR = 0;
            SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM TN_CauHinhBaoCao WHERE sLoaiHinh=@sLoaiHinh");
            cmd.Parameters.AddWithValue("@sLoaiHinh", sLoaiHinh);
            vR = Convert.ToInt32(Connection.GetValue(cmd, 0));
            cmd.Dispose();

            return vR;
        }

        public static Boolean Delete(String iID_MaBaoCao)
        {        
            Boolean vR = false;
            Bang bang = new Bang("TN_CauHinhBaoCao");
            bang.GiaTriKhoa = iID_MaBaoCao;
            bang.Delete();
            vR = true;
            return vR;
        }        
        public static DataTable Get_MucLucBaoCao()
        {
            DataTable vR;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "SELECT * FROM TN_CauHinhBaoCao WHERE iTrangThai = 1 ORDER BY iSTT";
            vR = Connection.GetDataTable(cmd);
            cmd.Dispose();

            return vR;
        }

        public static DataTable Get_BaoCao_Theo_KyHieu(String sLoaiHinh)
        {
            DataTable vR;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "SELECT TOP 1 TN_CauHinhBaoCao.* FROM TN_CauHinhBaoCao WHERE iTrangThai = 1 AND sLoaiHinh=@sLoaiHinh ORDER BY iSTT";
            cmd.Parameters.AddWithValue("@sLoaiHinh", sLoaiHinh);
            vR = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return vR;
        }

        public static DataTable Get_MucLucLoaiHinh(String iLoai)
        {
            DataTable vR;
            SqlCommand cmd = new SqlCommand();
            if (iLoai == "2")
            {
                cmd.CommandText = "SELECT sKyHieu sLoaiHinh, (sKyHieu + ' - ' + sTen) as sTen FROM TN_DanhMucLoaiHinh WHERE iTrangThai = 1 AND LEN(sKyHieu) > 2 ORDER BY sKyHieu";
            }
            else if (iLoai == "3")
            {
                cmd.CommandText =
                    String.Format(
                        @"SELECT DISTINCT sLNS as sLoaiHinh,sLNS+' - ' +sMoTa as sTen FROM NS_MucLucNganSach WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec AND sLNS LIKE '8%' AND sL='' AND LEN(sLNS)=7 ORDER BY sLNS");
            }
            int iNamLamViec = Convert.ToInt32(ReportModels.LayNamLamViec(System.Web.HttpContext.Current.User.Identity.Name));
            cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
            vR = Connection.GetDataTable(cmd);
            cmd.Dispose();

            return vR;
        }

    }
}