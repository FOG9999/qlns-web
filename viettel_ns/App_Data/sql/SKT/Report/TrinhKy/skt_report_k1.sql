USE [VIETTEL_NS1]
GO
/****** Object:  StoredProcedure [dbo].[skt_report_k1]    Script Date: 20/07/2019 5:51:39 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[skt_report_k1]
	@nam	int,
	@dvt	int,
	@phongban nvarchar(3),
	@donvis nvarchar(500)
AS
BEGIN 
		
	create table #Temp
	(
		id_DonVi nvarchar(3), Id_MLSKT uniqueidentifier, TuChi decimal(18,0), DacThu decimal(18,0)
	)

	Insert into #Temp
	select		Id_DonVi
				, Id_MLSKT
				, TuChi = SUM(TuChi)
				, DacThu = SUM(DacThu)
	from		(
				select	Id_DonVi
						, Id_MLSKT
						, TuChi
						, DacThu = 0 
				from	f_skt_report_dtkt(@nam,@nam-2,@donvis,@phongban,@dvt)
				
				union all
				
				select 	Id_DonVi
						, Id_MLSKT
						, TuChi = 0
						, DacThu 
				from	f_skt_report_dacthu(@nam,@nam-1,null,@donvis,@phongban,@dvt,0)		
				)re	
	group by	Id_DonVi, Id_MLSKT
	
	select	X1=SUBSTRING(KyHieu,1,1)
			, X2=SUBSTRING(KyHieu,1,3)
			, X3=SUBSTRING(KyHieu,1,6)
			, X4=SUBSTRING(KyHieu,1,9)
			, KyHieu
			, MoTa
			, id_DonVi
			, TenDonVi = case id_DonVi when '' then N'Tổng cộng' else TenDonVi end
			, TuChi = SUM(TuChi)
			, DacThu = sum(DacThu)
	from	
			(
			select		*
			from		#Temp
			where		TuChi + DacThu <> 0

			--union all

			--select		id_DonVi = ''
			--			, Id_MLSKT
			--			, TuChi = SUM(TuChi)
			--			, DacThu = SUM(DacThu)
			--from		#Temp
			--group by	Id_MLSKT
			--having		sum(tuChi) + sum(DacThu) <> 0
			) ct

			left join 

			(
			select	Id
					, KyHieu
					, MoTa
			from	SKT_MLSKT
			where	NamLamViec = @nam
			) ml 
			
			on ct.Id_MLSKT = ml.Id

			left join 

			(
			select	iID_MaDonVi
					, TenDonVi = iID_MaDonVi + ' - ' + sTen
			from	NS_DonVi
			where	iNamLamViec_DonVi = @nam 
					and iTrangThai = 1
			) dv

			on ct.id_DonVi = dv.iID_MaDonVi
	group by id_DonVi, TenDonVi, KyHieu, MoTa
END
