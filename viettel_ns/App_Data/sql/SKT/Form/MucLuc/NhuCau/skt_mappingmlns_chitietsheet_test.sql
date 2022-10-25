USE [VIETTEL_NS1]
GO

DECLARE	@return_value int

EXEC	@return_value = [dbo].[skt_mappingmlns_chitietsheet]
		@nam = 2020,
		@loai = 1,
		@Id_MLNhuCau = 'df8479cb-b3f7-4f74-8005-ef0e9a3ce5c5',
		@LNS = NULL,
		@L = NULL,
		@K = NULL,
		@M = NULL,
		@TM = NULL,
		@TTM = NULL,
		@NG = NULL

SELECT	'Return Value' = @return_value

GO