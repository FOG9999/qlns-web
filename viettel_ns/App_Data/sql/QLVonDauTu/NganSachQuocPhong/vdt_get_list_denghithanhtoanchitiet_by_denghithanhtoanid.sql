
SELECT iID_DeNghiThanhToanID, iID_DeNghiThanhToan_ChiTietID INTO #tmp
FROM VDT_TT_DeNghiThanhToan_ChiTiet 
WHERE iID_DeNghiThanhToanID = @iID_DeNghiThanhToanID

SELECT tmp.iID_DeNghiThanhToanID, tmp.iID_DeNghiThanhToan_ChiTietID, SUM(ISNULL(gtdt.fTienTrungThau,0)) as fDuToanGiaGoiThau INTO #tmpDuToan
		FROM #tmp as tmp
		INNER JOIN VDT_TT_DeNghiThanhToan as tbl on tmp.iID_DeNghiThanhToanID = tbl.iID_DeNghiThanhToanID
		INNER JOIN VDT_TT_DeNghiThanhToan_ChiTiet as dt on tmp.iID_DeNghiThanhToan_ChiTietID = dt.iID_DeNghiThanhToan_ChiTietID
		INNER JOIN VDT_DA_TT_HopDong as hd on dt.iID_HopDongID = hd.iID_HopDongID
		INNER JOIN VDT_DA_GoiThau as gt on hd.iID_GoiThauID = gt.iID_GoiThauID AND gt.dNgayLap <= tbl.dNgayDeNghi
		INNER JOIN VDT_DA_GoiThau as gtdt on gt.iID_GoiThauGocID = gtdt.iID_GoiThauGocID AND gtdt.dNgayLap <= tbl.dNgayDeNghi
GROUP BY tmp.iID_DeNghiThanhToanID, tmp.iID_DeNghiThanhToan_ChiTietID

SELECT tmp.iID_DeNghiThanhToanID, SUM(ISNULL(dt.fGiaTriThanhToan,0)) as fLuyKeThanhToan INTO #tmpLuyKeThanhToan
FROM #tmp as tmp
INNER JOIN VDT_TT_DeNghiThanhToan as tbl on tmp.iID_DeNghiThanhToanID = tbl.iID_DeNghiThanhToanID
INNER JOIN VDT_TT_DeNghiThanhToan_ChiTiet as dt on dt.iID_DeNghiThanhToanID = tmp.iID_DeNghiThanhToanID
GROUP BY tmp.iID_DeNghiThanhToanID


SELECT dt.*, da.sTenDuAn, nganh.sXauNoiMa as sNganh, hd.sSoHopDong, nt.sTenNhaThau, nt.sSoTaiKhoan as sTaiKhoanNganHang
FROM #tmp as tmp
INNER JOIN VDT_TT_DeNghiThanhToan as tbl on tmp.iID_DeNghiThanhToanID = tbl.iID_DeNghiThanhToanID
INNER JOIN VDT_TT_DeNghiThanhToan_ChiTiet as dt on tmp.iID_DeNghiThanhToan_ChiTietID = dt.iID_DeNghiThanhToan_ChiTietID
LEFT JOIN NS_MucLucNganSach as nganh on dt.iID_NganhID = nganh.iID_MaMucLucNganSach
LEFT JOIN VDT_DA_DuAn as da on dt.iID_DuAnID = da.iID_DuAnID
LEFT JOIN VDT_DA_TT_HopDong as hd on dt.iID_HopDongID = hd.iID_HopDongID
LEFT JOIN VDT_DM_NhaThau as nt on hd.iID_NhaThauThucHienID = nt.iID_NhaThauID
LEFT JOIN  #tmpDuToan as dutoan on dt.iID_DeNghiThanhToan_ChiTietID = dutoan.iID_DeNghiThanhToan_ChiTietID
LEFT JOIN #tmpLuyKeThanhToan as luyke on luyke.iID_DeNghiThanhToanID = dt.iID_DeNghiThanhToanID

DROP TABLE #tmp
DROP TABLE #tmpDuToan 
DROP TABLE #tmpLuyKeThanhToan