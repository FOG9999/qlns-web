DECLARE @iNamLamViec int set @iNamLamViec = 2019
DECLARE @dDateFrom date set @dDateFrom = null
DECLARE @dDateTo date set @dDateTo = null
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
	

select ndc.iID_NoiDungChi, ndc.sMaNoiDungChi, ndc.iID_Nguon, dt.sSoQuyetDinh, dt.dNgayQuyetDinh
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
		AND ndc.iID_NoiDungChi not in (SELECT iID_Parent FROM DM_NoiDungChi WHERE iID_Parent IS NOT NULL AND iTrangThai = 1)
		AND ndc.iNamLamViec = @iNamLamViec
GROUP BY ndc.iID_NoiDungChi, ndc.sMaNoiDungChi, ndc.iID_Nguon, dt.sSoQuyetDinh, dt.dNgayQuyetDinh;

SELECT 
(select sum(fTongTien/@dvt) from #TEMP_tong where iID_Nguon IN (SELECT iID_Nguon FROM #tmpIDNguonLoai0)) as fTongNSNN,
(select sum(fTongTien/@dvt) from #TEMP_tong where iID_Nguon IN (SELECT iID_Nguon FROM #tmpIDNguonLoai1)) as fTongTXQP


-- drop table #TEMP_tong;
-- drop table #tmpIDNguon;
