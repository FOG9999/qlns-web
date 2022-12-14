
declare @dvt int									set @dvt = 1000
declare @NamLamViec int								set @NamLamViec = 2020
--declare @Id_DonVi nvarchar(20)						set @Id_DonVi='10'
--declare @Id_DonVi nvarchar(20)						set @Id_DonVi='31'
--declare @Id_DonVi nvarchar(20)						set @Id_DonVi='29'
declare @Id_DonVi nvarchar(20)						set @Id_DonVi='29,31'
declare @Id_PhongBan nvarchar(20)					set @Id_PhongBan='07'

--#DECLARE#--

 
select 

	X1=SUBSTRING(KyHieu,1,1),
	X2=SUBSTRING(KyHieu,1,3),
	X3=SUBSTRING(KyHieu,1,6),
	X4=SUBSTRING(KyHieu,1,9),
	KyHieu,
	Nganh_Parent,
	Nganh,
	MoTa
	,Id_MucLuc
	,QT			=sum(QT)			 /@dvt
	,DT			=sum(DT)			 /@dvt
	,TonKho_DV	=sum(TonKho_DV)		 /@dvt
	,HuyDong_DV =sum(HuyDong_DV)	 /@dvt
	,TuChi_DV	=sum(TuChi_DV)		 /@dvt
	,MuaHang_DV =sum(MuaHang_DV)	 /@dvt
	,PhanCap_DV	=sum(PhanCap_DV)	 /@dvt
	,HuyDong	=sum(HuyDong)		 /@dvt
	,TuChi		=sum(TuChi)			 /@dvt
	,MuaHang	=sum(MuaHang)		 /@dvt
	,PhanCap	=sum(PhanCap)		 /@dvt

from

(

select 
	Id_MucLuc
	,Id_DonVi = (select Id_DonVi from SKT_ChungTu where Id=skt.Id_ChungTu)
	,QT		=0
	,DT		=0
	,TonKho_DV	
	,HuyDong_DV
	,TuChi_DV	
	,MuaHang_DV
	,PhanCap_DV
	,HuyDong
	,TuChi
	,MuaHang
	,PhanCap
 from SKT_ChungTuChiTiet as skt
 where	Id_ChungTu in 
		(select Id 
		 from SKT_ChungTu 
		 where	NamLamViec=@NamLamViec
				and iLoai=2
				and (@Id_DonVi is null or Id_DonVi in (select * from f_split(@Id_DonVi)))
				and (@Id_PhongBan is null or Id_PhongBan=@Id_PhongBan))

union all


-- quyet toan nam truoc
select 
	Id_MucLuc	=Id_MLNhuCau
	,Id_DonVi

	,QT			=sum(TuChi+MuaHang)
	,DT			=0
	,TonKho_DV	=0
	,HuyDong_DV	=0
	,TuChi_DV	=0
	,MuaHang_DV	=0
	,PhanCap_DV	=0
	,HuyDong	=0
	,TuChi		=0
	,MuaHang	=0
	,PhanCap	=0
 from [dbo].[f_skt_nc_qt_nganh_all](@NamLamViec,@NamLamViec-2,@Id_PhongBan,@Id_DonVi,1) 
 where	Id_MLNhuCau is not null
 group by Id_DonVi,Id_MLNhuCau


union all

-- du toan dau nam
select 
	Id_MucLuc	=Id_MLNhuCau
	,Id_DonVi

	,QT		=0
	,DT			=sum(TuChi+MuaHang)
	,TonKho_DV	=0
	,HuyDong_DV	=0
	,TuChi_DV	=0
	,MuaHang_DV	=0
	,PhanCap_DV	=0
	,HuyDong	=0
	,TuChi		=0
	,MuaHang	=0
	,PhanCap	=0
 from [f_skt_nc_dt_nganh_all](@NamLamViec,@NamLamViec-1,@Id_PhongBan,@Id_DonVi,1) 
 where	Id_MLNhuCau is not null
 group by Id_MLNhuCau,id_donvi

 ) as T


 -- lay muc luc nhu cau
 right join (
 select * from SKT_MLNhuCau 
 where	NamLamViec=@NamLamViec 
		and IsParent=0 
		and ((@id_donvi is null and KyHieu like '1-2%') or Nganh_Parent in (select * from f_split(@id_donvi)))) as ml
 on T.Id_MucLuc=ml.Id

 group by Id_MucLuc,KyHieu,Nganh_Parent,Nganh,MoTa

 having
	sum(QT)			 <>0
or	sum(DT)			 <>0
or	sum(TonKho_DV)	<>0
or	sum(HuyDong_DV)	 <>0
or	sum(TuChi_DV)	<>0
or	sum(MuaHang_DV)	 <>0
or	sum(PhanCap_DV)	 <>0
or	sum(HuyDong)	<>0
or	sum(TuChi)		<>0
or	sum(MuaHang)	<>0
or	sum(PhanCap)	<>0
 order by KyHieu


--select		
--		X1=SUBSTRING(KyHieu,1,1),
--		X2=SUBSTRING(KyHieu,1,3),
--		X3=SUBSTRING(KyHieu,1,6),
--		X4=SUBSTRING(KyHieu,1,9)
--		,KyHieu
--		,Nganh_Parent
--		,Nganh
--		,STT
--		,MoTa 
--from	SKT_MLNhuCau 
--where	NamLamViec=@NamLamViec
--		and Len(KyHieu) = 12
--order by	KyHieu

