
declare @dvt int									set @dvt = 1000
declare @username nvarchar(20)						set @username='baolq'
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='10'
declare @dDotNgay nvarchar(10)						set @dDotNgay='12/01/2021'

--#DECLARE#--

SELECT	*
FROM	DTBS_ChungTu
WHERE	iTrangThai=1 
		AND (@iID_MaPhongBan is null or iID_MaPhongBanDich=@iID_MaPhongBan)
		AND CONVERT(nvarchar(10),dNgayChungTu,103)=@dDotNgay
		AND sID_MaNguoiDungTao=@username
		--and iID_MaChungTu = 'd4741bec-dae8-4f94-9969-f9f822afd77f'
		--AND (@sLNS is null or sDSLNS like @sLNS)
