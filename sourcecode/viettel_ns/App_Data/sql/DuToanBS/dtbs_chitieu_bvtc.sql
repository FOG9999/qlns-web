
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2020
declare @iID_MaChungTu nvarchar(MAX)				set @iID_MaChungTu='65ccb586-16d2-4052-91d2-4f10a756bd28,85b2711c-620a-47ae-8eda-5d97dc2b1b63,3b6ad507-a318-405d-bb94-6e52e08e3849,1b20b59c-5fbc-42fa-b118-6f1ba92a2b2e,905ae383-805d-4ea0-841c-92770a725bb5,e644bb6c-0483-4c00-bbe7-93707a97d750,dbd8c50d-807c-4baa-a46d-ae19764539b9,c0547e70-5683-40eb-ad20-b24d2a007078'
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='52'
declare @iID_MaPhongBan nvarchar(2)					set @iID_MaPhongBan='10'
declare @sLNS nvarchar(100)							set @sLNS=''

--#DECLARE#--

SELECT  iID_MaDonVi,sTenDonVi= case when LEN(iID_MaDonVi) > 2 then SUBSTRING([dbo].F_GetTenDonVi(@iNamLamviec, iID_MaDonVi),7,LEN([dbo].F_GetTenDonVi(@iNamLamviec, iID_MaDonVi))-6) else [dbo].F_GetTenDonVi(@iNamLamviec, iID_MaDonVi) end,
        rTuChi      =SUM(rTuChi)/@dvt
FROM    DTBS_ChungTuChiTiet 
WHERE   iTrangThai=1 
        AND iNamLamViec=@iNamLamViec
        AND MaLoai=''
		AND iID_MaPhongBanDich=@iID_MaPhongBan
        AND iID_MaDonVi like @iID_MaDonVi
        AND iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu)) 
		AND sLNS = '1020900'

GROUP BY iID_MaDonVi
HAVING  SUM(rTuChi)<>0  