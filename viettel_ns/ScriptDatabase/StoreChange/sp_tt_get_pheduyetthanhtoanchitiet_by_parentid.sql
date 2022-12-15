
ALTER PROCEDURE [dbo].[sp_tt_get_pheduyetthanhtoanchitiet_by_parentid]
@uIdPheDuyet uniqueidentifier
AS
BEGIN
	SELECT (CASE 
				WHEN tbl.iLoaiThanhToan = 0 THEN 2 
				WHEN ISNULL(dt.fGiaTriThanhToanNN, 0) != 0 OR ISNULL(dt.fGiaTriThanhToanTN, 0) != 0 THEN 1
				WHEN ISNULL(dt.fGiaTriThuHoiNamTruocNN, 0) != 0 OR ISNULL(dt.fGiaTriThuHoiNamTruocTN, 0) != 0 THEN 4
				WHEN ISNULL(dt.fGiaTriThuHoiNamNayNN, 0) != 0 OR ISNULL(dt.fGiaTriThuHoiNamNayTN, 0) != 0 THEN 5
				WHEN ISNULL(dt.fGiaTriThuHoiUngTruocNamTruocNN, 0) != 0 OR ISNULL(dt.fGiaTriThuHoiUngTruocNamTruocTN, 0) != 0 THEN 6
				WHEN ISNULL(dt.fGiaTriThuHoiUngTruocNamNayNN, 0) != 0 OR ISNULL(dt.fGiaTriThuHoiUngTruocNamNayTN, 0) != 0 THEN 7
				ELSE 0 
			END) as iLoaiDeNghi,
			(CASE WHEN dt.iLoaiKeHoachVon >=3 THEN 1 
				WHEN dt.iLoaiKeHoachVon <=2 THEN 2 
			END) as iLoaiNamKH,
			dt.iID_KeHoachVonID, 
			(CASE 
				WHEN ISNULL(dt.fGiaTriThanhToanNN, 0) != 0 THEN ISNULL(dt.fGiaTriThanhToanNN, 0)
				WHEN ISNULL(dt.fGiaTriThuHoiNamTruocNN, 0) != 0 THEN ISNULL(dt.fGiaTriThuHoiNamTruocNN, 0)
				WHEN ISNULL(dt.fGiaTriThuHoiNamNayNN, 0) != 0 THEN ISNULL(dt.fGiaTriThuHoiNamNayNN, 0)
				WHEN ISNULL(dt.fGiaTriThuHoiUngTruocNamTruocNN, 0) != 0 THEN ISNULL(dt.fGiaTriThuHoiUngTruocNamTruocNN, 0)
				WHEN ISNULL(dt.fGiaTriThuHoiUngTruocNamNayNN, 0) != 0 THEN ISNULL(dt.fGiaTriThuHoiUngTruocNamNayNN, 0)
				ELSE 0 
			END) as fGiaTriNgoaiNuoc,
			(CASE 
				WHEN ISNULL(dt.fGiaTriThanhToanTN, 0) != 0 THEN ISNULL(dt.fGiaTriThanhToanTN, 0)
				WHEN ISNULL(dt.fGiaTriThuHoiNamTruocTN, 0) != 0 THEN ISNULL(dt.fGiaTriThuHoiNamTruocTN, 0)
				WHEN ISNULL(dt.fGiaTriThuHoiNamNayTN, 0) != 0 THEN ISNULL(dt.fGiaTriThuHoiNamNayTN, 0)
				WHEN ISNULL(dt.fGiaTriThuHoiUngTruocNamTruocTN, 0) != 0 THEN ISNULL(dt.fGiaTriThuHoiUngTruocNamTruocTN, 0)
				WHEN ISNULL(dt.fGiaTriThuHoiUngTruocNamNayTN, 0) != 0 THEN ISNULL(dt.fGiaTriThuHoiUngTruocNamNayTN, 0)
				ELSE 0 
			END) as fGiaTriTrongNuoc, 
			dt.sGhiChu,
			ml.sLNS, ml.sL, ml.sK, ml.sM, ml.sTM, ml.sTTM, ml.sNG,
			ml.sMoTa,
			dt.iID_MucID, dt.iID_TieuMucID, dt.iID_TietMucID, dt.iID_NganhID
	FROM VDT_TT_DeNghiThanhToan as tbl
	INNER JOIN VDT_TT_PheDuyetThanhToan_ChiTiet as dt on tbl.iID_DeNghiThanhToanID = dt.iID_DeNghiThanhToanID
	LEFT JOIN NS_MucLucNganSach as ml on  dt.iID_MucID = ml.iID_MaMucLucNganSach
									OR dt.iID_TieuMucID = ml.iID_MaMucLucNganSach
									OR dt.iID_TietMucID = ml.iID_MaMucLucNganSach
									OR dt.iID_NganhID = ml.iID_MaMucLucNganSach
									or 
									(
										dt.slns = isnull(ml.slns,'')
										and dt.sl = isnull(ml.sl,'')
										and dt.sk = isnull(ml.sk,'')
										and dt.sm = isnull(ml.sm,'')
										and dt.stm = isnull(ml.stm,'')
										and dt.sttm = isnull(ml.sttm,'')
										and dt.sng = isnull(ml.sng,'')
										and ml.stng = null
									)
	WHERE tbl.iID_DeNghiThanhToanID = @uIdPheDuyet
END