USE [CTC_DB_TEST]
GO
/****** Object:  StoredProcedure [dbo].[sp_vdt_thongtri_detail_by_id]    Script Date: 12/27/2022 5:58:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROC [dbo].[sp_vdt_thongtri_detail_by_id]
@iId uniqueidentifier
AS
BEGIN
	SELECT dt.iID_KieuThongTriID as IIdKieuThongTriId,
		tt.SMaKieuThongTri,
		dt.SSoThongTri,
		da.STenDuAn,
		dt.iID_DuAnID as IIdDuAnId,
		dt.iID_NhaThauID as IIdNhaThauId,
		ISNULL(dt.fSoTien, 0) as FSoTien,
		dt.iID_MucID as IIdMucId,
		dt.iID_TieuMucID as IIdTieuMucId,
		dt.iID_TietMucID as IIdTietMucId,
		dt.iID_NganhID as IIdNganhId,
		dt.iID_LoaiCongTrinhID as IIdLoaiCongTrinhId,
		dt.IId_LoaiNguonVonId as IIdLoaiNguonVonId,
		dt.iID_CapPheDuyetID as IIdCapPheDuyetId,
		ns.sLNS as SLns,
		ns.sL as SL,
		ns.sK as SK,
		ns.sM as SM,
		ns.sTM as STm,
		ns.sTTM as STtm,
		ns.sNG as SNg,
		nt.sTenNhaThau as SDonViThuHuong,
		lct.STenLoaiCongTrinh
	FROM VDT_ThongTri as tbl
	INNER JOIN VDT_ThongTri_ChiTiet as dt on tbl.iID_ThongTriID = dt.iID_ThongTriID
	INNER JOIN VDT_DA_DuAn as da on dt.iId_DuAnId = da.iId_DuAnId
	LEFT JOIN VDT_DM_KieuThongTri as tt on dt.iID_KieuThongTriID = tt.iID_KieuThongTriID
	LEFT JOIN NS_MucLucNganSach as ns on dt.iID_MucID = ns.iID_MaMucLucNganSach OR dt.iID_TieuMucID = ns.iID_MaMucLucNganSach OR dt.iID_TietMucID = ns.iID_MaMucLucNganSach OR dt.iID_NganhID = ns.iID_MaMucLucNganSach
	LEFT JOIN VDT_DM_NhaThau as nt on dt.iID_NhaThauID = nt.iID_NhaThauID
	LEFT JOIN VDT_DM_LoaiCongTrinh as lct on dt.iID_LoaiCongTrinhID = lct.iID_LoaiCongTrinh
	WHERE tbl.iID_ThongTriID = @iId
END
;