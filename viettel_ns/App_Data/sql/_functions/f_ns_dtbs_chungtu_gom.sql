USE [viettel_ns]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER FUNCTION [dbo].[f_ns_dtbs_chungtu_gom]
(    
      @iNamLamViec int,
	  @iID_MaPhongBan nvarchar(2)
)

RETURNS nvarchar(MAX)
AS

BEGIN

declare @results varchar(MAX);

select @results = coalesce(@results + ',', '') +  iID_MaChungTu
from DTBS_ChungTu_TLTH
where	iTrangThai=1
		and iNamLamViec=@iNamLamViec
		and iID_MaPhongBan=@iID_MaPhongBan

		
return @results;


END

/*


SELECT [dbo].f_ns_dtbs_chungtu_gom(2018,'07')

*/

GO
