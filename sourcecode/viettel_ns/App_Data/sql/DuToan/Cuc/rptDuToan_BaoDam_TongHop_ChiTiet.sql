declare @dvt int									set @dvt = 1
declare @nam int									set @nam = 2019
--declare @nganh	nvarchar(2000)						set @nganh='20,21,22,23,24,25,26,27,28,29,41,42,44,67,69,70,71,72,73,74,75,76,77,78,81,82'

--declare @nganh	nvarchar(2000)						set @nganh='60,61,62,64,65,66'
declare @nganh	nvarchar(2000)						set @nganh='23'
declare @username nvarchar(2000)					set @username='baolq'
declare @id_phongban nvarchar(2)					set @id_phongban=null
declare @id_donvi nvarchar(2000)					set @id_donvi='29'
declare @loai int									set @loai=null


--###--

select	Id_DonVi, TenDonVi, sNG
		,TenNganh, TuChi
from
(

-- dự toán
-- tự chi
select	Id_DonVi	=iID_MaDonVi, 
		sNG,
		TuChi		=sum(rTuChi+rHangNhap+rHangMua)/@dvt
from	DT_ChungTuChiTiet
where	iTrangThai in (1,2)
		and iNamLamViec=@nam
		and sLNS like '1%' and sLNS not like '109%' and sLNS not like '103%' and sLNS not like '105%' and sLNS not like '10202%' 
		and MaLoai in ('','2')
		and sNG <> '00'
		and (@loai is null or @loai=1)
group by iID_MaDonVi,sNG

union all
-- phan cap
-- tự chi

select * from (
select	Id_DonVi	=iID_MaDonVi,
		sNG,
		TuChi		=sum(rTuChi)/@dvt
from	DT_ChungTuChiTiet_PhanCap
where	iTrangThai in (1,2)
		and iNamLamViec=@nam
		and MaLoai in ('','2') 
		and iTrangThai = 1 
		and rTuChi <> 0 
		and (@loai is null or @loai=2)
group by iID_MaDonVi,sNG
) as t
) as t1

-- lay ten nganh
inner join 
(select sNG as nganh_id,sNG + ' - ' + sMoTa as TenNganh from NS_MucLucNganSach where iNamLamViec=@nam and iTrangThai=1 and sLNS='') as nganh
on t1.sNG=nganh.nganh_id

-- lay ten don vi
inner join 
(select iID_MaDonVi as dv_id, TenDonVi = (iID_MaDonVi +' - ' + sTen) from NS_DonVi where iNamLamViec_DonVi=@nam and iTrangThai=1) as dv
on dv.dv_id=t1.Id_DonVi

where	TuChi<>0 
and sNG in ('01','02','04','05','07','07','08','09','10','12','69')
order by Id_DonVi, sNG
