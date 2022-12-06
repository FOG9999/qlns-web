SELECT ctdt.*, 
	dvql.sTen AS sDonViQuanLy, 
	cdt.sTenCDT AS sChuDauTu, 
	nhomDA.sTenNhomDuAn AS sTenNhomDuAn, 
	hinhthucQL.sTenHinhThucQuanLy AS sTenHinhThucQuanLy, 
	phancapDA.sTen AS sPhanCapDuAn, 
	loaiCT.sTenLoaiCongTrinh AS sTenLoaiCongTrinh,
	duan.sTenDuAn,
	duan.sMaDuAn
FROM VDT_DA_ChuTruongDauTu ctdt
LEFT JOIN NS_DonVi dvql ON ctdt.iID_DonViQuanLyID = dvql.iID_Ma
LEFT JOIN DM_ChuDauTu cdt ON ctdt.iID_ChuDauTuID = cdt.ID
LEFT JOIN VDT_DM_NhomDuAn nhomDA ON ctdt.iID_NhomDuAnID = nhomDA.iID_NhomDuAnID
LEFT JOIN VDT_DM_HinhThucQuanLy hinhthucQL ON ctdt.iID_HinhThucQuanLyID = hinhthucQL.iID_HinhThucQuanLyID
LEFT JOIN VDT_DM_PhanCapDuAn phancapDA ON ctdt.iID_CapPheDuyetID = phancapDA.iID_PhanCapID
LEFT JOIN VDT_DM_LoaiCongTrinh loaiCT ON ctdt.iID_LoaiCongTrinhID = loaiCT.iID_LoaiCongTrinh
LEFT JOIN VDT_DA_DuAn as duan on ctdt.iID_DuAnID = duan.iID_DuAnID
WHERE ctdt.iID_ChuTruongDauTuID = @iID_ChuTruongDauTuID 
--AND ctdt.bActive = 1;
