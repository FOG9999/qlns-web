
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
		--sLNS	= case sLNS when '2140101' then '2140100' else sLNS end,
		sLNS,
		sL,sK,sM,sTM,sTTM,sNG, 
		(CONVERT(nvarchar(7),sLNS)	+'-'+
		 CONVERT(nvarchar(3),sL)	+'-'+
		 CONVERT(nvarchar(3),sK)	+'-'+
		 CONVERT(nvarchar(4),sM)	+'-'+
		 CONVERT(nvarchar(4),sTM)	+'-'+
		 CONVERT(nvarchar(2),sTTM)+'-'+
		 CONVERT(nvarchar(2),sNG))  as sXauNoiMa,
		--sMoTa=dbo.F_MoTa_sXauNoiMa(@iNamLamViec,sLNS+'-'+sL+'-'+sK+'-'+sM+'-'+sTM+'-'+sTTM+'-'+sNG),
        iId_MaDonVi,
		--sTenDonVi=[dbo].F_GetTenDonVi(@iNamLamViec,iId_MaDonVi),
		dv.sTenDonVi,
        SUM(rVuotChiTieu) rVuotChiTieu,
        SUM(rTonThatTonDong) as rTonThatTonDong
		 
		 
FROM
(

SELECT  sLNS,sL,sK,sM,sTM,sTTM,sNG,
        iID_MaDonVi,
        rChiTieu=0,
        --SUM(rTuChi) as rQuyetToan,
        --SUM(rDonViDeNghi) as rDonViDeNghi,
        SUM(rVuotChiTieu) rVuotChiTieu,
        SUM(rTonThatTonDong) as rTonThatTonDong

FROM    QTA_ChungTuChiTiet
WHERE   iTrangThai=1 
		AND iNamLamViec=@iNamLamViec
		AND (@iID_MaNamNganSach is null  or iID_MaNamNganSach in (select * from f_split(@namTuChi)))
		AND (@iID_MaPhongBan IS NULL OR iID_MaPhongBan in (select * from f_split(@iID_MaPhongBan)))
		AND (@iID_MaDonVi IS NULL OR iID_MaDonVi in (select * from F_Split(@iID_MaDonVi)))
GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iID_MaDonVi
having SUM(rVuotChiTieu)<>0 or SUM(rTonThatTonDong)<>0
 

) as T

-- lay ten don vi
inner join (SELECT iID_MaDonVi as dv_id, iID_MaDonVi + ' - ' + sTen as sTenDonVi FROM NS_DonVi WHERE iNamLamViec_DonVi=@iNamLamViec) as dv
on T.iID_MaDonVi=dv.dv_id


--ban lao + campuchia
--where sLNS in (2140100,2140101) and sTM in (7402) -- campuchia
--where sLNS in (2140100,2140101) and sTM in (7401) -- lao
--where sLNS in (2020501)
--where sLNS like '102%'

GROUP BY sLNS,sL,sK,sM,sTM,sTTM,sNG,iId_MaDonVi, dv.sTenDonVi
HAVING   
        SUM(rVuotChiTieu) <> 0 OR
        SUM(rTonThatTonDong) <> 0
