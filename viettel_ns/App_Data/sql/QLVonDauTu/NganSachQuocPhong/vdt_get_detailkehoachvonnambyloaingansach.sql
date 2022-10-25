
--#DECLARE#--

/*

Lấy danh sách dữ liệu hiển thị lên gridview Kế hoạch vốn năm

*/

SELECT DISTINCT ml.sXauNoiMa, da.sMaKetNoi, da.iID_DuAnID, da.sTenDuAn, da.iID_LoaiCongTrinhID, lct.iID_LoaiCongTrinh,
				tbl.iID_NguonVonID, tbl.iID_LoaiNguonVonID, tbl.iID_NhomQuanLyID,dt.iID_NganhID INTO #tmp
FROM VDT_KHV_KeHoachNam as tbl
INNER JOIN VDT_KHV_KeHoachNam_ChiTiet as dt on tbl.iID_KeHoachNamID = dt.iID_KeHoachNamID
INNER JOIN VDT_DA_DuAn as da on dt.iID_DuAnID = da.iID_DuAnID AND da.bIsKetThuc = 0
INNER JOIN VDT_DM_LoaiCongTrinh as lct on da.iID_LoaiCongTrinhID = lct.iID_LoaiCongTrinh
INNER JOIN NS_MucLucNganSach as ml on dt.iID_NganhID = ml.iID_MaMucLucNganSach
WHERE tbl.iNamKeHoach < @iNamKeHoach AND (@dNgayLap IS NULL OR  tbl.dNgayQuyetDinh < @dNgayLap) AND tbl.iID_DonViQuanLyID = @iDonViQuanLy AND tbl.iID_NguonVonID = @iNguonVon
	AND tbl.iID_LoaiNganSachID = @iLoaiNganSach 



-- Von da bo tri
SELECT tbl.iID_DonViQuanLyID, tbl.iID_NguonVonID, tbl.iID_LoaiNguonVonID, tbl.iNamKeHoach, tbl.dNgayQuyetDinh, tbl.iID_NhomQuanLyID, 
		dt.iID_DuAnID, dt.iID_NganhID,
		SUM(dt.fGiaTrPhanBo * dt.fTiGia * dt.fTiGiaDonVi) as fVonDaBoTri INTO #tblPhanBo
FROM VDT_KHV_PhanBoVon as tbl
INNER JOIN VDT_KHV_PhanBoVon_ChiTiet as dt on dt.iID_PhanBoVonID = tbl.iID_PhanBoVonID
GROUP BY tbl.iID_DonViQuanLyID, tbl.iID_NguonVonID, tbl.iID_LoaiNguonVonID, tbl.iNamKeHoach, tbl.dNgayQuyetDinh, tbl.iID_NhomQuanLyID, 
		dt.iID_DuAnID, dt.iID_NganhID

-- Gia tri dau tu
SELECT dt.iID_DuAnID, SUM(ISNULL(cp.fTiGiaDonVi, 1) * ISNULL(cp.fTiGia, 1) * cp.fTienPheDuyet) as fGiaTriDauTu INTO #tblGiaTriDauTu
FROM VDT_DA_QDDauTu as dt
INNER JOIN VDT_DA_QDDauTu_NguonVon as nv on dt.iID_QDDauTuID = nv.iID_QDDauTuID
INNER JOIN VDT_DA_QDDauTu_ChiPhi as cp on dt.iID_QDDauTuID = cp.iID_QDDauTuID
WHERE dt.dNgayQuyetDinh < = @dNgayLap AND nv.iID_NguonVonID = @iNguonVon
GROUP BY dt.iID_DuAnID

-- 
SELECT 1 as iParentId, ROW_NUMBER() OVER (ORDER BY tmp.sTenDuAn) as iId, tmp.*, pb.fVonDaBoTri, dt.fGiaTriDauTu
FROM #tmp as tmp
INNER JOIN #tblPhanBo as pb on tmp.iID_DuAnID = pb.iID_DuAnID
							AND tmp.iID_NguonVonID = pb.iID_NguonVonID
							AND tmp.iID_LoaiNguonVonID = pb.iID_LoaiNguonVonID
							AND tmp.iID_NhomQuanLyID = pb.iID_NhomQuanLyID
							AND tmp.iID_NganhID = pb.iID_NganhID
INNER JOIN #tblGiaTriDauTu as dt on tmp.iID_DuAnID = dt.iID_DuAnID
