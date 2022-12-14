USE [VIETTEL_NS1]
GO
/****** Object:  UserDefinedFunction [dbo].[f_skt_report_ng_skt_nc]    Script Date: 20/07/2019 5:52:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
	
	author:	hieppm
	date:	18/07/2019
	decs:	Lấy số đề nghị ngành theo ML nhu cầu
	params:	

*/

ALTER FUNCTION [dbo].[f_skt_report_ng_skt_nc]
	(		
	@nam int,
	@nganh nvarchar(500),
	@dvt int
	)

RETURNS TABLE 
AS
	 
RETURN

select		Id_MucLuc as Id_MLNhuCau
			, TonKho_DV = ISNULL(Sum(TonKho_DV/@dvt),0)
			, HuyDong_DV = ISNULL(Sum(HuyDong_DV/@dvt),0) 
			, HuyDong_Bql = ISNULL(Sum(HuyDong_Bql/@dvt),0)
			, HuyDong_B2 = ISNULL(Sum(HuyDong_B2/@dvt),0)
			, MuaHang_Dv = ISNULL(Sum(MuaHang_Dv/@dvt),0)
			, MuaHang_Bql = ISNULL(Sum(MuaHang_Bql/@dvt),0)
			, MuaHang_B2 = ISNULL(Sum(MuaHang_B2/@dvt),0)
			, PhanCap_DV = ISNULL(Sum(PhanCap_DV/@dvt),0)
			, PhanCap_Bql = ISNULL(Sum(PhanCap_Bql/@dvt),0)
			, PhanCap_B2 = ISNULL(Sum(PhanCap_B2/@dvt),0)
from		(
			select	Id_MucLuc
					, TonKho_DV 
					, HuyDong_DV 
					, HuyDong_Bql = HuyDong
					, HuyDong_B2 = 0
					, MuaHang_Dv
					, MuaHang_Bql = MuaHang
					, MuaHang_B2 = 0
					, PhanCap_DV
					, PhanCap_Bql = PhanCap
					, PhanCap_B2 = 0
			from	SKT_ChungTuChiTiet
			where	TonKho_DV + HuyDong + HuyDong_DV  + MuaHang + MuaHang_Dv + PhanCap + PhanCap_DV <> 0
					and Id_ChungTu in	(
										select	Id
										from	SKT_ChungTu
										where	NamLamViec = @nam
												and iLoai = 2
												and Id_PhongBan <> '02')

			union all

			select	Id_MucLuc
					, TonKho_DV = 0
					, HuyDong_DV = 0 
					, HuyDong_Bql = 0
					, HuyDong_B2 = HuyDong
					, MuaHang_Dv = 0
					, MuaHang_Bql = 0
					, MuaHang_B2 = MuaHang
					, PhanCap_DV = 0
					, PhanCap_Bql = 0
					, PhanCap_B2 = PhanCap
			from	SKT_ChungTuChiTiet
			where	HuyDong + MuaHang + PhanCap <> 0
					and Id_ChungTu in	(
										select	Id
										from	SKT_ChungTu
										where	NamLamViec = @nam
												and iLoai = 2
												and Id_PhongBan = '02')
			) ctct

			right join 

			(
			select	Id
			from	SKT_MLNhuCau
			where	NamLamViec = @nam
					and (@nganh is null or Nganh in (select * from f_split(@nganh)))
					and Nganh <> '00'
			) mlnc

			on ctct.Id_MucLuc = mlnc.Id
where		Id_MucLuc is not null
group by	Id_MucLuc