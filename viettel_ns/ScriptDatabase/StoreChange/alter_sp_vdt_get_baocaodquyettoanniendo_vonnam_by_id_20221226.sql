
ALTER PROC [dbo].[sp_vdt_get_baocaodquyettoanniendo_vonnam_by_id]
@iIdBCQuyetToanNienDo uniqueidentifier
AS
BEGIN
	DECLARE @iNamKeHoach int
	SELECT @iNamKeHoach = iNamKeHoach FROM VDT_QT_BCQuyetToanNienDo WHERE iID_BCQuyetToanNienDoID = @iIdBCQuyetToanNienDo

	SELECT DISTINCT iID_DuAnID, CAST(0 as bit) BIsChuyenTiep INTO #tmpDuAn
	FROM VDT_QT_BCQuyetToanNienDo_ChiTiet_01
	WHERE iID_BCQuyetToanNienDo = @iIdBCQuyetToanNienDo

	-- Tong muc dau tu
	SELECT tmp.iID_DuAnID, SUM(ISNULL(qd.fTongMucDauTuPheDuyet, 0)) as fTongMucDauTu INTO #tmpTongMucDauTu
	FROM #tmpDuAn as tmp
	INNER JOIN VDT_DA_QDDauTu as qd on tmp.iID_DuAnID = qd.iID_DuAnID
	WHERE qd.BActive = 1
	GROUP BY tmp.iID_DuAnID

	UPDATE tmp
	SET
		BIsChuyenTiep = 1
	FROM #tmpDuAn as tmp
	INNER JOIN (
		SELECT DISTINCT dt.iID_DuAnID 
		FROM VDT_KHV_PhanBoVon as tbl 
		INNER JOIN VDT_KHV_PhanBoVon_ChiTiet as dt on tbl.iID_PhanBoVonID = dt.iID_PhanBoVonID 
		WHERE tbl.bActive = 1 AND tbl.iNamKeHoach = (@iNamKeHoach - 1)
		) as mp on tmp.iID_DuAnID = mp.iID_DuAnID

	SELECT
		tbl.iID_DuAnID,
		da.sMaDuAn,
		da.sDiaDiem,
		BIsChuyenTiep,
		da.sTenDuAn,
		tbl.iID_LoaiCongTrinh,
		lct.SMaLoaiCongTrinh,
		lct.STenLoaiCongTrinh,
		ISNULL(tmdt.fTongMucDauTu, 0) as fTongMucDauTu, 
		ISNULL(tbl.FLKThanhToanDenTrcNamQuyetToan, 0) as fLuyKeThanhToanNamTruoc, --6
		ISNULL(tbl.FTamUngChuaThuHoiTrcNamQuyetToan, 0) as fTamUngTheoCheDoChuaThuHoiNamTruoc, --7
		ISNULL(tbl.FGiaTriTamUngDieuChinhGiam, 0) as fGiaTriTamUngDieuChinhGiam, --8
		ISNULL(tbl.FThuHoiUngNamTrc, 0) as fTamUngNamTruocThuHoiNamNay, --9
		ISNULL(tbl.FChiTieuNamTrcChuyenSang, 0) as fKHVNamTruocChuyenNamNay, --10
		ISNULL(tbl.fThanhToanKLHT_CTNamTrcChuyenSang, 0) as fTongThanhToanSuDungVonNamTruoc, --12
		ISNULL(tbl.FTamUngChuaThuHoi_CTNamTrcChuyenSang, 0) as fTamUngNamNayDungVonNamTruoc, CAST(0 as float) as fThuHoiTamUngNamNayDungVonNamTruoc,  --13
		ISNULL(tbl.FGiaTriNamTruocChuyenNamSau, 0) as fGiaTriNamTruocChuyenNamSau, --14
		ISNULL(tbl.FChiTieuNamNay, 0) as fKHVNamNay, --16
		ISNULL(tbl.FThanhToanKLHT_CTNamNay, 0) as fTongThanhToanSuDungVonNamNay, --18
		ISNULL(tbl.fTamUngChuaThuHoi_CTNamNay, 0) as fTongTamUngNamNay, CAST(0 as float) as fTongThuHoiTamUngNamNay, --19
		ISNULL(tbl.FGiaTriNamNayChuyenNamSau, 0) as fGiaTriNamNayChuyenNamSau --20
	FROM VDT_QT_BCQuyetToanNienDo_ChiTiet_01 as tbl
	INNER JOIN #tmpDuAn as tmp on tbl.iID_DuAnID = tmp.iID_DuAnID
	INNER JOIN VDT_DA_DuAn as da on tbl.iID_DuAnID = da.iID_DuAnID
	LEFT JOIN VDT_DM_LoaiCongTrinh as lct on tbl.iID_LoaiCongTrinh = lct.iID_LoaiCongTrinh
	LEFT JOIN #tmpTongMucDauTu as tmdt on tbl.iID_DuAnID = tmdt.iID_DuAnID
	WHERE iID_BCQuyetToanNienDo = @iIdBCQuyetToanNienDo

	DROP TABLE #tmpDuAn
	DROP TABLE #tmpTongMucDauTu
END
