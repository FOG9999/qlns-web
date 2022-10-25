--#DECLARE#--

/*

Lấy record sách nội dung chi BQP theo ID

*/

SELECT * 
FROM DM_NoiDungChi 
WHERE iID_NoiDungChi = @iId 
	AND bPublic = 1