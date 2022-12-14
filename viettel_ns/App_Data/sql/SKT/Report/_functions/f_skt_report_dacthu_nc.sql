USE [VIETTEL_NS1]
GO
/****** Object:  UserDefinedFunction [dbo].[f_skt_report_dacthu_nc]    Script Date: 20/07/2019 5:52:29 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
	
	author:	hieppm
	date:	18/07/2019
	decs:	Lấy tổng số điều chỉnh ngân sách dự toán đầu năm trước theo ML nhu cầu
	params:	

*/

ALTER FUNCTION [dbo].[f_skt_report_dacthu_nc]
	(		
	@nam int,
	@namNS int,
	@nganh nvarchar(500),
	@id_donvi nvarchar(500),
	@id_phongban nvarchar(10),
	@dvt int,
	@loai bit
	)

RETURNS @tabResult TABLE ( Id_DonVi nvarchar(10), Id_MLNhuCau uniqueidentifier, DacThu decimal(18,0))
AS
	 
BEGIN 
	declare @id_chungtu nvarchar(MAX)
	set	@id_chungtu = '3ac70842-e7fd-4b8c-9ba9-36b1870d4056,40fbd46c-fb94-4d65-b3a3-6ed5d83d7870,84a9279f-3fb5-4306-ae14-a78be6350762'

	INSERT INTO @tabResult
	select		Id_DonVi
				, Id_MLNhuCau
				, DacThu = SUM(TuChi)
	from 			
				(
				select	Id_DonVi
						, Id_MLNhuCau = CASE WHEN Id_DonVi = '29' and Id = '0A93EECD-2356-4AEA-BC35-3A7B3975DBC4' THEN 'b4b66e4a-9c40-4db7-a9eb-8bd5e0e93858' 
											 WHEN Id_DonVi = '31' and Id = '0A93EECD-2356-4AEA-BC35-3A7B3975DBC4' THEN 'e2c28788-3b05-47e5-b97f-9d2b237d2406' 
											 WHEN Id_DonVi = '33' and Id = '0A93EECD-2356-4AEA-BC35-3A7B3975DBC4' THEN '3682b520-6fc6-4883-8a90-3cefef911c5f' 
											 ELSE Id_MLNhuCau end 
						, TuChi 
				from
						(			
						select	
								XauNoiMa
								, NamNS
								, Id_DonVi
								, TuChi		= sum(TuChi) /@dvt
						from	SKT_ComDatasDacThu
						where	NamLamViec = @nam
								and (@id_phongban is null or Id_PhongBan = @id_phongban)
								and (@id_donvi is null or Id_DonVi in (select * from f_split(@id_donvi)))
								and (@nganh is null or Ng in (select * from f_split(@nganh)))
								and XauNoiMa in (
												select	XauNoiMa
												from	SKT_MLDacThu 
												where	NamLamViec = @nam 
														and NamNS = @namNS
														and DacThu = @loai)

						group by NamNS,Id_DonVi,XauNoiMa

						union all

						select  XauNoiMa = sXauNoiMa
								, NamNS = iNamLamViec
								, Id_DonVi = iID_MaDonVi
								, TuChi = sum(rTuChi) /@dvt
						from	DT_ChungTuChiTiet
						where	iID_MaChungTu in (select * from f_split(@id_chungtu))
								and (@id_donvi is null or iID_MaDonVi in (select * from f_split(@id_donvi))) 
								and (@loai = 0)
						group by iNamLamViec,iID_MaDonVi,sXauNoiMa
						) as t1 

						left join 

						(select		iID_MaMucLucNganSach as Id
									, sXauNoiMa
									, NamMl = iNamLamViec
						 from		NS_MucLucNganSach 
						 where		iNamLamViec = @namNS
									and iTrangThai = 1
						) as ml

						 ON t1.XauNoiMa = ml.sXauNoiMa
		
						left join 
		
						(select		Id_MLNhuCau, Id_MLNS
						 from		SKT_NCMLNS
						 where		NamLamViec = @nam 			 
						) as map

						 on ml.Id = map.Id_MLNS
				where	Id_DonVi IS NOT NULL) sdt				
	where		Id_MLNhuCau IS NOT NULL 
	group by	Id_DonVi, Id_MLNhuCau
RETURN
END