
IF(@iId IS NULL)
BEGIN
	SELECT COUNT(iID_KeHoach5Nam_DeXuatID) FROM VDT_KHV_KeHoach5Nam_DeXuat WHERE sSoQuyetDinh = @sSoQuyetDinh AND iNamLamViec = @iNamLamViec
END
ELSE
BEGIN
	SELECT COUNT(iID_KeHoach5Nam_DeXuatID) FROM VDT_KHV_KeHoach5Nam_DeXuat WHERE sSoQuyetDinh = @sSoQuyetDinh AND iID_KeHoach5Nam_DeXuatID <> @iId AND iNamLamViec = @iNamLamViec
END
