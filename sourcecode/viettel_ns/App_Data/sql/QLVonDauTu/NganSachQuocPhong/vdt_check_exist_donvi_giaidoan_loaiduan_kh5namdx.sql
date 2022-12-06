IF(@iID_KeHoach5Nam_DeXuatID IS NULL)
BEGIN
	SELECT COUNT(iID_KeHoach5Nam_DeXuatID) FROM VDT_KHV_KeHoach5Nam_DeXuat WHERE iGiaiDoanTu = @iGiaiDoanTu AND iGiaiDoanDen = @iGiaiDoanDen AND iID_DonViQuanLyID = @iID_DonViQuanLyID AND iNamLamViec = @iNamLamViec AND iLoai = @iLoai
END
ELSE
BEGIN
	SELECT COUNT(iID_KeHoach5Nam_DeXuatID) FROM VDT_KHV_KeHoach5Nam_DeXuat WHERE iGiaiDoanTu = @iGiaiDoanTu AND iGiaiDoanDen = @iGiaiDoanDen AND iID_DonViQuanLyID = @iID_DonViQuanLyID AND iID_KeHoach5Nam_DeXuatID <> @iID_KeHoach5Nam_DeXuatID AND iNamLamViec = @iNamLamViec AND iLoai = @iLoai
END