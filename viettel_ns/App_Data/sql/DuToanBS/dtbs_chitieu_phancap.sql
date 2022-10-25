
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
--declare @iID_MaChungTu nvarchar(MAX)				set @iID_MaChungTu='b99eb11d-d5d2-404c-9e53-086908d23f0c,2d4c4bde-b477-433d-b747-65cd7cfe709e,c9491a39-907e-4b9e-b9d0-f6cc4ddc8323,33f41452-5079-4709-869a-569c52fd41de,c0547e70-5683-40eb-ad20-b24d2a007078'
declare @iID_MaChungTu nvarchar(1000)				set @iID_MaChungTu='56f2d846-c52c-447d-ae93-004b071b5bb2,eacfe458-a102-4dd8-8cb9-29737e61ff46,23e2a77f-f920-4d83-a02b-4819c41a48a9,e869b682-03db-41c2-98c2-48bc5dfa827c,5f7dc13c-5e8f-40e3-928e-58d232b12a89,5984b5d5-fdd5-40a0-8f5d-adb2166fd407,3cfcacb8-c111-40c0-b3da-c693ad537c9e'
--declare @iID_MaChungTu nvarchar(200)				set @iID_MaChungTu='fa234f87-000c-4605-9f82-e49cf2a08fae'
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='51'
declare @iID_MaPhongBan nvarchar(2)					set @iID_MaPhongBan='10'

--#DECLARE#--

SELECT  sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM,sTTM,sNG,
		sMota = [dbo].F_MLNS_MoTa_LNS(@iNamLamviec,sLNS,sL,sK,sM,sTM,sTTM,sNG),
        iID_MaDonVi,sTenDonVi=[dbo].F_GetTenDonVi(@iNamLamviec, @iID_MaDonVi),
		rTongSo		=SUM(rTuChi+rHangNhap+rHangMua+rTonKho+rHienVat+rPhanCap+rDuPhong)/@dvt,
        rTuChi      =SUM(rTuChi)/@dvt, 
        rHienVat    =SUM(rHienVat)/@dvt, 
        rHangNhap   =SUM(rHangNhap)/@dvt, 
        rHangMua    =SUM(rHangMua)/@dvt, 
        rTonKho     =SUM(rTonKho)/@dvt, 
        rPhanCap    =SUM(rPhanCap)/@dvt, 
        rDuPhong    =SUM(rDuPhong)/@dvt
FROM
(
 
 
--SELECT  LEFT(sLNS,1) as sLNS1,
--        LEFT(sLNS,3) as sLNS3,
--        LEFT(sLNS,5) as sLNS5,
--        sLNS,sL,sK,sM,sTM,sTTM,sNG,
--        iID_MaDonVi,
--        rTuChi      =SUM(rTuChi), 
--        rHienVat    =SUM(rHienVat), 
--		rHangNhap	=SUM(rHangNhap),
--		rHangMua	=SUM(rHangMua),
--        rTonKho    =SUM(rTonKho), 
--        rPhanCap    =SUM(rPhanCap), 
--        rDuPhong    =SUM(rDuPhong)
--FROM    DTBS_ChungTuChiTiet 
--WHERE   iTrangThai=1 
--        AND iNamLamViec=@iNamLamViec
--        AND (MaLoai='' or MaLoai='2')
--		AND iID_MaPhongBanDich=@iID_MaPhongBan
--        AND iID_MaDonVi=@iID_MaDonVi
--		AND	iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu))
--		AND (LEFT(sLNS,3) not in (104))
--		AND (LEFT(sLNS,1) not in (2))
--   --     AND (
--			--iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu))
--			--OR iID_MaChungTu IN (SELECT iID_MaChungTuChiTiet FROM DTBS_ChungTuChiTiet WHERE iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu))))
--		--AND LEFT(sLNS,3) not in (104)
--GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sXauNoiMa,iID_MaDonVi
--HAVING  SUM(rTuChi)<>0 
--        OR SUM(rHangNhap)<>0 
--        OR SUM(rHangMua)<>0 
--        OR SUM(rHienVat)<>0 
--        OR SUM(rTonKho)<>0 
--        OR SUM(rPhanCap)<>0 
--        OR SUM(rDuPhong)<>0 


--UNION
                        


SELECT  LEFT(sLNS,1) as sLNS1,
        LEFT(sLNS,3) as sLNS3,
        LEFT(sLNS,5) as sLNS5,
        sLNS,sL,sK,sM,sTM,sTTM,sNG,
        iID_MaDonVi,
        rTuChi      =SUM(rTuChi), 
        rHienVat    =SUM(rHienVat), 
		rHangNhap	=SUM(rHangNhap),
		rHangMua	=SUM(rHangMua),
        rTonKho    =SUM(rTonKho),
        rPhanCap    =SUM(rPhanCap),
        rDuPhong    =SUM(rDuPhong)
--FROM f_ns_dtbs_phancap(@iNamLamViec,@iID_MaPhongBan, @iID_MaDonVi,@iID_MaChungTu)

FROM    DTBS_ChungTuChiTiet_PhanCap 
WHERE   iTrangThai=1 
        AND iNamLamViec=@iNamLamViec
		AND iID_MaPhongBanDich=@iID_MaPhongBan
		AND iID_MaDonVi=@iID_MaDonVi
        AND (
			iID_MaChungTu IN (SELECT iID_MaChungTuChiTiet FROM DTBS_ChungTuChiTiet WHERE iTrangThai=1 and iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu)))
				
			-- phan cap cho cac bql
			OR iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
									where iID_MaChungTu in (select iID_MaDotNganSach from DTBS_ChungTuChiTiet where iTrangThai=1 and iID_MaChungTu in (SELECT * FROM F_Split(@iID_MaChungTu))))

			------ phan cap cho b
			OR iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
								 where iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet where iID_MaChungTu in (SELECT * FROM F_Split(@iID_MaChungTu))))


				-- phan cap lan 2
			OR iID_MaChungTu in (	select iID_MaChungTuChiTiet 
										from DTBS_ChungTuChiTiet 
										where iTrangThai=1 and iID_MaChungTu in (
													select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
													where iTrangThai=1 and iID_MaChungTu in (   
																			select iID_MaChungTu from DTBS_ChungTu
																			where iTrangThai=1 and iID_MaChungTu in (
																									select iID_MaDuyetDuToanCuoiCung from DTBS_ChungTu
																									where iTrangThai=1 and iID_MaChungTu in (select * from F_Split(@iID_MaChungTu))))))


			-- PHAN CAP GUI B KHAC
			OR iID_MaChungTu in (	select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
									where iID_MaChungTu in (   
															select iID_MaChungTu from DTBS_ChungTu
															where iID_MaChungTu in (
																					select iID_MaDuyetDuToanCuoiCung from DTBS_ChungTu
																					where iID_MaChungTu in (select * from F_Split(@iID_MaChungTu)))))

			)

GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi
HAVING  SUM(rTuChi)<>0 
        OR SUM(rHangNhap)<>0 
        OR SUM(rHangMua)<>0 
        OR SUM(rTonKho)<>0 
		OR SUM(rHienVat)<>0
        OR SUM(rPhanCap)<>0 
        OR SUM(rDuPhong)<>0 


) as T1
GROUP BY sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi
