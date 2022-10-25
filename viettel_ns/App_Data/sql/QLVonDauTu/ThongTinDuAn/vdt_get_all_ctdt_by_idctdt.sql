--#DECLARE#--

/*

Lấy all chu truong dau tu chi phi theo ID ctdt

*/

select *
from VDT_DA_ChuTruongDauTu 
where iID_ChuTruongDauTuID = @iID_ChuTruongDauTuID
OR iID_ParentID = @iID_ChuTruongDauTuID




