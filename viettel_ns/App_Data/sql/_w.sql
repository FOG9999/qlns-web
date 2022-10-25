

select * from DTKT_MucLuc
order by code


select * from NS_MucLucNganSach_Nganh


select * from DTKT_ChungTuChiTiet
where	iTrangThai=1 and NamLamViec=2018
		and iLoai=2
order by Code
		


select distinct  Ng, Nganh,Right(code,2) as NganhCon, XauNoiMa = (Ng + '-' + Nganh + '-' + Right(code,2))
from DTKT_ChungTuChiTiet
where	iTrangThai=1 and NamLamViec=2018
		and iLoai=2
order by XauNoiMa



select distinct  Ng, Nganh,Right(code,2) as NganhCon, XauNoiMa = (Ng + '-' + Nganh + '-' + Right(code,2))
from DTKT_MucLuc
where	iTrangThai=1 and NamLamViec=2018
		--and Loai='2'
		and Nganh <> ''
		and LEN(Ng)=2
		and IsParent=0

order by XauNoiMa


select * from DTKT_ChungTuchitiet
where	iTrangThai=1
		and		Nganh='67'
		and Id_PhongBan='07'
		and Id_DonVi=33
		--and UserCreator='namhh'
		order by code

		

select * from DTKT_ChungTuchitiet

where	iTrangThai=1
		--and		Nganh='67'
		--and Id_PhongBan='07'
		--and Id_DonVi=44
		and iRequest=1
		--and UserCreator='namhh'
		and TuChi<>0
		order by code


--update DTKT_ChungTuChiTiet
--set TuChi=0
--where id='771EB983-EB38-46FE-8AC2-EE2EEC9A3CD1'
