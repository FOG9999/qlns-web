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

SELECT		rTuChi = SUM(rTuChi)
			, rPhanCap = SUM(rPhanCap)
			, rDuPhong = SUM(rDuPhong)
			, rDuPhongC1 = SUM(rDuPhongC1)
			, rHangNhap = SUM(rHangNhap)
FROM
-- Phần bìa bảo đảm--
			(SELECT		rTuChi = ISNULL(SUM((rHangMua)/@dvt),0)
						, rPhanCap = ISNULL(SUM(CASE WHEN sNG = '00' THEN 0 ELSE rPhanCap/@dvt END),0)
						, rDuPhong = ISNULL(SUM(CASE WHEN iID_MaDonVi <> 'C1' THEN rDuPhong/@dvt ELSE 0 END),0)
						, rDuPhongC1 = ISNULL(SUM(CASE WHEN iID_MaDonVi = 'C1'THEN rDuPhong/@dvt ELSE 0 END),0)
						, rHangNhap = ISNULL(SUM(rHangNhap/@dvt),0)
			 FROM		DT_ChungTuChiTiet
			 WHERE		iTrangThai <> 0 
						AND MaLoai in ('','2')						 
						AND (@@DKLNSBD)  
						AND iNamLamViec = @nam 

			 UNION ALL

			 SELECT		rTuChi = 0
						, rPhanCap = ISNULL(SUM(CASE WHEN sNG = '00' THEN 0 ELSE rTuChi/@dvt end),0)
						, rDuPhong = 0
						, rDuPhongC1 = 0
						, rHangNhap =0
			 FROM		DT_ChungTuChiTiet
			 WHERE		iTrangThai <> 0
						AND sLNS='1020100'
						AND iNamLamViec = @nam
			 HAVING		SUM(rTuChi)<>0 
						
			) as result