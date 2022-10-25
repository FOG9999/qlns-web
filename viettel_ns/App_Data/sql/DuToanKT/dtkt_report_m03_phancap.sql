declare @dvt int									set @dvt = 1000
declare @nam int									set @nam = 2018
declare @Id_PhongBan nvarchar(20)					set @Id_PhongBan='07' 
declare @Id_DonVi nvarchar(20)						set @Id_DonVi='12'
declare @Id_ChungTu uniqueidentifier				set @Id_ChungTu=null
declare @Username nvarchar(20)						set @Username=null
declare @request int								set @request=0
declare @nganh	nvarchar(2000)						set @nganh=null

--#DECLARE#--/

declare @iID_MaChungTu nvarchar(2000)					set @iID_MaChungTu='B99EB11D-D5D2-404C-9E53-086908D23F0C'

select * from
(

	--du toan dau nam - phan cap
	select	Id_DonVi = iID_MaDonVi, 
			Nganh=sNG, 
			TuChi		=sum(rTuChi)/@dvt
			--HangNhap	=sum(rHangNhap)/@dvt,
			--HangMua		=sum(rHangMua)/@dvt,
	from	DT_ChungTuChiTiet_PhanCap
	where	iTrangThai=1
			and iNamLamViec=@nam
			and iID_MaNamNganSach=2 and iID_MaNguonNganSach=1
			and sLNS='1020100' 
			and MaLoai in ('','2')
			and (@id_phongban is null or iID_MaPhongBanDich=@id_phongban)
			and (@id_donvi is null or iID_MaDonVi in (select * from f_split(@id_donvi)))
			--and (@nganh is null or sNG in (select * from f_split(@nganh)))
			and sNG<>'00'
	group by iID_MaDonVi, sNG


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
	union all
	select	Id_DonVi = iID_MaDonVi, 
			Nganh=sNG, 
			TuChi	=sum(rTuChi)/@dvt
	from	DTBS_ChungTuChiTiet_PhanCap
	where	iTrangThai=1 
			and iNamLamViec=@nam
			and MaLoai in ('','2')
			and iID_MaNamNganSach=2
			and iID_MaNguonNganSach=1
			and (@id_phongban is null or iID_MaPhongBanDich=@id_phongban)
			and (@id_donvi is null or iID_MaDonVi in (select * from f_split(@id_donvi)))
			--and (@nganh is null or sNG in (select * from f_split(@nganh)))
			and (sLNS='1020100')
			and (
					iID_MaChungTu IN (SELECT iID_MaChungTuChiTiet FROM DTBS_ChungTuChiTiet WHERE iTrangThai=1 and iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu)))
			 
				OR	-- phan cap cho b
					iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
									 where iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet where iID_MaChungTu in (SELECT * FROM F_Split(@iID_MaChungTu))))
								 
				
				OR -- phan cap lan 2
					iID_MaChungTu in (	select iID_MaChungTuChiTiet 
											from DTBS_ChungTuChiTiet 
											where iTrangThai=1 and iID_MaChungTu in (
														select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
														where iTrangThai=1 and iID_MaChungTu in (   
																				select iID_MaChungTu from DTBS_ChungTu
																				where iTrangThai=1 and iID_MaChungTu in (
																										select iID_MaDuyetDuToanCuoiCung from DTBS_ChungTu
																										where iTrangThai=1 and iID_MaChungTu in (select * from F_Split(@iID_MaChungTu))))))


			
				OR -- PHAN CAP GUI B KHAC
					iID_MaChungTu in (	select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
										where iID_MaChungTu in (   
																select iID_MaChungTu from DTBS_ChungTu
																where iID_MaChungTu in (
																						select iID_MaDuyetDuToanCuoiCung from DTBS_ChungTu
																						where iID_MaChungTu in (select * from F_Split(@iID_MaChungTu)))))
				)
	group by iID_MaDonVi, sNg

) as a
