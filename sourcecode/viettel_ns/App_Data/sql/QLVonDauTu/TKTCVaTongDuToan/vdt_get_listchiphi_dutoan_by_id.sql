

--#DECLARE#--
/*

Lấy danh sách dự chi phí dự toán theo dự án-

*/


--select dacp.iID_DuAn_ChiPhi,
--	dacp.sTenChiPhi,
--	dacp.iID_ChiPhi as iID_ChiPhiID,
--	dacp.iID_ChiPhi_Parent,
--	dtcp.fTienPheDuyet,
--	dtcp.fGiaTriDieuChinh,
--	dtcp.fTienPheDuyetQDDT,
--	dacp.iThuTu
--from VDT_DA_DuToan_ChiPhi dtcp
--	inner join VDT_DM_DuAn_ChiPhi dacp ON dacp.iID_DuAn_ChiPhi = dtcp.iID_DuAn_ChiPhi
--where dtcp.iID_DuToanID = @duToanId
--order by iThuTu desc


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
		dacp.iThuTu,
		CAST( ROW_NUMBER() OVER (PARTITION BY dacp.iID_ChiPhi_Parent  Order by  dacp.iID_ChiPhi_Parent ) as varchar(100))as iSTT
	from VDT_DA_DuToan_ChiPhi dtcp
	inner join VDT_DM_DuAn_ChiPhi dacp ON dacp.iID_DuAn_ChiPhi = dtcp.iID_DuAn_ChiPhi 
	where dtcp.iID_DuToanID = @duToanId and dacp.iID_ChiPhi_Parent  is null

	UNION ALL

	select  dacp.iID_DuAn_ChiPhi,
		dacp.sTenChiPhi,
		dacp.iID_ChiPhi as iID_ChiPhiID,
		dacp.iID_ChiPhi_Parent,
		dtcp.fTienPheDuyet,
		dtcp.fGiaTriDieuChinh,
		dtcp.fTienPheDuyetQDDT,
		dacp.iThuTu,
		CAST('z' as varchar(100))  as iSTT
	
	from VDT_DA_DuToan_ChiPhi dtcp
	inner join VDT_DM_DuAn_ChiPhi dacp ON dacp.iID_DuAn_ChiPhi = dtcp.iID_DuAn_ChiPhi
	INNER JOIN temp as tm on tm.iID_DuAn_ChiPhi = dacp.iID_ChiPhi_Parent
)

select * from temp
order by iSTT