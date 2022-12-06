DECLARE @iNamKeHoach int set @iNamKeHoach = 2019
DECLARE @iIdMaDonVi nvarchar(50) set @iIdMaDonVi = '21'
DECLARE @iIdNguonVon int set @iIdNguonVon = 2
DECLARE @iQuy int set @iQuy = 1
DECLARE @iTiGia bigint SET @iTiGia = 1

--###--
	--SELECT dt.iID_DuAnID, SUM(dt.fCapPhatBangLenhChi) as fCapPhatBangLenhChi, SUM(dt.fCapPhatTaiKhoBac) as fCapPhatTaiKhoBac INTO #tmp
	--FROM VDT_KHV_PhanBoVon as tbl
	--INNER JOIN VDT_KHV_PhanBoVon_ChiTiet as dt on tbl.iID_PhanBoVonID = dt.iID_PhanBoVonID
	--WHERE tbl.iID_MaDonViQuanLy = @iIdMaDonVi AND tbl.iNamKeHoach = @iNamKeHoach AND tbl.iID_NguonVonID = @iIdNguonVon
	--GROUP BY dt.iID_DuAnID
	
	SELECT dt.iID_DuAnID, SUM(dt.fCapPhatBangLenhChi) as fCapPhatBangLenhChi, SUM(dt.fCapPhatTaiKhoBac) as fCapPhatTaiKhoBac INTO #tmp
	FROM VDT_KHV_KeHoachVonNam_DuocDuyet as tbl
	INNER JOIN VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet as dt on tbl.iID_KeHoachVonNam_DuocDuyetID = dt.iID_KeHoachVonNam_DuocDuyetID
	WHERE tbl.iID_MaDonViQuanLy = @iIdMaDonVi AND tbl.iNamKeHoach = @iNamKeHoach AND tbl.iID_NguonVonID = @iIdNguonVon and tbl.bActive = 1
	GROUP BY dt.iID_DuAnID

	SELECT iID_DuAnId, 
		(CASE WHEN iCoQuanThanhToan = 2 THEN 'CQTC' ELSE N'Kho bạc' END ) as sLoaiThanhToan ,
		SUM(ISNULL(fThanhToanKLHTQuyTruoc, 0)) as fThanhToanKLHTQuyTruoc, SUM(ISNULL(fThanhToanTamUngQuyTruoc, 0)) as fThanhToanTamUngQuyTruoc INTO #tmpQuyTruoc
	FROM
	(
		SELECT tbl.iID_DuAnId, tbl.iCoQuanThanhToan,
			(CASE WHEN tbl.iLoaiThanhToan = 1 THEN SUM(ISNULL(dt.fGiaTriThanhToanNN, 0) + ISNULL(dt.fGiaTriThanhToanTN, 0)) ELSE CAST(0 as float) END) as fThanhToanKLHTQuyTruoc,
			(CASE WHEN tbl.iLoaiThanhToan = 2 THEN SUM(ISNULL(dt.fGiaTriThanhToanNN, 0) + ISNULL(dt.fGiaTriThanhToanTN, 0)) ELSE CAST(0 as float) END) as fThanhToanTamUngQuyTruoc
		FROM #tmp as tmp
		INNER JOIN VDT_TT_DeNghiThanhToan as tbl on tmp.iID_DuAnID = tbl.iID_DuAnId
		INNER JOIN VDT_TT_PheDuyetThanhToan_ChiTiet as dt on tbl.iID_DeNghiThanhToanID = dt.iID_DeNghiThanhToanID
		WHERE (
			(@iQuy = 1 AND MONTH(tbl.dNgayDeNghi) in (10,11,12) AND tbl.iNamKeHoach = (@iNamKeHoach - 1)) 
			OR (@iQuy = 2 AND MONTH(tbl.dNgayDeNghi) in (1,2,3) AND tbl.iNamKeHoach = @iNamKeHoach) 
			OR (@iQuy = 3 AND MONTH(tbl.dNgayDeNghi) in (4,5,6) AND tbl.iNamKeHoach = @iNamKeHoach) 
			OR (@iQuy = 4 AND MONTH(tbl.dNgayDeNghi) in (7,8,9) AND tbl.iNamKeHoach = @iNamKeHoach) 
		)
		GROUP BY tbl.iID_DuAnId, tbl.iLoaiThanhToan, tbl.iCoQuanThanhToan
	) as tbl
	GROUP BY tbl.iID_DuAnId, tbl.iCoQuanThanhToan

	SELECT iID_DuAnId, 
		(CASE WHEN iCoQuanThanhToan = 2 THEN 'CQTC' ELSE N'Kho bạc' END ) as sLoaiThanhToan ,
		SUM(ISNULL(fThanhToanKLHTQuyNay ,0)) as fThanhToanKLHTQuyNay, SUM(ISNULL(fThanhToanTamUngQuyNay ,0)) as fThanhToanTamUngQuyNay, SUM(ISNULL(fThuHoiUng ,0)) as fThuHoiUng  INTO #tmpQuyNay
	FROM
	(
		SELECT tbl.iID_DuAnId, tbl.iCoQuanThanhToan,
			(CASE WHEN tbl.iLoaiThanhToan = 1 THEN SUM(ISNULL(dt.fGiaTriThanhToanNN, 0) + ISNULL(dt.fGiaTriThanhToanTN, 0)) ELSE CAST(0 as float) END) as fThanhToanKLHTQuyNay,
			(CASE WHEN tbl.iLoaiThanhToan = 2 THEN SUM(ISNULL(dt.fGiaTriThanhToanNN, 0) + ISNULL(dt.fGiaTriThanhToanTN, 0)) ELSE CAST(0 as float) END) as fThanhToanTamUngQuyNay,
			SUM(ISNULL(dt.fGiaTriThuHoiNamNayNN, 0) + ISNULL(dt.fGiaTriThuHoiNamNayTN, 0)) as fThuHoiUng
		FROM #tmp as tmp
		INNER JOIN VDT_TT_DeNghiThanhToan as tbl on tmp.iID_DuAnID = tbl.iID_DuAnId
		INNER JOIN VDT_TT_PheDuyetThanhToan_ChiTiet as dt on tbl.iID_DeNghiThanhToanID = dt.iID_DeNghiThanhToanID
		WHERE tbl.iNamKeHoach = @iNamKeHoach AND 
			((@iQuy = 1 AND MONTH(tbl.dNgayDeNghi) in (1,2,3)) 
			OR (@iQuy = 2 AND MONTH(tbl.dNgayDeNghi) in (4,5,6)) 
			OR (@iQuy = 3 AND MONTH(tbl.dNgayDeNghi) in (7,8,9)) 
			OR (@iQuy = 4 AND MONTH(tbl.dNgayDeNghi) in (10,11,12))
		)
		GROUP BY tbl.iID_DuAnId, tbl.iLoaiThanhToan, tbl.iCoQuanThanhToan
	) as tbl
	GROUP BY tbl.iID_DuAnId, tbl.iCoQuanThanhToan



	SELECT tmp.iID_DuAnID, da.sTenDuAn, tmp.sLoaiThanhToan, da.iID_LoaiCongTrinhID, ISNULL(tmp.fGiaTri, 0)/@iTiGia as fKeHoachVonNam, 
		ISNULL(tblQuyTruoc.fThanhToanKLHTQuyTruoc, 0)/@iTiGia as fThanhToanKLHTQuyTruoc, ISNULL(tblQuyTruoc.fThanhToanTamUngQuyTruoc, 0)/@iTiGia as fThanhToanTamUngQuyTruoc, 
		ISNULL(tblQuyNay.fThanhToanKLHTQuyNay, 0)/@iTiGia as fThanhToanKLHTQuyNay, ISNULL(tblQuyNay.fThanhToanTamUngQuyNay, 0)/@iTiGia as fThanhToanTamUngQuyNay
		, ISNULL(tblQuyNay.fThuHoiUng, 0)/@iTiGia as fThuHoiUng, CAST(0 as float) as fGiaTriDeNghi, null as sGhiChu
	FROM 
	(
		SELECT iID_DuAnID, (CASE colname WHEN 'fCapPhatBangLenhChi' THEN N'CQTC' WHEN 'fCapPhatTaiKhoBac' THEN N'Kho bạc' END) as sLoaiThanhToan, fGiaTri
		FROM #tmp
		UNPIVOT 
		(fGiaTri FOR colname in (fCapPhatBangLenhChi, fCapPhatTaiKhoBac)) as dt
	) as tmp
	INNER JOIN VDT_DA_DuAn as da on tmp.iID_DuAnID = da.iID_DuAnID
	LEFT JOIN #tmpQuyTruoc as tblQuyTruoc on tmp.iID_DuAnID = tblQuyTruoc.iID_DuAnId AND tmp.sLoaiThanhToan = tblQuyTruoc.sLoaiThanhToan
	LEFT JOIN #tmpQuyNay as tblQuyNay on tmp.iID_DuAnID = tblQuyNay.iID_DuAnId AND tmp.sLoaiThanhToan = tblQuyNay.sLoaiThanhToan

