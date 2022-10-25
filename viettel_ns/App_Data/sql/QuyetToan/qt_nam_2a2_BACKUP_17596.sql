qt_nam_2a2.sql


--#DECLARE#--
/*

Báo cáo số liệu kết luận quyết toán ngân sách năm nay
*/

<<<<<<< HEAD
SELECT  LEFT(sLNS,1) as sLNS1,
        LEFT(sLNS,3) as sLNS3,
        LEFT(sLNS,5) as sLNS5,
		sLNS, sMoTa=[dbo].F_MLNS_MoTa(@iNamLamViec,sLNS),
        iId_MaDonVi,sTenDonVi=[dbo].F_GetTenDonVi(@iNamLamViec,iId_MaDonVi),
        SUM(rChiTieu) as rChiTieu,
=======
SELECT  sLNS1,sLNS3,sLNS5,sLNS
		,sMoTa=[dbo].F_MLNS_MoTa(@iNamLamViec,sLNS)
        ,iId_MaDonVi
		,sTenDonVi=[dbo].F_GetTenDonVi(@iNamLamViec,iId_MaDonVi)
        ,SUM(rChiTieu) as rChiTieu,
>>>>>>> b0862870980bb83d603e3a72d209989feba9e39e
        SUM(rQuyetToan) as rQuyetToan,
        SUM(rDonViDeNghi) as rDonViDeNghi,
        SUM(rVuotChiTieu) rVuotChiTieu,
        SUM(rTonThatTonDong) as rTonThatTonDong,
        SUM(rDaCapTien)as rDaCapTien,
        SUM(rChuaCapTien) as rChuaCapTien 
FROM
(

SELECT  sLNS,
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
SELECT  sLNS, 
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
SELECT  sLNS, 
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
<<<<<<< HEAD
	-- chuyen nam sau
	SELECT  sLNS,iId_MaDonVi,
			rDaCapTien =	SUM(rTien)
    FROM f_ns_table_chitieu_tien(@iNamLamViecNS,@iID_MaDonVi,@iID_MaPhongBan,1,null,1,null)
    GROUP BY sLNS, iId_MaDonVi
=======
	SELECT  sLNS1,sLNS3,sLNS5,sLNS, iId_MaDonVi,
            SUM(rTuchi + rHangNhap + rHangMua) as rDaCapTien 
    FROM f_ns_chitieu(@iNamLamViec + 1,@iID_MaDonVi,@iID_MaPhongBan,1,null,1,null)
    GROUP BY sLNS1,sLNS3,sLNS5,sLNS, iId_MaDonVi
>>>>>>> b0862870980bb83d603e3a72d209989feba9e39e

) as dt_namsau_dacaptien


union all
-- 3. nam sau - chua cap tien
SELECT  sLNS, 
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
<<<<<<< HEAD
	SELECT  sLNS,iId_MaDonVi,
			rChuaCapTien =	SUM(rTien)
    FROM f_ns_table_chitieu_tien(@iNamLamViecNS,@iID_MaDonVi,@iID_MaPhongBan,4,null,1,null)
    GROUP BY sLNS,iId_MaDonVi
=======
	SELECT  sLNS1,sLNS3,sLNS5,sLNS, iId_MaDonVi,
            SUM(rTuchi + rHangNhap + rHangMua) as rChuaCapTien 
    FROM f_ns_chitieu(@iNamLamViec + 1,@iID_MaDonVi,@iID_MaPhongBan,4,null,1,null)
    GROUP BY sLNS1,sLNS3,sLNS5,sLNS, iId_MaDonVi
>>>>>>> b0862870980bb83d603e3a72d209989feba9e39e

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
