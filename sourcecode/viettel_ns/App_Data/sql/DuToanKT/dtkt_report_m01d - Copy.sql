declare @dvt int									set @dvt = 1000
declare @nam int									set @nam = 2018
declare @Id_PhongBan nvarchar(20)					set @Id_PhongBan='10'
declare @Id_DonVi nvarchar(2000)					set @Id_DonVi=null

--#DECLARE#--/

declare @iID_MaChungTu nvarchar(2000)				set @iID_MaChungTu='B99EB11D-D5D2-404C-9E53-086908D23F0C'

select 
		Code1,Code2,Code3,Code,sMoTa,
		Nganh,
		Id_DonVi, TenDonVi,
		TuChi,
		DacThu,
		Dt_tuchi
from
(
--SELECT		Id_DonVi, SUBSTRING(Code,1,1) as Code1, SUBSTRING(Code,1,3) as Code2, SUBSTRING(Code,1,6) as Code3, Code, Nganh,
--			TuChi	=sum(TuChi)/@dvt,
--			DacThu	=sum(DacThu)/@dvt
--FROM		DTKT_ChungTuChiTiet
--WHERE		iTrangThai = 1
--			and NamLamViec = @nam
--			and iLoai=1
--			and iRequest=0
--			and (@Id_PhongBan is null or Id_PhongBanDich = @Id_PhongBan)
--			and (@Id_DonVi is null or Id_DonVi in (select * from f_split(@Id_DonVi)))
--			and Nganh <> '00'
--			and code in (select Code from DTKT_MucLuc where iTrangThai=1 and NamLamViec=@nam)

--GROUP BY	Id_DonVi, Code, Nganh, sMoTa


select * from
(
select	SUBSTRING(Code,1,1) as Code1, SUBSTRING(Code,1,3) as Code2, SUBSTRING(Code,1,6) as Code3, Code, 
		Nganh, sMoTa
from	DTKT_MucLuc
where	iTrangThai=1
		and NamLamViec=@nam
		and Nganh<>00
) as ml

left join 
	(
	select	
			Code as kt_id,
			--Nganh as kt_id,
			Id_DonVi,
			TuChi	=sum(TuChi)/@dvt,
			DacThu	=sum(DacThu)/@dvt
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
	group by Id_DonVi, Code, sMoTa

	) as kt
	on ml.Code=kt.kt_id

) as t1

join

(

select iID_MaDonVi as dt_dv, sNG as dt_ng, Dt_tuchi = SUM(rTuChi/@dvt)
from    
	(

	-- du toan dau nam - phan cap nganh bao dam ky thuat
	select	iID_MaDonVi, sNG, rTuChi = (rTuChi + rHangMua + rHangNhap)
	from	DT_ChungTuChiTiet
	where	iTrangThai = 1
			and iNamLamViec=@nam 
			and iID_MaNamNganSach=2 and iID_MaNguonNganSach=1
			and sLNS='1040100' 
			and iKyThuat=1
			and MaLoai=1
			and (@id_phongban is null or iID_MaPhongBanDich=@id_phongban)
			--and (@id_donvi is null or Id_DonVi in (select * from f_split(@id_donvi)))
			--and (@nganh is null or sNG in (select * from f_split(@nganh)))
			and iID_MaChungTuChiTiet not in (
									select distinct iID_MaChungTu from DT_ChungTuChiTiet_PhanCap
									where (iID_MaChungTu in (select iID_MaChungTuChiTiet from DT_ChungTuChiTiet where iNamLamViec=@nam and MaLoai in ('1') and iKyThuat=1)) and rTuChi<>0) 
 
			and iID_MaChungTuChiTiet not in (
					select iID_MaChungTu from DT_ChungTuChiTiet
					where	iTrangThai=1 and iNamLamViec=2018
							and sNG='23'
							and MaLoai=2)


	UNION ALL

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

	) as dt_results

	group by iID_MaDonVi, sNG
) as t2
on t1.Id_DonVi = t2.dt_dv and t1.Nganh = t2.dt_ng

-- lay ten don vi
inner join 
(select iID_MaDonVi as dv_id, iID_MaDonVi+ ' - ' + sTen  as TenDonVi from NS_DonVi where iNamLamViec_DonVi=@nam and iTrangThai=1) as dv
on dv.dv_id=t1.Id_DonVi

where	(TuChi<>0 or DacThu<>0 or Dt_tuchi<>0) and Nganh='38'
		--and 1=0



---------------------------

--select * from
--(
--select	SUBSTRING(Code,1,1) as Code1, SUBSTRING(Code,1,3) as Code2, SUBSTRING(Code,1,6) as Code3, Code, 
--		Nganh, sMoTa
--from	DTKT_MucLuc
--where	iTrangThai=1
--		and NamLamViec=@nam
--) as ml

--left join 
--	(
--	select	
--			Code, Nganh,
--			Id_DonVi,
--			TuChi	=sum(TuChi)/@dvt,
--			DacThu	=sum(DacThu)/@dvt
--	from DTKT_ChungTuChiTiet
--	where	iTrangThai = 1
--			and NamLamViec = @nam
--			and iLoai=1
--			and iRequest=0
--			and (@Id_PhongBan is null or Id_PhongBanDich = @Id_PhongBan)
--			and (@Id_DonVi is null or Id_DonVi in (select * from f_split(@Id_DonVi)))
--			and Nganh <> '00'
--			and code in (select Code from DTKT_MucLuc where iTrangThai=1 and NamLamViec=@nam)
--			and (TuChi<>0 or DacThu<>0)
--	group by Id_DonVi, Code, Nganh, sMoTa

--	) as kt
--	on ml.Nganh=kt.Nganh

--SELECT		Id_DonVi, SUBSTRING(Code,1,1) as Code1, SUBSTRING(Code,1,3) as Code2, SUBSTRING(Code,1,6) as Code3, Code, Nganh,
--			TuChi	=sum(TuChi)/@dvt,
--			DacThu	=sum(DacThu)/@dvt
--FROM		DTKT_ChungTuChiTiet
--WHERE		iTrangThai = 1
--			and NamLamViec = @nam
--			and iLoai=1
--			and iRequest=0
--			and (@Id_PhongBan is null or Id_PhongBanDich = @Id_PhongBan)
--			and (@Id_DonVi is null or Id_DonVi in (select * from f_split(@Id_DonVi)))
--			and Nganh <> '00'
--			and code in (select Code from DTKT_MucLuc where iTrangThai=1 and NamLamViec=@nam)

--GROUP BY	Id_DonVi, Code, Nganh, sMoTa


--select * from
--(

--select * from
--(
--select	SUBSTRING(Code,1,1) as Code1, SUBSTRING(Code,1,3) as Code2, SUBSTRING(Code,1,6) as Code3, Code, 
--		Nganh, sMoTa
--from	DTKT_MucLuc
--where	iTrangThai=1
--		and NamLamViec=@nam
--		and Nganh<>'00'
--		and IsParent=0
--) as ml

--left join 
--	(
--	select	
--			Code as kt_id,
--			--Nganh as kt_id,
--			Id_DonVi,
--			TuChi	=sum(TuChi)/@dvt,
--			DacThu	=sum(DacThu)/@dvt
--	from DTKT_ChungTuChiTiet
--	where	iTrangThai = 1
--			and NamLamViec = @nam
--			and iLoai=1
--			and iRequest=0
--			and (@Id_PhongBan is null or Id_PhongBanDich = @Id_PhongBan)
--			and (@Id_DonVi is null or Id_DonVi in (select * from f_split(@Id_DonVi)))
--			and Nganh <> '00'
--			and code in (select Code from DTKT_MucLuc where iTrangThai=1 and NamLamViec=@nam)
--			and (TuChi<>0 or DacThu<>0)
--	group by Id_DonVi, Code, sMoTa

--	) as kt
--	on ml.Code=kt.kt_id
--) as aaa
--where Nganh='38'



--- test
--select iID_MaDonVi as dt_dv, sNG as dt_ng, Dt_tuchi = SUM(rTuChi/@dvt)
--from    
--	(

--	-- du toan dau nam - phan cap nganh bao dam ky thuat
--	select	iID_MaDonVi, sNG, rTuChi = (rTuChi + rHangMua + rHangNhap)
--	from	DT_ChungTuChiTiet
--	where	iTrangThai = 1
--			and iNamLamViec=@nam 
--			and iID_MaNamNganSach=2 and iID_MaNguonNganSach=1
--			and sLNS='1040100' 
--			and iKyThuat=1
--			and MaLoai=1
--			and (@id_phongban is null or iID_MaPhongBanDich=@id_phongban)
--			--and (@id_donvi is null or Id_DonVi in (select * from f_split(@id_donvi)))
--			--and (@nganh is null or sNG in (select * from f_split(@nganh)))
--			and iID_MaChungTuChiTiet not in (
--									select distinct iID_MaChungTu from DT_ChungTuChiTiet_PhanCap
--									where (iID_MaChungTu in (select iID_MaChungTuChiTiet from DT_ChungTuChiTiet where iNamLamViec=@nam and MaLoai in ('1') and iKyThuat=1)) and rTuChi<>0) 
 
--			and iID_MaChungTuChiTiet not in (
--					select iID_MaChungTu from DT_ChungTuChiTiet
--					where	iTrangThai=1 and iNamLamViec=2018
--							and sNG='23'
--							and MaLoai=2)


--	UNION ALL

--	--du toan dau nam - phan cap
--	select	iID_MaDonVi, sNG, rTuChi 
--	from	DT_ChungTuChiTiet_PhanCap
--	where	iTrangThai = 1
--			and iNamLamViec=@nam
--			and iID_MaNamNganSach=2 and iID_MaNguonNganSach=1
--			and sLNS='1020100' 
--			and MaLoai in ('','2')
--			and (@id_phongban is null or iID_MaPhongBanDich=@id_phongban)
--			and (@id_donvi is null or iID_MaDonVi in (select * from f_split(@id_donvi)))
--			--and (@nganh is null or sNG in (select * from f_split(@nganh)))



--	--du toan bo sung dot ngay 30/3
--	union all
--	select	iID_MaDonVi, sNG, rTuChi		
--	from	DTBS_ChungTuChiTiet_PhanCap
--	where	iTrangThai=1 
--			and iNamLamViec=@nam
--			and MaLoai in ('','2')
--			and iID_MaNamNganSach=2
--			and iID_MaNguonNganSach=1
--			and (@id_phongban is null or iID_MaPhongBanDich=@id_phongban)
--			--and (@nganh is null or sNG in (select * from f_split(@nganh)))
--			--and (sLNS='1020100')
--			and (
--					iID_MaChungTu IN (SELECT iID_MaChungTuChiTiet FROM DTBS_ChungTuChiTiet WHERE iTrangThai=1 and iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu)))
			 
--				OR	-- phan cap cho b
--					iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
--										where iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet where iID_MaChungTu in (SELECT * FROM F_Split(@iID_MaChungTu))))
								 
				
--				OR -- phan cap lan 2
--					iID_MaChungTu in (	select iID_MaChungTuChiTiet 
--											from DTBS_ChungTuChiTiet 
--											where iTrangThai=1 and iID_MaChungTu in (
--														select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
--														where iTrangThai=1 and iID_MaChungTu in (   
--																				select iID_MaChungTu from DTBS_ChungTu
--																				where iTrangThai=1 and iID_MaChungTu in (
--																										select iID_MaDuyetDuToanCuoiCung from DTBS_ChungTu
--																										where iTrangThai=1 and iID_MaChungTu in (select * from F_Split(@iID_MaChungTu))))))


			
--				OR -- PHAN CAP GUI B KHAC
--					iID_MaChungTu in (	select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
--										where iID_MaChungTu in (   
--																select iID_MaChungTu from DTBS_ChungTu
--																where iID_MaChungTu in (
--																						select iID_MaDuyetDuToanCuoiCung from DTBS_ChungTu
--																						where iID_MaChungTu in (select * from F_Split(@iID_MaChungTu)))))
--				)

--	) as dt_results

--where	sNG='38'
--group by iID_MaDonVi, sNG
