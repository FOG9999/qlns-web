--#DECLARE#--

/*

Lấy record VDT_TT_DeNghiThanhToanUng theo ID 

*/

select dnttu.iID_DeNghiThanhToanID, dnttu.sSoDeNghi, dnttu.dNgayDeNghi,
				dnttu.sNguoiLap,
				dnttu.sGhiChu,
			dnttu.fGiaTriThanhToan,
			dv.sTen as sTenDonViQuanLy,
			nql.sTenNhomQuanLy
from VDT_TT_DeNghiThanhToanUng dnttu
left join NS_DonVi dv on dnttu.iID_DonViQuanLyID = dv.iID_Ma
left join VDT_DM_NhomQuanLy nql on dnttu.iID_NhomQuanLyID = nql.iID_NhomQuanLyID
where dnttu.iID_DeNghiThanhToanID = @iID_DeNghiThanhToanID

