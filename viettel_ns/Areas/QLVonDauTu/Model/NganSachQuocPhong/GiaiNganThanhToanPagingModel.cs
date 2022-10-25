using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;
using Viettel.Models.QLVonDauTu;

namespace VIETTEL.Areas.QLVonDauTu.Model.NganSachQuocPhong
{
    public class GiaiNganThanhToanPagingModel
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<GiaiNganThanhToanViewModel> lstData { get; set; }
    }
}