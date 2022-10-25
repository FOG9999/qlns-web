using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Viettel.Domain.DomainModel;
using VIETTEL.Common;

namespace VIETTEL.Areas.DanhMuc.Models
{
    public class DMNguonPagingModel
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<DanhMucNguonViewModel> lstData { get; set; }
    }
}