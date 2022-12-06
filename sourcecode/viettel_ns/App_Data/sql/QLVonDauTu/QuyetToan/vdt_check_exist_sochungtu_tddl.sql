
IF(@id IS NULL)
BEGIN
	SELECT count(ID) FROM VDT_NS_Traodoidulieu WHERE  sSoChungTu = @sSoChungTu  AND iNamLamViec = @iNamLamViec
END
ELSE
BEGIN
	DECLARE @idNew uniqueidentifier;
	select @idNew = ID  from VDT_NS_Traodoidulieu where sSoChungTu = @sSoChungTu and iNamLamViec = @iNamLamViec
	IF  @idNew  = @id
		BEGIN
			select count(ID) from VDT_NS_Traodoidulieu where ID = null
		END
	ELSE
		BEGIN
			select count(ID) from VDT_NS_Traodoidulieu where ID = @idNew
		END

END
