USE [VIETTEL_NS1]
GO
/****** Object:  StoredProcedure [dbo].[skt_report_thxdskt]    Script Date: 11/07/2019 7:56:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[skt_report_thxdskt]
	@nam	int,
	@dvt	int,
	@phongban nvarchar(3),
	@donvis nvarchar(500)
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
		id_DonVi nvarchar(3), id_NhuCau uniqueidentifier, TuChi_DTT_1 decimal(18,0), TuChi_DTT_2 decimal(18,0)
	)
				
	insert into #TempNamT(id_DonVi,id_NhuCau,TuChi_DTT_1,TuChi_DTT_2)
	select	Id_DonVi, Id_MLNhuCau, TuChi_DTT_1 = sum(TuChi_DTT_1), TuChi_DTT_2 = sum(TuChi_DTT_2)
	from (
			select Id_DonVi, Id_MLNhuCau, TuChi_DTT_1 = sum(TuChi), TuChi_DTT_2 = 0 from f_skt_report_thnhucau_nt(@nam,@donvis,@phongban,@dvt)	
			where TuChi <> 0 and NamNS = @nam - 1
			group by Id_DonVi, Id_MLNhuCau

			union all

			select Id_DonVi, Id_MLNhuCau, TuChi_DTT_1 = 0, TuChi_DTT_2 = sum(TuChi) from f_skt_report_thnhucau_nt(@nam,@donvis,@phongban,@dvt)	
			where TuChi <> 0 and NamNS = @nam - 2
			group by Id_DonVi, Id_MLNhuCau
		) as re
	group by Id_DonVi, Id_MLNhuCau

	create table #Temp
	(
		KyHieu nvarchar(13), MoTa nvarchar(Max), DonVi nvarchar(3), TenDonVi nvarchar(100), TuChi_DTT_2 decimal(18,0), TuChi_DTT_1 decimal(18,0), TonKho_DV decimal(18,0), 
		HuyDong_DV decimal(18,0), TuChi_DV decimal(18,0), HuyDong decimal(18,0), TuChi decimal(18,0)
	)	

	insert into #Temp(KyHieu,MoTa,DonVi,TenDonVi,TuChi_DTT_1,TuChi_DTT_2,TonKho_DV,HuyDong_DV,TuChi_DV,HuyDong,TuChi)
	
    select	KyHieu
			, MoTa
			, dv.iID_MaDonVi	
			, sTen		
			, TuChi_DTT_2	= ISNULL(TuChi_DTT_2,0)
			, TuChi_DTT_1	= ISNULL(TuChi_DTT_1,0)	
			, TonKho_DV		= ISNULL(TonKho_DV,0)
			, HuyDong_DV	= ISNULL(HuyDong_DV,0)
			, TuChi_DV		= ISNULL(TuChi_DV,0)
			, HuyDong		= ISNULL(HuyDong,0)
			, TuChi			= ISNULL(TuChi,0)					
	from	(
			select	Id
					, KyHieu
					, MoTa
			from	SKT_MLNhuCau 
			where	NamLamViec=@nam
					and 1 in (select * from f_split(Loai))
					and IsParent = 0
			)  ml			

			left join 

			(
			select	Id_Mucluc
					, Id_DonVi
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
							, Id_DonVi 
					from	SKT_ChungTu 
					where	Id_PhongBan = @phongban
							and NamLamViec = @nam) ct

					on ctct.Id_ChungTu = ct.Id_Ct		

			union all

			select	Id_Mucluc = id_NhuCau
					, Id_DonVi = id_DonVi
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

			(select	iID_MaDonVi
					, sTen
			from		NS_DonVi
			where		iNamLamViec_DonVi = @nam
					and iTrangThai = 1) dv

			on dv.iID_MaDonVi = re.Id_DonVi
	
	select Top(1)	nam
					, N_2 = CASE bangNS_2 WHEN 'QT' THEN N'QT ' + CONVERT(nvarchar(4),NamNS_2) 
										  WHEN 'DT' THEN N'DT đầu năm ' + CONVERT(nvarchar(4),NamNS_2)
										  ELSE '' END
					, N_1 = CASE bangNS_1 WHEN 'QT' THEN N'QT' + CONVERT(nvarchar(4),NamNS_1) 
										  WHEN 'DT' THEN N'DT đầu năm ' + CONVERT(nvarchar(4),NamNS_1)
										  ELSE '' END					 
	from			#TempTT

	select Distinct DonVi,TenDonVi 
	from			#Temp
	where			DonVi is not null
	order by		DonVi

	select			KyHieu 
					, DonVi
					, NamNS2 = sum(TuChi_DTT_1)
					, NamNS1 = sum(TuChi_DTT_2)
					, TonKho_DV = sum(TonKho_DV)
					, HuyDong_DV = sum(HuyDong_DV)
					, TuChi_DV = sum(TuChi_DV)
					, HuyDong = sum(HuyDong)
					, TuChi = sum(TuChi)
	from 
					(
					select			KyHieu = case when KyHieu like '1-2%' then '1-2'
																		  else SUBSTRING(KyHieu,0,7) end
									, DonVi
									, TuChi_DTT_1
									, TuChi_DTT_2
									, TonKho_DV
									, HuyDong_DV
									, TuChi_DV
									, HuyDong
									, TuChi
					from			#Temp
					where			DonVi is not null
									and (KyHieu like '1-1-01%' or KyHieu like '1-1-02%' or KyHieu like '1-2%')) re
	group by		KyHieu, DonVi
	order by		DonVi, KyHieu

END
