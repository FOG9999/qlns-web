declare @dvt int									set @dvt = 1
declare @iNamLamViec int							set @iNamLamViec = 2019
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='10' 
declare @iID_MaNguonNganSach nvarchar(20)			set @iID_MaNguonNganSach = '1'
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach = '2,4'
declare @iID_MaDonVi nvarchar(10)					set @iID_MaDonVi = '78'
--declare @dMaDot datetime							set @dMaDot=GetDATE()
declare @dMaDot datetime							set @dMaDot='2019-02-19 00:00:00'
DECLARE @@sLNS NVARCHAR(MAX)						SET @@sLNS = '4%'

--#DECLARE#--

select	sLNS
		, sL
		, sK
		, sM
		, sTM
		, sTTM
		, sNG
		, rTuChi = sum(rTien)
from	f_ns_table_chitieu_tien(@iNamLamViec,null,@iID_MaPhongBan,@iID_MaNamNganSach,@dMaDot,@dvt,null)
where	iID_MaDonVi like @iID_MaDonVi + '%'
group by sLNS
		, sL
		, sK
		, sM
		, sTM
		, sTTM
		, sNG