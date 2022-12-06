
 select gt.iID_HangMucID, gt.iID_ChiPhiID, gt.fTienGoiThau,hm.sTenHangMuc, hm.maOrder as maOrder
from VDT_DA_GoiThau_HangMuc gt
INNER JOIN VDT_DA_DuToan_DM_HangMuc hm ON hm.Id = gt.iID_HangMucID
where gt.iID_GoiThauID = @iId

union

select gt.iID_HangMucID, gt.iID_ChiPhiID, gt.fTienGoiThau,hm.sTenHangMuc, hm.smaOrder as maOrder
from VDT_DA_GoiThau_HangMuc gt
INNER JOIN VDT_DA_QDDauTu_DM_HangMuc hm ON hm.iID_QDDauTu_DM_HangMucID = gt.iID_HangMucID
where gt.iID_GoiThauID = @iId

--union

-- select gt.iID_HangMucID, gt.iID_ChiPhiID, gt.fTienGoiThau,hm.sTenHangMuc, hm.maOrder as maOrder
--from VDT_DA_GoiThau_HangMuc gt
--INNER JOIN VDT_DA_ChuTruongDauTu_HangMuc hm ON hm.iID_ChuTruongDauTu_HangMucID = gt.iID_HangMucID
--where gt.iID_GoiThauID = @iId

order by maOrder