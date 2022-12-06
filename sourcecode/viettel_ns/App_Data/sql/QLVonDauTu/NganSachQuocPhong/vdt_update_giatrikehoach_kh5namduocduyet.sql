UPDATE VDT_KHV_KeHoach5Nam
SET fGiaTriDuocDuyet = (
	select SUM(ISNULL(ctct.fVonBoTriTuNamDenNam, 0))
	from VDT_KHV_KeHoach5Nam_ChiTiet ctct
	--where ctct.iID_ParentID is null
	--where ctct.iID_ParentID is not null
		--and ctct.iID_KeHoach5NamID = @iID_KeHoach5NamID
	where ctct.iID_KeHoach5NamID = @iID_KeHoach5NamID
	GROUP BY ctct.iID_KeHoach5NamID
)
WHERE iID_KeHoach5NamID = @iID_KeHoach5NamID;