declare @iNamLamViec int							set @iNamLamViec = 2018
declare @userName nvarchar(200)						set @userName = 'chuctc'
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07' 
declare @iID_MaDonVi nvarchar(2000)					set @iID_MaDonVi=null
declare @iID_MaNguonNganSach nvarchar(20)			set @iID_MaNguonNganSach=null 
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach=null 
declare @Nganh nvarchar(2000)						set @Nganh=null

--#DECLARE#--/

declare @iID_MaChungTu nvarchar(MAX)
set @iID_MaChungTu = [dbo].f_ns_dtbs_chungtu_gom(@iNamLamViec,@iID_MaPhongBan)

SELECT	DISTINCT B.iID_MaNganh as iID_MaNganh
		, B.sTenNganh as sTenNganh 
		, A.sNG as sNG
FROM 
		(SELECT	DISTINCT sNG		 
		 FROM	DT_ChungTuChiTiet 
		 WHERE	iTrangThai = 1 
				AND (sLNS = '1020100' OR sLNS = '1020000')
				AND iNamLamViec = @iNamLamViec 
				AND ( @iID_MaPhongBan IS NULL OR iID_MaPhongBanDich IN (SELECT * FROM F_Split(@iID_MaPhongBan)))
				AND ( @iID_MaDonVi IS NULL OR iID_MaDonVi IN (SELECT * FROM F_Split(@iID_MaDonVi)))
				AND ( @iID_MaNguonNganSach IS NULL OR iID_MaNguonNganSach IN (SELECT * FROM F_Split(@iID_MaNguonNganSach))) 
				AND ( @iID_MaNamNganSach IS NULL OR iID_MaNamNganSach IN (SELECT * FROM F_Split(@iID_MaNamNganSach)))  
				AND rHienVat <> 0		 

		 UNION 

		 SELECT DISTINCT sNG
		 FROM	DT_ChungTuChiTiet_PhanCap 
		 WHERE	iTrangThai = 1 
				AND (sLNS = '1020100' OR sLNS = '1020000')
				AND iNamLamViec = @iNamLamViec 
				AND ( @iID_MaPhongBan IS NULL OR iID_MaPhongBanDich IN (SELECT * FROM F_Split(@iID_MaPhongBan)))
				AND ( @iID_MaDonVi IS NULL OR iID_MaDonVi IN (SELECT * FROM F_Split(@iID_MaDonVi)))
				AND ( @iID_MaNguonNganSach IS NULL OR iID_MaNguonNganSach IN (SELECT * FROM F_Split(@iID_MaNguonNganSach))) 
				AND ( @iID_MaNamNganSach IS NULL OR iID_MaNamNganSach IN (SELECT * FROM F_Split(@iID_MaNamNganSach)))  
				AND rHienVat <> 0

		 UNION 

		 SELECT DISTINCT sNG
		 FROM	DTBS_ChungTuChiTiet 
		 WHERE	iTrangThai = 1 
				AND (sLNS='1020100' OR sLNS='1020000')
				AND iNamLamViec=@iNamLamViec 
				AND ( @iID_MaPhongBan IS NULL OR iID_MaPhongBanDich IN (SELECT * FROM F_Split(@iID_MaPhongBan)))
				AND ( @iID_MaDonVi IS NULL OR iID_MaDonVi IN (SELECT * FROM F_Split(@iID_MaDonVi)))
				AND ( @iID_MaNguonNganSach IS NULL OR iID_MaNguonNganSach IN (SELECT * FROM F_Split(@iID_MaNguonNganSach))) 
				AND ( @iID_MaNamNganSach IS NULL OR iID_MaNamNganSach IN (SELECT * FROM F_Split(@iID_MaNamNganSach)))  
				AND iID_MaChungTu IN (SELECT maChungTu FROM F_NS_DSCtuBS_TheoDotGom(@iNamLamViec,@iID_MaPhongBan))
				AND rHienVat <> 0

		 UNION 

		 SELECT DISTINCT sNG
		 FROM	f_ns_dtbs_phancap(@iNamLamViec,@iID_MaPhongBan, @iID_MaDonVi,@iID_MaChungTu) 
		) AS A
		, NS_MucLucNganSach_Nganh AS B
WHERE	B.iID_MaNganhMLNS LIKE '%' + A.sNG + '%'
		AND B.iNamLamViec = @iNamLamViec
		--AND B.sMaNguoiQuanLy like '%' + @userName + '%'
		AND (@Nganh IS NULL OR iID_MaNganh IN (SELECT * FROM F_Split(@Nganh)))	
		AND iID_MaNganh <> '02'
