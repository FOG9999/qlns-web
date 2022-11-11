
--#DECLARE#--
/*

Lấy danh sách thông tin gói thầu

*/

--SELECT * 
--FROM VDT_DA_DuAn da
--inner join VDT_DA_QDDauTu dt ON dt.iID_DuAnID = da.iID_DuAnID
--WHERE iID_DonViQuanLyID = @iID_DonViQuanLyID

SELECT distinct da.iID_DuAnID,da.sTenDuAn ,da.sMaDuAn
FROM VDT_DA_DuAn da
inner join VDT_DA_QDDauTu dt ON dt.iID_DuAnID = da.iID_DuAnID
WHERE iID_DonViQuanLyID = @iID_DonViQuanLyID

--SELECT distinct da.iID_DuAnID,da.sTenDuAn ,da.sMaDuAn
--FROM VDT_DA_DuAn da
--where da.iID_DonViQuanLyID =  @iID_DonViQuanLyID
--and da.iID_DuAnID in (select iID_DuAnID from VDT_QDDT_KHLCNhaThau)
