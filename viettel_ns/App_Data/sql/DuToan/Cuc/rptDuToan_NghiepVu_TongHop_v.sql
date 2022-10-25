declare @dvt int									set @dvt = 1
declare @nam int									set @nam = 2019
--declare @nganh	nvarchar(2000)						set @nganh='20,21,22,23,24,25,26,27,28,29,41,42,44,67,69,70,71,72,73,74,75,76,77,78,81,82'

--declare @nganh	nvarchar(2000)						set @nganh='60,61,62,64,65,66'
declare @ng	nvarchar(2000)							set @ng='20,21,22,23,24,25,26,27,28,29,41,42,44,67,70,71,72,73,74,75,76,77,78,81,82'
declare @username nvarchar(2000)					set @username='baolq'
declare @id_phongban nvarchar(2)					set @id_phongban='07'
declare @id_donvi nvarchar(2000)					set @id_donvi=null
declare @loai int									set @loai=null


--###--

select * from
(
	SELECT		iID_MaNganh
				, sTenNganh
				, sLNS
				, sL
				, sK
				, sM
				, sTM
				, sTTM
				, sNG			
				,sXauNoiMa=(sLNS+'-'+sL+'-'+sK+'-'+sM+'-'+sTM+'-'+sTTM+'-'+sNG)
				----, sMoTa = dbo.F_MLNS_MoTa_LNS(2018,sLNS,sL,sK,sM,sTM,sTTM,sNG)	
				, iID_MaPhongBanDich
				, sTenPB=CASE iID_MaPhongBanDich WHEN '10' THEN N'@@sTenB10' 
												 WHEN '07' THEN N'@@sTenB6'
												 ELSE  N'@@sTenB' END
				, rTuChi = SUM(rTuChi)
				, rHienVat = SUM(rHienVat)
				, rChiTapTrung = SUM(rChiTapTrung)
	FROM
	-- Phần bìa bảo đảm--
				(
				SELECT		sLNS = case when sLNS='1020000' then '1020100' else sLNS end
							, sL
							, sK
							, sM
							, sTM
							, sTTM
							, sNG	
							--,sXauNoiMa
							, iID_MaPhongBanDich	
							, rTuChi = SUM(rTuChi/@dvt)
							, rChiTapTrung = SUM(rTuChi/@dvt)-SUM(CASE WHEN iID_MaPhongBan IN ('07','10') AND sLNS IN (1020100,1020000,1050000) THEN (rTuChi-rChiTapTrung)/@dvt ELSE 0 END)
							, rHienVat = SUM(rHienVat/@dvt)
				 FROM		DT_ChungTuChiTiet
				 WHERE		iTrangThai=1   
							AND sLNS in (1020100,1020000,1050000)  
							--AND (sLNS='1020100' OR sLNS='1020000')   
							AND iNamLamViec=@nam 
							--and (@id_phongban is null or iID_MaPhongBanDich in (select * from f_split(@Id_PhongBan)))
							--@@DKPB @@DKDV 
				GROUP BY	sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaPhongBanDich
				HAVING		SUM(rTuChi/@dvt)<>0 
							OR SUM(rHienVat/@dvt)<>0 
							OR SUM(rChiTapTrung/@dvt)<>0

				UNION ALL

				SELECT		
							--sLNS
							sLNS = case when sLNS='1020000' then '1020100' else sLNS end
							, sL
							, sK
							, sM
							, sTM
							, sTTM
							, sNG	
							, iID_MaPhongBanDich	
							, rTuChi = SUM(rTuChi/@dvt)
							, rChiTapTrung = SUM((rTuChi - CASE WHEN iID_MaPhongBanDich IN ('07','10') AND sLNS IN (1020100,1020000,1050000) THEN rTuChi-rChiTapTrung ELSE 0 END)/@dvt)
							, rHienVat = SUM(rHienVat/@dvt)
				 FROM		DT_ChungTuChiTiet_PhanCap
				 WHERE		iTrangThai=1  
							AND sLNS in (1020100,1020000,1050000)  
							AND iNamLamViec=@nam 
							--and (@id_phongban is null or iID_MaPhongBanDich in (select * from f_split(@Id_PhongBan)))
							--@@DKPB @@DKDV 
				GROUP BY	sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaPhongBanDich
				HAVING		SUM(rTuChi/@dvt)<>0 
							OR SUM(rHienVat/@dvt)<>0 
							OR SUM(rChiTapTrung/@dvt)<>0 
						
				) as result,
				(select sNG as iID_MaNganh, sMoTa as sTenNganh from NS_MucLucNganSach where iTrangThai = 1 and iNamLamViec=@nam and sLNS = '') as b
	WHERE		result.sNG = b.iID_MaNganh 
	GROUP BY	iID_MaNganh,sTenNganh,sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaPhongBanDich
) as a


 --lay mota lns
left join 
(select sXauNoiMa as mlns_id, sMoTa from dbo.f_mlns_mota2(@nam)) as mlns
on mlns.mlns_id=a.sXauNoiMa


--where rTuChi like '138%'
ORDER BY	iID_MaNganh
			, sM
			, sTM
			, sTTM
			, sNG
			, sTenPB

--;with mlns as
--(select sXauNoiMa as mlns_id, sMoTa
--		,ROW_NUMBER() over (partition by sXauNoiMa ORDER BY sMoTa) as row_id
--from	NS_MucLucNganSach 
--where	iTrangThai=1 and iNamLamViec=@nam and sNG<>'' and sLNS<>'')

--select * from mlns
--where row_id=2
