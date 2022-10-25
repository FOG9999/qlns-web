using System;
using System.Collections.Generic;
using System.Data;
using Viettel.Services;

namespace VIETTEL.Models
{
    /// <summary>
    /// Lớp DuToan_BangDuLieu cho phần bảng của Phân bổ chỉ tiêu
    /// </summary>
    public class Nguon_CTChiChiTiet_BangDuLieu : BangDuLieu_E
    {
        private readonly INguonNSService _nguonNSService = NguonNSService.Default;

        /// <summary>
        /// Hàm khởi tạo
        /// </summary>
        /// <param name="iID_MaChungTu"></param>
        /// <param name="MaND">Mã người dùng</param>
        /// <param name="IPSua">IP của máy yêu cầu</param>
        ///sLNS,sL,sK,sM,sTM,sTTM
        public Nguon_CTChiChiTiet_BangDuLieu(string Id_CTChi, Dictionary<string, string> arrGiaTriTimKiem, string MaND, string IPSua)
        {
            this._iID_Ma = Id_CTChi;
            this._MaND = MaND;
            this._IPSua = IPSua;

            var _arrDSTruongTimKiem = "CTMT,L,K,Ma_Nguon".Split(',');
                       
            _DuocSuaChiTiet = false;
            _ChiDoc = false;
            
            _dtChiTiet = _nguonNSService.GetChungTuChiTiet("Nguon_CTChi_ChiTiet", _iID_Ma, arrGiaTriTimKiem, _arrDSTruongTimKiem);               

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
                    MaHang = string.Format("{0}", R["Id_BTC"]);
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
                    new Cot(ten: "TongThu", tieuDe: "Tổng số tiền BTC cấp", doRongCot:150, chiDoc: true, coDinh: true, kieuNhap: "1"),
                    new Cot(ten: "TongChi", tieuDe: "Tổng số tiền đã chi", doRongCot:150, chiDoc: true, coDinh: true, kieuNhap: "1"),
                    new Cot(ten: "ConLai", tieuDe: "Còn lại", doRongCot:150, chiDoc: true, coDinh: false, kieuNhap: "1"),
                    new Cot(ten: "sMauSac", hien: false),
                    new Cot(ten: "sFontColor", hien: false),
                    new Cot(ten: "sFontBold",hien: false),
                }
            );      
            return items;
        }
        protected override void CapNhap_arrDuLieu()
        {
            _arrDuLieu = new List<List<string>>();
            for (int i = 0; i < _dtChiTiet.Rows.Count; i++)
            {
                _arrDuLieu.Add(new List<string>());
                DataRow R = _dtChiTiet.Rows[i];

                var conlai = Convert.ToDouble(R["ConLai"]);
                var tongchi = Convert.ToDouble(R["TongChi"]);

                for (int j = 0; j < _arrDSMaCot.Count - 3; j++)
                {
                    //Xac dinh gia tri
                    _arrDuLieu[i].Add(Convert.ToString(R[_arrDSMaCot[j]]));
                }

                if (conlai < 0)
                {
                    _arrDuLieu[i].Add("#FF0000");
                    _arrDuLieu[i].Add("");
                    _arrDuLieu[i].Add("");
                }
                else if (conlai == 0)
                {

                    _arrDuLieu[i].Add("#3399FF");
                    _arrDuLieu[i].Add("");
                    _arrDuLieu[i].Add("");
                }
                else
                {
                    if (!Convert.ToBoolean(R["bLaHangTong"]) && tongchi > 0)
                    {
                        _arrDuLieu[i].Add("#FFFF00");
                        _arrDuLieu[i].Add("");
                        _arrDuLieu[i].Add("");
                    } else if (!Convert.ToBoolean(R["bLaHangTong"]) && tongchi == 0)
                    {
                        _arrDuLieu[i].Add("#00FF00");
                        _arrDuLieu[i].Add("");
                        _arrDuLieu[i].Add("");
                    }                 
                }
            }
        }
    }
}
