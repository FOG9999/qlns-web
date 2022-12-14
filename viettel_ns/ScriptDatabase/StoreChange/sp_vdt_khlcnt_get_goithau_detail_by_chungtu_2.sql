USE [CTC_DB_TEST]
GO
/****** Object:  StoredProcedure [dbo].[sp_vdt_khlcnt_get_goithau_detail_by_chungtu_2]    Script Date: 08/12/2022 4:51:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROC [dbo].[sp_vdt_khlcnt_get_goithau_detail_by_chungtu_2]
@iLoaiChungTu int,
@lstId t_tbl_Ids READONLY
AS
BEGIN
	CREATE TABLE #tmp(
		iID_ChungTu uniqueidentifier,
		iID_NguonVonID int,
		iID_ChiPhiID uniqueidentifier,
		iID_HangMucID uniqueidentifier,
		iID_ParentId uniqueidentifier,
		sNoiDung nvarchar(500),
		iThuTu int,
		sMaOrder nvarchar(100),
		fGiaTriPheDuyet float,
		iProcessStatus int
	)

	IF(@iLoaiChungTu = 1)
	BEGIN
		-- Nguon von
		INSERT INTO #tmp(iID_ChungTu, iID_NguonVonID, sNoiDung, fGiaTriPheDuyet, iProcessStatus, iThuTu)
		SELECT tbl.iID_DuToanID, tbl.iID_NguonVonID, dt.sTen, ISNULL(tbl.fTienPheDuyet, 0), 2, 0
		FROM @lstId as tmp
		INNER JOIN VDT_DA_DuToan_Nguonvon as tbl on tbl.iID_DuToanID = tmp.iId
		INNER JOIN NS_NguonNganSach as dt on tbl.iID_NguonVonID = dt.iID_MaNguonNganSach

		-- Chi phi
		INSERT INTO #tmp(iID_ChungTu, iID_ChiPhiID, iID_ParentId, sNoiDung, fGiaTriPheDuyet, iProcessStatus, iThuTu)
		SELECT tbl.iID_DuToanID, tbl.iID_DuAn_ChiPhi, dt.iID_ChiPhi_Parent, dt.sTenChiPhi, ISNULL(tbl.fTienPheDuyet, 0), 2, cp.iThuTu
		FROM @lstId as tmp
		INNER JOIN VDT_DA_DuToan_ChiPhi as tbl on tmp.iId = tbl.iID_DuToanID
		INNER JOIN VDT_DM_DuAn_ChiPhi as dt on tbl.iID_DuAn_ChiPhi = dt.iID_DuAn_ChiPhi
		INNER JOIN VDT_DM_ChiPhi as cp on dt.iID_ChiPhi = cp.iID_ChiPhi

		-- Hang muc
		INSERT INTO #tmp(iID_ChungTu, iID_ChiPhiID, iID_HangMucID, iID_ParentId, sNoiDung, sMaOrder,fGiaTriPheDuyet, iProcessStatus, iThuTu )
		SELECT tbl.iID_DuToanID, tbl.iID_DuAn_ChiPhi, tbl.iID_HangMucID, dt.iID_ParentID, dt.sTenHangMuc, dt.maOrder, tbl.fTienPheDuyet, 2, 0
		FROM @lstId as tmp
		INNER JOIN VDT_DA_DuToan_HangMuc as tbl on tmp.iId = tbl.iID_DuToanID
		INNER JOIN VDT_DA_DuToan_DM_HangMuc as dt on tbl.iID_HangMucID = dt.Id
	END
	ELSE IF(@iLoaiChungTu = 2)
	BEGIN
		-- Nguon von
		INSERT INTO #tmp(iID_ChungTu, iID_NguonVonID, sNoiDung, fGiaTriPheDuyet, iProcessStatus, iThuTu)
		SELECT tbl.iID_QDDauTuID, tbl.iID_NguonVonID, dt.sTen, ISNULL(tbl.fTienPheDuyet, 0), 2, 0
		FROM @lstId as tmp
		INNER JOIN VDT_DA_QDDauTu_NguonVon as tbl on tbl.iID_QDDauTuID = tmp.iId
		INNER JOIN NS_NguonNganSach as dt on tbl.iID_NguonVonID = dt.iID_MaNguonNganSach
		
		-- Chi phi
		INSERT INTO #tmp(iID_ChungTu, iID_ChiPhiID, iID_ParentId, sNoiDung, fGiaTriPheDuyet, iProcessStatus, iThuTu)
		SELECT tbl.iID_QDDauTuID, tbl.iID_DuAn_ChiPhi, dt.iID_ChiPhi_Parent, dt.sTenChiPhi, ISNULL(tbl.fTienPheDuyet, 0), 2, cp.iThuTu
		FROM @lstId as tmp
		INNER JOIN VDT_DA_QDDauTu_ChiPhi as tbl on tmp.iId = tbl.iID_QDDauTuID
		INNER JOIN VDT_DM_DuAn_ChiPhi as dt on tbl.iID_DuAn_ChiPhi = dt.iID_DuAn_ChiPhi
		INNER JOIN VDT_DM_ChiPhi as cp on dt.iID_ChiPhi = cp.iID_ChiPhi


		-- Hang muc
		INSERT INTO #tmp(iID_ChungTu, iID_ChiPhiID, iID_HangMucID, iID_ParentId, sNoiDung, sMaOrder,fGiaTriPheDuyet, iProcessStatus, iThuTu)
		SELECT tbl.iID_QDDauTuID, tbl.iID_DuAn_ChiPhi, tbl.iID_HangMucID, dt.iID_ParentID, dt.sTenHangMuc, dt.smaOrder, tbl.fTienPheDuyet, 2, 0
		FROM @lstId as tmp
		INNER JOIN VDT_DA_QDDauTu_HangMuc as tbl on tmp.iId = tbl.iID_QDDauTuID
		INNER JOIN VDT_DA_QDDauTu_DM_HangMuc as dt on tbl.iID_HangMucID = dt.iID_QDDauTu_DM_HangMucID
	END
	ELSE
	BEGIN
	-- Nguon von
		INSERT INTO #tmp(iID_ChungTu, iID_NguonVonID, sNoiDung, fGiaTriPheDuyet, iProcessStatus, iThuTu)
		SELECT tbl.iID_ChuTruongDauTuID, tbl.iID_NguonVonID, dt.sTen, ISNULL(tbl.fTienPheDuyet, 0), 2, 0
		FROM @lstId as tmp
		INNER JOIN VDT_DA_ChuTruongDauTu_NguonVon as tbl on tbl.iID_ChuTruongDauTuID = tmp.iId
		INNER JOIN NS_NguonNganSach as dt on tbl.iID_NguonVonID = dt.iID_MaNguonNganSach
		
		-- Chi phi (Chu truong dau tu dang khong co chi phi -> lay trong danh muc chi phi VDT)
		INSERT INTO #tmp(iID_ChungTu, iID_ChiPhiID, iID_ParentId, sNoiDung, fGiaTriPheDuyet, iProcessStatus, iThuTu)
		SELECT tmp.iId, cp.iID_ChiPhi, null, cp.sTenChiPhi, 0, 2, cp.iThuTu
		FROM @lstId as tmp, VDT_DM_ChiPhi cp

		-- Hang muc
		INSERT INTO #tmp(iID_ChungTu, iID_ChiPhiID, iID_HangMucID, iID_ParentId, sNoiDung, sMaOrder,fGiaTriPheDuyet, iProcessStatus, iThuTu)
		SELECT tbl.iID_ChuTruongDauTuID, null, tbl.iID_HangMucID, dt.iID_ParentID, dt.sTenHangMuc, dt.smaOrder, tbl.fTienPheDuyet, 2, 0
		FROM @lstId as tmp
		INNER JOIN VDT_DA_ChuTruongDauTu_HangMuc as tbl on tmp.iId = tbl.iID_ChuTruongDauTuID
		INNER JOIN VDT_DA_ChuTruongDauTu_DM_HangMuc as dt on tbl.iID_HangMucID = dt.iID_ChuTruongDauTu_DM_HangMucID
	END

	SELECT *, CAST(0 as float) as fGiaTriGoiThau, 1 as isUsing FROM #tmp
	DROP TABLE #tmp
END