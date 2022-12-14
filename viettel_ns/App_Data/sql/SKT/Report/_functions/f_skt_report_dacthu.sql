USE [VIETTEL_NS1]
GO
/****** Object:  UserDefinedFunction [dbo].[f_skt_report_dacthu]    Script Date: 20/07/2019 5:52:27 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
	
	author:	hieppm	
	date:	17/07/2019
	decs:	Lấy tổng số điều chỉnh ngân sách dự toán đầu năm trước theo ML số kiểm tra
	params:	

*/

ALTER FUNCTION [dbo].[f_skt_report_dacthu]
	(		
	@nam int,
	@namNS int,
	@nganh nvarchar(500),
	@id_donvi nvarchar(500),
	@id_phongban nvarchar(10),
	@dvt int,
	@loai bit
	)

RETURNS @tabResult TABLE ( Id_DonVi nvarchar(10), Id_MLSKT uniqueidentifier, DacThu decimal(18,0))
AS
	 
BEGIN 
	declare @id_chungtu nvarchar(MAX)
	set	@id_chungtu = '3ac70842-e7fd-4b8c-9ba9-36b1870d4056,40fbd46c-fb94-4d65-b3a3-6ed5d83d7870,84a9279f-3fb5-4306-ae14-a78be6350762'

	INSERT INTO @tabResult
	select	Id_DonVi
			, Id_MLSKT
			, DacThu = SUM(DacThu)
	from 
			(
			select	*
			from
				(
				select	* 
				from	f_skt_report_dacthu_nc(@nam,@namNS,@nganh,@id_donvi,@id_phongban,@dvt,@loai)
				) sdt

				left join

				(
				select  Id_MLNhuCau as Id
						, Id_MLSKT
				from	SKT_NCSKT
				where	NamLamViec = @nam
				) mlskt

				on sdt.Id_MLNhuCau = mlskt.Id
				) re
	where		Id_MLSKT IS NOT NULL 
	group by	Id_DonVi, Id_MLSKT
RETURN
END