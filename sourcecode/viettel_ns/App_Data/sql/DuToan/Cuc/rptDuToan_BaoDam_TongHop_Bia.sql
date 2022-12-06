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
			, sMoTa = dbo.F_MLNS_MoTa_LNS(@nam,'1020100',sL,sK,sM,sTM,sTTM,sNG)	
			, iID_MaPhongBanDich
			, sTenPB=CASE iID_MaPhongBanDich WHEN '10' THEN N'@@sTenB10' 
											 WHEN '07' THEN N'@@sTenB6'
											 ELSE  N'@@sTenB' END
			, rTuChi = SUM(rTuChi)
			, rTonKho = SUM(rTonKho)
			, rHangNhap = SUM(rHangNhap)
			, rHangMua = SUM(rHangMua)
			, rPhanCap = SUM(rPhanCap)
			, rDuPhong = SUM(rDuPhong)
FROM
-- Phần bìa bảo đảm--
			(
			SELECT		sLNS
						, sL
						, sK
						, sM
						, sTM
						, sTTM
						, sNG	
						, iID_MaPhongBanDich	
						, rTuChi=SUM(rTuChi/@dvt)
						, rTonKho=SUM(rTonKho/@dvt)
						, rHangNhap=SUM(rHangNhap/@dvt)
						, rHangMua=SUM(rHangMua/@dvt)
						, rPhanCap=SUM(CASE WHEN (iID_MaChungTuChiTiet IN ('EF4390F7-336F-4C3A-B893-F42A48B7A821')) THEN 0 ELSE rPhanCap/@dvt END)
						, rDuPhong=SUM(rDuPhong/@dvt)
			 FROM		DT_ChungTuChiTiet
			 WHERE		iTrangThai <> 0
						AND sNG <> '00' 
						AND sLNS like '1040100%'
						AND MaLoai in ('','2')						 
						AND iNamLamViec = @nam  
						@@DKPB @@DKDV
			GROUP BY	sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaPhongBanDich
			HAVING		SUM(rTuChi)<>0 
						OR SUM(rHangNhap)<>0 
						OR SUM(rTonKho)<>0  
						OR SUM(rHangMua)<>0  
						OR SUM(rPhanCap)<>0 
						OR SUM(rDuPhong)<>0

			UNION ALL

			SELECT		sLNS = '1040100'
						, sL
						, sK
						, sM
						, sTM
						, sTTM
						, sNG	
						, iID_MaPhongBanDich	
						, rTuChi = 0
						, rTonKho = 0
						, rHangNhap =0
						, rHangMua = 0
						, rPhanCap = SUM(rTuChi/@dvt)
						, rDuPhong = 0
			 FROM		DT_ChungTuChiTiet
			 WHERE		iTrangThai <> 0
						AND sNG <> '00' 
						AND sLNS='1020100'
						--and iID_MaChungTuChiTiet not in ('9FD40249-7622-4CA6-8216-B480BD55B342','5EDE13A9-DE05-4A07-8071-015040180742','FAE4A846-65F8-4414-9AFE-A7BADA30CF54','1407E5DB-9C2B-4161-8862-C6F3B93D9A9A','608E8681-1BB4-47E9-A88E-44910EA0B70E','A6312D91-DC7F-4FE4-A195-3648DB201CEF','2AB8FB77-ED52-4FF3-812D-A8F71DB85BE2','B3F44417-112D-4A95-A410-A5B0E5566F1B','C08D38E4-0C09-4AE6-92A6-A146921316D3','E56CCBD1-7F61-4F3D-A152-BD8B7509B9ED')
						AND iNamLamViec = @nam 
						@@DKPB @@DKDV 
			GROUP BY	sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaPhongBanDich
			HAVING		SUM(rTuChi) <>0 

			UNION ALL

			SELECT		sLNS = '1040100'
						, sL
						, sK
						, sM
						, sTM
						, sTTM
						, sNG	
						, iID_MaPhongBanDich	
						, rTuChi = 0
						, rTonKho = 0
						, rHangNhap =0
						, rHangMua = 0
						, rPhanCap = SUM(rTuChi/@dvt)
						, rDuPhong = 0
			 FROM		DT_ChungTuChiTiet_PhanCap
			 WHERE		iTrangThai <> 0
						AND sNG <> '00' 
						AND sLNS='1020100'
						AND iNamLamViec = @nam 
						and MaLoai = '2'
						--and (sNG <> '23')
						@@DKPB @@DKDV 
			GROUP BY	sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaPhongBanDich
			HAVING		SUM(rTuChi) <>0 
						
			) as result,
			(select sNG as iID_MaNganh, sMoTa as sTenNganh from NS_MucLucNganSach where iTrangThai = 1 and iNamLamViec=@nam and sLNS = '') as b
WHERE		result.sNG = b.iID_MaNganh
GROUP BY	iID_MaNganh,sTenNganh,sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaPhongBanDich
ORDER BY	iID_MaNganh
			,sM
			,sTM
			,sTTM
			,sNG