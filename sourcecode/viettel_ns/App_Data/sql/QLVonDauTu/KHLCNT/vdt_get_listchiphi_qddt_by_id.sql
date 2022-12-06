

--#DECLARE#--
/*

Lấy danh sách dự chi phí QDDT theo dự án - chi tiết KHLCNT

*/


select dacp.iID_DuAn_ChiPhi,
	dacp.sTenChiPhi,
	dacp.iID_ChiPhi as iID_ChiPhiID,
	dacp.iID_ChiPhi_Parent,
	dtcp.fTienPheDuyet,
	dtcp.fGiaTriDieuChinh,
	dtcp.fTienPheDuyet as fTienPheDuyetQDDT,
	dacp.iThuTu
from VDT_DA_QDDauTu_ChiPhi dtcp
	inner join VDT_DM_DuAn_ChiPhi dacp ON dacp.iID_DuAn_ChiPhi = dtcp.iID_DuAn_ChiPhi
where dtcp.iID_QDDauTuID = @duToanId
order by iThuTu desc