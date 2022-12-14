USE [CTC_DB_TEST]
GO
/****** Object:  StoredProcedure [dbo].[proc_get_all_vdt_kehoachvonung_paging]    Script Date: 01/11/2022 9:03:27 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROC [dbo].[proc_get_all_vdt_kehoachvonung_paging]
@sSoQuyetDinh nvarchar(100),
@dNgayQuyetDinhFrom datetime,
@dNgayQuyetDinhTo datetime,
@iID_MaDonViQuanLyID uniqueidentifier,
@iNamKeHoach int,
@iIdNguonVon int,
@iNamLamViec int,
@CurrentPage int,
@ItemsPerPage int,
@iToTalItem int OUTPUT
AS
BEGIN
-- Tinh so lan dieu chinh
	WITH temp(Id,parent_name, current_name,sSoLanDieuChinh)
	AS
	(
		SELECT Id,sSoQuyetDinh,sSoQuyetDinh ,CAST(0 as int) as sSoLanDieuChinh
		FROM VDT_KHV_KeHoachVonUng khvu
		WHERE iID_ParentID IS NULL
		UNION ALL
		SELECT khvu.Id, pr.current_name , khvu.sSoQuyetDinh ,(pr.sSoLanDieuChinh +1)
		FROM VDT_KHV_KeHoachVonUng as khvu, temp as pr
		where khvu.iID_ParentID = pr.Id	
	)
		SELECT * INTO #tmpDieuChinh
		FROM temp
		option (maxrecursion 0);

SET @iToTalItem =  (SELECT COUNT(khvu.Id)
										FROM VDT_KHV_KeHoachVonUng khvu
										LEFT JOIN NS_DonVi dv on khvu.iID_DonViQuanLyID = dv.iID_Ma
										WHERE (ISNULL(@sSoQuyetDinh,'') = '' OR sSoQuyetDinh like CONCAT(N'%', @sSoQuyetDinh, N'%')) 
											AND (@dNgayQuyetDinhFrom IS NULL OR dNgayQuyetDinh >= @dNgayQuyetDinhFrom)
											AND (@dNgayQuyetDinhTo IS NULL OR dNgayQuyetDinh <= @dNgayQuyetDinhTo)
											AND (@iID_MaDonViQuanLyID IS NULL OR iID_DonViQuanLyID = @iID_MaDonViQuanLyID)
											AND (@iNamKeHoach IS NULL OR iNamKeHoach = @iNamKeHoach)
											AND (@iIdNguonVon IS NULL OR @iIdNguonVon = @iIdNguonVon))

SELECT khvu.Id as iID_KeHoachUngID, khvu.sSoQuyetDinh, khvu.dNgayQuyetDinh, khvu.iNamKeHoach, khvu.fGiaTriUng,
				dv.iID_Ma, dv.sTen as sTenDonViQL, nv.sTen as sNguonVon,(CASE WHEN sSoLanDieuChinh = 0 THEN '' ELSE tmp.parent_name END) as sDieuChinhTu, sSoLanDieuChinh, khvu.bActive,khvu.bIsGoc
FROM VDT_KHV_KeHoachVonUng khvu
LEFT JOIN NS_DonVi dv on khvu.iID_MaDonViQuanLy = dv.iID_MaDonVi AND dv.iNamLamViec_DonVi = @iNamLamViec
INNER JOIN NS_NguonNganSach nv ON nv.iID_MaNguonNganSach = khvu.iID_NguonVonID
INNER JOIN #tmpDieuChinh tmp ON tmp.Id = khvu.Id
WHERE (ISNULL(@sSoQuyetDinh,'') = '' OR sSoQuyetDinh like CONCAT(N'%', @sSoQuyetDinh, N'%')) 
	AND (@dNgayQuyetDinhFrom IS NULL OR dNgayQuyetDinh >= @dNgayQuyetDinhFrom)
	AND (@dNgayQuyetDinhTo IS NULL OR dNgayQuyetDinh <= @dNgayQuyetDinhTo)
	AND (@iID_MaDonViQuanLyID IS NULL OR iID_DonViQuanLyID = @iID_MaDonViQuanLyID)
	AND (@iNamKeHoach IS NULL OR iNamKeHoach = @iNamKeHoach)
	AND (@iIdNguonVon IS NULL OR @iIdNguonVon = @iIdNguonVon)
ORDER BY khvu.sSoQuyetDinh
OFFSET (@ItemsPerPage * (@CurrentPage-1)) ROWS
FETCH NEXT @ItemsPerPage ROWS ONLY
END

DROP TABLE #tmpDieuChinh
