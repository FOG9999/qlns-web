

declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iID_MaChungTu nvarchar(200)				set @iID_MaChungTu='df49e1d6-4fe3-4e20-8560-1c1fcf2e2b36,793e6855-7ce3-4edb-83c9-2c6313c214be,b428fad1-014b-48bc-b507-4bd8f65d737f,e0e9875a-8445-4af4-8295-6ba2ffdc1296,518383d8-4e8b-468f-8961-6c68dfb0c6ac'
--declare @iID_MaChungTu nvarchar(200)				set @iID_MaChungTu='fa234f87-000c-4605-9f82-e49cf2a08fae'
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='10'
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07' 
declare @sLNS nvarchar(100)							set @sLNS='1,2'



--#DECLARE#--
 
-- SELECT  LEFT(sLNS,1) as sLNS1,
--        LEFT(sLNS,3) as sLNS3,
--        LEFT(sLNS,5) as sLNS5,
--        sLNS,sL,sK,sM,sTM,sTTM,sNG,
--        iID_MaDonVi,

--        rTuChi      =SUM(rTuChi+rHangNhap+rHangMua), 
--		rTuChi		=SUM(rTuChi), 
--        rHangNhap    =SUM(rHangNhap), 
--        rHangMua    =SUM(rHangMua), 
--        rHienVat    =SUM(rHienVat), 
--        rPhanCap    =SUM(rPhanCap), 
--        rDuPhong    =SUM(rDuPhong)
----FROM	f_ns_dtbs_phancap(@iNamLamViec,@iID_MaPhongBan, @iID_MaDonVi,@iID_MaChungTu) 
--from

--(

select *
FROM    DTBS_ChungTuChiTiet_PhanCap 
WHERE   iTrangThai=1 
        AND iNamLamViec=@iNamLamViec
        AND iID_MaDonVi=@iID_MaDonVi
		AND iID_MaPhongBanDich=@iID_MaPhongBan

		--AND iBKhac='0'
        AND (
			iID_MaChungTu IN (SELECT iID_MaChungTuChiTiet FROM DTBS_ChungTuChiTiet WHERE iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu)))
				
			-- phan cap cho cac bql
			--OR iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
			--						where iID_MaChungTu in (select iID_MaDotNganSach from DTBS_ChungTuChiTiet where iID_MaChungTu in (SELECT * FROM F_Split(@iID_MaChungTu))))

			---- phan cap cho b
			--OR iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
			--					 where iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet where iID_MaChungTu in (SELECT * FROM F_Split(@iID_MaChungTu))))


			-- phan cap lan 2
			--OR iID_MaChungTu in (	select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
			--						where iID_MaChungTu in (
			--												select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
			--												where iID_MaChungTu in (   
			--																		select iID_MaChungTu from DTBS_ChungTu
			--																		where iID_MaChungTu in (
			--																								select iID_MaDuyetDuToanCuoiCung from DTBS_ChungTu
			--																								where iID_MaChungTu in (select * from F_Split(@iID_MaChungTu))))))

			-- PHAN CAP GUI B KHAC
			OR 
			iID_MaChungTu in (	select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
									where iID_MaChungTu in (   
															select iID_MaChungTu from DTBS_ChungTu
															where iID_MaChungTu in (
																					select iID_MaDuyetDuToanCuoiCung from DTBS_ChungTu
																					where iID_MaChungTu in (select * from F_Split(@iID_MaChungTu)))))



			)

--) as t


--GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi
--HAVING  SUM(rTuChi+rHangNhap+rHangMua)<>0 
--        OR SUM(rHienVat)<>0 
--        OR SUM(rPhanCap)<>0 
--        OR SUM(rDuPhong)<>0 
