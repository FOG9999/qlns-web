CREATE PROC sp_vdt_tt_export_denghithanhtoan_by_id
@iId t_tbl_Ids READONLY
AS
BEGIN
	SELECT DISTINCT iID_DeNghiThanhToanID, tbl.iLoai,
		(CASE WHEN khvn.iID_PhanBoVonID IS NOT NULL THEN khvn.sSoQuyetDinh
			WHEN khvu.Id IS NOT NULL THEN khvu.sSoQuyetDinh
			WHEN qt.iID_BCQuyetToanNienDoID IS NOT NULL THEN qt.sSoDeNghi
		END) as sSoQuyetDinh INTO #tmp
	FROM @iId as tmp
	INNER JOIN VDT_TT_DeNghiThanhToan_KHV as tbl on tmp.iId = tbl.iID_DeNghiThanhToanID
	LEFT JOIN VDT_KHV_PhanBoVon as khvn on tbl.iID_KeHoachVonID = khvn.iID_PhanBoVonID AND tbl.iLoai = 1
	LEFT JOIN VDT_KHV_KeHoachVonUng as khvu on tbl.iID_KeHoachVonID = khvu.Id AND tbl.iLoai = 2
	LEFT JOIN VDT_QT_BCQuyetToanNienDo as qt on tbl.iID_KeHoachVonID = qt.iID_BCQuyetToanNienDoID AND tbl.iLoai in (3,4)


	SELECT 
	  iID_DeNghiThanhToanID,
	  STUFF((
		SELECT '; ' + sSoQuyetDinh
		FROM #tmp 
		WHERE (iID_DeNghiThanhToanID = Results.iID_DeNghiThanhToanID) 
		FOR XML PATH(''),TYPE).value('(./text())[1]','VARCHAR(MAX)')
	  ,1,2,'') AS sKeHoachVon ,
	  STUFF((
		SELECT '; ' + CAST(iLoai as nvarchar(5))
		FROM #tmp 
		WHERE (iID_DeNghiThanhToanID = Results.iID_DeNghiThanhToanID) 
		FOR XML PATH(''),TYPE).value('(./text())[1]','VARCHAR(MAX)')
	  ,1,2,'') AS sLoaiKeHoachVon
	  INTO #tmpKhv
	FROM #tmp Results
	GROUP BY iID_DeNghiThanhToanID

	SELECT 
		ROW_NUMBER() OVER(ORDER BY tbl.iID_DeNghiThanhToanID DESC) as iSTT,
		tbl.*, ns.sTen as sNguonVon, lnv.sMoTa as sLoaiNguonVon, dv.sTen as sTenDonVi, 
		da.sTenDuAn, hd.sSoHopDong, hd.dNgayHopDong, hd.fTienHopDong as fGiaTriHopDong, nt.sMaNhaThau, da.sMaDuAn, khv.sKeHoachVon, khv.sLoaiKeHoachVon, tbl.iID_ChiPhiID as IIdChiPhiId,
		hd.sTenHopDong, pdtt.fGiaTriThanhToanTNDuocDuyet, pdtt.fGiaTriThanhToanNNDuocDuyet
	FROM @iId as tmp
	INNER JOIN VDT_TT_DeNghiThanhToan as tbl on tmp.iId = tbl.iID_DeNghiThanhToanID
	left join 
	(
	select iID_DeNghiThanhToanID,sum(pdtt.fGiaTriThanhToanTN) as fGiaTriThanhToanTNDuocDuyet, sum(pdtt.fGiaTriThanhToanNN) as fGiaTriThanhToanNNDuocDuyet
	from VDT_TT_PheDuyetThanhToan_ChiTiet pdtt
	group by iID_DeNghiThanhToanID
	)
	pdtt on tbl.iID_DeNghiThanhToanID = pdtt.iID_DeNghiThanhToanID
	LEFT JOIN NS_NguonNganSach as ns on tbl.iID_NguonVonID = ns.iID_MaNguonNganSach
	LEFT JOIN NS_MucLucNganSach as lnv on tbl.iID_LoaiNguonVonID = lnv.iID_MaMucLucNganSach
	LEFT JOIN NS_DonVi as dv on tbl.iID_DonViQuanLyID = dv.iID_Ma
	LEFT JOIN VDT_DA_DuAn as da on tbl.iID_DuAnId = da.iID_DuAnID
	LEFT JOIN VDT_DA_TT_HopDong as hd on tbl.iID_HopDongId = hd.iID_HopDongID
	LEFT JOIN VDT_DM_NhaThau as nt on tbl.iID_NhaThauId = nt.iID_NhaThauID
	LEFT JOIN #tmpKhv as khv on tbl.iID_DeNghiThanhToanID = khv.iID_DeNghiThanhToanID
END