
--#DECLARE#--
/*

Lấy danh sách chi phí theo dự án

*/

select distinct ns.iID_MaNguonNganSach,ns.sTen
from  VDT_DA_QDDauTu dt 
inner join VDT_DA_QDDauTu_NguonVon dtnv ON dtnv.iID_QDDauTuID = dt.iID_QDDauTuID
inner join NS_NguonNganSach ns ON ns.iID_MaNguonNganSach = dtnv.iID_NguonVonID
where dt.iID_DuAnID = @iID_DuAnID AND dt.dNgayQuyetDinh <= @dNgayLap