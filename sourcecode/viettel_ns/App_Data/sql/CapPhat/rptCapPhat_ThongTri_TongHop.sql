declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07' 
declare @iID_MaNguonNganSach nvarchar(20)			set @iID_MaNguonNganSach = '1'
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach = '2'
declare @sLNS nvarchar(20)							set @sLNS = '4080300'
declare @dNgayCapPhat datetime						set @dNgayCapPhat = '2/26/2018 12:00:00 AM'

--#DECLARE#--


SELECT		SUBSTRING(sLNS,1,1) as sLNS1
			, SUBSTRING(sLNS,1,3) as sLNS3
			, SUBSTRING(sLNS,1,5) as sLNS5
			, sLNS
			, sL
			, sK
			, sM
			, sTM
			, sTTM
			, sNG
			, sMoTa 
			, iID_MaDonVi
			, sTenDonVi
			, SUM(rTuChi) as rTuChi
FROM		CP_CapPhatChiTiet
WHERE		iTrangThai=1 
			AND iNamLamViec=@iNamLamViec  
			AND (@sLNS IS NULL OR sLNS IN (SELECT * FROM F_Split(@sLNS)))
			AND iID_MaDonVi IN (SELECT * FROM F_Split(@iID_MaDonVi))
			AND iID_MaCapPhat = @iID_MaCapPhat 
GROUP BY	sLNS
			, sL
			, sK
			, sM
			, sTM
			, sTTM
			, sNG
			, sMoTa 
			, iID_MaDonVi
			, sTenDonVi
HAVING		SUM(rTuChi)<>0 