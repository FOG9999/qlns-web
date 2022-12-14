USE [CTC_DB_TEST]
GO

/****** Object:  StoredProcedure [dbo].[sp_vdt_get_dutoan_by_duanid]    Script Date: 10/27/2022 5:51:01 PM ******/
DROP PROCEDURE [dbo].[sp_vdt_get_dutoan_by_duanid]
GO

/****** Object:  StoredProcedure [dbo].[sp_vdt_get_dutoan_by_duanid]    Script Date: 10/27/2022 5:51:01 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[sp_vdt_get_dutoan_by_duanid]
@iID_DuAnID uniqueidentifier,
@loaiChungTu int
AS
BEGIN
	IF(@loaiChungTu = 1)
	BEGIN
		SELECT tbl.iID_DuToanID, tbl.iID_DuAnID, tbl.sSoQuyetDinh, tbl.dNgayQuyetDinh, tbl.sNoiDung,
				tbl.fTongDuToanPheDuyet, tbl.bIsGoc, tbl.iID_ParentID, tbl.bLaTongDuToan, tbl.iID_DuToanGocID,
				tbl.sUserCreate, tbl.dDateCreate, tbl.sUserUpdate, tbl.dDateUpdate, da.sTenDuAn, dv.sTen as sTenDonVi, dv.iID_Ma as Id_DonVi, 
				(da.sKhoiCong + '-' + da.sKetThuc) as dThoiGianThucHien,
				NULL as fTongMucDauTuSauDieuChinh, NULL as iSoLanDieuChinh, NULL as sDiaDiem
		FROM VDT_DA_DuToan as tbl
		INNER JOIN VDT_DA_DuAn as da on tbl.iID_DuAnID = da.iID_DuAnID
		INNER JOIN NS_DonVi as dv on da.iID_DonViQuanLyID = dv.iID_Ma
		WHERE tbl.iID_DuAnID = @iID_DuAnID AND tbl.bActive = 1
	END
	ELSE
	IF(@loaiChungTu = 2)
	BEGIN
		SELECT tbl.iID_QDDauTuID as iID_DuToanID, tbl.iID_DuAnID, tbl.sSoQuyetDinh, tbl.dNgayQuyetDinh, null as sNoiDung,
					tbl.fTongMucDauTuPheDuyet as fTongDuToanPheDuyet, tbl.bIsGoc, tbl.iID_ParentID, 0 as bLaTongDuToan, null as iID_DuToanGocID,
					tbl.sUserCreate, tbl.dDateCreate, tbl.sUserUpdate, tbl.dDateUpdate, da.sTenDuAn, dv.sTen as sTenDonVi, dv.iID_Ma as Id_DonVi, 
					(da.sKhoiCong + '-' + da.sKetThuc) as dThoiGianThucHien,
					NULL as fTongMucDauTuSauDieuChinh, NULL as iSoLanDieuChinh, NULL as sDiaDiem
			FROM VDT_DA_QDDauTu as tbl
			INNER JOIN VDT_DA_DuAn as da on tbl.iID_DuAnID = da.iID_DuAnID
			INNER JOIN NS_DonVi as dv on da.iID_DonViQuanLyID = dv.iID_Ma
			WHERE tbl.iID_DuAnID = @iID_DuAnID AND tbl.bActive = 1
	END
	ELSE
		SELECT tbl.iID_ChuTruongDauTuID as iID_DuToanID, tbl.iID_DuAnID, tbl.sSoQuyetDinh, tbl.dNgayQuyetDinh, null as sNoiDung,
					tbl.fTMDTDuKienPheDuyet as fTongMucDauTuPheDuyet, tbl.bIsGoc, tbl.iID_ParentID, 0 as bLaTongDuToan, null as iID_DuToanGocID,
					tbl.sUserCreate, tbl.dDateCreate, tbl.sUserUpdate, tbl.dDateUpdate, da.sTenDuAn, dv.sTen as sTenDonVi, dv.iID_Ma as Id_DonVi, 
					(da.sKhoiCong + '-' + da.sKetThuc) as dThoiGianThucHien,
					NULL as fTongMucDauTuSauDieuChinh, NULL as iSoLanDieuChinh, NULL as sDiaDiem
			FROM VDT_DA_ChuTruongDauTu as tbl
			INNER JOIN VDT_DA_DuAn as da on tbl.iID_DuAnID = da.iID_DuAnID
			INNER JOIN NS_DonVi as dv on da.iID_DonViQuanLyID = dv.iID_Ma
			WHERE tbl.iID_DuAnID = @iID_DuAnID AND tbl.bActive = 1
END
GO

