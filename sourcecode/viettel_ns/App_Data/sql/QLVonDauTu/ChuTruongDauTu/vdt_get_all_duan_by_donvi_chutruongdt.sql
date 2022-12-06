
--#DECLARE#--
/*

Lấy danh sách dự án ở màn tạo mới phê duyệt dự án

*/


--Ban Dau--

--SELECT  da.* FROM VDT_DA_DuAn da  
--INNER JOIN VDT_DM_DonViThucHienDuAn dv
--ON dv.iID_DonVi = da.iID_DonViQuanLyID

-- WHERE 
--    da.iID_DuAnID in
--    (  
--        SELECT chitiet.iID_DuAnID FROM VDT_KHV_KeHoach5Nam_ChiTiet chitiet
--    )
-- AND da.iID_DuAnID NOT IN
--    (
--        SELECT distinct ct.iID_DuAnID from VDT_DA_ChuTruongDauTu ct
--        WHERE ct.iID_DuAnID = da.iID_DuAnID
--    )
--  AND dv.iID_MaDonVi = @maDonViThucHienDuAn

--Update--


	SELECT  da.* FROM VDT_DA_DuAn da  
	INNER JOIN NS_DonVi dv
	ON dv.iID_Ma = da.iID_DonViQuanLyID
	WHERE 
	 dv.iID_MaDonVi = @maDonViThucHienDuAn