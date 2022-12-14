
DECLARE @iIdMaDonViQuanLy nvarchar(100)
DECLARE @iNamLamViec int
DECLARE @iIDNguonVonID int
DECLARE @dNgayLap date
DECLARE @checkQDDT int


--DECLARE @sMaDuAn nvarchar(100)
--DECLARE @sTenDuAn nvarchar(100)
--DECLARE @sTen nvarchar(100)
--DECLARE @sLoaiDuAn nvarchar(100)
--DECLARE @sThoiGianThucHien nvarchar(100)
--DECLARE @sChuDauTu nvarchar(100)
--DECLARE @iIDKHVNDeXuatId uniqueidentifier = 'f68c5115-bcfd-49dc-8181-c4b4df53d4bc';
--DECLARE @iID_KeHoachVonNamDeXuatID_DieuChinh uniqueidentifier = '00000000-0000-0000-0000-000000000000';
--DECLARE @sTenDonViQuanLy nvarchar(100)
--DECLARE @lstDuAnID nvarchar(max) = '0e88eb47-2c0c-4939-aca7-af78009be8ff';
DECLARE @guidEmpty uniqueidentifier = CAST(0x0 AS UNIQUEIDENTIFIER)
	SELECT @iIdMaDonViQuanLy = iID_MaDonViQuanLy, @iNamLamViec = iNamKeHoach, @iIDNguonVonID = iID_NguonVonID, @dNgayLap = dNgayQuyetDinh
	FROM VDT_KHV_KeHoachVonNam_DeXuat khv
	LEFT JOIN NS_DonVi dv on dv.iID_Ma = khv.iID_DonViQuanLyID
	WHERE iID_KeHoachVonNamDeXuatID = @iIDKHVNDeXuatId;
	--Bang tmp lay don vi quan ly--
	SELECT iID_MaDonViQuanLy,  iNamKeHoach,  iID_NguonVonID,  dNgayQuyetDinh,  iID_DonViQuanLyID, dv.sTen as sTenDonViQuanLy INTO #tmpDonViQL
	FROM VDT_KHV_KeHoachVonNam_DeXuat khv
	LEFT JOIN NS_DonVi dv on dv.iID_Ma = khv.iID_DonViQuanLyID
	WHERE iID_KeHoachVonNamDeXuatID = @iIDKHVNDeXuatId
	-- col 1 : Tong muc dau tu duoc duyet
	BEGIN
		CREATE TABLE #tmpQDDT(iID_DuAnID uniqueidentifier, fTongMucDauTuPheDuyet float, sChuDauTu nvarchar(500), iID_ChuDauTuID uniqueidentifier)

		INSERT INTO #tmpQDDT(iID_DuAnID,fTongMucDauTuPheDuyet)
		SELECT DISTINCT tbl.iID_DuAnID, ISNULL(tbl.fTongMucDauTuPheDuyet, 0)
		FROM VDT_DA_QDDauTu AS tbl
		INNER JOIN VDT_DA_QDDauTu_NguonVon AS dt ON tbl.iID_QDDauTuID = dt.iID_QDDauTuID
		WHERE bActive = 1 
			AND dt.iID_NguonVonID = @iIDNguonVonID
			AND CAST(tbl.dNgayQuyetDinh as DATE) <= CAST(@dNgayLap as DATE)

		INSERT INTO #tmpQDDT(iID_DuAnID,fTongMucDauTuPheDuyet, sChuDauTu, iID_ChuDauTuID)
		SELECT DISTINCT tbl.iID_DuAnID, ISNULL(tbl.fTMDTDuKienPheDuyet, 0), cdt.sTenCDT, cdt.ID as iID_ChuDauTuID
		FROM VDT_DA_ChuTruongDauTu AS tbl
		INNER JOIN VDT_DA_ChuTruongDauTu_NguonVon AS dt ON tbl.iID_ChuTruongDauTuID = dt.iID_ChuTruongDauTuID
		LEFT JOIN DM_ChuDauTu as cdt on tbl.iID_ChuDauTuID = cdt.ID
		LEFT JOIN #tmpQDDT AS tmp ON tbl.iID_DuAnID = tmp.iID_DuAnID
		WHERE bActive = 1 
			AND tmp.iID_DuAnID IS NULL 
			AND dt.iID_NguonVonID = @iIDNguonVonID 
			AND CAST(tbl.dNgayQuyetDinh as DATE) <= CAST(@dNgayLap as DATE);

		-- trường hợp dự án được chọn không có trong quyết định đầu tư.
		set @checkQDDT = (select count(*) from #tmpQDDT);
		IF(@checkQDDT <= 0)
		--BEGIN
		--	iNSERT INTO #tmpQDDT(iID_DuAnID,fTongMucDauTuPheDuyet, sChuDauTu,iID_ChuDauTuID)
		--	SELECT DISTINCT da.iID_DuAnID, 0 as fTongMucDauTuPheDuyet,dmcdt.sTenCDT as sChuDauTu, dmcdt.ID as iID_ChuDauTuID FROM VDT_DA_DuAn da
		--	left join DM_ChuDauTu dmcdt on dmcdt.ID = da.iID_ChuDauTuID
		--	 cross join #tmpQDDT tm
		--	WHERE tm.iID_DuAnID NOT IN (SELECT * FROM dbo.f_split(@lstDuAnID)) AND da.iID_DuAnID IN (SELECT * FROM dbo.f_split(@lstDuAnID))
		--END
		--ELSE
		BEGIN
			iNSERT INTO #tmpQDDT(iID_DuAnID,fTongMucDauTuPheDuyet, sChuDauTu,iID_ChuDauTuID)
			SELECT DISTINCT da.iID_DuAnID, 0 as fTongMucDauTuPheDuyet,dmcdt.sTenCDT as sChuDauTu,dmcdt.ID as iID_ChuDauTuID  FROM VDT_DA_DuAn da
			left join DM_ChuDauTu dmcdt on dmcdt.ID = da.iID_ChuDauTuID
			WHERE  da.iID_DuAnID IN (SELECT * FROM dbo.f_split(@lstDuAnID))
		END

		--truong hop du an da chon khong co trong 2 truong hop tren--
		iNSERT INTO #tmpQDDT(iID_DuAnID,fTongMucDauTuPheDuyet, sChuDauTu,iID_ChuDauTuID)
		SELECT DISTINCT da.iID_DuAnID, 0 as fTongMucDauTuPheDuyet,dmcdt.sTenCDT as sChuDauTu,dmcdt.ID as iID_ChuDauTuID  FROM VDT_DA_DuAn da
		left join DM_ChuDauTu dmcdt on dmcdt.ID = da.iID_ChuDauTuID
		WHERE  da.iID_DuAnID  IN (SELECT * FROM dbo.f_split(@lstDuAnID))
			AND da.iID_DuAnID NOT IN (SELECT qddt.iID_DuAnID FROM #tmpQDDT as qddt)
	END
	SELECT DISTINCT tmp.iID_DuAnID INTO #tmpDuAnChuyenTiep
	FROM #tmpQDDT as tmp
	INNER JOIN VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet as dt on tmp.iID_DuAnID = dt.iID_DuAnID
	INNER JOIN VDT_KHV_KeHoachVonNam_DuocDuyet as tbl on dt.iID_KeHoachVonNam_DuocDuyetID = tbl.iID_KeHoachVonNam_DuocDuyetID AND tbl.iID_NguonVonID = @iIDNguonVonID
	WHERE tbl.iNamKeHoach = (@iNamLamViec - 1)

	SELECT dt.iID_DuAnID, SUM(ISNULL(dt.fCapPhatTaiKhoBac, 0) + ISNULL(dt.fCapPhatBangLenhChi, 0)) as fLuyKeVonDaBoTriHetNam INTO #tmpVonBoTriNam
	FROM #tmpQDDT as qd
	INNER JOIN VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet as dt on qd.iID_DuAnID = dt.iID_DuAnID
	INNER JOIN VDT_KHV_KeHoachVonNam_DuocDuyet as tbl on dt.iID_KeHoachVonNam_DuocDuyetID = tbl.iID_KeHoachVonNam_DuocDuyetID and tbl.iID_NguonVonID = @iIDNguonVonID
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
	--Lay Loai cong trinh, chu dau tu theo du an ---
	BEGIN
			
		WITH tmpCDT(iID_DuAnID,iID_ChuDauTuID,sChuDauTu)
		AS
		(
			SELECT da.iID_DuAnID ,ctdt.iID_ChuDauTuID,cdt.sTenCDT AS sChuDauTu 
			FROM VDT_DA_DuAn da
			LEFT JOIN VDT_DM_LoaiCongTrinh lct ON lct.iID_LoaiCongTrinh = da.iID_LoaiCongTrinhID
			LEFT JOIN VDT_DA_ChuTruongDauTu ctdt ON ctdt.iID_DuAnID = da.iID_DuAnID
			LEFT JOIN DM_ChuDauTu cdt ON cdt.ID = ctdt.iID_ChuDauTuID
			WHERE da.iID_DuAnID IN (SELECT * FROM f_split(@lstDuAnID)) AND da.iID_LoaiCongTrinhID  IS NOT NULL AND ctdt.bActive = 1
			UNION ALL

			SELECT da.iID_DuAnID , ctdt.iID_ChuDauTuID,cdt.sTenCDT as sChuDauTu 
			FROM VDT_DA_DuAn da
			LEFT JOIN VDT_DA_DuAn_HangMuc hm ON hm.iID_DuAnID = da.iID_DuAnID 
			LEFT JOIN VDT_DM_LoaiCongTrinh lct ON lct.iID_LoaiCongTrinh = hm.iID_LoaiCongTrinhID
			LEFT JOIN VDT_DA_ChuTruongDauTu ctdt ON ctdt.iID_DuAnID = da.iID_DuAnID
			LEFT JOIN DM_ChuDauTu cdt ON cdt.ID = ctdt.iID_ChuDauTuID
			WHERE da.iID_DuAnID IN (SELECT * FROM f_split(@lstDuAnID)) AND da.iID_LoaiCongTrinhID IS NULL AND ctdt.bActive = 1
		)
		SELECT DISTINCT * INTO #tmpCDT 
		FROM tmpCDT;

		WITH tmpLCTCDT(iID_DuAnID,sTenLoaiCongTrinh,iID_LoaiCongTrinh)
		AS
		(
			--SELECT da.iID_DuAnID , lct.sTenLoaiCongTrinh, lct.iID_LoaiCongTrinh
			--FROM VDT_DA_DuAn da
			--LEFT JOIN VDT_DM_LoaiCongTrinh lct ON lct.iID_LoaiCongTrinh = da.iID_LoaiCongTrinhID
			--WHERE da.iID_DuAnID in (SELECT * FROM f_split(@lstDuAnID)) 
			--UNION ALL

			SELECT da.iID_DuAnID , lct.sTenLoaiCongTrinh, lct.iID_LoaiCongTrinh
			FROM VDT_DA_DuAn da
			LEFT JOIN VDT_DA_DuAn_HangMuc hm ON hm.iID_DuAnID = da.iID_DuAnID 
			LEFT JOIN VDT_DM_LoaiCongTrinh lct ON lct.iID_LoaiCongTrinh = hm.iID_LoaiCongTrinhID
			WHERE da.iID_DuAnID IN (SELECT * FROM f_split(@lstDuAnID)) 
		)
		 
		SELECT DISTINCT * INTO #tmpLCTCDT
			FROM tmpLCTCDT;

	END
	--end LoaiCong trinh chu dau tu-----

	SELECT dv.sTen,tmdt.iID_DuAnID, sMaDuAn, sTenDuAn, tmpCDT.sChuDauTu, lda.sTenLoaiDuAn, CONCAT(da.sKhoiCong,'-',da.sKetThuc) as sThoiGianThucHien, tmpLCTCDT.sTenLoaiCongTrinh, pc.sTen as sCapPheDuyet,tmpLCTCDT.iID_LoaiCongTrinh,
		ISNULL(tmdt.fTongMucDauTuPheDuyet, 0) as fTongMucDauTuDuocDuyet, (ISNULL(lk.fLuyKeCuoiNam, 0) - ISNULL(vkd.fVonKeoDai, 0)) as fLuyKeVonNamTruoc, 
		ISNULL(vdg.fKHVN, 0) as fKeHoachVonDuocDuyetNamNay, ISNULL(vkd.fVonKeoDai, 0) as fVonKeoDaiCacNamTruoc, tmpCDT.iID_ChuDauTuID,
		(CAST(ISNULL(da.sKetThuc, '0') AS INT) - CAST(ISNULL(da.sKhoiCong, '0') AS INT)) AS iThoiGianThucHien, dahm.iID_DuAn_HangMucID INTO #tmp
	FROM #tmpQDDT as tmdt
	INNER JOIN VDT_DA_DuAn as da on tmdt.iID_DuAnID = da.iID_DuAnID
	LEFT JOIN VDT_DM_LoaiDuAn as lda on da.iID_LoaiDuAnId = lda.iID_LoaiDuAnID
	LEFT JOIN NS_DonVi dv on da.iID_DonViQuanLyID = dv.iID_Ma
	LEFT JOIN VDT_DM_LoaiCongTrinh as lct on da.iID_LoaiCongTrinhID = lct.iID_LoaiCongTrinh 
	LEFT JOIN VDT_DM_PhanCapDuAn as pc on da.iID_CapPheDuyetID = pc.iID_PhanCapID
	LEFT JOIN #tmpLuyKe as lk on tmdt.iID_DuAnID = lk.iID_DuAnID
	LEFT JOIN #tmpVonDuocGiao as vdg on tmdt.iID_DuAnID = vdg.iID_DuAnID
	LEFT JOIN #tmpVonKeoDai as vkd on tmdt.iID_DuAnID = vkd.iID_DuAnID
	LEFT JOIN #tmpLCTCDT as tmpLCTCDT on tmpLCTCDT.iID_DuAnID = tmdt.iID_DuAnID
	LEFT JOIN #tmpCDT as tmpCDT on tmpCDT.iID_DuAnID = tmdt.iID_DuAnID
	LEFT JOIN VDT_DA_DuAn_HangMuc dahm on dahm.iID_DuAnID = tmdt.iID_DuAnID AND dahm.iID_LoaiCongTrinhID = tmpLCTCDT.iID_LoaiCongTrinh
	WHERE tmdt.iID_DuAnID IN (SELECT * FROM f_split(@lstDuAnID))
	--lay gia tri dieu chinh--
	BEGIN
		CREATE TABLE #tmpGetfDieuChinh(iID_DuAnID uniqueidentifier,iID_KeHoachVonNamDeXuatChiTietID uniqueidentifier, FUocThucHienDC float,FThuHoiVonUngTruocDC  float,FThanhToanDC  float,iID_LoaiCongTrinhID uniqueidentifier)
		IF(@iID_KeHoachVonNamDeXuatID_DieuChinh <> @guidEmpty)
		BEGIN
			IF((SELECT COUNT(*) FROM VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet WHERE iID_KeHoachVonNamDeXuatID = @iID_KeHoachVonNamDeXuatID_DieuChinh ) > 0 )
				INSERT INTO #tmpGetfDieuChinh(iID_DuAnID,iID_KeHoachVonNamDeXuatChiTietID,FUocThucHienDC,FThuHoiVonUngTruocDC,FThanhToanDC,iID_LoaiCongTrinhID)
				SELECT 
				ISNULL(ct.iID_DuAnID, @guidEmpty),ISNULL(ct.iID_KeHoachVonNamDeXuatChiTietID, @guidEmpty),ISNULL(ct.fUocThucHien,0),ISNULL(ct.fThuHoiVonUngTruoc,0),ISNULL(ct.fThanhToan,0), ct.iID_LoaiCongTrinh FROM VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet ct
				WHERE ct.iID_KeHoachVonNamDeXuatID = @iID_KeHoachVonNamDeXuatID_DieuChinh
			ELSE
				INSERT INTO #tmpGetfDieuChinh(iID_DuAnID,iID_KeHoachVonNamDeXuatChiTietID,FUocThucHienDC,FThuHoiVonUngTruocDC,FThanhToanDC,iID_LoaiCongTrinhID)
				SELECT 
				ISNULL(ct.iID_DuAnID, @guidEmpty), @guidEmpty ,ISNULL(ct.fUocThucHien,0),ISNULL(ct.fThuHoiVonUngTruoc,0),ISNULL(ct.fThanhToan,0),ct.iID_LoaiCongTrinh FROM VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet ct
				WHERE ct.iID_KeHoachVonNamDeXuatID = @iIDKHVNDeXuatId
		END
		ELSE
		BEGIN
			DECLARE @iID_Parent uniqueidentifier = (SELECT iID_ParentId FROM VDT_KHV_KeHoachVonNam_DeXuat WHERE iID_KeHoachVonNamDeXuatID = @iIDKHVNDeXuatId)
			IF(@iID_Parent IS NOT NULL)
				INSERT INTO #tmpGetfDieuChinh(iID_DuAnID,iID_KeHoachVonNamDeXuatChiTietID,FUocThucHienDC,FThuHoiVonUngTruocDC,FThanhToanDC,iID_LoaiCongTrinhID)
				SELECT 
				ISNULL(ct.iID_DuAnID, @guidEmpty),ISNULL(ct.iID_KeHoachVonNamDeXuatChiTietID, @guidEmpty),ISNULL(ct.fUocThucHien,0),ISNULL(ct.fThuHoiVonUngTruoc,0),ISNULL(ct.fThanhToan,0),ct.iID_LoaiCongTrinh FROM VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet ct
				WHERE ct.iID_KeHoachVonNamDeXuatID = @iID_Parent
		END
	END
	--end--
	/*
		SELECT OUT 
		@type = 0: ko phai chung tu Tong hop
		@type = 1: la Chung Tu Tong Hop
	*/

--	select * FROM #tmpQDDT
--select * FROM #tmpDuAnChuyenTiep
--select * FROm #tmpLCTCDT
--select * FROM #tmp	

	DECLARE @type int set @type = (SELECT count(*) FROM VDT_KHV_KeHoachVonNam_DeXuat WHERE iID_KeHoachVonNamDeXuatID = @iIDKHVNDeXuatId AND sTongHop IS NOT NULL)
	IF(@type = 1)
		BEGIN 
			SELECT Distinct @iIDKHVNDeXuatId as iID_KeHoachVonNamDeXuatID,ISNULL(dt.iID_KeHoachVonNamDeXuatChiTietID, @guidEmpty) as iID_KeHoachVonNamDeXuatChiTietID, tmp.iID_DuAnID, tmp.sMaDuAn, tmp.sTenDuAn, tmp.sTenLoaiDuAn, tmp.sThoiGianThucHien, tmp.sTenLoaiCongTrinh, tmp.sCapPheDuyet, tmp.sChuDauTu, dt.fThuHoiVonUngTruoc,
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
				tmp.iID_LoaiCongTrinh,tmp.iID_ChuDauTuID,
				dvct.iID_Ma as iID_DonViQuanLyID,
				dvct.sTen as sTenDonViQuanLy,
				da.iID_DonViThucHienDuAnID as iID_DonViThucHienDuAn,
				dv.sTen as sTen,
				tmp.iID_DuAn_HangMucID as iID_DuAn_HangMucID

			FROM #tmp as tmp
			LEFT JOIN VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet as dt on tmp.iID_DuAnID = dt.iID_DuAnID AND dt.iID_KeHoachVonNamDeXuatID = @iIDKHVNDeXuatId AND tmp.iID_LoaiCongTrinh =dt.iID_LoaiCongTrinh
			LEFT JOIN VDT_DA_DuAn  da on  tmp.iID_DuAnID = da.iID_DuAnID
			LEFT JOIN #tmpDuAnChuyenTiep as dact on tmp.iID_DuAnID = dact.iID_DuAnID
			--LEFT JOIN VDT_DM_DonViThucHienDuAn dv on da.iID_DonViThucHienDuAnID = dv.iID_DonVi
			LEFT JOIN NS_DonVi dv on da.iID_DonViQuanLyID = dv.iID_Ma
			LEFT JOIN #tmpThDd khvnct on dt.iID_DuAnID = khvnct.iID_DuAnID and dt.iID_LoaiCongTrinh = khvnct.iID_LoaiCongTrinhID and khvnct.iID_NguonVonID = @iIDNguonVonID
			LEFT JOIN #tmpVonBoTriNam as vbthn on tmp.iID_DuAnID = vbthn.iID_DuAnID 
			LEFT JOIN #tmpKhTh as khth on tmp.iID_DuAnID = khth.iID_DuAnID
			LEFT JOIN NS_DonVi dvct on dvct.iID_Ma = dt.iID_DonViID
			WHERE (ISNULL(@sMaDuAn,'') = '' OR tmp.sMaDuAn LIKE N'%'+@sMaDuAn+'%')
			AND (ISNULL(@sTenDuAn,'') = '' OR tmp.sTenDuAn LIKE N'%'+@sTenDuAn+'%')
			AND (ISNULL(@sTenDonViQuanLy,'') = '' OR dvct.sTen LIKE N'%'+@sTenDonViQuanLy+'%')
			AND (ISNULL(@sLoaiDuAn,'') = '' OR (CASE WHEN dact.iID_DuAnID IS NULL THEN N'Mở mới' ELSE N'Chuyển tiếp' END) LIKE N'%'+@sLoaiDuAn+'%')
			AND (ISNULL(@sThoiGianThucHien,'') = '' OR tmp.sThoiGianThucHien LIKE N'%'+@sThoiGianThucHien+'%')
			AND (ISNULL(@sChuDauTu,'') = '' OR tmp.sChuDauTu LIKE N'%'+@sChuDauTu+'%')
		END
	ELSE
		BEGIN
			if(@iID_KeHoachVonNamDeXuatID_DieuChinh <> @guidEmpty)
			SELECT Distinct  
				(CASE WHEN  @iID_KeHoachVonNamDeXuatID_DieuChinh = @guidEmpty THEN  @iIDKHVNDeXuatId ELSE @iID_KeHoachVonNamDeXuatID_DieuChinh END) as iID_KeHoachVonNamDeXuatID ,
				(CASE  WHEN @iID_KeHoachVonNamDeXuatID_DieuChinh = @guidEmpty THEN ISNULL(dt.iID_KeHoachVonNamDeXuatChiTietID, @guidEmpty) ELSE fDieuChinh.iID_KeHoachVonNamDeXuatChiTietID END)as iID_KeHoachVonNamDeXuatChiTietID,
				tmp.iID_DuAnID, tmp.sMaDuAn, tmp.sTenDuAn, tmp.sTenLoaiDuAn, tmp.sThoiGianThucHien, tmp.sTenLoaiCongTrinh, tmp.sCapPheDuyet, tmp.sChuDauTu, dt.fThuHoiVonUngTruoc,
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
				tmp.iID_LoaiCongTrinh,tmp.iID_ChuDauTuID,
				tmpDonViQL.iID_DonViQuanLyID as iID_DonViQuanLyID,
				tmpDonViQL.sTenDonViQuanLy as sTenDonViQuanLy,
				da.iID_DonViThucHienDuAnID as iID_DonViThucHienDuAn,
				dv.sTen as sTen,
				fDieuChinh.FThanhToanDC as fThanhToanDC,
				fDieuChinh.FUocThucHienDC as fUocThucHienDC,
				fDieuChinh.FThuHoiVonUngTruocDC as fThuHoiVonUngTruocDC,
				tmp.iID_DuAn_HangMucID as iID_DuAn_HangMucID
			

				FROM #tmp as tmp
				LEFT JOIN VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet as dt on tmp.iID_DuAnID = dt.iID_DuAnID AND dt.iID_KeHoachVonNamDeXuatID = @iIDKHVNDeXuatId and dt.iID_LoaiCongTrinh = tmp.iID_LoaiCongTrinh
				LEFT JOIN VDT_DA_DuAn  da on  tmp.iID_DuAnID = da.iID_DuAnID
				LEFT JOIN #tmpDuAnChuyenTiep as dact on tmp.iID_DuAnID = dact.iID_DuAnID
				--LEFT JOIN VDT_DM_DonViThucHienDuAn dv on da.iID_DonViThucHienDuAnID = dv.iID_DonVi
				LEFT JOIN NS_DonVi dv on da.iID_DonViQuanLyID = dv.iID_Ma
				LEFT JOIN #tmpThDd khvnct on dt.iID_DuAnID = khvnct.iID_DuAnID and dt.iID_LoaiCongTrinh = khvnct.iID_LoaiCongTrinhID and khvnct.iID_NguonVonID = @iIDNguonVonID and khvnct.iID_LoaiCongTrinhID = tmp.iID_LoaiCongTrinh
				LEFT JOIN #tmpVonBoTriNam as vbthn on tmp.iID_DuAnID = vbthn.iID_DuAnID 
				LEFT JOIN #tmpKhTh as khth on tmp.iID_DuAnID = khth.iID_DuAnID
				CROSS JOIN #tmpDonViQL as tmpDonViQL
				LEFT JOIN #tmpGetfDieuChinh fDieuChinh on fDieuChinh.iID_DuAnID = tmp.iID_DuAnID and fDieuChinh.iID_LoaiCongTrinhID = tmp.iID_LoaiCongTrinh
				WHERE (ISNULL(@sMaDuAn,'') = '' OR tmp.sMaDuAn LIKE N'%'+@sMaDuAn+'%')
				AND (ISNULL(@sTenDuAn,'') = '' OR tmp.sTenDuAn LIKE N'%'+@sTenDuAn+'%')
				AND (ISNULL(@sTenDonViQuanLy,'') = '' OR tmpDonViQL.sTenDonViQuanLy LIKE N'%'+@sTenDonViQuanLy+'%')
				AND (ISNULL(@sLoaiDuAn,'') = '' OR (CASE WHEN dact.iID_DuAnID IS NULL THEN N'Mở mới' ELSE N'Chuyển tiếp' END) LIKE N'%'+@sLoaiDuAn+'%')
				AND (ISNULL(@sThoiGianThucHien,'') = '' OR tmp.sThoiGianThucHien LIKE N'%'+@sThoiGianThucHien+'%')
				AND (ISNULL(@sChuDauTu,'') = '' OR tmp.sChuDauTu LIKE N'%'+@sChuDauTu+'%')

			ELSE
			SELECT Distinct  

				(CASE WHEN  @iID_KeHoachVonNamDeXuatID_DieuChinh = @guidEmpty THEN  @iIDKHVNDeXuatId ELSE @iID_KeHoachVonNamDeXuatID_DieuChinh END) as iID_KeHoachVonNamDeXuatID ,
				(CASE  WHEN @iID_KeHoachVonNamDeXuatID_DieuChinh = @guidEmpty THEN ISNULL(dt.iID_KeHoachVonNamDeXuatChiTietID, @guidEmpty) ELSE fDieuChinh.iID_KeHoachVonNamDeXuatChiTietID END)as iID_KeHoachVonNamDeXuatChiTietID,
				tmp.iID_DuAnID, tmp.sMaDuAn, tmp.sTenDuAn, tmp.sTenLoaiDuAn, tmp.sThoiGianThucHien, tmp.sTenLoaiCongTrinh, tmp.sCapPheDuyet, tmp.sChuDauTu, dt.fThuHoiVonUngTruocDC,
				(CASE WHEN dt.iID_DuAnID IS NULL THEN tmp.fTongMucDauTuDuocDuyet ELSE dt.fTongMucDauTuDuocDuyet END) as fTongMucDauTuDuocDuyet,
				tmp.fLuyKeVonNamTruoc as fLuyKeVonNamTruoc,
				tmp.fKeHoachVonDuocDuyetNamNay as fKeHoachVonDuocDuyetNamNay,
				--tmp.fVonKeoDaiCacNamTruoc as fVonKeoDaiCacNamTruoc,
				dt.fVonKeoDaiCacNamTruoc as fVonKeoDaiCacNamTruoc,
				(CASE WHEN dt.iID_DuAnID IS NULL THEN 0 ELSE dt.fUocThucHien END) as fUocThucHienDC,
				(CASE WHEN dt.iID_DuAnID IS NULL THEN 0 ELSE dt.fUocThucHien END) as fUocThucHienDC,
				ISNULL(fDieuChinh.FThanhToanDC,0) as fThanhToanDC,
				ISNULL(vbthn.fLuyKeVonDaBoTriHetNam, 0) as fLuyKeVonDaBoTriHetNam,
				ISNULL(khth.fKeHoachTrungHanDuocDuyet, 0) as fKeHoachTrungHanDuocDuyet1,
				tmp.iThoiGianThucHien AS iThoiGianThucHien,
				ISNULL((SELECT TOP(1) fVonBoTriTuNamDenNam  FROM VDT_KHV_KeHoach5Nam_ChiTiet WHERE iID_DuAnID = tmp.iID_DuAnID AND iID_NguonVonID = @iIDNguonVonID), 0) as fKeHoachTrungHanDuocDuyet,
				(CASE WHEN dact.iID_DuAnID IS NULL THEN N'Mở mới' ELSE N'Chuyển tiếp' END) as sLoaiDuAn,
				tmp.iID_LoaiCongTrinh,tmp.iID_ChuDauTuID,
				tmpDonViQL.iID_DonViQuanLyID as iID_DonViQuanLyID,
				tmpDonViQL.sTenDonViQuanLy as sTenDonViQuanLy,
				da.iID_DonViThucHienDuAnID as iID_DonViThucHienDuAn,
				dv.sTen as sTen,
				(CASE WHEN dt.iID_DuAnID IS NULL THEN 0 ELSE dt.fThanhToan END) as fThanhToan,
				dt.fUocThucHien as fUocThucHien,
				dt.fThuHoiVonUngTruoc as fThuHoiVonUngTruoc,
				tmp.iID_DuAn_HangMucID as iID_DuAn_HangMucID


				

			FROM #tmp as tmp
			LEFT JOIN VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet as dt on tmp.iID_DuAnID = dt.iID_DuAnID AND dt.iID_KeHoachVonNamDeXuatID = @iIDKHVNDeXuatId AND tmp.iID_LoaiCongTrinh =dt.iID_LoaiCongTrinh
			LEFT JOIN VDT_DA_DuAn  da on  tmp.iID_DuAnID = da.iID_DuAnID
			LEFT JOIN #tmpDuAnChuyenTiep as dact on tmp.iID_DuAnID = dact.iID_DuAnID
			--LEFT JOIN VDT_DM_DonViThucHienDuAn dv on da.iID_DonViThucHienDuAnID = dv.iID_DonVi
			LEFT JOIN NS_DonVi dv on da.iID_DonViQuanLyID = dv.iID_Ma
			LEFT JOIN #tmpThDd khvnct on dt.iID_DuAnID = khvnct.iID_DuAnID and dt.iID_LoaiCongTrinh = khvnct.iID_LoaiCongTrinhID and khvnct.iID_NguonVonID = @iIDNguonVonID
			LEFT JOIN #tmpVonBoTriNam as vbthn on tmp.iID_DuAnID = vbthn.iID_DuAnID 
			LEFT JOIN #tmpKhTh as khth on tmp.iID_DuAnID = khth.iID_DuAnID
			CROSS JOIN #tmpDonViQL as tmpDonViQL
			LEFT JOIN #tmpGetfDieuChinh fDieuChinh on fDieuChinh.iID_DuAnID = tmp.iID_DuAnID
			WHERE (ISNULL(@sMaDuAn,'') = '' OR tmp.sMaDuAn LIKE N'%'+@sMaDuAn+'%')
			AND (ISNULL(@sTenDuAn,'') = '' OR tmp.sTenDuAn LIKE N'%'+@sTenDuAn+'%')
			AND (ISNULL(@sTenDonViQuanLy,'') = '' OR tmpDonViQL.sTenDonViQuanLy LIKE N'%'+@sTenDonViQuanLy+'%')
			AND (ISNULL(@sLoaiDuAn,'') = '' OR (CASE WHEN dact.iID_DuAnID IS NULL THEN N'Mở mới' ELSE N'Chuyển tiếp' END) LIKE N'%'+@sLoaiDuAn+'%')
			AND (ISNULL(@sThoiGianThucHien,'') = '' OR tmp.sThoiGianThucHien LIKE N'%'+@sThoiGianThucHien+'%')
			AND (ISNULL(@sChuDauTu,'') = '' OR tmp.sChuDauTu LIKE N'%'+@sChuDauTu+'%')
	 END

	DROP TABLE #tmp
	DROP TABLE #tmpVonBoTriNam
	DROP TABLE #tmpVonKeoDai
	DROP TABLE #tmpVonDuocGiao
	DROP TABLE #tmpLuyKe
	DROP TABLE #tmpQDDT
	DROP TABLE #tmpThDd
	DROP TABLE #tmpDuAnChuyenTiep
	DROP TABLE #tmpKhTh
	DROP TABLE #tmpLCTCDT
	DROP TABLE #tmpCDT
	DROP TABLE #tmpDonViQL
	DROP TABLE #tmpGetfDieuChinh



--	select * FROM VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet where iID_KeHoachVonNamDeXuatID = 'd4864ca4-7d72-4d22-8eaa-9420da6df589'
