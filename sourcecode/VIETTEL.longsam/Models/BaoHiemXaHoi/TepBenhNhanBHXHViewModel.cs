using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.BaoHiemXaHoi
{
    public class TepBenhNhanBHXHPagingViewModel
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<TepBenhNhanBHXHViewModel> Items { get; set; }
    }

    public class TepBenhNhanBHXHViewModel : BHXH_TepBenhNhan
    {

    }
}
