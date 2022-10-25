using System;
using System.Collections.Generic;
using System.Data;
using Viettel.Services;

namespace VIETTEL.Models
{
    /// <summary>
    /// Lớp DuToan_BangDuLieu cho phần bảng của Phân bổ chỉ tiêu
    /// </summary>
    public class Nguon_CTThuChiTiet_BangDuLieu : BangDuLieu_E
    {
        private readonly INguonNSService _nguonNSService = NguonNSService.Default;
               
        /// <summary>
        /// Hàm khởi tạo
        /// </summary>
        /// <param name="iID_MaChungTu"></param>
        /// <param name="MaND">Mã người dùng</param>
        /// <param name="IPSua">IP của máy yêu cầu</param>
        ///sLNS,sL,sK,sM,sTM,sTTM
        public Nguon_CTThuChiTiet_BangDuLieu(string Id_CTThu, Dictionary<string, string> arrGiaTriTimKiem, string MaND, string IPSua)
        {
            this._iID_Ma = Id_CTThu;
            this._MaND = MaND;
            this._IPSua = IPSua;
            var _arrDSTruongTimKiem = "CTMT,L,K,Ma_Nguon".Split(',');

            var row = _nguonNSService.GetChungTu("Nguon_CTThu", _iID_Ma);

            string NguoiTao = Convert.ToString(row["NguoiTao"]);
            
            if (MaND == NguoiTao)
            {
                _DuocSuaChiTiet = true;
                _ChiDoc = false;
            }
            else
            {
                _DuocSuaChiTiet = false;
                _ChiDoc = true;
            }

            _dtChiTiet = _nguonNSService.GetChungTuChiTiet("Nguon_CTThu_ChiTiet",_iID_Ma, arrGiaTriTimKiem, _arrDSTruongTimKiem);               

            _dtChiTiet_Cu = _dtChiTiet.Copy();           
            DienDuLieu();
        }
        /// <summary>
        /// Hàm điền dữ liệu
        /// Mục đích: Điền tất cả thông tin vào các tham số của đối tượng Bảng dữ liệu
        /// </summary>
        protected void DienDuLieu()
        {            
            if (_arrDuLieu == null)
            {                
                CotId = "Id_BTC";
                CotHangChaId = "Id_Cha";
                CotLaHangCha = "bLaHangTong";
                CapNhapDSCots();
                CapNhap_HangTongCong();
                CapNhap_arrLaHangCha();
                CapNhapDanhSachMaHang();
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
                string MaHang = "";
                if (!Convert.ToBoolean(R["bLaHangTong"]))
                {
                    MaHang = string.Format("{0}_{1}_{2}", R["Id"], R["Id_BTC"], R["SoTien"]);
                }
                _arrDSMaHang.Add(MaHang);
            }            
        }
        protected override IEnumerable<Cot> LayDSCot()
        {
            var items = new List<Cot>();
            items.AddRange(
                new List<Cot>()
                {
                    new Cot(ten: "CTMT", tieuDe: "Mã CTMT", doRongCot:60, kieuCanLe: "center", chiDoc: true, coDinh: true),
                    new Cot(ten: "L", tieuDe: "Loại", doRongCot:60, kieuCanLe: "center", chiDoc: true, coDinh: true),
                    new Cot(ten: "K", tieuDe: "Khoản", doRongCot:60, kieuCanLe: "center", chiDoc: true, coDinh: true),
                    new Cot(ten: "NoiDung_Nguon", tieuDe: "Nội dung nguồn bộ tài chính", doRongCot:280, kieuCanLe: "left", chiDoc: true, coDinh: true),
                    new Cot(ten: "SoTien", tieuDe: "Số tiền", doRongCot:120, kieuCanLe: "right", kieuNhap: "1"),
                    new Cot(ten: "GhiChu", tieuDe: "Ghi chú", doRongCot:300, kieuCanLe: "left"),
                }
            );      
            return items;
        }      
    }
}
