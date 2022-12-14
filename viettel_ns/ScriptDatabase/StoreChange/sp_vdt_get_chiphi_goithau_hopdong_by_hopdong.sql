USE [CTC_DB_TEST]
GO
/****** Object:  StoredProcedure [dbo].[sp_vdt_get_chiphi_goithau_hopdong_by_hopdong]    Script Date: 28/12/2022 1:33:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROC [dbo].[sp_vdt_get_chiphi_goithau_hopdong_by_hopdong]

@iIdHopDong uniqueidentifier
AS

BEGIN
	WITH CPDuToancTreeCTE AS (
		SELECT  
			hdhm.iID_ChiPhiID AS Id,
			gtnt.iID_HopDongID AS IIDHopDongID,
			gtnt.iID_GoiThauID AS IIDGoiThauID,
			hdhm.iID_HopDongGoiThauNhaThauID AS IdGoiThauNhaThau,
			hdhm.iID_ChiPhiID AS IIDChiPhiID,
			dacp.iID_ChiPhi_Parent AS IdChiPhiDuAnParent,
			NULL AS IIDHangMucID,
			NULL AS HangMucParentId,
			NULL AS IIDNhaThauID,
			dacp.sTenChiPhi AS STenChiPhi,
			NULL AS STenHangMuc,
			hdhm.fGiaTri AS FTienGoiThau,
			hdhm.fGiaTriTruocDC AS FTienGoiThauTruocDC,
			NULL AS IThuTu,
			CAST((dacp.iID_DuAn_ChiPhi) AS VARCHAR(MAX)) AS MaOrDer,
			gtcp.fTienGoiThau as FGiaTriDuocDuyet 
		FROM VDT_DA_HopDong_GoiThau_ChiPhi hdhm 
		INNER JOIN VDT_DA_HopDong_GoiThau_NhaThau gtnt ON gtnt.Id = hdhm.iID_HopDongGoiThauNhaThauID
		INNER JOIN VDT_DM_DuAn_ChiPhi dacp ON dacp.iID_DuAn_ChiPhi = hdhm.iID_ChiPhiID

		INNER JOIN VDT_DA_GoiThau_ChiPhi gtcp on dacp.iID_DuAn_ChiPhi = gtcp.iID_ChiPhiID

		WHERE gtnt.iID_HopDongID =  @iIdHopDong AND dacp.iID_ChiPhi_Parent IS NULL

		

		UNION ALL
		
		SELECT 
			hdhm.iID_ChiPhiID AS Id,
			gtnt.iID_HopDongID AS IIDHopDongID,
			gtnt.iID_GoiThauID AS IIDGoiThauID,
			hdhm.iID_HopDongGoiThauNhaThauID AS IdGoiThauNhaThau,
			hdhm.iID_ChiPhiID AS IIDChiPhiID,
			dacp.iID_ChiPhi_Parent AS IdChiPhiDuAnParent,
			NULL AS IIDHangMucID,
			NULL AS HangMucParentId,
			NULL AS IIDNhaThauID,
			dacp.sTenChiPhi AS STenChiPhi,
			NULL AS STenHangMuc,
			hdhm.fGiaTri AS FTienGoiThau,
			hdhm.fGiaTriTruocDC AS FTienGoiThauTruocDC,
			NULL AS IThuTu,
			CAST((M.MaOrDer + '-' + CAST(dacp.iID_DuAn_ChiPhi AS VARCHAR(MAX)) ) AS VARCHAR(MAX)) AS MaOrDer,
			gtcp.fTienGoiThau as FGiaTriDuocDuyet 
		FROM VDT_DA_HopDong_GoiThau_ChiPhi hdhm 
		INNER JOIN VDT_DA_HopDong_GoiThau_NhaThau gtnt ON gtnt.Id = hdhm.iID_HopDongGoiThauNhaThauID
		INNER JOIN VDT_DM_DuAn_ChiPhi dacp ON dacp.iID_DuAn_ChiPhi = hdhm.iID_ChiPhiID
		INNER JOIN CPDuToancTreeCTE M ON dacp.iID_ChiPhi_Parent = M.IIDChiPhiID 
		INNER JOIN VDT_DA_GoiThau_ChiPhi gtcp on dacp.iID_DuAn_ChiPhi = gtcp.iID_ChiPhiID
		WHERE gtnt.iID_HopDongID =  @iIdHopDong
	)
	SELECT
		tbl.* ,
		ISNULL(CAST(CASE WHEN parentId.iID_ChiPhi_Parent IS NOT NULL OR tbl.IdChiPhiDuAnParent IS NULL THEN 1 ELSE 0 END AS BIT), 0) AS IsHangCha,
		CAST (1 AS BIT ) AS IsChecked,
		tbl.FGiaTriDuocDuyet  AS FGiaTriDuocDuyet,
		NULL AS FGiaTriConLai
		FROM CPDuToancTreeCTE tbl
		LEFT JOIN
		(
			SELECT DISTINCT tb2.iID_ChiPhi_Parent FROM VDT_DA_HopDong_GoiThau_ChiPhi tb1
			INNER JOIN VDT_DA_HopDong_GoiThau_NhaThau gtnt ON gtnt.Id = tb1.iID_HopDongGoiThauNhaThauID
			INNER JOIN VDT_DM_DuAn_ChiPhi tb2 ON tb1.iID_ChiPhiID = tb2.iID_DuAn_ChiPhi AND gtnt.iID_HopDongID = @iIdHopDong and tb2.iID_ChiPhi_Parent IS NOT NULL
		) AS parentId ON parentId.iID_ChiPhi_Parent = tbl.IIDChiPhiID
		ORDER BY MaOrDer,IThuTu
END
;
;

