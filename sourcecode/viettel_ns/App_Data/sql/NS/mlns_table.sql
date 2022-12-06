
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @username nvarchar(20)						set @username='chuctc'
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07' 
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='44'
declare @iID_MaChungTu nvarchar(200)				set @iID_MaChungTu='2D705E35-6A6B-4BFE-86F0-AB055998A082'
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach='1,2'
declare @dMaDot datetime							set @dMaDot='2018-03-10 00:00:00.0'

--#DECLARE#--


select * from NS_MucLucNganSach 
where	iTrangThai=1 
		and iNamLamViec=@iNamLamViec 
		and sLNS !='' 
		and (@sLNS is null or sLNS like @sLNS) 
		and (@sL is null or sL like @sL) 
		and (@sK is null or sK like @sK) 
		and (@sM is null or sM like @sM) 
		and (@sTM is null or sTM like @sTM) 
		and (@sTTM is null or sTTM like @sTTM) 
		and (@sNG is null or sNG like @sNG)
order by sXauNoiMa
