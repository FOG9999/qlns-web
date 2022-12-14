USE [VIETTEL_NS1]
GO
/****** Object:  UserDefinedFunction [dbo].[f_skt_report_nt_ng_skt_nc]    Script Date: 20/07/2019 5:53:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
	
	author:	hieppm
	date:	18/07/2019
	decs:	Lấy tổng chỉ tiêu ngành năm trước theo ML nhu cầu
	params:	

*/

ALTER FUNCTION [dbo].[f_skt_report_nt_ng_skt_nc]
	(		
	@nam int,
	@namNS int,
	@nganh nvarchar(500),
	@dvt int
	)

RETURNS TABLE 
AS
	 
RETURN

select		Id_MLNhuCau
			, HangNhap		=sum(HangNhap)/@dvt 
			, HangMua		=sum(HangMua)/@dvt
from
			(	
			select	* 
			from
					(
					select	XauNoiMa							
							, HangNhap	
							, HangMua	
					from	SKT_ComDatas
					where	NamNS=@namNS
							and (@nganh IS NULL OR Ng in (select * from f_split(@nganh)))
							and (HangNhap + HangMua) <> 0
							and Lns like '104%'		
					) DT

					left join 

					(
					select	iID_MaMucLucNganSach
							, sXauNoiMa
					from	NS_MucLucNganSach
					where	iTrangThai = 1
							and iNamLamViec = @namNS
							and (@nganh is null or sNG in (select * from f_split(@nganh)))
							and sNG <> '00'
					) mlns

					on DT.XauNoiMa = mlns.sXauNoiMa

					left join

					(
					select	Id_MLNhuCau, Id_MLNS
					from	SKT_NCMLNS
					where	NamLamViec = @nam					
					) ncmlns

					on mlns.iID_MaMucLucNganSach = ncmlns.Id_MLNS					
			)as DT2
where		Id_MLNhuCau is not null
group by	Id_MLNhuCau