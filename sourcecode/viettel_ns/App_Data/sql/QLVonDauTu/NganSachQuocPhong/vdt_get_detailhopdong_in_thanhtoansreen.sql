

--#DECLARE#--

/*

Lấy thông tin dự án - hợp đồng trên màn hình Giải ngân - Thanh Toán
*/

DECLARE @fGiaTriHD decimal
DECLARE @fDaThanhToanTrongNam decimal
DECLARE @fDaTamUng decimal
DECLARE @fDaThuHoi decimal
DECLARE @fLuyKeThanhToanKLHT decimal
DECLARE @fDuToanGoiThau decimal


--- Gia tri hop dong
SET @fGiaTriHD = (SELECT SUM(child.fTienHopDong) as fGiaTriHD
					FROM VDT_DA_TT_HopDong as hd
					INNER JOIN VDT_DA_TT_HopDong as child on hd.iID_HopDongGocID = child.iID_HopDongGocID
					WHERE hd.iID_HopDongID = @iID_HopDongID)

SELECT @fDaThanhToanTrongNam = SUM(ISNULL(dt.fGiaTriThanhToan,0)),
		@fDaTamUng = SUM(ISNULL(dt.fGiaTriTamUng,0)),
		@fDaThuHoi = SUM(ISNULL(dt.fGiaTriThuHoi,0))
FROM VDT_TT_DeNghiThanhToan as tbl
INNER JOIN VDT_TT_DeNghiThanhToan_ChiTiet as dt on dt.iID_DeNghiThanhToanID = tbl.iID_DeNghiThanhToanID
WHERE tbl.dNgayDeNghi <= @dNgayDeNghi 
	AND tbl.iNamKeHoach = @iNamKeHoach
	AND tbl.iID_NguonVonID = @iID_NguonVonID
	and tbl.iid_loainguonvonid = @iid_loainguonvonid
	AND dt.iID_NganhID = @iID_NganhID
	AND dt.iID_HopDongID = @iID_HopDongID 
	AND dt.iID_DuAnID = @iID_DuAnID

SET @fLuyKeThanhToanKLHT =(SELECT SUM(ISNULL(dt.fGiaTriThanhToan,0))
							FROM VDT_TT_DeNghiThanhToan as tbl
							INNER JOIN VDT_TT_DeNghiThanhToan_ChiTiet as dt on dt.iID_DeNghiThanhToanID = tbl.iID_DeNghiThanhToanID
							WHERE tbl.dNgayDeNghi <= @dNgayDeNghi 
								AND tbl.iNamKeHoach <= @iNamKeHoach 
								AND tbl.iID_NguonVonID = @iID_NguonVonID
								AND tbl.iID_LoaiNguonVonID = @iID_LoaiNguonVonID
								AND dt.iID_NganhID = @iID_NganhID
								AND dt.iID_HopDongID = @iID_HopDongID 
								AND dt.iID_DuAnID = @iID_DuAnID)

SET @fDuToanGoiThau = (SELECT SUM(ISNULL(dt.fTienTrungThau,0))
						FROM VDT_DA_TT_HopDong as hd
						INNER JOIN VDT_DA_GoiThau as gt on hd.iID_GoiThauID = gt.iID_GoiThauID AND gt.dNgayQuyetDinh <= @dNgayDeNghi
						INNER JOIN VDT_DA_GoiThau as dt on gt.iID_GoiThauGocID = dt.iID_GoiThauGocID AND dt.dNgayQuyetDinh <= @dNgayDeNghi
						WHERE hd.iID_HopDongID = @iID_HopDongID)

SELECT hd.*, 
		gt.sTenGoiThau,
		nt.sTenNhaThau,
		nt.sNguoiLienHe as sDonViThuHuong,
		nt.sNganHang,
		nt.sSoTaiKhoan as sSoTaiKhoanNhaThau,
		ISNULL(@fGiaTriHD,0) as fGiaTriHD,
		ISNULL(@fDaThanhToanTrongNam,0)  as fDaThanhToanTrongNam,
		ISNULL(@fDaTamUng,0) as fDaTamUng,
		ISNULL(@fDaThuHoi,0) as fDaThuHoi,
		ISNULL(@fLuyKeThanhToanKLHT,0) as fLuyKeThanhToanKLHT,
		ISNULL(@fDuToanGoiThau,0) as fDuToanGoiThau
FROM VDT_DA_TT_HopDong as hd
LEFT JOIN VDT_DA_GoiThau as gt on hd.iID_GoiThauID = gt.iID_GoiThauID
LEFT JOIN VDT_DM_NhaThau as nt on hd.iID_NhaThauThucHienID = nt.iID_NhaThauID
WHERE hd.iID_HopDongID = @iID_HopDongID

