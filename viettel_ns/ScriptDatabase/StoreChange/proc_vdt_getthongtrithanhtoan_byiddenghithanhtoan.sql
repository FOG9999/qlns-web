USE [CTC_DB_TEST]
GO

/****** Object:  StoredProcedure [dbo].[proc_vdt_getthongtrithanhtoan_byiddenghithanhtoan]    Script Date: 12/28/2022 9:59:56 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[proc_vdt_getthongtrithanhtoan_byiddenghithanhtoan]
--declare
	-- Add the parameters for the stored procedure here
	@listIds varchar(max)
	as begin


	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT DISTINCT
	dn.iID_DeNghiThanhToanID as IIdDeNghiThanhToanId,
	dn.iID_DuAnId as IIdDuAnId,
	da.sTenDuAn as STenDuAn,
	lct.iID_LoaiCongTrinh as IIdLoaiCongTrinhId,
	lct.sTenLoaiCongTrinh as STenLoaiCongTrinh,
	dn.iID_MaDonViQuanLy,
	dn.iID_DonViQuanLyID,
	dn.iID_LoaiNguonVonID as IIdLoaiNguonVonId,
	pd.sM as SM,
	pd.sTM as STm,
	pd.sTTM as STtm,
	pd.sNG as sNg,
	--(ISNULL(pd.fGiaTriThanhToanNN,0) + ISNULL(pd.fGiaTriThanhToanTN,0)) as FSoTien,
	(
		case when dn.iLoaiThanhToan = 0 then (ISNULL(pd.fGiaTriThanhToanNN,0) + ISNULL(pd.fGiaTriThanhToanTN,0)) -- loại tạm ứng
		when ISNULL(pd.fGiaTriThanhToanNN, 0) != 0 OR ISNULL(pd.fGiaTriThanhToanTN, 0) != 0 THEN ISNULL(pd.fGiaTriThanhToanNN,0) + ISNULL(pd.fGiaTriThanhToanTN,0) -- thanh toán
		when ISNULL(pd.fGiaTriThuHoiNamTruocNN, 0) != 0 OR ISNULL(pd.fGiaTriThuHoiNamTruocTN, 0) != 0 THEN (ISNULL(pd.fGiaTriThuHoiNamTruocTN,0) + ISNULL(pd.fGiaTriThuHoiNamTruocNN,0)) -- thu hồi năm trước chuyển sang
		when ISNULL(pd.fGiaTriThuHoiNamNayNN, 0) != 0 OR ISNULL(pd.fGiaTriThuHoiNamNayTN, 0) != 0 THEN (ISNULL(pd.fGiaTriThuHoiNamNayTN,0) + ISNULL(pd.fGiaTriThuHoiNamNayNN,0)) -- thu hồi năm nay
		end
	) as FSoTien,
	pd.iID_MucID as IIdMucId,
	pd.iID_NganhID as IIdNganhId,
	pd.iID_TieuMucID as IIdTieuMucId,
	pd.iID_TietMucID as IIdTietMucId,
	dn.iID_NhaThauID as IIdNhaThauId,
	da.iID_CapPheDuyetID as IIdCapPheDuyetId,
	dn.iLoaiThanhToan as iLoaiThanhToan,
	(CASE 
				WHEN dn.iLoaiThanhToan = 0 THEN 2 
				WHEN ISNULL(pd.fGiaTriThanhToanNN, 0) != 0 OR ISNULL(pd.fGiaTriThanhToanTN, 0) != 0 THEN 1
				WHEN ISNULL(pd.fGiaTriThuHoiNamTruocNN, 0) != 0 OR ISNULL(pd.fGiaTriThuHoiNamTruocTN, 0) != 0 THEN 4
				WHEN ISNULL(pd.fGiaTriThuHoiNamNayNN, 0) != 0 OR ISNULL(pd.fGiaTriThuHoiNamNayTN, 0) != 0 THEN 5
				WHEN ISNULL(pd.fGiaTriThuHoiUngTruocNamTruocNN, 0) != 0 OR ISNULL(pd.fGiaTriThuHoiUngTruocNamTruocTN, 0) != 0 THEN 6
				WHEN ISNULL(pd.fGiaTriThuHoiUngTruocNamNayNN, 0) != 0 OR ISNULL(pd.fGiaTriThuHoiUngTruocNamNayTN, 0) != 0 THEN 7
				ELSE 0 
			END) as iLoaiDeNghi

		
	FROM VDT_TT_DeNghiThanhToan dn
	LEFT JOIN VDT_TT_PheDuyetThanhToan_ChiTiet pd on dn.iID_DeNghiThanhToanID = pd.iID_DeNghiThanhToanID
	LEFT JOIN VDT_DA_DuAn da on da.iID_DuAnID = dn.iID_DuAnId
	LEFT JOIN VDT_DM_LoaiCongTrinh as lct on da.iID_LoaiCongTrinhID = lct.iID_LoaiCongTrinh
	WHERE dn.iID_DeNghiThanhToanID IN (SELECT * FROM f_split(@listIds))



	end
	--select * from VDT_TT_PheDuyetThanhToan_ChiTiet where iID_DeNghiThanhToanID = 'a8b55ec5-521d-4bd8-8ba0-af7100b6fb4f'
GO

