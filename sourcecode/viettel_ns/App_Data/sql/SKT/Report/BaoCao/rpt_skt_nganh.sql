
declare @dvt int									set @dvt = 1000
declare @NamLamViec int								set @NamLamViec = 2020
--declare @Id_DonVi nvarchar(20)						set @Id_DonVi='10'
declare @Id_DonVi nvarchar(20)						set @Id_DonVi='51,52'
declare @Id_PhongBan nvarchar(20)					set @Id_PhongBan='10'
declare @nganh nvarchar(1000)						set @nganh=null
declare @ng nvarchar(1000)							set @ng=null
declare @code nvarchar(1000)						set @code='1-1-01'

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
	,SKT		=sum(SKT)			 /@dvt
	
	,HuyDong_DV	=sum(HuyDong_DV)	/@dvt
	,TuChi_DV	=sum(TuChi_DV)		/@dvt
	,MuaHang_DV	=sum(MuaHang_DV)	/@dvt
	,PhanCap_DV	=sum(PhanCap_DV)	/@dvt


	,HuyDong	=sum(HuyDong)		 /@dvt
	,TuChi		=sum(TuChi)			 /@dvt
	,MuaHang	=sum(MuaHang)		 /@dvt
	,PhanCap	=sum(PhanCap)		 /@dvt

	,HuyDong_B2 =sum(HuyDong_B2)	 /@dvt
	,TuChi_B2	=sum(TuChi_B2)		 /@dvt
	,MuaHang_B2 =sum(MuaHang_B2)	 /@dvt
	,PhanCap_B2	=sum(PhanCap_B2)	 /@dvt

from

(


-- so nhu cau BQL de nghi
select 
		Id_MLSKT as Id_MucLuc
		,QT			=0
		,DT			=0
		,SKT		=0
		
		
		,HuyDong_DV
		,TuChi_DV
		,MuaHang_DV
		,PhanCap_DV

		,HuyDong
		,TuChi
		,MuaHang
		,PhanCap

		,HuyDong_B2	=0
		,TuChi_B2	=0
		,MuaHang_B2	=0
		,PhanCap_B2	=0
from f_skt_nganh(@NamLamViec,@NamLamViec,@Id_DonVi,@Id_PhongBan,@dvt) 

union all
-- so nhu cau B2 de nghi
select 
		Id_MLSKT as Id_MucLuc
		,QT			=0
		,DT			=0
		,SKT		=0
		
		,HuyDong_DV
		,TuChi_DV
		,MuaHang_DV
		,PhanCap_DV

		,HuyDong
		,TuChi
		,MuaHang
		,PhanCap

		,HuyDong_B2		=  HuyDong
		,TuChi_B2		=  TuChi	
		,MuaHang_B2		=  MuaHang
		,PhanCap_B2		=  PhanCap
from f_skt_nganh(@NamLamViec,@NamLamViec,@Id_DonVi,'02',@dvt) 

union all
-- quyet toan nam truoc
select 
	
		Id_MLSKT as Id_MucLuc
		,QT			=sum(TuChi+MuaHang)
		,DT			=0
		,SKT		=0
		
		,HuyDong_DV	=0
		,TuChi_DV	=0
		,MuaHang_DV	=0
		,PhanCap_DV	=0

		,HuyDong	=0
		,TuChi		=0
		,MuaHang	=0
		,PhanCap	=0

	 

		,HuyDong_B2	=0
		,TuChi_B2	=0
		,MuaHang_B2	=0
		,PhanCap_B2	=0
from f_skt_qt_nganh(@NamLamViec,@NamLamViec-2,@Id_DonVi,@Id_PhongBan,1) 
group by Id_MLSKT

union all

-- du toan dau nam
select 
	
		Id_MLSKT as Id_MucLuc

		,QT			=0
		,DT			= MuaHang
		,SKT		=0
		
		,HuyDong_DV	=0
		,TuChi_DV	=0
		,MuaHang_DV	=0
		,PhanCap_DV	=0

		,HuyDong	=0
		,TuChi		=0
		,MuaHang	=0
		,PhanCap	=0

	 

		,HuyDong_B2	=0
		,TuChi_B2	=0
		,MuaHang_B2	=0
		,PhanCap_B2	=0
	 from f_skt_dt_nganh(@NamLamViec,@NamLamViec-1,@Id_DonVi,@Id_PhongBan,1) 
	 --from f_skt_dt_nganh(2020,2019,'51','10',1) 

union all
-- so kiem tra 2018
select 
	
		Id_MLSKT as Id_MucLuc

		,QT			=0
		,DT			=0
		,SKT		=sum(TuChi+MuaHang)
		
		,HuyDong_DV	=0
		,TuChi_DV	=0
		,MuaHang_DV	=0
		,PhanCap_DV	=0

		,HuyDong	=0
		,TuChi		=0
		,MuaHang	=0
		,PhanCap	=0

	 

		,HuyDong_B2	=0
		,TuChi_B2	=0
		,MuaHang_B2	=0
		,PhanCap_B2	=0
from f_skt_skt_nganh(@NamLamViec,@NamLamViec-2,@Id_DonVi,@Id_PhongBan,1) 
group by Id_MLSKT

 ) as T

 -- lay muc luc nhu cau
 right join (select * from SKT_MLSKT where NamLamViec=@NamLamViec and IsParent=0) as ml
 on T.Id_MucLuc=ml.Id

 where	 KyHieu like '1-2%'	--nganh bao dam
		and (@Id_DonVi is null or Nganh_Parent in (select * from f_split(@Id_DonVi)))
 group by Id_MucLuc,KyHieu,Nganh_Parent,Nganh,MoTa
 having 
    sum(QT)					<>0
 or sum(DT)					<>0
 or sum(SKT)				<>0
 or sum(HuyDong_DV)			<>0
 or sum(MuaHang_DV)			<>0
 or sum(PhanCap_DV)			<>0


 or sum(HuyDong)			<>0
 or sum(TuChi)				<>0
 or sum(MuaHang)			<>0
 or sum(PhanCap)			<>0
 or sum(HuyDong_B2)			<>0
 or sum(TuChi_B2)			<>0
 or sum(MuaHang_B2)			<>0
 or sum(PhanCap_B2)			<>0
 order by KyHieu
