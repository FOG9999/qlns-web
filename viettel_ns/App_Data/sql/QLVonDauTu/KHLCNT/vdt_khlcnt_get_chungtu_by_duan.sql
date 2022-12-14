IF(@iLoaiChungTu = 1)
BEGIN
	IF OBJECT_ID(N'tempdb..#tmp') IS NOT NULL
	BEGIN
	DROP TABLE #tmp
	END

	IF OBJECT_ID(N'tempdb..#tmpnhathau') IS NOT NULL
	BEGIN
	DROP TABLE #tmpnhathau
	END

	SELECT DISTINCT dt.iID_DuToanID as id, dt.sSoQuyetDinh, dt.dNgayQuyetDinh, dv.sTen as sTenDonVi, ISNULL(dt.fTongDuToanPheDuyet, 0) as fGiaTriPheDuyet into #tmp
	FROM VDT_DA_DuToan as dt
	INNER JOIN VDT_DA_DuAn as da on dt.iID_DuAnID = da.iID_DuAnID
	INNER JOIN NS_DonVi as dv on da.iID_DonViQuanLyID = dv.iID_Ma
	WHERE dt.iID_DuAnID = @iIdDuAnId AND dt.bActive = 1

	SELECT nt.Id,nt.iID_DuToanID, SUM(ISNULL(fTienTrungThau,0)) as tientrungthau, gt.iId_KHLCNhaThau into #tmpnhathau  FROM VDT_QDDT_KHLCNhaThau nt
	INNER JOIN VDT_DA_DuToan dt ON dt.iID_DuToanID = nt.iID_DuToanID
	INNER JOIN VDT_DA_GoiThau gt ON nt.Id = gt.iId_KHLCNhaThau
	WHERE nt.iID_DuToanID in (SELECT tmp.id FROM #tmp tmp )
	GROUP BY nt.Id, nt.iID_DuToanID, gt.iId_KHLCNhaThau

	SELECT tmp.* FROM #tmp as tmp
	LEFT JOIN #tmpnhathau nt on nt.iID_DuToanID = tmp.id
	WHERE (tmp.fGiaTriPheDuyet >= nt.tientrungthau and nt.iId_KHLCNhaThau = '00000000-0000-0000-0000-000000000000') or nt.iId_KHLCNhaThau = @idKHLCNT
	UNION ALL
	SELECT t.* FROM #tmp t
	WHERE t.id not in (SELECT nt.iID_DuToanID FROM #tmpnhathau nt)

END
ELSE
IF(@iLoaiChungTu = 2)
BEGIN
	IF OBJECT_ID(N'tempdb..#tmp1') IS NOT NULL
	BEGIN
	DROP TABLE #tmp1
	END

	IF OBJECT_ID(N'tempdb..#tmpnhathau1') IS NOT NULL
	BEGIN
	DROP TABLE #tmpnhathau1
	END

	SELECT dt.iID_QDDauTuID as id, dt.sSoQuyetDinh, dt.dNgayQuyetDinh, dv.sTen as sTenDonVi, ISNULL(dt.fTongMucDauTuPheDuyet, 0) as fGiaTriPheDuyet into #tmp1
	FROM VDT_DA_QDDauTu as dt
	INNER JOIN VDT_DA_DuAn as da on dt.iID_DuAnID = da.iID_DuAnID
	INNER JOIN NS_DonVi as dv on da.iID_DonViQuanLyID = dv.iID_Ma
	WHERE dt.iID_DuAnID = @iIdDuAnId AND dt.bActive = 1

	SELECT nt.Id,nt.iID_QDDauTuID, SUM(ISNULL(fTienTrungThau,0)) as tientrungthau, gt.iId_KHLCNhaThau into #tmpnhathau1  
	FROM VDT_QDDT_KHLCNhaThau nt
	INNER JOIN VDT_DA_GoiThau gt ON nt.Id = gt.iId_KHLCNhaThau
	WHERE nt.iID_QDDauTuID in (SELECT tmp.id FROM #tmp1 tmp )
	GROUP BY nt.Id, nt.iID_QDDauTuID, gt.iId_KHLCNhaThau

	SELECT tmp.* FROM #tmp1 as tmp
	LEFT JOIN #tmpnhathau1 nt on nt.iID_QDDauTuID = tmp.id
	WHERE (tmp.fGiaTriPheDuyet >= nt.tientrungthau and nt.iId_KHLCNhaThau = '00000000-0000-0000-0000-000000000000') or nt.iId_KHLCNhaThau = @idKHLCNT
	UNION ALL
	SELECT t.* FROM #tmp1 t
	WHERE t.id not in (SELECT nt.iID_QDDauTuID FROM #tmpnhathau1 nt)
END
ELSE 
BEGIN
	IF OBJECT_ID(N'tempdb..#tmp2') IS NOT NULL
	BEGIN
	DROP TABLE #tmp2
	END

	IF OBJECT_ID(N'tempdb..#tmpnhathau2') IS NOT NULL
	BEGIN
	DROP TABLE #tmpnhathau2
	END

	SELECT ct.iID_ChuTruongDauTuID as id, ct.sSoQuyetDinh, ct.dNgayQuyetDinh, dv.sTen as sTenDonVi, ISNULL(ct.fTMDTDuKienPheDuyet, 0) as fGiaTriPheDuyet into #tmp2
	FROM VDT_DA_ChuTruongDauTu as ct
	INNER JOIN VDT_DA_DuAn as da on ct.iID_DuAnID = da.iID_DuAnID
	INNER JOIN NS_DonVi as dv on da.iID_DonViQuanLyID = dv.iID_Ma
	WHERE ct.iID_DuAnID = @iIdDuAnId AND ct.bActive = 1

	SELECT nt.Id,nt.iID_ChuTruongDauTuID, SUM(ISNULL(fTienTrungThau,0)) as tientrungthau, gt.iId_KHLCNhaThau into #tmpnhathau2  
	FROM VDT_QDDT_KHLCNhaThau nt
	--INNER JOIN VDT_DA_DuToan dt ON dt.iID_DuToanID = nt.iID_DuToanID
	INNER JOIN VDT_DA_GoiThau gt ON nt.Id = gt.iId_KHLCNhaThau
	WHERE nt.iID_ChuTruongDauTuID in (SELECT tmp.id FROM #tmp2 tmp )
	GROUP BY nt.Id, nt.iID_ChuTruongDauTuID, gt.iId_KHLCNhaThau

	SELECT tmp.* FROM #tmp2 as tmp
	LEFT JOIN #tmpnhathau2 nt on nt.iID_ChuTruongDauTuID = tmp.id
	WHERE (tmp.fGiaTriPheDuyet >= nt.tientrungthau and nt.iId_KHLCNhaThau = '00000000-0000-0000-0000-000000000000') or nt.iId_KHLCNhaThau = @idKHLCNT
	UNION ALL
	SELECT t.* FROM #tmp2 t
	WHERE t.id not in (SELECT nt.iID_ChuTruongDauTuID FROM #tmpnhathau2 nt)
END