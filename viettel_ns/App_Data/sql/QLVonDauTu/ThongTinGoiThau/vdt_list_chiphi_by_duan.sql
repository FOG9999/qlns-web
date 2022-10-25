
--#DECLARE#--
/*

Lấy danh sách chi phí theo dự án

*/

select distinct cp.iID_ChiPhi,cp.sTenChiPhi
from  VDT_DA_QDDauTu dt 
inner join VDT_DA_QDDauTu_ChiPhi qdcp ON qdcp.iID_QDDauTuID = dt.iID_QDDauTuID
inner join VDT_DM_ChiPhi cp ON cp.iID_ChiPhi = qdcp.iID_ChiPhiID
where dt.iID_DuAnID = @iID_DuAnID AND dt.dNgayQuyetDinh <= @dNgayLap 