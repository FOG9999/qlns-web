--#DECLARE#--

/*

Lấy all QD dau tu nguon von theo ID qddt

*/

select qddt_nv.iID_QDDauTu_NguonVonID, 
				qddt_nv.iID_QDDauTuID,
				ns_nns.sTen as sTenNguonGocVon,
				tblBanDau.fTienPheDuyet as fGiaTriBanDau,
				tblPheDuyet.fGiaTriPheDuyet as fGiaTriPheDuyet
from VDT_DA_QDDauTu_NguonVon qddt_nv
left join NS_NguonNganSach ns_nns on qddt_nv.iID_NguonVonID = ns_nns.iID_MaNguonNganSach

left join (
	select qddt_nv.iID_NguonVonID, qddt_nv.fTienPheDuyet
	from VDT_DA_QDDauTu_NguonVon qddt_nv
	left join VDT_DA_QDDauTu qddt on qddt_nv.iID_QDDauTuID = qddt.iID_QDDauTuID and qddt.bIsGoc = 1
	where qddt_nv.iID_QDDauTuID = @iID_QDDauTuID
) as tblBanDau on qddt_nv.iID_NguonVonID = tblBanDau.iID_NguonVonID

left join (
	select qddt_nv.iID_NguonVonID, sum(qddt_nv.fTienPheDuyet) as fGiaTriPheDuyet
	from VDT_DA_QDDauTu_NguonVon qddt_nv
	left join VDT_DA_QDDauTu qddt on qddt_nv.iID_QDDauTuID = qddt.iID_QDDauTuID 
		and qddt.iID_DuAnID = @iID_DuAnID
	group by qddt_nv.iID_NguonVonID
) as tblPheDuyet on qddt_nv.iID_NguonVonID = tblPheDuyet.iID_NguonVonID
where qddt_nv.iID_QDDauTuID = @iID_QDDauTuID


