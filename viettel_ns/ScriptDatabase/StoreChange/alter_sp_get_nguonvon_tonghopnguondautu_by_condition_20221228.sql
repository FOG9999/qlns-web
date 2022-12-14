
ALTER PROCEDURE [dbo].[sp_get_nguonvon_tonghopnguondautu_by_condition]
@data t_tbl_tonghopdautu_2 READONLY
AS
BEGIN
	
	SELECT tbl.iID_TongHop_NguonNSDauTuID as Id, tbl.iID_ChungTu, tbl.iID_DuAnID, tbl.sMaNguon, tbl.sMaDich, tbl.sMaNguonCha, SUM(ISNULL(tbl.fGiaTri, 0)) as fGiaTri, tmp.IIDLoaiCongTrinh INTO #tmp
	FROM @data as tmp
	INNER JOIN VDT_TongHop_NguonNSDauTu as tbl on tmp.sMaNguon = tbl.sMaNguon 
											AND tmp.iID_ChungTu = tbl.iID_ChungTu 
											AND tbl.sMaDich = tbl.sMaDich 
											AND tbl.sMaTienTrinh in ('200', '100')
											AND tbl.iID_LoaiCongTrinh = tmp.IIDLoaiCongTrinh
	GROUP BY tbl.iID_TongHop_NguonNSDauTuID, tbl.iID_ChungTu, tbl.iID_DuAnID, tbl.sMaNguon, tbl.sMaDich, tbl.sMaNguonCha, tmp.IIDLoaiCongTrinh

	SELECT iID_ChungTu, iID_DuAnID, sMaNguon, SUM(ISNULL(fGiaTri, 0)) as fGiaTri, IIDLoaiCongTrinh INTO #tmpThanhToan
	FROM
	(
		SELECT tbl.iID_ChungTu, tbl.iID_DuAnID, tbl.sMaNguon as sMaNguon, ISNULL(tbl.fGiaTri, 0) as fGiaTri, tmp.IIDLoaiCongTrinh
		FROM @data as tmp
		INNER JOIN VDT_TongHop_NguonNSDauTu as tbl on tmp.sMaNguon = tbl.sMaDich
												AND tmp.iID_ChungTu = tbl.iID_ChungTu 
												AND tbl.sMaNguon = tmp.sMaDich 
												AND tbl.sMaTienTrinh = '300'
												AND tbl.iID_LoaiCongTrinh = tmp.IIDLoaiCongTrinh
		UNION ALL
		SELECT tbl.iID_ChungTu, tbl.iID_DuAnID, tbl.sMaNguonCha as sMaNguon, ISNULL(tbl.fGiaTri, 0) as fGiaTri, tmp.IIDLoaiCongTrinh
		FROM @data as tmp
		INNER JOIN VDT_TongHop_NguonNSDauTu as tbl on tmp.sMaNguon = tbl.sMaNguonCha
												AND tmp.iID_ChungTu = tbl.iId_MaNguonCha 
												AND tbl.sMaDich in ('201', '202', '211a', '212a')
												AND tbl.sMaTienTrinh = '200'
												AND tbl.bIsLog = 0
												AND tbl.iID_LoaiCongTrinh = tmp.IIDLoaiCongTrinh
	) as tbl
	GROUP BY iID_ChungTu, iID_DuAnID, sMaNguon, IIDLoaiCongTrinh

	SELECT tbl.Id,
		tbl.iID_ChungTu, 
		tbl.iID_DuAnID, 
		tbl.sMaNguon, 
		tbl.sMaDich, 
		tbl.sMaNguonCha, 
		tbl.IIDLoaiCongTrinh,
		(ISNULL(tbl.fGiaTri, 0) - ISNULL(tt.fGiaTri, 0)) as fGiaTri, 
		0 as iStatus, 
		CAST(0 as bit) as bIsLog, 
		NULL as iId_MaNguonCha,
		NULL as iThuHoiTUCheDo,
		NULL as ILoaiUng,
		NULL as IIDMucID,
		NULL as IIDTieuMucID,
		NULL as IIDTietMucID,
		NULL as IIDNganhID
	FROM #tmp as tbl
	LEFT JOIN #tmpThanhToan as tt on tbl.iID_ChungTu = tt.iID_ChungTu AND tbl.iID_DuAnID = tt.iID_DuAnID AND tbl.sMaNguon = tt.sMaNguon


	DROP TABLE #tmp
	DROP TABLE #tmpThanhToan
END