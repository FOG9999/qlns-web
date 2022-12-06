using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viettel.Domain.DomainModel
{
    public class NNSPhanNguonBTCTheoNDChiViewModel
	{
		public IEnumerable<NNSDMNoiDungChiViewModel> lstDMNoiDungChi { get; set; }
		public decimal? rSoTienBTCCap { get; set; }
		public decimal? rSoTienConLai { get; set; }
		public decimal? rSoTienCoTheChi { get; set; }
		public Guid iID_Nguon { get; set; }
		public Guid iIdPhanNguon { get; set; }
	}
}
