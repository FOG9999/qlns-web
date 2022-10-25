
--#DECLARE#--
/*

Xóa phê duyệt dự án

*/


DELETE VDT_DA_QDDauTu_NguonVon WHERE iID_QDDauTuID = @iID_QDDauTuID
	
	
	DELETE dmhm 
	FROM VDT_DA_QDDauTu_DM_HangMuc dmhm
		INNER JOIN VDT_DA_QDDauTu_HangMuc qdhm ON qdhm.iID_HangMucID = dmhm.iID_QDDauTu_DM_HangMucID 
		where qdhm.iID_QDDauTuID = @iID_QDDauTuID
		AND dmhm.iID_QDDauTu_DM_HangMucID not in 
		(
			select dmhm1.iID_QDDauTu_DM_HangMucID from VDT_DA_QDDauTu_DM_HangMuc dmhm1
			inner join VDT_DA_QDDauTu_HangMuc qdhm1 ON dmhm1.iID_QDDauTu_DM_HangMucID = qdhm1.iID_HangMucID AND qdhm1.iID_QDDauTuID = @iID_ParentID
		)
	DELETE from VDT_DM_DuAn_ChiPhi where iID_DuAn_ChiPhi IN (
		select iID_DuAn_ChiPhi from VDT_DA_QDDauTu_ChiPhi where iID_QDDauTuID = @iID_QDDauTuID	
	)
	-- DELETE dacp
	-- FROM VDT_DM_DuAn_ChiPhi dacp
		-- INNER JOIN VDT_DA_QDDauTu_ChiPhi qdcp ON qdcp.iID_DuAn_ChiPhi = dacp.iID_DuAn_ChiPhi AND dacp.iID_ChiPhi_Parent is not null
		-- WHERE  qdcp.iID_QDDauTuID = @iID_QDDauTuID
		-- AND dacp.iID_DuAn_ChiPhi not in 
		-- (
			-- select dacp1.iID_DuAn_ChiPhi from VDT_DM_DuAn_ChiPhi dacp1
			-- inner join VDT_DA_QDDauTu_ChiPhi qdcp1 ON dacp1.iID_DuAn_ChiPhi = qdcp1.iID_DuAn_ChiPhi AND qdcp1.iID_QDDauTuID = @iID_ParentID
		-- )

	DELETE VDT_DA_QDDauTu_ChiPhi WHERE iID_QDDauTuID = @iID_QDDauTuID

	DELETE VDT_DA_QDDauTu_HangMuc WHERE iID_QDDauTuID = @iID_QDDauTuID

	DELETE VDT_DA_QDDauTu where iID_QDDauTuID = @iID_QDDauTuID