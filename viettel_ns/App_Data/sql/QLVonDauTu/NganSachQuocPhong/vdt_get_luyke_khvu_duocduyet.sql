--#DECLARE#--

/*

Lấy all du an theo don vi quan ly va ngay quyet dinh của kế hoạch vốn ứng

*/

select ISNULL(sum(khvuct.fGiaTriUng), 0)
from VDT_KHV_KeHoachVonUng_ChiTiet khvuct
left join VDT_KHV_KeHoachVonUng khvu on khvuct.iID_KeHoachUngID = khvu.iID_KeHoachUngID
where khvuct.iID_DuAnID = @iID_DuAnID
and khvu.iID_DonViQuanLyID = @iID_DonViQuanLyID
and khvu.iID_NhomQuanLyID = @iID_NhomQuanLyID
and khvu.dNgayQuyetDinh <= @dNgayQuyetDinh
