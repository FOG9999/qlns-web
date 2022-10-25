
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iID_MaChungTu nvarchar(200)				set @iID_MaChungTu='2ECFEA63-B695-43A6-A456-E48CAB89ABAE'
declare @iID_MaPhongBan nvarchar(2)					set @iID_MaPhongBan='10'
declare @iID_MaNganh nvarchar(100)					set @iID_MaNganh='51'

--###--


declare @iID_MaChungTuGom nvarchar(200)
select @iID_MaChungTuGom=iID_MaChungTu from DTBS_ChungTu_TLTH where iID_MaChungTu_TLTH=@iID_MaChungTu

SELECT  sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM,sTTM,sNG,sXauNoiMa,
		--sMota = [dbo].F_MLNS_MoTa_LNS(@iNamLamviec,sLNS,sL,sK,sM,sTM,sTTM,sNG),
		rTongSo		=SUM(rTuChi+rHangNhap+rHangMua+rTonKho+rHienVat+rPhanCap+rDuPhong)/@dvt,
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
        AND sNG IN (SELECT * FROM F_Split((SELECT TOP(1) iID_MaNganhMLNS FROM NS_MucLucNganSach_Nganh WHERE iNamLamViec=@iNamLamViec AND iID_MaNganh=@iID_MaNganh)))
        AND iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTuGom))
GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sXauNoiMa
HAVING  SUM(rTuChi)<>0 
        OR SUM(rHangNhap)<>0 
        OR SUM(rHangMua)<>0 
        OR SUM(rHienVat)<>0 
        OR SUM(rTonKho)<>0 
        OR SUM(rPhanCap)<>0 
        OR SUM(rDuPhong)<>0 


UNION
                        
--SELECT  LEFT(sLNS,1) as sLNS1,
--        LEFT(sLNS,3) as sLNS3,
--        LEFT(sLNS,5) as sLNS5,
--        sLNS,sL,sK,sM,sTM,sTTM,sNG,sXauNoiMa,
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
--        AND (iID_MaChungTu IN (SELECT iID_MaChungTuChiTiet FROM DTBS_ChungTuChiTiet WHERE iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu)))
				
--				-- phan cap lan 2: cho cac bql
--				OR iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
--									 where iID_MaChungTu in (select iID_MaDotNganSach from DTBS_ChungTuChiTiet where iID_MaChungTu in (SELECT * FROM F_Split(@iID_MaChungTu))))
--				-- phan cap lan 2: cho b
--				--OR iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
--				--					 where iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet where iID_MaChungTu in (SELECT * FROM F_Split(@iID_MaChungTu))))
--			)

GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sXauNoiMa
HAVING  SUM(rTuChi)<>0 
        OR SUM(rHangNhap)<>0 
        OR SUM(rHangMua)<>0 
        OR SUM(rTonKho)<>0 
		OR SUM(rHienVat)<>0
        OR SUM(rPhanCap)<>0 
        OR SUM(rDuPhong)<>0 


) as T1
GROUP BY sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM,sTTM,sNG,sXauNoiMa
