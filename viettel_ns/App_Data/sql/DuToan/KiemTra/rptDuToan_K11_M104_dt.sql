declare @dvt int									set @dvt = 1000
declare @NamLamViec int								set @NamLamViec = 2019
declare @Id_PhongBan nvarchar(20)					set @Id_PhongBan='02,07,07,08,10' 
declare @Id_DonVi nvarchar(20)						set @Id_DonVi='12'
declare @id_nganh nvarchar(20)						set @id_nganh='20,21,22,23,24,25,26,27,28,29,41,42,44,67,69,70,71,72,73,74,75,76,77,78,81,82'
declare @Id_ChungTu uniqueidentifier				set @Id_ChungTu=null
declare @Username nvarchar(20)						set @Username=null
declare @request int								set @request=0
declare @sXauNoiMa int								set @sXauNoiMa=null

--#DECLARE#--/

select	
		sNG
		,rTuChi		=sum(rTuChi)
		,rHangNhap	=sum(rHangNhap)
		,rHangMua	=sum(rHangMua)
		,rPhanCap2a=0.0
		,rPhanCap2b=0.0
from
(

-- du toan
-- tu chi
select	
		sNG
		,rTuChi		=sum(rTuChi)/@dvt
		,rHangNhap	=sum(rHangNhap)/@dvt
		,rHangMua		=sum(rHangMua)/@dvt
		,rPhanCap2a=0.0
		,rPhanCap2b=0.0
from	DT_ChungTuChiTiet
where	iTrangThai=1
		and iNamLamViec=@NamLamViec
		and (@id_phongban is null or iID_MaPhongBanDich in (select * from f_split(@Id_PhongBan)))
		--and iID_MaPhongBanDich != '07'
		--and (@Id_DonVi is null or iID_MaDonVi in (select * from f_split(@Id_DonVi)))
		and MaLoai in ('',2)
		--and left(sLNS,3) in (104)
		and sLNS='1040100'

		and (@id_nganh IS NULL OR sNG in (select * from f_split(@id_nganh)))
		--and (sNG='00' or  left(sLNS,3) not in (109))
group by --sLNS,sL,sK, sM,sTM, sTTM,
		sNG

union all
-- phan cap
-- tu chi
select	
		--Id_DonVi	=iID_MaDonVi,
		--sLNS,sL,sK, sM,sTM, sTTM,sNG
		sNG
		,rTuChi		=sum(rTuChi+rHangNhap+rHangMua)/@dvt
		,rHangNhap	=0
		,rHangMua	=0
		,rPhanCap2a=0.0
		,rPhanCap2b=0.0
from	DT_ChungTuChiTiet_PhanCap
where	iTrangThai=1
		and iNamLamViec=@NamLamViec
		and iID_MaDonVi=@Id_DonVi
		and (@id_phongban is null or iID_MaPhongBanDich in (select * from f_split(@Id_PhongBan)))
		--and left(sLNS,3) not in (104)
group by sNG

) as t1

where rTuChi<> 0 or rHangMua<>0 or rHangNhap<>0
group by sNG
