select
	qdhm.iID_QDDauTu_HangMuciID,
	qdhm.iID_DuAn_ChiPhi,
	qdhm.iID_DuAn_ChiPhi,
	qdhm.iID_HangMucID,
	qdhm.fTienPheDuyet
from
	VDT_DA_QDDauTu_HangMuc qdhm
where
	qdhm.iID_QDDauTuID = @iID_QDDauTuID
	and qdhm.iID_DuAn_ChiPhi = @iID_DuAn_ChiPhi 