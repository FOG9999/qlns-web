
ALTER PROC [dbo].[sp_vdt_getthongtrichitiet]
@iIdQuyetToanId uniqueidentifier
AS
BEGIN
	SELECT dt.iID_DuAnID as IIdDuAnId, da.STenDuAn, 
		da.iID_LoaiCongTrinhID as IIdLoaiCongTrinhId, lct.STenLoaiCongTrinh,
		lct.LNS, lct.L, lct.K, lct.M, lct.TM, lct.TTM, lct.NG,ml.iID_MaMucLucNganSach as IIdMucLucNganSach,
		(ISNULL(dt.FThuHoiUngNamTrc, 0) + ISNULL(dt.fThanhToanKLHT_CTNamTrcChuyenSang, 0) + ISNULL(dt.FThanhToanKLHT_CTNamNay, 0)) as FSoTien, NULL as IIdMucId, NULL as IIdTieuMucId, NULL as IIdTietMucId, NULL as IIdNganhId
	FROM VDT_QT_BCQuyetToanNienDo as tbl
	INNER JOIN VDT_QT_BCQuyetToanNienDo_ChiTiet_01 as dt on tbl.iID_BCQuyetToanNienDoID = dt.iID_BCQuyetToanNienDo
	INNER JOIN VDT_DA_DuAn as da on dt.iID_DuAnID = da.iID_DuAnID
	LEFT JOIN VDT_DM_LoaiCongTrinh as lct on da.iID_LoaiCongTrinhID = lct.iID_LoaiCongTrinh
	LEFT JOIN NS_MucLucNganSach as ml on ISNULL(lct.LNS, '') = ISNULL(ml.sLNS, '') AND ISNULL(lct.L, '') = ISNULL(ml.sL, '') AND ISNULL(lct.K, '') = ISNULL(ml.sK, '') 
		AND ISNULL(lct.M, '') = ISNULL(ml.sM, '') AND ISNULL(lct.TM, '') = ISNULL(ml.sTM, '') AND ISNULL(lct.TTM, '') = ISNULL(ml.sTTM, '') AND ISNULL(lct.NG, '') = ISNULL(ml.sNG, '') 
		AND ml.sTNG IS NULL AND ml.iNamLamViec = tbl.iNamKeHoach
	WHERE tbl.iID_BCQuyetToanNienDoID = @iIdQuyetToanId
END