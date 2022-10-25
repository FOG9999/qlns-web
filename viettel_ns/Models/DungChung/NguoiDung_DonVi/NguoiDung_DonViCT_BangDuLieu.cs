using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Viettel.Data;
using Viettel.Services;

namespace VIETTEL.Models
{
    public class NguoiDung_DonViCT_BangDuLieu :    BangDuLieu_E
    {
        /// /// <summary>
        /// Hàm khởi tạo
        /// </summary>
        /// <param name="iID_MaChungTu"></param>
        /// <param name="MaND">Mã người dùng</param>
        /// <param name="IPSua">IP của máy yêu cầu</param>
        public NguoiDung_DonViCT_BangDuLieu(string maND, Dictionary<String, String> arrGiaTriTimKiem, String MaND, String IPSua)
        {
            this._iID_Ma = maND;
            this._MaND = MaND;
            this._IPSua = IPSua;
          
            _dtBang = null;
            
            _ChiDoc = false;
            _DuocSuaChiTiet = true;
            _dtChiTiet = Get_Dt_NguoiDungCT_DonVi(arrGiaTriTimKiem);
            _dtChiTiet_Cu = _dtChiTiet.Copy();
            DienDuLieu();
        }

        private DataTable Get_Dt_NguoiDungCT_DonVi(Dictionary<string, string> arrGiaTriTimKiem)
        {
            DataTable vR;
            string iNamLamViec = MucLucNganSachModels.LayNamLamViec(_MaND);
            string[] arrDSTruong = ("iID_MaDonVi").Split(',');

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmdvr = new SqlCommand("ns_nguoidung_donvi_edit", conn))
            {
                cmdvr.CommandType = CommandType.StoredProcedure;
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
                cmdvr.Parameters.AddWithValue("@nam", iNamLamViec);
                cmdvr.Parameters.AddWithValue("@sMaNguoiDung", _iID_Ma);

                vR = cmdvr.GetDataset().Tables[0];
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
                String MaHang = Convert.ToString($"{R["iID_MaNguoiDungDonVi"]}_{R["id"]}");
                _arrDSMaHang.Add(MaHang);
            }
        }
        protected override IEnumerable<Cot> LayDSCot()
        {
            var items = new List<Cot>();            
            items.AddRange(
                    new List<Cot>()
                    {
                        new Cot(ten: "Ten", tieuDe: "Tên đơn vị phân quyền", doRongCot:300, kieuCanLe: "left", chiDoc: true, coDinh: false),                       
                        new Cot(ten: "bCon", tieuDe: "Phân quyền", doRongCot:100, kieuCanLe: "right", chiDoc: false, coDinh: false, kieuNhap: "2"),
                        new Cot(ten: "id", hien: false),
                    }
                );
            return items;
        }
    }
}