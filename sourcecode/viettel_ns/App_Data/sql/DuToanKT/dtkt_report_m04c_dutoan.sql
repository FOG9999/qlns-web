
declare @dvt int									set @dvt = 1
declare @nam int									set @nam = 2018 
--declare @nganh	nvarchar(2000)						set @nganh='20,21,22,23,24,25,26,27,28,29,41,42,44,67,69,70,71,72,73,74,75,76,77,78,81,82'
declare @nganh	nvarchar(2000)						set @nganh='00'
declare @username nvarchar(2000)					set @username=null
declare @id_phongban nvarchar(2)					set @id_phongban='08'
declare @id_donvi nvarchar(2000)					set @id_donvi=null
declare @request int								set @request=0


--###--

declare @iID_MaChungTu nvarchar(2000)					set @iID_MaChungTu='B99EB11D-D5D2-404C-9E53-086908D23F0C'


select * from
(

--du toan kiem tra
select	Id_DonVi, Nganh, 
		TuChi		=sum(TuChi)/@dvt,
		DacThu		=sum(DacThu)/@dvt,
		--HangMua		=sum(HangMua)/@dvt,
		Loai = 1
from	DTKT_ChungTuChiTiet
where	iTrangThai=1
		and NamLamViec=@nam
		and iLoai=1
		and (@id_phongban is null or Id_PhongBanDich=@id_phongban)
		and (@id_donvi is null or Id_DonVi in (select * from f_split(@id_donvi)))
		and (@username is null or UserCreator=@username)
		and (@nganh is null or Nganh in (select * from f_split(@nganh)))
		and iRequest=@request
		--and Nganh <> '00'
group by Id_DonVi, Ng, Nganh


union all

-- du toan dau nam - phan cap nganh bao dam ky thuat
select	Id_DonVi = iID_MaDonVi, Nganh=sNG, 
		TuChi	=sum(rTuChi)/@dvt,
		DacThu=0,
		loai=0
from	DT_ChungTuChiTiet
where	iTrangThai=1
		and iNamLamViec=@nam
		and iID_MaNamNganSach=2 and iID_MaNguonNganSach=1
		--and sLNS='1040100' 
		--and iKyThuat=1
		--and MaLoai=1
		and (@id_phongban is null or iID_MaPhongBanDich=@id_phongban)
		--and (@id_donvi is null or Id_DonVi in (select * from f_split(@id_donvi)))
		--and (@username is null or UserCreator=@username)
		and (@nganh is null or sNG in (select * from f_split(@nganh)))
group by iID_MaDonVi, sNG

--du toan dau nam
--select	Id_DonVi = iID_MaDonVi, Nganh=sNG, 
--		TuChi		=sum(rTuChi)/@dvt,
--		DacThu=0,
--		--HangNhap	=sum(rHangNhap)/@dvt,
--		--HangMua		=sum(rHangMua)/@dvt,
--		Loai = 0
--from	DT_ChungTuChiTiet_PhanCap
--where	iTrangThai=1
--		and iNamLamViec=@nam
--		and iID_MaNamNganSach=2 and iID_MaNguonNganSach=1
--		and sLNS='1020100' 
--		and MaLoai in ('','2')
--		and (@id_phongban is null or iID_MaPhongBanDich=@id_phongban)
--		and (@id_donvi is null or iID_MaDonVi in (select * from f_split(@id_donvi)))
--		and (@username is null or sID_MaNguoiDungTao=@username)
--		and (@nganh is null or sNG in (select * from f_split(@nganh)))
--group by iID_MaDonVi, sNG


--union all
-- du toan dau nam - phan cap nganh bao dam ky thuat
--select	Id_DonVi = iID_MaDonVi, Nganh=sNG, 
--		TuChi	=sum(rTuChi)/@dvt,
--		DacThu=0,
--		loai=0
--from	DT_ChungTuChiTiet
--where	iTrangThai=1
--		and iNamLamViec=@nam
--		and iID_MaNamNganSach=2 and iID_MaNguonNganSach=1
--		and sLNS='1040100' 
--		and iKyThuat=1
--		and MaLoai=1
--		and (@id_phongban is null or iID_MaPhongBanDich=@id_phongban)
--		--and (@id_donvi is null or Id_DonVi in (select * from f_split(@id_donvi)))
--		--and (@username is null or UserCreator=@username)
--		and (@nganh is null or sNG in (select * from f_split(@nganh)))
--		and iID_MaChungTuChiTiet not in (
--								select distinct iID_MaChungTu from DT_ChungTuChiTiet_PhanCap
--								where (iID_MaChungTu in (select iID_MaChungTuChiTiet from DT_ChungTuChiTiet where iNamLamViec=@nam and MaLoai in ('1') and iKyThuat=1)) and rTuChi<>0) 
 
--		and iID_MaChungTuChiTiet not in (
--				select iID_MaChungTu from DT_ChungTuChiTiet
--				where	iTrangThai=1 and iNamLamViec=2018
--						and sNG='23'
--						and MaLoai=2)
--		--and iID_MaChungTuChiTiet not in ('6FAE525D-02E3-4F2A-A1FF-D31B59A6F305','F072B143-D20A-4FE2-951D-98018783993C')
--group by iID_MaDonVi, sNG

--du toan bo sung dot ngay 30/3
--union all

--select	Id_DonVi = iID_MaDonVi, Nganh=sNG, 
--		TuChi	=sum(rTuChi)/@dvt,
--		DacThu=0,
--		loai=0
--from	DTBS_ChungTuChiTiet_PhanCap
--where	iTrangThai=1 
--		and iNamLamViec=@nam
--		and MaLoai in ('','2')
--		and iID_MaNamNganSach=2
--		and iID_MaNguonNganSach=1
--		and (@id_phongban is null or iID_MaPhongBanDich=@id_phongban)
--		and (@nganh is null or sNG in (select * from f_split(@nganh)))
--        and (sLNS='1020100')
--		and (
--				iID_MaChungTu IN (SELECT iID_MaChungTuChiTiet FROM DTBS_ChungTuChiTiet WHERE iTrangThai=1 and iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu)))
			 
--			OR	-- phan cap cho b
--				iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
--								 where iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet where iID_MaChungTu in (SELECT * FROM F_Split(@iID_MaChungTu))))
								 
				
--			OR -- phan cap lan 2
--				iID_MaChungTu in (	select iID_MaChungTuChiTiet 
--										from DTBS_ChungTuChiTiet 
--										where iTrangThai=1 and iID_MaChungTu in (
--													select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
--													where iTrangThai=1 and iID_MaChungTu in (   
--																			select iID_MaChungTu from DTBS_ChungTu
--																			where iTrangThai=1 and iID_MaChungTu in (
--																									select iID_MaDuyetDuToanCuoiCung from DTBS_ChungTu
--																									where iTrangThai=1 and iID_MaChungTu in (select * from F_Split(@iID_MaChungTu))))))


			
--			OR -- PHAN CAP GUI B KHAC
--				iID_MaChungTu in (	select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
--									where iID_MaChungTu in (   
--															select iID_MaChungTu from DTBS_ChungTu
--															where iID_MaChungTu in (
--																					select iID_MaDuyetDuToanCuoiCung from DTBS_ChungTu
--																					where iID_MaChungTu in (select * from F_Split(@iID_MaChungTu)))))
--			)
--group by iID_MaDonVi, sNg



) as t1

-- lay ten nganh
inner join 
(select sNG,sNG + ' - ' + sMoTa as TenNganh from NS_MucLucNganSach where iNamLamViec=@nam and iTrangThai=1 and sLNS='') as nganh
on t1.Nganh=nganh.sNG

-- lay ten don vi
inner join 
(select iID_MaDonVi as dv_id, TenDonVi = (iID_MaDonVi +' - ' + sTen) from NS_DonVi where iNamLamViec_DonVi=@nam and iTrangThai=1) as dv
on dv.dv_id=t1.Id_DonVi

where TuChi<> 0 or DacThu<>0
