
GO
/****** Object:  StoredProcedure [dbo].[proc_get_all_kehoachvonnamduocduyet_paging]    Script Date: 25/10/2022 1:39:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROC [dbo].[proc_get_all_kehoachvonnamduocduyet_paging]
        @sSoQuyetDinh nvarchar(100),
        @dNgayQuyetDinhFrom datetime,
        @dNgayQuyetDinhTo datetime,
        @iNamKeHoach int,
        @iID_NguonVonID int,
        @iID_DonViQuanLyID uniqueidentifier,
        @CurrentPage int,
        @ItemsPerPage int,
        @iToTalItem int OUTPUT
AS
BEGIN
        WITH SoLieuDieuChinh AS 
         (
                SELECT 
                        ct.iID_KeHoachVonNam_DuocDuyetID, ct.iID_ParentId, ct.sSoQuyetDinh, ct.iNamKeHoach
                FROM 
                        VDT_KHV_KeHoachVonNam_DuocDuyet ct 
                WHERE 
                        ct.iID_ParentId is not null

                UNION ALL

                SELECT ct.iID_KeHoachVonNam_DuocDuyetID, ct.iID_ParentId, ct.sSoQuyetDinh, ct.iNamKeHoach
                FROM VDT_KHV_KeHoachVonNam_DuocDuyet ct JOIN SoLieuDieuChinh ctpr ON ct.iID_ParentId = ctpr.iID_KeHoachVonNam_DuocDuyetID 
                WHERE ct.iID_ParentId is not null
          ),SoLanDieuChinh AS (
                   SELECT 
                        sdc.iID_ParentId, sdc.iNamKeHoach, COUNT(sdc.iID_KeHoachVonNam_DuocDuyetID) AS iSoLanDieuChinh
                  FROM 
                        SoLieuDieuChinh sdc
                  GROUP BY sdc.iID_ParentId, sdc.iNamKeHoach
          ), TinhTongChiTietDC as(
		   SELECT 
				khvnam.iID_KeHoachVonNam_DuocDuyetID, SUM(ISNULL(khvndxct.fCapPhatBangLenhChiDC, 0)) as fCapPhatBangLenhChiDC, SUM(ISNULL(khvndxct.fCapPhatTaiKhoBacDC, 0)) as  fCapPhatTaiKhoBacDC   
		   
		   FROM 
				VDT_KHV_KeHoachVonNam_DuocDuyet khvnam
		   LEFT JOIN 
				VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet khvndxct on khvndxct.iID_KeHoachVonNam_DuocDuyetID = khvnam.iID_KeHoachVonNam_DuocDuyetID
			
		   GROUP BY khvnam.iID_KeHoachVonNam_DuocDuyetID
		  )


        SELECT 
                khvnam.*, 
                donVi.sTen AS sTenDonVi, 
                nv.sTen AS sTenNguonVon, 
                khvnamddpr.sSoQuyetDinh as DieuChinhTu,
                ('(' + cast(isnull(tmpdc.iSoLanDieuChinh, 0) as nvarchar(max)) + ')') as SoLanDieuChinh,
				ctdc.fCapPhatBangLenhChiDC,
				ctdc.fCapPhatTaiKhoBacDC
        INTO #TMP
        FROM VDT_KHV_KeHoachVonNam_DuocDuyet khvnam
        LEFT JOIN NS_DonVi donVi ON khvnam.iID_DonViQuanLyID = donVi.iID_Ma
        LEFT JOIN NS_NguonNganSach nv ON khvnam.iID_NguonVonID = nv.iID_MaNguonNganSach
        LEFT JOIN VDT_KHV_KeHoachVonNam_DuocDuyet khvnamddpr ON khvnam.iID_ParentId = khvnamddpr.iID_KeHoachVonNam_DuocDuyetID
        LEFT JOIN SoLanDieuChinh tmpdc ON khvnam.iID_ParentId = tmpdc.iID_ParentId
		LEFT JOIN TinhTongChiTietDC ctdc ON CTDC.iID_KeHoachVonNam_DuocDuyetID = khvnam.iID_KeHoachVonNam_DuocDuyetID
        WHERE 
                (ISNULL(@sSoQuyetDinh, '') = '' OR khvnam.sSoQuyetDinh LIKE CONCAT(N'%',@sSoQuyetDinh,N'%'))
                AND (@dNgayQuyetDinhFrom IS NULL OR khvnam.dNgayQuyetDinh >= @dNgayQuyetDinhFrom)
                AND (@dNgayQuyetDinhTo IS NULL OR khvnam.dNgayQuyetDinh <= @dNgayQuyetDinhTo)
                AND (@iNamKeHoach IS NULL OR khvnam.iNamKeHoach = @iNamKeHoach)
                AND (@iID_NguonVonID IS NULL OR khvnam.iID_NguonVonID = @iID_NguonVonID)
                AND (@iID_DonViQuanLyID IS NULL OR @iID_DonViQuanLyID = '00000000-0000-0000-0000-000000000000' OR khvnam.iID_DonViQuanLyID = @iID_DonViQuanLyID)
        ORDER BY dDateCreate desc

        declare @soLanDieuChinh int = 0;
        
        SET @iToTalItem =  (SELECT COUNT(*) from #TMP);
        SELECT *, '' as STT
        FROM #TMP
        ORDER BY dDateCreate desc
        OFFSET (@ItemsPerPage * (@CurrentPage-1)) ROWS
        FETCH NEXT @ItemsPerPage ROWS ONLY

        DROP TABLE #TMP
END
