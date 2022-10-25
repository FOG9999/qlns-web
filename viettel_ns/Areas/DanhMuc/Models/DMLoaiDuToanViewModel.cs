using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Viettel.Domain.DomainModel;
using VIETTEL.Models;

namespace VIETTEL.Areas.DanhMuc.Models
{
    public class DMLoaiDuToanViewModel
    {
        public SheetViewModel Sheet { get; set; }

        public string Id { get; set; }
        public DM_LoaiDuToan Entity { get; set; }
        public int Filter { get; set; }

        public Dictionary<int, string> FilterOptions { get; set; }
    }
}