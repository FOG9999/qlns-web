using System;
using System.Collections.Generic;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.QLVonDauTu
{
    public class VDTPheDuyetDuAnModel
    {
        public VDTQuyetDinhDauTuViewModel quyetDinhDauTu { get; set; }
        public IEnumerable<VDTQuyetDinhDauTuChiPhiModel> listQuyetDinhDauTuChiPhi { get; set; }
        public IEnumerable<VDTQuyetDinhDauTuNguonVonModel> listQuyetDinhDauTuNguonVon { get; set; }
        public IEnumerable<VDTQuyetDinhDauTuHangMucModel> listQuyetDinhDauTuHangMuc { get; set; }
        public IEnumerable<TL_TaiLieu> listTaiLieu { get; set; }
    }

    public class VDTPheDuyetDuAnCreateModel : VDT_DA_QDDauTu
    {
        public string sTenDuAn { get; set; }
        public string sMaDuAn { get; set; }
        public string sDiaDiem { get; set; }
        public string sSuCanThietDauTu { get; set; }
        public string sMucTieu { get; set; }
        public string sDienTichSuDungDat { get; set; }
        public string sNguonGocSuDungDat { get; set; }
        public string sQuyMo { get; set; }
        public string sKhoiCong { get; set; }
        public string sKetThuc { get; set; }
        public Guid? iID_DonViQuanLyID { get; set; }
        public string sDonViQuanLy { get; set; }
        public Guid? iID_ChuDauTuID { get; set; }
        public string sChuDauTu { get; set; }
        public Guid? iID_NhomDuAnID { get; set; }
        public string sTenNhomDuAn { get; set; }
        public Guid? iID_HinhThucQuanLyID { get; set; }
        public string sTenHinhThucQuanLy { get; set; }
        public double? fTongMucDauTuSauDieuChinh { get; set; }
        public int iSoLanDieuChinh { get; set; }
        public bool isDieuChinh { get; set; }
        public IEnumerable<VDTQuyetDinhDauTuChiPhiCreateModel> ListChiPhi { get; set; }
        public IEnumerable<VDTQuyetDinhDauTuNguonVonModel> ListNguonVon { get; set; }
        public IEnumerable<VDTQuyetDinhDauTuDMHangMucModel> ListHangMuc { get; set; }
        public IEnumerable<TL_TaiLieu> ListTaiLieu { get; set; }
    }

    public class VDTPheDuyetDuAnViewModel
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<VDTQuyetDinhDauTuViewModel> Items { get; set; }
    }

    public class VDTQuyetDinhDauTuViewModel : VDT_DA_QDDauTu
    {
        public string sTenDonVi { get; set; }
        public string iID_MaDonVi { get; set; }
        public Guid? iID_DonViQuanLyID { get; set; }
        public string sTenCDT { get; set; }
        public string sTenHinhThucQuanLy { get; set; }
        public string fTMDTDuKienPheDuyet { get; set; }
        public string sTenNhomDuAn { get; set; }
        public string sTenDuAn { get; set; }
        public string sMaDuAn { get; set; }
        public Guid? iID_NhomDuAnID { get; set; }
        public Guid? iID_ChuDauTuID { get; set; }
        public Guid? iID_DonViThucHienDuAnID { get; set; }
        public string iID_MaDonViThucHienDuAnID { get; set; }
        public string iID_MaCDT { get; set; }
        public int iSoLanDieuChinh { get; set; }
        public List<VDTQuyetDinhDauTuChiPhiCreateModel> ListChiPhi { get; set; }
        public List<VDTQuyetDinhDauTuNguonVonModel> ListNguonVon { get; set; }
        public List<VDTQuyetDinhDauTuDMHangMucModel> ListHangMuc { get; set; }
        public List<TL_TaiLieu> ListTaiLieu { get; set; }
    }

    public class VDTQuyetDinhDauTuChiPhiViewModel
    {
        public VDTQuyetDinhDauTuChiPhiViewModel()
        {
            sTenChiPhiCha = "";
        }
        public Guid iID_DuAn_ChiPhi { get; set; }
        public Guid? iID_ChiPhi_Parent { get; set; }
        public string Id { get; set; }
        public Guid? iID_ChiPhiID { get; set; }
        public string sMaChiPhi { get; set; }
        public string sTenChiPhi { get; set; }
        public double? fTienPheDuyet { get; set; }
        public string sTenChiPhiCha { get; set; }
        public bool IsLoaiChiPhi { get; set; }
    }

    public class VDTQuyetDinhDauTuChiPhiCreateModel
    {
        public Guid iID_DuAn_ChiPhi { get; set; }
        public Guid? iID_ChiPhi_Parent { get; set; }
        public Guid? Id { get; set; }
        public Guid? iID_QDDauTu_ChiPhiID { get; set; }
        public Guid? iID_ChiPhiID { get; set; }
        public string sMaChiPhi { get; set; }
        public string sTenChiPhi { get; set; }
        public double? fTienPheDuyet { get; set; }
        public double? fGiaTriTruocDieuChinh { get; set; }
        public double? fGiaTriDieuChinh { get; set; }
        public string sTenChiPhiCha { get; set; }
        public bool IsLoaiChiPhi { get; set; }
        public bool isDelete { get; set; }
        public int? iThuTu { get; set; }
    }

    public class VDTQuyetDinhDauTuChiPhiModel : VDT_DA_QDDauTu_ChiPhi
    {
        public string sTenChiPhi { get; set; }
        public double? fTruocDieuChinh { get; set; }
        public int? iLoaiDieuChinh { get; set; }
    }

    public class VDTQuyetDinhDauTuNguonVonModel : VDT_DA_QDDauTu_NguonVon
    {
        public Guid? Id { get; set; }
        public string sTenNguonVon { get; set; }
        public double? fGiaTriTruocDieuChinh { get; set; }
        public int? iLoaiDieuChinh { get; set; }
        public bool isDelete { get; set; }
    }

    public class VDTQuyetDinhDauTuDMHangMucModel
    {
        public Guid iID_QDDauTu_DM_HangMucID { get; set; }
        public Guid? iID_DuAnID { get; set; }
        public Guid? iID_ParentID { get; set; }
        public string sMaHangMuc { get; set; }
        public string sTenHangMuc { get; set; }
        public string sTenLoaiCongTrinh { get; set; }
        public string smaOrder { get; set; }
        public Guid? iID_LoaiCongTrinhID { get; set; }
        public Guid iID_QDDauTu_HangMuciID { get; set; }
        public Guid iID_QDDauTuID { get; set; }
        public Guid iID_DuAn_ChiPhi { get; set; }
        public double? fTienPheDuyet { get; set; }
        public bool isDelete { get; set; }
        public Guid iID_HangMucID { get; set; }
        public bool isEdit { get; set; }
        public Guid? iIDParentModifed { get; set; }
        public double? fGiaTriTruocDieuChinh { get; set; }
        public double? fGiaTriDieuChinh { get; set; }
    }

    public class VDTQuyetDinhDauTuHangMucModel : VDT_DA_QDDauTu_HangMuc
    {
        public string sMaHangMuc { get; set; }
        public string sTenHangMuc { get; set; }
        public string sHangMucCha { get; set; }
        public double? fTruocDieuChinh { get; set; }
        public double? fGiaTriDieuChinh { get; set; }
        public int? iLoaiDieuChinh { get; set; }
        public string iID_NguonVonID { get; set; }
    }

    public class VDTQuyetDinhDauTuNguonVonByChuTruongDauTu
    {
        public int? iID_NguonVonID { get; set; }
        public string sTenNguonVon { get; set; }
        public double? fTienPheDuyet { get; set; }
    }

    public class VDTQuyetDinhDauTuHangMucByChuTruongDauTu
    {
        public VDTQuyetDinhDauTuHangMucByChuTruongDauTu()
        {
            sHangMucCha = "";
        }
        public string Id { get; set; }
        public string sMaHangMuc { get; set; }
        public string sTenHangMuc { get; set; }
        public string sHangMucCha { get; set; }
        public double? fTienHangMuc { get; set; }
    }
}
