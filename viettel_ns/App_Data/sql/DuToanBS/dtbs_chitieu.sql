
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iID_MaChungTu nvarchar(200)				set @iID_MaChungTu='fa234f87-000c-4605-9f82-e49cf2a08fae'
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='31'
declare @sLNS nvarchar(100)							set @sLNS='1'

--#DECLARE#--



SELECT  sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM,sTTM,sNG,sXauNoiMa,sMota = [dbo].F_MLNS_MoTa_LNS(@iNamLamviec,sLNS,sL,sK,sM,sTM,sTTM,sNG),
        iID_MaDonVi,sTenDonVi=[dbo].F_GetTenDonVi(@iNamLamviec, @iID_MaDonVi),
		rTongSo		=SUM(rTuChi+rHienVat+rDuPhong)/@dvt,
        rTuChi      =SUM(rTuChi)/@dvt, 
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
        rTuChi      =SUM(rTuChi+rHangNhap+rHangMua), 
        rHienVat    =SUM(rHienVat), 
        rPhanCap    =SUM(rPhanCap), 
        rDuPhong    =SUM(rDuPhong)
FROM    DTBS_ChungTuChiTiet 
WHERE   iTrangThai=1 
        AND iNamLamViec=@iNamLamViec
        AND iID_MaDonVi=@iID_MaDonVi
        AND iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu))
		
GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sXauNoiMa,iID_MaDonVi
HAVING  SUM(rTuChi+rHangNhap+rHangMua)<>0 
        OR SUM(rHienVat)<>0 
        OR SUM(rPhanCap)<>0 
        OR SUM(rDuPhong)<>0 

UNION
                        
SELECT  LEFT(sLNS,1) as sLNS1,
        LEFT(sLNS,3) as sLNS3,
        LEFT(sLNS,5) as sLNS5,
        sLNS,sL,sK,sM,sTM,sTTM,sNG,sXauNoiMa,
        iID_MaDonVi,
        rTuChi      =SUM(rTuChi+rHangNhap+rHangMua), 
        rHienVat    =SUM(rHienVat), 
        rPhanCap    =SUM(rPhanCap), 
        rDuPhong    =SUM(rDuPhong)
FROM    DTBS_ChungTuChiTiet_PhanCap 
WHERE   iTrangThai=1 
        AND iNamLamViec=@iNamLamViec
        AND iID_MaDonVi=@iID_MaDonVi
        AND (iID_MaChungTu IN (SELECT iID_MaChungTuChiTiet FROM DTBS_ChungTuChiTiet WHERE iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu)))
			OR iID_MaChungTu in (
							select iID_MaChungTuChiTiet 
							from DTBS_ChungTuChiTiet 
							where iID_MaChungTu in (select iID_MaDotNganSach from DTBS_ChungTuChiTiet where iID_MaChungTu in (SELECT * FROM F_Split(@iID_MaChungTu))))

			-- phan cap lan 2
			OR 	iID_MaChungTu in (	select iID_MaChungTuChiTiet 
										from DTBS_ChungTuChiTiet 
										where iID_MaChungTu in (
													select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
													where iID_MaChungTu in (   
																			select iID_MaChungTu from DTBS_ChungTu
																			where iID_MaChungTu in (
																									select iID_MaDuyetDuToanCuoiCung from DTBS_ChungTu
																									where iID_MaChungTu in (select * from F_Split(@iID_MaChungTu))))))
							
							
			)
GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sXauNoiMa,iID_MaDonVi
HAVING  SUM(rTuChi+rHangNhap+rHangMua)<>0 
        OR SUM(rHienVat)<>0 
        OR SUM(rPhanCap)<>0 
        OR SUM(rDuPhong)<>0 

) as T1
WHERE (@sLNS is null or LEFT(sLNS,1) in (select * from f_split(@sLNS)))
GROUP BY sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM,sTTM,sNG,sXauNoiMa,iID_MaDonVi
