--#DECLARE#--

/*

Lấy all QD dau tu hang muc theo ID qddt

*/

select dt_cp.iID_DuToan_ChiPhiID, 
				dt_cp.iID_DuToanID, 
				dm_cp.sTenChiPhi,
				tblBanDau.fTienPheDuyet as fGiaTriBanDau,
				tblPheDuyet.fGiaTriPheDuyet as fGiaTriPheDuyet
from VDT_DA_DuToan_ChiPhi dt_cp
left join VDT_DM_ChiPhi dm_cp on dt_cp.iID_ChiPhiID = dm_cp.iID_ChiPhi

left join (
	select dt_cp.iID_ChiPhiID, dt_cp.fTienPheDuyet
	from VDT_DA_DuToan_ChiPhi dt_cp
	left join VDT_DA_DuToan dt on dt_cp.iID_DuToanID = dt.iID_DuToanID and dt.bIsGoc = 1
	where dt_cp.iID_DuToanID = @iID_DuToanID
) as tblBanDau on dt_cp.iID_ChiPhiID = tblBanDau.iID_ChiPhiID

left join (
	select dt_cp.iID_ChiPhiID, sum(dt_cp.fTienPheDuyet) as fGiaTriPheDuyet
	from VDT_DA_DuToan_ChiPhi dt_cp
	left join VDT_DA_DuToan dt on dt_cp.iID_DuToanID = dt.iID_DuToanID 
		and dt.iID_DuAnID = @iID_DuAnID
	group by dt_cp.iID_ChiPhiID
) as tblPheDuyet on dt_cp.iID_ChiPhiID = tblPheDuyet.iID_ChiPhiID
where dt_cp.iID_DuToanID = @iID_DuToanID
