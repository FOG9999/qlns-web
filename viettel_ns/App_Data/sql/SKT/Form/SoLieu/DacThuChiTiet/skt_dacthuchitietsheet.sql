
declare @dvt int									set @dvt = 1000
declare @NamLamViec int								set @NamLamViec = 2019
declare @id_chungtu uniqueidentifier				set @id_chungtu = '69DBC1C6-6002-41EC-A1A1-A036BC0388F8'
declare @nganh nvarchar(2000)						set @nganh='27,28,29'
declare @M nvarchar(4)								set @M=null
declare @Tm nvarchar(4)								set @Tm=null
declare @TTm nvarchar(4)							set @TTm=null
declare @Ng nvarchar(2)								set @Ng =null

--#DECLARE#--
 
select		dtct.Id
			, ml.*
			, TuChi = ISNULL((dtct.TuChi/@dvt),0)
from
			(select	iID_MaMucLucNganSach as Id_MaMLNS
					, M = sM
					, TM = sTM
					, TTM = sTTM
					, NG = sNG
					, MoTa = sMoTa
			 from	NS_MucLucNganSach
			 where	iTrangThai = 1
					and sLNS = '1040100'
					and iNamLamViec = @NamLamViec
					and sM <> '' 
					and sTM <> ''
					and sTTM <> ''
					and sNG IN (select * from f_split(@nganh))) as ml

			full join 

			(select Id,Id_MLNS,TuChi
			 from	SKT_DacThuChiTiet
			 where	Id_ChungTu = @id_chungtu) as dtct
				
			on	ml.Id_MaMLNS = dtct.Id_MLNS
where		(@M is null or ml.M like @M)
			and (@TM is null or ml.TM like @TM)
			and (@TTM is null or ml.TTM like @TTM)
			and (@NG is null or ml.NG like @NG)
order by	NG, M, TM, TTM