declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07' 
declare @iID_MaNguonNganSach nvarchar(20)			set @iID_MaNguonNganSach = '1'
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach = '2'
declare @sLNS nvarchar(20)							set @sLNS = '4080300'
declare @dNgayCapPhat datetime						set @dNgayCapPhat = '2/26/2018 12:00:00 AM'

--#DECLARE#--

SELECT	DISTINCT iID_MaDonVi, sTenDonVi as TenHT 
FROM 
		(SELECT DISTINCT iID_MaDonVi, sTenDonVi, sNG 
		 FROM CP_CapPhatChiTiet
         WHERE	iTrangThai=1 
				AND iID_MaCapPhat = @iID_MaCapPhat 
				AND sTenDonVi<>'' AND iID_MaDonVi<>''
				AND iID_MaPhongBan=@iID_MaPhongBan 
				AND iNamLamViec=@iNamLamViec) a, 
		(SELECT iID_MaNganhMLNS AS NganhMLNS 
		 FROM	NS_MucLucNganSach_Nganh 
         WHERE	iTrangThai = 1  
				AND iNamLamViec=@iNamLamViec 
				AND (@Nganh IS NULL OR iID_MaNganh IN (SELECT * FROM F_Split(@Nganh)))) b
WHERE	 b.NganhMLNS LIKE '%' + a.sNG + '%'
