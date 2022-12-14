USE [VIETTEL_NS1]
GO
/****** Object:  StoredProcedure [dbo].[skt_report_bc02]    Script Date: 20/07/2019 5:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[skt_report_bc02]
	@nam	int,
	@dvt	int,
	@nganh nvarchar(500),
	@loai int
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
		Id_MLNhuCau uniqueidentifier, C1  decimal(18,0), C2 decimal(18,0), C3 decimal(18,0), C4 decimal(18,0), C5 decimal(18,0), loai int
	)

	Insert into #Temp
	select		*
	from		(
				select	Id_MLNhuCau
						, C1 = DTT_2
						, C2 = DTT_1
						, C3 = NhuCau_DV
						, C4 = DN_Bql
						, C5 = DN_B2
						, loai
				from	f_skt_report_bc02_mh(@nam,@nganh,@dvt)
				
				union all
				
				select 	Id_MLNhuCau
						, C1 = DTT_2
						, C2 = DTT_1
						, C3 = NhuCau_DV
						, C4 = DN_Bql
						, C5 = DN_B2
						, loai
				from	f_skt_report_bc02_pc(@nam,@nganh,@dvt)	
				
				union all
				
				select 	Id_MLNhuCau
						, C1 = 0
						, C2 = NhuCau_DV
						, C3 = DN_Bql
						, C4 = DN_B2
						, C5 = 0
						, loai
				from	f_skt_report_bc02_tk(@nam,@nganh,@dvt)	
				)re	
	
	select Top(1)	nam
					, N_2 = CASE bangNS_2 WHEN 'QT' THEN N'QT ' + CONVERT(nvarchar(4),NamNS_2) 
										  WHEN 'DT' THEN N'DT đầu năm ' + CONVERT(nvarchar(4),NamNS_2)
										  ELSE '' END
					, N_1 = CASE bangNS_1 WHEN 'QT' THEN N'QT' + CONVERT(nvarchar(4),NamNS_1) 
										  WHEN 'DT' THEN N'DT đầu năm ' + CONVERT(nvarchar(4),NamNS_1)
										  ELSE '' END					 
	from			#TempTT

	select		KyHieu	
				, Nganh_Parent
				, TenNganhQL = RTRIM(LTRIM(TenNganhQL))
				, Nganh
				, TenNganh = RTRIM(LTRIM(TenNganh))
				, TenMuc = RTRIM(LTRIM(MoTa))
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
							and loai = @loai		
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