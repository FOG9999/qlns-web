declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07' 
declare @iID_MaNguonNganSach nvarchar(20)			set @iID_MaNguonNganSach = '1'
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach = '2'
declare @sLNS nvarchar(20)							set @sLNS = '4080300'
declare @dNgayCapPhat datetime						set @dNgayCapPhat = '2/26/2018 12:00:00 AM'

--#DECLARE#--

SELECT	sLNS1	=LEFT(sLNS,1)
		, sLNS3	=LEFT(sLNS,3)
		, sLNS5	=LEFT(sLNS,5)
		, sLNS
		, sL
		, sK
		, sM
		, sTM
		, sTTM
		, sNG
		, sMoTa
		, rTuChi	
FROM	(SELECT		sLNS
					, sL
					, sK
					, sM
					, sTM
					, sTTM
					, sNG
					, sMoTa
					, rTuChi = SUM(rTuChi)
		 FROM		CP_CapPhatChiTiet
		 WHERE		iTrangThai=1 
					AND iID_MaDonVi = @iID_MaDonVi
					AND (@sLNS IS NULL OR sLNS IN (SELECT * FROM F_Split(@sLNS)))
					AND iID_MaCapPhat = @iID_MaCapPhat 
		 GROUP by	sLNS
					, sL
					, sK
					, sM
					, sTM
					, sTTM
					, sNG
					, sMoTa
		 HAVING		SUM(rTuChi)<>0) AS r
ORDER BY sLNS
		 , sL
		 , sK
		 , sM
		 , sTM
		 , sTTM
		 , sNG
		 , sMoTa
		 