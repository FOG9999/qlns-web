declare @NamLamViec int								set @NamLamViec = 2020
declare @KyHieu nvarchar(12)						set @KyHieu = null
declare @Nganh nvarchar(12)							set @Nganh = '72'

--#DECLARE#--/

select		mlnc.*,Id,Id_PhongBans,Map = CASE WHEN idmap IS NOT NULL THEN N'Khóa' ELSE N'Không khóa' end
from

			(select		Id as id_MLNC
						, Nganh
						, KyHieu			
						, STT + ' ' + MoTa as MoTa
			from		SKT_MLNhuCau
			where		NamLamViec=@NamLamViec						
						and IsParent = 0) mlnc

			full join 

			(
			select		DISTINCT Id,Id_PhongBans, Id_MLNC as idmap
			from		SKT_EXCLUDE
			where		NamLamViec = @NamLamViec
			) map
			on 
			mlnc.id_MLNC =  map.idmap
where		(@KyHieu is null or KyHieu like @KyHieu)
			and (@Nganh is null or Nganh like @Nganh)
order by	KyHieu