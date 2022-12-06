USE [viettel_ns]
GO

/****** Object:  UserDefinedFunction [dbo].[F_NS_DTBS_ChungTuDaGom_DenDot]    Script Date: 27/03/2018 2:51:51 CH ******/
--DROP FUNCTION [dbo].[F_NS_DTBS_ChungTuDaGom_DenDot]
GO

/****** Object:  UserDefinedFunction [dbo].[F_NS_DTBS_ChungTuDaGom_DenDot]    Script Date: 27/03/2018 2:51:51 CH ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

alter FUNCTION [dbo].[f_ns_dtbs_dot_mota]
(    
      @iNamLamViec int,
	  @iID_MaPhongBan nvarchar(2),
	  @username nvarchar(50),
	  @dNgayChungTu datetime,
	  @sLNS varchar(100)
)

RETURNS nvarchar(2000)
AS

BEGIN


declare @results nvarchar(2000);


select @results = coalesce(@results + '; ', '') + sNoiDung
from DTBS_ChungTu
where	iTrangThai=1
		and iNamLamViec=@iNamLamViec
		and iID_MaPhongBan=@iID_MaPhongBan
		and sID_MaNguoiDungTao=@username
		and dNgayChungTu=@dNgayChungTu
RETURN @results

END

/*

SELECT [dbo].f_ns_dtbs_dot_mota(2018,'10','tranhnh','2018-04-17')

*/

GO
