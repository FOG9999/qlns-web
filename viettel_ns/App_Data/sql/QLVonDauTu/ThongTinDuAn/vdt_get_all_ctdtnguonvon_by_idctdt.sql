--#DECLARE#--

/*

Lấy all chu truong dau tu nguon von theo ID ctdt

*/

select ctdt_nv.*, ns_nns.sTen as sTenNguonGocVon
from VDT_DA_ChuTruongDauTu_NguonVon ctdt_nv
left join NS_NguonNganSach ns_nns on ctdt_nv.iID_NguonVonID = ns_nns.iID_MaNguonNganSach
where ctdt_nv.iID_ChuTruongDauTuID = @iID_ChuTruongDauTuID

