USE [VIETTEL_NS1]
GO
/****** Object:  UserDefinedFunction [dbo].[f_skt_report_bc02_tk]    Script Date: 20/07/2019 5:52:26 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
	
	author:	longsam
	date:	09/05/2018
	decs:	Lấy tổng chỉ tiêu ngân sách 
	params:	

*/

ALTER FUNCTION [dbo].[f_skt_report_bc02_tk]
	(		
	@nam int,
	@nganh nvarchar(500),
	@dvt int
	)

RETURNS TABLE 
AS
	 
RETURN

select	Id_MLNhuCau
		, TonKho = ISNULL(Sum(TonKho),0)
		, NhuCau_DV = ISNULL(Sum(NhuCau_DV),0)
		, DN_Bql = ISNULL(Sum(DN_Bql),0)
		, DN_B2 = ISNULL(Sum(DN_B2),0)
		, loai
from		(				
			select		Id_MLNhuCau
						, TonKho = TonKho_DV
						, NhuCau_DV = HuyDong_DV 
						, DN_Bql = HuyDong_Bql 
						, DN_B2 = HuyDong_B2 
						, loai = 3
			from		f_skt_report_ng_skt_nc(@nam,@nganh,@dvt)
			) re
group by	Id_MLNhuCau, loai