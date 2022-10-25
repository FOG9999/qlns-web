
--#DECLARE#--
/*

Lấy danh sách dự án ở màn tạo mới kế hoạch lựa chọn nhà thầu

*/
if(@iLoaiChungTu = 3)
BEGIN
	SELECT da.*
	FROM VDT_DA_DuAn da
	INNER JOIN NS_DonVi dv ON dv.iID_Ma = da.iID_DonViQuanLyID
	where  da.iID_MaDonViThucHienDuAnID = @iIdMaDonViQuanLy 
	AND EXISTS (select * from VDT_DA_ChuTruongDauTu WHERE iID_DuAnID = da.iID_DuAnID)
	AND NOT EXISTS (select * from VDT_QDDT_KHLCNhaThau where iID_DuAnID = da.iID_DuAnID)
END
ELSE
BEGIN
SELECT da.*
FROM
(
	SELECT DISTINCT da.iID_DuAnID 
	FROM VDT_DA_DuAn da
	INNER JOIN NS_DonVi dv ON dv.iID_Ma = da.iID_DonViQuanLyID
	LEFT JOIN VDT_DA_QDDauTu as qddt on @iLoaiChungTu = 2 AND qddt.bActive = 1 AND da.iID_DuAnID = qddt.iID_DuAnID
	LEFT JOIN VDT_DA_DuToan as dt on @iLoaiChungTu = 1 AND dt.bActive=1 AND da.iID_DuAnID = dt.iID_DuAnID
	where  da.iID_MaDonViThucHienDuAnID = @iIdMaDonViQuanLy AND (qddt.iID_QDDauTuID IS NOT NULL OR dt.iID_DuToanID IS NOT NULL)
) as tbl
INNER JOIN VDT_DA_DuAn as da on tbl.iID_DuAnID = da.iID_DuAnID
END