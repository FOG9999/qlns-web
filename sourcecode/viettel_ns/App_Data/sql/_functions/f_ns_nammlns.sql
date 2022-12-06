USE [viettel_ns1]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER FUNCTION [dbo].[f_ns_nammlns]
(    
      @iNamLamViec		int
)

RETURNS INT
AS
BEGIN
	Declare @value int;

	select	@value = YearOfMLNS 
	from	NS_NamMLNS 
	where	WYear = @iNamLamViec

	Return @value;
END
