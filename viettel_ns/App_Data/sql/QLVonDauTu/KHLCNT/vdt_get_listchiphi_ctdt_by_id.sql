

--#DECLARE#--
/*

Lấy danh sách dự chi phí QDDT theo dự án - chi tiết KHLCNT

*/


select dmcp.iID_ChiPhi as iID_DuAn_ChiPhi,
	dmcp.sTenChiPhi,
	dmcp.iID_ChiPhi as iID_ChiPhiID,
	null as iID_ChiPhi_Parent,
	0 as fTienPheDuyet,
	0 as fGiaTriDieuChinh,
	0 as fTienPheDuyetQDDT,
	dmcp.iThuTu
from VDT_DM_ChiPhi dmcp
order by iThuTu desc