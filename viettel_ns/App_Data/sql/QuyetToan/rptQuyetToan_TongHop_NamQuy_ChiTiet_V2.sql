declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @username nvarchar(20)						set @username='chuctc'
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07' 
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='14'
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach='2,4'
declare @iThang_Quy int								set @iThang_Quy='4'
declare @sLNS nvarchar(20)							set @sLNS=null
declare @dNgay datetime								set @dNgay='2018-12-31 00:00:00.0'

--#DECLARE#--


SELECT 
    LEFT(sLNS,1) as sLNS1,
    LEFT(sLNS,3) as sLNS3,
    LEFT(sLNS,5) as sLNS5,
    sLNS,sL,sK,sM,sTM,sTTM,sNG,--sMoTa,
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
		sLNS=   CASE    
                    --WHEN sLNS=1050100 then 1010000
                    WHEN sLNS=1020000 then 1020100
                    else sLNS END,
        --sLNS,
		sL,sK,sM,sTM,sTTM,sNG,sMoTa,
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
            AND (@iID_MaDonVi IS NULL OR iID_MaDonVi = @iID_MaDonVi)
            
            
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi

    UNION ALL

    -- lay chi tieu
    SELECT
        sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,
        iID_MaDonVi,
		 rTuChi=	CASE    
					WHEN sLNS LIKE '104%' then SUM(rTuChi)
					ELSE SUM(rTuChi) END,

        0 as Quy1,
        0 as Quy2,
        0 as Quy3,
        0 as Quy4
    FROM    f_ns_table_chitieu(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, @iID_MaNamNganSach,@dNgay,1,null)
    WHERE   (@sLNS IS NULL OR sLNS IN (SELECT * FROM f_split(@sLNS))) 
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi

	-- hang nhap

	union all
	 -- lay chi tieu
    SELECT
        sLNS='1040200',sL,sK,sM,sTM,sTTM,sNG,sMoTa,
		iID_MaDonVi,
		rTuChi= SUM(rHangNhap),

        0 as Quy1,
        0 as Quy2,
        0 as Quy3,
        0 as Quy4
    FROM    f_ns_table_chitieu(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, @iID_MaNamNganSach,@dNgay,1, null)
    WHERE   (@sLNS IS NULL OR sLNS IN (SELECT * FROM f_split(@sLNS))) 
			AND sLNS like '104%'
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi


	-- hang mua
	union all
	 -- lay chi tieu
    SELECT
        sLNS='1040300',sL,sK,sM,sTM,sTTM,sNG,sMoTa,
        iID_MaDonVi,
		rTuChi= SUM(rHangMua),

        0 as Quy1,
        0 as Quy2,
        0 as Quy3,
        0 as Quy4
    FROM    f_ns_table_chitieu(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, @iID_MaNamNganSach,@dNgay,1, null)
    WHERE   (@sLNS IS NULL OR sLNS IN (SELECT * FROM f_split(@sLNS))) 
			AND sLNS like '104%'
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,iID_MaDonVi

) a
INNER JOIN  (SELECT iID_MaDonVi as dv_id, (iID_MaDonVi + ' - ' + sTen) as sTenDonVi FROM NS_DonVi WHERE iTrangThai=1 and iNamLamViec_DonVi=@iNamLamViec) as dv 
ON a.iID_MaDonVi=dv.dv_id


WHERE LEFT(sLNS,1) IN (1,2,3,4)
		--and sM='7150' and sTM='7162'
GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi,sTenDonVi
HAVING  SUM(rTuChi)<>0 OR
        SUM(Quy1)<>0 OR
        SUM(Quy2)<>0 OR
        SUM(Quy3)<>0 OR
        SUM(Quy4)<> 0
