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

SELECT	rTuChiDT=SUM(rTuChiDT)
		, rTuChiDN=SUM(rTuChiDN)
		, rTuChiBV=SUM(rTuChiBV)
		, rHienVat=SUM(rHienVat)
		, rTuChiTuyVien=SUM(rTuChiTuyVien)
FROM	(
		SELECT	rTuChiDT= case when (iID_MaPhongBanDich <> '06' and iID_MaDonVi not in ('30','34','35','50','69','70','71','72','73','76','77','78','79','80','88','89','90','91','92','93','94') and LEN(iID_MaDonVi) = 2) then SUM((rTuChi+rHangNhap)/@dvt)-SUM(CASE WHEN sLNS IN (1020200) THEN rHangNhap/@dvt ELSE 0 END) else 0 end
				, rTuChiDN= case when (iID_MaPhongBanDich = '06' and LEN(iID_MaDonVi) = 2) or iID_MaDonVi in ('30','34','35','50','69','70','71','72','73','78','80','88','89','90','91','92','93','94') then SUM(rTuChi)/@dvt else 0 end
				, rTuChiBV= case when (iID_MaPhongBanDich <> '06' and iID_MaDonVi in ('76','77','79')) or LEN(iID_MaDonVi) > 2 then SUM(rTuChi)/@dvt else 0 end
				, rHienVat=SUM(rHienVat/@dvt)
				, rTuChiTuyVien=SUM(CASE WHEN sLNS IN (1020200) THEN rhangNhap/@dvt ELSE 0 END)
		FROM	DT_ChungTuChiTiet
		WHERE	iTrangThai <> 0
				AND (@@DKLNSNV) 
				AND iNamLamViec=@nam 
				@@DKPB @@DKDV 
		group by iID_MaPhongBanDich, iID_MaDonVi

		UNION ALL

		SELECT	rTuChiDT= case when (iID_MaPhongBanDich <> '06' and iID_MaDonVi not in ('30','34','35','50','69','70','71','72','73','76','77','78','79','80','88','89','90','91','92','93','94') and LEN(iID_MaDonVi) = 2) then SUM(rTuChi/@dvt) else 0 end
				, rTuChiDN= case when (iID_MaPhongBanDich = '06' and LEN(iID_MaDonVi) = 2) or iID_MaDonVi in ('30','34','35','50','69','70','71','72','73','78','80','88','89','90','91','92','93','94') then SUM(rTuChi/@dvt) else 0 end
				, rTuChiBV= case when (iID_MaPhongBanDich <> '06' and iID_MaDonVi in ('76','77','79')) or LEN(iID_MaDonVi) > 2 then SUM(rTuChi/@dvt) else 0 end	
				, rHienVat=SUM(rHienVat/@dvt)
				, rTuChiTuyVien=0
		FROM	DT_ChungTuChiTiet_PhanCap
		WHERE	iTrangThai <> 0
				AND (sLNS IN (1020000,1020100)) 
				AND iNamLamViec=@nam 
				AND MaLoai <> '1'
				@@DKPB @@DKDV
		group by iID_MaPhongBanDich, iID_MaDonVi) as result