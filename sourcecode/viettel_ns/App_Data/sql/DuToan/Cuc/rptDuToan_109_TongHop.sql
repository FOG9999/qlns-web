declare @dvt int									set @dvt = 1000
declare @NamLamViec int								set @NamLamViec = 2019
declare @Id_PhongBan nvarchar(20)					set @Id_PhongBan='02,07,07,08,10' 
declare @Id_DonVi nvarchar(20)						set @Id_DonVi=null
declare @Id_ChungTu uniqueidentifier				set @Id_ChungTu=null
declare @Username nvarchar(20)						set @Username=null


/*

author:		longsam
date:		07/12/2018
desc:		Bieu 4b-C, viec nha nuoc giao tinh vao nsqp 109

*/

--###--  

select	sLNS1=left(sLNS,1),
		sLNS3=left(sLNS,3),
		sLNS5=left(sLNS,5),
		* 
from
(
	select	
			sLNS,sL,sK, sM,sTM, sTTM,sNG,sMoTa
			,rTuChi			=sum(case	when iID_MaDonVi not in ('03','75') then rTuChi
										when iID_MaDonVi in ('75') then rChiTapTrung
										else 0 end)/@dvt
			,rChiTapTrung	=sum(case	when iID_MaDonVi in ('03') then rTuChi
										when iID_MaDonVi in ('75') then rTuChi-rChiTapTrung
										else 0 end)/@dvt
			,rPhanCap		=sum(rPhanCap)/@dvt
			,rDuPhong		=sum(rDuPhong)/@dvt
	from	DT_ChungTuChiTiet
	where	iTrangThai=1
			and iNamLamViec=@NamLamViec
			and (@id_phongban is null or iID_MaPhongBanDich in (select * from f_split(@Id_PhongBan)))
			and (@Id_DonVi is null or iID_MaDonVi in (select * from f_split(@Id_DonVi)))
			and (sLNS like '109%' or sLNS='1020500')
	group by sLNS,sL,sK, sM,sTM, sTTM,sNG,sMoTa
) as a
where rTuChi<>0 or rChiTapTrung<>0 or rPhanCap<>0 or rDuPhong<>0
