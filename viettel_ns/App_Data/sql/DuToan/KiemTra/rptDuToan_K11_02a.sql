declare @dvt int									set @dvt = 1
declare @nam int									set @nam = 2018 
declare @nganh	nvarchar(2000)						set @nganh='20,21,22,23,24,25,26,27,28,29,41,42,44,67,69,70,71,72,73,74,75,76,77,78,81,82'
declare @username nvarchar(2000)					set @username='baolq'
declare @id_phongban nvarchar(2)					set @id_phongban='10'
declare @id_donvi nvarchar(2000)					set @id_donvi=null
declare @loai int									set @loai=1


--###--

select Id_DonVi, TenDonVi, Nganh,TenNganh, TuChi from
(

--du toan kiem tra
select	Id_DonVi	=iID_MaDonVi, 
		Nganh		=sNG, 
		TuChi		=sum(rTuChi)/@dvt
from	DT_ChungTuChiTiet
where	iTrangThai=1
		and iNamLamViec=@nam
		and (@id_phongban is null or iID_MaPhongBanDich=@id_phongban)
		--and (@id_donvi is null or Id_DonVi in (select * from f_split(@id_donvi)))
		and (@nganh is null or sNG in (select * from f_split(@nganh)))
		and left(sLNS,3) not in (104)
		and left(sLNS,1) in (1,2)

group by iID_MaDonVi, sNg

--union all
----du toan kiem tra
--select	Id_DonVi	=iID_MaDonVi, 
--		Nganh		=sNG, 
--		TuChi		=sum(rTuChi)/@dvt
--from	DT_ChungTuChiTiet_PhanCap
--where	iTrangThai=1
--		and iNamLamViec=@nam
--		and (@id_phongban is null or iID_MaPhongBanDich=@id_phongban)
--		--and (@id_donvi is null or Id_DonVi in (select * from f_split(@id_donvi)))
--		and (@nganh is null or sNG in (select * from f_split(@nganh)))
--group by iID_MaDonVi, sNg

) as t1

-- lay ten nganh
inner join 
(select sNG,sNG + ' - ' + sMoTa as TenNganh from NS_MucLucNganSach where iNamLamViec=@nam and iTrangThai=1 and sLNS='') as nganh
on t1.Nganh=nganh.sNG

-- lay ten don vi
inner join 
(select iID_MaDonVi as dv_id, TenDonVi = (iID_MaDonVi +' - ' + sTen) from NS_DonVi where iNamLamViec_DonVi=@nam and iTrangThai=1) as dv
on dv.dv_id=t1.Id_DonVi

where TuChi<>0
