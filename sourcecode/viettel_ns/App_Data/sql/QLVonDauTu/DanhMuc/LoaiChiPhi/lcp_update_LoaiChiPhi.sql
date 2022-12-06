
/*

Cập nhật loại chi phí

*/
UPDATE VDT_DM_ChiPhi
SET
	sMaChiPhi = @sMaChiPhi,
	sTenVietTat = @sTenVietTat,
	sTenChiPhi = @sTenChiPhi,
	sMoTa = @sMoTa,
	iThuTu = @iThuTu,
	iSoLanSua = ISNULL(iSoLanSua,0) + 1,
	dNgaySua = GETDATE(),
	sIPSua = @sIPSua,
	sID_MaNguoiDungSua = @sUserLogin
WHERE iID_ChiPhi = @iId
