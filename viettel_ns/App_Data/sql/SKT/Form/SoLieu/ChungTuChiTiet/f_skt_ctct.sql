USE [VIETTEL_NS1]
GO
/****** Object:  UserDefinedFunction [dbo].[f_skt_ctct]    Script Date: 09/07/2019 3:28:02 AM ******/
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

ALTER FUNCTION [dbo].[f_skt_ctct]
	(		
	@nam int,
	@namNS nvarchar(9),
	@id_donvi nvarchar(10),
	@id_phongban nvarchar(10),
	@dvt int
	)

RETURNS @NS_NT	TABLE 
	(	
		namNS int
		, Id_MLNhuCau uniqueidentifier
		, TuChi float
		, MuaHang float
	) 
AS
	BEGIN

	insert into @NS_NT(namNS,Id_MLNhuCau,TuChi,MuaHang)
	select	NamNS 
			, Id_MLNhuCau
			, TuChi = SUM(TuChi)	
			, MuaHang = SUM(HangNhap + HangMua)	
	from
			(			
			select	
					XauNoiMa
					, NamNS
					, TuChi		= sum(TuChi) /@dvt
					, HangNhap	= sum(HangNhap) /@dvt											
					, HangMua	= sum(HangMua) /@dvt
			from	SKT_ComDatas
			where	NamLamViec = @nam
					and NamNS in (select * from f_split(@namNS))
					and (@id_phongban is null or Id_PhongBan = @id_phongban)
					and (@id_donvi is null or Id_DonVi = @id_donvi)	
			group by NamNS,XauNoiMa,Lns,L,K,M,Tm,Ttm,Ng
			) as t1 

			left join 

			(select		iID_MaMucLucNganSach as Id
						, sXauNoiMa
						, NamMl = iNamLamViec
			 from		NS_MucLucNganSach 
			 where		iNamLamViec in (select * from f_split(@namNS))
						and iTrangThai = 1) as ml
			 ON t1.XauNoiMa = ml.sXauNoiMa and t1.NamNS=ml.NamMl
		
			left join 
		
			(select		Id_MLNhuCau, Id_MLNS
			 from		SKT_NCMLNS
			 where		NamLamViec = @nam 
			 
			 union all
			 
			 select		Id_MLNhuCau = CASE @id_donvi WHEN '29' THEN 'b4b66e4a-9c40-4db7-a9eb-8bd5e0e93858' 
													 WHEN '31' THEN 'e2c28788-3b05-47e5-b97f-9d2b237d2406' 
													 WHEN '33' THEN '3682b520-6fc6-4883-8a90-3cefef911c5f' 
													 ELSE NULL end 
						, Id_MLNS = case when @id_donvi in ('29','31','33') then '0A93EECD-2356-4AEA-BC35-3A7B3975DBC4' else NULL end
						
						 
						) as map

			 on ml.Id = map.Id_MLNS
	group by NamNS,Id_MLNhuCau
RETURN
END