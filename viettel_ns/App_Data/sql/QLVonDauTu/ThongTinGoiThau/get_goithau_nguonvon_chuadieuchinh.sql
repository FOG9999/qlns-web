

select gt.iID_NguonVonID,gt.fTienGoiThau,nv.sTen as  sTenNguonVon
from VDT_DA_GoiThau_NguonVon gt
INNER JOIN NS_NguonNganSach nv ON nv.iID_MaNguonNganSach = gt.iID_NguonVonID
where gt.iID_GoiThauID = @iId