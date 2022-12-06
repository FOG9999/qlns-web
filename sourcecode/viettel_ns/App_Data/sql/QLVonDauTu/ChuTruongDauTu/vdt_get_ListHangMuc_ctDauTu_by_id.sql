SELECT  
hm.iID_ChuTruongDauTu_DM_HangMucID as iID_ChuTruongDauTu_DM_HangMucID,
hm.iID_ChuTruongDauTu_DM_HangMucID as iID_DuAn_HangMucID,
hm.iID_DuAnID,
hm.iID_ParentID,
hm.sMaHangMuc,
hm.sTenHangMuc,
lct.sTenLoaiCongTrinh,
hm.smaOrder,
hm.iID_LoaiCongTrinhID,
cthm.iID_ChuTruongDauTu_HangMucID

FROM VDT_DA_ChuTruongDauTu_HangMuc cthm
INNER JOIN VDT_DA_ChuTruongDauTu_DM_HangMuc hm on cthm.iID_HangMucID = hm.iID_ChuTruongDauTu_DM_HangMucID
LEFT JOIN VDT_DM_LoaiCongTrinh lct on lct.iID_LoaiCongTrinh = hm.iID_LoaiCongTrinhID
WHERE cthm.iID_ChuTruongDauTuID = @iID_ChuTruongDauTuID
order by hm.smaOrder