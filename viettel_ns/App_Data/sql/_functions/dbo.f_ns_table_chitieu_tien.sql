USE [VIETTEL_NS1]
GO
/****** Object:  UserDefinedFunction [dbo].[f_ns_table_chitieu_tien]    Script Date: 3/12/2019 8:01:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
	
	author:	hieppm
	date:	11/03/2019
	decs:	Lấy chỉ tiêu ngân sách (tiền)
	params:	loai	1. đầu năm, 2. bổ sung, null. tất cả

*/



ALTER FUNCTION [dbo].[f_ns_table_chitieu_tien]
	(
		
	@iNamLamViec int,
	@iID_MaDonVi nvarchar(2000),
	@iID_MaPhongBan nvarchar(10),
	@iID_MaNamNganSach nvarchar(10),
	@dNgay datetime,
	@dvt int,
	@loai nvarchar(10)
	
	)

RETURNS @tabResult TABLE ( sLNS nvarchar(7), sL nvarchar(3), sK nvarchar(3), sM nvarchar(4), sTM nvarchar(4), sTTM nvarchar(2), sNG nvarchar(2), 
							iID_MaDonVi nvarchar(2), rTien decimal(18,0))
AS
BEGIN

declare @iID_MaChungTu nvarchar(MAX)
select @iID_MaChungTu = [dbo].[f_ns_dtbs_chungtu_gom_todate](@iNamLamViec,@iID_MaNamNganSach,@iID_MaPhongBan,@dNgay)

INSERT INTO @tabResult(sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi,rTien)
-- du toan 
SELECT  sLNS	=CASE    
                WHEN sLNS LIKE '1020000' then 1020100
                else sLNS END, 
        sL,sK,sM,sTM,sTTM,sNG,
        LTRIM(RTRIM(iID_MaDonVi)),

        rTien		=SUM(rTuChi)/@dvt
FROM (

     --DU TOAN

    SELECT  sLNS,sL,sK,sM,sTM,sTTM,sNG,
            iID_MaDonVi,
			loai = 1,
			rTuChi	=	CASE    
						WHEN sLNS LIKE '104%' then SUM(rTuChi)
						ELSE SUM(rTuChi+rHangNhap+rHangMua) END

    FROM    DT_ChungTuChiTiet 
    WHERE   iTrangThai=1 
            AND iNamLamViec=@iNamLamViec 
			AND iID_MaNamNganSach  IN (SELECT * FROM f_split(@iID_MaNamNganSach))
			AND (((sLNS like '104%' and (MaLoai='' OR MaLoai='2')) or (sLNS not like '104%')))
            AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
			AND (@iID_MaDonVi is null or iID_MaDonVi in (select * from f_split(@iID_MaDonVi)))    
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi
    HAVING SUM(rTuChi)<>0 OR SUM(rHangNhap)<>0 OR SUM(rHangMua)<>0

	UNION ALL

	SELECT  sLNS = '1040200',sL,sK,sM,sTM,sTTM,sNG,
            iID_MaDonVi,
			loai = 1,
			rTuChi	= SUM(rHangNhap)
    FROM    DT_ChungTuChiTiet 
    WHERE   iTrangThai=1 
            AND iNamLamViec=@iNamLamViec 
			AND iID_MaNamNganSach  IN (SELECT * FROM f_split(@iID_MaNamNganSach))
			AND (sLNS like '104%' and (MaLoai='' OR MaLoai='2'))
            AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
			AND (@iID_MaDonVi is null or iID_MaDonVi in (select * from f_split(@iID_MaDonVi)))    
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi
    HAVING SUM(rHangNhap)<>0

	UNION ALL

	SELECT   
            sLNS = '1040300',sL,sK,sM,sTM,sTTM,sNG,
            iID_MaDonVi,
			loai = 1,
			rTuChi	=	SUM(rHangMua)

    FROM    DT_ChungTuChiTiet 
    WHERE   iTrangThai=1 
            AND iNamLamViec=@iNamLamViec 
			AND iID_MaNamNganSach  IN (SELECT * FROM f_split(@iID_MaNamNganSach))
			AND (sLNS like '104%' and (MaLoai='' OR MaLoai='2'))
            AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
			AND (@iID_MaDonVi is null or iID_MaDonVi in (select * from f_split(@iID_MaDonVi)))    
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi
    HAVING SUM(rHangMua)<>0

    UNION ALL

    -- DU TOAN - PHAN CAP
    SELECT   
            sLNS,sL,sK,sM,sTM,sTTM,sNG,
            iID_MaDonVi,
			loai = 1,
            
            rTuChi		=SUM(rTuChi)

    FROM    DT_ChungTuChiTiet_PhanCap 
    WHERE   iTrangThai=1 
            AND iNamLamViec=@iNamLamViec 
			AND iID_MaNamNganSach IN (SELECT * FROM f_split(@iID_MaNamNganSach))
            AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
			AND (@iID_MaDonVi is null or iID_MaDonVi in (select * from f_split(@iID_MaDonVi)))
    
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi  
	HAVING SUM(rTuChi)<>0
                        
    UNION ALL


    ---- BO SUNG
    SELECT  
            sLNS,sL,sK,sM,sTM,sTTM,sNG,
            iID_MaDonVi,
			loai = 2,
			rTuChi	=	CASE    
						WHEN sLNS LIKE '104%' then SUM(rTuChi)
						ELSE SUM(rTuChi+rHangNhap+rHangMua) END

        FROM DTBS_ChungTuChiTiet 
        WHERE  iTrangThai=1 
            AND iNamLamViec=@iNamLamViec 
			AND iID_MaNamNganSach IN (SELECT * FROM f_split(@iID_MaNamNganSach))
            AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
            AND (MaLoai='' OR MaLoai='2')
			AND (@iID_MaDonVi is null or iID_MaDonVi in (select * from f_split(@iID_MaDonVi)))
			AND iID_MaChungTu IN (select * from f_split(@iID_MaChungTu))
            
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi
    HAVING SUM(rTuChi)<>0 OR SUM(rHangNhap)<>0 OR SUM(rHangMua)<>0
	
    UNION ALL

	SELECT  
            sLNS='1040200',sL,sK,sM,sTM,sTTM,sNG,
            iID_MaDonVi,
			loai = 2,
			rTuChi	=	SUM(rHangNhap)

        FROM DTBS_ChungTuChiTiet 
        WHERE  iTrangThai=1 
            AND iNamLamViec=@iNamLamViec 
			AND iID_MaNamNganSach IN (SELECT * FROM f_split(@iID_MaNamNganSach))
            AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
            AND (sLNS like '104%' and (MaLoai='' OR MaLoai='2'))
			AND (@iID_MaDonVi is null or iID_MaDonVi in (select * from f_split(@iID_MaDonVi)))
			AND iID_MaChungTu IN (select * from f_split(@iID_MaChungTu))
            
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi
    HAVING SUM(rHangNhap)<>0
	
    UNION ALL

	SELECT  
            sLNS='1040300',sL,sK,sM,sTM,sTTM,sNG,
            iID_MaDonVi,
			loai = 2,
			rTuChi	=	SUM(rHangMua)

        FROM DTBS_ChungTuChiTiet 
        WHERE  iTrangThai=1 
            AND iNamLamViec=@iNamLamViec 
			AND iID_MaNamNganSach IN (SELECT * FROM f_split(@iID_MaNamNganSach))
            AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
            AND (sLNS like '104%' and (MaLoai='' OR MaLoai='2'))
			AND (@iID_MaDonVi is null or iID_MaDonVi in (select * from f_split(@iID_MaDonVi)))
			AND iID_MaChungTu IN (select * from f_split(@iID_MaChungTu))
            
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi
    HAVING SUM(rHangMua)<>0
	
    UNION ALL

    SELECT          
            sLNS,sL,sK,sM,sTM,sTTM,sNG,
            iID_MaDonVi, 
			loai = 2,
			       
            rTuChi		=SUM(rTuChi)
	FROM    f_ns_dtbs_phancap(@iNamLamViec,@iID_MaPhongBan, @iID_MaDonVi,@iID_MaChungTu)	
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi
    HAVING SUM(rTuChi)<>0

	--UNION ALL
	---- NS đặc biệt --
	--SELECT		sLNS,sL,sK,sM,sTM,sTTM,sNG,
	--			iID_MaDonVi,   
	--			loai = 2,
			       
	--			rTuChi		=SUM(rTuChi),
	--			rHienVat	=0,
	--			rTonKho		=0,
	--			rHangNhap	=0,
	--			rHangMua	=0,
	--			rPhanCap	=0,
	--			rDuPhong	=0
	--FROM		CP_CapPhatChiTiet 
	--WHERE		iTrangThai=1 
	--			AND (@iID_MaPhongBan is NULL OR iID_MaPhongBan=@iID_MaPhongBan)  
	--			AND iNamLamViec=@iNamLamViec 
	--			AND sLNS LIKE '3%'
	--			AND ( @iID_MaNamNganSach IS NULL OR iID_MaNamNganSach IN (SELECT * FROM F_Split(@iID_MaNamNganSach)))
	--			AND ( iID_MaDonVi = @iID_MaDonVi)

	--GROUP BY	sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi
	--HAVING		SUM(rTuChi)<>0
) as ct

WHERE	LEFT(sLNS,1) IN (1,2,3,4) 
		--and sLNS='1040100'
		and (@loai is null or loai in (select * from f_split(@loai)))
GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi    

RETURN
END