
-- [dbo].[sp_vdt_get_kehoachvon_capphatthanhtoan]
		
--DECLARE	@DuAnId varchar(max) SET @DuAnId = null
--DECLARE @NguonVonId int set @NguonVonId = 1
--DECLARE @dNgayDeNghi date set @dNgayDeNghi = null
--DECLARE @NamKeHoach int set @NamKeHoach = 2022
--DECLARE @iCoQuanThanhToan int set @iCoQuanThanhToan = 1
--DECLARE @iIdPheDuyet uniqueidentifier set @iIdPheDuyet = null
DECLARE	@DuAnId varchar(max);
DECLARE @NguonVonId int;
DECLARE @dNgayDeNghi date; 
DECLARE @NamKeHoach int;
DECLARE @iCoQuanThanhToan int;
DECLARE @iIdPheDuyet uniqueidentifier;

--#DECLARE#--

CREATE TABLE #tmp(
		Id uniqueidentifier, 
		sSoQuyetDinh nvarchar(100), 
		dNgayQuyetDinh datetime,
		iNamKeHoach int,
		iID_NguonVonID int,
		iPhanLoai int,
		fLenhChi float,
		fTongGiaTri float,
		sTenLoai nvarchar(600),
		sMaNguonCha nvarchar(100)
	)

	-- Ke hoach von nam
	
	SELECT Id INTO #tmpChungTuVonNam
	FROM 
	(
		SELECT iID_KeHoachVonNam_DuocDuyetID as Id, ROW_NUMBER() OVER(PARTITION BY iID_KeHoachVonNamGocID ORDER BY dNgayQuyetDinh DESC, dDateCreate DESC) as rn
		FROM VDT_KHV_KeHoachVonNam_DuocDuyet 
		WHERE CAST(dNgayQuyetDinh as DATE) <= CAST(@dNgayDeNghi as DATE)
			AND iNamKeHoach = @NamKeHoach
			AND iID_NguonVonID = @NguonVonId
	) as tbl 
	WHERE tbl.rn = 1

	INSERT INTO #tmp(Id, sSoQuyetDinh, dNgayQuyetDinh, iNamKeHoach, iID_NguonVonID, sMaNguonCha, sTenLoai, iPhanLoai)
	SELECT tbl.Id, dt.sSoQuyetDinh, dt.dNgayQuyetDinh, dt.iNamKeHoach, dt.iID_NguonVonID,
		dt.sMaNguon, N'Kế hoạch vốn năm', 1
	FROM #tmpChungTuVonNam as tbl
	INNER JOIN VDT_TongHop_NguonNSDauTu as dt on tbl.Id = dt.iID_ChungTu AND dt.iID_DuAnID = @DuAnId AND dt.iID_NguonVonID = @NguonVonId AND dt.bIsLog = 0
										AND (dt.sMaNguon in ('101', '102')  AND dt.sMaDich = '000' AND dt.sMaTienTrinh = '200')
	GROUP BY tbl.Id,dt.sSoQuyetDinh, dt.dNgayQuyetDinh, dt.iNamKeHoach, dt.iID_NguonVonID,dt.sMaNguon

	INSERT INTO #tmp(Id, sSoQuyetDinh, dNgayQuyetDinh, iNamKeHoach, iID_NguonVonID, sMaNguonCha, sTenLoai, iPhanLoai)
	SELECT tbl.iID_ChungTu, tbl.sSoQuyetDinh, tbl.dNgayQuyetDinh, tbl.iNamKeHoach, tbl.iID_NguonVonID, sMaNguon,
		CASE WHEN sMaNguon COLLATE DATABASE_DEFAULT in ('121a', '122a') THEN N'Kế hoạch vốn ứng'
			WHEN sMaNguon COLLATE DATABASE_DEFAULT in ('111', '112') THEN N'Kế hoạch năm trước chuyển sang'
			WHEN sMaNguon COLLATE DATABASE_DEFAULT in ('131', '132') THEN N'Kế hoạch ứng trước năm trước chuyển sang' END, 
		CASE WHEN sMaNguon COLLATE DATABASE_DEFAULT in ('121a', '122a') THEN 2
			WHEN sMaNguon COLLATE DATABASE_DEFAULT in ('111', '112') THEN 3
			WHEN sMaNguon COLLATE DATABASE_DEFAULT in ('131', '132') THEN 4 END
	FROM VDT_TongHop_NguonNSDauTu as tbl
	WHERE ((tbl.sMaNguon COLLATE DATABASE_DEFAULT in ('121a', '122a') AND sMaTienTrinh COLLATE DATABASE_DEFAULT = '200') OR (tbl.sMaNguon COLLATE DATABASE_DEFAULT in ('111', '112', '131', '132') AND sMaTienTrinh COLLATE DATABASE_DEFAULT = '100'))
		AND sMaDich COLLATE DATABASE_DEFAULT = '000'
		AND bIsLog = 0
		AND tbl.iID_DuAnID = @DuAnId AND tbl.iID_NguonVonID = @NguonVonId 
		AND CAST(tbl.dNgayQuyetDinh as DATE) <= CAST(@dNgayDeNghi as DATE)
		AND tbl.iNamKeHoach = @NamKeHoach
	GROUP BY tbl.iID_ChungTu, tbl.sSoQuyetDinh, tbl.dNgayQuyetDinh, tbl.iNamKeHoach, tbl.iID_NguonVonID, sMaNguon


	-- Luy ke da thanh toan
	SELECT tmp.id as iIdChungTu, tmp.sMaNguonCha, 
		SUM(ISNULL(CASE WHEN dt.sMaNguon = '000' THEN ISNULL(dt.fGiaTri, 0) ELSE 0 END, 0)) as fThanhToan,
		SUM(ISNULL(CASE WHEN dt.sMaDich = '000' THEN ISNULL(dt.fGiaTri, 0) ELSE 0 END, 0)) as fThuHoi INTO #tmpThanhToan
	FROM #tmp as tmp
	INNER JOIN VDT_TongHop_NguonNSDauTu as dt on dt.iId_MaNguonCha = tmp.id
												AND tmp.sMaNguonCha COLLATE DATABASE_DEFAULT = dt.sMaNguonCha COLLATE DATABASE_DEFAULT
												AND sMaTienTrinh COLLATE DATABASE_DEFAULT = '200'
												AND dt.iID_DuAnID = @DuAnId
												 AND bIsLog = 0
	WHERE dt.iID_ChungTu <> @iIdPheDuyet 
	GROUP BY tmp.id, tmp.sMaNguonCha


	CREATE TABLE #tmpMaNguon(sMaNguon nvarchar(100))
	IF(@iCoQuanThanhToan = 1)
	BEGIN
		INSERT INTO #tmpMaNguon(sMaNguon)
		VALUES('101'), ('121a'), ('111'), ('131')		
		UPDATE #tmp 
		SET fTongGiaTri =
		-- Kế hoạch vốn năm
		case when sMaNguonCha = '101' then (SELECT top 1 fCapPhatTaiKhoBac from VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet khvn_dd where khvn_dd.iID_KeHoachVonNam_DuocDuyetID=#tmp.Id)
		-- Kế hoạch vốn ứng
		else (SELECT top 1 fGiaTriUng from VDT_KHV_KeHoachVonUng_ChiTiet khvu_dd where khvu_dd.Id=#tmp.Id) end
		-- Kế hoạch năm trước chuyển sang (111) và kế hoạch vốn ứng năm trước chuyển sang (131) chưa clear
		WHERE Id in (SELECT Id FROM #tmp)
	END
	ELSE
	BEGIN
		INSERT INTO #tmpMaNguon(sMaNguon)
		VALUES('102'), ('122a'), ('112'), ('132')
		UPDATE #tmp 
		SET fTongGiaTri =
		-- Kế hoạch vốn năm
		case when sMaNguonCha = '102' then (SELECT top 1 fCapPhatBangLenhChi from VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet khvn_dd where khvn_dd.iID_KeHoachVonNam_DuocDuyetID=#tmp.Id)
		-- Kế hoạch vốn ứng
		else (SELECT top 1 fGiaTriUng from VDT_KHV_KeHoachVonUng_ChiTiet khvu_dd where khvu_dd.Id=#tmp.Id) end		
		-- Kế hoạch năm trước chuyển sang (112) và kế hoạch vốn ứng năm trước chuyển sang (132) chưa clear
		WHERE Id in (SELECT Id FROM #tmpChungTuVonNam) 
	END



	SELECT tmp.Id, 
		tmp.sSoQuyetDinh, 
		tmp.dNgayQuyetDinh, 
		tmp.iNamKeHoach, 
		tmp.iID_NguonVonID, 
		ISNULL(tmp.fTongGiaTri, 0) as fTongGiaTri,
		(ISNULL(dt.fThanhToan, 0) - ISNULL(dt.fThuHoi, 0)) as fLuyKeThanhToan,
		tmp.sMaNguonCha, 
		tmp.sTenLoai, 
		tmp.iPhanLoai,
		NULL as iID_DonViQuanLyID,
		NULL as iID_MaDonViQuanLy
	FROM #tmp as tmp
	INNER JOIN #tmpMaNguon as tbl on tmp.sMaNguonCha = tbl.sMaNguon
	LEFT JOIN #tmpThanhToan as dt on tmp.Id = dt.iIdChungTu 
		AND dt.sMaNguonCha COLLATE DATABASE_DEFAULT = tmp.sMaNguonCha COLLATE DATABASE_DEFAULT
	--WHERE (ISNULL(tmp.FTongGiaTri, 0) - ISNULL(dt.fThanhToan, 0) + ISNULL(dt.fThuHoi, 0)) != 0

	DROP TABLE #tmpMaNguon
	DROP TABLE #tmpThanhToan
	DROP TABLE #tmp
	DROP TABLE #tmpChungTuVonNam
	--drop table #tmpChungTuVonNam
