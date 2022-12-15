WITH temp
AS
(
	select  dacp.iID_DuAn_ChiPhi,
		dacp.sTenChiPhi,
		dacp.iID_ChiPhi as iID_ChiPhiID,
		dacp.iID_ChiPhi_Parent,
		dtcp.fTienPheDuyet,
		dtcp.fGiaTriDieuChinh,
		dtcp.fTienPheDuyetQDDT,
		dmcp.iThuTu,
		CAST( dmcp.iThuTu as varchar(100))as iSTT,
		CONCAT(dacp.iID_ChiPhi_Parent, dacp.iID_DuAn_ChiPhi) as ord
	from VDT_DA_DuToan_ChiPhi dtcp
	inner join VDT_DM_DuAn_ChiPhi dacp ON dacp.iID_DuAn_ChiPhi = dtcp.iID_DuAn_ChiPhi 
	left join VDT_DM_ChiPhi dmcp ON dtcp.iID_ChiPhiID = dmcp.iID_ChiPhi
	where dtcp.iID_DuToanID = @duToanId and dacp.iID_ChiPhi_Parent  is null
	union all
	select  dthm.iID_DuAn_ChiPhi,
		dahm.sTenHangMuc as sTenChiPhi,
		dahm.Id as iID_ChiPhiID,
		dthm.iID_DuAn_ChiPhi as iID_ChiPhi_Parent,
		dthm.fTienPheDuyet,
		dthm.fGiaTriDieuChinh,
		dthm.fTienPheDuyetQDDT,
		null as iThuTu,
		dahm.maOrder as iSTT,
		CONCAT(dthm.iID_DuAn_ChiPhi, dthm.iID_DuAn_ChiPhi) as ord
	
	from VDT_DA_DuToan_HangMuc dthm
	
	inner join VDT_DA_DuToan_DM_HangMuc dahm ON dthm.iID_HangMucID = dahm.Id
	where dthm.iID_DuToanID = @duToanId
)


select *, CONCAT(iSTT, ord) as ordpc
from temp final 
where final.iID_ChiPhi_Parent is null

union 

select child.*,
CONCAT(parent.iSTT, child.ord, child.iSTT) as ordpc
from temp child inner join temp parent on child.iID_ChiPhi_Parent = parent.iID_DuAn_ChiPhi
where parent.iID_ChiPhi_Parent is null and child.iID_ChiPhi_Parent in (select fn.iID_DuAn_ChiPhi from temp fn where fn.iID_ChiPhi_Parent is not null)
order by ordpc asc