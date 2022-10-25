DECLARE @iNguonNganSach uniqueidentifier set @iNguonNganSach = null
DECLARE @iNamLamViec int set @iNamLamViec = 2022
DECLARE @dDateFrom datetime set @dDateFrom = null
DECLARE @dDateTo datetime set @dDateTo = null
DECLARE @iSoCotBoSung int SET @iSoCotBoSung = 0

--#DECLARE#--

-- select distinct dt.dNgayQuyetDinh ,dt.iID_DuToan,
-- (ROW_NUMBER() OVER(ORDER BY dt.dNgayQuyetDinh ASC) + @iSoCotBoSung + 11) AS SoCot
-- from  NNS_DuToan dt 
-- where  dt.sMaLoaiDuToan != '001' and dt.sMaLoaiDuToan != '002'
	   -- AND dt.iNamLamViec = @iNamLamViec
 	   -- AND (@dDateFrom IS NULL OR @dDateFrom <= dt.dNgayQuyetDinh)
	   -- AND (@dDateTo IS NULL OR @dDateTo >= dt.dNgayQuyetDinh)
-- group by dt.dNgayQuyetDinh,dt.iID_DuToan


select dNgayQuyetDinh, sSoQuyetDinh, iID_DuToan, (ROW_NUMBER() OVER(ORDER BY dNgayQuyetDinh ASC, sSoQuyetDinh) + @iSoCotBoSung + 11) AS SoCot
from
(
	select distinct case when dt.dNgayQuyetDinh is not null then dt.dNgayQuyetDinh
	when dt.dNgayCongVan is not null then dt.dNgayCongVan end as dNgayQuyetDinh,
	case when dt.sSoQuyetDinh is not null then dt.sSoQuyetDinh
	when dt.sSoCongVan is not null then dt.sSoCongVan end as sSoQuyetDinh, 
	dt.iID_DuToan
	from  NNS_DuToan dt 
	where  dt.sMaLoaiDuToan != '001' and dt.sMaLoaiDuToan != '002'
		AND dt.iNamLamViec = @iNamLamViec
		AND (
			(dt.dNgayQuyetDinh is not null AND (
				(@dDateFrom IS NULL OR @dDateFrom <= dt.dNgayQuyetDinh)
				 AND (@dDateTo IS NULL OR @dDateTo >= dt.dNgayQuyetDinh)
			)) OR (dt.dNgayQuyetDinh is null AND dt.dNgayQuyetDinh is null AND dt.dNgayCongVan is not null AND (
				 (@dDateFrom IS NULL OR @dDateFrom <= dt.dNgayCongVan)
				 AND (@dDateTo IS NULL OR @dDateTo >= dt.dNgayCongVan)
			))
		)
) datadt
group by dNgayQuyetDinh, sSoQuyetDinh, iID_DuToan