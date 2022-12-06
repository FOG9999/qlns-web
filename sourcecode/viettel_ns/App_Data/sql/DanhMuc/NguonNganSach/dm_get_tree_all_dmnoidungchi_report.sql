DECLARE @iNamLamViec int				set @iNamLamViec = 2022
DECLARE @iLoaiNganSach int set @iLoaiNganSach = 0
DECLARE @iNguonNganSach uniqueidentifier set @iNguonNganSach = null
--#DECLARE#--
/*
Lấy danh sách danh mục nội dung chi
*/

CREATE TABLE #tmpIDNguon(iId_Nguon uniqueidentifier)

IF(@iNguonNganSach IS NOT NULL)
BEGIN
;WITH cteParent as(
		SELECT iId_Nguon, iID_NguonCha
		FROM DM_Nguon WHERE iID_Nguon = @iNguonNganSach AND bPublic = 1 AND iNamLamViec = @iNamLamViec
		UNION ALL
		SELECT cd.iID_Nguon, cd.iID_NguonCha
		FROM DM_Nguon as cd, cteParent as pr
		WHERE cd.iID_Nguon = pr.iID_NguonCha AND cd.bPublic = 1 AND cd.iNamLamViec = @iNamLamViec
		
	), cteChild as(
		SELECT iId_Nguon, iID_NguonCha
		FROM DM_Nguon WHERE iID_Nguon = @iNguonNganSach AND bPublic = 1 AND iNamLamViec = @iNamLamViec
		UNION ALL
		SELECT cd.iID_Nguon, cd.iID_NguonCha
		FROM DM_Nguon as cd, cteChild as pr
		WHERE cd.iID_NguonCha = pr.iID_Nguon AND cd.bPublic = 1 AND cd.iNamLamViec = @iNamLamViec
		
	)
	INSERT INTO #tmpIDNguon(iId_Nguon)
	SELECT DISTINCT * FROM (
	SELECT iId_Nguon FROM cteChild
	union all 
	SELECT iId_Nguon FROM cteParent
	) as a;
END
ELSE
BEGIN
	IF(@iLoaiNganSach is not null)
	BEGIN
		;WITH cteParent as(
		SELECT iId_Nguon, iID_NguonCha
		FROM DM_Nguon WHERE iLoaiNganSach = @iLoaiNganSach AND bPublic = 1 AND iNamLamViec = @iNamLamViec
		UNION ALL
		SELECT cd.iID_Nguon, cd.iID_NguonCha
		FROM DM_Nguon as cd, cteParent as pr
		WHERE cd.iID_Nguon = pr.iID_NguonCha AND cd.bPublic = 1 AND cd.iNamLamViec = @iNamLamViec
		
	), cteChild as(
		SELECT iId_Nguon, iID_NguonCha
		FROM DM_Nguon WHERE iLoaiNganSach = @iLoaiNganSach AND bPublic = 1 AND iNamLamViec = @iNamLamViec
		UNION ALL
		SELECT cd.iID_Nguon, cd.iID_NguonCha
		FROM DM_Nguon as cd, cteChild as pr
		WHERE cd.iID_NguonCha = pr.iID_Nguon AND cd.bPublic = 1 AND cd.iNamLamViec = @iNamLamViec
		
	)
	INSERT INTO #tmpIDNguon(iId_Nguon)
	SELECT DISTINCT * FROM (
	SELECT iId_Nguon FROM cteChild
	union all 
	SELECT iId_Nguon FROM cteParent
	) as a;
	END;
END

; WITH ndcParent AS (
SELECT ndc.iID_NoiDungChi, ndc.iID_Parent, ndc.sMaNoiDungChi, ndc.sTenNoiDungChi, ndc.sGhiChu
FROM DM_NoiDungChi ndc 
WHERE ndc.iNamLamViec = @iNamLamViec AND ndc.iTrangThai = 1
AND ((@iLoaiNganSach is null AND @iNguonNganSach is null) or ndc.iID_Nguon IN (select * from #tmpIDNguon))
		
		UNION ALL
		
		SELECT cd.iID_NoiDungChi, cd.iID_Parent, cd.sMaNoiDungChi, cd.sTenNoiDungChi, cd.sGhiChu
		FROM DM_NoiDungChi as cd, ndcParent as pr
		WHERE cd.iID_NoiDungChi = pr.iID_Parent AND cd.iTrangThai = 1 AND cd.iNamLamViec = @iNamLamViec
)

select DISTINCT tblNDC.* into #tmp1 from ndcParent tblNDC
inner JOIN orderedTree_view_dmnoidungchi view_dmndc on tblNDC.iID_NoiDungChi = view_dmndc.iID_NoiDungChi
WHERE (@iLoaiNganSach is null or (@iLoaiNganSach = 1 AND view_dmndc.depth < 2) OR (@iLoaiNganSach = 0 AND view_dmndc.depth < 3))

SELECT tblNDC.iID_NoiDungChi, tblNDC.iID_Parent, tblNDC.sMaNoiDungChi, tblNDC.sTenNoiDungChi, tblNDC.sGhiChu, CASE WHEN f.iID_Parent IS NULL THEN 0 ELSE 1 END AS bLaHangCha, view_dmndc.depth, view_dmndc.location, view_dmndc.rootParent
FROM #tmp1 tblNDC
LEFT JOIN (SELECT iID_Parent FROM #tmp1 GROUP BY iID_Parent) f ON f.iID_Parent = tblNDC.iID_NoiDungChi
INNER JOIN orderedTree_view_dmnoidungchi view_dmndc on tblNDC.iID_NoiDungChi = view_dmndc.iID_NoiDungChi
WHERE (@iLoaiNganSach is null or (@iLoaiNganSach = 1 AND view_dmndc.depth < 2) OR (@iLoaiNganSach = 0 AND view_dmndc.depth < 3))
order by cast('/' + replace(view_dmndc.iSTT, '.', '/') + '/' as hierarchyid)

-- SELECT DISTINCT tblNDC.*, view_dmndc.sMaNoiDungChi, view_dmndc.sMaNoiDungChiCha, view_dmndc.sTenNoiDungChi, view_dmndc.sGhiChu, view_dmndc.depth, view_dmndc.location, view_dmndc.bLaHangCha, view_dmndc.rootParent
-- FROM ndcParent tblNDC
-- INNER JOIN orderedTree_view_dmnoidungchi view_dmndc on tblNDC.iID_NoiDungChi = view_dmndc.iID_NoiDungChi
-- WHERE (@iLoaiNganSach is null or (@iLoaiNganSach = 1 AND view_dmndc.depth < 2) OR (@iLoaiNganSach = 0 AND view_dmndc.depth < 3))
-- order by view_dmndc.location

-- drop table #tmp1
-- drop table #tmp
-- drop table #tmpIDNguon


