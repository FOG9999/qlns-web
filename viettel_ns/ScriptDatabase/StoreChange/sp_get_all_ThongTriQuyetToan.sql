USE [CTC_DB_TEST]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_all_ThongTriQuyetToan]    Script Date: 28/11/2022 9:02:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[sp_get_all_ThongTriQuyetToan]
	@iID_DonViID UNIQUEIDENTIFIER = NULL,
	@iID_ChuongTrinhID UNIQUEIDENTIFIER = NULL,
	@sSoThongTri NVARCHAR(MAX) = null,
	@dNgayLap DATETIME = NULL,
	@iNamThucHien INT = NULL,
	@iLoaiThongTri INT = NULL,
	@iLoaiNoiDungChi INT = NULL,
	@SkipCount INT = 0,
    @MaxResultCount INT = 20,
    @TotalItems INT OUTPUT
AS
BEGIN
	DECLARE @MinDate DATETIME = NULL;
	DECLARE @MaxDate DATETIME = NULL;
	IF (@dNgayLap IS NOT NULL)
	BEGIN
		SET @MinDate = CONVERT(DATE, @dNgayLap);
		SET @MaxDate = DATEADD(DAY, 1, @MinDate);
	END
	
	SELECT
		TTQT.*,
		CONCAT(RTRIM(LTRIM(DV.iID_MaDonVi)), IIF(DV.sTen IS NULL OR RTRIM(LTRIM(DV.sTen)) = '', '', CONCAT(' - ', RTRIM(LTRIM(DV.sTen))))) AS sTenDonVi,
		KHCT_NVC.sTenNhiemVuChi
	INTO #Main
	FROM NH_QT_ThongTriQuyetToan AS TTQT
	LEFT JOIN NS_DonVi AS DV ON TTQT.iID_DonViID = DV.iID_Ma AND TTQT.iID_MaDonVi = DV.iID_MaDonVi
	LEFT JOIN NH_KHChiTietBQP_NhiemVuChi AS KHCT_NVC ON TTQT.iID_KHTT_NhiemVuChiID = KHCT_NVC.ID
	WHERE (@iID_DonViID IS NULL OR @iID_DonViID = '00000000-0000-0000-0000-000000000000' OR DV.iID_Ma = @iID_DonViID)
		AND (@iID_ChuongTrinhID IS NULL OR @iID_ChuongTrinhID = '00000000-0000-0000-0000-000000000000' OR KHCT_NVC.ID = @iID_ChuongTrinhID)
		AND (@sSoThongTri IS NULL OR TTQT.sSoThongTri LIKE ('%' + @sSoThongTri + '%'))
		AND (@dNgayLap IS NULL OR (TTQT.dNgayLap >= @MinDate AND TTQT.dNgayLap < @MaxDate))
		AND (@iNamThucHien IS NULL OR TTQT.iNamThongTri = @iNamThucHien)
		AND (@iLoaiThongTri IS NULL OR TTQT.iLoaiThongTri = @iLoaiThongTri)
		AND (@iLoaiNoiDungChi IS NULL OR TTQT.iLoaiNoiDungChi = @iLoaiNoiDungChi)

	SELECT @TotalItems = COUNT(1) FROM #Main;

	SELECT * FROM #Main
    ORDER BY #Main.dNgayTao DESC
    OFFSET @SkipCount ROWS
    FETCH NEXT @MaxResultCount ROWS ONLY
END