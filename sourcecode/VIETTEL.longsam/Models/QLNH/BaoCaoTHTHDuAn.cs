using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.QLNH
{
    public class BaoCaoTHTHDuAnModelPaging
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<NH_TT_ThanhToanViewModel> Items { get; set; } = new List<NH_TT_ThanhToanViewModel>();
    }

    public class NH_TT_ThanhToanViewModel : NH_TT_ThanhToan
    {
        public virtual string SChuDauTu { get; set; }
        public virtual string STenNhaThau { get; set; }
        public virtual string Depth { get; set; }
        public virtual string SLoaiNoiDungChi { get; set; }
        public virtual string SCoQuanThanhToan { get; set; }
        public virtual string SLoaiDeNghi { get; set; }
        public virtual string SfTongDeNghi_USD { get; set; }
        public virtual string SfTongDeNghi_VND { get; set; }
        public virtual string SfTongPheDuyet_USD { get; set; }
        public virtual string SfTongPheDuyet_VND { get; set; }
        public virtual double fGiaTriDuocCap_USD { get; set; }
        public virtual double fGiaTriDuocCap_VND { get; set; }
        public virtual double fGiaTriTTTU_USD { get; set; }
        public virtual double fGiaTriTTTU_VND { get; set; }
    }
    public class NH_DA_DuAnViewModel : NH_DA_DuAn
    {
        public virtual string SChuDauTu { get; set; }
        public virtual string STen { get; set; }
        public virtual string TongThoiGian { get; set; }

    }
    public class BaoCaoTHTHDuAnViewModel
    {
        public virtual NH_DA_DuAnViewModel DuAnModel { get; set; }
        public virtual BaoCaoTHTHDuAnModelPaging ListChiTiet { get; set; }
    }

    public class BaoCaoTHTHDuAnModel
    {
        public virtual IEnumerable<NH_TT_ThanhToanViewModel> Items { get; set; }
        public virtual double Sum { get; set; }
        public virtual double Sumgn { get; set; }
        public virtual int Stt { get; set; }
    }
}
