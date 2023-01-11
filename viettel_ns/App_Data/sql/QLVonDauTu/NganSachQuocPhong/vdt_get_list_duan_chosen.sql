--DECLARE @listId nvarchar(max)	set @listId = 'e50e651a-fb94-4665-b568-af8201101415,b35139de-b152-4bea-9aa5-af8200e8c96c'

SELECT tbl_sum.* FROM (
	select 
		cast(null as uniqueidentifier) as iID_ParentModified,
		null as iID_KeHoach5Nam_DeXuat_ChiTietID,
		null as iID_KeHoach5Nam_DeXuatID,	
		duan.sTenDuAn as sTen,
		ctdt.sSoQuyetDinh as sQuyetDinhCTDT,
		(
			CASE
				WHEN ctdt.iID_ChuDauTuID is null THEN ''
				ELSE CONCAT(cdt.sId_CDT, ' - ', cdt.sTenCDT)
			END
		) as sChuDauTu,
		(
			CASE
				WHEN dv.iID_Ma is null THEN ''
				ELSE CONCAT(dv.iID_MaDonVi, ' - ', dv.sTen)
			END
		) as sDonVi,
		duan.sDiaDiem as sDiaDiem,
		duan.sKhoiCong as iGiaiDoanTu,
		duan.sKetThuc as iGiaiDoanDen,
		(
			CASE
				WHEN lct.sMaLoaiCongTrinh is null THEN ''
				ELSE CONCAT(lct.sMaLoaiCongTrinh, ' - ', lct.sTenLoaiCongTrinh)
			END
		) as sTenLoaiCongTrinh,
		cast(0 as float) as fGiaTriKeHoach,
		cast(null as uniqueidentifier) as iID_DonViTienTeID,
		cast(0 as float) as fTiGiaDonVi,
		cast(0 as float) as fTiGia,
		'' as sTrangThai,
		'' as sGhiChu,
		cast(0 as float) as fGiaTriNamThuNhat,
		cast(0 as float) as fGiaTriNamThuHai,
		cast(0 as float) as fGiaTriNamThuBa,
		cast(0 as float) as fGiaTriNamThuTu,
		cast(0 as float) as fGiaTriNamThuNam,
		cast(0 as float) as fGiaTriBoTri,
		cast(0 as float) as fGiaTriBoTriDc,
		cast(0 as float) as fGiaTriNamThuNhatDc,
		cast(0 as float) as fGiaTriNamThuHaiDc,
		cast(0 as float) as fGiaTriNamThuBaDc,
		cast(0 as float) as fGiaTriNamThuTuDc,
		cast(0 as float) as fGiaTriNamThuNamDc,
		cast(0 as float) as fLuyKeNSQPDaBoTri,
		cast(0 as float) as fLuyKeNSQPDeNghiBoTri,
		nv.iID_NguonVonID as iID_NguonVonID,
		cast(null as uniqueidentifier) as iID_LoaiCongTrinhID,
		cast(0 as float) as fVonDaGiao,
		cast(0 as float) as fVonBoTriTuNamDenNam,
		cast(0 as float) as fHanMucDauTu,
		cast(null as uniqueidentifier) as iID_DonViQuanLyID,
		cast(null as uniqueidentifier) as iIDReference,
		cast(null as uniqueidentifier) as iID_ParentID,
		'' as sMaOrder,
		'' as sSTT,
		null as iIndexCode,
		(
			CASE 
				WHEN tbl_count_child.numChild > 0 THEN cast(1 as bit)
				ELSE cast(0 as bit)
			END
		) as bIsParent,
		duan.iID_DuAnID as iID_DuAnID,
		dv.iID_Ma as iID_DonViID,
		cast(0 as float) as fTongSo,
		cast(0 as float) as fTongSoNhuCauNSQP,
		'' as sDuAnCha,
		(
			CASE
				WHEN nns.iID_MaNguonNganSach is null THEN ''
				ELSE CONCAT(nns.iID_MaNguonNganSach, ' - ', nns.sTen)
			END
		) as sTenNganSach,
		tbl_count_child.numChild as numChild,
		cast(0 as float) as fHanmucNganhDX,
		cast(0 as float) as fVon5namNganhDX,
		cast(0 as float) as fVonsaunamNganhDX,
		cast(0 as float) as fTongVonBoTriNganh,
		cast(0 as float) as fHanmucCucTCDX,
		cast(0 as float) as fVon5namCTCDX,
		cast(0 as float) as fVonnamthunhatCTC,
		cast(0 as float) as fVonsaunamCTCDexuat,
		cast(0 as float) as fTongVonBoTriCuc,
		cast(0 as float) as fCucTCDeXuat,
		cast(0 as float) as fDuKienBoTriNamThu2,
		2 as iLevel
	from VDT_DA_DuAn duan
	left join VDT_DA_ChuTruongDauTu ctdt on duan.iID_DuAnID = ctdt.iID_DuAnID
	left join DM_ChuDauTu cdt on ctdt.iID_ChuDauTuID = cdt.ID
	LEFT JOIN NS_DonVi dv on duan.iID_DonViQuanLyID = dv.iID_Ma
	LEFT JOIN VDT_DM_LoaiCongTrinh lct on duan.iID_LoaiCongTrinhID = lct.iID_LoaiCongTrinh
	LEFT JOIN (
		select iID_DuAnID, count(iID_DuAn_HangMucID) as numChild
		from VDT_DA_DuAn_HangMuc	
		GROUP BY iID_DuAnID
	) tbl_count_child on duan.iID_DuAnID = tbl_count_child.iID_DuAnID
	LEFT JOIN VDT_DA_DuAn_NguonVon nv on nv.iID_DuAn = duan.iID_DuAnID
	LEFT JOIN NS_NguonNganSach nns on nv.iID_NguonVonID = nns.iID_MaNguonNganSach
	where duan.iID_DuAnID in (select * from dbo.splitstring(@listId))

	UNION ALL

	select 
		cast(null as uniqueidentifier) as iID_ParentModified,
		cast(null as uniqueidentifier) as iID_KeHoach5Nam_DeXuat_ChiTietID,
		cast(null as uniqueidentifier) as iID_KeHoach5Nam_DeXuatID,	
		dahm.sTenHangMuc as sTen,
		ctdt.sSoQuyetDinh as sQuyetDinhCTDT,
		(
			CASE
				WHEN ctdt.iID_ChuDauTuID is null THEN ''
				ELSE CONCAT(cdt.sId_CDT, ' - ', cdt.sTenCDT)
			END
		) as sChuDauTu,
		(
			CASE
				WHEN dv.iID_Ma is null THEN ''
				ELSE CONCAT(dv.iID_MaDonVi, ' - ', dv.sTen)
			END
		) as sDonVi,
		duan.sDiaDiem as sDiaDiem,
		duan.sKhoiCong as iGiaiDoanTu,
		duan.sKetThuc as iGiaiDoanDen,
		(
			CASE
				WHEN lct.sMaLoaiCongTrinh is null THEN ''
				ELSE CONCAT(lct.sMaLoaiCongTrinh, ' - ', lct.sTenLoaiCongTrinh)
			END
		) as sTenLoaiCongTrinh,
		cast(0 as float) as fGiaTriKeHoach,
		cast(null as uniqueidentifier) as iID_DonViTienTeID,
		cast(0 as float) as fTiGiaDonVi,
		cast(0 as float) as fTiGia,
		'' as sTrangThai,
		'' as sGhiChu,
		cast(0 as float) as fGiaTriNamThuNhat,
		cast(0 as float) as fGiaTriNamThuHai,
		cast(0 as float) as fGiaTriNamThuBa,
		cast(0 as float) as fGiaTriNamThuTu,
		cast(0 as float) as fGiaTriNamThuNam,
		cast(0 as float) as fGiaTriBoTri,
		cast(0 as float) as fGiaTriBoTriDc,
		cast(0 as float) as fGiaTriNamThuNhatDc,
		cast(0 as float) as fGiaTriNamThuHaiDc,
		cast(0 as float) as fGiaTriNamThuBaDc,
		cast(0 as float) as fGiaTriNamThuTuDc,
		cast(0 as float) as fGiaTriNamThuNamDc,
		cast(0 as float) as fLuyKeNSQPDaBoTri,
		cast(0 as float) as fLuyKeNSQPDeNghiBoTri,
		nv.iID_NguonVonID as iID_NguonVonID,
		dahm.iID_LoaiCongTrinhID as iID_LoaiCongTrinhID,
		cast(0 as float) as fVonDaGiao,
		cast(0 as float) as fVonBoTriTuNamDenNam,
		cast(0 as float) as fHanMucDauTu,
		cast(null as uniqueidentifier) as iID_DonViQuanLyID,
		cast(null as uniqueidentifier) as iIDReference,
		cast(null as uniqueidentifier) as iID_ParentID,
		'' as sMaOrder,
		'' as sSTT,	
		null as iIndexCode,
		cast(0 as bit) as bIsParent,
		duan.iID_DuAnID as iID_DuAnID,
		dv.iID_Ma as iID_DonViID,
		cast(0 as float) as fTongSo,
		cast(0 as float) as fTongSoNhuCauNSQP,
		'' as sDuAnCha,
		(
			CASE
				WHEN nns.iID_MaNguonNganSach is null THEN ''
				ELSE CONCAT(nns.iID_MaNguonNganSach, ' - ', nns.sTen)
			END
		) as sTenNganSach,
		null as numChild,
		cast(0 as float) as fHanmucNganhDX,
		cast(0 as float) as fVon5namNganhDX,
		cast(0 as float) as fVonsaunamNganhDX,
		cast(0 as float) as fTongVonBoTriNganh,
		cast(0 as float) as fHanmucCucTCDX,
		cast(0 as float) as fVon5namCTCDX,
		cast(0 as float) as fVonnamthunhatCTC,
		cast(0 as float) as fVonsaunamCTCDexuat,
		cast(0 as float) as fTongVonBoTriCuc,
		cast(0 as float) as fCucTCDeXuat,
		cast(0 as float) as fDuKienBoTriNamThu2,
		3 as iLevel
	from VDT_DA_DuAn_HangMuc dahm   
	left join VDT_DA_DuAn duan on dahm.iID_DuAnID = duan.iID_DuAnID
	left join VDT_DA_ChuTruongDauTu ctdt on duan.iID_DuAnID = ctdt.iID_DuAnID
	left join DM_ChuDauTu cdt on ctdt.iID_ChuDauTuID = cdt.ID
	LEFT JOIN NS_DonVi dv on duan.iID_DonViQuanLyID = dv.iID_Ma
	LEFT JOIN VDT_DM_LoaiCongTrinh lct on dahm.iID_LoaiCongTrinhID = lct.iID_LoaiCongTrinh
	LEFT JOIN VDT_DA_DuAn_NguonVon nv on nv.iID_DuAn = duan.iID_DuAnID
	LEFT JOIN NS_NguonNganSach nns on nv.iID_NguonVonID = nns.iID_MaNguonNganSach
	where duan.iID_DuAnID in (select * from dbo.splitstring(@listId))
) AS tbl_sum
order by iID_DuAnID, iLevel asc
