USE [VIETTEL_NS1]
GO

DECLARE	@return_value int

EXEC	@return_value = [dbo].[skt_checkmapmlns]
		@nam = 2020,
		@donvi = N'29',
		@phongban = N'07'
		
SELECT	'Return Value' = @return_value

GO
