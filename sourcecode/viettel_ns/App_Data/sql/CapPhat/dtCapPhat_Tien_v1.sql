declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07' 
declare @iID_MaNguonNganSach nvarchar(20)			set @iID_MaNguonNganSach = '1'
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach = '2'
declare @sLNS nvarchar(20)							set @sLNS = '4080300'
declare @dNgayCapPhat datetime						set @dNgayCapPhat = '2/26/2018 12:00:00 AM'

--#DECLARE#--

SELECT		iID_MaDonVi
			, sLNS
			, sL
			, sK			
			, @@MucCap
			, rTuChi	=SUM(rTuChi)
			, rHienVat	=SUM(rHienVat)
            , rTonKho	=SUM(rTonKho)
            , rHangNhap	=SUM(rHangNhap)
            , rHangMua	=SUM(rHangMua)
            , rPhanCap	=SUM(rPhanCap)
            , rDuPhong	=SUM(rDuPhong)
FROM (

			--DU TOAN

    SELECT      
            sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sXauNoiMa,
            iID_MaDonVi,sTenDonVi,
			loai = 1,
            rTuChi		=SUM(rTuChi),
            rHienVat	=SUM(rHienVat),
            rTonKho		=SUM(rTonKho),
            rHangNhap	=SUM(rHangNhap),
            rHangMua	=SUM(rHangMua),
            rPhanCap	=SUM(rPhanCap),
            rDuPhong	=SUM(rDuPhong)


    FROM    DT_ChungTuChiTiet 
    WHERE   iTrangThai=1 
            AND iNamLamViec=@iNamLamViec AND iID_MaNamNganSach  IN (SELECT * FROM f_split(@iID_MaNamNganSach))
            --AND  (MaLoai='' OR MaLoai='2')  

			AND (((sLNS like '104%' and (MaLoai='' OR MaLoai='2') ) or (sLNS not like '104%')))
            AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
			AND (@iID_MaDonVi is null or iID_MaDonVi in (select * from f_split(@iID_MaDonVi)))
    
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sXauNoiMa,iID_MaDonVi,sTenDonVi
    HAVING SUM(rTuChi)<>0 OR SUM(rHienVat)<>0 OR SUM(rTonKho)<>0 OR SUM(rHangNhap)<>0 OR SUM(rHangMua)<>0 OR SUM(rPhanCap)<>0 OR SUM(rDuPhong)<>0

    UNION ALL

    -- DU TOAN - PHAN CAP
    SELECT   
            sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sXauNoiMa,
            iID_MaDonVi,sTenDonVi,
			loai = 1,
            
            rTuChi		=SUM(rTuChi),
            rHienVat	=SUM(rHienVat),
            rTonKho		=SUM(rTonKho),
            rHangNhap	=SUM(rHangNhap),
            rHangMua	=SUM(rHangMua),
            rPhanCap	=SUM(rPhanCap),
            rDuPhong	=SUM(rDuPhong)

    FROM    DT_ChungTuChiTiet_PhanCap 
    WHERE   iTrangThai=1 
            AND iNamLamViec=@iNamLamViec 
			AND iID_MaNamNganSach IN (SELECT * FROM f_split(@iID_MaNamNganSach))
            AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
			AND (@iID_MaDonVi is null or iID_MaDonVi in (select * from f_split(@iID_MaDonVi)))
    
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sXauNoiMa,iID_MaDonVi,sTenDonVi    
	HAVING SUM(rTuChi)<>0 OR SUM(rHienVat)<>0 OR SUM(rTonKho)<>0 OR SUM(rHangNhap)<>0 OR SUM(rHangMua)<>0 OR SUM(rPhanCap)<>0 OR SUM(rDuPhong)<>0
                        
                        
    UNION ALL


    ---- BO SUNG
    SELECT  
            sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sXauNoiMa,
            iID_MaDonVi,sTenDonVi, 
			loai = 2,

            rTuChi		=SUM(rTuChi),
            rHienVat	=SUM(rHienVat),
            rTonKho		=SUM(rTonKho),
            rHangNhap	=SUM(rHangNhap),
            rHangMua	=SUM(rHangMua),
            rPhanCap	=SUM(rPhanCap),
            rDuPhong	=SUM(rDuPhong)

        FROM DTBS_ChungTuChiTiet 
        WHERE  iTrangThai=1 
            AND iNamLamViec=@iNamLamViec 
			AND iID_MaNamNganSach IN (SELECT * FROM f_split(@iID_MaNamNganSach))
            AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
            AND (MaLoai='' OR MaLoai='2')
			AND (@iID_MaDonVi is null or iID_MaDonVi in (select * from f_split(@iID_MaDonVi)))
			AND iID_MaChungTu IN (select * from F_NS_DTBS_ChungTuDaGom_DenDot(@iNamLamViec,@iID_MaPhongBan,@dNgay))
            
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sXauNoiMa,iID_MaDonVi,sTenDonVi
    HAVING SUM(rTuChi)<>0 OR SUM(rHienVat)<>0 OR SUM(rTonKho)<>0 OR SUM(rHangNhap)<>0 OR SUM(rHangMua)<>0 OR SUM(rPhanCap)<>0 OR SUM(rDuPhong)<>0


    UNION ALL

    SELECT            
            sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sXauNoiMa,
            iID_MaDonVi,sTenDonVi,    
			loai = 2,
			       
            rTuChi		=SUM(rTuChi),
            rHienVat	=SUM(rHienVat),
            rTonKho		=SUM(rTonKho),
            rHangNhap	=SUM(rHangNhap),
            rHangMua	=SUM(rHangMua),
            rPhanCap	=SUM(rPhanCap),
            rDuPhong	=SUM(rDuPhong)
	FROM    DTBS_ChungTuChiTiet_PhanCap 
	WHERE   iTrangThai=1 
			AND iNamLamViec=@iNamLamViec
			AND iID_MaPhongBanDich=@iID_MaPhongBan
			AND (@iID_MaDonVi is null or iID_MaDonVi in (select * from f_split(@iID_MaDonVi)))
			AND (
				iID_MaChungTu IN (SELECT iID_MaChungTuChiTiet FROM DTBS_ChungTuChiTiet WHERE iID_MaChungTu IN (SELECT * FROM F_NS_DTBS_ChungTuDaGom_DenDot(@iNamLamViec,@iID_MaPhongBan,@dNgay)))
				
				-- phan cap cho cac bql
				OR iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
										where iID_MaChungTu in (select iID_MaDotNganSach from DTBS_ChungTuChiTiet where iID_MaChungTu in (SELECT * FROM F_NS_DTBS_ChungTuDaGom_DenDot(@iNamLamViec,@iID_MaPhongBan,@dNgay))))

				---- phan cap cho b
				OR iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
									 where iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet where iID_MaChungTu in (SELECT * FROM F_NS_DTBS_ChungTuDaGom_DenDot(@iNamLamViec,@iID_MaPhongBan,@dNgay))))


					-- phan cap lan 2
				OR iID_MaChungTu in (	select iID_MaChungTuChiTiet 
											from DTBS_ChungTuChiTiet 
											where iID_MaChungTu in (
														select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
														where iID_MaChungTu in (   
																				select iID_MaChungTu from DTBS_ChungTu
																				where iID_MaChungTu in (
																										select iID_MaDuyetDuToanCuoiCung from DTBS_ChungTu
																										where iID_MaChungTu in (select * from F_NS_DTBS_ChungTuDaGom_DenDot(@iNamLamViec,@iID_MaPhongBan,@dNgay))))))
				)




    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,sXauNoiMa,iID_MaDonVi,sTenDonVi
    HAVING SUM(rTuChi)<>0 OR SUM(rHienVat)<>0 OR SUM(rTonKho)<>0 OR SUM(rHangNhap)<>0 OR SUM(rHangMua)<>0 OR SUM(rPhanCap)<>0 OR SUM(rDuPhong)<>0 
) as CT
GROUP BY sLNS,sL,sK,@@MucCap,sMoTa,sXauNoiMa,iID_MaDonVi,sTenDonVi		
HAVING SUM(rTuChi)<>0 OR SUM(rHienVat)<>0 OR  SUM(rTonKho)<>0 OR SUM(rHangNhap)<>0 OR SUM(rHangMua)<>0 OR SUM(rPhanCap)<>0 OR SUM(rDuPhong)<>0


