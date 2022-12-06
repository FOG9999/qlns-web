using System;

namespace Viettel.Models.QLVonDauTu
{
    public class VDTDuAnListDuToanChiPhiModel
    {
		public Guid iID_DuToan_ChiPhiID { get; set; }
		public Guid? iID_DuToanID { get; set; }
		public double? fGiaTriBanDau { get; set; }
		public double? fGiaTriPheDuyet { get; set; }
		public string sTenChiPhi { get; set; }
	}
}