USE [VIETTEL_NS1]
GO
/****** Object:  StoredProcedure [dbo].[skt_mappingmlns_chitietsheet]    Script Date: 08/10/2019 9:28:27 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[skt_mappingmlns_chitietsheet]
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
	values (@nam,@nam,@nam)	

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
				, mLNS as LNS
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
						and (@NG is null or mNG like @NG)
						and id <> '0A93EECD-2356-4AEA-BC35-3A7B3975DBC4') as ml
				ON map.Xau = ml.mXau
	where		(@loai = 1 and Id is null) or (@loai = 2 and Id is not null and Id_MLNhuCau = @Id_MLNhuCau)			
	order by	mXau
END
