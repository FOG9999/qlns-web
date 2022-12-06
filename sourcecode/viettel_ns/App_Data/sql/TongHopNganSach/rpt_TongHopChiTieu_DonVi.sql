
declare @dvt int									set @dvt = 1000
declare @iNamLamViec nvarchar(10)					set @iNamLamViec = 2019
declare @username nvarchar(20)						set @username='chuctc'
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan=null
declare @iID_MaDonVi nvarchar(200)					set @iID_MaDonVi='55'
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach='1'
declare @dMaDot datetime							set @dMaDot=GetDATE()
--declare @dMaDot datetime							set @dMaDot='2019-02-19 00:00:00'
declare @loai nvarchar(10)							set @loai=null

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
		rTuChi,
		rHienVat,
		rTonKho,
		rHangNhap,
		rHangMua,
		rPhanCap,
		rDuPhong 
from	f_ns_chitieu_full5(@iNamLamViec,@iID_MaDonVi,@iID_MaPhongBan,@iID_MaNamNganSach,null,@dMaDot,@dvt,@loai)

select sLNS1,
	sLNS3,
	sLNS5,
	sLNS,sL,sK,sM,sTM,sTTM,sNG,
	sXauNoiMa,
	SUM(rTuChi) as rTuChi, 
	SUM(rHienVat) as rHienVat,
	SUM(rTonKho) as rTonKho,
	SUM(rHangNhap) as rHangNhap,
	SUM(rHangMua) as rHangMua,
	SUM(rPhanCap) as rPhanCap,
	SUM(rDuPhong) as rDuPhong 
from
(

SELECT	
		sLNS1 = SUBSTRING(sLNS,1,1),
	    sLNS3 = SUBSTRING(sLNS,1,3),
		sLNS5 = SUBSTRING(sLNS,1,5),
		sLNS,sL,sK,sM,sTM,sTTM,sNG,
		sXauNoiMa=sLNS+'-'+sL+'-'+sK+'-'+sM+'-'+sTM+'-'+sTTM+'-'+sNG,
		SUM(rTuChi) as rTuChi, 
		SUM(rHienVat) as rHienVat,
		SUM(rTonKho) as rTonKho,
		rHangNhap = case when sLNS like '104%' then 0 else sum(rHangNhap) end,
		rHangMua = case when sLNS like '104%' then 0 else sum(rHangMua) end,
		SUM(rPhanCap) as rPhanCap,
		SUM(rDuPhong) as rDuPhong
from	@TempTable
group by  sLNS,sL,sK,sM,sTM,sTTM,sNG

union all

SELECT	
		sLNS1 = SUBSTRING(sLNS,1,1),
	    sLNS3 = SUBSTRING(sLNS,1,3),
		sLNS5 = '10402',
		sLNS='1040200',sL,sK,sM,sTM,sTTM,sNG,
		sXauNoiMa='1040200-'+sL+'-'+sK+'-'+sM+'-'+sTM+'-'+sTTM+'-'+sNG,
		rTuChi = 0, 
		rHienVat = 0,
		rTonKho = 0,
		rHangNhap = sum(rHangNhap),
		rHangMua = 0,
		rPhanCap = 0,
		rDuPhong = 0
from	@TempTable
where	rHangNhap <> 0 and sLNS like '104%'
group by  sLNS,sL,sK,sM,sTM,sTTM,sNG

union all

SELECT	
		sLNS1 = SUBSTRING(sLNS,1,1),
	    sLNS3 = SUBSTRING(sLNS,1,3),
		sLNS5 = '10403',
		sLNS='1040300',sL,sK,sM,sTM,sTTM,sNG,
		sXauNoiMa='1040300-'+sL+'-'+sK+'-'+sM+'-'+sTM+'-'+sTTM+'-'+sNG,
		rTuChi = 0, 
		rHienVat = 0,
		rTonKho = 0,
		rHangNhap = 0,
		rHangMua = sum(rHangMua),
		rPhanCap = 0,
		rDuPhong = 0
from	@TempTable
where	rHangMua <> 0 and sLNS like '104%'
group by  sLNS,sL,sK,sM,sTM,sTTM,sNG

union all

SELECT	
		sLNS1 = SUBSTRING(sLNS,1,1),
		sLNS3 = SUBSTRING(sLNS,1,3),
		sLNS5 = SUBSTRING(sLNS,1,5),
		sLNS,sL,sK,sM,sTM,sTTM,sNG,
		sXauNoiMa=sLNS+'-'+sL+'-'+sK+'-'+sM+'-'+sTM+'-'+sTTM+'-'+sNG,
		SUM(rTuChi)/@dvt as rTuChi, 
		0 as rHienVat,
		0 as rTonKho,
		0 as rHangNhap,
		0 as rHangMua,
		0 as rPhanCap,
		0 as rDuPhong
FROM    CP_CapPhatChiTiet
WHERE   iTrangThai=1 
        AND iNamLamViec=@iNamLamViec 
		AND (@iID_MaPhongBan IS NULL OR iID_MaPhongBan=@iID_MaPhongBan)
        AND iID_MaNamNganSach = '1' and rTuChi < 0 and 1 in (select * from f_split(@iID_MaNamNganSach)) 
        and iID_MaDonVi = @iID_MaDonVi
GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG

) as a
where	(rTuChi+rTonKho+rHienVat+rHangNhap+rHangMua+rDuPhong+rPhanCap)<>0
group by sLNS1,
	    sLNS3,
		sLNS5,
		sLNS,sL,sK,sM,sTM,sTTM,sNG,
		sXauNoiMa
order by sXauNoiMa
