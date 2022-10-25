--#DECLARE#--

/*

Lấy tổng giá trị phân bổ

*/

select ISNULL(dntt_ct.fTongGiaTriPhanBo, 0) as fTongGiaTriPhanBo
from VDT_DA_DuAn da
LEFT JOIN (
		select dnttct.iID_DuAnID, ISNULL(sum(dnttct.fGiaTriThanhToan), 0) as fTongGiaTriPhanBo 
		from VDT_TT_DeNghiThanhToan_ChiTiet dnttct
		left join VDT_TT_DeNghiThanhToan dntt on dnttct.iID_DeNghiThanhToanID = dntt.iID_DeNghiThanhToanID
						and dntt.iID_DonViQuanLyID = @iID_DonViQuanLyID
						and dntt.dNgayDeNghi <= @dNgayQuyetDinh
		where dnttct.iID_DuAnID = @iID_DuAnID
		group by dnttct.iID_DuAnID
) as dntt_ct on da.iID_DuAnID = dntt_ct.iID_DuAnID
where da.iID_DuAnID = @iID_DuAnID