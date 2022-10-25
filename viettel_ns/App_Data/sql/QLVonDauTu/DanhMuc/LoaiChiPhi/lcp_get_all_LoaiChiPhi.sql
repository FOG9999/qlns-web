DECLARE @sMaChiPhi nvarchar(50)					set @sMaChiPhi = ''
DECLARE @sTenChiPhi nvarchar(500)				set @sTenChiPhi = ''
--#DECLARE#--

/*

Lấy danh sách Loại Chi Phí

*/

SELECT * 
FROM VDT_DM_ChiPhi 
WHERE (ISNULL(@sMaChiPhi, '') = '' OR sMaChiPhi LIKE CONCAT(N'%',@sMaChiPhi,N'%')) 
	AND (ISNULL(@sTenChiPhi, '') = '' OR sTenChiPhi LIKE CONCAT(N'%',@sTenChiPhi,N'%')) 
ORDER BY sMaChiPhi