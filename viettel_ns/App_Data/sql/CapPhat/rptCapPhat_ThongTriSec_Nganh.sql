declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07' 
declare @iID_MaNguonNganSach nvarchar(20)			set @iID_MaNguonNganSach = '1'
declare  @iID_MaNamNganSach  nvarchar(20)			set @iID_MaNamNganSach = '2'
declare @iID_MaCapPhat nvarchar(36)					set @iID_MaCapPhat = '1792556e-f3ad-4b54-86f5-b1e77e108a0c'
declare @MaND nvarchar(20)							set @MaND = '%hangtt%'
declare @dNgayCapPhat datetime						set @dNgayCapPhat = '2/26/2018 12:00:00 AM'

--#DECLARE#--

SELECT	DISTINCT MaNganh as sLNS
		, TenNganh as TenHT 
FROM 
		(SELECT DISTINCT sNG  FROM	CP_CapPhatChiTiet
		 WHERE	iTrangThai=1 
				AND iID_MaCapPhat = @iID_MaCapPhat 
				AND iID_MaPhongBan= @iID_MaPhongBan 
				AND iNamLamViec=@iNamLamViec 
				AND rTuChi <> 0
			) a, 
		(SELECT iID_MaNganh as MaNganh, iID_MaNganhMLNS AS NganhMLNS, iID_MaNganh + ' - ' + sTenNganh as TenNganh 
		 FROM	NS_MucLucNganSach_Nganh 
		 WHERE	iTrangThai = 1 
				AND iNamLamViec=@iNamLamViec 				
				AND sMaNguoiQuanLy LIKE @MaND 
			) b
WHERE	b.NganhMLNS LIKE '%' + a.sNG + '%'
		AND b.MaNganh <> '02'
