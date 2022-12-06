USE [VIETTEL_NS1]
GO
/****** Object:  StoredProcedure [dbo].[skt_checkmapmlns]    Script Date: 10/07/2019 12:43:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[skt_checkmapmlns]
	@nam int,
	@donvi nvarchar(10),
	@phongban nvarchar(3)
AS
BEGIN 

	create table #Temp
	(
		 nam int, NamNS_1 int, bangNS_1 nvarchar(2), NamNS_2 int, bangNS_2 nvarchar(2)  		 
	) 

	insert into #Temp(nam,NamNS_1,NamNS_2)
	values (@nam,@nam-1,@nam-2)

	update #Temp
	set bangNS_1 = (select top(1) NamNS_1 from SKT_MapDataNS where nam = NamLamViec),
	bangNS_2 = (select top(1) NamNS_2 from SKT_MapDataNS where nam = NamLamViec)

	declare @NamLamViec int								set @NamLamViec = (select top(1) nam from #Temp)
	declare @NamNS_1 int								set @NamNS_1 = (select top(1) NamNS_1 from #Temp)
	declare @bangNS_1 nvarchar(2)						set @bangNS_1 = (select top(1) bangNS_1 from #Temp)
	declare @NamNS_2 int								set @NamNS_2 = (select top(1) NamNS_2 from #Temp)
	declare @bangNS_2 nvarchar(2)						set @bangNS_2 = (select top(1) bangNS_2 from #Temp)

	
	create table #TempNamT
	(
		namNS int, LNS nvarchar(7), L nvarchar(3), K nvarchar(3), M nvarchar(4), TM nvarchar(4), TTM nvarchar(2), NG nvarchar(2), MoTa nvarchar(MAX)
	)
		
	insert into #TempNamT(namNS,LNS,L,K,M,TM,TTM,NG,MoTa)
	select namNS,LNS,L,K,M,TM,TTM,NG,MoTa from f_skt_checkmap(@NamLamViec,CONVERT(nvarchar,@NamNS_1)+','+CONVERT(nvarchar,@NamNS_2),@donvi,@phongban)	 
	
	create table #Result
	(
		LNS nvarchar(7), L nvarchar(3), K nvarchar(3), M nvarchar(4), TM nvarchar(4), TTM nvarchar(2), NG nvarchar(2), MoTa nvarchar(MAX), N_2 nvarchar(1), N_1 nvarchar(1)
	)	

	insert into #Result
	select Distinct	LNS,L,K,M,TM,TTM,NG,MoTa,'',''					
	from			#TempNamT		

	update #Result
	set N_2 = 'K' 
	where (LNS + '-' + L + '-' + K + '-' + M + '-' + TM + '-' + TTM + '-' + NG) in (select (LNS + '-' + L + '-' + K + '-' + M + '-' + TM + '-' + TTM + '-' + NG) from #TempNamT where namNS = @NamNS_2) 

	update #Result
	set N_1 = 'K' 
	where (LNS + '-' + L + '-' + K + '-' + M + '-' + TM + '-' + TTM + '-' + NG) in (select (LNS + '-' + L + '-' + K + '-' + M + '-' + TM + '-' + TTM + '-' + NG) from #TempNamT where namNS = @NamNS_1) 

	select Top(1)	nam
					, N_2 = CASE bangNS_2 WHEN 'QT' THEN N'QT năm ' + CONVERT(nvarchar(4),NamNS_2) 
										  WHEN 'DT' THEN N'DT đầu năm ' + CONVERT(nvarchar(4),NamNS_2)
										  ELSE '' END
					, N_1 = CASE bangNS_1 WHEN 'QT' THEN N'QT năm ' + CONVERT(nvarchar(4),NamNS_1) 
										  WHEN 'DT' THEN N'DT năm ' + CONVERT(nvarchar(4),NamNS_1)
										  ELSE '' END					 
	from			#Temp

	select	*
	from	#Result
	order by LNS, L, K, M, TM, TTM, NG, MoTa
END
