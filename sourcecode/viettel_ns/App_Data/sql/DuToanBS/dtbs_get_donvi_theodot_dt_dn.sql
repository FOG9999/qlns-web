
declare @dvt int									set @dvt = 1000
declare @lenDV int									set @lenDV = 0
declare @iNamLamViec int							set @iNamLamViec = 2021
declare @iID_MaPhongBan nvarchar(20)				set @iID_MaPhongBan='07,08,10,17' 
declare @iID_MaPhongBanNguon nvarchar(20)			set @iID_MaPhongBanNguon='07,10' 
declare @sDSLNS nvarchar(20)						set @sDSLNS=null
declare @iID_MaDonVi nvarchar(20)					set @iID_MaDonVi=null
declare @iID_MaChungTu nvarchar(MAX) 				set @iID_MaChungTu='0dba0387-adce-4ef1-8609-b40d9fea1506'

--#DECLARE#--
declare @dn nvarchar(MAX)
set @dn = '30,34,35,50,69,70,71,72,73,78,80,88,89,90,91,92,93,94'

SELECT DISTINCT 
		value	= ct.iID_MaDonVi, 
		text	= (ct.iID_MaDonVi + ' - ' + dv.sTen), iSTT
		 
FROM 
(
    SELECT  distinct iID_MaDonVi = case when @lenDV = 0 then iID_MaDonVi else LEFT(iID_MaDonVi,@lenDV) end
    FROM    DTBS_ChungTuChiTiet 
    WHERE   iTrangThai=1 
            AND (@iID_MaPhongBan IS NULL OR (iID_MaPhongBanDich in (select * from f_split(@iID_MaPhongBan)) AND @iID_MaPhongBan <> '06' AND iID_MaDonVi not in (select * from f_split(@dn))) OR (@iID_MaPhongBan = '06' AND (iID_MaDonVi in (select * from f_split(@dn)) or iID_MaPhongBanDich = '06')))
            AND (@iID_MaPhongBanNguon IS NULL OR iID_MaPhongBan in (select * from f_split(@iID_MaPhongBanNguon)))
            AND (@iID_MaDonVi IS NULL OR iID_MaDonVi IN (SELECT * FROM F_Split(@iID_MaDonVi)))
            AND iID_MaChungTu IN (SELECT * FROM F_Split(@iID_MaChungTu))
			and (@sDSLNS is null or sLNS in (select * from f_split(@sDSLNS)))
			and (rTuChi + rHangNhap + rHangMua + rChiTaiNganh + rPhanCap + rDuPhong) <> 0

    UNION ALL
                
    SELECT  distinct iID_MaDonVi = case when @lenDV = 0 then iID_MaDonVi else LEFT(iID_MaDonVi,@lenDV) end
    FROM    DTBS_ChungTuChiTiet_PhanCap
    WHERE   iTrangThai = 1 
            AND (@iID_MaPhongBan IS NULL OR (iID_MaPhongBanDich in (select * from f_split(@iID_MaPhongBan)) AND @iID_MaPhongBan <> '06' AND iID_MaDonVi not in (select * from f_split(@dn))) OR (@iID_MaPhongBan = '06' AND (iID_MaDonVi in (select * from f_split(@dn)) or iID_MaPhongBanDich = '06')))
            AND (iID_MaPhongBan in (select * from f_split(@iID_MaPhongBanNguon)))
			and rTuChi <> 0
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
LEFT JOIN NS_DonVi AS dv ON dv.iID_MaDonVi = ct.iID_MaDonVi
WHERE dv.iNamLamViec_DonVi = @iNamLamViec and dv.iTrangThai = 1
ORDER BY dv.iSTT