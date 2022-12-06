using DomainModel;
using System;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using Viettel.Data;
using Viettel.Services;
using VIETTEL.Helpers;

namespace VIETTEL.Models.CapPhat
{
    public class CapPhat_ReportModels
    {
        public static int iID_MaPhanHe = PhanHeModels.iID_MaPhanHeCapPhat;

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
                     HAVING SUM(rTuChi)<>0", DK, DKDonVi, DKPhongBan, strDonVi);

            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            cmd.Parameters.AddWithValue("@iID_MaCapPhat", iDotCapPhat);
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();

            return dt;
        }

        //VungNV: 2015/09/28 lấy danh sách phòng ban
        public static DataTable LayDSPhongBan(String MaND)
        {
            SqlCommand cmd = new SqlCommand();
            String DKPhongBan = "";
            DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);

            String SQL = String.Format(
                        @"SELECT DISTINCT iID_MaPhongBan,sTenPhongBan
                        FROM CP_CapPhatChiTiet
                        WHERE iTrangThai=1 AND iNamLamViec=@iNamLamViec {0} 
                        ", DKPhongBan);

            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();
            DataRow dr = dt.NewRow();
            dr["iID_MaPhongBan"] = Guid.Empty;
            dr["sTenPhongBan"] = "--Chọn tất cả các B--";
            dt.Rows.InsertAt(dr, 0);

            return dt;
        }

        /// <summary>
        /// Lấy danh sách các đợt cấp phát
        /// </summary>
        /// <param name="MaND">Mã người dùng</param>
        /// <returns></returns>
        public static DataTable LayDotCapPhat(String MaND)
        {
            SqlCommand cmd = new SqlCommand();
            String DKPhongBan = "";
            DKPhongBan = ThuNopModels.DKPhongBan_QuyetToan(MaND, cmd);

            String SQL = String.Format(
                @"SELECT CONVERT(VARCHAR(10), dNgayCapPhat, 103) AS iNamCapPhat, iID_MaCapPhat
                FROM CP_CapPhat
                WHERE iID_MaCapPhat IN (SELECT DISTINCT iID_MaCapPhat FROM CP_CapPhatChiTiet
                                        WHERE iTrangThai = 1 AND iNamLamViec = @iNamLamViec {0})
                ORDER BY CP_CapPhat.dNgayCapPhat ASC", DKPhongBan);

            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@iNamLamViec", ReportModels.LayNamLamViec(MaND));
            DataTable dt = Connection.GetDataTable(cmd);
            cmd.Dispose();

            return dt;
        }

        /// <summary>
        /// Lấy giá trị của loại cấp phát
        /// </summary>
        /// <param name="MaND">Mã người dùng</param>
        /// <returns></returns>
        /// VungNV: 2015/09/30
        public static String LayLoaiCapPhat(String MaND)
        {
            String sLoaiCapPhat = "";
            String sTen = "rptCapPhat_ThongTri_LoaiCapPhat";
            String SQL = "";

            SQL = String.Format(
                @"SELECT sGhiChu 
                  FROM CP_GhiChu 
                  WHERE sTen=@sTen AND sID_MaNguoiDung=@sID_MaNguoiDung"
               );

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@sID_MaNguoiDung", MaND);
            cmd.Parameters.AddWithValue("@sTen", sTen);
            sLoaiCapPhat = Connection.GetValueString(cmd, "");
            cmd.Dispose();

            return sLoaiCapPhat;
        }

        /// <summary>
        /// Thêm mới hoặc cập nhật loại cấp phát
        /// </summary>
        /// <param name="LoaiCapPhat">Nội dung cấp phát</param>
        /// <param name="MaND">Mã người dùng</param>
        /// VungNV: 2015/11/11
        public static void CapNhatLoaiCapPhat(String LoaiCapPhat, String MaND, String iID_MaCapPhat)
        {
            String sTen = "rptCapPhat_ThongTri_LoaiCapPhat";
            String SQL = "";
            SQL = String.Format(
                @"IF NOT EXISTS(
                    SELECT sGhiChu 
                    FROM CP_GhiChu 
                    WHERE sTen =  @sTen AND sID_MaNguoiDung = @sID_MaNguoiDung
                )
            INSERT INTO CP_GhiChu(sTen, sID_MaNguoiDung, sGhiChu) 
                    VALUES( @sTen,@sID_MaNguoiDung, @sGhiChu)
            ELSE 
                UPDATE CP_GhiChu 
                SET sGhiChu=@sGhiChu 
                WHERE  sTen = @sTen AND  sID_MaNguoiDung = @sID_MaNguoiDung");

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = SQL;
            cmd.Parameters.AddWithValue("@sTen", iID_MaCapPhat);
            cmd.Parameters.AddWithValue("@sID_MaNguoiDung", MaND);
            cmd.Parameters.AddWithValue("@sGhiChu", LoaiCapPhat);
            Connection.UpdateDatabase(cmd);
            cmd.Dispose();

        }

        public static DataTable rptCapPhat_THChiTieu(String MaND, String loaiNS)
        {

            //DataTable vR;
            //String iNamLamViec = ReportModels.LayNamLamViec(System.Web.HttpContext.Current.User.Identity.Name);
            //String iID_MaPhongBan = NguoiDung_PhongBanModels.getTenPhongBan_NguoiDung(MaND);
            //DataTable dt = NganSach_HamChungModels.DSLNSCuaPhongBan(MaND);
            //String sTruongTien = "rTuChi,iID_MaCapPhatChiTiet,iID_MaDonVi,sTenDonVi,sMaCongTrinh,rTuChi_PhanBo,rTuChi_DaCap";
            String[] arrDSTruong = (MucLucNganSachModels.strDSTruong + ",iID_MaDonVi").Split(',');
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
            DataTable arrDV = NganSach_HamChungModels.DSDonViCuaNguoiDung(MaND);
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

    }
}
