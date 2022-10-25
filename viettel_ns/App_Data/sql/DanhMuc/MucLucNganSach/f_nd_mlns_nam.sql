USE [VIETTEL_NS1]
GO

/****** Object:  UserDefinedFunction [dbo].[f_nd_mlns_nam]    Script Date: 28/07/2019 10:54:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
	
	author:	hieppm
	date:	28/07/2019
	decs:	Lấy mô tả mục lục ngân sách theo năm và xâu nối mã
	params:	lns	 
*/

ALTER FUNCTION [dbo].[f_nd_mlns_nam]
(
	@iNamLamViec int,
	@sXauNoiMa nvarchar(31)
)

RETURNS NVARCHAR(MAX)
AS
BEGIN
	Declare @value nvarchar(MAX);

	SELECT	@value = sMoTa FROM NS_MucLucNganSach
	WHERE	iNamLamViec=@iNamLamViec
			AND sXauNoiMa = LTRIM(RTRIM(@sXauNoiMa))
	Return @value;
END
