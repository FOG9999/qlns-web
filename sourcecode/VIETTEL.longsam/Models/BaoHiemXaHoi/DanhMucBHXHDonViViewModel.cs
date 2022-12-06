using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.BaoHiemXaHoi
{
    public class DanhMucBHXHDonViPagingViewModel
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<DanhMucBHXHDonViViewModel> Items { get; set; }

    }
    public class DanhMucBHXHDonViViewModel : BHXH_DonVi
    {
        public string sTenBHXHDonViParent { get; set; }
        public string sTenNSDonViMapping { get; set; }
        public string sLoaiDonVi
        {
            get
            {
                if (bDoanhNghiep == null)
                    return string.Empty;
                else
                    return bDoanhNghiep == true ? "Là đơn vị dự toán" : "Là doanh nghiệp";
            }
        }

        public bool isParent { get; set; }
        public string sMaDonViParent { get; set; }
    }
}
