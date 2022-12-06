--#DECLARE#--

/*

Lấy all quyet toan chi phi theo id quyet toan

*/

select qtcp.iID_QuyetToan_ChiPhiID,
			qtcp.iID_QuyetToanID,
			qtcp.iID_ChiPhiID,
			qtcp.fTienPheDuyet,
			dmcp.sTenChiPhi
from VDT_QT_QuyetToan_ChiPhi qtcp
left join VDT_DM_ChiPhi dmcp on qtcp.iID_ChiPhiID = dmcp.iID_ChiPhi
where qtcp.iID_QuyetToanID = @iID_QuyetToanID