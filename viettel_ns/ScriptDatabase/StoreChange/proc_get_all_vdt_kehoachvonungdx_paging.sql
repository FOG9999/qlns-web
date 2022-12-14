
GO
/****** Object:  StoredProcedure [dbo].[proc_get_all_vdt_kehoachvonungdx_paging]    Script Date: 01/11/2022 12:00:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROC [dbo].[proc_get_all_vdt_kehoachvonungdx_paging]
@sSoQuyetDinh nvarchar(100),
@dNgayQuyetDinhFrom datetime,
@dNgayQuyetDinhTo datetime,
@iID_MaDonViQuanLyID nvarchar(100),
@iNamKeHoach int,
@iIdNguonVon int,
@CurrentPage int,
@ItemsPerPage int,
@iToTalItem int OUTPUT
AS
BEGIN
	SELECT khvu.Id, SUM(ISNULL(dt.fGiaTriDeNghi, 0)) as fTienKeHoachVUDX  INTO #tmp
	FROM VDT_KHV_KeHoachVonUng_DX khvu
	left JOIN VDT_KHV_KeHoachVonUng_DX_ChiTiet as dt on khvu.Id = dt.iID_KeHoachUngID
	LEFT JOIN NS_DonVi dv on khvu.iID_DonViQuanLyID = dv.iID_Ma
	WHERE (ISNULL(@sSoQuyetDinh,'') = '' OR sSoDeNghi like CONCAT(N'%', @sSoQuyetDinh, N'%')) 
		AND (@dNgayQuyetDinhFrom IS NULL OR dNgayDeNghi >= @dNgayQuyetDinhFrom)
		AND (@dNgayQuyetDinhTo IS NULL OR dNgayDeNghi <= @dNgayQuyetDinhTo)
		AND (@iID_MaDonViQuanLyID IS NULL OR dv.iID_MaDonVi = @iID_MaDonViQuanLyID)
		AND (@iNamKeHoach IS NULL OR iNamKeHoach = @iNamKeHoach)
		AND (@iIdNguonVon IS NULL OR iID_NguonVonID = @iIdNguonVon)
	GROUP BY khvu.Id

SET @iToTalItem = (SELECT COUNT(Id) FROM #tmp);

	--TemDieuChinh--
	WITH temp(Id,Parent_name,current_name,sSoLanDieuChinh)
	AS
	(
		SELECT Id,sSoDeNghi,sSoDeNghi,CAST(0 as int) as sSoLanDieuChinh
		FROM VDT_KHV_KeHoachVonUng_DX
		WHERE iID_ParentID IS NULL
		UNION ALL
		SELECT dx.Id,pr.current_name,dx.sSoDeNghi, (pr.sSoLanDieuChinh + 1) as sSoLanDieuChinh
		FROM VDT_KHV_KeHoachVonUng_DX dx, temp as pr
		WHERE dx.iID_ParentID = pr.Id
	)
	 SELECT * INTO #tmpDieuChinh
	 FROM temp			
			--select end--								
	SELECT tmp.Id as iID_KeHoachUngID, khvu.*, khvu.sSoDeNghi, khvu.dNgayDeNghi, khvu.fGiaTriUng,
					dv.iID_Ma as iID_DonViQuanLyID, dv.sTen as sTenDonVi, nv.sTen as sTenNguonVon, ISNULL(tmp.fTienKeHoachVUDX, 0) as fTienKeHoachVUDX, 
					(CASE WHEN sSoLanDieuChinh = 0 THEN '' ELSE tmpDieuChinh.parent_name END) as sDieuChinhTuSKH, tmpDieuChinh.sSoLanDieuChinh as iSoLanDieuChinh
	FROM VDT_KHV_KeHoachVonUng_DX khvu
	INNER JOIN #tmp as tmp on khvu.Id = tmp.Id
	LEFT JOIN NS_DonVi as dv on khvu.iID_DonViQuanLyID = dv.iID_Ma
	LEFT JOIN NS_NguonNganSach as nv on khvu.iID_NguonVonID = nv.iID_MaNguonNganSach
	INNER JOIN #tmpDieuChinh as tmpDieuChinh on tmpDieuChinh.Id = khvu.id 
	ORDER BY khvu.dDateCreate desc

	OFFSET (@ItemsPerPage * (@CurrentPage-1)) ROWS
	FETCH NEXT @ItemsPerPage ROWS ONLY

	DROP TABLE #tmp
	DROP TABLE #tmpDieuChinh
END


