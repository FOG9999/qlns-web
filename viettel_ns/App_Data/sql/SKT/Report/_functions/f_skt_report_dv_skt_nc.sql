USE [VIETTEL_NS1]
GO
/****** Object:  UserDefinedFunction [dbo].[f_skt_report_dv_skt_nc]    Script Date: 20/07/2019 5:52:32 AM ******/
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

ALTER FUNCTION [dbo].[f_skt_report_dv_skt_nc]
	(		
	@nam int,
	@donvis nvarchar(500),
	@phongban nvarchar(10),
	@dvt int
	)

RETURNS TABLE 
AS
	 
RETURN

select		Id_MucLuc as Id_MLNhuCau
			, Id_DonVi
			, Id_PhongBan
			, TuChi_DV = ISNULL(Sum(TuChi_DV/@dvt),0)
			, TuChi_Bql = ISNULL(Sum(TuChi_Bql/@dvt),0) 
			, TuChi_B2 = ISNULL(Sum(TuChi_B2/@dvt),0)
from		(
			select	Id_MucLuc
					, Id_ChungTu
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
												and Id_PhongBan <> '02'
												and (@donvis is null or Id_DonVi in (select * from f_split(@donvis)))
												and (@phongban is null or Id_PhongBanDich = @phongban))

			union all

			select	Id_MucLuc
					, Id_ChungTu
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
												and Id_PhongBan = '02'
												and (@donvis is null or Id_DonVi in (select * from f_split(@donvis)))
												and (@phongban is null or Id_PhongBanDich = @phongban))
			) ctct

			left join 

			(
			select	Id as Id_CT
					, Id_DonVi
					, Id_PhongBan
			from	SKT_ChungTu
			where	iLoai = 1
					and NamLamViec = @nam
					and (@donvis is null or Id_DonVi in (select * from f_split(@donvis)))
					and (@phongban is null or Id_PhongBanDich = @phongban)
			) ct

			on ctct.Id_ChungTu = ct.Id_CT						  

			right join 

			(
			select	Id
			from	SKT_MLNhuCau
			where	NamLamViec = @nam
			) mlnc

			on ctct.Id_MucLuc = mlnc.Id
where		Id_MucLuc is not null
group by	Id_MucLuc, Id_DonVi, Id_PhongBan