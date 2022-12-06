declare @iNamLamViec int set @iNamLamViec = 2019
declare @lstDonViBHXH nvarchar(MAX) set @lstDonViBHXH = '1,2,3,4,5'
--#DECLARE#--
select dv.iID_BHXH_DonViID, bn.iID_MaDonVi, bn.iThang, count(bn.iID_MaDonVi) AS iLuotDieuTri, sum(bn.iSoNgayDieuTri) as iSoNgayDieuTri INTO #tmpTongHop
from BHXH_BenhNhan bn
INNER JOIN BHXH_DonVi dv ON dv.iID_MaDonViBHXH = bn.iID_MaDonVi
WHERE bn.iNamLamViec = @iNamLamViec
GROUP BY dv.iID_BHXH_DonViID, bn.iID_MaDonVi, bn.iThang


select iID_BHXH_DonViID, 
		[1] AS T1,
    [2] AS T2,
    [3] AS T3,
    [4] AS T4,
    [5] AS T5,
    [6] AS T6,
    [7] AS T7,
    [8] AS T8,
    [9] AS T9,
    [10] AS T10,
    [11] AS T11,
    [12] AS T12
		INTO #tmpLuotDieuTri
		from (
		select iID_BHXH_DonViID, iID_MaDonVi, iThang, iLuotDieuTri from #tmpTongHop
		) as data
		pivot 
		(
			SUM(iLuotDieuTri) for iThang IN ( [1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12] )
		) as bangchuyen

select iID_BHXH_DonViID, 
		[1] AS T1,
    [2] AS T2,
    [3] AS T3,
    [4] AS T4,
    [5] AS T5,
    [6] AS T6,
    [7] AS T7,
    [8] AS T8,
    [9] AS T9,
    [10] AS T10,
    [11] AS T11,
    [12] AS T12
		INTO #tmpSoNgayDieuTri
		from (
		select iID_BHXH_DonViID, iID_MaDonVi, iThang, iSoNgayDieuTri from #tmpTongHop
		) as data
		pivot 
		(
			max(iSoNgayDieuTri) for iThang IN ( [1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12] )
		) as bangchuyen


;WITH orderedTree AS (
        SELECT bhxhdv.*, bhxhdv.iID_BHXH_DonViID AS parent, 1 AS depth, CAST(bhxhdv.iID_MaDonViBHXH AS NVARCHAR(MAX)) AS location,
		CAST(ROW_NUMBER() over (order by iID_MaDonViBHXH) AS nvarchar(max)) as sSTT
        FROM BHXH_DonVi bhxhdv
        WHERE bhxhdv.iID_ParentID IS NULL AND bhxhdv.iID_MaDonViBHXH IN (select * FROM splitstring(@lstDonViBHXH))
         
        UNION ALL
         
        SELECT child.*, orderedTree.parent, depth + 1, CAST(CONCAT(orderedTree.location, '.', child.iID_MaDonViBHXH) AS NVARCHAR(MAX)) AS location,
		orderedTree.sSTT + '.' + CAST(ROW_NUMBER() over (order by child.iID_MaDonViBHXH) AS nvarchar(max)) as sSTT
        FROM BHXH_DonVi child
        INNER JOIN orderedTree ON child.iID_ParentID = orderedTree.iID_BHXH_DonViID
		--WHERE child.iID_MaDonViBHXH IN (select * FROM splitstring(@lstDonViBHXH))
)


select DISTINCT treedv.iID_BHXH_DonViID, treedv.iID_MaDonViBHXH, treedv.sTen, treedv.iID_ParentID, treedv.depth, treedv.location, treedv.sSTT,
luotdieutri.T1 as iLuotDieuTriT1, 
luotdieutri.T2 as iLuotDieuTriT2, 
luotdieutri.T3 as iLuotDieuTriT3, 
luotdieutri.T4 as iLuotDieuTriT4, 
luotdieutri.T5 as iLuotDieuTriT5, 
luotdieutri.T6 as iLuotDieuTriT6, 
luotdieutri.T7 as iLuotDieuTriT7, 
luotdieutri.T8 as iLuotDieuTriT8, 
luotdieutri.T9 as iLuotDieuTriT9, 
luotdieutri.T10 as iLuotDieuTriT10, 
luotdieutri.T11 as iLuotDieuTriT11, 
luotdieutri.T12 as iLuotDieuTriT12, 

songaydieutri.T1 as iSoNgayDieuTriT1, 
songaydieutri.T2 as iSoNgayDieuTriT2, 
songaydieutri.T3 as iSoNgayDieuTriT3, 
songaydieutri.T4 as iSoNgayDieuTriT4, 
songaydieutri.T5 as iSoNgayDieuTriT5, 
songaydieutri.T6 as iSoNgayDieuTriT6, 
songaydieutri.T7 as iSoNgayDieuTriT7, 
songaydieutri.T8 as iSoNgayDieuTriT8, 
songaydieutri.T9 as iSoNgayDieuTriT9, 
songaydieutri.T10 as iSoNgayDieuTriT10, 
songaydieutri.T11 as iSoNgayDieuTriT11, 
songaydieutri.T12 as iSoNgayDieuTriT12

INTO #tmpAllData

from orderedTree treedv
LEFT JOIN #tmpLuotDieuTri luotdieutri ON treedv.iID_BHXH_DonViID = luotdieutri.iID_BHXH_DonViID
LEFT JOIN #tmpSoNgayDieuTri songaydieutri ON treedv.iID_BHXH_DonViID = songaydieutri.iID_BHXH_DonViID

;with C as(
	select T.iID_BHXH_DonViID, T.iID_MaDonViBHXH, T.sTen, T.sSTT, T.iID_BHXH_DonViID as RootID,
	T.iLuotDieuTriT1, 
	T.iLuotDieuTriT2, 
	T.iLuotDieuTriT3, 
	T.iLuotDieuTriT4, 
	T.iLuotDieuTriT5, 
	T.iLuotDieuTriT6, 
	T.iLuotDieuTriT7, 
	T.iLuotDieuTriT8, 
	T.iLuotDieuTriT9, 
	T.iLuotDieuTriT10, 
	T.iLuotDieuTriT11, 
	T.iLuotDieuTriT12, 

	T.iSoNgayDieuTriT1, 
	T.iSoNgayDieuTriT2, 
	T.iSoNgayDieuTriT3, 
	T.iSoNgayDieuTriT4, 
	T.iSoNgayDieuTriT5, 
	T.iSoNgayDieuTriT6, 
	T.iSoNgayDieuTriT7, 
	T.iSoNgayDieuTriT8, 
	T.iSoNgayDieuTriT9, 
	T.iSoNgayDieuTriT10, 
	T.iSoNgayDieuTriT11, 
	T.iSoNgayDieuTriT12
	from #tmpAllData T
	union all
	select T.iID_BHXH_DonViID, T.iID_MaDonViBHXH, T.sTen, T.sSTT, C.RootID,
	T.iLuotDieuTriT1, 
	T.iLuotDieuTriT2, 
	T.iLuotDieuTriT3, 
	T.iLuotDieuTriT4, 
	T.iLuotDieuTriT5, 
	T.iLuotDieuTriT6, 
	T.iLuotDieuTriT7, 
	T.iLuotDieuTriT8, 
	T.iLuotDieuTriT9, 
	T.iLuotDieuTriT10, 
	T.iLuotDieuTriT11, 
	T.iLuotDieuTriT12, 

	T.iSoNgayDieuTriT1, 
	T.iSoNgayDieuTriT2, 
	T.iSoNgayDieuTriT3, 
	T.iSoNgayDieuTriT4, 
	T.iSoNgayDieuTriT5, 
	T.iSoNgayDieuTriT6, 
	T.iSoNgayDieuTriT7, 
	T.iSoNgayDieuTriT8, 
	T.iSoNgayDieuTriT9, 
	T.iSoNgayDieuTriT10, 
	T.iSoNgayDieuTriT11, 
	T.iSoNgayDieuTriT12
	from #tmpAllData T
	inner join C ON T.iID_ParentID = C.iID_BHXH_DonViID
)

select DISTINCT T.iID_BHXH_DonViID, T.iID_MaDonViBHXH, T.sTen, T.iID_ParentID, T.depth, T.location, T.sSTT,
	S.iLuotDieuTriT1, 
	S.iLuotDieuTriT2, 
	S.iLuotDieuTriT3, 
	S.iLuotDieuTriT4, 
	S.iLuotDieuTriT5, 
	S.iLuotDieuTriT6, 
	S.iLuotDieuTriT7, 
	S.iLuotDieuTriT8, 
	S.iLuotDieuTriT9, 
	S.iLuotDieuTriT10, 
	S.iLuotDieuTriT11, 
	S.iLuotDieuTriT12, 

	S.iSoNgayDieuTriT1, 
	S.iSoNgayDieuTriT2, 
	S.iSoNgayDieuTriT3, 
	S.iSoNgayDieuTriT4, 
	S.iSoNgayDieuTriT5, 
	S.iSoNgayDieuTriT6, 
	S.iSoNgayDieuTriT7, 
	S.iSoNgayDieuTriT8, 
	S.iSoNgayDieuTriT9, 
	S.iSoNgayDieuTriT10, 
	S.iSoNgayDieuTriT11, 
	S.iSoNgayDieuTriT12
	FROM #tmpAllData T
	INNER JOIN (
		select RootID, 
		sum(iLuotDieuTriT1) as iLuotDieuTriT1, 
	sum(iLuotDieuTriT2) as iLuotDieuTriT2, 
	sum(iLuotDieuTriT3) as iLuotDieuTriT3, 
	sum(iLuotDieuTriT4) as iLuotDieuTriT4, 
	sum(iLuotDieuTriT5) as iLuotDieuTriT5, 
	sum(iLuotDieuTriT6) as iLuotDieuTriT6, 
	sum(iLuotDieuTriT7) as iLuotDieuTriT7, 
	sum(iLuotDieuTriT8) as iLuotDieuTriT8, 
	sum(iLuotDieuTriT9) as iLuotDieuTriT9, 
	sum(iLuotDieuTriT10) as iLuotDieuTriT10, 
	sum(iLuotDieuTriT11) as iLuotDieuTriT11, 
	sum(iLuotDieuTriT12) as iLuotDieuTriT12, 

	sum(iSoNgayDieuTriT1) as iSoNgayDieuTriT1,
	sum(iSoNgayDieuTriT2) as iSoNgayDieuTriT2,
	sum(iSoNgayDieuTriT3) as iSoNgayDieuTriT3,
	sum(iSoNgayDieuTriT4) as iSoNgayDieuTriT4,
	sum(iSoNgayDieuTriT5) as iSoNgayDieuTriT5,
	sum(iSoNgayDieuTriT6) as iSoNgayDieuTriT6,
	sum(iSoNgayDieuTriT7) as iSoNgayDieuTriT7,
	sum(iSoNgayDieuTriT8) as iSoNgayDieuTriT8,
	sum(iSoNgayDieuTriT9) as iSoNgayDieuTriT9,
	sum(iSoNgayDieuTriT10) as iSoNgayDieuTriT10,
	sum(iSoNgayDieuTriT11) as iSoNgayDieuTriT11,
	sum(iSoNgayDieuTriT12) as iSoNgayDieuTriT12
	from C
	GROUP BY RootID
	) as S ON T.iID_BHXH_DonViID = S.RootID
ORDER BY T.location, T.depth


-- DROP table #tmpTongHop
-- DROP table #tmpLuotDieuTri
-- DROP table #tmpSoNgayDieuTri
-- drop table #tmpAllData
-- 