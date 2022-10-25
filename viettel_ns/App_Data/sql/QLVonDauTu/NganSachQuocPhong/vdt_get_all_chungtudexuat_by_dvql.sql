--#DECLARE#--

/*

Lấy danh sách chứng từ đề xuất theo don vi quan ly

*/

SELECT DISTINCT Id, iID_DonViQuanLyID, sSoQuyetDinh, iGiaiDoanTu, iGiaiDoanDen
FROM VDT_KHV_KeHoach5Nam_DeXuat 
WHERE iID_DonViQuanLyID = @iID_DonViQuanLyID
	and	NamLamViec = @iNamLamViec