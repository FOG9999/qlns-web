declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07' 
declare @iID_MaNguonNganSach nvarchar(20)			set @iID_MaNguonNganSach = '1'
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi = null
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach = '2'
declare @loai nvarchar(20)							set @loai = '0,1'
declare @sLNS nvarchar(20)							set @sLNS = '1020100'
declare @dNgay datetime								set @dNgay = GETDATE()

--#DECLARE#--/
declare @iID_MaChungTu nvarchar(MAX)
select @iID_MaChungTu = [dbo].[f_ns_dtbs_chungtu_gom_todate](@iNamLamViec,'2',@iID_MaPhongBan,@dNgay)

SELECT		iID_MaDonVi
			, sNG
			, SUM(rHienVat) AS rHienVat
FROM (
		SELECT	sNG,
				iID_MaDonVi,
				loai = 0,
				rHienVat	= SUM(rHienVat)

		FROM    DT_ChungTuChiTiet 
		WHERE   iTrangThai=1 
				AND iNamLamViec=@iNamLamViec 
				AND iID_MaNamNganSach  IN (SELECT * FROM f_split('2'))
				AND sLNs in ('1020100')
				AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
				AND (@iID_MaDonVi is null or iID_MaDonVi in (select * from f_split(@iID_MaDonVi)))
				    
		GROUP BY sNG,iID_MaDonVi
		HAVING SUM(rHienVat)<>0 

		UNION ALL

		-- DU TOAN - PHAN CAP
		SELECT  sNG,
				iID_MaDonVi,
				loai = 0,
				rHienVat	= SUM(rHienVat)

		FROM    DT_ChungTuChiTiet_PhanCap 
		WHERE   iTrangThai=1 
				AND iNamLamViec=@iNamLamViec 
				AND iID_MaNamNganSach IN (SELECT * FROM f_split('2'))
				AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
				AND (@iID_MaDonVi is null or iID_MaDonVi in (select * from f_split(@iID_MaDonVi)))
    
		GROUP BY sNG,iID_MaDonVi  
		HAVING SUM(rHienVat)<>0
                        
                        
		UNION ALL


		---- BO SUNG
		SELECT  sNG,
				iID_MaDonVi,
				loai = 1, 
				rHienVat	=SUM(rHienVat)           

		FROM	DTBS_ChungTuChiTiet 
		WHERE   iTrangThai=1 
				AND iNamLamViec=@iNamLamViec 
				AND iID_MaNamNganSach IN (SELECT * FROM f_split('2'))
				AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
				AND (MaLoai='' OR MaLoai='2')
				AND sLNs in ('1020100')
				AND (@iID_MaDonVi is null or iID_MaDonVi in (select * from f_split(@iID_MaDonVi)))
				AND iID_MaChungTu IN (select * from f_split(@iID_MaChungTu))
            
		GROUP BY sNG,iID_MaDonVi
		HAVING SUM(rHienVat)<>0


		UNION ALL

		SELECT	sNG,
				iID_MaDonVi,
				loai = 1, 
				rHienVat	=SUM(rHienVat)
		FROM    f_ns_dtbs_phancap(@iNamLamViec,@iID_MaPhongBan,@iID_MaDonVi,@iID_MaChungTu)	
		GROUP BY sNG,iID_MaDonVi
		HAVING	SUM(rHienVat)<>0
) as CT		
WHERE		(@loai IS NULL OR loai IN (SELECT * FROM F_Split(@loai)))
GROUP BY	iID_MaDonVi
			, sNG
ORDER BY	iID_MaDonVi, sNG
