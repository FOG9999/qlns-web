
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2017
declare @iThang_Quy int								set @iThang_Quy = '1'
declare @username nvarchar(20)						set @username='quynhnl'
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='10'
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach='1,2'
declare @sLNS nvarchar(200)							set @sLNS='1010000'

 
--###--


select distinct iID_MaDonVi, sMoTa = (iID_MaDonVi + ' - ' + dv.sTen)
from	QTA_ChungTuChiTiet
inner join (select iID_MaDonVi as id, sTen from NS_DonVi where iTrangThai=1 and iNamLamViec_DonVi=@iNamLamViec) as dv on QTA_ChungTuChiTiet.iID_MaDonVi=dv.id
where	iTrangThai=1
		and iNamLamViec=@iNamLamViec
		and (@iID_MaPhongBan is null or iID_MaPhongBan=@iID_MaPhongBan)
		and iThang_Quy=@iThang_Quy
		and (sLNS in (select * from F_Split(@sLNS)))
		and (@username is null or sID_MaNguoiDungTao=@username)
		and (@iID_MaNamNganSach is null or iID_MaNamNganSach in (select * from F_Split(@iID_MaNamNganSach)))
		 