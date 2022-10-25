SELECT qddt.*, 
	duan.sTenDuAn,
	duan.sMaDuAn,
	duan.sDiaDiem,
	duan.sSuCanThietDauTu,
	duan.sMucTieu,
	duan.sDienTichSuDungDat,
	duan.sNguonGocSuDungDat,
	duan.sQuyMo,
	duan.sKhoiCong,
	duan.sKetThuc,
	dvql.sTen AS sDonViQuanLy, 
	cdt.sTen AS sChuDauTu, 
	nhomDA.sTenNhomDuAn AS sTenNhomDuAn, 
	hinhthucQL.sTenHinhThucQuanLy AS sTenHinhThucQuanLy,
	nhomDA.iID_NhomDuAnID,
	hinhthucQL.iID_HinhThucQuanLyID
FROM VDT_DA_QDDauTu qddt
LEFT JOIN VDT_DA_DuAn duan ON duan.iID_DuAnID = qddt.iID_DuAnID
LEFT JOIN NS_DonVi dvql ON duan.iID_DonViQuanLyID = dvql.iID_Ma
LEFT JOIN NS_DonVi cdt ON duan.iID_ChuDauTuID = cdt.iID_Ma
LEFT JOIN VDT_DM_NhomDuAn nhomDA ON duan.iID_NhomDuAnID = nhomDA.iID_NhomDuAnID
LEFT JOIN VDT_DM_HinhThucQuanLy hinhthucQL ON duan.iID_HinhThucQuanLyID = hinhthucQL.iID_HinhThucQuanLyID
WHERE qddt.iID_QDDauTuID = @iID_QDDauTuID AND qddt.bActive = 1;
