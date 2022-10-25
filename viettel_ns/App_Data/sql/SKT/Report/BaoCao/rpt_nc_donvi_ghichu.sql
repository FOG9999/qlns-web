
declare @dvt int									set @dvt = 1000
declare @NamLamViec int								set @NamLamViec = 2020
--declare @Id_DonVi nvarchar(20)						set @Id_DonVi='10'
declare @Id_DonVi nvarchar(20)						set @Id_DonVi='53'
declare @Id_PhongBan nvarchar(20)					set @Id_PhongBan='10'
declare @Id_PhongBanDich nvarchar(20)				set @Id_PhongBanDich='10'
declare @nganh nvarchar(1000)						set @nganh=null
declare @ng nvarchar(1000)							set @ng=null
declare @code nvarchar(1000)						set @code='1-1-01'
declare @loai nvarchar(1000)						set @loai='2'

--#DECLARE#--

 select Id_ChiTiet, Id_ChungTu, Id_MucLuc, ml.Nganh, mlns.sMoTa, ml.KyHieu, ml.MoTa, GhiChu from
(
select Id as Id_ChiTiet, Id_ChungTu, Id_MucLuc, GhiChu 
from	SKT_ChungTuChiTiet
where	GhiChu is not null
		and Id_ChungTu in (select id from SKT_ChungTu
						where	NamLamViec=@NamLamViec
								and iLoai=@loai
								and Id_PhongBan=@Id_PhongBan
								and Id_DonVi=@Id_DonVi)) as ct
left join (select * from SKT_MLNhuCau where NamLamViec=@NamLamViec) as ml
on ct.Id_MucLuc=ml.Id

left join (select sMoTa, sNg from NS_MucLucNganSach where iNamLamViec = @NamLamViec and sNg <> '' and sLNS = '') as mlns
on ml.Nganh = mlns.sNG

order by KyHieu