
select gt.iID_GoiThauID as iID_DuAn_GoiThau,
	gt.sTenGoiThau,
	gt.iID_GoiThauID as iID_GoiThauID,
	gt.fTienTrungThau as fTienPheDuyet,
	0 as fGiaTriDieuChinh,
	0 as fTienPheDuyetQDDT,
	0 as isDelete,
	1 as isParent
from VDT_DA_GoiThau gt
where gt.iId_KHLCNhaThau = @nhaThauId

