using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VIETTEL.Areas.QuyetToan.Models.Api
{
    #region Models


    /// <summary>
    /// A class which represents the QTA_ChungTu table.
    /// </summary>
    /*[Table("QTA_ChungTu")]*/
    public partial class QTA_ChungTu
    {
        [Key]
        public virtual Guid iID_MaChungTu { get; set; }
        public virtual string iID_MaDonVi { get; set; }
        public virtual string iID_MaPhongBan { get; set; }
        public virtual string sTenPhongBan { get; set; }
        public virtual int iNamLamViec { get; set; }
        public virtual int iID_MaNamNganSach { get; set; }
        public virtual string sTienToChungTu { get; set; }
        public virtual int iLoai { get; set; }
        public virtual int iSoChungTu { get; set; }
        public virtual string sDSLNS { get; set; }
        public virtual DateTime dNgayChungTu { get; set; }
        public virtual int iThang_Quy { get; set; }
        public virtual double rTuChi { get; set; }
        public virtual string sNoiDung { get; set; }
        public virtual DateTime? dNgayTao { get; set; }
        public virtual string sID_MaNguoiDungTao { get; set; }
        public virtual DateTime? dNgaySua { get; set; }
        public virtual string sID_MaNguoiDungSua { get; set; }
    }


    /// <summary>
    /// A class which represents the QTA_ChungTuChiTiet table.
    /// </summary>
    /*[Table("QTA_ChungTuChiTiet")]*/
    public partial class QTA_ChungTuChiTiet
    {
        //[Key]
        //public virtual int iID_MaChungTuChiTiet { get; set; }
        public virtual Guid iID_MaChungTu { get; set; }
        public virtual string iID_MaPhongBan { get; set; }
        public virtual string sTenPhongBan { get; set; }
        public virtual int iNamLamViec { get; set; }
        public virtual int iID_MaNguonNganSach { get; set; }
        public virtual int iID_MaNamNganSach { get; set; }
        public virtual int iThang_Quy { get; set; }
        //public virtual int bLoaiThang_Quy { get; set; }
        public virtual string iID_MaDonVi { get; set; }
        public virtual string sTenDonVi { get; set; }
        public virtual string sXauNoiMa { get; set; }
        public virtual string sLNS { get; set; }
        public virtual string sL { get; set; }
        public virtual string sK { get; set; }
        public virtual string sM { get; set; }
        public virtual string sTM { get; set; }
        public virtual string sTTM { get; set; }
        public virtual string sNG { get; set; }
        public virtual string sTNG { get; set; }
        public virtual string sMoTa { get; set; }
        //public virtual string sTenCongTrinh { get; set; }
        //public virtual decimal rNgay { get; set; }
        //public virtual decimal rNgay_ChiTieu { get; set; }
        //public virtual decimal rNgay_DaQuyetToan { get; set; }
        //public virtual decimal rSoNguoi { get; set; }
        //public virtual decimal rSoNguoi_ChiTieu { get; set; }
        //public virtual decimal rSoNguoi_DaQuyetToan { get; set; }
        //public virtual decimal rChiTaiKhoBac { get; set; }
        //public virtual decimal rChiTaiKhoBac_ChiTieu { get; set; }
        //public virtual decimal rChiTaiKhoBac_DaQuyetToan { get; set; }
        //public virtual decimal rTonKho { get; set; }
        //public virtual decimal rTonKho_ChiTieu { get; set; }
        //public virtual decimal rTonKho_DaQuyetToan { get; set; }
        public virtual decimal rTuChi { get; set; }
        public virtual decimal rTuChi_ChiTieu { get; set; }
        //public virtual decimal rTuChi_DaQuyetToan { get; set; }
        //public virtual decimal rChiTapTrung { get; set; }
        //public virtual decimal rChiTapTrung_ChiTieu { get; set; }
        //public virtual decimal rChiTapTrung_DaQuyetToan { get; set; }
        //public virtual decimal rHangNhap { get; set; }
        //public virtual decimal rHangNhap_ChiTieu { get; set; }
        //public virtual decimal rHangNhap_DaQuyetToan { get; set; }
        //public virtual decimal rHangMua { get; set; }
        //public virtual decimal rHangMua_ChiTieu { get; set; }
        //public virtual decimal rHangMua_DaQuyetToan { get; set; }
        //public virtual decimal rHienVat { get; set; }
        //public virtual decimal rHienVat_ChiTieu { get; set; }
        //public virtual decimal rHienVat_DaQuyetToan { get; set; }
        //public virtual decimal rDuPhong { get; set; }
        //public virtual decimal rDuPhong_ChiTieu { get; set; }
        //public virtual decimal rDuPhong_DaQuyetToan { get; set; }
        //public virtual decimal rPhanCap { get; set; }
        //public virtual decimal rPhanCap_ChiTieu { get; set; }
        //public virtual decimal rPhanCap_DaQuyetToan { get; set; }
        //public virtual decimal rTienThu { get; set; }
        //public virtual decimal rTienThu_ChiTieu { get; set; }
        //public virtual decimal rTienThu_DaQuyetToan { get; set; }
        //public virtual decimal rTongSo { get; set; }
        //public virtual decimal rTongSo_ChiTieu { get; set; }
        //public virtual decimal rTongSo_DaQuyetToan { get; set; }
        //public virtual decimal rNgay_DonVi { get; set; }
        //public virtual decimal rSoNguoi_DonVi { get; set; }
        //public virtual decimal rChiTaiKhoBac_DonVi { get; set; }
        //public virtual decimal rTonKho_DonVi { get; set; }
        //public virtual decimal rTuChi_DonVi { get; set; }
        //public virtual decimal rChiTapTrung_DonVi { get; set; }
        //public virtual decimal rHangNhap_DonVi { get; set; }
        //public virtual decimal rHangMua_DonVi { get; set; }
        //public virtual decimal rHienVat_DonVi { get; set; }
        //public virtual decimal rDuPhong_DonVi { get; set; }
        //public virtual decimal rPhanCap_DonVi { get; set; }
        //public virtual decimal rTienThu_DonVi { get; set; }
        //public virtual decimal rTongSo_DonVi { get; set; }
        //public virtual bool bsMaCongTrinh { get; set; }
        //public virtual bool bsTenCongTrinh { get; set; }
        //public virtual bool brNgay { get; set; }
        //public virtual bool brSoNguoi { get; set; }
        //public virtual bool brChiTaiKhoBac { get; set; }
        //public virtual bool brTonKho { get; set; }
        //public virtual bool brTuChi { get; set; }
        //public virtual bool brChiTapTrung { get; set; }
        //public virtual bool brHangNhap { get; set; }
        //public virtual bool brHangMua { get; set; }
        //public virtual bool brHienVat { get; set; }
        //public virtual bool brDuPhong { get; set; }
        //public virtual bool brPhanCap { get; set; }
        //public virtual bool brTienThu { get; set; }
        public virtual string sGhiChu { get; set; }
        //public virtual string sKienNghi { get; set; }
        public virtual DateTime? dNgayTao { get; set; }
        public virtual string sID_MaNguoiDungTao { get; set; }
        public virtual DateTime? dNgaySua { get; set; }
        public virtual string sID_MaNguoiDungSua { get; set; }
        //public virtual int? iLoai { get; set; }
        public virtual decimal rDonViDeNghi { get; set; }
        public virtual decimal rVuotChiTieu { get; set; }
        public virtual decimal rTonThatTonDong { get; set; }
        public virtual decimal rDaCapTien { get; set; }
        public virtual decimal rChuaCapTien { get; set; }
    }


    #endregion

    #region api

    public interface IQTApi
    {
        /// <summary>
        /// Mo ta
        /// </summary>
        /// <param name="namLamViec"></param>
        /// <param name="namNganSach"></param>
        /// <param name="id_phongban"></param>
        /// <param name="id_donvi"></param>
        /// <param name="iThang_Quy"></param>
        /// <param name="iLoai"></param>
        /// <returns></returns>
        IEnumerable<QTA_ChungTu> GetAll(int namLamViec, string namNganSach = "2", string id_phongban = null, string id_donvi = null, string iThang_Quy = null, string lns = null, string iLoai = null);

        /// <summary>
        /// LNS: Xac dinh lay quyet toan chi tiet toi mlns nao: lns, lk, m, tm, Ng
        /// Mac dinh toi Ng
        /// </summary>
        /// <param name="id_chungtu"></param>
        /// <param name="lns"></param>
        /// <returns></returns>
        IEnumerable<QTA_ChungTuChiTiet> GetById(string id_chungtu, string lns);

        IEnumerable<QTA_ChungTuChiTiet> GetById(string id_chungtu);

    }

    #endregion
}
