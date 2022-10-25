DECLARE @iNamLamViec int set @iNamLamViec = 2020
DECLARE @iLoaiNganSach int set @iLoaiNganSach = 1
DECLARE @iNguonNganSach uniqueidentifier set @iNguonNganSach = null
DECLARE @dvt int set @dvt = 1000
--#DECLARE#--

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
END;


-- get lst noidungchi
; WITH ndcParent AS (
SELECT ndc.iID_NoiDungChi, ndc.iID_Parent FROM DM_NoiDungChi ndc WHERE ndc.iNamLamViec = @iNamLamViec AND ndc.iTrangThai = 1
AND ((@iLoaiNganSach is null AND @iNguonNganSach is null) or ndc.iID_Nguon IN (select * from #tmpIDNguon))
		
		UNION ALL
		
		SELECT cd.iID_NoiDungChi, cd.iID_Parent
		FROM DM_NoiDungChi as cd, ndcParent as pr
		WHERE cd.iID_NoiDungChi = pr.iID_Parent AND cd.iTrangThai = 1 AND cd.iNamLamViec = @iNamLamViec
)

-- tinh tong 
select ndc.iID_NoiDungChi, ndc.sMaNoiDungChi, ndc.iID_Parent, sum(dnct_ndc.SoTien) as fTongTien into #TEMP_tong
from DM_NoiDungChi ndc
left join NNS_DotNhanChiTiet_NDChi dnct_ndc on ndc.iID_NoiDungChi = dnct_ndc.iID_NoiDungChi AND  dnct_ndc.iNamLamViec = @iNamLamViec
where ndc.iTrangThai = 1 
		AND ndc.iID_NoiDungChi in (SELECT iID_NoiDungChi from ndcParent)
		AND ndc.iNamLamViec = @iNamLamViec
GROUP BY ndc.iID_NoiDungChi, ndc.sMaNoiDungChi, ndc.iID_Parent;

;with C as  
(  
  select T.iID_NoiDungChi,  
         T.fTongTien,  
         T.iID_NoiDungChi as RootID  
  from #TEMP_tong T  
  union all  
  select T.iID_NoiDungChi,  
         T.fTongTien,  
         C.RootID  
  from #TEMP_tong T  
    inner join C   
      on T.iID_Parent = C.iID_NoiDungChi  
)  
  
select T.iID_NoiDungChi,  
 T.sMaNoiDungChi,  
       T.iID_Parent,  
       S.AmountIncludingChildren/@dvt as fTongTien,
	   view_dmndc.rootParent 
from #TEMP_tong T  
  inner join (  
             select RootID,  
                    sum(fTongTien) as AmountIncludingChildren  
             from C  
             group by RootID  
             ) as S  
    on T.iID_NoiDungChi = S.RootID  
		inner join orderedTree_view_dmnoidungchi view_dmndc on T.iID_NoiDungChi = view_dmndc.iID_NoiDungChi
where (@iLoaiNganSach is null or (@iLoaiNganSach = 1 AND view_dmndc.depth < 2) OR (@iLoaiNganSach = 0 AND view_dmndc.depth < 3))
order by view_dmndc.location;
 
drop table #TEMP_tong;
drop table #tmpIDNguon;