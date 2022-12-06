DECLARE @sCode nvarchar(50)			set @sCode = ''
DECLARE @sName nvarchar(500)		set @sName = ''

--#DECLARE#--

/*

Lấy danh sách loại dự toán

*/

SELECT * 
FROM DM_LoaiDuToan 
WHERE (ISNULL(@sCode, '') = '' OR sMaLoaiDuToan LIKE CONCAT('%',@sCode,'%')) 
	AND (ISNULL(@sName, '') = '' OR sTenLoaiDuToan LIKE CONCAT('%',@sName,'%'))
	AND iTrangThai = 1
	--AND iNamLamViec = @iNamLamViec
ORDER BY sMaLoaiDuToan