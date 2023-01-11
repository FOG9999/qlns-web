USE [CTC_DB_TEST]
GO

/****** Object:  StoredProcedure [dbo].[proc_get_all_duantheotrangthai_paging]    Script Date: 1/6/2023 5:58:21 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER PROC [dbo].[proc_get_all_duantheotrangthai_paging]
@sTenDuAn nvarchar(50),
@sKhoiCong nvarchar(50),
@sKetThuc nvarchar(50),
@iID_DonViQuanLyID uniqueidentifier,
@iID_CapPheDuyetID uniqueidentifier,
@iID_LoaiCongTrinhID uniqueidentifier,
@iTrangThai int,
@CurrentPage int,
@ItemsPerPage int,
@iToTalItem int OUTPUT
AS
BEGIN
SET @iToTalItem =  (SELECT COUNT(*) 
										FROM VDT_DA_DuAn 
										WHERE (ISNULL(@sTenDuAn, '') = '' OR sTenDuAn LIKE CONCAT(N'%',@sTenDuAn,N'%') OR sMaDuAn LIKE CONCAT(N'%',@sTenDuAn,N'%')) 
											AND (ISNULL(@sKhoiCong, '') = '' OR cast(@sKhoiCong as int) = 0 OR cast(sKhoiCong as int) >= cast(@sKhoiCong as int)) 
											AND (ISNULL(@sKetThuc, '') = '' OR cast(@sKetThuc as int) = 0 OR cast(sKetThuc as int) <= cast(@sKetThuc as int)) 
											AND (@iID_DonViQuanLyID is null  OR iID_DonViQuanLyID = @iID_DonViQuanLyID) 
											AND (@iID_CapPheDuyetID is null OR iID_CapPheDuyetID = @iID_CapPheDuyetID)
											AND (@iID_LoaiCongTrinhID is null OR iID_LoaiCongTrinhID = @iID_LoaiCongTrinhID)
											AND @iTrangThai = 
															CASE
																	WHEN @iTrangThai = 1 and sTrangThaiDuAn = 'KhoiTao' and (bIsKetThuc is null or bIsKetThuc != 1) THEN 1
																	WHEN @iTrangThai = 2 and sTrangThaiDuAn = 'THUC_HIEN' and (bIsKetThuc is null or bIsKetThuc != 1) THEN 2
																	WHEN @iTrangThai = 3 and bIsKetThuc = 1 THEN 3
																	ELSE 0
															END
															)

SELECT * , pcda.sTen as sTenCapPheDuyet, dv.sTen as sTenDonVi, lct.sTenLoaiCongTrinh
FROM VDT_DA_DuAn vdt_da
LEFT JOIN VDT_DM_PhanCapDuAn pcda on vdt_da.iID_CapPheDuyetID = pcda.iID_PhanCapID
LEFT JOIN NS_DonVi dv on vdt_da.iID_DonViQuanLyID = dv.iID_Ma
LEFT JOIN VDT_DM_LoaiCongTrinh lct on vdt_da.iID_LoaiCongTrinhID = lct.iID_LoaiCongTrinh
WHERE (ISNULL(@sTenDuAn, '') = '' OR sTenDuAn LIKE CONCAT(N'%',@sTenDuAn,N'%') OR sMaDuAn LIKE CONCAT(N'%',@sTenDuAn,N'%')) 
	AND (ISNULL(@sKhoiCong, '') = '' OR cast(@sKhoiCong as int) = 0 OR cast(sKhoiCong as int) >= cast(@sKhoiCong as int)) 
	AND (ISNULL(@sKetThuc, '') = '' OR cast(@sKetThuc as int) = 0 OR cast(sKetThuc as int) <= cast(@sKetThuc as int)) 
	AND (@iID_DonViQuanLyID is null  OR iID_DonViQuanLyID = @iID_DonViQuanLyID) 
	AND (@iID_CapPheDuyetID is null OR iID_CapPheDuyetID = @iID_CapPheDuyetID)
	AND (@iID_LoaiCongTrinhID is null OR iID_LoaiCongTrinhID = @iID_LoaiCongTrinhID)
	AND @iTrangThai = 
							CASE
									WHEN @iTrangThai = 1 and sTrangThaiDuAn = 'KhoiTao' and (bIsKetThuc is null or bIsKetThuc != 1) THEN 1
									WHEN @iTrangThai = 2 and sTrangThaiDuAn = 'THUC_HIEN' and (bIsKetThuc is null or bIsKetThuc != 1) THEN 2
									WHEN @iTrangThai = 3 and bIsKetThuc = 1 THEN 3
									ELSE 0
							END
ORDER BY dDateCreate desc
OFFSET (@ItemsPerPage * (@CurrentPage-1)) ROWS
FETCH NEXT @ItemsPerPage ROWS ONLY
END
GO

