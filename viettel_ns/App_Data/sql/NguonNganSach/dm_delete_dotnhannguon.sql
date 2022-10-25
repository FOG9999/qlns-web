--#DECLARE#--

/*

xóa đợt nhận

*/
delete NNS_DotNhanChiTiet_NDChi 
where iID_DotNhanChiTiet in (select iID_DotNhanChiTiet from NNS_DotNhanChiTiet where iID_DotNhan = @iId)

DELETE NNS_DotNhanChiTiet
WHERE iID_DotNhan = @iId


DELETE NNS_DotNhan
WHERE iID_DotNhan = @iId

	