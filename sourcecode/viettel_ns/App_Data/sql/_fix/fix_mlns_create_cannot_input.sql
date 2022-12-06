
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iID_MaNamNganSach nvarchar(200)			set @iID_MaNamNganSach='1,2'
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='99'
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='10'
declare @username nvarchar(20)						set @username='quynhnl'
declare @dMaDot datetime							set @dMaDot = '2018-01-28 00:00:00.000'

--###--
 

 --select * from dbo.[f_mlns](2018)
 --order by sXauNoiMa

 select * from NS_MucLucNganSach
 where iNamLamViec=2018
		and sXauNoiMa='2070000-340-341-7000-7049-10-40'

		
 select * from NS_MucLucNganSach
 where iNamLamViec=2018
		and sXauNoiMa='2070000'
