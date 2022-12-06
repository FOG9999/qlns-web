
IF(@iID_DotNhan IS NULL)
BEGIN
	select count(*) from NNS_DotNhan where sMaLoaiDuToan = @sMaLoaiDuToan and iNamLamViec = @iNamLamViec
END
ELSE
BEGIN
	select count(*) from NNS_DotNhan where sMaLoaiDuToan = @sMaLoaiDuToan and iNamLamViec = @iNamLamViec and iID_DotNhan <> @iID_DotNhan
END
