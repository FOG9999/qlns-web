
--#DECLARE#--
/*

Lấy danh sách thông tin dự án

*/


select nda.sTenNhomDuAn,nql.sTenNhomQuanLy,hql.sTenHinhThucQuanLy,da.*,dv.sTen as sTenDonvi
from VDT_DA_GoiThau gt
left join VDT_DA_DuAn da ON da.iID_DuAnID = gt.iID_DuAnID
left join NS_DonVi dv ON dv.iID_Ma = da.iID_DonViQuanLyID
left join VDT_DM_NhomDuAn nda ON nda.iID_NhomDuAnID = da.iID_NhomDuAnID
left join VDT_DM_NhomQuanLy nql ON nql.iID_NhomQuanLyID = da.iID_NhomQuanLyID
left join VDT_DM_HinhThucQuanLy hql ON hql.iID_HinhThucQuanLyID = da.iID_HinhThucQuanLyID
where gt.iID_GoiThauID = @iId