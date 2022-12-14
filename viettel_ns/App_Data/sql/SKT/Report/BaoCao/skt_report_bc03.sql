USE [VIETTEL_NS1]
GO
/****** Object:  StoredProcedure [dbo].[skt_report_bc03]    Script Date: 20/07/2019 5:51:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[skt_report_bc03]
	@nam	int,
	@dvt	int,
	@nganh nvarchar(500),
	@phongban nvarchar(10)
AS
BEGIN 
	
	create table #Temp
	(
		Id_MLNhuCau uniqueidentifier, C1 decimal(18,0), C2 decimal(18,0), C3 decimal(18,0)
	)

	Insert into #Temp
	select		*
	from		(
				select	Id_MLNhuCau
						, C1 = case when @phongban <> '02' then TuChi_Bql else TuChi_B2 end
						, C2 = 0
						, C3 = 0
				from	f_skt_report_dv_ng_skt_nc(@nam,@nganh,@dvt)
				
				union all
				
				select	Id_MLNhuCau
						, C1 = 0
						, C2 = case when @phongban <> '02' then MuaHang_Bql  else MuaHang_B2 end
						, C3 = case when @phongban <> '02' then PhanCap_Bql  else PhanCap_B2 end
				from	f_skt_report_ng_skt_nc(@nam,@nganh,@dvt)	
				)re		

	select		Nganh_Parent
				, TenNganhQL = RTRIM(LTRIM(TenNganhQL))
				, Nganh
				, TenNganh = RTRIM(LTRIM(TenNganh))
				, C1 = SUM(C1)
				, C2 = SUM(C2)
				, C3 = sum(C3)
	from	
				(
				select		*
				from		#Temp
				where		C1 + C2 + C3 <> 0	
				) ct

				left join 

				(
				select	Id
						, KyHieu
						, Nganh
						, Nganh_Parent
				from	SKT_MLNhuCau
				where	NamLamViec = @nam
				) ml 
			
				on ct.Id_MLNhuCau = ml.Id
			
				left join

				(
				select	sNG, sMoTa as TenNganh
				from	NS_MucLucNganSach
				where	iNamLamViec = @nam - 1	and sLNS = ''
				) nganh

				on ml.Nganh = nganh.sNG

				left join 

				(
				select	iID_MaNganh as nganh_id
						, TenNganhQL = 'Ngành ' + sTenNganh 
				from	NS_MucLucNganSach_Nganh 
				where	iNamLamViec = @nam 
						and iTrangThai = 1
				) dv
				on dv.nganh_id= ml.Nganh_Parent	
	group by	Nganh_Parent, TenNganhQL, Nganh, TenNganh
END
