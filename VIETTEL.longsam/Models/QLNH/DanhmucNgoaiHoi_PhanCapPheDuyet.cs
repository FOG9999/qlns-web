using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.QLNH
{
    public class DanhmucNgoaiHoi_PhanCapPheDuyetModelPaging
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<DanhmucNgoaiHoi_PhanCapPheDuyetModel> Items { get; set; }
    }

    public class DanhmucNgoaiHoi_PhanCapPheDuyetModel : NH_DM_PhanCapPheDuyet
    {
    }
    public partial class NH_DM_PhanCapPheDuyetReturnData
    {
        public virtual NH_DM_PhanCapPheDuyet PhanCapPheDuyetData { get; set; }
        public virtual bool IsReturn { get; set; }
        public virtual string errorMess { get; set; }


    }
}
