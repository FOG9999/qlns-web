using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viettel.Domain.DomainModel
{
    public class NNSDMNguonViewModel : DM_Nguon
    {
		public int alevel { get; set; }
		public Guid iIdPhanNguon { get; set; }
		public decimal? rSoTienBTCCap { get; set; }
		public decimal? rSoTienBTCDaPhan { get; set; }
		public decimal? rSoTienDaChi { get; set; }
		public decimal? rSoTienConLai { get; set; }
		public string location { get; set; }
	}
}
