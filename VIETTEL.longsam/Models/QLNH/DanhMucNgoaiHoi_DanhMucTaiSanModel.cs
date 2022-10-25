using System.Collections.Generic;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.QLNH
{
    public class  DanhMucNgoaiHoi_DanhMucTaiSanModelPaging
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<DanhmucNgoaiHoi_TaiSanModel> Items { get; set; }
    }
    public class DanhmucNgoaiHoi_TaiSanModel : NH_DM_LoaiTaiSan
    {

    }
}
