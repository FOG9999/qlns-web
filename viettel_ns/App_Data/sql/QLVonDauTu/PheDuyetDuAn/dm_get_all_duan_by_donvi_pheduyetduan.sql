
--#DECLARE#--
/*

Lấy danh sách dự án ở màn tạo mới phê duyệt dự án

*/


DECLARE @donViThucHienDuAn nvarchar(max);

SELECT @donViThucHienDuAn = dvda.iID_DonVi FROM VDT_DM_DonViThucHienDuAn dvda WHERE iID_MaDonVi IN (SELECT dv.iID_MaDonVi FROM NS_DonVi dv WHERE dv.iID_Ma = @donViQLId);

SELECT 
	* 
FROM 
	VDT_DA_DuAn duan
INNER JOIN 
	VDT_DM_DonViThucHienDuAn dv 
ON 
	dv.iID_DonVi = duan.iID_DonViThucHienDuAnID
WHERE 
	duan.iID_DonViThucHienDuAnID = @donViThucHienDuAn
	AND duan.iID_DuAnID NOT  IN
	(
		SELECT iID_DuAnID FROM VDT_DA_QDDauTu WHERE iID_DuAnID = duan.iID_DuAnID
			
	)
	AND duan.iID_DuAnID IN 
	(
		select iID_DuAnID from VDT_DA_ChuTruongDauTu where iID_DuAnID = duan.iID_DuAnID
	)