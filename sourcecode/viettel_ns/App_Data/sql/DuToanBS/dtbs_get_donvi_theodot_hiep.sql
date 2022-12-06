
declare @dvt int									set @dvt = 1000
declare @lenDV int									set @lenDV = 0
declare @iNamLamViec int							set @iNamLamViec = 2020
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07' 
declare @sDSLNS nvarchar(20)						set @sDSLNS=null
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi=null
declare @iID_MaChungTu nvarchar(MAX) 				set @iID_MaChungTu='d08ba392-0c6a-4dcd-8e4a-24740d940307'

--#DECLARE#--


SELECT DISTINCT 
		value	= ct.iID_MaDonVi, 
		text	= (ct.iID_MaDonVi + ' - ' + dv.sTen) 
FROM 
(
    SELECT  distinct iID_MaDonVi = case when @lenDV = 0 then iID_MaDonVi else LEFT(iID_MaDonVi,@lenDV) end
    FROM    DTBS_ChungTuChiTiet 
    WHERE   iTrangThai=1 
            AND (@iID_MaPhongBan IS NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)
            AND (@iID_MaDonVi IS NULL OR iID_MaDonVi IN (SELECT * FROM F_Split(@iID_MaDonVi)))
            AND iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu))
			and (@sDSLNS is null or sLNS in (select * from f_split(@sDSLNS)))

    UNION
                
    SELECT  distinct iID_MaDonVi = case when @lenDV = 0 then iID_MaDonVi else LEFT(iID_MaDonVi,@lenDV) end
    FROM    DTBS_ChungTuChiTiet_PhanCap
    WHERE   iTrangThai = 1 
            AND (@iID_MaPhongBan IS NULL OR iID_MaPhongBanDich=@iID_MaPhongBan)
            AND (@iID_MaDonVi IS NULL OR iID_MaDonVi IN (SELECT * FROM F_Split(@iID_MaDonVi)))
			and (@sDSLNS is null or sLNS in (select * from f_split(@sDSLNS)))
			 AND (iID_MaChungTu IN (SELECT iID_MaChungTuChiTiet FROM DTBS_ChungTuChiTiet WHERE iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu)))
				OR iID_MaChungTu in (	select iID_MaChungTuChiTiet 
										from DTBS_ChungTuChiTiet 
										where iID_MaChungTu in (
													select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
													where iID_MaChungTu in (   
																			select iID_MaChungTu from DTBS_ChungTu
																			where iID_MaChungTu in (
																									select iID_MaDuyetDuToanCuoiCung from DTBS_ChungTu
																									where iID_MaChungTu in (select * from F_Split(@iID_MaChungTu))))))

				---- phan cap lan 2: cho b
				OR iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet 
									 where iID_MaChungTu in (select iID_MaChungTuChiTiet from DTBS_ChungTuChiTiet where iID_MaChungTu in (SELECT * FROM F_Split(@iID_MaChungTu))))
				
				)



) as ct
JOIN NS_DonVi AS dv ON dv.iID_MaDonVi = ct.iID_MaDonVi
WHERE dv.iNamLamViec_DonVi = @iNamLamViec
