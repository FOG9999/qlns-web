
declare	@iIdDuAnId uniqueidentifier set @iIdDuAnId = '9048531e-8eaf-424f-a61e-ae4300fb5b46'
declare	@iIdNguonVonId int set @iIdNguonVonId = 1
declare	@dNgayDeNghi DATE set @dNgayDeNghi = '05/05/2022'
declare	@iNamKeHoach int set @iNamKeHoach = 2022
declare	@iCoQuanThanhToan int set @iCoQuanThanhToan = 1
declare	@iIdPheDuyet uniqueidentifier set @iIdPheDuyet = null

--#DECLARE#--

	CREATE TABLE #tmp(
		IIdKeHoachVonId uniqueidentifier,
		SSoQuyetDinh nvarchar(100),
		INamKeHoach int,
		ILoaiKeHoachVon int,
		ILoaiNamKhv int,
		ICoQuanThanhToan int,
		FGiaTriKHV float,
	)

	-- Ke hoach von nam
	BEGIN
		WITH tmp as 
		(
			SELECT tbl.iID_KeHoachVonNam_DuocDuyetID as Id, 
				ROW_NUMBER() OVER (PARTITION BY tbl.iID_KeHoachVonNamGocID ORDER BY tbl.dNgayQuyetDinh DESC) as rn
			FROM VDT_KHV_KeHoachVonNam_DuocDuyet as tbl
			INNER JOIN VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet as dt on tbl.iID_KeHoachVonNam_DuocDuyetID= dt.iID_KeHoachVonNam_DuocDuyetID
			WHERE dt.iID_DuAnID = @iIdDuAnId
				AND tbl.iID_NguonVonID = @iIdNguonVonId
				AND iNamKeHoach = @iNamKeHoach
				AND CAST(tbl.dNgayQuyetDinh as DATE) <= CAST(@dNgayDeNghi as DATE)
		)
		INSERT INTO #tmp(IIdKeHoachVonId, SSoQuyetDinh, INamKeHoach, ILoaiKeHoachVon, ILoaiNamKhv, ICoQuanThanhToan, FGiaTriKHV)
		SELECT tmp.Id , tbl.SSoQuyetDinh, tbl.INamKeHoach, 1, 2, @iCoQuanThanhToan,
				(CASE @iCoQuanThanhToan WHEN 1 THEN SUM(ISNULL(dt.fCapPhatTaiKhoBac, 0)) ELSE SUM(ISNULL(dt.fCapPhatBangLenhChi, 0)) END) as FGiaTri
		FROM tmp as tmp
		INNER JOIN VDT_KHV_KeHoachVonNam_DuocDuyet as tbl on tmp.Id = tbl.iID_KeHoachVonNam_DuocDuyetID
		INNER JOIN VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet as dt on tbl.iID_KeHoachVonNam_DuocDuyetID = dt.iID_KeHoachVonNam_DuocDuyetID
		WHERE tmp.rn = 1 AND dt.iID_DuAnID = @iIdDuAnId
		GROUP BY tmp.Id, tbl.SSoQuyetDinh, tbl.INamKeHoach
	END

	-- Ke hoach von ung
	BEGIN
		INSERT INTO #tmp(IIdKeHoachVonId, SSoQuyetDinh, INamKeHoach, ILoaiKeHoachVon, ILoaiNamKhv, ICoQuanThanhToan, FGiaTriKHV)
		SELECT tbl.Id, tbl.sSoQuyetDinh, tbl.iNamKeHoach, 2 , 2, @iCoQuanThanhToan,
			(CASE WHEN @iCoQuanThanhToan = 1 THEN SUM(ISNULL(dt.fCapPhatTaiKhoBac, 0)) ELSE SUM(ISNULL(dt.fCapPhatBangLenhChi, 0)) END)
		FROM VDT_KHV_KeHoachVonUng as tbl
		INNER JOIN VDT_KHV_KeHoachVonUng_ChiTiet as dt on tbl.Id = dt.iID_KeHoachUngID
		WHERE dt.iID_DuAnID = @iIdDuAnId
			AND tbl.iID_NguonVonID = @iIdNguonVonId
			AND CAST(tbl.dNgayQuyetDinh as DATE) <= CAST(@dNgayDeNghi as DATE)
			AND tbl.iNamKeHoach = @iNamKeHoach
		GROUP BY tbl.Id, tbl.sSoQuyetDinh, tbl.iNamKeHoach
	END

	-- Ke hoach nam truoc chuyen sang
	BEGIN
		INSERT INTO #tmp(IIdKeHoachVonId, SSoQuyetDinh, INamKeHoach, ILoaiKeHoachVon, ILoaiNamKhv, ICoQuanThanhToan, FGiaTriKHV)
		SELECT tbl.iID_BCQuyetToanNienDoID as Id, tbl.sSoDeNghi, tbl.iNamKeHoach, (CASE WHEN iLoaiThanhToan = 1 THEN 3 ELSE 4 END), 1, tbl.iCoQuanThanhToan,
			(CASE WHEN iLoaiThanhToan = 1 THEN SUM(ISNULL(dt.fGiaTriNamTruocChuyenNamSau, 0) + ISNULL(dt.fGiaTriNamNayChuyenNamSau, 0)) 
				ELSE SUM(ISNULL(dt.FGiaTriUngChuyenNamSau, 0) - (ISNULL(dt.fLKThanhToanDenTrcNamQuyetToan_KHUng, 0) - ISNULL(dt.FGiaTriThuHoiTheoGiaiNganThucTe, 0) + ISNULL(dt.fThanhToan_KHUngNamTrcChuyenSang, 0) + ISNULL(dt.fThanhToan_KHUngNamNay, 0))) END)
		FROM VDT_QT_BCQuyetToanNienDo as tbl
		INNER JOIN VDT_QT_BCQuyetToanNienDo_ChiTiet_01 as dt on tbl.iID_BCQuyetToanNienDoID = dt.iID_BCQuyetToanNienDo
		WHERE dt.iID_DuAnID = @iIdDuAnId
			AND tbl.iID_NguonVonID = @iIdNguonVonId
			AND CAST(tbl.dNgayDeNghi as DATE) < CAST(@dNgayDeNghi AS DATE)
			AND iNamKeHoach < @iNamKeHoach
			AND tbl.iCoQuanThanhToan = @iCoQuanThanhToan
		GROUP BY tbl.iID_BCQuyetToanNienDoID, tbl.sSoDeNghi, tbl.iNamKeHoach, iLoaiThanhToan, tbl.iCoQuanThanhToan
	END

	-- So tien da thanh toan
	SELECT tmp.IIdKeHoachVonId, tmp.ILoaiKeHoachVon, 
		SUM(ISNULL(dt.fGiaTriThanhToanNN, 0) + ISNULL(dt.fGiaTriThanhToanTN, 0)) as fThanhToan,
		SUM(ISNULL(fGiaTriThuHoiNamTruocTN, 0) + ISNULL(fGiaTriThuHoiNamTruocNN, 0) + ISNULL(fGiaTriThuHoiNamNayTN, 0) + ISNULL(fGiaTriThuHoiNamNayNN, 0)) AS fThuHoi
		INTO #tmpThanhToan
	FROM VDT_TT_DeNghiThanhToan as tbl
	INNER JOIN VDT_TT_PheDuyetThanhToan_ChiTiet as dt on tbl.iID_DeNghiThanhToanID = dt.iID_DeNghiThanhToanID
	INNER JOIN #tmp as tmp on dt.iID_KeHoachVonID = tmp.IIdKeHoachVonId
							 AND tmp.ILoaiKeHoachVon = dt.iLoaiKeHoachVon
	WHERE tbl.iID_DuAnId = @iIdDuAnId
		AND tbl.iCoQuanThanhToan = @iCoQuanThanhToan
		AND CAST(dNgayDeNghi as DATE) <= CAST(@dNgayDeNghi as DATE)
		AND (@iIdPheDuyet IS NULL OR tbl.iID_DeNghiThanhToanID <> @iIdPheDuyet)
	GROUP BY tmp.IIdKeHoachVonId, tmp.ILoaiKeHoachVon

	SELECT tbl.IIdKeHoachVonId, tbl.SSoQuyetDinh, tbl.ILoaiKeHoachVon, tbl.ILoaiNamKhv, tbl.INamKeHoach, tbl.ICoQuanThanhToan, tbl.FGiaTriKHV, SUM(ISNULL(dt.fThanhToan, 0)) as FGiaTriKHVDaThanhToan, SUM(ISNULL(dt.fThuHoi, 0)) as FGiaTriKHVDaThuHoi INTO #tbl
	FROM #tmp as tbl
	LEFT JOIN #tmpThanhToan as dt on tbl.IIdKeHoachVonId = dt.IIdKeHoachVonId AND tbl.ILoaiKeHoachVon = dt.ILoaiKeHoachVon
	GROUP BY tbl.IIdKeHoachVonId, tbl.SSoQuyetDinh, tbl.ILoaiKeHoachVon, tbl.ILoaiNamKhv, tbl.INamKeHoach, tbl.ICoQuanThanhToan, tbl.FGiaTriKHV

	SELECT iID_KeHoachVonID, iLoai INTO #khv
	FROM VDT_TT_DeNghiThanhToan_KHV
	WHERE iID_DeNghiThanhToanID = @iIdPheDuyet

	SELECT tbl.*, CAST(0 as float) as FGiaTriThanhToanTN, CAST(0 as float) FGiaTriThanhToanNN, CAST(0 as float) FGiaTriThuHoiTrongNuoc, CAST(0 as float) FGiaTriThuHoiNgoaiNuoc, 0 as ILoaiNamTamUng
	FROM #tbl as tbl
	INNER JOIN #khv as dt on tbl.IIdKeHoachVonId = dt.iID_KeHoachVonID AND tbl.ILoaiKeHoachVon = dt.iLoai
	WHERE tbl.FGiaTriKHV > (tbl.FGiaTriKHVDaThanhToan - tbl.FGiaTriKHVDaThuHoi)

	DROP TABLE #tmp
	DROP TABLE #tmpThanhToan
	DROP TABLE #tbl
	DROP TABLE #khv
