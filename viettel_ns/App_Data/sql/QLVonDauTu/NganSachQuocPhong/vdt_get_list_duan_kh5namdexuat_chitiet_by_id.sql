
DECLARE @iNamLamViec int				set @iNamLamViec = 0
DECLARE @sSTT nvarchar(500)			set @sSTT = ''
DECLARE @sTen nvarchar(500)			set @sTen = ''
DECLARE @sTenDonViQL nvarchar(500)	set @sTenDonViQL = ''
DECLARE @sDiaDiem nvarchar(500)		set @sDiaDiem = ''
DECLARE @iGiaiDoanTu nvarchar(500)	set @iGiaiDoanTu = ''
DECLARE @iGiaiDoanDen nvarchar(500)	set @iGiaiDoanDen = ''
--#DECLARE#--

/*

Lấy danh sách danh mục lục ngân sách để mapping NDC

*/

SELECT tree.* , 
	ISNULL(tree.fGiaTriNamThuNhat, 0) + ISNULL(tree.fGiaTriNamThuHai, 0) + ISNULL(tree.fGiaTriNamThuBa, 0) + ISNULL(tree.fGiaTriNamThuTu, 0) + ISNULL(tree.fGiaTriNamThuNam, 0) as fTongSo,
	ISNULL(tree.fGiaTriNamThuNhat, 0) + ISNULL(tree.fGiaTriNamThuHai, 0) + ISNULL(tree.fGiaTriNamThuBa, 0) + ISNULL(tree.fGiaTriNamThuTu, 0) + ISNULL(tree.fGiaTriNamThuNam, 0) + ISNULL(tree.fGiaTriBoTri, 0) as fTongSoNhuCauNSQP,
	(
		CASE
			WHEN dv.iID_MaDonVi is null THEN ''
			ELSE CONCAT(dv.iID_MaDonVi, ' - ', dv.sTen)
		END
	) as sTenDonViQL,
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
	tbl_count_child.numChild,
	CAST('0' as bit) as isMap into #TMP_duan_KH5NDXCT
FROM VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet tree
LEFT JOIN (
	select iID_ParentID, count(iID_ParentID) as numChild
	from VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet
	where iID_ParentID is not null
	GROUP BY iID_ParentID
) tbl_count_child on tree.iID_KeHoach5Nam_DeXuat_ChiTietID = tbl_count_child.iID_ParentID
LEFT JOIN NS_DonVi dv on tree.iID_DonViQuanLyID = dv.iID_Ma 
LEFT JOIN VDT_DM_LoaiCongTrinh lct on tree.iID_LoaiCongTrinhID = lct.iID_LoaiCongTrinh
LEFT JOIN NS_NguonNganSach nns on tree.iID_NguonVonID = nns.iID_MaNguonNganSach
LEFT JOIN VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet parent on tree.iID_ParentID = parent.iID_KeHoach5Nam_DeXuat_ChiTietID
where tree.iLevel != 1
	and tree.iID_KeHoach5Nam_DeXuatID = @iId;

select * from #TMP_duan_KH5NDXCT
where 1 = 1
	and (@sTen is null or sTen like @sTen)
	and (@sTenDonViQL is null or sTenDonViQL like @sTenDonViQL)
	and (@sDiaDiem is null or sDiaDiem like @sDiaDiem)
	and (@iGiaiDoanTu is null or iGiaiDoanTu like @iGiaiDoanTu)
	and (@iGiaiDoanDen is null or iGiaiDoanDen like @iGiaiDoanDen)
ORDER BY sMaOrder;

DROP TABLE #TMP_duan_KH5NDXCT;