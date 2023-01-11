

--#DECLARE#--

/*

update fGiaTriKeHoach VDT_KHV_KeHoach5Nam_DeXuat

*/

UPDATE VDT_KHV_KeHoach5Nam_DeXuat
SET fGiaTriKeHoach = (
	select SUM(ISNULL(fGiaTriNamThuNhat, 0) + ISNULL(fGiaTriNamThuHai, 0) + ISNULL(fGiaTriNamThuBa, 0) + ISNULL(fGiaTriNamThuTu, 0) + ISNULL(fGiaTriNamThuNam, 0))
	from VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet
	where iLevel = 2 
		and iID_KeHoach5Nam_DeXuatID = @iID_KeHoach5NamID
	GROUP BY iID_KeHoach5Nam_DeXuatID
)
WHERE iID_KeHoach5Nam_DeXuatID = @iID_KeHoach5NamID;