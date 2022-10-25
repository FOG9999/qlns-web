--#DECLARE#--

/*

Lấy record sách nội dung chi BQP theo ID

*/

SELECT ndc.* , parent.sMaNoiDungChi as sMaNoiDungChiCha
FROM DM_NoiDungChi ndc
LEFT JOIN DM_NoiDungChi parent on ndc.iID_Parent = parent.iID_NoiDungChi
WHERE ndc.iID_NoiDungChi = @iId 
	AND ndc.bPublic = 1