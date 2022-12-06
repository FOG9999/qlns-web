--#DECLARE#--

/*

Lấy record VDT_DA_DuToan theo ID du an

*/

select iID_DuToanID,
			iID_DuAnID,
			sSoQuyetDinh,
			dNgayQuyetDinh,
			sCoQuanPheDuyet,
			fTongDuToanPheDuyet,
			(select sum(fTongDuToanPheDuyet) from VDT_DA_DuToan where iID_DuAnID = @iID_DuAnID) as fTongDuToanPheDuyetCuoi,
			sNguoiKy
from VDT_DA_DuToan
where 
--bLaTongDuToan = 1 and
iID_DuAnID = @iID_DuAnID
order by bIsGoc DESC


