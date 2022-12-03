using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.QLVonDauTu
{
    public class KeHoach5NamDeXuatViewModel
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<KeHoach5NamDeXuatModel> Items { get; set; }
    }

    public class KeHoach5NamDeXuatModel : VDT_KHV_KeHoach5Nam_DeXuat
    {
        public string sTenDonvVi { get; set; }
        public string SoLanDieuChinh { get; set; }
        public string DieuChinhTu { get; set; }
        public bool isChecked { get; set; }
        public string SVoucherTypes { get; set; }
        public string sLoaiDuAn
        {
            get => iLoai.Equals(2) ? "Chuyển tiếp" : "Khởi công mới";
        }
    }

    public class DuAnKeHoach5NamModel
    {
        public List<DuAnKeHoach5Nam> Items { get; set; }
    }

    public class DuAnKeHoach5Nam
    {
        public Guid IDDuAnID { get; set; }
        public string SMaDuAn { get; set; }
        public string STenDuAn { get; set; }
        public string SDiaDiem { get; set; }
        public int IGiaiDoanTu { get; set; }
        public int IGiaiDoanDen { get; set; }
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

    public class KeHoach5NamDeXuatDataImportModel
    {
        public string sSTT { get; set; }
        public string sTen { get; set; }
        public string sTenDonViQL { get; set; }
        public string sDiaDiem { get; set; }
        public string iGiaiDoanTu { get; set; }
        public string iGiaiDoanDen { get; set; }
        public string sDuAnCha { get; set; }
        public string sTenLoaiCongTrinh { get; set; }
        public string sTenNganSach { get; set; }
        public string fHanMucDauTu { get; set; }
        public string fTongSoNhuCauNSQP { get; set; }
        public string fTongSo { get; set; }
        public string fGiaTriNamThuNhat { get; set; }
        public string fGiaTriNamThuHai { get; set; }
        public string fGiaTriNamThuBa { get; set; }
        public string fGiaTriNamThuTu { get; set; }
        public string fGiaTriNamThuNam { get; set; }
        public string fGiaTriBoTri { get; set; }
        public string sGhiChu { get; set; }
        public string iID_KeHoach5Nam_DeXuat_ChiTietID { get; set; }
        public string iID_ParentID { get; set; }
        public string iID_KeHoach5Nam_DeXuatID { get; set; }
        public string iID_DonViQuanLyID { get; set; }
        public string iID_LoaiCongTrinhID { get; set; }
        public string iID_NguonVonID { get; set; }
        public string numChild { get; set; }
        public string iIDReference { get; set; }
        public string iLevel { get; set; }
        public string sId_CDT { get; set; }
        public string iID_MaDonVi { get; set; }
        public int isMap { get; set; }
        public int bIsParent { get; set; }
        public string sMaOrder { get; set; }
        public string iIndexCode { get; set; }
        public string sMaLoaiCongTrinh { get; set; }
    }

    public class DonViTinhModel
    {
        public string DisplayItem { get; set; }
        public string ValueItem { get; set; }
    }

    public class KH5NDXExport
    {
        public IEnumerable<NS_NguonNganSach> lstDMNguonNganSach { get; set; }
        public IEnumerable<NS_DonVi> lstDonViQuanLy { get; set; }
        public IEnumerable<VDT_DM_DonViThucHienDuAn> lstDonViThucHienDuAn { get; set; }
        public int iGiaiDoanTu { get; set; }
        public int iGiaiDoanDen { get; set; }
        public Guid iID_DonViQuanLyID { get; set; }
        public Guid iID_KeHoach5Nam_DeXuatID { get; set; }
        public string iID_LoaiCongTrinh { get; set; }
        public string txt_TieuDe1 { get; set; }
        public string txt_TieuDe2 { get; set; }
        public string txt_TieuDe3 { get; set; }
        public string sDonViTinh { get; set; }
        public double? fDonViTinh { get; set; }
        public bool isTongHop { get; set; }
        public bool isModified { get; set; }
    }

    public class VdtKhvKeHoach5NamDeXuatChuyenTiepReportModel : KH5NDXExport
    {
        public string STT { get; set; }
        public Guid? IdLoaiCongTrinh { get; set; }
        public Guid? IdLoaiCongTrinhParent { get; set; }
        public string SMaLoaiCongTrinh { get; set; }
        public int? Loai { get; set; }
        public string STenDuAn { get; set; }
        public string STenDonVi { get; set; }
        public string SDiaDiem { get; set; }
        public string SThoiGianThucHien { get; set; }
        public string SSoQuyetDinh { get; set; }
        public DateTime? DNgayQuyetDinh { get; set; }
        public double? FHanMucDauTu { get; set; }
        public string STenNguonVon { get; set; }
        public double? FTongSoNhuCau { get; set; }
        public double? FTongSo { get; set; }
        public double? FGiaTriNamThuNhat { get; set; }
        public double? FGiaTriNamThuHai { get; set; }
        public double? FGiaTriNamThuBa { get; set; }
        public double? FGiaTriNamThuTu { get; set; }
        public double? FGiaTriNamThuNam { get; set; }
        public double? FGiaTriBoTri { get; set; }
        public string SGhiChu { get; set; }
        public bool IsHangCha { get; set; }
        public int? LoaiParent { get; set; }
        public int? IIdNguonVon { get; set; }
        public double? LuyKeVonNSQPDaBoTri { get; set; }
        public double? LuyKeVonNSQPDeNghiBoTri { get; set; }
        public double? TongLuyKe
        {
            get
            {
                return LuyKeVonNSQPDaBoTri + LuyKeVonNSQPDeNghiBoTri;
            }
        }
        public string SSoQuyetDinhNgayQuyetDinh { get; set; }
        public double? FHanMucDauTuQP { get; set; }
        public double? FHanMucDauTuNN { get; set; }
        public double? FHanMucDauTuDP { get; set; }
        public double? FHanMucDauTuOrther { get; set; }
    }

    public class KH5NDXPrintDataDieuChinhExportModel : KH5NDXExport
    {
        public string STT { get; set; }
        public string IdDonViQuanLy { get; set; }
        public string STenDuAn { get; set; }
        public double? FHanMucDauTuDuocDuyet { get; set; }
        public double? FTongSoDuocDuyet { get; set; }
        public double? FVonBoTriTuNamDenNamDuocDuyet { get; set; }
        public double? FVonBoTriSauNamDuocDuyet { get; set; }
        public string STrangThaiThucHien { get; set; }
        public double? FHanMucDauTuDeXuat { get; set; }
        public double? FTongSoDeXuat { get; set; }
        public double? FTongCongDeXuat { get; set; }
        public double? FGiaTriNamThuNhatDeXuat { get; set; }
        public double? FGiaTriNamThuHaiDeXuat { get; set; }
        public double? FGiaTriNamThuBaDeXuat { get; set; }
        public double? FGiaTriNamThuTuDeXuat { get; set; }
        public double? FGiaTriNamThuNamDeXuat { get; set; }
        public double? FGiaTriSauNamDeXuat { get; set; }
        public double? FHanMucDauTuChenhLech { get; set; }
        public double? FTongSoChenhLech { get; set; }
        public double? FVonBoTriTuNamDenNamChenhLech { get; set; }
        public double? FVonBoTriSauNamChenhLech { get; set; }
        public string SGhiChu { get; set; }
        public int? Loai { get; set; }
        public bool IsHangCha { get; set; }
    }

    public class KH5NDXPrintDataExportModel : KH5NDXExport
    {
        //get data report sp_vdt_khv_kehoach_5_nam_de_xuat_export
        public string STT { get; set; }
        public Guid? IdLoaiCongTrinh { get; set; }
        public Guid? IdLoaiCongTrinhParent { get; set; }
        public string SMaLoaiCongTrinh { get; set; }
        public int? Loai { get; set; }
        public string STenDuAn { get; set; }
        public string STenDonVi { get; set; }
        public string SDiaDiem { get; set; }
        public string SThoiGianThucHien { get; set; }
        public string SSoQuyetDinh { get; set; }
        public DateTime? DNgayQuyetDinh { get; set; }
        public double? FHanMucDauTu { get; set; }
        public string STenNguonVon { get; set; }
        public double? FTongSoNhuCau { get; set; }
        public double? FTongSo { get; set; }
        public double? FGiaTriNamThuNhat { get; set; }
        public double? FGiaTriNamThuHai { get; set; }
        public double? FGiaTriNamThuBa { get; set; }
        public double? FGiaTriNamThuTu { get; set; }
        public double? FGiaTriNamThuNam { get; set; }
        public double? FGiaTriBoTri { get; set; }
        public string SGhiChu { get; set; }
        public bool IsHangCha { get; set; }
        public int? LoaiParent { get; set; }
        public int? IIdNguonVon { get; set; }
        public double? LuyKeVonNSQPDaBoTri { get; set; }
        public double? LuyKeVonNSQPDeNghiBoTri { get; set; }
        public double? TongLuyKe { get; set; }
        public string SSoQuyetDinhNgayQuyetDinh { get; set; }
        public double? FHanMucDauTuQP { get; set; }
        public double? FHanMucDauTuNN { get; set; }
        public double? FHanMucDauTuDP { get; set; }
        public double? FHanMucDauTuOrther { get; set; }
        public int? iLevel { get; set; }
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
        public double? fTongVonBoTri { get; set; }
        public string sTenLoaiCongTrinh { get; set; }

    }

    public class ComboboxItem
    {
        public string DisplayItem { get; set; }
        public string ValueItem { get; set; }
    }
    public class FileFtpModel
    {
        public List<FileFtp> Items { get; set; }
    }
    public class FileFtp
    {
        public bool BIsCheck { get; set; }
        public int IStt { get; set; }
        public string SUrl { get; set; }
        public string SNameFile { get; set; }
        public string SAdversion { get; set; }
    }

    public class KH5NDXPrintDataExportNewModel : KH5NDXExport
    {
        public string STT { get; set; }
        public Guid? IdLoaiCongTrinh { get; set; }
        public Guid? IdLoaiCongTrinhParent { get; set; }
        public string SMaLoaiCongTrinh { get; set; }
        public string sTenLoaiCongTrinh { get; set; }
        public int? iLoai { get; set; }
        public int? LoaiParent { get; set; }
        public string STenDuAn { get; set; }
        public string STenDonVi { get; set; }
        public string sSoQuyetDinh { get; set; }
        public DateTime? dNgayQuyetDinh { get; set; }
        public string sTienDo { get; set; }
        public double fNSQP { get; set; }
        public double fChiPhiDuPhong { get; set; }
        public double fVonBoTriHetNam { get; set; }
        public double fSoConLaiPhaiBoTri { get; set; }
        public double fCucTCdexuat { get; set; }
        public double fBoDaGiaoDuToan { get; set; }
        public double fDuKienbotrinamthu2 { get; set; }
        public double fSoConLaiConBotri { get; set; }
        public double fSoTienConLaiChuaDeXuat { get; set; }
        public bool IsHangCha { get; set; }
        public int? IdNguonVon { get; set; }

        public string DNgayQuyetDinh
        {
            get
            {
                return this.dNgayQuyetDinh.HasValue ? this.dNgayQuyetDinh.Value.ToString("dd/MM/yyyy") : string.Empty;
            }
        }
        
        public string sSoQuyetDinhNgayQD
        {
            get
            {
                if (string.IsNullOrEmpty(this.sSoQuyetDinh))
                {
                    return string.Empty;
                }
                else
                {
                    return string.Concat(this.sSoQuyetDinh," , ", this.DNgayQuyetDinh);
                }
            }
        }


    }
}
