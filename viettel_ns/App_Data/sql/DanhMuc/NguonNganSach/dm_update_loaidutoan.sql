
BEGIN
	IF(@bPublic = 1)
	BEGIN
		UPDATE DM_LoaiDuToan
		SET
			sMaLoaiDuToan = @sCode,
			sTenLoaiDuToan = @sName,
			sGhiChu = @sNote,
			iSoLanSua = ISNULL(iSoLanSua,0) + 1,
			dNgaySua = GETDATE(),
			sID_MaNguoiDungSua = @sUserLogin
		WHERE iID_LoaiDuToan = @iId
	END
	ELSE
	BEGIN
		UPDATE DM_LoaiDuToan
		SET
			bPublic = 0,
			dNgaySua = GETDATE(),
			sID_MaNguoiDungSua = @sUserLogin
		WHERE iID_LoaiDuToan = @iId
	END
END