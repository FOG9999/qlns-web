DECLARE @iNamLamViec int set @iNamLamViec = 2018
DECLARE @dvt int set @dvt = 1000
--#DECLARE#--

CREATE TABLE #tmpIDNguonLoai0(iId_Nguon uniqueidentifier, iLoaiNganSach int)
CREATE TABLE #tmpIDNguonLoai1(iId_Nguon uniqueidentifier, iLoaiNganSach int)

;WITH cteParent as(
		SELECT iId_Nguon, iID_NguonCha
		FROM DM_Nguon WHERE iLoaiNganSach = 0 AND bPublic = 1 AND iNamLamViec = @iNamLamViec
		UNION ALL
		SELECT cd.iID_Nguon, cd.iID_NguonCha
		FROM DM_Nguon as cd, cteParent as pr
		WHERE cd.iID_Nguon = pr.iID_NguonCha AND cd.bPublic = 1 AND cd.iNamLamViec = @iNamLamViec
		
	), cteChild as(
		SELECT iId_Nguon, iID_NguonCha
		FROM DM_Nguon WHERE iLoaiNganSach = 0 AND bPublic = 1 AND iNamLamViec = @iNamLamViec
		UNION ALL
		SELECT cd.iID_Nguon, cd.iID_NguonCha
		FROM DM_Nguon as cd, cteChild as pr
		WHERE cd.iID_NguonCha = pr.iID_Nguon AND cd.bPublic = 1 AND cd.iNamLamViec = @iNamLamViec
		
	)
	INSERT INTO #tmpIDNguonLoai0(iId_Nguon)
	SELECT DISTINCT * FROM (
	SELECT iId_Nguon FROM cteChild
	union all 
	SELECT iId_Nguon FROM cteParent
	) as a;



;WITH cteParent1 as(
		SELECT iId_Nguon, iID_NguonCha
		FROM DM_Nguon WHERE iLoaiNganSach = 1 AND bPublic = 1 AND iNamLamViec = @iNamLamViec
		UNION ALL
		SELECT cd.iID_Nguon, cd.iID_NguonCha
		FROM DM_Nguon as cd, cteParent1 as pr
		WHERE cd.iID_Nguon = pr.iID_NguonCha AND cd.bPublic = 1 AND cd.iNamLamViec = @iNamLamViec
		
	), cteChild1 as(
		SELECT iId_Nguon, iID_NguonCha
		FROM DM_Nguon WHERE iLoaiNganSach = 1 AND bPublic = 1 AND iNamLamViec = @iNamLamViec
		UNION ALL
		SELECT cd.iID_Nguon, cd.iID_NguonCha
		FROM DM_Nguon as cd, cteChild1 as pr
		WHERE cd.iID_NguonCha = pr.iID_Nguon AND cd.bPublic = 1 AND cd.iNamLamViec = @iNamLamViec
		
	)
	
	INSERT INTO #tmpIDNguonLoai1(iId_Nguon)
	SELECT DISTINCT * FROM (
	SELECT iId_Nguon FROM cteChild1
	union all 
	SELECT iId_Nguon FROM cteParent1
	) as a;

select ndc.iID_NoiDungChi, ndc.sMaNoiDungChi, ndc.iID_Nguon, sum(dnct_ndc.SoTien) as fTongTien into #TEMP_tong
from DM_NoiDungChi ndc
left join NNS_DotNhanChiTiet_NDChi dnct_ndc on ndc.iID_NoiDungChi = dnct_ndc.iID_NoiDungChi AND  dnct_ndc.iNamLamViec = @iNamLamViec
where ndc.iTrangThai = 1 
		AND ndc.iID_NoiDungChi not in (SELECT iID_Parent FROM DM_NoiDungChi WHERE iID_Parent IS NOT NULL AND iTrangThai = 1)
		AND ndc.iNamLamViec = @iNamLamViec
	--AND  ((@iLoaiNganSach is null AND @iNguonNganSach is null) or ndc.iID_Nguon IN (select * from #tmpIDNguon))
GROUP BY ndc.iID_NoiDungChi, ndc.sMaNoiDungChi, ndc.iID_Nguon;


SELECT 
(select sum(fTongTien)/@dvt from #TEMP_tong where iID_Nguon IN (SELECT iID_Nguon FROM #tmpIDNguonLoai0)) as fTongNSNN,
(select sum(fTongTien)/@dvt from #TEMP_tong where iID_Nguon IN (SELECT iID_Nguon FROM #tmpIDNguonLoai1)) as fTongTXQP

drop table #TEMP_tong;
drop table #tmpIDNguonLoai0;
drop table #tmpIDNguonLoai1;
