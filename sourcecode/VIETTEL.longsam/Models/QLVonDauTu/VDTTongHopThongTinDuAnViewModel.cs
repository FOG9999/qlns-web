using System;
using System.Collections.Generic;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.QLVonDauTu
{
    public class VDTTongHopThongTinDuAnViewModel
    {
        public string stt { get; set; }
        public string sTienTe { get; set; }
        public Guid iID_DuAnID { get; set; }
        public string sMaDuAn { get; set; }
        public string sTenDuAn { get; set; }
        public Guid iID_DonViQuanLyID { get; set; }
        public string sTen { get; set; }
        public string sKhoiCong { get; set; }
        public string sKetThuc { get; set; }

        //Phe duyet chu truong dau tu
        public string sSoQuyetDinhCTDT { get; set; }
        public DateTime? dNgayQuyetDinhCTDT { get; set; }
        public string sNgayQuyetDinhCTDT { get { return dNgayQuyetDinhCTDT.HasValue ? dNgayQuyetDinhCTDT.Value.ToString("dd/MM/yyyy") : ""; } }
        public double fGiaTriDauTu { get; set; }
        public double fNganSachQPCCDT { get; set; }
        public double fNganSachKhacCCDT { get; set; }

        //Quyet dịnh dau tu
        public string sSoQuyetDinhQDDT { get; set; }
        public DateTime? dNgayQuyetDinhQDDT { get; set; }
        public string sNgayQuyetDinhQDDT { get { return dNgayQuyetDinhQDDT.HasValue ? dNgayQuyetDinhQDDT.Value.ToString("dd/MM/yyyy") : ""; } }
        public double fTongMucDauTu { get; set; }
        public double fNganSachQPQDDT { get; set; }
        public double fNganSachKhacQDDT { get; set; }


        //Quyet toan du an
        public string sSoQuyetDinhQT { get; set; }
        public DateTime? dNgayQuyetDinhQT { get; set; }
        public string sNgayQuyetDinhQT { get { return dNgayQuyetDinhQT.HasValue ? dNgayQuyetDinhQT.Value.ToString("dd/MM/yyyy") : ""; } }
        public double fGiaTriQuyetToan { get; set; }


        //kế hoạch trung hạn
        public double fKHTTDPD { get; set; }

        //Ke hoach von
        public double fLuyKeVonNamTruoc { get; set; }
        public double fLuyKeVonNamNay { get; set; }
        public double fKeHoachVonNamNay 
        { 
            get {
                return fLuyKeVonNamTruoc + fLuyKeVonNamNay;
            } 
        }

        //Thanh toán
        public double fDaThanhToan { get; set; }
        public double fChuaThanhToan
        {
            get
            {
                return fLuyKeVonNamNay - fDaThanhToan;
            }
        }

    }
    public class VDTTinhHinhDuAnViewModel
    {
        public int STT { get; set; }
        public string sMLNS { get; set; }
        public string sTenNhaThau { get; set; }
        public string sSoHopDong { get; set; }
        public int? iThoiGianThucHien { get; set; }
        public double? fTienHopDong { get; set; }

        public DateTime? dNgayThanhToan { get; set; }
        public double? fSoThanhToan { get; set; }
        public double? fSoTamUng { get; set; }
        public double? fThuHoiTamUng { get; set; }
        public double? fTongCongGiaiNgan { get; set; }
        public DateTime? dNgayDaCapUng { get; set; }

        public double? fSoDaCapUng { get; set; }
    }

    public class VDTBaoCaoCapPhatViewModel
    {
        public string iID_MaDonVi { get; set; }
        public string sTen { get; set; }
        public int? iNamKeHoach { get; set; }
        public double? fCapBacTaiKhoBac_DTNN { get; set; }
        public double? fCapBangLenhChi_DTNN { get; set; }
        public double? fTong_VUNCT { get; set; }
        public double? fCapBacTaiKhoBac_VUNCT { get; set; }
        public double? fCapBangLenhChi_VUNCT { get; set; }
        public double? fThanhToanKLHT { get; set; }
        public double? fTamUng { get; set; }
        public double? fThuHoiTamUng { get; set; }
        public string sGhiChu { get; set; }
        public double? fKhac { get; set; }
    }
    
    public class VDTBaoCaoTinhHinhDATTChungViewModel
    {
        public string sTenDuAn { get; set; }

        public string sTenDonVi { get; set; }
        public string sSoQuyetDinh { get; set; }
        public DateTime? dNgayQuyetDinh { get; set; }
        public double? fTongMucDauTuPheDuyet { get; set; }
        public string sThoiGianThucHien { get; set; }
        public string sTenNguonNganSach { get; set; }
        public double? fLuyKeVonDaBoTri { get; set; }
        public double? fNganSachQuocPhong { get; set; }
        public double? fLuyKeThanhToanQuaKhoBac { get; set; }
        public double? fKeHoachUng { get; set; }
        public double? fNguonNganSachQuocPhong { get; set; }

    }
}