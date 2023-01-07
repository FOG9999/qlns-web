USE [CTC_DB_TEST]
GO

/****** Object:  StoredProcedure [dbo].[sp_vdt_khv_ke_hoach_5_nam_de_xuat_export_new]    Script Date: 28/12/2022 5:26:22 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_vdt_khv_ke_hoach_5_nam_de_xuat_export_new]
	-- Add the parameters for the stored procedure here
	@Id nvarchar(max),
    @lct nvarchar(max),
    @IdNguonVon nvarchar(max),
    @MenhGiaTienTe float,
    @iNamLamViec int,
	@type int
AS
BEGIN
IF(@type = 1)
	 BEGIN
		select 
                        dmlct.iID_LoaiCongTrinh as IdLoaiCongTrinh,
                        dmlct.iID_Parent as IdLoaiCongTrinhParent,
                        dmlct.sMaLoaiCongTrinh as SMaLoaiCongTrinh,
						dmlct.sTenLoaiCongTrinh,
                        4 as Loai,
                        ctct.sTen as STenDuAn,
                        dv.sTen as STenDonVi,
                        ctct.fHanMucDauTu/@MenhGiaTienTe as FHanMucDauTu,
                        ns.sTen as STenNguonVon,                     
                        ((isnull(ctct.fGiaTriNamThuNhat, 0) 
                                + isnull(ctct.fGiaTriNamThuHai, 0) + isnull(ctct.fGiaTriNamThuBa, 0)
                                + isnull(ctct.fGiaTriNamThuTu, 0) + isnull(ctct.fGiaTriNamThuNam, 0))/@MenhGiaTienTe) as FTongSo,
                        (ctct.fGiaTriNamThuNhat/@MenhGiaTienTe) as FGiaTriNamThuNhat,
                        (ctct.fGiaTriNamThuHai/@MenhGiaTienTe) as FGiaTriNamThuHai,
                        (ctct.fGiaTriNamThuBa/@MenhGiaTienTe) as FGiaTriNamThuBa,
                        (ctct.fGiaTriNamThuTu/@MenhGiaTienTe) as FGiaTriNamThuTu,
                        (ctct.fGiaTriNamThuNam/@MenhGiaTienTe) as FGiaTriNamThuNam,
                        (ctct.fGiaTriBoTri/@MenhGiaTienTe) as FGiaTriBoTri,
                        ctct.sGhiChu as SGhiChu,
                        cast(0 as bit) as IsHangCha,
                        NEWID() as Id,
                        1 as LoaiParent,
                        ctct.iID_NguonVonID as IIdNguonVon,
                        null as DNgayQuyetDinh,
                        cast(0 as float) as FHanMucDauTuDP,
                        cast(0 as float) as FHanMucDauTuNN,
                        cast(0 as float) as FHanMucDauTuOrther,
                        cast(0 as float) as FHanMucDauTuQP,
   
						ISNULL(ctct.fHanmucNganhDX, 0)/@MenhGiaTienTe as fHanmucNganhDX,
						ISNULL(ctct.fVon5namNganhDX, 0)/@MenhGiaTienTe as fVon5namNganhDX,
						ISNULL(ctct.fVonsaunamNganhDX, 0)/@MenhGiaTienTe as fVonsaunamNganhDX,
						(ISNULL(ctct.fVon5namNganhDX, 0) + ISNULL(ctct.fVonsaunamNganhDX, 0))/@MenhGiaTienTe as fTongVonBoTriNganh,
						
						ISNULL(ctct.fHanmucCucTCDX, 0)/@MenhGiaTienTe as fHanmucCucTCDX,
						(ISNULL(ctct.fVon5namCTCDX, 0) + ISNULL(ctct.fVonsaunamCTCDexuat, 0))/@MenhGiaTienTe as fTongVonBoTriCuc,
						ISNULL(ctct.fVon5namCTCDX, 0)/@MenhGiaTienTe as fVon5namCTCDX,
						ISNULL(ctct.fVonnamthunhatCTC, 0)/@MenhGiaTienTe as fVonnamthunhatCTC,
						ISNULL(ctct.fVonsaunamCTCDexuat, 0)/@MenhGiaTienTe as fVonsaunamCTCDexuat,
						ISNULL(ctct.fCucTCDeXuat, 0)/@MenhGiaTienTe as fCucTCDeXuat,
						ISNULL(ctct.fDuKienBoTriNamThu2, 0)/@MenhGiaTienTe as fDuKienBoTriNamThu2,
						'' as SSoQuyetDinh,
						ctct.iLevel as iLevel,
						ctct.sSTT as STT

                from 
                        f_loai_cong_trinh_get_list_childrent(@lct) dmlct
                left join
                        VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet ctct
                on
                        dmlct.iID_LoaiCongTrinh = ctct.iID_LoaiCongTrinhID
				inner join 
						VDT_KHV_KeHoach5Nam_DeXuat vndx
                on ctct.iID_KeHoach5Nam_DeXuatID = vndx.iID_KeHoach5Nam_DeXuatID

                left join
                        NS_DonVi dv
                on vndx.iID_DonViQuanLyID = dv.iID_Ma
                left join 
                        NS_NguonNganSach ns
                on ctct.iID_NguonVonID = ns.iID_MaNguonNganSach
				
                where
                        ctct.iID_KeHoach5Nam_DeXuatID  in (select * from dbo.splitstring(@Id))
                        and ctct.iID_NguonVonID in (select * from dbo.splitstring(@IdNguonVon))
				order by STenDonVi,sTenLoaiCongTrinh
	END
ELSE
IF(@type =2)
	BEGIN
		select 
                        dmlct.iID_LoaiCongTrinh as IdLoaiCongTrinh,
                        dmlct.iID_Parent as IdLoaiCongTrinhParent,
                        dmlct.sMaLoaiCongTrinh as SMaLoaiCongTrinh,
						dmlct.sTenLoaiCongTrinh,
                        0 as Loai,
                        '' as STenDuAn,
                        '' as STenDonVi,
                        CAST(0 as float) as FHanMucDauTu,
                        '' as STenNguonVon,                     
                        CAST(0 as float) as FTongSo,
                        CAST(0 as float) as FGiaTriNamThuNhat,
                        CAST(0 as float) as FGiaTriNamThuHai,
                        CAST(0 as float) as FGiaTriNamThuBa,
                        CAST(0 as float) as FGiaTriNamThuTu,
                        CAST(0 as float) as FGiaTriNamThuNam,
                        CAST(0 as float) as FGiaTriBoTri,
                        '' as SGhiChu,
                        cast(1 as bit) as IsHangCha,
                        NEWID() as Id,
                        1 as LoaiParent,
                        '' as IIdNguonVon,
                        null as DNgayQuyetDinh,
                        cast(0 as float) as FHanMucDauTuDP,
                        cast(0 as float) as FHanMucDauTuNN,
                        cast(0 as float) as FHanMucDauTuOrther,
                        cast(0 as float) as FHanMucDauTuQP,
   
						 CAST(0 as float) as fHanmucNganhDX,
						CAST(0 as float) as fVon5namNganhDX,
						CAST(0 as float) as fVonsaunamNganhDX,
						CAST(0 as float) as fTongVonBoTriNganh,

						CAST(0 as float) as fHanmucCucTCDX,
						CAST(0 as float) as fTongVonBoTriCuc,
						CAST(0 as float) as fVon5namCTCDX,
						CAST(0 as float) as fVonnamthunhatCTC,
						CAST(0 as float) as fVonsaunamCTCDexuat,
						CAST(0 as float) as fCucTCDeXuat,
						CAST(0 as float) as fDuKienBoTriNamThu2,
						'' as SSoQuyetDinh,
						CAST(0 as int) as iLevel,
						CAST(1 as nvarchar) as STT

                from 
                        f_loai_cong_trinh_get_list_childrent(@lct) dmlct
                				
				WHERE
						 dmlct.bActive = 1
						
		 UNION ALL
		 select 
                        dmlct.iID_LoaiCongTrinh as IdLoaiCongTrinh,
                        dmlct.iID_Parent as IdLoaiCongTrinhParent,
                        dmlct.sMaLoaiCongTrinh as SMaLoaiCongTrinh,
						dmlct.sTenLoaiCongTrinh,
                        4 as Loai,
                        ctct.sTen as STenDuAn,
                        dv.sTen as STenDonVi,
                        ctct.fHanMucDauTu/@MenhGiaTienTe as FHanMucDauTu,
                        ns.sTen as STenNguonVon,                     
                        ((isnull(ctct.fGiaTriNamThuNhat, 0) 
                                + isnull(ctct.fGiaTriNamThuHai, 0) + isnull(ctct.fGiaTriNamThuBa, 0)
                                + isnull(ctct.fGiaTriNamThuTu, 0) + isnull(ctct.fGiaTriNamThuNam, 0))/@MenhGiaTienTe) as FTongSo,
                        (ctct.fGiaTriNamThuNhat/@MenhGiaTienTe) as FGiaTriNamThuNhat,
                        (ctct.fGiaTriNamThuHai/@MenhGiaTienTe) as FGiaTriNamThuHai,
                        (ctct.fGiaTriNamThuBa/@MenhGiaTienTe) as FGiaTriNamThuBa,
                        (ctct.fGiaTriNamThuTu/@MenhGiaTienTe) as FGiaTriNamThuTu,
                        (ctct.fGiaTriNamThuNam/@MenhGiaTienTe) as FGiaTriNamThuNam,
                        (ctct.fGiaTriBoTri/@MenhGiaTienTe) as FGiaTriBoTri,
                        ctct.sGhiChu as SGhiChu,
                        cast(0 as bit) as IsHangCha,
                        NEWID() as Id,
                        0 as LoaiParent,
                        ctct.iID_NguonVonID as IIdNguonVon,
                        null as DNgayQuyetDinh,
                        cast(0 as float) as FHanMucDauTuDP,
                        cast(0 as float) as FHanMucDauTuNN,
                        cast(0 as float) as FHanMucDauTuOrther,
                        cast(0 as float) as FHanMucDauTuQP,
   
						ISNULL(ctct.fHanmucNganhDX, 0)/@MenhGiaTienTe as fHanmucNganhDX,
						ISNULL(ctct.fVon5namNganhDX, 0)/@MenhGiaTienTe as fVon5namNganhDX,
						ISNULL(ctct.fVonsaunamNganhDX, 0)/@MenhGiaTienTe as fVonsaunamNganhDX,
						(ISNULL(ctct.fVon5namNganhDX, 0) + ISNULL(ctct.fVonsaunamNganhDX, 0))/@MenhGiaTienTe as fTongVonBoTriNganh,
						
						ISNULL(ctct.fHanmucCucTCDX, 0)/@MenhGiaTienTe as fHanmucCucTCDX,
						(ISNULL(ctct.fVon5namCTCDX, 0) + ISNULL(ctct.fVonsaunamCTCDexuat, 0))/@MenhGiaTienTe as fTongVonBoTriCuc,
						ISNULL(ctct.fVon5namCTCDX, 0)/@MenhGiaTienTe as fVon5namCTCDX,
						ISNULL(ctct.fVonnamthunhatCTC, 0)/@MenhGiaTienTe as fVonnamthunhatCTC,
						ISNULL(ctct.fVonsaunamCTCDexuat, 0)/@MenhGiaTienTe as fVonsaunamCTCDexuat,
						ISNULL(ctct.fCucTCDeXuat, 0)/@MenhGiaTienTe as fCucTCDeXuat,
						ISNULL(ctct.fDuKienBoTriNamThu2, 0)/@MenhGiaTienTe as fDuKienBoTriNamThu2,
						'' as SSoQuyetDinh,
						ctct.iLevel as iLevel,
						ctct.sSTT as STT

                from 
                        f_loai_cong_trinh_get_list_childrent(@lct) dmlct
                left join
                        VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet ctct
                on
                        dmlct.iID_LoaiCongTrinh = ctct.iID_LoaiCongTrinhID
				inner join 
						VDT_KHV_KeHoach5Nam_DeXuat vndx
                on ctct.iID_KeHoach5Nam_DeXuatID = vndx.iID_KeHoach5Nam_DeXuatID

                left join
                        NS_DonVi dv
                on vndx.iID_DonViQuanLyID = dv.iID_Ma
                left join 
                        NS_NguonNganSach ns
                on ctct.iID_NguonVonID = ns.iID_MaNguonNganSach
				
                where
                        ctct.iID_KeHoach5Nam_DeXuatID  in (select * from dbo.splitstring(@Id))
                        and ctct.iID_NguonVonID in (select * from dbo.splitstring(@IdNguonVon))
						--and ctct.iID_LoaiCongTrinhID in (SELECT * FROM dbo.f_split(@lct))
						and  vndx.sTongHop is not null
				order by sTenLoaiCongTrinh,Loai

	END
END



GO

