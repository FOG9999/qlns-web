
/*
	
	author:	longsam
	date:	19/04/2018
	decs:	Lấy chỉ tiêu ngân sách 
	params:	loai	1. đầu năm, 2. bổ sung, null. tất cả

*/



alter FUNCTION [dbo].[f_chitieu]
	(
		
	@iNamLamViec int,
	@iID_MaDonVi nvarchar(200),
	@iID_MaPhongBan nvarchar(10),
	@iID_MaNamNganSach nvarchar(10),
	@iID_MaNguonNganSach nvarchar(10),
	@sLNS nvarchar(200),
	@dNgay datetime,
	@dvt int,
	@loai nvarchar(10)
	)

/*
TEST

-- theo don vi

SELECT * FROM F_NS_ChiTieu_TheoDonVi(2018, NULL, '07', '2', null)

-- tat ca don vi

SELECT * FROM F_NS_ChiTieu(2018, '29', '07', '2', null,1,'1')
where rHienVat<>0

-- theo nam ngan sach
SELECT * FROM F_NS_ChiTieu_TheoDonVi(2018, '44', '07', '1,2')

*/ 


RETURNS TABLE 
AS
	RETURN

--declare @iID_MaChungTu nvarchar(MAX)
--select @iID_MaChungTu = coalesce(@iID_MaChungTu + ';', '') + id from F_NS_DTBS_ChungTuDaGom_DenDot(@iNamLamViec,@iID_MaPhongBan,@dNgay)

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
        sL,sK,sM,sTM,sTTM,sNG,
		sXauNoiMa = sLNS +'-'+ sL +'-' + sK + '-'+sM + '-' + sTM + '-' + sTTM + '-' + sNG,
		sMoTa,
        iID_MaDonVi,sTenDonVi, 

        rTuChi		=SUM(rTuChi)/@dvt,
        rHienVat	=SUM(rHienVat)/@dvt,
        rTonKho		=SUM(rTonKho)/@dvt,
        rHangNhap	=SUM(rHangNhap)/@dvt,
        rHangMua	=SUM(rHangMua)/@dvt,
        rPhanCap	=SUM(rPhanCap)/@dvt,
        rDuPhong	=SUM(rDuPhong)/@dvt
FROM (

     --DU TOAN

    SELECT      
            sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,
            iID_MaDonVi,
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
            AND iNamLamViec=@iNamLamViec 
			AND iID_MaNamNganSach  IN (SELECT * FROM f_split(@iID_MaNamNganSach))
			AND iID_MaNguonNganSach  IN (SELECT * FROM f_split(@iID_MaNguonNganSach))
            --AND  (MaLoai='' OR MaLoai='2')  

			AND (((sLNS like '104%' and (MaLoai='' OR MaLoai='2') ) or (sLNS not like '104%')))
            AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
			AND (@iID_MaDonVi is null or iID_MaDonVi in (select * from f_split(@iID_MaDonVi)))
    
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi,sTenDonVi
    HAVING SUM(rTuChi)<>0 OR SUM(rHienVat)<>0 OR SUM(rTonKho)<>0 OR SUM(rHangNhap)<>0 OR SUM(rHangMua)<>0 OR SUM(rPhanCap)<>0 OR SUM(rDuPhong)<>0

    UNION ALL

    -- DU TOAN - PHAN CAP
    SELECT   
            sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,
            iID_MaDonVi,
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
			AND iID_MaNguonNganSach  IN (SELECT * FROM f_split(@iID_MaNguonNganSach))
            AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
			AND (@iID_MaDonVi is null or iID_MaDonVi in (select * from f_split(@iID_MaDonVi)))
    
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi,sTenDonVi    
	HAVING SUM(rTuChi)<>0 OR SUM(rHienVat)<>0 OR SUM(rTonKho)<>0 OR SUM(rHangNhap)<>0 OR SUM(rHangMua)<>0 OR SUM(rPhanCap)<>0 OR SUM(rDuPhong)<>0
                        
                        
    UNION ALL


    ---- BO SUNG
    SELECT  
            sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,
            iID_MaDonVi, 
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
			AND iID_MaNguonNganSach  IN (SELECT * FROM f_split(@iID_MaNguonNganSach))
            AND (@iID_MaPhongBan is NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)  
            AND (MaLoai='' OR MaLoai='2')
			AND (@iID_MaDonVi is null or iID_MaDonVi in (select * from f_split(@iID_MaDonVi)))
			--AND		iID_MaChungTu IN (select * from f_ns_dtbs_chungtugom_dendot(@iNamLamViec,@iID_MaNamNganSach,@iID_MaPhongBan,@dNgay))
            AND ( 
					(iID_MaNamNganSach=2 and iID_MaChungTu IN (select * from f_ns_dtbs_chungtugom_dendot(@iNamLamViec,@iID_MaNamNganSach,@iID_MaPhongBan,@dNgay))) or
					iID_MaNamNganSach in (1,3,4,5)
				)
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi,sTenDonVi
    HAVING SUM(rTuChi)<>0 OR SUM(rHienVat)<>0 OR SUM(rTonKho)<>0 OR SUM(rHangNhap)<>0 OR SUM(rHangMua)<>0 OR SUM(rPhanCap)<>0 OR SUM(rDuPhong)<>0


    UNION ALL

    SELECT            
            sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,
            iID_MaDonVi,    
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
			AND iBKhac='0'
			AND (
				iID_MaChungTu IN (SELECT iID_MaChungTuChiTiet FROM DTBS_ChungTuChiTiet WHERE iID_MaChungTu IN (SELECT * FROM f_ns_dtbs_chungtugom_dendot(@iNamLamViec,@iID_MaNamNganSach,@iID_MaPhongBan,@dNgay)))
				
				-- phan cap cho cac bql
				OR iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
										where iID_MaChungTu in (select iID_MaDotNganSach from DTBS_ChungTuChiTiet where iID_MaChungTu in (SELECT * FROM f_ns_dtbs_chungtugom_dendot(@iNamLamViec,@iID_MaNamNganSach,@iID_MaPhongBan,@dNgay))))

				---- phan cap cho b
				OR iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
									 where iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet where iID_MaChungTu in (SELECT * FROM f_ns_dtbs_chungtugom_dendot(@iNamLamViec,@iID_MaNamNganSach,@iID_MaPhongBan,@dNgay))))


					-- phan cap lan 2
				OR iID_MaChungTu in (	select iID_MaChungTuChiTiet 
											from DTBS_ChungTuChiTiet 
											where iID_MaChungTu in (
														select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
														where iID_MaChungTu in (   
																				select iID_MaChungTu from DTBS_ChungTu
																				where iID_MaChungTu in (
																										select iID_MaDuyetDuToanCuoiCung from DTBS_ChungTu
																										where iID_MaChungTu in (select * from f_ns_dtbs_chungtugom_dendot(@iNamLamViec,@iID_MaNamNganSach,@iID_MaPhongBan,@dNgay))))))


				OR 
				iID_MaChungTu in (	select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
										where iID_MaChungTu in (   
																select iID_MaChungTu from DTBS_ChungTu
																where iID_MaChungTu in (
																					select iID_MaDuyetDuToanCuoiCung from DTBS_ChungTu
																					where iID_MaChungTu in (select * from f_ns_dtbs_chungtugom_dendot(@iNamLamViec,@iID_MaNamNganSach,@iID_MaPhongBan,@dNgay)))))
				
				
				)




    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi,sTenDonVi
    HAVING SUM(rTuChi)<>0 OR SUM(rHienVat)<>0 OR SUM(rTonKho)<>0 OR SUM(rHangNhap)<>0 OR SUM(rHangMua)<>0 OR SUM(rPhanCap)<>0 OR SUM(rDuPhong)<>0


) as ct
left join (select iID_MaDonVi as donvi_id, (iID_MaDonVi + ' - ' + sTen) as sTenDonVi from NS_DonVi where iTrangThai=1 and iNamLamViec_DonVi=@iNamLamViec) as donvi on  ct.iID_MaDonVi=donvi.donvi_id

WHERE	
		((@sLNS is null and (LEFT(sLNS,1) in (1,2,3,4)) 
			or 
				left(sLNS,1) IN (select * from f_split(@sLNS))
				or left(sLNS,3) IN (select * from f_split(@sLNS))
				or left(sLNS,7) IN (select * from f_split(@sLNS))
				))
		--and sLNS='1040100'
		and (@loai is null or loai in (select * from f_split(@loai)))
GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi,sTenDonVi
HAVING SUM(rTuChi)<>0 OR SUM(rHienVat)<>0 OR  SUM(rTonKho)<>0 OR SUM(rHangNhap)<>0 OR SUM(rHangMua)<>0 OR SUM(rPhanCap)<>0 OR SUM(rDuPhong)<>0
