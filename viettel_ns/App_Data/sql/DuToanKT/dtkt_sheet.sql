
declare @dvt int									set @dvt = 1000
declare @NamLamViec int								set @NamLamViec = 2018
declare @Id_ChungTu uniqueidentifier				set @Id_ChungTu='90518505-2f59-4e8a-8b66-661c9be3c770'
declare @Id_DonVi nvarchar(20)						set @Id_DonVi='10'
declare @nganh nvarchar(1000)						set @nganh=null
declare @ng nvarchar(1000)							set @ng=null
declare @code nvarchar(1000)						set @code=null
declare @byNg nvarchar(1000)						set @byNg='29'
declare @loai nvarchar(2)							set @loai='2'

--#DECLARE#--

 
select	* 
from	(
		select	Id
				, Id_Parent
				, IsParent
				, Code
				, Ng
				, Nganh
				, sMoTa 
		from	DTKT_MucLuc 
		where	iTrangThai = 1
				and NamLamViec=@NamLamViec
				--and @loai in (select * from f_split(Loai))
				) as ml

		left join 

		(	
		select	Id as Id_ChungTuChiTiet
				, Id_Mucluc
				, Id_DonVi
				, TuChi = TuChi/@dvt
				, DacThu = DacThu/@dvt
				, TangNV = TangNV/@dvt
				, GiamNV = GiamNV/@dvt 
				, HangNhap = HangNhap/@dvt
				, DacThu_HN = DacThu_HN/@dvt
				, TangNV_HN = TangNV_HN/@dvt
				, GiamNV_HN = GiamNV_HN/@dvt
				, HangMua = HangMua/@dvt
				, DacThu_HM = DacThu_HM/@dvt
				, TangNV_HM = TangNV_HM/@dvt
				, GiamNV_HM = GiamNV_HM/@dvt
				
				, GhiChu 
		from	DTKT_ChungTuChiTiet 
		where	iTrangThai=1
				and (Id_ChungTu is null or Id_ChungTu=@Id_ChungTu)
		) as ct

		on ct.Id_Mucluc=ml.Id 

where	(Nganh='' or @nganh is null or Nganh like @nganh)
		and (Ng is null or @ng is null or Ng like @ng)
		and (@code is null or Code like @code)
		--theo nganh
		and (@byNg is null or Ng is null or Ng in (select * from f_split(@byNg)))
order by ml.Code
