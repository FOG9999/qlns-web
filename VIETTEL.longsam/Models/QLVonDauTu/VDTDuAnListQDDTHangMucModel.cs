using System;

namespace Viettel.Models.QLVonDauTu
{
    public class VDTDuAnListQDDTHangMucModel
	{
		public Guid iID_QDDauTu_HangMuciID { get; set; }
		public Guid? iID_QDDauTuID { get; set; }
		public double? fGiaTriBanDau { get; set; }
		public double? fGiaTriPheDuyet { get; set; }
		public string sTenHangMuc { get; set; }
	}
}