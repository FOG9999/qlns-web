using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.QLVonDauTu
{
    public class KhoiTaoThongTinDuAnChiTietViewModel
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<VDT_KT_KhoiTao_DuLieu_ChiTiet_ViewModel> Items { get; set; }
        public Guid iID_KhoiTaoID { get; set; }
        public string sMaDonVi { get; set; }
    }

    public class VDT_KT_KhoiTao_DuLieu_ChiTiet_ViewModel : VDT_KT_KhoiTao_DuLieu_ChiTiet
    {
        public int STT { get; set; }
        public string sTenDuAn { get; set; }
        public string sMaLoaiCongTrinh { get; set; }
        public List<VDT_KT_KhoiTao_DuLieu_ChiTiet_ThanhToan> LstThanhToan { get; set; }
    }
}
