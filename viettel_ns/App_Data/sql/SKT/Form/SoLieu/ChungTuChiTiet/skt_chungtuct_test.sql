USE [VIETTEL_NS1]
GO

DECLARE	@return_value int

EXEC	@return_value = [dbo].[skt_chungtuct]
		@id = 'e0b4e411-0682-442e-bb82-b0d0cf735981',
		@dvt = 1000,
		@Nganh = NULL,
		@Nganh_Parent = null

SELECT	'Return Value' = @return_value

GO


