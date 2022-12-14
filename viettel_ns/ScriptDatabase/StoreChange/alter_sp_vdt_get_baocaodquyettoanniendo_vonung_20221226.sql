
ALTER PROC [dbo].[sp_vdt_get_baocaodquyettoanniendo_vonung]
@iIdMaDonVi nvarchar(100), 
@iNamKeHoach int, 
@iIdNguonVon int
AS
BEGIN
	CREATE TABLE #tmpUnion(IIDDuAnID uniqueidentifier, SMaDuAn nvarchar(500) , SDiaDiem nvarchar(500), STenDuAn nvarchar(500), iID_LoaiCongTrinh uniqueidentifier)

	INSERT INTO #tmpUnion(IIDDuAnID, SMaDuAn, SDiaDiem, STenDuAn, iID_LoaiCongTrinh)
	SELECT DISTINCT da.iID_DuAnID as IIDDuAnID, da.SMaDuAn, da.SDiaDiem, da.STenDuAn, dt.iID_LoaiCongTrinhID
	FROM VDT_KHV_KeHoachVonUng as tbl
	INNER JOIN VDT_KHV_KeHoachVonUng_ChiTiet as dt on tbl.Id = dt.iID_KeHoachUngID
	INNER JOIN VDT_DA_DuAn as da on dt.iID_DuAnID = da.iID_DuAnID
	INNER JOIN DM_ChuDauTu as cdt on da.iID_ChuDauTuID = cdt.ID
	WHERE cdt.sId_CDT = @iIdMaDonVi AND tbl.iNamKeHoach = @iNamKeHoach AND tbl.iID_NguonVonID = @iIdNguonVon
	UNION ALL
	SELECT DISTINCT da.iID_DuAnID as IIDDuAnID, da.SMaDuAn, da.SDiaDiem, da.STenDuAn, dt.iID_LoaiCongTrinh
	FROM VDT_QT_BCQuyetToanNienDo as tbl
	INNER JOIN VDT_QT_BCQuyetToanNienDo_ChiTiet_01 as dt on tbl.iID_BCQuyetToanNienDoID = dt.iID_BCQuyetToanNienDo
	INNER JOIN VDT_DA_DuAn as da on dt.iID_DuAnID = da.iID_DuAnID
	WHERE iNamKeHoach = @iNamKeHoach AND ((ISNULL(dt.fKHUngTrcChuaThuHoiTrcNamQuyetToan, 0) - ISNULL(dt.fThuHoiUngTruoc, 0) + ISNULL(dt.fKHUngNamNay, 0) - 
				(ISNULL(dt.fLKThanhToanDenTrcNamQuyetToan_KHUng, 0) - ISNULL(dt.fGiaTriThuHoiTheoGiaiNganThucTe, 0) + ISNULL(dt.fThanhToan_KHUngNamTrcChuyenSang, 0) + ISNULL(dt.fThanhToan_KHUngNamNay, 0))) <> 0)
	UNION ALL
	SELECT dt.iID_DuAnID as IIDDuAnID, da.SMaDuAn, da.SDiaDiem, da.STenDuAn, dt.iID_LoaiCongTrinh
	FROM 
	(SELECT iID_DuAnID , iID_LoaiCongTrinh,
		(CASE WHEN sMaNguon = '321b' AND sMaDich = '000' AND sMaTienTrinh = '100' THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) as fTongTien,
		(CASE WHEN sMaDich = '321b' AND sMaNguon = '000' AND sMaTienTrinh = '100' THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) as fKhauTru
	FROM VDT_TongHop_NguonNSDauTu
	WHERE iNamKeHoach = (@iNamKeHoach - 1) AND iID_MaDonViQuanLy = @iIdMaDonVi AND iID_NguonVonID = @iIdNguonVon
	GROUP BY iID_DuAnID, sMaNguon, sMaDich, sMaTienTrinh, iID_LoaiCongTrinh) as dt
	INNER JOIN VDT_DA_DuAn as da on dt.iID_DuAnID = da.iID_DuAnID
	WHERE (ISNULL(fTongTien, 0) - ISNULL(fKhauTru, 0)) > 0
	UNION ALL
	SELECT dt.iID_DuAnID as IIDDuAnID, da.SMaDuAn, da.SDiaDiem, da.STenDuAn, dt.iID_LoaiCongTrinh
	FROM 
	(SELECT iID_DuAnID , iID_LoaiCongTrinh,
		(CASE WHEN sMaNguon = '322b' AND sMaDich = '000' AND sMaTienTrinh = '100' THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) as fTongTien,
		(CASE WHEN sMaDich = '322b' AND sMaNguon = '000' AND sMaTienTrinh = '100' THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) as fKhauTru
	FROM VDT_TongHop_NguonNSDauTu
	WHERE iNamKeHoach = (@iNamKeHoach - 1) AND iID_MaDonViQuanLy = @iIdMaDonVi AND iID_NguonVonID = @iIdNguonVon
	GROUP BY iID_DuAnID, sMaNguon, sMaDich, sMaTienTrinh, iID_LoaiCongTrinh) as dt
	INNER JOIN VDT_DA_DuAn as da on dt.iID_DuAnID = da.iID_DuAnID
	WHERE (ISNULL(fTongTien, 0) - ISNULL(fKhauTru, 0)) > 0

	SELECT DISTINCT *, CAST(0 as bit) as BIsChuyenTiep INTO #tmp
	FROM #tmpUnion

	
	UPDATE tmp
	SET
		BIsChuyenTiep = 1
	FROM #tmp as tmp
	INNER JOIN (
		SELECT DISTINCT dt.iID_DuAnID 
		FROM VDT_KHV_PhanBoVon as tbl 
		INNER JOIN VDT_KHV_PhanBoVon_ChiTiet as dt on tbl.iID_PhanBoVonID = dt.iID_PhanBoVonID 
		WHERE tbl.bActive = 1 AND tbl.iNamKeHoach = (@iNamKeHoach - 1)
		) as mp on tmp.IIDDuAnID = mp.iID_DuAnID

	---- Kho bac
	BEGIN
		SELECT IIDDuAnID, tbl.iID_LoaiCongTrinh,
				SUM(ISNULL(fUngTruocChuaThuHoiNamTruoc, 0)) as fUngTruocChuaThuHoiNamTruoc,
				SUM(ISNULL(fUngTruocChuaThuHoiNamTruocDelete, 0)) as fUngTruocChuaThuHoiNamTruocDelete,
				SUM(ISNULL(fLuyKeThanhToanNamTruoc, 0)) as fLuyKeThanhToanNamTruoc,
				SUM(ISNULL(fLuyKeThanhToanNamTruocDelete, 0)) as fLuyKeThanhToanNamTruocDelete,
				SUM(ISNULL(fLuyKeTTKLHTChuaThuHoi, 0)) as fLuyKeTTKLHTChuaThuHoi,
				SUM(ISNULL(fLuyKeTTKLHTChuaThuHoiDelete, 0)) as fLuyKeTTKLHTChuaThuHoiDelete,
				SUM(ISNULL(fLuyKeThuHoiUngNamTruoc, 0)) as fLuyKeThuHoiUngNamTruoc,
				SUM(ISNULL(fLuyKeThuHoiUngNamTruocDelete, 0)) as fLuyKeThuHoiUngNamTruocDelete INTO #tmpNamTruocKB
		FROM
		(
			SELECT tmp.IIDDuAnID, tmp.iID_LoaiCongTrinh,
				(CASE WHEN sMaNguon = '321b' AND sMaTienTrinh = '100' AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) as fUngTruocChuaThuHoiNamTruoc,
				(CASE WHEN sMaDich = '321b' AND sMaTienTrinh = '300' AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) as fUngTruocChuaThuHoiNamTruocDelete,

				(CASE WHEN sMaDich = '403' AND sMaNguonCha = '321a' AND sMaTienTrinh = '100' AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fLuyKeThanhToanNamTruoc,
				(CASE WHEN sMaNguon = '403' AND sMaNguonCha = '321a' AND sMaTienTrinh = '300' AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fLuyKeThanhToanNamTruocDelete,

				(CASE WHEN sMaDich = '403a' AND sMaTienTrinh = '100' AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fLuyKeTTKLHTChuaThuHoi,
				(CASE WHEN sMaNguon = '403a' AND sMaTienTrinh = '300' AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fLuyKeTTKLHTChuaThuHoiDelete,

				(CASE WHEN sMaNguon = '211a' AND (sMaNguonCha IS NULL OR sMaNguonCha in ('121a', '131')) AND sMaTienTrinh = '200' AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fLuyKeThuHoiUngNamTruoc,
				(CASE WHEN sMaDich = '211a' AND (sMaNguonCha IS NULL OR sMaNguonCha in ('121a', '131')) AND sMaTienTrinh = '300' AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fLuyKeThuHoiUngNamTruocDelete
			FROM (SELECT DISTINCT * FROM #tmp) as tmp
			INNER JOIN VDT_TongHop_NguonNSDauTu as dt on tmp.IIDDuAnID = dt.iID_DuAnID AND tmp.iID_LoaiCongTrinh = dt.iID_LoaiCongTrinh
			WHERE iNamKeHoach = @iNamKeHoach -1
			GROUP BY tmp.IIDDuAnID, sMaDich, sMaNguon, sMaNguonCha, iID_NguonVonID, sMaTienTrinh, tmp.iId_LoaiCongTrinh
		) as tbl
		GROUP BY IIDDuAnID, tbl.iID_LoaiCongTrinh

		SELECT IIDDuAnID, tbl.iID_LoaiCongTrinh,
			SUM(ISNULL(fThanhToanKLHTNamTruocChuyenSang, 0)) as fThanhToanKLHTNamTruocChuyenSang,
			SUM(ISNULL(fThanhToanKLHTNamTruocChuyenSangDelete, 0)) as fThanhToanKLHTNamTruocChuyenSangDelete,
			SUM(ISNULL(fThanhToanUngNamTruocChuyenSang, 0)) as fThanhToanUngNamTruocChuyenSang,
			SUM(ISNULL(fThanhToanUngNamTruocChuyenSangDelete, 0)) as fThanhToanUngNamTruocChuyenSangDelete,
			
			SUM(ISNULL(fLuyKeThanhToanKHVU, 0)) as fLuyKeThanhToanKHVU,
			SUM(ISNULL(fLuyKeThanhToanKHVUDelete, 0)) as fLuyKeThanhToanKHVUDelete,

			SUM(ISNULL(fThuHoiUngTruocNamTruoc, 0)) as fThuHoiUngTruocNamTruoc,
			SUM(ISNULL(fThuHoiUngTruocNamTruocDelete, 0)) as fThuHoiUngTruocNamTruocDelete,

			SUM(ISNULL(fThuHoiTamUngNamNayVonNamTruoc, 0)) as fThuHoiTamUngNamNayVonNamTruoc,
			SUM(ISNULL(fThuHoiTamUngNamNayVonNamTruocDelete, 0)) as fThuHoiTamUngNamNayVonNamTruocDelete,
			SUM(ISNULL(fThuHoiTamUngNamTruocVonNamTruoc, 0)) as fThuHoiTamUngNamTruocVonNamTruoc,
			SUM(ISNULL(fThuHoiTamUngNamTruocVonNamTruocDelete, 0)) as fThuHoiTamUngNamTruocVonNamTruocDelete,

			SUM(ISNULL(fThuHoiVonUngNamNay, 0)) as fThuHoiVonUngNamNay,
			SUM(ISNULL(fThuHoiVonUngNamNayDelete, 0)) as fThuHoiVonUngNamNayDelete,

			SUM(ISNULL(fThuHoiVonUngKeHoachNamNay, 0)) as fThuHoiVonUngKeHoachNamNay,
			SUM(ISNULL(fThuHoiVonUngKeHoachNamNayDelete, 0)) as fThuHoiVonUngKeHoachNamNayDelete,

			SUM(ISNULL(fKHVUNamNay, 0)) as fKHVUNamNay,
			SUM(ISNULL(fKHVUNamNayDelete, 0)) as fKHVUNamNayDelete,

			SUM(ISNULL(fThanhToanKLHTTamUngNamNay, 0)) as fThanhToanKLHTTamUngNamNay,
			SUM(ISNULL(fThanhToanKLHTTamUngNamNayDelete, 0)) as fThanhToanKLHTTamUngNamNayDelete,
			SUM(ISNULL(fThanhToanUngNamNay, 0)) as fThanhToanUngNamNay,
			SUM(ISNULL(fThanhToanUngNamNayDelete, 0)) as fThanhToanUngNamNayDelete,

			SUM(ISNULL(fThuHoiTamUngNamNay, 0)) as fThuHoiTamUngNamNay,
			SUM(ISNULL(fThuHoiTamUngNamNayDelete, 0)) as fThuHoiTamUngNamNayDelete,
			SUM(ISNULL(fThuHoiTamUngNamTruoc, 0)) as fThuHoiTamUngNamTruoc,
			SUM(ISNULL(fThuHoiTamUngNamTruocDelete, 0)) as fThuHoiTamUngNamTruocDelete INTO #tmpNamNayKB
		FROM
		(
			SELECT tmp.IIDDuAnID, tmp.iID_LoaiCongTrinh,
				(CASE WHEN sMaDich = '201' AND sMaNguonCha = '131' AND sMaTienTrinh = '200' AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fThanhToanKLHTNamTruocChuyenSang,
				(CASE WHEN sMaNguon = '201' AND sMaNguonCha = '131' AND sMaTienTrinh = '300' AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fThanhToanKLHTNamTruocChuyenSangDelete,
				(CASE WHEN sMaDich = '211a' AND sMaNguonCha = '131' AND sMaTienTrinh = '200' AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fThanhToanUngNamTruocChuyenSang,
				(CASE WHEN sMaNguon = '211a' AND sMaNguonCha = '131' AND sMaTienTrinh = '300' AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fThanhToanUngNamTruocChuyenSangDelete,
				
				(CASE WHEN sMaDich = '201' AND sMaNguonCha in ('121a','131') AND sMaTienTrinh = '200' AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fLuyKeThanhToanKHVU,
				(CASE WHEN sMaNguon = '201' AND sMaNguonCha in ('121a','131') AND sMaTienTrinh = '300' AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fLuyKeThanhToanKHVUDelete,

				(CASE WHEN sMaNguon = '211a' AND sMaNguonCha = '101' AND iLoaiUng = 2 AND sMaTienTrinh = '200' THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) as fThuHoiUngTruocNamTruoc,
				(CASE WHEN sMaDich = '211a' AND sMaNguonCha = '101' AND iLoaiUng = 2 AND sMaTienTrinh = '300' THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) as fThuHoiUngTruocNamTruocDelete,

				(CASE WHEN sMaNguon = '211a' AND sMaNguonCha = '131' AND sMaTienTrinh = '200' AND (iThuHoiTUCheDo = 2 OR iThuHoiTUCheDo IS NULL) AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fThuHoiTamUngNamNayVonNamTruoc,
				(CASE WHEN sMaDich = '211a' AND sMaNguonCha = '131' AND sMaTienTrinh = '300' AND (iThuHoiTUCheDo = 2 OR iThuHoiTUCheDo IS NULL) AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fThuHoiTamUngNamNayVonNamTruocDelete,
				(CASE WHEN sMaNguon = '211a' AND sMaNguonCha = '131' AND sMaTienTrinh = '200' AND iThuHoiTUCheDo = 1 AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fThuHoiTamUngNamTruocVonNamTruoc,
				(CASE WHEN sMaDich = '211a' AND sMaNguonCha = '131' AND sMaTienTrinh = '300' AND iThuHoiTUCheDo = 1 AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fThuHoiTamUngNamTruocVonNamTruocDelete,

				(CASE WHEN sMaDich = '121a'AND sMaNguon = '000' AND bKeHoach = 1 AND sMaTienTrinh = '200' THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fThuHoiVonUngNamNay,
				(CASE WHEN sMaNguon = '121a'AND sMaDich = '000' AND bKeHoach = 1 AND sMaTienTrinh = '300' THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fThuHoiVonUngNamNayDelete,

				(CASE WHEN sMaDich = '121a'AND bKeHoach = 0 AND sMaTienTrinh = '200' THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fThuHoiVonUngKeHoachNamNay,
				(CASE WHEN sMaNguon = '121a'AND bKeHoach = 0 AND sMaTienTrinh = '300' THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fThuHoiVonUngKeHoachNamNayDelete,

				(CASE WHEN sMaNguon = '121a' AND sMaDich = '000' AND sMaTienTrinh = '200' THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fKHVUNamNay,
				(CASE WHEN sMaDich = '121a' AND sMaNguon = '000' AND sMaTienTrinh = '300' THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fKHVUNamNayDelete,

				(CASE WHEN sMaDich = '201' AND sMaNguonCha = '121a' AND sMaTienTrinh = '200' AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fThanhToanKLHTTamUngNamNay,
				(CASE WHEN sMaNguon = '201' AND sMaNguonCha = '121a' AND sMaTienTrinh = '300' AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fThanhToanKLHTTamUngNamNayDelete,
				(CASE WHEN sMaDich = '211a' AND sMaNguonCha = '121a' AND sMaTienTrinh = '200' AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) FThanhToanUngNamNay,
				(CASE WHEN sMaNguon = '211a' AND sMaNguonCha = '121a' AND sMaTienTrinh = '300' AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fThanhToanUngNamNayDelete,

				(CASE WHEN sMaNguon = '211a' AND sMaNguonCha = '121a' AND sMaTienTrinh = '200' AND iThuHoiTUCheDo = 2 AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fThuHoiTamUngNamNay,
				(CASE WHEN sMaDich = '211a' AND sMaNguonCha = '121a' AND sMaTienTrinh = '300' AND iThuHoiTUCheDo = 2 AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fThuHoiTamUngNamNayDelete,
				(CASE WHEN sMaNguon = '211a' AND sMaNguonCha = '121a' AND sMaTienTrinh = '200' AND iThuHoiTUCheDo = 1 AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fThuHoiTamUngNamTruoc,
				(CASE WHEN sMaDich = '211a' AND sMaNguonCha = '121a' AND sMaTienTrinh = '300' AND iThuHoiTUCheDo = 1 AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fThuHoiTamUngNamTruocDelete
			FROM (SELECT DISTINCT * FROM #tmp) as tmp
			INNER JOIN VDT_TongHop_NguonNSDauTu as dt on tmp.IIDDuAnID = dt.iID_DuAnID AND tmp.iID_LoaiCongTrinh = dt.iID_LoaiCongTrinh
			WHERE iNamKeHoach = @iNamKeHoach 
			GROUP BY tmp.IIDDuAnID, sMaDich, sMaNguon, sMaNguonCha, iID_NguonVonID, sMaTienTrinh, iThuHoiTUCheDo, iLoaiUng, bKeHoach, tmp.iID_LoaiCongTrinh
		) as tbl
		GROUP BY IIDDuAnID, iID_LoaiCongTrinh
	END
	
	-- CQTC
	BEGIN
		SELECT IIDDuAnID, tbl.iId_LoaiCongTrinh,
				SUM(ISNULL(fUngTruocChuaThuHoiNamTruoc, 0)) as fUngTruocChuaThuHoiNamTruoc,
				SUM(ISNULL(fUngTruocChuaThuHoiNamTruocDelete, 0)) as fUngTruocChuaThuHoiNamTruocDelete,
				SUM(ISNULL(fLuyKeThanhToanNamTruoc, 0)) as fLuyKeThanhToanNamTruoc,
				SUM(ISNULL(fLuyKeThanhToanNamTruocDelete, 0)) as fLuyKeThanhToanNamTruocDelete,
				SUM(ISNULL(fLuyKeTTKLHTChuaThuHoi, 0)) as fLuyKeTTKLHTChuaThuHoi,
				SUM(ISNULL(fLuyKeTTKLHTChuaThuHoiDelete, 0)) as fLuyKeTTKLHTChuaThuHoiDelete,
				SUM(ISNULL(fLuyKeThuHoiUngNamTruoc, 0)) as fLuyKeThuHoiUngNamTruoc,
				SUM(ISNULL(fLuyKeThuHoiUngNamTruocDelete, 0)) as fLuyKeThuHoiUngNamTruocDelete INTO #tmpNamTruocCQTC
		FROM
		(
			SELECT tmp.IIDDuAnID, tmp.iID_LoaiCongTrinh,
				(CASE WHEN sMaNguon = '322b' AND sMaTienTrinh = '100' AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) as fUngTruocChuaThuHoiNamTruoc,
				(CASE WHEN sMaDich = '322b' AND sMaTienTrinh = '300' AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) as fUngTruocChuaThuHoiNamTruocDelete,

				(CASE WHEN sMaDich = '404' AND sMaNguonCha = '322a' AND sMaTienTrinh = '100' AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fLuyKeThanhToanNamTruoc,
				(CASE WHEN sMaNguon = '404' AND sMaNguonCha = '322a' AND sMaTienTrinh = '300' AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fLuyKeThanhToanNamTruocDelete,
				
				(CASE WHEN sMaDich = '404a' AND sMaTienTrinh = '100' AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fLuyKeTTKLHTChuaThuHoi,
				(CASE WHEN sMaNguon = '404a' AND sMaTienTrinh = '300' AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fLuyKeTTKLHTChuaThuHoiDelete,

				(CASE WHEN sMaNguon = '212a' AND (sMaNguonCha IS NULL OR sMaNguonCha in ('122a', '132')) AND sMaTienTrinh = '200' AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fLuyKeThuHoiUngNamTruoc,
				(CASE WHEN sMaDich = '212a' AND (sMaNguonCha IS NULL OR sMaNguonCha in ('122a', '132')) AND sMaTienTrinh = '300' AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fLuyKeThuHoiUngNamTruocDelete
			FROM (SELECT DISTINCT * FROM #tmp) as tmp
			INNER JOIN VDT_TongHop_NguonNSDauTu as dt on tmp.IIDDuAnID = dt.iID_DuAnID AND tmp.iID_LoaiCongTrinh = dt.iID_LoaiCongTrinh
			WHERE iNamKeHoach = @iNamKeHoach - 1
			GROUP BY tmp.IIDDuAnID, sMaDich, sMaNguon, sMaNguonCha, iID_NguonVonID, sMaTienTrinh, tmp.iID_LoaiCongTrinh
		) as tbl
		GROUP BY IIDDuAnID, iID_LoaiCongTrinh

		SELECT IIDDuAnID, tbl.iID_LoaiCongTrinh,
			SUM(ISNULL(fThanhToanKLHTNamTruocChuyenSang, 0)) as fThanhToanKLHTNamTruocChuyenSang,
			SUM(ISNULL(fThanhToanKLHTNamTruocChuyenSangDelete, 0)) as fThanhToanKLHTNamTruocChuyenSangDelete,
			SUM(ISNULL(fThanhToanUngNamTruocChuyenSang, 0)) as fThanhToanUngNamTruocChuyenSang,
			SUM(ISNULL(fThanhToanUngNamTruocChuyenSangDelete, 0)) as fThanhToanUngNamTruocChuyenSangDelete,
			SUM(ISNULL(fThuHoiUngTruocNamTruoc, 0)) as fThuHoiUngTruocNamTruoc,
			SUM(ISNULL(fThuHoiUngTruocNamTruocDelete, 0)) as fThuHoiUngTruocNamTruocDelete,
			
			SUM(ISNULL(fThuHoiTamUngNamNayVonNamTruoc, 0)) as fThuHoiTamUngNamNayVonNamTruoc,
			SUM(ISNULL(fThuHoiTamUngNamNayVonNamTruocDelete, 0)) as fThuHoiTamUngNamNayVonNamTruocDelete,
			SUM(ISNULL(fThuHoiTamUngNamTruocVonNamTruoc, 0)) as fThuHoiTamUngNamTruocVonNamTruoc,
			SUM(ISNULL(fThuHoiTamUngNamTruocVonNamTruocDelete, 0)) as fThuHoiTamUngNamTruocVonNamTruocDelete,
			SUM(ISNULL(fLuyKeThanhToanKHVU, 0)) as fLuyKeThanhToanKHVU,
			SUM(ISNULL(fLuyKeThanhToanKHVUDelete, 0)) as fLuyKeThanhToanKHVUDelete,

			SUM(ISNULL(fLuyKeThanhToanTrongNam, 0)) as fLuyKeThanhToanTrongNam,
			SUM(ISNULL(fLuyKeThanhToanTrongNamDelete, 0)) as fLuyKeThanhToanTrongNamDelete,
			SUM(ISNULL(fThuHoiVonUngNamNay, 0)) as fThuHoiVonUngNamNay,
			SUM(ISNULL(fThuHoiVonUngNamNayDelete, 0)) as fThuHoiVonUngNamNayDelete,

			SUM(ISNULL(fThuHoiVonUngKeHoachNamNay, 0)) as fThuHoiVonUngKeHoachNamNay,
			SUM(ISNULL(fThuHoiVonUngKeHoachNamNayDelete, 0)) as fThuHoiVonUngKeHoachNamNayDelete,

			SUM(ISNULL(fKHVUNamNay, 0)) as fKHVUNamNay,
			SUM(ISNULL(fKHVUNamNayDelete, 0)) as fKHVUNamNayDelete,
			
			SUM(ISNULL(fThanhToanKLHTTamUngNamNay, 0)) as fThanhToanKLHTTamUngNamNay,
			SUM(ISNULL(fThanhToanKLHTTamUngNamNayDelete, 0)) as fThanhToanKLHTTamUngNamNayDelete,
			SUM(ISNULL(fThanhToanUngNamNay, 0)) as fThanhToanUngNamNay,
			SUM(ISNULL(fThanhToanUngNamNayDelete, 0)) as fThanhToanUngNamNayDelete,

			SUM(ISNULL(fThuHoiTamUngNamNay, 0)) as fThuHoiTamUngNamNay,
			SUM(ISNULL(fThuHoiTamUngNamNayDelete, 0)) as fThuHoiTamUngNamNayDelete,
			SUM(ISNULL(fThuHoiTamUngNamTruoc, 0)) as fThuHoiTamUngNamTruoc,
			SUM(ISNULL(fThuHoiTamUngNamTruocDelete, 0)) as fThuHoiTamUngNamTruocDelete INTO #tmpNamNayCQTC
		FROM
		(
			SELECT tmp.IIDDuAnID, tmp.iID_LoaiCongTrinh,
				(CASE WHEN sMaDich = '202' AND sMaNguonCha = '132' AND sMaTienTrinh = '200' AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fThanhToanKLHTNamTruocChuyenSang,
				(CASE WHEN sMaNguon = '202' AND sMaNguonCha = '132' AND sMaTienTrinh = '300' AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fThanhToanKLHTNamTruocChuyenSangDelete,
				(CASE WHEN sMaDich = '212a' AND sMaNguonCha = '132' AND sMaTienTrinh = '200' AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fThanhToanUngNamTruocChuyenSang,
				(CASE WHEN sMaNguon = '212a' AND sMaNguonCha = '132' AND sMaTienTrinh = '300' AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fThanhToanUngNamTruocChuyenSangDelete,
				
				(CASE WHEN sMaNguon = '212a' AND sMaNguonCha = '102' AND iLoaiUng = 2 AND sMaTienTrinh = '200' THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) as fThuHoiUngTruocNamTruoc,
				(CASE WHEN sMaDich = '212a' AND sMaNguonCha = '102' AND iLoaiUng = 2 AND sMaTienTrinh = '300' THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) as fThuHoiUngTruocNamTruocDelete,
				
				(CASE WHEN sMaDich = '202' AND sMaNguonCha in ('122a','132') AND sMaTienTrinh = '200' AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fLuyKeThanhToanKHVU,
				(CASE WHEN sMaNguon = '202' AND sMaNguonCha in ('122a','132') AND sMaTienTrinh = '300' AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fLuyKeThanhToanKHVUDelete,

				(CASE WHEN sMaNguon = '212a' AND sMaNguonCha = '132' AND sMaTienTrinh = '200' AND (iThuHoiTUCheDo = 2 OR iThuHoiTUCheDo IS NULL) AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fThuHoiTamUngNamNayVonNamTruoc,
				(CASE WHEN sMaDich = '212a' AND sMaNguonCha = '132' AND sMaTienTrinh = '300' AND (iThuHoiTUCheDo = 2 OR iThuHoiTUCheDo IS NULL) AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fThuHoiTamUngNamNayVonNamTruocDelete,
				(CASE WHEN sMaNguon = '212a' AND sMaNguonCha = '132' AND sMaTienTrinh = '200' AND iThuHoiTUCheDo = 1 AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fThuHoiTamUngNamTruocVonNamTruoc,
				(CASE WHEN sMaDich = '212a' AND sMaNguonCha = '132' AND sMaTienTrinh = '300' AND iThuHoiTUCheDo = 1 AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fThuHoiTamUngNamTruocVonNamTruocDelete,

				(CASE WHEN sMaNguon = '212a' AND sMaNguonCha = '132' AND sMaTienTrinh = '200' AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fLuyKeThanhToanTrongNam,
				(CASE WHEN sMaDich = '212a' AND sMaNguonCha = '132' AND sMaTienTrinh = '300' AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fLuyKeThanhToanTrongNamDelete,

				(CASE WHEN sMaDich = '122a' AND sMaNguon = '000' AND bKeHoach = 1 AND sMaTienTrinh = '200' THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fThuHoiVonUngNamNay,
				(CASE WHEN sMaNguon = '122a' AND sMaDich = '000' AND bKeHoach = 1 AND sMaTienTrinh = '300' THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fThuHoiVonUngNamNayDelete,
				
				(CASE WHEN sMaDich = '122a'AND bKeHoach = 0 AND sMaTienTrinh = '200' THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fThuHoiVonUngKeHoachNamNay,
				(CASE WHEN sMaNguon = '122a'AND bKeHoach = 0 AND sMaTienTrinh = '300' THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fThuHoiVonUngKeHoachNamNayDelete,

				(CASE WHEN sMaNguon = '122a' AND sMaDich = '000' AND sMaTienTrinh = '200' THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fKHVUNamNay,
				(CASE WHEN sMaDich = '122a' AND sMaNguon = '000' AND sMaTienTrinh = '300' THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fKHVUNamNayDelete,
			
				(CASE WHEN sMaDich = '202' AND sMaNguonCha = '122a' AND sMaTienTrinh = '200' AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fThanhToanKLHTTamUngNamNay,
				(CASE WHEN sMaNguon = '202' AND sMaNguonCha = '122a' AND sMaTienTrinh = '300' AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fThanhToanKLHTTamUngNamNayDelete,
				(CASE WHEN sMaDich = '212a' AND sMaNguonCha = '122a' AND sMaTienTrinh = '200' AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) FThanhToanUngNamNay,
				(CASE WHEN sMaNguon = '212a' AND sMaNguonCha = '122a' AND sMaTienTrinh = '300' AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fThanhToanUngNamNayDelete,

				(CASE WHEN sMaNguon = '212a' AND sMaNguonCha = '122a' AND sMaTienTrinh = '200' AND iThuHoiTUCheDo = 2 AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fThuHoiTamUngNamNay,
				(CASE WHEN sMaDich = '212a' AND sMaNguonCha = '122a' AND sMaTienTrinh = '300' AND iThuHoiTUCheDo = 2 AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fThuHoiTamUngNamNayDelete,
				(CASE WHEN sMaNguon = '212a' AND sMaNguonCha = '122a' AND sMaTienTrinh = '200' AND iThuHoiTUCheDo = 1 AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fThuHoiTamUngNamTruoc,
				(CASE WHEN sMaDich = '212a' AND sMaNguonCha = '122a' AND sMaTienTrinh = '300' AND iThuHoiTUCheDo = 1 AND iID_NguonVonID = @iIdNguonVon THEN SUM(ISNULL(fGiaTri, 0)) ELSE 0 END) fThuHoiTamUngNamTruocDelete
			FROM (SELECT DISTINCT * FROM #tmp) as tmp
			INNER JOIN VDT_TongHop_NguonNSDauTu as dt on tmp.IIDDuAnID = dt.iID_DuAnID AND tmp.iID_LoaiCongTrinh = dt.iID_LoaiCongTrinh
			WHERE iNamKeHoach = @iNamKeHoach 
			GROUP BY tmp.IIDDuAnID, sMaDich, sMaNguon, sMaNguonCha, iID_NguonVonID, sMaTienTrinh, iThuHoiTUCheDo, iLoaiUng, bKeHoach, tmp.iID_LoaiCongTrinh
		) as tbl
		GROUP BY IIDDuAnID, iID_LoaiCongTrinh
	END
	
	SELECT tmp.IIDDuAnID as iID_DuAnID, tmp.sMaDuAn, tmp.sDiaDiem, tmp.sTenDuAn, CAST(2 as int) as iCoQuanThanhToan, tmp.iID_LoaiCongTrinh, lct.SMaLoaiCongTrinh, lct.STenLoaiCongTrinh,tmp.BIsChuyenTiep,
		SUM(ISNULL(nt.fUngTruocChuaThuHoiNamTruoc, 0) - ISNULL(nt.fUngTruocChuaThuHoiNamTruocDelete, 0)) as fUngTruocChuaThuHoiNamTruoc, 
		SUM(ISNULL(nt.fLuyKeThanhToanNamTruoc, 0) - ISNULL(nt.fLuyKeThanhToanNamTruocDelete, 0)) as fLuyKeThanhToanNamTruoc, 
			
		SUM(ISNULL(nn.fThanhToanKLHTNamTruocChuyenSang, 0) - ISNULL(nn.fThanhToanKLHTNamTruocChuyenSangDelete, 0)) as fThanhToanKLHTNamTruocChuyenSang,
		SUM(ISNULL(nn.fThanhToanUngNamTruocChuyenSang, 0) - ISNULL(nn.fThanhToanUngNamTruocChuyenSangDelete, 0)) as fThanhToanUngNamTruocChuyenSang,
			
		SUM(ISNULL(nn.fThuHoiTamUngNamNayVonNamTruoc, 0) - ISNULL(nn.fThuHoiTamUngNamNayVonNamTruocDelete, 0)) as fThuHoiTamUngNamNayVonNamTruoc,
		SUM(ISNULL(nn.fThuHoiTamUngNamTruocVonNamTruoc, 0) - ISNULL(nn.fThuHoiTamUngNamTruocVonNamTruocDelete, 0)) as fThuHoiTamUngNamTruocVonNamTruoc,

		SUM(ISNULL(nn.fThuHoiVonUngNamNay, 0) - ISNULL(nn.fThuHoiVonUngNamNayDelete, 0)) as fThuHoiVonNamNay,
		SUM(ISNULL(nn.fKHVUNamNay, 0) - ISNULL(nn.fKHVUNamNayDelete, 0)) as fKHVUNamNay,
			
		SUM(ISNULL(nn.fThanhToanKLHTTamUngNamNay, 0) - ISNULL(nn.fThanhToanKLHTTamUngNamNayDelete, 0)) as fThanhToanKLHTTamUngNamNay ,
		SUM(ISNULL(nn.fThanhToanUngNamNay, 0) - ISNULL(nn.fThanhToanUngNamNayDelete, 0)) as fThanhToanUngNamNay ,
		SUM(ISNULL(nn.fThuHoiTamUngNamNay, 0) - ISNULL(nn.fThuHoiTamUngNamNayDelete, 0)) as fThuHoiTamUngNamNay ,
		SUM(ISNULL(nn.fThuHoiTamUngNamTruoc, 0) - ISNULL(nn.fThuHoiTamUngNamTruocDelete, 0)) as fThuHoiTamUngNamTruoc ,
		SUM((CASE WHEN @iIdNguonVon = 2 THEN
			(ISNULL(nn.fThuHoiVonUngKeHoachNamNay, 0) - ISNULL(nn.fThuHoiVonUngKeHoachNamNayDelete, 0))
		ELSE
			((ISNULL(nt.fLuyKeTTKLHTChuaThuHoi, 0) + ISNULL(nn.fThuHoiUngTruocNamTruoc, 0) + ISNULL(nn.fLuyKeThanhToanKHVU, 0))
			- (ISNULL(nt.fLuyKeTTKLHTChuaThuHoiDelete, 0) + ISNULL(nn.fThuHoiUngTruocNamTruocDelete, 0) + ISNULL(nn.fLuyKeThanhToanKHVUDelete, 0))) END)) as fGiaTriThuHoiTheoGiaiNganThucTe
	FROM (SELECT DISTINCT * FROM #tmp) as tmp
	LEFT JOIN #tmpNamNayCQTC as nn on tmp.IIDDuAnID = nn.IIDDuAnID AND tmp.iID_LoaiCongTrinh = nn.iID_LoaiCongTrinh
	LEFT JOIN #tmpNamTruocCQTC as nt on tmp.IIDDuAnID = nt.IIDDuAnID AND tmp.iID_LoaiCongTrinh = nt.iID_LoaiCongTrinh
	LEFT JOIN VDT_DM_LoaiCongTrinh as lct on tmp.iID_LoaiCongTrinh = lct.iID_LoaiCongTrinh
	GROUP BY tmp.IIDDuAnID, tmp.sMaDuAn, tmp.sDiaDiem, tmp.sTenDuAn, tmp.iID_LoaiCongTrinh, lct.SMaLoaiCongTrinh, lct.STenLoaiCongTrinh, tmp.BIsChuyenTiep
	UNION ALL
	SELECT tmp.IIDDuAnID, tmp.SMaDuAn, tmp.SDiaDiem, tmp.STenDuAn,  CAST(1 as int) as ICoQuanThanhToan, tmp.iID_LoaiCongTrinh, lct.SMaLoaiCongTrinh, lct.STenLoaiCongTrinh, tmp.BIsChuyenTiep,
		SUM(ISNULL(nt.fUngTruocChuaThuHoiNamTruoc, 0) - ISNULL(nt.fUngTruocChuaThuHoiNamTruocDelete, 0)) as FUngTruocChuaThuHoiNamTruoc, 
		SUM(ISNULL(nt.fLuyKeThanhToanNamTruoc, 0) - ISNULL(nt.fLuyKeThanhToanNamTruocDelete, 0)) as FLuyKeThanhToanNamTruoc, 

		SUM(ISNULL(nn.fThanhToanKLHTNamTruocChuyenSang, 0) - ISNULL(nn.fThanhToanKLHTNamTruocChuyenSangDelete, 0)) as FThanhToanKLHTNamTruocChuyenSang,
		SUM(ISNULL(nn.fThanhToanUngNamTruocChuyenSang, 0) - ISNULL(nn.fThanhToanUngNamTruocChuyenSangDelete, 0)) as FThanhToanUngNamTruocChuyenSang,
			
		SUM(ISNULL(nn.fThuHoiTamUngNamNayVonNamTruoc, 0) - ISNULL(nn.fThuHoiTamUngNamNayVonNamTruocDelete, 0)) as FThuHoiTamUngNamNayVonNamTruoc,
		SUM(ISNULL(nn.fThuHoiTamUngNamTruocVonNamTruoc, 0) - ISNULL(nn.fThuHoiTamUngNamTruocVonNamTruocDelete, 0)) as FThuHoiTamUngNamTruocVonNamTruoc,

		SUM(ISNULL(nn.fThuHoiVonUngNamNay, 0) - ISNULL(nn.fThuHoiVonUngNamNayDelete, 0)) as FThuHoiVonNamNay,
		SUM(ISNULL(nn.fKHVUNamNay, 0) - ISNULL(nn.fKHVUNamNayDelete, 0)) as FKHVUNamNay,

		SUM(ISNULL(nn.fThanhToanKLHTTamUngNamNay, 0) - ISNULL(nn.fThanhToanKLHTTamUngNamNayDelete, 0)) as FThanhToanKLHTTamUngNamNay ,
		SUM(ISNULL(nn.fThanhToanUngNamNay, 0) - ISNULL(nn.fThanhToanUngNamNayDelete, 0)) as FThanhToanUngNamNay ,
		SUM(ISNULL(nn.fThuHoiTamUngNamNay, 0) - ISNULL(nn.fThuHoiTamUngNamNayDelete, 0)) as FThuHoiTamUngNamNay ,
		SUM(ISNULL(nn.fThuHoiTamUngNamTruoc, 0) - ISNULL(nn.fThuHoiTamUngNamTruocDelete, 0)) as FThuHoiTamUngNamTruoc ,
		SUM((CASE WHEN @iIdNguonVon = 2 THEN
			(ISNULL(nn.fThuHoiVonUngKeHoachNamNay, 0) - ISNULL(nn.fThuHoiVonUngKeHoachNamNayDelete, 0))
		ELSE
			((ISNULL(nt.fLuyKeTTKLHTChuaThuHoi, 0) + ISNULL(nn.fThuHoiUngTruocNamTruoc, 0) + ISNULL(nn.fLuyKeThanhToanKHVU, 0))
			- (ISNULL(nt.fLuyKeTTKLHTChuaThuHoiDelete, 0) + ISNULL(nn.fThuHoiUngTruocNamTruocDelete, 0) + ISNULL(nn.fLuyKeThanhToanKHVUDelete, 0))) END)) as FGiaTriThuHoiTheoGiaiNganThucTe
	FROM (SELECT DISTINCT * FROM #tmp) as tmp
	LEFT JOIN #tmpNamNayKB as nn on tmp.IIDDuAnID = nn.IIDDuAnID AND tmp.iID_LoaiCongTrinh = nn.iID_LoaiCongTrinh
	LEFT JOIN #tmpNamTruocKB as nt on tmp.IIDDuAnID = nt.IIDDuAnID AND tmp.iID_LoaiCongTrinh = nt.iID_LoaiCongTrinh
	LEFT JOIN VDT_DM_LoaiCongTrinh as lct on tmp.iID_LoaiCongTrinh = lct.iID_LoaiCongTrinh
	GROUP BY tmp.IIDDuAnID, tmp.sMaDuAn, tmp.sDiaDiem, tmp.sTenDuAn, tmp.iID_LoaiCongTrinh, lct.SMaLoaiCongTrinh, lct.STenLoaiCongTrinh, tmp.BIsChuyenTiep

	DROP TABLE #tmpNamNayCQTC
	DROP TABLE #tmpNamTruocCQTC
	DROP TABLE #tmpNamNayKB
	DROP TABLE #tmpNamTruocKB
	DROP TABLE #tmp
END
