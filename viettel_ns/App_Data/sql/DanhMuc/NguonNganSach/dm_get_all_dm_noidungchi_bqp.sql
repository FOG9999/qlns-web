DECLARE @iNamLamViec int				set @iNamLamViec = 2020
DECLARE @sMaNoiDungChi nvarchar(500)	set @sMaNoiDungChi = null
DECLARE @sTenNoiDungChi nvarchar(500)	set @sTenNoiDungChi = null
DECLARE @sMaNoiDungChiCha nvarchar(500)	set @sMaNoiDungChiCha = null
DECLARE @sGhiChu nvarchar(500)	set @sGhiChu = null
DECLARE @sMaNguon nvarchar(50) set @sMaNguon = null
--#DECLARE#--

/*

Lấy danh sách danh mục lục ngân sách để mapping NDC

*/

;WITH orderedTree (iID_NoiDungChi, sMaNoiDungChi, sTenNoiDungChi, sGhiChu, iID_Parent, sMaNoiDungChiCha, iID_Nguon, iSTT, depth, location)
AS (SELECT iID_NoiDungChi, sMaNoiDungChi, sTenNoiDungChi, sGhiChu, iID_Parent,
						CAST('' AS NVARCHAR(MAX)) as sMaNoiDungChiCha,
						iID_Nguon,
						iSTT,
					 0 AS depth, 
					 CAST(sMaNoiDungChi AS NVARCHAR(MAX)) AS location
	  FROM DM_NoiDungChi 
		WHERE 1=1
				AND iID_Parent is null
				AND iTrangThai = 1
				AND iNamLamViec = @iNamLamViec
		UNION ALL
		SELECT child.iID_NoiDungChi, child.sMaNoiDungChi, child.sTenNoiDungChi, child.sGhiChu, child.iID_Parent,
					 CAST(parent.sMaNoiDungChi AS NVARCHAR(MAX)) as sMaNoiDungChiCha,
					 child.iID_Nguon,
					 child.iSTT,
					 parent.depth + 1 as depth,
           CAST(CONCAT(parent.location, '.', child.sMaNoiDungChi) AS NVARCHAR(MAX)) AS location
    FROM DM_NoiDungChi child
    INNER JOIN orderedTree parent ON child.iID_Parent = parent.iID_NoiDungChi
		WHERE 1=1 
				AND child.iTrangThai = 1
				AND child.iNamLamViec = @iNamLamViec
		)
SELECT tree.* , dmn.sMaNguon, tbl_count_child.numChild
FROM orderedTree tree
LEFT JOIN (
	select iID_Parent, count(iID_Parent) as numChild
	from DM_NoiDungChi
	where iTrangThai = 1 and iNamLamViec = @iNamLamViec and iID_Parent is not null
	GROUP BY iID_Parent
) tbl_count_child on tree.iID_NoiDungChi = tbl_count_child.iID_Parent
LEFT JOIN DM_Nguon dmn on tree.iID_Nguon = dmn.iID_Nguon and dmn.iNamLamViec = @iNamLamViec
where 1 = 1
and (@sMaNoiDungChi is null or tree.sMaNoiDungChi like @sMaNoiDungChi)
and (@sTenNoiDungChi is null or tree.sTenNoiDungChi like @sTenNoiDungChi)
and (@sMaNoiDungChiCha is null or tree.sMaNoiDungChiCha like @sMaNoiDungChiCha)
and (@sGhiChu is null or tree.sGhiChu like @sGhiChu)
and (@sMaNguon is null or dmn.sMaNguon like @sMaNguon)
ORDER BY
    cast('/' + replace(tree.iSTT, '.', '/') + '/' as hierarchyid)

