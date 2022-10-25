DECLARE @iNamLamViec int				set @iNamLamViec = 2022
DECLARE @sMaCTMT nvarchar(500)	set @sMaCTMT = null
DECLARE @sMaNguon nvarchar(500)	set @sMaNguon = null
DECLARE @sLoai nvarchar(500)	set @sLoai = null
DECLARE @sKhoan nvarchar(500)	set @sKhoan = null
DECLARE @sNoiDung nvarchar(500)	set @sNoiDung = null
DECLARE @sMaNguonCha nvarchar(500)	set @sMaNguonCha = null
--#DECLARE#--

/*

Lấy danh sách danh mục lục ngân sách để mapping NDC

*/

;WITH orderedTree (iID_Nguon, sMaNguon, sNoiDung, sMaCTMT, iID_NguonCha, sLoai, sKhoan, iLoaiNganSach, sMaNguonCha, iSTT, depth, location)
	AS	(SELECT iID_Nguon, sMaNguon, sNoiDung, sMaCTMT, iID_NguonCha, sLoai, sKhoan, iLoaiNganSach,
						CAST('' AS NVARCHAR(MAX)) as sMaNguonCha, iSTT,
					 0 AS depth, 
					 CAST(sMaNguon AS NVARCHAR(MAX)) AS location
		FROM DM_Nguon 
		WHERE 1=1
				AND iID_NguonCha is null
				AND iTrangThai = 1
				AND iNamLamViec = @iNamLamViec
		UNION ALL
		SELECT child.iID_Nguon, child.sMaNguon, child.sNoiDung, child.sMaCTMT, child.iID_NguonCha, child.sLoai, child.sKhoan, child.iLoaiNganSach,
					 CAST(parent.sMaNguon AS NVARCHAR(MAX)) as sMaNguonCha, child.iSTT,
					 parent.depth + 1 as depth,
           CAST(CONCAT(parent.location, '.', child.sMaNguon) AS NVARCHAR(MAX)) AS location
		FROM DM_Nguon child
		INNER JOIN orderedTree parent ON child.iID_NguonCha = parent.iID_Nguon
		WHERE child.iTrangThai = 1
				AND iNamLamViec = @iNamLamViec)
SELECT tree.*, 
case WHEN tree.iLoaiNganSach = 0 then N'Chi nhà nước thường xuyên' 
WHEN tree.iLoaiNganSach = 1 then N'Chi thường xuyên quốc phòng' 
end as sLoaiNganSach,
 tbl_count_child.numChild
FROM orderedTree tree 
LEFT JOIN (
	select iID_NguonCha, count(iID_NguonCha) as numChild
	from DM_Nguon
	where iTrangThai = 1 and iNamLamViec = @iNamLamViec and iID_NguonCha is not null
	GROUP BY iID_NguonCha
) tbl_count_child on tree.iID_Nguon = tbl_count_child.iID_NguonCha
where 1 = 1
and (@sMaCTMT is null or tree.sMaCTMT like @sMaCTMT)
and (@sMaNguon is null or tree.sMaNguon like @sMaNguon)
and (@sLoai is null or tree.sLoai like @sLoai)
and (@sKhoan is null or tree.sKhoan like @sKhoan)
and (@sNoiDung is null or tree.sNoiDung like @sNoiDung)
and (@sMaNguonCha is null or tree.sMaNguonCha like @sMaNguonCha)
order by
    cast('/' + replace(tree.iSTT, '.', '/') + '/' as hierarchyid), tree.sMaNguon