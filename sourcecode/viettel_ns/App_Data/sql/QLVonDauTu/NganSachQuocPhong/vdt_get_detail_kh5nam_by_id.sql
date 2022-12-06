--#DECLARE#--

/*

Lấy chi tiết KH 5 năm theo Id

*/

SELECT kh5nam.*, donVi.sTen as sTenDonvVi,
			(
				CASE
					WHEN kh5nam_dx.sSoQuyetDinh is null THEN ''
					ELSE CONCAT(kh5nam_dx.sSoQuyetDinh, ' (', kh5nam_dx.iGiaiDoanTu, ' - ', kh5nam_dx.iGiaiDoanDen, ')')
				END
			) as sChungTuDeXuat
FROM VDT_KHV_KeHoach5Nam kh5nam
INNER JOIN NS_DonVi as donVi on kh5nam.iID_DonViQuanLyID = donVi.iID_Ma
LEFT JOIN VDT_KHV_KeHoach5Nam_DeXuat kh5nam_dx on kh5nam.iID_KeHoach5NamDeXuatID = kh5nam_dx.iID_KeHoach5Nam_DeXuatID
WHERE kh5nam.iID_KeHoach5NamID = @iID_KeHoach5NamID