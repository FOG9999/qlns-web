declare @dvt int									set @dvt = 1000
declare @nam int									set @nam = 2018
declare @iID_MaNamNganSach nvarchar(200)			set @iID_MaNamNganSach='2'
declare @iID_MaNguonNganSach nvarchar(20)			set @iID_MaNguonNganSach='1'
declare @id_phongban nvarchar(20)					set @id_phongban=null
declare @lns		nvarchar(200)					set @lns = '104,109'
--declare @sNG		nvarchar(200)					set @sNG = '30,31,32,33,35,36,37,38,39,40,43,45,46,47,10'
declare @id_nganh		nvarchar(200)				set @id_nganh = null
declare @id_donvi		nvarchar(200)				set @id_donvi = null
declare @request		int							set @request = 0



--###--


select	Ng, TenNganh, 
		Code,
		sMoTa,
		NganhCon = RIGHT(Code,2),
		TuChi		=sum(TuChi)/@dvt,
		HangNhap	=sum(HangNhap)/@dvt,
		HangMua		=sum(HangMua)/@dvt,
		PhanCap		=sum(PhanCap)/@dvt,	
		DuPhong		=sum(DuPhong)/@dvt,	
		DacThu		=sum(DacThu)/@dvt,	
		DacThu1		=sum(DacThu1)/@dvt,	
		TuChi1		=sum(TuChi1)/@dvt,	
		HangMua1	=sum(HangMua1)/@dvt,	
		HangNhap1	=sum(HangNhap1)/@dvt,
		PhanCap1	=sum(PhanCap1)/@dvt
 from
(

	--du toan kiem tra

	-- nganh bao dam
	SELECT	Nganh as Ng, 
			--NganhCon = RIGHT(Code,2),
			Code,
			sMoTa,
			TuChi		=0,
			HangNhap	=0,
			HangMua		=0,
			PhanCap		=0,	
			DuPhong		=0,	
			--DacThu		=sum(DacThu),
			DacThu		=0,
			DacThu1		=sum(DacThu + DacThu_HM + DacThu_HN),
			TuChi1		= case @request when 0 then sum(TuChi) when 1 then sum(TuChi + TangNV - GiamNV) end,
			HangMua1	=sum(HangMua),
			HangNhap1	=sum(HangNhap),
			PhanCap1	=0
	FROM    DTKT_ChungTuChiTiet 
	WHERE   iTrangThai=1    
			AND NamLamViec=@nam
			--AND iRequest=@request
			AND (@id_phongban is null or Id_PhongBanDich=@id_phongban)
			AND (@id_donvi is null or Id_DonVi in (select * from f_split(@id_donvi)))
			AND Nganh not in ('00')
			AND iLoai=2
	group by Nganh,Code,sMoTa

	union all
	 
	-- ngan sach su dung -> truc tiep
	SELECT	Nganh as Ng,
			--NganhCon = RIGHT(Code,2),
			Code,
			sMoTa,
			TuChi		=0,
			HangNhap	=0,
			HangMua		=0,
			PhanCap		=0,	
			DuPhong		=0,	
			DacThu		=0,
			DacThu		=0,
			TuChi1		=0,
			HangMua1	=0,
			HangNhap1	=0,
			PhanCap1		= case @request when 0 then sum(TuChi) when 1 then sum(TuChi + TangNV - GiamNV) end
	FROM    DTKT_ChungTuChiTiet 
	WHERE   iTrangThai=1    
			AND NamLamViec=@nam
			--AND iRequest=@request
			--AND iRequest=@request
			--AND (@id_phongban is null or Id_PhongBanDich=@id_phongban)
			AND (@id_donvi is null or Id_DonVi in (select * from f_split(@id_donvi)))
			AND Nganh not in ('00')
			AND iLoai=1
	group by Nganh,Code,sMoTa

) as a


left join (select sNG as id, sNG + ' - ' + sMoTa as TenNganh from NS_MucLucNganSach where iTrangThai=1 and iNamLamViec=@nam and sLNS='') as mucluc
on a.NG = mucluc.id

--where	(DacThu<>0 or TuChi1<>0 or HangMua1<>0 or HangNhap1<>0)
		--and RIGHT(Code,2)='02'
group by  NG, TenNganh, Code, sMoTa
--order by NG
