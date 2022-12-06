
--#DECLARE#--
/*

Lấy danh sách thông tin gói thầu

*/

select gt.* 
from VDT_DA_GoiThau gt
left join VDT_DA_DuAn da ON da.iID_DuAnID = gt.iID_DuAnID
left join VDT_DM_NhaThau nt ON nt.iID_NhaThauID = gt.iID_NhaThauID
where gt.iID_GoiThauID = @iId
