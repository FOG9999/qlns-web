
/****** Object:  StoredProcedure [dbo].[sp_get_detail_KeHoachChiTietBQP]    Script Date: 04/11/2022 8:38:06 AM ******/
-- SET ANSI_NULLS ON
-- GO

-- SET QUOTED_IDENTIFIER ON
-- GO

-- ALTER PROC [dbo].[sp_get_detail_KeHoachChiTietBQP]
    -- @KHBQP_ID UNIQUEIDENTIFIER = NULL,
	-- @iID_BQuanLyID UNIQUEIDENTIFIER = NULL,
	-- @iID_DonViID UNIQUEIDENTIFIER = NULL,
	-- @sTenNhiemVuChi nvarchar(250),
	-- @sTenPhongBan nvarchar(250),
	-- @sTenDonVi nvarchar(250)
-- AS
-- BEGIN
	-- SELECT
		-- BQP_NVC.ID AS ID,
		-- BQP_NVC.sTenNhiemVuChi AS sTenNhiemVuChi,
		-- BQP_NVC.iID_BQuanLyID AS iID_BQuanLyID,
		-- CONCAT(RTRIM(LTRIM(PB.sTen)), IIF(PB.sMota IS NULL OR RTRIM(LTRIM(PB.sMoTa)) = '', '', CONCAT(' - ', RTRIM(LTRIM(PB.sMoTa))))) AS sTenPhongBan,
		-- BQP_NVC.iID_DonViID AS iID_DonViID,
		-- CONCAT(RTRIM(LTRIM(DV.iID_MaDonVi)), IIF(DV.sTen IS NULL OR RTRIM(LTRIM(DV.sTen)) = '', '', CONCAT(' - ', RTRIM(LTRIM(DV.sTen))))) AS sTenDonVi,
		-- DV.iID_MaDonVi AS iID_MaDonVi,
		-- TTCP_NVC.fGiaTri AS fGiaTriTTCP_USD,
		-- BQP_NVC.fGiaTriUSD AS fGiaTriBQP_USD,
		-- BQP_NVC.fGiaTriVND AS fGiaTriBQP_VND,
		-- BQP_NVC.sMaThuTu AS sMaThuTu,
		-- BQP_NVC.iID_KHTTTTCP_NhiemVuChiID AS iID_KHTTTTCP_NhiemVuChiID,
		-- BQP_NVC.iID_ParentID AS iID_ParentID,
		-- BQP_NVC.bIsTTCP AS bIsTTCP,
		-- IIF((BQP_NVC.bIsTTCP = 1 AND CheckAction.ID IS NOT NULL) OR (BQP_NVC.bIsTTCP = 0), CAST(1 AS BIT), CAST(0 AS BIT)) AS bIsAction,
		-- IIF(CheckDisable.ID IS NOT NULL, CAST(0 AS BIT), CAST(1 AS BIT)) AS bIsHasChild,
		-- CAST('/' + REPLACE(BQP_NVC.sMaThuTu, '.', '/') + '/' as HIERARCHYID) AS Sort
	-- FROM NH_KHChiTietBQP AS BQP
	-- LEFT JOIN NH_KHChiTietBQP_NhiemVuChi AS BQP_NVC ON BQP.ID = BQP_NVC.iID_KHChiTietID
	-- LEFT JOIN NH_KHTongTheTTCP_NhiemVuChi AS TTCP_NVC ON BQP_NVC.iID_KHTTTTCP_NhiemVuChiID = TTCP_NVC.ID AND BQP_NVC.bIsTTCP = 1
	-- LEFT JOIN NS_PhongBan AS PB ON BQP_NVC.iID_BQuanLyID = PB.iID_MaPhongBan
	-- LEFT JOIN NS_DonVi AS DV ON BQP_NVC.iID_DonViID = DV.iID_Ma AND BQP_NVC.iID_MaDonVi =  DV.iID_MaDonVi
	-- LEFT JOIN (SELECT ID FROM NH_KHChiTietBQP_NhiemVuChi AS NVMCheck
		-- WHERE NVMCheck.bIsTTCP = 1 AND NOT EXISTS (SELECT 1 FROM NH_KHChiTietBQP_NhiemVuChi AS NVPCheck WHERE NVPCheck.iID_ParentID = NVMCheck.ID AND NVPCheck.bIsTTCP = 1)) AS CheckAction ON BQP_NVC.ID = CheckAction.ID
	-- LEFT JOIN (SELECT ID FROM NH_KHChiTietBQP_NhiemVuChi AS NVMCheck
		-- WHERE NOT EXISTS (SELECT 1 FROM NH_KHChiTietBQP_NhiemVuChi AS NVPCheck WHERE NVPCheck.iID_ParentID = NVMCheck.ID)) AS CheckDisable ON BQP_NVC.ID = CheckDisable.ID
	-- WHERE (@KHBQP_ID IS NULL OR BQP.ID = @KHBQP_ID)
		-- AND (@iID_BQuanLyID IS NULL OR @iID_BQuanLyID = '00000000-0000-0000-0000-000000000000' OR BQP_NVC.iID_BQuanLyID = @iID_BQuanLyID)
		-- AND (@iID_DonViID IS NULL OR @iID_DonViID = '00000000-0000-0000-0000-000000000000' OR BQP_NVC.iID_DonViID = @iID_DonViID)
		-- AND (@sTenNhiemVuChi IS NULL OR @sTenNhiemVuChi ='' OR BQP_NVC.sTenNhiemVuChi like '%' + @sTenNhiemVuChi + '%')
		-- AND (@sTenPhongBan IS NULL OR @sTenPhongBan ='' OR PB.sTen like '%' + @sTenPhongBan + '%')
		-- AND (@sTenDonVi IS NULL OR @sTenDonVi ='' OR DV.sTen like '%' + @sTenDonVi + '%')
	-- ORDER BY Sort
-- END
-- GO

ALTER PROC [dbo].[sp_get_detail_KeHoachChiTietBQP]
  @KHBQP_ID UNIQUEIDENTIFIER = NULL,
	@iID_BQuanLyID UNIQUEIDENTIFIER = NULL,
	--@iID_DonViID UNIQUEIDENTIFIER = NULL,
	@sTenNhiemVuChi nvarchar(250),
	@sTenPhongBan nvarchar(250)
	--@sTenDonVi nvarchar(250)
AS
BEGIN
	SELECT
		BQP_NVC.ID AS ID,
		BQP_NVC.sTenNhiemVuChi AS sTenNhiemVuChi,
		BQP_NVC.iID_BQuanLyID AS iID_BQuanLyID,

		STUFF((
          SELECT ', ' + CONCAT(RTRIM(LTRIM(PB.sTen)), IIF(PB.sMota IS NULL OR RTRIM(LTRIM(PB.sMoTa)) = '', '', CONCAT(' - ', RTRIM(LTRIM(PB.sMoTa)))))
          FROM NH_KHChiTietBQPNhiemVuChi_PhongBan NVC_PB
		  JOIN NS_PhongBan AS PB ON PB.iID_MaPhongBan = NVC_PB.iID_NS_PhongBanID 
          WHERE BQP_NVC.ID = NVC_PB.iID_KHChiTietBQP_NhiemVuChiID
          FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, '') AS sTenPhongBan,
		--CONCAT(RTRIM(LTRIM(PB.sTen)), IIF(PB.sMota IS NULL OR RTRIM(LTRIM(PB.sMoTa)) = '', '', CONCAT(' - ', RTRIM(LTRIM(PB.sMoTa))))) AS sTenPhongBan,
		BQP_NVC.iID_DonViID AS iID_DonViID,
		CONCAT(RTRIM(LTRIM(DV.iID_MaDonVi)), IIF(DV.sTen IS NULL OR RTRIM(LTRIM(DV.sTen)) = '', '', CONCAT(' - ', RTRIM(LTRIM(DV.sTen))))) AS sTenDonVi,
		DV.iID_MaDonVi AS iID_MaDonVi,
		--TTCP_NVC.fGiaTri AS fGiaTriTTCP_USD,
		BQP_NVC.fGiaTriTTCP_USD AS fGiaTriTTCP_USD,
		BQP_NVC.fGiaTriUSD AS fGiaTriBQP_USD,
		BQP_NVC.fGiaTriVND AS fGiaTriBQP_VND,
		BQP_NVC.sMaThuTu AS sMaThuTu,
		BQP_NVC.iID_KHTTTTCP_NhiemVuChiID AS iID_KHTTTTCP_NhiemVuChiID,
		BQP_NVC.iID_ParentID AS iID_ParentID,
		BQP_NVC.bIsTTCP AS bIsTTCP,
		IIF((BQP_NVC.bIsTTCP = 1 AND CheckAction.ID IS NOT NULL) OR (BQP_NVC.bIsTTCP = 0), CAST(1 AS BIT), CAST(0 AS BIT)) AS bIsAction,
		IIF(CheckDisable.ID IS NOT NULL, CAST(0 AS BIT), CAST(1 AS BIT)) AS bIsHasChild,
		CAST('/' + REPLACE(BQP_NVC.sMaThuTu, '.', '/') + '/' as HIERARCHYID) AS Sort
	FROM NH_KHChiTietBQP AS BQP
	LEFT JOIN NH_KHChiTietBQP_NhiemVuChi AS BQP_NVC ON BQP.ID = BQP_NVC.iID_KHChiTietID
	--LEFT JOIN NH_KHTongTheTTCP_NhiemVuChi AS TTCP_NVC ON BQP_NVC.iID_KHTTTTCP_NhiemVuChiID = TTCP_NVC.ID AND BQP_NVC.bIsTTCP = 1
	--LEFT JOIN NS_PhongBan AS PB ON BQP_NVC.iID_BQuanLyID = PB.iID_MaPhongBan
	LEFT JOIN NS_DonVi AS DV ON BQP_NVC.iID_DonViID = DV.iID_Ma AND BQP_NVC.iID_MaDonVi =  DV.iID_MaDonVi
	LEFT JOIN (SELECT ID FROM NH_KHChiTietBQP_NhiemVuChi AS NVMCheck
		WHERE NVMCheck.bIsTTCP = 1 AND NOT EXISTS (SELECT 1 FROM NH_KHChiTietBQP_NhiemVuChi AS NVPCheck WHERE NVPCheck.iID_ParentID = NVMCheck.ID AND NVPCheck.bIsTTCP = 1)) AS CheckAction ON BQP_NVC.ID = CheckAction.ID
	LEFT JOIN (SELECT ID FROM NH_KHChiTietBQP_NhiemVuChi AS NVMCheck
		WHERE NOT EXISTS (SELECT 1 FROM NH_KHChiTietBQP_NhiemVuChi AS NVPCheck WHERE NVPCheck.iID_ParentID = NVMCheck.ID)) AS CheckDisable ON BQP_NVC.ID = CheckDisable.ID
	WHERE (@KHBQP_ID IS NULL OR BQP.ID = @KHBQP_ID)
		AND (@iID_BQuanLyID IS NULL OR @iID_BQuanLyID = '00000000-0000-0000-0000-000000000000' OR BQP_NVC.iID_BQuanLyID = @iID_BQuanLyID)
		--AND (@iID_DonViID IS NULL OR @iID_DonViID = '00000000-0000-0000-0000-000000000000' OR BQP_NVC.iID_DonViID = @iID_DonViID)
		AND (@sTenNhiemVuChi IS NULL OR @sTenNhiemVuChi ='' OR BQP_NVC.sTenNhiemVuChi like '%' + @sTenNhiemVuChi + '%')
		--AND (@sTenPhongBan IS NULL OR @sTenPhongBan ='' OR PB.sTen like '%' + @sTenPhongBan + '%')
		--AND (@sTenDonVi IS NULL OR @sTenDonVi ='' OR DV.sTen like '%' + @sTenDonVi + '%')
	ORDER BY Sort
END
