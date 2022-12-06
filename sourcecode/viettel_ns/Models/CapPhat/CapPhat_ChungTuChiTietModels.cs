using DomainModel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Viettel.Data;
using Viettel.Services;
using VIETTEL.Helpers;

namespace VIETTEL.Models
{
    public class CapPhat_ChungTuChiTietModels : CapPhatChungTuChiTietCha
    {
        private static DataTable _Chitiet;
        /// <summary>
        /// Hàm lấy dự liệu của bảng chức từ chi tiết để fill vào lưới nhập
        /// Dư liệu bao gồm:Lấy tất cả ngân sách từ bảng mục lục ngân sách left join với 
        /// bản chứng từ cấp phát chi tiết
        /// </summary>
        /// <param name="iID_MaCapPhat">Mã chứng từ cấp phát</param>
        /// <param name="arrGiaTriTimKiem">các giá trị tìm kiếm</param>
        /// <param name="MaND"></param>
        /// <returns></returns>
        public static DataTable LayDtChungTuChiTietCuc(string iID_MaCapPhat, Dictionary<String, String> arrGiaTriTimKiem, string MaND = "")
        {
            string sqlCapPhat = "Select * from Cp_CapPhat where iID_MaCapPhat = @iID_MaCapPhat";
            SqlCommand cmdCapPhat = new SqlCommand(sqlCapPhat);
            cmdCapPhat.Parameters.AddWithValue("@iID_MaCapPhat", iID_MaCapPhat);
            string sMucCap = Convert.ToString(Connection.GetDataTable(cmdCapPhat).Rows[0]["sLoai"]);
            string iNamLamViec = Convert.ToString(Connection.GetDataTable(cmdCapPhat).Rows[0]["iNamLamViec"]);
            string[] arrLNS = Convert.ToString(Connection.GetDataTable(cmdCapPhat).Rows[0]["sDSLNS"]).Split(',');
            cmdCapPhat.Dispose();
            DataTable vR;
            string[] arrDSTruong = (MucLucNganSachModels.strDSTruong + ",iID_MaDonVi").Split(',');

            using (var conn = ConnectionFactory.Default.GetConnection())
            // Chức năng sửa: cấp phát chứng từ
            // Người sửa: anhht
            // Nội dung sửa: sửa lại procedure, tăng tốc độ lấy dữ liệu
            using (var cmdvr = new SqlCommand("sp_get_cp_chungtuct_upgrade_v1", conn))
            {
                cmdvr.CommandType = CommandType.StoredProcedure;
                cmdvr.Parameters.AddWithValue("@id", iID_MaCapPhat.ToParamString());
                if (arrGiaTriTimKiem != null)
                {
                    for (int i = 0; i < arrDSTruong.Length; i++)
                    {
                        if (String.IsNullOrEmpty(arrGiaTriTimKiem[arrDSTruong[i]]) == false)
                        {
                            cmdvr.Parameters.AddWithValue("@" + arrDSTruong[i], arrGiaTriTimKiem[arrDSTruong[i]] + "%");
                        }
                        else if (arrDSTruong[i] != "sMoTa" && arrDSTruong[i] != "sTNG")
                        {
                            cmdvr.Parameters.AddWithValue("@" + arrDSTruong[i], DBNull.Value);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < arrDSTruong.Length; i++)
                    {
                        if (String.IsNullOrEmpty(arrGiaTriTimKiem[arrDSTruong[i]]) == true && arrDSTruong[i] != "sMoTa" && arrDSTruong[i] != "sTNG")
                        {
                            cmdvr.Parameters.AddWithValue("@" + arrDSTruong[i], DBNull.Value);
                        }
                    }
                }

                vR = cmdvr.GetDataset().Tables[0];
            }
            

            if (sMucCap == "sNG" && arrLNS.Length > 0)
            {
                //neu la bao dam lay danh sach theo cau hinh nguoi dung-nganh
                //neu la tro ly phong ban
                bool check = LuongCongViecModel.KiemTra_TroLyTongHop(MaND);
                bool bTrolyTHCuc = LuongCongViecModel.KiemTra_TroLyTongHopCuc(MaND);
                if (check == false && bTrolyTHCuc == false)
                {
                    if (arrLNS[0].Equals("1040100"))
                    {
                        String DKLoaiNg = "";
                        DKLoaiNg    += " AND sMaNguoiQuanLy like '%" + MaND + "%'";
                        string SQL  = String.Format($"SELECT iID_MaNganhMLNS FROM NS_MucLucNganSach_Nganh WHERE iTrangThai=1 and iNamLamViec = @iNamLamViec {DKLoaiNg}");
                        SqlCommand cmdBD = new SqlCommand();
                        cmdBD.CommandText = SQL;
                        cmdBD.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);

                        var dt = Connection.GetDataTable(cmdBD);
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
                    }
                }// end if check MaND
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
                        if (iID_MaMucLucNganSach == Convert.ToString(vR.Rows[j]["iID_MaMucLucNganSach"]) && Convert.ToString(vR.Rows[j]["iID_MaDonVi"]) != "")
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

            return vR;
        }
        public static DataTable LayDtChungTuChiTietCucOld(String iID_MaCapPhat, Dictionary<String, String> arrGiaTriTimKiem, String MaND = "")
        {
            // HungPX: lấy giá trị chi tiết đến của chứng từ
            DataTable dtCapPhat;
            SqlCommand cmdCapPhat = new SqlCommand();
            String sqlCapPhat = "Select * from Cp_CapPhat where iID_MaCapPhat = @iID_MaCapPhat";
            cmdCapPhat.Parameters.AddWithValue("@iID_MaCapPhat", iID_MaCapPhat);
            cmdCapPhat.CommandText = sqlCapPhat;
            dtCapPhat = Connection.GetDataTable(cmdCapPhat);
            String sLoai = Convert.ToString(dtCapPhat.Rows[0]["sLoai"]);
            String sMucCap = Convert.ToString(dtCapPhat.Rows[0]["sLoai"]);
            dtCapPhat.Dispose();
            cmdCapPhat.Dispose();

            //HungPX : điều kiện chỉ lấy những hàng chứng từ nhập chi tiết đến "sLoai"
            String DKsLoai = "";
            //if (string.IsNullOrEmpty(sLoai) || sLoai != "sNG")
            //{
            //    int IndexChiTietDen = CapPhat_BangDuLieu.getIndex(sLoai);
            //    String TenLoaiCon = MucLucNganSachModels.arrDSTruong[IndexChiTietDen + 1];
            //    DKsLoai += String.Format(@"AND {0} = '' ", TenLoaiCon);
            //}

            DataTable vR;
            DataTable arrDV = NganSach_HamChungModels.DSDonViCuaNguoiDung(MaND);
            DataTable dtChungTu = CapPhat_ChungTuModels.LayToanBoThongTinChungTu(iID_MaCapPhat);
            DataRow RChungTu = dtChungTu.Rows[0];
            DateTime ngayCT = Convert.ToDateTime(RChungTu["dNgayCapPhat"]);
            String iID_MaDonVi = Convert.ToString(RChungTu["iID_MaDonVi"]);
            String iLoai = Convert.ToString(RChungTu["iLoai"]);
            String sDSLNS = Convert.ToString(RChungTu["sDSLNS"]);
            String iNamLamViec = Convert.ToString(RChungTu["iNamLamViec"]);
            String iID_MaPhongBan = Convert.ToString(RChungTu["iID_MaPhongBan"]);
            String iID_MaTinhChatCapThu = Convert.ToString(RChungTu["iID_MaTinhChatCapThu"]);
            String sTruongTien = "rTuChi,iID_MaCapPhatChiTiet,iID_MaDonVi,sTenDonVi,rTuChi_PhanBo,rTuChi_DaCap";
            String[] arrDSTruong = (MucLucNganSachModels.strDSTruong + ",iID_MaDonVi").Split(',');
            String[] arrDSTruongTien = sTruongTien.Split(',');
            String[] arrLNS = sDSLNS.Split(',');

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
                        DK += String.Format(" AND {0} LIKE @{0}", arrDSTruong[i]);
                        cmd.Parameters.AddWithValue("@" + arrDSTruong[i], arrGiaTriTimKiem[arrDSTruong[i]] + "%");
                    }
                }
            }

            String DKLoai = "";
            //ngan sach quoc phong tung loai ns 

            if (arrLNS.Length > 0)
            {

                DKLoai += "sLNS IN (select * from f_split(@sDSLNS))";
                cmd.Parameters.AddWithValue("@sDSLNS", sDSLNS);

                if (iID_MaTinhChatCapThu != "1" && iID_MaTinhChatCapThu != "2")
                {
                    //lay danh sach LNS co chi tieu
                    //SQL = String.Format(@"select distinct(sLNS) from f_ns_table_chitieu(@iNamLamViec,@iID_MaDonVi, @iID_MaPhongBan,@iID_MaNamNganSach,@dMaDot,1,'1,2')                                                                                                                               
                    //                      where {0}", DKLoai);

                    SQL = String.Format(@"select distinct(sLNS) from f_ns_table_chitieu_all(@iNamLamViec,@iID_MaDonVi, @iID_MaPhongBan,@iID_MaNamNganSach,@dMaDot,1,'1,2')                                                                                                                               
                                          where {0}", DKLoai);

                    SqlCommand cmd1 = new SqlCommand(SQL);
                    if (sDSLNS.Contains("1040200") || sDSLNS.Contains("1040300"))
                    {
                        cmd1.Parameters.AddWithValue("@sDSLNS", sDSLNS + ",1040100");
                    }
                    else
                    {
                        cmd1.Parameters.AddWithValue("@sDSLNS", sDSLNS);
                    }
                    cmd1.Parameters.AddWithValue("@iID_MaDonVi", string.Join(",", arrDV.Rows.OfType<DataRow>().Select(r => r["iID_MaDonVi"].ToString())));
                    cmd1.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                    cmd1.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan);
                    cmd1.Parameters.AddWithValue("@iID_MaNamNganSach", RChungTu["iID_MaNamNganSach"]);
                    cmd1.Parameters.AddWithValue("@dMaDot", DateTime.Now);

                    cmd1.CommandTimeout = 600;
                    DataTable dtLNSChiTieu = Connection.GetDataTable(cmd1);

                    string lnsCT = string.Join(",", dtLNSChiTieu.Rows.OfType<DataRow>().Select(r => r["sLNS"].ToString()));
                    if (lnsCT.Contains("1040100"))
                    {
                        lnsCT += ",1040200,1040300";
                    }

                    String DKLNSChiTieu = "AND (sLNS IN (select * from f_split(@sDSLNSCT)) OR sLNS IN (select LEFT(Item,3) from f_split(@sDSLNSCT)))";
                    cmd.Parameters.AddWithValue("@sDSLNSCT", lnsCT);
                    DKLoai = String.Format("and ({0} OR sLNS IN (select LEFT(Item,3) from f_split(@sDSLNS))) {1}", DKLoai, DKLNSChiTieu);
                }
                else
                {
                    DKLoai = String.Format("and ({0} OR sLNS IN (select LEFT(Item,3) from f_split(@sDSLNS)))", DKLoai);
                }
            }
            else
                DKLoai += String.Format("and sLNS like '---%'");


            SQL = String.Format("SELECT iID_MaMucLucNganSach,iID_MaMucLucNganSach_Cha,bLaHangCha,sXauNoiMa,{0},{1} FROM NS_MucLucNganSach WHERE iNamLamViec=@iNamLamViec AND {2} {4} {5} ORDER BY {3}", MucLucNganSachModels.strDSTruong, MucLucNganSachModels.strDSDuocNhapTruongTien, DK, MucLucNganSachModels.strDSTruongSapXep, DKLoai, DKsLoai);

            //SQL = $"SELECT * FROM f_mlns_full(@iNamLamViec) WHERE {DK} {DKLoai} {DKsLoai} ORDER BY sXauNoiMa";

            cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
            cmd.CommandText = SQL;
            vR = Connection.GetDataTable(cmd);

            if (sMucCap == "sNG" && arrLNS.Length > 0)
            {
                //neu la bao dam lay danh sach theo cau hinh nguoi dung-nganh
                //neu la tro ly phong ban
                bool check = LuongCongViecModel.KiemTra_TroLyTongHop(MaND);
                bool bTrolyTHCuc = LuongCongViecModel.KiemTra_TroLyTongHopCuc(MaND);
                if (check == false && bTrolyTHCuc == false)
                {
                    if (arrLNS[0].Equals("1040100"))
                    {
                        String DKLoaiNg = "";
                        DKLoaiNg += " AND sMaNguoiQuanLy like '%" + MaND + "%'";
                        SQL = String.Format(@"SELECT iID_MaNganhMLNS FROM NS_MucLucNganSach_Nganh
                            WHERE iTrangThai=1 and iNamLamViec = @iNamLamViec {0}", DKLoaiNg);
                        SqlCommand cmdBD = new SqlCommand();
                        cmdBD.CommandText = SQL;
                        cmdBD.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);

                        var dt = Connection.GetDataTable(cmdBD);
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
            cmd.Dispose();
            //Lay toan bo Muc luc ngan sach-->
            DataColumn column;

            for (int j = 0; j < arrDSTruongTien.Length; j++)
            {
                column = new DataColumn(arrDSTruongTien[j], typeof(String));
                if (arrDSTruongTien[j] == "rTuChi_PhanBo" || arrDSTruongTien[j] == "rTuChi_DaCap" || arrDSTruongTien[j] == "rTuChi")
                {
                    column.DefaultValue = 0;
                }
                column.AllowDBNull = true;
                vR.Columns.Add(column);
            }


            //HungPX: Lấy thông tin cột tiền đã cấp
            //DataRow dr = dtChungTu.Rows[0];

            //Dien don vi
            if (iID_MaTinhChatCapThu != "1" && iID_MaTinhChatCapThu != "2")
            {
                String iID_MaDonVi_TK = arrGiaTriTimKiem["iID_MaDonVi"];
                int count1 = vR.Rows.Count;

                if (!String.IsNullOrEmpty(iID_MaDonVi))
                {
                    for (int i = arrDV.Rows.Count - 1; i >= 0; i--)
                    {
                        if (Convert.ToString(arrDV.Rows[i]["iID_MaDonVi"]) != iID_MaDonVi)
                            arrDV.Rows.RemoveAt(i);
                    }
                    for (int j = 0; j < count1; j++)
                    {
                        DataRow dr = vR.Rows[j];
                        if (sMucCap == "sM" && dr["sM"] != "")
                        {
                            dr["iID_MaDonVi"] = arrDV.Rows[0]["iID_MaDonVi"];
                            dr["sTenDonVi"] = arrDV.Rows[0]["sTen"];

                        }
                        if (sMucCap == "sTM" && dr["sTM"] != "")
                        {
                            dr["iID_MaDonVi"] = arrDV.Rows[0]["iID_MaDonVi"];
                            dr["sTenDonVi"] = arrDV.Rows[0]["sTen"];

                        }
                        if (sMucCap == "sNG" && dr["sNG"] != "")
                        {
                            dr["iID_MaDonVi"] = arrDV.Rows[0]["iID_MaDonVi"];
                            dr["sTenDonVi"] = arrDV.Rows[0]["sTen"];

                        }
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
                    for (int j = 0; j < count1; j++)
                    {
                        DataRow dr = vR.Rows[j];
                        if (sMucCap == "sM" && dr["sM"] != "")
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
                        if (sMucCap == "sTM" && dr["sTM"] != "")
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
                        if (sMucCap == "sNG" && dr["sNG"] != "")
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
                }

                DataView dv = vR.DefaultView;
                if (sMucCap == "sM")
                {
                    dv.Sort = "sLNS,sL,sK,sM,iID_MaDonVi";
                }
                else
                {
                    dv.Sort = "sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi";
                }
                vR = dv.ToTable();
            }

            cmd = new SqlCommand();
            //Lay Du Lieu Trong Bang CP_CapPhatChiTiet
            DK = "iTrangThai=1 AND iID_MaCapPhat=@iID_MaCapPhat AND rTuChi<>0";
            cmd.Parameters.AddWithValue("@iID_MaCapPhat", iID_MaCapPhat);
            SQL = String.Format("SELECT * FROM CP_CapPhatChiTiet WHERE {0} ORDER BY sXauNoiMa", DK);
            cmd.CommandText = SQL;

            DataTable dtChungTuChiTiet = Connection.GetDataTable(cmd);
            if (iID_MaTinhChatCapThu == "1" || iID_MaTinhChatCapThu == "2")
            {
                arrDSTruong = (MucLucNganSachModels.strDSTruong).Split(',');
            }
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
            if (iID_MaTinhChatCapThu != "1" && iID_MaTinhChatCapThu != "2")
            {
                LayCapPhatDaCap(iID_MaCapPhat, vR);
                LayChiTieu(iID_MaCapPhat, vR);
            }

            dtChungTu.Dispose();
            dtChungTuChiTiet.Dispose();
            cmd.Dispose();
            return vR;
        }
        private static void LayChiTieu(String iID_MaCapPhat, DataTable vR)
        {
            NameValueCollection data = CapPhat_ChungTuModels.LayThongTin(iID_MaCapPhat);
            DataTable dtChiTieu = new DataTable();

            #region definition input chitieu

            var sql = FileHelpers.GetSqlQuery("dtCapPhat_Tien.sql");

            #endregion

            #region dieu kien chitieu  

            String sLNSct = data["sDSLNS"];
            if (data["iLoai"] == "1")
            {
                if (!String.IsNullOrEmpty(data["sDSLNS"]))
                {
                    if (sLNSct.Contains("1040200") || sLNSct.Contains("1040300"))
                        sLNSct += ",1040100";
                    if (sLNSct.Contains("1020100"))
                    {
                        sLNSct += ",1020100,1020000";
                    }
                }
            }

            if (data["sLoai"] == "sTM")
            {
                sql = sql.Replace("@@MucCap", "sM,sTM");
            }
            else if (data["sLoai"] == "sM")
            {
                sql = sql.Replace("@@MucCap", "sM");
            }
            else
            {
                sql = sql.Replace("@@MucCap", "sM,sTM,sTTM,sNG");
            }

            var dkDonVi = new NganSachService().GetDonviListByUser(System.Web.HttpContext.Current.User.Identity.Name, data["iNamLamViec"])
                        .ToDictionary(x => x.iID_MaDonVi, x => x.iID_MaDonVi + " - " + x.sTen).Select(x => x.Key).Join();
            #endregion

            #region get data chitieu

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", data["iID_MaPhongBan"]);
                cmd.Parameters.AddWithValue("@iNamLamViec", data["iNamLamViec"]);
                //cmd.Parameters.AddWithValue("@iID_MaNguonNganSach", data["iID_MaNguonNganSach"]);
                //cmd.Parameters.AddWithValue("@iID_MaNamNganSach", data["iID_MaNamNganSach"]);
                if (String.IsNullOrEmpty(data["iID_NguonDuToan"]))
                {
                    cmd.Parameters.AddWithValue("@loai", DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@loai", data["iID_NguonDuToan"]);
                }
                cmd.Parameters.AddWithValue("@sLNS", sLNSct);
                cmd.Parameters.AddWithValue("@iID_MaDonVi", dkDonVi);

                dtChiTieu = cmd.GetTable();
            }
            #endregion

            int count = vR.Rows.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                String sLNS = Convert.ToString(vR.Rows[i]["sLNS"]);
                String sL = Convert.ToString(vR.Rows[i]["sL"]);
                String sK = Convert.ToString(vR.Rows[i]["sK"]);
                String sM = Convert.ToString(vR.Rows[i]["sM"]);
                String sTM = Convert.ToString(vR.Rows[i]["sTM"]);
                String sTTM = Convert.ToString(vR.Rows[i]["sTTM"]);
                String sNG = Convert.ToString(vR.Rows[i]["sNG"]);
                String iID_MaDonVi = Convert.ToString(vR.Rows[i]["iID_MaDonVi"]);
                String tienCap = Convert.ToString(vR.Rows[i]["rTuChi"]);
                String tienDaCap = Convert.ToString(vR.Rows[i]["rTuChi_DaCap"]);
                DataRow[] dr, dr1;
                if (data["sLoai"] == "sM")
                {
                    dr = dtChiTieu.Select("sLNS='" + sLNS + "' AND sL='" + sL + "' AND sK='" + sK + "' AND sM='" + sM + "' AND iID_MaDonVi='" + iID_MaDonVi + "'");
                    dr1 = dtChiTieu.Select("sLNS='" + sLNS + "' AND sL='" + sL + "' AND sK='" + sK + "' AND sM='" + sM + "'");
                }
                else if (data["sLoai"] == "sTM")
                {
                    dr = dtChiTieu.Select("sLNS='" + sLNS + "' AND sL='" + sL + "' AND sK='" + sK + "' AND sM='" + sM + "' AND sTM='" + sTM + "' AND iID_MaDonVi='" + iID_MaDonVi + "'");
                    dr1 = dtChiTieu.Select("sLNS='" + sLNS + "' AND sL='" + sL + "' AND sK='" + sK + "' AND sM='" + sM + "' AND sTM='" + sTM + "'");
                }
                else
                {
                    dr = dtChiTieu.Select("sLNS='" + sLNS + "' AND sL='" + sL + "' AND sK='" + sK + "' AND sM='" + sM + "' AND sTM='" + sTM + "' AND sTTM='" + sTTM + "' AND sNG='" + sNG + "' AND iID_MaDonVi='" + iID_MaDonVi + "'");
                    dr1 = dtChiTieu.Select("sLNS='" + sLNS + "' AND sL='" + sL + "' AND sK='" + sK + "' AND sM='" + sM + "' AND sTM='" + sTM + "' AND sTTM='" + sTTM + "' AND sNG='" + sNG + "'");
                }
                if (dr.Length > 0)
                {
                    //gan vao bang chung tu chi tiet VR
                    DataRow row1 = vR.Rows[i];
                    row1["rTuChi_PhanBo"] = Convert.ToDouble(dr[0]["rTuChi"]);
                }
                else if (dr1.Length == 0 && (!String.IsNullOrEmpty(tienCap) && !String.IsNullOrEmpty(tienDaCap)) && tienCap == "0" && tienDaCap == "0")
                {
                    if (data["sLoai"] == "sM")
                    {
                        if (Convert.ToString(vR.Rows[i]["sM"]) != "")
                        {
                            vR.Rows.RemoveAt(i);
                        }
                    }
                    if (data["sLoai"] == "sTM")
                    {
                        if (Convert.ToString(vR.Rows[i]["sTM"]) != "")
                        {
                            vR.Rows.RemoveAt(i);
                        }
                    }
                    if (data["sLoai"] == "sNG")
                    {
                        if (Convert.ToString(vR.Rows[i]["sNG"]) != "")
                        {
                            vR.Rows.RemoveAt(i);
                        }
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(tienCap) && !String.IsNullOrEmpty(tienDaCap) && tienCap == "0" && tienDaCap == "0")
                    {
                        if (data["sLoai"] == "sM")
                        {
                            if (Convert.ToString(vR.Rows[i]["sM"]) != "" && Convert.ToString(vR.Rows[i]["iID_MaDonVi"]) != "")
                            {
                                vR.Rows.RemoveAt(i);
                            }
                        }
                        if (data["sLoai"] == "sTM")
                        {
                            if (Convert.ToString(vR.Rows[i]["sTM"]) != "" && Convert.ToString(vR.Rows[i]["iID_MaDonVi"]) != "")
                            {
                                vR.Rows.RemoveAt(i);
                            }
                        }
                        if (data["sLoai"] == "sNG")
                        {
                            if (Convert.ToString(vR.Rows[i]["sNG"]) != "" && Convert.ToString(vR.Rows[i]["iID_MaDonVi"]) != "")
                            {
                                vR.Rows.RemoveAt(i);
                            }
                        }
                    }
                }
            }

        }
        private static void LayCapPhatDaCap(String iID_MaCapPhat, DataTable vR)
        {
            NameValueCollection data = CapPhat_ChungTuModels.LayThongTin(iID_MaCapPhat);
            DateTime ngayCT = Convert.ToDateTime(data["dNgayCapPhat"]);
            DataTable dtCapPhat = new DataTable();

            #region definition input dacap

            var sql = FileHelpers.GetSqlQuery("dtCapPhat_DaCap.sql");

            #endregion

            #region dieu kien dacap  

            String sLNSct = data["sDSLNS"];
            if (data["iLoai"] == "1")
            {
                if (!String.IsNullOrEmpty(data["sDSLNS"]))
                {
                    if (sLNSct.Contains("1040200") || sLNSct.Contains("1040300"))
                        sLNSct += ",1040100,1040200,1040300";
                    if (sLNSct.Contains("1020100"))
                    {
                        sLNSct += ",1020100,1020000";
                    }
                }
            }

            if (data["sLoai"] == "sTM")
            {
                sql = sql.Replace("@@select", @"SUBSTRING(sLNS,1,1) as sLNS1
                                            , SUBSTRING(sLNS, 1, 3) as sLNS3
                                            , sLNS5 = CASE WHEN sLNS = 1020000 then 10201 else SUBSTRING(sLNS, 1, 5) END
                                            , sLNS = CASE WHEN sLNS = 1020000 then 1020100 else sLNS END
                                            , sL
                                            , sK
                                            , sM
                                            , sTM");
                sql = sql.Replace("@@MucCap", @"sLNS
                                                , sL
                                                , sK
                                                , sM
                                                , sTM");
            }
            else if (data["sLoai"] == "sM")
            {
                sql = sql.Replace("@@select", @"SUBSTRING(sLNS,1,1) as sLNS1
                                            , SUBSTRING(sLNS, 1, 3) as sLNS3
                                            , sLNS5 = CASE WHEN sLNS = 1020000 then 10201 else SUBSTRING(sLNS, 1, 5) END
                                            , sLNS = CASE WHEN sLNS = 1020000 then 1020100 else sLNS END
                                            , sL
                                            , sK
                                            , sM
                                            ");
                sql = sql.Replace("@@MucCap", @"sLNS
                                                , sL
                                                , sK
                                                , sM");
            }
            else
            {
                sql = sql.Replace("@@select", @"SUBSTRING(sLNS,1,1) as sLNS1
                                            , SUBSTRING(sLNS, 1, 3) as sLNS3
                                            , sLNS5 = CASE WHEN sLNS = 1020000 then 10201 else SUBSTRING(sLNS, 1, 5) END
                                            , sLNS = CASE WHEN sLNS = 1020000 then 1020100 else sLNS END
                                            , sL
                                            , sK
                                            , sM
                                            , sTM
                                            , sTTM
                                            , sNG");
                sql = sql.Replace("@@MucCap", @"sLNS
                                                , sL
                                                , sK
                                                , sM
                                                , sTM
                                                , sTTM
                                                , sNG");
            }
            sql = sql.Replace("@@Tien_HienVat", @"( @sLNS IS NULL OR sLNS IN (SELECT * FROM F_Split(@sLNS))) AND 1 ");

            #endregion

            #region get data dacap

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", data["iID_MaPhongBan"]);
                cmd.Parameters.AddWithValue("@iNamLamViec", data["iNamLamViec"]);
                cmd.Parameters.AddWithValue("@iID_MaNguonNganSach", data["iID_MaNguonNganSach"]);
                cmd.Parameters.AddWithValue("@iID_MaNamNganSach", data["iID_MaNamNganSach"]);
                cmd.Parameters.AddWithValue("@sLNS", sLNSct);
                cmd.Parameters.AddWithValue("@dNgayCapPhat", ngayCT);
                cmd.Parameters.AddWithValue("@iSoCapPhat", data["iSoCapPhat"]);

                dtCapPhat = cmd.GetTable();
            }
            #endregion

            for (int i = 0; i < vR.Rows.Count; i++)
            {
                String sLNS = Convert.ToString(vR.Rows[i]["sLNS"]);
                String sL = Convert.ToString(vR.Rows[i]["sL"]);
                String sK = Convert.ToString(vR.Rows[i]["sK"]);
                String sM = Convert.ToString(vR.Rows[i]["sM"]);
                String sTM = Convert.ToString(vR.Rows[i]["sTM"]);
                String sTTM = Convert.ToString(vR.Rows[i]["sTTM"]);
                String sNG = Convert.ToString(vR.Rows[i]["sNG"]);
                String iID_MaDonVi = Convert.ToString(vR.Rows[i]["iID_MaDonVi"]);
                DataRow[] dr;
                if (data["sLoai"] == "sM")
                {
                    dr = dtCapPhat.Select("sLNS='" + sLNS + "' AND sL='" + sL + "' AND sK='" + sK + "' AND sM='" + sM + "' AND iID_MaDonVi='" + iID_MaDonVi + "'");
                }
                else if (data["sLoai"] == "sTM")
                {
                    dr = dtCapPhat.Select("sLNS='" + sLNS + "' AND sL='" + sL + "' AND sK='" + sK + "' AND sM='" + sM + "' AND sTM='" + sTM + "' AND iID_MaDonVi='" + iID_MaDonVi + "'");
                }
                else
                {
                    dr = dtCapPhat.Select("sLNS='" + sLNS + "' AND sL='" + sL + "' AND sK='" + sK + "' AND sM='" + sM + "' AND sTM='" + sTM + "' AND sTTM='" + sTTM + "' AND sNG='" + sNG + "' AND iID_MaDonVi='" + iID_MaDonVi + "'");
                }
                if (dr.Length > 0)
                {
                    //gan vao bang chung tu chi tiet VR
                    DataRow row1 = vR.Rows[i];
                    row1["rTuChi_DaCap"] = dr[0]["rTuChi"];
                }
            }

        }
        /// <summary>
        /// Hàm lấy tổng đã cấp phát cho đơn vị 
        /// Dùng trong trường hợp người dùng nhập một đơn vị mới vào chứng từ chi tiết 
        /// thì hệ thống phải load thông tin đã cấp phát của đơn vị đó lên lưới
        /// </summary>
        /// <param name="iDM_MaLoaiCapPhat"></param>
        /// <param name="iID_MaTinhChatCapThu"></param>
        /// <param name="sDSLNS"></param>
        /// <param name="iID_MaMucLucNganSach"></param>
        /// <param name="iID_MaDonVi"></param>
        /// <param name="iNamLamViec"></param>
        /// <param name="dNgayCapPhat"></param>
        /// <param name="iID_MaNguonNganSach"></param>
        /// <param name="iID_MaNamNganSach"></param>
        /// <returns></returns>
        public static DataTable LayDtTongCapPhatChoDonVi(String iID_MaMucLucNganSach,
                                                        String iID_MaDonVi,
                                                        NameValueCollection data)
        {
            SqlCommand cmd = new SqlCommand();
            String DK = "iTrangThai = 1";
            //DK += " AND iID_MaTrangThaiDuyet=@iID_MaTrangThaiDuyet";
            //cmd.Parameters.AddWithValue("@iID_MaTrangThaiDuyet", LuongCongViecModel.Get_iID_MaTrangThaiDuyet_DaDuyet(CapPhatModels.iID_MaPhanHe));
            DK += " AND iNamLamViec=@iNamLamViec";
            cmd.Parameters.AddWithValue("@iNamLamViec", data["iNamLamViec"]);
            DK += " AND iID_MaNguonNganSach=@iID_MaNguonNganSach";
            cmd.Parameters.AddWithValue("@iID_MaNguonNganSach", data["iID_MaNguonNganSach"]);
            DK += " AND iID_MaNamNganSach=@iID_MaNamNganSach";
            cmd.Parameters.AddWithValue("@iID_MaNamNganSach", data["iID_MaNamNganSach"]);

            //   DK += " AND iDM_MaLoaiCapPhat=@iDM_MaLoaiCapPhat";
            //  cmd.Parameters.AddWithValue("@iDM_MaLoaiCapPhat", iDM_MaLoaiCapPhat);
            //  DK += " AND iID_MaTinhChatCapThu=@iID_MaTinhChatCapThu";
            //  cmd.Parameters.AddWithValue("@iID_MaTinhChatCapThu", iID_MaTinhChatCapThu);
            //DK += " AND sLNS=@sLNS";
            //cmd.Parameters.AddWithValue("@sLNS", sDSLNS);

            DataTable vR = new DataTable();
            if (!string.IsNullOrEmpty(iID_MaMucLucNganSach))
            {
                DK += " AND iID_MaMucLucNganSach=@iID_MaMucLucNganSach";
                cmd.Parameters.AddWithValue("@iID_MaMucLucNganSach", iID_MaMucLucNganSach);
            }
            if (!string.IsNullOrEmpty(iID_MaDonVi))
            {
                DK += " AND iID_MaDonVi=@iID_MaDonVi";
                cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
                DK += " AND iID_MaCapPhat IN (SELECT iID_MaCapPhat FROM CP_CapPhat WHERE ((dNgayCapPhat < @dNgayCapPhat)  OR (dNgayCapPhat=@dNgayCapPhat AND iSoCapPhat<@iSoCapPhat)) )";
                cmd.Parameters.AddWithValue("@dNgayCapPhat", Convert.ToDateTime(data["dNgayCapPhat"]));
                cmd.Parameters.AddWithValue("@iSoCapPhat", data["iSoCapPhat"]);

                String[] arrDSTruongTien_So = MucLucNganSachModels.strDSTruongTien_So.Split(',');
                String strTruong = "";

                strTruong += String.Format("SUM(rTuChi) AS SumrTuChi");


                cmd.CommandText = String.Format("SELECT {0} FROM CP_CapPhatChiTiet WHERE {1}", strTruong, DK);
                vR = Connection.GetDataTable(cmd);
            }

            cmd.Dispose();
            return vR;
        }


        public static DataTable LayTongCapPhat(String iID_MaCapPhat)
        {
            SqlCommand cmd = new SqlCommand();
            String DK = "iTrangThai = 1";
            NameValueCollection data = CapPhat_ChungTuModels.LayThongTin(iID_MaCapPhat);

            //DK += " AND iID_MaTrangThaiDuyet=@iID_MaTrangThaiDuyet";
            //cmd.Parameters.AddWithValue("@iID_MaTrangThaiDuyet", LuongCongViecModel.Get_iID_MaTrangThaiDuyet_DaDuyet(CapPhatModels.iID_MaPhanHe));
            DK += " AND iNamLamViec=@iNamLamViec";
            cmd.Parameters.AddWithValue("@iNamLamViec", data["iNamLamViec"]);
            DK += " AND iID_MaNguonNganSach=@iID_MaNguonNganSach";
            cmd.Parameters.AddWithValue("@iID_MaNguonNganSach", data["iID_MaNguonNganSach"]);
            DK += " AND iID_MaNamNganSach=@iID_MaNamNganSach";
            cmd.Parameters.AddWithValue("@iID_MaNamNganSach", data["iID_MaNamNganSach"]);

            //   DK += " AND iDM_MaLoaiCapPhat=@iDM_MaLoaiCapPhat";
            //  cmd.Parameters.AddWithValue("@iDM_MaLoaiCapPhat", iDM_MaLoaiCapPhat);
            //  DK += " AND iID_MaTinhChatCapThu=@iID_MaTinhChatCapThu";
            //  cmd.Parameters.AddWithValue("@iID_MaTinhChatCapThu", iID_MaTinhChatCapThu);
            //DK += " AND sLNS=@sLNS";
            //cmd.Parameters.AddWithValue("@sLNS", sDSLNS);

            //if (!string.IsNullOrEmpty(iID_MaMucLucNganSach))
            //{
            //    DK += " AND iID_MaMucLucNganSach=@iID_MaMucLucNganSach";
            //    cmd.Parameters.AddWithValue("@iID_MaMucLucNganSach", iID_MaMucLucNganSach);
            //}
            //if (!string.IsNullOrEmpty(iID_MaDonVi))
            //{
            //    DK += " AND iID_MaDonVi=@iID_MaDonVi";
            //    cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
            //}
            //DK += " AND iID_MaCapPhat IN (SELECT iID_MaCapPhat FROM CP_CapPhat WHERE ((dNgayCapPhat < @dNgayCapPhat)  OR (dNgayCapPhat=@dNgayCapPhat AND iSoCapPhat<@iSoCapPhat)) )";
            //cmd.Parameters.AddWithValue("@dNgayCapPhat", dNgayCapPhat);
            //cmd.Parameters.AddWithValue("@iSoCapPhat", iSoCapPhat);

            String[] arrDSTruongTien_So = MucLucNganSachModels.strDSTruongTien_So.Split(',');
            String strTruong = "";
            for (int i = 0; i < arrDSTruongTien_So.Length; i++)
            {
                if (i > 0) strTruong += ",";
                strTruong += String.Format("SUM({0}) AS Sum{0}", arrDSTruongTien_So[i]);
            }

            cmd.CommandText = String.Format("SELECT {0} FROM CP_CapPhatChiTiet WHERE {1}", strTruong, DK);
            DataTable vR = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return vR;
        }

        public static DataTable Get_dtTongChiTieuChoDonVi(
                                                      String iID_MaDonVi,
                                                      int iNamLamViec,
                                                      int iID_MaNguonNganSach,
                                                      int iID_MaNamNganSach,
                                                      String iID_MaPhongBan,
                                                      String dNgayChungTu, String sLNS, String iLoai, String sMucCap
                                                       )
        {
            SqlCommand cmd = new SqlCommand();
            String DK = "iTrangThai = 1";
            if (iLoai == "1")
            {
                if (!String.IsNullOrEmpty(sLNS))
                {
                    if ("1020100".Equals(sLNS))
                    {
                        sLNS = "1020100,1020000";
                    }
                    String[] arrsLNS = sLNS.Split(',');
                    DK += " AND (";
                    for (int i = 0; i < arrsLNS.Length; i++)
                    {
                        DK += "sLNS = @sLNS" + i;
                        cmd.Parameters.AddWithValue("@sLNS" + i, arrsLNS[i]);
                        if (i < arrsLNS.Length - 1)
                            DK += " OR ";
                    }
                    DK += " )";
                }
            }
            //ngan sach dac biet
            else if (iLoai == "0")
            {
                DK += " AND ( sLNS like '4%')";
            }
            else
            {
                DK += " AND ( sLNS like '2%')";
            }
            //DK += " AND iID_MaTrangThaiDuyet=@iID_MaTrangThaiDuyet";
            //cmd.Parameters.AddWithValue("@iID_MaTrangThaiDuyet", LuongCongViecModel.Get_iID_MaTrangThaiDuyet_DaDuyet(QuyetToanModels.iID_MaPhanHeQuyetToan));
            String[] arrDonVi = iID_MaDonVi.Split(',');
            if (arrDonVi.Length > 0)
            {
                DK = DK + " AND ( ";
                for (int i = 0; i < arrDonVi.Length; i++)
                {
                    DK += "iID_MaDonVi=@iID_MaDonVi" + i;
                    if (i < arrDonVi.Length - 1)
                        DK += " OR ";
                    cmd.Parameters.AddWithValue("@iID_MaDonVi" + i, arrDonVi[i]);
                }
                DK = DK + " ) ";

            }
            DK += " AND iNamLamViec=@iNamLamViec";
            cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
            DK += " AND iID_MaNguonNganSach=@iID_MaNguonNganSach";
            cmd.Parameters.AddWithValue("@iID_MaNguonNganSach", iID_MaNguonNganSach);

            if (iID_MaNamNganSach == 2)
            {
                DK += " AND iID_MaNamNganSach IN (2) ";
            }
            else if (iID_MaNamNganSach == 1)
            {
                DK += " AND iID_MaNamNganSach IN (1) ";
            }
            else
            {
                DK += " AND iID_MaNamNganSach IN (1,2) ";
            }
            String iID_MaPhongBanQuanLy = DonViModels.getPhongBanCuaDonVi(iID_MaDonVi, System.Web.HttpContext.Current.User.Identity.Name);
            //String DKPhongBan = ThuNopModels.DKPhongBan_Dich(System.Web.HttpContext.Current.User.Identity.Name);
            //DK += " AND (iThang_Quy<@iThang_Quy OR (iThang_Quy=@iThang_Quy AND iID_MaChungTu IN (SELECT iID_MaChungTu FROM QTA_ChungTu WHERE iTrangThai=1 AND dNgayChungTu<@dNgayChungTu)))";
            //cmd.Parameters.AddWithValue("@iThang_Quy", iThang_Quy);
            //cmd.Parameters.AddWithValue("@dNgayChungTu", dNgayChungTu);
            //DK += " AND bLoaiThang_Quy=@bLoaiThang_Quy";
            //cmd.Parameters.AddWithValue("@bLoaiThang_Quy", bLoaiThang_Quy);

            String[] arrDSTruongTien_So = MucLucNganSachModels.strDSTruongTien_So.Split(',');
            String strTruong = "";
            for (int i = 0; i < arrDSTruongTien_So.Length; i++)
            {
                if (i > 0) strTruong += ",";
                strTruong += String.Format("SUM({0}) AS {0}", arrDSTruongTien_So[i]);
            }
            String DKMucCap = "";
            if (sMucCap == "sNG")
            {
                DKMucCap = "sTM,sTTM,sNG,";
            }
            else if (sMucCap == "sTM")
            {
                DKMucCap = "sTM,";
            }

            cmd.CommandText = String.Format(@"
SELECT  sLNS1,
                             sLNS3,
                             sLNS5,sLNS,sL,sK,{3} sM,{0}
FROM (

SELECT iID_MaMucLucNganSach,sXauNoiMa,SUBSTRING(sLNS,1,1) as sLNS1,
                            SUBSTRING(sLNS,1,3) as sLNS3,
                            sLNS5=CASE WHEN sLNS=1020000 then 10201 else  SUBSTRING(sLNS,1,5) END ,
                            sLNS =CASE WHEN sLNS=1020000 then 1020100 else sLNS END ,
                            sL,sK,{3}sM,sMoTa,{0}
                    FROM DT_ChungTuChiTiet 
                    WHERE {1} AND (MaLoai='' OR MaLoai='2') AND {2} 
                    GROUP BY sLNS,sL,sK,sM,{3} iID_MaMucLucNganSach,sXauNoiMa,sMota
                    HAVING SUM(rTuChi)<>0 OR SUM(rHienVat)<>0
                    UNION ALL
                    SELECT iID_MaMucLucNganSach,sXauNoiMa,SUBSTRING(sLNS,1,1) as sLNS1,
                            SUBSTRING(sLNS,1,3) as sLNS3,
                            sLNS5=CASE WHEN sLNS=1020000 then 10201 else  SUBSTRING(sLNS,1,5) END ,
                            sLNS =CASE WHEN sLNS=1020000 then 1020100 else sLNS END ,
                              sL,sK,{3}sM,sMoTa,{0}
                    FROM DT_ChungTuChiTiet_PhanCap 
                    WHERE {1}  AND (sLNS='1020100' OR sLNS='1020000' OR sLNS LIKE '207%')  AND {2} 
                    GROUP BY sLNS,sL,sK,sM,{3} iID_MaMucLucNganSach,sXauNoiMa,sMota
                    HAVING SUM(rTuChi)<>0 OR SUM(rHienVat)<>0
                    UNION ALL
                    SELECT iID_MaMucLucNganSach,sXauNoiMa,SUBSTRING(sLNS,1,1) as sLNS1,
                            SUBSTRING(sLNS,1,3) as sLNS3,
                            sLNS5=CASE WHEN sLNS=1020000 then 10201 else  SUBSTRING(sLNS,1,5) END ,
                            sLNS =CASE WHEN sLNS=1020000 then 1020100 else sLNS END ,
                             sL,sK,{3}sM,sMoTa,{0}
                    FROM DTBS_ChungTuChiTiet 
                    WHERE {1} AND (MaLoai='' OR MaLoai='2')                     
                    AND iID_MaChungTu IN (SELECT maChungTu FROM F_NS_DSCtuBS_TheoNgay(@iNamLamViec,@iID_MaPhongBan,@cNgay))
                    AND {2} 
                    GROUP BY sLNS,sL,sK,sM,{3} iID_MaMucLucNganSach,sXauNoiMa,sMota
                    HAVING SUM(rTuChi)<>0 OR SUM(rHienVat)<>0
                    UNION ALL
                    SELECT iID_MaMucLucNganSach,sXauNoiMa,SUBSTRING(sLNS,1,1) as sLNS1,
                            SUBSTRING(sLNS,1,3) as sLNS3,
                            sLNS5=CASE WHEN sLNS=1020000 then 10201 else  SUBSTRING(sLNS,1,5) END ,
                            sLNS =CASE WHEN sLNS=1020000 then 1020100 else sLNS END ,
                            sL,sK,{3}sM,sMoTa,{0}
                    FROM DTBS_ChungTuChiTiet_PhanCap as pc INNER JOIN (SELECT iID_MaChungTuChiTiet, iID_MaChungTu FROM DTBS_ChungTuChiTiet) as ctct ON ctct.iID_MaChungTuChiTiet = pc.iID_MaChungTu  
                    WHERE {1}  AND (sLNS='1020100' OR sLNS='1020000' OR sLNS LIKE '207%') 
                    AND ctct.iID_MaChungTu IN (SELECT maChungTu FROM F_NS_DSCtuBS_TheoNgay(@iNamLamViec,@iID_MaPhongBan,@cNgay))
                    AND {2} 
                    GROUP BY sLNS,sL,sK,sM,{3} iID_MaMucLucNganSach,sXauNoiMa,sMota
                    HAVING SUM(rTuChi)<>0 OR SUM(rHienVat)<>0) as CT
                    GROUP BY sLNS1,sLNS3, sLNS5,sLNS,sL,sK,{3} sM
                    ORDER BY sLNS,sL,sK,{3} sM
", strTruong, DK, iID_MaPhongBanQuanLy, DKMucCap);
            cmd.Parameters.AddWithValue("@cNgay", Convert.ToDateTime(dNgayChungTu));

            DataTable vR = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return vR;
        }

        public static DataTable Get_dtTongChiTieuChoDonVi_XauNoiMa(
                                                      String sXauNoiMa,
                                                      String iID_MaDonVi,
                                                      NameValueCollection data
                                                       )
        {
            SqlCommand cmd = new SqlCommand();
            String DK = "iTrangThai = 1";
            String sXauNoiMa1 = "-10000000000000";
            if (data["iLoai"] == "1")
            {
                if (!String.IsNullOrEmpty(data["sDSLNS"]))
                {
                    String sLNS = data["sDSLNS"];
                    if (sLNS.Contains("1020100"))
                    {
                        sLNS += ",1020100,1020000";
                        sXauNoiMa1 = sXauNoiMa.Replace("1020100", "1020000");
                    }
                    String[] arrsLNS = sLNS.Split(',');
                    DK += " AND (";
                    for (int i = 0; i < arrsLNS.Length; i++)
                    {
                        DK += "sLNS = @sLNS" + i;
                        cmd.Parameters.AddWithValue("@sLNS" + i, arrsLNS[i]);
                        if (i < arrsLNS.Length - 1)
                            DK += " OR ";
                    }
                    DK += " )";
                }
            }
            //ngan sach dac biet
            else if (data["iLoai"] == "0")
            {
                DK += " AND ( sLNS like '4%')";
            }
            else
            {
                DK += " AND ( sLNS like '2%')";
            }
            //DK += " AND iID_MaTrangThaiDuyet=@iID_MaTrangThaiDuyet";
            //cmd.Parameters.AddWithValue("@iID_MaTrangThaiDuyet", LuongCongViecModel.Get_iID_MaTrangThaiDuyet_DaDuyet(QuyetToanModels.iID_MaPhanHeQuyetToan));
            String[] arrDonVi = iID_MaDonVi.Split(',');
            if (arrDonVi.Length > 0)
            {
                DK = DK + " AND ( ";
                for (int i = 0; i < arrDonVi.Length; i++)
                {
                    DK += "iID_MaDonVi=@iID_MaDonVi" + i;
                    if (i < arrDonVi.Length - 1)
                        DK += " OR ";
                    cmd.Parameters.AddWithValue("@iID_MaDonVi" + i, arrDonVi[i]);
                }
                DK = DK + " ) ";

            }
            DK += " AND iNamLamViec=@iNamLamViec";
            cmd.Parameters.AddWithValue("@iNamLamViec", data["iNamLamViec"]);
            DK += " AND iID_MaNguonNganSach=@iID_MaNguonNganSach";
            cmd.Parameters.AddWithValue("@iID_MaNguonNganSach", data["iID_MaNguonNganSach"]);

            if (Convert.ToInt32(data["iID_MaNamNganSach"]) == 2)
            {
                DK += " AND iID_MaNamNganSach IN (2) ";
            }
            else if (Convert.ToInt32(data["iID_MaNamNganSach"]) == 1)
            {
                DK += " AND iID_MaNamNganSach IN (1) ";
            }
            else
            {
                DK += " AND iID_MaNamNganSach IN (1,2) ";
            }
            String iID_MaPhongBanQuanLy = DonViModels.getPhongBanCuaDonVi(iID_MaDonVi, System.Web.HttpContext.Current.User.Identity.Name);
            //String DKPhongBan = ThuNopModels.DKPhongBan_Dich(System.Web.HttpContext.Current.User.Identity.Name);
            //DK += " AND (iThang_Quy<@iThang_Quy OR (iThang_Quy=@iThang_Quy AND iID_MaChungTu IN (SELECT iID_MaChungTu FROM QTA_ChungTu WHERE iTrangThai=1 AND dNgayChungTu<@dNgayChungTu)))";
            //cmd.Parameters.AddWithValue("@iThang_Quy", iThang_Quy);
            //cmd.Parameters.AddWithValue("@dNgayChungTu", dNgayChungTu);
            //DK += " AND bLoaiThang_Quy=@bLoaiThang_Quy";
            //cmd.Parameters.AddWithValue("@bLoaiThang_Quy", bLoaiThang_Quy);

            String[] arrDSTruongTien_So = MucLucNganSachModels.strDSTruongTien_So.Split(',');
            String strTruong = "";
            for (int i = 0; i < arrDSTruongTien_So.Length; i++)
            {
                if (i > 0) strTruong += ",";
                strTruong += String.Format("SUM({0}) AS {0}", arrDSTruongTien_So[i]);
            }
            String DKMucCap = "";
            if (data["sLoai"] == "sNG")
            {
                DKMucCap = "sTM,sTTM,sNG,";
            }
            else if (data["sLoai"] == "sTTM")
            {
                DKMucCap = "sTM,sTTM,";
            }
            else if (data["sLoai"] == "sTM")
            {
                DKMucCap = "sTM,";
            }

            cmd.CommandText = String.Format(@"
SELECT  sLNS1,
                             sLNS3,
                             sLNS5,sLNS,sL,sK,{3} sM,{0}
FROM (

SELECT iID_MaMucLucNganSach,sXauNoiMa,SUBSTRING(sLNS,1,1) as sLNS1,
                            SUBSTRING(sLNS,1,3) as sLNS3,
                            sLNS5=CASE WHEN sLNS=1020000 then 10201 else  SUBSTRING(sLNS,1,5) END ,
                            sLNS =CASE WHEN sLNS=1020000 then 1020100 else sLNS END ,
                            sL,sK,{3}sM,sMoTa,{0}
                    FROM DT_ChungTuChiTiet 
                    WHERE {1} AND (MaLoai='' OR MaLoai='2') AND {2} AND (sXauNoiMa LIKE @sXauNoiMa OR sXauNoiMa LIKE @sXauNoiMa1) 
                    GROUP BY sLNS,sL,sK,sM,{3} iID_MaMucLucNganSach,sXauNoiMa,sMota
                    HAVING SUM(rTuChi)<>0 OR SUM(rHienVat)<>0
                    UNION ALL
                    SELECT iID_MaMucLucNganSach,sXauNoiMa,SUBSTRING(sLNS,1,1) as sLNS1,
                            SUBSTRING(sLNS,1,3) as sLNS3,
                            sLNS5=CASE WHEN sLNS=1020000 then 10201 else  SUBSTRING(sLNS,1,5) END ,
                            sLNS =CASE WHEN sLNS=1020000 then 1020100 else sLNS END ,
                              sL,sK,{3}sM,sMoTa,{0}
                    FROM DT_ChungTuChiTiet_PhanCap 
                    WHERE {1}  AND (sLNS='1020100' OR sLNS='1020000' OR sLNS LIKE '207%')  AND {2}  AND sXauNoiMa LIKE @sXauNoiMa
                    GROUP BY sLNS,sL,sK,sM,{3} iID_MaMucLucNganSach,sXauNoiMa,sMota
                    HAVING SUM(rTuChi)<>0 OR SUM(rHienVat)<>0
                        UNION ALL
                SELECT iID_MaMucLucNganSach,sXauNoiMa,SUBSTRING(sLNS,1,1) as sLNS1,
                            SUBSTRING(sLNS,1,3) as sLNS3,
                            sLNS5=CASE WHEN sLNS=1020000 then 10201 else  SUBSTRING(sLNS,1,5) END ,
                            sLNS =CASE WHEN sLNS=1020000 then 1020100 else sLNS END ,
                             sL,sK,{3}sM,sMoTa,{0}
                    FROM DTBS_ChungTuChiTiet 
                    WHERE {1} AND (MaLoai='' OR MaLoai='2') AND {2}  AND sXauNoiMa LIKE @sXauNoiMa
                    AND iID_MaChungTu IN (SELECT maChungTu FROM F_NS_DSCtuBS_TheoNgay(@iNamLamViec,@iID_MaPhongBan,@cNgay))
                    GROUP BY sLNS,sL,sK,sM,{3} iID_MaMucLucNganSach,sXauNoiMa,sMota
                    HAVING SUM(rTuChi)<>0 OR SUM(rHienVat)<>0
                    UNION ALL
                    SELECT iID_MaMucLucNganSach,sXauNoiMa,SUBSTRING(sLNS,1,1) as sLNS1,
                            SUBSTRING(sLNS,1,3) as sLNS3,
                            sLNS5=CASE WHEN sLNS=1020000 then 10201 else  SUBSTRING(sLNS,1,5) END ,
                            sLNS =CASE WHEN sLNS=1020000 then 1020100 else sLNS END ,
                            sL,sK,{3}sM,sMoTa,{0}
                    FROM DTBS_ChungTuChiTiet_PhanCap as pc INNER JOIN (SELECT iID_MaChungTuChiTiet, iID_MaChungTu FROM DTBS_ChungTuChiTiet) as ctct ON ctct.iID_MaChungTuChiTiet = pc.iID_MaChungTu 
                    WHERE {1}  AND (sLNS='1020100' OR sLNS='1020000' OR sLNS LIKE '207%') AND {2}  AND sXauNoiMa LIKE @sXauNoiMa
                    AND ctct.iID_MaChungTu IN (SELECT maChungTu FROM F_NS_DSCtuBS_TheoNgay(@iNamLamViec,@iID_MaPhongBan,@cNgay))
                    GROUP BY sLNS,sL,sK,sM,{3} iID_MaMucLucNganSach,sXauNoiMa,sMota
                    HAVING SUM(rTuChi)<>0 OR SUM(rHienVat)<>0) as CT
                    GROUP BY sLNS1,sLNS3, sLNS5,sLNS,sL,sK,{3} sM
                    ORDER BY sLNS,sL,sK,{3} sM
", strTruong, DK, iID_MaPhongBanQuanLy, DKMucCap);
            cmd.Parameters.AddWithValue("@sXauNoiMa", "%" + sXauNoiMa + "%");
            cmd.Parameters.AddWithValue("@sXauNoiMa1", "%" + sXauNoiMa1 + "%");
            cmd.Parameters.AddWithValue("@cNgay", Convert.ToDateTime(data["dNgayCapPhat"]));
            DataTable vR = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return vR;
        }
        /// <summary>
        /// Lấy thông tin trường đã cấp phát của chứng từ cấp phát hiện tại
        /// Giá trị đã cấp phát của đợt này là lũy kế cấp phát đã được duyệt của các đợt trước
        /// </summary>
        /// <param name="iID_MaCapPhat">Mã chứng từ cấp phát</param>
        /// <param name="dtChiTiet"></param>


        /// <summary>
        /// Hàm lấy dữ liệu từ bảng CP_ChungTuChiTiet. Các hàng được lấy thuộc về các chứng từ cùng loại và thuộc đợt trược.
        /// Dữ liệu lấy về là tổng trường tiền cấp phát vào đợt trước, giá trị này sẽ trở thành đã cấp ở đợt này
        /// Hàm sử dụng trong trường hợp lưới nhập được load lên lần đầu hoặc reload, giá trị đã cấp phát được lưu vào dtChiTiet
        /// </summary>
        /// <param name="RChungTuCP"></param>
        /// <returns></returns>
        private static DataTable LayDtTongDaCapPhat(DataRow RChungTuCP)
        {
            //String iID_MaMucLucNganSach,
            String iID_MaNamNganSach = Convert.ToString(RChungTuCP["iID_MaNamNganSach"]);
            String iID_MaNguonNganSach = Convert.ToString(RChungTuCP["iID_MaNguonNganSach"]);
            String iNamLamViec = Convert.ToString(RChungTuCP["iNamLamViec"]);
            String iDM_MaLoaiCapPhat = Convert.ToString(RChungTuCP["iDM_MaLoaiCapPhat"]);
            String iID_MaTinhChatCapThu = Convert.ToString(RChungTuCP["iID_MaTinhChatCapThu"]);
            String sDSLNS = Convert.ToString(RChungTuCP["sDSLNS"]);
            String sChiTietDen = Convert.ToString(RChungTuCP["sLoai"]);
            String dNgayCapPhat = Convert.ToString(RChungTuCP["dNgayCapPhat"]);

            SqlCommand cmd = new SqlCommand();
            String DK = "iTrangThai = 1";

            DK += " AND iID_MaTrangThaiDuyet=@iID_MaTrangThaiDuyet";
            cmd.Parameters.AddWithValue("@iID_MaTrangThaiDuyet", LuongCongViecModel.Get_iID_MaTrangThaiDuyet_DaDuyet(CapPhatModels.iID_MaPhanHe));

            DK += " AND iID_MaNguonNganSach=@iID_MaNguonNganSach";
            cmd.Parameters.AddWithValue("@iID_MaNguonNganSach", iID_MaNguonNganSach);
            DK += " AND iID_MaNamNganSach=@iID_MaNamNganSach";
            cmd.Parameters.AddWithValue("@iID_MaNamNganSach", iID_MaNamNganSach);

            DK += " AND iNamLamViec=@iNamLamViec";
            cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
            DK += " AND iDM_MaLoaiCapPhat=@iDM_MaLoaiCapPhat";
            cmd.Parameters.AddWithValue("@iDM_MaLoaiCapPhat", iDM_MaLoaiCapPhat);
            DK += " AND iID_MaTinhChatCapThu=@iID_MaTinhChatCapThu";
            cmd.Parameters.AddWithValue("@iID_MaTinhChatCapThu", iID_MaTinhChatCapThu);
            DK += " AND sLNS=@sLNS";
            cmd.Parameters.AddWithValue("@sLNS", sDSLNS);
            //DK += " AND sChiTietDen=@sChiTietDen";
            //cmd.Parameters.AddWithValue("@sChiTietDen", sChiTietDen);
            //DK += " AND dNgayCapPhat<=@dNgayCapPhat";
            DK += " AND iID_MaCapPhat IN (SELECT iID_MaCapPhat FROM CP_CapPhat WHERE dNgayCapPhat < @dNgayCapPhat)";
            cmd.Parameters.AddWithValue("@dNgayCapPhat", dNgayCapPhat);

            String[] arrDSTruongTien_So = MucLucNganSachModels.strDSTruongTien_So.Split(',');
            String strTruong = "";
            for (int i = 0; i < arrDSTruongTien_So.Length; i++)
            {
                if (i > 0) strTruong += ",";
                strTruong += String.Format("SUM({0}) AS {0}", arrDSTruongTien_So[i]);
            }

            String selectTable = String.Format("SELECT iID_MaMucLucNganSach,sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi,{0} FROM CP_CapPhatChiTiet WHERE {1} GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaMucLucNganSach,iID_MaDonVi", strTruong, DK);
            String orderTable = "SELECT * FROM (" + selectTable + ") A ORDER BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaMucLucNganSach";
            cmd.CommandText = orderTable;
            DataTable vR = Connection.GetDataTable(cmd);
            cmd.Dispose();
            return vR;
        }

        /// <summary>
        /// HungPX QUP
        /// Hàm cập nhật chứng từ chi tiết sau khi cập nhật chứng từ
        /// </summary>
        /// <param name="iID_MaCapPhat"></param>
        public static void DongBoChungTuChiTiet(string iID_MaCapPhat)
        {
            DataTable dtChungTu = CapPhat_ChungTuModels.LayChungTuCapPhat(iID_MaCapPhat);

            SqlCommand cmd = new SqlCommand();

            String SQL = "UPDATE CP_CapPhatChiTiet " +
                        "set dNgayCapPhat = @dNgayCapPhat , " +
                        "iDM_MaLoaiCapPhat = @iDM_MaLoaiCapPhat, " +
                        "iID_MaTinhChatCapThu = @iID_MaTinhChatCapThu " +
                        "where iID_MaCapPhat = @iID_MaCapPhat";
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@dNgayCapPhat", dtChungTu.Rows[0]["dNgayCapPhat"]);
            cmd.Parameters.AddWithValue("@iDM_MaLoaiCapPhat", dtChungTu.Rows[0]["iDM_MaLoaiCapPhat"]);
            cmd.Parameters.AddWithValue("@iID_MaTinhChatCapThu", dtChungTu.Rows[0]["iID_MaTinhChatCapThu"]);
            cmd.Parameters.AddWithValue("@iID_MaCapPhat", dtChungTu.Rows[0]["iID_MaCapPhat"]);
            Connection.UpdateDatabase(cmd);
            cmd.Dispose();
            dtChungTu.Dispose();
        }

        /// <summary>
        /// Hàm lấy thông tin của chỉ tiêu đã cấp cho đơn vị theo Mục lục ngân sách
        /// </summary>
        /// <param name="iID_MaMucLucNganSach"></param>
        /// <param name="iID_MaDonVi"></param>
        /// <param name="iNamLamViec"></param>
        /// <param name="iID_MaNguonNganSach"></param>
        /// <param name="iID_MaNamNganSach"></param>
        /// <returns></returns>
        public static String LayGiaTri_ChiTieu_DaCap(NameValueCollection data, DataRow dr,
                                                     String iID_MaMucLucNganSach,
                                                     String iID_MaDonVi)
        {
            String vR = "";
            String sXauNoiMa = Convert.ToString(dr["sXauNoiMa"]);
            DataTable dtTongPhanBo = CapPhat_ChungTuChiTietModels.Get_dtTongChiTieuChoDonVi_XauNoiMa(sXauNoiMa, iID_MaDonVi, data);
            DataTable dtTongDaCapPhat = CapPhat_ChungTuChiTietModels.LayDtTongCapPhatChoDonVi(iID_MaMucLucNganSach, iID_MaDonVi, data);


            String strTruong = "rTuChi";
            String strTongPhanBo = "";
            String strTongDaCapPhat = "";

            String Truong = "SumrTuChi";
            Object Value = 0;

            if (dtTongPhanBo.Rows.Count > 0 && dtTongPhanBo != null)
            {
                Value = dtTongPhanBo.Rows[0]["rTuChi"];
            }
            strTongPhanBo += Value;

            Value = 0;
            if (dtTongDaCapPhat.Rows.Count > 0 && dtTongDaCapPhat.Rows[0][Truong] != DBNull.Value && dtTongDaCapPhat != null)
            {
                Value = dtTongDaCapPhat.Rows[0]["SumrTuChi"];
            }
            strTongDaCapPhat += Value;


            dtTongPhanBo.Dispose();
            dtTongDaCapPhat.Dispose();

            vR = String.Format("{0}#{1}#{2}", strTruong, strTongPhanBo, strTongDaCapPhat);
            return vR;
        }

        public static DataTable LayDtChungTuChiTietDMNB(string iID_MaCapPhat, Dictionary<string, string> arrGiaTriTimKiem, string MaND)
        {
            DataTable vR;
            DataTable dtChungTu = CapPhat_ChungTuModels.LayToanBoThongTinChungTu(iID_MaCapPhat);
            DataRow RChungTu = dtChungTu.Rows[0];
            DateTime ngayCT = Convert.ToDateTime(RChungTu["dNgayCapPhat"]);
            string iID_MaDonVi = Convert.ToString(RChungTu["iID_MaDonVi"]);
            String iLoai = Convert.ToString(RChungTu["iLoai"]);
            String sDSLNS = Convert.ToString(RChungTu["sDSLNS"]);
            String iNamLamViec = Convert.ToString(RChungTu["iNamLamViec"]);
            String iID_MaPhongBan = Convert.ToString(RChungTu["iID_MaPhongBan"]);
            String sTruongTien = "rTuChi,iID_MaCapPhatChiTiet,iID_MaDonVi,sTenDonVi,sMaCongTrinh,rTuChi_PhanBo,rTuChi_DaCap";
            String[] arrDSTruong = (MucLucNganSachModels.strDSTruong + ",iID_MaDonVi").Split(',');
            String[] arrDSTruongTien = sTruongTien.Split(',');

            string sql, DK = "";
            SqlCommand cmd, cmd1 = new SqlCommand();

            //<--Lay toan bo Muc luc ngan sach
            cmd = new SqlCommand();
            DK = "iTrangThai=1";

            if (arrGiaTriTimKiem != null)
            {
                for (int i = 0; i < arrDSTruong.Length - 1; i++)
                {
                    if (String.IsNullOrEmpty(arrGiaTriTimKiem[arrDSTruong[i]]) == false)
                    {
                        DK += String.Format(" AND {0} LIKE @{0}", arrDSTruong[i]);
                        cmd.Parameters.AddWithValue("@" + arrDSTruong[i], arrGiaTriTimKiem[arrDSTruong[i]] + "%");
                    }
                }
            }
            if (!String.IsNullOrEmpty(iID_MaDonVi))
            {
                DK += String.Format(" AND iID_MaDonVi LIKE @iID_MaDonVi");
                cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
            }

            String DKLoai = "";
            //ngan sach quoc phong tung loai ns 

            if (!string.IsNullOrEmpty(sDSLNS))
            {
                DataTable dtNGChiTieu = new DataTable();
                #region definition input

                sql = FileHelpers.GetSqlQuery("dtCapPhat_HienVat_Nganh.sql");

                #endregion

                #region dieu kien

                String dkDonVi;
                if (!String.IsNullOrEmpty(iID_MaDonVi))
                {
                    dkDonVi = iID_MaDonVi;
                }
                else
                {
                    dkDonVi = new NganSachService().GetDonviListByUser(System.Web.HttpContext.Current.User.Identity.Name, iNamLamViec)
                        .ToDictionary(x => x.iID_MaDonVi, x => x.iID_MaDonVi + " - " + x.sTen).Select(x => x.Key).Join();
                }

                #endregion

                #region get data

                using (var conn = ConnectionFactory.Default.GetConnection())
                using (cmd1 = new SqlCommand(sql, conn))
                {
                    cmd1.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                    cmd1.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan);
                    cmd1.Parameters.AddWithValue("@Nganh", sDSLNS);
                    cmd1.Parameters.AddWithValue("@iID_MaDonVi", dkDonVi);
                    cmd1.Parameters.AddWithValue("@iID_MaNguonNganSach", DBNull.Value);
                    cmd1.Parameters.AddWithValue("@iID_MaNamNganSach", DBNull.Value);
                    cmd1.Parameters.AddWithValue("@userName", MaND);

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
                DKLoai += String.Format("and sNG like '---%'");

            sql = String.Format("SELECT iID_MaMucLucNganSach,iID_MaMucLucNganSach_Cha,bLaHangCha,sXauNoiMa,{0},{1} FROM NS_MucLucNganSach WHERE iNamLamViec=@iNamLamViec AND  {2} {4} ORDER BY {3}", MucLucNganSachModels.strDSTruong, MucLucNganSachModels.strDSDuocNhapTruongTien, DK, MucLucNganSachModels.strDSTruongSapXep, DKLoai);
            cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
            cmd.CommandText = sql;
            vR = Connection.GetDataTable(cmd);

            cmd.Dispose();
            //Lay toan bo Muc luc ngan sach-->
            DataColumn column;

            for (int j = 0; j < arrDSTruongTien.Length; j++)
            {
                column = new DataColumn(arrDSTruongTien[j], typeof(String));
                if (arrDSTruongTien[j] == "rTuChi_PhanBo" || arrDSTruongTien[j] == "rTuChi_DaCap" || arrDSTruongTien[j] == "rTuChi")
                {
                    column.DefaultValue = 0;
                }
                column.AllowDBNull = true;
                vR.Columns.Add(column);
            }

            //HungPX: Lấy thông tin cột tiền đã cấp
            //DataRow dr = dtChungTu.Rows[0];
            DataTable arrDV = NganSach_HamChungModels.DSDonViCuaNguoiDung(MaND);
            String iID_MaDonVi_TK = arrGiaTriTimKiem["iID_MaDonVi"];
            int count1 = vR.Rows.Count;

            //Dien don vi
            if (!String.IsNullOrEmpty(iID_MaDonVi))
            {
                for (int i = arrDV.Rows.Count - 1; i >= 0; i--)
                {
                    if (Convert.ToString(arrDV.Rows[i]["iID_MaDonVi"]) != iID_MaDonVi)
                        arrDV.Rows.RemoveAt(i);
                }
                for (int j = 0; j < count1; j++)
                {
                    DataRow dr = vR.Rows[j];
                    if (dr["sNG"] != "")
                    {

                        dr["iID_MaDonVi"] = arrDV.Rows[0]["iID_MaDonVi"];
                        dr["sTenDonVi"] = arrDV.Rows[0]["sTen"];
                    }
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
                cmd.Parameters.AddWithValue("@iID_MaCapPhat", iID_MaCapPhat);
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
            LayChiTieuDMNB(iID_MaCapPhat, vR);
            LayCapPhatDaCapDMNB(iID_MaCapPhat, vR);

            // long them
            // - nếu chưa cấp phát lần nào thì lấy toàn bộ chỉ tiêu sang
            ChuyenChiTieuSangCapPhatDMNB(vR);

            dtChungTu.Dispose();
            dtChungTuChiTiet.Dispose();
            cmd.Dispose();
            return vR;
        }

        private static void ChuyenChiTieuSangCapPhatDMNB(DataTable dt)
        {
            var loai = System.Web.HttpContext.Current.Request.GetQueryStringValue("f", 0);

            if (loai == 0)
                return;


            if (loai == 1)
            {
                dt.AsEnumerable()
                    .ToList()
                    .ForEach(r =>
                    {
                        if (!string.IsNullOrWhiteSpace(r["iID_MaDonVi"].ToString()))
                            r["rTuChi"] = double.Parse(r["rTuChi_PhanBo"].ToString()) - double.Parse(r["rTuChi_DaCap"].ToString());
                    });
            }

        }

        public static void LayChiTieuDMNB(string iID_MaCapPhat, DataTable vR)
        {
            NameValueCollection data = CapPhat_ChungTuModels.LayThongTin(iID_MaCapPhat);
            DateTime ngayCT = Convert.ToDateTime(data["dNgayCapPhat"]);
            DataTable dtChiTieu = new DataTable();

            #region definition input chitieu

            var sql = FileHelpers.GetSqlQuery("dtCapPhat_HienVat.sql");

            #endregion

            #region dieu kien chitieu  

            var dkDonVi = new NganSachService().GetDonviListByUser(System.Web.HttpContext.Current.User.Identity.Name, data["iNamLamViec"])
                        .ToDictionary(x => x.iID_MaDonVi, x => x.iID_MaDonVi + " - " + x.sTen).Select(x => x.Key).Join();

            #endregion

            #region get data chitieu

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", data["iID_MaPhongBan"]);
                cmd.Parameters.AddWithValue("@iNamLamViec", data["iNamLamViec"]);
                cmd.Parameters.AddWithValue("@iID_MaDonVi", dkDonVi);
                cmd.Parameters.AddWithValue("@loai", data["iID_NguonDuToan"].ToParamString());
                cmd.Parameters.AddWithValue("@dNgay", ngayCT);

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

        public static void LayCapPhatDaCapDMNB(string iID_MaCapPhat, DataTable vR)
        {
            NameValueCollection data = CapPhat_ChungTuModels.LayThongTin(iID_MaCapPhat);
            DateTime ngayCT = Convert.ToDateTime(data["dNgayCapPhat"]);
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
                cmd.Parameters.AddWithValue("@iID_MaPhongBan", data["iID_MaPhongBan"]);
                cmd.Parameters.AddWithValue("@iNamLamViec", data["iNamLamViec"]);
                cmd.Parameters.AddWithValue("@iID_MaNguonNganSach", data["iID_MaNguonNganSach"]);
                cmd.Parameters.AddWithValue("@iID_MaNamNganSach", data["iID_MaNamNganSach"]);
                cmd.Parameters.AddWithValue("@dNgayCapPhat", ngayCT);
                cmd.Parameters.AddWithValue("@iSoCapPhat", data["iSoCapPhat"]);

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
    }
}
