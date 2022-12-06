select * FROM VDT_QT_DeNghiQuyetToan AS vdt_qt_denghiquyettoan
	LEFT JOIN VDT_DA_DuAn AS vdt_da_duan ON vdt_da_duan.iID_DuAnID = vdt_qt_denghiquyettoan.iID_DuAnID
	INNER JOIN NS_DonVi as donVi on vdt_da_duan.iID_DonViQuanLyID = donVi.iID_Ma
	--INNER JOIN NS_PhongBan_DonVi AS ns_phongban_donvi on ns_phongban_donvi.iID_MaDonVi = donVi.iID_MaDonVi AND ns_phongban_donvi.sID_MaNguoiDung_DuocGiao = @sUserName
WHERE donvi.iNamLamViec_DonVi = @iNamLamViec 
--AND ns_phongban_donvi.iNamLamViec = @iNamLamViec