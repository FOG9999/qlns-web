using System.Collections.Generic;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;


namespace Viettel.Models.QLNH
{
    public class DMDonViTienTeViewModel
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<NH_DM_LoaiTienTeModel> Items { get; set; }
    }

    public class NH_DM_LoaiTienTeModel : NH_DM_LoaiTienTe
    {

    }
}
