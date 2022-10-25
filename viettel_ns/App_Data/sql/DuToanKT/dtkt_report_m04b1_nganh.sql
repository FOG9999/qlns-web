
declare @dvt int									set @dvt = 1000
declare @nam int									set @nam = 2018
declare @iID_MaNamNganSach nvarchar(200)			set @iID_MaNamNganSach='2'
declare @iID_MaNguonNganSach nvarchar(20)			set @iID_MaNguonNganSach='1'
declare @id_phongban nvarchar(20)					set @id_phongban=null
declare @lns		nvarchar(200)					set @lns = '104,109'
declare @Nganh		nvarchar(200)					set @Nganh = null
declare @id_nganh		nvarchar(200)				set @id_nganh = null
declare @id_donvi		nvarchar(200)				set @id_donvi = null
declare @request		int							set @request = 0
declare @loai		int								set @loai = 2



--###--


select	Id_DonVi,TenDonVi,
		Nganh, TenNganh, 
		Code,
		sMoTa,
		--NganhCon = RIGHT(Code,2),
		NganhCon = case when Nganh in ('60','61','62','64','65','66') then '02' else RIGHT(Code,2) end,
		TuChi		=sum(TuChi)/@dvt 
 from
(

	--du toan kiem tra

	-- nganh bao dam
	SELECT	Nganh, 
			--NganhCon = RIGHT(Code,2),
			Id_DonVi,
			Code,
			sMoTa,
			DacThu		=sum(DacThu + DacThu_HM + DacThu_HN),
			--TuChi		=sum(tuchi),
			TuChi		= case @request when 0 then sum(TuChi) when 1 then  sum(TuChi + TangNV + TangNV_HM + TangNV_HN - GiamNV - GiamNV_HM - GiamNV_HN) end,
			HangMua		=sum(HangMua),
			HangNhap	=sum(HangNhap)
	FROM    DTKT_ChungTuChiTiet 
	WHERE   iTrangThai=1    
			AND NamLamViec=@nam
			--AND iRequest=@request
			--AND (@id_phongban is null or Id_PhongBanDich=@id_phongban)
			AND (@id_donvi is null or Id_DonVi in (select * from f_split(@id_donvi)))
			AND Nganh not in ('00')
			AND (@Nganh is null or Nganh in (select * from f_split(@Nganh)))
			--AND Nganh in ('01','02','03','04','05','06','07','08','09','10','11','12','26','27','55','57','60','67','69')
			--AND Nganh in ('01','02','03','04','05','06','07','08','09','10','11','12','26','27','28','55','57','60','61','62','64','65','66','67','69')
			AND iLoai=@loai
			--AND TuChi<>0
	group by Nganh,Code,sMoTa,Id_DonVi

) as a


left join (select sNG as id, sNG + ' - ' + sMoTa as TenNganh from NS_MucLucNganSach where iTrangThai=1 and iNamLamViec=@nam and sLNS='') as mucluc
on a.Nganh = mucluc.id

-- lay ten don vi
inner join 
(select iID_MaDonVi as dv_id, TenDonVi = (iID_MaDonVi +' - ' + sTen) from NS_DonVi where iNamLamViec_DonVi=@nam and iTrangThai=1) as dv
on dv.dv_id=a.Id_DonVi

where	TuChi<>0
group by  Nganh, TenNganh, Code, sMoTa, Id_DonVi,TenDonVi
order by NganhCon, Nganh
