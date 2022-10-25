DECLARE @iNamLamViec int set @iNamLamViec = 2022
DECLARE @dNgayFrom datetime set @dNgayFrom = null
DECLARE @dNgayTo datetime set @dNgayTo = null
DECLARE @dvt int set @dvt = 1
declare @sTenCot nvarchar(max) set @sTenCot = '[8],[9],[10],[11],[12],[13],[14],[15],[16],[17],[18],[19],[24],[25],[26],[27]'

--#DECLARE#--

select distinct dn.dNgayQuyetDinh ,dn.iID_DotNhan,
(ROW_NUMBER() OVER(ORDER BY dn.dNgayQuyetDinh ASC) +7) AS SoCot into #tmpBoSung
from  NNS_DotNhan dn 
where  dn.sMaLoaiDuToan != '001' and dn.sMaLoaiDuToan != '002'
	   AND dn.iNamLamViec = @iNamLamViec
 	   AND (@dNgayFrom IS NULL OR @dNgayFrom <= dn.dNgayQuyetDinh)
	   AND (@dNgayTo IS NULL OR @dNgayTo >= dn.dNgayQuyetDinh)
group by dn.dNgayQuyetDinh,dn.iID_DotNhan

DECLARE @iSoCotBoSung int
SET @iSoCotBoSung = (SELECT COUNT(*) FROM #tmpBoSung)

-- temp thong tin so cot theo So Quyet dinh
select * INTO #tmpSoCot
FROM
(
select distinct dn.dNgayQuyetDinh, dn.sSoQuyetDinh, dn.iID_DotNhan as iID,
(ROW_NUMBER() OVER(ORDER BY dn.dNgayQuyetDinh ASC, sSoQuyetDinh) +7) AS SoCot
from  NNS_DotNhan dn 
where  dn.sMaLoaiDuToan != '001' and dn.sMaLoaiDuToan != '002'
	   AND dn.iNamLamViec = @iNamLamViec
 	   AND (@dNgayFrom IS NULL OR @dNgayFrom <= dn.dNgayQuyetDinh)
	   AND (@dNgayTo IS NULL OR @dNgayTo >= dn.dNgayQuyetDinh)
group by dn.dNgayQuyetDinh, dn.sSoQuyetDinh, dn.iID_DotNhan

union all

select dNgayQuyetDinh, sSoQuyetDinh, iID_DuToan as iID, (ROW_NUMBER() OVER(ORDER BY dNgayQuyetDinh ASC, sSoQuyetDinh) + @iSoCotBoSung + 11) AS SoCot 
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
				(@dNgayFrom IS NULL OR @dNgayFrom <= dt.dNgayQuyetDinh)
				 AND (@dNgayTo IS NULL OR @dNgayTo >= dt.dNgayQuyetDinh)
			)) OR (dt.dNgayQuyetDinh is null AND dt.dNgayQuyetDinh is null AND dt.dNgayCongVan is not null AND (
				 (@dNgayFrom IS NULL OR @dNgayFrom <= dt.dNgayCongVan)
				 AND (@dNgayTo IS NULL OR @dNgayTo >= dt.dNgayCongVan)
			))
		)
) datadt
group by dNgayQuyetDinh, sSoQuyetDinh, iID_DuToan
) as socot


DECLARE @parameter NVARCHAR(500)
SET @parameter = '@iNguonNganSach uniqueidentifier,
									@iNamLamViec int,
									@dNgayFrom datetime,
									@dNgayTo datetime,
									@iSoCotBoSung int,
									@dvt int'

DECLARE @sql nvarchar(max)

set @sql = 'select * from
(
select t.iID_Nguon, dsdotbosung.SoCot, SoTien = dsdotbosung.SoTien/@dvt from DM_Nguon t
--ORDER BY locations

left JOIN 
(
select dnct.iID_Nguon,dnct.SoTien,dn.dNgayQuyetDinh ,
#tmpSoCot.SoCot
from NNS_DotNhan dn
left join (
SELECT dnct.iID_Nguon, dn.iID_DotNhan, dnct.SoTien,dn.dNgayQuyetDinh
FROM NNS_DotNhan as dn
INNER JOIN NNS_DotNhanChiTiet as dnct on dn.iID_DotNhan = dnct.iID_DotNhan AND dnct.iID_Nguon = @iNguonNganSach
WHERE dn.iNamLamViec = @iNamLamViec 
AND (@dNgayFrom IS NULL OR @dNgayFrom <= dn.dNgayQuyetDinh)
AND (@dNgayTo IS NULL OR @dNgayTo >= dn.dNgayQuyetDinh)

) 
dnct ON dnct.iID_DotNhan = dn.iID_DotNhan
LEFT JOIN #tmpSoCot on #tmpSoCot.iID = dn.iID_DotNhan
where dn.sMaLoaiDuToan != ''001'' and dn.sMaLoaiDuToan != ''002''
AND dn.iNamLamViec = @iNamLamViec 
AND (@dNgayFrom IS NULL OR @dNgayFrom <= dn.dNgayQuyetDinh)
AND (@dNgayTo IS NULL OR @dNgayTo >= dn.dNgayQuyetDinh)
) dsdotbosung ON t.iID_Nguon = dsdotbosung.iID_Nguon
where t.iID_Nguon = @inguonngansach

union all
select t.iID_Nguon, dsdotdutoan.SoCot,
SoTien = dsdotdutoan.SoTien/@dvt from DM_Nguon t
left join (
select dtct.iid_nguon, dtct.sotien, #tmpSoCot.SoCot, case when dt.dNgayQuyetDinh is not null then dt.dNgayQuyetDinh
when dt.dNgayCongVan is not null then dt.dNgayCongVan end as dNgayQuyetDinh
from nns_dutoan dt
left join (SELECT iID_Nguon, iID_DuToan, dNgayQuyetDinh, sum(SoTien) as SoTien
from (
select noidungchi.iID_Nguon, case when dt.dNgayQuyetDinh is not null then dt.dNgayQuyetDinh
when dt.dNgayCongVan is not null then dt.dNgayCongVan end as dNgayQuyetDinh ,dt.iID_DuToan,dtct.SoTien
from NNS_DuToan dt 
INNER JOIN NNS_DuToanChiTiet as dtct on dt.iID_DuToan = dtct.iID_DuToan
INNER JOIN DM_NoiDungChi as noidungchi ON noidungchi.sMaNoiDungChi = dtct.sMaNoiDungChi
where dt.iNamLamViec = @iNamLamViec
 AND ((dt.dNgayQuyetDinh is not null AND (
 (@dNgayFrom IS NULL OR @dNgayFrom <= dt.dNgayQuyetDinh)
 AND (@dNgayTo IS NULL OR @dNgayTo >= dt.dNgayQuyetDinh)
 )) OR (dt.dNgayCongVan is not null AND (
 (@dNgayFrom IS NULL OR @dNgayFrom <= dt.dNgayCongVan)
 AND (@dNgayTo IS NULL OR @dNgayTo >= dt.dNgayCongVan)
 )))
) datadt
GROUP BY iID_Nguon, iID_DuToan, dNgayQuyetDinh
) 
dtct on dtct.iid_dutoan = dt.iid_dutoan
LEFT JOIN #tmpSoCot on #tmpSoCot.iID = dt.iID_DuToan
where dt.smaloaidutoan != ''001'' and dt.smaloaidutoan != ''002'' 
and dt.inamlamviec = @inamlamviec 
AND ((dt.dNgayQuyetDinh is not null AND (
 (@dNgayFrom IS NULL OR @dNgayFrom <= dt.dNgayQuyetDinh)
 AND (@dNgayTo IS NULL OR @dNgayTo >= dt.dNgayQuyetDinh)
 )) OR (dt.dNgayCongVan is not null AND (
 (@dNgayFrom IS NULL OR @dNgayFrom <= dt.dNgayCongVan)
 AND (@dNgayTo IS NULL OR @dNgayTo >= dt.dNgayCongVan)
 )))
) dsdotdutoan on t.iid_nguon = dsdotdutoan.iid_nguon
where t.iID_Nguon = @inguonngansach
) data

PIVOT(
SUM(SoTien) 
FOR SoCot IN ('+@sTenCot+')
) AS pivot_table;'

DECLARE @tblDmNguon TABLE (iID_Nguon uniqueidentifier)


insert into @tblDmNguon SELECT iID_Nguon FROM DM_Nguon

create table #tableTemp (
	iID_Nguon uniqueidentifier
)
		
DECLARE @columns NVARCHAR(MAX) = ''

SELECT  @columns += concat('c', SoCot) + ' decimal ,'
FROM #tmpSoCot
ORDER BY SoCot
SET @columns = LEFT(@columns, LEN(@columns) - 1)


DECLARE @sqlCreateTemp AS NVARCHAR(MAX)='';
set @sqlCreateTemp = 'alter table #tableTemp add '+@columns 


Exec(@sqlCreateTemp)

DECLARE @iID_Nguon uniqueidentifier

DECLARE cur CURSOR FOR SELECT iID_Nguon FROM @tblDmNguon
OPEN cur

FETCH NEXT FROM cur INTO @iID_Nguon

WHILE @@FETCH_STATUS = 0 BEGIN
    INSERT into #tableTemp EXEC SP_EXECUTESQL @sql, @parameter, @iNguonNganSach=@iID_Nguon, @iNamLamViec=@iNamLamViec, @dNgayFrom=@dNgayFrom,@dNgayTo=@dNgayTo,@iSoCotBoSung=@iSoCotBoSung, @dvt=@dvt
    FETCH NEXT FROM cur INTO @iID_Nguon
END

CLOSE cur    
DEALLOCATE cur

SELECT * FROM #tableTemp

--print @sql
--print @sqlCreateTemp

-- drop table #tableTemp
-- drop table #tmpSoCot