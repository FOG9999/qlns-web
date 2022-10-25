SELECT dt.iID_DuAn_ChiPhi, cp.sTenChiPhi, cp.iID_ChiPhi as iID_ChiPhiID, cp.iID_ChiPhi_Parent, CAST(0 as float) as fTienPheDuyet,
		CAST(0 as float) as fGiaTriDieuChinh, dt.fTienPheDuyet as fTienPheDuyetQDDT, cp.iThuTu as iThuTu, CAST(0 as bit) as isDelete
FROM VDT_DA_QDDauTu as tbl
INNER JOIN VDT_DA_QDDauTu_ChiPhi as dt on tbl.iID_QDDauTuID = dt.iID_QDDauTuID
INNER JOIN VDT_DM_DuAn_ChiPhi as cp on dt.iID_DuAn_ChiPhi = cp.iID_DuAn_ChiPhi
WHERE  tbl.bActive = 1 AND tbl.iID_DuAnID = @iIdDuAnId
order by iThuTu, cp.iID_ChiPhi_Parent, cp.sTenChiPhi