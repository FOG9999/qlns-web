declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iNamLamViecNS int							set @iNamLamViecNS = 2019
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan=null
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi=null
declare @iID_MaNamNganSach nvarchar(20)				set @iID_MaNamNganSach='2'
declare @namTuChi nvarchar(20)						set @namTuChi='2,4'
declare @namDaCapTien nvarchar(20)					set @namDaCapTien='1'
declare @namChuaCapTien nvarchar(20)				set @namChuaCapTien='4'

--#DECLARE#--

SELECT  sLNS1 = LEFT(sLNS,1),
        sLNS3 = LEFT(sLNS,3),
        sLNS5 = LEFT(sLNS,5),
		--sLNS	= case sLNS when '3010001' then '3010000' else sLNS end,
		sLNS,
		sL,sK,sM,sTM,sTTM,sNG, 
		--(CONVERT(nvarchar(7),case sLNS when '3010001' then '3010000' else sLNS end)	+'-'+
		(CONVERT(nvarchar(7),sLNS)	+'-'+
		 CONVERT(nvarchar(3),sL)	+'-'+
		 CONVERT(nvarchar(3),sK)	+'-'+
		 CONVERT(nvarchar(4),sM)	+'-'+
		 CONVERT(nvarchar(4),sTM)	+'-'+
		 CONVERT(nvarchar(2),sTTM)+'-'+
		 CONVERT(nvarchar(2),sNG))  as sXauNoiMa,
		--sMoTa=dbo.F_MoTa_sXauNoiMa(@iNamLamViec,sLNS+'-'+sL+'-'+sK+'-'+sM+'-'+sTM+'-'+sTTM+'-'+sNG),
        iId_MaDonVi,
		sTenDonVi=[dbo].F_GetTenDonVi(@iNamLamViec,iId_MaDonVi),
        SUM(rChiTieu) as rChiTieu,
        SUM(rQuyetToan) as rQuyetToan,

        SUM(rDonViDeNghi) as rDonViDeNghi,
        
		-- fix để cân số liệu
		--SUM(rQuyetToan + rVuotChiTieu) as rDonViDeNghi,
        SUM(rVuotChiTieu) rVuotChiTieu,
        SUM(rTonThatTonDong) as rTonThatTonDong,
        SUM(rDaCapTien)as rDaCapTien,
        SUM(rChuaCapTien) as rChuaCapTien,
		rSoQuaDuyet = case when SUM(rChiTieu - rDaCapTien - rChuaCapTien - rQuyetToan) < 0 then -SUM(rChiTieu - rDaCapTien - rChuaCapTien - rQuyetToan) else 0 end,
		rSoThua = case when SUM(rChiTieu - rDaCapTien - rChuaCapTien - rQuyetToan) > 0 then SUM(rChiTieu - rDaCapTien - rChuaCapTien - rQuyetToan) else 0 end
		 
FROM
(

SELECT  sLNS=case sLNS when '3010001' then '3010000' else sLNS end
		,sL,sK,sM,sTM,sTTM,sNG,
        iId_MaDonVi=left(iId_MaDonVi,2),
        rChiTieu=0,
        SUM(rTuChi) as rQuyetToan,
        SUM(rDonViDeNghi) as rDonViDeNghi,
        SUM(rVuotChiTieu) rVuotChiTieu,
		--rVuotChiTieu=SUM(CASE WHEN iThang_Quy =5 THEN rTuChi ELSE rVuotChiTieu END),
        SUM(rTonThatTonDong) as rTonThatTonDong,
        rDaCapTien=0,
        rChuaCapTien=0
FROM    QTA_ChungTuChiTiet
WHERE   iTrangThai=1 
		AND iNamLamViec=@iNamLamViec
		and iThang_Quy<>5
		AND (@iID_MaNamNganSach is null  or iID_MaNamNganSach in (select * from f_split(@namTuChi)))
		AND (@iID_MaPhongBan IS NULL OR iID_MaPhongBan in (select * from f_split(@iID_MaPhongBan)))
		AND (@iID_MaDonVi IS NULL OR iID_MaDonVi in (select * from F_Split(@iID_MaDonVi)))
GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi

union all

-- 1. chi tieu
SELECT   sLNS=case sLNS when '3010001' then '3010000' else sLNS end
		,sL,sK,sM,sTM,sTTM,sNG, 
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
	SELECT  sLNS,sL,sK,sM,sTM,sTTM,sNG,iId_MaDonVi=left(iId_MaDonVi,2),
            rChiTieu =	SUM(rTuChi)
    FROM f_ns_chitieu_full_tuchi(@iNamLamViec,@iID_MaDonVi,@iID_MaPhongBan,@namTuChi,null,GETDATE(),1,null)
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iId_MaDonVi
	
) as dt_chitieu
GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iId_MaDonVi

union all 
-- 2. nam sau - da cap tien
SELECT   sLNS=case sLNS when '3010001' then '3010000' else sLNS end
		,sL,sK,sM,sTM,sTTM,sNG, 
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
	-- nam sau cap ve
	SELECT  sLNS,sL,sK,sM,sTM,sTTM,sNG,iId_MaDonVi=left(iId_MaDonVi,2),
            rDaCapTien =	SUM(rTuChi)
    FROM f_ns_chitieu_full_tuchi(@iNamLamViecNS,@iID_MaDonVi,@iID_MaPhongBan,@namDaCapTien,null,GETDATE(),1,null)
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iId_MaDonVi

) as dt_namsau_dacaptien
GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iId_MaDonVi

union all 
-- 3. nam sau - chua cap tien
SELECT   sLNS=case sLNS when '3010001' then '3010000' else sLNS end
		,sL,sK,sM,sTM,sTTM,sNG, 
        iId_MaDonVi, 
        rChiTieu=0,
        rQuyetToan = 0,
        rDonViDeNghi = 0,
        rVuotChiTieu = 0,
        rTonThatTonDong = 0,
        rDaCapTien=0,
        rChuaCapTien = SUM(rChuaCapTien)
FROM    
(
	SELECT  sLNS,sL,sK,sM,sTM,sTTM,sNG,iId_MaDonVi=left(iId_MaDonVi,2),
            rChuaCapTien =	SUM(rTuChi)
    FROM f_ns_chitieu_full_tuchi(@iNamLamViecNS,@iID_MaDonVi,@iID_MaPhongBan,@namChuaCapTien,null,GETDATE(),1,null)
    GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iId_MaDonVi

) as dt_namsau_chuacaptien
GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iId_MaDonVi

) as T

--ban lao + campuchia
--where sLNS in (2140100,2140101) and sTM in (7402) -- campuchia
--where sLNS in (2140100,2140101) and sTM in (7401) -- lao
--where sLNS in (2020501)
--where sLNS like '214%'
--where sng in ('00','76')
	--and slns like '1%'

--where sLNS = '1020100' and sTM in ('6501','6502')

GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iId_MaDonVi
--GROUP BY case sLNS when '3010001' then '3010000' else sLNS end,sL,sK,sM,sTM,sTTM,sNG,iId_MaDonVi


HAVING  SUM(rChiTieu) <> 0 OR
        SUM(rQuyetToan) <> 0 OR
        SUM(rDonViDeNghi) <> 0 OR
        SUM(rVuotChiTieu) <> 0 OR
        SUM(rTonThatTonDong) <> 0 OR
        SUM(rDaCapTien) <> 0 OR
        SUM(rChuaCapTien) <> 0 
order by iId_MaDonVi