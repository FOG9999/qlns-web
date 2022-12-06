
BEGIN
	IF(@bPublic = 1) 
	--update
	BEGIN
		UPDATE NNS_DotNhan
		SET
			sMaLoaiDuToan = @sMaLoaiDuToan,
			sNoiDung = @sNoiDung,
			sSoChungTu = @sSoChungTu,
			sSoQuyetDinh = @sSoQuyetDinh,
			dNgayChungTu = @dNgayChungTu,
			dNgayQuyetDinh = @dNgayQuyetDinh,
			iSoLanSua = ISNULL(iSoLanSua,0) + 1,
			dNgaySua = GETDATE(),
			sID_MaNguoiDungSua = @sUserLogin
		WHERE iID_DotNhan = @iId
	END
	ELSE
	--delete
	BEGIN
		UPDATE NNS_DotNhan
		SET
			bPublic = 0,
			dNgaySua = GETDATE(),
			sID_MaNguoiDungSua = @sUserLogin
		WHERE iID_DotNhan = @iId
	END
END

