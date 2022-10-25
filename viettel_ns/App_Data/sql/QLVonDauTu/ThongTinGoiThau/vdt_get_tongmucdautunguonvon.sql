
--#DECLARE#--
/*

Lấy danh sách thông tin gói thầu

*/

select sum(fTienPheDuyet) as tongMucDauTuChiPhi
from VDT_DA_QDDauTu_NguonVon nv
left join VDT_DA_QDDauTu dt ON dt.iID_QDDauTuID = nv.iID_QDDauTuID
where nv.iID_NguonVonID = @iID
AND dt.iID_DuAnID = @iID_DuAnID
AND dt.dNgayQuyetDinh <= @dNgayLap