
declare @dvt int									set @dvt = 1
declare @iNamLamViec int							set @iNamLamViec = 2019
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='10' 
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='51'
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach='1,5'
declare @username nvarchar(20)						set @username=null


--#DECLARE#--

-- du toan
SELECT  

		
        sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,
        iID_MaDonVi,sTenDonVi, 

        rTuChi		=SUM(rTuChi)/@dvt,
        --rTonKho		=SUM(rTonKho)/@dvt,
        rHangNhap	=SUM(rHangNhap)/@dvt,
        rHienVat	=SUM(rHienVat)/@dvt,
        rHangMua	=SUM(rHangMua)/@dvt,
        --rPhanCap	=SUM(rPhanCap)/@dvt,
        rDuPhong	=SUM(rDuPhong)/@dvt,
		rTongCong	=SUM(rTuChi + rHangNhap+rHangMua+rDuPhong)/@dvt
FROM (

 --   -- DU TOAN
 --   SELECT      
             
                    
 --           sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sXauNoiMa,
 --           iID_MaDonVi,sTenDonVi,

 --           rTuChi		=SUM(rTuChi),
 --           rTonKho		=SUM(rTonKho),
 --           rHangNhap	=SUM(rHangNhap),
 --           rHienVat	=SUM(rHienVat),
 --           rHangMua	=SUM(rHangMua),
 --           rPhanCap	=SUM(rPhanCap),
 --           rDuPhong	=SUM(rDuPhong)


 --   FROM    DT_ChungTuChiTiet 
 --   WHERE   iTrangThai=1 
 --           AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
 --           AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach  IN (SELECT * FROM F_Split(@iID_MaNamNganSach))
 --           AND  (MaLoai='' OR MaLoai='2') 
 --           AND (@iID_MaDonVi IS NULL OR iID_MaDonVi=@iID_MaDonVi)
    
 --   GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sXauNoiMa,iID_MaDonVi,sTenDonVi
 --   HAVING SUM(rTuChi)<>0 OR SUM(rTonKho)<>0 OR SUM(rHangNhap)<>0 OR SUM(rHangMua)<>0 OR SUM(rPhanCap)<>0 OR SUM(rDuPhong)<>0
            
 --   UNION ALL
 --   -- DU TOAN - PHAN CAP
 --   SELECT   
 --           sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sXauNoiMa,
 --           iID_MaDonVi,sTenDonVi,
            
 --           rTuChi		=SUM(rTuChi),
 --           rTonKho		=SUM(rTonKho),
 --           rHangNhap	=SUM(rHangNhap),
 --           rHienVat	=SUM(rHienVat),
 --           rHangMua	=SUM(rHangMua),
 --           rPhanCap	=SUM(rPhanCap),
 --           rDuPhong	=SUM(rDuPhong)

 --   FROM    DT_ChungTuChiTiet_PhanCap 
 --   WHERE   iTrangThai=1 
 --           AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
 --           AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach IN (SELECT * FROM F_Split(@iID_MaNamNganSach))
 --           AND (@iID_MaDonVi IS NULL OR iID_MaDonVi=@iID_MaDonVi)

    
 --   GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sXauNoiMa,iID_MaDonVi,sTenDonVi    
	--HAVING SUM(rTuChi)<>0 OR SUM(rTonKho)<>0 OR SUM(rHangNhap)<>0 OR SUM(rHangMua)<>0 OR SUM(rPhanCap)<>0 OR SUM(rDuPhong)<>0

                        
                        
 --   UNION ALL

    ---- BO SUNG
    SELECT  
			sLNS1=LEFT(sLNS,1),
			sLNS3=  CASE    
						WHEN sLNS=1050100 then 101
						--WHEN sLNS=1040200 or sLNS=1040300 then 104
						else  LEFT(sLNS,3) END, 
			sLNS5=  CASE    
						WHEN sLNS=1050100 then 10100
						WHEN sLNS=1020000 then 10201
						--WHEN sLNS=1040200 or sLNS=1040300 then 10401
						else  LEFT(sLNS,5) END, 
			sLNS=   CASE    
						WHEN sLNS=1050100 then 1010000
						WHEN sLNS=1020000 then 1020100
						--WHEN sLNS=1040200 or sLNS=1040300 then 1040100
						else sLNS END,
            --sLNS,
			sL,sK,sM,sTM,sTTM,sNG,sMoTa,
            iID_MaDonVi,sTenDonVi,

            rTuChi		=SUM(rTuChi),
            --rTonKho		=SUM(rTonKho),
            rHangNhap	=SUM(rHangNhap),
            rHienVat	=SUM(rHienVat),
            rHangMua	=SUM(rHangMua),
            --rPhanCap	=SUM(rPhanCap),
            rDuPhong	=SUM(rDuPhong)

        FROM DTBS_ChungTuChiTiet 
        WHERE  iTrangThai=1 
            AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
            AND iNamLamViec=@iNamLamViec 
			AND iID_MaNamNganSach IN (SELECT * FROM F_Split(@iID_MaNamNganSach))
            AND (MaLoai='' OR MaLoai='2')
            AND (@iID_MaDonVi IS NULL OR iID_MaDonVi=@iID_MaDonVi)
            AND (@username IS NULL OR sID_MaNguoiDungTao=@username)
			--AND iID_MaChungTu IN (select * from F_NS_DTBS_ChungTuDaGom(@iNamLamViec,@iID_MaPhongBan))
            
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi,sTenDonVi
    HAVING SUM(rTuChi)<>0 OR SUM(rTonKho)<>0 OR SUM(rHangNhap)<>0 OR SUM(rHangMua)<>0 OR SUM(rPhanCap)<>0 OR SUM(rDuPhong)<>0


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

WHERE --LEFT(sLNS,1) IN (1,2,3,4)
		LEFT(sLNS,3)='104'
GROUP BY sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi,sTenDonVi    
HAVING SUM(rTuChi)<>0 OR SUM(rHangNhap)<>0 OR SUM(rHangMua)<>0 OR SUM(rDuPhong)<>0
