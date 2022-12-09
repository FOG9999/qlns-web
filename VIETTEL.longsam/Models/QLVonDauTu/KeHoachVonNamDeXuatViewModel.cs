using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.QLVonDauTu
{
    public class KeHoachVonNamDeXuatViewModel
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<VDT_KHV_KeHoachVonNam_DeXuat_ViewModel> Items { get; set; }
        public string chungTuTabIndex { get; set; }
        public string chungTuTongHopTabIndex { get; set; }
    }

    public class VDT_KHV_KeHoachVonNam_DeXuat_ViewModel : VDT_KHV_KeHoachVonNam_DeXuat
    {
        public string sTenDonVi { get; set; }
        public string sTenNguonVon { get; set; }
        public string sDieuChinhTuSKH { get; set; }
        public int iSoLanDieuDieuChinh { get; set; }
        public string sTrangThaiCanBo
        {
            get
            {
                if (bIsCanBoDuyet != null && bIsCanBoDuyet == true)
                    return "Được duyệt";
                else
                    return "Chưa duyệt";
            }
        }
        public string sTrangThaiKetNoi
        {
            get
            {
                if (bIsDuyet != null && bIsDuyet == true)
                    return "Được duyệt";
                else
                    return "Chưa duyệt";
            }

        }
        public bool isDieuChinh
        {
            get
            {
                if (iID_ParentId != null && iID_ParentId == Guid.Empty)
                    return true;
                else
                    return false;
            }
        }
        public bool isChecked { get; set; }
    }

    public class DuAnKeHoachVonNamModel
    {
        public List<DuAnKeHoachVonNam> Items { get; set; }
    }

    public class DuAnKeHoachVonNam
    {
        public Guid IDDuAnID { get; set; }
        public string SMaDuAn { get; set; }
        public string STenDuAn { get; set; }
        public string SDiaDiem { get; set; }
        public int INamKeHoach { get; set; }
        public double? FHanMucDauTu { get; set; }
        public Guid? IIdDonViId { get; set; }
        public string IIDMaDonVi { get; set; }
        public string STenDonVi { get; set; }
        public Guid? IIDLoaiCongTrinhID { get; set; }
        public string STenLoaiCongTrinh { get; set; }
        public int? IIDNguonVonID { get; set; }
        public string STenNguonVon { get; set; }
        public bool IsChecked { get; set; }
        public int? ILoai { get; set; }
    }

    public class KeHoachVonNamDeXuatDataImportModel
    {
        public string sSTT { get; set; }
        public string sTenDuAn { get; set; }
        public string sMaDuAn { get; set; }
        public string sChuDauTu { get; set; }
        public string sMaChuDauTu { get; set; }
        public string sDonViQuanLy { get; set; }
        public string sMaDonViQuanLy { get; set; }
        public string fTongMucDauTuDuocDuyet { get; set; }
        public string fLuyKeVonNamTruoc { get; set; }
        public string fKeHoachVonDuocDuyetNamNay { get; set; }
        public string fVonKeoDaiCacNamTruoc { get; set; }
        public string fUocThucHien { get; set; }
        public string fThuHoiVonUngTruoc { get; set; }
        public string fThanhToan { get; set; }
        public string sLoaiDuAn { get; set; }
    }

    public class VdtKhvVonNamDeXuatDieuChinhOrtherBudgetModel : KHVNDXPrintDataExportModel
    {
        public string STT { get; set; }
        public string STenDuAn { get; set; }
        public string DiaDiemXayDung { get; set; }
        public string DiaDiemMoTaiKhoanDuAn { get; set; }
        public string ChuDauTu { get; set; }
        public string MaSoDuAnDauTu { get; set; }
        public string MaNganhKinhTe { get; set; }
        public string NangLucThietKe { get; set; }
        public string ThoiGianThucHien { get; set; }
        public string SSoQuyetDinh { get; set; }
        public DateTime? DNgayQuyetDinh { get; set; }
        public string SoNgayThangNam
        {
            get => !IsHangCha ? string.Format("{0} - {1}", SSoQuyetDinh, DNgayQuyetDinh.HasValue ? DNgayQuyetDinh.Value.ToString("dd/MM/yyyy") : string.Empty) : string.Empty;
        }
        public double? TongSoVonDauTu { get; set; }
        public double? TongSoVonDauTuTrongNuoc { get; set; }
        public double? KeHoachVonDauTuGiaiDoan { get; set; }
        public double? VonThanhToanLuyKe { get; set; }
        public double? TongSoKeHoachVonNam { get; set; }
        public double? ThuHoiVonDaUngTruoc { get; set; }
        public double? VonThucHienTuDauNamDenNay { get; set; }
        public double? TongSoVonNamDieuChinh { get; set; }
        public double? ThuHoiVonDaUngTruocDieuChinh { get; set; }
        public double? TraNoXDCB { get; set; }
        public string SGhiChu { get; set; }
        public bool IsHangCha { get; set; }
        public int? Loai { get; set; }
        public Guid? IdNhomDuAn { get; set; }
    }

    public class PhanBoVonDonViDieuChinhNSQPReport : KHVNDXPrintDataExportModel
    {
        public string STT { get; set; }
        public string STenDuAn { get; set; }
        public Guid? IIdLoaiCongTrinh { get; set; }
        public Guid? IIdLoaiCongTrinhParent { get; set; }
        public int? Loai { get; set; }
        public int? LoaiParent { get; set; }
        public bool IsHangCha { get; set; }
        public string ThoiGianThucHien { get; set; }
        public string DiaDiemThucHien { get; set; }
        public string ChuDauTu { get; set; }
        public string SoPheDuyet { get; set; }
        public DateTime? NgayPheDuyet { get; set; }
        public double? TongMucDauTu { get; set; }
        public double? TongMucDauTuNSQP { get; set; }
        public double? VonBoTriDenHetNamTruoc { get; set; }
        public double? KeHoachVonDauTuNam { get; set; }
        public double? VonGiaiNganNam { get; set; }
        public double? DieuChinhVonNam { get; set; }
        public string SGhiChu { get; set; }
        public Guid? IdDuAn { get; set; }
        public int? IdNguonVon { get; set; }
        public int? LoaiCongTrinh { get; set; }
        public double VonBoTri5Nam { get; set; }
        public string dNgayPheDuyet {
            
            get => NgayPheDuyet.HasValue ? NgayPheDuyet.Value.ToString("dd/MM/yyyy") : string.Empty;
                }

    }

    public class KHVNDXExportModel : KHVNDXPrintDataExportModel
    {
        public string STT { get; set; }
        public string STenDuAn { get; set; }
        public string SChuDauTu { get; set; }
        public double? TongMucDauTuDuocDuyet { get; set; }
        public double? LuyKeVonThucHienTruocNam { get; set; }
        public double? TongSoKeHoachVon { get; set; }
        public double? KeHoachVonDuocGiao { get; set; }
        public double? VonKeoDaiCacNamTruoc { get; set; }
        public double? UocThucHien { get; set; }
        public double? LuyKeVonDaBoTriHetNam { get; set; }
        public double? TongNhuCauVonNamSau { get; set; }
        public double? ThuHoiVonUngTruoc { get; set; }
        public double? ThanhToan { get; set; }
        public Guid? IIdLoaiCongTrinh { get; set; }
        public Guid? IIdLoaiCongTrinhParent { get; set; }
        public int? Loai { get; set; }
        public int? LoaiParent { get; set; }
        public bool IsHangCha { get; set; }
        public int LoaiDuAn { get; set; }
    }

    public class KHVNDXPrintDataExportModel
    {
        public IEnumerable<NS_NguonNganSach> lstDMNguonNganSach { get; set; }
        public IEnumerable<NS_DonVi> lstDonViQuanLy { get; set; }
        public int iNamKeHoach { get; set; }
        public Guid iID_DonViQuanLyID { get; set; }
        public string iID_MaDonViQuanLyID { get; set; }
        public Guid iID_KeHoachVonNam_DeXuatID { get; set; }
        public string iID_LoaiCongTrinh { get; set; }
        public string txt_TieuDe1 { get; set; }
        public string txt_TieuDe2 { get; set; }
        public string txt_TieuDe3 { get; set; }
        public string sDonViTinh { get; set; }
        public double? fDonViTinh { get; set; }
        public int iIDNguonVon { get; set; }
        public string sTenNguonVon { get; set; }
        public int iIdBaoCaoMau { get; set; }
        public string iLoaiChungTu { get; set; }
        public string iIdCongTrinh { get; set; }
    }

    public class VdtKhvKeHoach5NamDeXuatExportModel
    {
        public string STT { get; set; }
        public Guid IdChiTiet { get; set; }
        public Guid? IdChungTu { get; set; }
        public int? NamLamViec { get; set; }
        public int? ILoai { get; set; }
        public Guid? IdParentVoucher { get; set; }
        public bool? BActive { get; set; }
        public bool? IsGoc { get; set; }
        public Guid? IIdDonViChungTu { get; set; }
        public string SSoKeHoach { get; set; }
        public int IGiaiDoanTu { get; set; }
        public int IGiaiDoanDen { get; set; }
        public Guid? IIdDuAn { get; set; }
        public string IdMaDonVi { get; set; }
        public Guid? IIdParentModified { get; set; }
        public string STen { get; set; }
        public string SDiaDiem { get; set; }
        public Guid? IIdDonVi { get; set; }
        public string STenDonVi { get; set; }
        public string SThoiGianThucHien { get; set; }
        public string STenLoaiCongTrinh { get; set; }
        public Guid? IIdLoaiCongTrinh { get; set; }
        public double? FHanMucDauTu { get; set; }
        public int? IIdNguonVon { get; set; }
        public string STenNguonVon { get; set; }
        public double? FGiaTriNamThuNhat { get; set; }
        public double? FGiaTriNamThuHai { get; set; }
        public double? FGiaTriNamThuBa { get; set; }
        public double? FGiaTriNamThuTu { get; set; }
        public double? FGiaTriNamThuNam { get; set; }
        public double? FGiaTriBoTri { get; set; }
        public string SGhiChu { get; set; }
        public string SMaLoaiCongTrinh { get; set; }
        public string SMaOrder { get; set; }
        public Guid? IdReference { get; set; }
        public double FTongSo { get; set; }
        public double FTongSoNhuCau { get; set; }
        public Guid? IdParent { get; set; }
        public bool IsParent { get; set; }
        public int IsStatus { get; set; }
        public int? Level { get; set; }
        public int? IndexCode { get; set; }
        public string STongHop { get; set; }
        public bool IsHangCha { get; set; }
        public double fHanmucNganhDX { get; set; }
        public double fTongVonBoTriNganh { get; set; }
        public double fVon5namNganhDX { get; set; }
        public double fVonsaunamNganhDX { get; set; }
        public double fHanmucCucTCDX { get; set; }
        public double fTongVonBoTriCuc { get; set; }
        public double fVon5namCTCDX { get; set; }
        public double fVonnamthunhatCTC { get; set; }
        public double fVonsaunamCTCDexuat { get; set; }
        public double fCucTCDeXuat { get; set; }
        public double fDuKienBoTriNamThu2 { get; set; }
    }
}
