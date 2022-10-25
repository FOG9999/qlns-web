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

SELECT	rTuChi=SUM(rTuChi)
		, rHienVat=SUM(rHienVat)
		, rTuChiTuyVien=SUM(rTuChiTuyVien)
FROM	(
		SELECT	rTuChi=SUM((rTuChi+rHangNhap+rHangMua)/@dvt)-SUM(CASE WHEN sLNS IN (1020200) THEN rHangNhap/@dvt ELSE 0 END)
				, rTuChiTuyVien=SUM(CASE WHEN sLNS IN (1020200) THEN rHangNhap/@dvt ELSE 0 END)
				, rHienVat=SUM(rHienVat/@dvt)
		FROM	DT_ChungTuChiTiet
		WHERE	iTrangThai <> 0 
				AND (@@DKCauHinh) 
				AND ((LEFT(iID_MaDonVi,2) = @iID_MaDonVi and LEN(@iID_MaDonVi) = 2) or iID_MaDonVi = @iID_MaDonVi)
				AND MaLoai <> '1' 
				AND iNamLamViec=@nam 
				@@DKPB @@DKDV

		UNION ALL

		SELECT	rTuChi=SUM(rTuChi/@dvt)
				, rTuChiTuyVien=0
				, rHienVat=SUM(rHienVat/@dvt)
		FROM	DT_ChungTuChiTiet_PhanCap
		WHERE	iTrangThai <> 0
				AND (sLNS IN (1020000,1020100) ) 
				AND MaLoai <> '1'
				AND ((LEFT(iID_MaDonVi,2) = @iID_MaDonVi and LEN(@iID_MaDonVi) = 2) or iID_MaDonVi = @iID_MaDonVi)
				AND iNamLamViec=@nam 
				@@DKPB @@DKDV) as result