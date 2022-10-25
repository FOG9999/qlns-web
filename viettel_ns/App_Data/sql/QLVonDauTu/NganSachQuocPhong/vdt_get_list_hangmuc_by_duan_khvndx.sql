
DECLARE @iIdMaDonViQuanLy nvarchar(100)
DECLARE @iNamLamViec int
DECLARE @iIDNguonVonID int
DECLARE @dNgayLap date

	SELECT @iIdMaDonViQuanLy = iID_MaDonViQuanLy, @iNamLamViec = iNamKeHoach, @iIDNguonVonID = iID_NguonVonID, @dNgayLap = dNgayQuyetDinh
	FROM VDT_KHV_KeHoachVonNam_DeXuat 
	WHERE iID_KeHoachVonNamDeXuatID = @iIDKHVNDeXuatId

	BEGIN
		CREATE TABLE #tmpQDDT(iID_DuAnID uniqueidentifier)

		INSERT INTO #tmpQDDT(iID_DuAnID)
		SELECT DISTINCT tbl.iID_DuAnID
		FROM VDT_DA_QDDauTu AS tbl
		INNER JOIN VDT_DA_QDDauTu_NguonVon AS dt ON tbl.iID_QDDauTuID = dt.iID_QDDauTuID
		WHERE bActive = 1 
			AND dt.iID_NguonVonID = @iIDNguonVonID
			AND CAST(tbl.dNgayQuyetDinh as DATE) <= CAST(@dNgayLap as DATE)

		INSERT INTO #tmpQDDT(iID_DuAnID)
		SELECT DISTINCT tbl.iID_DuAnID
		FROM VDT_DA_ChuTruongDauTu AS tbl
		INNER JOIN VDT_DA_ChuTruongDauTu_NguonVon AS dt ON tbl.iID_ChuTruongDauTuID = dt.iID_ChuTruongDauTuID
		LEFT JOIN DM_ChuDauTu as cdt on tbl.iID_ChuDauTuID = cdt.ID
		LEFT JOIN #tmpQDDT AS tmp ON tbl.iID_DuAnID = tmp.iID_DuAnID
		WHERE bActive = 1 
			AND tmp.iID_DuAnID IS NULL 
			AND dt.iID_NguonVonID = @iIDNguonVonID 
			AND CAST(tbl.dNgayQuyetDinh as DATE) <= CAST(@dNgayLap as DATE)
	END

	SELECT dahm.iID_DuAnID, lct.sTenLoaiCongTrinh AS sTenHangMuc, lct.iID_LoaiCongTrinh as iID_LoaiCongTrinhID FROM VDT_DA_DuAn_HangMuc dahm
	LEFT JOIN VDT_DM_LoaiCongTrinh lct ON lct.iID_LoaiCongTrinh = dahm.iID_LoaiCongTrinhID
	WHERE dahm.iID_DuAnID IN (SELECT iID_DuAnID FROM #tmpQDDT)
	--AND dahm.iID_DuAnID IN (SELECT * FROM f_split(@lstDuAnID))

	DROP TABLE #tmpQDDT