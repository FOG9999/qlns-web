DECLARE @iNguonNganSach uniqueidentifier set @iNguonNganSach = 'AC95369D-18E3-4450-A35A-0736A6889BAE'
DECLARE @iNamLamViec int set @iNamLamViec = 2019
DECLARE @dDateFrom datetime set @dDateFrom = null
DECLARE @dDateTo datetime set @dDateTo = null
DECLARE @iSoCotBoSung int SET @iSoCotBoSung = 0

--#DECLARE#--

select  dtct.iID_Nguon,dtct.SoTien,dt.dNgayQuyetDinh ,
(ROW_NUMBER() OVER(ORDER BY dt.dNgayQuyetDinh ASC) + @iSoCotBoSung + 11) AS SoCot
from NNS_DuToan dt
left join (
	SELECT noidungchi.iID_Nguon, dt.iID_DuToan,dt.dNgayQuyetDinh, sum(dtct.SoTien) as SoTien
	FROM NNS_DuToan as dt
	INNER JOIN NNS_DuToanChiTiet as dtct on dt.iID_DuToan = dtct.iID_DuToan
	INNER JOIN DM_NoiDungChi as noidungchi ON noidungchi.sMaNoiDungChi = dtct.sMaNoiDungChi
	WHERE dt.iNamLamViec = @iNamLamViec AND noidungchi.iID_Nguon = @iNguonNganSach
		AND (@dDateFrom IS NULL OR @dDateFrom <= dt.dNgayQuyetDinh)
		AND (@dDateTo IS NULL OR @dDateTo >= dt.dNgayQuyetDinh)
	GROUP BY noidungchi.iID_Nguon, dt.iID_DuToan, dt.dNgayQuyetDinh
) 
dtct ON dtct.iID_DuToan = dt.iID_DuToan
where dt.sMaLoaiDuToan != '001' and dt.sMaLoaiDuToan != '002' 
AND dt.iNamLamViec = @iNamLamViec 
AND (@dDateFrom IS NULL OR @dDateFrom <= dt.dNgayQuyetDinh)
AND (@dDateTo IS NULL OR @dDateTo >= dt.dNgayQuyetDinh)