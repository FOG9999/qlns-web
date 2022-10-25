--#DECLARE#--

/*

Lấy all chu truong dau tu chi phi theo ID ctdt

*/

select ctdt_cp.*, dm_cp.sTenChiPhi
from VDT_DA_ChuTruongDauTu_ChiPhi ctdt_cp
left join VDT_DM_ChiPhi dm_cp on ctdt_cp.iID_ChiPhiID = dm_cp.iID_ChiPhi
where iID_ChuTruongDauTuID = @iID_ChuTruongDauTuID




