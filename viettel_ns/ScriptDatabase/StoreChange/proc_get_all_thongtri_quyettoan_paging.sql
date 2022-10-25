
GO
/****** Object:  StoredProcedure [dbo].[proc_get_all_thongtri_quyettoan_paging]    */
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROC [dbo].[proc_get_all_thongtri_quyettoan_paging] 
@sMaThongTri nvarchar ( MAX ),
@iNamThongTri INT,
@dNgayThongTri DATETIME,
@sMaDonVi nvarchar ( MAX ),
@CurrentPage INT,
@ItemsPerPage INT,
@iToTalItem INT OUTPUT AS 
BEGIN
	SET @iToTalItem = (
		SELECT COUNT
			( tbl.iID_ThongTriID ) 
		FROM VDT_ThongTri as tbl
		INNER JOIN VDT_DM_LoaiThongTri as dm on tbl.iID_LoaiThongTriID = dm.iID_LoaiThongTriID
		LEFT JOIN NS_DonVi as dv on tbl.iID_DonViID = dv.iID_Ma
		LEFT JOIN NS_NguonNganSach as nv on tbl.sMaNguonVon = nv.sMoTa
		WHERE dm.iKieuLoaiThongTri = 1
			AND (ISNULL(@sMaThongTri, '') = '' OR tbl.sMaThongTri LIKE '%@sMaThongTri%')
			AND (ISNULL(@iNamThongTri, '') = '' OR tbl.iNamThongTri = @iNamThongTri)
			AND (ISNULL(@dNgayThongTri , '')= '' OR CAST(tbl.dNgayThongTri as DATE) <= CAST(@dNgayThongTri as DATE))
			AND (ISNULL(@sMaDonVi, '') = '' OR dv.iID_MaDonVi = @sMaDonVi)
		) 
		
	SELECT tbl.*,
		dv.sTen as sTenDonVi,
		dv.iID_MaDonVi as sMaDonVi,
		nv.sTen as sTenNguonNganSach
	FROM VDT_ThongTri as tbl
	INNER JOIN VDT_DM_LoaiThongTri as dm on tbl.iID_LoaiThongTriID = dm.iID_LoaiThongTriID
	LEFT JOIN NS_DonVi as dv on tbl.iID_DonViID = dv.iID_Ma
	LEFT JOIN NS_NguonNganSach as nv on tbl.sMaNguonVon = nv.iID_MaNguonNganSach
	WHERE dm.iKieuLoaiThongTri = 1
			AND (ISNULL(@sMaThongTri, '') = '' OR tbl.sMaThongTri LIKE '%@sMaThongTri%')
			AND (ISNULL(@iNamThongTri, '') = '' OR tbl.iNamThongTri = @iNamThongTri)
			AND (ISNULL(@dNgayThongTri , '')= '' OR CAST(tbl.dNgayThongTri as DATE) <= CAST(@dNgayThongTri as DATE))
			AND (ISNULL(@sMaDonVi, '') = '' OR dv.iID_MaDonVi = @sMaDonVi)
	ORDER BY dv.sTen 
	OFFSET ( @ItemsPerPage * ( @CurrentPage - 1 ) ) ROWS FETCH NEXT @ItemsPerPage ROWS ONLY 
END

