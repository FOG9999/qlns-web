using System.Collections.Generic;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.QLVonDauTu
{
    public class VDTPheDuyetVonUngNCTPagingModel
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<VDTPheDuyetVonUngNgoaiChiTieuInfoModel> lstData { get; set; }
    }
}