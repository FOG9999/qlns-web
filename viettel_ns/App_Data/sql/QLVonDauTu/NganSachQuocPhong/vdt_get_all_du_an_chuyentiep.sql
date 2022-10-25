SELECT
	duan.iID_DuAnID AS IDDuAnID,
	duan.sMaDuAn as SMaDuAn,
	duan.sTenDuAn AS STenDuAn,
	duan.sDiaDiem AS SDiaDiem,
	CAST(duan.sKhoiCong AS int) AS IGiaiDoanTu,
	CAST(duan.sKetThuc AS int) AS IGiaiDoanDen,
	duan.fHanMucDauTu AS FHanMucDauTu,
	donvi.iID_DonVi AS IIdDonViId,
	donvi.iID_MaDonVi AS IIDMaDonVi,
	donvi.sTenDonVi AS STenDonVi,
	null AS IIDLoaiCongTrinhID,
	'' AS STenLoaiCongTrinh,
	null AS IIDNguonVonID,
	'' AS STenNguonVon,
	cast(0 as bit) as IsChecked
FROM VDT_DA_DuAn duan
LEFT JOIN VDT_DM_DonViThucHienDuAn donvi
	ON duan.iID_DonViThucHienDuAnID  = donvi.iID_DonVi
WHERE
	duan.sTrangThaiDuAn = 'THUC_HIEN'
	AND duan.bIsKetThuc IS NULL
	AND iID_MaDonViThucHienDuAnID = @iID_MaDonViThucHienDuAnID