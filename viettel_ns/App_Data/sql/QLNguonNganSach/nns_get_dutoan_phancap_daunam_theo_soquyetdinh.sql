DECLARE @iNamLamViec int SET @iNamLamViec = 2021
DECLARE @dDateFrom datetime set @dDateFrom = null
DECLARE @dDateTo datetime set @dDateTo = null
DECLARE @sSoQuyetDinh nvarchar set @sSoQuyetDinh = 'QĐ_001'
DECLARE @iLoaiNganSach int set @iLoaiNganSach = null
DECLARE @iNguonNganSach uniqueidentifier set @iNguonNganSach = null
DECLARE @dvt int set @dvt = 1
--#DECLARE#--

CREATE TABLE #tmpIDNguon(iId_Nguon uniqueidentifier)

	IF(@iLoaiNganSach is not null)
	BEGIN
		;WITH orderedTree (iID_Nguon, sMaNguon, sNoiDung, sMaCTMT, iID_NguonCha, sLoai, sKhoan, iLoaiNganSach, sMaNguonCha, alevel, location)
		AS	(SELECT iID_Nguon, sMaNguon, sNoiDung, sMaCTMT, iID_NguonCha, sLoai, sKhoan, iLoaiNganSach,
							CAST('' AS NVARCHAR(MAX)) as sMaNguonCha,
						 0 AS alevel, 
						 CAST(sMaNguon AS NVARCHAR(MAX)) AS location
			FROM DM_Nguon 
			WHERE 1=1
					AND iID_NguonCha is null
					AND bPublic = 1
					AND iNamLamViec = @iNamLamViec
			UNION ALL
			SELECT child.iID_Nguon, child.sMaNguon, child.sNoiDung, child.sMaCTMT, child.iID_NguonCha, child.sLoai, child.sKhoan, parent.iLoaiNganSach,
						 CAST(parent.sMaNguon AS NVARCHAR(MAX)) as sMaNguonCha,
						 parent.alevel + 1 as alevel,
						 CAST(CONCAT(parent.location, '.', child.sMaNguon) AS NVARCHAR(MAX)) AS location
			FROM DM_Nguon child
			INNER JOIN orderedTree parent ON child.iID_NguonCha = parent.iID_Nguon
			WHERE child.bPublic = 1 AND child.iNamLamViec = @iNamLamViec)

		INSERT INTO #tmpIDNguon(iId_Nguon)
		SELECT iID_Nguon
	FROM orderedTree
	WHERE iLoaiNganSach = @iLoaiNganSach
	ORDER BY location
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
	SELECT dt.sSoQuyetDinh, dt.dNgayQuyetDinh, sMaNoiDungChi,sMaPhongBan, concat(sMaPhongBan, ' - ', sTenPhongBan) as sTenPhongBan, dtct.iID_MaDonVi, concat(dtct.iID_MaDonVi, ' - ', dv.sTen ) as TenDonVi, dtct.SoTien
	FROM NNS_DuToanChiTiet as dtct
	INNER JOIN NNS_DuToan as dt on dt.iID_DuToan = dtct.iID_DuToan AND dt.sMaLoaiDuToan = '001' AND dt.iNamLamViec = @iNamLamViec
									AND (@dDateFrom IS NULL OR @dDateFrom <= dNgayQuyetDinh)
									AND (@dDateTo IS NULL OR @dDateTo >= dNgayQuyetDinh)
	inner join NS_DonVi dv on dv.iID_MaDonVi = dtct.iID_MaDonVi and dv.iNamLamViec_DonVi = @iNamLamViec
) as dt
LEFT JOIN DM_NoiDungChi as ndc on ndc.sMaNoiDungChi = dt.sMaNoiDungChi
WHERE ndc.iTrangThai = 1
-- AND ndc.iID_NoiDungChi not in (SELECT iID_Parent FROM DM_NoiDungChi WHERE iID_Parent IS NOT NULL AND iTrangThai = 1)
		AND ndc.iNamLamViec = @iNamLamViec AND dt.sSoQuyetDinh = @sSoQuyetDinh
		AND  ((@iLoaiNganSach is null) or ndc.iID_Nguon IN (select * from #tmpIDNguon))
GROUP BY ndc.iID_NoiDungChi, ndc.iID_Parent, ndc.sMaNoiDungChi, dt.sMaPhongBan, dt.sTenPhongBan,iID_MaDonVi, TenDonVi, dt.dNgayQuyetDinh, dt.sSoQuyetDinh
-- ) dtTheoDV
-- ORDER BY sMaPhongBan, sMaNoiDungChi

select dNgayQuyetDinh, sSoQuyetDinh, sMaPhongBan, sTenPhongBan, iID_MaDonVi, TenDonVi, sum(iTongTien)/@dvt as iTongTien from #TEMP_tong
GROUP BY dNgayQuyetDinh, sSoQuyetDinh,sMaPhongBan, sTenPhongBan, iID_MaDonVi, TenDonVi

 drop table #TEMP_tong;
 drop table #tmpIDNguon;
 drop table #tmpNDC;