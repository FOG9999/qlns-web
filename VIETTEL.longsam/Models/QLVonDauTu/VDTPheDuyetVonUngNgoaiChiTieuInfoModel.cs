using System;

namespace Viettel.Models.QLVonDauTu
{
    public class VDTPheDuyetVonUngNgoaiChiTieuInfoModel
	{
		public Guid iID_DeNghiThanhToanID { get; set; }
		public string sSoDeNghi { get; set; }
		public DateTime? dNgayDeNghi { get; set; }
		public Guid iID_DonViQuanLyID { get; set; }
		public string sTenDonViQL { get; set; }
		public double? fGiaTriThanhToan { get; set; }
		public double? fGiaTriThuHoiUngNgoaiChiTieu { get; set; }
	}
}