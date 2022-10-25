SELECT
	dm_cp.iID_ChiPhi AS iID_ChiPhiID,
	dm_cp.sMaChiPhi,
	dm_cp.sTenChiPhi,
	(
	SELECT SUM
		( fTienPheDuyet ) 
	FROM
		VDT_DA_QDDauTu_ChiPhi 
	WHERE
		iID_ChiPhiID = dm_cp.iID_ChiPhi 
		AND iID_QDDauTuID IN ( SELECT iID_QDDauTuID FROM VDT_DA_QDDauTu WHERE ( iID_QDDauTuID =@iID_QDDauTuID OR iID_ParentID =@iID_QDDauTuID ) AND @dNgayPheDuyet >= dNgayQuyetDinh ) 
	) AS fTruocDieuChinh,
	0 as fGiaTriDieuChinh
FROM
	VDT_DM_ChiPhi dm_cp 
WHERE
	dm_cp.iID_ChiPhi IN (
	SELECT iID_ChiPhiID 
	FROM VDT_DA_QDDauTu_ChiPhi 
	WHERE iID_QDDauTuID IN ( SELECT iID_QDDauTuID FROM VDT_DA_QDDauTu WHERE ( iID_QDDauTuID =@iID_QDDauTuID OR iID_ParentID =@iID_QDDauTuID ) AND @dNgayPheDuyet >= dNgayQuyetDinh ) 
	) 
ORDER BY
	dm_cp.iThuTu;