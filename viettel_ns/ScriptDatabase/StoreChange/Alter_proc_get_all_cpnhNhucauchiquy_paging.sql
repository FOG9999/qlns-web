USE [CTC_DB_TEST]
GO
/****** Object:  StoredProcedure [dbo].[proc_get_all_cpnhNhucauchiquy_paging]    Script Date: 29/11/2022 4:25:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[proc_get_all_cpnhNhucauchiquy_paging]
	(@sSoDeNghi nvarchar(100), 
	@dNgayDeNghi datetime,
	@iID_BQuanLyID uniqueidentifier,
	@iID_DonViID uniqueidentifier,
	@iQuy int,
	@iNamKeHoach int,
	@tabIndex int,
	@CurrentPage int,
	@ItemsPerPage int,
	@iToTalItem int OUTPUT)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT NCCQ.* , concat (b.sTen, ' - ', b.sMoTa) as BPhongBan, concat (c.sTen, ' - ', c.sMoTa) as BQuanLy, TG.sTenTiGia as sTenTiGia  into #TMP  from NH_NhuCauChiQuy NCCQ  
	inner join NS_PhongBan  b on NCCQ.iID_BQuanLyID = b.iID_MaPhongBan
	left join NH_DM_TiGia  TG on NCCQ.iID_TiGiaID = TG.ID
	left join NS_DonVi c on NCCQ.iID_DonViID = c.iID_Ma and NCCQ.iID_MaDonVi COLLATE SQL_Latin1_General_CP1_CI_AS = c.iID_MaDonVi

	where (ISNULL(@sSoDeNghi,'') = '' or NCCQ.sSoDeNghi like CONCAT(N'%',@sSoDeNghi,N'%'))
	and (@dNgayDeNghi is null or (NCCQ.dNgayDeNghi >= @dNgayDeNghi and NCCQ.dNgayDeNghi < DATEADD(day, 1, @dNgayDeNghi)))
	and (@iID_BQuanLyID is null or NCCQ.iID_BQuanLyID = @iID_BQuanLyID)
	and (@iID_DonViID is null or NCCQ.iID_DonViID = @iID_DonViID) 
	and (ISNULL(@iQuy,'') = '' or @iQuy = 0 or NCCQ.iQuy = @iQuy)
	and (ISNULL(@iNamKeHoach,'') = '' or NCCQ.iNamKeHoach = @iNamKeHoach)
	and ( @tabIndex = 0 and sTongHop is null and iID_TongHopID is null) or (@tabIndex = 1)

	order by dNgayTao desc, sSoDeNghi

	SET @iToTalItem =  (SELECT COUNT(*) from #TMP);

	SELECT *
	FROM #TMP
	ORDER BY dNgayTao desc, sSoDeNghi
	OFFSET (@ItemsPerPage * (@CurrentPage-1)) ROWS
	FETCH NEXT @ItemsPerPage ROWS ONLY

	DROP TABLE #TMP
END
