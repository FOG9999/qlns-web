--#DECLARE#--

/*

Lấy giá trị dự toán cho phê duyệt quyết toán

*/

-- check ton tai tong du toan
select count(*) as chk into #TEMP_check 
from VDT_DA_DuToan where iID_DuAnID = @iID_DuAnID and bLaTongDuToan = 1 and dNgayQuyetDinh <= @dNgayQuyetDinh;

-- khi ton tai tong du toan
select ISNULL(sum(tblDuToan.fTienPheDuyet), 0) as fGiaTriDuToan into #TEMP_tongdutoan
from VDT_DM_ChiPhi dm_cp
left join (
	select dtcp.iID_ChiPhiID, dtcp.fTienPheDuyet from VDT_DA_DuToan_ChiPhi dtcp 
	inner join VDT_DA_DuToan dt on dtcp.iID_DuToanID = dt.iID_DuToanID and dt.iID_DuAnID = @iID_DuAnID
			and dt.bLaTongDuToan = 1
			and dt.dNgayQuyetDinh <= @dNgayQuyetDinh
			) tblDuToan on dm_cp.iID_ChiPhi = tblDuToan.iID_ChiPhiID
where dm_cp.iID_ChiPhi = @iID_ChiPhiID
group by dm_cp.iID_ChiPhi;

-- sum gia tri khi khong ton tai tong du toan
select ISNULL(sum(tblDuToan.fTienPheDuyet), 0) as fGiaTriDuToan into #TEMP_dutoan
from VDT_DM_ChiPhi dm_cp
left join (
	select dtcp.iID_ChiPhiID, dtcp.fTienPheDuyet from VDT_DA_DuToan_ChiPhi dtcp 
	inner join VDT_DA_DuToan dt on dtcp.iID_DuToanID = dt.iID_DuToanID and dt.iID_DuAnID = @iID_DuAnID
			and dt.bLaTongDuToan = 0
			and dt.dNgayQuyetDinh <= @dNgayQuyetDinh
			) tblDuToan on dm_cp.iID_ChiPhi = tblDuToan.iID_ChiPhiID
where dm_cp.iID_ChiPhi = @iID_ChiPhiID
group by dm_cp.iID_ChiPhi;


SELECT CASE (select chk from #TEMP_check)
	WHEN 0 THEN (select fGiaTriDuToan from #TEMP_dutoan)
	ELSE (select fGiaTriDuToan from #TEMP_tongdutoan)
END

DROP TABLE #TEMP_check;
DROP TABLE #TEMP_dutoan;
DROP TABLE #TEMP_tongdutoan;


