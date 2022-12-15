using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.QLVonDauTu
{
    public class VDT_DA_DuToan_ViewModel : VDT_DA_DuToan
    {
        public string sMaDonViQuanLy { get; set; }
        public Guid? iID_QDDauTuID { get; set; }
        public Guid? iID_ChuTruongDauTuID { get; set; }
        public string sTenDuToan { get; set; }
        public string sTenDuAn { get; set; }
        public string sMaDuAn { get; set; }
        public string sDiaDiem { get; set; }
        public string sKhoiCong { get; set; }
        public string sKetThuc { get; set; }
        public double fTongMucDauTu { get; set; }
        public string sTenDonViQL { get; set; }
        public double dGiaTriDuToanSauDieuChinh { get; set; }
        public int iTongSoDieuChinh { get; set; }
        public int iLoaiQuyetDinh { get; set; }
        public string sLoaiQuyetDinh
        {
            get
            {
                return bLaTongDuToan ? " Tổng dự toán" : "Dự toán";
            }
        }
        public bool isDieuChinh { get; set; }
        public string sNgayQuyetDinh
        {
            get
            {
                return (dNgayQuyetDinh == null ? "" : dNgayQuyetDinh.Value.ToString("dd/MM/yyyy"));
            }
        }
        public IEnumerable<VDT_DA_DuToan_ChiPhi_ViewModel> ListChiPhi { get; set; }
        public IEnumerable<VDT_DA_DuToan_Nguonvon_ViewModel> ListNguonVon { get; set; }
        public IEnumerable<VDT_DA_DuToan_HangMuc_ViewModel> ListHangMuc { get; set; }
    }
    public class VDTPheDuyetTKTCVaTDTViewModel
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<VDT_DA_DuToan_ViewModel> Items { get; set; }
    }

    public class VDT_DA_DuToan_ChiPhi_ViewModel
    {
        public Guid? iID_DuAn_ChiPhi { get; set; }
        public string sTenChiPhi { get; set; }
        public Guid iID_ChiPhiID { get; set; }
        public Guid? iID_ChiPhi_Parent { get; set; }
        public double fTienPheDuyet { get; set; }
        public double fGiaTriDieuChinh { get; set; }
        public double fTienPheDuyetQDDT { get; set; }
        public int? iThuTu { get; set; }
        public bool isDelete { get; set; }
        public bool isParent { get; set; }

        public double? fGiaTriKiemToan { get; set; }
        public double? fGiaTriDeNghiQuyetToan { get; set; }
        public double? fGiaTriQuyetToanAB { get; set; }
        public double? fGiaTriThamTra { get; set; }
        public double? fGiaTriQuyetToan { get; set; }
        public string sMaDuAn { get; set; }
        public string sTenDuAn { get; set; }
        public string iSTT { get; set; }
        public string ordpc { get; set; }
        public string sGiaTriThamTra
        {
            get
            {
                return fGiaTriThamTra.HasValue ? fGiaTriThamTra.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
        public string sGiaTriQuyetToan
        {
            get
            {
                return fGiaTriQuyetToan.HasValue ? fGiaTriQuyetToan.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
        public double? fChenhLechSoVoiDeNghi
        {
            get
            {
                return (fGiaTriQuyetToan == null ? 0 : fGiaTriQuyetToan) - fGiaTriDeNghiQuyetToan;
            }
        }

        public double? fChenhLechSoVoiDuToanNhap
        {
            get
            {
                return (fGiaTriQuyetToan == null ? 0 : fGiaTriQuyetToan) - fTienPheDuyet;
            }
        }

        public string sChenhLenhSoVoiDuToanNhap
        {
            get
            {
                return fChenhLechSoVoiDuToanNhap.HasValue ? fChenhLechSoVoiDuToanNhap.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
        public string sChenhLechSoVoiDeNghi
        {
            get
            {
                return fChenhLechSoVoiDeNghi.HasValue ? fChenhLechSoVoiDeNghi.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }

        public double? fChenhLenhSoVoiDuToan
        {
            get
            {
                return (fGiaTriDeNghiQuyetToan == null ? 0 : fGiaTriDeNghiQuyetToan) - fTienPheDuyet;
            }
        }
        public double? fChenhLenhSoVoiQuyetToanAB
        {
            get
            {
                return (fGiaTriDeNghiQuyetToan == null ? 0 : fGiaTriDeNghiQuyetToan) - (fGiaTriQuyetToanAB ?? 0);
            }
        }
        public double? fChenhLechSoVoiKetQuaKiemToan
        {
            get
            {
                return (fGiaTriDeNghiQuyetToan ?? 0) - (fGiaTriKiemToan ?? 0);
            }
        }
        public string sTienPheDuyet
        {
            get
            {
                return fTienPheDuyet.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
        public string sTienPheDuyetQDDT
        {
            get
            {
                return fTienPheDuyetQDDT.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
        public string sGiaTriKiemToan
        {
            get
            {
                return fGiaTriKiemToan.HasValue ? fGiaTriKiemToan.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
        public string sGiaTriDeNghiQuyetToan
        {
            get
            {
                return fGiaTriDeNghiQuyetToan.HasValue ? fGiaTriDeNghiQuyetToan.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
        public string sGiaTriQuyetToanAB
        {
            get
            {
                return fGiaTriQuyetToanAB.HasValue ? fGiaTriQuyetToanAB.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
        public string sChenhLenhSoVoiDuToan
        {
            get
            {
                return fChenhLenhSoVoiDuToan.HasValue ? fChenhLenhSoVoiDuToan.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
        public string sChenhLenhSoVoiQuyetToanAB
        {
            get
            {
                return fChenhLenhSoVoiQuyetToanAB.HasValue ? fChenhLenhSoVoiQuyetToanAB.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
        public string sChenhLechSoVoiKetQuaKiemToan
        {
            get
            {
                return fChenhLechSoVoiKetQuaKiemToan.HasValue ? fChenhLechSoVoiKetQuaKiemToan.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }

    }

    public class VDT_DA_NhaThau_GoiThau_ViewModel
    {
        public Guid? iID_DuAn_GoiThau { get; set; }
        public string sTenGoiThau { get; set; }
        public Guid iID_GoiThauID { get; set; }
        public double fTienPheDuyet { get; set; }
        public double fGiaTriDieuChinh { get; set; }
        public double fTienPheDuyetQDDT { get; set; }
        public int? iThuTu { get; set; }
        public bool isDelete { get; set; }
        public bool isParent { get; set; }

        public double? fGiaTriKiemToan { get; set; }
        public double? fGiaTriDeNghiQuyetToan { get; set; }
        public double? fGiaTriQuyetToanAB { get; set; }
        public double? fGiaTriThamTra { get; set; }
        public double? fGiaTriQuyetToan { get; set; }
        public string sMaDuAn { get; set; }
        public string sTenDuAn { get; set; }
        public string iSTT { get; set; }
        public string sGiaTriThamTra
        {
            get
            {
                return fGiaTriThamTra.HasValue ? fGiaTriThamTra.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
        public string sGiaTriQuyetToan
        {
            get
            {
                return fGiaTriQuyetToan.HasValue ? fGiaTriQuyetToan.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
        public double? fChenhLechSoVoiDeNghi
        {
            get
            {
                return (fGiaTriQuyetToan ?? 0) - fGiaTriDeNghiQuyetToan;
            }
        }

        public double? fChenhLechSoVoiDuToanNhap
        {
            get
            {
                return (fGiaTriQuyetToan ?? 0) - fTienPheDuyet;
            }
        }

        public string sChenhLenhSoVoiDuToanNhap
        {
            get
            {
                return fChenhLechSoVoiDuToanNhap.HasValue ? fChenhLechSoVoiDuToanNhap.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
        public string sChenhLechSoVoiDeNghi
        {
            get
            {
                return fChenhLechSoVoiDeNghi.HasValue ? fChenhLechSoVoiDeNghi.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }

        public double? fChenhLenhSoVoiDuToan
        {
            get
            {
                return (fGiaTriDeNghiQuyetToan ?? 0) - fTienPheDuyet;
            }
        }
        public double? fChenhLenhSoVoiQuyetToanAB
        {
            get
            {
                return (fGiaTriDeNghiQuyetToan ?? 0) - (fGiaTriQuyetToanAB ?? 0);
            }
        }
        public double? fChenhLechSoVoiKetQuaKiemToan
        {
            get
            {
                return (fGiaTriDeNghiQuyetToan ?? 0) - (fGiaTriKiemToan ?? 0);
            }
        }
        public string sTienPheDuyet
        {
            get
            {
                return fTienPheDuyet.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
        public string sGiaTriKiemToan
        {
            get
            {
                return fGiaTriKiemToan.HasValue ? fGiaTriKiemToan.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
        public string sGiaTriDeNghiQuyetToan
        {
            get
            {
                return fGiaTriDeNghiQuyetToan.HasValue ? fGiaTriDeNghiQuyetToan.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
        public string sGiaTriQuyetToanAB
        {
            get
            {
                return fGiaTriQuyetToanAB.HasValue ? fGiaTriQuyetToanAB.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
        public string sChenhLenhSoVoiDuToan
        {
            get
            {
                return fChenhLenhSoVoiDuToan.HasValue ? fChenhLenhSoVoiDuToan.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
        public string sChenhLenhSoVoiQuyetToanAB
        {
            get
            {
                return fChenhLenhSoVoiQuyetToanAB.HasValue ? fChenhLenhSoVoiQuyetToanAB.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
        public string sChenhLechSoVoiKetQuaKiemToan
        {
            get
            {
                return fChenhLechSoVoiKetQuaKiemToan.HasValue ? fChenhLechSoVoiKetQuaKiemToan.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }

    }
    public class VDT_DA_DuToan_Nguonvon_ViewModel
    {
        public int iID_NguonVonID { get; set; }
        public string sTenNguonVon { get; set; }
        public double fTienPheDuyet { get; set; }
        public double fGiaTriDieuChinh { get; set; }
        public double fGiaTriTruocDieuChinh { get; set; }
        public double fTienPheDuyetQDDT { get; set; }
        public bool isDelete { get; set; }
    }

    public class VDT_DA_DuToan_HangMuc_ViewModel
    {
        public Guid iID_HangMucID { get; set; }
        public Guid? iID_ParentID { get; set; }
        public string sTenHangMuc { get; set; }
        public string sTenLoaiCongTrinh { get; set; }
        public string smaOrder { get; set; }
        public Guid? iID_LoaiCongTrinhID { get; set; }
        public Guid iID_DuAn_ChiPhi { get; set; }
        public double? fTienPheDuyet { get; set; }
        public double? fTienPheDuyetQDDT { get; set; }
        public double? fGiaTriDieuChinh { get; set; }
        public Guid? iID_HangMucPhanChia { get; set; }
        public bool isParent { get; set; }
        public double? fGiaTriKiemToan { get; set; }
        public double? fGiaTriDeNghiQuyetToan { get; set; }
        public double? fGiaTriQuyetToanAB { get; set; }
        public double? fGiaTriThamTra { get; set; }
        public double? fGiaTriQuyetToan { get; set; }
        public string sGiaTriThamTra
        {
            get
            {
                return fGiaTriThamTra.HasValue ? fGiaTriThamTra.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
        public string sGiaTriQuyetToan
        {
            get
            {
                return fGiaTriQuyetToan.HasValue ? fGiaTriQuyetToan.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
        public double? fChenhLechSoVoiDeNghi
        {
            get
            {
                return (fGiaTriQuyetToan ?? 0) - fGiaTriDeNghiQuyetToan;
            }
        }

        public double? fChenhLechSoVoiDuToanNhap
        {
            get
            {
                return (fGiaTriQuyetToan ?? 0) - fTienPheDuyet;
            }
        }
        public string sChenhLenhSoVoiDuToanNhap
        {
            get
            {
                return fChenhLechSoVoiDuToanNhap.HasValue ? fChenhLechSoVoiDuToanNhap.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
        public string sChenhLechSoVoiDeNghi
        {
            get
            {
                return fChenhLechSoVoiDeNghi.HasValue ? fChenhLechSoVoiDeNghi.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
        public double? fChenhLenhSoVoiDuToan
        {
            get
            {
                return (fGiaTriDeNghiQuyetToan ?? 0) - fTienPheDuyet;
            }
        }
        public double? fChenhLenhSoVoiQuyetToanAB
        {
            get
            {
                return (fGiaTriDeNghiQuyetToan ?? 0) - (fGiaTriQuyetToanAB ?? 0);
            }
        }
        public double? fChenhLechSoVoiKetQuaKiemToan
        {
            get
            {
                return (fGiaTriDeNghiQuyetToan ?? 0) - (fGiaTriKiemToan ?? 0);
            }
        }
        public string sTienPheDuyet
        {
            get
            {
                return fTienPheDuyet.HasValue ? fTienPheDuyet.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
        public string sGiaTriKiemToan
        {
            get
            {
                return fGiaTriKiemToan.HasValue ? fGiaTriKiemToan.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
        public string sGiaTriDeNghiQuyetToan
        {
            get
            {
                return fGiaTriDeNghiQuyetToan.HasValue ? fGiaTriDeNghiQuyetToan.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
        public string sGiaTriQuyetToanAB
        {
            get
            {
                return fGiaTriQuyetToanAB.HasValue ? fGiaTriQuyetToanAB.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
        public string sChenhLenhSoVoiDuToan
        {
            get
            {
                return fChenhLenhSoVoiDuToan.HasValue ? fChenhLenhSoVoiDuToan.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
        public string sChenhLenhSoVoiQuyetToanAB
        {
            get
            {
                return fChenhLenhSoVoiQuyetToanAB.HasValue ? fChenhLenhSoVoiQuyetToanAB.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
        public string sChenhLechSoVoiKetQuaKiemToan
        {
            get
            {
                return fChenhLechSoVoiKetQuaKiemToan.HasValue ? fChenhLechSoVoiKetQuaKiemToan.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
    }

    public class VDT_DA_DuToan_ChiPhi_ByPheDuyetDuAn
    {
        public VDT_DA_DuToan_ChiPhi_ByPheDuyetDuAn()
        {
            sTenChiPhiCha = "";
            iID_ChiPhiCha = "";
        }
        public string Id { get; set; }
        public string iID_ChiPhiID { get; set; }
        public string sTenChiPhi { get; set; }
        public string sTenChiPhiCha { get; set; }
        public string iID_ChiPhiCha { get; set; }
        public double? fTienPheDuyet { get; set; }
        public bool IsDefault { get; set; }
    }

    public class VDT_DA_DuAnToan_NguonVon_ByPheDuyetDuAn
    {
        public int? iID_NguonVonID { get; set; }
        public string sTenNguonVon { get; set; }
        public double? fTienPheDuyet { get; set; }
    }

    public class VDT_DA_DuToan_HangMuc_ByPheDuyetDuAn
    {
        public VDT_DA_DuToan_HangMuc_ByPheDuyetDuAn()
        {
            sHangMucCha = "";
        }
        public string Id { get; set; }
        public string sMaHangMuc { get; set; }
        public string sTenHangMuc { get; set; }
        public string sHangMucCha { get; set; }
        public double? fTienHangMuc { get; set; }
        public string iID_ChiPhiID { get; set; }
    }


}
