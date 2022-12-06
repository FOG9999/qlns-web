
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07' 
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='11'
declare @iID_MaChungTu uniqueidentifier 			set @iID_MaChungTu='73060f95-0244-4b1d-a1ec-2d1373057328'

--#DECLARE#--


SELECT DISTINCT 
		value	= ct.iID_MaDonVi, 
		text	= (ct.iID_MaDonVi + ' - ' + dv.sTen) 
FROM 
(
    SELECT  DISTINCT iID_MaDonVi
    FROM    DTBS_ChungTuChiTiet 
    WHERE   iTrangThai=1 
            --AND iNamLamViec=@iNamLamViec 
            --AND (@iID_MaPhongBan IS NULL OR iID_MaPhongBan=@iID_MaPhongBan)
            --AND (@iID_MaDonVi IS NULL OR iID_MaDonVi IN (SELECT * FROM F_Split(@iID_MaDonVi)))
            AND iID_MaChungTu IN (SELECT * FROM F_SplitString2(@iID_MaChungTu))

    UNION
                
    SELECT  DISTINCT iID_MaDonVi
    FROM    DTBS_ChungTuChiTiet_PhanCap
    WHERE   iTrangThai = 1 
            --AND iNamLamViec = @iNamLamViec    
            --AND (@iID_MaPhongBan IS NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)
            --AND (@iID_MaDonVi IS NULL OR iID_MaDonVi IN (SELECT * FROM F_SplitString2(@iID_MaDonVi)))
            AND iID_MaChungTu IN (SELECT iID_MaChungTuChiTiet FROM DTBS_ChungTuChiTiet WHERE iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu)))
) as ct
JOIN NS_DonVi AS dv ON dv.iID_MaDonVi = ct.iID_MaDonVi
WHERE dv.iNamLamViec_DonVi = @iNamLamViec
