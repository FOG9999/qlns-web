declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='10' 
declare @iID_MaDonVi nvarchar(MAX)					set @iID_MaDonVi='02,03,40,51,52,53,55,56,57,61,65,75,99'

--#DECLARE#--

select  sM,sTM,sTTM,sNG,
		sMoTa,
        iID_MaDonVi, 
        sTenDonVi = [dbo].F_GetTenDonVi(@iNamLamViec,iID_MaDonVi),        
        sum(CT_DauNam) as CT_DauNam,
        sum(CT_BoSung) as CT_BoSung,
        sum(QT_rTuChi) as QT_rTuChi 
from
(

SELECT  sLNS1	=LEFT(sLNS,1),
		sLNS3	=CASE    
                WHEN sLNS=1050100 then 101
                WHEN sLNS LIKE '102%' then 102
                else  LEFT(sLNS,3) END, 
        sLNS5	=CASE    
                WHEN sLNS=1050100 then 10100
                WHEN sLNS LIKE '102%' then 10200
                else  LEFT(sLNS,5) END, 
        sLNS	=CASE    
                WHEN sLNS=1050100 then 1010000
                WHEN sLNS LIKE '102%' then 1020001
                else sLNS END, 
		sL,sK,sM,sTM,sTTM,sNG,
        DT1.iID_MaDonVi, 
        sum(HT_Cap) as HT_Cap,
        sum(HT_QuyetToan) as HT_QuyetToan,
        sum(CT_NamTruoc) as CT_NamTruoc,
        sum(CT_DauNam) as CT_DauNam,
        sum(CT_BoSung) as CT_BoSung,
        sum(CP_Cap) as CP_Cap,
        sum(CP_Thu) as CP_Thu,
        sum(QT_rTuChi) as QT_rTuChi
from
(

/*

HOP THUC CAP NAM TRUOC

*/

--SELECT  sLNS,
--		sL,sK,sM,sTM,sTTM,sNG,
--        HT_CAP.iID_MaDonVi, 
--        sum(rTuChi) as HT_Cap,
--        0 as HT_QuyetToan,
--        0 as CT_NamTruoc,
--        0 as CT_DauNam,
--        0 as CT_BoSung,
--        0 as CP_Cap,
--        0 as CP_Thu,
--        0 as QT_rTuChi
--FROM (
--    -- DU TOAN
--    SELECT  
--			sLNS,
--			sL,sK,sM,sTM,sTTM,sNG,
--            iID_MaDonVi,
--            SUM(rTuChi+rHangMua+rHangNhap) as rTuChi

--    FROM    DT_ChungTuChiTiet 
--    WHERE   iTrangThai=1 
--            AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
--            AND iNamLamViec=@iNamLamViec 
--			AND iID_MaNamNganSach = 1
--            AND  (MaLoai='' OR MaLoai='2') 
--    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi
--    HAVING SUM(rTuChi+rHangMua+rHangNhap)<>0 
            
--    UNION ALL
--    -- DU TOAN - PHAN CAP
--    SELECT   
--            sLNS,
--			sL,sK,sM,sTM,sTTM,sNG,
--            iID_MaDonVi,
--            SUM(rTuChi+rHangMua+rHangNhap) as rTuChi
--    FROM    DT_ChungTuChiTiet_PhanCap 
--    WHERE   iTrangThai=1 
--            AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
--            AND iNamLamViec=@iNamLamViec 
--			AND iID_MaNamNganSach = 1
--    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi
--    HAVING SUM(rTuChi+rHangMua+rHangNhap)<>0

--	union
--	 -- DU TOAN
--    SELECT  
--			sLNS,
--			sL,sK,sM,sTM,sTTM,sNG,
--            iID_MaDonVi,
--            SUM(rTuChi+rHangMua+rHangNhap) as rTuChi

--    FROM    DTBS_ChungTuChiTiet 
--    WHERE   iTrangThai=1 
--            AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
--            AND iNamLamViec=@iNamLamViec 
--			AND iID_MaNamNganSach = 1
--            AND  (MaLoai='' OR MaLoai='2') 
--    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi
--    HAVING SUM(rTuChi+rHangMua+rHangNhap)<>0 
            
--    UNION ALL
--    -- DU TOAN - PHAN CAP
--    SELECT   
--            sLNS,
--			sL,sK,sM,sTM,sTTM,sNG,
--            iID_MaDonVi,
--            SUM(rTuChi+rHangMua+rHangNhap) as rTuChi
--    FROM    DTBS_ChungTuChiTiet_PhanCap 
--    WHERE   iTrangThai=1 
--            AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
--            AND iNamLamViec=@iNamLamViec 
--			AND iID_MaNamNganSach = 1
--    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi
--    HAVING SUM(rTuChi+rHangMua+rHangNhap)<>0

--) as HT_CAP
--GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG, HT_CAP.iID_MaDonVi

--UNION ALL



--/*

--CHI TIEU NAM TRUOC

--*/

--SELECT  sLNS,
--		sL,sK,sM,sTM,sTTM,sNG,
--        CT_NAMTRUOC.iID_MaDonVi, 
--        0 as HT_Cap,
--        0 as HT_QuyetToan,
--        sum(rTuChi) as CT_NamTruoc,
--        0 as CT_DauNam,
--        0 as CT_BoSung,
--        0 as CP_Cap,
--        0 as CP_Thu,
--        0 as QT_rTuChi
--FROM (	

--    SELECT
--        sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi,
--		rTuChi=	CASE    
--				WHEN sLNS LIKE '104%' then SUM(rTuChi)
--				ELSE SUM(rTuChi+rHangNhap+rHangMua) END

--    FROM    f_ns_chitieu(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, 4, GETDATE(),1,null)
--    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi

--	union
--	SELECT
--        sLNS='1040200',sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi,
--		rTuChi=	sum(rHangNhap)

--    FROM    f_ns_chitieu(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, 4, GETDATE(),1,null)
--	where	sLNS like '104%'
--    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi

--	union
--	SELECT
--        sLNS='1040300',sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi,
--		rTuChi=	sum(rHangMua)

--    FROM    f_ns_chitieu(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, 4, GETDATE(),1,null)
--	where	sLNS like '104%'
--    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi


--) as CT_NAMTRUOC
--GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG, CT_NAMTRUOC.iID_MaDonVi

--UNION ALL


/*

DU TOAN NAM NAY

*/
-- du toan
SELECT  sLNS,sL,sK,sM,sTM,sTTM,sNG,
        CT.iID_MaDonVi, 
        0 as HT_Cap,
        0 as HT_QuyetToan,
        0 as CT_NamTruoc,
        sum(rTuChi) as CT_DauNam,
        sum(rBS) as CT_BoSung,
        0 as CP_Cap,
        0 as CP_Thu,
        0 as QT_rTuChi
FROM (

    -- DU TOAN

	-- rTuChi
    SELECT      
			sLNS,sL,sK,sM,sTM,sTTM,sNG,
            iID_MaDonVi,
            rTuChi=	CASE    
					WHEN sLNS LIKE '104%' then SUM(rTuChi)
					ELSE SUM(rTuChi+rHangNhap+rHangMua) END,
            0 as rBS
    FROM    DT_ChungTuChiTiet 
    WHERE   iTrangThai=1 
            AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
            AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach = 2
            AND  (MaLoai='' OR MaLoai='2') 
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi
    HAVING	SUM(rTuChi)<>0

	-- rHangNhap
	UNION 
	SELECT      
			sLNS='1040200',sL,sK,sM,sTM,sTTM,sNG,
            iID_MaDonVi,
            rTuChi=SUM(rHangNhap),
            rBS=0 
    FROM    DT_ChungTuChiTiet 
    WHERE   iTrangThai=1 
            AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
            AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach = 2
            AND  (MaLoai='' OR MaLoai='2') 
			AND sLNS like '104%'
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi
    HAVING	 SUM(rHangNhap)<>0

	-- rHangNhap
	UNION 
	SELECT      
			sLNS='1040300',sL,sK,sM,sTM,sTTM,sNG,
            iID_MaDonVi,
            rTuChi=SUM(rHangMua),
            rBS=0 
    FROM    DT_ChungTuChiTiet 
    WHERE   iTrangThai=1 
            AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
            AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach = 2
            AND  (MaLoai='' OR MaLoai='2') 
			AND sLNS like '104%'
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi
    HAVING	 SUM(rHangMua)<>0

            
    UNION ALL
    -- DU TOAN - PHAN CAP
    SELECT  
			sLNS,sL,sK,sM,sTM,sTTM,sNG,
            iID_MaDonVi,
            rTuChi=SUM(rTuChi),
            rBS=0 
    FROM    DT_ChungTuChiTiet_PhanCap 
    WHERE   iTrangThai=1 
            AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
            AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach = 2
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi
    HAVING	SUM(rTuChi)<>0  

    --//////////////////////////////////////////////////////////////////
    -- DU TOAN BO SUNG
    UNION ALL
    SELECT  sLNS,sL,sK,sM,sTM,sTTM,sNG,
            iID_MaDonVi,
            rTuChi=0,
            rBS=CASE    
					WHEN sLNS LIKE '104%' then SUM(rTuChi)
					ELSE  SUM(rTuChi+rHangNhap+rHangMua) END

        FROM DTBS_ChungTuChiTiet 
        WHERE  iTrangThai=1 
            AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
            AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach = 2
			AND iID_MaChungTu IN (SELECT * FROM f_split([dbo].f_ns_dtbs_chungtu_gom(@iNamLamViec,@iID_MaPhongBan)))
			
        GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi
        HAVING	SUM(rTuChi)<>0 

	-- DTBS - 1040200
	UNION ALL
    SELECT  sLNS='1040200',sL,sK,sM,sTM,sTTM,sNG,
            iID_MaDonVi,
            rTuChi=0,
            rBS=SUM(rHangNhap)
        FROM DTBS_ChungTuChiTiet 
        WHERE  iTrangThai=1 
            AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
            AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach = 2
            AND (MaLoai='' OR MaLoai='2')
			AND sLNS like '104%'
			AND iID_MaChungTu IN (SELECT * FROM f_split([dbo].f_ns_dtbs_chungtu_gom(@iNamLamViec,@iID_MaPhongBan)))
        GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi
        HAVING	SUM(rHangNhap)<>0 

	-- DTBS - 1040300
	UNION ALL
    SELECT  sLNS='1040300',sL,sK,sM,sTM,sTTM,sNG,
            iID_MaDonVi,
            rTuChi=0,
            rBS=SUM(rHangMua)
        FROM DTBS_ChungTuChiTiet 
        WHERE  iTrangThai=1 
            AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
            AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach = 2
            AND (MaLoai='' OR MaLoai='2')
			AND sLNS like '104%'
			AND iID_MaChungTu IN (SELECT * FROM f_split([dbo].f_ns_dtbs_chungtu_gom(@iNamLamViec,@iID_MaPhongBan)))
        GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi
        HAVING	SUM(rHangMua)<>0 

	-- PHAN CAP BO SUNG
    UNION ALL

	SELECT	             
			sLNS,sL,sK,sM,sTM,sTTM,sNG,
			iID_MaDonVi,
			rTuChi=0,
            rBS=SUM(rTuChi+rHangNhap+rHangMua)
	FROM f_ns_dtbs_phancap(@iNamLamViec,@iID_MaPhongBan, @iID_MaDonVi,[dbo].f_ns_dtbs_chungtu_gom(@iNamLamViec,@iID_MaPhongBan)) 
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi
    HAVING SUM(rTuChi)<>0 OR SUM(rHangNhap)<>0 OR SUM(rHangMua)<>0

) as ct


GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG, CT.iID_MaDonVi
--ORDER BY sLNS,iID_MaDonVi

UNION ALL

-- QUYET TOAN                   
SELECT  sLNS,
		sL,sK,sM,sTM,sTTM,sNG,
		iID_MaDonVi,
        0 as HT_Cap,
        0 as HT_QuyetToan,
        0 as CT_NamTruoc,
        0 as CT_DauNam,
        0 as CT_BoSung,
        0 as CP_Cap,
        0 as CP_Thu,
        sum(rTuChi) as QT_rTuChi

FROM    QTA_ChungTuChiTiet
WHERE   iTrangThai=1 
        AND (@iID_MaPhongBan is NULL OR iID_MaPhongBan=@iID_MaPhongBan)  
        AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach = 2
GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi
HAVING SUM(rTuChi)<>0 


--UNION ALL
---- CAP PHAT              
--SELECT  
--		sLNS,
--		sL,sK,sM,sTM,sTTM,sNG,
--        iID_MaDonVi,
--        0 as HT_Cap,
--        0 as HT_QuyetToan,
--        0 as CT_NamTruoc,
--        0 as CT_DauNam,
--        0 as CT_BoSung,
--        sum(rTuChi) as CP_Cap,
--        0 as CP_Thu,
--        0 as QT_rTuChi
--FROM    CP_CapPhatChiTiet
--WHERE   iTrangThai=1 
--        AND (@iID_MaPhongBan is NULL OR iID_MaPhongBan=@iID_MaPhongBan)  
--        AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach = 2
--GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi
--HAVING SUM(rTuChi)<>0 

) as dt1
GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi


) as DT

left join (
	select distinct sLNS as id,sL AS id1,sK AS id2,sM AS id3 ,sTM AS id4,sTTM AS id5,sNG AS id6,sMoTa 
	from NS_MucLucNganSach where iTrangThai=1 and iNamLamViec=@iNamLamViec and sL='' and sLNS=sXauNoiMa) as mlns on (dt.sLNS=mlns.id AND dt.sL=mlns.id1 AND dt.sK=mlns.id2 AND dt.sM=mlns.id3 AND dt.sTM=mlns.id4 AND dt.sTTM=mlns.id5 AND dt.sNG=mlns.id6)

where  
        sLNS1 in (1,2,3,4)
        AND sLNS not in(1050000)
        AND (@iID_MaDonVi IS NULL OR iID_MaDonVi IN (SELECT * FROM F_Split(@iID_MaDonVi)))
		AND sNG <> ''		
group by sM,sTM,sTTM,sNG, mlns.sMoTa, iID_MaDonVi
Having sum(CT_DauNam) <> 0 OR
        sum(CT_BoSung) <> 0 OR
        sum(QT_rTuChi) <> 0
ORDER BY iID_MaDonVi,sM,sTM,sTTM,sNG, mlns.sMoTa