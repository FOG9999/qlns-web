using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.QLVonDauTu
{
    public class KeHoachPhanBoVonDonViPheDuyetViewModel
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<VDTKHVPhanBoVonDonViPheDuyetViewModel> Items { get; set; }
    }

    public class VDTKHVPhanBoVonDonViPheDuyetViewModel : VDT_KHV_PhanBoVon_DonVi_PheDuyet
    {
        public string sTenDonVi { get; set; }
        public string sTenNguonVon { get; set; }
        public string STT { get; set; }
        public string SoLanDieuChinh { get; set; }
        public string DieuChinhTu { get; set; }
    }

    public class VDTKHVPhanBoVonDonViPheDuyetChiTietViewModel
    {
        public Guid iID_PhanBoVon_DonVi_PheDuyet_ChiTiet_ID { get; set; }
        public Guid? iID_PhanBoVon_DonVi_PheDuyet_ID { get; set; }
        public Guid? iID_DuAnID { get; set; }
        public Guid? iID_TienTeID { get; set; }
        public Guid? iID_DonViTienTeID { get; set; }
        public double? fTiGiaDonVi { get; set; }
        public double? fTiGia { get; set; }
        public string sTenDuAn { get; set; }
        public string sLoaiDuAn { get; set; }
        public string sMaDuAn { get; set; }
        public string sTenDonViThucHienDuAn { get; set; }
        public Guid? iID_LoaiCongTrinh { get; set; }
        public string sTenLoaiCongTrinh { get; set; }
        public int? iLoaiDuAn { get; set; }
        public string sTrangThaiDuAnDangKy { get; set; }
        public string sGhiChu { get; set; }
        public double? fGiaTriPhanBo { get; set; }
        public double? fGiaTriThuHoi { get; set; }
        public Guid? iID_Parent { get; set; }
        public bool bActive { get; set; }
        public int? Loai { get; set; }
        public string STT { get; set; }
    }

    public class VDTKHVPhanBoVonDonViPheDuyetExport
    {
        public IEnumerable<NS_DonVi> lstDonViQuanLy { get; set; }
        public Guid? iID_KeHoachVonNam_DuocDuyet_ChiTietID { get; set; }
        public Guid? iID_LoaiCongTrinh { get; set; }
        public string txtHeader1 { get; set; }
        public string txtHeader2 { get; set; }
        public string iNamLamViec { get; set; }
        public string sDonViTinh { get; set; }
        public string sValueDonViTinh { get; set; }
        public string sLoaiChungTu { get; set; }
        public string sLoaiCongTrinh { get; set; }
        public string sNguonVon { get; set; }

        public string STT { get; set; }
        public Guid? IdLoaiCongTrinh { get; set; }
        public Guid? IdLoaiCongTrinhParent { get; set; }
        public string sTenDuAn { get; set; }
        public string sLNS { get; set; }
		public string sL { get; set; }
		public string sK { get; set; }
		public string sM { get; set; }
		public string sTM { get; set; }
		public string sTTM { get; set; }
		public string sNG { get; set; }
        public double? FCapPhatTaiKhoBac { get; set; }
		public double? FCapPhatBangLenhChi { get; set; }
		public double? FGiaTriThuHoiNamTruocKhoBac { get; set; }
		public double? FGiaTriThuHoiNamTruocLenhChi { get; set; }
		public double? TongSo { get; set; }
        public Guid? IIdDuAn { get; set; }
        public int? Loai { get; set; }
        public bool IsHangCha { get; set; }
        public int? LoaiParent { get; set; }
        public int? LoaiCongTrinh { get; set; }
    }

    public class VDTKHVPhanBoVonDonViPheDuyetExportDieuChinh
    {
        public Guid? IdNhomDuAn { get; set; }
        public string STenDuAn { get; set; }
        public string DiaDiemXayDung { get; set; }
        public string DiaDiemMoTaiKhoanDuAn { get; set; }
		public string ChuDauTu {get;set;}
		public string MaSoDuAnDauTu {get;set;}
		public string MaNganhKinhTe { get; set; }
		public string NangLucThietKe { get; set; }
		public string ThoiGianThucHien { get; set; }
		public string SoNgayThangNam { get; set; }
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
        public string STT { get; set; }
    }
}