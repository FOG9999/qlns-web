--#DECLARE#--
/*
Lấy danh sách chi phí của gói thầu 
*/


select gt.iID_ChiPhiID,gt.fTienGoiThau,cp.sTenChiPhi ,
cp.iID_ChiPhi_Parent
from VDT_DA_GoiThau_ChiPhi gt
INNER JOIN VDT_DM_DuAn_ChiPhi cp ON cp.iID_DuAn_ChiPhi = gt.iID_ChiPhiID
where gt.iID_GoiThauID = @iID_GoiThauID
