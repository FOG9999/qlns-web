
declare @dvt int									set @dvt = 1000
declare @nam int									set @nam = 2018 
declare @irequest int								set @irequest = 0
declare @id_phongban_dich nvarchar(10)				set @id_phongban_dich = null
declare @nganh	nvarchar(2000)						set @nganh=null
declare @username nvarchar(2000)					set @username='chiendv'

--###--

select	Code	
		, Ng
		, TenDonVi = RTRIM(LTRIM(TenDonVi))
		, Nganh
		, TenMuc = RTRIM(LTRIM(TenMuc))
		, TenNganh = RTRIM(LTRIM(TenNganh))
		, TangNV_TC = TangNV_TC / @dvt
		, GiamNV_TC = GiamNV_TC / @dvt
		, TangNV_HM = TangNV_HM / @dvt
		, GiamNV_HM = GiamNV_HM / @dvt
		, TangNV_HN = TangNV_HN / @dvt
		, GiamNV_HN = GiamNV_HN / @dvt
from
		(		
		select		Code
					, Ng
					, Nganh
					, TangNV_TC		= sum(TangNV)	
					, GiamNV_TC		= sum(GiamNV)				
					, TangNV_HM		= sum(TangNV_HM)
					, GiamNV_HM		= sum(GiamNV_HM)	
					, TangNV_HN		= sum(TangNV_HN)
					, GiamNV_HN		= sum(GiamNV_HN)			
		from		DTKT_ChungTuChiTiet
		where		iTrangThai = 1
					and NamLamViec = @nam
					and iLoai = 2
					and iRequest = @irequest
					and (@id_phongban_dich is null or Id_PhongBanDich = @id_phongban_dich)			
					and (@nganh IS NULL OR Nganh in (select * from f_split(@nganh)))
					and (@username is null or UserCreator = @username)
					and (TangNV <> 0 or TangNV_HN <> 0 or TangNV_HM <> 0 or GiamNV <> 0 or GiamNV_HN <> 0 or GiamNV_HM <> 0)
		group by	Code
					, Ng
					, Nganh				

			
		) as t1

		-- lay ten nganh
		inner join 

		(
		select	MaMuc = Code
				, sMoTa as TenMuc 
		from	DTKT_MucLuc 
		where	NamLamViec = @nam 
				and iTrangThai = 1 
		) as mucluc

		on t1.Code=mucluc.MaMuc

		inner join

		(
		select	sNG
				, sMoTa as TenNganh
		from	NS_MucLucNganSach
		where	iNamLamViec = @nam
				and sLNS = ''
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
