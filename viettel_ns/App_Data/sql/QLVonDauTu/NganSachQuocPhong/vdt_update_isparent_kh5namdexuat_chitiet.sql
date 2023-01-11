

--#DECLARE#--

/*

update isparent

*/

UPDATE VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet SET bIsParent = 0
where iID_KeHoach5Nam_DeXuat_ChiTietID not in (
select DISTINCT iID_ParentID
	from VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet
	where iID_ParentID is not null
	AND iID_KeHoach5Nam_DeXuatID = @iID_KeHoach5NamID)
AND iID_KeHoach5Nam_DeXuatID = @iID_KeHoach5NamID and iLevel != 1;

UPDATE VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet SET bIsParent = 1
where iID_KeHoach5Nam_DeXuat_ChiTietID in (
select DISTINCT iID_ParentID
	from VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet
	where iID_ParentID is not null
	AND iID_KeHoach5Nam_DeXuatID = @iID_KeHoach5NamID and iID_KeHoach5Nam_DeXuat_ChiTietID != iID_ParentID)
AND iID_KeHoach5Nam_DeXuatID = @iID_KeHoach5NamID;