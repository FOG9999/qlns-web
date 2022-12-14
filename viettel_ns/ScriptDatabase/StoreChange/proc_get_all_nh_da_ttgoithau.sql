USE [CTC_DB_TEST]
GO
/****** Object:  StoredProcedure [dbo].[proc_get_all_nh_da_ttgoithau_paging]    Script Date: 02/11/2022 3:59:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[proc_get_all_nh_da_ttgoithau] 
@sTenGoiThau nvarchar(300),
@iDonVi uniqueidentifier,
@iChuongTrinh uniqueidentifier,
@iDuAn uniqueidentifier,
@iLoai int,
@iThoiGianThucHien int

AS
BEGIN
	WITH SoLieuDieuChinh AS 
	(
		SELECT gt.ID, gt.iID_GoiThauGocID, gt.iID_ParentAdjustID, gt.sTenGoiThau
		FROM NH_DA_GoiThau gt
		WHERE gt.iID_ParentAdjustID is not null
		UNION ALL
		SELECT gt.ID, gt.iID_GoiThauGocID, gt.iID_ParentAdjustID, gt.sTenGoiThau
		FROM NH_DA_GoiThau gt JOIN SoLieuDieuChinh sldc ON gt.iID_ParentAdjustID = sldc.ID 
		WHERE gt.iID_ParentAdjustID is not null
	)
	SELECT sdc.iID_ParentAdjustID, COUNT(sdc.iID_ParentAdjustID) AS iSoLanDieuChinh
	INTO #SoLanDieuChinh
	FROM SoLieuDieuChinh sdc
	GROUP BY sdc.iID_ParentAdjustID
	
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

	SELECT tmp.*,
		('(' + cast(isnull(sdc.iSoLanDieuChinh, 0) as nvarchar(MAX)) + ')') as sSoLanDieuChinh,
		CASE WHEN tmp.iID_ParentAdjustID is null THEN ''
			ELSE (SELECT TOP 1 gt.sTenGoiThau FROM NH_DA_GoiThau gt WHERE gt.ID = tmp.iID_ParentAdjustID)
		END sDieuChinhTu
	FROM #TMP tmp
	LEFT JOIN #SoLanDieuChinh sdc ON tmp.iID_ParentAdjustID = sdc.iID_ParentAdjustID
	ORDER BY tmp.dNgayTao DESC

	DROP TABLE #TMP
	DROP TABLE #SoLanDieuChinh
END