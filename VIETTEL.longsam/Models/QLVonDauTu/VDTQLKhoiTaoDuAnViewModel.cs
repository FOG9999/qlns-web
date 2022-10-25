using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.QLVonDauTu
{
    public class VDTQLKhoiTaoDuAnPagingModel
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<VDTQLKhoiTaoDuAnViewModel> lstData { get; set; }
    }

    public class VDTQLKhoiTaoDuAnViewModel : VDT_KT_KhoiTao
    {
        public string sTenDuAn { get; set; }
        public string sTenDonVi { get; set; }
        public double? fKHVonHetNamTruoc { get; set; }
        public double? fLuyKeThanhToanKLHT { get; set; }
        public double? fLuyKeThanhToanTamUng { get; set; }
        public double? fTamUngQuaKB { get; set; }
        public double? fThanhToanQuaKB { get; set; }
        public double? fTongMucDauTuPheDuyet { get; set; }
        public List<VDTKTKhoiTaoChiTietModel> lstKTChiTiet { get; set; }
        public List<VDTKTKhoiTaoChiTietNhaThau> lstKTChiTietNhaThau { get; set; }

        // VDT_DA_DuAn
        public string sMaDuAn { get; set; }
        public Guid? iID_ChuDauTuID { get; set; }
        public string sTenChuDauTu { get; set; }
        public Guid? iID_CapPheDuyetID { get; set; }
        public string sTenCapPheDuyet { get; set; }
        public Guid? iID_LoaiCongTrinhID { get; set; }
        public string sTenLoaiCongTrinh { get; set; }
        public Guid? iID_DonViQuanLyID { get; set; }
        public string sTenDonViQuanLy { get; set; }
        public Guid? iID_NhomQuanLyID { get; set; }
        public string sTenNhomQuanLy { get; set; }
        public string sKhoiCong { get; set; }
        public string sKetThuc { get; set; }

        // VDT_DA_QDDauTu
        public string sSoQDDT { get; set; }
        public DateTime? dNgayDuyetQDDT { get; set; }
        public string sCoQuanPheDuyetQDDT { get; set; }
        public string sNguoiKyQDDT { get; set; }

        // VDT_DA_DuToan
        public string sSoTKDT { get; set; }
        public DateTime? dNgayDuyetTKDT { get; set; }
        public string sCoQuanPheDuyetTKDT { get; set; }
        public string sNguoiKyTKDT { get; set; }
    }

    public class VDTKTKhoiTaoChiTietModel : VDT_KT_KhoiTao_ChiTiet
    {
        public string iId
        {
            get
            {
                return iID_KhoiTaoID.ToString();
            }
        }
        // VDT_DA_QDDauTu
        public string sSoQDDT { get; set; }
        public DateTime? dNgayPheDuyetQDDT { get; set; }
        public string sNgayPheDuyetQDDT
        {
            get
            {
                return (dNgayPheDuyetQDDT == null ? "" : dNgayPheDuyetQDDT.Value.ToString("dd/MM/yyyy"));
            }
        }
        public double? fTongMucDauTu { get; set; }

        // VDT_DA_DuToan
        public string sSoQuyetDinhTKDT { get; set; }
        public DateTime? dNgayPheDuyetTKDT { get; set; }
        public string sNgayPheDuyetTKDT
        {
            get
            {
                return (dNgayPheDuyetTKDT == null ? "" : dNgayPheDuyetTKDT.Value.ToString("dd/MM/yyyy"));
            }
        }
        public double? fGiaTriDuToan { get; set; }
        public string sMaDuAn { get; set; }
        public string sTenChuDauTu { get; set; }

        public string sTenNganh { get; set; }
        public string sTenNguonVon { get; set; }
        public string sTenLoaiNguonVon { get; set; }
        public string iID_LoaiNguonVonIDValue { get; set; }
    }

    public class VDTKTKhoiTaoChiTietNhaThau : VDT_KT_KhoiTao_ChiTiet_NhaThau
    {
        public Guid? iID_NganhID { get; set; }
        public int? iID_NguonVonId { get; set; }
        public string tGiaTriUng { get; set; }
    }
}
