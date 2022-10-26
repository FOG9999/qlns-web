
--#DECLARE#--
/*

Lấy danh sách dự nguồn vốn phê duyệt dự án theo dự án- màn chi tiểt KHLCNT

*/

select qdnv.iID_QDDauTu_NguonVonID,
qdnv.fTienPheDuyet,
qdnv.fTienPheDuyet - isnull(qdnv.fGiaTriDieuChinh,0) as fGiaTriTruocDieuChinh,
qdnv.iID_NguonVonID,
nv.sTen as sTenNguonVon,
qdnv.fTienPheDuyet,
qdnv.fGiaTriDieuChinh
from VDT_DA_QDDauTu_NguonVon qdnv
inner join NS_NguonNganSach nv ON nv.iID_MaNguonNganSach = qdnv.iID_NguonVonID
where iID_DuToanID = @duToanId