
DECLARE @listKhvIds NVARCHAR(MAX) 
--#DECLARE#--
SELECT ct.iID_KeHoachUngID, khvu.*, khvu.sSoDeNghi, khvu.dNgayDeNghi, khvu.fGiaTriUng,
				dv.iID_Ma AS iID_DonViQuanLyID, dv.sTen AS sTenDonVi, nv.sTen AS sTenNguonVon
FROM VDT_KHV_KeHoachVonUng_DX khvu
INNER JOIN VDT_KHV_KeHoachVonUng_DX_ChiTiet ct on ct.iID_KeHoachUngID = khvu.Id
LEFT JOIN NS_DonVi AS dv ON khvu.iID_DonViQuanLyID = dv.iID_Ma
LEFT JOIN NS_NguonNganSach AS nv ON khvu.iID_NguonVonID = nv.iID_MaNguonNganSach
WHERE  khvu.Id IN ((SELECT * FROM dbo.f_split(@listKhvIds)))

