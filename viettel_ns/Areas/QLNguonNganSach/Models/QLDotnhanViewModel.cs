using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VIETTEL.Models;

namespace VIETTEL.Areas.QLNguonNganSach.Models
{
    public class QLDotnhanViewModel
    {
        public SheetViewModel Sheet { get; set; }

        public string iID_DotNhan { get; set; }
        public int Filter { get; set; }

        public Dictionary<int, string> FilterOptions { get; set; }
    }
}