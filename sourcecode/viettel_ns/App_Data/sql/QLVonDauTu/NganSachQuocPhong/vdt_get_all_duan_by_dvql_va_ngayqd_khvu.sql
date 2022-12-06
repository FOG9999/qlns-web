--#DECLARE#--

/*

Lấy all du an theo don vi quan ly va ngay quyet dinh của kế hoạch vốn ứng

*/

SELECT DISTINCT duan.iID_DuAnID, duan.sMaDuAn, duan.sTenDuAn,
					--khvu.dNgayQuyetDinh,
					duan.iID_CapPheDuyetID,
					pcda.sMa as sMaCapPheDuyet
FROM VDT_DA_DuAn duan 
inner join VDT_KHV_KeHoachVonUng_ChiTiet khvuct on duan.iID_DuAnID = khvuct.iID_DuAnID
inner join VDT_KHV_KeHoachVonUng khvu on khvuct.iID_KeHoachUngID = khvu.iID_KeHoachUngID
left join VDT_DM_PhanCapDuAn pcda on duan.iID_CapPheDuyetID = pcda.iID_PhanCapID
WHERE duan.iID_DonViQuanLyID = @iID_DonViQuanLyID
and (khvu.dNgayQuyetDinh is null or khvu.dNgayQuyetDinh <= @dNgayQuyetDinh)
