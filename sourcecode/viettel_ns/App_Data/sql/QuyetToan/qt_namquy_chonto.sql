
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @username nvarchar(20)						set @username='chuctc'
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07' 
declare @iID_MaDonVi1 nvarchar(200)					set @iID_MaDonVi1='10'
declare @iID_MaDonVi2 nvarchar(20)					set @iID_MaDonVi2='10'
declare @iID_MaDonVi3 nvarchar(20)					set @iID_MaDonVi3='10'

declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach='1,2,4'
declare @iThang_Quy int								set @iThang_Quy='1'
declare @sLNS nvarchar(20)							set @sLNS=null
declare @dNgay datetime								set @dNgay='2018-03-10 00:00:00.0'


--#DECLARE#--


SELECT 
    LEFT(sLNS,1) as sLNS1,
    LEFT(sLNS,3) as sLNS3,
    LEFT(sLNS,5) as sLNS5,
    sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,
    SUM(ChiTieu1) as ChiTieu1,
    SUM(QuyetToan1) as QuyetToan1,
    SUM(ChiTieu2) as ChiTieu2,
    SUM(QuyetToan2) as QuyetToan2,
    SUM(ChiTieu3) as ChiTieu3,
    SUM(QuyetToan3) as QuyetToan3
FROM
(
    -- quyet toan 1
    SELECT 
        sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,
        ChiTieu1=0,        QuyetToan1=SUM(rTuChi),
		ChiTieu2=0,        QuyetToan2=0,
		ChiTieu3=0,        QuyetToan3=0
    FROM    QTA_ChungTuChiTiet
    WHERE   iTrangThai=1 
            AND iNamLamViec=@iNamLamViec 
            AND iThang_Quy<=@iThang_Quy
            AND iID_MaNamNganSach IN (SELECT * FROM f_split(@iID_MaNamNganSach))
            AND (@sLNS IS NULL OR sLNS IN (SELECT * FROM f_split(@sLNS)))
			AND (iID_MaDonVi in (select * from F_Split(@iID_MaDonVi1)))
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa

    UNION ALL

  -- quyet toan 2
    SELECT 
        sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,
        ChiTieu1=0,       QuyetToan1=0,
		ChiTieu2=0,       QuyetToan2=SUM(rTuChi),
		ChiTieu3=0,       QuyetToan3=0
    FROM    QTA_ChungTuChiTiet
    WHERE   iTrangThai=1 
            AND iNamLamViec=@iNamLamViec 
            AND iThang_Quy<=@iThang_Quy
            AND iID_MaNamNganSach IN (SELECT * FROM f_split(@iID_MaNamNganSach))
            AND (@sLNS IS NULL OR sLNS IN (SELECT * FROM f_split(@sLNS)))
			AND (iID_MaDonVi=@iID_MaDonVi2)
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa

    UNION ALL

	-- quyet toan 3
    SELECT 
        sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,
        ChiTieu1=0,		QuyetToan1=0,
		ChiTieu2=0,     QuyetToan2=0,
		ChiTieu3=0,     QuyetToan3=SUM(rTuChi)
    FROM    QTA_ChungTuChiTiet
    WHERE   iTrangThai=1 
            AND iNamLamViec=@iNamLamViec 
            AND iThang_Quy<=@iThang_Quy
            AND iID_MaNamNganSach IN (SELECT * FROM f_split(@iID_MaNamNganSach))
            AND (@sLNS IS NULL OR sLNS IN (SELECT * FROM f_split(@sLNS)))
			AND (iID_MaDonVi=@iID_MaDonVi3)
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa

    UNION ALL

    ---- lay chi tieu 1
    
	-- lay chi tieu
    SELECT
        sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,
		ChiTieu1=	CASE    
					WHEN sLNS LIKE '104%' then SUM(rTuChi)
					ELSE SUM(rTuChi+rHangMua+rHangNhap) END,

		QuyetToan1=0,
		ChiTieu2=0,     QuyetToan2=0,
		ChiTieu3=0,     QuyetToan3=0
    FROM    F_NS_ChiTieu_TheoDonVi(@iNamLamviec, @iID_MaDonVi1, @iID_MaPhongBan, @iID_MaNamNganSach,@dNgay,1)
    WHERE   (@sLNS IS NULL OR sLNS IN (SELECT * FROM f_split(@sLNS))) 
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa

	union all
	 -- hanh nhap
    SELECT
        sLNS='1040200',sL,sK,sM,sTM,sTTM,sNG,sMoTa,
        ChiTieu1=	SUM(rHangNhap),
		QuyetToan1=0,
		ChiTieu2=0,     QuyetToan2=0,
		ChiTieu3=0,     QuyetToan3=0
    FROM    F_NS_ChiTieu_TheoDonVi(@iNamLamviec, @iID_MaDonVi1, @iID_MaPhongBan, @iID_MaNamNganSach,@dNgay,1)
    WHERE   (@sLNS IS NULL OR sLNS IN (SELECT * FROM f_split(@sLNS))) 
			AND sLNS like '104%'
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa

	union all

	---- hang mua
	SELECT
        sLNS='1040300',sL,sK,sM,sTM,sTTM,sNG,sMoTa,
        ChiTieu1=	SUM(rHangMua),
		QuyetToan1=0,
		ChiTieu2=0,     QuyetToan2=0,
		ChiTieu3=0,     QuyetToan3=0
    FROM    F_NS_ChiTieu_TheoDonVi(@iNamLamviec, @iID_MaDonVi1, @iID_MaPhongBan, @iID_MaNamNganSach,@dNgay,1)
    WHERE   (@sLNS IS NULL OR sLNS IN (SELECT * FROM f_split(@sLNS))) 
			AND sLNS like '104%'
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa


	union
	-- lay chi tieu 2
    SELECT
        sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,
		ChiTieu1=0,		QuyetToan1=0,
		ChiTieu2=CASE    
					WHEN sLNS LIKE '104%' then SUM(rTuChi)
					ELSE SUM(rTuChi+rHangMua+rHangNhap) END,     
		QuyetToan2=0,
		ChiTieu3=0,     QuyetToan3=0
    FROM    F_NS_ChiTieu_TheoDonVi(@iNamLamviec, @iID_MaDonVi2, @iID_MaPhongBan, @iID_MaNamNganSach,@dNgay,1)
    WHERE   (@sLNS IS NULL OR sLNS IN (SELECT * FROM f_split(@sLNS))) 
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa

	union all
	 -- hanh nhap
    SELECT
        sLNS='1040200',sL,sK,sM,sTM,sTTM,sNG,sMoTa,
        ChiTieu1=0,		QuyetToan1=0,
		ChiTieu2=SUM(rHangNhap),     
		QuyetToan2=0,
		ChiTieu3=0,     QuyetToan3=0
    FROM    F_NS_ChiTieu_TheoDonVi(@iNamLamviec, @iID_MaDonVi2, @iID_MaPhongBan, @iID_MaNamNganSach,@dNgay,1)
    WHERE   (@sLNS IS NULL OR sLNS IN (SELECT * FROM f_split(@sLNS))) 
			AND sLNS like '104%'
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa

	union all

	---- hang mua
	SELECT
        sLNS='1040300',sL,sK,sM,sTM,sTTM,sNG,sMoTa,
        ChiTieu1=0,		QuyetToan1=0,
		ChiTieu2=SUM(rHangMua),     
		QuyetToan2=0,
		ChiTieu3=0,     QuyetToan3=0
    FROM    F_NS_ChiTieu_TheoDonVi(@iNamLamviec, @iID_MaDonVi2, @iID_MaPhongBan, @iID_MaNamNganSach,@dNgay,1)
    WHERE   (@sLNS IS NULL OR sLNS IN (SELECT * FROM f_split(@sLNS))) 
			AND sLNS like '104%'
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa


	union
	-- lay chi tieu 3
    SELECT
        sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,
		ChiTieu1=0,		QuyetToan1=0,
		ChiTieu2=0,     QuyetToan2=0,
		ChiTieu3=CASE    
					WHEN sLNS LIKE '104%' then SUM(rTuChi)
					ELSE SUM(rTuChi+rHangMua+rHangNhap) END, 
		QuyetToan3=0
    FROM    F_NS_ChiTieu_TheoDonVi(@iNamLamviec, @iID_MaDonVi3, @iID_MaPhongBan, @iID_MaNamNganSach,@dNgay,1)
    WHERE   (@sLNS IS NULL OR sLNS IN (SELECT * FROM f_split(@sLNS))) 
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa

	union all
	 -- hanh nhap
    SELECT
        sLNS='1040200',sL,sK,sM,sTM,sTTM,sNG,sMoTa,
        ChiTieu1=0,		QuyetToan1=0,
		ChiTieu2=0,     QuyetToan2=0,
		ChiTieu3=sum(rHangNhap),     QuyetToan3=0
    FROM    F_NS_ChiTieu_TheoDonVi(@iNamLamviec, @iID_MaDonVi3, @iID_MaPhongBan, @iID_MaNamNganSach,@dNgay,1)
    WHERE   (@sLNS IS NULL OR sLNS IN (SELECT * FROM f_split(@sLNS))) 
			AND sLNS like '104%'
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa

	union all

	---- hang mua
	SELECT
        sLNS='1040300',sL,sK,sM,sTM,sTTM,sNG,sMoTa,
        ChiTieu1=0,		QuyetToan1=0,
		ChiTieu2=0,     QuyetToan2=0,
		ChiTieu3=SUM(rHangMua),     QuyetToan3=0
    FROM    F_NS_ChiTieu_TheoDonVi(@iNamLamviec, @iID_MaDonVi3, @iID_MaPhongBan, @iID_MaNamNganSach,@dNgay,1)
    WHERE   (@sLNS IS NULL OR sLNS IN (SELECT * FROM f_split(@sLNS))) 
			AND sLNS like '104%'
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa

) a
 

WHERE LEFT(sLNS,1) IN (1,2,3,4)


GROUP BY a.sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa
HAVING  SUM(ChiTieu1)<>0 OR
        SUM(ChiTieu2)<>0 OR
        SUM(ChiTieu3)<>0 OR
        SUM(QuyetToan1)<>0 OR
        SUM(QuyetToan2)<> 0 OR
        SUM(QuyetToan3)<> 0
ORDER BY a.sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa
