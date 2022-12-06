using System;
using System.Data;
using System.Data.SqlClient;
using DomainModel;
using DomainModel.Abstract;

namespace VIETTEL.Models.ThuNop {
    public class ThuNop_ChungTuChiTietModels
    {
        public static void ThemChiTiet(String iID_MaChungTu, String MaND, String IPSua)
        {
            String[] arrDSTruongTien = MucLucNganSachModels.strDSTruongTien_ThuNop.Split(',');
            DataTable dtChungTu = ThuNop_ChungTuModels.GetChungTu(iID_MaChungTu);

            String iID_MaLoaiHinh = Convert.ToString(dtChungTu.Rows[0]["iID_MaChungTu"]);
            int iNamLamViec = Convert.ToInt32(dtChungTu.Rows[0]["iNamLamViec"]);
            int iID_MaNguonNganSach = Convert.ToInt32(dtChungTu.Rows[0]["iID_MaNguonNganSach"]);
            int iID_MaNamNganSach = Convert.ToInt32(dtChungTu.Rows[0]["iID_MaNamNganSach"]);
            Boolean bChiNganSach = Convert.ToBoolean(dtChungTu.Rows[0]["bChiNganSach"]);
            String sLNS = Convert.ToString(dtChungTu.Rows[0]["sDSLNS"]);
            int iLoai = Convert.ToInt32(dtChungTu.Rows[0]["iLoai"]);

            DataTable dt = NganSach_HamChungModels.DT_MucLucNganSach_sLNS_TheoDau(sLNS);

            Bang bang = new Bang("TN_ChungTuChiTiet");
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
            bang.CmdParams.Parameters.AddWithValue("@iLoai", dtChungTu.Rows[0]["iLoai"]);
            bang.CmdParams.Parameters.AddWithValue("@bChiNganSach", dtChungTu.Rows[0]["bChiNganSach"]);
            bang.CmdParams.Parameters.AddWithValue("@iID_MaDonVi", dtChungTu.Rows[0]["iID_MaDonVi"]);
            for (int i = 0; i < arrDSTruongTien.Length; i++)
            {
                bang.CmdParams.Parameters.AddWithValue("@b" + arrDSTruongTien[i], false);
            }


            for (int i = 0; i < dt.Rows.Count; i++)
            {

                //Dien thong tin cua Muc luc ngan sach
                NganSach_HamChungModels.ThemThongTinCuaMucLucNganSachKhongLayTruongTien(dt.Rows[i], bang.CmdParams.Parameters);
                //Điền các cột theo loại
                switch (iLoai)
                {
                    case 1:
                        bang.CmdParams.Parameters["@brNopNSQP"].Value = true;
                        bang.CmdParams.Parameters["@bsGhiChu"].Value = true;

                        break;
                    case 2:

                        bang.CmdParams.Parameters["@brTongThu"].Value = true;
                        bang.CmdParams.Parameters["@brTongChiPhi"].Value = true;
                        bang.CmdParams.Parameters["@brKhauHaoTSCD"].Value = true;
                        bang.CmdParams.Parameters["@brTienLuong"].Value = true;
                        bang.CmdParams.Parameters["@brQTNSKhac"].Value = true;
                        bang.CmdParams.Parameters["@brChiPhiKhac"].Value = true;
                        bang.CmdParams.Parameters["@brTongNopNSNN"].Value = true;
                        bang.CmdParams.Parameters["@brNopThueGTGT"].Value = true;
                        bang.CmdParams.Parameters["@brTongNopThueTNDN"].Value = true;
                        bang.CmdParams.Parameters["@brNopThueTNDNBQP"].Value = true;
                        bang.CmdParams.Parameters["@brPhiLePhi"].Value = true;
                        bang.CmdParams.Parameters["@brTongNopNSNNKhac"].Value = true;
                        bang.CmdParams.Parameters["@brNopNSNNKhacBQP"].Value = true;
                        bang.CmdParams.Parameters["@brNopNSQP"].Value = true;
                        bang.CmdParams.Parameters["@brBoSungKinhPhi"].Value = true;
                        bang.CmdParams.Parameters["@brTrichQuyDonVi"].Value = true;
                        bang.CmdParams.Parameters["@brSoChuaPhanPhoi"].Value = true;
                        bang.CmdParams.Parameters["@bsGhiChu"].Value = true;

                        break;
                    case 3:
                        bang.CmdParams.Parameters["@brDuToanDuocDuyet"].Value = true;
                        bang.CmdParams.Parameters["@brThucHien"].Value = true;
                        bang.CmdParams.Parameters["@brSoXacNhan"].Value = true;
                        bang.CmdParams.Parameters["@bsGhiChu"].Value = true;
                        break;
                }
                bang.Save();
            }
            dt.Dispose();
            dtChungTu.Dispose();
        }

        public static String InsertDuyetQuyetToan(String iID_MaChungTu, String NoiDung, String MaND, String IPSua)
        {
            String MaDuyetChungTu;
            Bang bang = new Bang("TN_DuyetChungTu");
            bang.MaNguoiDungSua = MaND;
            bang.IPSua = IPSua;
            bang.DuLieuMoi = true;
            bang.CmdParams.Parameters.AddWithValue("@iID_MaChungTu", iID_MaChungTu);
            bang.CmdParams.Parameters.AddWithValue("@sNoiDung", NoiDung);
            MaDuyetChungTu = Convert.ToString(bang.Save());
            return MaDuyetChungTu;
        }

        public static int Get_iID_MaTrangThaiDuyet_TuChoi(String MaND, String iID_MaChungTu)
        {
            int vR = -1;
            DataTable dt = ThuNop_ChungTuModels.GetChungTu(iID_MaChungTu);
            int iID_MaTrangThaiDuyet = Convert.ToInt32(dt.Rows[0]["iID_MaTrangThaiDuyet"]);
            dt.Dispose();
            if (LuongCongViecModel.NguoiDung_DuocSuaChungTu(SharedModels.iID_MaPhanHe, MaND, iID_MaTrangThaiDuyet))
            {
                int iID_MaTrangThaiDuyet_TuChoi = LuongCongViecModel.Luong_iID_MaTrangThaiDuyet_TuChoi(iID_MaTrangThaiDuyet);
                if (iID_MaTrangThaiDuyet_TuChoi > 0)
                {
                    SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM TN_ChungTuChiTiet WHERE iID_MaChungTu=@iID_MaChungTu AND bDongY=0");
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

        public static int Get_iID_MaTrangThaiDuyet_TrinhDuyet(String MaND, String iID_MaChungTu)
        {
            int vR = -1;
            DataTable dt = ThuNop_ChungTuModels.GetChungTu(iID_MaChungTu);
            int iID_MaTrangThaiDuyet = Convert.ToInt32(dt.Rows[0]["iID_MaTrangThaiDuyet"]);
            dt.Dispose();
            if (LuongCongViecModel.NguoiDung_DuocSuaChungTu(SharedModels.iID_MaPhanHe, MaND, iID_MaTrangThaiDuyet))
            {
                int iID_MaTrangThaiDuyet_TrinhDuyet = LuongCongViecModel.Luong_iID_MaTrangThaiDuyet_TrinhDuyet(iID_MaTrangThaiDuyet);
                if (iID_MaTrangThaiDuyet_TrinhDuyet > 0)
                {
                    vR = Convert.ToInt32(iID_MaTrangThaiDuyet_TrinhDuyet);
                }
            }
            return vR;
        }
        /// <summary>
        /// Lay danh sach chung tu chi tiet
        /// </summary>
        /// <param name="iID_MaChungTu"></param>
        /// <param name="arrGiaTriTimKiem"></param>
        /// <param name="Loai"></param>
        /// <param name="MaND"></param>
        /// <returns></returns>
        public static DataTable Get_dtChungTuChiTiet(String iID_MaChungTu, String Loai = "", String MaND = "", int iLoai = 1)
        {

            DataTable vR;
            String maDonVi = "", maPhongBan = "", DKLoaiHinh = "";      
            DataTable dtChungTu = ThuNop_ChungTuModels.GetChungTu(iID_MaChungTu);
            maDonVi = dtChungTu.Rows[0]["iID_MaDonVi"].ToString();
            maPhongBan = NguoiDung_PhongBanModels.getTenPhongBan_NguoiDung(MaND);
            
            DataRow RChungTu = dtChungTu.Rows[0];
            String sTruongTien = "";
            if (iLoai == 1)
            {
                sTruongTien = MucLucNganSachModels.strDSTruongTien_ThuNop_So_Loai1 + ",iID_MaChungTuChiTiet,iID_MaDonVi,sTenDonVi,sGhiChu";

            }
            else
            {
                sTruongTien = "iID_MaChungTuChiTiet,iID_MaDonVi,sTenDonVi," + MucLucNganSachModels.strDSTruongTien_ThuNop;

            }
            String[] arrDSTruong = MucLucNganSachModels.strDSTruong.Split(',');
            String[] arrDSTruongTien = sTruongTien.Split(',');

            String SQL, DK;
            SqlCommand cmd;

            //<--Lay toan bo Muc luc ngan sach
            cmd = new SqlCommand();           
            
            SQL =
                String.Format(
                    @"SELECT bLaHangCha,iID_MaLoaiHinh,
 iID_MaLoaiHinh_Cha,
sKyHieu ,sTen as sMoTa,bLaTong
 FROM TN_DanhMucLoaiHinh WHERE iTrangThai=1 {0}
 ORDER BY sKyhieu,iSTT",DKLoaiHinh);
            cmd.CommandText = SQL;
            vR = Connection.GetDataTable(cmd);
            cmd.Dispose();
            //Lay toan bo Muc luc ngan sach-->

            DataColumn column;

            for (int j = 0; j < arrDSTruongTien.Length; j++)
            {
                if (arrDSTruongTien[j].StartsWith("r") == true)
                {
                    column = new DataColumn(arrDSTruongTien[j], typeof(Double));
                    column.DefaultValue = 0;
                    vR.Columns.Add(column);
                }
                else
                {
                    column = new DataColumn(arrDSTruongTien[j], typeof(String));
                    column.AllowDBNull = true;
                    vR.Columns.Add(column);
                }
            }

            //Them cot duyet
            column = new DataColumn("bDongY", typeof(Boolean));
            column.AllowDBNull = true;
            vR.Columns.Add(column);
            column = new DataColumn("sLyDo", typeof(String));
            column.AllowDBNull = true;
            vR.Columns.Add(column);
            column = new DataColumn("sSoCT", typeof(String));
            column.AllowDBNull = true;
            vR.Columns.Add(column);
            column = new DataColumn("bThoaiThu", typeof(Boolean));
            column.AllowDBNull = true;
            vR.Columns.Add(column);
            
            cmd = new SqlCommand();
            
            //Lay Du Lieu Trong Bang TN_ChungTuChiTiet
            DK =String.Format("iTrangThai=1 AND iID_MaChungTu=@iID_MaChungTu ");         

            cmd.Parameters.AddWithValue("@iID_MaChungTu", iID_MaChungTu);
            SQL = String.Format("SELECT * FROM TN_ChungTuChiTiet WHERE {0} ORDER BY iID_MaDonVi,bThoaiThu", DK);
            cmd.CommandText = SQL;

            DataTable dtChungTuChiTiet = Connection.GetDataTable(cmd);
            int cs0 = 0;


            int vRCount = vR.Rows.Count;
            for (int i = 0; i < vRCount; i++)
            {
                int count = 0;
                for (int j = cs0; j < dtChungTuChiTiet.Rows.Count; j++)
                {

                    Boolean ok = true;

                    if (Convert.ToString(vR.Rows[i]["sKyHieu"]) != Convert.ToString(dtChungTuChiTiet.Rows[j]["sKyHieu"]))
                    {
                        ok = false;

                    }

                    if (ok)
                    {
                        if (count == 0)
                        {
                            for (int k = 0; k < vR.Columns.Count; k++)
                            {
                                if (vR.Columns[k].ColumnName == "iID_MaMucLucNganSach_Cha")
                                {
                                    vR.Rows[i][k] = vR.Rows[i][k];
                                    continue;
                                }
                                if (vR.Columns[k].ColumnName == "iID_MaMucLucNganSach")
                                {
                                    vR.Rows[i][k] = vR.Rows[i][k];
                                    continue;
                                }
                                if ((vR.Columns[k].ColumnName.StartsWith("b") == false && vR.Columns[k].ColumnName != "iID_MaMucLucNganSach_Cha") || vR.Columns[k].ColumnName == "bLaHangCha"
                                    || vR.Columns[k].ColumnName != "bLaTong" )
                                    vR.Rows[i][k] = dtChungTuChiTiet.Rows[j][vR.Columns[k].ColumnName];
                                else
                                    vR.Rows[i][k] = vR.Rows[i][k];
                            }
                            count++;
                        }
                        else
                        {
                            DataRow row = vR.NewRow();
                            for (int k = 0; k < vR.Columns.Count; k++)
                            {
                                if (vR.Columns[k].ColumnName == "iID_MaMucLucNganSach_Cha")
                                {
                                    row[k] = vR.Rows[i][k];
                                    continue;
                                }
                                if (vR.Columns[k].ColumnName == "iID_MaMucLucNganSach")
                                {
                                    row[k] = vR.Rows[i][k];
                                    continue;
                                }
                                if ((vR.Columns[k].ColumnName.StartsWith("b") == false && vR.Columns[k].ColumnName != "iID_MaMucLucNganSach_Cha") || vR.Columns[k].ColumnName == "bLaHangCha"
                                    || vR.Columns[k].ColumnName != "bLaTong" )
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

            return vR;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="NamLamViec"></param>
        /// <param name="iID_MaDonVi"></param>
        /// <returns></returns>
        public static DataTable Get_dlChungTuChiTiet(String MaND, String iID_MaPhongBan,String iID_MaChungTu, String iID_MaDonVi)
        {
            String DKDonVi = "", DKPhongBan = "", DK = "";
            SqlCommand cmd = new SqlCommand();


            DKDonVi = SharedModels.DKDonVi(MaND, cmd);
            DKPhongBan = SharedModels.DKPhongBan(MaND, cmd, iID_MaPhongBan);
            String[] arrDSTruong = MucLucNganSachModels.strDSTruongTien_ThuNop_So.Split(',');
            String SQL =
                String.Format(@"SELECT * FROM TN_DanhMucLoaiHinh
WHERE iTrangThai=1 
ORDER BY sTT,sKyHieu
");
            cmd = new SqlCommand(SQL);
            DataTable dtLoaiHinh = Connection.GetDataTable(cmd);


            String DKSELECT = "", DKHAVING = "";
            for (int k = 1; k < arrDSTruong.Length; k++)
            {
                DKSELECT += arrDSTruong[k] + "=SUM(" + arrDSTruong[k] + ")";
                DKHAVING += "SUM(" + arrDSTruong[k] + ") <>0 ";

                if (k < arrDSTruong.Length - 1)
                {
                    DKSELECT += ",";
                    DKHAVING += " OR ";

                }
            }
            cmd = new SqlCommand();
            if (!String.IsNullOrEmpty(iID_MaDonVi) && iID_MaDonVi != "-1")
            {
                DK += " AND iID_MaDonVi=@iID_MaDonVi";
                cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
            }
            if (!String.IsNullOrEmpty(iID_MaChungTu) && iID_MaChungTu != "-1")
            {
                DK += " AND iID_MaChungTu=@iID_MaChungTu";
                cmd.Parameters.AddWithValue("@iID_MaChungTu", iID_MaChungTu);
            }
            DKDonVi = SharedModels.DKDonVi(MaND, cmd);
            DKPhongBan = SharedModels.DKPhongBan(MaND, cmd, iID_MaPhongBan);
            SQL = String.Format(@"SELECT iID_MaLoaiHinh,sKyHieu,{0} FROM TN_ChungTuChiTiet
WHERE iTrangThai=1 {3} {4} {5}
GROUP BY iID_MaLoaiHinh,sKyHieu
HAVING {1}
ORDER BY sKyHieu", DKSELECT, DKHAVING, DKDonVi, DKPhongBan, DK);
            cmd.CommandText = SQL;
            DataTable dtChungTuChiTiet = Connection.GetDataTable(cmd);
            cmd.Dispose();

            for (int i = 1; i < arrDSTruong.Length; i++)
            {
                DataColumn dtc = new DataColumn();
                dtc.DefaultValue = 0;
                dtc.DataType = typeof(decimal);
                dtc.ColumnName = arrDSTruong[i];
                dtLoaiHinh.Columns.Add(dtc);
            }
            for (int i = 0; i < dtLoaiHinh.Rows.Count; i++)
            {
                for (int j = 0; j < dtChungTuChiTiet.Rows.Count; j++)
                {
                    Boolean ok = true;
                    if (Convert.ToString(dtLoaiHinh.Rows[i]["iID_MaLoaiHinh"]) !=
                        Convert.ToString(dtChungTuChiTiet.Rows[j]["iID_MaLoaiHinh"]))
                        ok = false;
                    if (ok)
                    {
                        for (int k = 1; k < arrDSTruong.Length; k++)
                        {
                            dtLoaiHinh.Rows[i][arrDSTruong[k]] = dtChungTuChiTiet.Rows[j][arrDSTruong[k]];
                        }
                    }
                }
            }
            int count = dtLoaiHinh.Rows.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                bool bCheck = false;
                if (Convert.ToBoolean(dtLoaiHinh.Rows[i]["bLaTong"]))
                {
                    //TInh tong cac truong tien
                    String iiD_MaLoaiHinh = Convert.ToString(dtLoaiHinh.Rows[i]["iiD_MaLoaiHinh"]);

                    for (int k = 1; k < arrDSTruong.Length; k++)
                    {
                        double S;
                        S = 0;
                        for (int j = i + 1; j < dtLoaiHinh.Rows.Count; j++)
                        {
                            if (iiD_MaLoaiHinh == Convert.ToString(dtLoaiHinh.Rows[j]["iiD_MaLoaiHinh_Cha"]))
                            {
                                if (!String.IsNullOrEmpty(Convert.ToString(dtLoaiHinh.Rows[j][arrDSTruong[k]])))
                                {
                                    S += Convert.ToDouble(dtLoaiHinh.Rows[j][arrDSTruong[k]]);
                                }
                            }
                        }
                        dtLoaiHinh.Rows[i][arrDSTruong[k]] = S;
                    }
                }
                //Kiem tra cac dong co so lieu =0 thi xoa trang
                for (int k = 1; k < arrDSTruong.Length; k++)
                {
                    if (Convert.ToDouble(dtLoaiHinh.Rows[i][arrDSTruong[k]]) != 0)
                    {
                        bCheck = true;
                    }
                }

                if (bCheck == false)
                {
                    dtLoaiHinh.Rows.RemoveAt(i);
                }
            }            
            return dtLoaiHinh;
        }
       
        /// <summary>
        /// Lay gia tri loai hinh da cap
        /// </summary>
        /// <param name="iID_MaMucLucNganSach"></param>
        /// <param name="iID_MaDonVi"></param>
        /// <param name="iNamLamViec"></param>
        /// <param name="dNgayCapPhat"></param>
        /// <param name="iID_MaNguonNganSach"></param>
        /// <param name="iID_MaNamNganSach"></param>
        /// <returns></returns>
        public static String LayGiaTri_LoaiHinh_DaCap(String iID_MaMucLucNganSach,
                                                     String iID_MaDonVi,
                                                     int iNamLamViec,

                                                     int iID_MaNguonNganSach,
                                                     int iID_MaNamNganSach)
        {
            SqlCommand cmd = new SqlCommand();
            String SQL =
                "SELECT SUM(rTuChi) as rTuChi FROM DT_ChungTuChiTiet WHERE iTrangThai=1";
            // SQL += " AND iID_MaTrangThaiDuyet=@iID_MaTrangThaiDuyet";
            // cmd.Parameters.AddWithValue("@iID_MaTrangThaiDuyet", LuongCongViecModel.Get_iID_MaTrangThaiDuyet_DaDuyet(PhanHeModels.iID_MaPhanHeDuToan));
            SQL += " AND iNamLamViec=@iNamLamViec";
            cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
            SQL += " AND iID_MaNguonNganSach=@iID_MaNguonNganSach";
            cmd.Parameters.AddWithValue("@iID_MaNguonNganSach", iID_MaNguonNganSach);
            SQL += " AND iID_MaNamNganSach=@iID_MaNamNganSach";
            cmd.Parameters.AddWithValue("@iID_MaNamNganSach", iID_MaNamNganSach);
            SQL += " AND iID_MaMucLucNganSach=@iID_MaMucLucNganSach";
            cmd.Parameters.AddWithValue("@iID_MaMucLucNganSach", iID_MaMucLucNganSach);
            SQL += " AND iID_MaDonVi=@iID_MaDonVi";
            cmd.Parameters.AddWithValue("@iID_MaDonVi", iID_MaDonVi);
            cmd.CommandText = String.Format(SQL);
            DataTable dtLoaiHinh_DuToan = Connection.GetDataTable(cmd);
            cmd.Parameters.Clear();
            cmd.Dispose();

            String vR = "";
            if (dtLoaiHinh_DuToan != null && dtLoaiHinh_DuToan.Rows.Count > 0)
            {
                vR = Convert.ToString(dtLoaiHinh_DuToan.Rows[0]["rTuChi"]);
                dtLoaiHinh_DuToan.Dispose();
            }


            return vR;
        }
                      
    }
}