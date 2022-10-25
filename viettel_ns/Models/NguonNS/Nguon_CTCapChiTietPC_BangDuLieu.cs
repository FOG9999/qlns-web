using System;
using System.Collections.Generic;
using System.Data;
using Viettel.Services;

namespace VIETTEL.Models
{
    /// <summary>
    /// Lớp DuToan_BangDuLieu cho phần bảng của Phân bổ chỉ tiêu
    /// </summary>
    public class Nguon_CTCapChiTietPC_BangDuLieu : BangDuLieu_E
    {
        private readonly INguonNSService _nguonNSService = NguonNSService.Default;
        private readonly ISharedService _sharedService = SharedService.Default;

        protected string _id_nguonBTC;
        
        /// <summary>
        /// Hàm khởi tạo
        /// </summary>
        /// <param name="iID_MaChungTu"></param>
        /// <param name="MaND">Mã người dùng</param>
        /// <param name="IPSua">IP của máy yêu cầu</param>
        ///sLNS,sL,sK,sM,sTM,sTTM
        public Nguon_CTCapChiTietPC_BangDuLieu(string Id_CTCap, string Id_NguonBTC, Dictionary<string, string> arrGiaTriTimKiem, string MaND, string IPSua)
        {
            this._iID_Ma = Id_CTCap;
            this._MaND = MaND;
            this._IPSua = IPSua;
            var _arrDSTruongTimKiem = "Ma_Nguon".Split(',');
            
            var row = _nguonNSService.GetChungTu("Nguon_CTChi", _iID_Ma);

            _id_nguonBTC = Id_NguonBTC;
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
            
            _dtChiTiet = _nguonNSService.GetChungTuChiTiet("Nguon_CTCap_ChiTiet_PhanCap", _iID_Ma, arrGiaTriTimKiem, _arrDSTruongTimKiem, _id_nguonBTC);               

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
                CotId = "Id_danhmuc";
                CotHangChaId = "Id_Cha";
                CotLaHangCha = "bLaHangTong";
                CapNhapDSCots();
                CapNhap_arrLaHangCha();
                CapNhapDanhSachMaHang();
                CapNhap_arrEdit();
                CapNhap_arrDuLieu();
                CapNhap_arrThayDoi();
            }
        }
        protected override void CapNhap_arrLaHangCha()
        {
            //Xác định hàng là hàng cha, con
            _arrCSCha = new List<int>();
            _arrLaHangCha = new List<bool>();

            for (int i = 0; i < dtChiTiet.Rows.Count; i++)
            {
                var r = dtChiTiet.Rows[i];
                //Xac dinh hang nay co phai la hang cha khong?
                arrLaHangCha.Add(Convert.ToBoolean(r[CotLaHangCha]));

                int parent = -1;
                for (int j = i - 1; j >= 0; j--)
                {
                    if (Convert.ToString(r[CotHangChaId]) ==
                        Convert.ToString(dtChiTiet.Rows[j]["Id_BTC"]) && r[CotId].ToString() == dtChiTiet.Rows[j][CotId].ToString())
                    {
                        parent = j;
                        break;
                    }
                }
                arrCSCha.Add(parent);
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
                    MaHang = string.Format("{0}_{1}_{2}", R["Id"], R["Id_danhmuc"],R["SoTien"]);
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
                    new Cot(ten: "NoiDung_Nguon", tieuDe: "Nội dung nguồn chi bộ quốc phòng", doRongCot:280, kieuCanLe: "left", chiDoc: true, coDinh: true),
                    new Cot(ten: "TenDonVi", tieuDe: "Đơn vị", doRongCot:200, kieuCanLe: "left", chiDoc: false, coDinh: false,kieuNhap: "3"),
                    new Cot(ten: "TenPhongBan", tieuDe: "Phòng ban", doRongCot:200, kieuCanLe: "left", chiDoc: false, coDinh: false,kieuNhap: "3"),
                    new Cot(ten: "SoTien", tieuDe: "Số tiền", doRongCot:120, kieuCanLe: "right", kieuNhap: "1"),
                    new Cot(ten: "GhiChu", tieuDe: "Ghi chú", doRongCot:300, kieuCanLe: "left"),
                    new Cot(ten: "Id_MaDonVi", hien: false),
                    new Cot(ten: "Id_PhongBan", hien: false),
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

                var conlai = Convert.ToDouble(R["SoTien"]);

                for (int j = 0; j < _arrDSMaCot.Count - 3; j++)
                {
                    //Xac dinh gia tri
                    _arrDuLieu[i].Add(Convert.ToString(R[_arrDSMaCot[j]]));
                }

                if (string.IsNullOrEmpty(R["Id_Cha"].ToString()))
                {
                    if (conlai < 0)
                    {
                        _arrDuLieu[i].Add("#FF0000");
                        _arrDuLieu[i].Add("");
                        _arrDuLieu[i].Add("");
                    }
                    else if(conlai == 0)
                    {
                        _arrDuLieu[i].Add("#3399FF");
                        _arrDuLieu[i].Add("");
                        _arrDuLieu[i].Add("");
                    }
                    else
                    {                        
                        _arrDuLieu[i].Add("#ffff00");
                        _arrDuLieu[i].Add("");
                        _arrDuLieu[i].Add("");                        
                    }
                }
            }
        }
    }
}
