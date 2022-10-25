declare @dvt int									set @dvt = 1000
declare @NamLamViec int								set @NamLamViec = 2020
declare @Id_DonVi nvarchar(20)						set @Id_DonVi='11'
declare @Id_PhongBan nvarchar(20)					set @Id_PhongBan='07'
declare @Id_PhongBan_Dich nvarchar(20)				set @Id_PhongBan_Dich='07'

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


---- so nhu cau BQL de nghi
--select 
--		Id_MLSKT as Id_MucLuc
--		,QT			=0
--		,DT			=0
--		,SKT		=0
--		,NC			=0
		
		
--		,TonKho_DV	=0
--		,HuyDong_DV	=0
--		,TuChi_DV	=0 

--		,HuyDong
--		,TuChi
--		,MuaHang
--		,PhanCap

--		,HuyDong_B2	=0
--		,TuChi_B2	=0
--		,MuaHang_B2	=0
--		,PhanCap_B2	=0
--from f_skt_nc(@NamLamViec,@NamLamViec,@Id_DonVi,@Id_PhongBan,@dvt) 

--union all

---- so nhu cau B2 de nghi
--select 
--		Id_MLSKT as Id_MucLuc
--		,QT			=0
--		,DT			=0
--		,SKT		=0
--		,NC			=0
		
--		,TonKho_DV	=0
--		,HuyDong_DV	=0
--		,TuChi_DV	=0 


--		,HuyDong	=0
--		,TuChi		=0
--		,MuaHang	=0
--		,PhanCap	=0

--		,HuyDong_B2		=  HuyDong
--		,TuChi_B2		=  TuChi	
--		,MuaHang_B2		=  MuaHang
--		,PhanCap_B2		=  PhanCap
--from f_skt_nc(@NamLamViec,@NamLamViec,@Id_DonVi,'02',@dvt) 



---- quyet toan nam truoc
--union all
--select 
	
--		Id_MLSKT as Id_MucLuc
--		,QT			=sum(TuChi)
--		,DT			=0
--		,SKT		=0
--		,NC			=0
		
--		,TonKho_DV	=0
--		,HuyDong_DV	=0
--		,TuChi_DV	=0 

--		,HuyDong	=0
--		,TuChi		=0
--		,MuaHang	=0
--		,PhanCap	=0

--		,HuyDong_B2	=0
--		,TuChi_B2	=0
--		,MuaHang_B2	=0
--		,PhanCap_B2	=0
--from f_skt_qt(@NamLamViec,@NamLamViec-2,@Id_DonVi,@Id_PhongBan,1) 
--group by Id_MLSKT


-- so nhu cau DV de nghi
--union all
--select 
--		Id_MLSKT as Id_MucLuc
--		,QT			=0
--		,DT			=0
--		,SKT		=0
--		,NC			= HuyDong_DV + TuChi_DV

--		,TonKho_DV
--		,HuyDong_DV
--		,TuChi_DV

--		,HuyDong	=0
--		,TuChi		=0
--		,MuaHang	=0
--		,PhanCap	=0

--		,HuyDong_B2	=0
--		,TuChi_B2	=0
--		,MuaHang_B2	=0
--		,PhanCap_B2	=0
--from f_skt_nc(@NamLamViec,@NamLamViec,@Id_DonVi,@Id_PhongBan,@dvt) 

--union all

---- du toan dau nam
--select 
	
--		Id_MLSKT as Id_MucLuc

--		,QT			=0
--		,DT			=TuChi
--		,SKT		=0
--		,NC			=0
		
--		,TonKho_DV	=0
--		,HuyDong_DV	=0
--		,TuChi_DV	=0 

--		,HuyDong	=0
--		,TuChi		=0
--		,MuaHang	=0
--		,PhanCap	=0

--		,HuyDong_B2	=0
--		,TuChi_B2	=0
--		,MuaHang_B2	=0
--		,PhanCap_B2	=0
--	 from f_skt_dt(@NamLamViec,@NamLamViec-1,@Id_DonVi,@Id_PhongBan,1) 

--union all
-- so kiem tra 2020 cua bql
select 
	
		Id_MucLuc

		,HuyDong	=0
		,TuChi		=sum(TuChi)
		,MuaHang	=0
		,PhanCap	=0

		,HuyDong_B2	=0
		,TuChi_B2	=0
		,MuaHang_B2	=0
		,PhanCap_B2	=0
from f_skt_nc(@NamLamViec,@Id_PhongBan,@Id_PhongBan_Dich,@Id_DonVi,1) 
group by Id_MucLuc



union all
-- so kiem tra 2020 cua b2
select 
	
		Id_MucLuc

		,HuyDong	=0
		,TuChi		=0
		,MuaHang	=0
		,PhanCap	=0

		,HuyDong_B2	=0
		,TuChi_B2	=sum(TuChi)
		,MuaHang_B2	=0
		,PhanCap_B2	=0
from f_skt_nc(@NamLamViec,'02',@Id_PhongBan_Dich,@Id_DonVi,1) 
group by Id_MucLuc

 ) as T


 -- lay muc luc nhu cau
 right join (select * from SKT_MLSKT where NamLamViec=@NamLamViec and IsParent=0) as ml
 on T.Id_MucLuc=ml.Id

 group by Id_MucLuc,KyHieu,Nganh_Parent,Nganh,MoTa
 having 
    sum(HuyDong)			<>0
 or sum(TuChi)				<>0
 or sum(MuaHang)			<>0
 or sum(PhanCap)			<>0
 or sum(HuyDong_B2)			<>0
 or sum(TuChi_B2)			<>0
 or sum(MuaHang_B2)			<>0
 or sum(PhanCap_B2)			<>0

 order by KyHieu
