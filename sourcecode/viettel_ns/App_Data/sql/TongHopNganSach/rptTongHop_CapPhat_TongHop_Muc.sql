
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @username nvarchar(20)						set @username='chuctc'
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07' 
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='10'
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach='1,2'
declare @iThang_Quy int								set @iThang_Quy='3'
declare @sLNS nvarchar(20)							set @sLNS=null
declare @dNgay datetime								set @dNgay='2018-10-10 00:00:00.0'


--#DECLARE#--



SELECT 
    LEFT(sLNS,1) as sLNS1,
    LEFT(sLNS,3) as sLNS3,
    LEFT(sLNS,5) as sLNS5,
    sLNS,sL,sK,sM,
	sMoTa = dbo.F_MLNS_MoTa_LNS(@iNamLamViec,sLNS,sL,sK,sM,'','',''),  
    SUM(rTuChi) as rTuChi,		
	SUM(rCapPhat) as rCapPhat
FROM
(
    -- quyet toan
    SELECT 
			sLNS,sL,sK,sM,sTM,sTTM,sNG,
			iID_MaDonVi,
			0 as rTuChi,
			sum(rTuChi) as rCapphat
    FROM    CP_CapPhatChiTiet
    WHERE   iTrangThai=1 
            AND iNamLamViec=@iNamLamViec 
			AND (@iID_MaPhongBan IS NULL OR iID_MaPhongBan=@iID_MaPhongBan)
            AND iID_MaNamNganSach IN (SELECT * FROM f_split(@iID_MaNamNganSach))
            AND (@sLNS IS NULL OR sLNS IN (SELECT * FROM f_split(@sLNS)))
            AND (@iID_MaDonVi is null or iID_MaDonVi IN (SELECT * FROM f_split(@iID_MaDonVi)))
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi

	UNION ALL
    
    SELECT
        sLNS,sL,sK,sM,sTM,sTTM,sNG,
        iID_MaDonVi,
        --rTuChi = SUM(rTuChi + rTonKho + rHangNhap + rHangMua + rPhanCap + rDuPhong),
		0 as rTuChi,
        rCapphat = SUM(rTien)
    --FROM    f_ns_table_chitieu(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, @iID_MaNamNganSach,@dNgay,1,null)
    FROM    f_ns_table_chitieu_tien(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, '1',@dNgay,1,null)
	where	1 in (select * from f_split(@iID_MaNamNganSach))
    --WHERE   (@sLNS IS NULL OR sLNS IN (SELECT * FROM f_split(@sLNS))) 
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi

    UNION ALL

    ---- lay chi tieu
    --SELECT
    --    sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,
    --    iID_MaDonVi, sTenDonVi,
    --    rTuChi = SUM(rTuChi + rTonKho + rHangNhap + rHangMua + rPhanCap + rDuPhong),
    --    0 as Quy1,
    --    0 as Quy2,
    --    0 as Quy3,
    --    0 as Quy4
    --FROM    F_NS_ChiTieu_TheoDonVi(@iNamLamviec, @iID_MaDonVi,  @iID_MaPhongBan, @iID_MaNamNganSach,@dNgay,1)
    --WHERE   (@sLNS IS NULL OR sLNS IN (SELECT * FROM f_split(@sLNS)))
    --GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi,sTenDonVi

	  -- lay chi tieu
	  SELECT
        sLNS,sL,sK,sM,sTM,sTTM,sNG,
        iID_MaDonVi,
        --rTuChi = SUM(rTuChi + rTonKho + rHangNhap + rHangMua + rPhanCap + rDuPhong),
		 rTuChi=	 SUM(rTien),
        0 as rCapPhat,
        0 as rQuyetToan
    --FROM    f_ns_table_chitieu(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, @iID_MaNamNganSach,@dNgay,1,null)
    FROM    f_ns_table_chitieu_tien(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, @iID_MaNamNganSach,@dNgay,1,null)
    --WHERE   (@sLNS IS NULL OR sLNS IN (SELECT * FROM f_split(@sLNS))) 
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi
 --   SELECT
	--		sLNS,sL,sK,sM,sTM,sTTM,sNG,
	--		iID_MaDonVi,
	--		rTuChi=	CASE    
	--					WHEN sLNS LIKE '104%' then SUM(rTuChi)
	--					WHEN sLNS = '1020200' then SUM(rTuChi+rHangNhap)
	--					ELSE SUM(rTuChi) END,
	--		0 as rCapPhat

 --   --FROM    F_NS_ChiTieu_TheoDonVi(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, @iID_MaNamNganSach,@dNgay,1)
 --   FROM    f_ns_table_chitieu(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, @iID_MaNamNganSach,@dNgay,@dvt,null)
 --   --FROM    f_ns_chitieu(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, @iID_MaNamNganSach,@dNgay,@dvt,null)
 --   WHERE   (@sLNS IS NULL OR sLNS IN (SELECT * FROM f_split(@sLNS))) 
 --   GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi

	--union all
	-- -- lay chi tieu
 --   SELECT
	--		sLNS='1040200',sL,sK,sM,sTM,sTTM,sNG,
	--		iID_MaDonVi,
	--		rTuChi= SUM(rHangNhap),
	--		0 as rCapPhat
 --   --FROM    F_NS_ChiTieu_TheoDonVi(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, @iID_MaNamNganSach,@dNgay,1)
 --   FROM    f_ns_table_chitieu(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, @iID_MaNamNganSach,@dNgay,@dvt,null)
 --   --FROM    f_ns_chitieu(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, @iID_MaNamNganSach,@dNgay,@dvt,null)
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
 --       0 as rCapPhat
 --   --FROM    F_NS_ChiTieu_TheoDonVi(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, @iID_MaNamNganSach,@dNgay,1)
 --   FROM    f_ns_table_chitieu(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, @iID_MaNamNganSach,@dNgay,@dvt,null)
 --   --FROM    f_ns_chitieu(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, @iID_MaNamNganSach,@dNgay,@dvt,null)
 --   WHERE   (@sLNS IS NULL OR sLNS IN (SELECT * FROM f_split(@sLNS))) 
	--		AND sLNS like '104%'
 --   GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi

) a
INNER JOIN  (SELECT iID_MaDonVi as dv_id, (iID_MaDonVi + ' - ' + sTen) as sTenDonVi FROM NS_DonVi WHERE iTrangThai=1 and iNamLamViec_DonVi=@iNamLamViec) as dv 
ON a.iID_MaDonVi=dv.dv_id

WHERE LEFT(sLNS,1) IN (2,3)
--WHERE LEFT(sLNS,1) IN (1,2,3,4)


GROUP BY a.sLNS,sL,sK,sM
HAVING  SUM(rTuChi)<>0 OR
        SUM(rCapPhat)<>0
ORDER BY a.sLNS,sL,sK,sM,sMoTa
