
ALTER FUNCTION [dbo].[F_MoTa_sXauNoiMa]
(
	@iNamLamViec int,
	@sXauNoiMa nvarchar(31)
)

/*
TEST

-- theo don vi

SELECT [dbo].F_MLNS_MoTa_LNS('2018', '2040200','070','083','8000','8049','90','00')

1	104	10403	1040300	010	011	7000	7012	00	08
-- tat ca don vi
SELECT * FROM F_NS_ChiTieu_TheoDonVi(2018, '44', '07', '2')

-- theo nam ngan sach
SELECT * FROM F_NS_ChiTieu_TheoDonVi(2018, '44', '07', '1,2')

*/ 


RETURNS NVARCHAR(200)
AS
BEGIN
	Declare @value nvarchar(200);

	SELECT	@value = sMoTa FROM NS_MucLucNganSach
	WHERE	iNamLamViec=@iNamLamViec
			AND sXauNoiMa = @sXauNoiMa
	Return @value;
END
