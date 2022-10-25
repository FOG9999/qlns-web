
SELECT tbl.iID_PhanBoVonID, dt.iID_PhanBoVon_ChiTietID, da.sMaKetNoi, da.iID_DuAnID, da.sTenDuAn, da.iID_LoaiCongTrinhID, @iNguonVon as iID_NguonVonID,
		tbl.iID_LoaiNguonVonID, tbl.iID_NhomQuanLyID,  dt.iID_NganhID, da.sKhoiCong, da.sKetThuc, da.iID_CapPheDuyetID INTO #tmp

FROM VDT_KHV_PhanBoVon as tbl
INNER JOIN VDT_KHV_PhanBoVon_ChiTiet as dt on tbl.iID_PhanBoVonID = dt.iID_PhanBoVonID 
											AND tbl.iNamKeHoach < @iNamKeHoach 
											AND (@dNgayLap IS NULL OR  tbl.dNgayQuyetDinh < @dNgayLap) 
											AND tbl.iID_DonViQuanLyID = @iDonViQuanLy 
											AND tbl.iID_NguonVonID = @iNguonVon
											AND tbl.iID_LoaiNguonVonID = @iLoaiNguonVon 
RIGHT JOIN VDT_DA_DuAn as da on dt.iID_DuAnID = da.iID_DuAnID AND ISNULL(da.bIsKetThuc,0) = 0
WHERE da.iID_DuAnID = @iIdDuAn


-- Von da bo tri
SELECT dt.iID_DuAnID, SUM(ISNULL(dt.fGiaTrPhanBo,1) * ISNULL(dt.fTiGia,1) * ISNULL(dt.fTiGiaDonVi,1)) as fVonDaBoTri INTO #tmpPhanBo
FROM VDT_KHV_PhanBoVon as tbl
INNER JOIN VDT_KHV_PhanBoVon_ChiTiet as dt on dt.iID_PhanBoVonID = tbl.iID_PhanBoVonID AND iID_NganhID = @iNganh
WHERE tbl.iNamKeHoach < @iNamKeHoach AND tbl.dNgayQuyetDinh < @dNgayLap
GROUP BY dt.iID_DuAnID

SELECT tmp .iID_DuAnID , (CASE WHEN tmp.fVonDaBoTri = 0 THEN ISNULL(da.fTongMucDauTu, 0) ELSE ISNULL(tmp.fVonDaBoTri,0) END ) as fVonDaBoTri INTO #tblPhanBo
FROM #tmpPhanBo as tmp
INNER JOIN VDT_DA_DuAn as da on tmp.iID_DuAnID = da.iID_DuAnID

-- Gia tri dau tu
SELECT dt.iID_DuAnID, SUM(ISNULL(nv.fTiGiaDonVi, 1) * ISNULL(nv.fTiGia, 1) * ISNULL(nv.fTienPheDuyet,0)) as fGiaTriDauTu INTO #tblGiaTriDauTu
FROM VDT_DA_QDDauTu as dt
INNER JOIN VDT_DA_QDDauTu_NguonVon as nv on dt.iID_QDDauTuID = nv.iID_QDDauTuID
WHERE dt.dNgayQuyetDinh < = @dNgayLap AND nv.iID_NguonVonID = @iNguonVon
GROUP BY dt.iID_DuAnID

-- Chi tieu dau nam
SELECT dt.iID_NganhID, SUM(ISNULL(dt.fGiaTrPhanBo,1) * ISNULL(dt.fTiGia,1)* ISNULL(dt.fTiGiaDonVi,1)) as fChiTieuDauNam INTO #tblChiTieuDauNam
FROM VDT_KHV_PhanBoVon as tbl
INNER JOIN VDT_KHV_PhanBoVon_ChiTiet as dt on tbl.iID_PhanBoVonID = dt.iID_PhanBoVonID
WHERE tbl.iID_ParentId IS NULL AND tbl.iNamKeHoach = @iNamKeHoach AND tbl.iID_DonViQuanLyID = @iDonViQuanLy 
	AND tbl.iID_LoaiNguonVonID = @iLoaiNguonVon AND tbl.dNgayQuyetDinh < @dNgayLap
GROUP BY dt.iID_NganhID


-- 
SELECT 1 as iParentId, tmp.iID_PhanBoVon_ChiTietID as iId, tmp.*, ISNULL(pb.fVonDaBoTri,0) as fVonDaBoTri, ISNULL(dtda.fGiaTriDauTu,0) as fGiaTriDauTu, ISNULL(ct.fChiTieuDauNam, 0) as fChiTieuDauNam
FROM #tmp as tmp
LEFT JOIN VDT_KHV_PhanBoVon_ChiTiet as dt on tmp.iID_PhanBoVon_ChiTietID = dt.iID_PhanBoVon_ChiTietID
LEFT JOIN #tblPhanBo as pb on tmp.iID_DuAnID = pb.iID_DuAnID
LEFT JOIN #tblGiaTriDauTu as dtda on tmp.iID_DuAnID = dtda.iID_DuAnID
LEFT JOIN #tblChiTieuDauNam as ct on ct.iID_NganhID = dt.iID_NganhID

DROP TABLE #tmp
DROP TABLE #tmpPhanBo
DROP TABLE #tblPhanBo
DROP TABLE #tblGiaTriDauTu