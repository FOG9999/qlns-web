DECLARE @iNamLamViec int SET @iNamLamViec = 2019
DECLARE @dDateFrom datetime set @dDateFrom = null
DECLARE @dDateTo datetime set @dDateTo = null
DECLARE @iNguonNganSach uniqueidentifier set @iNguonNganSach = null
DECLARE @sSoQuyetDinh nvarchar(max) set @sSoQuyetDinh = null
DECLARE @dvt int set @dvt = 1
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

SELECT iID_MaDonVi, TenDonVi as sTenDonVi, sMaPhongBan, sTenPhongBan, SUM(dtct.SoTien) as SoTien INTO #tmpDauNam
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
GROUP BY iID_MaDonVi, TenDonVi, sMaPhongBan, sTenPhongBan
												
SELECT DISTINCT iID_MaDonVi, TenDonVi as sTenDonVi, sMaPhongBan, sTenPhongBan, SUM(dtct.SoTien) as SoTien INTO #tmpBoSung
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
GROUP BY iID_MaDonVi, TenDonVi, sMaPhongBan, sTenPhongBan
	
									
SELECT DISTINCT iID_MaDonVi, TenDonVi as sTenDonVi, sMaPhongBan, sTenPhongBan INTO #tmpPgBanDV
FROM NNS_DuToanChiTiet as dtct
INNER JOIN NNS_DuToan as dt on dt.iID_DuToan = dtct.iID_DuToan AND (dt.sMaLoaiDuToan = '003' OR dt.sMaLoaiDuToan = '004' OR dt.sMaLoaiDuToan = '001') AND dt.iNamLamViec = @iNamLamViec
								AND (@dDateFrom IS NULL OR @dDateFrom <= dNgayQuyetDinh)
								AND (@dDateTo IS NULL OR @dDateTo >= dNgayQuyetDinh)
								AND (IsNull(@sSoQuyetDinh, '') = '' OR dt.sSoQuyetDinh LIKE '%'+@sSoQuyetDinh+'%')
LEFT JOIN DM_NoiDungChi as ndc on ndc.sMaNoiDungChi = dtct.sMaNoiDungChi
WHERE ndc.iTrangThai = 1
AND ndc.iID_NoiDungChi not in (SELECT iID_Parent FROM DM_NoiDungChi WHERE iID_Parent IS NOT NULL AND iTrangThai = 1)
AND(@iNguonNganSach is null or ndc.iID_Nguon IN (select * from #tmp))
		AND ndc.iNamLamViec = @iNamLamViec								
								
								
SELECT ct.iID_MaDonVi, concat(ct.iID_MaDonVi,' - ',ct.sTenDonVi) as sTenDonVi,ct.sMaPhongBan,  concat(ct.sMaPhongBan,' - ',ct.sTenPhongBan) as sTenPhongBan, dDaGiaoDauNam = ct.dDaGiaoDauNam/@dvt, dBSTrongNam = ct.dBSTrongNam/@dvt, (ct.dDaGiaoDauNam + ct.dBSTrongNam)/@dvt as dTongDuToan FROM
(SELECT pbdv.*, 
ISNULL((SELECT SoTien FROM #tmpDauNam daunam WHERE pbdv.iID_MaDonVi = daunam.iID_MaDonVi AND pbdv.sTenDonVi = daunam.sTenDonVi  AND pbdv.sMaPhongBan = daunam.sMaPhongBan  AND pbdv.sTenPhongBan = daunam.sTenPhongBan),0) as dDaGiaoDauNam,
ISNULL((SELECT SoTien FROM #tmpBoSung bosung WHERE pbdv.iID_MaDonVi = bosung.iID_MaDonVi AND pbdv.sTenDonVi = bosung.sTenDonVi  AND pbdv.sMaPhongBan = bosung.sMaPhongBan  AND pbdv.sTenPhongBan = bosung.sTenPhongBan),0) as dBSTrongNam
FROM #tmpPgBanDV pbdv) ct
ORDER BY sMaPhongBan, iID_MaDonVi