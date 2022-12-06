
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iID_MaChungTu nvarchar(200)				set @iID_MaChungTu='2ECFEA63-B695-43A6-A456-E48CAB89ABAE'
declare @iID_MaNganh nvarchar(100)					set @iID_MaNganh='51'

--###--


declare @iID_MaChungTuGom nvarchar(200)
select @iID_MaChungTuGom=iID_MaChungTu from DTBS_ChungTu_TLTH where iID_MaChungTu_TLTH=@iID_MaChungTu


SELECT  --sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM,sTTM,sNG,
		sXauNoiMa,
		sMota = [dbo].F_MLNS_MoTa_LNS(@iNamLamviec,sLNS,sL,sK,sM,sTM,sTTM,sNG),
        iID_MaDonVi,
		rTuChi =SUM(rTuChi)/@dvt,
		sGhiChu
FROM
(

SELECT  LEFT(sLNS,1) as sLNS1,
        LEFT(sLNS,3) as sLNS3,
        LEFT(sLNS,5) as sLNS5,
        sLNS,sL,sK,sM,sTM,sTTM,sNG,sXauNoiMa,
        iID_MaDonVi,
		rTuChi = SUM(rTuchi + rHangNhap + rHangMua),
        sGhiChu
FROM    DTBS_ChungTuChiTiet 
WHERE   iTrangThai=1 
        AND iNamLamViec=@iNamLamViec
        AND iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTuGom))
        AND sNG IN (SELECT * FROM F_Split((SELECT TOP(1) iID_MaNganhMLNS FROM NS_MucLucNganSach_Nganh WHERE iNamLamViec=@iNamLamViec AND iID_MaNganh=@iID_MaNganh)))
		AND (sGhiChu is not null OR sGhiChu <> ' ')
		
GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sXauNoiMa,iID_MaDonVi,sGhiChu,rTuChi

--UNION
                        
--SELECT  LEFT(sLNS,1) as sLNS1,
--        LEFT(sLNS,3) as sLNS3,
--        LEFT(sLNS,5) as sLNS5,
--        sLNS,sL,sK,sM,sTM,sTTM,sNG,sXauNoiMa,
--        iID_MaDonVi, 
--		rTuChi = SUM(rTuchi + rHangNhap + rHangMua),
--		sGhiChu
--FROM    DTBS_ChungTuChiTiet_PhanCap 
--WHERE   iTrangThai=1 
--		AND (sGhiChu is not null OR sGhiChu <> ' ')
--        AND iNamLamViec=@iNamLamViec
--        AND iID_MaDonVi=@iID_MaDonVi
--        AND (iID_MaChungTu IN (SELECT iID_MaChungTuChiTiet FROM DTBS_ChungTuChiTiet WHERE iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu)))
--			OR iID_MaChungTu in (
--							select iID_MaChungTuChiTiet 
--							from DTBS_ChungTuChiTiet 
--							where iID_MaChungTu in (select iID_MaDotNganSach from DTBS_ChungTuChiTiet where iID_MaChungTu in (SELECT * FROM F_Split(@iID_MaChungTu)))
--							))
--GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sXauNoiMa,iID_MaDonVi,sGhiChu,rTuChi

) as T1
GROUP BY sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM,sTTM,sNG,sXauNoiMa,iID_MaDonVi,sGhiChu,rTuChi
