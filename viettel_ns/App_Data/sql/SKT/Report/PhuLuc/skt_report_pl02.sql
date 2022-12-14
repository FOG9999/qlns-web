declare @dvt int									set @dvt = 1000
declare @NamLamViec int								set @NamLamViec = 2020
declare @Id_DonVi nvarchar(20)						set @Id_DonVi='51'
declare @Id_PhongBan nvarchar(20)					set @Id_PhongBan='10'
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

	,TuChi		=sum(0)		 /@dvt
	,HuyDong	=sum(HuyDong)		 /@dvt
	,MuaHang	=sum(MuaHang)		 /@dvt
	,DacThu		=sum(DacThu)		 /@dvt
	,PhanCap	=sum(PhanCap)		 /@dvt

from

(
--union all
-- so kiem tra 2020 cua bql hoac b2
select 
	
		Id_MucLuc
		,TuChi		=0
		,HuyDong	=sum(HuyDong)
		,MuaHang	=sum(MuaHang)
		,PhanCap	=0
		,DacThu		=sum(PhanCap)
from f_skt_nganh(@NamLamViec,@Id_PhongBan,@Id_PhongBan_Dich,@Id_DonVi,1) 
group by Id_MucLuc



------ so chi gián tiếp tại ngành
--union all
--select 
	
--		Id_MucLuc

--		,TuChi		=sum(TuChi + HuyDong + TangGiam)
--		,HuyDong	=0
--		,MuaHang	=0
--		,PhanCap	=0
--		,DacThu		=0
--from f_skt_nc(@NamLamViec,@Id_PhongBan,@Id_PhongBan_Dich,@Id_DonVi,1) 
--group by Id_MucLuc



---- ngan sach nghiep vu
union all
select 
	
		Id_MucLuc

		,TuChi		=0
		,HuyDong	=0
		,MuaHang	=0
		--,PhanCap	=sum(TuChi + HuyDong + TangGiam)
		,PhanCap	=sum(TuChi + HuyDong)
		,DacThu		=0
--from f_skt_nc(@NamLamViec,'06,07,10',null,null,1) 
from f_skt_nc(@NamLamViec,@Id_PhongBan,@Id_PhongBan_Dich,null,1) 
group by Id_MucLuc


 ) as T


 -- lay muc luc nhu cau
 right join (select * from SKT_MLSKT where NamLamViec=@NamLamViec and IsParent=0 and KyHieu like '1-2%') as ml
 on T.Id_MucLuc=ml.Id


 where (@Id_DonVi is null or Nganh_Parent in (select * from f_split(@Id_DonVi)))
 group by Id_MucLuc,KyHieu,Nganh_Parent,Nganh,MoTa
 having 
    sum(HuyDong)			<>0
 or sum(MuaHang)			<>0
 or sum(PhanCap)			<>0
 or sum(DacThu)				<>0

 order by KyHieu
