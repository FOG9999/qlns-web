
ALTER PROCEDURE [dbo].[sp_insert_tonghopnguondautu]
@iIDChungTu uniqueidentifier,
@sLoai nvarchar(100),
@data t_tbl_tonghopdautu_2 READONLY
AS
BEGIN
	DECLARE @iIDMaDonViQuanLy nvarchar(100)
	DECLARE @iIDNguonVonID int
	DECLARE @sSoQuyetDinh nvarchar(100)
	DECLARE @dNgayQuyetDinh DATETIME
	DECLARE @iNamKeHoach int

	IF (@sLoai = 'KHVN')
	BEGIN
		SELECT @iIDMaDonViQuanLy = iID_MaDonViQuanLy,
			@iIDNguonVonID = iID_NguonVonID,
			@sSoQuyetDinh = sSoQuyetDinh,
			@dNgayQuyetDinh = dNgayQuyetDinh,
			@iNamKeHoach = iNamKeHoach
		FROM VDT_KHV_PhanBoVon WHERE iID_PhanBoVonID = @iIDChungTu
	END
	ELSE IF (@sLoai = 'THANH_TOAN')
	BEGIN
		SELECT @iIDMaDonViQuanLy = iID_MaDonViQuanLy,
			@iIDNguonVonID = iID_NguonVonID,
			@sSoQuyetDinh = sSoDeNghi,
			@dNgayQuyetDinh = dNgayPheDuyet,
			@iNamKeHoach = iNamKeHoach
		FROM VDT_TT_DeNghiThanhToan WHERE iID_DeNghiThanhToanID = @iIDChungTu
	END

	INSERT INTO VDT_TongHop_NguonNSDauTu(iID_MaDonViQuanLy, iID_NguonVonID, sSoQuyetDinh, dNgayQuyetDinh, iNamKeHoach, iID_ChungTu, sMaNguon, sMaDich, sMaNguonCha, fGiaTri, iStatus, iID_DuAnID, iId_MaNguonCha, sMaTienTrinh, bIsLog, iThuHoiTUCheDo,ILoaiUng,bKeHoach,
										iID_MucID, iID_TieuMucID, iID_TietMucID, iID_NganhID, LNS, L, K, M, TM, TTM, NG, iID_LoaiCongTrinh)
	SELECT @iIDMaDonViQuanLy, @iIDNguonVonID, @sSoQuyetDinh, @dNgayQuyetDinh, @iNamKeHoach, @iIDChungTu, tbl.sMaNguon, tbl.sMaDich, tbl.sMaNguonCha, tbl.fGiaTri, tbl.iStatus, tbl.iID_DuAnID, tbl.iId_MaNguonCha, '200', 0, tbl.iThuHoiTUCheDo, tbl.ILoaiUng,
			(CASE WHEN sMaNguonCha in ('121a','122a') AND sMaDich in ('101', '102') THEN (CASE WHEN @sLoai = 'KHVN' THEN 1 ELSE 0 END) ELSE NULL END),
			tbl.IIDMucID, tbl.IIDTieuMucID, tbl.IIDTietMucID, tbl.IIDNganhID, ml.sLNS, ml.sL, ml.sK, ml.sM, ml.sTM, ml.sTTM, ml.sNG, tbl.IIDLoaiCongTrinh

	FROM @data as tbl
	LEFT JOIN NS_MucLucNganSach as ml on ml.iID_MaMucLucNganSach = tbl.IIDMucID OR ml.iID_MaMucLucNganSach = tbl.IIDTieuMucID OR ml.iID_MaMucLucNganSach = tbl.IIDTietMucID OR ml.iID_MaMucLucNganSach = tbl.IIDNganhID
END
