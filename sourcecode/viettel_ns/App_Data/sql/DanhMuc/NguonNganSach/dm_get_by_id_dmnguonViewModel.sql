--#DECLARE#--

/*

Lấy record sách nội dung chi BQP theo ID

*/


SELECT dmn.*,dmn2.sMaNguon as sMaNguonCha 
FROM DM_Nguon dmn 
LEFT JOIN DM_Nguon dmn2 ON dmn2.iID_Nguon = dmn.iID_NguonCha
WHERE dmn.iID_Nguon = @iID_Nguon 
	AND dmn.bPublic = 1