
declare @dvt int									set @dvt = 1000
declare @nam int									set @nam = 2018

--#DECLARE#--

select		SUBSTRING(KyHieu,1,1) as KyHieu1, 
			SUBSTRING(KyHieu,1,3) as KyHieu2, 
			SUBSTRING(KyHieu,1,6) as KyHieu3, 
			KyHieu, 
			STT,
			IsParent, 
			MoTa 
from		SKT_MucLuc
where		NamLamViec=@nam
order by	KyHieu