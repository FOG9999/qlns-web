
--#DECLARE#--
/*

Lấy danh sách dự nguồn vốn chủ trương đầu tư theo dự án- màn tạo mới phê duyệt dự án

*/


select qdnv.iID_QDDauTu_NguonVonID,
qdnv.fTienPheDuyetCTDT,
qdnv.fTienPheDuyet,
qdnv.fTienPheDuyet - isnull(qdnv.fGiaTriDieuChinh,0) as fGiaTriTruocDieuChinh,
qdnv.iID_NguonVonID,
nv.sTen as sTenNguonVon
from VDT_DA_QDDauTu_NguonVon qdnv
inner join NS_NguonNganSach nv ON nv.iID_MaNguonNganSach = qdnv.iID_NguonVonID
where iID_QDDauTuID = @qdDauTuId