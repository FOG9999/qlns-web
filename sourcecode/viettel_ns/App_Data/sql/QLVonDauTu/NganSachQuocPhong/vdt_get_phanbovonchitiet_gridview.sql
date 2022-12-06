
DECLARE @dNgayLap date
DECLARE @iNguonVon int
DECLARE @iNamKeHoach int

SELECT @dNgayLap= dNgayQuyetDinh, @iNguonVon = iID_NguonVonID, @iNamKeHoach = iNamKeHoach
FROM VDT_KHV_PhanBoVon 
WHERE iID_PhanBoVonID = @iID_PhanBoVon

-- Von da bo tri
SELECT dt.iID_DuAnID, dt.iID_NganhID, SUM(ISNULL(dt.fGiaTrPhanBo,1) * ISNULL(dt.fTiGia,1) * ISNULL(dt.fTiGiaDonVi,1)) as fVonDaBoTri INTO #tmpPhanBo
FROM VDT_KHV_PhanBoVon as tbl
INNER JOIN VDT_KHV_PhanBoVon_ChiTiet as dt on dt.iID_PhanBoVonID = tbl.iID_PhanBoVonID
WHERE tbl.iNamKeHoach < @iNamKeHoach AND tbl.dNgayQuyetDinh < @dNgayLap
GROUP BY dt.iID_DuAnID, dt.iID_NganhID


SELECT tmp.iID_NganhID, tmp.iID_DuAnID , (CASE WHEN tmp.fVonDaBoTri = 0 THEN ISNULL(da.fTongMucDauTu, 0) ELSE ISNULL(tmp.fVonDaBoTri,0) END ) as fVonDaBoTri INTO #tblPhanBo
FROM #tmpPhanBo as tmp
INNER JOIN VDT_DA_DuAn as da on tmp.iID_DuAnID = da.iID_DuAnID

SELECT dt.iID_DuAnID, SUM(ISNULL(cp.fTiGiaDonVi, 1) * ISNULL(cp.fTiGia, 1) * cp.fTienPheDuyet) as fGiaTriDauTu INTO #tblGiaTriDauTu
FROM VDT_DA_QDDauTu as dt
INNER JOIN VDT_DA_QDDauTu_NguonVon as nv on dt.iID_QDDauTuID = nv.iID_QDDauTuID
INNER JOIN VDT_DA_QDDauTu_ChiPhi as cp on dt.iID_QDDauTuID = cp.iID_QDDauTuID
WHERE dt.dNgayQuyetDinh < = @dNgayLap AND nv.iID_NguonVonID = @iNguonVon
GROUP BY dt.iID_DuAnID

SELECT tbl.*, CONCAT(ng.sM, ' - ', ng.sTM, ' - ', ng.sTTM, ' - ', ng.sNG) as sXauNoiMa, 
		da.sMaKetNoi, 
		da.sTenDuAn, da.iID_LoaiCongTrinhID, lct.sTenLoaiCongTrinh, ISNULL(gtdt.fGiaTriDauTu, 0) as fGiaTriDauTu, ISNULL(vdbt.fVonDaBoTri, 0) fVonDaBoTri
FROM VDT_KHV_PhanBoVon_ChiTiet as tbl
INNER JOIN VDT_DA_DuAn as da on tbl.iID_DuAnID = da.iID_DuAnID
LEFT JOIN NS_MucLucNganSach as ng on tbl.iID_NganhID = ng.iID_MaMucLucNganSach
LEFT JOIN VDT_DM_LoaiCongTrinh as lct on da.iID_LoaiCongTrinhID = lct.iID_LoaiCongTrinh
LEFT JOIN #tblGiaTriDauTu as gtdt on tbl.iID_DuAnID = gtdt.iID_DuAnID
LEFT JOIN #tblPhanBo as vdbt on tbl.iID_NganhID = vdbt.iID_NganhID AND tbl.iID_DuAnID = vdbt.iID_DuAnID
WHERE tbl.iID_PhanBoVonID = @iID_PhanBoVon
