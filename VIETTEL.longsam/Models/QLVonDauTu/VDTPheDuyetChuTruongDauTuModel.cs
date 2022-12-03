using System;
using System.Collections.Generic;
using System.Globalization;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.QLVonDauTu
{
    public class VDTPheDuyetChuTruongDauTuModel
    {
        public VDT_DA_ChuTruongDauTu chuTruongDauTu { get; set; }
        public IEnumerable<VDT_DA_ChuTruongDauTu_ChiPhi> listChuTruongDauTuChiPhi { get; set; }
        public IEnumerable<VDT_DA_ChuTruongDauTu_NguonVon> listChuTruongDauTuNguonVon { get; set; }
        public IEnumerable<VDTDADuAnHangMucModel> ListHangMuc { get; set; }
        public IEnumerable<TL_TaiLieu> listTaiLieu { get; set; }
    }

    public class VDT_DA_ChuTruongDauTuCreateModel : VDT_DA_ChuTruongDauTu
    {
        public IEnumerable<VDTChuTruongDauTuNguonVonModel> ListNguonVon { get; set; }
        public IEnumerable<VDTDADuAnHangMucModel> ListHangMuc { get; set; }
        public IEnumerable<TL_TaiLieu> ListTaiLieu { get; set; }
        public bool isDieuChinh { get; set; }
        //public string sDonViQuanLy { get; set; }
    }

    public class VDTPheDuyetChuTruongDauTuViewModel
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<VDTChuTruongDauTuViewModel> Items { get; set; }
    }

    public class VDTChuTruongDauTuViewModel : VDT_DA_ChuTruongDauTu
    {
        public string sSTT { get; set; }
        public string sDonViQuanLy { get; set; }
        public string sChuDauTu { get; set; }
        public string sTenNhomDuAn { get; set; }
        public string sTenHinhThucQuanLy { get; set; }
        public string sPhanCapDuAn { get; set; }
        public string sTenLoaiCongTrinh { get; set; }
        public string TenDuAn { get; set; }
        public string sMaDuAn { get; set; }
        public int iSoLanDieuChinh { get; set; }
        public IEnumerable<VDTChuTruongDauTuChiPhiModel> listChuTruongDauTuChiPhi { get; set; }
        public IEnumerable<VDTChuTruongDauTuNguonVonModel> listChuTruongDauTuNguonVon { get; set; }
        public IEnumerable<VDTDADuAnHangMucModel> ListHangMuc { get; set; }
        public IEnumerable<TL_TaiLieu> listTaiLieu { get; set; }
        public string sTMDTDuKienPheDuyet {
            get
            {
                return this.fTMDTDuKienPheDuyet.HasValue ? fTMDTDuKienPheDuyet.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : 0.ToString();
            }
        }

        public string sNgayDeNghi
        {
            get
            {
                return dNgayQuyetDinh.HasValue ? dNgayQuyetDinh.Value.ToString("dd/MM/yyyy") : string.Empty;
            }
        }

        public string sRowTitle { get; set; }
    }

    public class VDTChuTruongDauTuChiPhiModel : VDT_DA_ChuTruongDauTu_ChiPhi
    {
        public string sTenChiPhi { get; set; }
    }

    public class VDTChuTruongDauTuNguonVonModel : VDT_DA_ChuTruongDauTu_NguonVon
    {
        public string sTenNguonVon { get; set; }
        public string id { get; set; }
        public bool isDelete { get; set; }
        public double? fGiaTriTruocDieuChinh { get; set; }
    }

    public class DM_LoaiCongTrinhTreeView
    {
        public string id { get; set; }
        public string title { get; set; }
        public bool isSelectable { get; set; }
        //public string data { get; set; }
        public Guid iID_LoaiCongTrinh { get; set; }
        public Guid? iID_Parent { get; set; }
        public List<DM_LoaiCongTrinhTreeView> subs { get; set; }
    }

    public class VDTDADuAnHangMucModel : VDT_DA_DuAn_HangMuc
    {
        public VDTDADuAnHangMucModel()
        {
            sHangMucCha = "";
            sMaHangMuc = "";
            sTenHangMuc = "";
            sTenHangMucCha = "";
        }
        public string sHangMucCha { get; set; }
        public string sTenHangMucCha { get; set; }
        public string smaOrder { get; set; }
        public Guid iID_ChuTruongDauTu_HangMucID { get; set; }
        public Guid iID_ChuTruongDauTuID { get; set; }
        public Guid? iID_HangMucID { get; set; }
        public double? fTienPheDuyet { get; set; }
        public bool isDelete { get; set; }
    }

    public class ChuTruongHangMucModel
    {
        public Guid? iID_DuAn_HangMucID { get; set; }
        public Guid? iID_DuAnID { get; set; }
        public Guid? iID_ParentID { get; set; }
        public string sMaHangMuc { get; set; }
        public string sTenHangMuc { get; set; }
        public string sTenLoaiCongTrinh { get; set; }
        public double? fTienHangMuc { get; set; }
        public string smaOrder { get; set; }
        public Guid? iID_LoaiCongTrinhID { get; set; }
        public Guid iID_ChuTruongDauTu_HangMucID { get; set; }
        public Guid iID_ChuTruongDauTuID { get; set; }
        public bool isDelete { get; set; }
        public Guid? iID_HangMucID { get; set; }
    }

    public class VDTDADSNguonVonTheoIDDuAnModel
    {
        public object iID_DuAnID { get; set; }
        public int iID_NguonVonID { get; set; }
        public string sTen { get; set; }
        public float TongHanMucDauTu { get; set; }

    }
}
