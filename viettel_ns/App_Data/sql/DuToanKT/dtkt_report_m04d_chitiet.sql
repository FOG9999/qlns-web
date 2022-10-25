
declare @dvt int									set @dvt = 1
declare @nam int									set @nam = 2018 
declare @nganh	nvarchar(2000)						set @nganh='20,21,22,23,24,25,26,27,28,29,41,42,44,67,69,70,71,72,73,74,75,76,77,78,81,82'
declare @username nvarchar(2000)					set @username='baolq'
declare @id_phongban nvarchar(2)					set @id_phongban='10'
declare @id_donvi nvarchar(2000)					set @id_donvi=null
declare @loai int									set @loai=1


--###--

select * from
(

--du toan kiem tra
select	M,Tm,TTm,Ng,MoTa,
		TuChi		=sum(TuChi)/@dvt
		--HangNhap	=sum(HangNhap)/@dvt,
		--HangMua		=sum(HangMua)/@dvt,
from	DTKT_DacThuChiTiet
where	iTrangThai=1
		and NamLamViec=@nam
		--and (@id_phongban is null or Id_PhongBanDich=@id_phongban)
		and (@nganh is null or Ng in (select * from f_split(@nganh)))
group by M,Tm,TTm,Ng,MoTa


) as t1

-- lay ten nganh
inner join 
(select sNG,sNG + ' - ' + sMoTa as TenNganh from NS_MucLucNganSach where iNamLamViec=@nam and iTrangThai=1 and sLNS='') as nganh
on t1.Ng=nganh.sNG

-- lay ten don vi
--inner join 
--(select iID_MaDonVi as dv_id, TenDonVi = (iID_MaDonVi +' - ' + sTen) from NS_DonVi where iNamLamViec_DonVi=@nam and iTrangThai=1) as dv
--on dv.dv_id=t1.Id_DonVi

where TuChi<>0
