
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @username nvarchar(20)						set @username='chuctc'
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='17' 
declare @iID_MaDonVi nvarchar(max)					set @iID_MaDonVi='79,80,81,82,83,84,87,88,89,90,91,92,93,94,95,96,97,98,99,B17'
--declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='51'
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach='1,2,4'

--###--

declare @iID_MaChungTu nvarchar(MAX)
set @iID_MaChungTu = [dbo].[f_ns_dtbs_chungtu_gom_todate](@iNamLamViec,'1,2,4',@iID_MaPhongBan,GETDATE())

select  sLNS1,
		sLNS3, 
        sLNS5,
        sLNS,
		--sMoTa = [dbo].F_MLNS_MoTa(@iNamLamViec,sLNS),
		mlns.sMoTa,
        iID_MaDonVi, 
        sTenDonVi = [dbo].F_GetTenDonVi(@iNamLamViec,iID_MaDonVi),
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
                --WHEN sLNS=1050100 then 101
                WHEN sLNS LIKE '102%' then 102
                else  LEFT(sLNS,3) END, 
        sLNS5	=CASE    
                --WHEN sLNS=1050100 then 10100
                WHEN sLNS LIKE '102%' then 10200
                else  LEFT(sLNS,5) END, 
        sLNS	=CASE    
                --WHEN sLNS=1050100 then 1010000
                WHEN sLNS LIKE '102%' then 1020100
                else sLNS END, 
     
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

--1. Hợp thức cấp
SELECT  sLNS,
        HT_CAP.iID_MaDonVi, 
        sum(rTuChi) as HT_Cap,
        0 as HT_QuyetToan,
        0 as CT_NamTruoc,
        0 as CT_DauNam,
        0 as CT_BoSung,
        0 as CP_Cap,
        0 as CP_Thu,
        0 as QT_rTuChi
FROM (
 --   -- DU TOAN
 --   SELECT  
	--		sLNS,
 --           iID_MaDonVi,
 --           SUM(rTuChi+rHangMua+rHangNhap) as rTuChi

 --   FROM    DT_ChungTuChiTiet 
 --   WHERE   iTrangThai=1 
 --           AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
 --           AND iNamLamViec=@iNamLamViec 
	--		AND iID_MaNamNganSach = 1
 --           AND  (MaLoai='' OR MaLoai='2') 
 --   GROUP BY sLNS,iID_MaDonVi
 --   HAVING SUM(rTuChi+rHangMua+rHangNhap)<>0 
            
 --   UNION ALL
 --   -- DU TOAN - PHAN CAP
 --   SELECT   
 --           sLNS,
 --           iID_MaDonVi,
 --           SUM(rTuChi+rHangMua+rHangNhap) as rTuChi
 --   FROM    DT_ChungTuChiTiet_PhanCap 
 --   WHERE   iTrangThai=1 
 --           AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
 --           AND iNamLamViec=@iNamLamViec 
	--		AND iID_MaNamNganSach = 1
 --   GROUP BY sLNS,iID_MaDonVi
 --   HAVING SUM(rTuChi+rHangMua+rHangNhap)<>0

	--union
	-- -- DU TOAN
 --   SELECT  
	--		sLNS,
 --           iID_MaDonVi,
 --           SUM(rTuChi+rHangMua+rHangNhap) as rTuChi

 --   FROM    DTBS_ChungTuChiTiet 
 --   WHERE   iTrangThai=1 
 --           AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
 --           AND iNamLamViec=@iNamLamViec 
	--		AND iID_MaNamNganSach = 1
 --           AND  (MaLoai='' OR MaLoai='2') 
 --   GROUP BY sLNS,iID_MaDonVi
 --   HAVING SUM(rTuChi+rHangMua+rHangNhap)<>0 
            
 --   UNION ALL
 --   -- DU TOAN - PHAN CAP
 --   SELECT   
 --           sLNS,
 --           iID_MaDonVi,
 --           SUM(rTuChi+rHangMua+rHangNhap) as rTuChi
 --   FROM    DTBS_ChungTuChiTiet_PhanCap 
 --   WHERE   iTrangThai=1 
 --           AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
 --           AND iNamLamViec=@iNamLamViec 
	--		AND iID_MaNamNganSach = 1
 --   GROUP BY sLNS,iID_MaDonVi
 --   HAVING SUM(rTuChi+rHangMua+rHangNhap)<>0

	 -- nam sau cap ve
	SELECT  sLNS, iId_MaDonVi,
            rTuChi=	CASE    
				WHEN sLNS LIKE '104%' then SUM(rTuChi)
				ELSE SUM(rTuChi+rHangNhap+rHangMua) END 
    FROM f_ns_table_chitieu(@iNamLamViec,@iID_MaDonVi,@iID_MaPhongBan,1,GETDATE(),1,null)
    GROUP BY sLNS1,sLNS3,sLNS5,sLNS, iId_MaDonVi

	union

	SELECT  sLNS = '1040200', iId_MaDonVi,
            rTuChi=	SUM(rHangNhap)
    FROM f_ns_table_chitieu(@iNamLamViec,@iID_MaDonVi,@iID_MaPhongBan,1,GETDATE(),1,null)
	where sLNS like '104%'
    GROUP BY sLNS1,sLNS3,sLNS5,sLNS,iId_MaDonVi

	union

	SELECT  sLNS = '1040300', iId_MaDonVi,
            rTuChi=	SUM(rHangMua)
    FROM f_ns_table_chitieu(@iNamLamViec,@iID_MaDonVi,@iID_MaPhongBan,1,GETDATE(),1,null)
	where sLNS like '104%'
    GROUP BY sLNS1,sLNS3,sLNS5,sLNS,iId_MaDonVi

	union

	-- nam nay cap cho nam truoc
	SELECT  sLNS, iId_MaDonVi,
            rTuChi=	CASE    
				WHEN sLNS LIKE '104%' then SUM(rTuChi)
				ELSE SUM(rTuChi+rHangNhap+rHangMua) END 
    FROM f_ns_table_chitieu(@iNamLamViec,@iID_MaDonVi,@iID_MaPhongBan,5,GETDATE(),1,null)
    GROUP BY sLNS1,sLNS3,sLNS5,sLNS, iId_MaDonVi

	union

	SELECT  sLNS='1040200', iId_MaDonVi,
            rTuChi=	SUM(rHangNhap)
    FROM f_ns_table_chitieu(@iNamLamViec,@iID_MaDonVi,@iID_MaPhongBan,5,GETDATE(),1,null)
	where sLNS like '104%'
    GROUP BY sLNS1,sLNS3,sLNS5,sLNS, iId_MaDonVi

	union

	SELECT  sLNS = '1040300', iId_MaDonVi,
            rTuChi=	SUM(rHangMua)
    FROM f_ns_table_chitieu(@iNamLamViec,@iID_MaDonVi,@iID_MaPhongBan,5,GETDATE(),1,null)
	where sLNS like '104%'
    GROUP BY sLNS1,sLNS3,sLNS5,sLNS, iId_MaDonVi

) as HT_CAP
GROUP BY sLNS, HT_CAP.iID_MaDonVi

UNION ALL

--2. Hợp thức quyết toán

SELECT  sLNS,
        HT_QuyetToan.iID_MaDonVi, 
        0 as HT_Cap,
        sum(rTuChi) as HT_QuyetToan,
        0 as CT_NamTruoc,
        0 as CT_DauNam,
        0 as CT_BoSung,
        0 as CP_Cap,
        0 as CP_Thu,
        0 as QT_rTuChi
FROM (
SELECT  sLNS,
        iID_MaDonVi,
        SUM(rTuChi) as rTuChi
FROM    QTA_ChungTuChiTiet
WHERE   iTrangThai=1 
		AND iNamLamViec=@iNamLamViec
		AND iID_MaNamNganSach in (select * from f_split('1,5'))
		AND (@iID_MaPhongBan IS NULL OR iID_MaPhongBan=@iID_MaPhongBan)
		AND (@iID_MaDonVi IS NULL OR iID_MaDonVi in (select * from F_Split(@iID_MaDonVi)))
GROUP BY sLNS,iID_MaDonVi,sTenDonVi

) as HT_QuyetToan
GROUP BY sLNS, HT_QuyetToan.iID_MaDonVi

/*

CHI TIEU NAM TRUOC

*/

UNION ALL

SELECT  sLNS,
        CT_NAMTRUOC.iID_MaDonVi, 
        0 as HT_Cap,
        0 as HT_QuyetToan,
        sum(rTuChi) as CT_NamTruoc,
        0 as CT_DauNam,
        0 as CT_BoSung,
        0 as CP_Cap,
        0 as CP_Thu,
        0 as QT_rTuChi
FROM (

	 ---- DU TOAN BỔ SUNG
  --  SELECT  
		--	sLNS,
  --          iID_MaDonVi,
  --          SUM(rTuChi+rHangMua+rHangNhap) as rTuChi

  --  FROM    DTBS_ChungTuChiTiet 
  --  WHERE   iTrangThai=1 
  --          AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
  --          AND iNamLamViec=@iNamLamViec 
		--	AND iID_MaNamNganSach = 4
  --          AND  (MaLoai='' OR MaLoai='2') 
  --  GROUP BY sLNS,iID_MaDonVi
  --  HAVING SUM(rTuChi+rHangMua+rHangNhap)<>0 
            
  --  UNION ALL
  --  -- DU TOAN - PHAN CAP
  --  SELECT   
  --          sLNS,
  --          iID_MaDonVi,
  --          SUM(rTuChi+rHangMua+rHangNhap) as rTuChi
  --  FROM    DTBS_ChungTuChiTiet_PhanCap 
  --  WHERE   iTrangThai=1 
  --          AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
  --          AND iNamLamViec=@iNamLamViec 
		--	AND iID_MaNamNganSach = 4
  --  GROUP BY sLNS,iID_MaDonVi
  --  HAVING SUM(rTuChi+rHangMua+rHangNhap)<>0

    SELECT
        sLNS,iID_MaDonVi,
		rTuChi=	CASE    
				WHEN sLNS LIKE '104%' then SUM(rTuChi)
				ELSE SUM(rTuChi+rHangNhap+rHangMua) END

    FROM    f_ns_table_chitieu(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, 4, GETDATE(),1,null)
    GROUP BY sLNS,iID_MaDonVi

	union
	SELECT
        sLNS='1040200',iID_MaDonVi,
		rTuChi =	 SUM(rHangNhap)

    FROM    f_ns_table_chitieu(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, 4, GETDATE(),1,null)
	where	sLNS like '104%'
    GROUP BY sLNS,iID_MaDonVi

	union
	SELECT
        sLNS='1040300',iID_MaDonVi,
		rTuChi=	 SUM(rHangMua)

    FROM    f_ns_table_chitieu(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, 4, GETDATE(),1,null)
	where	sLNS like '104%'
    GROUP BY sLNS,iID_MaDonVi


) as CT_NAMTRUOC
GROUP BY sLNS, CT_NAMTRUOC.iID_MaDonVi

UNION ALL


/*

DU TOAN NAM NAY

*/
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
					ELSE SUM(rTuChi+rHangNhap+rHangMua) END,
            0 as rBS
    FROM    DT_ChungTuChiTiet 
    WHERE   (iTrangThai=1 OR iTrangThai =2)
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
    WHERE   (iTrangThai=1 OR iTrangThai =2)
            AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
            AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach = 2
            AND  (MaLoai='' OR MaLoai='2') 
			AND sLNS like '104%'
    GROUP BY sLNS,iID_MaDonVi
    HAVING	 SUM(rHangNhap)<>0

	-- rHangMua
	UNION 
	SELECT      
			sLNS='1040300',
            iID_MaDonVi,
            rTuChi=SUM(rHangMua) ,
            rBS=0 
    FROM    DT_ChungTuChiTiet 
    WHERE   (iTrangThai=1 OR iTrangThai =2)
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
    WHERE   (iTrangThai=1 OR iTrangThai =2)
            AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
            AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach = 2
    GROUP BY sLNS,iID_MaDonVi
    HAVING	SUM(rTuChi)<>0  

    --//////////////////////////////////////////////////////////////////
    -- DU TOAN BO SUNG
    UNION ALL
    SELECT  sLNS,
            iID_MaDonVi,
            rTuChi=0,
            rBS=CASE    
					WHEN sLNS LIKE '104%' then SUM(rTuChi)
					ELSE  SUM(rTuChi+rHangNhap+rHangMua) END

        FROM DTBS_ChungTuChiTiet 
        WHERE  iTrangThai=1 
            AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
            AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach = 2
            AND (MaLoai='' OR MaLoai='2')
			--AND iID_MaChungTu IN (SELECT * FROM f_split(@iID_MaChungTu))
			AND (iID_MaChungTu IN (SELECT * FROM f_split(@iID_MaChungTu))
				 OR iID_MaChungTu IN (SELECT iID_MaDotNganSach FROM DTBS_ChungTuChiTiet WHERE iID_MaChungTu IN (SELECT * FROM f_split(@iID_MaChungTu))))

        GROUP BY sLNS,iID_MaDonVi
        HAVING	SUM(rTuChi)<>0 

	-- DTBS - 1040200
	UNION ALL
    SELECT  sLNS='1040200',
            iID_MaDonVi,
            rTuChi=0,
            rBS=SUM(rHangNhap) 
        FROM DTBS_ChungTuChiTiet 
        WHERE  iTrangThai=1 
            AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
            AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach = 2
            AND (MaLoai='' OR MaLoai='2')
			AND sLNS like '104%'
			AND iID_MaChungTu IN (SELECT * FROM f_split(@iID_MaChungTu))
        GROUP BY sLNS,iID_MaDonVi
        HAVING	SUM(rHangNhap)<>0 

	-- DTBS - 1040300
	UNION ALL
    SELECT  sLNS='1040300',
            iID_MaDonVi,
            rTuChi=0,
            rBS=SUM(rHangMua) 
        FROM DTBS_ChungTuChiTiet 
        WHERE  iTrangThai=1 
            AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
            AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach = 2
            AND (MaLoai='' OR MaLoai='2')
			AND sLNS like '104%'
			AND iID_MaChungTu IN (SELECT * FROM f_split(@iID_MaChungTu))
        GROUP BY sLNS,iID_MaDonVi
        HAVING	SUM(rHangMua)<>0 

	-- PHAN CAP BO SUNG
    UNION ALL

	SELECT	             
			sLNS,
			iID_MaDonVi,
			rTuChi=0,
            rBS=SUM(rTuChi+rHangNhap+rHangMua)
	FROM f_ns_dtbs_phancap(@iNamLamViec,@iID_MaPhongBan, @iID_MaDonVi,@iID_MaChungTu) 
    GROUP BY sLNS,iID_MaDonVi
    HAVING SUM(rTuChi)<>0 OR SUM(rHangNhap)<>0 OR SUM(rHangMua)<>0

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
        --AND (@iID_MaPhongBan is NULL OR iID_MaPhongBan=@iID_MaPhongBan)  
        AND iNamLamViec=@iNamLamViec 
		AND iID_MaNamNganSach = 2
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
		AND sLNS <> ''
GROUP BY sLNS,iID_MaDonVi
HAVING SUM(rTuChi)<>0 

) as dt1
GROUP BY sLNS,iID_MaDonVi


) as DT

left join (select distinct sLNS as id,sMoTa from NS_MucLucNganSach where iTrangThai=1 and iNamLamViec=@iNamLamViec and sL='' and sLNS=sXauNoiMa) as mlns on dt.sLNS=mlns.id

where  
        sLNS1 in (1,2,3,4)
        --AND sLNS not in(1050000)
        AND (@iID_MaDonVi IS NULL OR iID_MaDonVi IN (SELECT * FROM F_Split(@iID_MaDonVi)))
		--and slns like '102%'
group by sLNS1, sLNS3, sLNS5,sLNS, iID_MaDonVi,mlns.sMoTa
having	SUM(HT_Cap)<>0 
		or sum(HT_QuyetToan)<>0
		or sum(CT_NamTruoc)<>0
		or sum(CT_DauNam) <>0
		or sum(CT_BoSung) <>0
		or sum(CP_Cap) <>0
		or sum(CP_Thu) <>0
		or sum(QT_rTuChi) <>0
ORDER BY sLNS, iID_MaDonVi


--select * from F_NS_DTBS_ChungTuDaGom(@iNamLamViec,@iID_MaPhongBan)
