
DECLARE @iIdMaDonViQuanLy nvarchar(100)
DECLARE @iNamLamViec int
DECLARE @iIDNguonVonID int
DECLARE @dNgayLap date

DECLARE @guidEmpty uniqueidentifier = CAST(0x0 AS UNIQUEIDENTIFIER)

	SELECT @iIdMaDonViQuanLy = iID_MaDonViQuanLy, @iNamLamViec = iNamKeHoach, @iIDNguonVonID = iID_NguonVonID, @dNgayLap = dNgayQuyetDinh
	FROM VDT_KHV_KeHoachVonNam_DeXuat 
	WHERE iID_KeHoachVonNamDeXuatID = @iIDKHVNDeXuatId
	-- col 1 : Tong muc dau tu duoc duyet
	BEGIN
		CREATE TABLE #tmpQDDT(iID_DuAnID uniqueidentifier, fTongMucDauTuPheDuyet float, sChuDauTu nvarchar(500))

		INSERT INTO #tmpQDDT(iID_DuAnID,fTongMucDauTuPheDuyet)
		SELECT DISTINCT tbl.iID_DuAnID, ISNULL(tbl.fTongMucDauTuPheDuyet, 0)
		FROM VDT_DA_QDDauTu AS tbl
		INNER JOIN VDT_DA_QDDauTu_NguonVon AS dt ON tbl.iID_QDDauTuID = dt.iID_QDDauTuID
		WHERE bActive = 1 
			AND dt.iID_NguonVonID = @iIDNguonVonID
			AND CAST(tbl.dNgayQuyetDinh as DATE) <= CAST(@dNgayLap as DATE)

		INSERT INTO #tmpQDDT(iID_DuAnID,fTongMucDauTuPheDuyet, sChuDauTu)
		SELECT DISTINCT tbl.iID_DuAnID, ISNULL(tbl.fTMDTDuKienPheDuyet, 0), cdt.sTenCDT
		FROM VDT_DA_ChuTruongDauTu AS tbl
		INNER JOIN VDT_DA_ChuTruongDauTu_NguonVon AS dt ON tbl.iID_ChuTruongDauTuID = dt.iID_ChuTruongDauTuID
		LEFT JOIN DM_ChuDauTu as cdt on tbl.iID_ChuDauTuID = cdt.ID
		LEFT JOIN #tmpQDDT AS tmp ON tbl.iID_DuAnID = tmp.iID_DuAnID
		WHERE bActive = 1 
			AND tmp.iID_DuAnID IS NULL 
			AND dt.iID_NguonVonID = @iIDNguonVonID 
			AND CAST(tbl.dNgayQuyetDinh as DATE) <= CAST(@dNgayLap as DATE)
	END

	SELECT DISTINCT tmp.iID_DuAnID INTO #tmpDuAnChuyenTiep
	FROM #tmpQDDT as tmp
	INNER JOIN VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet as dt on tmp.iID_DuAnID = dt.iID_DuAnID
	INNER JOIN VDT_KHV_KeHoachVonNam_DuocDuyet as tbl on dt.iID_KeHoachVonNam_DuocDuyetID = tbl.iID_KeHoachVonNam_DuocDuyetID
	WHERE tbl.iNamKeHoach = (@iNamLamViec - 1)

	SELECT dt.iID_DuAnID, SUM(ISNULL(dt.fCapPhatTaiKhoBac, 0) + ISNULL(dt.fCapPhatBangLenhChi, 0)) as fLuyKeVonDaBoTriHetNam INTO #tmpVonBoTriNam
	FROM #tmpQDDT as qd
	INNER JOIN VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet as dt on qd.iID_DuAnID = dt.iID_DuAnID
	INNER JOIN VDT_KHV_KeHoachVonNam_DuocDuyet as tbl on dt.iID_KeHoachVonNam_DuocDuyetID = tbl.iID_KeHoachVonNam_DuocDuyetID
	WHERE ISNULL(tbl.iNamKeHoach, 0) = (ISNULL(@iNamLamViec, 0) - 1)
		AND tbl.iID_MaDonViQuanLy = @iIdMaDonViQuanLy
		AND tbl.iID_NguonVonID = @iIDNguonVonID
		AND tbl.bActive = 1
	GROUP BY dt.iID_DuAnID

	SELECT qd.iID_DuAnID, dt.fVonBoTriTuNamDenNam as fKeHoachTrungHanDuocDuyet INTO #tmpKhTh
	FROM #tmpQDDT as qd
	INNER JOIN VDT_KHV_KeHoach5Nam_ChiTiet as dt on qd.iID_DuAnID = dt.iID_DuAnID
	INNER JOIN VDT_KHV_KeHoach5Nam as tbl on dt.iID_KeHoach5NamID = tbl.iID_KeHoach5NamID AND tbl.bActive = 1
	WHERE (tbl.iGiaiDoanTu IS NULL OR tbl.iGiaiDoanTu <= @iNamLamViec)
		AND (tbl.iGiaiDoanDen IS NULL OR tbl.iGiaiDoanDen >= @iNamLamViec)
		AND tbl.iID_NguonVonID = @iIDNguonVonID

	-- col 2 : Luy ke von thuc hien den cuoi nam
	BEGIN
		SELECT dt.iID_DuAnID, dt.iID_LoaiCongTrinh, SUM(ISNULL(dt.fCapPhatBangLenhChi, 0) + ISNULL(dt.fCapPhatTaiKhoBac, 0)) as fLuyKeCuoiNam INTO #tmpLuyKe
		FROM VDT_KHV_PhanBoVon as tbl
		INNER JOIN VDT_KHV_PhanBoVon_ChiTiet as dt on tbl.iID_PhanBoVonID = dt.iID_PhanBoVonID
		INNER JOIN #tmpQDDT as tmp on tbl.iID_MaDonViQuanLy = @iIdMaDonViQuanLy
								AND dt.iID_DuAnID = tmp.iID_DuAnID
		WHERE tbl.iNamKeHoach <= (@iNamLamViec - 2)
			AND tbl.iID_NguonVonID = @iIDNguonVonID
		GROUP BY dt.iID_DuAnID, dt.iID_LoaiCongTrinh

	END 

	-- col 3 : Ke hoach von duoc giao
	BEGIN
		SELECT dt.iID_DuAnID, SUM(ISNULL(dt.fCapPhatBangLenhChi, 0) + ISNULL(dt.fCapPhatTaiKhoBac, 0)) as fKHVN INTO #tmpVonDuocGiao
		FROM VDT_KHV_PhanBoVon as tbl
		INNER JOIN VDT_KHV_PhanBoVon_ChiTiet as dt on tbl.iID_PhanBoVonID = dt.iID_PhanBoVonID
		INNER JOIN #tmpQDDT as tmp on tbl.iID_MaDonViQuanLy = @iIdMaDonViQuanLy
								AND dt.iID_DuAnID = tmp.iID_DuAnID
		WHERE tbl.iNamKeHoach = @iNamLamViec
			AND tbl.iID_NguonVonID = @iIDNguonVonID
		GROUP BY dt.iID_DuAnID
	END

	-- col 4 : VonKeoDaiCacNamTruoc
	SELECT dt.iID_DuAnID, SUM(ISNULL(dt.fGiaTriNamTruocChuyenNamSau, 0) + ISNULL(dt.fGiaTriNamNayChuyenNamSau, 0) - ISNULL(dt.fGiaTriTamUngDieuChinhGiam, 0)) as fVonKeoDai INTO #tmpVonKeoDai
	FROM VDT_QT_BCQuyetToanNienDo as tbl 
	INNER JOIN VDT_QT_BCQuyetToanNienDo_ChiTiet_01 as dt on tbl.iID_BCQuyetToanNienDoID = dt.iID_BCQuyetToanNienDo
	INNER JOIN #tmpQDDT as tmp on dt.iID_DuAnID = tmp.iID_DuAnID AND tbl.iID_MaDonViQuanLy = @iIdMaDonViQuanLy
	WHERE tbl.iNamKeHoach = (@iNamLamViec - 1) AND tbl.iID_NguonVonID = @iIDNguonVonID
	GROUP BY dt.iID_DuAnID

	-- col 5 : Kế hoạch trung hạn được duyệt

	BEGIN
		SELECT
			khthct.*,
			khth.ILoai
			into #tmpThDd
		FROM
			VDT_KHV_KeHoach5Nam_ChiTiet khthct
		INNER JOIN
			VDT_KHV_KeHoach5Nam khth
		ON khthct.iID_KeHoach5NamID = khth.iID_KeHoach5NamID
		WHERE @iNamLamViec >= khth.iGiaiDoanTu and @iNamLamViec <= khth.iGiaiDoanDen
	END

	SELECT dv.sTen,tmdt.iID_DuAnID, sMaDuAn, sTenDuAn, tmdt.sChuDauTu, lda.sTenLoaiDuAn, CONCAT(da.sKhoiCong,'-',da.sKetThuc) as sThoiGianThucHien, lct.sTenLoaiCongTrinh, pc.sTen as sCapPheDuyet,
		ISNULL(tmdt.fTongMucDauTuPheDuyet, 0) as fTongMucDauTuDuocDuyet, (ISNULL(lk.fLuyKeCuoiNam, 0) - ISNULL(vkd.fVonKeoDai, 0)) as fLuyKeVonNamTruoc, 
		ISNULL(vdg.fKHVN, 0) as fKeHoachVonDuocDuyetNamNay, ISNULL(vkd.fVonKeoDai, 0) as fVonKeoDaiCacNamTruoc, 
		(CAST(ISNULL(da.sKetThuc, '0') AS INT) - CAST(ISNULL(da.sKhoiCong, '0') AS INT)) AS iThoiGianThucHien INTO #tmp
	FROM #tmpQDDT as tmdt
	INNER JOIN VDT_DA_DuAn as da on tmdt.iID_DuAnID = da.iID_DuAnID
	LEFT JOIN VDT_DM_LoaiDuAn as lda on da.iID_LoaiDuAnId = lda.iID_LoaiDuAnID
	LEFT JOIN NS_DonVi dv on da.iID_DonViQuanLyID = dv.iID_Ma
	LEFT JOIN VDT_DM_LoaiCongTrinh as lct on da.iID_LoaiCongTrinhID = lct.iID_LoaiCongTrinh
	LEFT JOIN VDT_DM_PhanCapDuAn as pc on da.iID_CapPheDuyetID = pc.iID_PhanCapID
	LEFT JOIN #tmpLuyKe as lk on tmdt.iID_DuAnID = lk.iID_DuAnID
	LEFT JOIN #tmpVonDuocGiao as vdg on tmdt.iID_DuAnID = vdg.iID_DuAnID
	LEFT JOIN #tmpVonKeoDai as vkd on tmdt.iID_DuAnID = vkd.iID_DuAnID
	WHERE tmdt.iID_DuAnID IN (SELECT * FROM f_split(@lstDuAnID))

	SELECT @iIDKHVNDeXuatId as iID_KeHoachVonNamDeXuatID, ISNULL(dt.iID_KeHoachVonNamDeXuatChiTietID, @guidEmpty) as iID_KeHoachVonNamDeXuatChiTietID, tmp.iID_DuAnID, tmp.sMaDuAn, tmp.sTen, tmp.sTenDuAn, tmp.sTenLoaiDuAn, tmp.sThoiGianThucHien, tmp.sTenLoaiCongTrinh, tmp.sCapPheDuyet, tmp.sChuDauTu, dt.fThuHoiVonUngTruoc,
		(CASE WHEN dt.iID_DuAnID IS NULL THEN tmp.fTongMucDauTuDuocDuyet ELSE dt.fTongMucDauTuDuocDuyet END) as fTongMucDauTuDuocDuyet,
		tmp.fLuyKeVonNamTruoc as fLuyKeVonNamTruoc,
		tmp.fKeHoachVonDuocDuyetNamNay as fKeHoachVonDuocDuyetNamNay,
		--tmp.fVonKeoDaiCacNamTruoc as fVonKeoDaiCacNamTruoc,
		dt.fVonKeoDaiCacNamTruoc as fVonKeoDaiCacNamTruoc,
		(CASE WHEN dt.iID_DuAnID IS NULL THEN 0 ELSE dt.fUocThucHien END) as fUocThucHien,
		(CASE WHEN dt.iID_DuAnID IS NULL THEN 0 ELSE dt.fUocThucHien END) as fUocThucHien,
		(CASE WHEN dt.iID_DuAnID IS NULL THEN 0 ELSE dt.fThanhToan END) as fThanhToan,
		ISNULL(vbthn.fLuyKeVonDaBoTriHetNam, 0) as fLuyKeVonDaBoTriHetNam,
		ISNULL(khth.fKeHoachTrungHanDuocDuyet, 0) as fKeHoachTrungHanDuocDuyet1,
		tmp.iThoiGianThucHien AS iThoiGianThucHien,
		ISNULL((SELECT TOP(1) fVonBoTriTuNamDenNam  FROM VDT_KHV_KeHoach5Nam_ChiTiet WHERE iID_DuAnID = tmp.iID_DuAnID AND iID_NguonVonID = @iIDNguonVonID), 0) as fKeHoachTrungHanDuocDuyet,
		(CASE WHEN dact.iID_DuAnID IS NULL THEN N'Mở mới' ELSE N'Chuyển tiếp' END) as sLoaiDuAn,
		dt.iID_LoaiCongTrinh
	FROM #tmp as tmp
	LEFT JOIN VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet as dt on tmp.iID_DuAnID = dt.iID_DuAnID AND dt.iID_KeHoachVonNamDeXuatID = @iIDKHVNDeXuatId
	LEFT JOIN VDT_DA_DuAn  da on  dt.iID_DuAnID = da.iID_DuAnID
	LEFT JOIN #tmpDuAnChuyenTiep as dact on tmp.iID_DuAnID = dact.iID_DuAnID
	LEFT JOIN NS_DonVi dv on da.iID_DonViQuanLyID = dv.iID_Ma
	LEFT JOIN #tmpThDd khvnct on dt.iID_DuAnID = khvnct.iID_DuAnID and dt.iID_LoaiCongTrinh = khvnct.iID_LoaiCongTrinhID and khvnct.iID_NguonVonID = @iIDNguonVonID
	LEFT JOIN #tmpVonBoTriNam as vbthn on tmp.iID_DuAnID = vbthn.iID_DuAnID 
	LEFT JOIN #tmpKhTh as khth on tmp.iID_DuAnID = khth.iID_DuAnID
	WHERE (ISNULL(@sMaDuAn,'') = '' OR tmp.sMaDuAn LIKE N'%'+@sMaDuAn+'%')
	AND (ISNULL(@sTenDuAn,'') = '' OR tmp.sTenDuAn LIKE N'%'+@sTenDuAn+'%')
	AND (ISNULL(@sTen,'') = '' OR dv.sTen LIKE N'%'+@sTen+'%')
	AND (ISNULL(@sLoaiDuAn,'') = '' OR (CASE WHEN dact.iID_DuAnID IS NULL THEN N'Mở mới' ELSE N'Chuyển tiếp' END) LIKE N'%'+@sLoaiDuAn+'%')
	AND (ISNULL(@sThoiGianThucHien,'') = '' OR tmp.sThoiGianThucHien LIKE N'%'+@sThoiGianThucHien+'%')
	AND (ISNULL(@sChuDauTu,'') = '' OR tmp.sChuDauTu LIKE N'%'+@sChuDauTu+'%')

	DROP TABLE #tmp
	DROP TABLE #tmpVonBoTriNam
	DROP TABLE #tmpVonKeoDai
	DROP TABLE #tmpVonDuocGiao
	DROP TABLE #tmpLuyKe
	DROP TABLE #tmpQDDT
	DROP TABLE #tmpThDd
	DROP TABLE #tmpDuAnChuyenTiep
	DROP TABLE #tmpKhTh