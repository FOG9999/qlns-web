SELECT
	hm.iID_HangMucID,
	da_hm.sTenHangMuc,
	SUM ( hm.fTienPheDuyet ) AS fTruocDieuChinh,
	0 AS fGiaTriDieuChinh 
FROM
	VDT_DA_QDDauTu_HangMuc hm
	INNER JOIN VDT_DA_DuAn_HangMuc da_hm ON hm.iID_HangMucID = da_hm.iID_DuAn_HangMucID 
	INNER JOIN VDT_DA_QDDauTu qd_dt ON hm.iID_QDDauTuID = qd_dt.iID_QDDauTuID
WHERE
qd_dt.iID_QDDauTuID IN ( SELECT iID_QDDauTuID FROM VDT_DA_QDDauTu WHERE ( iID_QDDauTuID =@iID_QDDauTuID OR iID_ParentID =@iID_QDDauTuID ) AND @dNgayPheDuyet >= dNgayQuyetDinh ) 
GROUP BY
	hm.iID_HangMucID,
	da_hm.sTenHangMuc;