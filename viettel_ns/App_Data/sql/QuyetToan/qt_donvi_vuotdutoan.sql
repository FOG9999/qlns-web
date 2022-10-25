
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2017
declare @username nvarchar(20)						set @username='quynhnl'
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07'
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi=NULL
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach='2'
declare @iID_MaNguonNganSach nvarchar(20)			set @iID_MaNguonNganSach='1'

 
--###--

select distinct iID_MaDonVi, RTRIM(LTRIM(iID_MaDonVi)) + ' - ' + sTenDonVi as sTenDonVi
from	QTA_ChungTuChiTiet 
where	iTrangThai=1 
		and iNamLamViec=@iNamLamViec
		and iID_MaNamNganSach in (select * from f_split(@iID_MaNamNganSach))
		and (@iID_MaDonVi is null or iID_MaDonVi in (select * from f_split(@iID_MaDonVi)))
		and (@iID_MaPhongBan is null or iID_MaPhongBan=@iID_MaPhongBan)
		and (rVuotChiTieu<>0 or rTonThatTonDong<>0)
order by iID_MaDonVi
 