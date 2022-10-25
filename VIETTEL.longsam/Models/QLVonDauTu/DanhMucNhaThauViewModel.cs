using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.QLVonDauTu
{
    public class DanhMucNhaThauViewModel
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<VDT_DM_NhaThau_ViewModel> Items { get; set; }
    }

    public class VDT_DM_NhaThau_ViewModel : VDT_DM_NhaThau
    {

    }
}
