declare @dvt int									set @dvt = 1000
declare @NamLamViec int								set @NamLamViec = 2019
declare @Id_PhongBan nvarchar(20)					set @Id_PhongBan='02,07,07,08,10' 
declare @Id_DonVi nvarchar(20)						set @Id_DonVi=null
declare @Id_ChungTu uniqueidentifier				set @Id_ChungTu=null
declare @Username nvarchar(20)						set @Username=null
declare @request int								set @request=0
declare @sXauNoiMa int								set @sXauNoiMa=null


/*

author:		longsam
date:		07/12/2018
desc:		Bieu 4a-C, phan chi cho doanh nghiep

*/

--###--

select * from
(

select	
		sLNS,sL,sK, sM,sTM, sTTM,sNG,sMoTa
		,NG=sM+'.'+sTM+'.'+sTTM+'.'+sNG
		,iID_MaDonVi
		,rTuChi		=sum(rTuChi)/@dvt
		,rHangNhap	=sum(rHangNhap)/@dvt
		,rDuPhong	=sum(rDuPhong)/@dvt
from	DT_ChungTuChiTiet
where	iTrangThai in (1,2)
		and iNamLamViec=@NamLamViec
		and (@id_phongban is null or iID_MaPhongBanDich in (select * from f_split(@Id_PhongBan)))
		and (@Id_DonVi is null or iID_MaDonVi in (select * from f_split(@Id_DonVi)))
		and sLNS='1050000'
group by sLNS,sL,sK, sM,sTM, sTTM,sNG,sMoTa,iID_MaDonVi
		
		) as a


-- lay ten don vi
inner join 
(select iID_MaDonVi as dv_id, TenDonVi = sTen
from	NS_DonVi 
where	iTrangThai=1 and iNamLamViec_DonVi=@NamLamViec) as dv
on dv.dv_id=a.iID_MaDonVi
