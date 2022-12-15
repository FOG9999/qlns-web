--#DECLARE#--

/*

Lấy thong phe duyet quyet toan theo id

*/

select qt.iID_QuyetToanID,
			qt.iID_DuAnID,
			qt.sSoQuyetDinh,
			qt.dNgayQuyetDinh,
			qt.sCoQuanPheDuyet,
			qt.sNguoiKy,
			qt.sNoiDung,
			da.iID_DonViQuanLyID,
			da.sMaDuAn,
			da.sTenDuAn,
			dn.sSoBaoCao as sTenDeNghi,
			dv.sTen as sTenDonViQuanLy, 
			qt.fChiPhiThietHai,
			qt.fChiPhiKhongTaoNenTaiSan,
			qt.fTaiSanDaiHanThuocCDTQuanLy,
			qt.fTaiSanDaiHanDonViKhacQuanLy,
			qt.fTaiSanNganHanThuocCDTQuanLy,
			qt.fTaiSanNganHanDonViKhacQuanLy,
			qt.iID_DeNghiQuyetToanID
from VDT_QT_QuyetToan qt
left join VDT_DA_DuAn da on qt.iID_DuAnID = da.iID_DuAnID
left join ns_donvi dv on da.iID_DonViQuanLyID = dv.iID_Ma
left join VDT_QT_DeNghiQuyetToan dn on qt.iID_DeNghiQuyetToanID = dn.iID_DeNghiQuyetToanID
where qt.iID_QuyetToanID = @iID_QuyetToanID
