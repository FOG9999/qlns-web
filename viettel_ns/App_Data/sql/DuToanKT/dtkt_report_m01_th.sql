
declare @dvt int									set @dvt = 1
declare @nam int									set @nam = 2018 
declare @nganh	nvarchar(2000)						set @nganh='00'
declare @id_phongban nvarchar(2)					set @id_phongban=null
declare @id_donvi nvarchar(2000)					set @id_donvi=null
declare @loai int									set @loai=1
declare @request int								set @request=1


--###--

select * from
(

---du toan kiem tra (dot 0)
	select	
			Nganh,
			Id_DonVi,
			--TuChi		=sum(TuChi+TangNV-GiamNV + DacThu),
			TuChi		= case @request when 0 then sum(TuChi)/@dvt when 1 then  sum(TuChi + TangNV - GiamNV)/@dvt end,
			--TuChi		=sum(TuChi),
			DacThu		=sum(DacThu)/@dvt
	from DTKT_ChungTuChiTiet
	where	iTrangThai = 1
			and NamLamViec = @nam
			and iLoai=1
			--and iRequest=0
			and (@Id_PhongBan is null or Id_PhongBanDich = @Id_PhongBan)
			and (@Id_DonVi is null or Id_DonVi in (select * from f_split(@Id_DonVi)))
			and (Nganh <> '00')
			and (left(code,6) not in ('1-1-07'))
			and code in (select Code from DTKT_MucLuc where iTrangThai=1 and NamLamViec=@nam)
			--and (TuChi<>0 or DacThu<>0)
	group by Id_DonVi, Nganh



) as a

 --lay dtkt_mucluc
inner join 
	(select  Nganh as ml_id,
			SUBSTRING(Code,1,1) as Code1, 
			SUBSTRING(Code,1,3) as Code2, 
			SUBSTRING(Code,1,6) as Code3,
			SUBSTRING(Code,1,9) as Code4,
			Code,sMoTa
	 from	DTKT_MucLuc where iTrangThai=1 and NamLamViec=@nam and IsParent=0 and Loai like '1%' and RIGHT(Code,2)='00') as ml
on	ml.ml_id=a.Nganh

-- lay ten don vi
inner join 
(select iID_MaDonVi as dv_id, TenDonVi = (iID_MaDonVi +' - ' + sTen) from NS_DonVi where iNamLamViec_DonVi=@nam and iTrangThai=1) as dv
on dv.dv_id=a.Id_DonVi

--where DacThu<> 0 or TuChi<>0
