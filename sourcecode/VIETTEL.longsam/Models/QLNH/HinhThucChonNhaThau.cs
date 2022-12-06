using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.QLNH
{
    public class DanhMucNgoaiHoi_DanhMucHinhThucChonNhaThauModelPaging
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<DanhmucNgoaiHoi_HinhThucChonNhaThauModel> Items { get; set; }
    }
    public class DanhmucNgoaiHoi_HinhThucChonNhaThauModel : NH_DM_HinhThucChonNhaThau
    {

    }
}
