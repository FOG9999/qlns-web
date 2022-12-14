USE [CTC_DB_TEST]
GO
/****** Object:  StoredProcedure [dbo].[proc_get_all_nh_da_tthopdong_paging_demo]    Script Date: 02/11/2022 2:29:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[proc_get_all_nh_da_tthopdong_paging_demo] 
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
		AND (@sTenHopDong IS NULL OR hd.sTenHopDong LIKE CONCAT(N'%',@sTenHopDong,N'%'))
		AND (@iDonVi IS NULL OR hd.iID_DonViID = @iDonVi)
		AND (@iChuongTrinh IS NULL OR hd.iID_KHCTBQP_ChuongTrinhID = @iChuongTrinh)
		AND (@iDuAn IS NULL OR hd.iID_DuAnID = @iDuAn)
		AND (@sSoHopDong IS NULL OR hd.sSoHopDong LIKE CONCAT(N'%',@sSoHopDong,N'%'))
		AND (@iLoaiHopDong IS NULL OR hd.iID_LoaiHopDongID = @iLoaiHopDong)
	ORDER BY hd.dNgayTao DESC

	SET @iToTalItem =  (SELECT COUNT(*) from #TMP);

	SELECT tmp.iID_KHCTBQP_ChuongTrinhID, tmp.sTenChuongTrinh, SUM(tmp.fGiaTriEUR) as fGiaTriEUR, SUM(tmp.fGiaTriUSD) as fGiaTriUSD, SUM(tmp.fGiaTriVND) as fGiaTriVND, SUM(tmp.fGiaTriNgoaiTeKhac) as fGiaTriNgoaiTeKhac
	FROM #TMP tmp
	where tmp.bIsActive = 1
	GROUP BY tmp.iID_KHCTBQP_ChuongTrinhID, tmp.sTenChuongTrinh
	ORDER BY sTenChuongTrinh
	OFFSET (@ItemsPerPage * (@CurrentPage-1)) ROWS
	FETCH NEXT @ItemsPerPage ROWS ONLY

	DROP TABLE #TMP

END