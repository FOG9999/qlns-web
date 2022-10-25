--#DECLARE#--

/*

Lấy all QD dau tu hang muc theo ID qddt

*/

select qddt_hm.iID_QDDauTu_HangMuciID, 
				qddt_hm.iID_QDDauTuID,
				da_hm.sTenHangMuc,
				tblBanDau.fTienPheDuyet as fGiaTriBanDau,
				tblPheDuyet.fGiaTriPheDuyet as fGiaTriPheDuyet
from VDT_DA_QDDauTu_HangMuc qddt_hm
left join VDT_DA_QDDauTu_DM_HangMuc da_hm on qddt_hm.iID_HangMucID = da_hm.iID_QDDauTu_DM_HangMucID

left join (
	select qddt_hm.iID_HangMucID, qddt_hm.fTienPheDuyet
	from VDT_DA_QDDauTu_HangMuc qddt_hm
	left join VDT_DA_QDDauTu qddt on qddt_hm.iID_QDDauTuID = qddt.iID_QDDauTuID and qddt.bIsGoc = 1
	where qddt_hm.iID_QDDauTuID = @iID_QDDauTuID
) as tblBanDau on qddt_hm.iID_HangMucID = tblBanDau.iID_HangMucID

left join (
	select qddt_hm.iID_HangMucID, sum(qddt_hm.fTienPheDuyet) as fGiaTriPheDuyet
	from VDT_DA_QDDauTu_HangMuc qddt_hm
	left join VDT_DA_QDDauTu qddt on qddt_hm.iID_QDDauTuID = qddt.iID_QDDauTuID 
		and qddt.iID_DuAnID = @iID_DuAnID
	group by qddt_hm.iID_HangMucID
) as tblPheDuyet on qddt_hm.iID_HangMucID = tblPheDuyet.iID_HangMucID
where qddt_hm.iID_QDDauTuID = @iID_QDDauTuID

