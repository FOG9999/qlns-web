using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Viettel.Data;
using Viettel.Services;

namespace VIETTEL.Models
{
    public class NguoiDung_DonVi_BangDuLieu:    BangDuLieu_E
    {
        /// /// <summary>
        /// Hàm khởi tạo
        /// </summary>
        /// <param name="iID_MaChungTu"></param>
        /// <param name="MaND">Mã người dùng</param>
        /// <param name="IPSua">IP của máy yêu cầu</param>
        public NguoiDung_DonVi_BangDuLieu(Dictionary<String, String> arrGiaTriTimKiem, String MaND, String IPSua)
        {
            this._iID_Ma = "";
            this._MaND = MaND;
            this._IPSua = IPSua;
          
            _dtBang = null;
            
            _ChiDoc = false;
            _DuocSuaChiTiet = true;
            _dtChiTiet = Get_Dt_NguoiDung_DonVi(arrGiaTriTimKiem,MaND);
            _dtChiTiet_Cu = _dtChiTiet.Copy();
            DienDuLieu();
        }

        private DataTable Get_Dt_NguoiDung_DonVi(Dictionary<string, string> arrGiaTriTimKiem, string maND)
        {
            DataSet vS;
            string iNamLamViec = MucLucNganSachModels.LayNamLamViec(maND);
            string[] arrDSTruong = ("sMaNguoiDung,iID_MaDonVi").Split(',');

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmdvr = new SqlCommand("ns_nguoidung_donvi_index", conn))
            {
                cmdvr.CommandType = CommandType.StoredProcedure;
                if (arrGiaTriTimKiem != null)
                {
                    for (int i = 0; i < arrDSTruong.Length; i++)
                    {
                        if (String.IsNullOrEmpty(arrGiaTriTimKiem[arrDSTruong[i]]) == false)
                        {
                            cmdvr.Parameters.AddWithValue("@" + arrDSTruong[i], "%" + arrGiaTriTimKiem[arrDSTruong[i]] + "%");
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
                cmdvr.Parameters.AddWithValue("@nam", iNamLamViec);

                vS = cmdvr.GetDataset();
            }

            var vR = vS.Tables[0].SelectDistinct("NguoiDung","sMaNguoiDung");
            vR.Columns.Add(new DataColumn($"iID_MaDonVi", typeof(string)));
            vR.Columns.Add(new DataColumn($"sTenDonVi", typeof(string)));

            for (int i = 0; i < vR.Rows.Count; i++)
            {
                var row = vR.Rows[i];
                row["iID_MaDonVi"] = !string.IsNullOrEmpty(vS.Tables[1].AsEnumerable().Where(x => x.Field<string>("sMaNguoiDung") == row["sMaNguoiDung"].ToString()).Select(x => x.Field<string>("iID_MaDonVi")).Join(",")) 
                    ? vS.Tables[1].AsEnumerable().Where(x => x.Field<string>("sMaNguoiDung") == row["sMaNguoiDung"].ToString()).Select(x => x.Field<string>("iID_MaDonVi")).Join(", ") : "";
                row["sTenDonVi"] = !string.IsNullOrEmpty(vS.Tables[1].AsEnumerable().Where(x => x.Field<string>("sMaNguoiDung") == row["sMaNguoiDung"].ToString()).Select(x => x.Field<string>("Ten")).Join(","))
                    ? vS.Tables[1].AsEnumerable().Where(x => x.Field<string>("sMaNguoiDung") == row["sMaNguoiDung"].ToString()).Select(x => x.Field<string>("Ten")).Join(", ") : "";
            }

            return vR;
        }

        /// <summary>
        /// Hàm điền dữ liệu
        /// Mục đích: Điền tất cả thông tin vào các tham số của đối tượng Bảng dữ liệu
        /// </summary>
        protected void DienDuLieu()
        {
            if (_arrDuLieu == null)
            {
                CapNhapDanhSachMaHang();                
                CapNhapDSCots();
                CapNhap_arrEdit();
                CapNhap_arrDuLieu();
                CapNhap_arrThayDoi();
            }
        }

        /// <summary>
        /// Hàm cập nhập vào tham số _arrDSMaHang
        /// </summary>
        protected void CapNhapDanhSachMaHang()
        {
            _arrDSMaHang = new List<string>();
            for (int i = 0; i < _dtChiTiet.Rows.Count; i++)
            {
                DataRow R = _dtChiTiet.Rows[i];
                String MaHang = Convert.ToString(R["sMaNguoiDung"]);
                _arrDSMaHang.Add(MaHang);
            }
        }
        protected override IEnumerable<Cot> LayDSCot()
        {
            var items = new List<Cot>();            
            items.AddRange(
                    new List<Cot>()
                    {
                        new Cot(ten: "sMaNguoiDung", tieuDe: "Mã người dùng", doRongCot:150, kieuCanLe: "left", chiDoc: true, coDinh: false),
                        new Cot(ten: "sTenDonVi", tieuDe: "Danh sách đơn vị quản lý", doRongCot:1200, kieuCanLe: "left", chiDoc: true, coDinh: false),                       
                        new Cot(ten: "iID_MaDonVi", hien: false),
                    }
                );
            return items;
        }
    }
}