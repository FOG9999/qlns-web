declare @iNamLamViec int							set @iNamLamViec = 2019
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='08' 
declare @iID_MaNguonNganSach nvarchar(20)			set @iID_MaNguonNganSach = '2'
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach = '2,4'
declare @iID_MaDonVi nvarchar(10)					set @iID_MaDonVi = '15'
DECLARE @@sLNS NVARCHAR(MAX)						SET @@sLNS = '104%'

--#DECLARE#--

declare @iID_MaChungTu nvarchar(MAX)
set @iID_MaChungTu = [dbo].f_ns_dtbs_chungtu_gom(@iNamLamViec,@iID_MaPhongBan)

SELECT		sLNS
			, sL
			, sK			
			, sM
			, sTM
			, sTTM
			, sNG
			, SUM(rTuChi) AS rTuChi
FROM (	
			-- DU TOAN --

			-- Tu chi -- 
			SELECT		iID_MaDonVi
						, iID_MaMucLucNganSach
						, sXauNoiMa
						, sLNS = CASE WHEN sLNS=1020000 then 1020100 else sLNS END 
						, sL
						, sK
						, sM
						, sTM
						, sTTM
						, sNG
						, sMoTa
						, rTuChi=	CASE    
									WHEN sLNS LIKE '104%' then SUM(rTuChi)
									ELSE SUM(rTuChi+rHangNhap+rHangMua) END
			FROM		DT_ChungTuChiTiet 
			WHERE		iTrangThai = 1 
						AND iNamLamViec=@iNamLamViec 						
						--AND ( @iID_MaNguonNganSach IS NULL OR iID_MaNguonNganSach IN (SELECT * FROM F_Split(@iID_MaNguonNganSach))) 
						AND ( @iID_MaNamNganSach IS NULL OR iID_MaNamNganSach IN (SELECT * FROM F_Split(@iID_MaNamNganSach)))
						AND ( MaLoai='' OR MaLoai='2')  
						AND ( @iID_MaPhongBan IS NULL OR iID_MaPhongBanDich IN (SELECT * FROM F_Split(@iID_MaPhongBan)))
						AND ( iID_MaDonVi = @iID_MaDonVi)
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
						OR SUM(rHangNhap) <> 0
						OR SUM(rHangMua) <> 0

			-- Hang Nhap --	  
			
			UNION ALL 

			SELECT		iID_MaDonVi
						, iID_MaMucLucNganSach
						, sXauNoiMa
						, sLNS='1040200'
						, sL
						, sK
						, sM
						, sTM
						, sTTM
						, sNG
						, sMoTa
						, rTuChi =	SUM(rHangNhap)
			FROM		DT_ChungTuChiTiet 
			WHERE		iTrangThai = 1 
						AND iNamLamViec=@iNamLamViec 						
						--AND ( @iID_MaNguonNganSach IS NULL OR iID_MaNguonNganSach IN (SELECT * FROM F_Split(@iID_MaNguonNganSach))) 
						AND ( @iID_MaNamNganSach IS NULL OR iID_MaNamNganSach IN (SELECT * FROM F_Split(@iID_MaNamNganSach)))
						AND ( MaLoai='' OR MaLoai='2')  
						AND ( @iID_MaPhongBan IS NULL OR iID_MaPhongBanDich IN (SELECT * FROM F_Split(@iID_MaPhongBan)))
						AND ( iID_MaDonVi = @iID_MaDonVi)
						AND sLNS like '104%'
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
			HAVING		SUM(rHangNhap)<>0

			-- Hang Mua -- 			

			UNION ALL 

			SELECT		iID_MaDonVi
						, iID_MaMucLucNganSach
						, sXauNoiMa
						, sLNS='1040300'
						, sL
						, sK
						, sM
						, sTM
						, sTTM
						, sNG
						, sMoTa
						, rTuChi =	SUM(rHangMua)
			FROM		DT_ChungTuChiTiet 
			WHERE		iTrangThai = 1 
						AND iNamLamViec=@iNamLamViec 						
						--AND ( @iID_MaNguonNganSach IS NULL OR iID_MaNguonNganSach IN (SELECT * FROM F_Split(@iID_MaNguonNganSach))) 
						AND ( @iID_MaNamNganSach IS NULL OR iID_MaNamNganSach IN (SELECT * FROM F_Split(@iID_MaNamNganSach)))
						AND ( MaLoai='' OR MaLoai='2')  
						AND ( @iID_MaPhongBan IS NULL OR iID_MaPhongBanDich IN (SELECT * FROM F_Split(@iID_MaPhongBan)))
						AND ( iID_MaDonVi = @iID_MaDonVi)
						AND sLNS like '104%'
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
			HAVING		SUM(rHangMua)<>0
                
			-- DU TOAN - PHAN CAP --				

			UNION ALL

			SELECT		iID_MaDonVi
						, iID_MaMucLucNganSach
						, sXauNoiMa
						, sLNS = CASE WHEN sLNS=1020000 then 1020100 else sLNS END 
						, sL
						, sK
						, sM
						, sTM
						, sTTM
						, sNG
						, sMoTa
						, SUM(rTuChi) AS rTuChi
			FROM		DT_ChungTuChiTiet_PhanCap 
			WHERE		iTrangThai = 1 
						AND iNamLamViec=@iNamLamViec 						
						--AND ( @iID_MaNguonNganSach IS NULL OR iID_MaNguonNganSach IN (SELECT * FROM F_Split(@iID_MaNguonNganSach))) 
						AND ( @iID_MaNamNganSach IS NULL OR iID_MaNamNganSach IN (SELECT * FROM F_Split(@iID_MaNamNganSach)))
						AND ( @iID_MaPhongBan IS NULL OR iID_MaPhongBanDich IN (SELECT * FROM F_Split(@iID_MaPhongBan)))
						AND ( iID_MaDonVi = @iID_MaDonVi)
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
			HAVING		SUM(rTuChi) <>0
                   
			-- NAM NAY -- 

			-- DU TOAN BO SUNG --

			UNION ALL

			SELECT		iID_MaDonVi
						, iID_MaMucLucNganSach
						, sXauNoiMa
						, sLNS = CASE WHEN sLNS=1020000 then 1020100 else sLNS END 
						, sL
						, sK
						, sM
						, sTM
						, sTTM
						, sNG
						, sMoTa
						, SUM(rTuChi) AS rTuChi
			FROM		DTBS_ChungTuChiTiet 
			WHERE		iTrangThai = 1 
						AND iNamLamViec = @iNamLamViec 						
						--AND ( @iID_MaNguonNganSach IS NULL OR iID_MaNguonNganSach IN (SELECT * FROM F_Split(@iID_MaNguonNganSach))) 
						AND ( @iID_MaNamNganSach IS NULL OR iID_MaNamNganSach IN (SELECT * FROM F_Split(@iID_MaNamNganSach)))
						AND ( MaLoai='' OR MaLoai='2') 
						AND ( @iID_MaPhongBan IS NULL OR iID_MaPhongBanDich IN (SELECT * FROM F_Split(@iID_MaPhongBan)))
						AND ( iID_MaDonVi = @iID_MaDonVi)
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
			HAVING		SUM(rTuChi) <>0
                   
			-- DTBS - Hang Nhap --

			UNION ALL

			SELECT		iID_MaDonVi
						, iID_MaMucLucNganSach
						, sXauNoiMa
						, sLNS = '1040200'
						, sL
						, sK
						, sM
						, sTM
						, sTTM
						, sNG
						, sMoTa
						, rTuChi =	SUM(rHangNhap)
			FROM		DTBS_ChungTuChiTiet 
			WHERE		iTrangThai = 1 
						AND iNamLamViec = @iNamLamViec 						
						--AND ( @iID_MaNguonNganSach IS NULL OR iID_MaNguonNganSach IN (SELECT * FROM F_Split(@iID_MaNguonNganSach))) 
						AND ( @iID_MaNamNganSach IS NULL OR iID_MaNamNganSach IN (SELECT * FROM F_Split(@iID_MaNamNganSach)))
						AND ( MaLoai='' OR MaLoai='2') 
						AND ( @iID_MaPhongBan IS NULL OR iID_MaPhongBanDich IN (SELECT * FROM F_Split(@iID_MaPhongBan)))
						AND ( iID_MaDonVi = @iID_MaDonVi)
						AND iID_MaChungTu IN (SELECT * FROM f_split(@iID_MaChungTu))
						AND sLNS like '104%'
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
			HAVING		SUM(rHangNhap)<>0 

			-- DTBS - Hang Mua --

			UNION ALL

			SELECT		iID_MaDonVi
						, iID_MaMucLucNganSach
						, sXauNoiMa
						, sLNS = '1040300'
						, sL
						, sK
						, sM
						, sTM
						, sTTM
						, sNG
						, sMoTa
						, rTuChi =	SUM(rHangMua)
			FROM		DTBS_ChungTuChiTiet 
			WHERE		iTrangThai = 1 
						AND iNamLamViec = @iNamLamViec 						
						--AND ( @iID_MaNguonNganSach IS NULL OR iID_MaNguonNganSach IN (SELECT * FROM F_Split(@iID_MaNguonNganSach))) 
						AND ( @iID_MaNamNganSach IS NULL OR iID_MaNamNganSach IN (SELECT * FROM F_Split(@iID_MaNamNganSach)))
						AND ( MaLoai='' OR MaLoai='2') 
						AND ( @iID_MaPhongBan IS NULL OR iID_MaPhongBanDich IN (SELECT * FROM F_Split(@iID_MaPhongBan)))
						AND ( iID_MaDonVi = @iID_MaDonVi)
						AND iID_MaChungTu IN (SELECT * FROM f_split(@iID_MaChungTu))
						AND sLNS like '104%'
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
			HAVING		SUM(rHangMua)<>0

			-- DU TOAN BO SUNG - PHAN CAP --

			UNION ALL

			SELECT		iID_MaDonVi
						, iID_MaMucLucNganSach
						, sXauNoiMa
						, sLNS =CASE WHEN sLNS = 1020000 then 1020100 else sLNS END 
						, sL
						, sK
						, sM
						, sTM
						, sTTM
						, sNG
						, sMoTa
						, SUM(rTuChi+rHangNhap+rHangMua) AS rTuChi
			FROM		f_ns_dtbs_phancap(@iNamLamViec,@iID_MaPhongBan,@iID_MaDonVi,@iID_MaChungTu) 
			WHERE								
						( @iID_MaNamNganSach IS NULL OR iID_MaNamNganSach IN (SELECT * FROM F_Split(@iID_MaNamNganSach)))
						--AND ( @iID_MaNguonNganSach IS NULL OR iID_MaNguonNganSach IN (SELECT * FROM F_Split(@iID_MaNguonNganSach))) 
						
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
			HAVING		SUM(rTuChi)<>0 
						OR SUM(rHangNhap)<>0 
						OR SUM(rHangMua)<>0

			--- CHI TIEU NAM TRUOC ---

			-- DU TOAN BO SUNG --

			UNION ALL 

			SELECT	iID_MaDonVi
						, iID_MaMucLucNganSach
						, sXauNoiMa
						, sLNS = CASE WHEN sLNS=1020000 then 1020100 else sLNS END 
						, sL
						, sK
						, sM
						, sTM
						, sTTM
						, sNG
						, sMoTa
						, SUM(rTuChi+rHangMua+rHangNhap) as rTuChi

			FROM		DTBS_ChungTuChiTiet 
			WHERE		iTrangThai=1 
						AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
						AND iNamLamViec=@iNamLamViec 
						AND ( @iID_MaNamNganSach IS NULL OR iID_MaNamNganSach IN (SELECT * FROM F_Split(@iID_MaNamNganSach)))
						AND iID_MaNamNganSach <> '2'
						AND  (MaLoai='' OR MaLoai='2') 
						AND ( iID_MaDonVi = @iID_MaDonVi)


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
			HAVING		SUM(rTuChi+rHangMua+rHangNhap)<>0 
            
			UNION ALL

			-- DU TOAN BO SUNG - PHAN CAP --
			SELECT		iID_MaDonVi
						, iID_MaMucLucNganSach
						, sXauNoiMa
						, sLNS =CASE WHEN sLNS = 1020000 then 1020100 else sLNS END 
						, sL
						, sK
						, sM
						, sTM
						, sTTM
						, sNG
						, sMoTa
						, SUM(rTuChi+rHangNhap+rHangMua) AS rTuChi
			FROM		DTBS_ChungTuChiTiet_PhanCap 
			WHERE		iTrangThai=1 
						AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
						AND iNamLamViec=@iNamLamViec 
						AND ( @iID_MaNamNganSach IS NULL OR iID_MaNamNganSach IN (SELECT * FROM F_Split(@iID_MaNamNganSach)))
						AND iID_MaNamNganSach <> '2'
						AND ( iID_MaDonVi = @iID_MaDonVi)

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
			HAVING		SUM(rTuChi+rHangMua+rHangNhap)<>0

			UNION ALL

			-- NS đặc biệt --
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
						, SUM(rTuChi) AS rTuChi
			FROM		CP_CapPhatChiTiet 
			WHERE		iTrangThai=1 
						AND (@iID_MaPhongBan is NULL OR iID_MaPhongBan=@iID_MaPhongBan)  
						AND iNamLamViec=@iNamLamViec 
						AND ( @iID_MaNamNganSach IS NULL OR iID_MaNamNganSach IN (SELECT * FROM F_Split(@iID_MaNamNganSach)))
						AND ( iID_MaDonVi = @iID_MaDonVi)

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
			HAVING		SUM(rTuChi)<>0
) as CT
WHERE		(sLNS like @@sLNS) @@DKLNSDB
GROUP BY	sLNS
			, sL
			, sK
			, sM
			, sTM
			, sTTM
			, sNG			
ORDER BY	sLNS
			, sL
			, sK
			, sM
			, sTM
			, sTTM
			, sNG
