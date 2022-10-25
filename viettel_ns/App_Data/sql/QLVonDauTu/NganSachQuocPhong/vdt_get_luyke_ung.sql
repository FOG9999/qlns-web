--#DECLARE#--

/*

Lấy lũy kế ứng theo dự án và hợp đồng

*/

select ISNULL(sum(dnttuct.fGiaTriThanhToan), 0) as fLKSoVonDaTamUng,
		ISNULL(sum(dnttuct.fGiaTriThuHoiUngNgoaiChiTieu), 0) as fLKThuHoiUng
from VDT_TT_DeNghiThanhToanUng_ChiTiet dnttuct
left join VDT_TT_DeNghiThanhToanUng dnttu on dnttuct.iID_DeNghiThanhToanID = dnttu.iID_DeNghiThanhToanID
where dnttuct.iID_DuAnID = @iID_DuAnID
and (@iID_HopDongID is null or dnttuct.iID_HopDongID = @iID_HopDongID)
and dnttu.iID_DonViQuanLyID = @iID_DonViQuanLyID
and dnttu.iID_NhomQuanLyID = @iID_NhomQuanLyID
and dnttu.dNgayDeNghi <= @dNgayQuyetDinh