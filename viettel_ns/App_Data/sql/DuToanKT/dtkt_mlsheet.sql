
declare @dvt int									set @dvt = 1000
declare @NamLamViec int								set @NamLamViec = 2018
declare @nganh nvarchar(1000)						set @nganh=null
declare @ng nvarchar(1000)							set @ng=null
declare @code nvarchar(1000)						set @code=null
declare @byNg nvarchar(1000)						set @byNg='29'
declare @loai nvarchar(10)							set @loai = '1'

--#DECLARE#--

 
select		Id
			, Id_Parent
			, IsParent
			, Loai
			, Code
			, Ng
			, Nganh
			, STT
			, sMoTa 
			,XauNoiMa
			,XauNoiMa_x
from		DTKT_MucLuc 
where		iTrangThai = 1
			and NamLamViec=@NamLamViec
			and (@code is null or Code like @code)
			and (Nganh='' or @nganh is null or Nganh like @nganh)
			and (Ng is null or @ng is null or Ng like @ng)
order by	Code
