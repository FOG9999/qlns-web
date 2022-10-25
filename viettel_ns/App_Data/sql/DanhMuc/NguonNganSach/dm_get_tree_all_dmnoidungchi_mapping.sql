DECLARE @iNamLamViec int				set @iNamLamViec = 2022
DECLARE @sTenNoiDungChi nvarchar(500)	set @sTenNoiDungChi = ''
DECLARE @sMaNoiDungChi nvarchar(500)	set @sMaNoiDungChi = ''
DECLARE @sGhiChu nvarchar(500)	set @sGhiChu = ''
DECLARE @iMap int set @iMap = null
--#DECLARE#--

/*

Lấy danh sách danh mục nội dung chi

*/

;WITH orderedTree (iID_NoiDungChi, sMaNoiDungChi, sTenNoiDungChi, sGhiChu, iID_Parent, depth, location)
AS (SELECT iID_NoiDungChi, sMaNoiDungChi, sTenNoiDungChi, sGhiChu, iID_Parent,
					 0 AS depth, 
					 CAST(sMaNoiDungChi AS NVARCHAR(MAX)) AS location
	  FROM DM_NoiDungChi 
		WHERE 1=1
				AND iID_Parent is null
				AND iTrangThai = 1
				And iNamLamViec = @iNamLamViec
		UNION ALL
		SELECT child.iID_NoiDungChi, child.sMaNoiDungChi, child.sTenNoiDungChi, child.sGhiChu, child.iID_Parent,
					 parent.depth + 1 as depth,
           CAST(CONCAT(parent.location, '.', child.sMaNoiDungChi) AS NVARCHAR(MAX)) AS location
    FROM DM_NoiDungChi child
    INNER JOIN orderedTree parent ON child.iID_Parent = parent.iID_NoiDungChi
		WHERE child.iTrangThai = 1
		And iNamLamViec = @iNamLamViec
		)
		
SELECT dmndc.*, 
		CAST((case when tbl1.iID_NoiDungChi is not null and tblCheck.iID_NoiDungChi is not null then N'Đã map'
				when tbl1.iID_NoiDungChi is not null and tblCheck.iID_NoiDungChi is null then N'Chưa map' 
				else N'' end) as nvarchar(50)) as sMap ,
		CAST((case when tbl1.iID_NoiDungChi is not null then '0' else '1' end) as bit) as bLaHangCha 
		INTO #tmp
FROM orderedTree dmndc
LEFT JOIN (
  select iID_NoiDungChi from DM_NoiDungChi where iID_NoiDungChi not in
  (
    select distinct iID_Parent from DM_NoiDungChi where iTrangThai = 1 and iID_Parent is not null
  ) and iTrangThai = 1 And iNamLamViec = @iNamLamViec
) tbl1 on tbl1.iID_NoiDungChi = dmndc.iID_NoiDungChi
LEFT JOIN (select distinct iID_NoiDungChi from NNS_NDChi_MLNhuCau where NamLamViec = @iNamLamViec) tblCheck on dmndc.iID_NoiDungChi = tblCheck.iID_NoiDungChi
WHERE 1 = 1
AND (ISNULL(@sTenNoiDungChi, '') = '' OR dmndc.sTenNoiDungChi LIKE CONCAT(N'%',@sTenNoiDungChi,N'%'))
AND (ISNULL(@sMaNoiDungChi, '') = '' OR dmndc.sMaNoiDungChi LIKE CONCAT(N'%',@sMaNoiDungChi,N'%'))
AND (ISNULL(@sGhiChu, '') = '' OR dmndc.sGhiChu LIKE CONCAT(N'%',@sGhiChu,N'%'))
AND (@iMap is null or @iMap = 0 OR (@iMap = 1 AND (tbl1.iID_NoiDungChi is null or (tbl1.iID_NoiDungChi is not null and tblCheck.iID_NoiDungChi is not null))) OR (@iMap = 2 AND (tbl1.iID_NoiDungChi is null or (tbl1.iID_NoiDungChi is not null and tblCheck.iID_NoiDungChi is null))))
ORDER BY location

;WITH cte(iID_Parent) AS(
    SELECT iID_Parent FROM #tmp where iID_Parent is not null AND bLaHangCha = 0
    UNION ALL
    SELECT i.iID_Parent FROM cte c
    INNER JOIN #tmp i ON c.iID_Parent = i.iID_NoiDungChi
    WHERE i.iID_Parent is not null
)


SELECT iID_Parent, count(*) as countChild into #countChild
FROM cte
GROUP BY iID_Parent
ORDER BY iID_Parent

select #tmp.*, #countChild.countChild FROM #tmp
LEFT JOIN #countChild ON #tmp.iID_NoiDungChi = #countChild.iID_Parent
where (@iMap is null or @iMap = 0 OR ((@iMap = 1 OR @iMap = 2) AND (#tmp.bLaHangCha = 0 OR (#tmp.bLaHangCha = 1 and countChild is not null ))))
ORDER BY #tmp.location

drop table #countChild
drop table #tmp