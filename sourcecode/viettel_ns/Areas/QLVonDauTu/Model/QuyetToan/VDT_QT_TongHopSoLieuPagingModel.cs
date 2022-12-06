using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLVonDauTu;
using VIETTEL.Common;

namespace VIETTEL.Areas.QLVonDauTu.Model.QuyetToan
{
    public class VDT_QT_TongHopSoLieuPagingModel
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<VDT_QT_TongHopSoLieuViewModel> lstData { get; set; }
    }
}