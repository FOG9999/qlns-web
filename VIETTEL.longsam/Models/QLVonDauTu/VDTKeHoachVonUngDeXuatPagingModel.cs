using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.QLVonDauTu
{
    public class VDTKeHoachVonUngDeXuatPagingModel
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<VDTKeHoachVonUngDeXuatModel> lstData { get; set; }
        public string chungTuTabIndex { get; set; }
        public string chungTuTongHopTabIndex { get; set; }
    }
}
