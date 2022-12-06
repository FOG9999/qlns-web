
--#DECLARE#--
/*

Lấy thông tin qddautu ở màn tạo sửa QDDT

*/


select qddt.sSoQuyetDinh,
	qddt.dNgayQuyetDinh,
	qddt.SMoTa,
	qddt.iID_QDDauTuID,
	qddt.iID_DuAnID,
	qddt.fTongMucDauTuPheDuyet,
	ctdt.fTMDTDuKienPheDuyet,
	qddt.bActive,
	qddt.bIsGoc,
	qddt.iID_ParentID,
	duan.sTenDuAn ,
	duan.sMaDuAn,
	duan.sDiaDiem,duan.sQuyMo,
	duan.iID_DonViQuanLyID,
	duan.iID_ChuDauTuID,
	duan.iID_NhomDuAnID,
	duan.sKhoiCong ,
	duan.sKetThuc,
	duan.iID_HinhThucQuanLyID,
	dv.sTen as sTenDonVi,
	cdt.sTenCDT,
	nda.sTenNhomDuAn,
	htql.sTenHinhThucQuanLy
 from VDT_DA_QDDauTu qddt
 inner join VDT_DA_DuAn duan ON duan.iID_DuAnID = qddt.iID_DuAnID
 left join NS_DonVi dv ON dv.iID_Ma = duan.iID_DonViQuanLyID
 left join DM_ChuDauTu cdt  ON cdt.ID = duan.iID_ChuDauTuID
 left join VDT_DM_NhomDuAn nda ON nda.iID_NhomDuAnID = duan.iID_NhomDuAnID
 left join VDT_DM_HinhThucQuanLy htql ON htql.iID_HinhThucQuanLyID = duan.iID_HinhThucQuanLyID
 left join VDT_DA_ChuTruongDauTu ctdt ON qddt.iID_ChuTruongDauTuID = ctdt.iID_ChuTruongDauTuID
 where qddt.iID_QDDauTuID = @iID_QDDauTuID