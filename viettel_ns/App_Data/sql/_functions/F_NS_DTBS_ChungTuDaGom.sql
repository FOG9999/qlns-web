USE [viettel_ns]
GO

/****** Object:  UserDefinedFunction [dbo].[F_NS_DTBS_ChungTuDaGom]    Script Date: 27/03/2018 2:50:47 CH ******/
DROP FUNCTION [dbo].[F_NS_DTBS_ChungTuDaGom]
GO

/****** Object:  UserDefinedFunction [dbo].[F_NS_DTBS_ChungTuDaGom]    Script Date: 27/03/2018 2:50:47 CH ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[F_NS_DTBS_ChungTuDaGom]
(    
      @iNamLamViec int,
	  @iID_MaPhongBan nvarchar(2)
)

RETURNS TABLE
AS
RETURN
select * from F_NS_DTBS_ChungTuDaGom_DenDot(@iNamLamViec,@iID_MaPhongBan,NULL)
 
/*

SELECT * FROM F_NS_DTBS_ChungTuDaGom(2018,'07')

*/
GO
