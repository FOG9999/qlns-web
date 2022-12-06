--#DECLARE#--

/*

Lấy record sách nội dung chi BQP theo ID

*/

SELECT * 
FROM DM_KhoiDonViQuanLy 
WHERE iID_Khoi = @iId 
	AND bPublic = 1