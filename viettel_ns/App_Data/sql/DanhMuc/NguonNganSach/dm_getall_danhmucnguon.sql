DECLARE @sMaNguon nvarchar(50)			set @sMaNguon = ''
DECLARE @sNoiDung nvarchar(500)		set @sNoiDung = ''
DECLARE @iID_NguonCha guid(500)		set @iID_NguonCha = ''

--#DECLARE#--

/*

Lấy danh sách nguồn ngân sách

*/

SELECT dmn.* ,dmn2.sMaNguon as sMaNguonCha
FROM DM_Nguon dmn
left join DM_Nguon dmn2 ON dmn2.iID_Nguon = dmn.iID_NguonCha
WHERE (ISNULL(@sMaNguon, '') = '' OR sMaNguon LIKE CONCAT(N'%',@sMaNguon,N'%')) 
	AND (ISNULL(@sNoiDung, '') = '' OR sNoiDung LIKE CONCAT(N'%',@sNoiDung,N'%'))
	AND (@iID_NguonCha = '00000000-0000-0000-0000-000000000000' OR iID_NguonCha = @iID_NguonCha)
	AND bPublic = 1
	
