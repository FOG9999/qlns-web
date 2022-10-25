
IF(@iId IS NULL)
BEGIN
	SELECT COUNT(iID_NoiDungChi) FROM DM_NoiDungChi WHERE sMaNoiDungChi = @sMaNoiDungChi AND bPublic = 1 AND iNamLamViec = @iNamLamViec
END
ELSE
BEGIN
	SELECT COUNT(iID_NoiDungChi) FROM DM_NoiDungChi WHERE sMaNoiDungChi = @sMaNoiDungChi AND iID_NoiDungChi <> @iId AND bPublic = 1 AND iNamLamViec = @iNamLamViec
END
