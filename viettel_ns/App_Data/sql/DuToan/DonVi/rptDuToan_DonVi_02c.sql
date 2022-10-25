declare @dvt int									set @dvt = 1
declare @nam int									set @nam = 2019
--declare @nganh	nvarchar(2000)						set @nganh='20,21,22,23,24,25,26,27,28,29,41,42,44,67,69,70,71,72,73,74,75,76,77,78,81,82'

--declare @nganh	nvarchar(2000)						set @nganh='60,61,62,64,65,66'
declare @nganh	nvarchar(2000)						set @nganh='04'
declare @username nvarchar(2000)					set @username='baolq'
declare @id_phongban nvarchar(2)					set @id_phongban=null
declare @id_donvi nvarchar(2000)					set @id_donvi=null
declare @loai int									set @loai=null


--###--

select	Id_DonVi, TenDonVi, sLNS,sL,sK, sM,sTM, sTTM, sNG
		,sXauNoiMa = sLNS + '-' + sL + '-' + sK+ '-' +sM+ '-' +sTM+ '-' +sTTM+ '-' +sNG
		,Nganh= sM + '-' + sTM+'-' + sTTM + '-' + sNG
		,TenNganh, TuChi, Loai, iPhanCap
from
(

-- dự toán
-- tự chi
select	Id_DonVi	=iID_MaDonVi, 
		sLNS,sL,sK, sM,sTM, sTTM,sNG,
		TuChi		=sum(rTuChi)/@dvt,
		Loai=1
		,iPhanCap=0
from	DT_ChungTuChiTiet
where	iTrangThai=1
		and iNamLamViec=@nam
		and (@id_phongban is null or iID_MaPhongBanDich=@id_phongban)
		and (@nganh is null or sNG in (select * from f_split(@nganh)))
		and sLNS in (1020100)
		and left(sLNS,1) in (1,2)
		and (@loai is null or @loai=1)
group by iID_MaDonVi,sLNS,sL,sK, sM,sTM, sTTM,sNG


union all
-- hiện vật
select	Id_DonVi	=iID_MaDonVi, 
		sLNS,sL,sK, sM,sTM, sTTM,sNG,
		TuChi		=sum(rHienVat)/@dvt,
		Loai=2
		,iPhanCap=0
from	DT_ChungTuChiTiet
where	iTrangThai=1
		and iNamLamViec=@nam
		and (@id_phongban is null or iID_MaPhongBanDich=@id_phongban)
		and (@nganh is null or sNG in (select * from f_split(@nganh)))
		and sLNS in (1020100)
		and left(sLNS,1) in (1,2)
		and (@loai is null or @loai=1)

group by iID_MaDonVi,sLNS,sL,sK, sM,sTM, sTTM,sNG


union all
-- phan cap
-- tự chi
select	Id_DonVi	=iID_MaDonVi,
		sLNS,sL,sK, sM,sTM, sTTM,sNG,
		TuChi		=sum(rTuChi)/@dvt,
		Loai=1
		,iPhanCap=1
from	DT_ChungTuChiTiet_PhanCap
where	iTrangThai=1
		and iNamLamViec=@nam
		and (@id_phongban is null or iID_MaPhongBanDich=@id_phongban)
		and (@nganh is null or sNG in (select * from f_split(@nganh)))
		and sLNS in (1020100)
		and left(sLNS,1) in (1,2)
		and (@loai is null or @loai=2)
group by iID_MaDonVi,sLNS,sL,sK, sM,sTM, sTTM,sNG



union all
-- hiện vật
select	Id_DonVi	=iID_MaDonVi,
		sLNS,sL,sK, sM,sTM, sTTM,sNG,
		TuChi		=sum(rHienVat)/@dvt,
		Loai=2
		,iPhanCap=1
from	DT_ChungTuChiTiet_PhanCap
where	iTrangThai=1
		and iNamLamViec=@nam
		and (@id_phongban is null or iID_MaPhongBanDich=@id_phongban)
		and (@nganh is null or sNG in (select * from f_split(@nganh)))
		and sLNS in (1020100)
		and left(sLNS,1) in (1,2)
		and (@loai is null or @loai=2)
group by iID_MaDonVi,sLNS,sL,sK, sM,sTM, sTTM,sNG

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
order by Nganh, Id_DonVi, sLNS,sL,sK, sM,sTM, sTTM,sNG, Loai
