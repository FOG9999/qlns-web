
declare @NamLamViec int								set @NamLamViec = 2020
declare @Nganh nvarchar(1000)						set @Nganh=null
declare @Nganh_Parent nvarchar(1000)							set @Nganh_Parent=null
declare @byNg nvarchar(1000)						set @byNg=null
declare @loai nvarchar(10)							set @loai = null
declare @KyHieu nvarchar(12)							set @KyHieu = null


--#DECLARE#--

 
select		Id
			, Id_Parent
			, IsParent
			, KyHieu
			, Loai
			, Nganh_Parent
			, Nganh
			, M
			, STT
			, MoTa 
			, KyHieuCha
from		SKT_MLSKTNT 
where		NamLamViec=@NamLamViec
			and (@Loai is null or Loai like @Loai)
			and (@KyHieu is null or KyHieu like @KyHieu)
			and (@Nganh is null or Nganh like @Nganh)
			and (@M is null or M like @M)
			and (Nganh_Parent is null or @Nganh_Parent is null or Nganh_Parent like @Nganh_Parent)
order by	KyHieu
