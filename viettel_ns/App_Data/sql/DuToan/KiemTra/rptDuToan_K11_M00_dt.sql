declare @dvt int									set @dvt = 1000
declare @NamLamViec int								set @NamLamViec = 2019
declare @Id_PhongBan nvarchar(20)					set @Id_PhongBan='02,07,07,08,10' 
declare @Id_DonVi nvarchar(20)						set @Id_DonVi='40'
declare @Id_ChungTu uniqueidentifier				set @Id_ChungTu=null
declare @Username nvarchar(20)						set @Username=null
declare @request int								set @request=0
declare @sXauNoiMa int								set @sXauNoiMa=null

--#DECLARE#--/

select	sLNS,sL,sK, sM,sTM, sTTM, sNG
		,sXauNoiMa
		,rTuChi
		
from
(

-- du toan
-- tu chi
select	Id_DonVi	=iID_MaDonVi, 
		sLNS,sL,sK, sM,sTM, sTTM,sNG
		,sXauNoiMa = sLNS + '-' + sL + '-' + sK+ '-' +sM+ '-' +sTM+ '-' +sTTM+ '-' +sNG
		,rTuChi		=sum(rTuChi+rHangNhap+rHangMua)/@dvt
from	DT_ChungTuChiTiet
where	iTrangThai=1
		and iNamLamViec=@NamLamViec
		and (@id_phongban is null or iID_MaPhongBanDich in (select * from f_split(@Id_PhongBan)))
		and (@Id_DonVi is null or iID_MaDonVi in (select * from f_split(@Id_DonVi)))
		and left(sLNS,3) not in (104)
		and left(sLNS,1) in (1)	
group by iID_MaDonVi,sLNS,sL,sK, sM,sTM, sTTM,sNG

--union all
---- phan cap
---- tu chi
--select	Id_DonVi	=iID_MaDonVi,
--		sLNS,sL,sK, sM,sTM, sTTM,sNG
--		,sXauNoiMa = sLNS + '-' + sL + '-' + sK+ '-' +sM+ '-' +sTM+ '-' +sTTM+ '-' +sNG
--		,rTuChi		=sum(rTuChi+rHangNhap+rHangMua)/@dvt
--from	DT_ChungTuChiTiet_PhanCap
--where	iTrangThai=1
--		and iNamLamViec=@NamLamViec
--		and iID_MaDonVi=@Id_DonVi
--		and (@id_phongban is null or iID_MaPhongBanDich in (select * from f_split(@Id_PhongBan)))
--		--and left(sLNS,3) not in (104)
--group by iID_MaDonVi,sLNS,sL,sK, sM,sTM, sTTM,sNG

) as t1

--where sNG='00'
