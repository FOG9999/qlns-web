qt_nam_2a2.sql

declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07' 
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='10,11,12,13,14,15,17,19,21,22,23,24,29,31,33,41,42,43,44,45,46,47,81,82,83,84,87,95,96,97,98'
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach='1,2'

--#DECLARE#--
/*

Báo cáo số liệu kết luận quyết toán ngân sách năm nay
*/

SELECT  sLNS1 = CASE WHEN @sOrL = 1 THEN sLNS
					ELSE LEFT(sLNS,1) END
		,sLNS3 = CASE WHEN @sOrL = 1 THEN sLNS
					ELSE LEFT(sLNS,3) END
		,sLNS5 = CASE WHEN @sOrL = 1 THEN sLNS
					ELSE LEFT(sLNS,5) END,
		sLNS, sMoTa=[dbo].F_MLNS_MoTa(@iNamLamViec,sLNS),
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

SELECT  sLNS = CASE WHEN @sOrL = 1 and sLNS NOT LIKE '1%' THEN LEFT(sLNS,1)
					WHEN @sOrL = 1 and sLNS LIKE '1%' THEN LEFT(sLNS,3) ELSE sLNS END,
        iID_MaDonVi,
        rChiTieu=0,
        SUM(rTuChi) as rQuyetToan,
        SUM(rDonViDeNghi) as rDonViDeNghi,
        SUM(rVuotChiTieu) rVuotChiTieu,
        SUM(rTonThatTonDong) as rTonThatTonDong,
        SUM(rDaCapTien)as rDaCapTien,
        SUM(rChuaCapTien) as rChuaCapTien 
FROM    QTA_ChungTuChiTiet
WHERE   iTrangThai=1 
		AND iNamLamViec=@iNamLamViec
		AND (@iID_MaNamNganSach is null or iID_MaNamNganSach in (select * from f_split(@iID_MaNamNganSach)))
		AND (@iID_MaPhongBan IS NULL OR iID_MaPhongBan=@iID_MaPhongBan)
		AND (@iID_MaDonVi IS NULL OR iID_MaDonVi in (select * from F_Split(@iID_MaDonVi)))
GROUP BY sLNS,iID_MaDonVi,sTenDonVi

union all
-- 1. chi tieu
SELECT  sLNS = CASE WHEN @sOrL = 1 and sLNS NOT LIKE '1%' THEN LEFT(sLNS,1)
					WHEN @sOrL = 1 and sLNS LIKE '1%' THEN LEFT(sLNS,3) ELSE sLNS END,
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
   -- chi tieu nam nay
	SELECT  sLNS, iId_MaDonVi, 
			rChiTieu =	SUM(rTien)
    FROM f_ns_table_chitieu_tien(@iNamLamViec,@iID_MaDonVi,@iID_MaPhongBan,'2,4',null,1,null)
    GROUP BY sLNS, iId_MaDonVi	
	
) as dt_chitieu
GROUP BY sLNS,iId_MaDonVi


union all
-- 2. nam sau - da cap tien
SELECT  sLNS = CASE WHEN @sOrL = 1 and sLNS NOT LIKE '1%' THEN LEFT(sLNS,1)
					WHEN @sOrL = 1 and sLNS LIKE '1%' THEN LEFT(sLNS,3) ELSE sLNS END,
        iId_MaDonVi, 
        rChiTieu=0,
        rQuyetToan = 0,
        rDonViDeNghi = 0,
        rVuotChiTieu = 0,
        rTonThatTonDong = 0,
        rDaCapTien,
        rChuaCapTien = 0 
FROM    
(
	-- chuyen nam sau
	SELECT  sLNS,iId_MaDonVi,
			rDaCapTien =	SUM(rTien)
    FROM f_ns_table_chitieu_tien(@iNamLamViecNS,@iID_MaDonVi,@iID_MaPhongBan,1,null,1,null)
    GROUP BY sLNS, iId_MaDonVi

) as dt_namsau_dacaptien


union all
-- 3. nam sau - chua cap tien
SELECT  sLNS = CASE WHEN @sOrL = 1 and sLNS NOT LIKE '1%' THEN LEFT(sLNS,1)
					WHEN @sOrL = 1 and sLNS LIKE '1%' THEN LEFT(sLNS,3) ELSE sLNS END,
        iId_MaDonVi, 
        rChiTieu=0,
        rQuyetToan = 0,
        rDonViDeNghi = 0,
        rVuotChiTieu = 0,
        rTonThatTonDong = 0,
        rDaCapTien=0,
        rChuaCapTien
FROM    
(
	SELECT  sLNS,iId_MaDonVi,
			rChuaCapTien =	SUM(rTien)
    FROM f_ns_table_chitieu_tien(@iNamLamViecNS,@iID_MaDonVi,@iID_MaPhongBan,4,null,1,null)
    GROUP BY sLNS,iId_MaDonVi

) as dt_namsau_chuacaptien


) as T
GROUP BY sLNS,iId_MaDonVi
HAVING  SUM(rChiTieu) <> 0 OR
        SUM(rQuyetToan) <> 0 OR
        SUM(rDonViDeNghi) <> 0 OR
        SUM(rVuotChiTieu) <> 0 OR
        SUM(rTonThatTonDong) <> 0 OR
        SUM(rDaCapTien) <> 0 OR
        SUM(rChuaCapTien) <> 0 
ORDER BY sLNS1,sLNS3,sLNS5,sLNS,iID_MaDonVi