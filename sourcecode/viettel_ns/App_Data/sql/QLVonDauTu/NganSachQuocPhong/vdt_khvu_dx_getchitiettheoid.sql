DECLARE @iId uniqueidentifier set @iId= '257d8072-13eb-439a-a143-adeb00906893'
DECLARE @iNamLamViec int set @iNamLamViec = 2020

--#DECLARE#--

SELECT khvu.*, dv.sTen as sTenDonVi, nv.sTen as sTenNguonVon
FROM VDT_KHV_KeHoachVonUng_DX khvu
LEFT JOIN NS_DonVi as dv on khvu.iID_MaDonViQuanLy = dv.iID_MaDonVi AND dv.iNamLamViec_DonVi = @iNamLamViec
LEFT JOIN NS_NguonNganSach as nv on khvu.iID_NguonVonID = nv.iID_MaNguonNganSach
WHERE khvu.Id= @iId




