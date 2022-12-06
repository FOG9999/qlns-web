declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07' 
declare @iID_MaNguonNganSach nvarchar(20)			set @iID_MaNguonNganSach = '1'
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach = '2'
declare @sLNS nvarchar(20)							set @sLNS = '4080300'
declare @dNgayCapPhat datetime						set @dNgayCapPhat = '2/26/2018 12:00:00 AM'

--#DECLARE#--/

declare @iID_MaChungTu nvarchar(MAX)
set @iID_MaChungTu = [dbo].f_ns_dtbs_chungtu_gom(@iNamLamViec,@iID_MaPhongBan)

SELECT		iID_MaDonVi
			, sNG
			, SUM(rHienVat) AS rHienVat
FROM (
			-- DU TOAN --

			SELECT		iID_MaDonVi
						, sNG
						, SUM(rHienVat) AS rHienVat
			FROM		DT_ChungTuChiTiet 
			WHERE		iTrangThai = 1 
						AND iNamLamViec=@iNamLamViec 
						AND (sLNS='1020100' OR sLNS='1020000')	
						AND ( @iID_MaNguonNganSach IS NULL OR iID_MaNguonNganSach IN (SELECT * FROM F_Split(@iID_MaNguonNganSach))) 
						AND ( @iID_MaNamNganSach IS NULL OR iID_MaNamNganSach IN (SELECT * FROM F_Split(@iID_MaNamNganSach)))  
						AND ( @iID_MaPhongBan IS NULL OR iID_MaPhongBanDich IN (SELECT * FROM F_Split(@iID_MaPhongBan)))
			GROUP BY	iID_MaDonVi
						, sNG
            HAVING		SUM(rHienVat)<>0
                    
			-- DU TOAN - PHAN CAP --

			UNION ALL

			SELECT		iID_MaDonVi
						, sNG
						, SUM(rHienVat) AS rHienVat
			FROM		DT_ChungTuChiTiet_PhanCap 
			WHERE		iTrangThai = 1 
						AND iNamLamViec=@iNamLamViec 
						AND (sLNS='1020100' OR sLNS='1020000')	
						AND ( @iID_MaNguonNganSach IS NULL OR iID_MaNguonNganSach IN (SELECT * FROM F_Split(@iID_MaNguonNganSach))) 
						AND ( @iID_MaNamNganSach IS NULL OR iID_MaNamNganSach IN (SELECT * FROM F_Split(@iID_MaNamNganSach)))  
						AND ( @iID_MaPhongBan IS NULL OR iID_MaPhongBanDich IN (SELECT * FROM F_Split(@iID_MaPhongBan)))
			GROUP BY	iID_MaDonVi
						, sNG
            HAVING		SUM(rHienVat)<>0
                    
			-- DU TOAN BO SUNG --

			UNION ALL

			SELECT		iID_MaDonVi
						, sNG
						, SUM(rHienVat) AS rHienVat
			FROM		DTBS_ChungTuChiTiet 
			WHERE		iTrangThai = 1 						
						AND iNamLamViec=@iNamLamViec 
						AND (sLNS='1020100' OR sLNS='1020000')	
						AND ( @iID_MaNguonNganSach IS NULL OR iID_MaNguonNganSach IN (SELECT * FROM F_Split(@iID_MaNguonNganSach))) 
						AND ( @iID_MaNamNganSach IS NULL OR iID_MaNamNganSach IN (SELECT * FROM F_Split(@iID_MaNamNganSach)))  
						AND ( @iID_MaPhongBan IS NULL OR iID_MaPhongBanDich IN (SELECT * FROM F_Split(@iID_MaPhongBan)))
						AND iID_MaChungTu IN (SELECT * FROM f_split(@iID_MaChungTu))
			GROUP BY	iID_MaDonVi
						, sNG
            HAVING		SUM(rHienVat)<>0
                    
			-- DU TOAN BO SUNG - PHAN CAP --

			UNION ALL

			SELECT		iID_MaDonVi
						, sNG
						, SUM(rHienVat) AS rHienVat
			FROM		f_ns_dtbs_phancap(@iNamLamViec,@iID_MaPhongBan, @iID_MaDonVi,@iID_MaChungTu)
			WHERE		(sLNS='1020100' OR sLNS='1020000')	
						AND ( @iID_MaNguonNganSach IS NULL OR iID_MaNguonNganSach IN (SELECT * FROM F_Split(@iID_MaNguonNganSach))) 
						AND ( @iID_MaNamNganSach IS NULL OR iID_MaNamNganSach IN (SELECT * FROM F_Split(@iID_MaNamNganSach)))  
			GROUP BY	iID_MaDonVi
						, sNG
            HAVING		SUM(rHienVat)<>0


) as CT			
GROUP BY	iID_MaDonVi
			, sNG
ORDER BY	sNG
			, iID_MaDonVi
