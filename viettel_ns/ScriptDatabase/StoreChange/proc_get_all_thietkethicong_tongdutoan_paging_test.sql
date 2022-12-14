
GO
/****** Object:  StoredProcedure [dbo].[proc_get_all_thietkethicong_tongdutoan_paging_test]    Script Date: 27/10/2022 4:59:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROC [dbo].[proc_get_all_thietkethicong_tongdutoan_paging_test]
	@sUserName NVARCHAR(MAX),
	@iNamLamViec INT,
	@sTenDuAn NVARCHAR(MAX),
	@sTenDuToan NVARCHAR(MAX),
	@sSoQuyetDinh NVARCHAR(100),
	@dPheDuyetTuNgay DATETIME,
	@dPheDuyetDenNgay DATETIME,
	@sMaDonViQL NVARCHAR(MAX),
	@bIsTongDuToan BIT,
	@CurrentPage INT,
	@ItemsPerPage INT,
	@iToTalItem INT OUTPUT
AS
BEGIN
WITH SoLieuDieuChinh AS 
	 (
		SELECT 
			dt.iID_DuToanID, dt.iID_ParentId, dt.sSoQuyetDinh
		FROM 
			VDT_DA_DuToan dt 
		WHERE 
			dt.iID_ParentId is not null

		UNION ALL

		SELECT dt.iID_DuToanID, dt.iID_ParentId, dt.sSoQuyetDinh
		FROM VDT_DA_DuToan dt JOIN SoLieuDieuChinh dtpr ON dt.iID_ParentId = dtpr.iID_DuToanID 
		WHERE dt.iID_ParentId is not null
	  ),SoLanDieuChinh AS (
		   SELECT 
			sdc.iID_ParentId, COUNT(sdc.iID_ParentId) AS iSoLanDieuChinh
		  FROM 
			SoLieuDieuChinh sdc
		  GROUP BY sdc.iID_ParentId
	  )
SELECT 
	dt.*, da.sMaDuAn, 
	('('+ da.sMaDuAn +')' + ' - ' + da.sTenDuAn) as sTenDuAn,
	dv.sTen AS sTenDonViQL, (isnull(sdc.iSoLanDieuChinh, 0)) as SoLanDieuChinh INTO #tmp
FROM
	VDT_DA_DuToan dt
LEFT JOIN 
	VDT_DA_DuAn da 
	ON dt.iID_DuAnID = da.iID_DuAnID
LEFT JOIN 
	NS_DonVi dv 
	ON da.iID_DonViQuanLyID = dv.iID_Ma
LEFT JOIN
	SoLanDieuChinh sdc
	ON sdc.iID_ParentID = dt.iID_ParentID
WHERE
	da.iID_DonViQuanLyID IN
	(
	SELECT
		b.iID_Ma 
	FROM
		NS_NguoiDung_DonVi a,
		ns_donvi b 
	WHERE
		a.iID_MaDonVi = b.iID_MaDonVi 
		--AND a.iNamLamViec = @iNamLamViec 
		--AND b.iNamLamViec_DonVi = @iNamLamViec 
		AND a.sMaNguoiDung = @sUserName 
		AND a.iTrangThai = 1 
		AND b.iTrangThai = 1 )
	AND ( ISNULL( @sTenDuAn, '' ) = '' OR da.sTenDuAn LIKE CONCAT ( N'%',@sTenDuAn, N'%' ) )
	AND ( ISNULL( @sTenDuToan, '' ) = '' OR dt.sTenDuToan LIKE CONCAT ( N'%',@sTenDuToan, N'%' ) )
	AND ( ISNULL( @sSoQuyetDinh, '' ) = '' OR dt.sSoQuyetDinh LIKE CONCAT ( N'%',@sSoQuyetDinh, N'%' ) )
	AND ( @dPheDuyetTuNgay IS NULL OR ( dt.dNgayQuyetDinh >= @dPheDuyetTuNgay ) ) 
	AND ( @dPheDuyetDenNgay IS NULL OR ( dt.dNgayQuyetDinh <= @dPheDuyetDenNgay ) )
	AND ( ISNULL( @sMaDonViQL, '' ) = '' OR dv.iID_Ma = @sMaDonViQL )
	--AND (dt.bLaTongDuToan = @bIsTongDuToan)
	--AND (dt.bActive = 1)
	--AND (da.bIsDeleted = 1)

SELECT iID_DuAnID, SUM(fTongDuToanPheDuyet) AS fTong INTO #tmpTong
FROM #tmp
GROUP BY iID_DuAnID

SELECT iID_DuAnID, COUNT(*) AS iSoLanChinhSua INTO #tmpSoLanChinhSua
FROM #tmp
WHERE bIsGoc <> 1
GROUP BY iID_DuAnID


SELECT  tbl.*, tong.fTong AS dGiaTriDuToanSauDieuChinh, tbl.SoLanDieuChinh AS iTongSoDieuChinh
FROM #tmp AS tbl
LEFT JOIN #tmpTong AS tong ON tbl.iID_DuAnID = tong.iID_DuAnID
LEFT JOIN #tmpSoLanChinhSua AS tblChinhSua ON tbl.iID_DuAnID = tblChinhSua.iID_DuAnID
ORDER BY tbl.dDateCreate

OFFSET (@ItemsPerPage * (@CurrentPage-1)) ROWS
FETCH NEXT @ItemsPerPage ROWS ONLY

SET @iToTalItem = (SELECT 
						COUNT(dt.iID_DuToanID)
				   FROM 
						VDT_DA_DuToan dt
				   LEFT JOIN 
						VDT_DA_DuAn da 
						ON dt.iID_DuAnID = da.iID_DuAnID
				   LEFT JOIN 
						NS_DonVi dv 
						ON da.iID_DonViQuanLyID = dv.iID_Ma
				   WHERE
						da.iID_DonViQuanLyID IN
						(
						SELECT
							b.iID_Ma 
						FROM
							NS_NguoiDung_DonVi a,
							ns_donvi b 
						WHERE
							a.iID_MaDonVi = b.iID_MaDonVi 
							--AND a.iNamLamViec = @iNamLamViec 
							--AND b.iNamLamViec_DonVi = @iNamLamViec 
							AND a.sMaNguoiDung = @sUserName 
							AND a.iTrangThai = 1 
							AND b.iTrangThai = 1 )
						AND ( ISNULL( @sTenDuAn, '' ) = '' OR da.sTenDuAn LIKE CONCAT ( N'%',@sTenDuAn, N'%' ) )
						AND ( ISNULL( @sSoQuyetDinh, '' ) = '' OR dt.sSoQuyetDinh LIKE CONCAT ( N'%',@sSoQuyetDinh, N'%' ) )
						AND ( @dPheDuyetTuNgay IS NULL OR ( dt.dNgayQuyetDinh >= @dPheDuyetTuNgay ) ) 
						AND ( @dPheDuyetDenNgay IS NULL OR ( dt.dNgayQuyetDinh <= @dPheDuyetDenNgay ) )
						AND ( ISNULL( @sMaDonViQL, '' ) = '' OR dv.iID_Ma = @sMaDonViQL )
						--AND (dt.bLaTongDuToan = @bIsTongDuToan)
						--AND (dt.bActive = 1)
						--AND (da.bIsDeleted = 1)
					)

DROP TABLE #tmp
DROP TABLE #tmpTong
DROP TABLE #tmpSoLanChinhSua

END