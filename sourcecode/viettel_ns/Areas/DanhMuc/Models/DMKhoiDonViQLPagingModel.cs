using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Viettel.Domain.DomainModel;
using VIETTEL.Common;

namespace VIETTEL.Areas.DanhMuc.Models
{
    public class DMKhoiDonViQLPagingModel
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<DM_KhoiDonViQuanLy> lstData { get; set; }

    }
}