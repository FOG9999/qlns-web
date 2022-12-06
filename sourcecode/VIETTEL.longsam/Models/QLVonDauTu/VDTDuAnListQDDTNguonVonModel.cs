using System;

namespace Viettel.Models.QLVonDauTu
{
    public class VDTDuAnListQDDTNguonVonModel
	{
		public Guid iID_QDDauTu_NguonVonID { get; set; }
		public Guid? iID_QDDauTuID { get; set; }
		public double? fGiaTriBanDau { get; set; }
		public double? fGiaTriPheDuyet { get; set; }
		public string sTenNguonGocVon { get; set; }
	}
}