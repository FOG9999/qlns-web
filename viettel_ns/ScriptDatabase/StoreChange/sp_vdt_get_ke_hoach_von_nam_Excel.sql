USE [CTC_DB_TEST]
GO
/****** Object:  StoredProcedure [dbo].[sp_vdt_get_ke_hoach_von_nam_Excel]    Script Date: 09/12/2022 9:12:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_vdt_get_ke_hoach_von_nam_Excel]
	@Id nvarchar(max)
AS
BEGIN
	select DISTINCT ct.*,
	da.sTenDuAn as STenDuAn,
	cdt.sTenCDT as SChuDauTu,
	ctdt.iID_MaChuDauTuID as SMaChuDauTu,
	dv.sTen as SDonViQuanLy,
	dv.iID_MaDonVi as SMaDonViQuanLy,
	ct.fLuyKeVonNamTruoc as LuyKeVonThucHienTruocNam,
	ct.fTongMucDauTuDuocDuyet as TongMucDauTuDuocDuyet,
	ct.fThanhToan as ThanhToan,
	ct.fThuHoiVonUngTruoc as ThuHoiVonUngTruoc,
   (ISNULL(ct.fThanhToan,0) + ISNULL(ct.fThuHoiVonUngTruoc,0))as TongNhuCauVonNamSau ,
    0 as LuyKeVonDaBoTriHetNam,
    ct.fUocThucHien as UocThucHien,
	ct.fVonKeoDaiCacNamTruoc as VonKeoDaiCacNamTruoc,
	ct.fKeHoachVonDuocDuyetNamNay as KeHoachVonDuocGiao,
	dx.iNamKeHoach,
	(ISNULL(ct.fKeHoachVonDuocDuyetNamNay,0) + ISNULL(ct.fVonKeoDaiCacNamTruoc,0)) as TongSoKeHoachVon INTO #data
	from VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet ct
	inner join VDT_KHV_KeHoachVonNam_DeXuat dx on ct.iID_KeHoachVonNamDeXuatID = dx.iID_KeHoachVonNamDeXuatID
	LEFT  JOIN VDT_DA_DuAn as da on ct.iID_DuAnID = da.iID_DuAnID
	LEFT join VDT_DA_ChuTruongDauTu ctdt on ctdt.iID_ChuDauTuID = da.iID_ChuDauTuID
	LEFT join DM_ChuDauTu cdt on cdt.ID = ctdt.iID_ChuDauTuID
	LEFT join NS_DonVi dv on dv.iID_Ma = dx.iID_DonViQuanLyID
	where ct.iID_KeHoachVonNamDeXuatID = @Id
	--and ctdt.bActive <> 0
	
	select ROW_NUMBER()OVER(ORDER BY STenDuAn ASC) as STT, * FROM #data
	drop table #data
END


--exec sp_vdt_get_ke_hoach_von_nam_Excel '583af5ee-7b20-46a7-ab0c-2313766321c5'