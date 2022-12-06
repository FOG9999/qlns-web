
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2017
declare @iThang_Quy int								set @iThang_Quy = '1'
declare @username nvarchar(20)						set @username='quynhnl'
declare @iID_MaDonVi nvarchar(200)					set @iID_MaDonVi='76'
declare @iID_MaPhongBan nvarchar(2)					set @iID_MaPhongBan='10'
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach='1,2'
declare @sLNS nvarchar(200)							set @sLNS='1010000'

 
--###--

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
					, rTuChi =sum(rTuChi)/@dvt
		 FROM		QTA_ChungTuChiTiet
		 WHERE		iTrangThai=1
					and iID_MaChungTu = @iID_MaChungTu
					and iNamLamViec=@iNamLamViec
					and (@iThang_Quy is null or iThang_Quy=@iThang_Quy)
					and (@iID_MaPhongBan is null or iID_MaPhongBan=@iID_MaPhongBan)
					and (@iID_MaDonVi is null or iID_MaDonVi in (select * from F_Split(@iID_MaDonVi)))
					and (@sLNS is null or sLNS in (select * from F_Split(@sLNS)))
					and (@iID_MaNamNganSach is null or iID_MaNamNganSach in (select * from F_Split(@iID_MaNamNganSach)))
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
		 