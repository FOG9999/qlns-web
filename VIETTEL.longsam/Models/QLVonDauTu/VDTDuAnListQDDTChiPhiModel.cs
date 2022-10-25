using System;

namespace Viettel.Models.QLVonDauTu
{
    public class VDTDuAnListQDDTChiPhiModel
    {
		public Guid iID_QDDauTu_ChiPhiID { get; set; }
		public Guid? iID_QDDauTuID { get; set; }
		public double? fGiaTriBanDau { get; set; }
		public double? fGiaTriPheDuyet { get; set; }
		public string sTenChiPhi { get; set; }
	}
}