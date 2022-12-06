IF(@loaiChungTu = 1)
BEGIN
	SELECT tktc.*,
		   da.sMaDuAn,
		   da.sTenDuAn,
		   da.sDiaDiem,
		   da.sKhoiCong,
		   da.sKetThuc,
		   da.fTongMucDauTu,
		   dv.sTenDonVi AS sTenDonViQL,
		   dv.iID_MaDonVi as sMaDonViQuanLy
	FROM VDT_DA_DuToan tktc
	LEFT JOIN VDT_DA_DuAn da ON tktc.iID_DuAnID = da.iID_DuAnID
	LEFT JOIN VDT_DM_DonViThucHienDuAn dv ON da.iID_DonViThucHienDuAnID = dv.iID_DonVi 
	WHERE tktc.iID_DuToanID = @iID_DuToanID
END
ELSE IF (@loaiChungTu = 2)
BEGIN
	SELECT tktc.*,
		   da.sMaDuAn,
		   da.sTenDuAn,
		   da.sDiaDiem,
		   da.sKhoiCong,
		   da.sKetThuc,
		   da.fTongMucDauTu,
		   dv.sTenDonVi AS sTenDonViQL,
		   dv.iID_MaDonVi as sMaDonViQuanLy
	FROM VDT_DA_QDDauTu tktc
	LEFT JOIN VDT_DA_DuAn da ON tktc.iID_DuAnID = da.iID_DuAnID
	LEFT JOIN VDT_DM_DonViThucHienDuAn dv ON da.iID_DonViThucHienDuAnID = dv.iID_DonVi 
	WHERE tktc.iID_QDDauTuID = @iID_DuToanID
END
ELSE
	BEGIN
	SELECT tktc.*,
		   da.sMaDuAn,
		   da.sTenDuAn,
		   da.sDiaDiem,
		   da.sKhoiCong,
		   da.sKetThuc,
		   da.fTongMucDauTu,
		   dv.sTenDonVi AS sTenDonViQL,
		   dv.iID_MaDonVi as sMaDonViQuanLy
	FROM VDT_DA_ChuTruongDauTu tktc
	LEFT JOIN VDT_DA_DuAn da ON tktc.iID_DuAnID = da.iID_DuAnID
	LEFT JOIN VDT_DM_DonViThucHienDuAn dv ON da.iID_DonViThucHienDuAnID = dv.iID_DonVi 
	WHERE tktc.iID_ChuTruongDauTuID = @iID_DuToanID
END