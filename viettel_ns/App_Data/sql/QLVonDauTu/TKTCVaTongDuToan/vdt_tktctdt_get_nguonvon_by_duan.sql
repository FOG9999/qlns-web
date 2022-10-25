SELECT qdnv.iID_NguonVonID, ns.sTen as sTenNguonVon, CAST(0 as float) as fTienPheDuyet, CAST(0 as float) as fGiaTriDieuChinh, qdnv.fTienPheDuyet as fTienPheDuyetQDDT
FROM VDT_DA_QDDauTu_NguonVon as qdnv
INNER JOIN VDT_DA_QDDauTu as qd on qd.iID_QDDauTuID = qdnv.iID_QDDauTuID
INNER JOIN NS_NguonNganSach as ns on qdnv.iID_NguonVonID = ns.iID_MaNguonNganSach
WHERE  qd.iID_DuAnID = @iIdDuAnId
and qd.bActive = 1


