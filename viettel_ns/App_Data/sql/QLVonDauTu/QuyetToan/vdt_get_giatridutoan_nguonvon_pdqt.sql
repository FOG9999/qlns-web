--#DECLARE#--

/*

Lấy giá trị dự toán cho phê duyệt quyết toán

*/

-- check ton tai tong du toan
select count(*) as chk into #TEMP_check 
from VDT_DA_DuToan where iID_DuAnID = @iID_DuAnID and bLaTongDuToan = 1 and dNgayQuyetDinh <= @dNgayQuyetDinh;

-- khi ton tai tong du toan
select ISNULL(sum(tblDuToan.fTienPheDuyet), 0) as fGiaTriDuToan into #TEMP_tongdutoan
from NS_NguonNganSach dm_nns
left join (
	select dtnv.iID_NguonVonID, dtnv.fTienPheDuyet from VDT_DA_DuToan_Nguonvon dtnv
	inner join VDT_DA_DuToan dt on dtnv.iID_DuToanID = dt.iID_DuToanID and dt.iID_DuAnID = @iID_DuAnID
			and dt.bLaTongDuToan = 1
			and dt.dNgayQuyetDinh <= @dNgayQuyetDinh
) tblDuToan on dm_nns.iID_MaNguonNganSach = tblDuToan.iID_NguonVonID
where dm_nns.iID_MaNguonNganSach = @iID_MaNguonNganSach
group by dm_nns.iID_MaNguonNganSach;

-- sum gia tri khi khong ton tai tong du toan
select ISNULL(sum(tblDuToan.fTienPheDuyet), 0) as fGiaTriDuToan into #TEMP_dutoan
from NS_NguonNganSach dm_nns
left join (
	select dtnv.iID_NguonVonID, dtnv.fTienPheDuyet from VDT_DA_DuToan_Nguonvon dtnv
	inner join VDT_DA_DuToan dt on dtnv.iID_DuToanID = dt.iID_DuToanID and dt.iID_DuAnID = @iID_DuAnID
			and dt.bLaTongDuToan = 0
			and dt.dNgayQuyetDinh <= @dNgayQuyetDinh
) tblDuToan on dm_nns.iID_MaNguonNganSach = tblDuToan.iID_NguonVonID
where dm_nns.iID_MaNguonNganSach = @iID_MaNguonNganSach
group by dm_nns.iID_MaNguonNganSach;


SELECT CASE (select chk from #TEMP_check)
	WHEN 0 THEN (select fGiaTriDuToan from #TEMP_dutoan)
	ELSE (select fGiaTriDuToan from #TEMP_tongdutoan)
END

DROP TABLE #TEMP_check;
DROP TABLE #TEMP_dutoan;
DROP TABLE #TEMP_tongdutoan;

--select ISNULL(sum(dtnv.fTienPheDuyet), 0) as fGiaTriDuToan 
--from NS_NguonNganSach dm_nns
--left join VDT_DA_DuToan_Nguonvon dtnv on dm_nns.iID_MaNguonNganSach = dtnv.iID_NguonVonID
--left join VDT_DA_DuToan dt on dtnv.iID_DuToanID = dt.iID_DuToanID and dt.iID_DuAnID = @iID_DuAnID
--			--and dt.dNgayQuyetDinh <= @dNgayQuyetDinh
--where dm_nns.iID_MaNguonNganSach = @iID_MaNguonNganSach
--group by dm_nns.iID_MaNguonNganSach;
