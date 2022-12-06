DECLARE @iNamLamViec int set @iNamLamViec = 2019

--#DECLARE#--

select ndc.*, (ISNULL(dotnhan.SoTien, 0) - ISNULL(dutoanchitiet.SoTien, 0)) as SoTien from orderedTree_view_dmnoidungchi ndc
left JOIN (
SELECT iID_NoiDungChi, sum(SoTien) as SoTien FROM NNS_DotNhanChiTiet_NDChi WHERE iNamLamViec = @iNamLamViec GROUP BY iID_NoiDungChi
) as dotnhan ON dotnhan.iID_NoiDungChi = ndc.iID_NoiDungChi 
left JOIN (
SELECT sMaNoiDungChi, sum(SoTien) as SoTien FROM NNS_DuToanChiTiet WHERE iNamLamViec = @iNamLamViec GROUP BY sMaNoiDungChi
) AS dutoanchitiet ON dutoanchitiet.sMaNoiDungChi = ndc.sMaNoiDungChi
where iNamLamViec = @iNamLamViec AND bLaHangCha = 0
order by sMaNoiDungChi