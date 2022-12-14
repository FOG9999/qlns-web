USE [VIETTEL_NS1]
GO
/****** Object:  UserDefinedFunction [dbo].[f_skt_report_dv_ng_skt_nc]    Script Date: 20/07/2019 5:52:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
	
	author:	hieppm
	date:	18/07/2019
	decs:	Lấy số đề nghị đơn vị theo ML nhu cầu
	params:	

*/

ALTER FUNCTION [dbo].[f_skt_report_dv_ng_skt_nc]
	(		
	@nam int,
	@nganh nvarchar(500),
	@dvt int
	)

RETURNS TABLE 
AS
	 
RETURN

select		Id_MucLuc as Id_MLNhuCau
			, TuChi_DV = ISNULL(Sum(TuChi_DV/@dvt),0)
			, TuChi_Bql = ISNULL(Sum(TuChi_Bql/@dvt),0) 
			, TuChi_B2 = ISNULL(Sum(TuChi_B2/@dvt),0)
from		(
			select	Id_MucLuc
					, TuChi_DV
					, TuChi_Bql = TuChi
					, TuChi_B2 = 0
			from	SKT_ChungTuChiTiet
			where	TuChi_DV + TuChi <> 0
					and Id_ChungTu in	(
										select	Id
										from	SKT_ChungTu
										where	NamLamViec = @nam
												and iLoai = 1
												and Id_PhongBan <> '02')

			union all

			select	Id_MucLuc
					, TuChi_DV = 0
					, TuChi_Bql = 0
					, TuChi_B2 = TuChi / @dvt
			from	SKT_ChungTuChiTiet
			where	TuChi <> 0
					and Id_ChungTu in	(
										select	Id
										from	SKT_ChungTu
										where	NamLamViec = @nam
												and iLoai = 1
												and Id_PhongBan = '02')
			) ctct					  

			right join 

			(
			select	Id
			from	SKT_MLNhuCau
			where	NamLamViec = @nam
					and (@nganh is null or Nganh in (select * from f_split(@nganh)))
			) mlnc

			on ctct.Id_MucLuc = mlnc.Id
where		Id_MucLuc is not null
group by	Id_MucLuc