DECLARE @sMaThongTri nvarchar (MAX) SET @sMaThongTri = 'TT'
DECLARE @iNamThongTri INT SET @iNamThongTri = 2019
DECLARE @dNgayThongTri DATETIME SET @dNgayThongTri = '06/08/2021'
DECLARE @sMaDonVi nvarchar (MAX) SET @sMaDonVi = null
DECLARE @iNamLamViec INT SET @iNamLamViec = 2019
DECLARE @sUserName nvarchar (MAX) SET @sUserName = 'tlb10'
		
--#DECLARE#--
		
SELECT
	thongtri.*,
	dv.sTen AS sTenDonVi 
FROM VDT_ThongTri thongtri
LEFT JOIN NS_DonVi dv ON thongtri.iID_DonViID = dv.iID_Ma 
WHERE
thongtri.iID_DonViID IN (
SELECT
	b.iID_Ma 
FROM
	NS_NguoiDung_DonVi a,
	ns_donvi b 
WHERE
	a.iID_MaDonVi = b.iID_MaDonVi 
	AND a.iNamLamViec = @iNamLamViec 
	AND b.iNamLamViec_DonVi = @iNamLamViec 
	AND a.sMaNguoiDung = @sUserName 
	AND a.iTrangThai = 1 
	AND b.iTrangThai = 1 
) 
AND thongtri.bIsCanBoDuyet = 1 
AND ( ISNULL( @sMaDonVi, '' ) = '' OR dv.iID_MaDonVi = @sMaDonVi ) 
AND ( ISNULL( @sMaThongTri, '' ) = '' OR thongtri.sMaThongTri LIKE CONCAT ( N'%',@sMaThongTri, N'%' ) ) 
AND ( @iNamThongTri IS NULL OR thongtri.iNamThongTri = @iNamThongTri ) 
AND ( @dNgayThongTri IS NULL OR ( thongtri.dNgayThongTri <= @dNgayThongTri ) ) 
ORDER BY dv.sTen 
