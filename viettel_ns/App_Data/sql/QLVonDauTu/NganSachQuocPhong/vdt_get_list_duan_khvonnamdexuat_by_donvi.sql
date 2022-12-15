--DECLARE @iIdMaDonViQuanLy nvarchar(50) ='220'
--DECLARE @iNamKeHoach int =2022

 SELECT da.iID_DuAnID, da.sMaDuAn, da.sTenDuAn INTO #tmp
FROM VDT_DA_DuAn da
--WHERE (da.iID_MaDonVi = @iIdMaDonViQuanLy ) AND (da.iID_DuAnID IN (SELECT ctdt.iID_DuAnID FROM VDT_KHV_KeHoach5Nam_ChiTiet ctdt))
WHERE ( da.iID_DonViQuanLyID = @iIdDonViQuanLy) AND (da.iID_DuAnID IN (SELECT ctdt.iID_DuAnID FROM VDT_KHV_KeHoach5Nam_ChiTiet ctdt))

SELECT DISTINCT tmp.iID_DuAnID INTO #tmpDuAnChuyenTiep
	FROM #tmp as tmp
	INNER JOIN VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet as dt on tmp.iID_DuAnID = dt.iID_DuAnID
	INNER JOIN VDT_KHV_PhanBoVon_DonVi_PheDuyet as tbl on dt.iID_PhanBoVon_DonVi_PheDuyet_ID = tbl.Id
	WHERE tbl.iNamKeHoach = @iNamKeHoach

SELECT tmp.iID_DuAnID, tmp.sMaDuAn, tmp.sTenDuAn, 
(CASE WHEN dact.iID_DuAnID IS NULL THEN N'Mở mới' ELSE N'Chuyển tiếp' END) as sTenLoaiDuAn
FROM #tmp as tmp
LEFT JOIN #tmpDuAnChuyenTiep as dact on tmp.iID_DuAnID = dact.iID_DuAnID
where tmp.iID_DuAnID not in (select iID_DuAnID from VDT_QT_QuyetToan)

DROP TABLE #tmp
DROP TABLE #tmpDuAnChuyenTiep