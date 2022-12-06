--#DECLARE#--

/*

Lấy all quyet toan chi phi theo id quyet toan

*/

select qtnv.iID_QuyetToan_NguonVonID,
			qtnv.iID_QuyetToanID,
			qtnv.iID_NguonVonID,
			qtnv.fTienPheDuyet,
			nns.sTen
from VDT_QT_QuyetToan_Nguonvon qtnv
left join NS_NguonNganSach nns on qtnv.iID_NguonVonID = nns.iID_MaNguonNganSach
where qtnv.iID_QuyetToanID = @iID_QuyetToanID