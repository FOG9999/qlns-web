using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.QLNH
{
    public class NHDMNoiDungChiViewModel
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<DanhmucNgoaiHoi_NoiDungChiModel> Items { get; set; }
    }
    public class DanhmucNgoaiHoi_NoiDungChiModel : NH_DM_NoiDungChi
    {

    }
}

