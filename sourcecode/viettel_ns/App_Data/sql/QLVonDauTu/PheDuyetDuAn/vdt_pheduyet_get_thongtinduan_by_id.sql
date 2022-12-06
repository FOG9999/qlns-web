
--#DECLARE#--
/*

Lấy thông tin dự án ở màn tạo mới phê duyệt dự án

*/


SELECT da.* ,
dv.sTen as sTenDonvi,
cdt.sTenCDT as sTenChuDauTu,
nda.sTenNhomDuAn 
FROM VDT_DA_DuAn da 
left join NS_DonVi dv ON dv.iID_Ma = da.iID_DonViQuanLyID
left join DM_ChuDauTu cdt ON cdt.ID = da.iID_ChuDauTuID
left join VDT_DM_NhomDuAn nda ON nda.iID_NhomDuAnID = da.iID_NhomDuAnID
 WHERE da.iID_DuAnID = @iId