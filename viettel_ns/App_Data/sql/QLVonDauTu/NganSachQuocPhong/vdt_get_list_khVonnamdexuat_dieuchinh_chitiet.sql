--DECLARE @iNamKeHoach INT;
--SELECT @iNamKeHoach = iNamKehoach FROM VDT_KHV_KeHoachVonNam_DeXuat WHERE iID_KeHoachVonNamDeXuatID = @iID_KeHoachVonNamDeXuatID
--SELECT DISTINCT
--dxct.*
--From VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet dxct 
--inner join VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet ddct on ddct.iID_DuAnID = dxct.iID_DuAnID
--where ddct.iID_PhanBoVon_DonVi_PheDuyet_ID in (select khvndd.Id from VDT_KHV_PhanBoVon_DonVi_PheDuyet khvndd where iNamKeHoach = @iNamKeHoach)
SELECT DISTINCT
dxct.*
From VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet dxct 
inner join VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet ddct on ddct.iID_DuAnID = dxct.iID_DuAnID
where ddct.iID_DuAnID in (select ddct.iID_DuAnID from VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet ddct where iID_KeHoachVonNamDeXuatID = @iID_KeHoachVonNamDeXuatID)