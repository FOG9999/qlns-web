using System;

namespace Viettel.Models.QLVonDauTu
{
    public class VDTDuAnInfoModel
    {
		public virtual Guid iID_DuAnID { get; set; }
		public virtual string sMaDuAn { get; set; }
		public virtual string sMaKetNoi { get; set; }
		public virtual string sTenDuAn { get; set; }
		public virtual Guid iID_DonViQuanLyID { get; set; }
		public virtual Guid? iID_ChuDauTuID { get; set; }
		public virtual string sMucTieu { get; set; }
		public virtual string sQuyMo { get; set; }
		public virtual string sDiaDiem { get; set; }
		public virtual string sSuCanThietDauTu { get; set; }
		public virtual string sSoTaiKhoan { get; set; }
		public virtual string sDiaDiemMoTaiKhoan { get; set; }
		public virtual string sDienTichSuDungDat { get; set; }
		public virtual string sNguonGocSuDungDat { get; set; }
		public virtual double? fTongMucDauTuDuKien { get; set; }
		public virtual double? fTongMucDauTuThamDinh { get; set; }
		public virtual double? fTongMucDauTu { get; set; }
		public virtual Guid? iID_TienTeID { get; set; }
		public virtual Guid? iID_DonViTienTeID { get; set; }
		public virtual double? fTiGiaDonVi { get; set; }
		public virtual double? fTiGia { get; set; }
		public virtual Guid? iID_NhomDuAnID { get; set; }
		public virtual Guid? iID_NganhDuAnID { get; set; }
		public virtual Guid? iID_LoaiDuAnId { get; set; }
		public virtual Guid? iID_HinhThucDauTuID { get; set; }
		public virtual Guid? iID_HinhThucQuanLyID { get; set; }
		public virtual Guid? iID_NhomQuanLyID { get; set; }
		public virtual Guid? iID_LoaiCongTrinhID { get; set; }
		public virtual Guid? iID_CapPheDuyetID { get; set; }
		public virtual string sKhoiCong { get; set; }
		public virtual string sKetThuc { get; set; }
		public virtual DateTime? dKhoiCongThucTe { get; set; }
		public virtual DateTime? dKetThucThucTe { get; set; }
		public virtual string sTrangThaiDuAn { get; set; }
		public virtual bool? bLaDuAnChinhThuc { get; set; }
		public virtual Guid? iID_ParentID { get; set; }
		public virtual string sCanBoPhuTrach { get; set; }
		public virtual bool? bIsKetThuc { get; set; }
		public virtual string sUserCreate { get; set; }
		public virtual DateTime? dDateCreate { get; set; }
		public virtual string sUserUpdate { get; set; }
		public virtual DateTime? dDateUpdate { get; set; }
		public virtual string sUserDelete { get; set; }
		public virtual DateTime? dDateDelete { get; set; }
		public virtual bool? bIsDeleted { get; set; }
		public virtual bool? bIsCanBoDuyet { get; set; }
		public virtual bool? bIsDuyet { get; set; }
		public virtual bool? bIsDuPhong { get; set; }
		public virtual double? fHanMucDauTu { get; set; }
		//Them truong lay ra
		public virtual string sTenCapPheDuyet { get; set; }
		public virtual string sTenDonVi { get; set; }
		public virtual string sTenLoaiCongTrinh { get; set; }
		public virtual string sTenChuDauTu { get; set; }
		public virtual string sTenHinhThucQuanLy { get; set; }

		public virtual DateTime? dNgayLap { get; set; }
	}
}