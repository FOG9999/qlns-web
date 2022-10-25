using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.QLVonDauTu
{
    public class KhoiTaoThongTinDuAnViewModel
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<VDT_KT_KhoiTao_DuLieu_ViewModel> Items { get; set; }
    }

    public class VDT_KT_KhoiTao_DuLieu_ViewModel: VDT_KT_KhoiTao_DuLieu
    {
        public string sTenDonVi { get; set; }
    }
}
