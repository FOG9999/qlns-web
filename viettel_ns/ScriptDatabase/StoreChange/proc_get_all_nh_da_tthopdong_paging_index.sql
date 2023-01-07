-- CREATE PROCEDURE [dbo].[proc_get_all_nh_da_tthopdong_paging_index] 
-- @dNgayHopDong datetime,
-- @sTenHopDong nvarchar(300),
-- @iDonVi uniqueidentifier,
-- @iChuongTrinh uniqueidentifier,
-- @iDuAn uniqueidentifier,
-- @sSoHopDong varchar(100),
-- @iLoaiHopDong uniqueidentifier,
-- @CurrentPage int,
-- @ItemsPerPage int,
-- @iToTalItem int OUTPUT
-- AS
-- BEGIN	
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

	-- SELECT tmp.iID_KHCTBQP_ChuongTrinhID, tmp.sTenChuongTrinh, SUM(tmp.fGiaTriEUR) AS fGiaTriEUR, SUM(tmp.fGiaTriUSD) AS fGiaTriUSD, SUM(tmp.fGiaTriVND) AS fGiaTriVND, SUM(tmp.fGiaTriNgoaiTeKhac) AS fGiaTriNgoaiTeKhac
	-- INTO #Result
	-- FROM #TMP tmp
	-- WHERE tmp.bIsActive = 1
	-- GROUP BY tmp.iID_KHCTBQP_ChuongTrinhID, tmp.sTenChuongTrinh
	
	-- SET @iToTalItem =  (SELECT COUNT(*) from #Result);
	
	-- SELECT * FROM #Result
	-- ORDER BY sTenChuongTrinh
	-- OFFSET (@ItemsPerPage * (@CurrentPage-1)) ROWS
	-- FETCH NEXT @ItemsPerPage ROWS ONLY

	-- DROP TABLE #TMP
	-- DROP TABLE #Result
-- END

ALTER PROCEDURE [dbo].[proc_get_all_nh_da_tthopdong_paging_index] 
	@dNgayHopDong datetime,
	@sTenHopDong nvarchar(300),
	@iDonVi uniqueidentifier,
	@iChuongTrinh uniqueidentifier,
	@iDuAn uniqueidentifier,
	@sSoHopDong varchar(100),
	@iLoaiHopDong uniqueidentifier,
	@CurrentPage int,
	@ItemsPerPage int,
	@iToTalItem int OUTPUT
AS
BEGIN	
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
		AND (@sTenHopDong IS NULL OR @sTenHopDong = '' OR hd.sTenHopDong LIKE CONCAT(N'%',@sTenHopDong,N'%'))
		AND (@iDonVi IS NULL OR hd.iID_DonViID = @iDonVi)
		AND (@iChuongTrinh IS NULL OR hd.iID_KHCTBQP_ChuongTrinhID = @iChuongTrinh)
		AND (@iDuAn IS NULL OR hd.iID_DuAnID = @iDuAn)
		AND (@sSoHopDong IS NULL OR @sSoHopDong = '' OR hd.sSoHopDong LIKE CONCAT(N'%',@sSoHopDong,N'%'))
		AND (@iLoaiHopDong IS NULL OR hd.iID_LoaiHopDongID = @iLoaiHopDong)
	ORDER BY hd.dNgayTao DESC

	SELECT tmp.iID_KHCTBQP_ChuongTrinhID,
		MAX(tmp.dNgayTao) AS createDate,
		SUM(CASE WHEN tmp.bIsActive = 1 THEN tmp.fGiaTriEUR ELSE 0 END) AS fGiaTriEUR,
		SUM(CASE WHEN tmp.bIsActive = 1 THEN tmp.fGiaTriUSD ELSE 0 END) AS fGiaTriUSD,
		SUM(CASE WHEN tmp.bIsActive = 1 THEN tmp.fGiaTriVND ELSE 0 END) AS fGiaTriVND,
		SUM(CASE WHEN tmp.bIsActive = 1 THEN tmp.fGiaTriNgoaiTeKhac ELSE 0 END) AS fGiaTriNgoaiTeKhac
	INTO #Result
	FROM #TMP tmp
	GROUP BY tmp.iID_KHCTBQP_ChuongTrinhID;
	
	SET @iToTalItem = (SELECT COUNT(*) from #Result);
	
	SELECT rs.*, khct.sTenNhiemVuChi AS sTenChuongTrinh FROM #Result rs
	LEFT JOIN NH_KHChiTietBQP_NhiemVuChi khct ON rs.iID_KHCTBQP_ChuongTrinhID = khct.ID
	ORDER BY rs.createDate DESC
	OFFSET (@ItemsPerPage * (@CurrentPage-1)) ROWS
	FETCH NEXT @ItemsPerPage ROWS ONLY

	DROP TABLE #TMP
	DROP TABLE #Result
END