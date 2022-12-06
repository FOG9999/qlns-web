--#DECLARE#--

/*

Lấy record VDT_DA_QDDauTu theo ID du an

*/

select iID_QDDauTuID,
			iID_DuAnID,
			sSoQuyetDinh,
			dNgayQuyetDinh,
			sSoToTrinh,
			sSoThamDinh,
			sCoQuanPheDuyet,
			(select sum(fTongMucDauTuPheDuyet) from VDT_DA_QDDauTu where iID_DuAnID = @iID_DuAnID and bIsGoc = 1 GROUP BY iID_DuAnID) as fTongMucDauTuPheDuyet,
			(select sum(fTongMucDauTuPheDuyet) from VDT_DA_QDDauTu where iID_DuAnID = @iID_DuAnID) as fTongMucDauTuPheDuyetCuoi,
			(select count(*) from VDT_DA_QDDauTu where iID_DuAnID = @iID_DuAnID and bIsGoc <> 1) as iSoLanDieuChinh,
			sNguoiKy
from VDT_DA_QDDauTu 
where iID_DuAnID = @iID_DuAnID
order by bIsGoc DESC

