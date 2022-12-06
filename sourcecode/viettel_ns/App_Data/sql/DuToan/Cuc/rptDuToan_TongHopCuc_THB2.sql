declare @dvt int									set @dvt = 1
declare @nam int									set @nam = 2019
declare @xLNS nvarchar(MAX)							set @xLNS='1030100,1030900'


--###--

SELECT		a.iID_MaDonVi,sTen
			, SUM(BD) AS BD
			, SUM(TX) AS TX
			, SUM(NV) AS NV
			, SUM(XDCB) AS XDCB
			, SUM(DN) AS DN
			, SUM(QPKhac) AS QPKhac
			, SUM(NN) AS NN
			, SUM(NSKhac) AS NSKhac
			, SUM(TonKho) AS TonKho
			, SUM(HienVat) AS HienVat
			, a.iLoai
			, sLoai = case iLoai when 'b' then N'Khối doanh nghiệp'
								 when 'a' then N'Khối dự toán'
								 else N'Khối bệnh viện' end
FROM
			(
			SELECT 
						iID_MaDonVi
						, BD=SUM(CASE WHEN (@@sLNSBD) AND MaLoai in ('',2) THEN (rTuChi+rHangMua+rHangNhap)/@dvt ELSE 0 END)
						, TX=SUM(CASE WHEN (@@sLNSTX) THEN rTuChi /@dvt ELSE 0 END)
						, NV=SUM(CASE WHEN (@@sLNSNV) THEN (rTuChi+rHangNhap)/@dvt  ELSE 0 END)
						, XDCB=SUM(CASE WHEN (@@sLNSXDCB)  THEN rTuChi/@dvt ELSE 0 END)
						, DN=SUM(CASE WHEN (@@sLNSDN) THEN rTuChi/@dvt ELSE 0 END)
						, QPKhac=SUM(CASE WHEN (@@sLNSQPK) THEN rTuChi/@dvt ELSE 0 END)
						, NN=SUM(CASE WHEN (@@sLNSNN) THEN rTuChi/@dvt ELSE 0 END)
						, NSKhac=SUM(CASE WHEN (@@sLNSKHAC AND NOT @@sLNSDN) THEN rTuChi/@dvt ELSE 0 END)
						, TonKho=SUM(CASE  WHEN itrangthai=1 THEN rTonKho/@dvt ELSE 0 END)
						, HienVat=SUM(CASE WHEN (@@sLNSNV) AND itrangthai=1 THEN rHienVat/@dvt ELSE 0 END)
						, iLoai = case when (iID_MaPhongBanDich = '06' and LEN(iID_MaDonVi) = 2) or iID_MaDonVi in ('30','34','35','50','69','70','71','72','73','78','80','88','89','90','91','92','93','94') then 'b' 
									   when iID_MaPhongBanDich <> '06' and iID_MaDonVi not in ('30','34','35','50','69','70','71','72','73','76','77','78','79','80','88','89','90','91','92','93','94') and LEN(iID_MaDonVi) = 2 then 'a'
									   else 'c' end
			FROM		DT_ChungTuChiTiet
			WHERE		iTrangThai <> 0 AND iNamLamViec=@nam @@DKPB 
						AND MaLoai in ('','2')
			GROUP BY	iID_MaDonVi,iID_MaPhongBanDich

			UNION ALL

			SELECT 
						iID_MaDonVi
						, BD=SUM(CASE WHEN (@@sLNSBD) AND MaLoai<>'1' THEN (rTuChi+rHangMua+rHangNhap)/@dvt ELSE 0 END)
						, TX=SUM(CASE WHEN (@@sLNSTX) THEN rTuChi /@dvt ELSE 0 END)
						, NV=SUM(CASE WHEN  (@@sLNSNV) THEN rTuChi/@dvt  ELSE 0 END)
						, XDCB=SUM(CASE WHEN  (@@sLNSXDCB)  THEN rTuChi/@dvt ELSE 0 END)
						, DN=SUM(CASE WHEN (@@sLNSDN) THEN rTuChi/@dvt ELSE 0 END)
						, QPKhac=SUM(CASE WHEN (@@sLNSQPK) THEN rTuChi/@dvt ELSE 0 END)
						, NN=SUM(CASE WHEN  (@@sLNSNN) THEN rTuChi/@dvt ELSE 0 END)
						, NSKhac=SUM(CASE WHEN  (@@sLNSKHAC AND NOT @@sLNSDN) THEN rTuChi/@dvt ELSE 0 END)
						, TonKho=SUM(CASE  WHEN itrangthai=1 THEN rTonKho/@dvt ELSE 0 END)
						, HienVat=SUM(CASE WHEN sLNS IN (1020000,1020100) AND MaLoai <> '1' AND itrangthai=1 THEN rHienVat/@dvt ELSE 0 END)
						, iLoai = case when (iID_MaPhongBanDich = '06' and LEN(iID_MaDonVi) = 2) or iID_MaDonVi in ('30','34','35','50','69','70','71','72','73','78','80','88','89','90','91','92','93','94') then 'b' 
									   when iID_MaPhongBanDich <> '06' and iID_MaDonVi not in ('30','34','35','50','69','70','71','72','73','76','77','78','79','80','88','89','90','91','92','93','94') and LEN(iID_MaDonVi) = 2 then 'a'
									   else 'c' end
			FROM		DT_ChungTuChiTiet_PhanCap
			WHERE		iTrangThai <> 0 AND iNamLamViec=@nam @@DKPB
						AND (sLNS IN (1020000,1020100))
						AND (MaLoai <> 1)
			GROUP BY	iID_MaDonVi,iID_MaPhongBanDich
			)  as a

			INNER JOIN 
			
			(SELECT iid_madonvi,sTen FROM NS_DonVi WHERE iNamLamViec_DonVi=@nam) as b

			ON a.iID_MaDonVi=b.iID_MaDonVi
GROUP BY	a.iID_MaDonVi,sTen,a.iLoai
HAVING		SUM(BD) <> 0 
			OR SUM(TX) <> 0 
			OR SUM(NV) <> 0 
			OR SUM(NV) <> 0 
			OR SUM(XDCB) <> 0 
			OR SUM(DN) <> 0 
			OR SUM(QPKhac) <> 0  
			OR SUM(NN) <> 0  
			OR SUM(NSKhac) <> 0  
			OR SUM(TonKho) <> 0  
			OR SUM(HienVat) <> 0 
ORDER BY	a.iID_MaDonVi