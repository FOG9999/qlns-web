declare @dvt int									set @dvt = 1000
declare @NamLamViec int								set @NamLamViec = 2019
declare @Id_PhongBan nvarchar(20)					set @Id_PhongBan='02,07,07,08,10' 
declare @Id_DonVi nvarchar(20)						set @Id_DonVi='90'
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
		,rPhanCap2a	=sum(rPhanCap2a)
		,rPhanCap2b	=sum(rPhanCap2b)
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
		--and (@Id_DonVi is null or iID_MaDonVi in (select * from f_split(@Id_DonVi)))
		and MaLoai in ('')
		and sLNS='1040100'
		and (@id_nganh IS NULL OR sNG in (select * from f_split(@id_nganh)))
group by sNG



-- phan cap nganh
union all
select	
		sNG
		,rTuChi		=0.0
		,rHangNhap	=0.0
		,rHangMua	=0.0
		,rPhanCap2a	=sum(rPhanCap)/@dvt
		,rPhanCap2b=0.0
from	DT_ChungTuChiTiet
where	iTrangThai=1
		and iNamLamViec=@NamLamViec
		--and (@Id_DonVi is null or iID_MaDonVi in (select * from f_split(@Id_DonVi)))
		and (@id_phongban is null or iID_MaPhongBanDich in (select * from f_split(@Id_PhongBan)))
		and MaLoai=''
		and sLNS in (1040100) 
		--and sLNS in (1020100) 
group by sNG

-- bo trang bi nhom I

union all		
select	
		sNG
		,rTuChi		=0.0
		,rHangNhap	=0.0
		,rHangMua	=0.0
		,rPhanCap2a	=sum(-rPhanCap)/@dvt
		,rPhanCap2b=0.0
from	DT_ChungTuChiTiet
where	iTrangThai=1
		and iNamLamViec=@NamLamViec
		--and (@Id_DonVi is null or iID_MaDonVi in (select * from f_split(@Id_DonVi)))
		and (@id_phongban is null or iID_MaPhongBanDich in (select * from f_split(@Id_PhongBan)))
		and iID_MaDonVi=51
		and sXauNoiMa='1040100-010-011-6950-6954-10-23'
group by sNG


union all

-- phan cap - donvi lap
select	
		sNG
		,rTuChi		=0.0
		,rHangNhap	=0.0
		,rHangMua	=0.0
		,rPhanCap2a	=0.0
		,rPhanCap2b	=sum(rTuChi)/@dvt
from	DT_ChungTuChiTiet
where	iTrangThai=1
		and iNamLamViec=@NamLamViec
		and (@id_phongban is null or iID_MaPhongBanDich in (select * from f_split(@Id_PhongBan)))
		--and (@Id_DonVi is null or iID_MaDonVi in (select * from f_split(@Id_DonVi)))
		and sLNS='1020100'
		--and iID_MaChungTu not in (select iID_MaChungTu from DT_ChungTu where iTrangThai=2)
		--and iID_MaChungTu not in ('3ac70842-e7fd-4b8c-9ba9-36b1870d4056','40fbd46c-fb94-4d65-b3a3-6ed5d83d7870')
		 --AND iKyThuat=1 AND MaLoai=1 
		--and sNG in (60,61,62,64,65,66)
group by sNG


--union all
--select	
--		sNG
--		,rTuChi		=0.0
--		,rHangNhap	=0.0
--		,rHangMua	=0.0
--		,rPhanCap21=0.0
--		,rPhanCap2b	=sum(rTuChi)/@dvt
--from	DT_ChungTuChiTiet_PhanCap
--where	iTrangThai=1
--		and iNamLamViec=@NamLamViec
--		and (@Id_DonVi is null or iID_MaDonVi in (select * from f_split(@Id_DonVi)))
--		and (@id_phongban is null or iID_MaPhongBanDich in (select * from f_split(@Id_PhongBan)))
--		and MaLoai=''
--		and sLNS='1020100' 
--group by sNG

-- bo trang bi nhom I

--union all		
--select	
--		sNG
--		,rTuChi		=0.0
--		,rHangNhap	=0.0
--		,rHangMua	=0.0
--		,rPhanCap2a=0.0
--		,rPhanCap2b	=sum(rPhanCap)/@dvt
--from	DT_ChungTuChiTiet
--where	iTrangThai=1
--		and iNamLamViec=@NamLamViec
--		--and (@Id_DonVi is null or iID_MaDonVi in (select * from f_split(@Id_DonVi)))
--		and (@id_phongban is null or iID_MaPhongBanDich in (select * from f_split(@Id_PhongBan)))
--		and iID_MaDonVi=51
--		and sXauNoiMa='1040100-010-011-6950-6954-10-23'
--group by sNG


) as t1

group by sNG
--having sum(rTuChi)<> 0 or sum(rHangMua)<>0 or sum(rHangNhap)<>0 or sum(rPhanCap2a)<>0 or sum(rPhanCap2a)<>0
