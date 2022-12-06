declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07' 
declare @iID_MaNguonNganSach nvarchar(20)			set @iID_MaNguonNganSach = '1'
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach = '2'
declare @sLNS nvarchar(20)							set @sLNS = '4080300'
declare @dNgayCapPhat datetime						set @dNgayCapPhat = '2/26/2018 12:00:00 AM'

--#DECLARE#--/

SELECT		iID_MaDonVi
			, sNG
			, SUM(rHienVat) AS rHienVat
FROM (

			SELECT		iID_MaDonVi
						, sNG
						, SUM(rHienVat) AS rHienVat
			FROM		DT_ChungTuChiTiet 
			WHERE		iTrangThai = 1 
						AND (sLNS='1020100' OR sLNS='1020000')
						AND iNamLamViec=@iNamLamViec 
						AND ( @iID_MaNguonNganSach IS NULL OR iID_MaNguonNganSach IN (SELECT * FROM F_Split(@iID_MaNguonNganSach))) 
						AND ( @iID_MaNamNganSach IS NULL OR iID_MaNamNganSach IN (SELECT * FROM F_Split(@iID_MaNamNganSach)))  
						AND ( @iID_MaPhongBan IS NULL OR iID_MaPhongBanDich IN (SELECT * FROM F_Split(@iID_MaPhongBan)))
			GROUP BY	iID_MaDonVi
						, sNG
            HAVING		SUM(rHienVat)<>0
                    
			UNION ALL

			SELECT		iID_MaDonVi
						, sNG
						, SUM(rHienVat) AS rHienVat
			FROM		DT_ChungTuChiTiet_PhanCap 
			WHERE		iTrangThai = 1 
						AND (sLNS='1020100' OR sLNS='1020000')
						AND iNamLamViec=@iNamLamViec 
						AND ( @iID_MaNguonNganSach IS NULL OR iID_MaNguonNganSach IN (SELECT * FROM F_Split(@iID_MaNguonNganSach))) 
						AND ( @iID_MaNamNganSach IS NULL OR iID_MaNamNganSach IN (SELECT * FROM F_Split(@iID_MaNamNganSach)))  
						AND ( @iID_MaPhongBan IS NULL OR iID_MaPhongBanDich IN (SELECT * FROM F_Split(@iID_MaPhongBan)))
			GROUP BY	iID_MaDonVi
						, sNG
            HAVING		SUM(rHienVat)<>0
                    
			UNION ALL

			SELECT		iID_MaDonVi
						, sNG
						, SUM(rHienVat) AS rHienVat
			FROM		DTBS_ChungTuChiTiet 
			WHERE		iTrangThai = 1 
						AND (sLNS='1020100' OR sLNS='1020000')
						AND iNamLamViec=@iNamLamViec 
						AND ( @iID_MaNguonNganSach IS NULL OR iID_MaNguonNganSach IN (SELECT * FROM F_Split(@iID_MaNguonNganSach))) 
						AND ( @iID_MaNamNganSach IS NULL OR iID_MaNamNganSach IN (SELECT * FROM F_Split(@iID_MaNamNganSach)))  
						AND ( @iID_MaPhongBan IS NULL OR iID_MaPhongBanDich IN (SELECT * FROM F_Split(@iID_MaPhongBan)))
						AND iID_MaChungTu IN (SELECT maChungTu FROM F_NS_DSCtuBS_TheoDotGom(@iNamLamViec,@iID_MaPhongBan))
			GROUP BY	iID_MaDonVi
						, sNG
            HAVING		SUM(rHienVat)<>0
                    
			UNION ALL

			SELECT		iID_MaDonVi
						, sNG
						, SUM(rHienVat) AS rHienVat
			FROM		DTBS_ChungTuChiTiet_PhanCap as pc 
						INNER JOIN	(SELECT	iID_MaChungTuChiTiet
											, iID_MaChungTu 
									 FROM	DTBS_ChungTuChiTiet
									) as ctct 
						ON ctct.iID_MaChungTuChiTiet = pc.iID_MaChungTu 
			WHERE		iTrangThai = 1 
						AND (sLNS='1020100' OR sLNS='1020000')
						AND iNamLamViec=@iNamLamViec 
						AND ( @iID_MaNguonNganSach IS NULL OR iID_MaNguonNganSach IN (SELECT * FROM F_Split(@iID_MaNguonNganSach))) 
						AND ( @iID_MaNamNganSach IS NULL OR iID_MaNamNganSach IN (SELECT * FROM F_Split(@iID_MaNamNganSach)))  
						AND ( @iID_MaPhongBan IS NULL OR iID_MaPhongBanDich IN (SELECT * FROM F_Split(@iID_MaPhongBan)))
						AND ( ctct.iID_MaChungTu IN (SELECT maChungTu FROM F_NS_DSCtuBS_TheoDotGom(@iNamLamViec,@iID_MaPhongBan)) OR
                              ctct.iID_MaChungTu in (SELECT iID_MaChungTuChiTiet 
													 FROM	DTBS_ChungTuChiTiet 
                                                     WHERE  iID_MaChungTu in (SELECT iID_MaChungTu 
																			  FROM	 DTBS_ChungTu
                                                                              WHERE  iID_MaChungTu in (SELECT iID_MaDuyetDuToanCuoiCung 
																									   FROM	  DTBS_ChungTu
                                                                                                       WHERE iID_MaChungTu in (SELECT maChungTu FROM F_NS_DSCtuBS_TheoDotGom(@iNamLamViec,@iID_MaPhongBan)))))) 						
			GROUP BY	iID_MaDonVi
						, sNG
            HAVING		SUM(rHienVat)<>0
) as CT	
GROUP BY	iID_MaDonVi
			, sNG
ORDER BY	sNG
			, iID_MaDonVi




			