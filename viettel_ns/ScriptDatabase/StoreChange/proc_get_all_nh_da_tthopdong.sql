USE [CTC_DB_TEST]
GO
/****** Object:  StoredProcedure [dbo].[proc_get_all_nh_da_tthopdong]    Script Date: 01/11/2022 8:28:12 AM ******/
-- SET ANSI_NULLS ON
-- GO
-- SET QUOTED_IDENTIFIER ON
-- GO
-- Create PROCEDURE [dbo].[proc_get_all_nh_da_tthopdong] 
-- @dNgayHopDong datetime,
-- @sTenHopDong nvarchar(300),
-- @iDonVi uniqueidentifier,
-- @iChuongTrinh uniqueidentifier,
-- @iDuAn uniqueidentifier,
-- @sSoHopDong varchar(100),
-- @iLoaiHopDong uniqueidentifier
-- AS
-- BEGIN
	-- WITH SoLieuDieuChinh AS 
	-- (
		-- SELECT hd.ID, hd.iID_HopDongGocID, hd.iID_ParentAdjustID, hd.sSoHopDong
		-- FROM NH_DA_HopDong hd
		-- WHERE hd.iID_ParentAdjustID is not null
		-- UNION ALL
		-- SELECT hd.ID, hd.iID_HopDongGocID, hd.iID_ParentAdjustID, hd.sSoHopDong
		-- FROM NH_DA_HopDong hd JOIN SoLieuDieuChinh sldc ON hd.iID_ParentAdjustID = sldc.ID 
		-- WHERE hd.iID_ParentAdjustID is not null
	-- )
	-- SELECT sdc.iID_ParentAdjustID, COUNT(sdc.iID_ParentAdjustID) AS iSoLanDieuChinh
	-- INTO #SoLanDieuChinh
	-- FROM SoLieuDieuChinh sdc
	-- GROUP BY sdc.iID_ParentAdjustID
	
	-- SELECT hd.*
		-- ,dv.sTen AS sTenDonVi
		-- ,da.sTenDuAn
		-- ,khct.sTenNhiemVuChi AS sTenChuongTrinh
		-- ,dmhd.sTenLoaiHopDong
		-- ,tg.sMaTienTeGoc
	-- INTO #TMP
	-- FROM NH_DA_HopDong hd
	-- LEFT JOIN NS_DonVi dv ON hd.iID_MaDonVi = dv.iID_MaDonVi AND hd.iID_DonViID = dv.iID_Ma AND dv.iTrangThai = 1
	-- LEFT JOIN NH_DA_DuAn da ON hd.iID_DuAnID = da.ID
	-- LEFT JOIN NH_KHChiTietBQP_NhiemVuChi khct ON hd.iID_KHCTBQP_ChuongTrinhID = khct.ID
	-- LEFT JOIN NH_DM_LoaiHopDong dmhd ON hd.iID_LoaiHopDongID = dmhd.ID
	-- LEFT JOIN NH_DM_TiGia tg ON hd.iID_TiGiaID = tg.ID
	-- WHERE 
		-- (@dNgayHopDong IS NULL OR hd.dNgayHopDong = @dNgayHopDong)
		-- AND (@sTenHopDong IS NULL OR hd.sTenHopDong LIKE CONCAT(N'%',@sTenHopDong,N'%'))
		-- AND (@iDonVi IS NULL OR hd.iID_DonViID = @iDonVi)
		-- AND (@iChuongTrinh IS NULL OR hd.iID_KHCTBQP_ChuongTrinhID = @iChuongTrinh)
		-- AND (@iDuAn IS NULL OR hd.iID_DuAnID = @iDuAn)
		-- AND (@sSoHopDong IS NULL OR hd.sSoHopDong LIKE CONCAT(N'%',@sSoHopDong,N'%'))
		-- AND (@iLoaiHopDong IS NULL OR hd.iID_LoaiHopDongID = @iLoaiHopDong)
	-- ORDER BY hd.dNgayTao DESC


	-- SELECT tmp.*,
		-- ('(' + cast(isnull(sdc.iSoLanDieuChinh, 0) as nvarchar(MAX)) + ')') as sSoLanDieuChinh,
		-- CASE WHEN tmp.iID_ParentAdjustID is null THEN ''
			-- ELSE (SELECT TOP 1 hd.sSoHopDong FROM NH_DA_HopDong hd WHERE hd.ID = tmp.iID_ParentAdjustID)
		-- END sDieuChinhTu
	-- FROM #TMP tmp
	-- LEFT JOIN #SoLanDieuChinh sdc ON tmp.iID_ParentAdjustID = sdc.iID_ParentAdjustID
	-- ORDER BY tmp.dNgayTao DESC

	-- DROP TABLE #TMP
	-- DROP TABLE #SoLanDieuChinh
-- END

ALTER PROCEDURE [dbo].[proc_get_all_nh_da_tthopdong] 
@dNgayHopDong datetime,
@sTenHopDong nvarchar(300),
@iDonVi uniqueidentifier,
@iChuongTrinh uniqueidentifier,
@iDuAn uniqueidentifier,
@sSoHopDong varchar(100),
@iLoaiHopDong uniqueidentifier

AS
BEGIN
	WITH SoLieuDieuChinh AS 
	(
		SELECT hd.ID, hd.iID_HopDongGocID, hd.iID_ParentAdjustID, hd.sSoHopDong
		FROM NH_DA_HopDong hd
		WHERE hd.iID_ParentAdjustID is not null
		UNION ALL
		SELECT hd.ID, hd.iID_HopDongGocID, hd.iID_ParentAdjustID, hd.sSoHopDong
		FROM NH_DA_HopDong hd JOIN SoLieuDieuChinh sldc ON hd.iID_ParentAdjustID = sldc.ID 
		WHERE hd.iID_ParentAdjustID is not null
	)
	SELECT sdc.iID_ParentAdjustID, COUNT(sdc.iID_ParentAdjustID) AS iSoLanDieuChinh
	INTO #SoLanDieuChinh
	FROM SoLieuDieuChinh sdc
	GROUP BY sdc.iID_ParentAdjustID
	
	SELECT hd.*
		,dv.sTen AS sTenDonVi
		,da.sTenDuAn
		,khct.sTenNhiemVuChi AS sTenChuongTrinh
		,dmhd.sTenLoaiHopDong
		,tg.sMaTienTeGoc
	INTO #TMP
	FROM NH_DA_HopDong hd
	LEFT JOIN NS_DonVi dv ON hd.iID_MaDonVi = dv.iID_MaDonVi AND hd.iID_DonViID = dv.iID_Ma AND dv.iTrangThai = 1
	LEFT JOIN NH_DA_DuAn da ON hd.iID_DuAnID = da.ID
	LEFT JOIN NH_KHChiTietBQP_NhiemVuChi khct ON hd.iID_KHCTBQP_ChuongTrinhID = khct.ID
	LEFT JOIN NH_DM_LoaiHopDong dmhd ON hd.iID_LoaiHopDongID = dmhd.ID
	LEFT JOIN NH_DM_TiGia tg ON hd.iID_TiGiaID = tg.ID
	WHERE 
		(@dNgayHopDong IS NULL OR hd.dNgayHopDong = @dNgayHopDong)
		AND (@sTenHopDong IS NULL OR @sTenHopDong='' OR hd.sTenHopDong LIKE CONCAT(N'%',@sTenHopDong,N'%'))
		AND (@iDonVi IS NULL OR hd.iID_DonViID = @iDonVi)
		AND (@iChuongTrinh IS NULL OR hd.iID_KHCTBQP_ChuongTrinhID = @iChuongTrinh)
		AND (@iDuAn IS NULL OR hd.iID_DuAnID = @iDuAn)
		AND (@sSoHopDong IS NULL OR @sSoHopDong='' OR hd.sSoHopDong LIKE CONCAT(N'%',@sSoHopDong,N'%'))
		AND (@iLoaiHopDong IS NULL OR hd.iID_LoaiHopDongID = @iLoaiHopDong)
	ORDER BY hd.dNgayTao DESC


	SELECT tmp.*,
		('(' + cast(isnull(sdc.iSoLanDieuChinh, 0) as nvarchar(MAX)) + ')') as sSoLanDieuChinh,
		CASE WHEN tmp.iID_ParentAdjustID is null THEN ''
			ELSE (SELECT TOP 1 hd.sSoHopDong FROM NH_DA_HopDong hd WHERE hd.ID = tmp.iID_ParentAdjustID)
		END sDieuChinhTu
	FROM #TMP tmp
	LEFT JOIN #SoLanDieuChinh sdc ON tmp.iID_ParentAdjustID = sdc.iID_ParentAdjustID
	ORDER BY tmp.dNgayTao DESC


	DROP TABLE #TMP
	DROP TABLE #SoLanDieuChinh
END