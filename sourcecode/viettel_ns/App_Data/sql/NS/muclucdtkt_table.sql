
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @username nvarchar(20)						set @username='chuctc'
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07' 
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='44'
declare @iID_MaChungTu nvarchar(200)				set @iID_MaChungTu='2D705E35-6A6B-4BFE-86F0-AB055998A082'
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach='1,2'
declare @dMaDot datetime							set @dMaDot='2018-03-10 00:00:00.0'

--#DECLARE#--


select * from DTKT_MucLuc
where	Status=1 
		and WorkingYear=@iNamLamViec 
		and Code !=''
order by Code
