
declare @dvt int									set @dvt = 1
declare @nam int									set @nam = 2018 
--declare @nganh	nvarchar(2000)						set @nganh='20,21,22,23,24,25,26,27,28,29,41,42,44,67,69,70,71,72,73,74,75,76,77,78,81,82'
declare @nganh	nvarchar(2000)						set @nganh='30,31'
declare @username nvarchar(2000)					set @username='tranhnh'
declare @id_phongban nvarchar(2000)					set @id_phongban='10'


--###--

select distinct Ng, TenNganh from
(
select	Ng 
from	DTKT_ChungTuChiTiet
where	iTrangThai=1
		and NamLamViec=@nam
		and (@id_phongban is null or Id_PhongBanDich=@id_phongban)
		and iLoai=1
		and (@username is null or UserCreator=@username)
		and TuChi<>0
) as t1

-- lay ten nganh
inner join 
(select iID_MaNganh, iID_MaNganh + ' - ' + sTenNganh as TenNganh from NS_MucLucNganSach_Nganh where iNamLamViec=@nam) as nganh
on t1.Ng=nganh.iID_MaNganh
 