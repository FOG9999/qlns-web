using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLVonDauTu;
using VIETTEL.Common;

namespace VIETTEL.Areas.QLVonDauTu.Model.QuyetToan
{
    public class VdtQtBcQuyetToanNienDoPagingModel
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<VdtQtBcQuyetToanNienDoViewModel> lstData { get; set; }
    }
    public class Dropdown_QuyetToanNienDo
    {
        public int? Value { get; set; }
        public string Label { get; set; }
    }
    public class Dropdown_Find
    {
        public virtual int valueId { get; set; }
        public virtual string labelName { get; set; }

    }
}