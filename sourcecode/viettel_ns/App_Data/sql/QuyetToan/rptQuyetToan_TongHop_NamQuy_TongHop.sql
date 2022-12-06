
declare @dvt int									set @dvt = 1
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @username nvarchar(20)						set @username='chuctc'
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07' 
--declare @iID_MaDonVi nvarchar(MAX)					set @iID_MaDonVi='10,11,12,13,14,15,17,19,21,22,23,24,29,31,33,41,42,43,44,45,46,47,81,82,83,84,87,95,96,97,98'
declare @iID_MaDonVi nvarchar(MAX)					set @iID_MaDonVi=null
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach='1,2,4,5'
declare @iThang_Quy int								set @iThang_Quy='1'
declare @sLNS nvarchar(20)							set @sLNS=null
declare @dNgay datetime								set @dNgay=GETDATE()


--###--



SELECT 
    LEFT(sLNS,1) as sLNS1,
    LEFT(sLNS,3) as sLNS3,
    LEFT(sLNS,5) as sLNS5,
    sLNS,sL,sK,sM,sTM,sTTM,sNG,
	sMoTa = dbo.F_MLNS_MoTa_LNS(@iNamLamViec,sLNS,sL,sK,sM,sTM,sTTM,sNG),
    iID_MaDonVi,
	sTenDonVi,
    SUM(rTuChi)/@dvt as rTuChi,
    SUM(Quy1)/@dvt as Quy1,
    SUM(Quy2)/@dvt as Quy2,
    SUM(Quy3)/@dvt as Quy3,
    SUM(Quy4)/@dvt as Quy4
FROM
(
    -- quyet toan
    SELECT 
        sLNS,sL,sK,sM,sTM,sTTM,sNG,
        iID_MaDonVi,
        rTuChi=0,
        Quy1=SUM(CASE WHEN (iThang_Quy=1) THEN rTuChi ELSE 0 END),
        Quy2=SUM(CASE WHEN (iThang_Quy=2) THEN rTuChi ELSE 0 END),
        Quy3=SUM(CASE WHEN (iThang_Quy=3) THEN rTuChi ELSE 0 END),
        Quy4=SUM(CASE WHEN (iThang_Quy=4) THEN rTuChi ELSE 0 END)
    FROM    QTA_ChungTuChiTiet
    WHERE   iTrangThai=1 
            AND iNamLamViec=@iNamLamViec 
			AND (@iID_MaPhongBan IS NULL OR iID_MaPhongBan=@iID_MaPhongBan)
            AND iThang_Quy<=@iThang_Quy
            AND iID_MaNamNganSach IN (SELECT * FROM f_split(@iID_MaNamNganSach))
            AND (@sLNS IS NULL OR sLNS IN (SELECT * FROM f_split(@sLNS)))
            AND (@iID_MaDonVi is null or iID_MaDonVi IN (SELECT * FROM f_split(@iID_MaDonVi)))
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
		--sXauNoiMa=sLNS+'-'+sL+'-'+sK+'-'+sM+'-'+sTM+'-'+sTTM+'-'+sNG,
        iID_MaDonVi,
        rTuChi = SUM(rTuChi),
		 --rTuChi=	CASE    
			--		WHEN sLNS LIKE '104%' then SUM(rTuChi)
			--		ELSE SUM(rTuChi) END,

        0 as Quy1,
        0 as Quy2,
        0 as Quy3,
        0 as Quy4
    --FROM    f_ns_table_chitieu(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, @iID_MaNamNganSach,@dNgay,1,null)
    FROM    f_ns_chitieu_full_tuchi(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, @iID_MaNamNganSach,null,@dNgay,1,null)
    --FROM    F_NS_ChiTieu_TheoDonVi(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, @iID_MaNamNganSach,@dNgay,1)
    WHERE   (@sLNS IS NULL OR sLNS IN (SELECT * FROM f_split(@sLNS))) 
			--AND LEFT(sLNS,1) IN (1) 
			--and ((sTM = '6149' and sTTM = '20' and sNG = '00') or (sTM = '6449' and sTTM = '80' and sNG = '00'))
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi

	--union all
	-- -- lay chi tieu
 --   SELECT
 --       sLNS='1040200',sL,sK,sM,sTM,sTTM,sNG,
	--	sXauNoiMa=sLNS+'-'+sL+'-'+sK+'-'+sM+'-'+sTM+'-'+sTTM+'-'+sNG, 
 --       iID_MaDonVi,
	--	rTuChi= SUM(rHangNhap),

 --       0 as Quy1,
 --       0 as Quy2,
 --       0 as Quy3,
 --       0 as Quy4
 --   --FROM    f_ns_table_chitieu(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, @iID_MaNamNganSach,@dNgay,1,null)
 --   FROM    f_ns_chitieu_full(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, @iID_MaNamNganSach,null,@dNgay,1,null)
 --   --FROM    F_NS_ChiTieu_TheoDonVi(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, @iID_MaNamNganSach,@dNgay,1)
 --   WHERE   (@sLNS IS NULL OR sLNS IN (SELECT * FROM f_split(@sLNS))) 
	--		AND sLNS like '104%'
	--		and rHangNhap<>0
 --   GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi


	---- hang mua
	--union all
	-- -- lay chi tieu
 --   SELECT
 --       sLNS='1040300',sL,sK,sM,sTM,sTTM,sNG,
	--	sXauNoiMa=sLNS+'-'+sL+'-'+sK+'-'+sM+'-'+sTM+'-'+sTTM+'-'+sNG, 
 --       iID_MaDonVi,
	--	rTuChi= SUM(rHangMua),

 --       0 as Quy1,
 --       0 as Quy2,
 --       0 as Quy3,
 --       0 as Quy4
 --   --FROM    F_NS_ChiTieu_TheoDonVi(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, @iID_MaNamNganSach,@dNgay,1)
 --   --FROM    f_ns_table_chitieu(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, @iID_MaNamNganSach,@dNgay,1,null)
 --   FROM    f_ns_chitieu_full(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, @iID_MaNamNganSach,null,@dNgay,1,null)
 --   WHERE   (@sLNS IS NULL OR sLNS IN (SELECT * FROM f_split(@sLNS))) 
	--		AND sLNS like '104%'
	--		and rHangMua<>0
 --   GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi

) a

-- donvi
INNER JOIN  (SELECT iID_MaDonVi as dv_id, (iID_MaDonVi + ' - ' + sTen) as sTenDonVi FROM NS_DonVi WHERE iTrangThai=1 and iNamLamViec_DonVi=@iNamLamViec) as dv 
ON a.iID_MaDonVi=dv.dv_id


---- mota mlns
--INNER join (select convert(nvarchar(32), sXauNoiMa) as id,sMoTa from NS_MucLucNganSach where iTrangThai=1 and iNamLamViec=@iNamLamViec and sLNS<>'' and sNG<>'') as mlns 
--on mlns.id = a.sXauNoiMa


--where sLNS = '1010000' and sM in ('6000','6100')

GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi,sTenDonVi
HAVING  SUM(rTuChi)<>0 OR
        SUM(Quy1)<>0 OR
        SUM(Quy2)<>0 OR
        SUM(Quy3)<>0 OR
        SUM(Quy4)<>0
ORDER BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi
