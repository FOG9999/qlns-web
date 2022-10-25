DECLARE @iNamLamViec int set @iNamLamViec = 2020
DECLARE @dDateFrom date set @dDateFrom = null
DECLARE @dDateTo date set @dDateTo = null
DECLARE @iLoaiNganSach int set @iLoaiNganSach = 0
DECLARE @iNguonNganSach uniqueidentifier set @iNguonNganSach = null
DECLARE @dvt int set @dvt = 1000

--#DECLARE#--

/*

Lấy tổng số tiền dự toán theo năm làm việc

*/

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


select ndc.iID_NoiDungChi, ndc.sMaNoiDungChi, ndc.iID_Parent, dt.sSoQuyetDinh, dt.dNgayQuyetDinh
			, sum(dt.SoTien) as fTongTien into #TEMP_tong
from DM_NoiDungChi ndc
LEFT JOIN (
	select dt.sSoQuyetDinh,dt.dNgayQuyetDinh, sMaNoiDungChi,SoTien 
	from NNS_DuToanChiTiet as ct 
	INNER JOIN (
					SELECT TOP(1) iID_DuToan, sSoQuyetDinh, dNgayQuyetDinh 
					FROM NNS_DuToan 
					WHERE iNamLamViec = @iNamLamViec AND sMaLoaiDuToan = '001'
					AND (@dDateFrom IS NULL OR (@dDateFrom <= dNgayQuyetDinh))
					AND (@dDateTo IS NULL OR (@dDateTo >= dNgayQuyetDinh))
				) as dt on ct.iID_DuToan = dt.iID_DuToan
	 WHERE ct.iNamLamViec = @iNamLamViec
) as dt on dt.sMaNoiDungChi = ndc.sMaNoiDungChi
where ndc.iTrangThai = 1 
		--AND ndc.iID_NoiDungChi not in (SELECT iID_Parent FROM DM_NoiDungChi WHERE iID_Parent IS NOT NULL AND iTrangThai = 1)
		AND ndc.iNamLamViec = @iNamLamViec
		--AND  ((@iLoaiNganSach is null AND @iNguonNganSach is null) OR ndc.iID_Nguon IN (select * from #tmpIDNguon))
		AND ndc.iID_NoiDungChi in (SELECT iID_NoiDungChi from ndcParent)
GROUP BY ndc.iID_NoiDungChi, ndc.sMaNoiDungChi, ndc.iID_Parent, dt.sSoQuyetDinh, dt.dNgayQuyetDinh;



;with C as  
(  
  select T.iID_NoiDungChi, 
				 T.sSoQuyetDinh,
				 T.dNgayQuyetDinh, 
         T.fTongTien,  
         T.iID_NoiDungChi as RootID  
  from #TEMP_tong T  
  union all  
  select T.iID_NoiDungChi,  
				 T.sSoQuyetDinh,
				 T.dNgayQuyetDinh, 	
				 T.fTongTien, 
         C.RootID  
  from #TEMP_tong T  
    inner join C   
      on T.iID_Parent = C.iID_NoiDungChi  
)  
	
	
select T.iID_NoiDungChi,  
			 T.sMaNoiDungChi,
       T.iID_Parent,  
			 S.sSoQuyetDinh,
			 S.dNgayQuyetDinh,
       S.AmountIncludingChildren/@dvt as fTongTien
from #TEMP_tong T  
  inner join (  
             select RootID, sSoQuyetDinh, dNgayQuyetDinh, 
                    sum(fTongTien) as AmountIncludingChildren  
             from C  
						 WHERE C.fTongTien is not null
             group by RootID, sSoQuyetDinh, dNgayQuyetDinh  
             ) as S  
    on T.iID_NoiDungChi = S.RootID  
		inner join orderedTree_view_dmnoidungchi view_dmndc on T.iID_NoiDungChi = view_dmndc.iID_NoiDungChi
where (@iLoaiNganSach is null or (@iLoaiNganSach = 1 AND view_dmndc.depth < 2) OR (@iLoaiNganSach = 0 AND view_dmndc.depth < 3))
order by view_dmndc.location;


drop table #TEMP_tong;
drop table #tmpIDNguon;
