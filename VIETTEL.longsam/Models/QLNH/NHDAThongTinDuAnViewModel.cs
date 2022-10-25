using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;
using Viettel.Models.Shared;

namespace Viettel.Models.QLNH
{
    public class NHDAThongTinDuAnViewModel
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<NHDAThongTinDuAnModel> Items { get; set; }
        public SelectList ListPhongBan { get; set; }
        public List<DM_ChuDauTu> ListChuDauTu { get; set; }
        public SelectList ListDonVi { get; set; }
        public List<NH_DM_PhanCapPheDuyet> ListDanhMucPCPD { get; set; }
        public NHDAThongTinDuAnModel ThongTinDuAnDetail { get; set; }
        public List<NH_DM_TiGia_ChiTiet> ListTyGia { get; set; }
        public List<LookupDto<Guid, string>> ListBQP { get; set; }
        public List<NH_KHChiTietBQP_NhiemVuChi> ListChuongTrinh { get; set; }
        public List<NH_DM_ChiPhi> ListChiPhi { get; set; }
    }
    public class NH_DA_DuAn_ChiPhiDto
    {
        public virtual Guid ID { get; set; }
        public virtual Guid? iID_ChiPhiID { get; set; }
        public virtual Guid? iID_DuAnID { get; set; }
        public virtual string HopDongUSD { get; set; }
        public virtual string HopDongVND { get; set; }
        public virtual string HopDongEUR { get; set; }
        public virtual string HopDongNgoaiTeKhac { get; set; }

    }
    public class NH_DA_DuAn_ChiPhiModel : NH_DA_DuAn_ChiPhi
    {
        public virtual string sTenChiPhi { get; set; }
    }

    public class NH_KHChiTietBQP_NhiemVuChiModel : NH_KHChiTietBQP_NhiemVuChi
    {
        public string sSoKeHoachBQP { get; set; }
    }
    public class DM_ChuDauTuModel : DM_ChuDauTu
    {
        public string sChuDauTu { get; set; }
    }
    public class NS_PhongBanModel : NS_PhongBan
    {
        public string sBQuanLy { get; set; }
    }
    //public class NS_DonViModel : NS_DonVi
    //{
    //    public string sDonVi { get; set; }
    //}
    public class NHDAThongTinDuAnModel : NH_DA_DuAn
    {

        public string sDonVi { get; set; }
        public string sBQuanLy { get; set; }
        public string sChuDauTu { get; set; }
        public string sPhanCapPheDuyet { get; set; }
        public string sSoLanDieuChinh { get; set; }
        public string sDieuChinhTu { get; set; }
        public int depth { get; set; }
        public string dNgayChuTruongDauTuStr
        {
            get
            {
                return dNgayChuTruongDauTu.HasValue ? dNgayChuTruongDauTu.Value.ToString("dd/MM/yyyy") : string.Empty;
            }
        }
        public string dNgayQuyetDinhDauTuStr
        {
            get
            {
                return dNgayQuyetDinhDauTu.HasValue ? dNgayQuyetDinhDauTu.Value.ToString("dd/MM/yyyy") : string.Empty;
            }
        }
        public string sGiaTriUSD { get; set; }
        public string sGiaTriVND { get; set; }
        public string sGiaTriEUR { get; set; }
        public string sGiaTriNgoaiTeKhac { get; set; }
        public string sTenTiGia { get; set; }
        public string sTenMaNgoaiTeKhac { get; set; }
        public string sSoKeHoachBQP { get; set; }
        public string sTenNhiemVuChi { get; set; }
        public Guid iID_BQP { get; set; }

        public string BQP { get; set; }

        public List<TableChiPhi> TableChiPhis { get; set; }
        public Guid? iID_TiGia_ChiTietID { get; set; }
    }
    public class TableChiPhi
    {
        public string fGiaTriUSD { get; set; }
        public string fGiaTriVND { get; set; }
        public string fGiaTriNgoaiTeKhac { get; set; }
        public string fGiaTriEUR { get; set; }
        public string sTenChiPhi { get; set; }
    }

    public class NHDAThongTinDuAnImportModel
    {
        public string STT { get; set; }
        public string sMaDuAn { get; set; }
        public bool IsMaDuAnWrong { get; set; }
        public string sTenDuAn { get; set; }
        public bool IsTenDuAnWrong { get; set; }
        public string sSoChuTruongDauTu { get; set; }
        public bool IsSoChuTruongDauTuWrong { get; set; }
        public string sNgayChuTruongDauTu { get; set; }
        public bool IssNgayChuTruongDauTuWrong { get; set; }
        public string sSoQuetDinhDauTu { get; set; }
        public bool IsSoQuetDinhDauTuWrong { get; set; }
        public string sNgayQuyetDinhDauTu { get; set; }
        public bool IsNgayQuyetDinhDauTuWrong { get; set; }
        public string sSoDuToan { get; set; }
        public bool IsSoDuToanWrong { get; set; }
        public string sNgayDuToan { get; set; }
        public bool IsNgayDuToanWrong { get; set; }
        public string sChuDauTu { get; set; }
        public Guid? iID_ChuDauTu { get; set; }
        public bool IsChuDauTuWrong { get; set; }
        public string sPhanCapPheDuyet { get; set; }
        public Guid? iID_PhanCapPheDuyet { get; set; }
        public bool IsPhanCapPheDuyetWrong { get; set; }
        public string sKhoiCong { get; set; }
        public bool IsKhoiCongWrong { get; set; }
        public string sKetThuc { get; set; }
        public bool IsKetThucWrong { get; set; }
        public string sErrorMessage { get; set; }
        public bool IsDataWrong { get; set; }
    }

}
