
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iID_MaChungTu nvarchar(200)				set @iID_MaChungTu='b99eb11d-d5d2-404c-9e53-086908d23f0c,2d4c4bde-b477-433d-b747-65cd7cfe709e,c9491a39-907e-4b9e-b9d0-f6cc4ddc8323,33f41452-5079-4709-869a-569c52fd41de,c0547e70-5683-40eb-ad20-b24d2a007078'
--declare @iID_MaChungTu nvarchar(200)				set @iID_MaChungTu='fa234f87-000c-4605-9f82-e49cf2a08fae'
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='51'
declare @iID_MaPhongBan nvarchar(2)					set @iID_MaPhongBan='10'
declare @sLNS nvarchar(100)							set @sLNS='1'

--#DECLARE#--



SELECT  sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM,sTTM,sNG,sXauNoiMa,sMota = [dbo].F_MLNS_MoTa_LNS(@iNamLamviec,sLNS,sL,sK,sM,sTM,sTTM,sNG),
        iID_MaDonVi,sTenDonVi=[dbo].F_GetTenDonVi(@iNamLamviec, @iID_MaDonVi),
		rTongSo		=SUM(rTuChi+rHangNhap+rHangMua+rTonKho+rPhanCap+rDuPhong)/@dvt,
        rTuChi      =SUM(rTuChi)/@dvt, 
        rHangNhap   =SUM(rHangNhap)/@dvt, 
        rHangMua    =SUM(rHangMua)/@dvt, 
        rTonKho     =SUM(rTonKho)/@dvt, 
        rHienVat    =SUM(rHienVat)/@dvt, 
        rPhanCap    =SUM(rPhanCap)/@dvt, 
        rDuPhong    =SUM(rDuPhong)/@dvt
FROM
(

SELECT  LEFT(sLNS,1) as sLNS1,
        LEFT(sLNS,3) as sLNS3,
        LEFT(sLNS,5) as sLNS5,
        sLNS,sL,sK,sM,sTM,sTTM,sNG,sXauNoiMa,
        iID_MaDonVi,
        rTuChi      =SUM(rTuChi), 
		rHangNhap	=SUM(rHangNhap),
		rHangMua	=SUM(rHangMua),
        rTonKho    =SUM(rTonKho), 
        rHienVat    =SUM(rHienVat), 
        rPhanCap    =SUM(rPhanCap), 
        rDuPhong    =SUM(rDuPhong)
FROM    DTBS_ChungTuChiTiet 
WHERE   iTrangThai=1 
        AND iNamLamViec=@iNamLamViec
        AND (MaLoai='' or MaLoai='2')
		AND iID_MaPhongBanDich=@iID_MaPhongBan
        AND iID_MaDonVi=@iID_MaDonVi
        AND iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu)) 

GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sXauNoiMa,iID_MaDonVi
HAVING  SUM(rTuChi)<>0 
        OR SUM(rHangNhap)<>0 
        OR SUM(rHangMua)<>0 
        OR SUM(rHienVat)<>0 
        OR SUM(rTonKho)<>0 
        OR SUM(rPhanCap)<>0 
        OR SUM(rDuPhong)<>0 


--UNION
                        
--SELECT  LEFT(sLNS,1) as sLNS1,
--        LEFT(sLNS,3) as sLNS3,
--        LEFT(sLNS,5) as sLNS5,
--        sLNS,sL,sK,sM,sTM,sTTM,sNG,sXauNoiMa,
--        iID_MaDonVi,
--        rTuChi      =SUM(rTuChi), 
--        rHienVat    =SUM(rHienVat), 
--		rHangNhap	=SUM(rHangNhap),
--		rHangMua	=SUM(rHangMua),
--        rTonKho    =SUM(rTonKho),
--        rPhanCap    =SUM(rPhanCap),
--        rDuPhong    =SUM(rDuPhong)
--FROM    DTBS_ChungTuChiTiet_PhanCap 
--WHERE   iTrangThai=1 
--        AND iNamLamViec=@iNamLamViec
--        AND (MaLoai='' or MaLoai='2')
--		AND iID_MaPhongBanDich=@iID_MaPhongBan
--		AND iID_MaDonVi=@iID_MaDonVi
--        AND (iID_MaChungTu IN (SELECT iID_MaChungTuChiTiet FROM DTBS_ChungTuChiTiet WHERE iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu)))
				
--				-- phan cap lan 2: cho cac bql
--				--OR iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
--				--					 where iID_MaChungTu in (select iID_MaDotNganSach from DTBS_ChungTuChiTiet where iID_MaChungTu in (SELECT * FROM F_Split(@iID_MaChungTu))))

--				-- phan cap lan 2: cho b
--				--OR iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
--				--					 where iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet where iID_MaChungTu in (SELECT * FROM F_Split(@iID_MaChungTu))))
--			)

--GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sXauNoiMa,iID_MaDonVi
--HAVING  SUM(rTuChi)<>0 
--        OR SUM(rHangNhap)<>0 
--        OR SUM(rHangMua)<>0 
--        OR SUM(rTonKho)<>0 
--		OR SUM(rHienVat)<>0
--        OR SUM(rPhanCap)<>0 
--        OR SUM(rDuPhong)<>0 


) as T1
WHERE (@sLNS is null or LEFT(sLNS,1) in (select * from f_split(@sLNS)))
GROUP BY sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM,sTTM,sNG,sXauNoiMa,iID_MaDonVi
