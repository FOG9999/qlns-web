declare @dvt int									set @dvt = 1000
declare @NamLamViec int								set @NamLamViec = 2020
declare @Loai int									set @Loai = null
declare @KyHieu nvarchar(12)						set @KyHieu = null
declare @NamNS nvarchar(9)							set @NamNS = '2018'


--#DECLARE#--

select		mlnc.*, Map = CASE WHEN idmap IS NOT NULL THEN N'Đã map' ELSE N'Chưa map' end
from
			(
			select		Id
						, Loai = CASE WHEN	KyHieu LIKE '1-2%' THEN N'Ngân sách nghiệp vụ'
									  WHEN  KyHieu LIKE '1-3%' THEN N'Việc nhà nước giao tính vào KPQP'
									  WHEN  KyHieu LIKE '2%' THEN N'Chi hoạt động sự nghiệp'
									  WHEN  KyHieu LIKE '3%' THEN N'Ngân sách nhà nước khác'
									  ELSE N'Bảo đảm đời sống' END
						, Nganh_Parent
						, Nganh
						, KyHieu
						, MoTa
			from		SKT_MLNhuCau 
			where		NamLamViec=@NamLamViec
						and IsParent = 0) mlnc

			full join 

			(
			select		DISTINCT Id_MLNhuCau as idmap
			from		SKT_MLNS
			where		NamLamViec = @NamLamViec
			) map
			on 
			mlnc.Id =  map.idmap
where		(@KyHieu is null or KyHieu like @KyHieu)
			and (@Nganh is null or Nganh like @Nganh)
			and (@Nganh_Parent is null or Nganh_Parent like @Nganh_Parent)
order by	KyHieu
			