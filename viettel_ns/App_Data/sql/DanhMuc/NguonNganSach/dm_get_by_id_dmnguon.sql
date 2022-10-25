--#DECLARE#--

/*

Lấy record sách nội dung chi BQP theo ID

*/

SELECT * 
FROM DM_Nguon 
WHERE iID_Nguon = @iID_Nguon 
	AND bPublic = 1