USE [VIETTEL_NS1]
GO
/****** Object:  StoredProcedure [dbo].[skt_report_bc01]    Script Date: 20/07/2019 5:48:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[skt_report_bc01]
	@nam	int,
	@dvt	int,
	@donvi	nvarchar(500),
	@phongban nvarchar(10)
AS
BEGIN 
		
	create table #TempTT
	(
		 nam int, NamNS_1 int, bangNS_1 nvarchar(2), NamNS_2 int, bangNS_2 nvarchar(2)  		 
	) 

	insert into #TempTT(nam,NamNS_1,NamNS_2)
	values (@nam,@nam-1,@nam-2)

	update #TempTT
	set bangNS_1 = (select top(1) NamNS_1 from SKT_MapDataNS where nam = NamLamViec),
	bangNS_2 = (select top(1) NamNS_2 from SKT_MapDataNS where nam = NamLamViec)

	create table #Temp
	(
		Id_MLNhuCau uniqueidentifier, C1  decimal(18,0), C2 decimal(18,0), C3 decimal(18,0), C4 decimal(18,0), C5 decimal(18,0)
	)

	Insert into #Temp
	select		*
	from		(
				select		Id_MLNhuCau
							, C1 = sum(TuChi)
							, C2 = 0 
							, C3 = 0
							, C4 = 0
							, C5 = 0
				from		f_skt_report_thnhucau_nt(@nam,@donvi,@phongban,@dvt)	
				where		TuChi <> 0 and NamNS = @nam - 2
				group by	Id_MLNhuCau

				union all

				select		Id_MLNhuCau
							, C1 = 0
							, C2 = sum(TuChi)
							, C3 = 0
							, C4 = 0
							, C5 = 0
				from		f_skt_report_thnhucau_nt(@nam,@donvi,@phongban,@dvt)	
				where		TuChi <> 0 and NamNS = @nam - 1
				group by	Id_MLNhuCau
				
				union all

				select		Id_MLNhuCau
							, C1 = 0
							, C2 = 0
							, C3 = sum(TuChi_DV)
							, C4 = sum(TuChi_Bql)
							, C5 = sum(TuChi_B2)
				from		f_skt_report_dv_skt_nc(@nam,@donvi,@phongban,@dvt)
				where		TuChi_DV + TuChi_Bql + TuChi_B2 <> 0
				group by	Id_MLNhuCau
				)re	
	
	select Top(1)	nam
					, N_2 = CASE bangNS_2 WHEN 'QT' THEN N'QT ' + CONVERT(nvarchar(4),NamNS_2) 
										  WHEN 'DT' THEN N'DT đầu năm ' + CONVERT(nvarchar(4),NamNS_2)
										  ELSE '' END
					, N_1 = CASE bangNS_1 WHEN 'QT' THEN N'QT' + CONVERT(nvarchar(4),NamNS_1) 
										  WHEN 'DT' THEN N'DT đầu năm ' + CONVERT(nvarchar(4),NamNS_1)
										  ELSE '' END					 
	from			#TempTT

	select		X1=SUBSTRING(KyHieu,1,1)
				, X2=SUBSTRING(KyHieu,1,3)
				, X3=SUBSTRING(KyHieu,1,6)
				, X4=SUBSTRING(KyHieu,1,9)
				, KyHieu					
				, MoTa = RTRIM(LTRIM(MoTa))
				, C1 = SUM(C1)
				, C2 = SUM(C2)
				, C3 = sum(C3)
				, C4 = sum(C4)
				, C5 = sum(C5)
	from	
				(
				select		*
				from		#Temp
				where		C1 + C2 + C3 + C4 + C5<> 0	
				) ct

				left join 

				(
				select	Id
						, KyHieu
						, Nganh
						, Nganh_Parent
						, MoTa
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
	group by	Nganh_Parent, TenNganhQL, Nganh, TenNganh, KyHieu, MoTa
END