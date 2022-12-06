
BEGIN
	IF(@bPublic = 1)
	BEGIN
		UPDATE DM_NoiDungChi
		SET
			sMaNoiDungChi = @sCode,
			sTenNoiDungChi = @sName,
			iID_Parent = @iID_Parent,
			sGhiChu = @sNote,
			iSoLanSua = ISNULL(iSoLanSua,0) + 1,
			dNgaySua = GETDATE(),
			sID_MaNguoiDungSua = @sUserLogin
		WHERE iID_NoiDungChi = @iId
	END
	ELSE
	BEGIN
		UPDATE DM_NoiDungChi
		SET
			bPublic = 0,
			dNgaySua = GETDATE(),
			sID_MaNguoiDungSua = @sUserLogin
		WHERE iID_NoiDungChi = @iId
	END
END

