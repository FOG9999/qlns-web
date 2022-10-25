declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2020
declare @username nvarchar(20)						set @username='chuctc'
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07' 
declare @iID_MaDonVi nvarchar(200)					set @iID_MaDonVi='11'

declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach='2,4'
declare @iThang_Quy int								set @iThang_Quy='4'
declare @sLNS nvarchar(20)							set @sLNS=null
declare @dNgay datetime								set @dNgay=GETDATE()


--#DECLARE#--



SELECT 
    LEFT(sLNS,1) as sLNS1,
    LEFT(sLNS,3) as sLNS3,
    LEFT(sLNS,5) as sLNS5,
    sLNS,sL,sK,sM,sTM,sTTM,sNG,
	(CONVERT(nvarchar(10),sLNS)	+'-'+
	 CONVERT(nvarchar(10),sL)	+'-'+
	 CONVERT(nvarchar(10),sK)	+'-'+
	 CONVERT(nvarchar(10),sM)	+'-'+
	 CONVERT(nvarchar(10),sTM)	+'-'+
	 CONVERT(nvarchar(10),sTTM)+'-'+
	 CONVERT(nvarchar(10),sNG))  as sXauNoiMa,
    SUM(ChiTieu)/@dvt as ChiTieu,
    SUM(QuyetToan)/@dvt as QuyetToan
FROM
(
    -- quyet toan 1
    SELECT 
        sLNS,sL,sK,sM,sTM,sTTM,sNG,
        ChiTieu=0,        
		QuyetToan=SUM(rTuChi)
    FROM    QTA_ChungTuChiTiet
    WHERE   iTrangThai=1 
            AND iNamLamViec=@iNamLamViec 
            AND iThang_Quy<=@iThang_Quy
			AND iID_MaPhongBan = @iID_MaPhongBan
            AND iID_MaNamNganSach IN (SELECT * FROM f_split(@iID_MaNamNganSach))
            AND (@sLNS IS NULL OR sLNS IN (SELECT * FROM f_split(@sLNS)))
			AND (iID_MaDonVi in (select * from F_Split(@iID_MaDonVi)))
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi

    UNION ALL

	-- Lấy chỉ tiêu đầu năm
    SELECT
        sLNS,sL,sK,sM,sTM,sTTM,sNG,
		ChiTieu = sum(rTuChi)
		,QuyetToan=0
    FROM    f_ns_chitieu_full_tuchi(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, @iID_MaNamNganSach,NULL,@dNgay,1,null)
    WHERE   (@sLNS IS NULL OR sLNS IN (SELECT * FROM f_split(@sLNS))) and (@iID_MaDonVi like '%,%' or LEN(@iID_MaDonVi) = 2) 
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi	

	UNION ALL

	-- Giảm chỉ tiêu bv dã chuyển về đơn vị cấp 2 (từng dơn vị cấp 2)
    SELECT
        sLNS,sL,sK,sM,sTM,sTTM,sNG,
		ChiTieu = sum(rTuChi)
		,QuyetToan=0
    FROM    f_ns_chitieu_full_tuchi_bv_g(@iNamLamviec, null, @iID_MaPhongBan, @iID_MaNamNganSach,NULL,@dNgay,1,null)
    WHERE   (@sLNS IS NULL OR sLNS IN (SELECT * FROM f_split(@sLNS))) and @iID_MaDonVi not like '%,%' and LEFT(iID_MaDonVi,2) = @iID_MaDonVi and LEN(@iID_MaDonVi) = 2
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi	

	UNION ALL

	-- Lấy chỉ tiêu bv đợt bổ sung
    SELECT
        sLNS,sL,sK,sM,sTM,sTTM,sNG,
		ChiTieu = sum(rTuChi)
		,QuyetToan=0
    FROM    f_ns_chitieu_full_tuchi_bv(@iNamLamviec, @iID_MaDonVi, @iID_MaPhongBan, @iID_MaNamNganSach,NULL,@dNgay,1,null)
    WHERE   (@sLNS IS NULL OR sLNS IN (SELECT * FROM f_split(@sLNS))) and @iID_MaDonVi not like '%,%' and LEN(@iID_MaDonVi) = 3
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi

) a
 

WHERE LEFT(sLNS,1) IN (1,2,3,4) 
--and
--	((sLNS='1010000' and sM='6000' and sTM='6001') or	
--	(sLNS='1010000' and sM='6100' and sTM='6101') or
--	(sLNS='1010000' and sM='6100' and sTM='6115') or
--	(sLNS='1010000' and sM='6400' and sTM='6449' and sTTM='10'))
GROUP BY a.sLNS,sL,sK,sM,sTM,sTTM,sNG
HAVING  SUM(ChiTieu)<>0 OR
        SUM(QuyetToan)<>0
ORDER BY a.sLNS,sL,sK,sM,sTM,sTTM,sNG
