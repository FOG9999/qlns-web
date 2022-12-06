
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iID_MaChungTu nvarchar(200)				set @iID_MaChungTu='7766267b-b46e-4cf1-a190-332213a70ee3'
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='12'

--#DECLARE#--



SELECT  --sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM,sTTM,sNG,
		sXauNoiMa,
		sMota = [dbo].F_MLNS_MoTa_LNS(@iNamLamviec,sLNS,sL,sK,sM,sTM,sTTM,sNG),
        iID_MaDonVi,
		rTuChi =SUM(rTuChi)/@dvt,
		--sTenDonVi=[dbo].F_GetTenDonVi(@iNamLamviec, @iID_MaDonVi),
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
        AND iID_MaDonVi=@iID_MaDonVi
        AND iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu))
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
