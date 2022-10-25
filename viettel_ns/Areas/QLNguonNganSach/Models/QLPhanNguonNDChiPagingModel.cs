using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Viettel.Domain.DomainModel;
using VIETTEL.Common;

namespace VIETTEL.Areas.QLNguonNganSach.Models
{
    public class QLPhanNguonNDChiPagingModel
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<NNSDMNguonViewModel> lstData { get; set; }
        public Guid? iIdPhanNguon { get; set; }
    }
}