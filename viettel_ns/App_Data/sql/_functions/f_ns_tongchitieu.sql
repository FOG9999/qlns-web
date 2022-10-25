/*
	
	author:	hiep
	date:	09/05/2018
	decs:	Lấy tổng chỉ tiêu ngân sách 
	params:	

*/

ALTER FUNCTION [dbo].[f_ns_tongchitieu]
	(
		
	@iNamLamViec int,
	@iID_MaDonVi nvarchar(500),
	@iID_MaPhongBan nvarchar(10),
	@iID_MaNamNganSach nvarchar(10),
	@iID_MaNguonNganSach nvarchar(10)
	
	)

RETURNS @NS_ChiTieu	TABLE 
	(
	
		iID_MaDonVi nvarchar(10)
		, iID_MaMucLucNganSach uniqueidentifier
		, sXauNoiMa nvarchar(200)
		, sLNS nvarchar(10)
		, sL nvarchar(10)
		, sK nvarchar(10)
		, sM nvarchar(10)
		, sTM nvarchar(10)
		, sTTM nvarchar(10)
		, sNG nvarchar(10)
		, sMoTa nvarchar(MAX)
		, rTuChi decimal(18,0)
		, rHienVat decimal(18,0)
		, rTonKho decimal(18,0)
		, rHangNhap decimal(18,0)
		, rHangMua decimal(18,0)
		, rPhanCap decimal(18,0)
		, rDuPhong decimal(18,0)
	) 
AS
	BEGIN

	DECLARE @iID_MaChungTu nvarchar(2000)
	SET		@iID_MaChungTu = [dbo].f_ns_dtbs_chungtu_gom(@iNamLamViec,@iID_MaPhongBan)

	INSERT INTO @NS_ChiTieu(iID_MaDonVi, iID_MaMucLucNganSach, sXauNoiMa, sLNS, sL, sK, sM, stM, sTTM, sNG, sMoTa, rTuChi, rHienVat, rTonKho, rHangNhap, rHangMua, rPhanCap, rDuPhong)
	SELECT	iID_MaDonVi
			, iID_MaMucLucNganSach
			, sXauNoiMa
			, sLNS 
			, sL
			, sK
			, sM
			, sTM
			, sTTM
			, sNG
			, sMoTa
			, rTuChi
			, rHienVat
			, rTonKho
			, rHangNhap
			, rHangMua
			, rPhanCap
			, rDuPhong
	FROM (	
			-- DU TOAN --
			
			SELECT		iID_MaDonVi
						, iID_MaMucLucNganSach
						, sXauNoiMa
						, sLNS 
						, sL
						, sK
						, sM
						, sTM
						, sTTM
						, sNG
						, sMoTa
						, rTuChi	=SUM(rTuChi)
						, rHienVat	=SUM(rHienVat)
						, rTonKho	=SUM(rTonKho)
						, rHangNhap	=SUM(rHangNhap)
						, rHangMua	=SUM(rHangMua)
						, rPhanCap	=SUM(rPhanCap)
						, rDuPhong	=SUM(rDuPhong)
			FROM		DT_ChungTuChiTiet 
			WHERE		iTrangThai = 1 
						AND iNamLamViec=@iNamLamViec 						
						AND ( @iID_MaNguonNganSach IS NULL OR iID_MaNguonNganSach IN (SELECT * FROM F_Split(@iID_MaNguonNganSach))) 
						AND ( @iID_MaNamNganSach IS NULL OR iID_MaNamNganSach IN (SELECT * FROM F_Split(@iID_MaNamNganSach)))
						AND ( MaLoai='' OR MaLoai='2')  
						AND ( @iID_MaPhongBan IS NULL OR iID_MaPhongBanDich IN (SELECT * FROM F_Split(@iID_MaPhongBan)))
						AND ( @iID_MaDonVi IS NULL OR iID_MaDonVi IN (SELECT * FROM F_Split(@iID_MaDonVi)))
			GROUP BY	iID_MaDonVi
						, sLNS
						, sL
						, sK
						, sM
						, sTM
						, sTTM
						, sNG
						, iID_MaMucLucNganSach
						, sXauNoiMa
						, sMota
			HAVING		SUM(rTuChi) <> 0 
						OR SUM(rHienVat) <> 0 
						OR SUM(rTonKho) <> 0 
						OR SUM(rHangNhap) <> 0 
						OR SUM(rHangMua) <> 0 
						OR SUM(rPhanCap) <> 0 
						OR SUM(rDuPhong) <> 0
                
			-- DU TOAN - PHAN CAP --				

			UNION ALL

			SELECT		iID_MaDonVi
						, iID_MaMucLucNganSach
						, sXauNoiMa
						, sLNS 
						, sL
						, sK
						, sM
						, sTM
						, sTTM
						, sNG
						, sMoTa
						, rTuChi	=SUM(rTuChi)
						, rHienVat	=SUM(rHienVat)
						, rTonKho	=SUM(rTonKho)
						, rHangNhap	=SUM(rHangNhap)
						, rHangMua	=SUM(rHangMua)
						, rPhanCap	=SUM(rPhanCap)
						, rDuPhong	=SUM(rDuPhong)
			FROM		DT_ChungTuChiTiet_PhanCap 
			WHERE		iTrangThai = 1 
						AND iNamLamViec=@iNamLamViec 						
						AND ( @iID_MaNguonNganSach IS NULL OR iID_MaNguonNganSach IN (SELECT * FROM F_Split(@iID_MaNguonNganSach))) 
						AND ( @iID_MaNamNganSach IS NULL OR iID_MaNamNganSach IN (SELECT * FROM F_Split(@iID_MaNamNganSach)))
						AND ( @iID_MaPhongBan IS NULL OR iID_MaPhongBanDich IN (SELECT * FROM F_Split(@iID_MaPhongBan)))
						AND ( @iID_MaDonVi IS NULL OR iID_MaDonVi IN (SELECT * FROM F_Split(@iID_MaDonVi)))
			GROUP BY	iID_MaDonVi
						, sLNS
						, sL
						, sK
						, sM
						, sTM
						, sTTM
						, sNG
						, iID_MaMucLucNganSach
						, sXauNoiMa
						, sMota
			HAVING		SUM(rTuChi) <> 0 
						OR SUM(rHienVat) <> 0 
						OR SUM(rTonKho) <> 0 
						OR SUM(rHangNhap) <> 0 
						OR SUM(rHangMua) <> 0 
						OR SUM(rPhanCap) <> 0 
						OR SUM(rDuPhong) <> 0
                   
			-- DU TOAN BO SUNG --

			UNION ALL

			SELECT		iID_MaDonVi
						, iID_MaMucLucNganSach
						, sXauNoiMa
						, sLNS 
						, sL
						, sK
						, sM
						, sTM
						, sTTM
						, sNG
						, sMoTa
						, rTuChi	=SUM(rTuChi)
						, rHienVat	=SUM(rHienVat)
						, rTonKho	=SUM(rTonKho)
						, rHangNhap	=SUM(rHangNhap)
						, rHangMua	=SUM(rHangMua)
						, rPhanCap	=SUM(rPhanCap)
						, rDuPhong	=SUM(rDuPhong)
			FROM		DTBS_ChungTuChiTiet 
			WHERE		iTrangThai = 1 
						AND iNamLamViec = @iNamLamViec 						
						AND ( @iID_MaNguonNganSach IS NULL OR iID_MaNguonNganSach IN (SELECT * FROM F_Split(@iID_MaNguonNganSach))) 
						AND ( @iID_MaNamNganSach IS NULL OR iID_MaNamNganSach IN (SELECT * FROM F_Split(@iID_MaNamNganSach)))
						AND ( MaLoai='' OR MaLoai='2') 
						AND ( @iID_MaPhongBan IS NULL OR iID_MaPhongBanDich IN (SELECT * FROM F_Split(@iID_MaPhongBan)))
						AND ( @iID_MaDonVi IS NULL OR iID_MaDonVi IN (SELECT * FROM F_Split(@iID_MaDonVi)))
						AND iID_MaChungTu IN (SELECT * FROM f_split(@iID_MaChungTu))
			GROUP BY	iID_MaDonVi
						, sLNS
						, sL
						, sK
						, sM
						, sTM
						, sTTM
						, sNG
						, iID_MaMucLucNganSach
						, sXauNoiMa
						, sMota
			HAVING		SUM(rTuChi) <> 0 
						OR SUM(rHienVat) <> 0 
						OR SUM(rTonKho) <> 0 
						OR SUM(rHangNhap) <> 0 
						OR SUM(rHangMua) <> 0 
						OR SUM(rPhanCap) <> 0 
						OR SUM(rDuPhong) <> 0

			-- DU TOAN BO SUNG - PHAN CAP --

			UNION ALL

			SELECT		iID_MaDonVi
						, iID_MaMucLucNganSach
						, sXauNoiMa
						, sLNS 
						, sL
						, sK
						, sM
						, sTM
						, sTTM
						, sNG
						, sMoTa
						, rTuChi	=SUM(rTuChi)
						, rHienVat	=SUM(rHienVat)
						, rTonKho	=SUM(rTonKho)
						, rHangNhap	=SUM(rHangNhap)
						, rHangMua	=SUM(rHangMua)
						, rPhanCap	=SUM(rPhanCap)
						, rDuPhong	=SUM(rDuPhong)
			FROM		f_ns_dtbs_phancap(@iNamLamViec, @iID_MaPhongBan, @iID_MaDonVi, @iID_MaChungTu) 
			WHERE		( @iID_MaNguonNganSach IS NULL OR iID_MaNguonNganSach IN (SELECT * FROM F_Split(@iID_MaNguonNganSach))) 
						AND ( @iID_MaNamNganSach IS NULL OR iID_MaNamNganSach IN (SELECT * FROM F_Split(@iID_MaNamNganSach)))
			GROUP BY	iID_MaDonVi
						, sLNS
						, sL
						, sK
						, sM
						, sTM
						, sTTM
						, sNG
						, iID_MaMucLucNganSach
						, sXauNoiMa
						, sMota
			HAVING		SUM(rTuChi) <> 0 
						OR SUM(rHienVat) <> 0 
						OR SUM(rTonKho) <> 0 
						OR SUM(rHangNhap) <> 0 
						OR SUM(rHangMua) <> 0 
						OR SUM(rPhanCap) <> 0 
						OR SUM(rDuPhong) <> 0
) as CT
RETURN
END
