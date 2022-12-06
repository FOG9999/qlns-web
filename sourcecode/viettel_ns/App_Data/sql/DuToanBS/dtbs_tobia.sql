
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @username nvarchar(20)						set @username='chuctc'
declare @iID_MaChungTu nvarchar(200)				set @iID_MaChungTu='07' 
declare @iID_MaNganh nvarchar(20)					set @iID_MaNganh='51'
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach='1,2'
declare @sLNS nvarchar(200)							set @sLNS='104%'

--#DECLARE#--


SELECT  sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa,
        rTuChi      =SUM(rTuChi)/@dvt,
        rHangNhap   =SUM(rHangNhap)/@dvt,
        rHangMua    =SUM(rHangMua)/@dvt,
        rTonKho     =SUM(rTonKho)/@dvt,
        rPhanCap    =SUM(rPhanCap)/@dvt,
        rDuPhong    =SUM(rDuPhong)/@dvt 
FROM    DTBS_ChungTuChiTiet 
WHERE   iTrangThai=1  
        --AND sLNS='1040100'  
        --AND LEFT(sLNS,3) IN (104,109)
        AND (sLNS like @sLNS)
        AND (MaLoai='' OR MaLoai='2') 
        AND iNamLamViec=@iNamLamViec
		and iid_madonvi='51'
        AND sNG IN (SELECT * FROM F_Split((SELECT TOP(1) iID_MaNganhMLNS FROM NS_MucLucNganSach_Nganh WHERE iNamLamViec=@iNamLamViec AND iID_MaNganh=@iID_MaNganh)))
        AND iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu))

GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa
HAVING  SUM(rTuChi) <>0
        OR SUM(rHangMua) <>0
        OR SUM(rHangNhap) <>0
        OR SUM(rTonkho) <>0
        OR SUM(rPhanCap) <>0
        OR SUM(rDuPhong) <>0
