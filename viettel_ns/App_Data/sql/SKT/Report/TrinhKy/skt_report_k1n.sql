USE [VIETTEL_NS1]
GO
/****** Object:  StoredProcedure [dbo].[skt_report_k1n]    Script Date: 20/07/2019 5:51:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[skt_report_k1n]
	@nam	int,
	@dvt	int,
	@nganh nvarchar(500)
AS
BEGIN 
		
	create table #Temp
	(
		Id_MLSKT uniqueidentifier, HangNhap decimal(18,0), HangMua decimal(18,0),DacThu decimal(18,0)
	)

	Insert into #Temp
	select		Id_MLSKT
				, HangNhap = SUM(HangNhap)
				, HangMua = SUM(HangMua)
				, DacThu = SUM(DacThu)
	from		(
				select	Id_MLSKT
						, HangNhap
						, HangMua
						, DacThu = 0 
				from	f_skt_report_nt_ng_skt(@nam,@nam-1,@nganh,@dvt)
				
				union all
				
				select 	Id_MLSKT
						, HangNhap = 0
						, HangMua = 0
						, DacThu 
				from	f_skt_report_dacthu(@nam,@nam-1,@nganh,null,null,@dvt,1)		
				)re	
	group by	Id_MLSKT
	
	select		KyHieu	
				, Nganh_Parent
				, TenNganhQL = RTRIM(LTRIM(TenNganhQL))
				, Nganh
				, TenNganh = RTRIM(LTRIM(TenNganh))
				, TenMuc = RTRIM(LTRIM(MoTa))
				, HangNhap = SUM(HangNhap)
				, HangMua = SUM(HangMua)
				, DacThu = sum(DacThu)
	from	
				(
				select		*
				from		#Temp
				where		HangNhap + HangMua + DacThu <> 0			
				) ct

				left join 

				(
				select	Id
						, KyHieu
						, Nganh
						, Nganh_Parent
						, MoTa
				from	SKT_MLSKT
				where	NamLamViec = @nam
				) ml 
			
				on ct.Id_MLSKT = ml.Id
			
				left join

				(
				select	sNG, sMoTa as TenNganh
				from	NS_MucLucNganSach
				where	iNamLamViec = @nam - 1	and sLNS = ''
				) nganh

				on ml.Nganh = nganh.sNG

				left join 

				(
				select	iID_MaNganh as nganh_id
						, TenNganhQL = 'Ngành ' + sTenNganh  
				from	NS_MucLucNganSach_Nganh 
				where	iNamLamViec = @nam 
						and iTrangThai = 1
				) dv
				on dv.nganh_id= ml.Nganh_Parent	
	group by	Nganh_Parent, TenNganhQL, Nganh, TenNganh, KyHieu, MoTa
END
