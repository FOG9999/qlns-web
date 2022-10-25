
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @username nvarchar(20)						set @username='quynhnl'
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='06' 
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='44'
declare @iID_MaChungTu nvarchar(200)				set @iID_MaChungTu='2D705E35-6A6B-4BFE-86F0-AB055998A082'
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach='1,5'
declare @dMaDot datetime							set @dMaDot='2018-03-10 00:00:00.0'

--#DECLARE#--

select distinct iID_MaDonVi,sTenDonVi
from	DTBS_ChungTuChiTiet
where	iTrangThai=1
		and iNamLamViec=@iNamLamViec
		and iID_MaNamNganSach in (select * from F_Split(@iID_MaNamNganSach))
		and (@iID_MaPhongBan is null or iID_MaPhongBanDich=@iID_MaPhongBan)
        and (MaLoai='' OR MaLoai='2')
		and LEFT(sLNS,3)='104'
