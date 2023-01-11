SELECT
	duan.iID_DuAnID AS IDDuAnID,
	duan.sMaDuAn as SMaDuAn,
	duan.sTenDuAn AS STenDuAn,
	duan.sDiaDiem AS SDiaDiem,
	CAST(duan.sKhoiCong AS int) AS IGiaiDoanTu,
	CAST(duan.sKetThuc AS int) AS IGiaiDoanDen,
	duan.fHanMucDauTu AS FHanMucDauTu,
	donvi.iID_Ma AS IIdDonViId,
	donvi.iID_MaDonVi AS IIDMaDonVi,
	donvi.sTen AS STenDonVi,
	null AS IIDLoaiCongTrinhID,
	'' AS STenLoaiCongTrinh,
	null AS IIDNguonVonID,
	'' AS STenNguonVon,
	ctdt.sSoQuyetDinh as SSoQuyetDinh,
	cast(0 as bit) as IsChecked
FROM VDT_DA_DuAn duan
JOIN VDT_DA_ChuTruongDauTu ctdt 
	ON duan.iID_DuAnID = ctdt.iID_DuAnID
LEFT JOIN NS_DonVi donvi
	ON duan.iID_DonViQuanLyID  = donvi.iID_Ma

WHERE
	duan.sTrangThaiDuAn = 'KhoiTao'	
	AND duan.iID_DonViQuanLyID = @idDonVi