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

SELECT dt.iID_DuToan, dt.sSoQuyetDinh, dt.iID_MaDonVi, concat(dt.iID_MaDonVi,' - ',dt.sTenDonVi) as sTenDonVi, dt.sMaPhongBan, concat(dt.sMaPhongBan,' - ',dt.sTenPhongBan) as sTenPhongBan, SUM(dt.SoTien)/@dvt as iTongTien
FROM 
(
	SELECT dt.iID_DuToan,
	case when dt.sSoQuyetDinh is not null then dt.sSoQuyetDinh
	when dt.sSoCongVan is not null then dt.sSoCongVan end as sSoQuyetDinh,
	case when dt.dNgayQuyetDinh is not null then dt.dNgayQuyetDinh
	when dt.dNgayCongVan is not null then dt.dNgayCongVan end as dNgayQuyetDinh,
	sMaNoiDungChi, dtct.iID_MaDonVi, dv.sten as sTenDonVi, sMaPhongBan, sTenPhongBan, dtct.iID_NhiemVu, dtct.SoTien
	FROM NNS_DuToanChiTiet as dtct
	inner join NS_DonVi dv on dv.iID_MaDonVi = dtct.iID_MaDonVi
	INNER JOIN NNS_DuToan as dt on dt.iID_DuToan = dtct.iID_DuToan AND (dt.sMaLoaiDuToan = '003' OR dt.sMaLoaiDuToan = '004') AND dt.iNamLamViec = @iNamLamViec
									AND (
			(dt.dNgayQuyetDinh is not null AND (
				(@dDateFrom IS NULL OR @dDateFrom <= dt.dNgayQuyetDinh)
				 AND (@dDateTo IS NULL OR @dDateTo >= dt.dNgayQuyetDinh)
			)) OR (dt.dNgayCongVan is not null AND (
				 (@dDateFrom IS NULL OR @dDateFrom <= dt.dNgayCongVan)
				 AND (@dDateTo IS NULL OR @dDateTo >= dt.dNgayCongVan)
			))
		)
		AND (IsNull(@sSoQuyetDinh, '') = '' OR dt.sSoQuyetDinh LIKE '%'+@sSoQuyetDinh+'%' OR dt.sSoCongVan LIKE '%'+@sSoQuyetDinh+'%')
		and dv.iNamLamViec_DonVi = @iNamLamViec
) as dt
LEFT JOIN DM_NoiDungChi as ndc on ndc.sMaNoiDungChi = dt.sMaNoiDungChi
WHERE ndc.iTrangThai = 1
AND ndc.iID_NoiDungChi not in (SELECT iID_Parent FROM DM_NoiDungChi WHERE iID_Parent IS NOT NULL AND iTrangThai = 1)
AND(@iNguonNganSach is null or ndc.iID_Nguon IN (select * from #tmp))
		AND ndc.iNamLamViec = @iNamLamViec
GROUP BY dt.iID_DuToan, dt.iID_MaDonVi, dt.sTenDonVi, dt.sMaPhongBan, dt.sTenPhongBan, dt.dNgayQuyetDinh, dt.sSoQuyetDinh
ORDER BY dt.sMaPhongBan, dt.iID_MaDonVi