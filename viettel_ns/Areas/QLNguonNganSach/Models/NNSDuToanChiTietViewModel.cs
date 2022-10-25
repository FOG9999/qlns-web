using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Viettel.Domain.DomainModel;
using VIETTEL.Models;

namespace VIETTEL.Areas.QLNguonNganSach.Models
{
    public class NNSDuToanChiTietViewModel
    {
        public SheetViewModel Sheet { get; set; }
        public string Id_MaChungTu { get; set; }
        public string Id_NhiemVu { get; set; }
        public string Id_DuToan { get; set; }
        public NNS_DuToan Entity { get; set; }
        public int Filter { get; set; }

        public Dictionary<int, string> FilterOptions { get; set; }
    }
}