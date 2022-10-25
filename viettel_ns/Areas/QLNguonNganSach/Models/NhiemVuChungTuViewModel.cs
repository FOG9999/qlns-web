using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLNguonNganSach;

namespace VIETTEL.Areas.QLNguonNganSach.Models
{
    public class NhiemVuChungTuViewModel
    {
        public List<NNS_DuToan_NhiemVu_ChungTuViewModel> Items { get; set; }
    }
}