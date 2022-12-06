USE [VIETTEL_NS1]
GO
/****** Object:  UserDefinedFunction [dbo].[f_skt_checkmap]    Script Date: 10/07/2019 12:44:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
	
	author:	hiep
	date:	09/05/2018
	decs:	Lấy tổng chỉ tiêu ngân sách 
	params:	

*/

ALTER FUNCTION [dbo].[f_skt_checkmap]
	(		
	@nam int,
	@namNS nvarchar(9),
	@donvi nvarchar(10),
	@phongban nvarchar(3)
	)

RETURNS @NS_NT	TABLE 
	(	
		namNS int
		, LNS nvarchar(7)
		, L nvarchar(3)
		, K nvarchar(3)
		, M nvarchar(4)
		, TM nvarchar(4)
		, TTM nvarchar(2)
		, NG nvarchar(2)
		, MoTa nvarchar(MAX)
	) 
AS
	BEGIN

	insert into @NS_NT(namNS,LNS,L,K,M,TM,TTM,NG,MoTa)
	select	NamNS,LNS,L,K,M,TM,TTM,NG,sMoTa			
	from
			(
			select	LNS,L,K,M,TM,TTM,NG,XauNoiMa
						,NamNS			
				from	SKT_ComDatas
				where	Id_PhongBan <> '05'
						and NamLamViec = 2020
						and LNS NOT LIKE '8%'
						and LNS NOT LIKE '3%'
						and LNS NOT LIKE '4%'
						and Id_DonVi = @donvi
						and Id_Phongban = @phongban
			) as t1 

			left join 

			(select	iID_MaMucLucNganSach as Id
						, sXauNoiMa, sMoTa, Nam = iNamLamViec
				 from	NS_MucLucNganSach 
				 where	iNamLamViec in (select * from f_split(@namNS))
						and iTrangThai = 1) as ml
				ON t1.XauNoiMa = ml.sXauNoiMa and t1.NamNS = ml.Nam
		
	where	Id not in (select	Id_MLNS
					   from		SKT_NCMLNS
					   where	NamLamViec = @nam)	
	group by NamNS,LNS,L,K,M,TM,TTM,NG,sMoTa
RETURN
END