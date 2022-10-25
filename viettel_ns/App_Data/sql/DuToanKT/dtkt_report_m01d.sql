declare @dvt int									set @dvt = 1000
declare @nam int									set @nam = 2018
declare @Id_PhongBan nvarchar(20)					set @Id_PhongBan='07'
declare @Id_DonVi nvarchar(2000)					set @Id_DonVi=null
declare @request int								set @request=NULL
--#DECLARE#--/

declare @iID_MaChungTu nvarchar(2000)				set @iID_MaChungTu='B99EB11D-D5D2-404C-9E53-086908D23F0C'

select 	
		SUBSTRING(Code,1,1) as Code1, SUBSTRING(Code,1,3) as Code2, SUBSTRING(Code,1,6) as Code3,
		Code,
		Nganh,
		Id_DonVi, TenDonVi,
		TuChi		=sum(TuChi)/@dvt,
		DacThu		=sum(DacThu)/@dvt,
		TangNV		=sum(TangNV)/@dvt,
		GiamNV		=sum(GiamNV)/@dvt,
		Dt_tuchi	=sum(Dt_tuchi)/@dvt
		--*
from
(
	--du toan kiem tra (dot 0)
	select	
			Nganh,
			Id_DonVi,
			TuChi		=sum(TuChi),
			DacThu		=sum(DacThu),
			TangNV		=0,
			GiamNV		=0,
			Dt_tuchi	=0
	from DTKT_ChungTuChiTiet
	where	iTrangThai = 1
			and NamLamViec = @nam
			and iLoai=1
			and iRequest=0
			and (@Id_PhongBan is null or Id_PhongBanDich = @Id_PhongBan)
			and (@Id_DonVi is null or Id_DonVi in (select * from f_split(@Id_DonVi)))
			and Nganh <> '00'
			and code in (select Code from DTKT_MucLuc where iTrangThai=1 and NamLamViec=@nam)
			and (TuChi<>0 or DacThu<>0)
	group by Id_DonVi, Nganh

	--du toan kiem tra (cac dot tang giam nhiem vu)
	union all
	select	
			Nganh,
			Id_DonVi,
			TuChi		=0,
			DacThu		=0,
			TangNV		=sum(TangNV),
			GiamNV		=sum(GiamNV),
			Dt_tuchi	=0
	from DTKT_ChungTuChiTiet
	where	iTrangThai = 1
			and NamLamViec = @nam
			and iLoai=1
			and iRequest=1
			and (@request is null or iLan <= 1)
			and (@Id_PhongBan is null or Id_PhongBanDich = @Id_PhongBan)
			and (@Id_DonVi is null or Id_DonVi in (select * from f_split(@Id_DonVi)))
			and Nganh <> '00'
			and code in (select Code from DTKT_MucLuc where iTrangThai=1 and NamLamViec=@nam)
			and (TangNV<>0 or GiamNV<>0)
	group by Id_DonVi, Nganh


	union all


	--du toan dau nam
	select	
			Nganh		=sNG,
			Id_DonVi	=iID_MaDonVi,
			TuChi=0,
			DacThu=0,
			TangNV=0,
			GiamNV=0,
			Dt_tuchi	=sum(rTuChi)
		
	from    
	(

		-- du toan dau nam - phan cap nganh bao dam ky thuat
		--select	iID_MaDonVi, sNG, rTuChi = (rTuChi + rHangMua + rHangNhap)
		--from	DT_ChungTuChiTiet
		--where	iTrangThai = 1
		--		and iNamLamViec=@nam 
		--		and iID_MaNamNganSach=2 and iID_MaNguonNganSach=1
		--		and sLNS like '102%' 
		--		--and sLNS='1040100' 
		--		and iKyThuat=1
		--		and MaLoai=1
		--		and (@id_phongban is null or iID_MaPhongBanDich=@id_phongban)
		--		--and (@id_donvi is null or Id_DonVi in (select * from f_split(@id_donvi)))
		--		--and (@nganh is null or sNG in (select * from f_split(@nganh)))
		--		and iID_MaChungTuChiTiet not in (
		--								select distinct iID_MaChungTu from DT_ChungTuChiTiet_PhanCap
		--								where (iID_MaChungTu in (select iID_MaChungTuChiTiet from DT_ChungTuChiTiet where iNamLamViec=@nam and MaLoai in ('1') and iKyThuat=1)) and rTuChi<>0) 
 
		--		and iID_MaChungTuChiTiet not in (
		--				select iID_MaChungTu from DT_ChungTuChiTiet
		--				where	iTrangThai=1 and iNamLamViec=2018
		--						and sNG='23'
		--						and MaLoai=2)


		--UNION ALL

		--du toan dau nam - phan cap
		select	iID_MaDonVi, sNG, rTuChi 
		from	DT_ChungTuChiTiet_PhanCap
		where	iTrangThai = 1
				and iNamLamViec=@nam
				and iID_MaNamNganSach=2 and iID_MaNguonNganSach=1
				and sLNS='1020100' 
				and MaLoai in ('','2')
				and (@id_phongban is null or iID_MaPhongBanDich=@id_phongban)
				and (@id_donvi is null or iID_MaDonVi in (select * from f_split(@id_donvi)))
				--and (@nganh is null or sNG in (select * from f_split(@nganh)))



		--du toan bo sung dot ngay 30/3
		union all
		select	iID_MaDonVi, sNG, rTuChi		
		from	DTBS_ChungTuChiTiet_PhanCap
		where	iTrangThai=1 
				and iNamLamViec=@nam
				and MaLoai in ('','2')
				and iID_MaNamNganSach=2
				and iID_MaNguonNganSach=1
				and (@id_phongban is null or iID_MaPhongBanDich=@id_phongban)
				and (@id_donvi is null or iID_MaDonVi in (select * from f_split(@id_donvi)))
				--and (@nganh is null or sNG in (select * from f_split(@nganh)))
				--and (sLNS='1020100')
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
		) as dt
		group by iID_MaDonVi, sNG
		

) as a

 --lay dtkt_mucluc
inner join 
	(select  Nganh as ml_id,
			SUBSTRING(Code,1,1) as Code1, SUBSTRING(Code,1,3) as Code2, SUBSTRING(Code,1,6) as Code3,Code
	 from	DTKT_MucLuc where iTrangThai=1 and NamLamViec=@nam and IsParent=0 and Loai like '1%' and RIGHT(Code,2)='00') as ml
on	ml.ml_id=a.Nganh

 --lay ten don vi
inner join 
	(select iID_MaDonVi as dv_id, iID_MaDonVi+ ' - ' + sTen  as TenDonVi from NS_DonVi where iNamLamViec_DonVi=@nam and iTrangThai=1) as dv
on	dv.dv_id=a.Id_DonVi

where	(TuChi<>0 or DacThu<>0 or Dt_tuchi<>0 or TangNV<>0 or GiamNV<>0)
		--and Nganh='26'
group by Id_DonVi
		, TenDonVi
		, Nganh
		, Code
ORDER BY Nganh
 