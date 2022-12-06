DECLARE @iNamLamViec int SET @iNamLamViec = 2018
DECLARE @dDateFrom datetime set @dDateFrom = null
DECLARE @dDateTo datetime set @dDateTo = null
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

SELECT dt.sSoQuyetDinh, sMaNoiDungChi, sMaPhongBan, sTenPhongBan, dtct.iID_NhiemVu, SUM(dtct.SoTien) as SoTien into #tempDtct
	FROM NNS_DuToanChiTiet as dtct
	INNER JOIN NNS_DuToan as dt on dt.iID_DuToan = dtct.iID_DuToan AND dt.sMaLoaiDuToan <> '001' AND dt.iNamLamViec = @iNamLamViec
									AND (@dDateFrom IS NULL OR @dDateFrom <= dNgayQuyetDinh)
									AND (@dDateTo IS NULL OR @dDateTo >= dNgayQuyetDinh)
	GROUP BY dt.sSoQuyetDinh, sMaNoiDungChi, sMaPhongBan, sTenPhongBan, dtct.iID_NhiemVu						


SELECT data.* into #tempNSNN 
FROM
(
SELECT dt.sMaPhongBan, dt.sTenPhongBan, dt.iID_NhiemVu, SUM(dt.SoTien) as iTongTien
FROM #tempDtct dt
LEFT JOIN DM_NoiDungChi as ndc on ndc.sMaNoiDungChi = dt.sMaNoiDungChi
WHERE ndc.iTrangThai = 1
AND ndc.iID_NoiDungChi not in (SELECT iID_Parent FROM DM_NoiDungChi WHERE iID_Parent IS NOT NULL AND iTrangThai = 1)
AND ndc.iID_Nguon IN (select iID_Nguon from #tmpIDNguonLoai0)
		AND ndc.iNamLamViec = @iNamLamViec 
GROUP BY dt.sMaPhongBan, dt.sTenPhongBan, dt.iID_NhiemVu
) data
INNER JOIN NNS_DuToan_NhiemVu nv ON data.iID_NhiemVu = nv.iID_NhiemVu


SELECT data.* into #tempTXQP FROM
(
SELECT dt.sMaPhongBan, dt.sTenPhongBan, dt.iID_NhiemVu, SUM(dt.SoTien) as iTongTien
FROM #tempDtct dt
LEFT JOIN DM_NoiDungChi as ndc on ndc.sMaNoiDungChi = dt.sMaNoiDungChi
WHERE ndc.iTrangThai = 1
AND ndc.iID_NoiDungChi not in (SELECT iID_Parent FROM DM_NoiDungChi WHERE iID_Parent IS NOT NULL AND iTrangThai = 1)
AND ndc.iID_Nguon IN (select iID_Nguon from #tmpIDNguonLoai1)
		AND ndc.iNamLamViec = @iNamLamViec 
GROUP BY dt.sMaPhongBan, dt.sTenPhongBan, dt.iID_NhiemVu
) data
INNER JOIN NNS_DuToan_NhiemVu nv ON data.iID_NhiemVu = nv.iID_NhiemVu


SELECT DISTINCT data.sSoQuyetDinh, data.sMaPhongBan, data.sTenPhongBan, data.iID_NhiemVu, nv.sNhiemVu as GhiChu,
(SELECT sum(iTongTien)/@dvt FROM #tempNSNN nsnn WHERE data.sMaPhongBan = nsnn.sMaPhongBan AND data.iID_NhiemVu = nsnn.iID_NhiemVu) as fTienNSNN, 
(SELECT sum(iTongTien)/@dvt FROM #tempTXQP txqp WHERE data.sMaPhongBan = txqp.sMaPhongBan AND data.iID_NhiemVu = txqp.iID_NhiemVu) as fTienTXQP 
FROM #tempDtct data
INNER JOIN NNS_DuToan_NhiemVu nv ON data.iID_NhiemVu = nv.iID_NhiemVu
ORDER BY data.sMaPhongBan, data.iID_NhiemVu
