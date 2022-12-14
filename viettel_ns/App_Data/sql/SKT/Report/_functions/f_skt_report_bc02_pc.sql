USE [VIETTEL_NS1]
GO
/****** Object:  UserDefinedFunction [dbo].[f_skt_report_bc02_pc]    Script Date: 20/07/2019 5:52:25 AM ******/
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

ALTER FUNCTION [dbo].[f_skt_report_bc02_pc]
	(		
	@nam int,
	@nganh nvarchar(500),
	@dvt int
	)

RETURNS TABLE 
AS
	 
RETURN

select	Id_MLNhuCau
		, DTT_2 = ISNULL(Sum(DTT_2),0)
		, DTT_1 = ISNULL(Sum(DTT_1),0)
		, NhuCau_DV = ISNULL(Sum(NhuCau_DV),0)
		, DN_Bql = ISNULL(Sum(DN_Bql),0)
		, DN_B2 = ISNULL(Sum(DN_B2),0)
		, loai
from		(	
			select		Id_MLNhuCau
						, DTT_2 = 0
						, DTT_1 = DacThu
						, NhuCau_DV = 0 
						, DN_Bql = 0 
						, DN_B2 = 0 
						, loai = 2
			from		f_skt_report_dacthu_nc(@nam,@nam-1,@nganh,null,null,@dvt,1)

			union all

			select		Id_MLNhuCau
						, DTT_2 = 0
						, DTT_1 = 0
						, NhuCau_DV = PhanCap_DV 
						, DN_Bql = PhanCap_Bql 
						, DN_B2 = PhanCap_B2 
						, loai = 2
			from		f_skt_report_ng_skt_nc(@nam,@nganh,@dvt)
			) re
group by	Id_MLNhuCau, loai