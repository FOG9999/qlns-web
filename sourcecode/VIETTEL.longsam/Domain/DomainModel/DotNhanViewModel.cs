using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Extensions;

namespace Viettel.Domain.DomainModel
{
    public class DotNhanPagingViewModel
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<DotNhanViewModel> Items { get; set; }
       
    }

    public class DotNhanViewModel : NNS_DotNhan
    {
        public decimal SoTien { get; set; }
    }
}
