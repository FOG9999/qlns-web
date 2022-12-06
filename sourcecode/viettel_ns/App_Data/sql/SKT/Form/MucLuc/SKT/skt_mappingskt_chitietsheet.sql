
declare @dvt int									set @dvt = 1000
declare @NamLamViec int								set @NamLamViec = 2021
declare @Id_MLNhuCau nvarchar(1000)					set @Id_MLNhuCau ='4cf06880-cf2d-41e7-ab7e-819383d26a62'
declare @KyHieu nvarchar(12)						set @KyHieu =null
declare @Nganh nvarchar(12)							set @Nganh =null
declare @Nganh_Parent nvarchar(12)					set @Nganh_Parent =null
declare @loai nvarchar(12)							set @loai =1



--#DECLARE#--

select		Id,Id_MLNhuCau,Id_MaSKT,Nganh,Nganh_Parent,KyHieu,MoTa,Map = CASE WHEN Id Is not null then N'Chọn' else N'Không chọn' end
from	
			(select		Id,Id_MLNhuCau,Id_MLSKTNT
		     from		SKT_NCSKT
			 where		NamLamViec = @NamLamViec) map

			FULL JOIN

			(
			select		Id as Id_MaSKT
						, Nganh_Parent
						, Nganh
						, KyHieu
						, STT + ' ' + MoTa as MoTa						
			from		SKT_MLSKTNT
			where		NamLamViec = @NamLamViec						
						and IsParent = 0
						and (@KyHieu is null or KyHieu like @KyHieu)
						and (@Nganh is null or Nganh like @Nganh)
						and (Nganh_Parent is null or @Nganh_Parent is null or Nganh_Parent like @Nganh_Parent)	
						and KyHieu in (select KyHieu from SKT_ComDatasSKTNT	where NamLamViec = @NamLamViec)				
			) as ml
			ON map.Id_MLSKTNT = ml.Id_MaSKT		
where		(@loai = 1 and Id is null) or (@loai = 2 and Id is not null and Id_MLNhuCau = @Id_MLNhuCau)	
order by	KyHieu