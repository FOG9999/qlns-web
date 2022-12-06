DECLARE @iNamLamViec int SET @iNamLamViec = 2020
DECLARE @dDateFrom datetime set @dDateFrom = null
DECLARE @dDateTo datetime set @dDateTo = null
DECLARE @sSoQuyetDinh nvarchar set @sSoQuyetDinh = 'QĐ_001'
DECLARE @iLoaiNganSach int set @iLoaiNganSach = 0
DECLARE @iNguonNganSach uniqueidentifier set @iNguonNganSach = null
DECLARE @dvt int set @dvt = 1
--#DECLARE#--

CREATE TABLE #tmpIDNguon(iId_Nguon uniqueidentifier)

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

-- get lst noidungchi
; WITH ndcParent AS (
SELECT ndc.iID_NoiDungChi, ndc.iID_Parent FROM DM_NoiDungChi ndc WHERE ndc.iNamLamViec = @iNamLamViec AND ndc.iTrangThai = 1
AND (@iLoaiNganSach is null or ndc.iID_Nguon IN (select * from #tmpIDNguon))
		
		UNION ALL
		
		SELECT cd.iID_NoiDungChi, cd.iID_Parent
		FROM DM_NoiDungChi as cd, ndcParent as pr
		WHERE cd.iID_NoiDungChi = pr.iID_Parent AND cd.iTrangThai = 1 AND cd.iNamLamViec = @iNamLamViec
)

SELECT * into #tmpNDC FROM ndcParent 

-- SELECT dtTheoDV.* FROM
-- (
SELECT ndc.iID_NoiDungChi, ndc.iID_Parent, dt.dNgayQuyetDinh,dt.sSoQuyetDinh,ndc.sMaNoiDungChi, dt.sMaPhongBan, dt.sTenPhongBan,iID_MaDonVi, TenDonVi, SUM(dt.SoTien) as iTongTien INTO #TEMP_tong
FROM 
(
	SELECT dt.sSoQuyetDinh, dt.dNgayQuyetDinh, sMaNoiDungChi,sMaPhongBan, concat(sMaPhongBan, ' - ', sTenPhongBan) as sTenPhongBan, iID_MaDonVi, concat(iID_MaDonVi, ' - ', TenDonVi) as TenDonVi, dtct.SoTien
	FROM NNS_DuToanChiTiet as dtct
	INNER JOIN NNS_DuToan as dt on dt.iID_DuToan = dtct.iID_DuToan AND dt.sMaLoaiDuToan = '001' AND dt.iNamLamViec = @iNamLamViec
									AND (@dDateFrom IS NULL OR @dDateFrom <= dNgayQuyetDinh)
									AND (@dDateTo IS NULL OR @dDateTo >= dNgayQuyetDinh)
) as dt
LEFT JOIN DM_NoiDungChi as ndc on ndc.sMaNoiDungChi = dt.sMaNoiDungChi
WHERE ndc.iTrangThai = 1
-- AND ndc.iID_NoiDungChi not in (SELECT iID_Parent FROM DM_NoiDungChi WHERE iID_Parent IS NOT NULL AND iTrangThai = 1)
		AND ndc.iNamLamViec = @iNamLamViec AND dt.sSoQuyetDinh = @sSoQuyetDinh
		AND  ((@iLoaiNganSach is null) or ndc.iID_Nguon IN (select * from #tmpIDNguon))
GROUP BY ndc.iID_NoiDungChi, ndc.iID_Parent, ndc.sMaNoiDungChi, dt.sMaPhongBan, dt.sTenPhongBan,iID_MaDonVi, TenDonVi, dt.dNgayQuyetDinh, dt.sSoQuyetDinh
-- ) dtTheoDV
-- ORDER BY sMaPhongBan, sMaNoiDungChi


SELECT tmpTong.* into #TEMP_tong1 FROM
(SELECT * FROM #TEMP_tong
UNION 
SELECT ndc.iID_NoiDungChi, ndc.iID_Parent, null as dNgayQuyetDinh, null as sSoQuyetDinh, ndc.sMaNoiDungChi, null as sMaPhongBan, null as sTenPhongBan, null as iID_MaDonVi, null as TenDonVi, 0 as iTongTien
FROM DM_NoiDungChi ndc 
WHERE  not EXISTS (SELECT * FROM #TEMP_tong WHERE iID_NoiDungChi = ndc.iID_NoiDungChi) AND EXISTS (SELECT iID_NoiDungChi FROM #tmpNDC WHERE iID_NoiDungChi = ndc.iID_NoiDungChi)) tmpTong

;with C as  
(  
  select T.iID_NoiDungChi, 
				 T.sSoQuyetDinh,
				 T.dNgayQuyetDinh, 
				 T.sMaNoiDungChi,
				 T.sMaPhongBan, 
				 T.sTenPhongBan, 
				 T.iID_MaDonVi, 
				 T.TenDonVi, 
         T.iTongTien,  
         T.iID_NoiDungChi as RootID  
  from #TEMP_tong1 T  
  union all  
  select T.iID_NoiDungChi, 
				 T.sSoQuyetDinh,
				 T.dNgayQuyetDinh, 
				 T.sMaNoiDungChi,
				 T.sMaPhongBan, 
				 T.sTenPhongBan, 
				 T.iID_MaDonVi, 
				 T.TenDonVi, 
         T.iTongTien,
         C.RootID  
  from #TEMP_tong1 T  
    inner join C   
      on T.iID_Parent = C.iID_NoiDungChi  
)  


	SELECT DISTINCT dtTheoDV.* FROM
(
select T.iID_NoiDungChi,  
			 T.sMaNoiDungChi,
       T.iID_Parent,  
			 S.sSoQuyetDinh,
			 S.dNgayQuyetDinh,
			 S.sMaPhongBan,
			 S.sTenPhongBan, 
			 S.iID_MaDonVi,
			 S.TenDonVi,
       S.AmountIncludingChildren/@dvt as iTongTien
from 
#TEMP_tong1 T  
  left join (  
             select RootID, sSoQuyetDinh, dNgayQuyetDinh, sMaPhongBan, sTenPhongBan, iID_MaDonVi, TenDonVi, 
                    sum(iTongTien) as AmountIncludingChildren  
             from C  
						 WHERE C.iTongTien is not null
             group by RootID, sSoQuyetDinh, dNgayQuyetDinh, sMaPhongBan, sTenPhongBan, iID_MaDonVi, TenDonVi   
             ) as S  
    on T.iID_NoiDungChi = S.RootID
		inner join orderedTree_view_dmnoidungchi view_dmndc on T.iID_NoiDungChi = view_dmndc.iID_NoiDungChi
where (@iLoaiNganSach is null or (@iLoaiNganSach = 1 AND view_dmndc.depth < 2) OR (@iLoaiNganSach = 0 AND view_dmndc.depth < 3))
)
dtTheoDV
ORDER BY sMaPhongBan, iID_MaDonVi 


drop table #TEMP_tong;
drop table #tmpIDNguon;
drop table #tmpNDC;
drop table #TEMP_tong1
