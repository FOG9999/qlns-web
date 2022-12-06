select 
hm.iID_QDDauTu_DM_HangMucID as iID_DuToan_DM_HangMucID,
hm.iID_DuAnID,
hm.iID_ParentID,
hm.sMaHangMuc,
hm.sTenHangMuc,
lct.sTenLoaiCongTrinh,
hm.smaOrder as smaOrder,
hm.iID_LoaiCongTrinhID as iID_LoaiCongTrinhID,
dthm.iID_QDDauTu_HangMuciID as iID_DuToan_HangMuciID,
dthm.iID_DuAn_ChiPhi,
dthm.fTienPheDuyet,
dthm.iID_HangMucID,
dthm.fTienPheDuyet as fTienPheDuyetQDDT
from VDT_DA_QDDauTu_DM_HangMuc hm
inner join VDT_DA_QDDauTu_HangMuc dthm ON dthm.iID_HangMucID = hm.iID_QDDauTu_DM_HangMucID
left join VDT_DM_LoaiCongTrinh lct ON lct.iID_LoaiCongTrinh = hm.iID_LoaiCongTrinhID
where dthm.iID_QDDauTuID = @duToanId

order by smaOrder