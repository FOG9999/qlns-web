declare @dvt int									set @dvt = 1
declare @nam int									set @nam = 2018 
--declare @nganh	nvarchar(2000)						set @nganh='60,61,62,64,65,66'
declare @nganh	nvarchar(2000)						set @nganh=null
declare @username nvarchar(2000)					set @username=null
declare @irequest int								set @irequest = 0
declare @id_phongban_dich nvarchar(10)				set @id_phongban_dich = null


--###--

select	Code	
		, Ng
		, TenDonVi = RTRIM(LTRIM(TenDonVi))
		, Nganh
		, TenMuc = RTRIM(LTRIM(TenMuc))
		, TenNganh = RTRIM(LTRIM(TenNganh))
		, TuChi
		, HangNhap
		, HangMua
		, DacThu
		, ThongBaoPC
from
		(
		select	Code
				, Ng
				, Nganh
				, TuChi			= sum(TuChi)/@dvt	
				, HangNhap		= sum(HangNhap)/@dvt
				, HangMua		= sum(HangMua)/@dvt
				, DacThu		= sum(DacThu)/@dvt
				, ThongBaoPC	= sum(ThongBaoPC)/@dvt
		from	(
				select		
							Code
							, Ng
							, Nganh
							, TuChi		= sum(TuChi+ TangNV - GiamNV)					
							--, TuChi		= sum(TuChi)					
							, HangNhap	= sum(HangNhap)
							, HangMua	= sum(HangMua)		
							, DacThu	= sum(DacThu + DacThu_HN + DacThu_HM)
							, ThongBaoPC  = 0
				from		DTKT_ChungTuChiTiet
				where		iTrangThai = 1
							and NamLamViec = @nam
							and iLoai = 2
							--and iRequest = 0
							and (@id_phongban_dich is null or Id_PhongBanDich = @id_phongban_dich)			
							and (@nganh IS NULL OR Nganh in (select * from f_split(@nganh)))
							and (@username is null or UserCreator = @username)
							--and (TuChi <> 0 or HangNhap <> 0 or HangMua <> 0 or DacThu <> 0 or DacThu_HN <> 0 or DacThu_HM <> 0)
				group by	Code
							,Ng
							, Nganh

				union all

				select		Code
							, Ng
							, Nganh
							, TuChi		= 0
							, HangNhap	= 0
							, HangMua	= 0
							, DacThu	= sum(DacThu)
							, ThongBaoPC  = sum(TuChi+ TangNV - GiamNV)	
				from		DTKT_ChungTuChiTiet
				where		iTrangThai = 1
							and NamLamViec = @nam
							and iLoai = 1
							--and iRequest = 0
							and (@nganh IS NULL OR Nganh in (select * from f_split(@nganh)))
							--and (TuChi <> 0 or DacThu <> 0)
				group by	Code,Ng,Nganh
				) as r
		group by Code, Ng, Nganh
		) as t1

		-- lay ten nganh
		inner join 

		(
		select	MaMuc = Code, sMoTa as TenMuc 
		from	DTKT_MucLuc 
		where	NamLamViec = @nam 
				and iTrangThai = 1 
		) as mucluc

		on t1.Code=mucluc.MaMuc

		inner join

		(
		select	sNG, sMoTa as TenNganh
		from	NS_MucLucNganSach
		where	iNamLamViec = @nam	and sLNS = ''
		) as nganh

		on t1.Nganh = nganh.sNG

		-- lay ten don vi
		inner join 

		(
		select	iID_MaDonVi as dv_id
				, TenDonVi = sTen 
		from	NS_DonVi 
		where	iNamLamViec_DonVi = @nam 
				and iTrangThai = 1
		) as dv
		on dv.dv_id=t1.Ng	
ORDER BY Ng, Nganh	
