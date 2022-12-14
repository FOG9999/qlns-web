USE [VIETTEL_NS1]
GO
/****** Object:  UserDefinedFunction [dbo].[f_skt_mlns]    Script Date: 09/07/2019 1:47:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
	
	author:	hiep
	date:	02/07/2019
	decs:	Lấy mục lục ngân sách theo số liệu quyết toán
	params:		

*/

ALTER FUNCTION [dbo].[f_skt_mlns]
	(		
	@nam nvarchar(9)
	)

RETURNS @NS_NT	TABLE 
	(	
		namNS int
		, id uniqueidentifier
		, LNS nvarchar(7)
		, L nvarchar(3)
		, K nvarchar(3)
		, M nvarchar(4)
		, TM nvarchar(4)
		, TTM nvarchar(2)
		, NG nvarchar(2)
		, MoTa nvarchar(Max)
	) 
AS
	BEGIN

	insert into @NS_NT
	select distinct NamNS,Id,LNS,L,K,M,TM,TTM,NG,sMoTa
	from
				(				
				select	LNS,L,K,M,TM,TTM,NG,XauNoiMa
						,NamNS			
				from	SKT_ComDatas
				where	Id_PhongBan <> '05'
						and NamNS in (select * from f_split(@nam))
						and LNS NOT LIKE '8%'
				) as t1 

				left join 

				(select	iID_MaMucLucNganSach as Id
						, sXauNoiMa, sMoTa, Nam = iNamLamViec
				 from	NS_MucLucNganSach 
				 where	iNamLamViec in (select * from f_split(@nam))
						and iTrangThai = 1) as ml
				ON t1.XauNoiMa = ml.sXauNoiMa and t1.NamNS = ml.Nam
RETURN
END