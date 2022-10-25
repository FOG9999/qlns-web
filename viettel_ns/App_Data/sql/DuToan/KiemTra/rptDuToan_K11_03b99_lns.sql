declare @dvt int									set @dvt = 1
declare @nam int									set @nam = 2019
--declare @nganh	nvarchar(2000)						set @nganh='20,21,22,23,24,25,26,27,28,29,41,42,44,67,69,70,71,72,73,74,75,76,77,78,81,82'

--declare @nganh	nvarchar(2000)						set @nganh='60,61,62,64,65,66'
declare @nganh	nvarchar(2000)						set @nganh='01,02,03,04,05,07,07,08,09,10,11,12,26,27,28,55,57,60,61,62,64,65,66,67,69'

--declare @nganh	nvarchar(2000)						set @nganh='01'
declare @username nvarchar(2000)					set @username='baolq'
declare @id_phongban nvarchar(2)					set @id_phongban=null
declare @id_donvi nvarchar(2000)					set @id_donvi=null
declare @loai int									set @loai=null					--1: TuChi	2: HienVat
declare @iPhanCap int								set @iPhanCap=0					--1: TuChi	2: HienVat


--###--
 
 select *,iTrangThai=1, iNamLamViec=@nam from 
 (

	 select distinct sXauNoiMa,sNG,sMoTa
	 from	DT_ChungTuChiTiet
	 where	iTrangThai=1
			and iNamLamViec=@nam
			and sLNS in (1020100,1040100)
			and (rTuChi<>0 or rHangNhap<>0)
			and sNG in (select * from f_split(@nganh))
	) as a
	order by sNG
