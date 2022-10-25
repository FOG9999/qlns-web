
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @username nvarchar(20)						set @username='chuctc'
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07' 
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='44'
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach='1,2'
declare @dMaDot datetime							set @dMaDot='2018-03-10 00:00:00.0'

--#DECLARE#--


-- du toan
SELECT  sLNS1,sLNS3,sLNS5,
        sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sXauNoiMa,
        iID_MaDonVi,sTenDonVi,
        SUM(rTuChi/@dvt) as CT_DauNam,
        SUM(rBS/@dvt) as CT_BoSung,
        SUM(rTuChi/@dvt + rBS/@dvt) as CT,
        SUM(rTonKho/@dvt) as rTonKho,
        SUM(rHangNhap/@dvt) as rHangNhap,
        SUM(rHienVat/@dvt) as rHienVat,
        SUM(rTuChi/@dvt) as rTuChi,
        SUM(rHangMua/@dvt) as rHangMua,
        SUM(rPhanCap/@dvt) as rPhanCap,
        SUM(rDuPhong/@dvt) as rDuPhong

FROM (

    -- DU TOAN
    SELECT      
            LEFT(sLNS,1) as sLNS1,
            --LEFT(sLNS,3) as sLNS3,
            --LEFT(sLNS,5) as sLNS5,
            --sLNS,
                
            sLNS3=  CASE    
                    WHEN sLNS=1050100 then '101'
                    else  LEFT(sLNS,3) END, 
            sLNS5=  CASE    
                    WHEN sLNS=1050100 then '10100'
                    WHEN sLNS=1020000 then '10201'
                    else  LEFT(sLNS,5) END, 
            sLNS=   CASE    
                    WHEN sLNS=1050100 then '1010000'
                    WHEN sLNS=1020000 then '1020100'
                    else sLNS END,
                    
            sL,sK,sM,sTM,sTTM,sNG,sMoTa,sXauNoiMa,
            iID_MaDonVi,sTenDonVi,
            0 as rBS,

            SUM(rTonKho) as rTonKho,
            SUM(rHangNhap) as rHangNhap,
            SUM(rHienVat) as rHienVat,
            SUM(rTuChi) as rTuChi,
            SUM(rHangMua) as rHangMua,
            SUM(rPhanCap) as rPhanCap,
            SUM(rDuPhong) as rDuPhong


    FROM    DT_ChungTuChiTiet 
    WHERE   iTrangThai=1 
            AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
            AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach  IN (SELECT * FROM F_SplitString2(@iID_MaNamNganSach))
            AND  (MaLoai='' OR MaLoai='2') 
            AND (@iID_MaDonVi IS NULL OR iID_MaDonVi=@iID_MaDonVi)
            AND (iID_MaChungTu IN (SELECT DISTINCT iID_MaChungTu FROM DT_ChungTu WHERE dNgayChungTu <= @dMaDot AND sID_MaNguoiDungTao = @username))
    
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sXauNoiMa,iID_MaDonVi,sTenDonVi
    HAVING SUM(rTuChi)<>0 
            
    UNION ALL
    -- DU TOAN - PHAN CAP
    SELECT  LEFT(sLNS,1) as sLNS1,
            LEFT(sLNS,3) as sLNS3,
            LEFT(sLNS,5) as sLNS5, 
            sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sXauNoiMa,
            iID_MaDonVi,sTenDonVi,
            0 as rBS,
            
            SUM(rTonKho) as rTonKho,
            SUM(rHangNhap) as rHangNhap,
            SUM(rHienVat) as rHienVat,
            SUM(rTuChi) as rTuChi,
            SUM(rHangMua) as rHangMua,
            SUM(rPhanCap) as rPhanCap,
            SUM(rDuPhong) as rDuPhong

    FROM    DT_ChungTuChiTiet_PhanCap 
    WHERE   iTrangThai=1 
            AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
            AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach IN (SELECT * FROM F_SplitString2(@iID_MaNamNganSach))
            AND (@iID_MaDonVi IS NULL OR iID_MaDonVi=@iID_MaDonVi)

    
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sXauNoiMa,iID_MaDonVi,sTenDonVi
    HAVING SUM(rTuChi)<>0 
                        
                        
    UNION ALL
    ---- BO SUNG
    SELECT  LEFT(sLNS,1) as sLNS1,
            sLNS3=  CASE    
                    WHEN sLNS=1050100 then '101'
                    else  LEFT(sLNS,3) END, 
            sLNS5=  CASE    
                    WHEN sLNS=1050100 then '10100'
                    WHEN sLNS=1020000 then 10201
                    else  LEFT(sLNS,5) END, 
            sLNS=   CASE    
                    WHEN sLNS=1050100 then '1010000'
                    WHEN sLNS=1020000 then '1020100'
                    else sLNS END,
                    
            sL,sK,sM,sTM,sTTM,sNG,sMoTa,sXauNoiMa,
            iID_MaDonVi,sTenDonVi, 
            SUM(rTuChi) as rBS,
            SUM(rTonKho) as rTonKho,
            SUM(rHangNhap) as rHangNhap,
            SUM(rHienVat) as rHienVat,
            SUM(rTuChi) as rTuChi,
            SUM(rHangMua) as rHangMua,
            SUM(rPhanCap) as rPhanCap,
            SUM(rDuPhong) as rDuPhong

        FROM DTBS_ChungTuChiTiet 
        WHERE  iTrangThai=1 
            AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
            AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach IN (SELECT * FROM F_SplitString2(@iID_MaNamNganSach))
            AND (MaLoai='' OR MaLoai='2')
            AND (@iID_MaDonVi IS NULL OR iID_MaDonVi=@iID_MaDonVi)
            AND (iID_MaChungTu IN (SELECT DISTINCT iID_MaChungTu FROM DTBS_ChungTu WHERE dNgayChungTu <= @dMaDot AND sID_MaNguoiDungTao = @username))
            
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sXauNoiMa,iID_MaDonVi,sTenDonVi
    HAVING SUM(rTuChi)<>0 

    UNION ALL

    SELECT  LEFT(sLNS,1) as sLNS1,
            LEFT(sLNS,3) as sLNS3,
            LEFT(sLNS,5) as sLNS5,            
            sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sXauNoiMa,
            iID_MaDonVi,sTenDonVi,           
            0 as rBS,
            SUM(rTonKho) as rTonKho,
            SUM(rHangNhap) as rHangNhap,
            SUM(rHienVat) as rHienVat,
            SUM(rTuChi) as rTuChi,
            SUM(rHangMua) as rHangMua,
            SUM(rPhanCap) as rPhanCap,
            SUM(rDuPhong) as rDuPhong
    FROM    DTBS_ChungTuChiTiet_PhanCap 
    WHERE   iTrangThai=1 
            AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
            AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach IN (SELECT * FROM F_SplitString2(@iID_MaNamNganSach))
            AND (@iID_MaDonVi IS NULL OR iID_MaDonVi=@iID_MaDonVi)
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sXauNoiMa,iID_MaDonVi,sTenDonVi
    HAVING SUM(rTuChi)<>0 

) as ct

WHERE sLNS1 IN (1,2,3,4)
GROUP BY sLNS1,sLNS3, sLNS5,sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sXauNoiMa,iID_MaDonVi,sTenDonVi
HAVING	SUM(rTuChi)<>0 OR
		SUM(rTonKho)<>0 OR
		SUM(rHienVat)<>0 OR
		SUM(rHangNhap)<>0 OR
		SUM(rHangMua)<>0 OR
		SUM(rDuPhong)<>0 OR
		SUM(rPhanCap)<>0
