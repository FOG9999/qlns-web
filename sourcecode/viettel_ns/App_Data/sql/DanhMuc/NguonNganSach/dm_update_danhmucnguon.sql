BEGIN
IF(@bPublic = 1)
	BEGIN
		UPDATE DM_Nguon
		SET
			sMaCTMT = @sMaCTMT,
			sMaNguon = @sMaNguon,
			sLoai = @sLoai,
			sKhoan = @sKhoan,
			sNoiDung = @sNoiDung,
			iID_NguonCha = @iID_NguonCha,
			dNgaySua = GETDATE(),
			iSoLanSua = ISNULL(iSoLanSua,0) + 1,
			sID_MaNguoiDungSua = @sUserLogin
		WHERE iID_Nguon = @iId
	END
	ELSE
	BEGIN
		UPDATE DM_Nguon
		SET
			bPublic = 0,
			dNgaySua = GETDATE(),
			sID_MaNguoiDungSua = @sUserLogin
		WHERE iID_Nguon = @iId
	END
END
