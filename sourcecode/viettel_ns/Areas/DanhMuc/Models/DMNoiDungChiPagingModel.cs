using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace VIETTEL.Areas.DanhMuc.Models
{
    public class DMNoiDungChiPagingModel
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<DMNoiDungChiViewModel> lstData { get; set; }
    }
}