
/*

Cập nhật nns phân nguồn

*/
UPDATE NNS_PhanNguon
SET
	sNoiDung = @sNoiDung,
	sSoChungTu = @sSoChungTu,
	dNgayChungTu = @dNgayChungTu,
	sSoQuyetDinh = @sSoQuyetDinh,
	dNgayQuyetDinh = @dNgayQuyetDinh,
	iNamLamViec = @iNamLamViec,
	iID_MaNguonNganSach = @iID_MaNguonNganSach,
	iID_MaNamNganSach = @iID_MaNamNganSach,
	iSoLanSua = ISNULL(iSoLanSua,0) + 1,
	dNgaySua = GETDATE(),
	sIPSua = @sIPSua,
	sID_MaNguoiDungSua = @sUserLogin
WHERE iID_PhanNguon = @iId
