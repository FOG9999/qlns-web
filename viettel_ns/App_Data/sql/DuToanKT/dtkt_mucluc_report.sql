
declare @dvt int									set @dvt = 1000
declare @NamLamViec int								set @NamLamViec = 2018
declare @Id_DonVi nvarchar(20)						set @Id_DonVi='10'
declare @nganh nvarchar(1000)						set @nganh=null
declare @ng nvarchar(1000)							set @ng=null
declare @code nvarchar(1000)						set @code='1-1-01'

--#DECLARE#--

 
select		SUBSTRING(Code,1,1) as Code1
			, SUBSTRING(Code,1,3) as Code2
			, SUBSTRING(Code,1,6) as Code3
			, SUBSTRING(Code,1,9) as Code4
			, Code
			, Ng
			, Nganh
			, STT
			, sMoTa 
from		DTKT_MucLuc 
where		iTrangThai = 1
			and NamLamViec=@NamLamViec
			and Len(Code) = 12
order by	Code