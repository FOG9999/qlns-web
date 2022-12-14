USE [CTC_DB_TEST]
GO
/****** Object:  StoredProcedure [dbo].[proc_get_all_nh_da_ttgoithau_paging_demo]    Script Date: 02/11/2022 4:26:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create PROCEDURE [dbo].[proc_get_all_nh_da_ttgoithau_paging_demo] 
@sTenGoiThau nvarchar(300),
@iDonVi uniqueidentifier,
@iChuongTrinh uniqueidentifier,
@iDuAn uniqueidentifier,
@iLoai int,
@iThoiGianThucHien int,
@CurrentPage int,
@ItemsPerPage int,
@iToTalItem int OUTPUT

AS
BEGIN	
	SELECT gt.*
		,dv.sTen AS sTenDonVi
		,da.sTenDuAn
		,khct.sTenNhiemVuChi AS sTenChuongTrinh
	INTO #TMP
	FROM NH_DA_GoiThau gt
	LEFT JOIN NS_DonVi dv ON gt.iID_MaDonVi = dv.iID_MaDonVi AND gt.iID_DonViID = dv.iID_Ma AND dv.iTrangThai = 1
	LEFT JOIN NH_DA_DuAn da ON gt.iID_DuAnID = da.ID
	LEFT JOIN NH_KHChiTietBQP_NhiemVuChi khct ON gt.iID_KHCTBQP_ChuongTrinhID = khct.ID
	WHERE 
		(@sTenGoiThau IS NULL OR gt.sTenGoiThau LIKE CONCAT(N'%',@sTenGoiThau,N'%'))
		AND (@iDonVi IS NULL OR gt.iID_DonViID = @iDonVi)
		AND (@iChuongTrinh IS NULL OR gt.iID_KHCTBQP_ChuongTrinhID = @iChuongTrinh)
		AND (@iDuAn IS NULL OR gt.iID_DuAnID = @iDuAn)
		AND (@iLoai = 0 OR @iLoai IS NULL OR gt.iPhanLoai = @iLoai)
		AND (@iThoiGianThucHien IS NULL OR gt.iThoiGianThucHien = @iThoiGianThucHien)
	ORDER BY gt.dNgayTao DESC

	SET @iToTalItem =  (SELECT COUNT(*) from #TMP);

	SELECT tmp.iID_KHCTBQP_ChuongTrinhID, tmp.sTenChuongTrinh, SUM(tmp.fGiaTriEUR) as fGiaTriEUR, SUM(tmp.fGiaTriUSD) as fGiaTriUSD, SUM(tmp.fGiaTriVND) as fGiaTriVND, SUM(tmp.fGiaTriNgoaiTeKhac) as fGiaTriNgoaiTeKhac
	FROM #TMP tmp
	WHERE tmp.bIsActive = 1
	GROUP BY tmp.iID_KHCTBQP_ChuongTrinhID, tmp.sTenChuongTrinh
	ORDER BY sTenChuongTrinh
	OFFSET (@ItemsPerPage * (@CurrentPage-1)) ROWS
	FETCH NEXT @ItemsPerPage ROWS ONLY

	DROP TABLE #TMP

END