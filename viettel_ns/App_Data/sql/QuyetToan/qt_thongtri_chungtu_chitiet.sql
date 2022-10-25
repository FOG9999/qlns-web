
declare @dvt int									set @dvt = 1000
declare @iID_MaChungTu nvarchar(100)				set @iID_MaChungTu= 'd137c0a5-5daf-4c15-8fbf-56e9df15e10b'
declare @sLNS nvarchar(200)							set @sLNS=null

 
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
					and (@sLNS is null or sLNS in (select * from F_Split(@sLNS)))
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
		 