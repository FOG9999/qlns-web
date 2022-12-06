
declare @lstNhomDuAn t_tbl_pbv_string, @lstDonVi t_tbl_string,@iNamKeHoach int;

	insert into @lstDonVi(sId)
	select
		pbv.iID_DonViQuanLyID
	from
		VDT_KHV_KeHoachVonNam_DuocDuyet pbv 
	where pbv.iID_KeHoachVonNam_DuocDuyetID in (select * from dbo.f_split(@lstId))

	select
		@iNamKeHoach = iNamKeHoach
	from
		VDT_KHV_KeHoachVonNam_DuocDuyet pbv
	where pbv.iID_KeHoachVonNam_DuocDuyetID in (select TOP 1 * from dbo.f_split(@lstId))

	insert into @lstNhomDuAn(
		sId,
		sMoTa
	)
	select
		distinct
		nda.iID_NhomDuAnID,
		nda.sTenNhomDuAn
	from
		VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet pbvct
	inner join
		VDT_DA_DuAn da
	on
		pbvct.iID_DuAnID = da.iID_DuAnID
	inner join
		VDT_DM_NhomDuAn nda
	on da.iID_NhomDuAnID = nda.iID_NhomDuAnID
	where
		pbvct.iID_KeHoachVonNam_DuocDuyetID in (select * from dbo.f_split(@lstId))


	select
		nda.sId as IdNhomDuAn,
		nda.sMoTa as STenDuAn,
		'' as DiaDiemXayDung,
		'' as DiaDiemMoTaiKhoanDuAn,
		'' as ChuDauTu,
		'' as MaSoDuAnDauTu,
		'' as MaNganhKinhTe,
		'' as NangLucThietKe,
		'' as ThoiGianThucHien,
		'' as SoNgayThangNam,
		cast(0 as float) as TongSoVonDauTu,
		cast(0 as float) as TongSoVonDauTuTrongNuoc,
		cast(0 as float) as KeHoachVonDauTuGiaiDoan,
		cast(0 as float) as VonThanhToanLuyKe,
		cast(0 as float) as TongSoKeHoachVonNam,
		cast(0 as float) as ThuHoiVonDaUngTruoc,
		cast(0 as float) as VonThucHienTuDauNamDenNay,
		cast(0 as float) as TongSoVonNamDieuChinh,
		cast(0 as float) as ThuHoiVonDaUngTruocDieuChinh,
		cast(0 as float) as TraNoXDCB,
		'' as SGhiChu,
		cast(1 as bit) as IsHangCha,
		1 as Loai
	from
		@lstNhomDuAn nda

	union all

	select
		nda.iID_NhomDuAnID as IdNhomDuAn,
		da.sTenDuAn as STenDuAn,
		da.sDiaDiem as DiaDiemXayDung,
		'' as DiaDiemMoTaiKhoanDuAn,
		'' as ChuDauTu,
		'' as MaSoDuAnDauTu,
		'' as MaNganhKinhTe,
		'' as NangLucThietKe,
		(da.sKhoiCong + '-' + da.sKetThuc) as ThoiGianThucHien,
		(qddt.sSoQuyetDinh + '-' + cast(qddt.dNgayQuyetDinh as nvarchar(max))) as SoNgayThangNam,
		qddt.fTongMucDauTuPheDuyet/@DonViTienTe as TongSoVonDauTu,
		(
			select
				SUM(qddtnv.fTienPheDuyet)
			from
				VDT_DA_QDDauTu_NguonVon qddtnv
			where
				qddtnv.iID_QDDauTuID = qddt.iID_QDDauTuID
				and qddtnv.iID_NguonVonID = 2
		) as TongSoVonDauTuTrongNuoc,
		(
			select
				isnull(SUM(khnct.fVonBoTriTuNamDenNam), 0)/@DonViTienTe
			from
				VDT_KHV_KeHoach5Nam khn
			inner join
				@lstDonVi lsdv
			on
				khn.iID_DonViQuanLyID = lsdv.sId
			inner join
				VDT_KHV_KeHoach5Nam_ChiTiet khnct
			on
				khnct.iID_KeHoach5NamID = khn.iID_KeHoach5NamID
			where khn.iGiaiDoanTu <= @iNamKeHoach and khn.iGiaiDoanDen >= @iNamKeHoach
		)as KeHoachVonDauTuGiaiDoan,
		(
			select
				isnull(SUM(pdttct.fGiaTriThanhToanTN), 0)/@DonViTienTe + isnull(SUM(pdttct.fGiaTriThanhToanNN), 0)/@DonViTienTe - isnull(SUM(pdttct.fGiaTriThuHoiNamNayNN), 0)/@DonViTienTe
			from
				VDT_TT_DeNghiThanhToan dntt
			inner join
				VDT_TT_PheDuyetThanhToan_ChiTiet pdttct
			on pdttct.iID_DeNghiThanhToanID = dntt.iID_DeNghiThanhToanID
			inner join
				VDT_DA_DuAn da
			on dntt.iID_DuAnId = da.iID_DuAnID
			where dntt.iID_DuAnId = pbvct.iID_DuAnID
			and da.sKhoiCong >= @iNamKeHoach
		) as VonThanhToanLuyKe,
		(
			select 
				isnull(SUM(pbvdvct.fThanhToan), 0)/@DonViTienTe
			from 
				VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet pbvdvct
			inner join
				VDT_KHV_KeHoachVonNam_DeXuat pbvdv
			on pbvdvct.iID_KeHoachVonNamDeXuatID = pbvdv.iID_KeHoachVonNamDeXuatID
			where 
				pbvdvct.iID_DuAnID = pbvct.iID_DuAnID
				and pbvdv.iNamKeHoach = @iNamKeHoach
		) as TongSoKeHoachVonNam,
		(
			select 
				isnull(SUM(pbvdvct.fThuHoiVonUngTruoc), 0)/@DonViTienTe
			from 
				VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet pbvdvct
			inner join
				VDT_KHV_KeHoachVonNam_DeXuat pbvdv
			on pbvdvct.iID_KeHoachVonNamDeXuatID = pbvdv.iID_KeHoachVonNamDeXuatID
			where 
				pbvdvct.iID_DuAnID = pbvct.iID_DuAnID
				and pbvdv.iNamKeHoach = @iNamKeHoach
		) as ThuHoiVonDaUngTruoc,
		(
			select
				isnull(SUM(pdttct.fGiaTriThanhToanTN), 0)/@DonViTienTe + isnull(SUM(pdttct.fGiaTriThanhToanNN), 0)/@DonViTienTe - isnull(SUM(pdttct.fGiaTriThuHoiNamNayNN), 0)/@DonViTienTe
			from
				VDT_TT_DeNghiThanhToan dntt
			inner join
				VDT_TT_PheDuyetThanhToan_ChiTiet pdttct
			on pdttct.iID_DeNghiThanhToanID = dntt.iID_DeNghiThanhToanID
			inner join
				VDT_DA_DuAn da
			on dntt.iID_DuAnId = da.iID_DuAnID
			where dntt.iID_DuAnId = pbvct.iID_DuAnID
			and dntt.dNgayDeNghi >= cast('01-01-' + cast(@iNamKeHoach as nvarchar(10)) as date) and dntt.dNgayDeNghi <= GETDATE()
		) as VonThucHienTuDauNamDenNay,
		cast(0 as float) as TongSoVonNamDieuChinh,
		(
			select 
				isnull(SUM(pbvdvct.fThuHoiVonUngTruoc), 0)/@DonViTienTe
			from 
				VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet pbvdvct
			inner join
				VDT_KHV_KeHoachVonNam_DeXuat pbvdv
			on pbvdvct.iID_KeHoachVonNamDeXuatID = pbvdv.iID_KeHoachVonNamDeXuatID
			where 
				pbvdvct.iID_DuAnID = pbvct.iID_DuAnID
				and pbvdv.iNamKeHoach <= @iNamKeHoach
				and bIsGoc = 0 and bActive = 1
		) as ThuHoiVonDaUngTruocDieuChinh,
		(
			select 
				isnull(SUM(pbvdvct.fThanhToan), 0)/@DonViTienTe
			from 
				VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet pbvdvct
			inner join
				VDT_KHV_KeHoachVonNam_DeXuat pbvdv
			on pbvdvct.iID_KeHoachVonNamDeXuatID = pbvdv.iID_KeHoachVonNamDeXuatID
			where 
				pbvdvct.iID_DuAnID = pbvct.iID_DuAnID
				and pbvdv.iNamKeHoach <= @iNamKeHoach
				and bIsGoc = 0 and bActive = 1
		) as TraNoXDCB,
		'' as SGhiChu,
		cast(0 as bit) as IsHangCha,
		2 as Loai
	from
		VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet pbvct
	inner join
		VDT_DA_DuAn da
	on 
		pbvct.iID_DuAnID = da.iID_DuAnID
	left join
		VDT_DA_QDDauTu qddt
	on
		da.iID_DuAnID = qddt.iID_DuAnID
		and qddt.bActive = 1
	left join
		VDT_DM_NhomDuAn nda
	on
		da.iID_NhomDuAnID = nda.iID_NhomDuAnID
	where
		pbvct.iID_KeHoachVonNam_DuocDuyetID in (select * from dbo.f_split(@lstId))
