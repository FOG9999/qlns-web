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

CREATE FUNCTION [dbo].[F_NS_DTBS_ChungTuDaGom_DenDot]
(    
      @iNamLamViec int,
	  @iID_MaPhongBan nvarchar(2),
	  @dNgay datetime
)

RETURNS @Output TABLE  
(
	id NVARCHAR(36) 
)
AS

BEGIN


declare @results varchar(2000);

if (@dNgay IS NULL) 
set @dNgay=GETDATE()


select @results = coalesce(@results + ',', '') +  iID_MaChungTu
from DTBS_ChungTu_TLTH
where	iTrangThai=1
		and iNamLamViec=@iNamLamViec
		and iID_MaPhongBan=@iID_MaPhongBan
		and dNgayChungTu<=@dNgay

INSERT @Output (id)
select distinct * from F_Split(@results)

RETURN

END

/*

SELECT * FROM F_NS_DTBS_ChungTuDaGom(2018,'07')

*/

GO
