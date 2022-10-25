
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @username nvarchar(20)						set @username='tranhnh'
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan=null
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='44'
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach='1,2'
declare @dDotNgay datetime							set @dDotNgay='2018-04-17 00:00:00.0'
declare @sLNS nvarchar(20)							set @sLNS='2'

--#DECLARE#--

SELECT	*
FROM	DTBS_ChungTu
WHERE	iTrangThai=1 
		AND (@iID_MaPhongBan is null or iID_MaPhongBanDich=@iID_MaPhongBan)
		AND dNgayChungTu=@dDotNgay
		AND sID_MaNguoiDungTao=@username
		AND (sDSLNS like '107%' or sDSLNS like '2%' or sDSLNS like '3%' or sDSLNS like ',3' or sDSLNS like '4%' or sDSLNS like '%,4%')
