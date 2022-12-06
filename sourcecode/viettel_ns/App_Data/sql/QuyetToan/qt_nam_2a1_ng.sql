
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iNamLamViecNS int							set @iNamLamViecNS = 2019
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07' 
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='00,01,02,03,10,11,12,13,14,15,17,18,19,20,21,22,23,24,29,30,31,33,34,35,41,42,43,44,45,46,47,48,50,51,52,53,55,56,57,61,65,66,67,68,69,70,71,72,73,75,76,77,79,81,82,83,84,87,88,89,92,93,94,95,96,97,98,99,B16,B5'
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach='2'

--#DECLARE#--

SELECT  LEFT(sLNS,1) as sLNS1,
        LEFT(sLNS,3) as sLNS3,
        LEFT(sLNS,5) as sLNS5,
		sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa=dbo.F_MoTa_sXauNoiMa(@iNamLamViec,sLNS+'-'+sL+'-'+sK+'-'+sM+'-'+sTM+'-'+sTTM+'-'+sNG),
        iId_MaDonVi,sTenDonVi=[dbo].F_GetTenDonVi(@iNamLamViec,iId_MaDonVi),
        SUM(rChiTieu) as rChiTieu,
        SUM(rQuyetToan) as rQuyetToan,
        SUM(rDonViDeNghi) as rDonViDeNghi,
        SUM(rVuotChiTieu) rVuotChiTieu,
        SUM(rTonThatTonDong) as rTonThatTonDong,
        SUM(rDaCapTien)as rDaCapTien,
        SUM(rChuaCapTien) as rChuaCapTien 
FROM
(

SELECT  
        sLNS,sL,sK,sM,sTM,sTTM,sNG,
        iID_MaDonVi,
        rChiTieu=0,
        SUM(rTuChi) as rQuyetToan,
        SUM(rDonViDeNghi) as rDonViDeNghi,
        SUM(rVuotChiTieu) rVuotChiTieu,
        SUM(rTonThatTonDong) as rTonThatTonDong,
        rDaCapTien=0,
        rChuaCapTien=0
FROM    QTA_ChungTuChiTiet
WHERE   iTrangThai=1 
		AND iNamLamViec=@iNamLamViec
		AND (@iID_MaNamNganSach is null or iID_MaNamNganSach in (select * from f_split(@iID_MaNamNganSach)))
		AND (@iID_MaPhongBan IS NULL OR iID_MaPhongBan=@iID_MaPhongBan)
		AND (@iID_MaDonVi IS NULL OR iID_MaDonVi in (select * from F_Split(@iID_MaDonVi)))
GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi,sTenDonVi

union all

-- 1. chi tieu
SELECT  sLNS,sL,sK,sM,sTM,sTTM,sNG,
        iId_MaDonVi, 
        sum(rChiTieu) as rChiTieu,
        rQuyetToan = 0,
        rDonViDeNghi = 0,
        rVuotChiTieu = 0,
        rTonThatTonDong = 0,
        rDaCapTien = 0,
        rChuaCapTien = 0 
FROM    
(
   SELECT  sLNS,sL,sK,sM,sTM,sTTM,sNG,iId_MaDonVi,
            rChiTieu =	SUM(rTien)
    FROM f_ns_table_chitieu_tien(@iNamLamViec,@iID_MaDonVi,@iID_MaPhongBan,'1,5',null,1,null)
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iId_MaDonVi

) as dt_chitieu
GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iId_MaDonVi


union all
-- 2. nam sau - da cap tien
SELECT  sLNS,sL,sK,sM,sTM,sTTM,sNG,
        iId_MaDonVi, 
        rChiTieu=0,
        rQuyetToan = 0,
        rDonViDeNghi = 0,
        rVuotChiTieu = 0,
        rTonThatTonDong = 0,
        sum(rDaCapTien) as rDaCapTien,
        rChuaCapTien = 0 
FROM    
(
	-- nam nay cap cho nam truoc
	SELECT  sLNS,sL,sK,sM,sTM,sTTM,sNG,iId_MaDonVi,
            SUM(rTien) as rDaCapTien 
    FROM f_ns_table_chitieu_tien(@iNamLamViecNS,@iID_MaDonVi,@iID_MaPhongBan,5,null,1,null)
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iId_MaDonVi

) as dt_namsau_dacaptien
GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iId_MaDonVi

) as T
GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iId_MaDonVi
HAVING  SUM(rChiTieu) <> 0 OR
        SUM(rQuyetToan) <> 0 OR
        SUM(rDonViDeNghi) <> 0 OR
        SUM(rVuotChiTieu) <> 0 OR
        SUM(rTonThatTonDong) <> 0 OR
        SUM(rDaCapTien) <> 0 OR
        SUM(rChuaCapTien) <> 0 
