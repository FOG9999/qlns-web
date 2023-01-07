--DECLARE @sTenLoaiCongTrinh nvarchar(max)

SELECT  DISTINCT pr.iID_LoaiCongTrinh, pr.iID_Parent, pr.sMaLoaiCongTrinh, pr.sTenLoaiCongTrinh, pr.sTenVietTat, pr.iThuTu, pr.sMoTa, child.sTenLoaiCongTrinh as sTenLoaiCha, (CASE WHEN cd.iID_Parent IS NULL THEN 0 ELSE 1 END) as bHasChild INTO #tmp
FROM VDT_DM_LoaiCongTrinh as pr
LEFT JOIN VDT_DM_LoaiCongTrinh as cd on pr.iID_LoaiCongTrinh = cd.iID_Parent AND cd.bActive = 1
LEFT JOIN VDT_DM_LoaiCongTrinh as child on pr.iID_Parent = child.iID_LoaiCongTrinh AND child.bActive = 1
WHERE pr.bActive = 1;

WITH cte(iID_LoaiCongTrinh, iID_Parent, sMaLoaiCongTrinh, sTenLoaiCongTrinh, sTenVietTat, iThuTu, sMoTa, sTenLoaiCha, sLevelTab,bHasChild, location)
AS
(
  SELECT iID_LoaiCongTrinh, iID_Parent, sMaLoaiCongTrinh, sTenLoaiCongTrinh, sTenVietTat, iThuTu, sMoTa, sTenLoaiCha, CAST('' as nvarchar(250)) as sLevelTab, bHasChild, 
     CAST(ROW_NUMBER() OVER(ORDER BY sMaLoaiCongTrinh ) AS NVARCHAR(MAX)) as location
  FROM #tmp WHERE iID_Parent IS NULL
  UNION ALL
  SELECT cd.iID_LoaiCongTrinh, cd.iID_Parent, cd.sMaLoaiCongTrinh, cd.sTenLoaiCongTrinh, cd.sTenVietTat, cd.iThuTu, cd.sMoTa, cd.sTenLoaiCha, CAST(CONCAT(pr.sLevelTab,' ') as nvarchar(250)) as sLevelTab, cd.bHasChild, 
 CONCAT(pr.location,'.',CAST(ROW_NUMBER() OVER(ORDER BY cd.sMaLoaiCongTrinh) AS NVARCHAR(MAX))) AS location
  FROM cte as pr
  Inner join #tmp  as cd on cd.iID_Parent = pr.iID_LoaiCongTrinh
 -- WHERE cd.iID_Parent = pr.iID_LoaiCongTrinh
)
SELECT iID_LoaiCongTrinh, iID_Parent, sMaLoaiCongTrinh, sTenLoaiCongTrinh, sTenVietTat, iThuTu, sMoTa, sTenLoaiCha, sLevelTab, bHasChild,cast('/' + replace(location, '.', '/') + '/' as hierarchyid) AS sort  FROM cte
where
(ISNULL(@sTenLoaiCongTrinh, '') = '' OR cte.sTenLoaiCongTrinh LIKE CONCAT(N'%',@sTenLoaiCongTrinh,N'%'))
AND (ISNULL(@sTenVietTat, '') = '' OR cte.sTenVietTat LIKE CONCAT(N'%',@sTenVietTat,N'%'))
AND (ISNULL(@sMaLoaiCongTrinh, '') = '' OR cte.sMaLoaiCongTrinh LIKE CONCAT(N'%',@sMaLoaiCongTrinh,N'%'))
AND (ISNULL(@iThuTu, '') = '' OR cte.iThuTu LIKE CONCAT(N'%',@iThuTu,N'%'))
AND (ISNULL(@sMoTa, '') = '' OR cte.sMoTa LIKE CONCAT(N'%',@sMoTa,N'%'))


ORDER BY sort
DROP TABLE #tmp