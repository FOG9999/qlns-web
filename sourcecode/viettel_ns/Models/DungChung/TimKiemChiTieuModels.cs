using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Viettel.Services;
using Viettel.Data;
using DomainModel;

namespace VIETTEL.Models.TimKiemChiTieu {
    public class TimKiemChiTieuModels
    {        

        //public static DataTable GetDuLieu(String maNguoiDung, String iID_MaDonVi, Dictionary<String, String> arrGiaTriTimKiem, int Trang = 1, int SoBanGhi = 0)
        //{
        //    // Khai báo biến 
        //    string iID_MaPhongBan = null, iID_MaNamNganSach = null;
        //    var cmd = new SqlCommand();            

        //    #region điều kiện

        //    DataTable dtCauHinh = NguoiDungCauHinhModels.LayCauHinh(maNguoiDung);
        //    DataRow R = dtCauHinh.Rows[0];
        //    String sTenPB = NguoiDung_PhongBanModels.getTenPhongBan_NguoiDung(maNguoiDung);

        //    var iNamLamViec = R["iNamLamViec"];
        //    var iID_MaNguonNganSach = R["iID_MaNguonNganSach"];
        //    if (sTenPB != "02" || sTenPB != "2" || sTenPB != "11")
        //        iID_MaPhongBan = sTenPB;
        //    if (Convert.ToInt32(R["iID_MaNamNganSach"]) == 2) {
        //        iID_MaNamNganSach = "2";
        //    } else if (Convert.ToInt32(R["iID_MaNamNganSach"]) == 1) {
        //        iID_MaNamNganSach = "1";
        //    }

        //    String[] arrDSTruongTimKiem = (ChiTieuModels.strDSTruong + ",dDotNgay,sQD,iID_MaPhongBan,sID_MaNguoiDungTao").Split(',');
        //    String dkTim = "";

        //    #region tìm kiếm
        //    if (arrGiaTriTimKiem != null) {
        //        for (int i = 0; i < arrDSTruongTimKiem.Length; i++) {
        //            if (String.IsNullOrEmpty(arrGiaTriTimKiem[arrDSTruongTimKiem[i]]) == false) {
        //                if (!String.IsNullOrEmpty(dkTim)) dkTim += " AND ";
        //                if (arrGiaTriTimKiem[arrDSTruongTimKiem[i]].ToString() == "-1" && arrDSTruongTimKiem[i] == "sLNS") {
        //                    dkTim += "1=1";
        //                } else if (arrGiaTriTimKiem[arrDSTruongTimKiem[i]].Length < 7 && arrDSTruongTimKiem[i] == "sLNS") {
        //                    dkTim += String.Format("{0} LIKE @{0}", arrDSTruongTimKiem[i]);
        //                    cmd.Parameters.AddWithValue("@" + arrDSTruongTimKiem[i], arrGiaTriTimKiem[arrDSTruongTimKiem[i]] + "%");
        //                } else if (arrDSTruongTimKiem[i] == "iID_MaPhongBan")
        //                {
        //                    dkTim += String.Format("iID_MaPhongBan LIKE @{0}", arrDSTruongTimKiem[i]+"_input");
        //                    cmd.Parameters.AddWithValue("@" + arrDSTruongTimKiem[i] + "_input", "%" + arrGiaTriTimKiem[arrDSTruongTimKiem[i]] + "%");
        //                }
        //                    else {
        //                    dkTim += String.Format("{0}=@{0}", arrDSTruongTimKiem[i]);
        //                    cmd.Parameters.AddWithValue("@" + arrDSTruongTimKiem[i], arrGiaTriTimKiem[arrDSTruongTimKiem[i]]);
        //                }
        //            }
        //        }
        //        if (String.IsNullOrEmpty(dkTim) == false) {
        //            dkTim = "WHERE ( " + dkTim + ")";
        //        } else {
        //            dkTim = "WHERE (1 = 1)";
        //        }

        //    }
        //    #endregion 

        //    #endregion

        //    // Khởi tạo hàm sql
        //    #region sql

        //    var sql = String.Format(@" SELECT dulieu.*, sTenDonVi = (dulieu.iID_MaDonVi + ' - ' + sTen) 
        //                               FROM (SELECT * 
        //                                    FROM F_NS_ChiTieu_ChiTiet_TheoDonVi_LNS(
        //                                        @iNamLamViec, @iID_MaPhongBan, @iID_MaNamNganSach, @iID_MaNguonNganSach, @sMaNguoiDung)
        //                                    {0}) AS dulieu JOIN NS_DonVi ON dulieu.iID_MaDonVi = NS_DonVi.iID_MaDonVi
        //                               WHERE NS_DonVi.iNamLamViec_DonVi = @iNamLamViec
        //                               ORDER BY iID_MaDonVi, dDotNgay, sLNS, sL, sK, sM, sTM, sTTM, sNG
        //                               ", dkTim);
        //    #endregion

        //    #region load data

        //    using (var conn = ConnectionFactory.Default.GetConnection())
        //    {
        //        using (cmd)
        //        {
        //            cmd.CommandText = sql;
        //            cmd.Connection = conn;
        //            cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
        //            cmd.Parameters.AddWithValue("@iID_MaNguonNganSach", iID_MaNguonNganSach);
        //            cmd.Parameters.AddWithValue("@iID_MaNamNganSach", iID_MaNamNganSach);
        //            cmd.Parameters.AddWithValue("@iID_MaPhongBan", iID_MaPhongBan);
        //            cmd.Parameters.AddWithValue("@sMaNguoiDung", maNguoiDung);
        //            var dt = cmd.GetTable();                    
        //            return dt;
        //        }
        //    }

        //    #endregion
        //}

        public static DataTable GetDuLieu(String maNguoiDung, String iID_MaDonVi, Dictionary<String, String> arrGiaTriTimKiem, int Trang = 1, int SoBanGhi = 0)
        {
            DataTable vR;
            string iNamLamViec = MucLucNganSachModels.LayNamLamViec(maNguoiDung);
            string iID_MaPhongBanDich = "";
            String[] arrDSTruong = (ChiTieuModels.strDSTruong + ",dDotNgay,sQD,iID_MaPhongBan,sID_MaNguoiDungTao").Split(',');

            string sTenPB = NguoiDung_PhongBanModels.getTenPhongBan_NguoiDung(maNguoiDung);

            if (sTenPB != "02" || sTenPB != "2" || sTenPB != "11")
                iID_MaPhongBanDich = sTenPB;

            var cmdvr = new SqlCommand("select * from f_ns_chitieu_bql(@iNamLamViec,@iID_MaPhongBanDich,@sMaNguoiDung,@dDotNgay,@sQD,@sID_MaNguoiDungTao,@sLNS,@sL,@sK,@sM,@sTM,@sTTM,@sNG,@iID_MaDonVi,@iID_MaPhongBan,@sMoTa)");
          
            if (arrGiaTriTimKiem != null)
            {
                for (int i = 0; i < arrDSTruong.Length; i++)
                {
                    if (String.IsNullOrEmpty(arrGiaTriTimKiem[arrDSTruong[i]]) == false)
                    {
                        cmdvr.Parameters.AddWithValue("@" + arrDSTruong[i], arrGiaTriTimKiem[arrDSTruong[i]] + "%");
                    }
                    else
                    {
                        cmdvr.Parameters.AddWithValue("@" + arrDSTruong[i], DBNull.Value);
                    }
                }
            }
            else
            {
                for (int i = 0; i < arrDSTruong.Length; i++)
                {
                    if (String.IsNullOrEmpty(arrGiaTriTimKiem[arrDSTruong[i]]) == true)
                    {
                        cmdvr.Parameters.AddWithValue("@" + arrDSTruong[i], DBNull.Value);
                    }
                }
            }
            cmdvr.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
            cmdvr.Parameters.AddWithValue("@iID_MaPhongBanDich", iID_MaPhongBanDich);
            cmdvr.Parameters.AddWithValue("@sMaNguoiDung", maNguoiDung);
            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                using (cmdvr)
                {
                    cmdvr.Connection = conn;
                    vR = cmdvr.GetTable();
                }
            }

            return vR;
        }

        public static DataTable LayDSDvi(String iNamLamViec, String maNguoiDung)
        {
            #region sql

            var sql = @"
                    SELECT * FROM F_NS_DSDvi_TheoND(@iNamLamViec,@sMaNguoiDung)
                    ";
            #endregion

            #region load data

            using (var conn = ConnectionFactory.Default.GetConnection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@iNamLamViec", iNamLamViec);
                    cmd.Parameters.AddWithValue("@sMaNguoiDung", maNguoiDung);
                    var dt = cmd.GetTable();
                    return dt;
                }
            }

            #endregion
        }
    }
}