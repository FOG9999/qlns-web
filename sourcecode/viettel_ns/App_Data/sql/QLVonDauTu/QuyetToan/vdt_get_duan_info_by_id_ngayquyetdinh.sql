--#DECLARE#--

/*

Lấy thong du an theo id va ngay quyet dinh

*/

-- Tong muc dau tu
select iID_DuAnID, ISNULL(sum(fTongMucDauTuPheDuyet), 0) as fTongMucDauTuPheDuyet into #TEMP_tmdt
from VDT_DA_QDDauTu
where iID_DuAnID = @iID_DuAnID
group by iID_DuAnID;

-- Tinh Ke hoach ung
select khvuct.iID_DuAnID, ISNULL(sum(khvuct.fGiaTriUng), 0) as fGiaTriUng into #TEMP_khu 
from VDT_KHV_KeHoachVonUng_ChiTiet khvuct
--left join VDT_KHV_KeHoachVonUng khvu on khvuct.iID_KeHoachUngID = khvu.iID_KeHoachUngID
left join VDT_KHV_KeHoachVonUng khvu on khvuct.iID_KeHoachUngID = khvu.Id
where khvuct.iID_DuAnID = @iID_DuAnID
and khvu.iID_DonViQuanLyID = @iID_DonViQuanLyID
and khvu.dNgayQuyetDinh <= @dNgayQuyetDinh
group by khvuct.iID_DuAnID;

-- Von ung da cap, da thu hoi
select dnttuct.iID_DuAnID,
		ISNULL(sum(dnttuct.fGiaTriThanhToan), 0) as fLKSoVonDaTamUng,
		ISNULL(sum(dnttuct.fGiaTriThuHoiUngNgoaiChiTieu), 0) as fLKThuHoiUng into #TEMP_vonung
from VDT_TT_DeNghiThanhToanUng_ChiTiet dnttuct
left join VDT_TT_DeNghiThanhToanUng dnttu on dnttuct.iID_DeNghiThanhToanID = dnttu.iID_DeNghiThanhToanID
where dnttuct.iID_DuAnID = @iID_DuAnID
and dnttu.iID_DonViQuanLyID = @iID_DonViQuanLyID
and dnttu.dNgayDeNghi <= @dNgayQuyetDinh
group by dnttuct.iID_DuAnID;

-- Lay thong tin du an chi tiet
select duan.iID_DuAnID,
			duan.sDiaDiem,
			duan.sKhoiCong,
			duan.sKetThuc,
			ISNULL(tmdt.fTongMucDauTuPheDuyet, 0) as fTongMucDauTuPheDuyet,
			ISNULL(khu.fGiaTriUng, 0) as fGiaTriUng,
			ISNULL(vonung.fLKSoVonDaTamUng, 0) as fLKSoVonDaTamUng,
			ISNULL(vonung.fLKThuHoiUng, 0) as fLKThuHoiUng,
			ISNULL(vonung.fLKSoVonDaTamUng, 0) - ISNULL(vonung.fLKThuHoiUng, 0) as fConPhaiThuHoi,
			dt.sSoQuyetDinh as sDuToan,
			dt.iID_DuToanID,
			dnqt.iID_DeNghiQuyetToanID
from VDT_DA_DuAn duan
left join #TEMP_tmdt tmdt on duan.iID_DuAnID = tmdt.iID_DuAnID
left join #TEMP_khu khu on duan.iID_DuAnID = khu.iID_DuAnID
left join #TEMP_vonung vonung on duan.iID_DuAnID = vonung.iID_DuAnID
left join VDT_QT_DeNghiQuyetToan dnqt on dnqt.iID_DuAnID = duan.iID_DuAnID
left join VDT_DA_DuToan dt on dnqt.iID_DuToanID = dt.iID_DuToanID
where duan.iID_DuAnID = @iID_DuAnID

drop table #TEMP_tmdt;
drop table #TEMP_khu;
drop table #TEMP_vonung;