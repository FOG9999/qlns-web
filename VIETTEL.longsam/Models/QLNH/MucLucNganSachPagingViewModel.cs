using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.QLNH
{
    public class MucLucNganSachPagingViewModel
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<MucLucNganSachViewModel> Items { get; set; }
    }
    public class MucLucNganSachViewModel : NS_MucLucNganSach
    {
        public decimal SoTien { get; set; }
    }
}
