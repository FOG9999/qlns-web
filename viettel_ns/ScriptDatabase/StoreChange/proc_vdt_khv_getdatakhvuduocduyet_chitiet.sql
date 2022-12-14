
GO
/****** Object:  StoredProcedure [dbo].[proc_vdt_khv_getdatakhvuduocduyet_chitiet]    Script Date: 01/11/2022 8:54:54 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,DungNV>
-- Create date: <27/09 Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[proc_vdt_khv_getdatakhvuduocduyet_chitiet]
	-- Add the parameters for the stored procedure here
	@listDuAnIds nvarchar(max), 
	@iID_KeHoachVDuocDuyetId uniqueidentifier,
	@iId_KHVUDX_ID uniqueidentifier
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DECLARE @iIdKeHoachVUChiTiet uniqueidentifier = (SELECT Id FROM VDT_KHV_KeHoachVonUng_ChiTiet WHERE iID_KeHoachUngID = @iID_KeHoachVDuocDuyetId)
	DECLARE @iIdKeHoachDeXuat uniqueidentifier = (SELECT iID_KeHoachVonUngDeXuatID FROM VDT_KHV_KeHoachVonUng WHERE Id = @iID_KeHoachVDuocDuyetId)

	IF @iIdKeHoachVUChiTiet IS NULL -- Data tao moi
		BEGIN			
			SELECT  0 as sXauNoiMa, da.iID_DuAnID,da.sTenDuAn, da.sMaDuAn, da.sTrangThaiDuAn as sTrangThaiDuAnDangKy, dx.fGiaTriUng as fGiaTriDeNghi, da.fTongMucDauTu
			from VDT_DA_DuAn as da 
			left join VDT_KHV_KeHoachVonUng_DX_ChiTiet ct on ct.iID_DuAnID = da.iID_DuAnID
			inner join VDT_KHV_KeHoachVonUng_DX dx on dx.Id = ct.iID_KeHoachUngID
			where da.iID_DuAnID in (select * from dbo.f_split(@listDuAnIds)) and dx.Id = @iId_KHVUDX_ID
		END
	ELSE
		BEGIN
			SELECT iID_DuAnID, SUM(ISNULL(fGiaTriDeNghi, 0)) as fGiaTriDeNghi INTO #tmpDeXuat 
			FROM VDT_KHV_KeHoachVonUng_DX_ChiTiet WHERE iID_KeHoachUngID = @iIdKeHoachDeXuat
			GROUP BY iID_DuAnID

			SELECT dt.Id,dt.iID_KeHoachUngID,ml.sXauNoiMa, dt.iID_DuAnID, dt.iID_MucID, dt.iID_TieuMucID, dt.iID_TietMucID, dt.iID_NganhID, 
					da.sTenDuAn, da.sMaDuAn, dt.sTrangThaiDuAnDangKy, dx.fGiaTriDeNghi, da.fTongMucDauTu, dt.fCapPhatTaiKhoBac, dt.fCapPhatBangLenhChi, dt.sGhiChu,
					ml.sLNS, ml.sL, ml.sK, ml.sM, ml.sTM, ml.sTTM, ml.sNG,dt.fCapPhatBangLenhChiDC,dt.fCapPhatTaiKhoBacDC
			FROM  VDT_KHV_KeHoachVonUng_ChiTiet as dt
			INNER JOIN VDT_DA_DuAn as da on dt.iID_DuAnID = da.iID_DuAnID
			LEFT JOIN #tmpDeXuat as dx on dt.iID_DuAnID = dx.iID_DuAnID
			LEFT JOIN NS_MucLucNganSach as ml on dt.iID_MucID = ml.iID_MaMucLucNganSach
													OR dt.iID_TieuMucID = ml.iID_MaMucLucNganSach
													OR dt.iID_TietMucID = ml.iID_MaMucLucNganSach
													OR dt.iID_NganhID = ml.iID_MaMucLucNganSach
			where dt.iID_KeHoachUngID = @iID_KeHoachVDuocDuyetId
		END
END
