--#DECLARE#--

/*

Lấy record NNS phân nguồn noi dung chi theo iID_NoiDungChi, iID_Nguon, iID_PhanNguon

*/

select iID_NoiDungChi, SoTien, GhiChu, iID_Nguon, iID_PhanNguon
from NNS_PhanNguon_NDChi
Where iID_NoiDungChi = @iID_NoiDungChi
	AND iID_Nguon = @iIdNguon
	AND iID_PhanNguon = @iIdPhanNguon