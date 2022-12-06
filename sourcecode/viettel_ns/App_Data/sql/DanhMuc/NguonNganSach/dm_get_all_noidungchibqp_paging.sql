DECLARE @sCode nvarchar(50)			set @sCode = ''
DECLARE @sName nvarchar(500)		set @sName = ''
DECLARE @CurrentPage int
DECLARE @ItemsPerPage int
DECLARE @TotalItems int

--#DECLARE#--

/*

Lấy danh sách nội dung chi BQP

*/

SET @TotalItems =  (SELECT COUNT(iID_NoiDungChi) 
					FROM DM_NoiDungChi 
					WHERE (ISNULL(@sCode, '') = '' OR sMaNoiDungChi LIKE CONCAT(N'%',@sCode,N'%')) 
						AND (ISNULL(@sName, '') = '' OR sTenNoiDungChi LIKE CONCAT(N'%',@sName,N'%'))
						AND bPublic = 1)

SELECT * 
FROM DM_NoiDungChi 
WHERE (ISNULL(@sCode, '') = '' OR sMaNoiDungChi LIKE CONCAT(N'%',@sCode,N'%')) 
	AND (ISNULL(@sName, '') = '' OR sTenNoiDungChi LIKE CONCAT(N'%',@sName,N'%'))
	AND bPublic = 1
ORDER BY sMaNoiDungChi
OFFSET (@ItemsPerPage * (@CurrentPage-1)) ROWS
FETCH NEXT @ItemsPerPage ROWS ONLY

