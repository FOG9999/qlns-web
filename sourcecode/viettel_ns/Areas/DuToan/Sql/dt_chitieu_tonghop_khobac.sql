declare @dvt int									set @dvt = 1
declare @nam int									set @nam = 2021
declare @xLNS nvarchar(MAX)							set @xLNS='1030100,1030900'


--###--

SELECT		iID_MaDonVi
			, sTenDonVi
			, rB5 = sum(rB5)
			, rB7 = sum(rB7)
			, rB8 = sum(rB8)
			, rB10 = sum(rB10)	
FROM		(
			SELECT		iID_MaDonVi
						, sTenDonVi
						, rB5 = case when iID_MaPhongBanDich = '05' then SUM(rTuChi - rChiTapTrung) else 0 end
						, rB7 = case when iID_MaPhongBanDich = '07' then SUM(rTuChi - rChiTapTrung) else 0 end
						, rB8 = case when iID_MaPhongBanDich = '08' then SUM(rTuChi - rChiTapTrung) else 0 end
						, rB10 = case when iID_MaPhongBanDich = '10' then SUM(rTuChi - rChiTapTrung) else 0 end			
			FROM		(
						---- Dữ liệu ngoài Ngoại tệ ----
						SELECT		iID_MaDonVi = LEFT(iID_MaDonVi,2)
									, iID_MaPhongBanDich
									, rTuChi = (SUM(rTuChi) + SUM(rHangMua))/@dvt
									, rChiTapTrung = SUM(CASE WHEN (sLNS IN (@@xsLNS) OR iID_MaPhongBanDich = '06' OR iID_MaDonVi IN (@@xDonVi) OR (iID_MaDonVi in ('34','50') AND sLNS <> '1030100')) THEN (rTuChi + rHangMua)/@dvt 
															  ELSE rChiTapTrung/@dvt END)
									, loai = 0
						 FROM		DT_ChungTuChiTiet
						 WHERE		iTrangThai <> 0
									AND sLNS LIKE '1%'  
									AND iNamLamViec=@nam  
									AND MaLoai <> 1 
						GROUP BY	iID_MaDonVi, iID_MaPhongBanDich
						HAVING		SUM(rTuChi/@dvt) + SUM(rHangMua/@dvt) <> 0 
									OR SUM(rChiTapTrung/@dvt) <> 0

						UNION ALL

						SELECT		iID_MaDonVi = LEFT(iID_MaDonVi,2)
									, iID_MaPhongBanDich						
									, rTuChi = (SUM(rTuChi))/@dvt
									, rChiTapTrung = SUM(CASE WHEN (sLNS IN (@@xsLNS) OR iID_MaPhongBanDich = '06' OR iID_MaDonVi IN (@@xDonVi)) THEN (rTuChi)/@dvt 
															  ELSE rChiTapTrung/@dvt END)
									, loai = 0
						FROM		DT_ChungTuChiTiet_PhanCap
						WHERE		iTrangThai <> 0
									AND sLNS LIKE '1%'  
									AND iNamLamViec=@nam 
									AND MaLoai <> 1
						GROUP BY	iID_MaDonVi, iID_MaPhongBanDich
						HAVING		SUM(rTuChi/@dvt)<> 0 
									OR SUM(rChiTapTrung/@dvt) <> 0
			
						UNION ALL

						---- Dữ liệu Ngoại tệ ----
						SELECT		iID_MaDonVi = LEFT(iID_MaDonVi,2)
									, iID_MaPhongBanDich						
									, rTuChi = (SUM(rHangNhap))/@dvt
									, rChiTapTrung = (SUM(rHangNhap))/@dvt
									, loai = 1
						 FROM		DT_ChungTuChiTiet
						 WHERE		iTrangThai <> 0
									AND sLNS LIKE '1%'  
									AND iNamLamViec=@nam  
									AND MaLoai <> 1 
						GROUP BY	iID_MaDonVi, iID_MaPhongBanDich
						HAVING		SUM(rHangNhap/@dvt) <> 0 
									OR SUM(rChiTapTrung/@dvt) <> 0

						UNION ALL

						SELECT		iID_MaDonVi = LEFT(iID_MaDonVi,2)
									, iID_MaPhongBanDich						
									, rTuChi = (SUM(rHangNhap))/@dvt
									, rChiTapTrung = (SUM(rHangNhap))/@dvt
									, loai = 1
						FROM		DT_ChungTuChiTiet_PhanCap
						WHERE		iTrangThai <> 0
									AND sLNS LIKE '1%'  
									AND iNamLamViec=@nam 
									AND MaLoai <> 1
						GROUP BY	iID_MaDonVi, iID_MaPhongBanDich
						HAVING		SUM(rHangNhap/@dvt) <> 0 
									OR SUM(rChiTapTrung/@dvt) <> 0

						UNION ALL

						SELECT		iID_MaDonVi = case when iID_MaDonVi in ('C1','C2','C3','C4','L1','B6') then '00' else iID_MaDonVi end
									, iID_MaPhongBanDich						
									, rTuChi = (SUM(rDuPhong))/@dvt
									, rChiTapTrung = (SUM(rDuPhong))/@dvt
									, loai = 1
						FROM		DT_ChungTuChiTiet
						WHERE		iTrangThai <> 0
									AND sLNS LIKE '1%'  
									AND iNamLamViec=@nam 
									AND MaLoai <> 1
						GROUP BY	iID_MaDonVi, iID_MaPhongBanDich
						HAVING		SUM(rDuPhong/@dvt) <> 0 
						) as a

						inner join 

						(select iID_MaDonVi as id, sTen as sTenDonVi from NS_DonVi where iNamLamViec_DonVi = @nam and iTrangThai = 1) as dv

						on a.iID_MaDonVi = dv.id
			WHERE		(rTuChi - rChiTapTrung) > 0
			GROUP BY	iID_MaDonVi,sTenDonVi, iID_MaPhongBanDich) as re
--where		iID_MaDonVi in (SELECT DISTINCT iID_MaDonVi
--							FROM (SELECT * FROM NS_PhongBan_DonVi WHERE iTrangThai = 1 AND iNamLamViec = @nam) as a 
--							INNER JOIN (SELECT * FROM NS_PhongBan WHERE iTrangThai = 1) as b ON a.iID_MaPhongBan = b.iID_MaPhongBan
--							WHERE b.sKyHieu = '07')
GROUP BY	iID_MaDonVi,sTenDonVi
ORDER BY	iID_MaDonVi