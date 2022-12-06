using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Viettel.Domain.DomainModel;
using VIETTEL.Models;

namespace VIETTEL.Areas.DanhMuc.Models
{
    public class DMNguonViewModel
    {
        public SheetViewModel Sheet { get; set; }

        public int Filter { get; set; }

        public Dictionary<int, string> FilterOptions { get; set; }

        public IEnumerable<DM_Nguon> lstDMNguon { get; set; }

        public string iID_Nguon { get; set; }

        public string sMaNguon { get; set; }

        public string STT { get; set; }
        public bool bDelete { get; set; }
    }
}