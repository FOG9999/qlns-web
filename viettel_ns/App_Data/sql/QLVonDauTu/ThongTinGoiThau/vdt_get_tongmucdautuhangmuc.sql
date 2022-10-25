
--#DECLARE#--
/*

Lấy danh sách thông tin gói thầu

*/

select sum(fTienPheDuyet) as tongMucDauTuChiPhi
from VDT_DA_QDDauTu_HangMuc hm
left join VDT_DA_QDDauTu dt ON dt.iID_QDDauTuID = hm.iID_QDDauTuID
where hm.iID_HangMucID = @iID
AND dt.iID_DuAnID = @iID_DuAnID
AND dt.dNgayQuyetDinh <= @dNgayLap