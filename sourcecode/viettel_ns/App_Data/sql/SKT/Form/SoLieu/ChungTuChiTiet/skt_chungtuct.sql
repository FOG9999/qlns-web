USE [VIETTEL_NS1]
GO
/****** Object:  StoredProcedure [dbo].[skt_chungtuct]    Script Date: 09/07/2019 3:27:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[skt_chungtuct]
	@id uniqueidentifier,
	@dvt	int,
	@Nganh nvarchar(10),
	@Nganh_Parent nvarchar(10)
AS
BEGIN 

	create table #Temp
	(
		 nam int, NamNS_1 int, NamNS_2 int, id_DonVi nvarchar(3), id_PhongBan nvarchar(2), id_PhongBanDich nvarchar(2), loai nvarchar(1), byNg nvarchar(10)  		 
	) 

	insert into #Temp(nam,NamNS_1,NamNS_2,id_DonVi,id_PhongBan,id_PhongBanDich,loai)
	select NamLamViec,NamLamViec-1,NamLamViec-2,Id_DonVi,Id_PhongBan,Id_PhongBanDich,iLoai from SKT_ChungTu where Id = @id

	declare @NamLamViec int								set @NamLamViec = (select top(1) nam from #Temp)
	declare @id_DonVi nvarchar(10)						set @id_DonVi = (select top(1) id_DonVi from #Temp)
	declare @NamNS_1 int								set @NamNS_1 = (select top(1) NamNS_1 from #Temp)
	declare @NamNS_2 int								set @NamNS_2 = (select top(1) NamNS_2 from #Temp)
	declare @id_PhongBanDich nvarchar(10)				set @id_PhongBanDich= (select top(1) id_PhongBanDich from #Temp)
	declare @id_PhongBan nvarchar(10)					set @id_PhongBan= (select top(1) id_PhongBan from #Temp)
	declare @loai nvarchar(1)							set @loai= (select top(1) loai from #Temp) 
	declare @byNg nvarchar(1000)						set @byNg= null

	if (@loai = 2)
		set @byNg = (select top(1) id_DonVi from #Temp)
	
	create table #TempNamT
	(
		namNS int, id_NhuCau uniqueidentifier, TuChi decimal(18,0), MuaHang decimal(18,0)
	)
				
	insert into #TempNamT(namNS,id_NhuCau,TuChi,MuaHang)
	select namNS, Id_MLNhuCau, TuChi, MuaHang from f_skt_ctct(@NamLamViec,CONVERT(nvarchar,@NamNS_1)+','+CONVERT(nvarchar,@NamNS_2),@id_DonVi,@id_PhongBanDich,@dvt)		
	
	create table #Result
	(
		Id uniqueidentifier, Id_Parent uniqueidentifier, IsParent bit, KyHieu nvarchar(13), Nganh_Parent nvarchar(3), Nganh nvarchar(2), MoTa nvarchar(MAX),
		Id_ChungTuChiTiet uniqueidentifier, TuChi_DTT_2 decimal(18,0), MuaHang_DTT_2 decimal(18,0), TuChi_DTT_1 decimal(18,0), MuaHang_DTT_1 decimal(18,0), 
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
				, Tang = 0
				, Giam = 0
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
						, GhiChu 
				from	SKT_ChungTuChiTiet 
				where	(Id_ChungTu=@id)
				) as ct

				on ct.Id_Mucluc=ml.Id 

				left join 

				(select id_NhuCau as KHN_1
						, TuChi as TuChi_DTT_1
						, MuaHang as MuaHang_DTT_1 
				 from	#TempNamT 
				 where	namNS = @NamNS_1
				 ) as namN_1

				 on ml.Id = namN_1.KHN_1

				 left join 

				(select id_NhuCau as KHN_2
						, TuChi as TuChi_DTT_2
						, MuaHang as MuaHang_DTT_2
				 from	#TempNamT 
				 where	namNS = @NamNS_2
				 ) as namN_2

				 on ml.Id = namN_2.KHN_2

	where		(@Nganh is null or Nganh like @Nganh)
				and (Nganh_Parent is null or @Nganh_Parent is null or Nganh_Parent like @Nganh_Parent)
				and (@byNg is null or Nganh_Parent is null or Nganh_Parent in (select * from f_split(@byNg)))
	order by	ml.KyHieu

	if (@id_PhongBan = '02' or @id_PhongBan = '11') 
		begin		
			create table #TempValue
			(
				Id_MucLuc uniqueidentifier, TonKho_DV decimal(18,0), HuyDong_DV decimal(18,0), TuChi_DV decimal(18,0), MuaHang_DV decimal(18,0), PhanCap_DV decimal(18,0),
				TongCong_DV decimal(18,0),HuyDong_Bql decimal(18,0), TuChi_Bql decimal(18,0), MuaHang_Bql decimal(18,0), PhanCap_Bql decimal(18,0), TongCong_Bql decimal(18,0)
			)
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
	
					
	select	Id, Id_Parent, IsParent, KyHieu, Nganh_Parent, Nganh, MoTa,
			Id_ChungTuChiTiet, TuChi_DTT_2, MuaHang_DTT_2, TuChi_DTT_1, MuaHang_DTT_1, 
			TonKho_DV, HuyDong_DV, TuChi_DV, MuaHang_DV, PhanCap_DV, TongCong_DV,
			HuyDong_Bql, TuChi_Bql, MuaHang_Bql, PhanCap_Bql, TongCong_Bql, HuyDong, TuChi, 
			MuaHang, PhanCap, TongCong
			, Tang = CASE WHEN @loai = 1 and (TongCong - TuChi_DTT_1) > 0 THEN ISNULL(TongCong - TuChi_DTT_1,0)
						  WHEN @loai = 2 and (TongCong - MuaHang_DTT_1) > 0 THEN ISNULL(TongCong - MuaHang_DTT_1,0)
						  ELSE 0 END
			, Giam = CASE WHEN @loai = 1 and (TongCong - TuChi_DTT_1) < 0 THEN ISNULL(TuChi_DTT_1 - TongCong,0)
							  WHEN @loai = 2 and (TongCong - MuaHang_DTT_1) < 0 THEN ISNULL(MuaHang_DTT_1 - TongCong,0)
							  ELSE 0 END
			, GhiChu 
	from	#Result
END
