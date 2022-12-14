USE [VIETTEL_NS1]
GO
/****** Object:  UserDefinedFunction [dbo].[f_skt_report_nt_ng_skt]    Script Date: 20/07/2019 5:53:27 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
	
	author:	hieppm
	date:	18/07/2019
	decs:	Lấy tổng chỉ tiêu ngành năm trước theo ML số kiểm tra
	params:		

*/

ALTER FUNCTION [dbo].[f_skt_report_nt_ng_skt]
	(		
	@nam int,
	@namNS int,
	@nganh nvarchar(500),
	@dvt int
	)

RETURNS TABLE 
AS
	 
RETURN

select		Id_MLSKT
			, HangNhap		=sum(HangNhap)/@dvt 
			, HangMua		=sum(HangMua)/@dvt
from
			(	
			select	* 
			from
					(
					select	*
					from	f_skt_report_nt_ng_skt_nc(@nam,@namNS,@nganh,@dvt)							
					) DT										

					left join

					(
					select	Id = Id_MLNhuCau, Id_MLSKT
					from	SKT_NCSKT
					where	NamLamViec = @nam					
					) ncskt

					on DT.Id_MLNhuCau = ncskt.Id

					left join

					(
					select	Id as Id_SKT
							, KyHieu 
					from	SKT_MLSKT 
					where	NamLamViec=@nam
					) ml
					on ncskt.Id=ml.Id_SKT
			)as DT2
where		Id_MLSKT is not null
group by	Id_MLSKT