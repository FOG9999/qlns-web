--#DECLARE#--

/*

Lấy all du toan nguon von theo ID du toan

*/

select dt_nv.iID_DuToan_NguonVonID, 
				dt_nv.iID_DuToanID, 
				ns_nns.sTen as sTenNguonGocVon,
				tblBanDau.fTienPheDuyet as fGiaTriBanDau,
				tblPheDuyet.fGiaTriPheDuyet as fGiaTriPheDuyet
from VDT_DA_DuToan_NguonVon dt_nv
left join NS_NguonNganSach ns_nns on dt_nv.iID_NguonVonID = ns_nns.iID_MaNguonNganSach

left join (
	select dt_nv.iID_NguonVonID, dt_nv.fTienPheDuyet
	from VDT_DA_DuToan_NguonVon dt_nv
	left join VDT_DA_DuToan dt on dt_nv.iID_DuToanID = dt.iID_DuToanID and dt.bIsGoc = 1
	where dt_nv.iID_DuToanID = @iID_DuToanID
) as tblBanDau on dt_nv.iID_NguonVonID = tblBanDau.iID_NguonVonID

left join (
	select dt_nv.iID_NguonVonID, sum(dt_nv.fTienPheDuyet) as fGiaTriPheDuyet
	from VDT_DA_DuToan_NguonVon dt_nv
	left join VDT_DA_DuToan dt on dt_nv.iID_DuToanID = dt.iID_DuToanID 
		and dt.iID_DuAnID = @iID_DuAnID
	group by dt_nv.iID_NguonVonID
) as tblPheDuyet on dt_nv.iID_NguonVonID = tblPheDuyet.iID_NguonVonID
where dt_nv.iID_DuToanID = @iID_DuToanID
