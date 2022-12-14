USE [CTC_DB_TEST]
GO
/****** Object:  StoredProcedure [dbo].[proc_get_all_kehoachvonnamdexuat_paging2]    Script Date: 20/10/2022 3:46:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROC [dbo].[proc_get_all_kehoachvonnamdexuat_paging2]
@sSoQuyetDinh nvarchar(100),
@dNgayQuyetDinhFrom datetime,
@dNgayQuyetDinhTo datetime,
@iNamKeHoach int,
@iID_NguonVonID int,
@iID_DonViQuanLyID uniqueidentifier,
@isTongHop int,
@CurrentPage int,
@ItemsPerPage int,
@iToTalItem int OUTPUT
AS
BEGIN

SELECT khvnam.*, donVi.sTen AS sTenDonVi, nv.sTen AS sTenNguonVon INTO #TMP
FROM (
	SELECT 
		vkhvnam.*, 
		CASE WHEN (vkhvnam.sTongHop is null and vkhvnam.iID_TongHopParent is null) THEN 1 ELSE 2 END isTongHop
		-- INTO #TMP_kh5namdx
	FROM VDT_KHV_KeHoachVonNam_DeXuat vkhvnam
	)  khvnam 
LEFT JOIN NS_DonVi donVi ON khvnam.iID_DonViQuanLyID = donVi.iID_Ma
LEFT JOIN NS_NguonNganSach nv ON khvnam.iID_NguonVonID = nv.iID_MaNguonNganSach

WHERE 
	(ISNULL(@sSoQuyetDinh, '') = '' OR khvnam.sSoQuyetDinh LIKE CONCAT(N'%',@sSoQuyetDinh,N'%'))
	AND (@dNgayQuyetDinhFrom IS NULL OR khvnam.dNgayQuyetDinh >= @dNgayQuyetDinhFrom)
	AND (@dNgayQuyetDinhTo IS NULL OR khvnam.dNgayQuyetDinh <= @dNgayQuyetDinhTo)
	AND (@iNamKeHoach IS NULL OR khvnam.iNamKeHoach = @iNamKeHoach)
	AND (@iID_NguonVonID IS NULL OR khvnam.iID_NguonVonID = @iID_NguonVonID)
	AND (@iID_DonViQuanLyID IS NULL OR @iID_DonViQuanLyID = '00000000-0000-0000-0000-000000000000' OR khvnam.iID_DonViQuanLyID = @iID_DonViQuanLyID)
	AND (khvnam.isTongHop = @isTongHop OR @isTongHop is null)
	--AND bActive = 1
ORDER BY sSoQuyetDinh

SET @iToTalItem =  (SELECT COUNT(*) from #TMP);

WITH temp(iID_KeHoachVonNamDeXuatID, parent_name, current_name , row_num)
AS
(
	SELECT iID_KeHoachVonNamDeXuatID, sSoQuyetDinh, sSoQuyetDinh, CAST(0 as int) as row_num
	FROM VDT_KHV_KeHoachVonNam_DeXuat as  khvnam
	WHERE iID_ParentID IS NULL
	UNION ALL
	SELECT khvnam.iID_KeHoachVonNamDeXuatID, pr.current_name , khvnam.sSoQuyetDinh ,(pr.row_num + 1)
	FROM VDT_KHV_KeHoachVonNam_DeXuat as khvnam, temp as pr
	WHERE khvnam.iID_ParentID = pr.iID_KeHoachVonNamDeXuatID
)
SELECT * INTO #tmpDieuChinh
FROM temp

	
SELECT tmp.*, dc.row_num as iSoLanDieuDieuChinh, (CASE WHEN row_num = 0 THEN '' ELSE dc.parent_name END) as sDieuChinhTuSKH into #final
FROM #TMP as tmp
INNER JOIN #tmpDieuChinh as dc on tmp.iID_KeHoachVonNamDeXuatID = dc.iID_KeHoachVonNamDeXuatID

select *, DATEADD(ms, 2, dDateCreate) as date
from #final final 
where final.sTongHop is not null

union 

select child.*,
DATEADD(ms, 2, parent.dDateCreate) as date
from #final child inner join #final parent on child.iID_TongHopParent = parent.iID_KeHoachVonNamDeXuatID
where child.iID_TongHopParent in (select fn.iID_KeHoachVonNamDeXuatID from #final fn where fn.sTongHop is not null)
order by date desc

OFFSET (@ItemsPerPage * (@CurrentPage-1)) ROWS
FETCH NEXT @ItemsPerPage ROWS ONLY

DROP TABLE #TMP
DROP TABLE #tmpDieuChinh
END



