using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viettel.Domain.DomainModel
{
    public class NNSPhanNguonNDChiTempViewModel
	{
		public  Guid iID_NoiDungChi { get; set; }
		public  decimal SoTien { get; set; }
		public  string GhiChu { get; set; }
		public  Guid iID_Nguon { get; set; }
		public  Guid iID_PhanNguon { get; set; }
	}
}
