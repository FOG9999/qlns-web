USE [viettel_ns]

--GO
--DROP FUNCTION [dbo].[F_NS_LNS_PhongBan]

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[f_ns_phongban]
(    
	  @iID_MaPhongBan nvarchar(2)
)

RETURNS uniqueidentifier  
AS

BEGIN

declare @id_phongban uniqueidentifier;

select top(1) @id_phongban=iID_MaPhongBan from NS_PhongBan
where sKyHieu=@iID_MaPhongBan;



RETURN @id_phongban
END


/*

select [dbo].f_ns_phongban('07')


*/
