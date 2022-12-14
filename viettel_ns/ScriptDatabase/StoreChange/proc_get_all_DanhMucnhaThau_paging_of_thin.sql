USE [CTC_DB_TEST]
GO
/****** Object:  StoredProcedure [dbo].[proc_get_all_DanhMucnhaThau_paging_of_thin]    Script Date: 09/11/2022 2:21:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:    <Author,,Name>
-- Create date: <Create Date,,>
-- Description:  <Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[proc_get_all_DanhMucnhaThau_paging_of_thin]
 @sMaChuDauTu nvarchar(50),
@sTenChuDauTu nvarchar(500),
@iNamLamViec int,
@CurrentPage int,
@ItemsPerPage int,
@iToTalItem int OUTPUT
AS
BEGIN
  -- SET NOCOUNT ON added to prevent extra result sets from
  -- interfering with SELECT statements.
  SET NOCOUNT ON;

    -- Insert statements for procedure here
  SELECT cdt.*,ListNameParent.sTenCDT as sTenChuDauTuCha into #TMP
	FROM DM_ChuDauTu cdt
	left join DM_ChuDauTu ListNameParent on cdt.Id_Parent=ListNameParent.ID
	WHERE 
		((ISNULL(@sMaChuDauTu, '') = '' or cdt.sId_CDT like CONCAT(N'%',@sMaChuDauTu,N'%'))
    and (ISNULL(@sTenChuDauTu, '') = '' or cdt.sTenCDT like CONCAT(N'%',@sTenChuDauTu,N'%'))
  and cdt.iNamLamViec=@iNamLamViec)
  SET @iToTalItem =  (SELECT COUNT(*) from #TMP);

  SELECT *
  FROM #TMP
  order by dDateCreated desc
  OFFSET (@ItemsPerPage * (@CurrentPage-1)) ROWS
  FETCH NEXT @ItemsPerPage ROWS ONLY

  DROP TABLE #TMP
END