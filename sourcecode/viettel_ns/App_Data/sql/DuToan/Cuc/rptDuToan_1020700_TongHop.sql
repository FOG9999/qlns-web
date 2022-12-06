declare @dvt int									set @dvt = 1000
declare @NamLamViec int								set @NamLamViec = 2019
declare @Id_PhongBan nvarchar(20)					set @Id_PhongBan='02,07,07,08,10' 
declare @Id_DonVi nvarchar(20)						set @Id_DonVi=null
declare @Id_ChungTu uniqueidentifier				set @Id_ChungTu=null
declare @Username nvarchar(20)						set @Username=null
declare @lns int									set @lns=1020700


/*

author:		longsam
date:		07/12/2018
desc:		BHYT

*/

--###--

select	
		sLNS,sL,sK, sM,sTM, sTTM,sNG,sMoTa
		,rTuChi		=sum(rTuChi+rDuPhong)/@dvt
		,rHienVat	=sum(rHienVat)/@dvt
from	DT_ChungTuChiTiet
where	iTrangThai=1
		and iNamLamViec=@NamLamViec
		and (@id_phongban is null or iID_MaPhongBanDich in (select * from f_split(@Id_PhongBan)))
		and (@Id_DonVi is null or iID_MaDonVi in (select * from f_split(@Id_DonVi)))
		and sLNS=@lns
		and (rTuChi<>0 or rDuPhong<>0 or rHienVat<>0)
group by sLNS,sL,sK, sM,sTM, sTTM,sNG,sMoTa
