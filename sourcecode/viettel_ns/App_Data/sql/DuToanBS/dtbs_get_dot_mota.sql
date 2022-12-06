

declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @username nvarchar(20)						set @username='tranhnh'
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='10' 
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='44'
declare @sLNS nvarchar(20)							set @sLNS='104%'
declare @dNgayChungTu datetime						set @dNgayChungTu='2018-04-17'

--###--


declare @results nvarchar(2000);

select @results = coalesce(@results + '; ', '') + sNoiDung
from DTBS_ChungTu
where	iTrangThai=1
		and iNamLamViec=@iNamLamViec
		and iID_MaPhongBan=@iID_MaPhongBan
		and sID_MaNguoiDungTao=@username
		and dNgayChungTu=@dNgayChungTu
		and (sDSLNS like @sLNS)

select @results
