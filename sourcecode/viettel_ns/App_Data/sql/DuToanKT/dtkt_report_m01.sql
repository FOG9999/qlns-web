declare @dvt int									set @dvt = 1000
declare @nam int									set @nam = 2018
declare @Id_PhongBan nvarchar(20)					set @Id_PhongBan='07' 
declare @Id_DonVi nvarchar(2000)					set @Id_DonVi='10,11,12,13'

--#DECLARE#--/

select * from
(
SELECT		Id_DonVi, SUBSTRING(Code,1,1) as Code1, SUBSTRING(Code,1,3) as Code2, SUBSTRING(Code,1,6) as Code3, Code,
			iRequest,
			TuChi	=sum(TuChi)/@dvt
FROM		DTKT_ChungTuChiTiet
WHERE		iTrangThai = 1
			and NamLamViec = @nam
			and iLoai=1
			and (@Id_PhongBan is null or Id_PhongBan = @Id_PhongBan)
			and Id_DonVi in (select * from f_split(@Id_DonVi))
			and (TuChi<>0 or DacThu<>0)
GROUP BY	Id_DonVi, Code, sMoTa, iRequest
) as t1


-- lay ten don vi
inner join 
(select iID_MaDonVi as dv_id, TenDonVi = sTen from NS_DonVi where iNamLamViec_DonVi=@nam and iTrangThai=1) as dv
on dv.dv_id=t1.Id_DonVi
