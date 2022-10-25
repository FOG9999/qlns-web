using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Viettel.Domain.DomainModel;
using VIETTEL.Models;

namespace VIETTEL.Areas.QLNguonNganSach.Models
{
    public class QLPhanNguonNDChiViewModel
    {
        public SheetViewModel Sheet { get; set; }

        public int Filter { get; set; }

        public Dictionary<int, string> FilterOptions { get; set; }

        public IEnumerable<NNSDMNoiDungChiViewModel> lstDMNoiDungChi { get; set; }
        public decimal? rSoTienBTCCap { get; set; }
        public decimal? rSoTienConLai { get; set; }
        public decimal? rSoTienCoTheChi { get; set; }
        public Guid iID_Nguon { get; set; }
        public Guid iIdPhanNguon { get; set; }
        public string sNoiDung { get; set; }
    }
}