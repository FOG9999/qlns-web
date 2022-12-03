
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <30/11/2022 ,,>
-- Description:	<sp_ke_hoach_trung_han_de_xuat_chuyen_tiep_report,,>
-- =============================================
CREATE PROCEDURE sp_ke_hoach_trung_han_de_xuat_chuyen_tiep_report
	-- Add the parameters for the stored procedure here
	@lstId nvarchar(max),
	@lstLct nvarchar(max),
	@lstIdNguonVon nvarchar(max),
    @DonViTienTe float,
	@type int
AS
BEGIN
	IF(@type = 1)
		BEGIN
			--Tinh fTongVondabotri-- col7
			select
                        pbvct.iID_DuAnID,
                        (SUM(isnull(pbvct.fCapPhatTaiKhoBac, 0)) + SUM(isnull(pbvct.fCapPhatBangLenhChi, 0))) as fVonBoTriHetNam
                        into #tmpPhanBoVon
                from
                        VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet pbvct
                inner join
                        VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet khthct
                on pbvct.iID_DuAnID = khthct.iID_DuAnID
                inner join
                        VDT_DA_DuAn da
                on pbvct.iID_DuAnID = da.iID_DuAnID
                inner join
                        VDT_KHV_KeHoach5Nam_DeXuat khthdx
                on
                        khthct.iID_KeHoach5Nam_DeXuatID = khthdx.iID_KeHoach5Nam_DeXuatID
                inner join
                        VDT_KHV_KeHoachVonNam_DuocDuyet pbv
                on
                        pbvct.iID_KeHoachVonNam_DuocDuyetID = pbv.iID_KeHoachVonNam_DuocDuyetID
				left join 
						VDT_DA_DuAn_NguonVon danv
				on
						danv.iID_DuAn = da.iID_DuAnID
                where
                        khthct.iID_KeHoach5Nam_DeXuatID in (select * from dbo.splitstring(@lstId))
						and danv.iID_NguonVonID in (select * from dbo.splitstring(@lstIdNguonVon))
                        and pbv.iNamKeHoach = (khthdx.iGiaiDoanTu -1)
						and pbv.bActive = 1
                group by pbvct.iID_DuAnID

				--Tinh col 10--

				select
                        pbvct.iID_DuAnID,
                        (SUM(isnull(pbvct.fCapPhatTaiKhoBac, 0)) + SUM(isnull(pbvct.fCapPhatBangLenhChi, 0))) as fBoDaGiaoDuToan
                        into #tmpPhanBoVonDuToan
                from
                        VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet pbvct
                inner join
                        VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet khthct
                on pbvct.iID_DuAnID = khthct.iID_DuAnID
                inner join
                        VDT_DA_DuAn da
                on pbvct.iID_DuAnID = da.iID_DuAnID
                inner join
                        VDT_KHV_KeHoach5Nam_DeXuat khthdx
                on
                        khthct.iID_KeHoach5Nam_DeXuatID = khthdx.iID_KeHoach5Nam_DeXuatID
                inner join
                        VDT_KHV_KeHoachVonNam_DuocDuyet pbv
                on
                        pbvct.iID_KeHoachVonNam_DuocDuyetID = pbv.iID_KeHoachVonNam_DuocDuyetID
				left join 
						VDT_DA_DuAn_NguonVon danv
				on
						danv.iID_DuAn = da.iID_DuAnID
                where
                        khthct.iID_KeHoach5Nam_DeXuatID in (select * from dbo.splitstring(@lstId))
						and danv.iID_NguonVonID in (select * from dbo.splitstring(@lstIdNguonVon))
                        and pbv.iNamKeHoach = khthdx.iGiaiDoanTu
						and pbv.bActive = 1
                group by pbvct.iID_DuAnID

			SELECT tbl_sum1.* FROM (
			SELECT 
				dmlct.iID_LoaiCongTrinh as IdLoaiCongTrinh,
				dmlct.iID_Parent as IdLoaiCongTrinhParent,
                dmlct.sMaLoaiCongTrinh as SMaLoaiCongTrinh,
				dmlct.sTenLoaiCongTrinh,
				khdx.iLoai as iLoai,
				3 as LoaiParent,
				da.sTenDuAn as STenDuAn,
                dv.sTen as STenDonVi,
				khdx.sSoQuyetDinh as sSoQuyetDinh,
				khdx.dNgayQuyetDinh as dNgayQuyetDinh,
				CONCAT(qddtt.sKhoiCong, ' - ', qddtt.sKetThuc) as sTienDo, --col4
				qddtnv.fTienPheDuyetCTDT/@DonViTienTe  as fNSQP, --col5
				CAST(0 as float)  as fChiPhiDuPhong, --col6
				pbv.fVonBoTriHetNam/@DonViTienTe as fVonBoTriHetNam,  --col7
				(ISNULL(qddtnv.fTienPheDuyetCTDT,0) - ISNULL(pbv.fVonBoTriHetNam,0))/@DonViTienTe as fSoConLaiPhaiBoTri, --col8
				khthct.fCucTCDeXuat/@DonViTienTe as fCucTCdexuat, --col9
				pbvdv.fBoDaGiaoDuToan/@DonViTienTe as fBoDaGiaoDuToan, --col10
				khthct.fDuKienBoTriNamThu2/@DonViTienTe as fDuKienbotrinamthu2, --col11
				(ISNULL(khthct.fCucTCDeXuat,0) - ISNULL(pbvdv.fBoDaGiaoDuToan,0) - ISNULL(khthct.fDuKienBoTriNamThu2,0))/@DonViTienTe as fSoConLaiConBotri, --col12 = 9-10-11
				(ISNULL(qddtnv.fTienPheDuyet,0) - ISNULL(pbv.fVonBoTriHetNam,0) - ISNULL(khthct.fCucTCDeXuat,0))/@DonViTienTe as fSoTienConLaiChuaDeXuat ,--col13 = 5-7-9
				CAST(0 as bit) as IsHangCha,
				nv.iID_MaNguonNganSach as IdNguonVon
			FROM 			 
                        VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet khthct                
                left join
                        VDT_DA_DuAn da
                on
                        khthct.iID_DuAnID = da.iID_DuAnID
				left join
                        VDT_DM_LoaiCongTrinh dmlct
                on
                        da.iID_LoaiCongTrinhID = dmlct.iID_LoaiCongTrinh               
                left join
                        VDT_DA_QDDauTu qddtt
                on khthct.iID_DuAnID = qddtt.iID_DuAnID
				left join
						VDT_DA_DuAn_NguonVon danv
				on
						danv.iID_DuAn = da.iID_DuAnID
                left join 
                        VDT_DA_QDDauTu_NguonVon qddtnv 
                on qddtt.iID_QDDauTuID = qddtnv.iID_QDDauTuID and danv.iID_NguonVonID = qddtnv.iID_NguonVonID
                left join
                        NS_NguonNganSach nv
                on
                        danv.iID_NguonVonID = nv.iID_MaNguonNganSach
                left join
                        #tmpPhanBoVonDuToan pbvdv
                on
                        pbvdv.iID_DuAnID = khthct.iID_DuAnID
                left join
                        #tmpPhanBoVon pbv
                on 
                        pbv.iID_DuAnID = khthct.iID_DuAnID
               inner join 
                        VDT_KHV_KeHoach5Nam_DeXuat khdx
                on khdx.iID_KeHoach5Nam_DeXuatID = khthct.iID_KeHoach5Nam_DeXuatID
			    left join
                        NS_DonVi dv
                on
                        khdx.iID_DonViQuanLyID = dv.iID_Ma
			WHERE
				khthct.iID_KeHoach5Nam_DeXuatID in (select * from dbo.splitstring(@lstId))
				and danv.iID_NguonVonID in (select * from dbo.splitstring(@lstIdNguonVon)) ) as tbl_sum1
			
			ORder BY SMaLoaiCongTrinh,LoaiParent;

			drop table #tmpPhanBoVon;
			drop table #tmpPhanBoVonDuToan;
		END
	ELSE
	IF(@type = 2)
		BEGIN
			--Tinh fTongVondabotri--
			select
                        pbvct.iID_DuAnID,
                        (SUM(isnull(pbvct.fCapPhatTaiKhoBac, 0)) + SUM(isnull(pbvct.fCapPhatBangLenhChi, 0))) as fVonBoTriHetNam
                        into #tmpPhanBoVonTH
                from
                        VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet pbvct
                inner join
                        VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet khthct
                on pbvct.iID_DuAnID = khthct.iID_DuAnID
                inner join
                        VDT_DA_DuAn da
                on pbvct.iID_DuAnID = da.iID_DuAnID
                inner join
                        VDT_KHV_KeHoach5Nam_DeXuat khthdx
                on
                        khthct.iID_KeHoach5Nam_DeXuatID = khthdx.iID_KeHoach5Nam_DeXuatID
                inner join
                        VDT_KHV_KeHoachVonNam_DuocDuyet pbv
                on
                        pbvct.iID_KeHoachVonNam_DuocDuyetID = pbv.iID_KeHoachVonNam_DuocDuyetID
				left join 
						VDT_DA_DuAn_NguonVon danv
				on
						danv.iID_DuAn = da.iID_DuAnID
                where
                        khthct.iID_KeHoach5Nam_DeXuatID in (select * from dbo.splitstring(@lstId))
						and danv.iID_NguonVonID in (select * from dbo.splitstring(@lstIdNguonVon))
                        and pbv.iNamKeHoach = (khthdx.iGiaiDoanTu -1)
						and pbv.bActive = 1
                group by pbvct.iID_DuAnID

				--Tinh col 10--

				select
                        pbvct.iID_DuAnID,
                        (SUM(isnull(pbvct.fCapPhatTaiKhoBac, 0)) + SUM(isnull(pbvct.fCapPhatBangLenhChi, 0))) as fBoDaGiaoDuToan
                        into #tmpPhanBoVonDuToanTH
                from
                        VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet pbvct
                inner join
                        VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet khthct
                on pbvct.iID_DuAnID = khthct.iID_DuAnID
                inner join
                        VDT_DA_DuAn da
                on pbvct.iID_DuAnID = da.iID_DuAnID
                inner join
                        VDT_KHV_KeHoach5Nam_DeXuat khthdx
                on
                        khthct.iID_KeHoach5Nam_DeXuatID = khthdx.iID_KeHoach5Nam_DeXuatID
                inner join
                        VDT_KHV_KeHoachVonNam_DuocDuyet pbv
                on
                        pbvct.iID_KeHoachVonNam_DuocDuyetID = pbv.iID_KeHoachVonNam_DuocDuyetID
				left join 
						VDT_DA_DuAn_NguonVon danv
				on
						danv.iID_DuAn = da.iID_DuAnID
                where
                        khthct.iID_KeHoach5Nam_DeXuatID in (select * from dbo.splitstring(@lstId))
						and danv.iID_NguonVonID in (select * from dbo.splitstring(@lstIdNguonVon))
                        and pbv.iNamKeHoach = khthdx.iGiaiDoanTu
						and pbv.bActive = 1
                group by pbvct.iID_DuAnID

			SELECT tbl_sum2.* FROM ( SELECT 
				dmlct.iID_LoaiCongTrinh as IdLoaiCongTrinh,
				dmlct.iID_Parent as IdLoaiCongTrinhParent,
                dmlct.sMaLoaiCongTrinh as SMaLoaiCongTrinh,
				dmlct.sTenLoaiCongTrinh,
				2 as iLoai,
				2 as LoaiParent,
				dmlct.sTenLoaiCongTrinh as STenDuAn,
                '' as STenDonVi,
				'' as sSoQuyetDinh,
				'' as dNgayQuyetDinh,
				'' as sTienDo, --col4
				CAST(0 as float)  as fNSQP, --col5
				CAST(0 as float)  as fChiPhiDuPhong, --col6
				CAST(0 as float) as fVonBoTriHetNam,  --col7
				CAST(0 as float) as fSoConLaiPhaiBoTri, --col8
				CAST(0 as float) as fCucTCdexuat, --col9
				CAST(0 as float) as fBoDaGiaoDuToan, --col10
				CAST(0 as float) as fDuKienbotrinamthu2, --col11
				CAST(0 as float) as fSoConLaiConBotri, --col12 = 9-10-11
				CAST(0 as float) as fSoTienConLaiChuaDeXuat, --col13 = 5-7-9
				CAST(1 as bit) as IsHangCha,
				null as IdNguonVon
			FROM 
				f_loai_cong_trinh_get_list_childrent(@lstLct) dmlct
			WHERE
				dmlct.bActive = 1

			UNION ALL
			SELECT 
				dmlct.iID_LoaiCongTrinh as IdLoaiCongTrinh,
				dmlct.iID_Parent as IdLoaiCongTrinhParent,
                dmlct.sMaLoaiCongTrinh as SMaLoaiCongTrinh,
				dmlct.sTenLoaiCongTrinh,
				khdx.iLoai as iLoai,
				3 as LoaiParent,
				da.sTenDuAn as STenDuAn,
                dv.sTen as STenDonVi,
				khdx.sSoQuyetDinh as sSoQuyetDinh,
				khdx.dNgayQuyetDinh as dNgayQuyetDinh,
				CONCAT(qddtt.sKhoiCong, ' - ', qddtt.sKetThuc) as sTienDo, --col4
				qddtnv.fTienPheDuyetCTDT/@DonViTienTe  as fNSQP, --col5
				CAST(0 as float)  as fChiPhiDuPhong, --col6
				pbv.fVonBoTriHetNam/@DonViTienTe as fVonBoTriHetNam,  --col7
				(ISNULL(qddtnv.fTienPheDuyetCTDT,0) - ISNULL(pbv.fVonBoTriHetNam,0))/@DonViTienTe as fSoConLaiPhaiBoTri, --col8
				khthct.fCucTCDeXuat/@DonViTienTe as fCucTCdexuat, --col9
				pbvdv.fBoDaGiaoDuToan/@DonViTienTe as fBoDaGiaoDuToan, --col10
				khthct.fDuKienBoTriNamThu2/@DonViTienTe as fDuKienbotrinamthu2, --col11
				(ISNULL(khthct.fCucTCDeXuat,0) - ISNULL(pbvdv.fBoDaGiaoDuToan,0) - ISNULL(khthct.fDuKienBoTriNamThu2,0))/@DonViTienTe as fSoConLaiConBotri, --col12 = 9-10-11
				(ISNULL(qddtnv.fTienPheDuyet,0) - ISNULL(pbv.fVonBoTriHetNam,0) - ISNULL(khthct.fCucTCDeXuat,0))/@DonViTienTe as fSoTienConLaiChuaDeXuat ,--col13 = 5-7-9
				CAST(0 as bit) as IsHangCha,
				nv.iID_MaNguonNganSach as IdNguonVon

			FROM 			 
                        VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet khthct                
                left join
                        VDT_DA_DuAn da
                on
                        khthct.iID_DuAnID = da.iID_DuAnID
				left join
                        VDT_DM_LoaiCongTrinh dmlct
                on
                        da.iID_LoaiCongTrinhID = dmlct.iID_LoaiCongTrinh                
                left join
                        VDT_DA_QDDauTu qddtt
                on khthct.iID_DuAnID = qddtt.iID_DuAnID
				left join
						VDT_DA_DuAn_NguonVon danv
				on
						danv.iID_DuAn = da.iID_DuAnID
                left join 
                        VDT_DA_QDDauTu_NguonVon qddtnv 
                on qddtt.iID_QDDauTuID = qddtnv.iID_QDDauTuID and danv.iID_NguonVonID = qddtnv.iID_NguonVonID
                left join
                        NS_NguonNganSach nv
                on
                        danv.iID_NguonVonID = nv.iID_MaNguonNganSach
                left join
                        #tmpPhanBoVonDuToanTH pbvdv
                on
                        pbvdv.iID_DuAnID = khthct.iID_DuAnID
                left join
                        #tmpPhanBoVonTH pbv
                on 
                        pbv.iID_DuAnID = khthct.iID_DuAnID
                inner join 
                        VDT_KHV_KeHoach5Nam_DeXuat khdx
                on khdx.iID_KeHoach5Nam_DeXuatID = khthct.iID_KeHoach5Nam_DeXuatID
				 left join
                        NS_DonVi dv
                on
                        khdx.iID_DonViQuanLyID = dv.iID_Ma
			WHERE
				khthct.iID_KeHoach5Nam_DeXuatID in (select * from dbo.splitstring(@lstId))
				and danv.iID_NguonVonID in (select * from dbo.splitstring(@lstIdNguonVon)) ) as tbl_sum2
			ORder BY SMaLoaiCongTrinh,LoaiParent;

			drop table #tmpPhanBoVonTH;
			drop table #tmpPhanBoVonDuToanTH;
		END
END
GO


--drop PROCEDURE sp_ke_hoach_trung_han_de_xuat_chuyen_tiep_report


