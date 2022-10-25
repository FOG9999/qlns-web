using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.QLVonDauTu
{
    public class KeHoach5NamModel
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<KeHoach5NamViewModel> Items { get; set; }
    }

    public class KeHoach5NamViewModel: VDT_KHV_KeHoach5Nam
    {
        public string sTenNguonVon { get; set; }
        public string sTenLoaiNganSach { get; set; }
        public string sTenDonvi { get; set; }
        public string sLoai { get; set; }
        public string sKhoan { get; set; }
        public  int soLanDC { get; set; }

        public List<KeHoach5NamChiTietViewModel> listChiTiet { get; set; }
    }
}
