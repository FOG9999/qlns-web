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
			, sTenPB= N'Bệnh viện'
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
						, iID_MaPhongBanDich	
						, rTuChi = SUM(rTuChi/@dvt)
						, rChiTapTrung = SUM(rTuChi/@dvt)-SUM(CASE WHEN iID_MaPhongBan IN ('07','10') AND sLNS IN ('1020100','1020000','1050000') THEN (rTuChi-rChiTapTrung)/@dvt ELSE 0 END)
						, rHienVat = SUM(rHienVat/@dvt)
			 FROM		DT_ChungTuChiTiet
			 WHERE		iTrangThai in (1,2)
						AND sLNS IN ('1020100','1020000','1050000')
						AND iNamLamViec=@nam 
						AND (LEN(iID_MaDonVi) > 2 or iID_MaDonVi in ('76','77','79'))
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
						, iID_MaPhongBanDich	
						, rTuChi = SUM(rTuChi/@dvt)
						, rChiTapTrung = SUM((rTuChi - CASE WHEN iID_MaPhongBanDich IN ('07','10') AND sLNS IN ('1020000','1020100') THEN rTuChi-rChiTapTrung ELSE 0 END)/@dvt)
						, rHienVat = SUM(rHienVat/@dvt)
			 FROM		DT_ChungTuChiTiet_PhanCap
			 WHERE		iTrangThai=1  
						AND (sLNS='1020100' OR sLNS='1020000')  
						AND iNamLamViec=@nam 
						AND (LEN(iID_MaDonVi) > 2 or iID_MaDonVi in ('76','77','79'))
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
