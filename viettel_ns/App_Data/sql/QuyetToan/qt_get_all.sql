
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2017
declare @username nvarchar(20)						set @username='quynhnl'
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='10'
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='29'
declare @iID_MaNganh nvarchar(20)					set @iID_MaNganh=51
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach='1,2'
declare @iID_MaNguonNganSach nvarchar(20)			set @iID_MaNguonNganSach='1'
declare @dDotNgay datetime							set @dDotNgay='2018-03-28 00:00:00.0'
declare @iID_MaChungTu nvarchar(200)				set @iID_MaChungTu='f053c61e-6041-46ce-a87c-8c2f9aef12cc,fa234f87-000c-4605-9f82-e49cf2a08fae'

 
--###--



select * from 
(

 select	iID_MaChungTu,
		iID_MaDonVi,
		iID_MaPhongBan,
		iSoChungTu,
		sSoChungTu	= (sTienToChungTu+CONVERT(nvarchar,iSoChungTu)),
		sDSLNS,
		iThang_Quy,
		sNoiDung,
		iID_MaTrangThaiDuyet,
		dNgayChungTu

 from	QTA_ChungTu
 where	iTrangThai=1
		and iNamLamViec=@iNamLamViec
		and iID_MaNamNganSach=2
		and (@iID_MaPhongBan is null or iID_MaPhongBan=@iID_MaPhongBan)
		and (@username is null or sID_MaNguoiDungTao=@username)

) as T
join (select iID_MaDonVi as id_dv, sTenDonVi = (iID_MaDonVi + ' - ' + sTen) from NS_DonVi where iNamLamViec_DonVi=@iNamLamViec) as dv on dv.id_dv = iID_MaDonVi
join (select iID_MaTrangThaiDuyet as id_duyet, sTen as sTrangThaiDuyet from NS_PhanHe_TrangThaiDuyet where iID_MaPhanHe=5) as duyet on duyet.id_duyet = iID_MaTrangThaiDuyet

order by dNgayChungTu, iSoChungTu
