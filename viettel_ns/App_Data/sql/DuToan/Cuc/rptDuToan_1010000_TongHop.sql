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

SELECT		sM
			, sTM
			, sTTM
			, sNG
			, sMoTa
			, KhoiDoanhNghiep=SUM(CASE WHEN iID_MaPhongBanDich='07' and LEN(iID_MaDonVi) = 2 THEN (rTuChi)/@dvt ELSE  0 END)
			, KhoiBTTM=SUM(CASE WHEN iID_MaPhongBanDich='10' AND iID_MaDonVi not in ('76','77','79') AND LEN(iID_MaDonVi) = 2 THEN (rTuChi)/@dvt ELSE  0 END)
			, KhoiQKQD=SUM(CASE WHEN iID_MaPhongBanDich<>'07' AND iID_MaPhongBanDich<>'10' AND LEN(iID_MaDonVi) = 2 THEN (rTuChi)/@dvt ELSE  0 END)
			, KhoiBV=SUM(CASE WHEN iID_MaDonVi in ('76','77','79') OR LEN(iID_MaDonVi) > 2 THEN (rTuChi)/@dvt ELSE  0 END)
FROM		DT_ChungTuChiTiet
WHERE		iTrangThai in (1,2) AND rTuChi <> 0 AND sLNS = '1010000' AND iNamLamViec=@nam @@DKPB @@DKDV
GROUP BY	sM,sTM,sTTM,sNG,sMoTa