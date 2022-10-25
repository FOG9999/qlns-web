using System;

namespace Viettel.Models.QLVonDauTu
{
    public class VDTDuAnListDuToanNguonVonModel
	{
		public Guid iID_DuToan_NguonVonID { get; set; }
		public Guid? iID_DuToanID { get; set; }
		public double? fGiaTriBanDau { get; set; }
		public double? fGiaTriPheDuyet { get; set; }
		public string sTenNguonGocVon { get; set; }
	}
}