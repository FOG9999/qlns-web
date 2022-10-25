
--#DECLARE#--
/*

Lấy danh sách dự nguồn vốn chủ trương đầu tư theo dự án- màn tạo mới phê duyệt dự án

*/


select dacp.iID_DuAn_ChiPhi,
	dacp.iID_ChiPhi_Parent,
	dacp.iID_DuAn_ChiPhi as Id,
	qdcp.iID_QDDauTu_ChiPhiID,
	qdcp.fTienPheDuyet,
	qdcp.fTienPheDuyet - isnull(qdcp.fGiaTriDieuChinh,0) as fGiaTriTruocDieuChinh,
	0 as fGiaTriDieuChinh,
	qdcp.iID_DuAn_ChiPhi as iID_ChiPhiID,
	dacp.sTenChiPhi,
	dacp.iThuTu,
	qdcp.iID_QDDauTuID
from VDT_DA_QDDauTu_ChiPhi qdcp
	inner join VDT_DM_DuAn_ChiPhi dacp ON dacp.iID_DuAn_ChiPhi = qdcp.iID_DuAn_ChiPhi
where qdcp.iID_QDDauTuID = @qdDauTuId
order by iThuTu,dacp.iID_ChiPhi_Parent,dacp.sTenChiPhi