using DomainModel;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Collections.Generic;

namespace Viettel.Services
{
    public interface INguonNSService
    {
        #region Commons
        string getId(string tablename, string ma, string nam);
        void Update(string tablenameset, string idset, string tablenameget, string idget);
        void UpdateSotienChungTu(string tablenameset, string id);
        int getSoChungTu(string tablename, string loai, string Nam);
        DataTable GetLoaiChungTu(string loai);
        DataTable GetDSChungTu(string tablename, string loai, string nam);
        DataRow GetChungTu(string tablename, string id);
        void Delete_ChungTuChiTiet(string tablename, string columnname, string id, string maND, string iPSua);
        void Insert(string tenBangSLGOC, string tenBangChiTiet, string id);
        DataTable GetChungTuChiTiet(string tablename, string id, Dictionary<string, string> arrGiaTriTimKiem, string[] _arrDSTruongTimKiem, string id_nguon = null);
        bool checkDuLieu(string id);

        #endregion
    }

    public class NguonNSService : INguonNSService
    {
        private readonly IConnectionFactory _connectionFactory = ConnectionFactory.Default;
        private readonly INganSachService _ngansachService = NganSachService.Default;

        private static INguonNSService _default;

        public static INguonNSService Default
        {
            get { return _default ?? (_default = new NguonNSService()); }
        }

        #region Commons
        public string getId(string tablename, string ma, string nam)
        {
            SqlCommand cmd = new SqlCommand();
            string SQL = "select Id from " + tablename + " where Nam = @nam and Ma_Nguon = @ma";
            cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@nam", nam);
            cmd.Parameters.AddWithValue("@ma", ma);
            DataTable dt = Connection.GetDataTable(cmd);
            if (dt.Rows.Count > 0)
            {
                return Convert.ToString(dt.Rows[0][0]);
            }
            else
            {
                return null;
            }
        }
        public void Update(string tablenameset, string idset, string tablenameget, string idget)
        {
            var sql = "";
            if (tablenameget == "Nguon_DMBTC")
            {
                if (tablenameset == "Nguon_SLGOCThu")
                {
                    sql = "sp_update_SLGOCThu_DMBTC";
                } else if (tablenameset == "Nguon_SLGOCChi")
                {
                    sql = "sp_update_SLGOCChi_DMBTC";
                }
            }
            else if (tablenameget == "Nguon_DMBQP")
            {
                if (tablenameset == "Nguon_SLGOCThu")
                {
                    sql = "sp_update_SLGOCThu_DMBBQP";
                }
                else if (tablenameset == "Nguon_SLGOCChi")
                {
                    sql = "sp_update_SLGOCChi_DMBQP";
                }
            }
            else if (tablenameget == "Nguon_CTThu_ChiTiet" && tablenameset == "Nguon_SLGOCThu")
            {                
                sql = "sp_update_SLGOCThu_CTThuCT";                
            }
            else if (tablenameget == "Nguon_CTChi_ChiTiet" && tablenameset == "Nguon_SLGOCChi")
            {
                sql = "sp_update_SLGOCChi_CTChiCT";
            }
            else
            {
                return;
            }
            SqlCommand cmd = new SqlCommand(sql);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Idset", !string.IsNullOrEmpty(idset) ? (object) idset : DBNull.Value);
            cmd.Parameters.AddWithValue("@Idget", idget);
            var count = Connection.UpdateDatabase(cmd);
        }
        public void UpdateSotienChungTu(string tablenameset, string id)
        {
            string SQL = "select SoTien from " + tablenameset + "_ChiTiet where " + (tablenameset == "Nguon_CTThu" ? "Id_CTThu" : "Id_CTChi") + " = @Id and TrangThai = 1 and SoTien <> 0";
            var cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@Id", id);
            DataTable dt = Connection.GetDataTable(cmd);

            decimal tong = 0;
            if (dt.Rows.Count > 0)
            {
                tong = dt.AsEnumerable().Sum(x => x.Field<decimal>("SoTien", 0));
            }

            SQL = "update " + tablenameset + " set TongTien = @tong where Id=@id";
            cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@tong", tong);
            var count = Connection.UpdateDatabase(cmd);
        }
        public int getSoChungTu(string tablename, string loai, string Nam)
        {
            string SQL = "select SoChungTu from " + tablename + " where " + " Nam = @Nam " + (loai != "cap" && tablename == "Nguon_CTChi" ? "and Loai = 0" : "and Loai <> 0") + " and TrangThai = 1 order by SoChungTu DESC";
            var cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@Nam", Nam);
            DataTable dt = Connection.GetDataTable(cmd);

            var count = 1;

            if (dt.Rows.Count > 0)
            {
                for (int i = dt.Rows.Count - 1; i >= 0; i--)
                {
                    if ((count < Convert.ToInt32(dt.Rows[0][0]) && count == Convert.ToInt32(dt.Rows[i][0])) || count == Convert.ToInt32(dt.Rows[0][0]))
                    {
                        count++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
                       
            return count;
        }
        public DataTable GetLoaiChungTu(string loai)
        {
            DataTable ds = new DataTable();
            ds.Columns.Add("loaiChungTu", typeof(String));
            ds.Columns.Add("TenChungTu", typeof(String));

            if (loai == "thu")
            {
                DataRow dtr = ds.NewRow();
                dtr["loaiChungTu"] = "1";
                dtr["TenChungTu"] = "Kinh phí năm trước chuyển sang";
                ds.Rows.Add(dtr);
                dtr = ds.NewRow();
                dtr["loaiChungTu"] = "2";
                dtr["TenChungTu"] = "Kinh phí giao đầu năm";
                ds.Rows.Add(dtr);
                dtr = ds.NewRow();
                dtr["loaiChungTu"] = "3";
                dtr["TenChungTu"] = "Kinh phí bổ sung trong năm trước 30/09";
                ds.Rows.Add(dtr);
                dtr = ds.NewRow();
                dtr["loaiChungTu"] = "4";
                dtr["TenChungTu"] = "Kinh phí bổ sung sau 30/09";
                ds.Rows.Add(dtr);
            }
            else if (loai == "cap")
            {
                DataRow dtr = ds.NewRow();
                dtr["loaiChungTu"] = "1";
                dtr["TenChungTu"] = "Kinh phí giao đầu năm";
                ds.Rows.Add(dtr);
                dtr = ds.NewRow();
                dtr["loaiChungTu"] = "2";
                dtr["TenChungTu"] = "Kinh phí bổ sung trong năm trước 30/09 (Quyết định Bộ)";
                ds.Rows.Add(dtr);
                dtr = ds.NewRow();
                dtr["loaiChungTu"] = "21";
                dtr["TenChungTu"] = "Kinh phí bổ sung trong năm trước 30/09 (Chờ Bộ phê duyệt)";
                ds.Rows.Add(dtr);
                dtr = ds.NewRow();
                dtr["loaiChungTu"] = "3";
                dtr["TenChungTu"] = "Kinh phí bổ sung sau 30/09 (Quyết định Bộ)";
                ds.Rows.Add(dtr);
                dtr = ds.NewRow();
                dtr["loaiChungTu"] = "31";
                dtr["TenChungTu"] = "Kinh phí bổ sung sau 30/09 (Chờ Bộ phê duyệt)";
                ds.Rows.Add(dtr);
            } else
            {
                DataRow dtr = ds.NewRow();
                dtr["loaiChungTu"] = "0";
                dtr["TenChungTu"] = "Phân ngân sách chi bộ quốc phòng";
                ds.Rows.Add(dtr);
            }

            return ds;
        }
        public DataTable GetDSChungTu(string tablename, string loai, string nam)
        {
            string SQL = tablename == "Nguon_CTThu" ? "select Id, ChungTu = 'BTC - ' + CONVERT(nvarchar(10),SoChungTu), SoQD, NgayQD, LoaiCT = case Loai when 1 then N'Kinh phí năm trước chuyển sang' when 2 then N'Kinh phí giao đầu năm' when 3 then N'Kinh phí bổ sung trong năm trước 30/09' else N'Kinh phí bổ sung sau 30/09' end, NoiDung, TongTien, NguoiTao from " + tablename + " where " + " Nam = @Nam and TrangThai = 1 order by SoChungTu DESC"
                : (loai == "cap" ? "select Id, ChungTu = 'CBQP - ' + CONVERT(nvarchar(10),SoChungTu), SoQD, NgayQD, LoaiCT = case Loai when 1 then N'Kinh phí giao đầu năm (đã phân cấp)' when 2 then N'Kinh phí bổ sung trong năm trước 30/09 (Quyết định Bộ)' when 3 then N'Kinh phí bổ sung trong năm trước 30/09 (Chờ Bộ phê duyệt)' when 4 then N'Kinh phí bổ sung sau 30/09 (Quyết định Bộ)' else N'Kinh phí bổ sung sau 30/09 (Chờ Bộ phê duyệt)' end, NoiDung, TongTien, NguoiTao from " + tablename + " where " + " Nam = @Nam and TrangThai = 1 and Loai <> 0 order by SoChungTu DESC" :
                "select Id, ChungTu = 'PCBQP - ' + CONVERT(nvarchar(10),SoChungTu), SoQD, NgayQD, LoaiCT = N'Phân ngân sách chi bộ quốc phòng', NoiDung, TongTien, NguoiTao from " + tablename + " where " + " Nam = @Nam and TrangThai = 1 and Loai = 0 order by SoChungTu DESC");
            var cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@Nam", nam);
            DataTable dt = Connection.GetDataTable(cmd);
            return dt;
        }
        public DataRow GetChungTu(string tablename, string id)
        {
            string SQL = tablename == "Nguon_CTThu" ? "select Id, SoChungTu = CONVERT(nvarchar(10),SoChungTu), SoQD, NgayQD, LoaiCT = case Loai when 1 then N'Kinh phí năm trước chuyển sang' when 2 then N'Kinh phí giao đầu năm' when 3 then N'Kinh phí bổ sung trong năm trước 30/09' else N'Kinh phí bổ sung sau 30/09' end, NoiDung, NguoiTao, Nam  from " + tablename + " where " + " Id = @id and TrangThai = 1 order by SoChungTu DESC"
                : "select Id, SoChungTu = CONVERT(nvarchar(10),SoChungTu), SoQD, NgayQD, LoaiCT = case Loai when 0 then N'Phân ngân sách chi bộ quốc phòng' when 1 then N'Kinh phí giao đầu năm (đã phân cấp)' when 2 then N'Kinh phí bổ sung trong năm trước 30/09 (Quyết định Bộ)' when 3 then N'Kinh phí bổ sung trong năm trước 30/09 (Chờ Bộ phê duyệt)' when 4 then N'Kinh phí bổ sung sau 30/09 (Quyết định Bộ)' else N'Kinh phí bổ sung sau 30/09 (Chờ Bộ phê duyệt)' end, NoiDung, NguoiTao, Nam from " + tablename + " where " + " Id = @id and TrangThai = 1 order by SoChungTu DESC";
            var cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@id", id);
            DataTable dt = Connection.GetDataTable(cmd);
            return dt.Rows[0];
        }
        public void Delete_ChungTuChiTiet(string tablename, string columnname, string id, string maND, string iPSua)
        {
            string SQL = "update " + tablename + " set TrangThai = 0, NguoiSua=@maND, IpSua=@ipSua, NgaySua=@day where " + columnname + "= @id";
            var cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@maND", maND);
            cmd.Parameters.AddWithValue("@ipSua", iPSua);
            cmd.Parameters.AddWithValue("@day", DateTime.Now);
            var count = Connection.UpdateDatabase(cmd);
        }        
        public void Insert(string tenBangSLGOC, string tenBangChiTiet, string id)
        {
            var sql = "";
            switch (tenBangSLGOC)
            {
                case "Nguon_SLGOCThu":
                    sql = "sp_insert_SLGOCThu";
                    break;
                case "Nguon_SLGOCChi":
                    sql = "sp_insert_SLGOCChi";
                    break;
                default:
                    break;
            }
            SqlCommand cmd = new SqlCommand(sql);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", id);
            var count = Connection.UpdateDatabase(cmd);
        }
        public DataTable GetChungTuChiTiet(string tablename, string id, Dictionary<string, string> arrGiaTriTimKiem, string[] _arrDSTruongTimKiem, string id_nguon = null)
        {
            var sql = "";
            switch (tablename)
            {
                case "Nguon_CTThu_ChiTiet":
                    sql = "sp_get_CTThu_ChiTiet";
                    break;
                case "Nguon_CTChi_ChiTiet":
                    sql = "sp_get_CTChi_ChiTiet";
                    break;
                case "Nguon_CTChi_ChiTiet_PhanCap":
                    sql = "sp_get_CTChi_ChiTiet_PhanCap";
                    break;
                case "Nguon_CTCap_ChiTiet":
                    sql = "sp_get_CTCap_ChiTiet";
                    break;
                case "Nguon_CTCap_ChiTiet_PhanCap":
                    sql = "sp_get_CTCap_ChiTiet_PhanCap";
                    break;
                default:
                    break;
            }
            SqlCommand cmd = new SqlCommand(sql);
            cmd.CommandType = CommandType.StoredProcedure;
            for (int i = 0; i < _arrDSTruongTimKiem.Length; i++)
            {
                if (string.IsNullOrEmpty(arrGiaTriTimKiem[_arrDSTruongTimKiem[i]]) == false)                                  
                    cmd.Parameters.AddWithValue("@" + _arrDSTruongTimKiem[i], "%" + arrGiaTriTimKiem[_arrDSTruongTimKiem[i]] + "%");
                else
                    cmd.Parameters.AddWithValue("@" + _arrDSTruongTimKiem[i], DBNull.Value);
            }
            cmd.Parameters.AddWithValue("@id", id);
            if (!string.IsNullOrEmpty(id_nguon))
                cmd.Parameters.AddWithValue("@iddm", id_nguon);
            var dt = Connection.GetDataTable(cmd);
            return dt;
        }

        public bool checkDuLieu(string id)
        {
            string SQL = "select Id from Nguon_SLGOCThu where Id_NguonBTC = @id and TrangThai = 1 union all select Id from Nguon_SLGOCChi where Id_NguonBTC = @id and TrangThai = 1";
            var cmd = new SqlCommand(SQL);
            cmd.Parameters.AddWithValue("@id", id);
            DataTable dt = Connection.GetDataTable(cmd);
            return dt.Rows.Count > 0;
        }
        #endregion
    }
}
