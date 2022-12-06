
--#DECLARE#--
/*

Lấy danh sách hạng mục- màn tạo mới phê duyệt dự án

*/


select 
hm.iID_ChuTruongDauTu_DM_HangMucID as iID_QDDauTu_DM_HangMucID,
hm.iID_DuAnID,
hm.iID_ParentID,
hm.sMaHangMuc,
hm.sTenHangMuc,
lct.sTenLoaiCongTrinh,
hm.smaOrder,
hm.iID_LoaiCongTrinhID


from VDT_DA_ChuTruongDauTu_DM_HangMuc hm
inner join VDT_DA_ChuTruongDauTu_HangMuc cthm ON cthm.iID_HangMucID = hm.iID_ChuTruongDauTu_DM_HangMucID
left join VDT_DM_LoaiCongTrinh lct ON lct.iID_LoaiCongTrinh = hm.iID_LoaiCongTrinhID
where cthm.iID_ChuTruongDauTuID = @iID_ChuTruongDauTuID

order by smaOrder