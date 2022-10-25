
--Lấy danh sách Chứng từ

declare @sTongHop nvarchar(max);
select @sTongHop =  sTongHop  from VDT_KHV_KeHoachVonNam_DeXuat where iID_KeHoachVonNamDeXuatID = @iID_KeHoachVonNamDeXuatID;

SELECT * , dv.sTen as sTenDonVi, ns.sTen as sTenNguonVon FROM splitstring(@sTongHop) as tmp
INNER JOIN VDT_KHV_KeHoachVonNam_DeXuat as dx on tmp.Name = dx.iID_KeHoachVonNamDeXuatID
INNER JOIN NS_DonVi as dv on dv.iID_Ma = dx.iID_DonViQuanLyID
INNER JOIN NS_NguonNganSach as ns on ns.iID_MaNguonNganSach = dx.iID_NguonVonID
