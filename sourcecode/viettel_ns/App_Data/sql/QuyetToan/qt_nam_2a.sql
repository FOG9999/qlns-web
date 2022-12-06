
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iNamLamViecNS int							set @iNamLamViecNS = 2019
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07' 
declare @iID_MaDonVi nvarchar(MAX)					set @iID_MaDonVi='10,11,12,13,14,15,17,19,21,22,23,24,29,31,33,41,42,43,44,45,46,47,79,81,82,83,84,87,95,96,97,98'
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach='1,2'
declare @sOrL int									set @sOrL = 0

--#DECLARE#--

SELECT  sLNS1 = CASE WHEN @sOrL = 1 THEN sLNS
					ELSE LEFT(sLNS,1) END
		,sLNS3 = CASE WHEN @sOrL = 1 THEN sLNS
					ELSE LEFT(sLNS,3) END
		,sLNS5 = CASE WHEN @sOrL = 1 THEN sLNS
					ELSE LEFT(sLNS,5) END,
		sLNS,
		--sMoTa=[dbo].F_MLNS_MoTa(@iNamLamViec,sLNS),
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
        rDaCapTien=0,
        rChuaCapTien=0
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
	SELECT  sLNS, iId_MaDonVi, 
			rChiTieu =	SUM(rTuChi)
    --FROM f_ns_table_chitieu_tien(@iNamLamViec,@iID_MaDonVi,@iID_MaPhongBan,'1,2,4,5',null,1,null)
	from f_ns_chitieu_full_tuchi(@iNamLamViec,@iID_MaDonVi,@iID_MaPhongBan,@namTuChi,null,getdate(),1,null)

    GROUP BY sLNS, iId_MaDonVi	

) as dt_chitieu
GROUP BY sLNS, iId_MaDonVi


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
        sum(rDaCapTien) as rDaCapTien,
        rChuaCapTien = 0 
FROM    
(
	-- so chuyen nam sau
	SELECT  sLNS,iId_MaDonVi,
			--rDaCapTien =	SUM(rTien)
			rDaCapTien =	SUM(rTuChi)
    --FROM f_ns_table_chitieu_tien(@iNamLamViecNS,@iID_MaDonVi,@iID_MaPhongBan,'1,5',null,1,null)
	from f_ns_chitieu_full_tuchi(@iNamLamViecNS,@iID_MaDonVi,@iID_MaPhongBan,@namDaCapTien,null,getdate(),1,null)
    GROUP BY sLNS, iId_MaDonVi		

) as dt_namsau_dacaptien
GROUP BY sLNS, iId_MaDonVi

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
			rChuaCapTien =	SUM(rTuChi)
	from f_ns_chitieu_full_tuchi(@iNamLamViecNS,@iID_MaDonVi,@iID_MaPhongBan,@namChuaCapTien,null,getdate(),1,null)
    --FROM f_ns_table_chitieu_tien(@iNamLamViecNS,@iID_MaDonVi,@iID_MaPhongBan,4,null,1,null)
    GROUP BY sLNS,iId_MaDonVi

) as dt_namsau_chuacaptien

) as T

-- dung cho bieu rutgon
where (@sOrL=0 or (len(sLNS)=1 or (left(sLNS,3) in (104,109))))

GROUP BY sLNS,iId_MaDonVi
HAVING  SUM(rChiTieu) <> 0 OR
        SUM(rQuyetToan) <> 0 OR
        SUM(rDonViDeNghi) <> 0 OR
        SUM(rVuotChiTieu) <> 0 OR
        SUM(rTonThatTonDong) <> 0 OR
        SUM(rDaCapTien) <> 0 OR
        SUM(rChuaCapTien) <> 0 
ORDER BY sLNS,iID_MaDonVi
