using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Viettel.Domain.DomainModel;
using VIETTEL.Models;

namespace VIETTEL.Areas.QLNguonNganSach.Models
{
    public class NNSDuToanNhiemVuViewModel
    {
        public SheetViewModel Sheet { get; set; }
        public string IdDuToan { get; set; }
        //public string Id { get; set; }
        public NNS_DuToan_NhiemVu Entity { get; set; }
        public int Filter { get; set; }

        public Dictionary<int, string> FilterOptions { get; set; }
    }
}