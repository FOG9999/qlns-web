using System;

namespace Viettel.Models.QLVonDauTu
{
    public class VDTKeHoachVonUngInfoModel
	{
		public virtual Guid iID_KeHoachUngID { get; set; }
		public virtual string sSoQuyetDinh { get; set; }
		public virtual DateTime? dNgayQuyetDinh { get; set; }
		public virtual double? fGiaTriUng { get; set; }
		public virtual Guid iID_Ma { get; set; }
		public virtual string sTenDonViQL { get; set; }
		public virtual string sNguonVon { get; set; }
		public virtual int? iNamKeHoach { get; set; }
		public virtual int? sSolanDieuChinh { get; set; }
		public virtual string sDieuChinhTu{ get; set; }
        public virtual bool bActive { get; set; }
        public virtual bool bIsGoc { get; set; }
    }
}