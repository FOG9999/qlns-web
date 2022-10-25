

---- chi tieu ngan sach nam 
SELECT iID_DuAnID, iID_NganhID, ISNULL(SUM(ISNULL(dt.fGiaTrPhanBo,0)) ,0) as fChiTieuNganSachNam INTO #tblChiTieuNganSach
FROM VDT_KHV_PhanBoVon as tbl
INNER JOIN VDT_KHV_PhanBoVon_ChiTiet as dt on tbl.iID_PhanBoVonID = dt.iID_PhanBoVonID
WHERE tbl.iNamKeHoach = @iNamKeHoach
	AND tbl.dNgayQuyetDinh <= @dNgayQuyetDinh
	AND tbl.iID_DonViQuanLyID = @iIdDonViQuanLyId
	AND tbl.iID_NguonVonID = @iIdNguonVonId
GROUP BY iID_DuAnID, iID_NganhID

---- Cap phat von nam nay
SELECT iID_DuAnID, iID_NganhID, ISNULL(SUM(ISNULL(dt.fGiaTriThanhToan,0)),0) as fCapPhatVonNamNay INTO #tmpCapPhatVonNamNay
FROM VDT_TT_DeNghiThanhToan as tbl
INNER JOIN VDT_TT_DeNghiThanhToan_ChiTiet  as dt on tbl.iID_DeNghiThanhToanID = dt.iID_DeNghiThanhToanID
WHERE tbl.iNamKeHoach = @iNamKeHoach
	AND tbl.dNgayDeNghi <= @dNgayQuyetDinh
	AND tbl.iID_DonViQuanLyID = @iIdDonViQuanLyId
	AND tbl.iID_NguonVonID = @iIdNguonVonId
	AND tbl.iID_LoaiNguonVonID = @iIdLoaiNguonVon
GROUP BY iID_DuAnID, iID_NganhID

SELECT iID_DuAnID, ISNULL(SUM(ISNULL(fTongMucDauTuPheDuyet,0)),0) AS fTongMucDauTuPheDuyet INTO #tmpTongMucPheDuyet
FROM
(SELECT iID_DuAnID, SUM(fTongMucDauTuPheDuyet * ISNULL(fTiGia,1) * ISNULL(fTiGiaDonVi,1)) as fTongMucDauTuPheDuyet
FROM VDT_DA_QDDauTu
WHERE dNgayQuyetDinh <= @dNgayLap
	AND ISNULL(fTongMucDauTuPheDuyet,0) <> 0
GROUP BY iID_DuAnID
UNION
SELECT iID_DuAnID, SUM(ISNULL(dt.fTienPheDuyet,0) * ISNULL(dt.fTiGia,1) * ISNULL(dt.fTiGiaDonVi,1) ) as fTongMucDauTuPheDuyet
FROM VDT_DA_QDDauTu as tbl
INNER JOIN VDT_DA_QDDauTu_NguonVon as dt on tbl.iID_QDDauTuID = dt.iID_QDDauTuID
WHERE ISNULL(tbl.fTongMucDauTuPheDuyet,0) = 0 
	AND tbl.dNgayQuyetDinh <= @dNgayLap
	AND dt.iID_NguonVonID = @iIdNguonVonId
GROUP BY iID_DuAnID
) as tbl
GROUP BY iID_DuAnID

SELECT tbl.*, da.sTenDuAn, CONCAT(ml.sM, '-', ml.sTM, '-', ml.sTTM, '-', ml.sNG) as sXauNoiChuoi,
		ISNULL(ctns.fChiTieuNganSachNam,0) as fChiTieuNganSachNam, 
		ISNULL(cpv.fCapPhatVonNamNay, 0) as fCapPhatVonNamNay,
		ISNULL(pd.fTongMucDauTuPheDuyet, 0) as fTongMucDauTuPheDuyet
FROM VDT_QT_DeNghiQuyetToanNienDo_TroLy_ChiTiet as tbl
LEFT JOIN VDT_DA_DuAn as da on tbl.iID_DuAnID = da.iID_DuAnID
LEFT JOIN NS_MucLucNganSach as ml on tbl.iID_NganhID = ml.iID_MaMucLucNganSach
LEFT JOIN #tblChiTieuNganSach as ctns on tbl.iID_DuAnID = ctns.iID_DuAnID AND tbl.iID_NganhID = ctns.iID_NganhID
LEFT JOIN #tmpCapPhatVonNamNay as cpv on tbl.iID_DuAnID = cpv.iID_DuAnID AND tbl.iID_NganhID = cpv.iID_NganhID
LEFT JOIN #tmpTongMucPheDuyet as pd on tbl.iID_DuAnID = pd.iID_DuAnID
WHERE tbl.iID_DeNghiQuyetToanNienDoID = @iID

DROP TABLE #tblChiTieuNganSach
DROP TABLE #tmpCapPhatVonNamNay


