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

-- du toan
-- tu chi
select	Id_DonVi	=iID_MaDonVi, 
		sLNS,sL,sK, sM,sTM, sTTM,sNG,
		TuChi		=sum(rTuChi)/@dvt,
		Loai=1
		,iPhanCap=0
from	DT_ChungTuChiTiet
where	iTrangThai=1
		and iNamLamViec=@nam
		and (@id_phongban is null or iID_MaPhongBanDich=@id_phongban)
		--and (@id_donvi is null or Id_DonVi in (select * from f_split(@id_donvi)))
		and (@nganh is null or sNG in (select * from f_split(@nganh)))
		and sLNS in (1020100)
		and left(sLNS,1) in (1,2)
		and (@loai is null or @loai=1)
group by iID_MaDonVi,sLNS,sL,sK, sM,sTM, sTTM,sNG


union all
-- tu chi
select	Id_DonVi	=iID_MaDonVi, 
		sLNS,sL,sK, sM,sTM, sTTM,sNG,
		TuChi		=sum(rHienVat)/@dvt,
		Loai=2
		,iPhanCap=0
from	DT_ChungTuChiTiet
where	iTrangThai=1
		and iNamLamViec=@nam
		and (@id_phongban is null or iID_MaPhongBanDich=@id_phongban)
		--and (@id_donvi is null or Id_DonVi in (select * from f_split(@id_donvi)))
		and (@nganh is null or sNG in (select * from f_split(@nganh)))
		and sLNS in (1020100)
		and left(sLNS,1) in (1,2)
		and (@loai is null or @loai=1)

group by iID_MaDonVi,sLNS,sL,sK, sM,sTM, sTTM,sNG


union all
-- phan cap
-- tu chi
select	Id_DonVi	=iID_MaDonVi,
		sLNS,sL,sK, sM,sTM, sTTM,sNG,
		TuChi		=sum(rTuChi)/@dvt,
		Loai=1
		,iPhanCap=1
from	DT_ChungTuChiTiet_PhanCap
where	iTrangThai=1
		and iNamLamViec=@nam
		and (@id_phongban is null or iID_MaPhongBanDich=@id_phongban)
		--and (@id_donvi is null or Id_DonVi in (select * from f_split(@id_donvi)))
		and (@nganh is null or sNG in (select * from f_split(@nganh)))
		and sLNS in (1020100)
		and left(sLNS,1) in (1,2)
		and (@loai is null or @loai=2)
group by iID_MaDonVi,sLNS,sL,sK, sM,sTM, sTTM,sNG



union all
-- hienvat
select	Id_DonVi	=iID_MaDonVi,
		sLNS,sL,sK, sM,sTM, sTTM,sNG,
		TuChi		=sum(rHienVat)/@dvt,
		Loai=2
		,iPhanCap=1
from	DT_ChungTuChiTiet_PhanCap
where	iTrangThai=1
		and iNamLamViec=@nam
		and (@id_phongban is null or iID_MaPhongBanDich=@id_phongban)
		--and (@id_donvi is null or Id_DonVi in (select * from f_split(@id_donvi)))
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
		and  sM in (6900,6950,7000,7050,7750)
		and  sTM not in (7012)

		--and
		--	(sLNS in (1020100)
		--	and sM in (6900,6950,7000,7050,7750)
		--	--and (sNG in (04,05,07,07,08,09,10,57,61,62))
		--	--and (sNG not in (04,05,07,07,08,09,10) or sTM not in (7012))

		--	and (
		--		-- oki
		--			(sNG in (04,05,07,08,09,10) and sTM not in (7012))
		--		or	(sNG in (57,61,62,64,66))
		--		or	(sNG in (60))
		--		or	(sNG in (65) and (sTM<>7001 or sTTM in (40)))
		--		or	(sNG in (67))
		--		or	(sNG in (11) 
		--				and iPhanCap=1
		--				and sM=6900 and sTM=6905 and sTTM=10 
		--			)

		--		-- dang test
		--		or	(sNG in (07))
		--		or	(sNG in (69))


		--		)
		--)
order by Nganh, Id_DonVi, sLNS,sL,sK, sM,sTM, sTTM,sNG, Loai
