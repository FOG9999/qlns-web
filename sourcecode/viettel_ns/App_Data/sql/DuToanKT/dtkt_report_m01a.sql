declare @dvt int									set @dvt = 1000
declare @nam int									set @nam = 2018
declare @Id_PhongBan nvarchar(20)					set @Id_PhongBan='06'
declare @Id_DonVi nvarchar(2000)					set @Id_DonVi=null

--#DECLARE#--/

select	Id_DonVi,TenDonVi,
		SUBSTRING(Code,1,1) as Code1, SUBSTRING(Code,1,3) as Code2, SUBSTRING(Code,1,6) as Code3, Code,
		STT,sMoTa,Ng,
		TuChi	=sum(TuChi)/@dvt,
		DacThu	=sum(DacThu)/@dvt,
		DacThu2	=sum(DacThu2)/@dvt
from
(
	SELECT		Id_DonVi, Code,
				TuChi	=sum(TuChi+TangNV-GiamNV),
				DacThu	=sum(DacThu),
				DacThu2	=0
	FROM		DTKT_ChungTuChiTiet
	WHERE		iTrangThai = 1
				and NamLamViec = @nam
				and iLoai=1
				and (@Id_PhongBan is null or Id_PhongBanDich = @Id_PhongBan)
				and (@Id_DonVi is null or Id_DonVi in (select * from f_split(@Id_DonVi)))
				and code in (select code from DTKT_MucLuc where iTrangThai=1 and NamLamViec=@nam)
				and ((TuChi+TangNV-GiamNV)<>0 or DacThu<>0)
				and (left(Code, 6) not in ('1-1-07') or id_phongban='02')
	GROUP BY	Id_DonVi, Code, sMoTa

	--union all

	---- lay dac thu tai nganh
	--SELECT		Id_DonVi, Code,
	--			TuChi	=0,
	--			DacThu	=0,
	--			DacThu2	=sum(DacThu)
	--FROM		DTKT_ChungTuChiTiet
	--WHERE		iTrangThai = 1
	--			and NamLamViec = @nam
	--			and iLoai=2
	--			and (@Id_PhongBan is null or Id_PhongBan = @Id_PhongBan)
	--			and (@Id_DonVi is null or Id_DonVi in (select * from f_split(@Id_DonVi)))
	--			and (TuChi<>0 or DacThu<>0)
	--			and iRequest=0
	--			and code in (select code from DTKT_MucLuc where iTrangThai=1 and NamLamViec=@nam)
	--GROUP BY	Id_DonVi, Code, sMoTa

) as a


-- lay dtkt_mucluc
inner join 
	(select Code as ml_id,
			STT,
			Loai,
			Ng,
			Nganh,
			sMoTa		=case nganh 
						 when '00' then STT + ' ' + sMoTa
						 else Nganh +' - ' + sMoTa
						 end
	 from	DTKT_MucLuc where iTrangThai=1 and NamLamViec=@nam and IsParent=0 and Loai like '1%') as ml
on	ml.ml_id=a.Code


-- lay ten don vi
inner join 
(select iID_MaDonVi as dv_id, TenDonVi = iID_MaDonVi + ' - ' + sTen from NS_DonVi where iNamLamViec_DonVi=@nam and iTrangThai=1) as dv
on dv.dv_id=a.Id_DonVi
GROUP BY	Id_DonVi,TenDonVi, Code, sMoTa,STT,Ng,Loai
