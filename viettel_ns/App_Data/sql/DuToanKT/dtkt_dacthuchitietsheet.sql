
declare @dvt int									set @dvt = 1000
declare @NamLamViec int								set @NamLamViec = 2018
declare @NganhBD nvarchar(2)						set @NganhBD=null
declare @M nvarchar(4)								set @M=null
declare @Tm nvarchar(4)								set @Tm=null
declare @TTm nvarchar(4)							set @TTm=null
declare @Ng nvarchar(2)								set @Ng =69

--#DECLARE#--
 
select	dtct.Id
		, iID_MaMucLucNganSach
		, ml.M
		, ml.Tm
		, ml.TTm
		, ml.Ng
		, ml.MoTa
		, TuChi = (dtct.TuChi/@dvt)
from 
		(select	iID_MaMucLucNganSach
				, M = sM
				, Tm = sTM
				, TTm = sTTM
				, Ng = sNG
				, MoTa = sMoTa
		 from	NS_MucLucNganSach
		 where	iTrangThai = 1
				and sLNS = '1040100'
				and iNamLamViec = @NamLamViec
				and sM <> '' 
				and sTM <> ''
				and sTTM <> ''
				and sNG IN (select distinct	Nganh 
							from			DTKT_ChungTuChiTiet
							where			NamLamViec = 2018 and DacThu <> 0)) as ml

		left join 

		(select Id,M,Tm,TTm,Ng,TuChi=TuChi
		 from	DTKT_DacThuChiTiet
		 where	iTrangThai = 1
				and NamLamViec = @NamLamViec
				and TuChi <> 0) as dtct
				
		on	ml.M = dtct.M 
			and ml.Tm = dtct.Tm 
			and ml.TTm = dtct.TTm
			and ml.Ng = dtct.Ng 
			--and ml.MoTa = dtct.MoTa   
where	(@M is null or ml.M like @M)
		and (@Tm is null or ml.Tm like @Tm)
		and (@TTm is null or ml.TTm like @TTm)
		and (@Ng is null or ml.Ng like @Ng)
order by Ng, M, Tm, TTm
