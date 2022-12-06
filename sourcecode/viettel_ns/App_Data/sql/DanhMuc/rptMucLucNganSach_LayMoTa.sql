declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07' 
declare @iID_MaNguonNganSach nvarchar(20)			set @iID_MaNguonNganSach = '1'
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach = '2'
declare @sLNS nvarchar(20)							set @sLNS = '104%'
declare @dNgayCapPhat datetime						set @dNgayCapPhat = '2/26/2018 12:00:00 AM'

--#DECLARE#--/

SELECT	sMoTa 
FROM	NS_MucLucNganSach
WHERE	iTrangThai=1 
		AND iNamLamViec = @iNamLamViec 
		AND sLNS = @sLNS 
		AND (@sL IS NULL OR sL IN (SELECT * FROM F_Split(@sL)))
		AND (@sK IS NULL OR sK IN (SELECT * FROM F_Split(@sK)))
		AND (@sM IS NULL OR sM = @sM) 
