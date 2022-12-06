DECLARE @iNamLamViec int SET @iNamLamViec = 2020
DECLARE @dDateFrom datetime set @dDateFrom = null
DECLARE @dDateTo datetime set @dDateTo = null
DECLARE @iNguonNganSach uniqueidentifier set @iNguonNganSach = null
DECLARE @sSoQuyetDinh nvarchar(max) set @sSoQuyetDinh = null
DECLARE @dvt int set @dvt = 1000
--#DECLARE#--

CREATE TABLE #tmp(iId_Nguon uniqueidentifier)

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
	INSERT INTO #tmp(iId_Nguon)
	SELECT DISTINCT * FROM (
	SELECT iId_Nguon FROM cteChild
	union all 
	SELECT iId_Nguon FROM cteParent
	) as a;
END
ELSE
BEGIN
	INSERT INTO #tmp(iId_Nguon)
	SELECT iID_Nguon FROM DM_Nguon where bPublic = 1 AND iNamLamViec = @iNamLamViec ORDER BY sMaNguon
END;

SELECT iID_MaDonVi, SUM(dtct.SoTien) as SoTien INTO #tmpDauNam
FROM NNS_DuToanChiTiet as dtct 
INNER JOIN NNS_DuToan as dt on dt.iID_DuToan = dtct.iID_DuToan AND dt.sMaLoaiDuToan = '001' AND dt.iNamLamViec = @iNamLamViec
								AND (@dDateFrom IS NULL OR @dDateFrom <= dNgayQuyetDinh)
								AND (@dDateTo IS NULL OR @dDateTo >= dNgayQuyetDinh)
								AND (IsNull(@sSoQuyetDinh, '') = '' OR dt.sSoQuyetDinh LIKE '%'+@sSoQuyetDinh+'%')
LEFT JOIN DM_NoiDungChi as ndc on ndc.sMaNoiDungChi = dtct.sMaNoiDungChi
WHERE ndc.iTrangThai = 1
AND ndc.iID_NoiDungChi not in (SELECT iID_Parent FROM DM_NoiDungChi WHERE iID_Parent IS NOT NULL AND iTrangThai = 1)
AND(@iNguonNganSach is null or ndc.iID_Nguon IN (select * from #tmp))
		AND ndc.iNamLamViec = @iNamLamViec								
GROUP BY iID_MaDonVi
									
SELECT iID_MaDonVi, SUM(dtct.SoTien) as SoTien INTO #tmpBoSung
FROM NNS_DuToanChiTiet as dtct
INNER JOIN NNS_DuToan as dt on dt.iID_DuToan = dtct.iID_DuToan AND (dt.sMaLoaiDuToan = '003' OR dt.sMaLoaiDuToan = '004') AND dt.iNamLamViec = @iNamLamViec
								AND (@dDateFrom IS NULL OR @dDateFrom <= dNgayQuyetDinh)
								AND (@dDateTo IS NULL OR @dDateTo >= dNgayQuyetDinh)
								AND (IsNull(@sSoQuyetDinh, '') = '' OR dt.sSoQuyetDinh LIKE '%'+@sSoQuyetDinh+'%')
LEFT JOIN DM_NoiDungChi as ndc on ndc.sMaNoiDungChi = dtct.sMaNoiDungChi
WHERE ndc.iTrangThai = 1
AND ndc.iID_NoiDungChi not in (SELECT iID_Parent FROM DM_NoiDungChi WHERE iID_Parent IS NOT NULL AND iTrangThai = 1)
AND(@iNguonNganSach is null or ndc.iID_Nguon IN (select * from #tmp))
		AND ndc.iNamLamViec = @iNamLamViec								
GROUP BY iID_MaDonVi

SELECT dv.iID_MaDonVi, concat(dv.iID_MaDonVi,' - ', dv.sTen) as sTenDonVi, ISNULL(daunam.SoTien/@dvt, 0) as dDaGiaoDauNam, ISNULL(bosung.SoTien/@dvt,0) as dBSTrongNam, (ISNULL(daunam.SoTien/@dvt, 0) + ISNULL(bosung.SoTien/@dvt,0)) as dTongDuToan FROM NS_DonVi dv
LEFT JOIN #tmpDauNam daunam ON daunam.iID_MaDonVi = dv.iID_MaDonVi
LEFT JOIN #tmpBoSung bosung ON bosung.iID_MaDonVi = dv.iID_MaDonVi
WHERE dv.iNamLamViec_DonVi = @iNamLamViec AND (daunam.SoTien is not null or bosung.SoTien is not null)
ORDER BY iID_MaDonVi

drop table #tmpDauNam
drop table #tmpBoSung