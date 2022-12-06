SELECT
	dm_nv.iID_MaNguonNganSach AS iID_NguonVonID,
	dm_nv.sTen AS sTenNguonVon,
	(
	SELECT SUM
		( fTienPheDuyet ) 
	FROM
		VDT_DA_QDDauTu_NguonVon 
	WHERE
		iID_NguonVonID = dm_nv.iID_MaNguonNganSach 
		AND iID_QDDauTuID IN ( SELECT iID_QDDauTuID FROM VDT_DA_QDDauTu WHERE ( iID_QDDauTuID =@iID_QDDauTuID OR iID_ParentID =@iID_QDDauTuID ) AND @dNgayPheDuyet >= dNgayQuyetDinh ) 
	) AS fTruocDieuChinh,
	0 as fGiaTriDieuChinh
FROM
	NS_NguonNganSach dm_nv 
WHERE
	dm_nv.iID_MaNguonNganSach IN (
	SELECT iID_NguonVonID 
	FROM VDT_DA_QDDauTu_NguonVon 
	WHERE iID_QDDauTuID IN ( SELECT iID_QDDauTuID FROM VDT_DA_QDDauTu WHERE ( iID_QDDauTuID =@iID_QDDauTuID OR iID_ParentID =@iID_QDDauTuID ) AND @dNgayPheDuyet >= dNgayQuyetDinh ) 
	) 
ORDER BY
	dm_nv.iSTT;