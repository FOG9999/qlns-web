declare @dvt int									set @dvt = 1000
declare @NamLamViec int								set @NamLamViec = 2019
declare @Id_PhongBan nvarchar(20)					set @Id_PhongBan=null
declare @Id_DonVi nvarchar(20)						set @Id_DonVi=null
declare @Id_ChungTu uniqueidentifier				set @Id_ChungTu=null
declare @Username nvarchar(20)						set @Username=null
declare @request int								set @request=1

--#DECLARE#--/


;with a as
(
select 
		Id_DonVi,
		Nganh,sMoTa,
		KiemTra =sum(KiemTra)/@dvt,
		TuChi	=sum(TuChi)/@dvt,
		SoSanh  =sum(TuChi-KiemTra)/@dvt,
		t       = case when sum(TuChi-KiemTra)>0 then '' else 'G' end
from
(
	-- sokiemtra
	SELECT		Id_DonVi,
				Nganh
				,KiemTra= (TuChi + TangNV - GiamNV)
				,TuChi	=0
	FROM		DTKT_ChungTuChiTiet
	WHERE		iTrangThai = 1
				and NamLamViec = @NamLamViec-1
				and iLoai=1
				and (@Id_DonVi is null or Id_DonVi in (select * from f_split(@Id_DonVi)))
				and (@Id_PhongBan is null or Id_PhongBanDich in (select * from f_split(@Id_PhongBan)))
				and code in (select Code from DTKT_MucLuc where iTrangThai=1 and NamLamViec=@NamLamViec-1)
				and (Nganh<>'00')

	---- so dutoan 
	union all

	select	Id_DonVi	=iID_MaDonVi, 
			Nganh		=sNG
			,KiemTra	=0
			,TuChi		=(rTuChi+rHangNhap+rHangMua)
	from	DT_ChungTuChiTiet
	where	iTrangThai=1
			and iNamLamViec=@NamLamViec
			and (@id_phongban is null or iID_MaPhongBanDich in (select * from f_split(@Id_PhongBan)))
			and (@Id_DonVi is null or iID_MaDonVi in (select * from f_split(@Id_DonVi)))
			and left(sLNS,3) in (102)
			and sLNS not in (1020200,1020800)	
			and (sNG<>'00')

) as t1

-- lay ten nganh
inner join 
(select sMoTa, sXauNoiMa as id from NS_MucLucNganSach where iNamLamViec=@NamLamViec-1 and iTrangThai=1 and sLNS='') as mlns
on t1.Nganh=mlns.id 

where	Nganh<>'00'
		and (KiemTra<>0 or TuChi<>0)
		--and Nganh='36'

GROUP BY	Id_DonVi, Nganh,sMoTa
)



--tongcong
--select	Nganh,sMoTa,
--		KiemTra =sum(KiemTra),
--		TuChi	=sum(TuChi),
--		SoSanh	=sum(SoSanh)
--from a
--group by Nganh,sMoTa
--having sum(KiemTra-TuChi)<>0
--order by Nganh


 --theo don vi
select * from a
--where Nganh='36'
