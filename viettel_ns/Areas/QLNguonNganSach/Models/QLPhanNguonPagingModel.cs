using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Viettel.Domain.DomainModel;
using VIETTEL.Common;

namespace VIETTEL.Areas.QLNguonNganSach.Models
{
    public class QLPhanNguonPagingModel
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<NNS_PhanNguon> lstData { get; set; }
    }
}