
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2019
declare @username nvarchar(20)						set @username='chuctc'
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07' 
declare @iID_MaDonVi1 nvarchar(200)					set @iID_MaDonVi1='10'

declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach='1,2,4'
declare @iThang_Quy int								set @iThang_Quy='1'
declare @sLNS nvarchar(20)							set @sLNS=null
declare @dNgay datetime								set @dNgay='2018-03-10 00:00:00.0'


--#DECLARE#--

DECLARE @TempTable TABLE(
	sLNS1 nvarchar(50),
	sLNS3 nvarchar(50),	
	sLNS5 nvarchar(50),	  
	sLNS nvarchar(50),
    sL nvarchar(50),
	sK nvarchar(50),
	sM nvarchar(50),
	sTM nvarchar(50),
	sTTM nvarchar(50),
	sNG nvarchar(50),
	sMoTa nvarchar(max),
	sXauNoiMa nvarchar(max),
    iID_MaDonVi nvarchar(5),
	sTenDonVi nvarchar(200), 
	iID_MaPhongBan nvarchar(10),
	rTuChi decimal(18,0),
	rHienVat decimal(18,0),
	rTonKho decimal(18,0),
	rHangNhap decimal(18,0),
	rHangMua decimal(18,0),
	rPhanCap decimal(18,0),
	rDuPhong decimal(18,0)
)

INSERT INTO @TempTable
SELECT sLNS1,
		sLNS3,	
		sLNS5,	  
		sLNS,
		sL,
		sK,
		sM,
		sTM,
		sTTM,
		sNG,
		sMoTa,
		sXauNoiMa,
		iID_MaDonVi,
		sTenDonVi, 
		iID_MaPhongBan,
		rTuChi,
		rHienVat,
		rTonKho,
		rHangNhap,
		rHangMua,
		rPhanCap,
		rDuPhong 
from	f_ns_chitieu_theodonvi_bql(@iNamLamviec, @iID_MaNganSach, @iID_MaDonVi, @iID_MaPhongBan, @iID_MaNamNganSach,@dNgay,1)

SELECT 
    LEFT(sLNS,1) as sLNS1,
    LEFT(sLNS,3) as sLNS3,
    LEFT(sLNS,5) as sLNS5,
    sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,
	(CONVERT(nvarchar(7),sLNS)	+'-'+
	 CONVERT(nvarchar(3),sL)	+'-'+
	 CONVERT(nvarchar(3),sK)	+'-'+
	 CONVERT(nvarchar(4),sM)	+'-'+
	 CONVERT(nvarchar(4),sTM)	+'-'+
	 CONVERT(nvarchar(2),sTTM)+'-'+
	 CONVERT(nvarchar(2),sNG))  as sXauNoiMa,
    SUM(CTB7) as CTB7,
    SUM(CTB10) as CTB10
FROM
(
    ---- lay chi tieu b7
    
	-- lay chi tieu
    SELECT
        sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,
		CTB7=	CASE    
					WHEN sLNS LIKE '104%' then SUM(rTuChi)
					ELSE SUM(rTuChi+rHangMua+rHangNhap) END,

		CTB10=0
    --FROM    f_ns_chitieu_theodonvi_bql(@iNamLamviec, @iID_MaNganSach, @iID_MaDonVi, @iID_MaPhongBan, @iID_MaNamNganSach,@dNgay,1)
	FROM @TempTable
    WHERE   (@sLNS IS NULL OR sLNS IN (SELECT * FROM f_split(@sLNS))) AND iID_MaPhongBan = '07'
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa

	UNION ALL

	 -- hang nhap
    SELECT
        sLNS='1040200',sL,sK,sM,sTM,sTTM,sNG,sMoTa,
        CTB7=	SUM(rHangNhap),
		CTB10=0
    --FROM    f_ns_chitieu_theodonvi_bql(@iNamLamviec, @iID_MaNganSach, @iID_MaDonVi, @iID_MaPhongBan, @iID_MaNamNganSach,@dNgay,1)
	FROM @TempTable
    WHERE   (@sLNS IS NULL OR sLNS IN (SELECT * FROM f_split(@sLNS))) AND iID_MaPhongBan = '07'
			AND sLNS like '104%'
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa

	UNION ALL

	---- hang mua
	SELECT
        sLNS='1040300',sL,sK,sM,sTM,sTTM,sNG,sMoTa,
        CTB7=	SUM(rHangMua),
		CTB10=0
    --FROM    f_ns_chitieu_theodonvi_bql(@iNamLamviec, @iID_MaNganSach, @iID_MaDonVi, @iID_MaPhongBan, @iID_MaNamNganSach,@dNgay,1)
	FROM @TempTable
    WHERE   (@sLNS IS NULL OR sLNS IN (SELECT * FROM f_split(@sLNS))) AND iID_MaPhongBan = '07'
			AND sLNS like '104%'
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa


	UNION ALL

	---- lay chi tieu b10

	-- lay chi tieu
    SELECT
        sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,
		CTB7=0,		
		CTB10=CASE    
					WHEN sLNS LIKE '104%' then SUM(rTuChi)
					ELSE SUM(rTuChi+rHangMua+rHangNhap) END
    --FROM    f_ns_chitieu_theodonvi_bql(@iNamLamviec, @iID_MaNganSach, @iID_MaDonVi, @iID_MaPhongBan, @iID_MaNamNganSach,@dNgay,1)
	FROM @TempTable
    WHERE   (@sLNS IS NULL OR sLNS IN (SELECT * FROM f_split(@sLNS))) AND iID_MaPhongBan = '10'
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa

	UNION ALL
	 -- hang nhap
    SELECT
        sLNS='1040200',sL,sK,sM,sTM,sTTM,sNG,sMoTa,
        CTB7=0,		
		CTB10=SUM(rHangNhap)
    --FROM    f_ns_chitieu_theodonvi_bql(@iNamLamviec, @iID_MaNganSach, @iID_MaDonVi, @iID_MaPhongBan, @iID_MaNamNganSach,@dNgay,1)
	FROM @TempTable
    WHERE   (@sLNS IS NULL OR sLNS IN (SELECT * FROM f_split(@sLNS))) AND iID_MaPhongBan = '10'
			AND sLNS like '104%'
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa

	UNION ALL

	---- hang mua
	SELECT
        sLNS='1040300',sL,sK,sM,sTM,sTTM,sNG,sMoTa,
        CTB7=0,
		CTB10=SUM(rHangMua)
    --FROM    f_ns_chitieu_theodonvi_bql(@iNamLamviec, @iID_MaNganSach, @iID_MaDonVi, @iID_MaPhongBan, @iID_MaNamNganSach,@dNgay,1)
	FROM @TempTable
    WHERE   (@sLNS IS NULL OR sLNS IN (SELECT * FROM f_split(@sLNS))) AND iID_MaPhongBan = '10'
			AND sLNS like '104%'
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa
) a
 

WHERE LEFT(sLNS,1) IN (1,2,3,4)


GROUP BY a.sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa
HAVING  SUM(CTB7)<>0 OR
        SUM(CTB10)<>0
ORDER BY a.sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa