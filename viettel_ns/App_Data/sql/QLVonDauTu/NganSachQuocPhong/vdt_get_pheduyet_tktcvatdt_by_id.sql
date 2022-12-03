
SELECT tktc.*,
da.sMaDuAn,
da.sTenDuAn,
da.sDiaDiem,
da.sKhoiCong,
da.sKetThuc,
da.fTongMucDauTu,
dv.sTen AS sTenDonViQL,
dv.iID_MaDonVi as sMaDonViQuanLy
FROM VDT_DA_DuToan tktc
LEFT JOIN VDT_DA_DuAn da ON tktc.iID_DuAnID = da.iID_DuAnID
LEFT JOIN NS_DonVi dv ON da.iID_DonViQuanLyID = dv.iID_Ma
WHERE tktc.iID_DuToanID = @iID_DuToanID