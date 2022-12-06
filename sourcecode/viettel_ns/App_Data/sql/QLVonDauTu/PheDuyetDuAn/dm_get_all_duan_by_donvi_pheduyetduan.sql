
--#DECLARE#--
/*

Lấy danh sách dự án ở màn tạo mới phê duyệt dự án

*/


--DECLARE @donViThucHienDuAn nvarchar(max);

--SELECT @donViThucHienDuAn = dvda.iID_Ma FROM NS_DonVi dvda WHERE iID_MaDonVi IN (SELECT dv.iID_MaDonVi FROM NS_DonVi dv WHERE dv.iID_Ma = @donViQLId);

SELECT 
	* 
FROM 
	VDT_DA_DuAn duan
INNER JOIN 
	NS_DonVi dv 
ON 
	dv.iID_Ma = duan.iID_DonViQuanLyID
WHERE 
	duan.iID_DonViQuanLyID IN (SELECT dvda.iID_Ma FROM NS_DonVi dvda WHERE iID_MaDonVi IN (SELECT dv.iID_MaDonVi FROM NS_DonVi dv WHERE dv.iID_Ma = @donViQLId))
	AND duan.iID_DuAnID NOT  IN
	(
		SELECT iID_DuAnID FROM VDT_DA_QDDauTu WHERE iID_DuAnID = duan.iID_DuAnID
			
	)
	AND duan.iID_DuAnID IN 
	(
		select iID_DuAnID from VDT_DA_ChuTruongDauTu where iID_DuAnID = duan.iID_DuAnID
	)