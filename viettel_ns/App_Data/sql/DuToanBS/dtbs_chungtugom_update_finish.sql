

declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @id_chungtu nvarchar(max)					set @id_chungtu='df49e1d6-4fe3-4e20-8560-1c1fcf2e2b36,793e6855-7ce3-4edb-83c9-2c6313c214be,b428fad1-014b-48bc-b506-4bd8f65d737f,e0e9875a-8445-4af4-8295-6ba2ffdc1296,518383d8-4e8b-468f-8961-6c68dfb0c6ac'
--declare @id_chungtu nvarchar(200)					set @id_chungtu='fa234f87-000c-4605-9f82-e49cf2a08fae'
--declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='10'
--declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07' 
--declare @iID_MaDonVi nvarchar(2000)					set @iID_MaDonVi='10,11,12,13,14,15,17,19,21,22,23,24,29,31,33,41,42,43,44,45,46,47,81,82,83,84,87,95,96,97,98'

declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='10' 
declare @iID_MaDonVi nvarchar(200)					set @iID_MaDonVi='02,03,40,51,52,53,55,56,57,61,65,75,76,77,79,99'

declare @iCheck int									set @iCheck=1
declare @iCheckDate datetime						set @iCheckDate=GETDATE() 


--###--

--select @id_chungtu=coalesce(@id_chungtu + ',','') + id from f_ns_dtbs_chungtugom_dendot(2018,2,@iID_MaPhongBan,getdate())

-- du toan bo sung
UPDATE	DTBS_ChungTuChiTiet
SET		iCheck=@iCheck,iCheckDate=@iCheckDate
FROM	DTBS_ChungTuChiTiet 
WHERE	iTrangThai=1 
		AND iNamLamViec=@iNamLamViec 
		--AND iID_MaNamNganSach IN (SELECT * FROM f_split(@iID_MaNamNganSach))
		--AND (@iID_MaNguonNganSach is null or iID_MaNguonNganSach  IN (SELECT * FROM f_split(@iID_MaNguonNganSach)))
		and iID_MaPhongBanDich=@iID_MaPhongBan
		AND (MaLoai='' OR MaLoai='2')
		AND iBKhac=0
		AND (@iID_MaDonVi is null or iID_MaDonVi in (select * from f_split(@iID_MaDonVi)))
		AND iID_MaChungTu IN (select * from f_split(@id_chungtu))


-- phan cap nsbd binh thuong
UPDATE	DTBS_ChungTuChiTiet_PhanCap
SET		iCheck=@iCheck,iCheckDate=@iCheckDate
from	DTBS_ChungTuChiTiet_PhanCap
where	iTrangThai=1
		and iNamLamViec=@iNamLamViec
		AND (@iID_MaDonVi is null or iID_MaDonVi in (select * from f_split(@iID_MaDonVi)))
		and iID_MaPhongBanDich=@iID_MaPhongBan
		--and iBKhac=1
		--and iID_MaChungTu in (select iID_MaDotNganSach from DTBS_ChungTuChiTiet where iID_MaPhongBanDich=@iID_MaPhongBan and  iID_MaChungTu in (SELECT * FROM f_split(@id_chungtu)))
		and iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet where iTrangThai=1 and iID_MaPhongBanDich=@iID_MaPhongBan and  iID_MaChungTu in (SELECT * FROM f_split(@id_chungtu)))


-- BDKT phan cap binh thuong, từ 104 o chungtuchitiet-> ve 102 phancap
--UPDATE	DTBS_ChungTuChiTiet_PhanCap
--SET		iCheck=@iCheck,iCheckDate=@iCheckDate
--where	iTrangThai=1
--		and iNamLamViec=@iNamLamViec
--		AND (@iID_MaDonVi is null or iID_MaDonVi in (select * from f_split(@iID_MaDonVi)))
--		and iID_MaPhongBanDich=@iID_MaPhongBan
--		--and iBKhac=1
--		and iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet where  iID_MaPhongBanDich=@iID_MaPhongBan and iID_MaChungTu in (select * from f_split(@id_chungtu))) 
 

/*
 BDKT phan cap 2 lan
 */
UPDATE	DTBS_ChungTuChiTiet_PhanCap
SET		iCheck=@iCheck,iCheckDate=@iCheckDate
where	iTrangThai=1
		and iNamLamViec=@iNamLamViec
		and (@iID_MaDonVi is null or iID_MaDonVi in (select * from f_split(@iID_MaDonVi)))
		and iID_MaPhongBanDich=@iID_MaPhongBan
		and MaLoai=2	-- chi co phan cap 2 lan thi maloai =2
		--and iBKhac=0
		--and iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet where  iTrangThai=1 and iID_MaPhongBanDich=@iID_MaPhongBan and iID_MaChungTu  in (select iID_MaDotNganSach from DTBS_ChungTuChiTiet where iTrangThai=1 and iID_MaChungTu in (select * from f_split(@id_chungtu))))
		and iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet where  iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet where  iTrangThai=1 and iID_MaChungTu  in (select iID_MaDuyetDuToanCuoiCung from DTBS_ChungTu where iTrangThai=1 and iID_MaChungTu in (select * from f_split(@id_chungtu)))))
		 