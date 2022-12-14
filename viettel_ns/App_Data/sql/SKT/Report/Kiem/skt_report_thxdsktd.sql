USE [VIETTEL_NS1]
GO
/****** Object:  StoredProcedure [dbo].[skt_report_thxdskt]    Script Date: 11/07/2019 7:56:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[skt_report_thxdsktd]
	@nam	int,
	@dvt	int,
	@phongban nvarchar(3),
	@donvi nvarchar(300)
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

	create table #TempNamT
	(
		id_NhuCau uniqueidentifier, TuChi_DTT_1 decimal(18,0), TuChi_DTT_2 decimal(18,0)
	)
				
	insert into #TempNamT(id_NhuCau,TuChi_DTT_1,TuChi_DTT_2)
	select	Id_MLNhuCau, TuChi_DTT_1 = sum(TuChi_DTT_1), TuChi_DTT_2 = sum(TuChi_DTT_2)
	from (
			select Id_MLNhuCau, TuChi_DTT_1 = sum(TuChi), TuChi_DTT_2 = 0 from f_skt_report_thnhucau_nt(@nam,@donvi,@phongban,@dvt)	
			where TuChi <> 0 and NamNS = @nam - 1
			group by Id_DonVi, Id_MLNhuCau

			union all

			select Id_MLNhuCau, TuChi_DTT_1 = 0, TuChi_DTT_2 = sum(TuChi) from f_skt_report_thnhucau_nt(@nam,@donvi,@phongban,@dvt)	
			where TuChi <> 0 and NamNS = @nam - 2
			group by Id_DonVi, Id_MLNhuCau
		) as re
	group by Id_MLNhuCau

	create table #Temp
	(
		Nganh nvarchar(2), TenNganh nvarchar(Max), KyHieu nvarchar(12),TuChi_DTT_2 decimal(18,0), TuChi_DTT_1 decimal(18,0), TonKho_DV decimal(18,0), 
		HuyDong_DV decimal(18,0), TuChi_DV decimal(18,0), HuyDong decimal(18,0), TuChi decimal(18,0)
	)	

	insert into #Temp(Nganh,TenNganh,KyHieu,TuChi_DTT_1,TuChi_DTT_2,TonKho_DV,HuyDong_DV,TuChi_DV,HuyDong,TuChi)	
    select	Nganh
			, sMoTa
			, KyHieu
			, TuChi_DTT_2	= ISNULL(TuChi_DTT_2,0)
			, TuChi_DTT_1	= ISNULL(TuChi_DTT_1,0)	
			, TonKho_DV		= ISNULL(TonKho_DV,0)
			, HuyDong_DV	= ISNULL(HuyDong_DV,0)
			, TuChi_DV		= ISNULL(TuChi_DV,0)
			, HuyDong		= ISNULL(HuyDong,0)
			, TuChi			= ISNULL(TuChi,0)					
	from	(
			select	Id
					, Nganh
					, KyHieu = SUBSTRING(KyHieu,0,10)
			from	SKT_MLNhuCau 
			where	NamLamViec=@nam
					and 1 in (select * from f_split(Loai))
					and IsParent = 0
					and KyHieu like '1-2%'
			)  ml

			left join 				

			(
			select	Id_Mucluc
					, TuChi_DTT_1 = 0
					, TuChi_DTT_2 = 0
					, TonKho_DV		= ISNULL(TonKho_DV,0)
					, HuyDong_DV	= ISNULL(HuyDong_DV,0)
					, TuChi_DV		= ISNULL(TuChi_DV,0)
					, HuyDong		= ISNULL(HuyDong,0)
					, TuChi			= ISNULL(TuChi,0)
			from
					(	
					select	Id_Mucluc		
							, Id_ChungTu					
							, TonKho_DV = TonKho_DV/@dvt
							, HuyDong_DV = HuyDong_DV/@dvt
							, TuChi_DV = TuChi_DV/@dvt
							, HuyDong = HuyDong/@dvt
							, TuChi = TuChi/@dvt
					from	SKT_ChungTuChiTiet 
					where	(TonKho_DV + HuyDong_DV + TuChi_DV + HuyDong + TuChi) <> 0
					) ctct

					right join 

					(
					select	Id as Id_Ct
					from	SKT_ChungTu 
					where	Id_PhongBan = @phongban
							and NamLamViec = @nam
							and (@donvi is null or Id_DonVi in (select * from f_split(@donvi)))) ct

					on ctct.Id_ChungTu = ct.Id_Ct		

			union all

			select	Id_Mucluc = id_NhuCau
					, TuChi_DTT_1
					, TuChi_DTT_2
					, TonKho_DV		= 0
					, HuyDong_DV	= 0
					, TuChi_DV		= 0
					, HuyDong		= 0
					, TuChi			= 0
			from	#TempNamT 
			)  re
					
			on re.Id_MucLuc = ml.Id

			left join

			(
			select	sNG
					, sMoTa
			from	NS_MucLucNganSach
			where	iTrangThai = 1
					and sLNS = ''
					and sNG <> '00'
					and sNG <> ''
					and iNamLamViec = (@nam - 1)
			) mlns

			on mlns.sNG = ml.Nganh	
	order by KyHieu		
	
	select Top(1)	nam
					, N_2 = CASE bangNS_2 WHEN 'QT' THEN N'QT ' + CONVERT(nvarchar(4),NamNS_2) 
										  WHEN 'DT' THEN N'DT đầu năm ' + CONVERT(nvarchar(4),NamNS_2)
										  ELSE '' END
					, N_1 = CASE bangNS_1 WHEN 'QT' THEN N'QT' + CONVERT(nvarchar(4),NamNS_1) 
										  WHEN 'DT' THEN N'DT đầu năm ' + CONVERT(nvarchar(4),NamNS_1)
										  ELSE '' END					 
	from			#TempTT

	select			Nganh
					, TenNganh 
					, KyHieu
					, NamNS2 = sum(TuChi_DTT_1)
					, NamNS1 = sum(TuChi_DTT_2)
					, TonKho_DV = sum(TonKho_DV)
					, HuyDong_DV = sum(HuyDong_DV)
					, TuChi_DV = sum(TuChi_DV)
					, HuyDong = sum(HuyDong)
					, TuChi = sum(TuChi)
	from 
					(
					select			Nganh
									, TenNganh
									, KyHieu
									, TuChi_DTT_1
									, TuChi_DTT_2
									, TonKho_DV
									, HuyDong_DV
									, TuChi_DV
									, HuyDong
									, TuChi
					from			#Temp) re
	group by		Nganh,TenNganh, KyHieu
	order by		KyHieu

END