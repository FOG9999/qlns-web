using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.QLVonDauTu
{
    public class VDTQuanLyTTHopDongViewModel
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<VDT_DA_TT_HopDong_ViewModel> Items { get; set; }
    }

    public class VDT_DA_TT_HopDong_ViewModel : VDT_DA_TT_HopDong
    {
        public string sTenDuAn { get; set; }
        public string sMaDuAn { get; set; }
        public string sTenDonViQL { get; set; }
        public string sDiaDiem { get; set; }
        public string sKhoiCong { get; set; }
        public string sKetThuc { get; set; }
        public double fTongMucDauTu { get; set; }
        public string sTenGoiThau { get; set; }
        public double fGiaTriSauDieuChinh { get; set; }
        public double? fGiaTriDieuChinh { get; set; }
        public int iTongSoDieuChinh { get; set; }
        public string sTenLoaiHopDong { get; set; }
        public string sTenNhaThau { get; set; }
        public double fGiaTriGoiThau { get; set; }
        public string sSuCanThietDauTu { get; set; }
        public string sMucTieu { get; set; }
        public string sDienTichSuDungDat { get; set; }
        public string sNguonGocSuDungDat { get; set; }
        public string sQuyMo { get; set; }
        public string sTenNhomDuAn { get; set; }
        public string sTenHinhThucQuanLy { get; set; }
        public string TenDonVi { get; set; }
        public string ChuDauTu { get; set; }
        public string sTenHopDong { get; set; }
    }
}
