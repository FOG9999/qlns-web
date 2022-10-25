--#DECLARE#--

/*

Lấy record sách loại dự toán theo ID

*/

SELECT * 
FROM DM_LoaiDuToan 
WHERE iID_LoaiDuToan = @iId 
	AND bPublic = 1