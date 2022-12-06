--#DECLARE#--

/*

Lấy all chu truong dau tu chi phi theo ID ctdt

*/

select qddt_cp.iID_QDDauTu_ChiPhiID, 
				qddt_cp.iID_QDDauTuID, 
				dm_cp.sTenChiPhi,
				tblBanDau.fTienPheDuyet as fGiaTriBanDau,
				tblPheDuyet.fGiaTriPheDuyet as fGiaTriPheDuyet
from VDT_DA_QDDauTu_ChiPhi qddt_cp
left join VDT_DM_DuAn_ChiPhi dm_cp on qddt_cp.iID_DuAn_ChiPhi = dm_cp.iID_DuAn_ChiPhi

left join (
	select qddt_cp.iID_DuAn_ChiPhi, qddt_cp.fTienPheDuyet
	from VDT_DA_QDDauTu_ChiPhi qddt_cp
	left join VDT_DA_QDDauTu qddt on qddt_cp.iID_QDDauTuID = qddt.iID_QDDauTuID and qddt.bIsGoc = 1
	where qddt_cp.iID_QDDauTuID = @iID_QDDauTuID
) as tblBanDau on qddt_cp.iID_DuAn_ChiPhi = tblBanDau.iID_DuAn_ChiPhi

left join (
	select qddt_cp.iID_DuAn_ChiPhi, sum(qddt_cp.fTienPheDuyet) as fGiaTriPheDuyet
	from VDT_DA_QDDauTu_ChiPhi qddt_cp
	left join VDT_DA_QDDauTu qddt on qddt_cp.iID_QDDauTuID = qddt.iID_QDDauTuID 
		and qddt.iID_DuAnID = @iID_DuAnID
	group by qddt_cp.iID_DuAn_ChiPhi
) as tblPheDuyet on qddt_cp.iID_DuAn_ChiPhi = tblPheDuyet.iID_DuAn_ChiPhi
where qddt_cp.iID_QDDauTuID = @iID_QDDauTuID

