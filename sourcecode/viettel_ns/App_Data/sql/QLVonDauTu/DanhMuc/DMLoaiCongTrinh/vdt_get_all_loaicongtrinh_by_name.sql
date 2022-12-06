--DECLARE @sTenVietTat nvarchar(100) set @sTenVietTat = null
--DECLARE @sMaLoaiCongTrinh nvarchar(50) set @sMaLoaiCongTrinh = null
--DECLARE @iThuTu int set @iThuTu = 59
--DECLARE @sTenLoaiCongTrinh nvarchar(300) set @sTenLoaiCongTrinh = null

--DECLARE @sMoTa nvarchar(MAX) set @sMoTa = null

--Find VDT_DM_LoaiCongTrinh by name
SELECT  DISTINCT pr.iID_LoaiCongTrinh, pr.iID_Parent, pr.sMaLoaiCongTrinh, pr.sTenLoaiCongTrinh, pr.sTenVietTat, pr.iThuTu, pr.sMoTa, child.sTenLoaiCongTrinh as sTenLoaiCha, (CASE WHEN pr.iID_Parent IS NULL THEN 1 ELSE 0 END) as bHasChild INTO #tmp
FROM VDT_DM_LoaiCongTrinh as pr
LEFT JOIN VDT_DM_LoaiCongTrinh as cd on pr.iID_LoaiCongTrinh = cd.iID_Parent AND cd.bActive = 1
LEFT JOIN VDT_DM_LoaiCongTrinh as child on pr.iID_Parent = child.iID_LoaiCongTrinh AND child.bActive = 1
WHERE pr.bActive = 1 AND 
(ISNULL(@sTenLoaiCongTrinh, '') = '' OR pr.sTenLoaiCongTrinh LIKE CONCAT(N'%',@sTenLoaiCongTrinh,N'%')) AND
(ISNULL(@sTenVietTat, '') = '' OR pr.sTenVietTat LIKE CONCAT(N'%',@sTenVietTat,N'%')) AND
(ISNULL(@sMaLoaiCongTrinh, '') = '' OR pr.sMaLoaiCongTrinh LIKE CONCAT(N'%',@sMaLoaiCongTrinh,N'%')) AND
(ISNULL(@sMoTa, '') = '' OR pr.sMoTa LIKE CONCAT(N'%',@sMoTa,N'%')) AND
(ISNULL(@iThuTu, '0') = '' OR pr.iThuTu LIKE CONCAT('%',@iThuTu,'%'));
WITH cte(iID_LoaiCongTrinh, iID_Parent, sMaLoaiCongTrinh, sTenLoaiCongTrinh, sTenVietTat, iThuTu, sMoTa, sTenLoaiCha, sLevelTab, bHasChild, location)
AS
(
	SELECT lct.iID_LoaiCongTrinh, lct.iID_Parent, lct.sMaLoaiCongTrinh, lct.sTenLoaiCongTrinh, lct.sTenVietTat, lct.iThuTu, lct.sMoTa, sTenLoaiCha, CAST('' as nvarchar(250)) as sLevelTab,bHasChild, 
			CAST(lct.iID_LoaiCongTrinh as nvarchar(max)) as location
	FROM VDT_DM_LoaiCongTrinh lct , #tmp tmp
	WHERE lct.iID_LoaiCongTrinh  = tmp.iID_Parent
	UNION ALL
	SELECT cd.iID_LoaiCongTrinh, cd.iID_Parent, cd.sMaLoaiCongTrinh, cd.sTenLoaiCongTrinh, cd.sTenVietTat, cd.iThuTu, cd.sMoTa, cd.sTenLoaiCha, CAST(CONCAT(pr.sLevelTab,' ') as nvarchar(250)) as sLevelTab, cd.bHasChild, 
			CAST(CONCAT(pr.location, CAST(cd.iID_LoaiCongTrinh as nvarchar(max))) as nvarchar(max)) as location
	FROM cte as pr, #tmp as cd
	WHERE cd.iID_Parent = pr.iID_LoaiCongTrinh
)
SELECT DISTINCT iID_LoaiCongTrinh, iID_Parent, sMaLoaiCongTrinh, sTenLoaiCongTrinh, sTenVietTat, iThuTu, sMoTa, sTenLoaiCha, sLevelTab, bHasChild, location  FROM cte 
ORDER BY location
DROP TABLE #tmp