USE [VIETTEL_NS1]
GO
/****** Object:  UserDefinedFunction [dbo].[f_skt_ctctdt]    Script Date: 27/06/2019 1:25:28 AM ******/
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

CREATE FUNCTION [dbo].[f_skt_kiemtong]
	(		
	@nam int,
	@namNS int
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
	select	@namNS
			,sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa			
	from
			(
			-- du toan
			-- tu chi
			select	
					sLNS,sL,sK,sM,sTM,sTTM,sNG,sLNS + '-' + sL + '-' + sK + '-' + sM + '-' + sTM + '-' + sTTM + '-' + sNG as XauNoiMa					
			from	DT_ChungTuChiTiet
			where	iTrangThai=1
					and iNamLamViec=@NamNS
					and iID_MaPhongBan <> '05'
					and MaLoai in ('','2')
					and sLNS NOT LIKE '8%'

			union all

			select	
					sLNS,sL,sK,sM,sTM,sTTM,sNG,sLNS + '-' + sL + '-' + sK + '-' + sM + '-' + sTM + '-' + sTTM + '-' + sNG as XauNoiMa					
			from	DT_ChungTuChiTiet_PhanCap
			where	iTrangThai=1
					and iID_MaPhongBan <> '05'
					and iNamLamViec=@NamNS
					and sLNS NOT LIKE '8%'
			group by sLNS,sL,sK,sM,sTM,sTTM,sNG
			) as t1 

			left join 

			(select		iID_MaMucLucNganSach as Id
						, sXauNoiMa, sMoTa
			 from		NS_MucLucNganSach 
			 where		iNamLamViec = @NamNS
						and iTrangThai = 1) as ml
			 on t1.XauNoiMa = ml.sXauNoiMa
		
	where	Id not in (select	Id_MLNS
					   from		SKT_NCMLNS
					   where	NamLamViec = @nam)	
	group by sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa
RETURN
END