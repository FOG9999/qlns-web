
declare @dvt int									set @dvt = 1000
declare @nam int									set @nam = 2018

--#DECLARE#--

select SUBSTRING(Code,1,1) as Code1, SUBSTRING(Code,1,3) as Code2, SUBSTRING(Code,1,6) as Code3, Code, STT,IsParent, sMoTa from DTKT_MucLuc
where	iTrangThai=1
	and NamLamViec=@nam
order by Code