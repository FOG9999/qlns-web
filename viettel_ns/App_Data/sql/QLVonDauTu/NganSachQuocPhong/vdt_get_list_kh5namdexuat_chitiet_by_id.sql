
DECLARE @iNamLamViec int				set @iNamLamViec = 0
DECLARE @sTen nvarchar(500)			set @sTen = ''
DECLARE @sTenDonVi nvarchar(500)	set @sTenDonVi = ''
DECLARE @sDiaDiem nvarchar(500)		set @sDiaDiem = ''
DECLARE @iGiaiDoanTu nvarchar(500)	set @iGiaiDoanTu = ''
DECLARE @iGiaiDoanDen nvarchar(500)	set @iGiaiDoanDen = ''
DECLARE @sDonViThucHienDuAn nvarchar(max) set @sDonViThucHienDuAn = ''
DECLARE @sDonVi nvarchar(max) set @sDonVi = ''
--#DECLARE#--

/*

Lấy danh sách danh mục lục ngân sách để mapping NDC

*/

SELECT 
	tree.iID_ParentModified,
	tree.iID_KeHoach5Nam_DeXuat_ChiTietID,
	tree.iID_KeHoach5Nam_DeXuatID,
	tree.fGiaTriKeHoach,
	tree.iID_DonViTienTeID,
	tree.fTiGiaDonVi,
	tree.fTiGia,
	tree.sTrangThai,
	tree.sGhiChu,
	case
		when 
			khthdxctpr.iID_KeHoach5Nam_DeXuat_ChiTietID is not null
		then
			khthdxctpr.fGiaTriNamThuNhat
		 else
			 tree.fGiaTriNamThuNhat
	end fGiaTriNamThuNhat,
	case
		when 
			khthdxctpr.iID_KeHoach5Nam_DeXuat_ChiTietID is not null
		then
			khthdxctpr.fGiaTriNamThuHai
		 else
			 tree.fGiaTriNamThuHai
	end fGiaTriNamThuHai,
	case
		when 
			khthdxctpr.iID_KeHoach5Nam_DeXuat_ChiTietID is not null
		then
			khthdxctpr.fGiaTriNamThuBa
		 else
			 tree.fGiaTriNamThuBa
	end fGiaTriNamThuBa,
	case
		when 
			khthdxctpr.iID_KeHoach5Nam_DeXuat_ChiTietID is not null
		then
			khthdxctpr.fGiaTriNamThuTu
		 else
			 tree.fGiaTriNamThuTu
	end fGiaTriNamThuTu,
	case
		when 
			khthdxctpr.iID_KeHoach5Nam_DeXuat_ChiTietID is not null
		then
			khthdxctpr.fGiaTriNamThuNam
		 else
			 tree.fGiaTriNamThuNam
	end fGiaTriNamThuNam,
	case
		when 
			khthdxctpr.iID_KeHoach5Nam_DeXuat_ChiTietID is not null
		then
			khthdxctpr.fGiaTriBoTri
		 else
			 tree.fGiaTriBoTri
	end fGiaTriBoTri,
	tree.fGiaTriNamThuNhat as fGiaTriNamThuNhatDc,
	tree.fGiaTriNamThuHai as fGiaTriNamThuHaiDc,
	tree.fGiaTriNamThuBa as fGiaTriNamThuBaDc,
	tree.fGiaTriNamThuTu as fGiaTriNamThuTuDc,
	tree.fGiaTriNamThuNam as fGiaTriNamThuNamDc,
	tree.fGiaTriBoTri as fGiaTriBoTriDc,
	cast(0 as float) as fLuyKeNSQPDaBoTri,
	cast(0 as float) as fLuyKeNSQPDeNghiBoTri,
	tree.iID_NguonVonID,
	tree.iID_LoaiCongTrinhID,
	tree.fVonDaGiao,
	tree.fVonBoTriTuNamDenNam,
	tree.fHanMucDauTu,
	tree.iID_DonViQuanLyID,
	tree.sTen,
	tree.iIDReference,
	tree.sDiaDiem,
	tree.iGiaiDoanTu,
	tree.iGiaiDoanDen,
	tree.iID_ParentID,
	tree.sMaOrder,
	tree.sSTT,
	tree.iLevel,
	tree.iIndexCode,
	tree.bIsParent,
	tree.iID_DuAnID,
	tree.iID_DonViID,
	--parent.sTen as sDuAnCha,
	ISNULL(tree.fGiaTriNamThuNhat, 0) + ISNULL(tree.fGiaTriNamThuHai, 0) + ISNULL(tree.fGiaTriNamThuBa, 0) + ISNULL(tree.fGiaTriNamThuTu, 0) + ISNULL(tree.fGiaTriNamThuNam, 0) as fTongSo,
	case 
		when tree.iID_NguonVonID = 1
		then
			ISNULL(tree.fHanMucDauTu, 0)
		else
			0
	end fTongSoNhuCauNSQP,
	--CONCAT(dv.iID_MaDonVi, ' - ', dv.sTen) as sTenDonViQL,
	--(
		--CASE
			--WHEN dv.iID_MaDonVi is null THEN ''
			--ELSE CONCAT(dv.iID_MaDonVi, ' - ', dv.sTenDonVi)
		--END
	--) as sDonViThucHienDuAn,
	(
		CASE
			WHEN parent.sSTT is null THEN ''
			ELSE CONCAT(parent.sSTT, ' - ', parent.sTen)
		END
	) as sDuAnCha,
	(
		CASE
			WHEN lct.sMaLoaiCongTrinh is null THEN ''
			ELSE CONCAT(lct.sMaLoaiCongTrinh, ' - ', lct.sTenLoaiCongTrinh)
		END
	) as sTenLoaiCongTrinh,
	(
		CASE
			WHEN nns.iID_MaNguonNganSach is null THEN ''
			ELSE CONCAT(nns.iID_MaNguonNganSach, ' - ', nns.sTen)
		END
	) as sTenNganSach,
	(
		CASE
			WHEN dv.iID_Ma is null THEN ''
			ELSE CONCAT(dv.iID_MaDonVi, ' - ', dv.sTen)
		END
	) as sDonVi,
	tbl_count_child.numChild,
	tree.fHanmucNganhDX,
	tree.fVon5namNganhDX,
	tree.fVonsaunamNganhDX,
	(ISNULL(tree.FVon5namNganhDX,0) + ISNULL(tree.fVonsaunamNganhDX,0)) as fTongVonBoTriNganh,

	tree.fHanmucCucTCDX,
	tree.fVon5namCTCDX,
	tree.fVonnamthunhatCTC,
	tree.fVonsaunamCTCDexuat,
	(ISNULL(tree.FVon5namCTCDX,0) + ISNULL(tree.fVonsaunamCTCDexuat,0)) as fTongVonBoTriCuc,
	tree.fCucTCDeXuat,
	tree.fDuKienBoTriNamThu2

FROM VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet tree
LEFT JOIN (
	select iID_ParentID, count(iID_ParentID) as numChild
	from VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet
	where iID_ParentID is not null
	GROUP BY iID_ParentID
) tbl_count_child on tree.iID_KeHoach5Nam_DeXuat_ChiTietID = tbl_count_child.iID_ParentID
LEFT JOIN NS_DonVi dv on tree.iID_DonViID = dv.iID_Ma
LEFT JOIN VDT_DM_LoaiCongTrinh lct on tree.iID_LoaiCongTrinhID = lct.iID_LoaiCongTrinh
LEFT JOIN NS_NguonNganSach nns on tree.iID_NguonVonID = nns.iID_MaNguonNganSach
LEFT JOIN VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet parent on tree.iID_ParentID = parent.iID_KeHoach5Nam_DeXuat_ChiTietID
LEFT JOIN VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet khthdxctpr on tree.iID_ParentModified = khthdxctpr.iID_KeHoach5Nam_DeXuat_ChiTietID
where 1 = 1
	and tree.iID_KeHoach5Nam_DeXuatID = @iId
	and (@sTen is null or tree.sTen like @sTen)
	--and (@sDonViThucHienDuAn is null or dv.sTenDonVi like @sDonViThucHienDuAn)
	and (@sDiaDiem is null or tree.sDiaDiem like @sDiaDiem)
	and (@iGiaiDoanTu is null or tree.iGiaiDoanTu like @iGiaiDoanTu)
	and (@iGiaiDoanDen is null or tree.iGiaiDoanDen like @iGiaiDoanDen)
	and (@sDonVi is null or dv.sTen like @sDonVi)
ORDER BY tree.sMaOrder