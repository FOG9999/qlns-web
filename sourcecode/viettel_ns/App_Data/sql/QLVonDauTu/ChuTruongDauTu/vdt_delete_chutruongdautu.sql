
--#DECLARE#--
/*

Lấy danh sách hạng mục chủ trương đầu tư

*/

DELETE w
		FROM VDT_DA_ChuTruongDauTu_DM_HangMuc w
		INNER JOIN VDT_DA_ChuTruongDauTu_HangMuc e ON e.iID_HangMucID = w.iID_ChuTruongDauTu_DM_HangMucID
		WHERE e.iID_ChuTruongDauTuID = @iID_ChuTruongDauTuID
		AND w.iID_ChuTruongDauTu_DM_HangMucID NOT IN
		(
			select w1.iID_ChuTruongDauTu_DM_HangMucID  FROM VDT_DA_ChuTruongDauTu_DM_HangMuc w1
			INNER JOIN VDT_DA_ChuTruongDauTu_HangMuc e ON e.iID_HangMucID = w.iID_ChuTruongDauTu_DM_HangMucID
			WHERE e.iID_ChuTruongDauTuID = @parentId
		)

    DELETE VDT_DA_ChuTruongDauTu_NguonVon WHERE iID_ChuTruongDauTuID = @iID_ChuTruongDauTuID


	DELETE VDT_DA_ChuTruongDauTu_HangMuc WHERE iID_ChuTruongDauTuID = @iID_ChuTruongDauTuID