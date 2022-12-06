ALTER PROCEDURE [dbo].[proc_get_all_nh_da_ttgoithau_paging_index] 
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

	SELECT tmp.iID_KHCTBQP_ChuongTrinhID, tmp.sTenChuongTrinh, SUM(tmp.fGiaTriEUR) AS fGiaTriEUR, SUM(tmp.fGiaTriUSD) AS fGiaTriUSD, SUM(tmp.fGiaTriVND) AS fGiaTriVND, SUM(tmp.fGiaTriNgoaiTeKhac) AS fGiaTriNgoaiTeKhac
	INTO #Result
	FROM #TMP tmp
	WHERE tmp.bIsActive = 1
	GROUP BY tmp.iID_KHCTBQP_ChuongTrinhID, tmp.sTenChuongTrinh
	
	SET @iToTalItem =  (SELECT COUNT(*) from #Result);
	
	SELECT * FROM #Result
	ORDER BY sTenChuongTrinh
	OFFSET (@ItemsPerPage * (@CurrentPage-1)) ROWS
	FETCH NEXT @ItemsPerPage ROWS ONLY

	DROP TABLE #TMP
	DROP TABLE #Result
END