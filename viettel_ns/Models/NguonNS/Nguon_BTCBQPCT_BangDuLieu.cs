using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Viettel.Data;
using Viettel.Services;

namespace VIETTEL.Models
{
    public class Nguon_BTCBQPCT_BangDuLieu :    BangDuLieu_E
    {
        /// /// <summary>
        /// Hàm khởi tạo
        /// </summary>
        /// <param name="iID_MaChungTu"></param>
        /// <param name="MaND">Mã người dùng</param>
        /// <param name="IPSua">IP của máy yêu cầu</param>
        public Nguon_BTCBQPCT_BangDuLieu(string Id_MaNguon, string loai,Dictionary<string, string> arrGiaTriTimKiem, string MaND, string IPSua)
        {
            this._iID_Ma = Id_MaNguon;
            this._MaND = MaND;
            this._IPSua = IPSua;          
            
            _ChiDoc = false;
            _DuocSuaChiTiet = false;
            _dtChiTiet = Get_Dt_Nguon_DMBTC(Id_MaNguon, loai, arrGiaTriTimKiem);
            _dtChiTiet_Cu = _dtChiTiet.Copy();
            DienDuLieu();
        }

        private DataTable Get_Dt_Nguon_DMBTC(string Id_MaNguon, string loai, Dictionary<string, string> arrGiaTriTimKiem)
        {
            DataTable vR;
            string iNamLamViec = MucLucNganSachModels.LayNamLamViec(_MaND);
            string[] arrDSTruong = ("Ma_Nguon").Split(',');

            using (var conn = ConnectionFactory.Default.GetConnection())
            using (var cmdvr = new SqlCommand("sp_nguon_btcbqp_edit", conn))
            {
                cmdvr.CommandType = CommandType.StoredProcedure;
                if (arrGiaTriTimKiem != null)
                {
                    for (int i = 0; i < arrDSTruong.Length; i++)
                    {
                        if (string.IsNullOrEmpty(arrGiaTriTimKiem[arrDSTruong[i]]) == false)
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
                        if (string.IsNullOrEmpty(arrGiaTriTimKiem[arrDSTruong[i]]) == true)
                        {
                            cmdvr.Parameters.AddWithValue("@" + arrDSTruong[i], DBNull.Value);
                        }
                    }
                }
                cmdvr.Parameters.AddWithValue("@nam", iNamLamViec);

                vR = cmdvr.GetDataset().Tables[0];
            }

            if (vR.Rows.Count == 0)
            {
                var nr = vR.NewRow();
                nr["bLaHangTong"] = false;
                vR.Rows.Add(nr);
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
                CotId = "Id";
                CotHangChaId = "Id_Cha";
                CotLaHangCha = "bLaHangTong";
                CapNhap_arrLaHangCha();
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
                string MaHang = Convert.ToString($"{R["Id"]}");
                _arrDSMaHang.Add(MaHang);
            }
        }
        protected override void CapNhap_arrEdit()
        {
            _arrEdit = new List<List<string>>();

            _dtChiTiet.AsEnumerable().ToList()
                .ForEach(r =>
                {
                    var items = new List<string>();
                    DSCots.ToList()
                        .ForEach(c =>
                        {
                            items.Add("1");
                        });

                    _arrEdit.Add(items);
                });
        }
        protected override IEnumerable<Cot> LayDSCot()
        {
            var items = new List<Cot>();
            items.AddRange(
                    new List<Cot>()
                    {
                        new Cot(ten: "Ma_Nguon", hien: false),
                        new Cot(ten: "NoiDung_Nguon", tieuDe: "Nội dung", doRongCot:300, kieuCanLe: "left", chiDoc: false, coDinh: false),
                        new Cot(ten: "Ma_NguonCha", hien: false),
                        new Cot(ten: "bLaHangTong", hien: false),
                        new Cot(ten: "Id", hien: false),
                        new Cot(ten: "Id_cha", hien: false),
                    }
                );
            return items;
        }
    }
}