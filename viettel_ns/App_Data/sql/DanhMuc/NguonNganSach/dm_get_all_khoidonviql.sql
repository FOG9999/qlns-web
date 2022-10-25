DECLARE @sMaKhoi nvarchar(50)			set @sMaKhoi = ''
DECLARE @sTenKhoi nvarchar(500)		set @sTenKhoi = ''

--#DECLARE#--

/*

Lấy danh sách nội dung chi BQP

*/

SELECT * 
FROM DM_KhoiDonViQuanLy 
WHERE (ISNULL(@sMaKhoi, '') = '' OR sMaKhoi LIKE CONCAT(N'%',@sMaKhoi,N'%')) 
	AND (ISNULL(@sTenKhoi, '') = '' OR sTenKhoi LIKE CONCAT(N'%',@sTenKhoi,N'%'))
	AND bPublic = 1