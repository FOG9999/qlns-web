
--#DECLARE#--
/*

Lấy danh sách chi phí theo dự án

*/
select  distinct hm.iID_DuAn_HangMucID,hm.sTenHangMuc
from  VDT_DA_QDDauTu dt 
inner join VDT_DA_QDDauTu_HangMuc dthm ON dthm.iID_QDDauTuID = dt.iID_QDDauTuID
inner join VDT_DA_DuAn_HangMuc hm ON hm.iID_DuAn_HangMucID = dthm.iID_HangMucID
where dt.iID_DuAnID = @iID_DuAnID AND dt.dNgayQuyetDinh <= @dNgayLap