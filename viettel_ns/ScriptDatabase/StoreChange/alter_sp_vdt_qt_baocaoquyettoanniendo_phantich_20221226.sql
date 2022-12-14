
ALTER PROC [dbo].[sp_vdt_qt_baocaoquyettoanniendo_phantich]
@iIdMaDonVi nvarchar(100),
@iNamKeHoach int,
@iIdNguonVon int
AS
BEGIN
	SELECT DISTINCT da.iID_DuAnID as IIDDuAnID, da.SMaDuAn, da.SDiaDiem, da.STenDuAn, ISNULL(pc.sMa, '') as sMaPhanCap, dt.iID_LoaiCongTrinh INTO #tmpUnion
	FROM VDT_KHV_KeHoachVonNam_DuocDuyet as tbl
	INNER JOIN VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet as dt on tbl.iID_KeHoachVonNam_DuocDuyetID = dt.iID_KeHoachVonNam_DuocDuyetID
	INNER JOIN VDT_DA_DuAn as da on dt.iID_DuAnID = da.iID_DuAnID
	INNER JOIN NS_DonVi as dv on da.iID_DonViQuanLyID = dv.iID_Ma
	LEFT JOIN VDT_DM_PhanCapDuAn as pc on da.iID_CapPheDuyetID = pc.iID_PhanCapID
	WHERE dv.iID_MaDonVi = @iIdMaDonVi AND tbl.iNamKeHoach = @iNamKeHoach AND tbl.iID_NguonVonID = @iIdNguonVon
	UNION ALL
	SELECT DISTINCT da.iID_DuAnID as IIDDuAnID, da.SMaDuAn, da.SDiaDiem, da.STenDuAn, ISNULL(pc.sMa, '') as sMaPhanCap, dt.iID_LoaiCongTrinh
	FROM VDT_QT_BCQuyetToanNienDo as tbl
	INNER JOIN VDT_QT_BCQuyetToanNienDo_ChiTiet_01 as dt on tbl.iID_BCQuyetToanNienDoID = dt.iID_BCQuyetToanNienDo
	INNER JOIN VDT_DA_DuAn as da on dt.iID_DuAnID = da.iID_DuAnID
	LEFT JOIN VDT_DM_PhanCapDuAn as pc on da.iID_CapPheDuyetID = pc.iID_PhanCapID
	WHERE iNamKeHoach = @iNamKeHoach AND (ISNULL(dt.fGiaTriNamTruocChuyenNamSau, 0) <> 0 OR ISNULL(dt.fGiaTriNamNayChuyenNamSau, 0) <> 0)

	SELECT DISTINCT IIDDuAnID as IIDDuAnID, SMaDuAn, SDiaDiem, STenDuAn, sMaPhanCap, iID_LoaiCongTrinh, CAST(0 as bit) BIsChuyenTiep INTO #tmp
	FROM #tmpUnion
	
	UPDATE tmp
	SET
		BIsChuyenTiep = 1
	FROM #tmp as tmp
	INNER JOIN (
		SELECT DISTINCT dt.iID_DuAnID 
		FROM VDT_KHV_PhanBoVon as tbl 
		INNER JOIN VDT_KHV_PhanBoVon_ChiTiet as dt on tbl.iID_PhanBoVonID = dt.iID_PhanBoVonID 
		WHERE tbl.bActive = 1 AND tbl.iNamKeHoach = (@iNamKeHoach - 1)
		) as mp on tmp.IIDDuAnID = mp.iID_DuAnID

	-- Bao cao nam truoc
	SELECT iID_DuAnID, dt.iID_LoaiCongTrinh,
		SUM(ISNULL(dt.fDuToanCNSChuaGiaiNganTaiKB, 0)) as FDuToanCNSChuaGiaiNganTaiKB,		-- col 1
		SUM(ISNULL(dt.fDuToanCNSChuaGiaiNganTaiDV, 0)) as FDuToanCNSChuaGiaiNganTaiDv,		-- col 2
		SUM(ISNULL(dt.fDuToanCNSChuaGiaiNganTaiCuc, 0)) as FDuToanCNSChuaGiaiNganTaiCuc		-- col 3
		INTO #tmpBaoCaoNamTruoc
	FROM VDT_QT_BCQuyetToanNienDo as tbl
	INNER JOIN VDT_QT_BCQuyetToanNienDo_PhanTich as dt on tbl.iID_BCQuyetToanNienDoID = dt.iID_BCQuyetToanNienDo
	INNER JOIN #tmp as tmp on dt.iID_DuAnID = tmp.IIDDuAnID AND dt.iID_LoaiCongTrinh = tmp.iID_LoaiCongTrinh
	WHERE tbl.iNamKeHoach = (@iNamKeHoach - 1) AND tbl.iID_NguonVonID = @iIdNguonVon
	GROUP BY iID_DuAnID, dt.iID_LoaiCongTrinh

	-- Gia tri nam truoc
	SELECT IIDDuAnID, tbl.iID_LoaiCongTrinh,
		(SUM(ISNULL(fLuyKeTamUngChuaThuHoi_KhvNam, 0)) - SUM(ISNULL(fLuyKeTamUngChuaThuHoi_KhvNamDelete, 0))) as fLuyKeTamUngChuaThuHoi_KhvNam,
		(SUM(ISNULL(fLuyKeTamUngChuaThuHoi_KhvNam_UQ, 0)) - SUM(ISNULL(fLuyKeTamUngChuaThuHoi_KhvNam_UQDelete, 0))) as fLuyKeTamUngChuaThuHoi_KhvNam_UQ ,
		(SUM(ISNULL(fTamUngChuaThuHoiKHVN, 0)) - SUM(ISNULL(fTamUngChuaThuHoiKHVNDelete, 0))) as fTamUngChuaThuHoiKHVN 
		INTO #tmpNamTruoc
	FROM
	(
		SELECT tmp.IIDDuAnID, tmp.iID_LoaiCongTrinh,
			(CASE WHEN sMaDich = '414a' AND sMaNguonCha = '302' AND sMaTienTrinh = '100' AND sMaPhanCap <> 'UQ' THEN ISNULL(fGiaTri, 0) ELSE 0 END) as fLuyKeTamUngChuaThuHoi_KhvNam,				-- col 18
			(CASE WHEN sMaNguon = '414a' AND sMaNguonCha = '302' AND sMaTienTrinh = '300' AND sMaPhanCap <> 'UQ' THEN ISNULL(fGiaTri, 0) ELSE 0 END) as fLuyKeTamUngChuaThuHoi_KhvNamDelete,

			(CASE WHEN sMaDich = '414a' AND sMaNguonCha = '302' AND sMaTienTrinh = '100' AND sMaPhanCap = 'UC' THEN ISNULL(fGiaTri, 0) ELSE 0 END) as fLuyKeTamUngChuaThuHoi_KhvNam_UQ,				-- col 19
			(CASE WHEN sMaNguon = '414a' AND sMaNguonCha = '302' AND sMaTienTrinh = '300' AND sMaPhanCap = 'UC' THEN ISNULL(fGiaTri, 0) ELSE 0 END) as fLuyKeTamUngChuaThuHoi_KhvNam_UQDelete,

			(CASE WHEN sMaDich = '413a' AND sMaNguonCha = '301' AND sMaTienTrinh = '100' THEN ISNULL(fGiaTri, 0) ELSE 0 END) as fTamUngChuaThuHoiKHVN,				-- col 19
			(CASE WHEN sMaNguon = '413a' AND sMaNguonCha = '301' AND sMaTienTrinh = '300' THEN ISNULL(fGiaTri, 0) ELSE 0 END) as fTamUngChuaThuHoiKHVNDelete

		FROM #tmp as tmp
		INNER JOIN VDT_TongHop_NguonNSDauTu as dt on tmp.IIDDuAnID = dt.iID_DuAnID AND tmp.iID_LoaiCongTrinh = dt.iID_LoaiCongTrinh
		WHERE dt.iID_NguonVonID = @iIdNguonVon AND dt.iNamKeHoach = (@iNamKeHoach - 1)
	) as tbl
	GROUP BY IIDDuAnID, tbl.iID_LoaiCongTrinh


	-- Gia tri nam nay
	SELECT IIDDuAnID, tbl.iID_LoaiCongTrinh,
		(SUM(ISNULL(fChiTieuNamNayKb, 0)) - SUM(ISNULL(fChiTieuNamNayKbDelete, 0))) as fChiTieuNamNayKb,
		(SUM(ISNULL(fChiTieuNamNayLc, 0)) - SUM(ISNULL(fChiTieuNamNayLcDelete, 0))) as fChiTieuNamNayLc,
		(SUM(ISNULL(fTamUngNamNayKB, 0)) - SUM(ISNULL(fTamUngNamNayKBDelete, 0))) as fTamUngNamNayKB,
		(SUM(ISNULL(fThuHoiUngNamNayKB, 0)) - SUM(ISNULL(fThuHoiUngNamNayKBDelete, 0))) as fThuHoiUngNamNayKB,
		(SUM(ISNULL(fTamUngNamNayLC, 0)) - SUM(ISNULL(fTamUngNamNayLCDelete, 0))) as fTamUngNamNayLC,
		(SUM(ISNULL(fThanhToanKHVNNamNay, 0)) - SUM(ISNULL(fThanhToanKHVNNamNayDelete, 0))) as fThanhToanKHVNNamNay,
		(SUM(ISNULL(fThanhToanKHVNChuyenSang, 0)) - SUM(ISNULL(fThanhToanKHVNChuyenSangDelete, 0))) as fThanhToanKHVNChuyenSang
		INTO #tmpNamNay
	FROM
	(
		SELECT tmp.IIDDuAnID, tmp.iId_LoaiCongTrinh,
			(CASE WHEN sMaNguon = '101' AND sMaTienTrinh = '200' THEN ISNULL(fGiaTri, 0) ELSE 0 END) as fChiTieuNamNayKb,		-- col 6
			(CASE WHEN sMaDich = '101' AND sMaTienTrinh = '300' THEN ISNULL(fGiaTri, 0) ELSE 0 END) as fChiTieuNamNayKbDelete,

			(CASE WHEN sMaNguon = '102' AND sMaTienTrinh = '200' THEN ISNULL(fGiaTri, 0) ELSE 0 END) as fChiTieuNamNayLc,		-- col 7
			(CASE WHEN sMaDich = '102' AND sMaTienTrinh = '300' THEN ISNULL(fGiaTri, 0) ELSE 0 END) as fChiTieuNamNayLcDelete,

			(CASE WHEN sMaDich = '202' AND sMaNguonCha = '112' AND sMaTienTrinh = '200' THEN ISNULL(fGiaTri, 0) ELSE 0 END) as fThanhToanKHVNChuyenSang,		-- col 10
			(CASE WHEN sMaNguon = '202' AND sMaNguonCha = '112' AND sMaTienTrinh = '300' THEN ISNULL(fGiaTri, 0) ELSE 0 END) as fThanhToanKHVNChuyenSangDelete,

			(CASE WHEN sMaNguon = '212a' AND iThuHoiTUCheDo in (1,2) AND sMaTienTrinh = '200' AND sMaNguonCha = '102' AND sMaPhanCap <> 'UQ' THEN ISNULL(fGiaTri, 0) ELSE 0 END) as fTamUngNamNayKB,		-- col 18
			(CASE WHEN sMaDich = '212a' AND iThuHoiTUCheDo in (1,2) AND sMaTienTrinh = '300' AND sMaNguonCha = '102' AND sMaPhanCap <> 'UQ' THEN ISNULL(fGiaTri, 0) ELSE 0 END) as fTamUngNamNayKBDelete,

			(CASE WHEN sMaNguon = '211a' AND iThuHoiTUCheDo in (1,2) AND sMaTienTrinh = '200' AND sMaNguonCha = '101' AND sMaPhanCap = 'UQ' THEN ISNULL(fGiaTri, 0) ELSE 0 END) as fThuHoiUngNamNayKB,		-- col 19
			(CASE WHEN sMaDich = '211a' AND iThuHoiTUCheDo in (1,2) AND sMaTienTrinh = '300' AND sMaNguonCha = '101' AND sMaPhanCap = 'UQ' THEN ISNULL(fGiaTri, 0) ELSE 0 END) as fThuHoiUngNamNayKBDelete,

			(CASE WHEN sMaDich = '212a' AND sMaTienTrinh = '200' AND sMaNguonCha = '102' AND sMaPhanCap <> 'UQ' THEN ISNULL(fGiaTri, 0) ELSE 0 END) as fTamUngNamNayLC,		-- col 18
			(CASE WHEN sMaNguon = '212a' AND sMaTienTrinh = '300' AND sMaNguonCha = '102' AND sMaPhanCap <> 'UQ' THEN ISNULL(fGiaTri, 0) ELSE 0 END) as fTamUngNamNayLCDelete, 

			(CASE WHEN sMaDich = '202' AND sMaTienTrinh = '200' AND sMaNguonCha = '102' THEN ISNULL(fGiaTri, 0) ELSE 0 END) as fThanhToanKHVNNamNay,		-- col 21
			(CASE WHEN sMaNguon = '202' AND sMaTienTrinh = '300' AND sMaNguonCha = '102' THEN ISNULL(fGiaTri, 0) ELSE 0 END) as fThanhToanKHVNNamNayDelete
		FROM #tmp as tmp
		INNER JOIN VDT_TongHop_NguonNSDauTu as dt on tmp.IIDDuAnID = dt.iID_DuAnID AND tmp.iID_LoaiCongTrinh = dt.iID_LoaiCongTrinh
		WHERE dt.iID_NguonVonID = @iIdNguonVon AND dt.iNamKeHoach = @iNamKeHoach
	) as tbl
	GROUP BY IIDDuAnID, tbl.iID_LoaiCongTrinh

	-- Thong tri nam nay
	SELECT IIDDuAnID, tbl.iID_LoaiCongTrinh,
		SUM(ISNULL(fCapHopThuc, 0)) as fCapHopThuc,
		SUM(ISNULL(fCapKinhPhi, 0)) as fCapKinhPhi,
		SUM(ISNULL(fCapHopThucBoXung, 0)) as fCapHopThucBoXung,
		SUM(ISNULL(fCapKinhPhiBoXung, 0)) as fCapKinhPhiBoXung,
		SUM(ISNULL(fCapHopThucChuyenSang, 0)) as fCapHopThucChuyenSang,
		SUM(ISNULL(fCapKinhPhiChuyenSang, 0)) as fCapKinhPhiChuyenSang
		INTO #tmpThongTri
	FROM
	(
		SELECT tmp.IIDDuAnID, tmp.iID_LoaiCongTrinh,
			(CASE WHEN tbl.iLoaiThongTri = 4 THEN ISNULL(dt.fSoTien, 0) ELSE 0 END) as fCapHopThuc,
			(CASE WHEN tbl.iLoaiThongTri = 3 THEN ISNULL(dt.fSoTien, 0) ELSE 0 END) as fCapKinhPhi,
			(CASE WHEN tbl.iLoaiThongTri = 4 AND tbl.iNamNganSach = 1 THEN ISNULL(dt.fSoTien, 0) ELSE 0 END) as fCapHopThucBoXung,
			(CASE WHEN tbl.iLoaiThongTri = 3 AND tbl.iNamNganSach = 1 THEN ISNULL(dt.fSoTien, 0) ELSE 0 END) as fCapKinhPhiBoXung,
			(CASE WHEN tbl.iLoaiThongTri = 4 AND tbl.iNamNganSach = 2 THEN ISNULL(dt.fSoTien, 0) ELSE 0 END) as fCapHopThucChuyenSang,
			(CASE WHEN tbl.iLoaiThongTri = 3 AND tbl.iNamNganSach = 2 THEN ISNULL(dt.fSoTien, 0) ELSE 0 END) as fCapKinhPhiChuyenSang
		FROM VDT_ThongTri as tbl
		INNER JOIN VDT_ThongTri_ChiTiet as dt on tbl.iID_ThongTriID = dt.iID_ThongTriID
		INNER JOIN #tmp as tmp on dt.iID_DuAnID = tmp.IIDDuAnID AND dt.iID_LoaiCongTrinhID = tmp.iID_LoaiCongTrinh
		LEFT JOIN NS_NguonNganSach as nv on tbl.sMaNguonVon = nv.sMoTa
		WHERE tbl.iNamThongTri = @iNamKeHoach 
			AND (nv.iID_MaNguonNganSach IS NULL OR nv.iID_MaNguonNganSach = @iIdNguonVon)
	) as tbl
	GROUP BY IIDDuAnID, tbl.iID_LoaiCongTrinh



	SELECT tmp.* , da.sTenDuAn, tmp.iID_LoaiCongTrinh, lct.SMaLoaiCongTrinh, lct.STenLoaiCongTrinh,
		ISNULL(bcNamTruoc.FDuToanCNSChuaGiaiNganTaiKB, 0) as FDuToanCnsChuaGiaiNganTaiKbNamTruoc,												-- col 1
		ISNULL(bcNamTruoc.fDuToanCNSChuaGiaiNganTaiDV, 0) as FDuToanCnsChuaGiaiNganTaiDvNamTruoc,												-- col 2
		ISNULL(bcNamTruoc.fDuToanCNSChuaGiaiNganTaiCuc, 0) as FDuToanCnsChuaGiaiNganTaiCucNamTruoc,												-- col 3
		ISNULL(nn.fChiTieuNamNayKB, 0) as FChiTieuNamNayKb,																						-- col 6
		ISNULL(nn.fChiTieuNamNayLC, 0) as FChiTieuNamNayLc,																						-- col 7
		(ISNULL(nn.fThanhToanKHVNChuyenSang, 0) + ISNULL(tt.fCapHopThucBoXung, 0) + ISNULL(fCapKinhPhiBoXung, 0)) as FSoCapNamTrcCs,			-- col 10
		(ISNULL(nn.fThanhToanKHVNNamNay, 0) + ISNULL(tt.fCapHopThucChuyenSang, 0) + ISNULL(tt.fCapKinhPhiChuyenSang, 0)) as FSoCapNamNay,		-- col 11
		CAST(0 as float) as FDnQuyetToanNamTrc,																									-- col 13
		CAST(0 as float) as FDnQuyetToanNamNay,																									-- col 14
		(ISNULL(fLuyKeTamUngChuaThuHoi_KhvNam, 0) - ISNULL(fTamUngNamNayKB, 0) + ISNULL(fTamUngNamNayLC, 0)) as FTuChuaThuHoiTaiCuc,			-- col 18
		(ISNULL(fLuyKeTamUngChuaThuHoi_KhvNam_UQ, 0) - ISNULL(fTamUngNamNayKB, 0) + ISNULL(fTamUngNamNayLC, 0)) as FTuChuaThuHoiTaiDonVi,		-- col 19
		(ISNULL(bcNamTruoc.fDuToanCNSChuaGiaiNganTaiCuc, 0) + ISNULL(nn.fChiTieuNamNayKB, 0)													-- col 21
			- (ISNULL(fLuyKeTamUngChuaThuHoi_KhvNam, 0) - ISNULL(fTamUngNamNayKB, 0) + ISNULL(fTamUngNamNayLC, 0))
			- fThanhToanKHVNNamNay) as FDuToanCnsChuaGiaiNganTaiCuc,
		(ISNULL(bcNamTruoc.FDuToanCNSChuaGiaiNganTaiDv, 0) + ISNULL(tt.fCapHopThuc, 0) + ISNULL(tt.fCapKinhPhi, 0)								-- col 22
			- (ISNULL(nn.fTamUngNamNayLC, 0)) + ISNULL(nn.fThanhToanKHVNNamNay, 0) - ISNULL(nn.fTamUngNamNayKB, 0)) as FDuToanCnsChuaGiaiNganTaiDv	,																						
		(ISNULL(bcNamTruoc.fDuToanCNSChuaGiaiNganTaiKB, 0) + ISNULL(nn.fChiTieuNamNayKB, 0) - ISNULL(tt.fCapHopThuc, 0)) as FDuToanCnsChuaGiaiNganTaiKb	,
		CAST(0 as float) as FDuToanThuHoi
	FROM #tmp as tmp
	INNER JOIN VDT_DA_DuAn as da on tmp.IIDDuAnID = da.iID_DuAnID
	LEFT JOIN #tmpBaoCaoNamTruoc as bcNamTruoc on tmp.IIDDuAnID = bcNamTruoc.iID_DuAnID AND tmp.iID_LoaiCongTrinh = bcNamTruoc.iID_LoaiCongTrinh
	LEFT JOIN #tmpThongTri as tt on tmp.IIDDuAnID = tt.IIDDuAnID AND tmp.iID_LoaiCongTrinh = tt.iID_LoaiCongTrinh
	LEFT JOIN #tmpNamNay as nn on tmp.IIDDuAnID = nn.IIDDuAnID AND tmp.iID_LoaiCongTrinh = nn.iID_LoaiCongTrinh
	LEFT JOIN #tmpNamTruoc as nt on tmp.IIDDuAnID = nt.IIDDuAnID AND tmp.iID_LoaiCongTrinh = nt.iID_LoaiCongTrinh
	LEFT JOIN VDT_DM_LoaiCongTrinh as lct on tmp.iID_LoaiCongTrinh = lct.iID_LoaiCongTrinh

	DROP TABLE #tmpNamNay
	DROP TABLE #tmpNamTruoc
	DROP TABLE #tmpUnion
	DROP TABLE #tmp
END