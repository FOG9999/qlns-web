
ALTER PROC [dbo].[sp_vdt_get_quyettoanniendovonung_by_parentid]
@iIdQuyetToanId uniqueidentifier
AS
BEGIN
	DECLARE @iNamKeHoach int
	SELECT @iNamKeHoach = iNamKeHoach FROM VDT_QT_BCQuyetToanNienDo WHERE iID_BCQuyetToanNienDoID = @iIdQuyetToanId

	SELECT DISTINCT iID_DuAnID, CAST(0 as bit) BIsChuyenTiep INTO #tmp
	FROM VDT_QT_BCQuyetToanNienDo_ChiTiet_01
	WHERE iID_BCQuyetToanNienDo = @iIdQuyetToanId

	UPDATE tmp
	SET
		BIsChuyenTiep = 1
	FROM #tmp as tmp
	INNER JOIN (
		SELECT DISTINCT dt.iID_DuAnID 
		FROM VDT_KHV_PhanBoVon as tbl 
		INNER JOIN VDT_KHV_PhanBoVon_ChiTiet as dt on tbl.iID_PhanBoVonID = dt.iID_PhanBoVonID 
		WHERE tbl.bActive = 1 AND tbl.iNamKeHoach = (@iNamKeHoach - 1)
		) as mp on tmp.iID_DuAnID = mp.iID_DuAnID

	SELECT
		--tbl.iID_DuAnID as iID_DuAnID,		
		da.sTenDuAn,
		da.sDiaDiem,
		da.sMaDuAn,	
		BIsChuyenTiep,
		tbl.iID_LoaiCongTrinh,
		lct.SMaLoaiCongTrinh,
		lct.STenLoaiCongTrinh,
		ISNULL(tbl.FKHUngTrcChuaThuHoiTrcNamQuyetToan, 0) as FUngTruocChuaThuHoiNamTruoc, --1 
		ISNULL(tbl.fLKThanhToanDenTrcNamQuyetToan_KHUng, 0) as FLuyKeThanhToanNamTruoc,  --2
		ISNULL(tbl.fThanhToan_KHUngNamTrcChuyenSang, 0) as FThanhToanKLHTNamTruocChuyenSang, CAST(0 as float) as FThanhToanUngNamTruocChuyenSang, CAST(0 as float) as FThuHoiTamUngNamNayVonNamTruoc, CAST(0 as float) as FThuHoiTamUngNamTruocVonNamTruoc, --4
		ISNULL(tbl.FThuHoiUngTruoc, 0) as fThuHoiVonNamNay, --5 
		ISNULL(tbl.FGiaTriThuHoiTheoGiaiNganThucTe, 0) as fGiaTriThuHoiTheoGiaiNganThucTe, --6
		ISNULL(tbl.FKHUngNamNay, 0) as fKHVUNamNay, --7 
		ISNULL(tbl.fThanhToan_KHUngNamNay, 0) as FThanhToanKLHTTamUngNamNay, CAST(0 as float) as FThanhToanUngNamNay, CAST(0 as float) as FThuHoiTamUngNamNay, CAST(0 as float) as FThuHoiTamUngNamTruoc --8
	FROM VDT_QT_BCQuyetToanNienDo_ChiTiet_01 as tbl
	INNER JOIN VDT_DA_DuAn as da on tbl.iID_DuAnID = da.iID_DuAnID
	INNER JOIN #tmp as tmp on tbl.iID_DuAnID = tmp.iID_DuAnID
	LEFT JOIN VDT_DM_LoaiCongTrinh as lct on tbl.iID_LoaiCongTrinh = lct.iID_LoaiCongTrinh
	WHERE iID_BCQuyetToanNienDo = @iIdQuyetToanId

	DROP TABLE #tmp
END
;