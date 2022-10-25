declare @dvt int									set @dvt = 1
declare @nam int									set @nam = 2019
--declare @nganh	nvarchar(2000)						set @nganh='20,21,22,23,24,25,26,27,28,29,41,42,44,67,69,70,71,72,73,74,75,76,77,78,81,82'

--declare @nganh	nvarchar(2000)						set @nganh='60,61,62,64,65,66'
declare @nganh	nvarchar(2000)						set @nganh='01,02,03,04,05,07,07,08,09,10,11,12,26,27,28,55,57,60,61,62,64,65,66,67,69'

--declare @nganh	nvarchar(2000)						set @nganh='01'
declare @username nvarchar(2000)					set @username='baolq'
declare @id_phongban nvarchar(2)					set @id_phongban=null
declare @id_donvi nvarchar(2000)					set @id_donvi=null
declare @loai int									set @loai=null					--1: TuChi	2: HienVat
declare @iPhanCap int								set @iPhanCap=0					--1: TuChi	2: HienVat


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
		TuChi		=sum(rTuChi+rHangNhap)/@dvt,
		Loai=1,
		iPhanCap=	case	when sLNS=1040100 then 0
							when sLNS=1020100 then 1
							end
from	DT_ChungTuChiTiet
where	iTrangThai=1
		and iNamLamViec=@nam
		and (@id_phongban is null or iID_MaPhongBanDich=@id_phongban)
		and (@nganh is null or sNG in (select * from f_split(@nganh)))
		--and left(sLNS,1) in (1,2)
		and (@loai is null or @loai=1)
group by iID_MaDonVi,sLNS,sL,sK, sM,sTM, sTTM,sNG


union all
-- tu chi
select	Id_DonVi	=iID_MaDonVi, 
		sLNS,sL,sK, sM,sTM, sTTM,sNG,
		TuChi		=sum(rHienVat)/@dvt,
		Loai=2
		,iPhanCap=	case	when sLNS=1040100 then 0
							when sLNS=1020100 then 1
							end
from	DT_ChungTuChiTiet
where	iTrangThai=1
		and iNamLamViec=@nam
		and (@id_phongban is null or iID_MaPhongBanDich=@id_phongban)
		and (@nganh is null or sNG in (select * from f_split(@nganh)))
		and (@loai is null or @loai=2)

group by iID_MaDonVi,sLNS,sL,sK, sM,sTM, sTTM,sNG

union all
-- phan cap
-- tu chi
select	Id_DonVi	=iID_MaDonVi,
		sLNS,sL,sK, sM,sTM, sTTM,sNG,
		TuChi		=sum(rTuChi+rHangNhap)/@dvt,
		Loai=1
		,iPhanCap=	case	when sLNS=1040100 then 0
							when sLNS=1020100 then 1
							end
from	DT_ChungTuChiTiet_PhanCap
where	iTrangThai=1
		and iNamLamViec=@nam
		and (@id_phongban is null or iID_MaPhongBanDich=@id_phongban)
		and (@nganh is null or sNG in (select * from f_split(@nganh)))
		--and left(sLNS,1) in (1,2)
		and (@loai is null or @loai=1)
group by iID_MaDonVi,sLNS,sL,sK, sM,sTM, sTTM,sNG



union all
-- hienvat
select	Id_DonVi	=iID_MaDonVi,
		sLNS,sL,sK, sM,sTM, sTTM,sNG,
		TuChi		=sum(rHienVat)/@dvt,
		Loai=2
		,iPhanCap=	case	when sLNS=1040100 then 0
							when sLNS=1020100 then 1
							end
from	DT_ChungTuChiTiet_PhanCap
where	iTrangThai=1
		and iNamLamViec=@nam
		and (@id_phongban is null or iID_MaPhongBanDich=@id_phongban)
		and (@nganh is null or sNG in (select * from f_split(@nganh)))
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
		-- nguyen phan cap
		--and sLNS in (1020100)
		--and  sM in (6900,6950,7000,7750)
		--and  sTM not in (7012)
		--and sNG='04'

		-- 1. phan cap don vi
		and (
				(sLNS in (1020100)
				and sM in (6900,6950,7000,7750)
				and	sTM not in (7012)
				--and (sNG in (f_split(@nganh)
				--	--or (sNG=11 and )
				--))
				--and sNG='xxx'
				and (@iPhanCap is null or @iPhanCap=1)
				)

			or
				(

					sLNS in (1040100)
					and (@iPhanCap is null or @iPhanCap=0)
					-- OK DA CHECK
					and sM in (6900,6950,7000,7050,7750)
					--and (sNG in (04,05,07,07,08,09,10,57,61,62))
					--and (sNG not in (04,05,07,07,08,09,10) or sTM not in (7012))

					and (
							(sNG in (01,02,04,05,07,07,08,09,10,12) and sTM not in (7012))
						or	(sNG in (57,61,62,64,65,66))
						or	(sNG in (60))
						or	(sNG in (67))
						or	(sNG in (69) and sTM not in (7012))
						or	(sNG in (11) 
								and (
										(sM=6900 and sTM in (6905))
									or	(sM=7000 and sTM in (7012) and sTTM in (10))
									or	(sM=7750 and sTM in (7799) and sTTM in (40))

									)
							)
						)
				)
			)


		---- gom ca bao dam
		--and sLNS in (1040100)
		---- OK DA CHECK
		--and sM in (6900,6950,7000,7050,7750)
		----and (sNG in (04,05,07,07,08,09,10,57,61,62))
		----and (sNG not in (04,05,07,07,08,09,10) or sTM not in (7012))

		--and (
		--		(sNG in (04,05,07,07,08,09,10) and sTM not in (7012))
		--	or	(sNG in (57,61,62,64,65,66))
		--	or	(sNG in (60))
		--	or	(sNG in (67))
		--	--or	(sNG in (69) and sTM not in (7012))
		--	or	(sNG in (11) 
		--			and (
		--					(sM=6900 and sTM in (6905))
		--				or	(sM=7000 and sTM in (7012) and sTTM in (10))
		--				or	(sM=7750 and sTM in (7799) and sTTM in (40))

		--				)
		--		)
		--	)

		-- trinhsat
		--and sNG='11'
		--and  sM in (6900,6950,7000,7750) 
		--and  (sM <> 6900 or (sTM in (6905)))
		--and  (sM <> 7000 or (sTM in (7012) and sTTM in (10)))
		--and  (sM <> 7750 or (sTM in (7799) and sTTM in (40)))

		--and sNG='57' and  sM in (6900,6950,7000,7750)
		--and sNG='61' and  sM in (6900,6950,7000,7750)
		--and sNG='62' and  sM in (6900,6950,7000,7050,7750)

		----------------------------------
		-- DANG TEST
		-- canh sat bien
		--and sNG='12' 
		--and  sM in (6900,6950,7000,7750)
		--and  sTM not in (7012)

		--and sNG='69' and  sM in (6900,6950,7000,7050,7750)
		--and sTM not in (7012)
		
order by Nganh, Id_DonVi, sLNS,sL,sK, sM,sTM, sTTM,sNG, Loai
