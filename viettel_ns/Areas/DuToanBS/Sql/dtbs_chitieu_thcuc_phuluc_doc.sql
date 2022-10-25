
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2020
declare @iID_MaChungTu nvarchar(MAX)				set @iID_MaChungTu='cf05d0d2-b410-4ef3-af33-b1b90bbb4f65,3da4d334-b8e6-4975-bb3a-cff0e77122e0,35047e92-7d57-4c61-9068-d9b2f18e5272,c7b229ec-ab8a-4023-b3cd-ee935ae855ff,3f267b1d-eb70-4949-946b-4d6f8d87d83f,b17ff3af-d6e8-4018-a314-606917cac2a2'
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='33'

--#DECLARE#--
declare @dn nvarchar(MAX)
set @dn = '30,34,341,35,50,501,69,70,71,72,73,78,80,88,89,90,91,92,93,94'

SELECT  sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM,sTTM,sNG,sXauNoiMa,sMota = [dbo].F_MLNS_MoTa_LNS(@iNamLamviec,sLNS,sL,sK,sM,sTM,sTTM,sNG),
        iID_MaDonVi,sTenDonVi=[dbo].F_GetTenDonVi(@iNamLamviec, @iID_MaDonVi),
        rTuChi      =SUM(rTuChi)/@dvt
FROM
(

SELECT  LEFT(sLNS,1) as sLNS1,
        LEFT(sLNS,3) as sLNS3,
        sLNS5 = case when sLNS = '1020000' then '10201' else LEFT(sLNS,5) end,
        sLNS = case when sLNS = '1020000' then '1020100' else sLNS end,sL,sK,sM,sTM,sTTM,sNG,
		sXauNoiMa = case when sLNS = '1020000' then REPLACE(sXauNoiMa,'1020000','1020100') else sXauNoiMa end,
        iID_MaDonVi = LEFT(iID_MaDonVi,2),
        rTuChi      =SUM(rTuChi)
FROM    DTBS_ChungTuChiTiet 
WHERE   iTrangThai=1 
        AND iNamLamViec=@iNamLamViec
        AND (MaLoai='' or MaLoai='2')
        AND iID_MaDonVi=@iID_MaDonVi
		AND ((@loai = 1 and iID_MaPhongBanDich <> '06' and iID_MaDonVi not in (select * from f_split(@dn))) or (@loai = 4 and (iID_MaPhongBanDich = '06' or iID_MaDonVi in (select * from f_split(@dn)))))
		AND (@phongban is null or iID_MaPhongBan = @phongban)
        AND iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu)) 

GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sXauNoiMa,iID_MaDonVi
HAVING  SUM(rTuChi)<>0 

UNION ALL
                        
SELECT  LEFT(sLNS,1) as sLNS1,
        LEFT(sLNS,3) as sLNS3,
        sLNS5 = case when sLNS = '1020000' then '10201' else LEFT(sLNS,5) end,
        sLNS = case when sLNS = '1020000' then '1020100' else sLNS end,sL,sK,sM,sTM,sTTM,sNG,
		sXauNoiMa = case when sLNS = '1020000' then REPLACE(sXauNoiMa,'1020000','1020100') else sXauNoiMa end,
        iID_MaDonVi,
        rTuChi      =SUM(rTuChi)
FROM    DTBS_ChungTuChiTiet_PhanCap 
WHERE   iTrangThai=1 
        AND iNamLamViec=@iNamLamViec
        AND (MaLoai='' or MaLoai='2')
		AND iID_MaDonVi=@iID_MaDonVi
		AND (@phongban is null)
		AND ((@loai = 1 and iID_MaPhongBanDich <> '06' and iID_MaDonVi not in (select * from f_split(@dn))) or (@loai = 4 and (iID_MaPhongBanDich = '06' or iID_MaDonVi in (select * from f_split(@dn)))))
        AND (
			iID_MaChungTu IN (SELECT iID_MaChungTuChiTiet FROM DTBS_ChungTuChiTiet WHERE iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu)))
				
			-- phan cap cho cac bql
			OR iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
									where iID_MaChungTu in (select iID_MaDotNganSach from DTBS_ChungTuChiTiet where iID_MaChungTu in (SELECT * FROM F_Split(@iID_MaChungTu))))

			---- phan cap cho b
			OR 	iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
								 where iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet where iID_MaChungTu in (SELECT * FROM F_Split(@iID_MaChungTu))))


			-- phan cap lan 2
			OR iID_MaChungTu in (	select iID_MaChungTuChiTiet 
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
HAVING  SUM(rTuChi)<>0 

UNION ALL
                        
SELECT  LEFT(sLNS,1) as sLNS1,
        LEFT(sLNS,3) as sLNS3,
        sLNS5 = case when sLNS = '1020000' then '10201' else LEFT(sLNS,5) end,
        sLNS = case when sLNS = '1020000' then '1020100' else sLNS end,sL,sK,sM,sTM,sTTM,sNG,
		sXauNoiMa = case when sLNS = '1020000' then REPLACE(sXauNoiMa,'1020000','1020100') else sXauNoiMa end,
        iID_MaDonVi,
        rTuChi      =SUM(rTuChi)
FROM    DTBS_ChungTuChiTiet
WHERE   iTrangThai=1 
        AND iNamLamViec=@iNamLamViec
        AND MaLoai='2'
		AND iID_MaDonVi=@iID_MaDonVi
		AND (@phongban is null)
		AND ((@loai = 1 and iID_MaPhongBanDich <> '06' and iID_MaDonVi not in (select * from f_split(@dn))) or (@loai = 4 and (iID_MaPhongBanDich = '06' or iID_MaDonVi in (select * from f_split(@dn)))))
        AND (

			-- phan cap lan 2
			iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
								 where iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet where iID_MaChungTu in (SELECT * FROM F_Split(@iID_MaChungTu))))
			)

GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sXauNoiMa,iID_MaDonVi
HAVING  SUM(rTuChi)<>0 

) as T1
WHERE sLNS not like '104%'
GROUP BY sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM,sTTM,sNG,sXauNoiMa,iID_MaDonVi