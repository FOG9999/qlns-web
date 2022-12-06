USE [VIETTEL_NS1]
GO
/****** Object:  UserDefinedFunction [dbo].[F_NS_ChiTieu_ChiTiet_TheoDonVi_LNS]    Script Date: 11/10/2018 9:37:30 PM ******/

/*
	
	author:	hieppm	
	date:	10/11/2018
	decs:	Lấy chỉ tiêu chi tiết theo đơn vị, loại ngân sách

*/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER FUNCTION [dbo].[F_NS_ChiTieu_ChiTiet_TheoDonVi_LNS]
	(
		
	@iNamLamViec int,
	@iID_MaPhongBan nvarchar(10),
	@iID_MaNamNganSach nvarchar(10),
	@iID_MaNguonNganSach nvarchar(10),	
	@sMaNguoiDung nvarchar(50)	
	
	)

RETURNS TABLE 
AS
	RETURN
		

-- Lay Chi Tieu chi tiet theo Don vi, LNS		
SELECT	iID_MaDonVi
		, sLNS =CASE WHEN sLNS=1020000 then 1020100 else sLNS END
		, sL
		, sK
		, sM
		, sTM
		, sTTM
		, sNG
		, sMoTa
		, SUM(rChiTaiKhoBac) AS rChiTaiKhoBac
		, SUM(rTonKho) AS rTonKho
		, SUM(rTuChi) AS rTuChi
		, SUM(rChiTapTrung) AS rChiTapTrung
		, SUM(rHangNhap) AS rHangNhap
		, SUM(rHangMua) AS rHangMua
		, SUM(rHienVat) AS rHienVat
		, SUM(rPhanCap) AS rPhanCap
		, SUM(rDuPhong) AS rDuPhong
		, dDotNgay 
		, iID_MaPhongBan = 'B - ' + iID_MaPhongBan
		, sID_MaNguoiDungTao
FROM 
	(
	-- Chi Tieu Du Toan Chi Tiet Dau Nam

	SELECT	iID_MaDonVi
			,sLNS =CASE WHEN sLNS=1020000 then 1020100 else sLNS END
			, sL
			, sK
			, sM
			, sTM
			, sTTM
			, sNG
			, sMoTa
			, SUM(rChiTaiKhoBac) AS rChiTaiKhoBac
			, SUM(rTonKho) AS rTonKho
			, SUM(rTuChi) AS rTuChi
			, SUM(rChiTapTrung) AS rChiTapTrung
			, SUM(rHangNhap) AS rHangNhap
			, SUM(rHangMua) AS rHangMua
			, SUM(rHienVat) AS rHienVat
			, SUM(rPhanCap) AS rPhanCap
			, SUM(rDuPhong) AS rDuPhong
			, dDotNgay = '01/01/' + cast(@iNamLamViec as varchar(4)) 
			, iID_MaPhongBan
			, sID_MaNguoiDungTao
	FROM	DT_ChungTuChiTiet 
	WHERE	(iTrangThai = 1 or iTrangThai = 2)
			AND (MaLoai='' OR MaLoai='2') 
			AND iNamLamViec=@iNamLamViec 
			AND iID_MaNamNganSach = 2						
			AND iID_MaDonVi IN (SELECT iID_MaDonVi FROM F_NS_DSDvi_TheoND(@iNamLamViec,@sMaNguoiDung))		  
			AND (@iID_MaPhongBan IS NULL OR iID_MaPhongBanDich IN (SELECT * FROM F_SplitString2(@iID_MaPhongBan))) 		 	  
	GROUP BY iID_MaDonVi
			, sLNS
			, sL
			, sK
			, sM
			, sTM
			, sTTM
			, sNG
			, sMoTa 
			, iID_MaPhongBan
			, sID_MaNguoiDungTao
	HAVING	SUM(rChiTaiKhoBac) <> 0 
			OR SUM(rTonKho) <> 0 
			OR SUM(rTuChi) <> 0 
			OR SUM(rChiTapTrung) <> 0 
			OR SUM(rHangNhap) <> 0 
			OR SUM(rHangMua) <> 0 
			OR SUM(rHienVat) <> 0 
			OR SUM(rPhanCap) <> 0 
			OR SUM(rDuPhong) <> 0
					
	UNION ALL

	-- Chi Tieu Du Toan - Phan Cap Dau Nam
	SELECT	iID_MaDonVi
			, sLNS =CASE WHEN sLNS=1020000 then 1020100 else sLNS END
			, sL
			, sK
			, sM
			, sTM
			, sTTM
			, sNG
			, sMoTa
			, 0 AS rChiTaiKhoBac 
			, 0 AS rTonKho
			, SUM(rTuChi) AS rTuChi
			, 0 AS rChiTapTrung
			, 0 AS rHangNhap
			, 0 AS rHangMua
			, SUM(rHienVat) AS rHienVat
			, 0 AS rPhanCap
			, 0 AS rDuPhong
			, dDotNgay = '01/01/' + cast(@iNamLamViec as varchar(4))
			, iID_MaPhongBan
			, sID_MaNguoiDungTao
	FROM	DT_ChungTuChiTiet_PhanCap 
	WHERE	(iTrangThai = 1 or iTrangThai = 2)
			AND iNamLamViec = @iNamLamViec 
			AND iID_MaNamNganSach = 2		  
			AND iID_MaDonVi IN (SELECT iID_MaDonVi FROM F_NS_DSDvi_TheoND(@iNamLamViec,@sMaNguoiDung))		  
			AND (@iID_MaPhongBan IS NULL OR iID_MaPhongBanDich IN (SELECT * FROM F_SplitString2(@iID_MaPhongBan))) 		  
	GROUP BY iID_MaDonVi
			, sLNS
			, sL
			, sK
			, sM
			, sTM
			, sTTM
			, sNG
			, sMoTa 
			, iID_MaPhongBan
			, sID_MaNguoiDungTao
	HAVING	SUM(rChiTaiKhoBac) <> 0 
			OR SUM(rTonKho) <> 0 
			OR SUM(rTuChi) <> 0 
			OR SUM(rChiTapTrung) <> 0 
			OR SUM(rHangNhap) <> 0 
			OR SUM(rHangMua) <> 0 
			OR SUM(rHienVat) <> 0 
			OR SUM(rPhanCap) <> 0 
			OR SUM(rDuPhong) <> 0

	UNION ALL

	-- Chi Tieu Bo Sung Chi Tiet trong nam 
	SELECT	iID_MaDonVi
			, sLNS =CASE WHEN sLNS=1020000 then 1020100 else sLNS END
			, sL
			, sK
			, sM
			, sTM
			, sTTM
			, sNG
			, sMoTa
			, SUM(rChiTaiKhoBac) AS rChiTaiKhoBac
			, SUM(rTonKho) AS rTonKho
			, SUM(rTuChi) AS rTuChi
			, SUM(rChiTapTrung) AS rChiTapTrung
			, SUM(rHangNhap) AS rHangNhap
			, SUM(rHangMua) AS rHangMua
			, SUM(rHienVat) AS rHienVat
			, SUM(rPhanCap) AS rPhanCap
			, SUM(rDuPhong) AS rDuPhong
			, dDotNgay 
			, iID_MaPhongBan
			, sID_MaNguoiDungTao
	FROM	(SELECT CONVERT(nvarchar, ct.dDot, 103) as dDotNgay, ctct.* 
			 FROM	(SELECT * FROM F_NS_DSCtuBS_TheoDotGom(@iNamLamViec,@iID_MaPhongBan)) as ct 
					INNER JOIN DTBS_ChungTuChiTiet as ctct ON ct.maChungTu = ctct.iID_MaChungTu 
			 WHERE	ctct.iTrangThai = 1 
					AND ctct.iID_MaChungTu IN (SELECT * FROM f_split([dbo].f_ns_dtbs_chungtu_gom(@iNamLamViec,@iID_MaPhongBan)))) as a
	WHERE	iTrangThai = 1 
			AND (MaLoai='' OR MaLoai='2') 
			AND iNamLamViec=@iNamLamViec 
		  	AND iID_MaNamNganSach = 2
			AND iID_MaDonVi IN (SELECT iID_MaDonVi FROM F_NS_DSDvi_TheoND(@iNamLamViec,@sMaNguoiDung))		  
			AND (@iID_MaPhongBan IS NULL OR iID_MaPhongBanDich IN (SELECT * FROM F_SplitString2(@iID_MaPhongBan)))					  	
	GROUP BY iID_MaDonVi
			, sLNS
			, sL
			, sK
			, sM
			, sTM
			, sTTM
			, sNG
			, sMoTa
			, dDotNgay 
			, iID_MaPhongBan
			, sID_MaNguoiDungTao
	HAVING	SUM(rChiTaiKhoBac) <> 0 
			OR SUM(rTonKho) <> 0 
			OR SUM(rTuChi) <> 0 
			OR SUM(rChiTapTrung) <> 0 
			OR SUM(rHangNhap) <> 0 
			OR SUM(rHangMua) <> 0 
			OR SUM(rHienVat) <> 0 
			OR SUM(rPhanCap) <> 0 
			OR SUM(rDuPhong) <> 0
	
	UNION ALL

	-- Chi Tieu Bo Sung Phan Cap trong nam 
	SELECT	iID_MaDonVi
			,sLNS =CASE WHEN sLNS=1020000 then 1020100 else sLNS END
			, sL
			, sK
			, sM
			, sTM
			, sTTM
			, sNG
			, sMoTa
			, 0 AS rChiTaiKhoBac 
			, 0 AS rTonKho
			, SUM(rTuChi) AS rTuChi
			, 0 AS rChiTapTrung
			, 0 AS rHangNhap
			, 0 AS rHangMua
			, SUM(rHienVat) AS rHienVat
			, 0 AS rPhanCap
			, 0 AS rDuPhong
			, dDotNgay 
			, iID_MaPhongBan
			, sID_MaNguoiDungTao
	FROM	f_ns_dtbs_phancap_with_date(@iNamLamViec,@iID_MaPhongBan) 
	GROUP BY iID_MaDonVi
			, sLNS
			, sL
			, sK
			, sM
			, sTM
			, sTTM
			, sNG
			, sMoTa
			, dDotNgay 
			, iID_MaPhongBan
			, sID_MaNguoiDungTao
	HAVING	SUM(rChiTaiKhoBac) <> 0 
			OR SUM(rTonKho) <> 0 
			OR SUM(rTuChi) <> 0 
			OR SUM(rChiTapTrung) <> 0 
			OR SUM(rHangNhap) <> 0 
			OR SUM(rHangMua) <> 0 
			OR SUM(rHienVat) <> 0 
			OR SUM(rPhanCap) <> 0 
			OR SUM(rDuPhong) <> 0

	--- NAM TRUOC CHUYEN SANG ---

	-- DU TOAN BO SUNG --

	UNION ALL 

	SELECT		iID_MaDonVi
				, sLNS = CASE WHEN sLNS=1020000 then 1020100 else sLNS END 
				, sL
				, sK
				, sM
				, sTM
				, sTTM
				, sNG
				, sMoTa				
				, SUM(rChiTaiKhoBac) AS rChiTaiKhoBac
				, SUM(rTonKho) AS rTonKho
				, SUM(rTuChi) AS rTuChi
				, SUM(rChiTapTrung) AS rChiTapTrung
				, SUM(rHangNhap) AS rHangNhap
				, SUM(rHangMua) AS rHangMua
				, SUM(rHienVat) AS rHienVat
				, SUM(rPhanCap) AS rPhanCap
				, SUM(rDuPhong) AS rDuPhong
				, dDotNgay
				, iID_MaPhongBan
				, sID_MaNguoiDungTao	
	FROM		(SELECT CONVERT(nvarchar, ct.dNgayChungTu, 103) as dDotNgay, ctct.* 
				 FROM DTBS_ChungTu AS ct INNER JOIN DTBS_ChungTuChiTiet AS ctct
				 ON ct.iID_MaChungTu = ctct.iID_MaChungTu) as a
	WHERE		iTrangThai=1 
				AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
				AND iNamLamViec=@iNamLamViec 
				AND iID_MaNamNganSach = 4
				AND  (MaLoai='' OR MaLoai='2') 
	GROUP BY	iID_MaDonVi
				, sLNS
				, sL
				, sK
				, sM
				, sTM
				, sTTM
				, sNG
				, sMoTa
				, dDotNgay 
				, iID_MaPhongBan
				, sID_MaNguoiDungTao
	HAVING		SUM(rChiTaiKhoBac) <> 0 
				OR SUM(rTonKho) <> 0 
				OR SUM(rTuChi) <> 0 
				OR SUM(rChiTapTrung) <> 0 
				OR SUM(rHangNhap) <> 0 
				OR SUM(rHangMua) <> 0 
				OR SUM(rHienVat) <> 0 
				OR SUM(rPhanCap) <> 0 
				OR SUM(rDuPhong) <> 0
            
	UNION ALL

	-- DU TOAN BO SUNG - PHAN CAP --
	SELECT		iID_MaDonVi
				, sLNS =CASE WHEN sLNS = 1020000 then 1020100 else sLNS END 
				, sL
				, sK
				, sM
				, sTM
				, sTTM
				, sNG
				, sMoTa				
				, 0 AS rChiTaiKhoBac 
				, 0 AS rTonKho
				, SUM(rTuChi) AS rTuChi
				, 0 AS rChiTapTrung
				, 0 AS rHangNhap
				, 0 AS rHangMua
				, SUM(rHienVat) AS rHienVat
				, 0 AS rPhanCap
				, 0 AS rDuPhong
				, dDotNgay
				, iID_MaPhongBan
				, sID_MaNguoiDungTao
	FROM		(SELECT CONVERT(nvarchar, ct.dNgayChungTu, 103) as dDotNgay, ctpc.*  
				 FROM	DTBS_ChungTu AS ct 
						INNER JOIN DTBS_ChungTuChiTiet as ctct ON ct.iID_MaChungTu = ctct.iID_MaChungTu 
						INNER JOIN DTBS_ChungTuChiTiet_PhanCap as ctpc ON ctct.iID_MaChungTuChiTiet = ctpc.iID_MaChungTu) as b 
	WHERE		iTrangThai=1 
				AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
				AND iNamLamViec=@iNamLamViec 
				AND iID_MaNamNganSach = 4
	GROUP BY	iID_MaDonVi
				, sLNS
				, sL
				, sK
				, sM
				, sTM
				, sTTM
				, sNG
				, sMoTa
				, dDotNgay 
				, iID_MaPhongBan
				, sID_MaNguoiDungTao
	HAVING		SUM(rChiTaiKhoBac) <> 0 
				OR SUM(rTonKho) <> 0 
				OR SUM(rTuChi) <> 0 
				OR SUM(rChiTapTrung) <> 0 
				OR SUM(rHangNhap) <> 0 
				OR SUM(rHangMua) <> 0 
				OR SUM(rHienVat) <> 0 
				OR SUM(rPhanCap) <> 0 
				OR SUM(rDuPhong) <> 0
	) AS ChiTieu 
GROUP BY iID_MaDonVi,sLNS, sL, sK, sM, sTM, sTTM, sNG, sMoTa,dDotNgay,sID_MaNguoiDungTao, iID_MaPhongBan