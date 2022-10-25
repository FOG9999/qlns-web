
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2021
declare @username nvarchar(20)						set @username='chuctc'
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07' 
declare @iID_MaDonVi nvarchar(max)					set @iID_MaDonVi=null
--declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='51'
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach='1,2,4'

--###--

declare @date datetime
set @date = GETDATE()

declare @iID_MaChungTu nvarchar(MAX)
set @iID_MaChungTu = [dbo].[f_ns_dtbs_chungtu_gom_todate](@iNamLamViec,'1,2,4',@iID_MaPhongBan,@date)


DECLARE @Temp1 TABLE
(
	sLNS1 nvarchar(50),
	sLNS3 nvarchar(50),	
	sLNS5 nvarchar(50),	  
	sLNS nvarchar(50),
	sL nvarchar(50),
	sK nvarchar(50),
	sM nvarchar(50),
	sTM nvarchar(50),
	sTTM nvarchar(50),
	sNG nvarchar(50),
	sMoTa nvarchar(max),
	sXauNoiMa nvarchar(max),
	iID_MaDonVi nvarchar(5),
	sTenDonVi nvarchar(200), 
	rTuChi decimal(18,0),
	rHienVat decimal(18,0),
	rTonKho decimal(18,0),
	rHangNhap decimal(18,0),
	rHangMua decimal(18,0),
	rPhanCap decimal(18,0),
	rDuPhong decimal(18,0)
) 

INSERT INTO @Temp1
SELECT sLNS1,
		sLNS3,	
		sLNS5,	  
		sLNS,
        sL,
		sK,
		sM,
		sTM,
		sTTM,
		sNG,
		sMoTa,
		sXauNoiMa,
        iID_MaDonVi,
		sTenDonVi, 
        rTuChi,
        rHienVat,
        rTonKho,
        rHangNhap,
        rHangMua,
        rPhanCap,
        rDuPhong
FROM f_ns_chitieu_full5(@iNamLamViec,@iID_MaDonVi,@iID_MaPhongBan,1,null,@date,1,null)

DECLARE @Temp2 TABLE
(
	sLNS1 nvarchar(50),
	sLNS3 nvarchar(50),	
	sLNS5 nvarchar(50),	  
	sLNS nvarchar(50),
	sL nvarchar(50),
	sK nvarchar(50),
	sM nvarchar(50),
	sTM nvarchar(50),
	sTTM nvarchar(50),
	sNG nvarchar(50),
	sMoTa nvarchar(max),
	sXauNoiMa nvarchar(max),
	iID_MaDonVi nvarchar(5),
	sTenDonVi nvarchar(200), 
	rTuChi decimal(18,0),
	rHienVat decimal(18,0),
	rTonKho decimal(18,0),
	rHangNhap decimal(18,0),
	rHangMua decimal(18,0),
	rPhanCap decimal(18,0),
	rDuPhong decimal(18,0)
) 

INSERT INTO @Temp2
SELECT sLNS1,
		sLNS3,	
		sLNS5,	  
		sLNS,
        sL,
		sK,
		sM,
		sTM,
		sTTM,
		sNG,
		sMoTa,
		sXauNoiMa,
        iID_MaDonVi,
		sTenDonVi, 
        rTuChi,
        rHienVat,
        rTonKho,
        rHangNhap,
        rHangMua,
        rPhanCap,
        rDuPhong
FROM f_ns_chitieu_full5(@iNamLamViec,@iID_MaDonVi,@iID_MaPhongBan,5,null,@date,1,null)

DECLARE @Temp3 TABLE
(
	sLNS1 nvarchar(50),
	sLNS3 nvarchar(50),	
	sLNS5 nvarchar(50),	  
	sLNS nvarchar(50),
	sL nvarchar(50),
	sK nvarchar(50),
	sM nvarchar(50),
	sTM nvarchar(50),
	sTTM nvarchar(50),
	sNG nvarchar(50),
	sMoTa nvarchar(max),
	sXauNoiMa nvarchar(max),
	iID_MaDonVi nvarchar(5),
	sTenDonVi nvarchar(200), 
	rTuChi decimal(18,0),
	rHienVat decimal(18,0),
	rTonKho decimal(18,0),
	rHangNhap decimal(18,0),
	rHangMua decimal(18,0),
	rPhanCap decimal(18,0),
	rDuPhong decimal(18,0)
	) 

INSERT INTO @Temp3
SELECT sLNS1,
		sLNS3,	
		sLNS5,	  
		sLNS,
        sL,
		sK,
		sM,
		sTM,
		sTTM,
		sNG,
		sMoTa,
		sXauNoiMa,
        iID_MaDonVi,
		sTenDonVi, 
        rTuChi,
        rHienVat,
        rTonKho,
        rHangNhap,
        rHangMua,
        rPhanCap,
        rDuPhong
FROM f_ns_chitieu_full5(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, 4,null,@date,1,null)

DECLARE @Temp4 TABLE
	(
		sLNS1 nvarchar(50),
		sLNS3 nvarchar(50),	
		sLNS5 nvarchar(50),	  
		sLNS nvarchar(50),
        sL nvarchar(50),
		sK nvarchar(50),
		sM nvarchar(50),
		sTM nvarchar(50),
		sTTM nvarchar(50),
		sNG nvarchar(50),
		sMoTa nvarchar(max),
		sXauNoiMa nvarchar(max),
        iID_MaDonVi nvarchar(5),
		sTenDonVi nvarchar(200), 
        rTuChi decimal(18,0),
        rHienVat decimal(18,0),
        rTonKho decimal(18,0),
        rHangNhap decimal(18,0),
        rHangMua decimal(18,0),
        rPhanCap decimal(18,0),
        rDuPhong decimal(18,0)
	) 

INSERT INTO @Temp4
SELECT sLNS1,
		sLNS3,	
		sLNS5,	  
		sLNS,
        sL,
		sK,
		sM,
		sTM,
		sTTM,
		sNG,
		sMoTa,
		sXauNoiMa,
        iID_MaDonVi,
		sTenDonVi, 
        rTuChi,
        rHienVat,
        rTonKho,
        rHangNhap,
        rHangMua,
        rPhanCap,
        rDuPhong
FROM f_ns_chitieu_full5(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, 2,null,@date,1,1)

DECLARE @Temp5 TABLE
(
	sLNS1 nvarchar(50),
	sLNS3 nvarchar(50),	
	sLNS5 nvarchar(50),	  
	sLNS nvarchar(50),
	sL nvarchar(50),
	sK nvarchar(50),
	sM nvarchar(50),
	sTM nvarchar(50),
	sTTM nvarchar(50),
	sNG nvarchar(50),
	sMoTa nvarchar(max),
	sXauNoiMa nvarchar(max),
	iID_MaDonVi nvarchar(5),
	sTenDonVi nvarchar(200), 
	rTuChi decimal(18,0),
	rHienVat decimal(18,0),
	rTonKho decimal(18,0),
	rHangNhap decimal(18,0),
	rHangMua decimal(18,0),
	rPhanCap decimal(18,0),
	rDuPhong decimal(18,0)
) 

INSERT INTO @Temp5
SELECT sLNS1,
		sLNS3,	
		sLNS5,	  
		sLNS,
        sL,
		sK,
		sM,
		sTM,
		sTTM,
		sNG,
		sMoTa,
		sXauNoiMa,
        iID_MaDonVi,
		sTenDonVi, 
        rTuChi,
        rHienVat,
        rTonKho,
        rHangNhap,
        rHangMua,
        rPhanCap,
        rDuPhong
FROM f_ns_chitieu_full5(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, 2,null,@date,1,2)
				 
DECLARE @Temp6 TABLE
(
	sLNS1 nvarchar(50),
	sLNS3 nvarchar(50),	
	sLNS5 nvarchar(50),	  
	sLNS nvarchar(50),
	sL nvarchar(50),
	sK nvarchar(50),
	sM nvarchar(50),
	sTM nvarchar(50),
	sTTM nvarchar(50),
	sNG nvarchar(50),
	sMoTa nvarchar(max),
	sXauNoiMa nvarchar(max),
	iID_MaDonVi nvarchar(5),
	sTenDonVi nvarchar(200), 
	rTuChi decimal(18,0),
	rHienVat decimal(18,0),
	rTonKho decimal(18,0),
	rHangNhap decimal(18,0),
	rHangMua decimal(18,0),
	rPhanCap decimal(18,0),
	rDuPhong decimal(18,0)
) 

INSERT INTO @Temp6
SELECT sLNS1,
		sLNS3,	
		sLNS5,	  
		sLNS,
        sL,
		sK,
		sM,
		sTM,
		sTTM,
		sNG,
		sMoTa,
		sXauNoiMa,
        iID_MaDonVi,
		sTenDonVi, 
        rTuChi,
        rHienVat,
        rTonKho,
        rHangNhap,
        rHangMua,
        rPhanCap,
        rDuPhong
FROM f_ns_chitieu_full5_bv_g(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, 2,null,@date,1,2)
				 
DECLARE @Temp7 TABLE
(
	sLNS1 nvarchar(50),
	sLNS3 nvarchar(50),	
	sLNS5 nvarchar(50),	  
	sLNS nvarchar(50),
	sL nvarchar(50),
	sK nvarchar(50),
	sM nvarchar(50),
	sTM nvarchar(50),
	sTTM nvarchar(50),
	sNG nvarchar(50),
	sMoTa nvarchar(max),
	sXauNoiMa nvarchar(max),
	iID_MaDonVi nvarchar(5),
	sTenDonVi nvarchar(200), 
	rTuChi decimal(18,0),
	rHienVat decimal(18,0),
	rTonKho decimal(18,0),
	rHangNhap decimal(18,0),
	rHangMua decimal(18,0),
	rPhanCap decimal(18,0),
	rDuPhong decimal(18,0)
) 

INSERT INTO @Temp7
SELECT sLNS1,
		sLNS3,	
		sLNS5,	  
		sLNS,
        sL,
		sK,
		sM,
		sTM,
		sTTM,
		sNG,
		sMoTa,
		sXauNoiMa,
        iID_MaDonVi,
		sTenDonVi, 
        rTuChi,
        rHienVat,
        rTonKho,
        rHangNhap,
        rHangMua,
        rPhanCap,
        rDuPhong
FROM f_ns_chitieu_full5_bv(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, 2,null,@date,1,2)


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
                WHEN sLNS=1050100 then 101
                WHEN sLNS LIKE '102%' then 102
                else  LEFT(sLNS,3) END, 
        sLNS5	=CASE    
                WHEN sLNS=1050100 then 10100
                WHEN sLNS LIKE '102%' then 10200
                else  LEFT(sLNS,5) END, 
        sLNS	=CASE    
                WHEN sLNS=1050100 then 1010000
                WHEN sLNS LIKE '102%' AND sLNS <> '1020200' then 1020100
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
  
	 -- nam sau cap ve
	SELECT  sLNS, iId_MaDonVi = left(iID_MaDonVi,2),
            rTuChi=	CASE    
				WHEN sLNS LIKE '104%' then SUM(rTuChi)
				ELSE SUM(rTuChi+rHangNhap+rHangMua) END 
    --FROM f_ns_chitieu_full5(@iNamLamViec,@iID_MaDonVi,@iID_MaPhongBan,1,null,@date,1,null)
	FROM @Temp1
    GROUP BY sLNS1,sLNS3,sLNS5,sLNS, iId_MaDonVi

	UNION ALL

	SELECT  sLNS = '1040200', iId_MaDonVi = left(iID_MaDonVi,2),
            rTuChi=	SUM(rHangNhap)
    --FROM f_ns_chitieu_full5(@iNamLamViec,@iID_MaDonVi,@iID_MaPhongBan,1,null,@date,1,null)
	FROM @Temp1
	where sLNS like '104%'
    GROUP BY sLNS1,sLNS3,sLNS5,sLNS,iId_MaDonVi

	UNION ALL

	SELECT  sLNS = '1040300', iId_MaDonVi = left(iID_MaDonVi,2),
            rTuChi=	SUM(rHangMua)
    --FROM f_ns_chitieu_full5(@iNamLamViec,@iID_MaDonVi,@iID_MaPhongBan,1,null,@date,1,null)
	FROM @Temp1
	where sLNS like '104%'
    GROUP BY sLNS1,sLNS3,sLNS5,sLNS,iId_MaDonVi

	UNION ALL

	-- nam nay cap cho nam truoc
	SELECT  sLNS, iId_MaDonVi = left(iID_MaDonVi,2),
            rTuChi=	CASE    
				WHEN sLNS LIKE '104%' then SUM(rTuChi)
				ELSE SUM(rTuChi+rHangNhap+rHangMua) END 
    --FROM f_ns_chitieu_full5(@iNamLamViec,@iID_MaDonVi,@iID_MaPhongBan,5,null,@date,1,null)
	FROM @Temp2
    GROUP BY sLNS1,sLNS3,sLNS5,sLNS, iId_MaDonVi

	UNION ALL

	SELECT  sLNS='1040200', iId_MaDonVi = left(iID_MaDonVi,2),
            rTuChi=	SUM(rHangNhap)
    --FROM f_ns_chitieu_full5(@iNamLamViec,@iID_MaDonVi,@iID_MaPhongBan,5,null,@date,1,null)
	FROM @Temp2
	where sLNS like '104%'
    GROUP BY sLNS1,sLNS3,sLNS5,sLNS, iId_MaDonVi

	UNION ALL

	SELECT  sLNS = '1040300', iId_MaDonVi = left(iID_MaDonVi,2),
            rTuChi=	SUM(rHangMua)
    --FROM f_ns_chitieu_full5(@iNamLamViec,@iID_MaDonVi,@iID_MaPhongBan,5,null,@date,1,null)
	FROM @Temp2
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
        iID_MaDonVi = left(iID_MaDonVi,2),
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

-- du toan dau nam
    SELECT
        sLNS,iID_MaDonVi = left(iID_MaDonVi,2),
		rTuChi=	CASE    
				WHEN sLNS LIKE '104%' then SUM(rTuChi)
				ELSE SUM(rTuChi+rHangNhap+rHangMua) END

    --FROM    f_ns_chitieu_full5(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, 4,null,@date,1,null)
	FROM @Temp3
    GROUP BY sLNS,iID_MaDonVi

	UNION ALL
	SELECT
        sLNS='1040200',iID_MaDonVi = left(iID_MaDonVi,2),
		rTuChi =	 SUM(rHangNhap)

    --FROM    f_ns_chitieu_full5(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, 4,null,@date,1,null)
	FROM @Temp3
	where	sLNS like '104%'
    GROUP BY sLNS,iID_MaDonVi

	UNION ALL
	SELECT
        sLNS='1040300',iID_MaDonVi = left(iID_MaDonVi,2),
		rTuChi=	 SUM(rHangMua)

    --FROM    f_ns_chitieu_full5(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, 4,null,@date,1,null)
	FROM @Temp3
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

    -- DU TOAN DAU NAM

	-- rTuChi
    SELECT      
			sLNS,
            iID_MaDonVi,
            rTuChi=	CASE    
					WHEN sLNS LIKE '104%' then SUM(rTuChi)
					ELSE SUM(rTuChi+rHangNhap+rHangMua) END,
            0 as rBS
    --FROM    f_ns_chitieu_full5(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, 2,null,@date,1,1)
	FROM @Temp4
    GROUP BY sLNS,iID_MaDonVi
    HAVING	SUM(rTuChi)<>0

	-- rHangNhap
	UNION ALL 
	SELECT      
			sLNS='1040200',
            iID_MaDonVi,
            rTuChi=SUM(rHangNhap),
            rBS=0 
    --FROM    f_ns_chitieu_full5(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, 2,null,@date,1,1)
	FROM @Temp4
    GROUP BY sLNS,iID_MaDonVi
    HAVING	 SUM(rHangNhap)<>0

	-- rHangMua
	UNION ALL 
	SELECT      
			sLNS='1040300',
            iID_MaDonVi,
            rTuChi=SUM(rHangMua) ,
            rBS=0 
    --FROM    f_ns_chitieu_full5(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, 2,null,@date,1,1)
	FROM @Temp4
    GROUP BY sLNS,iID_MaDonVi
    HAVING	 SUM(rHangMua)<>0

    
    --//////////////////////////////////////////////////////////////////
    -- DU TOAN BO SUNG

    UNION ALL
    SELECT  sLNS,
            iID_MaDonVi,
            rTuChi=0,
            rBS=CASE    
					WHEN sLNS LIKE '104%' then SUM(rTuChi)
					ELSE  SUM(rTuChi+rHangNhap+rHangMua) END

       
    --FROM    f_ns_chitieu_full5(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, 2,null,@date,1,2)
	FROM @Temp5
	where	LEN(iId_MaDonVi) = 2
    GROUP BY sLNS,iID_MaDonVi
    HAVING	SUM(rTuChi+rHangNhap+rHangMua)<>0 


	-- DTBS - 1040200
	UNION ALL
    SELECT  sLNS='1040200',
            iID_MaDonVi,
            rTuChi=0,
            rBS=SUM(rHangNhap) 
    --FROM    f_ns_chitieu_full5(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, 2,null,@date,1,2)
	FROM @Temp5
	where	LEN(iId_MaDonVi) = 2
    GROUP BY sLNS,iID_MaDonVi
    HAVING	SUM(rHangNhap)<>0 

	-- DTBS - 1040300
	UNION ALL
    SELECT  sLNS='1040300',
            iID_MaDonVi,
            rTuChi=0,
            rBS=SUM(rHangMua) 
       
    --FROM    f_ns_chitieu_full5(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, 2,null,@date,1,2)
	FROM @Temp5
	where	LEN(iId_MaDonVi) = 2
    GROUP BY sLNS,iID_MaDonVi
    HAVING	SUM(rHangMua)<>0 

	-- GIAM SO CHUYEN DU TOAN BENH VIEN VE CAP 2
	UNION ALL
    SELECT  sLNS,
            iID_MaDonVi = left(iID_MaDonVi,2),
            rTuChi=0,
            rBS=CASE    
					WHEN sLNS LIKE '104%' then SUM(rTuChi)
					ELSE  SUM(rTuChi+rHangNhap+rHangMua) END
       
    --FROM    f_ns_chitieu_full5_bv_g(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, 2,null,@date,1,2)
	FROM @Temp6
		
    GROUP BY sLNS,iID_MaDonVi
    HAVING	SUM(rTuChi+rHangNhap+rHangMua)<>0 

	-- DTBS - 1040200
	UNION ALL
    SELECT  sLNS='1040200',
            iID_MaDonVi = left(iID_MaDonVi,2),
            rTuChi=0,
            rBS=SUM(rHangNhap) 
    --FROM    f_ns_chitieu_full5_bv_g(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, 2,null,@date,1,2)
	FROM @Temp6
		
    GROUP BY sLNS,iID_MaDonVi
    HAVING	SUM(rHangNhap)<>0 

	-- DTBS - 1040300
	UNION ALL
    SELECT  sLNS='1040300',
            iID_MaDonVi = left(iID_MaDonVi,2) ,
            rTuChi=0,
            rBS=SUM(rHangMua) 
       
    --FROM    f_ns_chitieu_full5_bv_g(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, 2,null,@date,1,2)
	FROM @Temp6
		
    GROUP BY sLNS,iID_MaDonVi
    HAVING	SUM(rHangMua)<>0 

	-- SO Du Toan Bo sung cho benh vien
	UNION ALL
    SELECT  sLNS,
            iID_MaDonVi ,
            rTuChi=0,
            rBS=CASE    
					WHEN sLNS LIKE '104%' then SUM(rTuChi)
					ELSE  SUM(rTuChi+rHangNhap+rHangMua) END
       
    --FROM    f_ns_chitieu_full5_bv(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, 2,null,@date,1,2)
	FROM @Temp7
    GROUP BY sLNS,iID_MaDonVi
    HAVING	SUM(rTuChi)<>0 

	-- DTBS - 1040200
	UNION ALL
    SELECT  sLNS='1040200',
            iID_MaDonVi ,
            rTuChi=0,
            rBS=SUM(rHangNhap) 
    --FROM    f_ns_chitieu_full5_bv(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, 2,null,@date,1,2)
	FROM @Temp7
    GROUP BY sLNS,iID_MaDonVi
    HAVING	SUM(rHangNhap)<>0 

	-- DTBS - 1040300
	UNION ALL
    SELECT  sLNS='1040300',
            iID_MaDonVi ,
            rTuChi=0,
            rBS=SUM(rHangMua) 
       
    --FROM    f_ns_chitieu_full5_bv(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, 2,null,@date,1,2)
	FROM @Temp7
    GROUP BY sLNS,iID_MaDonVi
    HAVING	SUM(rHangMua)<>0 

) as ct


GROUP BY sLNS, CT.iID_MaDonVi
--ORDER BY sLNS,iID_MaDonVi

UNION ALL

-- QUYET TOAN                   
SELECT  sLNS,

		iID_MaDonVi = left(iID_MaDonVi,2),
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

        iID_MaDonVi = left(iID_MaDonVi,2),
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
        AND sLNS not in(1050000)
        AND (@iID_MaDonVi IS NULL OR iID_MaDonVi IN (SELECT * FROM F_Split(@iID_MaDonVi)))
		and sLNS like '2030100%' and sLNS not like '201%'
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
