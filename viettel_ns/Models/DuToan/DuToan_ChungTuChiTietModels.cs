using DomainModel;
using DomainModel.Abstract;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Viettel.Data;
using Viettel.Services;

namespace VIETTEL.Models
{
    public partial class DuToan_ChungTuChiTietModels
    {
        public const String sLNSBaoDam = "1040100";

        public const int iID_MaTrangThaiDuyetKT = 106;

        public static DataTable Getdata(String MaChungTu, String sOrder = "", String sLNS = "", int Trang = 1, int SoBanGhi = 0)
        {
            DataTable dt;
            String SQL = "SELECT * FROM DT_ChungTuChiTiet";
            String DK = "";
            String SapXep;
            if (sOrder != null && sOrder != "")
            {
                SapXep = sOrder;
            }
            else
            {
                SapXep = "dNgayTao DESC";
            }
            SqlCommand cmd = new SqlCommand();
            if (String.IsNullOrEmpty(MaChungTu) == false)
            {
                if (DK != "") DK += " AND ";
                DK += "iID_MaChungTu=@iID_MaChungTu";
                cmd.Parameters.AddWithValue("@iID_MaChungTu", MaChungTu);
            }
            if (String.IsNullOrEmpty(sLNS) == false)
            {
                if (DK != "") DK += " AND ";
                DK += "sLNS=@sLNS";
                cmd.Parameters.AddWithValue("@sLNS", sLNS);
            }
            if (String.IsNullOrEmpty(DK) == false)
            {
                DK = " WHERE " + DK;
            }
            SQL = SQL + DK;
            cmd.CommandText = SQL;
            dt = CommonFunction.dtData(cmd, SapXep, Trang, SoBanGhi);
            cmd.Dispose();
            return dt;
        }
        //Lấy dt chi tiết ngân sách
        public static DataTable Getdata(String MaChungTu,
                                    String sOrder = "",
                                    String sLNS = "",
                                    String sM = "",
                                    String sTM = "",
                                    String sTTM = "",
                                    String sNG = "",
                                    String sTNG = "",
                                    String iID_MaTrangThaiDuyet = "",
                                    int Trang = 1, int SoBanGhi = 0)
        {
            DataTable dt;
            String SQL = "SELECT * FROM DT_ChungTuChiTiet";
            String DK = "";
            String SapXep;
            if (sOrder != null && sOrder != "")
            {
                SapXep = sOrder;
            }
            else
            {
                SapXep = "dNgayTao DESC";
            }
            SqlCommand cmd = new SqlCommand();
            if (String.IsNullOrEmpty(MaChungTu) == false)
            {
                if (DK != "") DK += " AND ";
                DK += "iID_MaChungTu=@iID_MaChungTu";
                cmd.Parameters.AddWithValue("@iID_MaChungTu", MaChungTu);
            }
            if (String.IsNullOrEmpty(sLNS) == false)
            {
                if (DK != "") DK += " AND ";
                DK += "sLNS=@sLNS";
                cmd.Parameters.AddWithValue("@sLNS", sLNS);
            }
            if (String.IsNullOrEmpty(sM) == false)
            {
                if (DK != "") DK += " AND ";
                DK += "sM=@sM";
                cmd.Parameters.AddWithValue("@sM", sM);
            }
            if (String.IsNullOrEmpty(sTM) == false)
            {
                if (DK != "") DK += " AND ";
                DK += "sTM=@sTM";
                cmd.Parameters.AddWithValue("@sTM", sTM);
            }
            if (String.IsNullOrEmpty(sTTM) == false)
            {
                if (DK != "") DK += " AND ";
                DK += "sTTM=@sTTM";
                cmd.Parameters.AddWithValue("@sTTM", sTTM);
            }
            if (String.IsNullOrEmpty(sNG) == false)
            {
                if (DK != "") DK += " AND ";
                DK += "sNG=@sNG";
                cmd.Parameters.AddWithValue("@sNG", sNG);
            }
            if (String.IsNullOrEmpty(sNG) == false)
            {
                if (DK != "") DK += " AND ";
                DK += "sTNG=@sTNG";
                cmd.Parameters.AddWithValue("@sTNG", sTNG);
            }
            if (CommonFunction.IsNumeric(iID_MaTrangThaiDuyet))
            {
                if (DK != "") DK += " AND ";
                DK += "iID_MaTrangThaiDuyet=@iID_MaTrangThaiDuyet";
                cmd.Parameters.AddWithValue("@iID_MaTrangThaiDuyet", iID_MaTrangThaiDuyet);
            }
            if (String.IsNullOrEmpty(DK) == false)
            {
                DK = " WHERE " + DK;
            }
            SQL = SQL + DK;
            cmd.CommandText = SQL;
            dt = CommonFunction.dtData(cmd, SapXep, Trang, SoBanGhi);
            cmd.Dispose();
            return dt;
        }

        public static DataTable GetChungTuChiTiet_LichSu(String iID_MaChungTu)
        {
            DataTable vR;
            String SQL;
            SqlCommand cmd;
            SQL = "SELECT TOP 10 * FROM DT_ChungTuChiTiet WHERE iID_MaChungTu=@iID_MaChungTu ORDER BY dNgaySua DESC";
            cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@iID_MaChungTu", iID_MaChungTu);
            vR = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return vR;
        }
        public static DataTable GetChungTu(String iID_MaChungTu)
        {
            DataTable vR;
            SqlCommand cmd = new SqlCommand("SELECT * FROM DT_ChungTu WHERE iTrangThai <> 0 AND iID_MaChungTu=@iID_MaChungTu");
            cmd.Parameters.AddWithValue("@iID_MaChungTu", iID_MaChungTu);
            vR = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return vR;
        }
        public static DataTable GetChungTu_Gom(String iID_MaChungTu)
        {
            DataTable vR;
            SqlCommand cmd = new SqlCommand("SELECT * FROM DT_ChungTu_TLTH WHERE iTrangThai=1 AND iID_MaChungTu_TLTH=@iID_MaChungTu");
            cmd.Parameters.AddWithValue("@iID_MaChungTu", iID_MaChungTu);
            vR = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return vR;
        }
        public static DataTable GetChungTuChiTiet(String iID_MaChungTu, Dictionary<String, String> arrGiaTriTimKiem, String MaND, String sLNS)
        {

            DataTable vR, dtChungTu;
            String SQL, DK = "", Dk1 = "";
            SqlCommand cmd;
            DataTable dt = NganSach_HamChungModels.DSLNSCuaPhongBan(MaND);
            DataTable arrDV = NganSach_HamChungModels.DSDonViCuaNguoiDung(MaND);

            dtChungTu = GetChungTu(iID_MaChungTu);
            DataRow RChungTu = dtChungTu.Rows[0];

            String[] arrLNS = RChungTu["sDSLNS"].ToString().Split(',');
            String DKLNS = "";
            cmd = new SqlCommand();
            for (int i = 0; i < arrLNS.Length; i++)
            {
                DKLNS += " sLNS LIKE @LNS" + i;
                if (i < arrLNS.Length - 1)
                    DKLNS += " OR ";
                cmd.Parameters.AddWithValue("LNS" + i, arrLNS[i] + "%");
            }
            if (String.IsNullOrEmpty(DKLNS)) DKLNS = "0=1";
            DK = String.Format("iTrangThai = 1 AND ({0}) ", DKLNS);

            String sTruongTien = MucLucNganSachModels.strDSTruongTien + ",iID_MaChungTuChiTiet,iID_MaDonVi,sTenDonVi,sTenDonVi_BaoDam,iID_MaPhongBanDich,iID_MaPhongBan,sGhiChu,sMaCongTrinh";
            String[] arrDSTruong = MucLucNganSachModels.strDSTruong.Split(',');
            String[] arrDSTruong_TK = (MucLucNganSachModels.strDSTruong + ",iID_MaDonVi").Split(',');
            String[] arrDSTruongTien = sTruongTien.Split(',');
            //<--Lay toan bo Muc luc ngan sach
            if (arrGiaTriTimKiem != null)
            {
                for (int i = 0; i < arrDSTruong.Length; i++)
                {
                    if (String.IsNullOrEmpty(arrGiaTriTimKiem[arrDSTruong[i]]) == false)
                    {
                        if (arrDSTruong[i] == "sLNS")
                        {
                            if (!String.IsNullOrEmpty(sLNS))
                            {
                                String[] arrLNSTK = sLNS.Split(',');
                                DK += " AND( ";
                                for (int j = 0; j < arrLNSTK.Length; j++)
                                {
                                    DK += " sLNS LIKE @sLNSTK" + j;
                                    cmd.Parameters.AddWithValue("@sLNSTK" + j, "%" + arrLNSTK[j] + "%");
                                    if (j < arrLNSTK.Length - 1)
                                        DK += " OR ";
                                }
                                DK += ")";
                            }
                        }
                        else
                        {
                            DK += String.Format(" AND {0} LIKE @{0}", arrDSTruong[i]);
                            cmd.Parameters.AddWithValue("@" + arrDSTruong[i], "%" + arrGiaTriTimKiem[arrDSTruong[i]] + "%");
                        }
                    }
                }
            }
            if (dt.Rows.Count > 0)
                DK += " AND( ";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                String Nguon = Convert.ToString(dt.Rows[i]["sLNS"]).Substring(0, 1);
                String LoaiNS = Convert.ToString(dt.Rows[i]["sLNS"]).Substring(0, 3);
                DK += "  (sLNS=@sLNS" + i + " OR (SUBSTRING(sLNS,1,1)=" + Nguon + " AND LEN(sLNS)=1) OR  (SUBSTRING(sLNS,1,3)=" + LoaiNS + " AND LEN(sLNS)=3))";
                if (i < dt.Rows.Count - 1)
                    DK += " OR ";
                cmd.Parameters.AddWithValue("@sLNS" + i, dt.Rows[i]["sLNS"]);
            }
            if (dt.Rows.Count > 0)
                DK += " ) ";
            SQL = String.Format("SELECT iID_MaMucLucNganSach,iID_MaMucLucNganSach_Cha,bLaHangCha,sXauNoiMa,{0},{1} FROM NS_MucLucNganSach WHERE iNamLamViec=@iNamLamViec AND {2}  ORDER BY {3}", MucLucNganSachModels.strDSTruong, MucLucNganSachModels.strDSDuocNhapTruongTien.Replace("bChiTaiNganh","bChiTaiNganh=1"), DK, MucLucNganSachModels.strDSTruongSapXep);
            int iNamLamViec = Convert.ToInt32(ReportModels.LayNamLamViec(MaND));
            cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
            cmd.CommandText = SQL;
            vR = Connection.GetDataTable(cmd);
            cmd.Dispose();
            //Lay toan bo Muc luc ngan sach-->
            cmd = new SqlCommand();
            DK = "iTrangThai in (1,2,3) and iID_MaChungTu=@iID_MaChungTu";
            cmd.Parameters.AddWithValue("@iID_MaChungTu", iID_MaChungTu);

            if (arrGiaTriTimKiem != null)
            {
                if (String.IsNullOrEmpty(arrGiaTriTimKiem["iID_MaDonVi"]) == false)
                {
                    DK += String.Format(" AND {0} LIKE @{0}", "iID_MaDonVi");
                    cmd.Parameters.AddWithValue("@" + "iID_MaDonVi", "%" + arrGiaTriTimKiem["iID_MaDonVi"] + "%");
                }
                for (int i = 0; i < arrDSTruong.Length; i++)
                {
                    if (String.IsNullOrEmpty(arrGiaTriTimKiem[arrDSTruong[i]]) == false)
                    {
                        if (arrDSTruong[i] == "sLNS")
                        {
                            if (!String.IsNullOrEmpty(sLNS))
                            {
                                String[] arrLNSTK = sLNS.Split(',');
                                DK += " AND( ";
                                for (int j = 0; j < arrLNSTK.Length; j++)
                                {
                                    DK += " sLNS LIKE @sLNSTK" + j;
                                    cmd.Parameters.AddWithValue("@sLNSTK" + j, "%" + arrLNSTK[j] + "%");
                                    if (j < arrLNSTK.Length - 1)
                                        DK += " OR ";
                                }
                                DK += ")";
                            }
                        }
                        else
                        {
                            DK += String.Format(" AND {0} LIKE @{0}", arrDSTruong[i]);
                            cmd.Parameters.AddWithValue("@" + arrDSTruong[i], "%" + arrGiaTriTimKiem[arrDSTruong[i]] + "%");
                        }
                    }
                }
            }
            DK += " AND (";
            for (int i = 1; i < arrDSTruongTien.Length - 8; i++)
            {
                DK += arrDSTruongTien[i] + "<>0 OR ";
            }
            DK = DK.Substring(0, DK.Length - 3);
            DK += " or sGhiChu is not null) ";
            
            SQL = String.Format("SELECT *,sTenDonVi as sTenDonVi_BaoDam FROM DT_ChungTuChiTiet WHERE {0} ORDER BY sXauNoiMa,iID_MaDonVi", DK);
            cmd.CommandText = SQL;

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
                            for (int k = 0; k < vR.Columns.Count - 1; k++)
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
            if (RChungTu["iTrangThai"].ToString() == "3" && (RChungTu["sDSLNS"].ToString().Contains("1020100") || RChungTu["sDSLNS"].ToString().StartsWith("2")))
            {
                var sql = "sp_dt_chungtuct_nganh_nsqp";
                if (RChungTu["sDSLNS"].ToString().StartsWith("2"))
                {
                    sql = "sp_dt_chungtuct_nganh_nsnn";
                }
                String iID_MaDonVi_TK = arrGiaTriTimKiem["iID_MaDonVi"];
                int count1 = vR.Rows.Count;

                if (!String.IsNullOrEmpty(RChungTu["iID_MaDonVi"].ToString()))
                {
                    for (int i = arrDV.Rows.Count - 1; i >= 0; i--)
                    {
                        if (Convert.ToString(arrDV.Rows[i]["iID_MaDonVi"]) != RChungTu["iID_MaDonVi"].ToString())
                            arrDV.Rows.RemoveAt(i);
                    }                   
                }
                else
                {
                    if (iID_MaDonVi_TK != null && iID_MaDonVi_TK != "")
                    {
                        for (int i = arrDV.Rows.Count - 1; i >= 0; i--)
                        {
                            if (Convert.ToString(arrDV.Rows[i]["iID_MaDonVi"]) != iID_MaDonVi_TK)
                                arrDV.Rows.RemoveAt(i);
                        }
                    }
                }

                var ds_dv = "";
                for (int i = 0; i < arrDV.Rows.Count; i++)
                {
                    if (ds_dv == "")
                    {
                        ds_dv = "'" + arrDV.Rows[i]["iID_MaDonVi"].ToString() + "'";
                    }
                    else
                    {
                        ds_dv += ",'" + arrDV.Rows[i]["iID_MaDonVi"].ToString() + "'";
                    }
                }
                ds_dv = "iID_MaDonVi in (" + ds_dv + ")";                

                column = new DataColumn("rDonViDN", typeof(String));
                column.AllowDBNull = true;
                vR.Columns.Add(column);
                column = new DataColumn("rTongCong", typeof(String));
                column.AllowDBNull = true;
                vR.Columns.Add(column);

                var dtDonviChiTiet = new DataTable();

                using (var conn = ConnectionFactory.Default.GetConnection())
                using (var cmdvr = new SqlCommand(sql, conn))
                {
                    cmdvr.CommandType = CommandType.StoredProcedure;
                    cmdvr.Parameters.AddWithValue("@id", RChungTu["iID_MaChungTu"].ToString().ToParamString());

                    dtDonviChiTiet = cmdvr.GetDataset().Tables[0];
                }

                dtDonviChiTiet = dtDonviChiTiet.Select(
                    ds_dv).Count() > 0 ? dtDonviChiTiet.Select(
                    ds_dv).CopyToDataTable() : dtDonviChiTiet.Clone();
                var view = dtDonviChiTiet.DefaultView;
                view.Sort = "sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi";
                dtDonviChiTiet = view.ToTable();
                               
                int dCount = vR.Rows.Count;
                cs0 = 0;
                for (int i = 0; i < dCount; i++)
                {
                    var dtr = vR.Rows[i];
                    if (Convert.ToBoolean(dtr["bLaHangCha"]) == false) {
                        dtr["rTongCong"] = Convert.IsDBNull(vR.Rows[i]["rTuChi"]) ? 0 : Convert.ToDecimal(vR.Rows[i]["rTuChi"]);
                        for (int j = cs0; j < dtDonviChiTiet.Rows.Count; j++)
                        {                        
                            bool ok = true;
                            var rdtr = dtDonviChiTiet.Rows[j];
                            for (int k = 0; k < arrDSTruong.Length; k++)
                            {
                                var cot = arrDSTruong[k];
                                if (Convert.ToString(dtr[cot]) != Convert.ToString(rdtr[cot]))
                                {
                                    ok = false;
                                    break;
                                }
                            }
                            if (ok)
                            {
                                var dcheck = vR.AsEnumerable().Where(x => x.Field<string>("sXauNoiMa") == dtr["sXauNoiMa"].ToString() && x.Field<string>("iID_MaDonVi") == rdtr["iID_MaDonVi"].ToString());
                                var bcount = dcheck == null || !dcheck.Any();
                                if (dtr["iID_MaDonVi"].ToString() == rdtr["iID_MaDonVi"].ToString())
                                {
                                    dtr["rDonViDN"] = rdtr["rDonViDN"];
                                    dtr["rTongCong"] = (Convert.IsDBNull(dtr["rTuChi"]) ? 0 : Convert.ToDecimal(dtr["rTuChi"])) + Convert.ToDecimal(rdtr["rDonViDN"]);
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(dtr["iID_MaDonVi"].ToString()))
                                    {                                    
                                        dtr["iID_MaDonVi"] = rdtr["iID_MaDonVi"];
                                        dtr["sTenDonVi"] = rdtr["sTen"];
                                        dtr["sTenDonVi_BaoDam"] = rdtr["sTen"];
                                        dtr["iID_MaPhongBan"] = RChungTu["iID_MaPhongBan"];
                                        dtr["iID_MaPhongBanDich"] = RChungTu["iID_MaPhongBanDich"];
                                        dtr["rDonViDN"] = rdtr["rDonViDN"];
                                        dtr["rTuChi"] = 0;
                                        dtr["rTongCong"] = rdtr["rDonViDN"];
                                    }
                                    else {
                                        if (bcount)
                                        { 
                                            DataRow row = vR.NewRow();
                                            row.ItemArray = dtr.ItemArray;
                                            row["iID_MaChungTuChiTiet"] = "";
                                            row["iID_MaDonVi"] = rdtr["iID_MaDonVi"];
                                            row["sTenDonVi"] = rdtr["sTen"];
                                            row["sTenDonVi_BaoDam"] = rdtr["sTen"];
                                            row["iID_MaPhongBan"] = RChungTu["iID_MaPhongBan"];
                                            row["iID_MaPhongBanDich"] = RChungTu["iID_MaPhongBanDich"];
                                            row["rDonViDN"] = rdtr["rDonViDN"];
                                            row["rTuChi"] = 0;
                                            row["rTongCong"] = rdtr["rDonViDN"];
                                            row["sGhiChu"] = null;
                                            vR.Rows.InsertAt(row, i + 1);
                                            i++;
                                            dCount++;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                                cs0 = j + 1;
                            }
                        }
                    }
                }
                dtDonviChiTiet.Dispose();
            }
            else if (RChungTu["sDSLNS"].ToString().StartsWith("2") && RChungTu["iTrangThai"].ToString() == "2")
            {
                String iID_MaDonVi_TK = arrGiaTriTimKiem["iID_MaDonVi"];
                int count1 = vR.Rows.Count;

                if (!String.IsNullOrEmpty(RChungTu["iID_MaDonVi"].ToString()))
                {
                    for (int i = arrDV.Rows.Count - 1; i >= 0; i--)
                    {
                        if (Convert.ToString(arrDV.Rows[i]["iID_MaDonVi"]) != RChungTu["iID_MaDonVi"].ToString())
                            arrDV.Rows.RemoveAt(i);
                    }
                }
                else
                {
                    if (iID_MaDonVi_TK != null && iID_MaDonVi_TK != "")
                    {
                        for (int i = arrDV.Rows.Count - 1; i >= 0; i--)
                        {
                            if (Convert.ToString(arrDV.Rows[i]["iID_MaDonVi"]) != iID_MaDonVi_TK)
                                arrDV.Rows.RemoveAt(i);
                        }
                    }
                }

                var ds_dv = "";
                for (int i = 0; i < arrDV.Rows.Count; i++)
                {
                    if (ds_dv == "")
                    {
                        ds_dv = "'" + arrDV.Rows[i]["iID_MaDonVi"].ToString() + "'";
                    }
                    else
                    {
                        ds_dv += ",'" + arrDV.Rows[i]["iID_MaDonVi"].ToString() + "'";
                    }
                }
                ds_dv = "iID_MaDonVi in (" + ds_dv + ")";

                column = new DataColumn("rTuChi_NT", typeof(String));
                column.AllowDBNull = true;
                vR.Columns.Add(column);

                var dtDonviChiTiet = new DataTable();

                using (var conn = ConnectionFactory.Default.GetConnection())
                using (var cmdvr = new SqlCommand("sp_dutoan_chungtuct_nt", conn))
                {
                    cmdvr.CommandType = CommandType.StoredProcedure;
                    cmdvr.Parameters.AddWithValue("@id", RChungTu["iID_MaChungTu"].ToString().ToParamString());

                    dtDonviChiTiet = cmdvr.GetDataset().Tables[0];
                }

                dtDonviChiTiet = dtDonviChiTiet.Select(
                    ds_dv).Count() > 0 ? dtDonviChiTiet.Select(
                    ds_dv).CopyToDataTable() : dtDonviChiTiet.Clone();
                var view = dtDonviChiTiet.DefaultView;
                view.Sort = "sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi";
                dtDonviChiTiet = view.ToTable();

                int dCount = vR.Rows.Count;
                cs0 = 0;
                for (int i = 0; i < dCount; i++)
                {
                    var dtr = vR.Rows[i];
                    if (Convert.ToBoolean(dtr["bLaHangCha"]) == false)
                    {                        
                        for (int j = cs0; j < dtDonviChiTiet.Rows.Count; j++)
                        {
                            bool ok = true;
                            var rdtr = dtDonviChiTiet.Rows[j];
                            for (int k = 0; k < arrDSTruong.Length - 1; k++)
                            {
                                var cot = arrDSTruong[k];
                                if (Convert.ToString(dtr[cot]) != Convert.ToString(rdtr[cot]))
                                {
                                    ok = false;
                                    break;
                                }
                            }
                            if (ok)
                            {
                                var dcheck = vR.AsEnumerable().Where(x => x.Field<string>("sXauNoiMa") == dtr["sXauNoiMa"].ToString() && x.Field<string>("iID_MaDonVi") == rdtr["iID_MaDonVi"].ToString());
                                var bcount = dcheck == null || !dcheck.Any();
                                if (dtr["iID_MaDonVi"].ToString() == rdtr["iID_MaDonVi"].ToString())
                                {
                                    dtr["rTuChi_NT"] = rdtr["rTuChi_NT"];
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(dtr["iID_MaDonVi"].ToString()))
                                    {
                                        dtr["iID_MaDonVi"] = rdtr["iID_MaDonVi"];
                                        dtr["sTenDonVi"] = rdtr["sTen"];
                                        dtr["sTenDonVi_BaoDam"] = rdtr["sTen"];
                                        dtr["iID_MaPhongBan"] = RChungTu["iID_MaPhongBan"];
                                        dtr["iID_MaPhongBanDich"] = RChungTu["iID_MaPhongBanDich"];
                                        dtr["rTuChi_NT"] = rdtr["rTuChi_NT"];
                                        dtr["rTuChi"] = 0;
                                    }
                                    else
                                    {
                                        if (bcount)
                                        {
                                            DataRow row = vR.NewRow();
                                            row.ItemArray = dtr.ItemArray;
                                            row["iID_MaChungTuChiTiet"] = "";
                                            row["iID_MaDonVi"] = rdtr["iID_MaDonVi"];
                                            row["sTenDonVi"] = rdtr["sTen"];
                                            row["sTenDonVi_BaoDam"] = rdtr["sTen"];
                                            row["iID_MaPhongBan"] = RChungTu["iID_MaPhongBan"];
                                            row["iID_MaPhongBanDich"] = RChungTu["iID_MaPhongBanDich"];
                                            row["rTuChi_NT"] = rdtr["rTuChi_NT"];
                                            row["rTuChi"] = 0;
                                            row["sGhiChu"] = null;
                                            vR.Rows.InsertAt(row, i + 1);
                                            i++;
                                            dCount++;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                                cs0 = j + 1;
                            }
                        }
                    }
                }
                dtDonviChiTiet.Dispose();
            }

            var viewvr = vR.DefaultView;
            viewvr.Sort = "sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi";
            vR = viewvr.ToTable();

            vR.Columns.Add("bPhanCap", typeof(Boolean));
            if (arrLNS.Length > 0)
            {
                //neu la bao dam lay danh sach theo cau hinh nguoi dung-nganh
                //neu la tro ly phong ban
                bool bTrolyTHCuc = LuongCongViecModel.KiemTra_TroLyPB11_02(MaND);
                if (bTrolyTHCuc == false && NguoiDung_PhongBanModels.getTenPhongBan_NguoiDung(MaND) != "02")
                {
                    if (arrLNS[0].Equals("1040100"))
                    {
                        DK = "";
                        DK += " AND sMaNguoiQuanLy like '%" + MaND + "%'";
                        SQL = String.Format(@"SELECT iID_MaNganhMLNS FROM NS_MucLucNganSach_Nganh
            WHERE iTrangThai=1 and iNamLamViec = @iNamLamViec {0}", DK);
                        SqlCommand cmdBD = new SqlCommand();
                        cmdBD.CommandText = SQL;
                        cmdBD.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);

                        dt = Connection.GetDataTable(cmdBD);
                        String DKMaNganh = "";
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            DKMaNganh += dt.Rows[i]["iID_MaNganhMLNS"];
                            if (i < dt.Rows.Count - 1)
                                DKMaNganh += ",";
                        }
                        String[] arrMaNganh = DKMaNganh.Split(',');

                        //Xoa cac nganh khong phan quyen
                        for (int i = vR.Rows.Count - 1; i >= 0; i--)
                        {
                            bool bXoa = true;
                            for (int j = 0; j < arrMaNganh.Length; j++)
                            {
                                if (Convert.ToString(vR.Rows[i]["sNG"]) == arrMaNganh[j] || Convert.ToBoolean(vR.Rows[i]["bLaHangCha"]) == true)
                                {
                                    bXoa = false;
                                    break;
                                }
                            }
                            if (bXoa)
                            {
                                vR.Rows.RemoveAt(i);
                            }
                        }
                        //Xoa cac muc luc cha khong co con
                        for (int i = vR.Rows.Count - 1; i >= 0; i--)
                        {
                            bool bXoa = true;
                            if (Convert.ToBoolean(vR.Rows[i]["bLaHangCha"]))
                            {
                                String iID_MaMucLucNganSach = Convert.ToString(vR.Rows[i]["iID_MaMucLucNganSach"]);
                                for (int j = i + 1; j < vR.Rows.Count; j++)
                                {
                                    if (iID_MaMucLucNganSach == Convert.ToString(vR.Rows[j]["iID_MaMucLucNganSach_Cha"]))
                                    {
                                        bXoa = false;
                                        break;
                                    }
                                }
                                if (bXoa)
                                {
                                    vR.Rows.RemoveAt(i);
                                }
                            }
                        }
                    }
                }// end if check MaND
            }
            
            dtChungTu.Dispose();
            dtChungTuChiTiet.Dispose();
            vR.Dispose();
            cmd.Dispose();
            return vR;
        }
        public static DataTable GetChungTuChiTiet_ChiTapTrung(String iID_MaChungTu, Dictionary<String, String> arrGiaTriTimKiem, String MaND, String sLNS)
        {

            DataTable vR, dtChungTu;
            String SQL, DK = "", Dk1 = "";
            SqlCommand cmd;
            DataTable dt = NganSach_HamChungModels.DSLNSCuaPhongBan(MaND);

            dtChungTu = GetChungTu(iID_MaChungTu);
            DK = String.Format("iTrangThai=1 AND sLNS IN ({0}) ", sLNS);

            DataRow RChungTu = dtChungTu.Rows[0];

            String sTruongTien = MucLucNganSachModels.strDSTruongTien + ",iID_MaChungTuChiTiet,iID_MaDonVi,sTenDonVi,sTenDonVi_BaoDam,iID_MaPhongBanDich,iID_MaPhongBan,sGhiChu,sMaCongTrinh";
            String[] arrDSTruong = MucLucNganSachModels.strDSTruong.Split(',');
            String[] arrDSTruong_TK = (MucLucNganSachModels.strDSTruong + ",iID_MaDonVi").Split(',');
            String[] arrDSTruongTien = sTruongTien.Split(',');



            //<--Lay toan bo Muc luc ngan sach
            cmd = new SqlCommand();


            if (arrGiaTriTimKiem != null)
            {
                for (int i = 0; i < arrDSTruong.Length; i++)
                {
                    if (String.IsNullOrEmpty(arrGiaTriTimKiem[arrDSTruong[i]]) == false)
                    {
                        if (arrDSTruong[i] == "sLNS")
                        {
                            DK += String.Format(" AND sLNS IN ({0}) ", sLNS);
                        }
                        else
                        {
                            DK += String.Format(" AND {0} LIKE @{0}", arrDSTruong[i]);
                            cmd.Parameters.AddWithValue("@" + arrDSTruong[i], "%" + arrGiaTriTimKiem[arrDSTruong[i]] + "%");
                        }
                    }
                }
            }
            if (dt.Rows.Count > 0)
                DK += " AND( ";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                String Nguon = Convert.ToString(dt.Rows[i]["sLNS"]).Substring(0, 1);
                String LoaiNS = Convert.ToString(dt.Rows[i]["sLNS"]).Substring(0, 3);
                DK += "  (sLNS=@sLNS" + i + " OR (SUBSTRING(sLNS,1,1)=" + Nguon + " AND LEN(sLNS)=1) OR  (SUBSTRING(sLNS,1,3)=" + LoaiNS + " AND LEN(sLNS)=3))";
                if (i < dt.Rows.Count - 1)
                    DK += " OR ";
                cmd.Parameters.AddWithValue("@sLNS" + i, dt.Rows[i]["sLNS"]);
            }
            if (dt.Rows.Count > 0)
                DK += " ) ";
            SQL = String.Format("SELECT iID_MaMucLucNganSach,iID_MaMucLucNganSach_Cha,bLaHangCha,sXauNoiMa,{0},{1} FROM NS_MucLucNganSach WHERE iNamLamViec=@iNamLamViec AND {2}  ORDER BY {3}", MucLucNganSachModels.strDSTruong, MucLucNganSachModels.strDSDuocNhapTruongTien, DK, MucLucNganSachModels.strDSTruongSapXep);
            int iNamLamViec = Convert.ToInt32(ReportModels.LayNamLamViec(MaND));
            cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
            cmd.CommandText = SQL;
            vR = Connection.GetDataTable(cmd);
            cmd.Dispose();
            //Lay toan bo Muc luc ngan sach-->
            cmd = new SqlCommand();
            DK = "iTrangThai=1 AND iID_MaChungTu=@iID_MaChungTu";
            cmd.Parameters.AddWithValue("@iID_MaChungTu", iID_MaChungTu);


            if (arrGiaTriTimKiem != null)
            {
                if (String.IsNullOrEmpty(arrGiaTriTimKiem["iID_MaDonVi"]) == false)
                {
                    DK += String.Format(" AND {0} LIKE @{0}", "iID_MaDonVi");
                    cmd.Parameters.AddWithValue("@" + "iID_MaDonVi", "%" + arrGiaTriTimKiem["iID_MaDonVi"] + "%");
                }
                for (int i = 0; i < arrDSTruong.Length; i++)
                {
                    if (String.IsNullOrEmpty(arrGiaTriTimKiem[arrDSTruong[i]]) == false)
                    {
                        if (arrDSTruong[i] == "sLNS")
                        {
                            DK += String.Format(" AND sLNS IN ({0}) ", sLNS);
                        }
                        else
                        {
                            DK += String.Format(" AND {0} LIKE @{0}", arrDSTruong[i]);
                            cmd.Parameters.AddWithValue("@" + arrDSTruong[i], "%" + arrGiaTriTimKiem[arrDSTruong[i]] + "%");
                        }
                    }
                }
            }
            DK += " AND (";
            for (int i = 1; i < arrDSTruongTien.Length - 8; i++)
            {
                DK += arrDSTruongTien[i] + "<>0 OR ";
            }
            DK = DK.Substring(0, DK.Length - 3);
            DK += ") ";


            SQL = String.Format("SELECT *,sTenDonVi as sTenDonVi_BaoDam FROM DT_ChungTuChiTiet WHERE {0} ORDER BY sXauNoiMa,iID_MaDonVi", DK);
            cmd.CommandText = SQL;

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
                            for (int k = 0; k < vR.Columns.Count - 1; k++)
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
            SQL = String.Format(@"SELECT sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,
SUM(rTuChi) as rTuChi
FROM
(
SELECT sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi,sTenDonVi
,rTuChi=SUM(rTuChi)
 FROM DT_ChungTuChiTiet
 WHERE iTrangThai in (1,2)  AND sLNS in ('1010000','1030100') AND iID_MaDonVi =@iID_MaDonVi AND iNamLamViec=@iNamLamViec 
AND iID_MaPhongBan!='06'
 GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi,sTenDonVi
 UNION ALL
SELECT sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi,sTenDonVi
,rTuChi=SUM(rTuChi)
 FROM DT_ChungTuChiTiet
 WHERE iTrangThai in (1,2)  AND (sLNS='1020100' OR sLNS='1020000' OR sLNS='1091400' OR sLNS='1091500') 
AND iID_MaPhongBan!='06' and iID_MaPhongBanDich = @iID_MaPhongBan
 AND iID_MaDonVi =@iID_MaDonVi AND iNamLamViec=@iNamLamViec
 GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi,sTenDonVi

 UNION ALL
 SELECT sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi,sTenDonVi
,rTuChi=SUM(rTuChi)
 FROM DT_ChungTuChiTiet_PhanCap
 WHERE iTrangThai in (1,2)  AND (sLNS='1020100' OR sLNS='1020000' OR sLNS='1091400' OR sLNS='1091500') 
AND iID_MaPhongBan!='06' and iID_MaPhongBanDich = @iID_MaPhongBan
 AND iID_MaDonVi =@iID_MaDonVi AND iNamLamViec=@iNamLamViec
 GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi,sTenDonVi) a
 GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa
 HAVING SUM(rTuChi)<>0
 ORDER BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa
 ");
            cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@iNamLamViec", RChungTu["iNamLamViec"]);
            cmd.Parameters.AddWithValue("@iID_MaDonVi", RChungTu["iID_MaDonVi"]);
            cmd.Parameters.AddWithValue("@iID_MaPhongBan", RChungTu["iID_MaPhongBan"]);
            DataTable dtChiTieu = Connection.GetDataTable(cmd);

            //ghep dtChiTieu vao VR
            arrDSTruong = "sLNS,sL,sK,sM,sTM,sTM,sTTM,sNG".Split(',');
            for (int i = 0; i < vRCount; i++)
            {
                int count = 0;
                for (int j = cs0; j < dtChiTieu.Rows.Count; j++)
                {

                    Boolean ok = true;
                    for (int k = 0; k < arrDSTruong.Length; k++)
                    {
                        if (Convert.ToString(vR.Rows[i][arrDSTruong[k]]) != Convert.ToString(dtChiTieu.Rows[j][arrDSTruong[k]]))
                        {
                            ok = false;
                            break;
                        }
                    }
                    if (ok)
                    {
                        if (count == 0)
                        {

                            vR.Rows[i]["rTuChi"] = dtChiTieu.Rows[j]["rTuChi"];

                            count++;
                        }
                        else
                        {
                            DataRow row = vR.NewRow();
                            for (int k = 0; k < vR.Columns.Count - 1; k++)
                            {
                                if (vR.Columns[k].ColumnName.StartsWith("b") == false)
                                    row[k] = dtChiTieu.Rows[j][vR.Columns[k].ColumnName];
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





            vR.Columns.Add("bPhanCap", typeof(Boolean));
            dtChungTu.Dispose();
            dtChungTuChiTiet.Dispose();
            vR.Dispose();
            cmd.Dispose();
            return vR;
        }


        public static DataTable GetChungTuChiTietLan2(String iID_MaChungTu, Dictionary<String, String> arrGiaTriTimKiem, String MaND, String sLNS)
        {

            DataTable vR, dt;
            String SQL, DK = "", Dk1 = "";
            SqlCommand cmd = new SqlCommand();
            SQL = String.Format(@"SELECT sTenDonVi as sTenDonVi_BaoDam,* FROM DT_ChungTuChiTiet WHERE iID_MaChungTuChiTiet=@iID_MaChungTu");
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iID_MaChungTu", iID_MaChungTu);
            dt = Connection.GetDataTable(cmd);
            String iID_MaNganh = Convert.ToString(dt.Rows[0]["iID_MaDonVi"]);
            int iNamLamViec = Convert.ToInt32(ReportModels.LayNamLamViec(MaND));
            cmd = new SqlCommand();
            SQL = String.Format(@"SELECT sTenDonVi as sTenDonVi_BaoDam,* FROM DT_ChungTuChiTiet WHERE iID_MaChungTu=@iID_MaChungTu AND MaLoai=2");
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iID_MaChungTu", iID_MaChungTu);
            vR = Connection.GetDataTable(cmd);
            //Nếu có chua co sẽ them 1 dong với MaLoai=2
            String iID_MaPhongBan = "";
            DataTable dtPhongBan = NganSach_HamChungModels.DSBQLCuaNguoiDung(MaND);
            if (vR.Rows.Count == 0)
            {

                if (dtPhongBan != null && dtPhongBan.Rows.Count > 0)
                {
                    DataRow drPhongBan = dtPhongBan.Rows[0];
                    iID_MaPhongBan = Convert.ToString(drPhongBan["sKyHieu"]);

                    dtPhongBan.Dispose();
                }
                if (iID_MaPhongBan == "06")
                {
                    //Lay mo ta nganh 
                    DataRow r = dt.Rows[0];
                    String sXauNoiMa = "";
                    if (iNamLamViec <= 2017)
                        sXauNoiMa = "1050000-460-468-8950-8999-20-60";
                    else
                        sXauNoiMa = "1050000-010-011-6950-6999-90-60";

                    String sMoTa = Convert.ToString(CommonFunction.LayTruong("NS_MucLucNganSach", "sXauNoiMa", sXauNoiMa, "sMoTa"));
                    Bang bang = new Bang("DT_ChungTuChiTiet");
                    bang.DuLieuMoi = true;
                    bang.CmdParams.Parameters.AddWithValue("@iID_MaChungTu", r["iID_MaChungTuChiTiet"]);
                    bang.CmdParams.Parameters.AddWithValue("@iID_MaPhongBan", r["iID_MaPhongBan"]);
                    bang.CmdParams.Parameters.AddWithValue("@sTenPhongBan", r["sTenPhongBan"]);
                    bang.CmdParams.Parameters.AddWithValue("@iNamLamViec", r["iNamLamViec"]);
                    bang.CmdParams.Parameters.AddWithValue("@iID_MaNguonNganSach", r["iID_MaNguonNganSach"]);
                    bang.CmdParams.Parameters.AddWithValue("@iID_MaNamNganSach", r["iID_MaNamNganSach"]);
                    bang.CmdParams.Parameters.AddWithValue("@bChiNganSach", r["bChiNganSach"]);
                    bang.CmdParams.Parameters.AddWithValue("@iID_MaTrangThaiDuyet", r["iID_MaTrangThaiDuyet"]);
                    bang.CmdParams.Parameters.AddWithValue("@iKyThuat", r["iKyThuat"]);
                    bang.CmdParams.Parameters.AddWithValue("@MaLoai", 2);
                    bang.CmdParams.Parameters.AddWithValue("@iID_MaDonVi", r["iID_MaDonVi"]);
                    bang.CmdParams.Parameters.AddWithValue("@sTenDonVi", r["sTenDonVi"]);
                    bang.CmdParams.Parameters.AddWithValue("@sGhiChu", r["sGhiChu"]);
                    bang.CmdParams.Parameters.AddWithValue("@iID_MaPhongBanDich", r["iID_MaPhongBanDich"]);
                    bang.CmdParams.Parameters.AddWithValue("@iID_MaMucLucNganSach", r["iID_MaMucLucNganSach"]);
                    bang.CmdParams.Parameters.AddWithValue("@iID_MaMucLucNganSach_Cha", r["iID_MaMucLucNganSach_Cha"]);
                    bang.CmdParams.Parameters.AddWithValue("@sXauNoiMa", sXauNoiMa);
                    bang.CmdParams.Parameters.AddWithValue("@sLNS", "1050000");
                    if (iNamLamViec <= 2017)
                    {
                        bang.CmdParams.Parameters.AddWithValue("@sL", "460");
                        bang.CmdParams.Parameters.AddWithValue("@sK", "468");
                    }
                    else
                    {
                        bang.CmdParams.Parameters.AddWithValue("@sL", "010");
                        bang.CmdParams.Parameters.AddWithValue("@sK", "011");
                    }
                    bang.CmdParams.Parameters.AddWithValue("@sM", "6950");
                    bang.CmdParams.Parameters.AddWithValue("@sTM", "6999");
                    bang.CmdParams.Parameters.AddWithValue("@sTTM", "20");
                    bang.CmdParams.Parameters.AddWithValue("@sNG", "60");
                    bang.CmdParams.Parameters.AddWithValue("@sTNG", r["sTNG"]);
                    bang.CmdParams.Parameters.AddWithValue("@sMoTa", sMoTa);
                    bang.CmdParams.Parameters.AddWithValue("@bsMaCongTrinh", false);
                    bang.CmdParams.Parameters.AddWithValue("@bsTenCongTrinh", false);
                    bang.CmdParams.Parameters.AddWithValue("@brNgay", false);
                    bang.CmdParams.Parameters.AddWithValue("@brSoNguoi", false);
                    bang.CmdParams.Parameters.AddWithValue("@brChiTaiKhoBac", false);
                    bang.CmdParams.Parameters.AddWithValue("@brChiTapTrung", false);
                    // bang.Save();
                    r = dt.Rows[0];
                    sMoTa = Convert.ToString(CommonFunction.LayTruong("NS_MucLucNganSach", "sXauNoiMa", sXauNoiMa, "sMoTa"));                   
                    bang.Save();
                }
                else
                {
                    String iID_MaNganh_MLNS = "";
                    SQL = String.Format("SELECT TOP 1 iID_MaNganhMLNS FROM NS_MucLucNganSach_Nganh WHERE iTrangThai=1 AND iNamLamViec = @iNamLamViec AND iID_MaNganh=@iID_MaNganh");
                    cmd = new SqlCommand(SQL);
                    cmd.Parameters.AddWithValue("@iID_MaNganh", iID_MaNganh);
                    cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                    iID_MaNganh_MLNS = Connection.GetValueString(cmd, "");
                    String[] arrMaNganh = iID_MaNganh_MLNS.Split(',');

                    for (int i = 0; i < arrMaNganh.Length; i++)
                    {
                        //Lay mo ta nganh 
                        DataRow r = dt.Rows[0];
                        String sXauNoiMa = r["sLNS"] + "-" + r["sL"] + "-" + r["sK"] + "-" + r["sM"] + "-" + r["sTM"] + "-" + r["sTTM"] + "-" + arrMaNganh[i];
                        String sMoTa = Convert.ToString(CommonFunction.LayTruong("NS_MucLucNganSach", "sXauNoiMa", sXauNoiMa, "sMoTa"));
                        Bang bang = new Bang("DT_ChungTuChiTiet");
                        bang.DuLieuMoi = true;
                        bang.CmdParams.Parameters.AddWithValue("@iID_MaChungTu", r["iID_MaChungTuChiTiet"]);
                        bang.CmdParams.Parameters.AddWithValue("@iID_MaPhongBan", r["iID_MaPhongBan"]);
                        bang.CmdParams.Parameters.AddWithValue("@sTenPhongBan", r["sTenPhongBan"]);
                        bang.CmdParams.Parameters.AddWithValue("@iNamLamViec", r["iNamLamViec"]);
                        bang.CmdParams.Parameters.AddWithValue("@iID_MaNguonNganSach", r["iID_MaNguonNganSach"]);
                        bang.CmdParams.Parameters.AddWithValue("@iID_MaNamNganSach", r["iID_MaNamNganSach"]);
                        bang.CmdParams.Parameters.AddWithValue("@bChiNganSach", r["bChiNganSach"]);
                        bang.CmdParams.Parameters.AddWithValue("@iID_MaTrangThaiDuyet", r["iID_MaTrangThaiDuyet"]);
                        bang.CmdParams.Parameters.AddWithValue("@iKyThuat", r["iKyThuat"]);
                        bang.CmdParams.Parameters.AddWithValue("@MaLoai", 2);
                        bang.CmdParams.Parameters.AddWithValue("@iID_MaDonVi", r["iID_MaDonVi"]);
                        bang.CmdParams.Parameters.AddWithValue("@sTenDonVi", r["sTenDonVi"]);
                        bang.CmdParams.Parameters.AddWithValue("@sGhiChu", r["sGhiChu"]);
                        bang.CmdParams.Parameters.AddWithValue("@iID_MaPhongBanDich", r["iID_MaPhongBanDich"]);
                        bang.CmdParams.Parameters.AddWithValue("@iID_MaMucLucNganSach", r["iID_MaMucLucNganSach"]);
                        bang.CmdParams.Parameters.AddWithValue("@iID_MaMucLucNganSach_Cha", r["iID_MaMucLucNganSach_Cha"]);
                        bang.CmdParams.Parameters.AddWithValue("@sXauNoiMa", sXauNoiMa);
                        bang.CmdParams.Parameters.AddWithValue("@sLNS", r["sLNS"]);
                        bang.CmdParams.Parameters.AddWithValue("@sL", r["sL"]);
                        bang.CmdParams.Parameters.AddWithValue("@sK", r["sK"]);
                        bang.CmdParams.Parameters.AddWithValue("@sM", r["sM"]);
                        bang.CmdParams.Parameters.AddWithValue("@sTM", r["sTM"]);
                        bang.CmdParams.Parameters.AddWithValue("@sTTM", r["sTTM"]);
                        bang.CmdParams.Parameters.AddWithValue("@sNG", arrMaNganh[i]);
                        bang.CmdParams.Parameters.AddWithValue("@sTNG", r["sTNG"]);
                        bang.CmdParams.Parameters.AddWithValue("@sMoTa", sMoTa);
                        bang.CmdParams.Parameters.AddWithValue("@bsMaCongTrinh", false);
                        bang.CmdParams.Parameters.AddWithValue("@bsTenCongTrinh", false);
                        bang.CmdParams.Parameters.AddWithValue("@brNgay", false);
                        bang.CmdParams.Parameters.AddWithValue("@brSoNguoi", false);
                        bang.CmdParams.Parameters.AddWithValue("@brChiTaiKhoBac", false);
                        bang.CmdParams.Parameters.AddWithValue("@brChiTapTrung", false);
                        bang.Save();
                    }
                }//end if maphongban06


            }

            cmd = new SqlCommand();
            SQL = String.Format(@"SELECT iID_MaMucLucNganSach,iID_MaMucLucNganSach_Cha,bLaHangCha,sXauNoiMa,sTenCongTrinh,sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,sMoTa,iID_MaChungTuChiTiet,iID_MaDonVi,sTenDonVi,sTenDonVi as sTenDonVi_BaoDam,iID_MaPhongBanDich,iID_MaPhongBan
,rNgay,rSoNguoi,rChiTaiKhoBac,rTonKho,rTuChi,rChiTapTrung,rHangNhap,rHangMua,rHienVat,rDuPhong,rPhanCap,sGhiChu,sMaCongTrinh
,brTuChi,brHienVat,brHangNhap,brHangMua,brTonKho,brPhanCap,brDuPhong,bsTenCongTrinh,brNgay,brSoNguoi,brChiTaiKhoBac,brChiTapTrung,dNgayTao
FROM DT_ChungTuChiTiet_PhanCap
WHERE  iTrangThai=1 AND iID_MaChungTu=@iID_MaChungTu

UNION

SELECT iID_MaMucLucNganSach,iID_MaMucLucNganSach_Cha,bLaHangCha,sXauNoiMa,sTenCongTrinh, sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,sMoTa,iID_MaChungTuChiTiet,iID_MaDonVi,sTenDonVi,sTenDonVi as sTenDonVi_BaoDam,iID_MaPhongBanDich,iID_MaPhongBan
,rNgay,rSoNguoi,rChiTaiKhoBac,rTonKho,rTuChi,rChiTapTrung,rHangNhap,rHangMua,rHienVat,rDuPhong,rPhanCap,sGhiChu,sMaCongTrinh
,brTuChi,brHienVat,brHangNhap,brHangMua,brTonKho,brPhanCap,brDuPhong,bsTenCongTrinh,brNgay,brSoNguoi,brChiTaiKhoBac,brChiTapTrung,dNgayTao
FROM DT_ChungTuChiTiet
WHERE iTrangThai=1 AND iID_MaChungTu=@iID_MaChungTu

ORDER BY dNgayTao,sLNS, sM,sTM,sTTM,sNG, iID_MaDonVi");
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iID_MaChungTu", iID_MaChungTu);
            vR = Connection.GetDataTable(cmd);
            if (vR.Rows.Count > 0)
            {
                vR.Rows[0]["brTuChi"] = true;
                vR.Rows[0]["brHienVat"] = true;
                vR.Rows[0]["brHangNhap"] = true;
                vR.Rows[0]["brHangMua"] = true;
                vR.Rows[0]["brTonKho"] = true;
                vR.Rows[0]["brPhanCap"] = true;
                vR.Rows[0]["brDuPhong"] = true;
            }
            vR.Columns.Add("bPhanCap", typeof(Boolean));
            vR.Dispose();

            return vR;
        }


        public static DataTable GetChungTuChiTiet_Gom(String iID_MaChungTu, Dictionary<String, String> arrGiaTriTimKiem, String MaND)
        {

            DataTable vR, dtChungTu;
            String SQL, DK = "", Dk1 = "";
            SqlCommand cmd;
            DataTable dt = NganSach_HamChungModels.DSLNSCuaPhongBan(MaND);

            dtChungTu = GetChungTu_Gom(iID_MaChungTu);

            String[] arrChungTu = Convert.ToString(dtChungTu.Rows[0]["iID_MaChungTu"]).Split(',');
            cmd = new SqlCommand();
            for (int i = 0; i < arrChungTu.Length; i++)
            {
                DK += " iID_MaChungTu=@iID_MaChungTu" + i;
                if (i < arrChungTu.Length - 1)
                    DK += " OR ";
                cmd.Parameters.AddWithValue("iID_MaChungTu" + i, arrChungTu[i]);
            }
            if (!String.IsNullOrEmpty(DK))
            {
                DK = "AND(" + DK + ")";
            }
            SQL = String.Format(@"SELECT DISTINCT sDSLNS FROM DT_ChungTu WHERE iTrangThai=1 {0} ", DK);
            DK = "";
            cmd.CommandText = SQL;
            DataTable dtsLNS = Connection.GetDataTable(cmd);
            cmd.Dispose();
            if (dtsLNS.Rows.Count > 0)
            {
                for (int i = 0; i < dtsLNS.Rows.Count; i++)
                {
                    Dk1 += String.Format(" sLNS LIKE '{0}%' ", dtsLNS.Rows[i]["sDSLNS"]);
                    if (i < dtsLNS.Rows.Count - 1)
                        Dk1 += " OR ";
                }
                DK = String.Format("iTrangThai=1 AND ({0}) ", Dk1);
            }


            DataRow RChungTu = dtChungTu.Rows[0];

            String sTruongTien = MucLucNganSachModels.strDSTruongTien + ",iID_MaChungTuChiTiet,iID_MaDonVi,sTenDonVi,sMaCongTrinh";
            String[] arrDSTruong = MucLucNganSachModels.strDSTruong.Split(',');
            String[] arrDSTruong_TK = (MucLucNganSachModels.strDSTruong + ",iID_MaDonVi").Split(',');
            String[] arrDSTruongTien = sTruongTien.Split(',');



            //<--Lay toan bo Muc luc ngan sach
            cmd = new SqlCommand();


            if (arrGiaTriTimKiem != null)
            {
                for (int i = 0; i < arrDSTruong.Length; i++)
                {
                    if (String.IsNullOrEmpty(arrGiaTriTimKiem[arrDSTruong[i]]) == false)
                    {
                        DK += String.Format(" AND {0} LIKE @{0}", arrDSTruong[i]);
                        cmd.Parameters.AddWithValue("@" + arrDSTruong[i], "%" + arrGiaTriTimKiem[arrDSTruong[i]] + "%");
                    }
                }
            }
            if (dt.Rows.Count > 0)
                DK += " AND( ";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                String Nguon = Convert.ToString(dt.Rows[i]["sLNS"]).Substring(0, 1);
                String LoaiNS = Convert.ToString(dt.Rows[i]["sLNS"]).Substring(0, 3);
                DK += "  (sLNS=@sLNS" + i + " OR (SUBSTRING(sLNS,1,1)=" + Nguon + " AND LEN(sLNS)=1) OR  (SUBSTRING(sLNS,1,3)=" + LoaiNS + " AND LEN(sLNS)=3))";
                if (i < dt.Rows.Count - 1)
                    DK += " OR ";
                cmd.Parameters.AddWithValue("@sLNS" + i, dt.Rows[i]["sLNS"]);
            }
            if (dt.Rows.Count > 0)
                DK += " ) ";
            SQL = String.Format("SELECT iID_MaMucLucNganSach,iID_MaMucLucNganSach_Cha,bLaHangCha,sXauNoiMa,{0},{1} FROM NS_MucLucNganSach WHERE iNamLamViec=@iNamLamViec AND {2}  ORDER BY {3}", MucLucNganSachModels.strDSTruong, MucLucNganSachModels.strDSDuocNhapTruongTien, DK, MucLucNganSachModels.strDSTruongSapXep);
            int iNamLamViec = Convert.ToInt32(ReportModels.LayNamLamViec(MaND));
            cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
            cmd.CommandText = SQL;
            vR = Connection.GetDataTable(cmd);
            cmd.Dispose();
            //Lay toan bo Muc luc ngan sach-->


            DK = "";
            cmd = new SqlCommand();
            for (int i = 0; i < arrChungTu.Length; i++)
            {
                DK += " iID_MaChungTu=@iID_MaChungTu" + i;
                if (i < arrChungTu.Length - 1)
                    DK += " OR ";
                cmd.Parameters.AddWithValue("iID_MaChungTu" + i, arrChungTu[i]);
            }

            DK = "iTrangThai=1 AND (" + DK + ")";

            if (arrGiaTriTimKiem != null)
            {
                if (String.IsNullOrEmpty(arrGiaTriTimKiem["iID_MaDonVi"]) == false)
                {
                    DK += String.Format(" AND {0} LIKE @{0}", "iID_MaDonVi");
                    cmd.Parameters.AddWithValue("@" + "iID_MaDonVi", "%" + arrGiaTriTimKiem["iID_MaDonVi"] + "%");
                }
            }
            DK += " AND (";
            for (int i = 1; i < arrDSTruongTien.Length - 4; i++)
            {
                DK += arrDSTruongTien[i] + "<>0 OR ";
            }
            DK = DK.Substring(0, DK.Length - 3);
            DK += ") ";


            SQL = String.Format("SELECT * FROM DT_ChungTuChiTiet WHERE {0} ORDER BY sXauNoiMa,iID_MaDonVi", DK);
            cmd.CommandText = SQL;

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
                                vR.Rows[i][k] = dtChungTuChiTiet.Rows[j][vR.Columns[k].ColumnName];
                            }
                            count++;
                        }
                        else
                        {
                            DataRow row = vR.NewRow();
                            for (int k = 0; k < vR.Columns.Count - 1; k++)
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
            vR.Columns.Add("bPhanCap", typeof(Boolean));
            dtChungTu.Dispose();
            dtChungTuChiTiet.Dispose();
            vR.Dispose();
            cmd.Dispose();
            return vR;
        }

        public static DataTable GetChungTuChiTiet_PhanCap(String iID_MaChungTu, Dictionary<String, String> arrGiaTriTimKiem, String MaND, String sLNS, String sXauNoiMa, String iKyThuat, String MaLoai)
        {

            DataTable vR;
            DataTable dt = NganSach_HamChungModels.DSLNSCuaPhongBan(MaND);
            String sTruongTien = MucLucNganSachModels.strDSTruongTien + ",iID_MaChungTuChiTiet,iID_MaDonVi,sTenDonVi,sTenDonVi_BaoDam,iID_MaPhongBanDich,iID_MaPhongBan,sGhiChu,sMaCongTrinh";
            String[] arrDSTruong = MucLucNganSachModels.strDSTruong.Split(',');
            String[] arrDSTruong_TK = (MucLucNganSachModels.strDSTruong + ",iID_MaDonVi").Split(',');
            String[] arrDSTruongTien = sTruongTien.Split(',');

            String SQL, DK;
            SqlCommand cmd;

            #region NganhKyThuat
            if (iKyThuat == "1")
            {
                sLNS = "1020100";
                //<--Lay toan bo Muc luc ngan sach
                cmd = new SqlCommand();
                //Phan cap lan 2: danh sach nganh theo nguoi quan ly
                if (MaLoai == "2")
                {
                    DK = String.Format("iTrangThai=1 AND sLNS LIKE '{0}%' AND sXauNoiMa LIKE '%{1}%'", sLNS, sXauNoiMa);
                }
                else
                {
                    DK = String.Format("iTrangThai=1 AND sLNS LIKE '{0}%' AND sXauNoiMa LIKE '%{1}%'", sLNS, sXauNoiMa);
                }

                if (arrGiaTriTimKiem != null)
                {
                    //if (arrGiaTriTimKiem["iID_MaDonVi"] != null)
                    //    DK += String.Format("iID_MaDonVi={0}", arrGiaTriTimKiem["iID_MaDonVi"]);
                    for (int i = 0; i < arrDSTruong.Length; i++)
                    {
                        if (String.IsNullOrEmpty(arrGiaTriTimKiem[arrDSTruong[i]]) == false)
                        {
                            DK += String.Format(" AND {0} LIKE @{0}", arrDSTruong[i]);
                            cmd.Parameters.AddWithValue("@" + arrDSTruong[i], "%" + arrGiaTriTimKiem[arrDSTruong[i]] + "%");
                        }
                    }
                }
                if (dt.Rows.Count > 0)
                    DK += " AND( ";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    String Nguon = Convert.ToString(dt.Rows[i]["sLNS"]).Substring(0, 1);
                    String LoaiNS = Convert.ToString(dt.Rows[i]["sLNS"]).Substring(0, 3);
                    DK += "  (sLNS=@sLNS" + i + " OR (SUBSTRING(sLNS,1,1)=" + Nguon + " AND LEN(sLNS)=1) OR  (SUBSTRING(sLNS,1,3)=" + LoaiNS + " AND LEN(sLNS)=3))";
                    if (i < dt.Rows.Count - 1)
                        DK += " OR ";
                    cmd.Parameters.AddWithValue("@sLNS" + i, dt.Rows[i]["sLNS"]);
                }
                if (dt.Rows.Count > 0)
                    DK += " ) ";
                SQL = String.Format("SELECT iID_MaMucLucNganSach,iID_MaMucLucNganSach_Cha,bLaHangCha,sXauNoiMa,{0},{1} FROM NS_MucLucNganSach WHERE iNamLamViec=@iNamLamViec AND {2}  ORDER BY {3}", MucLucNganSachModels.strDSTruong, MucLucNganSachModels.strDSDuocNhapTruongTien, DK, MucLucNganSachModels.strDSTruongSapXep);
                int iNamLamViec = Convert.ToInt32(ReportModels.LayNamLamViec(MaND));
                cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                cmd.CommandText = SQL;
                vR = Connection.GetDataTable(cmd);
                cmd.Dispose();
                //Lay toan bo Muc luc ngan sach-->
                DK = "iTrangThai=1 AND iID_MaChungTu=@iID_MaChungTu";

                if (arrGiaTriTimKiem != null)
                {
                    if (String.IsNullOrEmpty(arrGiaTriTimKiem["iID_MaDonVi"]) == false)
                    {
                        DK += String.Format(" AND {0} LIKE @{0}", "iID_MaDonVi");
                        cmd.Parameters.AddWithValue("@" + "iID_MaDonVi", "%" + arrGiaTriTimKiem["iID_MaDonVi"] + "%");
                    }
                    if (String.IsNullOrEmpty(arrGiaTriTimKiem["iID_MaPhongBanDich"]) == false)
                    {
                        DK += String.Format(" AND {0} LIKE @{0}", "iID_MaPhongBanDich");
                        cmd.Parameters.AddWithValue("@" + "iID_MaPhongBanDich", "%" + arrGiaTriTimKiem["iID_MaPhongBanDich"] + "%");
                    }
                    for (int i = 0; i < arrDSTruong.Length; i++)
                    {
                        if (String.IsNullOrEmpty(arrGiaTriTimKiem[arrDSTruong[i]]) == false)
                        {
                            DK += String.Format(" AND {0} LIKE @{0}", arrDSTruong[i]);

                        }
                    }
                }
                DK += " AND (";
                for (int i = 1; i < arrDSTruongTien.Length - 8; i++)
                {
                    DK += arrDSTruongTien[i] + "<>0 OR ";
                }
                DK = DK.Substring(0, DK.Length - 3);
                DK += ") ";

                cmd.Parameters.AddWithValue("@iID_MaChungTu", iID_MaChungTu);
                SQL = String.Format(@"SELECT iID_MaMucLucNganSach,iID_MaMucLucNganSach_Cha,bLaHangCha,sXauNoiMa,sTenCongTrinh,sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,sMoTa,iID_MaChungTuChiTiet,iID_MaDonVi,sTenDonVi,sTenDonVi as sTenDonVi_BaoDam,iID_MaPhongBanDich,iID_MaPhongBan
,rNgay,rSoNguoi,rChiTaiKhoBac,rTonKho,rTuChi,rChiTapTrung,rHangNhap,rHangMua,rHienVat,rDuPhong,rPhanCap,sGhiChu
FROM DT_ChungTuChiTiet_PhanCap
WHERE  {0}

UNION

SELECT iID_MaMucLucNganSach,iID_MaMucLucNganSach_Cha,bLaHangCha,sXauNoiMa,sTenCongTrinh, sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,sMoTa,iID_MaChungTuChiTiet,iID_MaDonVi,sTenDonVi,sTenDonVi as sTenDonVi_BaoDam,iID_MaPhongBanDich,iID_MaPhongBan
,rNgay,rSoNguoi,rChiTaiKhoBac,rTonKho,rTuChi,rChiTapTrung,rHangNhap,rHangMua,rHienVat,rDuPhong,rPhanCap,sGhiChu
FROM DT_ChungTuChiTiet
WHERE {0}

ORDER BY sM,sTM,sTTM,sNG, iID_MaDonVi", DK);
                cmd.CommandText = SQL;

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

                if (sLNS.Substring(0, 3) == "207" || sLNS.Contains("4080300"))
                {
                }
                else if (sLNS.Substring(0, 3) != "109")
                {
                    sLNS = "1020100";
                }

                else
                {
                    sLNS = "1020100";
                }
                //<--Lay toan bo Muc luc ngan sach
                cmd = new SqlCommand();
                DK = String.Format("iTrangThai=1 AND sLNS LIKE '{0}%' AND sXauNoiMa LIKE '%{1}%'", sLNS, sXauNoiMa);
                if (arrGiaTriTimKiem != null)
                {
                    //if (arrGiaTriTimKiem["iID_MaDonVi"] != null)
                    //    DK += String.Format("iID_MaDonVi={0}", arrGiaTriTimKiem["iID_MaDonVi"]);
                    for (int i = 0; i < arrDSTruong.Length; i++)
                    {
                        if (String.IsNullOrEmpty(arrGiaTriTimKiem[arrDSTruong[i]]) == false)
                        {
                            DK += String.Format(" AND {0} LIKE @{0}", arrDSTruong[i]);
                            cmd.Parameters.AddWithValue("@" + arrDSTruong[i], "%" + arrGiaTriTimKiem[arrDSTruong[i]] + "%");
                        }
                    }
                }
                if (dt.Rows.Count > 0)
                    DK += " AND( ";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    String Nguon = Convert.ToString(dt.Rows[i]["sLNS"]).Substring(0, 1);
                    String LoaiNS = Convert.ToString(dt.Rows[i]["sLNS"]).Substring(0, 3);
                    DK += "  (sLNS=@sLNS" + i + " OR (SUBSTRING(sLNS,1,1)=" + Nguon + " AND LEN(sLNS)=1) OR  (SUBSTRING(sLNS,1,3)=" + LoaiNS + " AND LEN(sLNS)=3))";
                    if (i < dt.Rows.Count - 1)
                        DK += " OR ";
                    cmd.Parameters.AddWithValue("@sLNS" + i, dt.Rows[i]["sLNS"]);
                }
                if (dt.Rows.Count > 0)
                    DK += " ) ";
                SQL = String.Format("SELECT iID_MaMucLucNganSach,iID_MaMucLucNganSach_Cha,bLaHangCha,sXauNoiMa,{0},{1} FROM NS_MucLucNganSach WHERE iNamLamViec=@iNamLamViec AND {2}  ORDER BY {3}", MucLucNganSachModels.strDSTruong, MucLucNganSachModels.strDSDuocNhapTruongTien, DK, MucLucNganSachModels.strDSTruongSapXep);
                int iNamLamViec = Convert.ToInt32(ReportModels.LayNamLamViec(MaND));
                cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                cmd.CommandText = SQL;
                vR = Connection.GetDataTable(cmd);
                cmd.Dispose();
                //Lay toan bo Muc luc ngan sach-->
                DK = "iTrangThai=1 AND iID_MaChungTu=@iID_MaChungTu";

                if (arrGiaTriTimKiem != null)
                {
                    if (String.IsNullOrEmpty(arrGiaTriTimKiem["iID_MaDonVi"]) == false)
                    {
                        DK += String.Format(" AND {0} LIKE @{0}", "iID_MaDonVi");
                        cmd.Parameters.AddWithValue("@" + "iID_MaDonVi", "%" + arrGiaTriTimKiem["iID_MaDonVi"] + "%");
                    }
                    if (String.IsNullOrEmpty(arrGiaTriTimKiem["iID_MaPhongBanDich"]) == false)
                    {
                        DK += String.Format(" AND {0} LIKE @{0}", "iID_MaPhongBanDich");
                        cmd.Parameters.AddWithValue("@" + "iID_MaPhongBanDich", "%" + arrGiaTriTimKiem["iID_MaPhongBanDich"] + "%");
                    }
                    for (int i = 0; i < arrDSTruong.Length; i++)
                    {
                        if (String.IsNullOrEmpty(arrGiaTriTimKiem[arrDSTruong[i]]) == false)
                        {
                            DK += String.Format(" AND {0} LIKE @{0}", arrDSTruong[i]);

                        }
                    }
                }
                DK += " AND (";
                for (int i = 1; i < arrDSTruongTien.Length - 8; i++)
                {
                    DK += arrDSTruongTien[i] + "<>0 OR ";
                }
                DK = DK.Substring(0, DK.Length - 3);
                DK += ") ";

                cmd.Parameters.AddWithValue("@iID_MaChungTu", iID_MaChungTu);
                SQL = String.Format("SELECT *,sTenDonVi as sTenDonVi_BaoDam  FROM DT_ChungTuChiTiet_PhanCap WHERE {0} ORDER BY sXauNoiMa,iID_MaDonVi", DK);
                cmd.CommandText = SQL;

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



        public static int Delete_ChungTuChiTiet(String MaChungTuChiTiet, String IPSua, String MaNguoiDungSua)
        {
            int vR = 0;
            try
            {
                //Xóa dữ liệu trong bảng DT_DotNganSach
                Bang bang = new Bang("DT_ChungTuChiTiet");
                bang.MaNguoiDungSua = MaNguoiDungSua;
                bang.IPSua = IPSua;
                bang.GiaTriKhoa = MaChungTuChiTiet;
                bang.Delete();
                vR = 1;
            }
            catch
            {
                vR = 0;
            }
            return vR;
        }

        public static DataTable Get_dtChiTietMucLucNganSach(String[] arrDSTruong, List<String> lstDSGiaTri)
        {
            SqlCommand cmd = new SqlCommand();

            String DK = "";
            for (int i = 0; i < lstDSGiaTri.Count; i++)
            {
                if (DK != "") DK += " AND ";
                DK += String.Format("@{0}={0}", arrDSTruong[i]);
                cmd.Parameters.AddWithValue("@" + arrDSTruong[i], lstDSGiaTri[i]);
            }
            cmd.CommandText = "SELECT * FROM NS_MucLucNganSach WHERE " + DK;
            DataTable vR = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return vR;
        }

        /// <summary>
        /// Thêm 1 chứng từ chi tiết
        /// </summary>
        /// <param name="ParentID">Phần trước của danh sách giá trị truyền vào</param>
        /// <param name="Values">Danh sách giá trị truyền vào (Đối với Form là Form.Request)</param>
        /// <param name="MaND"></param>
        /// <param name="IPSua"></param>
        /// <returns>Danh sách các lỗi của giá trị truyền vào</returns>
        public static NameValueCollection ThemChungTuChiTiet(String iID_MaChungTu, String ParentID, NameValueCollection Values, String MaND, String IPSua)
        {
            String sLNS = Values[ParentID + "_sLNS"];
            String iID_MaDonVi = Values[ParentID + "_iID_MaDonVi"];
            String sTenDonVi = Values[ParentID + "_sTenDonVi"];
            String DSTruong = MucLucNganSachModels.strDSTruong;
            String[] arrDSTruong = DSTruong.Split(',');

            List<String> lstDSGiaTri = new List<String>();
            for (int i = 0; i < arrDSTruong.Length - 1; i++)
            {
                lstDSGiaTri.Add(Convert.ToString(Values[ParentID + "_" + arrDSTruong[i]]));
            }
            DataTable dtMucLucNganSach = DuToan_ChungTuChiTietModels.Get_dtChiTietMucLucNganSach(arrDSTruong, lstDSGiaTri);

            Bang bang = new Bang("DT_ChungTuChiTiet");
            bang.MaNguoiDungSua = MaND;
            bang.IPSua = IPSua;
            NameValueCollection arrLoi = bang.TruyenGiaTri(ParentID, Values);
            bang.DuLieuMoi = true;

            //bang.CmdParams.Parameters.AddWithValue("@sTenCongTrinh", Values[ParentID + "_sTenCongTrinh"]);

            //Bắt buộc phải có iID_MaMucLucNganSach
            if (String.IsNullOrEmpty(iID_MaDonVi))
            {
                arrLoi.Add("err_iID_MaDonVi", "Chưa chọn đơn vị");
            }

            if (arrLoi.Count == 0)
            {
                String iID_MaMucLucNganSach = "";
                String sXauNoiMa = "";

                for (int i = 0; i < arrDSTruong.Length - 1; i++)
                {
                    String sXauMaTG = Convert.ToString(Values[ParentID + "_" + arrDSTruong[i]]);
                    if (String.IsNullOrEmpty(sXauMaTG))
                    {
                        break;
                    }
                    else
                    {
                        if (sXauNoiMa != "") sXauNoiMa += "-";
                        sXauNoiMa += sXauMaTG;
                    }
                }

                if (dtMucLucNganSach.Rows.Count > 0)
                {
                    iID_MaMucLucNganSach = Convert.ToString(dtMucLucNganSach.Rows[0]["iID_MaMucLucNganSach"]);
                    //<--Thêm tham số từ bảng MucLucNganSach
                    String[] arrDSDuocNhapTruongTien = DuToanModels.strDSDuocNhapTruongTien.Split(',');//suadutoan
                    for (int i = 0; i < arrDSDuocNhapTruongTien.Length; i++)
                    {
                        bang.CmdParams.Parameters.AddWithValue("@" + arrDSDuocNhapTruongTien[i], dtMucLucNganSach.Rows[0][arrDSDuocNhapTruongTien[i]]);
                    }
                    //-->Thêm tham số từ bảng MucLucNganSach
                }

                //<--Xác định trường rTongSo
                Double rTongSo = 0;
                Double rTongSo_DonVi = 0;
                String DSTruongTien = DuToanModels.strDSTruongTien;
                String[] arrDSTruongTien = DSTruongTien.Split(',');
                for (int i = 0; i < arrDSTruongTien.Length; i++)
                {
                    if (arrDSTruongTien[i] != "rChiTapTrung" && arrDSTruongTien[i] != "rNgay" && arrDSTruongTien[i] != "rSoNguoi" && arrDSTruongTien[i] != "rChiTapTrung_DonVi" && arrDSTruongTien[i] != "rNgay_DonVi" && arrDSTruongTien[i] != "rSoNguoi_DonVi" && arrDSTruongTien[i].StartsWith("r"))
                    {
                        if (CommonFunction.IsNumeric(bang.CmdParams.Parameters["@" + arrDSTruongTien[i]].Value) && arrDSTruongTien[i].EndsWith("_DonVi") == false)
                        {
                            rTongSo += Convert.ToDouble(bang.CmdParams.Parameters["@" + arrDSTruongTien[i]].Value);
                        }
                        if (CommonFunction.IsNumeric(bang.CmdParams.Parameters["@" + arrDSTruongTien[i]].Value) && arrDSTruongTien[i].EndsWith("_DonVi") == true)
                        {
                            rTongSo_DonVi += Convert.ToDouble(bang.CmdParams.Parameters["@" + arrDSTruongTien[i]].Value);
                        }
                    }
                }
                bang.CmdParams.Parameters.AddWithValue("@rTongSo", rTongSo);
                bang.CmdParams.Parameters.AddWithValue("@rTongSo_DonVi", rTongSo_DonVi);
                //-->Xác định trường rTongSo

                //<--Thêm tham số từ bảng DT_ChungTu
                DataTable dtChungTu = DuToan_ChungTuModels.GetChungTu(iID_MaChungTu);
                bang.CmdParams.Parameters.AddWithValue("@iID_MaDotNganSach", dtChungTu.Rows[0]["iID_MaDotNganSach"]);
                bang.CmdParams.Parameters.AddWithValue("@iID_MaChungTu", dtChungTu.Rows[0]["iID_MaChungTu"]);
                bang.CmdParams.Parameters.AddWithValue("@iID_MaPhongBan", dtChungTu.Rows[0]["iID_MaPhongBan"]);
                bang.CmdParams.Parameters.AddWithValue("@iID_MaTrangThaiDuyet", dtChungTu.Rows[0]["iID_MaTrangThaiDuyet"]);
                bang.CmdParams.Parameters.AddWithValue("@iNamLamViec", dtChungTu.Rows[0]["iNamLamViec"]);
                bang.CmdParams.Parameters.AddWithValue("@iID_MaNguonNganSach", dtChungTu.Rows[0]["iID_MaNguonNganSach"]);
                bang.CmdParams.Parameters.AddWithValue("@iID_MaNamNganSach", dtChungTu.Rows[0]["iID_MaNamNganSach"]);
                bang.CmdParams.Parameters.AddWithValue("@bChiNganSach", dtChungTu.Rows[0]["bChiNganSach"]);
                //-->Thêm tham số từ bảng DT_ChungTu

                if (iID_MaMucLucNganSach != "")
                {
                    bang.CmdParams.Parameters.AddWithValue("@iID_MaMucLucNganSach", iID_MaMucLucNganSach);
                }
                bang.CmdParams.Parameters.AddWithValue("@sXauNoiMa", sXauNoiMa);
                bang.CmdParams.Parameters.AddWithValue("@sTenDonVi", sTenDonVi);
                bang.Save();

                //<--Cập nhập lại bảng DT_ChungTu
                SqlCommand cmd;
                String sDSLNS = Convert.ToString(dtChungTu.Rows[0]["sDSLNS"]);
                if (sDSLNS.IndexOf(sLNS + ";") < 0)
                {
                    sDSLNS += sLNS + ";";
                    cmd = new SqlCommand();
                    cmd.Parameters.AddWithValue("@sDSLNS", sDSLNS);
                    DuToan_ChungTuModels.UpdateRecord(iID_MaChungTu, cmd.Parameters, MaND, IPSua);
                    cmd.Dispose();
                }
                dtChungTu.Dispose();
                //-->Cập nhập lại bảng DT_ChungTu

                //<--Cập nhập lại bảng DT_DotNganSach
                String iID_MaDotNganSach = Convert.ToString(dtChungTu.Rows[0]["iID_MaDotNganSach"]);
                cmd = new SqlCommand("SELECT sDSLNS FROM DT_DotNganSach WHERE iID_MaDotNganSach=@iID_MaDotNganSach");
                cmd.Parameters.AddWithValue("@iID_MaDotNganSach", iID_MaDotNganSach);
                sDSLNS = Connection.GetValueString(cmd, "");
                cmd.Dispose();
                if (sDSLNS.IndexOf(sLNS + ";") < 0)
                {
                    sDSLNS += sLNS + ";";
                    cmd = new SqlCommand();
                    cmd.Parameters.AddWithValue("@sDSLNS", sDSLNS);
                    DuToan_DotNganSachModels.UpdateRecord(iID_MaDotNganSach, cmd.Parameters, MaND, IPSua);
                    cmd.Dispose();
                }
                //-->Cập nhập lại bảng DT_DotNganSach
            }
            return arrLoi;
        }

        public static String DinhDangTruongTien(Object GT)
        {
            String sGT = "&nbsp;";
            if (CommonFunction.IsNumeric(GT))
            {
                if (Convert.ToDouble(GT) != 0)
                {
                    sGT = CommonFunction.DinhDangSo(Convert.ToString(GT));
                }
            }
            else
            {
                sGT = Convert.ToString(GT);
            }
            return sGT;
        }

        public static DataTable Get_dtCayChiTiet(String iID_MaChungTu)
        {
            DataTable vR = null;
            String SQL;

            SQL = "SELECT  sLNS, iID_MaDonVi FROM DT_ChungTuChiTiet WHERE iID_MaChungTu=@iID_MaChungTu GROUP BY sLNS, iID_MaDonVi";
            SQL = String.Format("SELECT DISTINCT NS_DonVi.sTen AS TenDonVi, tb1.* FROM ({0}) tb1 INNER JOIN NS_DonVi ON tb1.iID_MaDonVi=NS_DonVi.iID_MaDonVi ORDER BY sLNS, tb1.iID_MaDonVi,NS_DonVi.sTen", SQL);
            SqlCommand cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@iID_MaChungTu", iID_MaChungTu);
            vR = Connection.GetDataTable(cmd);
            cmd.Dispose();

            vR.Columns.Add("TenLoaiNganSach", typeof(String));
            vR.Columns.Add("iID_MaMucLucNganSach", typeof(String));

            for (int i = 0; i < vR.Rows.Count; i++)
            {
                String sLNS = Convert.ToString(vR.Rows[i]["sLNS"]);
                SQL = "SELECT * FROM NS_MucLucNganSach WHERE sLNS=@sLNS AND sL=''";
                cmd = new SqlCommand(SQL);
                cmd.Parameters.AddWithValue("@sLNS", sLNS);
                DataTable dt = Connection.GetDataTable(cmd);
                if (dt != null && dt.Rows.Count > 0)
                {
                    vR.Rows[i]["iID_MaMucLucNganSach"] = dt.Rows[0]["iID_MaMucLucNganSach"];
                    vR.Rows[i]["TenLoaiNganSach"] = dt.Rows[0]["sMoTa"];
                }
                else
                {
                    vR.Rows[i]["TenLoaiNganSach"] = sLNS;
                }
                cmd.Dispose();
                dt.Dispose();
            }

            return vR;
        }

        public static int Get_iID_MaTrangThaiDuyet_TuChoi(String MaND, String MaChungTu)
        {
            int vR = -1;
            DataTable dt = DuToan_ChungTuModels.GetChungTu(MaChungTu);
            int iID_MaTrangThaiDuyet = Convert.ToInt32(dt.Rows[0]["iID_MaTrangThaiDuyet"]);
            int iCheck = Convert.ToInt32(dt.Rows[0]["iCheck"]);
            dt.Dispose();


            //Trolytonghop duoc trinh chung tu minh tao
            Boolean checkTroLyTongHop = LuongCongViecModel.KiemTra_TroLyTongHop(MaND);
            Boolean CheckTrangThaiDuyetMoiTao = LuongCongViecModel.KiemTra_TrangThaiKhoiTao(DuToanModels.iID_MaPhanHe, iID_MaTrangThaiDuyet);
            //la tro tong hop va la trang thai moi tao
            if (checkTroLyTongHop && CheckTrangThaiDuyetMoiTao)
            {
                int iID_MaTrangThaiDuyet_TuChoi = LuongCongViecModel.Luong_iID_MaTrangThaiDuyet_TuChoi(iID_MaTrangThaiDuyet);
                if (iID_MaTrangThaiDuyet_TuChoi > 0)
                {
                    vR = Convert.ToInt32(iID_MaTrangThaiDuyet_TuChoi);
                }

            }
            //la tro ly tong hop chua gom chung tu
            //else if (checkTroLyTongHop && iCheck == 0)
            //{
            //    vR = -1;
            //}
            else
            {
                if (LuongCongViecModel.NguoiDung_DuocSuaChungTu(PhanHeModels.iID_MaPhanHeDuToan, MaND, iID_MaTrangThaiDuyet))
                {
                    int iID_MaTrangThaiDuyet_TuChoi = LuongCongViecModel.Luong_iID_MaTrangThaiDuyet_TuChoi(iID_MaTrangThaiDuyet);
                    if (iID_MaTrangThaiDuyet_TuChoi > 0)
                    {
                        //SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM DT_ChungTuChiTiet WHERE iID_MaChungTu=@iID_MaChungTu AND bDongY=0");
                        //cmd.Parameters.AddWithValue("@iID_MaChungTu", MaChungTu);
                        //if (Convert.ToInt32(Connection.GetValue(cmd, 0)) > 0)
                        //{
                        vR = Convert.ToInt32(iID_MaTrangThaiDuyet_TuChoi);
                        //}
                        //cmd.Dispose();
                    }
                }
            }
            return vR;
        }

        public static int Get_iID_MaTrangThaiDuyet_TrinhDuyet(String MaND, String MaChungTu)
        {
            int vR = -1;
            DataTable dt = DuToan_ChungTuModels.GetChungTu(MaChungTu);
            int iID_MaTrangThaiDuyet = Convert.ToInt32(dt.Rows[0]["iID_MaTrangThaiDuyet"]);
            int iCheck = Convert.ToInt32(dt.Rows[0]["iCheck"]);
            String iID_MaNguoiDungTao = Convert.ToString(dt.Rows[0]["sID_MaNguoiDungTao"]);
            dt.Dispose();

            //Trolytonghop duoc trinh chung tu minh tao
            Boolean checkTroLyTongHop = LuongCongViecModel.KiemTra_TroLyTongHop(MaND);
            Boolean CheckTrangThaiDuyetMoiTao = LuongCongViecModel.KiemTra_TrangThaiKhoiTao(DuToanModels.iID_MaPhanHe, iID_MaTrangThaiDuyet);
            //la tro tong hop va la trang thai moi tao
            if (checkTroLyTongHop && CheckTrangThaiDuyetMoiTao)
            {
                int iID_MaTrangThaiDuyet_TrinhDuyet = LuongCongViecModel.Luong_iID_MaTrangThaiDuyet_TrinhDuyet(iID_MaTrangThaiDuyet);
                if (iID_MaTrangThaiDuyet_TrinhDuyet > 0)
                {
                    vR = Convert.ToInt32(iID_MaTrangThaiDuyet_TrinhDuyet);
                }

            }

            //la tro ly tong hop chua gom chung tu
            //else if (checkTroLyTongHop && iCheck == 0)
            //{
            //    vR = -1;
            //}
            else if (checkTroLyTongHop && MaND == iID_MaNguoiDungTao && LuongCongViecModel.KiemTra_TrangThaiTuChoi(PhanHeModels.iID_MaPhanHeDuToan, iID_MaTrangThaiDuyet))
            {
                int iID_MaTrangThaiDuyet_TrinhDuyet = LuongCongViecModel.Luong_iID_MaTrangThaiDuyet_TrinhDuyet(iID_MaTrangThaiDuyet);
                if (iID_MaTrangThaiDuyet_TrinhDuyet > 0)
                {
                    vR = Convert.ToInt32(iID_MaTrangThaiDuyet_TrinhDuyet);
                }
            }
            else
            {
                if (LuongCongViecModel.NguoiDung_DuocSuaChungTu(PhanHeModels.iID_MaPhanHeDuToan, MaND, iID_MaTrangThaiDuyet))
                {
                    int iID_MaTrangThaiDuyet_TrinhDuyet = LuongCongViecModel.Luong_iID_MaTrangThaiDuyet_TrinhDuyet(iID_MaTrangThaiDuyet);
                    if (iID_MaTrangThaiDuyet_TrinhDuyet > 0)
                    {
                        vR = Convert.ToInt32(iID_MaTrangThaiDuyet_TrinhDuyet);
                    }
                }
            }
            return vR;
        }
        //Lay trang thai duyet từ chối ngân sách bảo đảm
        public static int Get_iID_MaTrangThaiDuyet_BaoDam_TuChoi(String MaND, String MaChungTu)
        {
            int vR = -1;
            DataTable dt = DuToan_ChungTuModels.GetChungTu(MaChungTu);
            int iID_MaTrangThaiDuyet = Convert.ToInt32(dt.Rows[0]["iID_MaTrangThaiDuyet"]);
            int iCheck = Convert.ToInt32(dt.Rows[0]["iCheck"]);
            String iKyThuat = Convert.ToString(dt.Rows[0]["iKyThuat"]);
            dt.Dispose();


            //Trolytonghop duoc trinh chung tu minh tao
            Boolean checkTroLyTongHop = LuongCongViecModel.KiemTra_TroLyTongHop(MaND);
            Boolean CheckTrangThaiDuyetMoiTao = false;
            if (iKyThuat == "1")
            {
                CheckTrangThaiDuyetMoiTao = LuongCongViecModel.KiemTra_TrangThaiKhoiTao(PhanHeModels.iID_MaPhanHeChiTieu, iID_MaTrangThaiDuyet);
            }
            else
            {
                CheckTrangThaiDuyetMoiTao = LuongCongViecModel.KiemTra_TrangThaiKhoiTao(DuToanModels.iID_MaPhanHe, iID_MaTrangThaiDuyet);
            }
            //la tro tong hop va la trang thai moi tao
            if (checkTroLyTongHop && CheckTrangThaiDuyetMoiTao)
            {
                int iID_MaTrangThaiDuyet_TuChoi = LuongCongViecModel.Luong_iID_MaTrangThaiDuyet_TuChoi(iID_MaTrangThaiDuyet);
                if (iID_MaTrangThaiDuyet_TuChoi > 0)
                {
                    vR = Convert.ToInt32(iID_MaTrangThaiDuyet_TuChoi);
                }

            }
            //la tro ly tong hop chua gom chung tu
            //else if (checkTroLyTongHop && iCheck == 0)
            //{
            //    vR = -1;
            //}
            else
            {
                if (iKyThuat == "1")
                {
                    if (LuongCongViecModel.NguoiDung_DuocSuaChungTu(PhanHeModels.iID_MaPhanHeChiTieu, MaND, iID_MaTrangThaiDuyet))
                    {
                        int iID_MaTrangThaiDuyet_TuChoi = LuongCongViecModel.Luong_iID_MaTrangThaiDuyet_TuChoi(iID_MaTrangThaiDuyet);
                        if (iID_MaTrangThaiDuyet_TuChoi > 0)
                        {
                            vR = Convert.ToInt32(iID_MaTrangThaiDuyet_TuChoi);
                        }
                    }
                }
                else
                {
                    if (LuongCongViecModel.NguoiDung_DuocSuaChungTu(PhanHeModels.iID_MaPhanHeDuToan, MaND, iID_MaTrangThaiDuyet))
                    {
                        int iID_MaTrangThaiDuyet_TuChoi = LuongCongViecModel.Luong_iID_MaTrangThaiDuyet_TuChoi(iID_MaTrangThaiDuyet);
                        if (iID_MaTrangThaiDuyet_TuChoi > 0)
                        {
                            vR = Convert.ToInt32(iID_MaTrangThaiDuyet_TuChoi);
                        }
                    }
                }
            }
            return vR;
        }
        //Lay trang thai duyet ngân sách bảo đảm
        public static int Get_iID_MaTrangThaiDuyet_BaoDam_TrinhDuyet(String MaND, String MaChungTu)
        {
            int vR = -1;
            DataTable dt = DuToan_ChungTuModels.GetChungTu(MaChungTu);
            int iID_MaTrangThaiDuyet = Convert.ToInt32(dt.Rows[0]["iID_MaTrangThaiDuyet"]);
            int iCheck = Convert.ToInt32(dt.Rows[0]["iCheck"]);
            String iID_MaNguoiDungTao = Convert.ToString(dt.Rows[0]["sID_MaNguoiDungTao"]);
            String iKyThuat = Convert.ToString(dt.Rows[0]["iKyThuat"]);
            dt.Dispose();
            //Trolytonghop duoc trinh chung tu minh tao
            Boolean checkTroLyTongHop = LuongCongViecModel.KiemTra_TroLyTongHop(MaND);
            Boolean CheckTrangThaiDuyetMoiTao = false;
            int iID_MaTrangThaiDuyet_TrinhDuyet = LuongCongViecModel.Luong_iID_MaTrangThaiDuyet_TrinhDuyet(iID_MaTrangThaiDuyet);
            bool bcheckTC = false;
            if (iKyThuat == "1")
            {
                //     CheckTrangThaiDuyetMoiTao = LuongCongViecModel.KiemTra_TrangThaiKhoiTao(PhanHeModels.iID_MaPhanHeChiTieu, iID_MaTrangThaiDuyet);
                if (LuongCongViecModel.NguoiDung_DuocSuaChungTu(PhanHeModels.iID_MaPhanHeChiTieu, MaND, iID_MaTrangThaiDuyet))
                {
                    vR = Convert.ToInt32(iID_MaTrangThaiDuyet_TrinhDuyet);
                }
                return vR;
            }
            else
            {
                CheckTrangThaiDuyetMoiTao = LuongCongViecModel.KiemTra_TrangThaiKhoiTao(DuToanModels.iID_MaPhanHe, iID_MaTrangThaiDuyet);
                bcheckTC = LuongCongViecModel.KiemTra_TrangThaiTuChoi(PhanHeModels.iID_MaPhanHeDuToan, iID_MaTrangThaiDuyet);
            }

            //la tro tong hop va la trang thai moi tao
            if (checkTroLyTongHop && CheckTrangThaiDuyetMoiTao)
            {

                if (iID_MaTrangThaiDuyet_TrinhDuyet > 0)
                {
                    vR = Convert.ToInt32(iID_MaTrangThaiDuyet_TrinhDuyet);
                }

            }

            //la tro ly tong hop chua gom chung tu
            else if (checkTroLyTongHop && iCheck == 0)
            {
                vR = -1;
            }
            else if (checkTroLyTongHop && MaND == iID_MaNguoiDungTao && bcheckTC)
            {
                if (iID_MaTrangThaiDuyet_TrinhDuyet > 0)
                {
                    vR = Convert.ToInt32(iID_MaTrangThaiDuyet_TrinhDuyet);
                }
            }
            else
            {
                if (iKyThuat == "1")
                {
                    if (LuongCongViecModel.NguoiDung_DuocSuaChungTu(PhanHeModels.iID_MaPhanHeChiTieu, MaND, iID_MaTrangThaiDuyet))
                    {
                        if (iID_MaTrangThaiDuyet_TrinhDuyet > 0)
                        {
                            vR = Convert.ToInt32(iID_MaTrangThaiDuyet_TrinhDuyet);
                        }
                    }
                }
                else
                {
                    if (LuongCongViecModel.NguoiDung_DuocSuaChungTu(PhanHeModels.iID_MaPhanHeDuToan, MaND, iID_MaTrangThaiDuyet))
                    {
                        if (iID_MaTrangThaiDuyet_TrinhDuyet > 0)
                        {
                            vR = Convert.ToInt32(iID_MaTrangThaiDuyet_TrinhDuyet);
                        }
                    }
                }
            }
            return vR;
        }

        public static int Get_iID_MaTrangThaiDuyet_Gom_BaoDam_TuChoi(String MaND, String MaChungTu)
        {
            int vR = -1;
            DataTable dt = DuToan_ChungTuModels.GetChungTu_Gom(MaChungTu);
            int iID_MaTrangThaiDuyet = Convert.ToInt32(dt.Rows[0]["iID_MaTrangThaiDuyet"]);
            dt.Dispose();
            if (LuongCongViecModel.NguoiDung_DuocSuaChungTu(PhanHeModels.iID_MaPhanHeDuToan, MaND, iID_MaTrangThaiDuyet))
            {
                int iID_MaTrangThaiDuyet_TuChoi = LuongCongViecModel.Luong_iID_MaTrangThaiDuyet_TuChoi(iID_MaTrangThaiDuyet);
                if (iID_MaTrangThaiDuyet_TuChoi > 0)
                {
                    //SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM DT_ChungTuChiTiet WHERE iID_MaChungTu=@iID_MaChungTu AND bDongY=0");
                    //cmd.Parameters.AddWithValue("@iID_MaChungTu", MaChungTu);
                    //if (Convert.ToInt32(Connection.GetValue(cmd, 0)) > 0)
                    //{
                    vR = Convert.ToInt32(iID_MaTrangThaiDuyet_TuChoi);
                    //}
                    //cmd.Dispose();
                }
            }
            return vR;
        }

        public static int Get_iID_MaTrangThaiDuyet_Gom_BaoDam_TrinhDuyet(String MaND, String MaChungTu)
        {
            int vR = -1;
            DataTable dt = DuToan_ChungTuModels.GetChungTu_Gom(MaChungTu);
            int iID_MaTrangThaiDuyet = Convert.ToInt32(dt.Rows[0]["iID_MaTrangThaiDuyet"]);
            dt.Dispose();
            if (LuongCongViecModel.NguoiDung_DuocSuaChungTu(PhanHeModels.iID_MaPhanHeDuToan, MaND, iID_MaTrangThaiDuyet))
            {
                int iID_MaTrangThaiDuyet_TrinhDuyet = LuongCongViecModel.Luong_iID_MaTrangThaiDuyet_TrinhDuyet(iID_MaTrangThaiDuyet);
                if (iID_MaTrangThaiDuyet_TrinhDuyet > 0)
                {
                    vR = Convert.ToInt32(iID_MaTrangThaiDuyet_TrinhDuyet);
                }
            }
            return vR;
        }

        public static int Get_iID_MaTrangThaiDuyet_Gom_TuChoi(String MaND, String MaChungTu)
        {
            int vR = -1;
            DataTable dt = DuToan_ChungTuModels.GetChungTu_Gom(MaChungTu);
            int iID_MaTrangThaiDuyet = Convert.ToInt32(dt.Rows[0]["iID_MaTrangThaiDuyet"]);
            dt.Dispose();
            if (LuongCongViecModel.NguoiDung_DuocSuaChungTu(PhanHeModels.iID_MaPhanHeDuToan, MaND, iID_MaTrangThaiDuyet))
            {
                int iID_MaTrangThaiDuyet_TuChoi = LuongCongViecModel.Luong_iID_MaTrangThaiDuyet_TuChoi(iID_MaTrangThaiDuyet);
                if (iID_MaTrangThaiDuyet_TuChoi > 0)
                {
                    //SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM DT_ChungTuChiTiet WHERE iID_MaChungTu=@iID_MaChungTu AND bDongY=0");
                    //cmd.Parameters.AddWithValue("@iID_MaChungTu", MaChungTu);
                    //if (Convert.ToInt32(Connection.GetValue(cmd, 0)) > 0)
                    //{
                    vR = Convert.ToInt32(iID_MaTrangThaiDuyet_TuChoi);
                    //}
                    //cmd.Dispose();
                }
            }
            return vR;
        }

        public static int Get_iID_MaTrangThaiDuyet_Gom_TrinhDuyet(String MaND, String MaChungTu)
        {
            int vR = -1;
            DataTable dt = DuToan_ChungTuModels.GetChungTu_Gom(MaChungTu);
            int iID_MaTrangThaiDuyet = Convert.ToInt32(dt.Rows[0]["iID_MaTrangThaiDuyet"]);
            dt.Dispose();
            if (LuongCongViecModel.NguoiDung_DuocSuaChungTu(PhanHeModels.iID_MaPhanHeDuToan, MaND, iID_MaTrangThaiDuyet))
            {
                int iID_MaTrangThaiDuyet_TrinhDuyet = LuongCongViecModel.Luong_iID_MaTrangThaiDuyet_TrinhDuyet(iID_MaTrangThaiDuyet);
                if (iID_MaTrangThaiDuyet_TrinhDuyet > 0)
                {
                    vR = Convert.ToInt32(iID_MaTrangThaiDuyet_TrinhDuyet);
                }
            }
            return vR;
        }

        public static int Get_iID_MaTrangThaiDuyet_Gom_THCuc_TrinhDuyet(String MaND, String MaChungTu)
        {
            int vR = -1;
            DataTable dt = DuToan_ChungTuModels.GetChungTu_Gom_THCuc(MaChungTu);
            int iID_MaTrangThaiDuyet = Convert.ToInt32(dt.Rows[0]["iID_MaTrangThaiDuyet"]);
            dt.Dispose();
            if (LuongCongViecModel.NguoiDung_DuocSuaChungTu(PhanHeModels.iID_MaPhanHeDuToan, MaND, iID_MaTrangThaiDuyet))
            {
                int iID_MaTrangThaiDuyet_TrinhDuyet = LuongCongViecModel.Luong_iID_MaTrangThaiDuyet_TrinhDuyet(iID_MaTrangThaiDuyet);
                if (iID_MaTrangThaiDuyet_TrinhDuyet > 0)
                {
                    vR = Convert.ToInt32(iID_MaTrangThaiDuyet_TrinhDuyet);
                }
            }
            return vR;
        }
        public static int Get_iID_MaTrangThaiDuyet_Gom_THCuc_TuChoi(String MaND, String MaChungTu)
        {
            int vR = -1;
            DataTable dt = DuToan_ChungTuModels.GetChungTu_Gom_THCuc(MaChungTu);
            int iID_MaTrangThaiDuyet = Convert.ToInt32(dt.Rows[0]["iID_MaTrangThaiDuyet"]);
            dt.Dispose();
            if (LuongCongViecModel.NguoiDung_DuocSuaChungTu(PhanHeModels.iID_MaPhanHeDuToan, MaND, iID_MaTrangThaiDuyet))
            {
                int iID_MaTrangThaiDuyet_TrinhDuyet = LuongCongViecModel.Luong_iID_MaTrangThaiDuyet_TuChoi(iID_MaTrangThaiDuyet);
                if (iID_MaTrangThaiDuyet_TrinhDuyet > 0)
                {
                    vR = Convert.ToInt32(iID_MaTrangThaiDuyet_TrinhDuyet);
                }
            }
            return vR;
        }
        /// <summary>
        /// Lấy tổng dự toán đã duyệt của sLNS trong iNamLamViec
        /// </summary>
        /// <param name="sLNS"></param>
        /// <param name="iNamLamViec"></param>
        /// <param name="iID_MaNguonNganSach"></param>
        /// <param name="iID_MaNamNganSach"></param>
        /// <param name="bChiNganSach"></param>
        /// <returns></returns>
        public static DataTable LayTongDuToan(String sLNS,
                                              int iNamLamViec,
                                              String dNgayDotNganSach,
                                              int iID_MaNguonNganSach,
                                              int iID_MaNamNganSach,
                                              Boolean bChiNganSach, String iID_MaChiTieu)
        {
            DataTable vR;
            String SQL = "", DK = "";
            //nghiep edit : ko group ma hang muc cong trinh
            String[] arrDSTruongTien = MucLucNganSachModels.strDSTruongTien_So.Split(',');
            String strTruong = MucLucNganSachModels.strDSTruongSapXep + ",iID_MaMucLucNganSach";
            String strTruongGroup = MucLucNganSachModels.strDSTruongSapXep + ",iID_MaMucLucNganSach";

            for (int i = 0; i < arrDSTruongTien.Length; i++)
            {
                if (arrDSTruongTien[i].StartsWith("r"))
                {
                    strTruong += String.Format(",SUM({0}) AS Sum{0}", arrDSTruongTien[i]);
                }
                else
                {
                    strTruong += String.Format(",{0}", arrDSTruongTien[i]);
                    strTruongGroup += String.Format(",{0}", arrDSTruongTien[i]);
                }
            }

            int iID_MaTrangThaiDuyet_DaDuyet = LuongCongViecModel.Get_iID_MaTrangThaiDuyet_DaDuyet(PhanHeModels.iID_MaPhanHeDuToan);
            DK = " sLNS=@sLNS AND iID_MaChungTu IN ( SELECT iID_MaDuToan FROM PB_ChiTieu_DuToan WHERE iID_MaChiTieu=@iID_MaChiTieu)";
            //DK = "iTrangThai=1 AND " +
            //     "iID_MaTrangThaiDuyet=@iID_MaTrangThaiDuyet_DaDuyet AND " +
            //     "sLNS=@sLNS AND " +
            //     "iNamLamViec=@iNamLamViec AND " +
            //     "iID_MaNguonNganSach=@iID_MaNguonNganSach AND " +
            //     "iID_MaNamNganSach=@iID_MaNamNganSach AND " +
            //     "bChiNganSach=@bChiNganSach AND " +
            //     "iID_MaDotNganSach IN (SELECT iID_MaDotNganSach FROM DT_DotNganSach WHERE iNamLamViec=@iNamLamViec AND iID_MaNguonNganSach=@iID_MaNguonNganSach AND iID_MaNamNganSach=@iID_MaNamNganSach AND dNgayDotNganSach <= @dNgayDotNganSach)";

            SQL = String.Format("SELECT {0} FROM DT_ChungTuChiTiet WHERE {1} GROUP BY {2} ", strTruong, DK, strTruongGroup);
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@iID_MaChiTieu", iID_MaChiTieu);
            //cmd.Parameters.AddWithValue("@iID_MaTrangThaiDuyet_DaDuyet", iID_MaTrangThaiDuyet_DaDuyet);
            cmd.Parameters.AddWithValue("@sLNS", sLNS);
            //cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
            //cmd.Parameters.AddWithValue("@iID_MaNguonNganSach", iID_MaNguonNganSach);
            //cmd.Parameters.AddWithValue("@iID_MaNamNganSach", iID_MaNamNganSach);
            //cmd.Parameters.AddWithValue("@bChiNganSach", bChiNganSach);
            //cmd.Parameters.AddWithValue("@dNgayDotNganSach", String.Format("{0:MM/dd/yyyy}",Convert.ToDateTime(dNgayDotNganSach)));
            cmd.CommandText = SQL;
            vR = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return vR;
        }
        /// <summary>
        /// Xac dinh bao nhieu LNS
        /// </summary>
        /// <param name="sLNS"></param>
        /// <param name="isCheck"></param>
        /// <returns></returns>
        public static DataTable Get_LNS(String sLNS, ref int iCheck)
        {
            DataTable vR = null;
            String SQL;
            SQL =
                "select  distinct sL,sK from NS_MucLucNganSach where sLNS=@sLNS and sL<>'' and sK<>'' ";
            SqlCommand cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@sLNS", sLNS);
            vR = Connection.GetDataTable(cmd);
            iCheck = vR.Rows.Count;

            cmd.Dispose();
            return vR;
        }

        public static DataTable Get_LNS_80(String sLNS, ref int iCheck)
        {
            DataTable vR = null;
            String SQL;
            SQL =
                "select  distinct sL,sK,sM,sTM,sTTM from NS_MucLucNganSach where sLNS LIKE @sLNS and sL<>'' and sK<>'' ";
            SqlCommand cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@sLNS", sLNS + "%");
            vR = Connection.GetDataTable(cmd);
            iCheck = vR.Rows.Count;

            cmd.Dispose();
            return vR;
        }

        public static DataTable Get_RowChungTuChiTiet(String iID_MaChungTu)
        {
            DataTable vR = null;
            String SQL;
            SQL =
                "select  * from DT_ChungTuChiTiet where iID_MaChungTuChiTiet=@iID_MaChungTu";
            SqlCommand cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@iID_MaChungTu", iID_MaChungTu);
            vR = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return vR;
        }
        public static double dTongConLai(String iID_MaChungTu, String TruongTien)
        {
            String SQL = "";
            SqlCommand cmd;
            double dTong;
            SQL = String.Format(@"SELECT SUM({0})  FROM (
SELECT SUM({0}) as {0}
 FROM  DT_ChungTuChiTiet_PhanCap
 WHERE iTrangThai=1 AND iID_MaChungTu=@iID_MaChungTu

UNION
SELECT SUM({0}) as {0} 
 FROM  DT_ChungTuChiTiet
 WHERE iTrangThai=1 AND iID_MaChungTu=@iID_MaChungTu
) a", TruongTien);
            cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@iID_MaChungTu", iID_MaChungTu);
            dTong = Convert.ToDouble(Connection.GetValue(cmd, 0));
            return dTong;
        }
        public static Boolean CheckDonViBaoDam2Lan(String iID_MaDonVi)
        {
            String SQL = "";
            SqlCommand cmd;
            SQL = String.Format(@"SELECT COUNT(sTenKhoa) FROM DC_DanhMuc
WHERE sTenKhoa=@sTenKhoa AND iID_MaLoaiDanhMuc IN(
SELECT iID_MaLoaiDanhMuc FROM DC_LoaiDanhMuc
WHERE sTenBang='DVBDKT')");
            cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@sTenKhoa", iID_MaDonVi);
            int vR = Convert.ToInt16(Connection.GetValue(cmd, 0));
            if (vR > 0) return true;
            else return false;
        }



    }
}
