declare @dvt int									set @dvt = 1000
declare @NamLamViec int								set @NamLamViec = 2020
declare @Id_DonVi nvarchar(20)						set @Id_DonVi='BC-11'
declare @Id_PhongBan nvarchar(20)					set @Id_PhongBan='02'
declare @Id_PhongBan_Dich nvarchar(20)				set @Id_PhongBan_Dich='10'

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
--union all
-- so kiem tra 2020 cua bql
select 
	
		Id_MucLuc

		,HuyDong	=0
		--,TuChi		=sum(TuChi+HuyDong+TangGiam)
		,TuChi		=sum(TuChi+HuyDong)
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
		--,TuChi_B2	=sum(TuChi+HuyDong+TangGiam)
		,TuChi_B2	=sum(TuChi+HuyDong)
		,MuaHang_B2	=0
		,PhanCap_B2	=0
from f_skt_nc(@NamLamViec,'02',@Id_PhongBan_Dich,@Id_DonVi,1) 
group by Id_MucLuc

 ) as T


 -- lay muc luc nhu cau
 right join (select * from SKT_MLNhuCau where NamLamViec=@NamLamViec and IsParent=0) as ml
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
