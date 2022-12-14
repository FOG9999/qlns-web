declare @dvt int									set @dvt = 1
declare @nam int									set @nam = 2019
--declare @nganh	nvarchar(2000)						set @nganh='20,21,22,23,24,25,26,27,28,29,41,42,44,67,69,70,71,72,73,74,75,76,77,78,81,82'

--declare @nganh	nvarchar(2000)						set @nganh='60,61,62,64,65,66'
declare @nganh	nvarchar(2000)						set @nganh='04'
declare @username nvarchar(2000)					set @username='baolq'
declare @id_phongban nvarchar(2)					set @id_phongban=null
declare @id_donvi nvarchar(2000)					set @id_donvi=null
declare @loai int									set @loai=null


--###--

SELECT		sLNS
			, sL
			, sK
			, sM
			, sTM
			, sTTM
			, sNG
			, sMoTa
			, iID_MaDonVi
			, SUM(rTuChi) as rTuChi
			, SUM(rHienVat) as rHienVat
			, SUM(rChiTapTrung) as rChiTapTrung
FROM
			(
			SELECT		sLNS
						, sL
						, sK
						, sM
						, sTM
						, sTTM
						, sNG
						, sMoTa
						, iID_MaDonVi
						, rTuChi=SUM(rTuChi/@dvt)
						, rHienVat=SUM(rHienVat/@dvt)
						, rChiTapTrung=SUM(CASE WHEN iID_MaPhongBan='07' OR (iID_MaChungTu in ('765ab85a-c301-4b3f-9277-0a8bfabfa361') AND iID_MaDonVi = '40') THEN rTuChi/@dvt ELSE rChiTapTrung/@dvt END)
			 FROM		DT_ChungTuChiTiet
			 WHERE		iTrangThai=1  
						AND (sLNS='1020100' OR sLNS='1020000') 
						AND sNG <> '00' 
						AND {3}
						AND iID_MaDonVi = @iID_MaDonVi 
						AND iNamLamViec = @nam 
						{1} {2}
			 GROUP BY	sLNS
						, sL
						, sK
						, sM
						, sTM
						, sTTM
						, sNG
						, sMoTa
						, iID_MaDonVi
			 HAVING		SUM(rTuChi/@dvt)<>0 
						OR SUM(rHienVat/@dvt)<>0 
						OR SUM(rChiTapTrung/@dvt)<>0

			 UNION ALL

			 SELECT		sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi
						,rTuChi=SUM(rTuChi/@dvt)
						,rHienVat=SUM(rHienVat/@dvt)
						,rChiTapTrung=SUM(CASE WHEN iID_MaPhongBanDich='07' THEN rTuChi/@dvt ELSE rChiTapTrung/@dvt END)
			 FROM		DT_ChungTuChiTiet_PhanCap
			 WHERE		iTrangThai=1  
						AND (sLNS='1020100' OR sLNS='1020000')  
						AND {3}
						AND iID_MaDonVi =@iID_MaDonVi 
						AND iNamLamViec=@iNamLamViec
						{1} {2}
			GROUP BY	sLNS
						, sL
						, sK
						, sM
						, sTM
						, sTTM
						, sNG
						, sMoTa
						, iID_MaDonVi
			HAVING		SUM(rTuChi/@dvt)<>0 
						OR SUM(rHienVat/@dvt)<>0 
						OR SUM(rChiTapTrung/@dvt)<>0) a
GROUP BY	sLNS
			, sL
			, sK
			, sM
			, sTM
			, sTTM
			, sNG
			, sMoTa
			, iID_MaDonVi
HAVING		SUM(rTuChi/@dvt)<>0 
			OR SUM(rHienVat/@dvt)<>0
ORDER BY	sM,sTM,sTTM,sNG
