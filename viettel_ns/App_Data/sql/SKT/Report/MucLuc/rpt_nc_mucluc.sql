
declare @dvt int									set @dvt = 1000
declare @NamLamViec int								set @NamLamViec = 2020
declare @Id_DonVi nvarchar(20)						set @Id_DonVi='10'
declare @nganh nvarchar(1000)						set @nganh=null
declare @ng nvarchar(1000)							set @ng=null
declare @code nvarchar(1000)						set @code='1-1-01'

--#DECLARE#--

 
select		
		X1=SUBSTRING(KyHieu,1,1),
		X2=SUBSTRING(KyHieu,1,3),
		X3=SUBSTRING(KyHieu,1,6),
		X4=SUBSTRING(KyHieu,1,9)
		,KyHieu
		,Nganh_Parent
		,Nganh
		,STT
		,MoTa 
from	SKT_MLNhuCau 
where	NamLamViec=@NamLamViec
		and Len(KyHieu) = 12
order by	KyHieu