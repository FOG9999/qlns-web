USE [CTC_DB_TEST]
GO
/****** Object:  StoredProcedure [dbo].[sp_vdt_get_phan_bo_von_don_vi_dieu_chinh_report]    Script Date: 21/12/2022 5:22:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[sp_vdt_get_phan_bo_von_don_vi_dieu_chinh_report]
        @lstId nvarchar(max),
        @lstLct nvarchar(max),
        @YearPlan int,
        @type int,
        @DonViTienTe float
AS
BEGIN
        declare
                @idNguonVon int = 1;
		--if(@lstLct = '00000000-0000-0000-0000-000000000000')
		--			select DISTINCT iID_LoaiCongTrinh into #tmp from VDT_DM_LoaiCongTrinh where bActive =1
		--			select @lstLct=   Coalesce(@lstLct + ',' + convert(nvarchar(36), iID_LoaiCongTrinh),convert(nvarchar(36), iID_LoaiCongTrinh)) from #tmp;
		--			drop table #tmp
        if(@type = 1)
        begin
                        select tbl_kcm.* into #tbl_data_kcm from (

                        select
                                '' as STT,
                                lct.sTenLoaiCongTrinh as STenDuAn,
                                lct.iID_LoaiCongTrinh as IIdLoaiCongTrinh,
                                lct.iID_Parent as IIdLoaiCongTrinhParent,
                                3 as Loai,
                                case
                                        when lct.iID_Parent is null then 0 else 1
                                end LoaiParent,
                                cast(1 as bit) as IsHangCha,
                                '' as ThoiGianThucHien,
                                '' as DiaDiemThucHien,
                                '' as ChuDauTu,
                                '' as SoPheDuyet,
                                null as NgayPheDuyet,
                                cast(0 as float) as TongMucDauTu,
                                cast(0 as float) as VonBoTriDenHetNamTruoc,
                                cast(0 as float) as KeHoachVonDauTuNam,
                                cast(0 as float) as VonGiaiNganNam,
                                cast(0 as float) as DieuChinhVonNam,
                                '' as SGhiChu,
                                null as IdDuAn,
                                0 as IdNguonVon,
                                1 as LoaiCongTrinh
                        from f_loai_cong_trinh_get_list_childrent(@lstLct) lct

                        union all

                        select
                                distinct
                                '' as STT,
                                CASE
									WHEN dahm.sTenHangMuc IS NULL THEN da.sTenDuAn ELSE dahm.sTenHangMuc END as STenDuAn,
                                case
                                        when da.iID_LoaiCongTrinhID is not null then da.iID_LoaiCongTrinhID 
                                        else dahm.iID_LoaiCongTrinhID
                                end IIdLoaiCongTrinh,
                                null as IIdLoaiCongTrinhParent,
                                4 as Loai,
                                2 as LoaiParent,
                                cast(0 as bit) as IsHangCha,
                                (da.sKhoiCong + '-' + da.sKetThuc) as ThoiGianThucHien,
                                da.sDiaDiem as DiaDiemThucHien,
                                cdt.sTenCDT as ChuDauTu,
                                qddt.sSoQuyetDinh as SoPheDuyet,
                                qddt.dNgayQuyetDinh as NgayPheDuyet,
                                qddt.fTongMucDauTuPheDuyet/@DonViTienTe as TongMucDauTu,
                                (
                                        select 
                                                isnull(SUM(pbvddct.fCapPhatTaiKhoBac), 0)/@DonViTienTe + isnull(SUM(pbvddct.fCapPhatBangLenhChi), 0)/@DonViTienTe
                                        from
                                                VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet pbvddct
                                        inner join
                                                VDT_KHV_KeHoachVonNam_DuocDuyet pbvdd
                                        on pbvddct.iID_KeHoachVonNam_DuocDuyetID = pbvdd.iID_KeHoachVonNam_DuocDuyetID
                                        where
                                                pbvddct.iID_DuAnID = pbvct.iID_DuAnID
                                                and pbvdd.iNamKeHoach <= (@YearPlan -1)
                                ) as VonBoTriDenHetNamTruoc,
                                pbvct.fThanhToan/@DonViTienTe as KeHoachVonDauTuNam,
                                (
                                        select        
                                                isnull(SUM(pdtt.fGiaTriThanhToanTN/@DonViTienTe), 0)
                                        from
                                                VDT_TT_DeNghiThanhToan dntt
                                        inner join        
                                                VDT_TT_PheDuyetThanhToan_ChiTiet pdtt
                                        on dntt.iID_DeNghiThanhToanID = pdtt.iID_DeNghiThanhToanID
                                        where pbvct.iID_DuAnID = dntt.iID_DuAnId
                                ) as VonGiaiNganNam,
                                pbvct.fThanhToan/@DonViTienTe as DieuChinhVonNam,
                                '' as SGhiChu,
                                pbvct.iID_DuAnID as IdDuAn,
                                @idNguonVon as IdNguonVon,
                                1 as LoaiCongTrinh
                        from
                                VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet pbvct
                        inner join
                                VDT_DA_DuAn da
                        on pbvct.iID_DuAnID = da.iID_DuAnID
                        left join
                                VDT_DA_DuAn_HangMuc dahm
                        on pbvct.iID_DuAnID = dahm.iID_DuAnID and dahm.iID_NguonVonID = @idNguonVon AND dahm.iID_LoaiCongTrinhID = pbvct.iID_LoaiCongTrinh
                        left join
                                VDT_DA_QDDauTu qddt
                        on pbvct.iID_DuAnID = qddt.iID_DuAnID and qddt.bActive = 1
                        left join 
                                DM_ChuDauTu cdt 
                        on da.iID_ChuDauTuID = cdt.ID
                        where
                                pbvct.iID_KeHoachVonNamDeXuatID in (select * from dbo.splitstring(@lstId))
                                and (da.iID_LoaiCongTrinhID is not null or dahm.iID_LoaiCongTrinhID is not null)
                                and pbvct.ILoaiDuAn = 1
                ) as tbl_kcm

                insert into #tbl_data_kcm(
                        STT,
                        STenDuAn,
                        IIdLoaiCongTrinh,
                        IIdLoaiCongTrinhParent,
                        Loai,
                        LoaiParent,
                        IsHangCha,
                        ThoiGianThucHien,
                        DiaDiemThucHien,
                        ChuDauTu,
                        SoPheDuyet,
                        NgayPheDuyet,
                        TongMucDauTu,
                        VonBoTriDenHetNamTruoc,
                        KeHoachVonDauTuNam,
                        VonGiaiNganNam,
                        DieuChinhVonNam,
                        SGhiChu,
                        IdDuAn,
                        IdNguonVon,
                        LoaiCongTrinh
                )
                values('A', N'CÔNG TRÌNH MỞ MỚI', null, null, 1, 0, 1, '', '', '', '', null, 0, 0, 0, 0, 0, '', null, null, 1)
				 --select * from ##tbl_data_kcm order by IIdLoaiCongTrinh, Loai
                select ROW_NUMBER() OVER(order by IIdLoaiCongTrinh, Loai) as sSTT,tbldata.* from #tbl_data_kcm as tbldata order by IIdLoaiCongTrinh, Loai;
                drop table #tbl_data_kcm;
        end
        else
        begin
                select tbl_kct.* into #tbl_data_kct from (

                select
                        '' as STT,
                        lct.sTenLoaiCongTrinh as STenDuAn,
                        lct.iID_LoaiCongTrinh as IIdLoaiCongTrinh,
                        lct.iID_Parent as IIdLoaiCongTrinhParent,
                        3 as Loai,
                        case
                                when lct.iID_Parent is null then 0 else 1
                        end LoaiParent,
                        cast(1 as bit) as IsHangCha,
                        '' as ThoiGianThucHien,
                        '' as DiaDiemThucHien,
                        '' as ChuDauTu,
                        '' as SoPheDuyet,
                        null as NgayPheDuyet,
                        cast(0 as float) as TongMucDauTu,
                        cast(0 as float) as VonBoTriDenHetNamTruoc,
                        cast(0 as float) as KeHoachVonDauTuNam,
                        cast(0 as float) as VonGiaiNganNam,
                        cast(0 as float) as DieuChinhVonNam,
                        '' as SGhiChu,
                        null as IdDuAn,
                        0 as IdNguonVon,
                        2 as LoaiCongTrinh
                from f_loai_cong_trinh_get_list_childrent(@lstLct) lct

                union all

                select
                        distinct
                        '' as STT,
						 CASE
							WHEN dahm.sTenHangMuc IS NULL THEN da.sTenDuAn ELSE dahm.sTenHangMuc END as STenDuAn,
						case
                                when da.iID_LoaiCongTrinhID is not null then da.iID_LoaiCongTrinhID 
                                else dahm.iID_LoaiCongTrinhID
                        end IIdLoaiCongTrinh,
                        null as IIdLoaiCongTrinhParent,
                        4 as Loai,
                        2 as LoaiParent,
                        cast(0 as bit) as IsHangCha,
                        (da.sKhoiCong + '-' + da.sKetThuc) as ThoiGianThucHien,
                        da.sDiaDiem as DiaDiemThucHien,
                        cdt.sTenCDT as ChuDauTu,
                        qddt.sSoQuyetDinh as SoPheDuyet,
                        qddt.dNgayQuyetDinh as NgayPheDuyet,
                        qddt.fTongMucDauTuPheDuyet/@DonViTienTe as TongMucDauTu,
                        (
                                select 
                                        isnull(SUM(pbvddct.fCapPhatTaiKhoBac), 0)/@DonViTienTe + isnull(SUM(pbvddct.fCapPhatBangLenhChi), 0)/@DonViTienTe
                                from
                                        VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet pbvddct
                                inner join
                                        VDT_KHV_KeHoachVonNam_DuocDuyet pbvdd
                                on pbvddct.iID_KeHoachVonNam_DuocDuyetID = pbvdd.iID_KeHoachVonNam_DuocDuyetID
                                where
                                        pbvddct.iID_DuAnID = pbvct.iID_DuAnID
                                        and pbvdd.iNamKeHoach <= (@YearPlan -1)
                        ) as VonBoTriDenHetNamTruoc,
                        pbvct.fThanhToan/@DonViTienTe as KeHoachVonDauTuNam,
                        (
                                select        
                                        isnull(SUM(pdtt.fGiaTriThanhToanTN/@DonViTienTe), 0)
                                from
                                        VDT_TT_DeNghiThanhToan dntt
                                inner join        
                                        VDT_TT_PheDuyetThanhToan_ChiTiet pdtt
                                on dntt.iID_DeNghiThanhToanID = pdtt.iID_DeNghiThanhToanID
                                where pbvct.iID_DuAnID = dntt.iID_DuAnId
                        ) as VonGiaiNganNam,
                        pbvct.fThanhToan/@DonViTienTe as DieuChinhVonNam,
                        '' as SGhiChu,
                        pbvct.iID_DuAnID as IdDuAn,
                        @idNguonVon as IdNguonVon,
                        2 as LoaiCongTrinh
                from
                        VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet pbvct
                inner join
                        VDT_DA_DuAn da
                on pbvct.iID_DuAnID = da.iID_DuAnID
                left join
                        VDT_DA_DuAn_HangMuc dahm
                on pbvct.iID_DuAnID = dahm.iID_DuAnID and dahm.iID_NguonVonID = @idNguonVon AND dahm.iID_LoaiCongTrinhID = pbvct.iID_LoaiCongTrinh
                left join
                        VDT_DA_QDDauTu qddt
                on pbvct.iID_DuAnID = qddt.iID_DuAnID and qddt.bActive = 1
                left join
                        DM_ChuDauTu cdt
                on da.iID_ChuDauTuID = cdt.ID
                where
                        pbvct.iID_KeHoachVonNamDeXuatID in (select * from dbo.splitstring(@lstId))
                        and (da.iID_LoaiCongTrinhID is not null or dahm.iID_LoaiCongTrinhID is not null)
                        and pbvct.ILoaiDuAn = 2
        ) as tbl_kct

                insert into #tbl_data_kct(
                        STT,
                        STenDuAn,
                        IIdLoaiCongTrinh,
                        IIdLoaiCongTrinhParent,
                        Loai,
                        LoaiParent,
                        IsHangCha,
                        ThoiGianThucHien,
                        DiaDiemThucHien,
                        ChuDauTu,
                        SoPheDuyet,
                        NgayPheDuyet,
                        TongMucDauTu,
                        VonBoTriDenHetNamTruoc,
                        KeHoachVonDauTuNam,
                        VonGiaiNganNam,
                        DieuChinhVonNam,
                        SGhiChu,
                        IdDuAn,
                        IdNguonVon,
                        LoaiCongTrinh
                )
                values('B', N'CÔNG TRÌNH CHUYỂN TIẾP', null, null, 1, 0, 1, '', '', '', '', null, 0, 0, 0, 0, 0, '', null, null, 2)

                --select * from #tbl_data_kct order by IIdLoaiCongTrinh, Loai
				select ROW_NUMBER() OVER(order by IIdLoaiCongTrinh, Loai) as sSTT,tbldatact.* from #tbl_data_kct as tbldatact order by IIdLoaiCongTrinh, Loai;
                drop table #tbl_data_kct;
        end
END
