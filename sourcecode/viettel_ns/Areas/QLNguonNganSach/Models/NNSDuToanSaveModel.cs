using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Viettel.Domain.DomainModel;

namespace VIETTEL.Areas.QLNguonNganSach.Models
{
    public class NNSDuToanSaveModel : NNS_DuToan
    {
        public string ListChungTuDotGom { get; set; }
        public string ListChungTu { get; set; }
    }
}