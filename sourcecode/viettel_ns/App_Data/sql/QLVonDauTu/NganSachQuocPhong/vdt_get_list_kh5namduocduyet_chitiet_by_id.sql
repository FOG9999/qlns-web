
DECLARE @iNamLamViec int				set @iNamLamViec = 0
DECLARE @sTenLoaiCongTrinh nvarchar(500)set @sTenLoaiCongTrinh = ''
DECLARE @sTen nvarchar(500)			set @sTen = ''
DECLARE @sTenDonViQL nvarchar(500)	set @sTenDonViQL = ''
DECLARE @sDiaDiem nvarchar(500)		set @sDiaDiem = ''
DECLARE @sDonVi nvarchar(max) set @sDonVi = ''
--#DECLARE#--

/*

Lấy danh sách danh kế hoạch trung hạn được duyệt chi tiết

*/

SELECT kh5n_ct.iID_KeHoach5Nam_ChiTietID , 
	kh5n_ct.iID_KeHoach5NamID,
	lct.sTenLoaiCongTrinh as sTenLoaiCongTrinh,
	kh5n_ct.sTen,
	--dv.sTenDonVi as sTenDonViQL,
	CONCAT(dv.iID_MaDonVi, ' - ', dv.sTen) as sDonVi,
	duan.sDiaDiem,
	CONCAT(duan.sKhoiCong, ' - ', duan.sKetThuc) as sThoiGianThucHien,
	nns.sTen as sTenNganSach,
	ISNULL(kh5n_ct.fHanMucDauTu, 0) as fHanMucDauTu,
	kh5n_ct.iID_DonViID,
	case
		when kh5n_ct.iID_NguonVonID = 1 
			then kh5n_ct.fHanMucDauTu 
	end fTongNhuCauNSQP,
	case
		when kh5n_ct.iID_ParentID is not null
		then
			kh5nctpr.fVonDaGiao
		else
			kh5n_ct.fVonDaGiao
	end fVonDaGiao,
	case
		when kh5n_ct.iID_ParentID is not null
		then
			kh5nctpr.fVonBoTriTuNamDenNam
		else
			kh5n_ct.fVonBoTriTuNamDenNam
	end fVonBoTriTuNamDenNam,
	case
		when kh5n_ct.iID_ParentID is not null
		then
			kh5nctpr.fGiaTriBoTri
		else
			kh5n_ct.fGiaTriBoTri
	end fGiaTriBoTri,
	ISNULL(kh5n_ct.fVonDaGiao, 0) as fVonDaGiaoDc,
	ISNULL(kh5n_ct.fVonBoTriTuNamDenNam, 0) as fVonBoTriTuNamDenNamDc,
	ISNULL(kh5n_ct.fGiaTriBoTri, 0) as fGiaTriBoTriDc,
	kh5n_ct.sGhiChu,
	kh5n_ct.iID_ParentID
FROM VDT_KHV_KeHoach5Nam_ChiTiet kh5n_ct
INNER JOIN VDT_DA_DuAn duan on kh5n_ct.iID_DuAnID = duan.iID_DuAnID
--LEFT JOIN VDT_DM_DonViThucHienDuAn dv on kh5n_ct.iID_DonViQuanLyID = dv.iID_DonVi 
LEFT JOIN VDT_DM_LoaiCongTrinh lct on kh5n_ct.iID_LoaiCongTrinhID = lct.iID_LoaiCongTrinh
LEFT JOIN NS_NguonNganSach nns on kh5n_ct.iID_NguonVonID = nns.iID_MaNguonNganSach
LEFT JOIN VDT_KHV_KeHoach5Nam_ChiTiet kh5nctpr on kh5n_ct.iID_ParentID = kh5nctpr.iID_KeHoach5Nam_ChiTietID
LEFT JOIN NS_DonVi dv on dv.iID_Ma = kh5n_ct.iID_DonViID
where 1 = 1
	and kh5n_ct.iID_KeHoach5NamID = @iID_KeHoach5NamID
	and (@sTen is null or kh5n_ct.sTen like @sTen)
	and (@sTenLoaiCongTrinh is null or lct.sTenLoaiCongTrinh like @sTenLoaiCongTrinh)
	--and (@sTenDonViQL is null or dv.sTenDonVi like @sTenDonViQL)
	and (@sDiaDiem is null or duan.sDiaDiem like @sDiaDiem)
	and (@sDonVi is null or dv.sTen like @sDonVi)
	ORDER BY duan.sTenDuAn