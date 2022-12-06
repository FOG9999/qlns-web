
SELECT  DISTINCT pr.iID_LoaiCongTrinh, pr.iID_Parent, pr.sMaLoaiCongTrinh, pr.sTenLoaiCongTrinh, pr.sTenVietTat, pr.iThuTu,(CASE WHEN cd.iID_Parent IS NULL THEN 0 ELSE 1 END) as bHasChild INTO #tmp
FROM VDT_DM_LoaiCongTrinh as pr
LEFT JOIN VDT_DM_LoaiCongTrinh as cd on pr.iID_LoaiCongTrinh = cd.iID_Parent AND cd.bActive = 1
WHERE pr.bActive = 1 AND pr.iID_LoaiCongTrinh <> @iId;

WITH cte(iID_LoaiCongTrinh, iID_Parent, sMaLoaiCongTrinh, sTenLoaiCongTrinh, sTenVietTat, iThuTu, sLevelTab,bHasChild, location)
AS
(
	SELECT iID_LoaiCongTrinh, iID_Parent, sMaLoaiCongTrinh, sTenLoaiCongTrinh, sTenVietTat, iThuTu, CAST('' as nvarchar(250)) as sLevelTab, bHasChild, 
			CAST(iID_LoaiCongTrinh as nvarchar(max)) as location
	FROM #tmp WHERE iID_Parent IS NULL
	UNION ALL
	SELECT cd.iID_LoaiCongTrinh, cd.iID_Parent, cd.sMaLoaiCongTrinh, cd.sTenLoaiCongTrinh, cd.sTenVietTat, cd.iThuTu, CAST(CONCAT(pr.sLevelTab,' ') as nvarchar(250)) as sLevelTab, cd.bHasChild, 
			CAST(CONCAT(pr.location, CAST(cd.iID_LoaiCongTrinh as nvarchar(max))) as nvarchar(max)) as location
	FROM cte as pr, #tmp as cd
	WHERE cd.iID_Parent = pr.iID_LoaiCongTrinh
)
SELECT iID_LoaiCongTrinh, iID_Parent, sMaLoaiCongTrinh, sTenLoaiCongTrinh, sTenVietTat, iThuTu,sLevelTab, bHasChild  FROM cte ORDER BY location
DROP TABLE #tmp