using System.Collections.Generic;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.QLNH
{
    public class DanhmucNgoaiHoi_LoaiHopDongModelPaging
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<DanhmucNgoaiHoi_LoaiHopDongModel> Items { get; set; }
    }
    public class DanhmucNgoaiHoi_LoaiHopDongModel : NH_DM_LoaiHopDong
    {
    }
}
