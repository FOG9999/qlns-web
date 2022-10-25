_declare @NamLamViec int								set @NamLamViec = 2020
declare @NamNS int								set @NamNS = 2019
declare @M nvarchar(12)						set @M = null
declare @TM nvarchar(12)							set @TM = null
declare @TTM nvarchar(12)							set @TTM = null
declare @NG nvarchar(12)							set @NG = null


--#DECLARE#--/

select	mldt.Id,mldt.NamNS,mldt.M,mldt.Tm,mldt.Ttm,mldt.Ng,sMoTa,DacThu = CASE DacThu WHEN 1 THEN N'Đặc thù' ELSE N'Không phải đặc thù' END
from 
		(
		select	Id,NamNS,M,Tm,Ttm,Ng,DacThu
		from	SKT_MLDacThu
		where	NamLamViec = @NamLamViec	
				and (@NamNS is null or NamNS = @NamNS)
				and (@M is null or M like @M)
				and (@TM is null or TM like @TM)
				and (@TTM is null or TTM like @TTM)
				and (@NG is null or NG like @NG)
		) mldt

		left join 

		(
		select	sM,sTM,sTTM,sNG,sMoTa
		from	NS_MucLucNganSach
		where	iNamLamViec = (@NamLamViec - 1)
				and iTrangThai = 1
				and bLaHangCha = 0
				and sLNS = '1020100'
				and sL = '010' 
				and sK = '011'
		) mlns

		on mldt.M = mlns.sM and mldt.Tm = mlns.sTM and mldt.Ttm = mlns.sTTM and mldt.Ng = mlns.sNG
