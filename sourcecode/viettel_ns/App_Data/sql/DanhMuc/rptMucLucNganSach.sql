declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07' 
declare @iID_MaNguonNganSach nvarchar(20)			set @iID_MaNguonNganSach = '1'
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach = '2'
declare @sLNS nvarchar(20)							set @sLNS = '104%'
declare @dNgayCapPhat datetime						set @dNgayCapPhat = '2/26/2018 12:00:00 AM'

--#DECLARE#--/

SELECT		DISTINCT sLNS
			, sL
			, @@select
FROM		NS_MucLucNganSach
WHERE		iTrangThai=1 
			AND iNamLamViec=@iNamLamViec		
			AND (sLNS like @@sLNS)
			AND (@sM IS NULL OR sM = @sM)
ORDER BY	sLNS
			, sL
			, @@select
