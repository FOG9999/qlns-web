--#DECLARE#--

/*

Lấy all du an theo don vi quan ly va ngay quyet dinh - DNQT

*/

SELECT DISTINCT duan.iID_DuAnID, duan.sMaDuAn, duan.sTenDuAn, duan.sTrangThaiDuAn, duan.bIsKetThuc
FROM VDT_DA_DuAn duan 
inner join VDT_QT_DeNghiQuyetToan dnqt on duan.iID_DuAnID = dnqt.iID_DuAnID
WHERE duan.iID_DonViQuanLyID = @iID_DonViQuanLyID
	AND duan.iID_DuAnID not in (select iID_DuAnID FROM VDT_QT_QuyetToan)
	AND (dnqt.dThoiGianLapBaoCao is null or dnqt.dThoiGianLapBaoCao <= @dNgayQuyetDinh)