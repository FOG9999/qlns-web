using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;

namespace Viettel.Models.QLVonDauTu
{
    public class VDT_QT_DeNghiQuyetToanViewModel : VDT_QT_DeNghiQuyetToan
    {
        public int STT { get; set; }
        public int iID_MaNguonNganSach { get; set; }
        public string sMaDuAn { get; set; }
        public string sTen { get; set; }
        public float fTienPheDuyet { get; set; }
        public float fCapPhatBangLenhChi { get; set; }
        public float fCapPhatTaiKhoBac { get; set; }
        public float SumKeHoachVon { get; set; }
        public double? SumCDTQuanLy { get; set; }
        public double? SumKhacQuanLy { get; set; }
        public string sTenDuAn { get; set; }
        public string sTenNhomDuAn { get; set; }
        public string sTenChuDauTu { get; set; }        
        public double GiaTriDeNghiQuyetToan { get; set; }
        public string dThoiGianKhoiCongAsString { get; set; }
        public string dThoiGianHoanThanhAsString { get; set; }
        public List<VDTDuToanNguonVonModel> listNguonVon { get; set; }
        public List<VDT_DA_DuToan_ChiPhi_ViewModel> listChiPhi { get; set; }
        public List<VDT_DA_DuToan_HangMuc_ViewModel> listHangMuc { get; set; }
        public string sGiaTriDeNghiQuyetToan
        {
            get
            {
                return fGiaTriDeNghiQuyetToan.HasValue ? fGiaTriDeNghiQuyetToan.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
        public string sChiPhiThietHai
        {
            get
            {
                return fChiPhiThietHai.HasValue ? fChiPhiThietHai.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
        public string sChiPhiKhongTaoNenTaiSan
        {
            get
            {
                return fChiPhiKhongTaoNenTaiSan.HasValue ? fChiPhiKhongTaoNenTaiSan.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
        public string sTaiSanDaiHanThuocCDTQuanLy
        {
            get
            {
                return fTaiSanDaiHanThuocCDTQuanLy.HasValue ? fTaiSanDaiHanThuocCDTQuanLy.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
        public string sTaiSanDaiHanDonViKhacQuanLy
        {
            get
            {
                return fTaiSanDaiHanDonViKhacQuanLy.HasValue ? fTaiSanDaiHanDonViKhacQuanLy.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
        public string sTaiSanNganHanThuocCDTQuanLy
        {
            get
            {
                return fTaiSanNganHanThuocCDTQuanLy.HasValue ? fTaiSanNganHanThuocCDTQuanLy.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
        public string sTaiSanNganHanDonViKhacQuanLy
        {
            get
            {
                return fTaiSanNganHanDonViKhacQuanLy.HasValue ? fTaiSanNganHanDonViKhacQuanLy.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
        public string sThoiGianLapBaoCao
        {
            get
            {
                return dThoiGianLapBaoCao.HasValue ? dThoiGianLapBaoCao.Value.ToString("dd/MM/yyyy") : string.Empty;
            }
        }
        public string sThoiGianKhoiCong
        {
            get
            {
                return dThoiGianKhoiCong.HasValue ? dThoiGianKhoiCong.Value.ToString("dd/MM/yyyy") : string.Empty;
            }
        }
        public string sThoiGianHoanThanh
        {
            get
            {
                return dThoiGianHoanThanh.HasValue ? dThoiGianHoanThanh.Value.ToString("dd/MM/yyyy") : string.Empty;
            }
        }

        public string sThoiGianKhoiCongDuAn { get; set; }
        public string sThoiGianHoanThanhDuAn { get; set; }
    }

    public class VDTDuToanNguonVonModel: VDT_DA_DuToan_Nguonvon
    {
        public string sTenNguonVon { get; set; }
        public string sTienPheDuyet
        {
            get
            {
                return fTienPheDuyet.HasValue ? fTienPheDuyet.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
        public double? fTienCDTQuyetToan { get; set; }
        public string sTienCDTQuyetToan
        {
            get
            {
                return fTienCDTQuyetToan.HasValue ? fTienCDTQuyetToan.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
        public string sTienToTrinh
        {
            get
            {
                return fTienToTrinh.HasValue ? fTienToTrinh.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
    }
    
    public class NguonVonQuyetToanKeHoachQuery: VDT_DA_DuToan_Nguonvon
    {
        public int IIdNguonVonId { get; set; }
        public double GiaTriPheDuyet { get; set; }
        public string TenNguonVon { get; set; }
        public double TienDeNghi { get; set; }
        public double fCapPhatTaiKhoBac { get; set; }
        public double fCapPhatBangLenhChi { get; set; }
    }

    public class VdtDaDuToanChiPhiDataQuery: VDT_DA_DuToan_Nguonvon
    {
        public Guid? Id { get; set; }
        public string TenChiPhi { get; set; }
        public Guid? IdDuToanChiPhi { get; set; }
        public Guid? IdChiPhi { get; set; }
        public Guid? IdDuToan { get; set; }
        public double? GiaTriPheDuyet { get; set; }
        public Guid? IdChiPhiDuAn { get; set; }
        public bool? IsHangCha { get; set; }
        public bool? IsLoaiChiPhi { get; set; }
        public int? IThuTu { get; set; }
        public Guid? IdChiPhiDuAnParent { get; set; }
        public bool? IsDuAnChiPhiOld { get; set; }
        public bool? IsEditHangMuc { get; set; }
        public string MaOrder { get; set; }
        public double? FGiaTriDieuChinh { get; set; }
        public double? GiaTriTruocDieuChinh { get; set; }
        public int PhanCap { get; set; }
        public Guid ChiPhiId { get; set; }
        public string MaChiPhi { get; set; }
    }

    public class VDT_QT_DeNghiQuyetToanChiTietViewModel : VDT_QT_DeNghiQuyetToan_ChiTiet
    {
        public string sTenDuAn { get; set; }
        public int STT { get; set; }
        public string sMaDuAn { get; set; }
        public string sTenChiPhi { get; set; }
        public string sMaChiPhi { get; set; }
        public double? deviant { get; set; }
        public double? fTongDuToanPheDuyet { get; set; }
        public double? fChiPhiKhongTaoNenTaiSan { get; set; }
        public double? fChiPhiThietHai { get; set; }
    }
}
