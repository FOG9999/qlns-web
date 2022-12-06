using System.Collections.Generic;
using System.Web.Mvc;
using VIETTEL.Models;

namespace VIETTEL.Areas.QuyetToan.Models
{
    public class ChungTuChiTietViewModel
    {
        public SheetViewModel Sheet { get; set; }

        public string Id { get; set; }
        public int Filter { get; set; }

        public Dictionary<int, string> FilterOptions { get; set; }
    }

    public class BaocaoThongTriViewModel
    {
        public string iID_MaChungTu { get; set; }
        public string iID_MaDanhMuc { get; set; }
        public string sGhiChu { get; set; }

        public IEnumerable<SelectListItem> Items { get; set; }

    }
}
