--#DECLARE#--

/*

Lấy record VDT_DA_DuToan theo ID du an

*/

SELECT vdt_da.* , pcda.sTen as sTenCapPheDuyet, dv.sTen as sTenDonVi, lct.sTenLoaiCongTrinh, chudautu.sTen as sTenChuDauTu, htql.sTenHinhThucQuanLy
FROM VDT_DA_DuAn vdt_da
LEFT JOIN VDT_DM_PhanCapDuAn pcda on vdt_da.iID_CapPheDuyetID = pcda.iID_PhanCapID
LEFT JOIN NS_DonVi dv on vdt_da.iID_DonViQuanLyID = dv.iID_Ma
LEFT JOIN NS_DonVi chudautu on vdt_da.iID_ChuDauTuID = chudautu.iID_Ma
LEFT JOIN VDT_DM_LoaiCongTrinh lct on vdt_da.iID_LoaiCongTrinhID = lct.iID_LoaiCongTrinh
LEFT JOIN VDT_DM_HinhThucQuanLy htql on vdt_da.iID_HinhThucQuanLyID = htql.iID_HinhThucQuanLyID
WHERE iID_DuAnID = @iID_DuAnID



