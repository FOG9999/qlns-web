
--#DECLARE#--
/*

Lấy danh sách thông tin gói thầu

*/

delete VDT_DA_GoiThau_ChiPhi where iID_GoiThauID = @iId 

delete VDT_DA_GoiThau_NguonVon where iID_GoiThauID = @iId

delete VDT_DA_GoiThau_HangMuc where iID_GoiThauID = @iId