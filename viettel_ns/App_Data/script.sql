USE [Local_NS]
GO
/****** Object:  StoredProcedure [dbo].[sp_ncskt_comdata_ctct]    Script Date: 12/05/2020 6:17:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
	
	author:	hiep
	date:	08/05/2020
	decs:	Lấy số liệu chứng từ form nhập nhu cầu - số kiểm tra
	params:	

*/

CREATE PROCEDURE [dbo].[sp_ncskt_comdata_ctct]
	@id uniqueidentifier,
	@dvt	int,
	@Nganh nvarchar(10),
	@Nganh_Parent nvarchar(10)
AS
BEGIN 

	create table #Temp
	(
		 nam int, id_DonVi nvarchar(5), id_PhongBan nvarchar(2), byNg nvarchar(10), loai int  		 
	) 

	insert into #Temp(nam,id_DonVi,id_PhongBan,loai)
	select NamLamViec,Id_DonVi,Id_PhongBan,iLoai from SKT_ComData_ChungTu where Id = @id

	declare @NamLamViec int								set @NamLamViec = (select top(1) nam from #Temp)
	declare @id_DonVi nvarchar(10)						set @id_DonVi = (select top(1) id_DonVi from #Temp)
	declare @id_PhongBan nvarchar(10)					set @id_PhongBan= (select top(1) id_PhongBan from #Temp)
	declare @loai nvarchar(1)							set @loai= (select top(1) loai from #Temp) 
	declare @byNg nvarchar(1000)						set @byNg= null
		
	select		Id
				, ml.Id_Parent
				, ml.IsParent
				, ml.KyHieu
				, ml.Nganh_Parent
				, ml.Nganh
				, ml.MoTa
				, Id_ChungTuChiTiet
				, TuChi_3 = ISNULL(TuChi_3,0)
				, MuaHang_3 = ISNULL(MuaHang_3,0)
				, PhanCap_3 = ISNULL(PhanCap_3,0)
				, TuChi_2 = ISNULL(TuChi_2,0)
				, MuaHang_2 = ISNULL(MuaHang_2,0)
				, TuChi_1 = ISNULL(TuChi_1,0)
				, MuaHang_1 = ISNULL(MuaHang_1,0)								  
	from		(
				select	Id
						, Id_Parent
						, IsParent
						, KyHieu
						, Nganh_Parent
						, Nganh
						, MoTa
				from	SKT_MLNhuCau 
				where	NamLamViec=@NamLamViec
						and @loai in (select * from f_split(Loai))
						) as ml

				left join 

				(	
				select	Id as Id_ChungTuChiTiet
						, KyHieu							
						, TuChi_3 = ISNULL(TuChi_3/@dvt,0)
						, MuaHang_3 = ISNULL(MuaHang_3/@dvt,0)
						, PhanCap_3 = ISNULL(PhanCap_3/@dvt,0)
						, TuChi_2 = ISNULL(TuChi_2/@dvt,0)
						, MuaHang_2 = ISNULL(MuaHang_2/@dvt,0)
						, TuChi_1 = ISNULL(TuChi_1/@dvt,0)
						, MuaHang_1 = ISNULL(MuaHang_1/@dvt,0)
				from	SKT_ComData_ChungTuChiTiet 
				where	(Id_DonVi = @id_DonVi and Id_PhongBan = @id_PhongBan and NamLamViec = @NamLamViec)
				) as ct

				on ct.KyHieu=ml.KyHieu 				

	where		(@Nganh is null or Nganh like @Nganh)
				and (Nganh_Parent is null or @Nganh_Parent is null or Nganh_Parent like @Nganh_Parent)
				and (@byNg is null or Nganh_Parent is null or Nganh_Parent in (select * from f_split(@byNg)))
	order by	ml.KyHieu	
END


GO
/****** Object:  StoredProcedure [dbo].[sp_ncskt_copymlnc_mlskt]    Script Date: 12/05/2020 6:17:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
	
	author:	hiep
	date:	08/05/2020
	decs:	Copy bộ mục lục nhu cầu - số kiểm tra tư năm source sang năm dest
	params:	

*/

CREATE PROCEDURE [dbo].[sp_ncskt_copymlnc_mlskt]
	@namd int,
	@nams int
AS
BEGIN 
		
	delete from SKT_MLSKTNT
	where	NamLamViec = @namd 

	insert into SKT_MLSKTNT(
							[Id]
							,[Id_Parent]
							,[IsParent]
							,[Loai]
							,[M]
							,[Nganh_Parent]
							,[Nganh]
							,[STT]
							,[KyHieu]
							,[MoTa]
							,[KyHieuCha]
							,[NamLamViec]
							,[DateCreated]
							,[UserCreator]
							,[DateModified]
							,[IpModified]
							,[UserModifier]
							,[AuditCount]
							,[AuditLog])
	select					[Id]
							,[Id_Parent]
							,[IsParent]
							,[Loai]
							,[M]
							,[Nganh_Parent]
							,[Nganh]
							,[STT]
							,[KyHieu]
							,[MoTa]
							,[KyHieuCha]
							,@namd
							,GETDATE()
							,'admin'
							,NULL
							,NULL
							,NULL
							,NULL
							,NULL
	from					SKT_MLNhuCau
	where					NamLamViec = @nams

	delete from SKT_MLNhuCau
	where	NamLamViec = @namd 
	
	insert into SKT_MLNhuCau(
							[Id]
							,[Id_Parent]
							,[IsParent]
							,[Loai]
							,[M]
							,[Nganh_Parent]
							,[Nganh]
							,[STT]
							,[KyHieu]
							,[MoTa]
							,[KyHieuCha]
							,[NamLamViec]
							,[DateCreated]
							,[UserCreator]
							,[DateModified]
							,[IpModified]
							,[UserModifier]
							,[AuditCount]
							,[AuditLog])
	select					newid()
							,'00000000-0000-0000-0000-000000000000'
							,[IsParent]
							,[Loai]
							,[M]
							,[Nganh_Parent]
							,[Nganh]
							,[STT]
							,[KyHieu]
							,[MoTa]
							,[KyHieuCha]
							,@namd
							,GETDATE()
							,'admin'
							,NULL
							,NULL
							,NULL
							,NULL
							,NULL
	from					SKT_MLNhuCau
	where					NamLamViec = @nams

	update SKT_MLNhuCau
	set Id_Parent = ISNULL((select s.Id from SKT_MLNhuCau as s where s.NamLamViec = @namd and s.KyHieu = SKT_MLNhuCau.KyHieuCha),'00000000-0000-0000-0000-000000000000')
	where NamLamViec = @namd

END



GO
/****** Object:  StoredProcedure [dbo].[sp_ncskt_ctct]    Script Date: 12/05/2020 6:17:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
	
	author:	hiep
	date:	08/05/2020
	decs:	Lấy số liệu chứng từ form nhập nhu cầu - số kiểm tra
	params:	

*/

CREATE PROCEDURE [dbo].[sp_ncskt_ctct]
	@id uniqueidentifier,
	@dvt	int,
	@Nganh nvarchar(10),
	@Nganh_Parent nvarchar(10)
AS
BEGIN 

	create table #Temp
	(
		 nam int, NamNS_1 int, NamNS_2 int, NamNS_3 int, id_DonVi nvarchar(5), id_PhongBan nvarchar(2), id_PhongBanDich nvarchar(2), loai nvarchar(1), byNg nvarchar(10)  		 
	) 

	insert into #Temp(nam,NamNS_3,id_DonVi,id_PhongBan,id_PhongBanDich,loai)
	select NamLamViec,NamLamViec-1,Id_DonVi,Id_PhongBan,Id_PhongBanDich,iLoai from SKT_ChungTu where Id = @id

	update #Temp
	set		NamNS_1 = (select CAST(SUBSTRING(s.NamNS_1,CHARINDEX('_',s.NamNS_1,0)+1,LEN(s.NamNS_1)) as int) from SKT_MapDataNS as s where s.NamLamViec = nam),
			NamNS_2 = (select CAST(SUBSTRING(s.NamNS_2,CHARINDEX('_',s.NamNS_1,0)+1,LEN(s.NamNS_2)) as int) from SKT_MapDataNS as s where s.NamLamViec = nam)

	declare @NamLamViec int								set @NamLamViec = (select top(1) nam from #Temp)
	declare @id_DonVi nvarchar(10)						set @id_DonVi = (select top(1) id_DonVi from #Temp)
	declare @NamNS_1 int								set @NamNS_1 = (select top(1) NamNS_1 from #Temp)
	declare @NamNS_2 int								set @NamNS_2 = (select top(1) NamNS_2 from #Temp)
	declare @NamNS_3 int								set @NamNS_3 = (select top(1) NamNS_3 from #Temp)
	declare @id_PhongBanDich nvarchar(10)				set @id_PhongBanDich= (select top(1) id_PhongBanDich from #Temp)
	declare @id_PhongBan nvarchar(10)					set @id_PhongBan= (select top(1) id_PhongBan from #Temp)
	declare @loai nvarchar(1)							set @loai= (select top(1) loai from #Temp) 
	declare @byNg nvarchar(1000)						set @byNg= null

	if (@loai = 2)
		set @byNg = (select top(1) id_DonVi from #Temp)
	
	create table #TempNamT
	(
		namNS int, KyHieu nvarchar(12), TuChi decimal(18,0), MuaHang decimal(18,0), PhanCap decimal(18,0)
	)
				
	insert into #TempNamT
	select namNS, KyHieu, TuChi, MuaHang, 0 from f_ncskt_ctctdtnt(@NamLamViec,@id_DonVi,@id_PhongBanDich,@dvt)
	
	insert into #TempNamT
	select namNS, KyHieu, TuChi, MuaHang, PhanCap from f_ncskt_ctctsktnt(@NamLamViec,@loai,@id_DonVi,@id_PhongBanDich,@dvt)			
	
	create table #Result
	(
		Id uniqueidentifier, Id_Parent uniqueidentifier, IsParent bit, KyHieu nvarchar(13), Nganh_Parent nvarchar(3), Nganh nvarchar(2), MoTa nvarchar(MAX),
		Id_ChungTuChiTiet uniqueidentifier, TuChi_DTT_3 decimal(18,0), MuaHang_DTT_3 decimal(18,0), PhanCap_DTT_3 decimal(18,0), TuChi_DTT_2 decimal(18,0), 
		MuaHang_DTT_2 decimal(18,0), TuChi_DTT_1 decimal(18,0), MuaHang_DTT_1 decimal(18,0), 
		TonKho_DV decimal(18,0), HuyDong_DV decimal(18,0), TuChi_DV decimal(18,0), MuaHang_DV decimal(18,0), PhanCap_DV decimal(18,0), TongCong_DV decimal(18,0),
		HuyDong_Bql decimal(18,0), TuChi_Bql decimal(18,0), MuaHang_Bql decimal(18,0), PhanCap_Bql decimal(18,0), TongCong_Bql decimal(18,0),HuyDong decimal(18,0), 
		TuChi decimal(18,0), MuaHang decimal(18,0), PhanCap decimal(18,0), TongCong decimal(18,0), Tang decimal(18,0), Giam decimal(18,0), GhiChu nvarchar(MAX)  
	)	

	insert into #Result
	select		ml.Id
				, ml.Id_Parent
				, ml.IsParent
				, ml.KyHieu
				, ml.Nganh_Parent
				, ml.Nganh
				, ml.MoTa
				, Id_ChungTuChiTiet
				, TuChi_DTT_3 = ISNULL(TuChi_DTT_3,0)
				, MuaHang_DTT_3 = ISNULL(MuaHang_DTT_3,0)
				, PhanCap_DTT_3 = ISNULL(PhanCap_DTT_3,0)
				, TuChi_DTT_2 = ISNULL(TuChi_DTT_2,0)
				, MuaHang_DTT_2 = ISNULL(MuaHang_DTT_2,0)
				, TuChi_DTT_1 = ISNULL(TuChi_DTT_1,0)
				, MuaHang_DTT_1 = ISNULL(MuaHang_DTT_1,0)				
				, TonKho_DV = ISNULL(TonKho_DV,0)
				, HuyDong_DV = ISNULL(HuyDong_DV,0)
				, TuChi_DV = ISNULL(TuChi_DV,0)
				, MuaHang_DV = ISNULL(MuaHang_DV,0)
				, PhanCap_DV = ISNULL(PhanCap_DV,0)
				, TongCong_DV = ISNULL(TongCong_DV,0)
				, HuyDong_Bql = ISNULL(HuyDong_Bql,0)
				, TuChi_Bql = ISNULL(TuChi_Bql,0)
				, MuaHang_Bql = ISNULL(MuaHang_Bql,0)
				, PhanCap_Bql = ISNULL(PhanCap_Bql,0)
				, TongCong_Bql = ISNULL(TongCong_Bql,0)
				, HuyDong = ISNULL(HuyDong,0)
				, TuChi = ISNULL(TuChi,0)			
				, MuaHang = ISNULL(MuaHang,0)		
				, PhanCap = ISNULL(PhanCap,0)
				, TongCong = ISNULL(TongCong,0)
				, Tang 
				, Giam 
				, GhiChu  
	from		(
				select	Id
						, Id_Parent
						, IsParent
						, KyHieu
						, Nganh_Parent
						, Nganh
						, MoTa
				from	SKT_MLNhuCau 
				where	NamLamViec=@NamLamViec
						and @loai in (select * from f_split(Loai))
						) as ml

				left join 

				(	
				select	Id as Id_ChungTuChiTiet
						, Id_Mucluc							
						, TonKho_DV = TonKho_DV/@dvt
						, HuyDong_DV = HuyDong_DV/@dvt
						, TuChi_DV = TuChi_DV/@dvt
						, MuaHang_DV = MuaHang_DV/@dvt	
						, PhanCap_DV = PhanCap_DV/@dvt
						, TongCong_DV = CASE @loai when 1 then (HuyDong_DV + TuChi_DV)/@dvt
												   when 2 then (HuyDong_DV + MuaHang_DV + PhanCap_DV)/@dvt
												   else 0 end	
						, HuyDong_Bql = 0
						, TuChi_Bql = 0
						, MuaHang_Bql = 0
						, PhanCap_Bql = 0
						, TongCong_Bql = 0
						, HuyDong = HuyDong/@dvt
						, TuChi = TuChi/@dvt
						, MuaHang = MuaHang/@dvt				
						, PhanCap = PhanCap/@dvt
						, TongCong = CASE @loai when 1 then (HuyDong + TuChi)/@dvt
												when 2 then (HuyDong + MuaHang + PhanCap)/@dvt
												else 0 end	
						, Tang = Tang/@dvt
						, Giam = Giam/@dvt		
						, GhiChu 
				from	SKT_ChungTuChiTiet 
				where	(Id_ChungTu=@id)
				) as ct

				on ct.Id_Mucluc=ml.Id 

				left join 

				(select KyHieu as KHN_1
						, TuChi as TuChi_DTT_1
						, MuaHang as MuaHang_DTT_1 
				 from	#TempNamT 
				 where	namNS = @NamNS_1
				 ) as namN_1

				 on ml.KyHieu = namN_1.KHN_1

				 left join 

				(select KyHieu as KHN_2
						, TuChi as TuChi_DTT_2
						, MuaHang as MuaHang_DTT_2
				 from	#TempNamT 
				 where	namNS = @NamNS_2
				 ) as namN_2

				 on ml.KyHieu = namN_2.KHN_2

				 left join 

				(select KyHieu as KHN_3
						, TuChi as TuChi_DTT_3
						, MuaHang as MuaHang_DTT_3
						, PhanCap as PhanCap_DTT_3
				 from	#TempNamT 
				 where	namNS = @NamNS_3
				 ) as namN_3

				 on ml.KyHieu = namN_3.KHN_3

	where		(@Nganh is null or Nganh like @Nganh)
				and (Nganh_Parent is null or @Nganh_Parent is null or Nganh_Parent like @Nganh_Parent)
				and (@byNg is null or Nganh_Parent is null or Nganh_Parent in (select * from f_split(@byNg)))
	order by	ml.KyHieu

	create table #TempValue
	(
		Id_MucLuc uniqueidentifier, TonKho_DV decimal(18,0), HuyDong_DV decimal(18,0), TuChi_DV decimal(18,0), MuaHang_DV decimal(18,0), PhanCap_DV decimal(18,0),
		TongCong_DV decimal(18,0),HuyDong_Bql decimal(18,0), TuChi_Bql decimal(18,0), MuaHang_Bql decimal(18,0), PhanCap_Bql decimal(18,0), TongCong_Bql decimal(18,0)
	)
	if (@id_PhongBan = '02') 
		begin		
			
			insert into #TempValue
			select		Id_MucLuc
						, TonKho_DV		= ISNULL(SUM(TonKho_DV) / @dvt, 0)
						, HuyDong_DV	= ISNULL(SUM(HuyDong_DV) / @dvt, 0)
						, TuChi_DV		= ISNULL(SUM(TuChi_DV) / @dvt, 0)
						, MuaHang_DV	= ISNULL(SUM(MuaHang_DV) / @dvt, 0)
						, PhanCap_DV	= ISNULL(SUM(PhanCap_DV) / @dvt, 0)	
						, TongCong_DV	= CASE @loai when 1 then ISNULL(SUM(HuyDong_DV + TuChi_DV)/@dvt, 0)
													 when 2 then ISNULL(SUM(HuyDong_DV + MuaHang_DV + PhanCap_DV)/@dvt, 0)
													 else 0 end		
						, HuyDong_Bql	= ISNULL(SUM(HuyDong) / @dvt, 0)
						, TuChi_Bql		= ISNULL(SUM(TuChi) / @dvt, 0)
						, MuaHang_Bql	= ISNULL(SUM(MuaHang) / @dvt, 0)
						, PhanCap_Bql	= ISNULL(SUM(PhanCap) / @dvt, 0)
						, TongCong_Bql	= CASE @loai when 1 then ISNULL(SUM(HuyDong + TuChi)/@dvt, 0)
													 when 2 then ISNULL(SUM(HuyDong + MuaHang + PhanCap)/@dvt, 0)
													 else 0 end
			from		SKT_ChungTuChiTiet 
			where		Id_ChungTu in	(select Id 
										 from	SKT_ChungTu 
										 where 	NamLamViec = @NamLamViec
												and iLoai = @loai
												and Id_DonVi = @id_DonVi
												and Id_PhongBan = Id_PhongBanDich
												and Id_PhongBanDich = @id_PhongBanDich)		
			group by	Id_MucLuc

			update	#Result
			set		TonKho_DV		= ISNULL((select top(1) TonKho_DV from #TempValue where Id = Id_MucLuc),0)
					, HuyDong_DV	= ISNULL((select top(1) HuyDong_DV from #TempValue where Id = Id_MucLuc),0)
					, TuChi_DV		= ISNULL((select top(1) TuChi_DV from #TempValue where Id = Id_MucLuc),0)
					, MuaHang_DV	= ISNULL((select top(1) MuaHang_DV from #TempValue where Id = Id_MucLuc),0)
					, PhanCap_DV	= ISNULL((select top(1) PhanCap_DV from #TempValue where Id = Id_MucLuc),0)	
					, TongCong_DV	= ISNULL((select top(1) TongCong_DV from #TempValue where Id = Id_MucLuc),0)	
					, HuyDong_Bql	= ISNULL((select top(1) HuyDong_Bql from #TempValue where Id = Id_MucLuc),0)
					, TuChi_Bql		= ISNULL((select top(1) TuChi_Bql from #TempValue where Id = Id_MucLuc),0)
					, MuaHang_Bql	= ISNULL((select top(1) MuaHang_Bql from #TempValue where Id = Id_MucLuc),0)
					, PhanCap_Bql	= ISNULL((select top(1) PhanCap_Bql from #TempValue where Id = Id_MucLuc),0)
					, TongCong_Bql	= ISNULL((select top(1) TongCong_Bql from #TempValue where Id = Id_MucLuc),0)
			where	IsParent = 0			
		end	
	else if (@id_PhongBan = '11') 
		begin		
			
			insert into #TempValue
			select		Id_MucLuc
						, TonKho_DV		= 0
						, HuyDong_DV	= 0
						, TuChi_DV		= 0
						, MuaHang_DV	= 0
						, PhanCap_DV	= 0
						, TongCong_DV	= 0	
						, HuyDong_Bql	= ISNULL(SUM(HuyDong) / @dvt, 0)
						, TuChi_Bql		= ISNULL(SUM(TuChi) / @dvt, 0)
						, MuaHang_Bql	= ISNULL(SUM(MuaHang) / @dvt, 0)
						, PhanCap_Bql	= ISNULL(SUM(PhanCap) / @dvt, 0)
						, TongCong_Bql	= CASE @loai when 1 then ISNULL(SUM(HuyDong + TuChi)/@dvt, 0)
													 when 2 then ISNULL(SUM(HuyDong + MuaHang + PhanCap)/@dvt, 0)
													 else 0 end
			from		SKT_ChungTuChiTiet 
			where		Id_ChungTu in	(select Id 
										 from	SKT_ChungTu 
										 where 	NamLamViec = @NamLamViec
												and iLoai = @loai
												and Id_DonVi = @id_DonVi
												and Id_PhongBan = '02'
												and Id_PhongBanDich = @id_PhongBanDich)		
			group by	Id_MucLuc

			update	#Result
			set		TonKho_DV		= ISNULL((select top(1) TonKho_DV from #TempValue where Id = Id_MucLuc),0)
					, HuyDong_DV	= ISNULL((select top(1) HuyDong_DV from #TempValue where Id = Id_MucLuc),0)
					, TuChi_DV		= ISNULL((select top(1) TuChi_DV from #TempValue where Id = Id_MucLuc),0)
					, MuaHang_DV	= ISNULL((select top(1) MuaHang_DV from #TempValue where Id = Id_MucLuc),0)
					, PhanCap_DV	= ISNULL((select top(1) PhanCap_DV from #TempValue where Id = Id_MucLuc),0)	
					, TongCong_DV	= ISNULL((select top(1) TongCong_DV from #TempValue where Id = Id_MucLuc),0)	
					, HuyDong_Bql	= ISNULL((select top(1) HuyDong_Bql from #TempValue where Id = Id_MucLuc),0)
					, TuChi_Bql		= ISNULL((select top(1) TuChi_Bql from #TempValue where Id = Id_MucLuc),0)
					, MuaHang_Bql	= ISNULL((select top(1) MuaHang_Bql from #TempValue where Id = Id_MucLuc),0)
					, PhanCap_Bql	= ISNULL((select top(1) PhanCap_Bql from #TempValue where Id = Id_MucLuc),0)
					, TongCong_Bql	= ISNULL((select top(1) TongCong_Bql from #TempValue where Id = Id_MucLuc),0)
			where	IsParent = 0			
		end			
	
	if (@loai != 3 and @loai != 4)
		begin
			if (@id_PhongBan != '11')
				begin				
					select		Id, Id_Parent, IsParent, KyHieu, Nganh_Parent, Nganh, MoTa,
								Id_ChungTuChiTiet, TuChi_DTT_3, MuaHang_DTT_3, PhanCap_DTT_3, TuChi_DTT_2, MuaHang_DTT_2, TuChi_DTT_1, MuaHang_DTT_1, 
								TonKho_DV, HuyDong_DV, TuChi_DV, MuaHang_DV, PhanCap_DV, TongCong_DV,
								HuyDong_Bql, TuChi_Bql, MuaHang_Bql, PhanCap_Bql, TongCong_Bql, HuyDong, TuChi, 
								MuaHang, PhanCap, TongCong
								, Tang = CASE WHEN @loai = 1 and (TongCong - TuChi_DTT_3) > 0 THEN ISNULL(TongCong - TuChi_DTT_3,0)
											  WHEN @loai = 2 and (TongCong - MuaHang_DTT_3 - PhanCap_DTT_3) > 0 THEN ISNULL(TongCong - MuaHang_DTT_3 - PhanCap_DTT_3,0)
											  ELSE 0 END
								, Giam = CASE WHEN @loai = 1 and (TongCong - TuChi_DTT_3) < 0 THEN ISNULL(TuChi_DTT_3 - TongCong,0)
												  WHEN @loai = 2 and (TongCong - MuaHang_DTT_3 - PhanCap_DTT_3) < 0 THEN ISNULL(MuaHang_DTT_3 + PhanCap_DTT_3- TongCong,0)
												  ELSE 0 END
								, GhiChu 
					from		#Result
					order by	KyHieu
				end 
			else 
				begin
					select		Id, Id_Parent, IsParent, KyHieu, Nganh_Parent, Nganh, MoTa,
								Id_ChungTuChiTiet, TuChi_DTT_3, MuaHang_DTT_3, PhanCap_DTT_3, TuChi_DTT_2, MuaHang_DTT_2, TuChi_DTT_1, MuaHang_DTT_1,
								HuyDong_Bql, TuChi_Bql, MuaHang_Bql, PhanCap_Bql, TongCong_Bql, HuyDong, TuChi, 
								MuaHang, PhanCap, TongCong, Tang, Giam, GhiChu 
					from		#Result
					order by	KyHieu
				end
		end
	else 
		begin
			select		Id, Id_Parent, IsParent, KyHieu, Nganh_Parent, Nganh, MoTa,
						Id_ChungTuChiTiet, Tang, Giam, GhiChu 
			from		#Result
			order by	KyHieu
		end

END


GO
/****** Object:  StoredProcedure [dbo].[sp_ncskt_getcomdata]    Script Date: 12/05/2020 6:17:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
	
	author:	hiep
	date:	08/05/2020
	decs:	Lấy số liệu các năm trước đưa vào bảng để căn cứ xây dựng số kiểm tra
	params:	

*/

CREATE PROCEDURE [dbo].[sp_ncskt_getcomdata]
	@nam int
AS
BEGIN 
	
	delete from SKT_ComDatas
	where NamLamViec = @nam
	delete from SKT_ComDatasDacThu
	where NamLamViec = @nam
	delete from SKT_ComDatasSKTNT
	where NamLamViec = @nam
	declare @maxrdata int
	declare @maxrdatadt int
	declare @maxrdataskt int
	select @maxrdata = max(Id) from SKT_ComDatas
	select @maxrdatadt = max(Id) from SKT_ComDatasDacThu
	select @maxrdataskt = max(Id) from SKT_ComDatasSKTNT	

	if @maxrdata is null set @maxrdata = 0
	if @maxrdatadt is null set @maxrdatadt = 0
	if @maxrdataskt is null set @maxrdataskt = 0

	DBCC CHECKIDENT('SKT_ComDatas',RESEED,@maxrdata)
	DBCC CHECKIDENT('SKT_ComDatasDacThu',RESEED,@maxrdatadt)
	DBCC CHECKIDENT('SKT_ComDatasSKTNT',RESEED,@maxrdataskt)
	
	create table #Temp
	(
		 NamNS_1 int, bangNS_1 nvarchar(2), NamNS_2 int, bangNS_2 nvarchar(2)  		 
	) 	

	insert into #Temp
	select top(1)	CAST(SUBSTRING(NamNS_1,CHARINDEX('_',NamNS_1,0)+1,LEN(NamNS_1)) as int)
					, SUBSTRING(NamNS_1,0,CHARINDEX('_',NamNS_1,0))
					, CAST(SUBSTRING(NamNS_2,CHARINDEX('_',NamNS_2,0)+1,LEN(NamNS_2)) as int)
					, SUBSTRING(NamNS_2,0,CHARINDEX('_',NamNS_2,0)) 
	from			SKT_MapDataNS 
	where			NamLamViec = @nam

	declare @NamNS_1 int								set @NamNS_1 = (select top(1) NamNS_1 from #Temp)
	declare @bangNS_1 nvarchar(2)						set @bangNS_1 = (select top(1) bangNS_1 from #Temp)
	declare @NamNS_2 int								set @NamNS_2 = (select top(1) NamNS_2 from #Temp)
	declare @bangNS_2 nvarchar(2)						set @bangNS_2 = (select top(1) bangNS_2 from #Temp)

	if (@bangNS_2 = 'QT')		
		begin
			insert into SKT_ComDatas(NamLamViec,NamNS,Loai,Id_DonVi,Id_PhongBan,XauNoiMa,Lns,L,K,M,Tm,Ttm,Ng,TuChi,HangNhap,HangMua)
			select @nam,@NamNS_2,'QT',iID_MaDonVi,iID_MaPhongBan,sLNS+'-'+sL+'-'+sK+'-'+sM+'-'+sTM+'-'+sTTM+'-'+sNG,sLNS,sL,sK,sM,sTM,sTTM,sNG,TuChi,HangNhap,HangMua from f_skt_cdqt(@NamNS_2)
		end
	else if (@bangNS_2 = 'DT')		
		begin
			insert into SKT_ComDatas(NamLamViec,NamNS,Loai,Id_DonVi,Id_PhongBan,XauNoiMa,Lns,L,K,M,Tm,Ttm,Ng,TuChi,HangNhap,HangMua)
			select @nam,@NamNS_2,'DT',iID_MaDonVi,iID_MaPhongBan,sLNS+'-'+sL+'-'+sK+'-'+sM+'-'+sTM+'-'+sTTM+'-'+sNG,sLNS,sL,sK,sM,sTM,sTTM,sNG,TuChi,HangNhap,HangMua from f_skt_cddt(@NamNS_2)	

			insert into SKT_ComDatasDacThu(NamLamViec,NamNS,Loai,Id_DonVi,Id_PhongBan,XauNoiMa,Lns,L,K,M,Tm,Ttm,Ng,TuChi)
			select @nam,@NamNS_2,'DT',iID_MaDonVi,iID_MaPhongBan,sLNS+'-'+sL+'-'+sK+'-'+sM+'-'+sTM+'-'+sTTM+'-'+sNG,sLNS,sL,sK,sM,sTM,sTTM,sNG,TuChi from f_skt_cddtdt(@NamNS_2)	 		
		end
		
	if (@bangNS_1 = 'QT')
		begin		
			insert into SKT_ComDatas(NamLamViec,NamNS,Loai,Id_DonVi,Id_PhongBan,XauNoiMa,Lns,L,K,M,Tm,Ttm,Ng,TuChi,HangNhap,HangMua)
			select @nam,@NamNS_1,'QT',iID_MaDonVi,iID_MaPhongBan,sLNS+'-'+sL+'-'+sK+'-'+sM+'-'+sTM+'-'+sTTM+'-'+sNG,sLNS,sL,sK,sM,sTM,sTTM,sNG,TuChi,HangNhap,HangMua from f_skt_cdqt(@NamNS_1)
		end
	else if (@bangNS_1 = 'DT')	
		begin	
			insert into SKT_ComDatas(NamLamViec,NamNS,Loai,Id_DonVi,Id_PhongBan,XauNoiMa,Lns,L,K,M,Tm,Ttm,Ng,TuChi,HangNhap,HangMua)
			select @nam,@NamNS_1,'DT',iID_MaDonVi,iID_MaPhongBan,sLNS+'-'+sL+'-'+sK+'-'+sM+'-'+sTM+'-'+sTTM+'-'+sNG,sLNS,sL,sK,sM,sTM,sTTM,sNG,TuChi,HangNhap,HangMua from f_skt_cddt(@NamNS_1)

			insert into SKT_ComDatasDacThu(NamLamViec,NamNS,Loai,Id_DonVi,Id_PhongBan,XauNoiMa,Lns,L,K,M,Tm,Ttm,Ng,TuChi)
			select @nam,@NamNS_1,'DT',iID_MaDonVi,iID_MaPhongBan,sLNS+'-'+sL+'-'+sK+'-'+sM+'-'+sTM+'-'+sTTM+'-'+sNG,sLNS,sL,sK,sM,sTM,sTTM,sNG,TuChi from f_skt_cddtdt(@NamNS_1)
		end	

	insert into SKT_ComDatasSKTNT(iLoai,Id_DonVi,Id_PhongBanDich,NamLamViec,NamNS,KyHieu,HuyDong,TuChi,MuaHang,PhanCap)
	select iLoai,Id_DonVi,Id_PhongBanDich,@nam,NamLamViec,KyHieu,HuyDong,TuChi,MuaHang,PhanCap from f_skt_datapast(@nam-1)

	update SKT_ComDatasSKTNT
	set Id_SKTNT = ISNULL((select s.Id from SKT_MLSKTNT as s where s.NamLamViec = @nam and s.KyHieu = SKT_ComDatasSKTNT.KyHieu),'00000000-0000-0000-0000-000000000000')
	where NamLamViec = @nam
END


GO
/****** Object:  StoredProcedure [dbo].[sp_ncskt_getcomdata_ctct]    Script Date: 12/05/2020 6:17:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
	
	author:	hiep
	date:	08/05/2020
	decs:	Lấy số liệu chi tiết 3 năm làm căn cứ
	params:	

*/

CREATE PROCEDURE [dbo].[sp_ncskt_getcomdata_ctct]
	@nam int
AS
BEGIN 

	delete from SKT_ComData_ChungTuChiTiet where NamLamViec = @nam	
	declare @NamNS_3 int
	set @NamNS_3 = @nam - 1
	declare @NamNS_1 int
	declare @NamNS_2 int
	select	@NamNS_1 = (select CAST(SUBSTRING(s.NamNS_1,CHARINDEX('_',s.NamNS_1,0)+1,LEN(s.NamNS_1)) as int) from SKT_MapDataNS as s where s.NamLamViec = @nam),
			@NamNS_2 = (select CAST(SUBSTRING(s.NamNS_2,CHARINDEX('_',s.NamNS_1,0)+1,LEN(s.NamNS_2)) as int) from SKT_MapDataNS as s where s.NamLamViec = @nam)
	
	create table #TempNamT
	(
		namNS int, id_donvi nvarchar(5), id_phongban nvarchar(3), KyHieu nvarchar(12), TuChi decimal(18,0), MuaHang decimal(18,0), PhanCap decimal(18,0)
	)
				
	insert into #TempNamT
	select namNS, Id_Donvi, Id_PhongBan, KyHieu, TuChi, MuaHang, 0 from f_ncskt_ctctdtnt(@nam,null,null,1)
	
	insert into #TempNamT
	select namNS, Id_Donvi, Id_PhongBan, KyHieu, TuChi, MuaHang, PhanCap from f_ncskt_ctctsktnt(@nam,null,null,null,1)	
	
	insert into SKT_ComData_ChungTuChiTiet
	select		newid()
				, @nam
				, id_phongban 
				, id_donvi
				, Id 
				, TuChi_1 = sum(TuChi_1)
				, MuaHang_1 = sum(MuaHang_1)
				, TuChi_2 = sum(TuChi_2)
				, MuaHang_2 = sum(MuaHang_2)
				, TuChi_3 = sum(TuChi_3)
				, MuaHang_3 = sum(MuaHang_3)
				, PhanCap_3 = sum(PhanCap_3) 
				, GETDATE()
				, 'admin'
				, NULL
				, NULL
				, NULL
				, NULL
				, NULL
	from 
				(select KyHieu
						, id_donvi 
						, id_phongban 
						, TuChi_1 = TuChi
						, MuaHang_1 = MuaHang
						, TuChi_2 = 0
						, MuaHang_2 = 0
						, TuChi_3 = 0
						, MuaHang_3 = 0
						, PhanCap_3 = 0
				from	#TempNamT 
				where	namNS = @NamNS_1
			
				union all 

				select	KyHieu
						, id_donvi 
						, id_phongban 
						, TuChi_1 = 0
						, MuaHang_1 = 0
						, TuChi_2 = TuChi
						, MuaHang_2 = MuaHang
						, TuChi_3 = 0
						, MuaHang_3 = 0
						, PhanCap_3 = 0
				from	#TempNamT 
				where	namNS = @NamNS_2

				union all

				select	KyHieu
						, id_donvi 
						, id_phongban 
						, TuChi_1 = 0
						, MuaHang_1 = 0
						, TuChi_2 = 0
						, MuaHang_2 = 0
						, TuChi_3 = TuChi
						, MuaHang_3 = MuaHang
						, PhanCap_3 = PhanCap
				from	#TempNamT 
				where	namNS = @NamNS_3
				) as data

				left join 

				(
				select	KyHieu as Id
				from	SKT_MLNhuCau
				where	NamLamViec = @nam
				) as ml

				on ml.Id = data.KyHieu
	where		Id is not null	
	group by	Id, id_donvi, id_phongban
END


GO
/****** Object:  StoredProcedure [dbo].[sp_ncskt_getdacthu]    Script Date: 12/05/2020 6:17:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
	
	author:	hiep
	date:	08/05/2020
	decs:	Lấy mục lục số đặc thù năm trước
	params:	

*/

CREATE PROCEDURE [dbo].[sp_ncskt_getdacthu]
	@nam int
AS
BEGIN 

	delete from SKT_MLDacThu
	where NamLamViec = @nam		

	declare @NamNS int								
	set		@NamNS = @nam - 1	

	insert into		SKT_MLDacThu(M,Tm,Ttm,Ng,NamLamViec,NamNS)	
	select Distinct	M,TM,TTM,NG,@nam,@NamNS
	from			SKT_ComDatasDacThu
	where			NamNS=@NamNS		

	update SKT_MLDacThu
	set Id_MLNS = (select top (1) iID_MaMucLucNganSach from NS_MucLucNganSach where iTrangThai = 1 and sXauNoiMa = '1020100-010-011-' + M + '-' + Tm + '-' + Ttm + '-' + Ng and iNamLamViec = @NamNS)
END


GO
/****** Object:  StoredProcedure [dbo].[sp_ncskt_mapncmlns_chitietsheet]    Script Date: 12/05/2020 6:17:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
	
	author:	hiep
	date:	08/05/2020
	decs:	Lấy số liệu mapping nhập nhu cầu - số kiểm tra và MLNS chi tiết theo năm
	params:	

*/

CREATE PROCEDURE [dbo].[sp_ncskt_mapncmlns_chitietsheet]
	@nam int,
	@loai int,
	@Id_MLNhuCau uniqueidentifier,
	@LNS nvarchar(7),
	@L nvarchar(3),
	@K nvarchar(3),
	@M nvarchar(4),
	@TM nvarchar(4),
	@TTM nvarchar(2),
	@NG nvarchar(2)
AS
BEGIN 

	create table #Temp
	(
		 nam int, NamNS_1 int, NamNS_2 int 		 
	) 

	insert into #Temp(nam,NamNS_1,NamNS_2)
	values (@nam,@nam-2,@nam-1)	

	declare @NamNS_1 int								set @NamNS_1 = (select top(1) NamNS_1 from #Temp)
	declare @NamNS_2 int								set @NamNS_2 = (select top(1) NamNS_2 from #Temp)
			
	create table #TempNamT
	(
		id uniqueidentifier, mLNS nvarchar(7), mL nvarchar(3), mK nvarchar(3), mM nvarchar(4), mTM nvarchar(4), mTTM nvarchar(2), mNG nvarchar(2), MoTa nvarchar(Max)
	)
		
	insert into #TempNamT
	select * from f_skt_mlns(CONVERT(nvarchar,@NamNS_1)+','+CONVERT(nvarchar,@NamNS_2),@nam)

	select		Id
				, Id_MLNhuCau
				, mXau as Xau
				, mLNS as Lns
				, mL as L
				, mK as K
				, mM as M
				, mTM as TM
				, mTTM as TTM
				, mNG as NG
				, MoTa
				, Map = CASE WHEN Id Is not null then N'Chọn' else N'Không chọn' end
	from	
				(select	Id, Id_MLNhuCau, Xau
				from	SKT_MLNS
				where	NamLamViec = @nam) map

				FULL JOIN

				(select	mLNS
						, mL
						, mK
						, mM
						, mTM
						, mTTM
						, mNG
						, mXau = mLns+'-'+mL+'-'+mK+'-'+mM+'-'+mTM+'-'+mTTM+'-'+mNG
						, MoTa
				from	#TempNamT 
				where	(@LNS is null or mLNS like @LNS)
						and (@L is null or mL like @L)
						and (@K is null or mK like @K)
						and (@M is null or mM like @M)
						and (@TM is null or mTM like @TM)
						and (@TTM is null or mTTM like @TTM)
						and (@NG is null or mNG like @NG)) as ml
				ON map.Xau = ml.mXau
	where		(@loai = 1 and Id is null) or (@loai = 2 and Id is not null and Id_MLNhuCau = @Id_MLNhuCau)			
	order by	mXau
END


GO
/****** Object:  StoredProcedure [dbo].[sp_ncskt_mapncmlns_sheet]    Script Date: 12/05/2020 6:17:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
	
	author:	hiep
	date:	08/05/2020
	decs:	Lấy bảng mục lục nhu cầu theo năm ngân sách
	params:	

*/

CREATE PROCEDURE [dbo].[sp_ncskt_mapncmlns_sheet]
	@NamLamViec int,
	@Nganh_Parent nvarchar(3),
	@Nganh nvarchar(2),
	@KyHieu  nvarchar(12)
AS
BEGIN 

	select		mlnc.*, Map = CASE WHEN idmap IS NOT NULL THEN N'Đã map' ELSE N'Chưa map' end
	from
				(
				select		Id
							, Loai = CASE WHEN	KyHieu LIKE '1-2%' THEN N'Ngân sách nghiệp vụ'
										  WHEN  KyHieu LIKE '1-3%' THEN N'Việc nhà nước giao tính vào KPQP'
										  WHEN  KyHieu LIKE '2%' THEN N'Chi hoạt động sự nghiệp'
										  WHEN  KyHieu LIKE '3%' THEN N'Ngân sách nhà nước khác'
										  ELSE N'Bảo đảm đời sống' END
							, Nganh_Parent
							, Nganh
							, KyHieu
							, MoTa
				from		SKT_MLNhuCau 
				where		NamLamViec=@NamLamViec
							and IsParent = 0) mlnc

				full join 

				(
				select		DISTINCT Id_MLNhuCau as idmap
				from		SKT_MLNS
				where		NamLamViec = @NamLamViec
				) map
				on 
				mlnc.Id =  map.idmap
	where		(@KyHieu is null or KyHieu like @KyHieu)
				and (@Nganh is null or Nganh like @Nganh)
				and (@Nganh_Parent is null or Nganh_Parent like @Nganh_Parent)
	order by	KyHieu
END


GO
/****** Object:  StoredProcedure [dbo].[sp_ncskt_mapncmskt_chitietsheet]    Script Date: 12/05/2020 6:17:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
	
	author:	hiep
	date:	08/05/2020
	decs:	Lấy bảng mục lục nhu cầu theo năm ngân sách
	params:	

*/

CREATE PROCEDURE [dbo].[sp_ncskt_mapncmskt_chitietsheet]
	@NamLamViec int,
	@loai int,
	@Id_MLNhuCau uniqueidentifier,
	@Nganh_Parent nvarchar(3),
	@Nganh nvarchar(2),
	@KyHieu  nvarchar(12)
AS
BEGIN 

	select		Id,Id_MLNhuCau,Id_MaSKT,Nganh,Nganh_Parent,KyHieu,MoTa,Map = CASE WHEN Id Is not null then N'Chọn' else N'Không chọn' end
	from	
				(select		Id,Id_MLNhuCau,Id_MLSKTNT
				 from		SKT_NCSKT
				 where		NamLamViec = @NamLamViec) map

				FULL JOIN

				(
				select		Id as Id_MaSKT
							, Nganh_Parent
							, Nganh
							, KyHieu
							, STT + ' ' + MoTa as MoTa						
				from		SKT_MLSKTNT
				where		NamLamViec = @NamLamViec						
							and IsParent = 0
							and (@KyHieu is null or KyHieu like @KyHieu)
							and (@Nganh is null or Nganh like @Nganh)
							and (Nganh_Parent is null or @Nganh_Parent is null or Nganh_Parent like @Nganh_Parent)	
							and KyHieu in (select KyHieu from SKT_ComDatasSKTNT	where NamLamViec = @NamLamViec)				
				) as ml
				ON map.Id_MLSKTNT = ml.Id_MaSKT		
	where		(@loai = 1 and Id is null) or (@loai = 2 and Id is not null and Id_MLNhuCau = @Id_MLNhuCau)	
	order by	KyHieu
END


GO
/****** Object:  StoredProcedure [dbo].[sp_ncskt_mapncmskt_sheet]    Script Date: 12/05/2020 6:17:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
	
	author:	hiep
	date:	08/05/2020
	decs:	Lấy bảng mục lục nhu cầu theo năm ngân sách
	params:	

*/

CREATE PROCEDURE [dbo].[sp_ncskt_mapncmskt_sheet]
	@NamLamViec int,
	@Nganh_Parent nvarchar(3),
	@Nganh nvarchar(2),
	@KyHieu  nvarchar(12)
AS
BEGIN 

	select		mlnc.*, Map = CASE WHEN idmap IS NOT NULL THEN N'Đã map' ELSE N'Chưa map' end
	from
				(
				select		Id
							, Loai = CASE WHEN	KyHieu LIKE '1-2%' THEN N'Ngân sách nghiệp vụ'
										  WHEN  KyHieu LIKE '1-3%' THEN N'Việc nhà nước giao tính vào KPQP'
										  WHEN  KyHieu LIKE '2%' THEN N'Chi hoạt động sự nghiệp'
										  WHEN  KyHieu LIKE '3%' THEN N'Ngân sách nhà nước khác'
										  ELSE N'Bảo đảm đời sống' END
							, Nganh_Parent
							, Nganh
							, KyHieu
							, MoTa
				from		SKT_MLNhuCau 
				where		NamLamViec=@NamLamViec
							and IsParent = 0) mlnc

				full join 

				(
				select		DISTINCT Id_MLNhuCau as idmap
				from		SKT_NCSKT
				where		NamLamViec = @NamLamViec
				) map
				on 
				mlnc.Id =  map.idmap
	where		(@KyHieu is null or KyHieu like @KyHieu)
				and (@Nganh is null or Nganh like @Nganh)
				and (@Nganh_Parent is null or Nganh_Parent like @Nganh_Parent)
	order by	KyHieu
END


GO
/****** Object:  StoredProcedure [dbo].[sp_ncskt_mlncsheet]    Script Date: 12/05/2020 6:17:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
	
	author:	hiep
	date:	08/05/2020
	decs:	Lấy bảng mục lục nhu cầu theo năm ngân sách
	params:	

*/

CREATE PROCEDURE [dbo].[sp_ncskt_mlncsheet]
	@NamLamViec int,
	@Loai nvarchar(10),
	@Nganh_Parent nvarchar(3),
	@Nganh nvarchar(2),
	@M nvarchar(4),
	@KyHieu  nvarchar(12)
AS
BEGIN 

	select		Id
				, Id_Parent
				, IsParent
				, KyHieu
				, Loai
				, Nganh_Parent
				, Nganh
				, M
				, STT
				, MoTa 
				, KyHieuCha
	from		SKT_MLNhuCau 
	where		NamLamViec=@NamLamViec
				and (@Loai is null or Loai like @Loai)
				and (@KyHieu is null or KyHieu like @KyHieu)
				and (@Nganh is null or Nganh like @Nganh)
				and (@M is null or M like @M)
				and (Nganh_Parent is null or @Nganh_Parent is null or Nganh_Parent like @Nganh_Parent)
	order by	KyHieu
END


GO
/****** Object:  StoredProcedure [dbo].[sp_ncskt_mlsktsheet]    Script Date: 12/05/2020 6:17:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
	
	author:	hiep
	date:	08/05/2020
	decs:	Lấy bảng mục lục nhu cầu theo năm ngân sách
	params:	

*/

CREATE PROCEDURE [dbo].[sp_ncskt_mlsktsheet]
	@NamLamViec int,
	@Loai nvarchar(10),
	@Nganh_Parent nvarchar(3),
	@Nganh nvarchar(2),
	@M nvarchar(4),
	@KyHieu  nvarchar(12)
AS
BEGIN 

	select		Id
				, Id_Parent
				, IsParent
				, KyHieu
				, Loai
				, Nganh_Parent
				, Nganh
				, M
				, STT
				, MoTa 
				, KyHieuCha
	from		SKT_MLSKTNT 
	where		NamLamViec=@NamLamViec
				and (@Loai is null or Loai like @Loai)
				and (@KyHieu is null or KyHieu like @KyHieu)
				and (@Nganh is null or Nganh like @Nganh)
				and (@M is null or M like @M)
				and (Nganh_Parent is null or @Nganh_Parent is null or Nganh_Parent like @Nganh_Parent)
	order by	KyHieu
END


GO
/****** Object:  StoredProcedure [dbo].[sp_ncskt_report_ghichu]    Script Date: 12/05/2020 6:17:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_ncskt_report_ghichu]
	@nam	int,
	@loai   int,
	@phongban nvarchar(3),
	@donvi nvarchar(5)
AS
BEGIN 
	
	 select	ml.Nganh, mlns.TenNganh, ml.KyHieu, ml.MoTa, GhiChu 
	 from
			(
			select	Id_MucLuc, GhiChu 
			from	SKT_ChungTuChiTiet
			where	GhiChu is not null
					and Id_ChungTu in (
									  select id from SKT_ChungTu
									  where	 NamLamViec=@nam
											 and iLoai=@loai
											 and Id_PhongBan=@phongban
											 and Id_DonVi=@donvi)
			) as ct

			left join 
			
			(
			select	* 
			from	SKT_MLNhuCau 
			where	NamLamViec=@nam
			) as ml

			on ct.Id_MucLuc=ml.Id

			left join 
			(
			select	sMoTa as TenNganh, sNg 
			from	NS_MucLucNganSach 
			where	iNamLamViec = @nam and sNg <> '' and sLNS = ''
			) as mlns

			on ml.Nganh = mlns.sNG

	order by KyHieu
END


GO
/****** Object:  StoredProcedure [dbo].[sp_ncskt_report_ik01]    Script Date: 12/05/2020 6:17:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_ncskt_report_ik01]
	@nam	int,
	@dvt	int,
	@phongban nvarchar(3),
	@donvi nvarchar(999)
AS
BEGIN 
	
	declare @NamNS_1 int								
	select @NamNS_1 = CAST(SUBSTRING(s.NamNS_1,CHARINDEX('_',s.NamNS_1,0)+1,LEN(s.NamNS_1)) as int) from SKT_MapDataNS as s where s.NamLamViec = @nam
	declare @NamNS_2 int								
	select @NamNS_2 = CAST(SUBSTRING(s.NamNS_2,CHARINDEX('_',s.NamNS_1,0)+1,LEN(s.NamNS_2)) as int) from SKT_MapDataNS as s where s.NamLamViec = @nam
	declare @NamNS_3 int								
	select @NamNS_3 = @nam - 1
	
	create table #TempNamT
	(
		namNS int, KyHieu nvarchar(12), TuChi decimal(18,0)
	)
				
	insert into #TempNamT
	select namNS, KyHieu, TuChi from f_ncskt_ctctdtnt(@nam,@donvi,@phongban,@dvt)
	
	insert into #TempNamT
	select namNS, KyHieu, TuChi from f_ncskt_ctctsktnt(@nam,1,@donvi,@phongban,@dvt)	
	
	select		X1=SUBSTRING(KyHieu,1,1)
				, X2=SUBSTRING(KyHieu,1,3)
				, X3=SUBSTRING(KyHieu,1,6)
				, X4=SUBSTRING(KyHieu,1,9)
				, Nganh
				, KyHieu
				, MoTa
				, C1 = SUM(C1)
				, C2 = SUM(C2)
				, C3 = SUM(C3)
				, TonKho_DV = SUM(TonKho_DV)
				, HuyDong_DV = SUM(HuyDong_DV)
				, TuChi_DV = SUM(TuChi_DV)
				, TuChi = SUM(TuChi)
	from	
				(
				select		KyHieu, C1 = TuChi, C2 = 0, C3 = 0, TonKho_DV = 0, HuyDong_DV = 0, TuChi_DV = 0, TuChi = 0
				from		#TempNamT
				where		TuChi <> 0 and namNS = @NamNS_1

				union all

				select		KyHieu, C1 = 0, C2 = TuChi, C3 = 0, TonKho_DV = 0, HuyDong_DV = 0, TuChi_DV = 0, TuChi = 0
				from		#TempNamT
				where		TuChi <> 0 and namNS = @NamNS_2

				union all

				select		KyHieu, C1 = 0, C2 = 0, C3 = TuChi, TonKho_DV = 0, HuyDong_DV = 0, TuChi_DV = 0, TuChi = 0
				from		#TempNamT
				where		TuChi <> 0 and namNS = @NamNS_3

				union all
				select		KyHieu, C1 = 0, C2 = 0, C3 = 0, TonKho_DV, HuyDong_DV, TuChi_DV, TuChi
				from		f_ncskt_ctctnssdnn(@nam,@donvi,@phongban,@dvt)
				where       (TonKho_DV + HuyDong_DV + TuChi_DV + TuChi) <> 0
				) ct

				left join 

				(
				select	KyHieu as kh_nc
						, Nganh
						, MoTa
				from	SKT_MLNhuCau
				where	NamLamViec = @nam
				) ml 
			
				on ct.KyHieu = ml.kh_nc
	where		KyHieu is not null		
	group by	KyHieu, MoTa, Nganh
	having		sum(C1) <> 0 or sum(C2) <> 0 or sum(C3) <> 0 or sum(TonKho_DV) <> 0	or sum(HuyDong_DV) <> 0 or sum(TuChi_DV) <> 0 or sum(TuChi) <> 0
END


GO
/****** Object:  StoredProcedure [dbo].[sp_ncskt_report_tk01]    Script Date: 12/05/2020 6:17:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_ncskt_report_tk01]
	@nam	int,
	@dvt	int,
	@phongban nvarchar(3),
	@donvi nvarchar(5)
AS
BEGIN 
	
	select		X1=SUBSTRING(KyHieu,1,1)
				, X2=SUBSTRING(KyHieu,1,3)
				, X3=SUBSTRING(KyHieu,1,6)
				, X4=SUBSTRING(KyHieu,1,9)
				, KyHieu
				, MoTa
				, C1 = SUM(C1)/@dvt
				, C2 = SUM(C2)/@dvt
				, C3 = SUM(C3)/@dvt
	from	
				(
				select		KyHieu, C1 = TuChi_1, C2 = TuChi_2, C3 = TuChi_3
				from		SKT_ComData_ChungTuChiTiet
				where		(TuChi_1 + TuChi_2 + TuChi_3) <> 0 
							and NamLamViec = @nam
							and Id_PhongBan = @phongban
							and Id_DonVi = @donvi				
				) ct

				left join 

				(
				select	KyHieu as kh_nc
						, MoTa
				from	SKT_MLNhuCau
				where	NamLamViec = @nam
				) ml 
			
				on ct.KyHieu = ml.kh_nc
	where		KyHieu is not null		
	group by	KyHieu, MoTa
	having		sum(C1) <> 0 or sum(C2) <> 0 or sum(C3) <> 0
END


GO
/****** Object:  StoredProcedure [dbo].[sp_ncskt_report_tk02]    Script Date: 12/05/2020 6:17:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_ncskt_report_tk02]
	@nam	int,
	@dvt	int,
	@phongban nvarchar(3),
	@nganh nvarchar(5)
AS
BEGIN 
	
	select		X1=SUBSTRING(KyHieu,1,1)
				, X2=SUBSTRING(KyHieu,1,3)
				, X3=SUBSTRING(KyHieu,1,6)
				, X4=SUBSTRING(KyHieu,1,9)
				, KyHieu
				, MoTa
				, C1 = SUM(C1)/@dvt
				, C2 = SUM(C2)/@dvt
				, C3 = SUM(C3)/@dvt
				, C4 = SUM(C4)/@dvt
	from	
				(
				select		KyHieu, C1 = MuaHang_1, C2 = MuaHang_2, C3 = MuaHang_3, C4 = PhanCap_3
				from		SKT_ComData_ChungTuChiTiet
				where		(MuaHang_1 + MuaHang_2 + MuaHang_3 + PhanCap_3) <> 0 
							and NamLamViec = @nam
							and Id_DonVi = @nganh
							and Id_PhongBan = @phongban
				) ct

				left join 

				(
				select	KyHieu as kh_nc
						, MoTa
				from	SKT_MLNhuCau
				where	NamLamViec = @nam
				) ml 
			
				on ct.KyHieu = ml.kh_nc
	where		KyHieu is not null		
	group by	KyHieu, MoTa
	having		sum(C1) <> 0 or sum(C2) <> 0 or sum(C3) <> 0 or sum(C4) <> 0
END


GO
/****** Object:  StoredProcedure [dbo].[sp_ncskt_report_tk03]    Script Date: 12/05/2020 6:17:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_ncskt_report_tk03]
	@nam	int,
	@dvt	int,
	@phongban nvarchar(3),
	@donvis nvarchar(999)
AS
BEGIN 
	
	select		X1=SUBSTRING(KyHieu,1,1)
				, X2=SUBSTRING(KyHieu,1,3)
				, X3=SUBSTRING(KyHieu,1,6)
				, X4=SUBSTRING(KyHieu,1,9)
				, Id_DonVi
				, TenDonVi
				, KyHieu
				, MoTa
				, C1 = SUM(C1)/@dvt
				, C2 = SUM(C2)/@dvt
				, C3 = SUM(C3)/@dvt
	from	
				(
				select		Id_DonVi, KyHieu, C1 = TuChi_1, C2 = TuChi_2, C3 = TuChi_3
				from		SKT_ComData_ChungTuChiTiet
				where		(TuChi_1 + TuChi_2 + TuChi_3) <> 0  
							and NamLamViec = @nam	
							and Id_PhongBan = @phongban
							and Id_DonVi in (select * from f_split(@donvis))	
				) ct

				left join 

				(
				select	KyHieu as kh_nc
						, MoTa
				from	SKT_MLNhuCau
				where	NamLamViec = @nam
				) ml 
			
				on ct.KyHieu = ml.kh_nc

				left join 

				(
				select	iID_MaDonVi, TenDonVi = iID_MaDonVi + ' - ' + sTen
				from	NS_DonVi
				where	iTrangThai = 1 and iNamLamViec_DonVi = @nam
				) dv

				on ct.Id_DonVi = dv.iID_MaDonVi
	where		KyHieu is not null		
	group by	KyHieu, MoTa, Id_DonVi, TenDonVi
	having		sum(C1) <> 0 or sum(C2) <> 0 or sum(C3) <> 0
END


GO
/****** Object:  UserDefinedFunction [dbo].[f_ncskt_ctctdtnt]    Script Date: 12/05/2020 6:17:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
	
	author:	hiep
	date:	08/05/2020
	decs:	Lấy chỉ tiêu ngân sách năm trước theo năm ngân sách 
	params:	

*/

CREATE FUNCTION [dbo].[f_ncskt_ctctdtnt]
	(		
	@nam int,
	@id_donvi nvarchar(999),
	@id_phongban nvarchar(20),
	@dvt int
	)

RETURNS @NS_NT	TABLE 
	(	
		namNS int
		, Id_Donvi nvarchar(5)
		, Id_PhongBan nvarchar(3)
		, KyHieu nvarchar(12)
		, TuChi float
		, MuaHang float
	) 
AS
	BEGIN

	insert into @NS_NT(namNS,Id_Donvi,Id_PhongBan,KyHieu,TuChi,MuaHang)
	select	NamNS 
			, Id_Donvi
			, Id_PhongBan
			, KyHieu
			, TuChi = case when KyHieu in ('1-1-04-01-00') then SUM(TuChi+HangNhap) else sum(TuChi) end
			, MuaHang = case when KyHieu in ('1-1-04-01-00') then SUM(HangMua) else SUM(HangNhap + HangMua) end 	
	from
			(			
			select	XauNoiMa
					, NamNS
					, Id_Donvi
					, Id_PhongBan
					, TuChi		= sum(TuChi) /@dvt
					, HangNhap	= sum(HangNhap) /@dvt											
					, HangMua	= sum(HangMua) /@dvt
			from	SKT_ComDatas
			where	NamLamViec = @nam
					and (@id_phongban is null or Id_PhongBan in (select * from f_split(@id_phongban)))
					and (@id_donvi is null or Id_DonVi in (select * from f_split(@id_donvi)))	
			group by NamNS,XauNoiMa,Id_Donvi,Id_PhongBan

			union all

			select	XauNoiMa
					, NamNS
					, Id_Donvi
					, Id_PhongBan
					, TuChi		= sum(TuChi) /@dvt
					, HangNhap	= 0											
					, HangMua	= 0
			from	SKT_ComDatasDacThu
			where	NamLamViec = @nam
					and (@id_phongban is null or Id_PhongBan in (select * from f_split(@id_phongban)))
					and (@id_donvi is null or Id_DonVi in (select * from f_split(@id_donvi)))	
					and XauNoiMa <> '1020100-010-011-6950-6954-10-23'
			group by NamNS,XauNoiMa,Id_Donvi,Id_PhongBan
			) as t1 
		
			left join 
		
			(
			select		KyHieu, Xau
			from		SKT_MLNS
			where		NamLamViec = @nam 
			) as map

			 on t1.XauNoiMa = map.Xau
	group by NamNS,KyHieu,Id_Donvi,Id_PhongBan

RETURN
END

GO
/****** Object:  UserDefinedFunction [dbo].[f_ncskt_ctctnganhnn]    Script Date: 12/05/2020 6:17:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
	
	author:	hiep
	date:	08/05/2020
	decs:	Lấy chi tiết số kiểm tra năm nay theo năm ngân sách, loại ngân sách (nssd, nsbd)
	params:	

*/

CREATE FUNCTION [dbo].[f_ncskt_ctctnganhnn]
	(		
	@nam int,
	@donvi nvarchar(5),
	@phongban nvarchar(3),
	@dvt int
	)

RETURNS @NS_NT	TABLE 
	(	
		KyHieu nvarchar(12)
		, TonKho_DV float	
		, HuyDong_DV float
		, TuChi_DV float	
		, MuaHang_DV float
		, PhanCap_DV float
		, HuyDong float	 
		, MuaHang float
		, PhanCap float
	) 
AS
	BEGIN

	insert into @NS_NT
	select	KyHieu
			, TonKho_DV = sum(TonKho_DV) /@dvt	
			, HuyDong_DV = sum(HuyDong_DV) /@dvt
			, TuChi_DV = sum(TuChi_DV) /@dvt	
			, MuaHang_DV = sum(MuaHang_DV) /@dvt
			, PhanCap_DV = sum(PhanCap_DV) /@dvt
			, HuyDong = sum(HuyDong) /@dvt
			, MuaHang = sum(MuaHang) /@dvt
			, PhanCap = sum(PhanCap) /@dvt			
	from
			(			
			select	Id_MucLuc
					, TonKho_DV	
					, HuyDong_DV
					, TuChi_DV	
					, MuaHang_DV
					, PhanCap_DV
					, HuyDong
					, MuaHang	
					, PhanCap	
			from	SKT_ChungTuChiTiet
			where	Id_ChungTu in 
					(select Id 
					 from SKT_ChungTu 
					 where	NamLamViec=@nam
							and iloai=2
							and (@donvi is null or Id_DonVi = @donvi)
							and (@phongban is null or Id_PhongBan=@phongban))
			) as t1 
		
			left join 
		
			(
			select		Id, KyHieu
			from		SKT_MLNhuCau
			where		NamLamViec = @nam 			 
			) as ml

			 on t1.Id_MucLuc = ml.Id
	group by KyHieu
	
RETURN
END

GO
/****** Object:  UserDefinedFunction [dbo].[f_ncskt_ctctnssdnn]    Script Date: 12/05/2020 6:17:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
	
	author:	hiep
	date:	08/05/2020
	decs:	Lấy chi tiết số kiểm tra năm nay theo năm ngân sách, loại ngân sách (nssd, nsbd)
	params:	

*/

CREATE FUNCTION [dbo].[f_ncskt_ctctnssdnn]
	(		
	@nam int,
	@donvi nvarchar(999),
	@phongban nvarchar(20),
	@dvt int
	)

RETURNS @NS_NT	TABLE 
	(	
		KyHieu nvarchar(12)
		, Id_DonVi nvarchar(5)
		, TonKho_DV float	
		, HuyDong_DV float
		, TuChi_DV float	
		, MuaHang_DV float
		, PhanCap_DV float
		, TuChi float
	) 
AS
	BEGIN

	insert into @NS_NT
	select	KyHieu
			, Id_DonVi
			, TonKho_DV = sum(TonKho_DV) /@dvt	
			, HuyDong_DV = sum(HuyDong_DV) /@dvt
			, TuChi_DV = sum(TuChi_DV) /@dvt	
			, MuaHang_DV = sum(MuaHang_DV) /@dvt
			, PhanCap_DV = sum(PhanCap_DV) /@dvt
			, TuChi	  = sum(TuChi) /@dvt			
	from
			(			
			select	Id_ChungTu
					, Id_MucLuc
					, TonKho_DV	
					, HuyDong_DV
					, TuChi_DV	
					, MuaHang_DV
					, PhanCap_DV
					, TuChi		
			from	SKT_ChungTuChiTiet
			) as t1 
		
			left join 
		
			(
			select	Id, KyHieu
			from	SKT_MLNhuCau
			where	NamLamViec = @nam 			 
			) as ml

			 on t1.Id_MucLuc = ml.Id

			 
			left join
			  
			(
			select	Id, Id_DonVi 
			from	SKT_ChungTu 
			where	NamLamViec=@nam
					and iloai=1
					and (@donvi is null or Id_DonVi in (select * from f_split(@donvi)))
					and (@phongban is null or Id_PhongBan in (select * from f_split(@phongban)))
			) as ct

			on t1.Id_ChungTu = ct.Id
	group by KyHieu, Id_DonVi
	
RETURN
END

GO
/****** Object:  UserDefinedFunction [dbo].[f_ncskt_ctctsktnt]    Script Date: 12/05/2020 6:17:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
	
	author:	hiep
	date:	08/05/2020
	decs:	Lấy số kiểm tra năm trước theo năm ngân sách 
	params:	

*/

CREATE FUNCTION [dbo].[f_ncskt_ctctsktnt]
	(		
	@nam int,
	@loai nvarchar(1),
	@id_donvi nvarchar(999),
	@id_phongban nvarchar(20),
	@dvt int
	)

RETURNS @NS_NT	TABLE 
	(	
		namNS int
		, Id_Donvi nvarchar(5)
		, Id_PhongBan nvarchar(3)
		, KyHieu nvarchar(12)
		, TuChi float
		, MuaHang float
		, PhanCap float
	) 
AS
	BEGIN

	insert into @NS_NT
	select	NamNS 
			, Id_Donvi 
			, Id_PhongBan
			, KyHieu
			, TuChi	  = sum(TuChi)
			, MuaHang = sum(MuaHang)
			, PhanCap = sum(PhanCap)
	from
			(			
			select	KyHieu as Kh_SKT
					, Id_Donvi 
					, Id_PhongBan = Id_PhongBanDich
					, NamNS
					, TuChi		= sum(TuChi) /@dvt
					, MuaHang	= sum(MuaHang) /@dvt
					, PhanCap	= sum(PhanCap) /@dvt
			from	SKT_ComDatasSKTNT
			where	NamLamViec = @nam
					and (@loai is null or iLoai = @loai)
					and (@id_phongban is null or Id_PhongBanDich in (select * from f_split(@id_phongban)))
					and (@id_donvi is null or Id_DonVi in (select * from f_split(@id_donvi)))
			group by NamNS,KyHieu,Id_Donvi,Id_PhongBanDich
			) as t1 
		
			left join 
		
			(
			select		KyHieu, KyHieu_SKT
			from		SKT_NCSKT
			where		NamLamViec = @nam 			 
			) as map

			 on t1.Kh_SKT = map.KyHieu_SKT
	group by NamNS,KyHieu,Id_Donvi,Id_PhongBan

RETURN
END

GO
/****** Object:  UserDefinedFunction [dbo].[f_ncskt_dtdtnt]    Script Date: 12/05/2020 6:17:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
	
	author:	hiep
	date:	08/05/2020
	decs:	Lấy số chỉ tiêu đơn vị được phân cấp đợt đầu năm theo năm ngân sách
	params:	

*/

CREATE FUNCTION [dbo].[f_ncskt_dtdtnt]
	(		
	@namNS int
	)

RETURNS TABLE 	
AS			
RETURN	
	select		iID_MaDonVi
				, iID_MaPhongBan = iID_MaPhongBanDich
				, sLNS
				, sL
				, sK
				, sM
				, sTM
				, sTTM
				, sNG
				, TuChi		= ISNULL(sum(TuChi),0)
	from		(
				select	iID_MaDonVi
						, iID_MaPhongBanDich
						, sLNS
						, sL
						, sK
						, sM
						, sTM
						, sTTM
						, sNG
						, TuChi		= rTuChi
				from	DT_ChungTuChiTiet_PhanCap
				where	iTrangThai in (1,2)
						and iNamLamViec=@NamNS
						and sLNS in (1020100) AND MaLoai in ('','2')
						and rTuChi <> 0

				union all

				select	iID_MaDonVi
						, iID_MaPhongBanDich
						, sLNS = '1020100'
						, sL
						, sK
						, sM
						, sTM
						, sTTM
						, sNG
						, TuChi		= (rTuChi + rHangNhap + rHangMua)						
				from	DT_ChungTuChiTiet
				where	iTrangThai in (1,2)
						and iNamLamViec=@NamNS
						and MaLoai = '2'
						and sLNS='1040100' 
						and (rTuChi + rHangNhap + rHangMua) <> 0
				) as re
	where		iID_MaDonVi IS NOT NULL 
				and iID_MaDonVi <> ''
				and (iID_MaPhongBanDich IS NOT NULL or iID_MaPhongBanDich <> '')
				and TuChi <> 0
	group by	iID_MaDonVi,iID_MaPhongBanDich,sLNS,sL,sK,sM,sTM,sTTM,sNG

GO
/****** Object:  UserDefinedFunction [dbo].[f_ncskt_dtnt]    Script Date: 12/05/2020 6:17:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
	
	author:	hiep
	date:	08/05/2020
	decs:	Lấy số chỉ tiêu đơn vị tự lập dự toán chi tiết theo năm ngân sách 
	params:	

*/

CREATE FUNCTION [dbo].[f_ncskt_dtnt]
	(		
	@namNS int
	)

RETURNS TABLE 	
AS			
RETURN	
	select		iID_MaDonVi
				, iID_MaPhongBan = iID_MaPhongBanDich
				, sLNS
				, sL
				, sK
				, sM
				, sTM
				, sTTM
				, sNG
				, TuChi		= ISNULL(sum(TuChi),0)
				, HangNhap	= ISNULL(sum(HangNhap),0)
				, HangMua	= ISNULL(sum(HangMua),0)
	from		(
				select		iID_MaDonVi
							, iID_MaPhongBanDich
							, sLNS
							, sL
							, sK
							, sM
							, sTM
							, sTTM
							, sNG
							, TuChi		= sum(rTuChi)
							, HangNhap	= sum(rHangNhap) 
							, HangMua	= sum(rHangMua)
				from		DT_ChungTuChiTiet
				where		iTrangThai <> 0
							and iNamLamViec=@NamNS
							and MaLoai in ('')
				group by	iID_MaDonVi,iID_MaPhongBanDich,sLNS,sL,sK,sM,sTM,sTTM,sNG				
				) as re
	where		iID_MaDonVi IS NOT NULL 
				and iID_MaDonVi <> ''
				and (iID_MaPhongBanDich IS NOT NULL or iID_MaPhongBanDich <> '')
				and (TuChi + HangNhap + HangMua) <> 0
	group by	iID_MaDonVi,iID_MaPhongBanDich,sLNS,sL,sK,sM,sTM,sTTM,sNG

GO
/****** Object:  UserDefinedFunction [dbo].[f_ncskt_qtnt]    Script Date: 12/05/2020 6:17:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
	
	author:	hiep
	date:	08/05/2020
	decs:	Lấy số quyết toán theo năm ngân sách 
	params:	

*/

CREATE FUNCTION [dbo].[f_ncskt_qtnt]
	(		
	@namNS int
	)

RETURNS TABLE 	
AS			
RETURN	
	select		iID_MaDonVi
				, iID_MaPhongBan
				, sLNS
				, sL
				, sK
				, sM
				, sTM
				, sTTM
				, sNG
				, TuChi		= CASE when sLNS not in ('1040200', '1040300') then sum(rTuChi) 
										else 0 end
				, HangNhap	= CASE sLNS when '1040200' then sum(rTuChi) 
										else 0 end
				, HangMua	= CASE sLNS when '1040300' then sum(rTuChi)
										else 0 end
	from		QTA_ChungTuChiTiet
	where		iTrangThai=1
				and iNamLamViec=@namNS
				and (iID_MaDonVi IS NOT NULL or iID_MaDonVi <> '')
				and (iID_MaPhongBan IS NOT NULL or iID_MaPhongBan <> '')
				and rTuChi <> 0
				and iID_MaNamNganSach in ('2','4')
				and sLNS not like '3%'
	group by	iID_MaDonVi,iID_MaPhongBan,sLNS,sL,sK,sM,sTM,sTTM,sNG

GO
