
declare @NamLamViec nvarchar(2000)								set @NamLamViec = '2018,2019'

--#DECLARE#--

select		iNamLamViec as Nam
			, sXauNoiMa as XauNoiMa
			, sMoTa as MoTa
from		NS_MucLucNganSach
where		bLaHangCha = 0 
			and iTrangThai = 1
			and iNamLamViec in (select * from f_split(@NamLamViec))						
order by	Nam,sXauNoiMa
