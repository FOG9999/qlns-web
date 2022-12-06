DECLARE @iID_KeHoachUngID uniqueidentifier set @iID_KeHoachUngID = ''
DECLARE @iNamLamViec int set @iNamLamViec = 2022

--#DECLARE#--

select khvu.Id as iID_KeHoachUngID, khvu.sSoQuyetDinh, khvu.dNgayQuyetDinh,
			khvu.iNamKeHoach, khvu.fGiaTriUng,
			dv.sTen as sTenDonViQuanLy,
			nv.sTen as sTenNguonVon,
			khvu_dx.sSoDeNghi as sSoDeNghi_KHVUDX
from VDT_KHV_KeHoachVonUng khvu
LEFT JOIN NS_DonVi dv on khvu.iID_MaDonViQuanLy = dv.iID_MaDonVi AND dv.iNamLamViec_DonVi = @iNamLamViec
INNER JOIN NS_NguonNganSach nv ON nv.iID_MaNguonNganSach = khvu.iID_NguonVonID
INNER JOIN VDT_KHV_KeHoachVonUng_DX khvu_dx ON khvu_dx.Id = khvu.iID_KeHoachVonUngDeXuatID
where khvu.Id = @iID_KeHoachUngID