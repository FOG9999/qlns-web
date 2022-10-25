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

SELECT		rTuChi = ISNULL(SUM(rTuChi),0)
			, rHangNhap = ISNULL(SUM(rHangNhap),0)
			, rPhanCap = ISNULL(SUM(rPhanCap),0)
			, rDuPhong = ISNULL(SUM(rDuPhong),0)
FROM
-- Phần bìa bảo đảm--
			(SELECT		rTuChi = ISNULL(SUM((rTuChi+rHangMua)/@dvt),0)
						, rPhanCap = ISNULL(SUM(rPhanCap/@dvt),0)
						, rDuPhong= ISNULL(SUM(rDuPhong/@dvt),0)
						, rHangNhap=ISNULL(SUM(rHangNhap/@dvt),0)
			 FROM		DT_ChungTuChiTiet
			 WHERE      iTrangThai <> 0
						AND(sLNS LIKE '104%') 
						AND sNG <> '00' 
						AND (@ng is null or sNG IN (select * from f_split(@ng)))
						AND MaLoai<>1   
						AND iNamLamViec = @nam  
						AND (@id_donvi is null or iID_MaDonVi in (select * from f_split(@id_donvi)))

			UNION ALL

			SELECT		rTuChi = 0
						, rPhanCap = ISNULL(SUM(rTuChi/@dvt),0)
						, rDuPhong = 0
						, rHangNhap =0
			 FROM		DT_ChungTuChiTiet
			 WHERE      iTrangThai <> 0
						AND sNG <> '00' 
						AND sLNS='1020100'
						AND iID_MaChungTu not in ('9837dd82-8c15-48bb-b272-2c543d407882')
						AND iNamLamViec = @nam  
						AND (@id_donvi is null or iID_MaDonVi in (select * from f_split(@id_donvi)))
						AND (@ng is null or sNG IN (select * from f_split(@ng)))
			HAVING		SUM(rTuChi)<>0 
						
			) as result