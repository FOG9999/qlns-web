using System;
using System.Collections.Generic;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.QLVonDauTu
{
    public class VDTDuAnListCTDTNguonVonModel
	{
		public Guid iID_ChuTruongDauTu_NguonVonID { get; set; }
		public Guid iID_ChuTruongDauTuID { get; set; }
		public int iID_NguonVonID { get; set; }
		public double? fTienToTrinh { get; set; }
		public double? fTienThamDinh { get; set; }
		public double? fTienPheDuyet { get; set; }
		public Guid? iID_TienTeID { get; set; }
		public Guid? iID_DonViTienTeID { get; set; }
		public double? fTiGiaDonVi { get; set; }
		public double? fTiGia { get; set; }
		public string sTenNguonGocVon { get; set; }
	}
}