using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.KeHoachChiTietBQP
{
    public class NH_KHChiTietBQPModel : NH_KHChiTietBQP
    {
        public virtual string sNam { get; set; }
        public virtual string sDieuChinhTu { get; set; }
        public virtual Guid? iID_BQuanLyID { get; set; }
        public virtual Guid? iID_DonViID { get; set; }
        public virtual int? iLoaiTTCP { get; set; }
    }

    public class NH_KHChiTietBQPViewModel 
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE, CurrentPage = 1 };
        public List<NH_KHChiTietBQPModel> Items { get; set; }
    }

    public class NH_KHChiTietBQPFilter
    {
        public virtual int? GiaiDoanTu { get; set; }
        public virtual int? GiaiDoanDen { get; set; }
        public virtual string SoKeHoach { get; set; }
        public virtual DateTime? NgayBanHanh { get; set; }
    }

    public class NH_KHChiTietBQP_NVCViewModel
    {
        public virtual Guid ID { get; set; }
        public virtual int? iGiaiDoanTu { get; set; }
        public virtual int? iGiaiDoanDen { get; set; }
        public virtual int? iNamKeHoach { get; set; }
        public virtual Guid? iID_TiGiaID { get; set; }
        public virtual string sSoKeHoachBQP { get; set; }
		public virtual DateTime? dNgayKeHoachBQP { get; set; }
        public virtual string sSoKeHoachTTCP { get; set; }
        public virtual DateTime? dNgayKeHoachTTCP { get; set; }
        public virtual int? iLoai { get; set; }
        public virtual List<NH_KHChiTietBQP_NVCModel> Items { get; set; }
        public virtual bool IsEdit { get; set; }
        public virtual string State { get; set; }
    }

    public class NH_KHChiTietBQP_NVCModel
    {
        public virtual Guid? ID { get; set; }                               // ID nhiệm vụ chi (BQP + TTCP)
        public virtual string sTenNhiemVuChi { get; set; }                  // Tên nhiệm vụ chi (BQP + TTCP)
        public virtual Guid? iID_BQuanLyID { get; set; }
        public virtual string sTenPhongBan { get; set; }
        public virtual Guid? iID_DonViID { get; set; }
        public virtual string sTenDonVi { get; set; }
        public virtual string iID_MaDonVi { get; set; }
        public virtual float? fGiaTriTTCP_USD { get; set; }
        public virtual float? fGiaTriBQP_USD { get; set; }
        public virtual float? fGiaTriBQP_VND { get; set; }
        public virtual string sMaThuTu { get; set; }
        public virtual Guid? iID_KHTTTTCP_NhiemVuChiID { get; set; }
        public virtual Guid? iID_ParentID { get; set; }
        public virtual bool bIsTTCP { get; set; }
        public virtual bool bIsAction { get; set; }
        public virtual bool bIsHasChild { get; set; }
    }

    public class NH_KHChiTietBQP_NhiemVuChiCreateDto
    {
        public virtual Guid ID { get; set; }
        public virtual string sTenNhiemVuChi { get; set; }
        public virtual Guid? iID_BQuanLyID { get; set; }
        public virtual string iID_MaDonVi { get; set; }
        public virtual Guid? iID_DonViID { get; set; }
        public virtual string fGiaTriUSD { get; set; }
        public virtual string fGiaTriVND { get; set; }
        public virtual string sMaThuTu { get; set; }
        public virtual Guid? iID_KHTTTTCP_NhiemVuChiID { get; set; }
        public virtual Guid? iID_ParentID { get; set; }
        public virtual bool bIsTTCP { get; set; }
    }

    public class NH_DM_TiGia_ChiTiet_ViewModel
    {
        public virtual Guid? iID_TiGiaID { get; set; }
        public virtual Guid? iID_TiGiaChiTietID { get; set; }
        public virtual string sMaTienTeGoc { get; set; }
        public virtual string sMaTienTeQuyDoi { get; set; }
        public virtual float fTiGia { get; set; }
    }

    public class CalcTiGiaModel
    {
        public virtual string sMoney { get; set; }
        public virtual decimal dResult { get; set; }
        public virtual string sMaThuTu { get; set; }
        public virtual int iLevel { get; set; }
        public virtual int iGroup { get; set; }
        public virtual int iIndexRow { get; set; }
    }

    public class LookupKHBQP { 
        public virtual Guid Id { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual int? iGiaiDoanTu { get; set; }
        public virtual int? iGiaiDoanDen { get; set; }
        public virtual int? iNamKeHoach { get; set; }
        public virtual int iLoai { get; set; }
    }
}
