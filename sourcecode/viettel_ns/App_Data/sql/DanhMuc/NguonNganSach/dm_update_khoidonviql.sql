BEGIN
	IF(@bPublic = 1)
	BEGIN
		UPDATE DM_KhoiDonViQuanLy
		SET
			sMaKhoi = @sMaKhoi,
			sTenKhoi = @sTenKhoi,
			sGhiChu = @sNote,
			iSoLanSua = ISNULL(iSoLanSua,0) + 1,
			dNgaySua = GETDATE(),
			sID_MaNguoiDungSua = @sUserLogin
		WHERE iID_Khoi = @iId
	END
	ELSE
	BEGIN
		UPDATE DM_KhoiDonViQuanLy
		SET
			bPublic = 0,
			dNgaySua = GETDATE(),
			sID_MaNguoiDungSua = @sUserLogin
		WHERE iID_Khoi = @iId
	END
END