using DomainModel;
using DomainModel.Abstract;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;

namespace VIETTEL.Models
{
    public partial class DuToan_ChungTuChiTietModels
    {

        public static DataTable LayDanhSachChungTuChiTiet2(string maChungTu, Dictionary<String, String> arrGiaTriTimKiem = null)
        {
            DataTable dt = null;
            string dk = "";
            String DSTruong = MucLucNganSachModels.strDSTruong;
            String[] arrDSTruong = DSTruong.Split(',');
            SqlCommand cmd = new SqlCommand();
            if (arrGiaTriTimKiem != null)
            {                
                for (int i = 0; i < arrDSTruong.Length; i++)
                {
                    if (String.IsNullOrEmpty(arrGiaTriTimKiem[arrDSTruong[i]]) == false)
                    {
                        dk += String.Format(" AND {0} LIKE @{0}", arrDSTruong[i]);
                        cmd.Parameters.AddWithValue("@" + arrDSTruong[i], "%" + arrGiaTriTimKiem[arrDSTruong[i]] + "%");
                    }
                }
            }

            // lay cac chung tu chi tiet co phan cap, gom ca so am va duong           
            var sql = String.Format(@"select  * 
                                      from    DT_ChungTuChiTiet 
                                      where   iTrangThai in (1,2)
                                              and iID_MaChungTu = @iID_MaChungTu 
                                              and (rPhanCap <> 0) {0}
                                      order by sXauNoiMa", dk);            

            cmd.Parameters.AddWithValue("@iID_MaChungTu", maChungTu);
            cmd.CommandText = sql;
            dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return dt;
        }

        public static DataTable GetChungTuChiTiet_PhanCap2(string maChungTu, Dictionary<string, string> dicGiaTriTimKiem, string MaND, string sLNS, string sXauNoiMa, string iKyThuat, string MaLoai)
        {
            DataTable vR;
            DataTable dt = NganSach_HamChungModels.DSLNSCuaPhongBan(MaND);
            string sTruongTien = MucLucNganSachModels.strDSTruongTien + ",iID_MaChungTuChiTiet,iID_MaDonVi,sTenDonVi,sTenDonVi_BaoDam,iID_MaPhongBanDich,iID_MaPhongBan,sGhiChu,sMaCongTrinh";
            string[] arrDSTruong = MucLucNganSachModels.strDSTruong.Split(',');
            string[] arrDSTruong_TK = (MucLucNganSachModels.strDSTruong + ",iID_MaDonVi").Split(',');
            string[] arrDSTruongTien = sTruongTien.Split(',');
            string[] arrsXauNoiMa = sXauNoiMa.Split(',');
            string[] arriID_MaChungTu = maChungTu.Split(',');
            string[] arrsLNS = sLNS.Split(',');
            string dksXauNoiMa = "", dkiID_MaChungTu = "", dksLNS = "";
            for (int i = 0; i < arrsXauNoiMa.Length; i++)
            {
                if (string.IsNullOrEmpty(dksXauNoiMa))
                {
                    dksXauNoiMa += String.Format("sXauNoiMa LIKE '%{0}%'", arrsXauNoiMa[i]);
                }
                else
                {
                    dksXauNoiMa += String.Format(" OR sXauNoiMa LIKE '%{0}%'", arrsXauNoiMa[i]);
                }
            }
            dksXauNoiMa = " AND ( " + dksXauNoiMa + " )";
            for (int i = 0; i < arrsLNS.Length; i++)
            {
                string arLNS = arrsLNS[i];
                if (sLNS.StartsWith("2") || sLNS.Contains("4050000") || sLNS.Contains("1091800") || sLNS.Contains("4450000") || sLNS.Contains("4080300"))
                {
                }
                else
                {
                    arLNS = "1020100";
                }
                if (string.IsNullOrEmpty(dksLNS))
                {
                    dksLNS += String.Format("sLNS LIKE '%{0}%'", arLNS);
                }
                else
                {
                    dksLNS += String.Format(" OR sLNS LIKE '%{0}%'", arLNS);
                }
            }

            dksLNS = " AND ( " + dksLNS + " )";

            for (int i = 0; i < arriID_MaChungTu.Length; i++)
            {
                if (string.IsNullOrEmpty(dkiID_MaChungTu))
                {
                    dkiID_MaChungTu += String.Format("iID_MaChungTu = @iID_MaChungTu" + i);
                }
                else
                {
                    dkiID_MaChungTu += String.Format(" OR iID_MaChungTu = @iID_MaChungTu" + i);
                }
            }
            dkiID_MaChungTu = " AND ( " + dkiID_MaChungTu + " )";

            String sql, dk;
            SqlCommand cmd;

            #region NganhKyThuat
            if (iKyThuat == "1")
            {
                //<--Lay toan bo Muc luc ngan sach
                cmd = new SqlCommand();
                //Phan cap lan 2: danh sach nganh theo nguoi quan ly
                if (MaLoai == "2")
                {
                    dk = String.Format("iTrangThai=1 {0} {1}", dksLNS, dksXauNoiMa);
                }
                else
                {
                    dk = String.Format("iTrangThai=1 {0} {1}", dksLNS, dksXauNoiMa);
                }
                if (dicGiaTriTimKiem != null)
                {
                    for (int i = 0; i < arrDSTruong.Length; i++)
                    {
                        if (String.IsNullOrEmpty(dicGiaTriTimKiem[arrDSTruong[i]]) == false)
                        {
                            dk += String.Format(" AND {0} LIKE @{0}", arrDSTruong[i]);
                            cmd.Parameters.AddWithValue("@" + arrDSTruong[i], "%" + dicGiaTriTimKiem[arrDSTruong[i]] + "%");
                        }
                    }
                }
                if (dt.Rows.Count > 0)
                    dk += " AND( ";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    String Nguon = Convert.ToString(dt.Rows[i]["sLNS"]).Substring(0, 1);
                    String LoaiNS = Convert.ToString(dt.Rows[i]["sLNS"]).Substring(0, 3);
                    dk += "  (sLNS=@sLNS" + i + " OR (SUBSTRING(sLNS,1,1)=" + Nguon + " AND LEN(sLNS)=1) OR  (SUBSTRING(sLNS,1,3)=" + LoaiNS + " AND LEN(sLNS)=3))";
                    if (i < dt.Rows.Count - 1)
                        dk += " OR ";
                    cmd.Parameters.AddWithValue("@sLNS" + i, dt.Rows[i]["sLNS"]);
                }
                if (dt.Rows.Count > 0)
                    dk += " ) ";
                sql = String.Format("SELECT iID_MaMucLucNganSach,iID_MaMucLucNganSach_Cha,bLaHangCha,sXauNoiMa,{0},{1} FROM NS_MucLucNganSach WHERE {2} AND iNamLamViec = @iNamLamViec ORDER BY {3}", MucLucNganSachModels.strDSTruong, MucLucNganSachModels.strDSDuocNhapTruongTien, dk, MucLucNganSachModels.strDSTruongSapXep);
                cmd.CommandText = sql;
                string iNamLamViec = ReportModels.LayNamLamViec(System.Web.HttpContext.Current.User.Identity.Name);
                //if (Convert.ToInt32(iNamLamViec) <= 2017) iNamLamViec = "2017";
                //else iNamLamViec = "2018";
                cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                vR = Connection.GetDataTable(cmd);
                cmd.Dispose();
                //Lay toan bo Muc luc ngan sach-->
                dk = "iTrangThai=1 " + dkiID_MaChungTu;
                for (int i = 0; i < arriID_MaChungTu.Length; i++)
                {
                    cmd.Parameters.AddWithValue("@iID_MaChungTu" + i, arriID_MaChungTu[i]);
                }

                if (dicGiaTriTimKiem != null)
                {
                    if (String.IsNullOrEmpty(dicGiaTriTimKiem["iID_MaDonVi"]) == false)
                    {
                        dk += String.Format(" AND {0} LIKE @{0}", "iID_MaDonVi");
                        cmd.Parameters.AddWithValue("@" + "iID_MaDonVi", "%" + dicGiaTriTimKiem["iID_MaDonVi"] + "%");
                    }
                    if (String.IsNullOrEmpty(dicGiaTriTimKiem["iID_MaPhongBanDich"]) == false)
                    {
                        dk += String.Format(" AND {0} LIKE @{0}", "iID_MaPhongBanDich");
                        cmd.Parameters.AddWithValue("@" + "iID_MaPhongBanDich", "%" + dicGiaTriTimKiem["iID_MaPhongBanDich"] + "%");
                    }
                    for (int i = 0; i < arrDSTruong.Length; i++)
                    {
                        if (String.IsNullOrEmpty(dicGiaTriTimKiem[arrDSTruong[i]]) == false)
                        {
                            dk += String.Format(" AND {0} LIKE @{0}", arrDSTruong[i]);

                        }
                    }
                }
                dk += " AND (";
                for (int i = 1; i < arrDSTruongTien.Length - 8; i++)
                {
                    dk += arrDSTruongTien[i] + "<>0 OR ";
                }
                dk = dk.Substring(0, dk.Length - 3);
                dk += ") ";

                sql = String.Format(@"SELECT iID_MaMucLucNganSach,iID_MaMucLucNganSach_Cha,bLaHangCha,sXauNoiMa,sTenCongTrinh,sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,sMoTa,iID_MaChungTuChiTiet,iID_MaDonVi,sTenDonVi,sTenDonVi as sTenDonVi_BaoDam,iID_MaPhongBanDich,iID_MaPhongBan
                                        ,rNgay,rSoNguoi,rChiTaiKhoBac,rTonKho,rTuChi,rChiTaiNganh,rChiTapTrung,rHangNhap,rHangMua,rHienVat,rDuPhong,rPhanCap,sGhiChu
                                        FROM DT_ChungTuChiTiet_PhanCap
                                        WHERE  {0}

                                        UNION

                                        SELECT iID_MaMucLucNganSach,iID_MaMucLucNganSach_Cha,bLaHangCha,sXauNoiMa,sTenCongTrinh, sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,sMoTa,iID_MaChungTuChiTiet,iID_MaDonVi,sTenDonVi,sTenDonVi as sTenDonVi_BaoDam,iID_MaPhongBanDich,iID_MaPhongBan
                                        ,rNgay,rSoNguoi,rChiTaiKhoBac,rTonKho,rTuChi,rChiTaiNganh,rChiTapTrung,rHangNhap,rHangMua,rHienVat,rDuPhong,rPhanCap,sGhiChu
                                        FROM DT_ChungTuChiTiet
                                        WHERE {0}

                                        ORDER BY sM,sTM,sTTM,sNG, iID_MaDonVi", dk);
                cmd.CommandText = sql;

                DataTable dtChungTuChiTiet = Connection.GetDataTable(cmd);
                int cs0 = 0;
                DataColumn column;



                for (int j = 0; j < arrDSTruongTien.Length; j++)
                {


                    column = new DataColumn(arrDSTruongTien[j], typeof(String));
                    column.AllowDBNull = true;
                    vR.Columns.Add(column);

                }
                int vRCount = vR.Rows.Count;
                for (int i = 0; i < vRCount; i++)
                {
                    int count = 0;
                    for (int j = cs0; j < dtChungTuChiTiet.Rows.Count; j++)
                    {

                        Boolean ok = true;
                        for (int k = 2; k < arrDSTruong.Length; k++)
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
                                for (int k = 0; k < vR.Columns.Count - 1; k++)
                                {
                                    if (vR.Columns[k].ColumnName.StartsWith("b") == false || vR.Columns[k].ColumnName == "bLaHangCha")
                                        vR.Rows[i][k] = dtChungTuChiTiet.Rows[j][vR.Columns[k].ColumnName];
                                    else
                                        vR.Rows[i][k] = vR.Rows[i][k];
                                }
                                count++;
                            }
                            else
                            {
                                DataRow row = vR.NewRow();
                                for (int k = 0; k < vR.Columns.Count - 1; k++)
                                {
                                    if (vR.Columns[k].ColumnName.StartsWith("b") == false || vR.Columns[k].ColumnName == "bLaHangCha")
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
            }
            #endregion

            #region Nganh khac

            else
            {
                cmd = new SqlCommand();
                dk = String.Format("iTrangThai=1 {0} {1}", dksLNS, dksXauNoiMa);
                if (dicGiaTriTimKiem != null)
                {
                    //if (arrGiaTriTimKiem["iID_MaDonVi"] != null)
                    //    DK += String.Format("iID_MaDonVi={0}", arrGiaTriTimKiem["iID_MaDonVi"]);
                    for (int i = 0; i < arrDSTruong.Length; i++)
                    {
                        if (String.IsNullOrEmpty(dicGiaTriTimKiem[arrDSTruong[i]]) == false)
                        {
                            dk += String.Format(" AND {0} LIKE @{0}", arrDSTruong[i]);
                            cmd.Parameters.AddWithValue("@" + arrDSTruong[i], "%" + dicGiaTriTimKiem[arrDSTruong[i]] + "%");
                        }
                    }
                }
                if (dt.Rows.Count > 0)
                    dk += " AND( ";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    String Nguon = Convert.ToString(dt.Rows[i]["sLNS"]).Substring(0, 1);
                    String LoaiNS = Convert.ToString(dt.Rows[i]["sLNS"]).Substring(0, 3);
                    dk += "  (sLNS=@sLNS" + i + " OR (SUBSTRING(sLNS,1,1)=" + Nguon + " AND LEN(sLNS)=1) OR  (SUBSTRING(sLNS,1,3)=" + LoaiNS + " AND LEN(sLNS)=3))";
                    if (i < dt.Rows.Count - 1)
                        dk += " OR ";
                    cmd.Parameters.AddWithValue("@sLNS" + i, dt.Rows[i]["sLNS"]);
                }
                if (dt.Rows.Count > 0)
                    dk += " ) ";
                sql = String.Format("SELECT iID_MaMucLucNganSach,iID_MaMucLucNganSach_Cha,bLaHangCha,sXauNoiMa,{0},{1} FROM NS_MucLucNganSach WHERE iNamLamViec = @iNamLamViec AND {2} ORDER BY {3}", MucLucNganSachModels.strDSTruong, MucLucNganSachModels.strDSDuocNhapTruongTien, dk, MucLucNganSachModels.strDSTruongSapXep);
                cmd.CommandText = sql;
                string iNamLamViec = ReportModels.LayNamLamViec(System.Web.HttpContext.Current.User.Identity.Name);
                //if (Convert.ToInt32(iNamLamViec) <= 2017) iNamLamViec = "2017";
                //else iNamLamViec = "2018";
                cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                vR = Connection.GetDataTable(cmd);
                cmd.Dispose();
                //Lay toan bo Muc luc ngan sach-->
                dk = "iTrangThai=1 " + dkiID_MaChungTu;
                for (int i = 0; i < arriID_MaChungTu.Length; i++)
                {
                    cmd.Parameters.AddWithValue("@iID_MaChungTu" + i, arriID_MaChungTu[i]);
                }

                if (dicGiaTriTimKiem != null)
                {
                    if (String.IsNullOrEmpty(dicGiaTriTimKiem["iID_MaDonVi"]) == false)
                    {
                        dk += String.Format(" AND {0} LIKE @{0}", "iID_MaDonVi");
                        cmd.Parameters.AddWithValue("@" + "iID_MaDonVi", "%" + dicGiaTriTimKiem["iID_MaDonVi"] + "%");
                    }
                    if (String.IsNullOrEmpty(dicGiaTriTimKiem["iID_MaPhongBanDich"]) == false)
                    {
                        dk += String.Format(" AND {0} LIKE @{0}", "iID_MaPhongBanDich");
                        cmd.Parameters.AddWithValue("@" + "iID_MaPhongBanDich", "%" + dicGiaTriTimKiem["iID_MaPhongBanDich"] + "%");
                    }
                    for (int i = 0; i < arrDSTruong.Length; i++)
                    {
                        if (String.IsNullOrEmpty(dicGiaTriTimKiem[arrDSTruong[i]]) == false)
                        {
                            dk += String.Format(" AND {0} LIKE @{0}", arrDSTruong[i]);

                        }
                    }
                }
                dk += " AND (";
                for (int i = 1; i < arrDSTruongTien.Length - 8; i++)
                {
                    dk += arrDSTruongTien[i] + "<>0 OR ";
                }
                dk = dk.Substring(0, dk.Length - 3);
                dk += ") ";

                sql = String.Format("SELECT *,sTenDonVi as sTenDonVi_BaoDam FROM DT_ChungTuChiTiet_PhanCap WHERE {0} ORDER BY sXauNoiMa,iID_MaDonVi", dk);
                cmd.CommandText = sql;

                DataTable dtChungTuChiTiet = Connection.GetDataTable(cmd);
                int cs0 = 0;
                DataColumn column;

                for (int j = 0; j < arrDSTruongTien.Length; j++)
                {
                    column = new DataColumn(arrDSTruongTien[j], typeof(String));
                    column.AllowDBNull = true;
                    vR.Columns.Add(column);
                }
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
                                for (int k = 0; k < vR.Columns.Count - 1; k++)
                                {
                                    if (vR.Columns[k].ColumnName.StartsWith("b") == false || vR.Columns[k].ColumnName == "bLaHangCha")
                                        vR.Rows[i][k] = dtChungTuChiTiet.Rows[j][vR.Columns[k].ColumnName];
                                    else
                                        vR.Rows[i][k] = vR.Rows[i][k];
                                }
                                count++;
                            }
                            else
                            {
                                DataRow row = vR.NewRow();
                                for (int k = 0; k < vR.Columns.Count - 1; k++)
                                {
                                    if (vR.Columns[k].ColumnName.StartsWith("b") == false || vR.Columns[k].ColumnName == "bLaHangCha")
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
            }
            #endregion

            vR.Columns.Add("bPhanCap", typeof(Boolean));
            return vR;
        }
    }
}
