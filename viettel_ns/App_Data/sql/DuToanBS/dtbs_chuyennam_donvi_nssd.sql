
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2019
declare @username nvarchar(20)						set @username=null
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan=null
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='41'
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach='1,4,5'

--#DECLARE#--

select distinct iID_MaDonVi,sTenDonVi
from	DTBS_ChungTuChiTiet
where	iTrangThai=1
		and iNamLamViec=@iNamLamViec
		and iID_MaNamNganSach in (select * from F_Split(@iID_MaNamNganSach))
		and (@iID_MaPhongBan is null or iID_MaPhongBanDich=@iID_MaPhongBan)
		and (@username is null or sID_MaNguoiDungTao=@username)
		--and LEFT(sLNS,3) != '104'
		and iID_MaDonVi != ''
