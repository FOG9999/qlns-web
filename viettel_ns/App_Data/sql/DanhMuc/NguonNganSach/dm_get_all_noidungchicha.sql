--#DECLARE#--

/*

Lấy danh sách nội dung chi BQP

*/

WITH tree (iID_NoiDungChi)
AS (Select iID_NoiDungChi from DM_NoiDungChi where iID_NoiDungChi = @iID_NoiDungChi AND iNamLamViec = @iNamLamViec
		UNION ALL
		Select child.iID_NoiDungChi
		From DM_NoiDungChi child
		INNER JOIN tree parent on child.iID_Parent = parent.iID_NoiDungChi
		Where child.iNamLamViec = @iNamLamViec
		)
Select * into #tmpID 
from tree;

SELECT * 
FROM DM_NoiDungChi 
WHERE (@iID_NoiDungChi is null OR iID_Parent is null OR iID_Parent != @iID_NoiDungChi)
AND (@iID_NoiDungChi is null OR iID_NoiDungChi not in(select * from #tmpID))
AND bPublic = 1
AND iNamLamViec = @iNamLamViec
ORDER BY sMaNoiDungChi;

drop table #tmpID 