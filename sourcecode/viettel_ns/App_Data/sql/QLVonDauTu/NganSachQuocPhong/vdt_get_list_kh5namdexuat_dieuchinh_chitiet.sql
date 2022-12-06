	--declare @VoucherAgregate uniqueidentifier;
	--select @VoucherAgregate = iID_TongHopParent from VDT_KHV_KeHoach5Nam_DeXuat where iID_KeHoach5Nam_DeXuatID = @iID_KeHoach5Nam_DeXuatID;
	
	SELECT DISTINCT
		
		ctct.sSTT,
		ctct.iID_KeHoach5Nam_DeXuat_ChiTietID,
		ctct.iID_KeHoach5Nam_DeXuatID,
		ctctdd.iID_DuAnID,
		ctct.sTen,
		ctct.sDiaDiem,
		ctct.iGiaiDoanTu,
		ctct.iGiaiDoanDen,
		ctct.iID_LoaiCongTrinhID,
		ctct.iID_NguonVonID,
		ctct.iID_DonViQuanLyID,
		ctct.fHanMucDauTu,
		ctct.fGiaTriNamThuNhat,
		ctct.fGiaTriNamThuHai,
		ctct.fGiaTriNamThuBa,
		ctct.fGiaTriNamThuTu,
		ctct.fGiaTriNamThuNam,
		ctct.fGiaTriBoTri,
		ctct.fGiaTriKeHoach,
		ctct.iID_ParentModified,
		ctct.sGhiChu,
		ctct.iLevel,
		ctct.iIndexCode,
		ctct.sMaOrder,
		ctct.iIDReference,
		ctct.iID_ParentID,
		ctct.bIsParent,
		ctct.sTrangThai,
		ctct.iID_TongHop,
		ctct.iID_MaDonVi
		
		 
	FROM VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet ctct
	INNER JOIN VDT_DA_DuAn da
		ON ctct.iID_KeHoach5Nam_DeXuat_ChiTietID = da.iID_DuAnKHTHDeXuatID OR ctct.iIDReference = da.iID_DuAnKHTHDeXuatID
	INNER JOIN VDT_KHV_KeHoach5Nam_ChiTiet ctctdd
		ON ctctdd.iID_DuAnID = da.iID_DuAnID
	WHERE 1=1
		and ctct.iID_KeHoach5Nam_DeXuatID in ( select  iID_TongHopParent from VDT_KHV_KeHoach5Nam_DeXuat where iID_KeHoach5Nam_DeXuatID = @iID_KeHoach5Nam_DeXuatIDPara)
		--and ctct.iID_KeHoach5Nam_DeXuatID = @VoucherAgregate
		AND ctct.iID_TongHop = @iID_KeHoach5Nam_DeXuatIDPara
