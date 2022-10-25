
declare @dvt int									set @dvt = 1000
declare @NamLamViec int								set @NamLamViec = 2018
declare @Id_DonVi nvarchar(20)						set @Id_DonVi='10'
declare @nganh nvarchar(1000)						set @nganh=null
declare @ng nvarchar(1000)							set @ng=null
declare @code nvarchar(1000)						set @code='1-1-01'

--#DECLARE#--

 
select		SUBSTRING(KyHieu,1,1) as KyHieu1
			, SUBSTRING(KyHieu,1,3) as KyHieu2
			, SUBSTRING(KyHieu,1,6) as KyHieu3
			, SUBSTRING(KyHieu,1,9) as KyHieu4
			, KyHieu
			, Nganh_Parent
			, Nganh
			, STT
			, STTBC
			, MoTa 
from		SKT_MLNhuCau 
where		NamLamViec=@NamLamViec
			and Len(KyHieu) = 12
order by	KyHieu