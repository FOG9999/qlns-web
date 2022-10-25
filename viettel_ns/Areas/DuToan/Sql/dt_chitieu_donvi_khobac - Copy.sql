declare @dvt int									set @dvt = 1000
declare @nam int									set @nam = 2020
declare @xLNS nvarchar(MAX)							set @xLNS='1020200'
declare @@xDonVi nvarchar(MAX)						set @@xDonVi='30,34,35,50,69,70,71,72,73,78,80,88,89,90,91,92,93,94'


--###--

SELECT		sLNS1 = LEFT(sLNS,1), sLNS3 = LEFT(sLNS,3), sLNS5 = LEFT(sLNS,5), sLNS, sL, sK, sM, sTM
			, sMoTa = case when sLNS like '101%' then N'Lương, phụ cấp, tiền ăn'
						   when sLNS like '102%' then N'Nghiệp vụ HC'
						   when sLNS = '1040200' then N'Ngân sách bảo đảm - ngoại tệ'
						   when sLNS = '1040300' then N'Ngân sách bảo đảm - Hàng mua' 
						   when sLNS like '103%' then N'Ngân sách XDCB'
						   when sLNS like '105%' then N'Kinh phí hỗ trợ doanh nghiệp'
						   else N'NSQP khác' end
			, rKhoBac = SUM(rTuChi-rChiTapTrung) 
FROM		(
			---- Dữ liệu ngoài Ngoại tệ ----
			SELECT		sLNS, sL, sK, sM, sTM
						, rTuChi = SUM(rTuChi)/@dvt
						, rChiTapTrung = SUM(CASE WHEN (sLNS IN (@@xsLNS) OR iID_MaPhongBanDich = '06' OR iID_MaDonVi IN (@@xDonVi) OR (iID_MaDonVi in ('34','50') AND sLNS <> '1030100')) THEN (rTuChi)/@dvt 
												  ELSE rChiTapTrung/@dvt END)
						, loai = 0
			 FROM		DT_ChungTuChiTiet
			 WHERE		iTrangThai <> 0
						AND sLNS LIKE '1%'
						AND iNamLamViec=@nam  
						AND MaLoai <> 1 
						AND iID_MaDonVi = @iID_MaDonVi
			GROUP BY	sLNS, sL, sK, sM, sTM
			HAVING		SUM(rTuChi/@dvt) <> 0 
						OR SUM(rChiTapTrung/@dvt) <> 0

			UNION ALL

			SELECT		sLNS = '1040300' , sL, sK, sM, sTM
						, rTuChi = SUM(rHangMua)/@dvt
						, rChiTapTrung = SUM(CASE WHEN (sLNS IN (@@xsLNS) OR iID_MaPhongBanDich = '06' OR iID_MaDonVi IN (@@xDonVi) OR (iID_MaDonVi in ('34','50') AND sLNS <> '1030100')) THEN (rHangMua)/@dvt 
												  ELSE rChiTapTrung/@dvt END)
						, loai = 0
			 FROM		DT_ChungTuChiTiet
			 WHERE		iTrangThai <> 0
						AND sLNS LIKE '104%'  
						AND iNamLamViec=@nam  
						AND MaLoai <> 1 
						AND iID_MaDonVi = @iID_MaDonVi
			GROUP BY	sLNS, sL, sK, sM, sTM
			HAVING		SUM(rHangMua/@dvt) <> 0 
						OR SUM(rChiTapTrung/@dvt) <> 0

			UNION ALL

			SELECT		sLNS, sL, sK, sM, sTM
						, rTuChi = SUM(rTuChi)/@dvt
						, rChiTapTrung = SUM(CASE WHEN (sLNS IN (@@xsLNS) OR iID_MaPhongBanDich = '06' OR iID_MaDonVi IN (@@xDonVi)) THEN rTuChi/@dvt 
												  ELSE rChiTapTrung/@dvt END)
						, loai = 0
			FROM		DT_ChungTuChiTiet_PhanCap
			WHERE		iTrangThai <> 0
						AND sLNS LIKE '1%'  
						AND iNamLamViec=@nam 
						AND MaLoai <> 1
						AND iID_MaDonVi = @iID_MaDonVi
			GROUP BY	sLNS, sL, sK, sM, sTM
			HAVING		SUM(rTuChi/@dvt) <> 0 
						OR SUM(rChiTapTrung/@dvt) <> 0
			
			UNION ALL

			---- Dữ liệu Ngoại tệ ----
			SELECT		sLNS = case when sLNS = '1040100' then '1040200' else sLNS end, sL, sK, sM, sTM
						, rTuChi = (SUM(rHangNhap))/@dvt
						, rChiTapTrung = (SUM(rHangNhap))/@dvt
						, loai = 1
			 FROM		DT_ChungTuChiTiet
			 WHERE		iTrangThai <> 0 
						AND sLNS LIKE '1%'  
						AND iNamLamViec=@nam  
						AND MaLoai <> 1 
						AND iID_MaDonVi = @iID_MaDonVi
			GROUP BY	sLNS, sL, sK, sM, sTM
			HAVING		SUM(rHangNhap) <> 0

			UNION ALL

			SELECT		sLNS = case when sLNS = '1040100' then '1040200' else sLNS end, sL, sK, sM, sTM
						, rTuChi = (SUM(rHangNhap))/@dvt
						, rChiTapTrung = (SUM(rHangNhap))/@dvt
						, loai = 1
			FROM		DT_ChungTuChiTiet_PhanCap
			WHERE		iTrangThai <> 0
						AND sLNS LIKE '1%'  
						AND iNamLamViec=@nam 
						AND MaLoai <> 1
						AND iID_MaDonVi = @iID_MaDonVi
			GROUP BY	sLNS, sL, sK, sM, sTM
			HAVING		SUM(rHangNhap) <> 0
			) as a

GROUP BY	sLNS, sL, sK, sM, sTM
HAVING		sum(rTuChi - rChiTapTrung) <> 0
ORDER BY	sLNS, sL, sK, sM, sTM