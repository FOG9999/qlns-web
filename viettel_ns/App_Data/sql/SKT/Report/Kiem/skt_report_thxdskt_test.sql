USE [VIETTEL_NS1]
GO

DECLARE	@return_value int

EXEC	@return_value = [dbo].[skt_report_thxdskt]
		@nam = 2020,
		@dvt = 1000,
		@donvis = 96,
		@phongban = N'07'
		
SELECT	'Return Value' = @return_value

GO
