
--#DECLARE#--
/*

Lấy danh sách thông tin gói thầu

*/

select sum(fTienPheDuyet) as tongMucDauTuChiPhi
from VDT_DA_QDDauTu_ChiPhi qdcp
left join VDT_DA_QDDauTu dt ON dt.iID_QDDauTuID = qdcp.iID_QDDauTuID
where qdcp.iID_ChiPhiID = @iID
AND dt.iID_DuAnID = @iID_DuAnID
AND dt.dNgayQuyetDinh <= @dNgayLap