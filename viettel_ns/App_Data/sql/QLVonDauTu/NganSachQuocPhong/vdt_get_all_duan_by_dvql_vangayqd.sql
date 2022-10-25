--#DECLARE#--

/*

Lấy all du an theo don vi quan ly va ngay quyet dinh

*/

SELECT DISTINCT duan.iID_DuAnID, duan.sMaDuAn, duan.sTenDuAn, duan.sTrangThaiDuAn, duan.bIsKetThuc
FROM VDT_DA_DuAn duan 
inner join VDT_DA_QDDauTu qddt on duan.iID_DuAnID = qddt.iID_DuAnID
WHERE iID_DonViQuanLyID = @iID_DonViQuanLyID
and (qddt.dNgayQuyetDinh is null or qddt.dNgayQuyetDinh <= @dNgayQuyetDinh)