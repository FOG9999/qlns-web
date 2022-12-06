DECLARE @iNguonNganSach uniqueidentifier set @iNguonNganSach = null
DECLARE @iNamLamViec int set @iNamLamViec = 2022
DECLARE @dDateFrom datetime set @dDateFrom = null
DECLARE @dDateTo datetime set @dDateTo = null
DECLARE @iSoCotBoSung int SET @iSoCotBoSung = 0

--#DECLARE#--

/*

lấy các đợt nhận trong năm

*/

select distinct dn.dNgayQuyetDinh, dn.sSoQuyetDinh, dn.iID_DotNhan,
(ROW_NUMBER() OVER(ORDER BY dn.dNgayQuyetDinh ASC, dn.sSoQuyetDinh) +7) AS SoCot
from  NNS_DotNhan dn 
where  dn.sMaLoaiDuToan != '001' and dn.sMaLoaiDuToan != '002'
	   AND dn.iNamLamViec = @iNamLamViec
 	   AND (@dDateFrom IS NULL OR @dDateFrom <= dn.dNgayQuyetDinh)
	   AND (@dDateTo IS NULL OR @dDateTo >= dn.dNgayQuyetDinh)
group by dn.dNgayQuyetDinh, dn.sSoQuyetDinh, dn.iID_DotNhan
