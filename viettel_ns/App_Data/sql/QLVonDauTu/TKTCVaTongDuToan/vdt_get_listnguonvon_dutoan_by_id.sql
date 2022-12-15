
--#DECLARE#--
/*

Lấy danh sách dự nguồn vốn chủ trương đầu tư theo dự án- màn tạo mới phê duyệt dự án

*/

select qdnv.iID_DuToan_NguonVonID,
qdnv.fTienPheDuyet,
qdnv.fTienPheDuyet - isnull(qdnv.fGiaTriDieuChinh,0) as fGiaTriTruocDieuChinh,
qdnv.iID_NguonVonID,
nv.sTen as sTenNguonVon,
qdnv.fTienPheDuyetQDDT,
qdnv.fTienPheDuyet as fGiaTriDieuChinh
from VDT_DA_DuToan_Nguonvon qdnv
inner join NS_NguonNganSach nv ON nv.iID_MaNguonNganSach = qdnv.iID_NguonVonID
where iID_DuToanID = @duToanId