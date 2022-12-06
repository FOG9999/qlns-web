DECLARE @iNguonNganSach uniqueidentifier set @iNguonNganSach = null
DECLARE @iNamLamViec int set @iNamLamViec = 2022
DECLARE @dNgayFrom datetime set @dNgayFrom = null
DECLARE @dNgayTo datetime set @dNgayTo = null
DECLARE @dvt int SET @dvt = 1000

-- DROP TABLE #tmp

--###--
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

WITH dutoandagiao(iID_Nguon,iID_DuToan, SoTien, dNgayQuyetDinh, sMaLoaiDuToan)
as (
select dtct.iID_Nguon, dtct.iID_DuToan, dtct.SoTien, case when dt.dNgayQuyetDinh is not null then dt.dNgayQuyetDinh
when dt.dNgayCongVan is not null then dt.dNgayCongVan end as dNgayQuyetDinh, dt.sMaLoaiDuToan
from NNS_DuToan dt
left join (
SELECT iID_Nguon, iID_DuToan, dNgayQuyetDinh, sMaLoaiDuToan, sum(SoTien) as SoTien
from (
select noidungchi.iID_Nguon, case when dt.dNgayQuyetDinh is not null then dt.dNgayQuyetDinh
when dt.dNgayCongVan is not null then dt.dNgayCongVan end as dNgayQuyetDinh ,dt.iID_DuToan, dt.sMaLoaiDuToan,dtct.SoTien
from  NNS_DuToan dt 
INNER JOIN NNS_DuToanChiTiet as dtct on dt.iID_DuToan = dtct.iID_DuToan
	INNER JOIN DM_NoiDungChi as noidungchi ON noidungchi.sMaNoiDungChi = dtct.sMaNoiDungChi
where dt.iNamLamViec = @iNamLamViec
		 AND ((dt.dNgayQuyetDinh is not null AND (
			 (@dNgayFrom IS NULL OR @dNgayFrom <= dt.dNgayQuyetDinh)
			 AND (@dNgayTo IS NULL OR @dNgayTo >= dt.dNgayQuyetDinh)
		 )) OR (dt.dNgayQuyetDinh is null AND dt.dNgayCongVan is not null AND (
			 (@dNgayFrom IS NULL OR @dNgayFrom <= dt.dNgayCongVan)
			 AND (@dNgayTo IS NULL OR @dNgayTo >= dt.dNgayCongVan)
		 )))
		 ) datadt
		 GROUP BY iID_Nguon, iID_DuToan, dNgayQuyetDinh,sMaLoaiDuToan)
		 dtct ON dtct.iID_DuToan = dt.iID_DuToan
where --dt.sMaLoaiDuToan != '001' and dt.sMaLoaiDuToan != '002' 
 dt.iNamLamViec = @iNamLamViec 
AND ((dt.dNgayQuyetDinh is not null AND (
			 (@dNgayFrom IS NULL OR @dNgayFrom <= dt.dNgayQuyetDinh)
			 AND (@dNgayTo IS NULL OR @dNgayTo >= dt.dNgayQuyetDinh)
		 )) OR (dt.dNgayQuyetDinh is null AND dt.dNgayCongVan is not null AND (
			 (@dNgayFrom IS NULL OR @dNgayFrom <= dt.dNgayCongVan)
			 AND (@dNgayTo IS NULL OR @dNgayTo >= dt.dNgayCongVan)
		 )))
)

SELECT  dmn.iID_Nguon, dmn.sMaCTMT,dmn.sLoai,dmn.sKhoan,dmn.sNoiDung, dmn.iSTT, dmn.depth,
		sum( CASE WHEN nsdn.sMaLoaiDuToan = '001' THEN dnct.SoTien else 0 END)/@dvt as dTDauNam,
    sum( CASE WHEN nsdn.sMaLoaiDuToan = '002' THEN dnct.SoTien else 0 END)/@dvt as dtChuyenSang,
		sum( CASE WHEN nsdn.sMaLoaiDuToan != '001' AND nsdn.sMaLoaiDuToan != '002' THEN dnct.SoTien else 0 END)/@dvt as NhaNuocBosung,
    sum(case when dnct.SoTien is null then 0 else dnct.SoTien end)/@dvt as TongDuToan,
		-- START dt đã giao --
		dagiaoDauNam = dagiao.dagiaoDauNam/@dvt, dagiaoChuyenSang = dagiao.dagiaoChuyenSang/@dvt, dagiaoDuToan = dagiao.dagiaoDuToan/@dvt, dagiaoTongDuToan = dagiao.dagiaoTongDuToan/@dvt,
		(sum(case when dnct.SoTien is null then 0 else dnct.SoTien end) - dagiaoTongDuToan)/@dvt as conLai,
		-- END dt đã giao --
    (case when tbl1.iID_Nguon is not null then '' else 'isCha' end)  as bLaHangCha 
FROM orderedTree_view_dmnguon as dmn
INNER JOIN #tmp as tmp on dmn.iID_Nguon = tmp.iID_Nguon
left join  NNS_DotNhanChiTiet dnct ON dnct.iID_Nguon = dmn.iID_Nguon AND dnct.iNamLamViec = @iNamLamViec
left join NNS_DotNhan nsdn ON nsdn.iID_DotNhan = dnct.iID_DotNhan 
							AND (@dNgayFrom IS NULL OR nsdn.dNgayQuyetDinh>= @dNgayFrom) 
							AND (@dNgayTo IS NULL OR nsdn.dNgayQuyetDinh <= @dNgayTo)
left join 
(
  select iID_Nguon from DM_Nguon where iID_Nguon not in
  (
    select distinct iID_NguonCha from DM_Nguon where bPublic = 1 and iID_NguonCha is not null
  )
) tbl1 on tbl1.iID_Nguon = dmn.iID_Nguon

LEFT JOIN (
SELECT iID_Nguon, sum(CASE WHEN sMaLoaiDuToan = '001' THEN SoTien else 0 END) as dagiaoDauNam,
sum(CASE WHEN sMaLoaiDuToan = '002' THEN SoTien else 0 END) as dagiaoChuyenSang,
sum(CASE WHEN sMaLoaiDuToan != '001' AND sMaLoaiDuToan != '002' THEN SoTien else 0 END) as dagiaoDuToan,
sum(case when SoTien is null then 0 else SoTien end) as dagiaoTongDuToan
from dutoandagiao GROUP BY iID_Nguon
) dagiao ON dagiao.iID_Nguon = dmn.iID_Nguon
WHERE dmn.iNamLamViec = @iNamLamViec
group by dmn.iID_Nguon, dmn.sMaCTMT,dmn.sLoai,dmn.sKhoan,dmn.sNoiDung, dmn.iSTT, dmn.location, tbl1.iID_Nguon, dmn.depth, dagiao.dagiaoDauNam, dagiao.dagiaoChuyenSang, dagiao.dagiaoDuToan, dagiao.dagiaoTongDuToan
ORDER BY cast('/' + replace(dmn.iSTT, '.', '/') + '/' as hierarchyid)

DROP TABLE #tmp