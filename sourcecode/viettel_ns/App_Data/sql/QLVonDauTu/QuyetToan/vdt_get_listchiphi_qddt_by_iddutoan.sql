
SELECT qddtcp.*, dmdacp.iID_ChiPhi, qddtcp.fTienPheDuyet
FROM VDT_DA_QDDauTu_ChiPhi qddtcp
JOIN VDT_DM_DuAn_ChiPhi dmdacp ON qddtcp.iID_DuAn_ChiPhi = dmdacp.iID_DuAn_ChiPhi
JOIN VDT_DA_DuToan dt ON dt.iID_QDDauTuID = qddtcp.iID_QDDauTuID
WHERE dt.iID_DuToanID = @duToanId
