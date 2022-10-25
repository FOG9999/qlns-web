using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.QLVonDauTu
{
    public class KeHoach5NamDuocDuyetViewModel
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<KeHoach5NamDuocDuyetModel> Items { get; set; }
    }

    public class KeHoach5NamDuocDuyetModel : VDT_KHV_KeHoach5Nam
    {
        public string sTenDonvVi { get; set; }
        public string sChungTuDeXuat { get; set; }
        public double? fGiaTriDieuChinh { get; set; }
        public string SoLanDieuChinh { get; set; }
        public string DieuChinhTu { get; set; }
        public string sLoaiDuAn
        {
            get => iLoai.Equals(2) ? "Chuyển tiếp" : "Khởi công mới";
        }
    }

    public class KH5NDDExport
    {
        public IEnumerable<NS_NguonNganSach> lstDMNguonNganSach { get; set; }
        public IEnumerable<NS_DonVi> lstDonViQuanLy { get; set; }
        public IEnumerable<VDT_DM_DonViThucHienDuAn> lstDonViThucHienDuAn { get; set; }
        public int iGiaiDoanTu { get; set; }
        public int iGiaiDoanDen { get; set; }
        public Guid iID_DonViQuanLyID { get; set; }
        public Guid iID_KeHoach5NamID { get; set; }
        public int? iID_NguonVonID { get; set; }
        public string sTenNguonVon { get; set; }
        public string iID_LoaiCongTrinh { get; set; }
        public string txt_TieuDe1 { get; set; }
        public string txt_TieuDe2 { get; set; }
        public string txt_TieuDe3 { get; set; }
        public string sDonViTinh { get; set; }
        public double? fDonViTinh { get; set; }
        public bool isTongHop { get; set; }
        public bool isModified { get; set; }
    }

    public class KH5NDDPrintDataExportModel : KH5NDDExport
    {
        public Guid? IdLoaiCongTrinh { get; set; }
        public Guid? IdLoaiCongTrinhParent { get; set; }
        public string SMaLoaiCongTrinh { get; set; }
        public int? Loai { get; set; }
        public int? LoaiParent { get; set; }
        public string STenDuAn { get; set; }
        public double? FHanMucDauTu { get; set; }
        public double? FVonDaGiao { get; set; }
        public double? FTongVonBoTri { get; set; }
        public double? FGiaTriKeHoach { get; set; }
        public double? FVonBoTri { get; set; }
        public double? sTongSo { get; set; }
        public string GhiChu { get; set; }
        public bool IsHangCha { get; set; }
        public string STT { get; set; }
    }

    public class KH5NDDPrintDataChuyenTiepExportModel : KH5NDDExport
    {
        public string STT { get; set; }
        public string IIdMaDonVi { get; set; }
        public string STenDuAn { get; set; }
        public string STienDoThucHien { get; set; }
        public string SSoQuyetDinh { get; set; }
        public DateTime? DNgayQuyetDinh { get; set; }
        public string NgayThangQuyetDinh
        {
            get => (IsHangCha.HasValue && !IsHangCha.Value) ? (string.Format("{0} - {1}", SSoQuyetDinh, (DNgayQuyetDinh.HasValue ? DNgayQuyetDinh.Value.ToString("dd/MM/yyyy") : string.Empty))) : string.Empty;
        }
        public double? TongMucDauTu { get; set; }
        public double? TongMucDauTuNSQP { get; set; }
        public double? ChiPhiDuPhong { get; set; }
        public double? TongSo { get; set; }
        public double? VonBoTriHetNam { get; set; }
        public double? VonDaBoTriNam { get; set; }
        public double? TongMucDauTuPhaiBoTri { get; set; }
        public double? KeHoachVonNamDoDuyet { get; set; }
        public double? KeHoachVonDeNghiBoTriNam { get; set; }
        public double? ChenhLechSoVoiQuyetDinhBo { get; set; }
        public string SGhiChu { get; set; }
        public bool? IsHangCha { get; set; }
    }

    public class VdtKhvKeHoach5NamExportModel
    {
        public Guid? Id { get; set; }
        public Guid? IIdDuAnId { get; set; }
        public string STenDuAn { get; set; }
        public string SMaDuAn { get; set; }
        public int IGiaiDoanTu { get; set; }
        public int IGiaiDoanDen { get; set; }
        public string SSoKeHoach { get; set; }
        public string SDiaDiem { get; set; }
        public string ThoiGianThucHien { get; set; }
        public string SGhiChu { get; set; }
        public Guid? IIdLoaiCongTrinhId { get; set; }
        public string STenLoaiCongTrinh { get; set; }
        public string IIdMaDonVi { get; set; }
        public string STenDonVi { get; set; }
        public int? IIdNguonVonId { get; set; }
        public string STenNguonVon { get; set; }
        public double? FHanMucDauTu { get; set; }
        public double? FTongSoNhuCauNSQP { get; set; }
        public double? FVonDaGiao { get; set; }
        public double? FVonBoTriTuNamDenNam { get; set; }
        public double? FGiaTriSau5Nam { get; set; }
        public Guid? IIdParentId { get; set; }
        public Guid? IdChungTu { get; set; }
        public Guid? IdChungTuParent { get; set; }
        public Guid? IIdDonViId { get; set; }
        public bool? BActive { get; set; }
        public bool? IsGoc { get; set; }
        public int? ILoaiChungTu { get; set; }
        public int? IGiaiDoanTuCT { get; set; }
        public int? IGiaiDoanDenCT { get; set; }
        public int INamLamViec { get; set; }
        public Guid? IIdDonViThucHienDuAn { get; set; }
        public string IIdMaDonViThucHienDuAn { get; set; }
    }
}
