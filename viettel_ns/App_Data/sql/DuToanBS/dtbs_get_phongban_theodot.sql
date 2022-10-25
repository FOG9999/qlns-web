
declare @dvt int									set @dvt = 1000
declare @iNamLamViec int							set @iNamLamViec = 2018
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07' 
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi='31'
declare @iID_MaChungTu nvarchar(MAX) 			set @iID_MaChungTu='f053c61e-6041-46ce-a87c-8c2f9aef12cc,fa234f87-000c-4605-9f82-e49cf2a08fae'

--#DECLARE#--


SELECT DISTINCT 
		value	= ct.iID_MaPhongBanDich, 
		text	= (ct.iID_MaPhongBanDich + ' - ' + pb.sTen) 
FROM 
(
    SELECT  DISTINCT iID_MaPhongBanDich
    FROM    DTBS_ChungTuChiTiet 
    WHERE   iTrangThai=1 
            AND (@iID_MaDonVi IS NULL OR iID_MaDonVi IN (SELECT * FROM F_Split(@iID_MaDonVi)))
            AND iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu))
			and (@sDSLNS is null or sLNS in (select * from f_split(@sDSLNS)))

    UNION 
                
    SELECT  DISTINCT iID_MaPhongBanDich
    FROM    DTBS_ChungTuChiTiet_PhanCap
    WHERE   iTrangThai = 1 
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
JOIN NS_PhongBan AS pb ON pb.sKyHieu = ct.iID_MaPhongBanDich