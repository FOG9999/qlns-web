
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @username nvarchar(20)						set @username='chinpth'
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='10' 
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi=NULL
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach='1,2'
declare @dMaDot datetime							set @dMaDot='2018-03-10 00:00:00.0'

--#DECLARE#--

select  sLNS1,
		sLNS3, 
        sLNS5,
        sLNS,

        'smota' as sMoTa,
        --sMoTa,
        DT.iID_MaDonVi, 
        DV.sTen as sTenDonVi,
        sum(HT_Cap) as HT_Cap,
        sum(HT_QuyetToan) as HT_QuyetToan,
        sum(CT_NamTruoc) as CT_NamTruoc,
        sum(CT_DauNam) as CT_DauNam,
        sum(CT_BoSung) as CT_BoSung,
        sum(CP_Cap) as CP_Cap,
        sum(CP_Thu) as CP_Thu,
        sum(QT_rTuChi) as QT_rTuChi from
(

SELECT  sLNS1	=LEFT(sLNS,1),
		sLNS3	=CASE    
                WHEN sLNS=1050100 then '101'
                WHEN sLNS LIKE '102%' then '102'
                else  LEFT(sLNS,3) END, 
        sLNS5	=CASE    
                WHEN sLNS=1050100 then '10100'
                WHEN sLNS LIKE '102%' then 10200
                else  LEFT(sLNS,5) END, 
        sLNS	=CASE    
                WHEN sLNS=1050100 then '1010000'
                WHEN sLNS LIKE '102%' then '1020001'
                else sLNS END, 
        --sLNS3, 
        --sLNS5,
        --sLNS,

        --'smota' as sMoTa,
        DT1.iID_MaDonVi, 
        --DV.sTen as sTenDonVi,
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

---- HOP THUC NAM TRUOC
--    -- CAP HOP THUC       
--SELECT  LEFT(sLNS,1) as sLNS1,
--        LEFT(sLNS,3) as sLNS3,
--        LEFT(sLNS,5) as sLNS5,
--        sLNS,
--        iID_MaDonVi,
--        sum(rTuChi) as HT_Cap,
--        0 as HT_QuyetToan,
--        0 as CT_NamTruoc,
--        0 as CT_DauNam,
--        0 as CT_BoSung,
--        0 as CP_Cap,
--        0 as CP_Thu,
--        0 as QT_rTuChi
--FROM    CP_CapPhatChiTiet
--WHERE   iTrangThai=1 
--        AND (@iID_MaPhongBan is NULL OR iID_MaPhongBan=@iID_MaPhongBan)  
--        AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach = 1
--GROUP BY sLNS,iID_MaDonVi
--HAVING SUM(rTuChi)<>0 

--UNION ALL

---- QUYET TOAN HOP THUC
--SELECT  LEFT(sLNS,1) as sLNS1,
--        LEFT(sLNS,3) as sLNS3,
--        LEFT(sLNS,5) as sLNS5,
--        sLNS,
--        iID_MaDonVi,
--        0 as HT_Cap,
--        sum(rTuChi) as HT_QuyetToan,
--        0 as CT_NamTruoc,
--        0 as CT_DauNam,
--        0 as CT_BoSung,
--        0 as CP_Cap,
--        0 as CP_Thu,
--        0 as QT_rTuChi

--FROM    QTA_ChungTuChiTiet
--WHERE   iTrangThai=1 
--        AND (@iID_MaPhongBan is NULL OR iID_MaPhongBan=@iID_MaPhongBan)  
--        AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach = 1
--        --AND iThang_Quy IN (1,2)
--GROUP BY sLNS,iID_MaDonVi
--HAVING SUM(rTuChi)<>0 

--UNION ALL

---- CHI TIEU NAM TRUOC
---- du toan
--SELECT  sLNS1,sLNS3,sLNS5,sLNS,
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

    

--    -- DU TOAN
--    SELECT  sLNS1	=LEFT(sLNS,1),
--			sLNS3	=LEFT(sLNS,3),
--			sLNS5	=LEFT(sLNS,5),
--			sLNS,

--            iID_MaDonVi,
--            SUM(rTuChi+rHangMua+rHangNhap) as rTuChi

--    FROM    DT_ChungTuChiTiet 
--    WHERE   iTrangThai=1 
--            AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
--            AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach = 4
--            AND  (MaLoai='' OR MaLoai='2') 
--    GROUP BY sLNS,iID_MaDonVi
--    HAVING SUM(rTuChi+rHangMua+rHangNhap)<>0 
--    --order by sLNS, iID_MaDonVi
            
--    UNION ALL
--    -- DU TOAN - PHAN CAP
--    SELECT  LEFT(sLNS,1) as sLNS1,
--            LEFT(sLNS,3) as sLNS3,
--            LEFT(sLNS,5) as sLNS5,
--            sLNS,

--            iID_MaDonVi,
--            SUM(rTuChi+rHangMua+rHangNhap) as rTuChi
--    FROM    DT_ChungTuChiTiet_PhanCap 
--    WHERE   iTrangThai=1 
--            AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
--            AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach = 4
--            --AND (sLNS IN (101000,1020100,'1020001',1040100,1090200,1090400,1090500,1091300) OR sLNS LIKE '207%') 
--    GROUP BY sLNS,iID_MaDonVi
--    HAVING SUM(rTuChi+rHangMua+rHangNhap)<>0
   

--) as CT_NAMTRUOC
--GROUP BY sLNS1,sLNS3, sLNS5,sLNS, CT_NAMTRUOC.iID_MaDonVi

--UNION ALL

-- du toan
SELECT  sLNS,
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
			sLNS,
            iID_MaDonVi,
            rTuChi=	CASE    
					WHEN sLNS LIKE '104%' then SUM(rTuChi)
					ELSE  SUM(rTuChi+rHangNhap+rHangMua) END,
            0 as rBS
    FROM    DT_ChungTuChiTiet 
    WHERE   iTrangThai=1 
            AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
            AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach = 2
            AND  (MaLoai='' OR MaLoai='2') 
    GROUP BY sLNS,iID_MaDonVi
    HAVING	SUM(rTuChi)<>0

	-- rHangNhap
	UNION 
	SELECT      
			sLNS='1040200',
            iID_MaDonVi,
            rTuChi=SUM(rHangNhap),
            rBS=0 
    FROM    DT_ChungTuChiTiet 
    WHERE   iTrangThai=1 
            AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
            AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach = 2
            AND  (MaLoai='' OR MaLoai='2') 
			AND sLNS like '104%'
    GROUP BY sLNS,iID_MaDonVi
    HAVING	 SUM(rHangNhap)<>0

	-- rHangNhap
	UNION 
	SELECT      
			sLNS='1040300',
            iID_MaDonVi,
            rTuChi=SUM(rHangMua),
            rBS=0 
    FROM    DT_ChungTuChiTiet 
    WHERE   iTrangThai=1 
            AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
            AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach = 2
            AND  (MaLoai='' OR MaLoai='2') 
			AND sLNS like '104%'
    GROUP BY sLNS,iID_MaDonVi
    HAVING	 SUM(rHangMua)<>0

            
    UNION ALL
    -- DU TOAN - PHAN CAP
    SELECT  
			sLNS,
            iID_MaDonVi,
            rTuChi=SUM(rTuChi),
            rBS=0 
    FROM    DT_ChungTuChiTiet_PhanCap 
    WHERE   iTrangThai=1 
            AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
            AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach = 2
    GROUP BY sLNS,iID_MaDonVi
    HAVING	SUM(rTuChi)<>0  

    --////////////////////
    -- DU TOAN BO SUNG
    UNION ALL
    SELECT  sLNS,
            iID_MaDonVi,
            rTuChi=0,
            rBS=SUM(rTuChi)
        FROM DTBS_ChungTuChiTiet 
        WHERE  iTrangThai=1 
            AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
            AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach = 2
            AND (MaLoai='' OR MaLoai='2')
			AND iID_MaChungTu IN (SELECT * FROM F_NS_DTBS_ChungTuDaGom(@iNamLamViec,@iID_MaPhongBan))
        GROUP BY sLNS,iID_MaDonVi
        HAVING	SUM(rTuChi)<>0 

	---- DTBS - 1040200
	--UNION ALL
 --   SELECT  sLNS='1040200',
 --           iID_MaDonVi,
 --           rTuChi=0,
 --           rBS=SUM(rHangNhap)
 --       FROM DTBS_ChungTuChiTiet 
 --       WHERE  iTrangThai=1 
 --           AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
 --           AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach = 2
 --           AND (MaLoai='' OR MaLoai='2')
	--		AND iID_MaChungTu IN (SELECT * FROM F_NS_DTBS_ChungTuDaGom(@iNamLamViec,@iID_MaPhongBan))
 --       GROUP BY sLNS,iID_MaDonVi
 --       HAVING	SUM(rHangNhap)<>0 

	---- DTBS - 1040300
	--UNION ALL
 --   SELECT  sLNS='1040300',
 --           iID_MaDonVi,
 --           rTuChi=0,
 --           rBS=SUM(rHangMua)
 --       FROM DTBS_ChungTuChiTiet 
 --       WHERE  iTrangThai=1 
 --           AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
 --           AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach = 2
 --           AND (MaLoai='' OR MaLoai='2')
	--		AND iID_MaChungTu IN (SELECT * FROM F_NS_DTBS_ChungTuDaGom(@iNamLamViec,@iID_MaPhongBan))
 --       GROUP BY sLNS,iID_MaDonVi
 --       HAVING	SUM(rHangMua)<>0 



	-- PHAN CAP BO SUNG
    UNION ALL

    SELECT  pc.sLNS as sLNS,
            pc.iID_MaDonVi as iID_MaDonVi,
			rTuChi=0,
            rBS=SUM(pc.rTuChi+pc.rHangNhap+pc.rHangMua)
    FROM    DTBS_ChungTuChiTiet_PhanCap as pc INNER JOIN DTBS_ChungTuChiTiet as ctct ON ctct.iID_MaChungTuChiTiet = pc.iID_MaChungTu
    WHERE   pc.iTrangThai=1 
            AND (@iID_MaPhongBan is NULL OR pc.iID_MaPhongBanDich=@iID_MaPhongBan) 
			AND ctct.iID_MaChungTu IN (SELECT * FROM F_NS_DTBS_ChungTuDaGom(@iNamLamViec,@iID_MaPhongBan))
            AND pc.iNamLamViec=@iNamLamViec AND pc.iID_MaNamNganSach = 2
    GROUP BY pc.sLNS,pc.iID_MaDonVi
    HAVING	SUM(pc.rTuChi)<>0 OR
			SUM(pc.rHangNhap)<>0 OR
			SUM(pc.rHangMua)<>0

) as ct


GROUP BY sLNS, CT.iID_MaDonVi
--ORDER BY sLNS,iID_MaDonVi



UNION ALL

-- QUYET TOAN                   
SELECT  sLNS,

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
        --AND iThang_Quy IN (1,2)
GROUP BY sLNS,iID_MaDonVi
HAVING SUM(rTuChi)<>0 
--order by iID_MaDonVi
-- END 101


UNION ALL
-- CAP PHAT              
SELECT  
		sLNS,

        iID_MaDonVi,
        0 as HT_Cap,
        0 as HT_QuyetToan,
        0 as CT_NamTruoc,
        0 as CT_DauNam,
        0 as CT_BoSung,
        sum(rTuChi) as CP_Cap,
        0 as CP_Thu,
        0 as QT_rTuChi
FROM    CP_CapPhatChiTiet
WHERE   iTrangThai=1 
        AND (@iID_MaPhongBan is NULL OR iID_MaPhongBan=@iID_MaPhongBan)  
        AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach = 2
GROUP BY sLNS,iID_MaDonVi
HAVING SUM(rTuChi)<>0 

) as dt1
GROUP BY sLNS,iID_MaDonVi


) as DT

-- LAY TEN DON VI
JOIN NS_DonVi AS DV
ON DV.iID_MaDonVi = DT.iID_MaDonVi AND DV.iNamLamViec_DonVi = @iNamLamViec

where  
        sLNS1 in (2,4)
        AND sLNS not in(1050000)
        AND (@iID_MaDonVi IS NULL OR DT.iID_MaDonVi IN (SELECT * FROM F_Split(@iID_MaDonVi)))
group by sLNS1, sLNS3, sLNS5,sLNS, DT.iID_MaDonVi, DV.sTen, sMoTa
ORDER BY sLNS, DT.iID_MaDonVi
