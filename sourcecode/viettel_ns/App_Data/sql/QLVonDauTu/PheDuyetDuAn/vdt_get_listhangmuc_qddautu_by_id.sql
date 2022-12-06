
--#DECLARE#--
/*

Lấy danh sách hạng mục- màn tạo mới phê duyệt dự án

*/


select 
hm.iID_QDDauTu_DM_HangMucID as iID_QDDauTu_DM_HangMucID,
hm.iID_DuAnID,
hm.iID_ParentID,
hm.sMaHangMuc,
hm.sTenHangMuc,
lct.sTenLoaiCongTrinh,
hm.smaOrder,
hm.iID_LoaiCongTrinhID,
qdhm.iID_QDDauTu_HangMuciID,
qdhm.iID_DuAn_ChiPhi,
qdhm.fTienPheDuyet,
qdhm.iID_HangMucID,
qdhm.fTienPheDuyet - isnull(qdhm.fGiaTriDieuChinh,0) as fGiaTriTruocDieuChinh

from VDT_DA_QDDauTu_DM_HangMuc hm
inner join VDT_DA_QDDauTu_HangMuc qdhm ON qdhm.iID_HangMucID = hm.iID_QDDauTu_DM_HangMucID
left join VDT_DM_LoaiCongTrinh lct ON lct.iID_LoaiCongTrinh = hm.iID_LoaiCongTrinhID
where qdhm.iID_QDDauTuID = @qdDauTuId

order by smaOrder