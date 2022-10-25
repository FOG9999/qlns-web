
declare @dvt int									set @dvt = 1000
declare @nam int									set @nam = 2018
declare @iID_MaNamNganSach nvarchar(200)			set @iID_MaNamNganSach='2'
declare @iID_MaNguonNganSach nvarchar(20)			set @iID_MaNguonNganSach='1'
declare @id_phongban nvarchar(20)					set @id_phongban='07'
declare @lns		nvarchar(200)					set @lns = '104,109'
--declare @sNG		nvarchar(200)					set @sNG = '30,31,32,33,35,36,37,38,39,40,43,45,46,47,10'
declare @id_nganh		nvarchar(200)				set @id_nganh = null
declare @id_donvi		nvarchar(200)				set @id_donvi = null
declare @request		int							set @request = 1



--###--

select	Nganh, TenNganh,
		Id_DonVi, TenDonVi,
		TangNV		=sum(TangNV)/@dvt,
		GiamNV		=sum(GiamNV)/@dvt
 from
(
	select	Nganh, 
			Id_DonVi,
			TangNV,
			GiamNV
	from	DTKT_ChungTuChiTiet
	where	iTrangThai=1
			and (@loai is null or iLoai=@loai)
			and NamLamViec=@nam
			and (@id_phongban is null or Id_PhongBanDich=@id_phongban)
			--and iRequest=1
			--and (iLan >0 and (@request is null or iLan <= @request))
			and Code in (select code from DTKT_MucLuc where iTrangThai=1 and NamLamViec=@nam)
			and (TangNV<>0 or GiamNV<>0)
) as a

-- lay ten nganh
left join (select sNG as id, sNG + ' - ' + sMoTa as TenNganh from NS_MucLucNganSach where iTrangThai=1 and iNamLamViec=@nam and sLNS='') as mucluc
on a.Nganh = mucluc.id


-- lay ten don vi
inner join 
(select iID_MaDonVi as dv_id, TenDonVi = (iID_MaDonVi +' - ' + sTen) from NS_DonVi where iNamLamViec_DonVi=@nam and iTrangThai=1) as dv
on dv.dv_id=a.Id_DonVi

group by  Nganh,TenNganh, Id_DonVi, TenDonVi 
order by Nganh
