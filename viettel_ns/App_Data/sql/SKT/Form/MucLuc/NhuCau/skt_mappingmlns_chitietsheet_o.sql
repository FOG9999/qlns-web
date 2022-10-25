USE [VIETTEL_NS1]
GO
/****** Object:  StoredProcedure [dbo].[skt_mappingmlns_chitietsheet]    Script Date: 09/07/2019 1:47:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[skt_mappingmlns_chitietsheet]
	@nam int,
	@loai int,
	@Id_MLNhuCau uniqueidentifier,
	@NamNS nvarchar(9),
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
	values (@nam,@nam-1,@nam-2)	

	declare @NamNS_1 int								set @NamNS_1 = (select top(1) NamNS_1 from #Temp)
	declare @NamNS_2 int								set @NamNS_2 = (select top(1) NamNS_2 from #Temp)
		
	create table #TempNamT
	(
		namNS int, id uniqueidentifier, LNS nvarchar(7), L nvarchar(3), K nvarchar(3), M nvarchar(4), TM nvarchar(4), TTM nvarchar(2), NG nvarchar(2), MoTa nvarchar(Max)
	)
		
	insert into #TempNamT
	select * from f_skt_mlns(CONVERT(nvarchar,@NamNS_1)+','+CONVERT(nvarchar,@NamNS_2))

	select		Id,Id_MLNhuCau,Id_MaMLNS,LNS,L,K,M,TM,TTM,NG,NamNS,MoTa,Map = CASE WHEN Id Is not null then N'Chọn' else N'Không chọn' end
	from	
				(select	Id,Id_MLNhuCau,Id_MLNS
				from	SKT_NCMLNS
				where	NamLamViec = @nam) map

				FULL JOIN

				(select	id as Id_MaMLNS
						, LNS
						, L
						, K
						, M
						, TM
						, TTM
						, NG
						, NamNS
						, MoTa
				from	#TempNamT 
				where	(@NamNS is null or namNS like @NamNS)
						and (@LNS is null or LNS like @LNS)
						and (@L is null or L like @L)
						and (@K is null or K like @K)
						and (@M is null or M like @M)
						and (@TM is null or TM like @TM)
						and (@TTM is null or TTM like @TTM)
						and (@NG is null or NG like @NG)
						and id <> '0A93EECD-2356-4AEA-BC35-3A7B3975DBC4') as ml
				ON map.Id_MLNS = ml.Id_MaMLNS	
	where		(@loai = 1 and Id is null) or (@loai = 2 and Id is not null and Id_MLNhuCau = @Id_MLNhuCau	)			
	order by	NamNS,LNS,L,K,M,TM,TTM,NG
END
