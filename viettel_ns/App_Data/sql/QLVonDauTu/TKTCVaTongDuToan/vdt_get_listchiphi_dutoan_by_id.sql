

--#DECLARE#--
/*

Lấy danh sách dự chi phí dự toán theo dự án-

*/


select dacp.iID_DuAn_ChiPhi,
	dacp.sTenChiPhi,
	dacp.iID_ChiPhi as iID_ChiPhiID,
	dacp.iID_ChiPhi_Parent,
	dtcp.fTienPheDuyet,
	dtcp.fGiaTriDieuChinh,
	dtcp.fTienPheDuyetQDDT,
	dacp.iThuTu
from VDT_DA_DuToan_ChiPhi dtcp
	inner join VDT_DM_DuAn_ChiPhi dacp ON dacp.iID_DuAn_ChiPhi = dtcp.iID_DuAn_ChiPhi
where dtcp.iID_DuToanID = @duToanId
order by iThuTu desc