

declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @username nvarchar(20)						set @username='chuctc'
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='10' 
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='51'
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach='2,4'
declare @iThang_Quy int								set @iThang_Quy='1'
declare @sLNS nvarchar(20)							set @sLNS=null
declare @dNgay datetime								set @dNgay=GETDATE()



--#DECLARE#--




SELECT 
    LEFT(sLNS,1) as sLNS1,
    LEFT(sLNS,3) as sLNS3,
    LEFT(sLNS,5) as sLNS5,
    sLNS,sL,sK,sM,sTM,sTTM,sNG,
	--sMoTa = dbo.F_MLNS_MoTa_LNS(@iNamLamViec,sLNS,sL,sK,sM,sTM,sTTM,sNG),  
    iID_MaDonVi,
    sTenDonVi,
    SUM(rTuChi) as rTuChi,
    SUM(rCapPhat) as rCapPhat,
    SUM(rQuyetToan) as rQuyetToan
FROM
(
	
	---- cap phat ngan sach dac biet
	--SELECT
        

	--	sLNS,sL,sK,sM,sTM,sTTM,sNG,
 --       iID_MaDonVi,
 --       --rTuChi = SUM(rTuChi + rTonKho + rHangNhap + rHangMua + rPhanCap + rDuPhong),
	--	 rTuChi=	CASE    
	--				WHEN sLNS LIKE '104%' then SUM(rTuChi)
	--				WHEN sLNS = '1020200' then SUM(rTuChi+rHangNhap)
	--				ELSE SUM(rTuChi) END,
 --       0 as rCapPhat
 --   FROM    f_ns_chitieu(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, @iID_MaNamNganSach,@dNgay,1,null)
 --   WHERE   sLNS like '3%'
 --   GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi

	--union all

    -- da cap phat
    SELECT 
        sLNS,sL,sK,sM,sTM,sTTM,sNG,
        iID_MaDonVi,
        0 as rTuChi,
		sum(rTuChi) as rCapPhat,
		0 as rQuyetToan
    FROM    CP_CapPhatChiTiet
    WHERE   iTrangThai=1 
            AND iNamLamViec=@iNamLamViec 
			AND (@iID_MaPhongBan IS NULL OR iID_MaPhongBan=@iID_MaPhongBan)
			AND (@sLNS IS NULL OR sLNS IN (SELECT * FROM f_split(@sLNS))) 
            --AND iThang_Quy<=@iThang_Quy
            AND iID_MaNamNganSach IN (SELECT * FROM f_split(@iID_MaNamNganSach))
            AND (@iID_MaDonVi IS NULL OR iID_MaDonVi in (select * from f_split(@iID_MaDonVi)))
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi

	UNION ALL

	SELECT
        sLNS,sL,sK,sM,sTM,sTTM,sNG,
        iID_MaDonVi,
        --rTuChi = SUM(rTuChi + rTonKho + rHangNhap + rHangMua + rPhanCap + rDuPhong),
		0 as rTuChi,
        rCapPhat = SUM(rTien),
        0 as rQuyetToan
    --FROM    f_ns_table_chitieu(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, @iID_MaNamNganSach,@dNgay,1,null)
    FROM    f_ns_table_chitieu_tien(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, '1',@dNgay,1,null)
	where	1 in (select * from f_split(@iID_MaNamNganSach))
    --WHERE   (@sLNS IS NULL OR sLNS IN (SELECT * FROM f_split(@sLNS))) 
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi

	-- quyet toan
    UNION ALL

	SELECT 
        sLNS,sL,sK,sM,sTM,sTTM,sNG,
        iID_MaDonVi,
        0 as rTuChi,
		0 as rCapPhat,
		sum(rTuChi) as rQuyetToan
    FROM    QTA_ChungTuChiTiet
    WHERE   iTrangThai=1 
            AND iNamLamViec=@iNamLamViec 
			AND (@iID_MaPhongBan IS NULL OR iID_MaPhongBan=@iID_MaPhongBan)
			AND (@sLNS IS NULL OR sLNS IN (SELECT * FROM f_split(@sLNS))) 
            --AND iThang_Quy<=@iThang_Quy
            AND iID_MaNamNganSach IN (SELECT * FROM f_split(@iID_MaNamNganSach))
            AND (@iID_MaDonVi IS NULL OR iID_MaDonVi in (select * from f_split(@iID_MaDonVi)))
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi


    UNION ALL

    -- lay chi tieu
    SELECT
        sLNS,sL,sK,sM,sTM,sTTM,sNG,
        iID_MaDonVi,
        --rTuChi = SUM(rTuChi + rTonKho + rHangNhap + rHangMua + rPhanCap + rDuPhong),
		-- rTuChi=	CASE    
		--			WHEN sLNS LIKE '104%' then SUM(rTuChi)
		--			WHEN sLNS = '1020200' then SUM(rTuChi+rHangNhap)
		--			ELSE SUM(rTuChi) END,
  --      0 as rCapPhat,
		--0 as rQuyetToan
		rTuChi=	 SUM(rTien),
        0 as rCapPhat,
        0 as rQuyetToan

    FROM    f_ns_table_chitieu_tien(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, @iID_MaNamNganSach,@dNgay,1,null)
    --WHERE   (@sLNS IS NULL OR sLNS IN (SELECT * FROM f_split(@sLNS))) 
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi

	---- hang nhap

	--union all
	-- -- lay chi tieu
 --   SELECT
 --       sLNS='1040200',sL,sK,sM,sTM,sTTM,sNG,
	--	iID_MaDonVi,
	--	rTuChi= SUM(rHangNhap),
 --       0 as rCapPhat,
	--	0 as rQuyetToan


 --   FROM    f_ns_chitieu(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, @iID_MaNamNganSach,@dNgay,1,null)
 --   WHERE   (@sLNS IS NULL OR sLNS IN (SELECT * FROM f_split(@sLNS))) 
	--		AND sLNS like '104%'
 --   GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi


	---- hang mua
	--union all
	-- -- lay chi tieu
 --   SELECT
 --       sLNS='1040300',sL,sK,sM,sTM,sTTM,sNG,
 --       iID_MaDonVi,
	--	rTuChi= SUM(rHangMua),
 --       0 as rCapPhat,
	--	0 as rQuyetToan

 --   FROM    f_ns_chitieu(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, @iID_MaNamNganSach,@dNgay,1,null)
 --   WHERE   (@sLNS IS NULL OR sLNS IN (SELECT * FROM f_split(@sLNS))) 
	--		AND sLNS like '104%'
 --   GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi

) a
INNER JOIN  (SELECT iID_MaDonVi as dv_id, (iID_MaDonVi + ' - ' + sTen) as sTenDonVi FROM NS_DonVi WHERE iTrangThai=1 and iNamLamViec_DonVi=@iNamLamViec) as dv 
ON a.iID_MaDonVi=dv.dv_id


--WHERE LEFT(sLNS,1) IN (2,3)
WHERE LEFT(sLNS,1) IN (1,2,3,4)
GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi,sTenDonVi
HAVING  SUM(rTuChi)<>0 OR
        SUM(rCapPhat)<>0 OR
        SUM(rQuyetToan)<>0 
