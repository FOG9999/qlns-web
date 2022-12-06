using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.QLVonDauTu
{
    public class DanhMucDonViThucHienDuAnViewModel
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<VDT_DM_DonViThucHienDuAn_ViewModel> Items { get; set; }
    }

    public class VDT_DM_DonViThucHienDuAn_ViewModel : VDT_DM_DonViThucHienDuAn
    {
        public string sTenDonViCha { get; set; }
    }
}
