
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='10' 
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='51'
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach='1'
declare @username nvarchar(20)						set @username='quynhnl'


--#DECLARE#--

-- du toan
SELECT  

		sLNS1=LEFT(sLNS,1),
        sLNS3=  CASE    
                    WHEN sLNS=1050100 then 101
                    else  LEFT(sLNS,3) END, 
        sLNS5=  CASE    
                    WHEN sLNS=1050100 then 10100
                    WHEN sLNS=1020000 then 10201
                    else  LEFT(sLNS,5) END, 
        sLNS=   CASE    
                    WHEN sLNS=1050100 then 1010000
                    WHEN sLNS=1020000 then 1020100
                    else sLNS END,
        sL,sK,sM,sTM,sTTM,sNG,sMoTa,sXauNoiMa,
        iID_MaDonVi,sTenDonVi, 
		rChuaCap	=SUM(rChuaCap),
		rDaCap		=SUM(rDaCap),
		rTongCong	=SUM(rChuaCap + rDaCap)/@dvt
FROM (

    ---- CHUA CAP TIEN
    SELECT  
        sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sXauNoiMa,
        iID_MaDonVi,sTenDonVi,
		rChuaCap	= SUM(rTuChi + rHienVat + rHangNhap + rHangMua + rDuPhong),
		rDaCap=0

    FROM DTBS_ChungTuChiTiet 
    WHERE  iTrangThai=1 
        AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
        AND iNamLamViec=@iNamLamViec
		AND iID_MaNamNganSach IN (4)
        AND (MaLoai='' OR MaLoai='2')
        AND (@iID_MaDonVi IS NULL OR iID_MaDonVi=@iID_MaDonVi)
        AND (@username IS NULL OR sID_MaNguoiDungTao=@username)
            
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sXauNoiMa,iID_MaDonVi,sTenDonVi
    HAVING SUM(rTuChi)<>0 OR SUM(rTonKho)<>0 OR SUM(rHangNhap)<>0 OR SUM(rHangMua)<>0 OR SUM(rPhanCap)<>0 OR SUM(rDuPhong)<>0

	---- DA CAP TIEN
	UNION ALL
	SELECT  
        sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sXauNoiMa,
        iID_MaDonVi,sTenDonVi,
		rChuaCap	= 0,
		rDaCap		=SUM(rTuChi + rHienVat + rHangNhap + rHangMua + rDuPhong)

    FROM DTBS_ChungTuChiTiet 
    WHERE  iTrangThai=1 
        AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
        AND iNamLamViec=@iNamLamViec
		AND iID_MaNamNganSach in (1,5)
        AND (MaLoai='' OR MaLoai='2')
        AND (@iID_MaDonVi IS NULL OR iID_MaDonVi=@iID_MaDonVi)
        AND (@username IS NULL OR sID_MaNguoiDungTao=@username)
            
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sXauNoiMa,iID_MaDonVi,sTenDonVi
    HAVING SUM(rTuChi)<>0 OR SUM(rHangNhap)<>0 OR SUM(rHangMua)<>0 OR SUM(rPhanCap)<>0 OR SUM(rDuPhong)<>0


   -- UNION ALL

   -- SELECT            
   --         sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sXauNoiMa,
   --         iID_MaDonVi,sTenDonVi,    
			       
   --         rTuChi		=SUM(rTuChi),
   --         rTonKho		=SUM(rTonKho),
   --         rHangNhap	=SUM(rHangNhap),
   --         rHienVat	=SUM(rHienVat),
   --         rHangMua	=SUM(rHangMua),
   --         rPhanCap	=SUM(rPhanCap),
   --         rDuPhong	=SUM(rDuPhong)

   -- FROM    DTBS_ChungTuChiTiet_PhanCap INNER JOIN (select iID_MaChungTu as Id, iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet) as ctct on ctct.iID_MaChungTuChiTiet = iID_MaChungTu
   -- WHERE   iTrangThai=1 
   --         AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
   --         AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach IN (SELECT * FROM F_Split(@iID_MaNamNganSach))
   --         AND (@iID_MaDonVi IS NULL OR iID_MaDonVi=@iID_MaDonVi)
			----AND ctct.Id IN (select * from F_NS_DTBS_ChungTuDaGom(@iNamLamViec,@iID_MaPhongBan))
   -- GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sXauNoiMa,iID_MaDonVi,sTenDonVi
   -- HAVING SUM(rTuChi)<>0 OR SUM(rTonKho)<>0 OR SUM(rHangNhap)<>0 OR SUM(rHangMua)<>0 OR SUM(rPhanCap)<>0 OR SUM(rDuPhong)<>0


) as ct

WHERE	LEFT(sLNS,1) IN (1,2,3,4)
		and LEFT(sLNS,3)  <> '104'
GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sXauNoiMa,iID_MaDonVi,sTenDonVi    
HAVING SUM(rDaCap)<>0 OR SUM(rChuaCap)<>0
