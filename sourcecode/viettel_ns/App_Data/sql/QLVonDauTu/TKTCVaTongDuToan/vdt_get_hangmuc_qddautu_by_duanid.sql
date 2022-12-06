SELECT 
	dm.iID_QDDauTu_DM_HangMucID as iID_HangMucID,
	dm.iID_ParentID,
	dm.sTenHangMuc,
	ct.sTenLoaiCongTrinh,
	dm.smaOrder,
	dm.iID_LoaiCongTrinhID,
	hm.iID_DuAn_ChiPhi,
	hm.fTienPheDuyet as fTienPheDuyetQDDT
FROM VDT_DA_QDDauTu as qd
INNER JOIN VDT_DA_QDDauTu_HangMuc as hm on qd.iID_QDDauTuID = hm.iID_QDDauTuID
INNER JOIN VDT_DA_QDDauTu_DM_HangMuc as dm on hm.iID_HangMucID = dm.iID_QDDauTu_DM_HangMucID
LEFT JOIN VDT_DM_LoaiCongTrinh as ct on dm.iID_LoaiCongTrinhID = ct.iID_LoaiCongTrinh
WHERE qd.iID_DuAnID = @iIdDuAnId and qd.bActive = 1