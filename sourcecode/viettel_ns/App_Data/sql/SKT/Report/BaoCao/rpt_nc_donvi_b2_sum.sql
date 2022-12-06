
declare @dvt int									set @dvt = 1000
declare @NamLamViec int								set @NamLamViec = 2020
--declare @Id_DonVi nvarchar(20)						set @Id_DonVi='10'
declare @Id_DonVi nvarchar(20)						set @Id_DonVi='51,52'
declare @Id_PhongBan nvarchar(20)					set @Id_PhongBan=null
declare @Id_PhongBan_Dich nvarchar(20)				set @Id_PhongBan_Dich=null
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

	,Id_DonVi
	,TenDonVi
	,QT			=sum(QT)			 /@dvt
	,DT			=sum(DT)			 /@dvt

	,HuyDong_B2 =sum(HuyDong_B2)	 /@dvt
	,TuChi_B2	=sum(TuChi_B2)		 /@dvt
	,MuaHang_B2 =sum(MuaHang_B2)	 /@dvt
	,PhanCap_B2	=sum(PhanCap_B2)	 /@dvt

	,HuyDong	=sum(HuyDong)		 /@dvt
	,TuChi		=sum(TuChi)			 /@dvt
	,MuaHang	=sum(MuaHang)		 /@dvt
	,PhanCap	=sum(PhanCap)		 /@dvt

from

(

-- so bql
select 
	Id_MucLuc
	,Id_DonVi
	,QT		=0
	,DT		=0

	,HuyDong_B2		=0
	,TuChi_B2		=0
	,MuaHang_B2		=0
	,PhanCap_B2		=0

	,HuyDong
	,TuChi
	,MuaHang
	,PhanCap

from f_nc_dv_all(@NamLamViec,@Id_PhongBan,@Id_PhongBan_Dich,@Id_DonVi,@dvt) 

-- so giam benh vien tu chu của donvi
union all
select 
	Id_MucLuc
	,Id_DonVi
	,QT		=0
	,DT		=0

	,HuyDong_B2		=0
	,TuChi_B2		=0
	,MuaHang_B2		=0
	,PhanCap_B2		=0

	,HuyDong		=0
	,TuChi			=TangGiam
	,MuaHang		=0
	,PhanCap		=0

from f_nc_bv_giam_dv(@NamLamViec,@Id_PhongBan,@Id_PhongBan_Dich,@Id_DonVi,@dvt) 




-- so B2
union all
select 
	Id_MucLuc
	,Id_DonVi


	,QT		=0
	,DT		=0

	,HuyDong_B2		=HuyDong
	--,TuChi_B2		=TuChi		
	,TuChi_B2		=TuChi	+ TangGiam		
	,MuaHang_B2		=MuaHang
	,PhanCap_B2		=PhanCap

	,HuyDong	=0
	,TuChi		=0
	,MuaHang	=0
	,PhanCap	=0
from f_nc_dv_all(@NamLamViec,'02',@Id_PhongBan_Dich,@Id_DonVi,@dvt)

-- so giam benh vien tu chu của B2
union all
select 
	Id_MucLuc
	,Id_DonVi
	,QT		=0
	,DT		=0

	,HuyDong_B2		=0
	,TuChi_B2		=TangGiam
	,MuaHang_B2		=0
	,PhanCap_B2		=0

	,HuyDong		=0
	,TuChi			=0
	,MuaHang		=0
	,PhanCap		=0

from f_nc_bv_giam_dv(@NamLamViec,'02',@Id_PhongBan_Dich,@Id_DonVi,@dvt) 




---------------------- NGANH

-- NGANH - BQL
union all
select 
	Id_MucLuc
	,Id_DonVi = (select Id_DonVi from SKT_ChungTu where Id=SKT_ChungTuChiTiet.Id_ChungTu)

	,QT		=0
	,DT		=0

	,HuyDong_B2		=0
	,TuChi_B2		=0
	,MuaHang_B2		=0
	,PhanCap_B2		=0

	,HuyDong	=0
	,TuChi		=MuaHang + PhanCap
	,MuaHang	=0
	,PhanCap	=0
 from SKT_ChungTuChiTiet 
 where	Id_ChungTu in 
		(select Id 
		 from SKT_ChungTu 
		 where	NamLamViec=@NamLamViec
				and iLoai=2
				and (@Id_DonVi is null or Id_DonVi in (select * from f_split(@Id_DonVi)))
				and (@Id_PhongBan_Dich is null or Id_PhongBanDich in (select * from f_split(@Id_PhongBan_Dich)))
				and Id_PhongBan<>'02')


---- NGANH B2
union all
select 
	Id_MucLuc
	,Id_DonVi = (select Id_DonVi from SKT_ChungTu where Id=SKT_ChungTuChiTiet.Id_ChungTu)

	,QT		=0
	,DT		=0

	,HuyDong_B2		=HuyDong
	,TuChi_B2		= MuaHang + PhanCap
	,MuaHang_B2		=MuaHang
	,PhanCap_B2		=PhanCap

	,HuyDong	=0
	,TuChi		=0
	,MuaHang	=0
	,PhanCap	=0
 from SKT_ChungTuChiTiet 
 where	Id_ChungTu in 
		(select Id 
		 from SKT_ChungTu 
		 where	NamLamViec=@NamLamViec
				and iLoai=2
				and (@Id_DonVi is null or Id_DonVi in (select * from f_split(@Id_DonVi)))
				and (@Id_PhongBan_Dich is null or Id_PhongBanDich in (select * from f_split(@Id_PhongBan_Dich)))
				and Id_PhongBan='02')



  
---------------------- NGANH --------------------END



---- quyet toan nam truoc
--union all
--select 
--	Id_MucLuc	=Id_MLNhuCau
--	,Id_DonVi

--	--,QT			=TuChi+MuaHang
--	,QT			=sum(TuChi)
--	,DT			=0
	
--	,HuyDong_B2		=0
--	,TuChi_B2		=0
--	,MuaHang_B2		=0
--	,PhanCap_B2		=0

--	,HuyDong	=0
--	,TuChi		=0
--	,MuaHang	=0
--	,PhanCap	=0
-- from [dbo].[f_skt_nc_qt_all](@NamLamViec,@NamLamViec-2,@Id_PhongBan_Dich,@Id_DonVi,1) 
-- where	Id_MLNhuCau is not null
-- group by Id_DonVi,Id_MLNhuCau



----du toan dau nam
union all
select 
	Id_MucLuc	=Id_MLNhuCau
	,Id_DonVi

	,QT		=0
	--,DT		=TuChi+MuaHang
	,DT			=TuChi

	,HuyDong_B2	=0
	,TuChi_B2	=0
	,MuaHang_B2	=0
	,PhanCap_B2	=0

	,HuyDong	=0
	,TuChi		=0
	,MuaHang	=0
	,PhanCap	=0
 from f_skt_nc_dt(@NamLamViec,@NamLamViec-1,@Id_PhongBan_Dich,@Id_DonVi,1) 
 where	Id_MLNhuCau is not null

 ) as T


 -- lay muc luc nhu cau
 right join (select * from SKT_MLNhuCau where NamLamViec=@NamLamViec and IsParent=0) as ml
 on T.Id_MucLuc=ml.Id

 
 
 -- lay ten don vi
 left join (select iID_MaDonVi as dv_id, TenDonVi = iID_MaDonVi + ' - ' + sTen from NS_DonVi where iNamLamViec_DonVi=@NamLamViec) as dv
 on T.Id_DonVi=dv.dv_id


 group by Id_MucLuc,KyHieu,Nganh_Parent,Nganh,MoTa,Id_DonVi,TenDonVi
 having
--	sum(QT)			 <>0
--or	sum(DT)			 <>0
--or	sum(HuyDong_B2)	 <>0
--or	sum(TuChi_B2)	<>0
--or	sum(MuaHang_B2)	 <>0
--or	sum(PhanCap_B2)	 <>0
--or	sum(HuyDong)	<>0
--or	sum(TuChi)		<>0
--or	sum(MuaHang)	<>0
--or	sum(PhanCap)	<>0


	sum(TuChi_B2)	<>0


 order by KyHieu,Id_DonVi


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

