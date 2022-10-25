
--#DECLARE#--
/*

Lấy danh sách hạng mục- màn tạo mới phê duyệt dự án

*/


select 
hm.Id as iID_DuToan_DM_HangMucID,
hm.iID_DuAnID,
hm.iID_ParentID,
hm.sMaHangMuc,
hm.sTenHangMuc,
lct.sTenLoaiCongTrinh,
hm.maOrder as smaOrder,
hm.iID_LoaiCongTrinhID as iID_LoaiCongTrinhID,
dthm.iID_DuToan_HangMuciID,
dthm.iID_DuAn_ChiPhi,
dthm.fTienPheDuyet,
dthm.iID_HangMucID,
dthm.fTienPheDuyetQDDT
from VDT_DA_DuToan_DM_HangMuc hm
inner join VDT_DA_DuToan_HangMuc dthm ON dthm.iID_HangMucID = hm.Id
left join VDT_DM_LoaiCongTrinh lct ON lct.iID_LoaiCongTrinh = hm.iID_LoaiCongTrinhID
where dthm.iID_DuToanID = @duToanId

order by maOrder