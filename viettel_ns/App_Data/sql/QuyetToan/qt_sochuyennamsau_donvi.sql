
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2019
declare @username nvarchar(20)						set @username='quynhnl'
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='06'
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi=NULL
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach='2'
declare @iID_MaNguonNganSach nvarchar(20)			set @iID_MaNguonNganSach='1'

 
--###--

--select distinct iID_MaDonVi, RTRIM(LTRIM(iID_MaDonVi)) + ' - ' + sTenDonVi as sTenDonVi
--from	QTA_ChungTuChiTiet 
--where	iTrangThai=1 
--		and iNamLamViec=@iNamLamViec
--		and (@iID_MaDonVi is null or iID_MaDonVi in (select * from f_split(@iID_MaDonVi)))
--		and (@iID_MaPhongBan is null or iID_MaPhongBan=@iID_MaPhongBan)
--		and (rDaCapTien<>0 or rChuaCapTien<>0)
--order by iID_MaDonVi

select distinct iID_MaDonVi, 
		sTenDonVi
		--RTRIM(LTRIM(iID_MaDonVi)) + ' - ' + sTenDonVi as sTenDonVi
from	DTBS_ChungTuChiTiet 
where	iTrangThai=1 
		and iNamLamViec=@iNamLamViec
		and (@iID_MaDonVi is null or iID_MaDonVi in (select * from f_split(@iID_MaDonVi)))
		and (@iID_MaPhongBan is null or iID_MaPhongBan=@iID_MaPhongBan)
		and iID_MaNamNganSach in (1,4,5)
		and (rTuChi<>0 or rHangNhap<>0 or rHangMua<>0)
order by iID_MaDonVi
