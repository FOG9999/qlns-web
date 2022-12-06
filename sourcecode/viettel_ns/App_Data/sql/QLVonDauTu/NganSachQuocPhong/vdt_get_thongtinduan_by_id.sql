

--#DECLARE#--

/*

Lấy thông tin dự án - hợp đồng trên màn hình Giải ngân - Thanh Toán
*/

SELECT da.iID_DuAnID,
			da.sDiaDiem,
			da.sKhoiCong,
			da.sKetThuc,
			--(select ISNULL(sum(fTienPheDuyet), 0) from VDT_DA_QDDauTu_ChiPhi
			--	where iID_QDDauTuID in (select iID_QDDauTuID from VDT_DA_QDDauTu where iID_DuAnID = @iID_DuAnID)) as fTongMucDauTu
			da.fTongMucDauTu
FROM VDT_DA_DuAn da
WHERE da.iID_DuAnID = @iID_DuAnID
