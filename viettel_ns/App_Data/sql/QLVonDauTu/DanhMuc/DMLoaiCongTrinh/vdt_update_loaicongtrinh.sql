BEGIN
	IF(@bPublic = 1)
	BEGIN
		UPDATE VDT_DM_LoaiCongTrinh
		SET
			sMaLoaiCongTrinh = @sMaLoaiCongTrinh,
			sTenLoaiCongTrinh = @sTenLoaiCongTrinh,
			sTenVietTat = @sTenVietTat,
			iID_Parent = @iParentID,
			iThuTu = @iThuTu,
			sMoTa = @sMoTa,
			iSoLanSua = ISNULL(iSoLanSua,0) + 1,
			dNgaySua = GETDATE(),
			sID_MaNguoiDungSua = @sUserLogin,
			LNS =@LNS,
			L = @L,
			K = @K,
			M = @M,
			TM = @TM,
			TTM = @TTM,
			NG = @NG,
			TNG1 =@TNG1,
			TNG2 = @TNG2

		WHERE iID_LoaiCongTrinh = @iId AND bActive = 1
	END
	ELSE
	BEGIN
		UPDATE VDT_DM_LoaiCongTrinh
		SET
			bActive = 0,
			dNgaySua = GETDATE(),
			sID_MaNguoiDungSua = @sUserLogin
		WHERE iID_LoaiCongTrinh = @iId AND bActive = 1
	END
END
