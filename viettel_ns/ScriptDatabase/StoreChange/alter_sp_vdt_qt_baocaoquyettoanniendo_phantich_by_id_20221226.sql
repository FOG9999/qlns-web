
ALTER PROC [dbo].[sp_vdt_qt_baocaoquyettoanniendo_phantich_by_id]
@iIDQuyetToanId uniqueidentifier
AS
BEGIN
	DECLARE @iNamLamViec int
	DECLARE @iIdNguonVonId int
	DECLARE @iIdMaDonVi nvarchar(100)
	SELECT @iNamLamViec = iNamKeHoach, @iIdNguonVonId = iID_NguonVonID, @iIdMaDonVi = iID_MaDonViQuanLy
	FROM VDT_QT_BCQuyetToanNienDo WHERE iID_BCQuyetToanNienDoID = @iIDQuyetToanId

	SELECT dt.iID_DuAnID, dt.iID_LoaiCongTrinh, 
		SUM(ISNULL(dt.FDuToanCnsChuaGiaiNganTaiKb, 0)) as FDuToanCnsChuaGiaiNganTaiKbNamTruoc,
		SUM(ISNULL(dt.FDuToanCnsChuaGiaiNganTaiDv, 0)) as FDuToanCnsChuaGiaiNganTaiDvNamTruoc,
		SUM(ISNULL(dt.FDuToanCnsChuaGiaiNganTaiCuc, 0)) as FDuToanCnsChuaGiaiNganTaiCucNamTruoc INTO #tmpNamTruoc
	FROM VDT_QT_BCQuyetToanNienDo as tbl
	INNER JOIN VDT_QT_BCQuyetToanNienDo_PhanTich as dt on tbl.iID_BCQuyetToanNienDoID = dt.iID_BCQuyetToanNienDo
	WHERE iNamKeHoach = (@iNamLamViec - 1) AND iID_NguonVonID = @iIdNguonVonId AND iID_MaDonViQuanLy = @iIdMaDonVi
	GROUP BY dt.iID_DuAnID, dt.iID_LoaiCongTrinh
	
	SELECT DISTINCT iID_DuAnID, CAST(0 as BIT) BIsChuyenTiep INTO #tmp
	FROM VDT_QT_BCQuyetToanNienDo_PhanTich
	WHERE iID_BCQuyetToanNienDo = @iIDQuyetToanId

	UPDATE tmp
	SET
		BIsChuyenTiep = 1
	FROM #tmp as tmp
	INNER JOIN (
		SELECT DISTINCT dt.iID_DuAnID 
		FROM VDT_KHV_PhanBoVon as tbl 
		INNER JOIN VDT_KHV_PhanBoVon_ChiTiet as dt on tbl.iID_PhanBoVonID = dt.iID_PhanBoVonID 
		WHERE tbl.bActive = 1 AND tbl.iNamKeHoach = (@iNamLamViec - 1)
		) as mp on tmp.iID_DuAnID = mp.iID_DuAnID

	SELECT tbl.iID_DuAnID as IIdDuAnId,
		da.STenDuAn,
		tmp.BIsChuyenTiep,
		tbl.iID_LoaiCongTrinh,
		lct.SMaLoaiCongTrinh,
		lct.STenLoaiCongTrinh,
		nt.FDuToanCnsChuaGiaiNganTaiKbNamTruoc,
		nt.FDuToanCnsChuaGiaiNganTaiDvNamTruoc,
		nt.FDuToanCnsChuaGiaiNganTaiCucNamTruoc,
		tbl.FDuToanCnsChuaGiaiNganTaiKb,
		tbl.FDuToanCnsChuaGiaiNganTaiDv,
		tbl.FDuToanCnsChuaGiaiNganTaiCuc,
		tbl.FTuChuaThuHoiTaiCuc,
		tbl.FChiTieuNamNayKb,
		tbl.FChiTieuNamNayLc,
		tbl.FSoCapNamTrcCs,
		tbl.FSoCapNamNay,
		tbl.FDnQuyetToanNamTrc,
		tbl.FDnQuyetToanNamNay,
		tbl.FTuChuaThuHoiTaiDonVi,
		tbl.FDuToanThuHoi
	FROM VDT_QT_BCQuyetToanNienDo_PhanTich as tbl
	INNER JOIN VDT_DA_DuAn as da on tbl.iID_DuAnID = da.iID_DuAnID
	INNER JOIN #tmp as tmp on tbl.iID_DuAnID = tmp.iID_DuAnID
	LEFT JOIN #tmpNamTruoc as nt on tbl.iID_DuAnID = nt.iID_DuAnID AND tbl.iID_LoaiCongTrinh = nt.iID_LoaiCongTrinh
	LEFT JOIN VDT_DM_LoaiCongTrinh as lct on tbl.iID_LoaiCongTrinh = lct.iID_LoaiCongTrinh
	WHERE tbl.iID_BCQuyetToanNienDo = @iIDQuyetToanId

	DROP TABLE #tmpNamTruoc
END