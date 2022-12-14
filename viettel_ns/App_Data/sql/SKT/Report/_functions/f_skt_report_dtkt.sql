USE [VIETTEL_NS1]
GO
/****** Object:  UserDefinedFunction [dbo].[f_skt_report_dtkt]    Script Date: 20/07/2019 5:52:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
	
	author:	hieppm
	date:	18/07/2019
	decs:	Lấy tổng số kiểm tra năm trước theo ML số kiểm tra
	params:	

*/

ALTER FUNCTION [dbo].[f_skt_report_dtkt]
	(		
	@nam int,
	@namNS int,
	@id_donvi nvarchar(500),
	@id_phongban nvarchar(10),
	@dvt int
	)

RETURNS TABLE 
AS
	 
RETURN

select		Id_MLSKT
			, Id_DonVi
			, TuChi		=sum(TuChi)/@dvt 
from
			(	
			select	* 
			from
					(
					select	Code
							, Id_DonVi
							, TuChi = TuChi + TangNV - GiamNV
					from	DTKT_ChungTuChiTiet
					where	iTrangThai=1
							and NamLamViec=@namNS
							and (@Id_PhongBan is null or Id_PhongBan=@Id_PhongBan)
							and (@Id_DonVi is null or Id_DonVi in (select * from f_split(@Id_DonVi)))
							and Code not like ('1-1-07%')
					) DT

					left join 
					(
					select	Id as Id_MLSKT
							, KyHieu 
					from	SKT_MLSKT 
					where	NamLamViec=@nam
					) ml
					on DT.Code=ml.KyHieu
			)as DT2
where		Id_MLSKT is not null
group by	Id_MLSKT, Id_DonVi

