DECLARE @iIdThongTriId uniqueidentifier SET @iIdThongTriId = ''
		
--#DECLARE#--
		
SELECT dt.iID_DuAnID as IIdDuAnId, da.STenDuAn, 
		da.iID_LoaiCongTrinhID as IIdLoaiCongTrinhId, lct.STenLoaiCongTrinh,
		lct.LNS as SLns, lct.L as SL, lct.K as SK, lct.M as SM, lct.TM as STm, lct.TTM as STtm, lct.NG as SNg,
		ISNULL(dt.fSoTien, 0) as FSoTien, dt.iID_MucID as IIdMucId, dt.iID_TieuMucID as IIdTieuMucId, dt.iID_TietMucID as IIdTietMucId, dt.iID_NganhID as IIdNganhId
FROM VDT_ThongTri_ChiTiet as dt
INNER JOIN VDT_DA_DuAn as da on dt.iID_DuAnID = da.iID_DuAnID
LEFT JOIN VDT_DM_LoaiCongTrinh as lct on dt.iID_LoaiCongTrinhID = lct.iID_LoaiCongTrinh
WHERE dt.iID_ThongTriID = @iIdThongTriId