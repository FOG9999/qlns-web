SELECT
tbl.iID_ChuTruongDauTu_NguonVonID,
tbl.fTienPheDuyetCTDT,
tbl.fTienPheDuyet,
tbl.fTienPheDuyet- ISNULL(tbl.fGiaTriDieuChinh,0) as fGiatriTruocDieuChinh,
tbl.iID_NguonVonID,
dt.sTen as sTenNguonVon
FROM VDT_DA_ChuTruongDauTu_NguonVon as tbl
INNER JOIN NS_NguonNganSach as dt on tbl.iID_NguonVonID = dt.iID_MaNguonNganSach
WHERE tbl.iID_ChuTruongDauTuID = @iID_ChuTruongDauTuID
