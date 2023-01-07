USE [CTC_DB_TEST]
GO

/****** Object:  StoredProcedure [dbo].[sp_vdt_kehoachtrunghan_dexuat_export]    Script Date: 28/12/2022 5:25:53 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER PROC [dbo].[sp_vdt_kehoachtrunghan_dexuat_export]
@iId uniqueidentifier
AS
BEGIN
	SELECT 
		dt.SMaOrder as SMaOrder, 
		dt.iIDReference as IdReference, 
		dt.sSTT as STT, 
		dt.iID_KeHoach5Nam_DeXuat_ChiTietID as IdChiTiet, 
		@iId as IdChungTu,
		dx.iNamLamViec as NamLamViec,
		dx.ILoai as ILoai,
		dx.iID_ParentId as IdParentVoucher,
		dx.bActive as BActive,
		dx.bIsGoc as IsGoc,
		dx.iID_DonViQuanLyID as IIdDonViChungTu,
		dx.sSoQuyetDinh as SSoKeHoach,
		dx.iGiaiDoanTu as IGiaiDoanTu,
		dx.iGiaiDoanDen as IGiaiDoanDen,
		dx.iID_MaDonViQuanLy as MaDonViChungTu,
		dt.iID_DuAnID as IIdDuAn,
		dt.iID_MaDonVi as IdMaDonVi,
		dt.iID_ParentModified as IIdParentModified,
		dt.sTen as STen, 
		SDiaDiem as SDiaDiem, 
		dv.iID_Ma as IIdDonVi, 
		dv.sTen as STenDonVi, 
		CONCAT(CAST(dt.IGiaiDoanTu as nvarchar(10)), '-', CAST(dt.IGiaiDoanDen as nvarchar(10))) as SThoiGianThucHien,
		lct.sTenLoaiCongTrinh as STenLoaiCongTrinh, 
		dt.iID_LoaiCongTrinhID as IIdLoaiCongTrinh, 
		dt.fHanMucDauTu as FHanMucDauTu, 
		dt.iID_NguonVonID as IIdNguonVon, 
		nv.sTen as STenNguonVon,
		dt.fGiaTriNamThuNhat as FGiaTriNamThuNhat, 
		dt.fGiaTriNamThuHai as FGiaTriNamThuHai,
		dt.fGiaTriNamThuBa as FGiaTriNamThuBa,
		dt.fGiaTriNamThuTu as FGiaTriNamThuTu,
		dt.fGiaTriNamThuNam as FGiaTriNamThuNam,
		dt.fGiaTriBoTri as FGiaTriBoTri,
		dt.sGhiChu as SGhiChu, 
		dt.iID_ParentID as IdParent, 
		lct.sMaLoaiCongTrinh as SMaLoaiCongTrinh, 
		dt.bIsParent as IsParent, 
		dt.iLevel as IsStatus, 
		dt.iLevel as Level, 
		dt.iIndexCode as IndexCode,
		dx.sTongHop as STongHop,
		dt.fHanmucNganhDX as fHanmucNganhDX,
		(ISNULL(dt.fVon5namNganhDX,0) + ISNULL(dt.fVonsaunamNganhDX,0)) as fTongVonBoTriNganh,
		dt.fVon5namNganhDX as fVon5namNganhDX,
		dt.fVonsaunamNganhDX as fVonsaunamNganhDX,
		dt.fHanmucCucTCDX as fHanmucCucTCDX,
		(ISNULL(dt.fVon5namCTCDX,0) + ISNULL(dt.fVonsaunamCTCDexuat,0)) as fTongVonBoTriCuc,
		dt.fVon5namCTCDX as fVon5namCTCDX,
		dt.fVonnamthunhatCTC as fVonnamthunhatCTC,
		dt.fVonsaunamCTCDexuat as fVonsaunamCTCDexuat,
		dt.fCucTCDeXuat as fCucTCDeXuat,
		dt.fDuKienBoTriNamThu2 as fDuKienBoTriNamThu2

	FROM VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet as dt
	inner join
		VDT_KHV_KeHoach5Nam_DeXuat dx
	on dx.iID_KeHoach5Nam_DeXuatID = dt.iID_KeHoach5Nam_DeXuatID
	--LEFT JOIN VDT_DM_DonViThucHienDuAn as dv on dt.iID_DonViQuanLyID = dv.iiD_DonVi
	LEFT JOIN NS_DonVi as dv on dt.iID_DonViID = dv.iID_Ma
	LEFT JOIN VDT_DM_LoaiCongTrinh as lct on dt.iID_LoaiCongTrinhID = lct.iID_LoaiCongTrinh
	LEFT JOIN NS_NguonNganSach as nv on dt.iID_NguonVonID = nv.iID_MaNguonNganSach
	WHERE dt.iID_KeHoach5Nam_DeXuatID = @iId order by dt.SMaOrder
END
GO

