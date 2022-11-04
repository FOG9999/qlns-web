/****** Object:  StoredProcedure [dbo].[sp_get_detail_KeHoachTongTheTTCP]    Script Date: 04/11/2022 8:38:55 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER PROC [dbo].[sp_get_detail_KeHoachTongTheTTCP]
	@KHTTCP_ID UNIQUEIDENTIFIER = NULL,
	@iID_BQuanLyID UNIQUEIDENTIFIER = NULL,
	@sTenNhiemVuChi nvarchar(250),
	@sTenPhongBan nvarchar(250)
AS
BEGIN
	SELECT
		TTCP_NVC.ID AS ID,
		TTCP_NVC.sTenNhiemVuChi AS sTenNhiemVuChi,
		TTCP_NVC.iID_BQuanLyID AS iID_BQuanLyID,
		CONCAT(RTRIM(LTRIM(PB.sTen)), IIF(PB.sMota IS NULL OR RTRIM(LTRIM(PB.sMoTa)) = '', '', CONCAT(' - ', RTRIM(LTRIM(PB.sMoTa))))) AS sTenPhongBan,
		TTCP_NVC.fGiaTri AS fGiaTri,
		TTCP_NVC.sMaThuTu AS sMaThuTu,
		TTCP_NVC.iID_ParentID AS iID_ParentID,
		IIF(CheckDisable.ID IS NOT NULL, CAST(0 AS BIT), CAST(1 AS BIT)) AS bIsHasChild,
		CAST('/' + REPLACE(TTCP_NVC.sMaThuTu, '.', '/') + '/' as HIERARCHYID) AS Sort
	FROM NH_KHTongTheTTCP_NhiemVuChi AS TTCP_NVC
	LEFT JOIN NS_PhongBan AS PB ON TTCP_NVC.iID_BQuanLyID = PB.iID_MaPhongBan
	LEFT JOIN (SELECT ID FROM NH_KHTongTheTTCP_NhiemVuChi AS NVMCheck
		WHERE NOT EXISTS (SELECT 1 FROM NH_KHTongTheTTCP_NhiemVuChi AS NVPCheck WHERE NVPCheck.iID_ParentID = NVMCheck.ID)) AS CheckDisable ON TTCP_NVC.ID = CheckDisable.ID
	WHERE (@KHTTCP_ID IS NULL OR TTCP_NVC.iID_KHTongTheID = @KHTTCP_ID)
		AND (@iID_BQuanLyID IS NULL OR @iID_BQuanLyID = '00000000-0000-0000-0000-000000000000' OR TTCP_NVC.iID_BQuanLyID = @iID_BQuanLyID)
		AND (@sTenNhiemVuChi IS NULL OR TTCP_NVC.sTenNhiemVuChi like '%' + @sTenNhiemVuChi + '%')
		AND (@sTenPhongBan IS NULL OR PB.sTen like '%' + @sTenPhongBan + '%')
	ORDER BY Sort
END
GO

