declare @dvt int									set @dvt = 1000
declare @nam int									set @nam = 2019
--declare @nganh	nvarchar(2000)						set @nganh='20,21,22,23,24,25,26,27,28,29,41,42,44,67,69,70,71,72,73,74,75,76,77,78,81,82'

declare @nganh	nvarchar(2000)						set @nganh='60,61,62,64,65,66'
declare @ngcha	nvarchar(2000)						set @ngcha='99'
declare @username nvarchar(2000)					set @username='baolq'
declare @id_phongban nvarchar(2)					set @id_phongban=null
declare @id_donvi nvarchar(2000)					set @id_donvi='29'
declare @loai int									set @loai=1


--###--

select	Id_DonVi, TenDonVi 
		,sNG
		,TenNganh, TuChi, Loai
from
(
-- dự toán

select	Id_DonVi	= iID_MaDonVi,sNG,
		TuChi		= sum(rTuChi+rHangNhap+rHangMua)/@dvt,
		Loai=1
from	DT_ChungTuChiTiet
where	iTrangThai in (1,2)
		and iNamLamViec=@nam
		and sXauNoiMa in (select Xau from NS_MucLucNganSach_BDKT)
		and MaLoai in ('','2')
		and iID_MaDonVi <> ''

group by iID_MaDonVi,sNG

union all

-- phân cấp

select	Id_DonVi	=iID_MaDonVi,sNG,
		TuChi		=sum(rTuChi)/@dvt,
		Loai=1
from	DT_ChungTuChiTiet_PhanCap
where	iTrangThai=1
		and iNamLamViec=@nam
		and sXauNoiMa in (select Xau from NS_MucLucNganSach_BDKT)
		and sLNS in (1020100)
group by iID_MaDonVi,sNG
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
--and sNG = 59	
order by Id_DonVi,sNG,Loai