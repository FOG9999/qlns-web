declare @dvt int									set @dvt = 1
declare @nam int									set @nam = 2019
--declare @nganh	nvarchar(2000)						set @nganh='20,21,22,23,24,25,26,27,28,29,41,42,44,67,69,70,71,72,73,74,75,76,77,78,81,82'

--declare @nganh	nvarchar(2000)						set @nganh='60,61,62,64,65,66'
declare @ng	nvarchar(2000)							set @ng='20,21,22,23,24,25,26,27,28,29,41,42,44,67,70,71,72,73,74,75,76,77,78,81,82'
declare @username nvarchar(2000)					set @username='baolq'
declare @id_phongban nvarchar(2)					set @id_phongban=null
declare @id_donvi nvarchar(2000)					set @id_donvi=null
declare @loai int									set @loai=null


--###--

SELECT		iID_MaNganh
			, sTenNganh
			, sLNS
			, sL
			, sK
			, sM
			, sTM
			, sTTM
			, sNG			
			, sMoTa = dbo.f_mlns_mota_xau(@nam,sLNS+'-'+sL+'-'+sK+'-'+sM+'-'+sTM+'-'+sTTM+'-'+sNG)	
			, iID_MaPhongBanDich
			, sTenPB=CASE iID_MaPhongBanDich WHEN '10' THEN N'@@sTenB10' 
											 WHEN '06' THEN N'@@sTenB6'
											 WHEN '07' THEN N'@@sTenB'
											 ELSE N'@@sTenHos' END
			, rTuChi = SUM(rTuChi)
			, rHienVat = SUM(rHienVat)
			, rChiTapTrung = SUM(rChiTapTrung)
FROM
-- Phần bìa bảo đảm--
			(
			SELECT		sLNS = '1020100'
						, sL
						, sK
						, sM
						, sTM
						, sTTM
						, sNG	
						, iID_MaPhongBanDich = case when LEN(iID_MaDonVi) > 2 or iID_MaDonVi in ('76','77','79') THEN 'BV' ELSE iID_MaPhongBanDich END	
						, rTuChi = SUM(rTuChi/@dvt)
						, rChiTapTrung = SUM(rTuChi/@dvt)-SUM(CASE WHEN (iID_MaPhongBan IN ('07','10') AND sLNS IN ('1020100','1020000') AND iID_MaChungTu <> '765ab85a-c301-4b3f-9277-0a8bfabfa361') OR (iID_MaChungTu = '765ab85a-c301-4b3f-9277-0a8bfabfa361' AND iID_MaDonVi <> '40') THEN (rTuChi-rChiTapTrung)/@dvt ELSE 0 END)
						, rHienVat = SUM(rHienVat/@dvt)
			 FROM		DT_ChungTuChiTiet
			 WHERE		iTrangThai <> 0
						AND sLNS IN ('1020100','1020000')
						AND iNamLamViec=@nam 
						@@DKPB @@DKDV 
			GROUP BY	sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaPhongBanDich,iID_MaDonVi
			HAVING		SUM(rTuChi/@dvt)<>0 
						OR SUM(rHienVat/@dvt)<>0 
						OR SUM(rChiTapTrung/@dvt)<>0

			UNION ALL

			SELECT		sLNS = '1020100'
						, sL
						, sK
						, sM
						, sTM
						, sTTM
						, sNG	
						, iID_MaPhongBanDich = case when LEN(iID_MaDonVi) > 2 or iID_MaDonVi in ('76','77','79') THEN 'BV' ELSE iID_MaPhongBanDich END	
						, rTuChi = SUM(rTuChi/@dvt)
						, rChiTapTrung = SUM((rTuChi - CASE WHEN iID_MaPhongBanDich IN ('07','10') AND sLNS IN ('1020000','1020100') THEN rTuChi-rChiTapTrung ELSE 0 END)/@dvt)
						, rHienVat = SUM(rHienVat/@dvt)
			 FROM		DT_ChungTuChiTiet_PhanCap
			 WHERE		iTrangThai=1  
						AND (sLNS='1020100' OR sLNS='1020000')  
						AND iNamLamViec=@nam 
						@@DKPB @@DKDV 
			GROUP BY	sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaPhongBanDich,iID_MaDonVi
			HAVING		SUM(rTuChi/@dvt)<>0 
						OR SUM(rHienVat/@dvt)<>0 
						OR SUM(rChiTapTrung/@dvt)<>0 
						
			) as result,
			(select sNG as iID_MaNganh, sMoTa as sTenNganh from NS_MucLucNganSach where iTrangThai = 1 and iNamLamViec=@nam and sLNS = '') as b
WHERE		result.sNG = b.iID_MaNganh 
GROUP BY	iID_MaNganh,sTenNganh,sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaPhongBanDich
ORDER BY	iID_MaNganh
			, sM
			, sTM
			, sTTM
			, sNG
			, sTenPB
