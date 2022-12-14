declare @dvt int									set @dvt = 1000
declare @NamLamViec int								set @NamLamViec = 2020
declare @Id_DonVi nvarchar(20)						set @Id_DonVi='51'
declare @Id_PhongBan nvarchar(20)					set @Id_PhongBan='02'
declare @Id_PhongBan_Dich nvarchar(20)				set @Id_PhongBan_Dich=null

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
	,TenDonVi = case Id_DonVi when '' then N'Tổng cộng' 
							  when 'GS' then N'Để lại Bộ giao sau' 
							  else TenDonVi end
	,C2		=sum(TuChi)		 /@dvt

from

(



-- ngan sach nghiep vu
select 
	
		Id_MucLuc
		,Id_DonVi = case Id_DonVi when 'TC' then 'GS' else Id_DonVi end 
		,TuChi		=sum(TuChi+TangGiam)
from f_nc_dv_all(@NamLamViec,@Id_PhongBan,@Id_PhongBan_Dich,null,1) 
--from f_nc_dv(@NamLamViec,@Id_PhongBan,@Id_PhongBan_Dich,null,1) 
group by Id_MucLuc,Id_DonVi

---- giảm bệnh viện tự chủ
union all
select 
	
		Id_MucLuc
		--,Id_DonVi = case Id_DonVi when 'TC-10' then 'GS-10' else Id_DonVi end 
		,Id_DonVi = left(Id_DonVi,2)
		,TuChi		=sum(TangGiam)
from f_nc_bv_giam_dv(@NamLamViec,@Id_PhongBan,@Id_PhongBan_Dich,null,1) 
group by Id_MucLuc,Id_DonVi


---- lay tong cong
union all
select 
	
		Id_MucLuc
		,Id_DonVi=''
		,TuChi		=sum(TuChi+TangGiam)
--from f_nc_dv(@NamLamViec,@Id_PhongBan,@Id_PhongBan_Dich,null,1) 
from f_nc_dv_all(@NamLamViec,@Id_PhongBan,@Id_PhongBan_Dich,null,1)			-- lay so Tong Hop tat ca cho b2

where Id_DonVi not in ('GS','TC','00')

group by Id_MucLuc,Id_DonVi

---- giảm bệnh viện tự chủ
union all
select 
	
		Id_MucLuc
		,Id_DonVi=''
		,TuChi		=sum(TangGiam)
from f_nc_bv_giam_dv(@NamLamViec,@Id_PhongBan,@Id_PhongBan_Dich,null,1) 
--- bo cuc
where Id_DonVi not in ('GS','TC','00')
group by Id_MucLuc,Id_DonVi





----------------- LAY NGANH BAO DAM


union all
select 
	Id_MucLuc
	,Id_DonVi = (select left(Id_DonVi,2) from SKT_ChungTu where Id=SKT_ChungTuChiTiet.Id_ChungTu)
	,TuChi		= MuaHang + PhanCap
 from SKT_ChungTuChiTiet 
 where	Id_ChungTu in 
		(select Id 
		 from SKT_ChungTu 
		 where	NamLamViec=@NamLamViec
				and iLoai=2
				--and (@Id_DonVi is null or Id_DonVi in (select * from f_split(@Id_DonVi)))
				and (@Id_PhongBan_Dich is null or Id_PhongBanDich in (select * from f_split(@Id_PhongBan_Dich)))
				and Id_PhongBan in (select * from f_split(@Id_PhongBan)))
		 


				
union all -- tong nganh
select 
	Id_MucLuc
	,Id_DonVi = ''
	,TuChi		= MuaHang + PhanCap
 from SKT_ChungTuChiTiet 
 where	Id_ChungTu in 
		(select Id 
		 from SKT_ChungTu 
		 where	NamLamViec=@NamLamViec
				and iLoai=2
				--- bo cuc
				and Id_DonVi not in ('GS','TC','00')
				--and (@Id_DonVi is null or Id_DonVi in (select * from f_split(@Id_DonVi)))
				and (@Id_PhongBan_Dich is null or Id_PhongBanDich in (select * from f_split(@Id_PhongBan_Dich)))
				and Id_PhongBan in (select * from f_split(@Id_PhongBan)))

		





----------------- LAY NGANH BAO DAM -------------- END


 ) as T


 -- lay muc luc nhu cau
 right join (select * from SKT_MLNhuCau where NamLamViec=@NamLamViec and IsParent=0
  --and KyHieu like '1-2%'
  ) as ml
 on T.Id_MucLuc=ml.Id


 -- lay ten don vi
 left join (select iID_MaDonVi as dv_id, TenDonVi =  sTen from NS_DonVi where iTrangThai=1 and iNamLamViec_DonVi=@NamLamViec) as dv
 --on (T.Id_DonVi<>'' and dv.dv_id like T.Id_DonVi + '%')
 on dv.dv_id =T.Id_DonVi or T.Id_DonVi+'-10' =dv.dv_id


 --where (@Id_DonVi is null or Nganh_Parent in (select * from f_split(@Id_DonVi)))
 --where Nganh_Parent='53'


 -- bo tai cục
 where Id_DonVi not in ('GS','TC','00')
 group by Id_MucLuc,KyHieu,Nganh_Parent,Nganh,MoTa,Id_DonVi,TenDonVi
 having 
    sum(TuChi)			<>0

 order by KyHieu
