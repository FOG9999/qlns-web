CREATE PROC [dbo].[sp_vdt_get_phan_bo_von_don_vi_phe_duyet_nguonvon_dieu_chinh_report]
	@lstId nvarchar(max),
	@lstDonVi nvarchar(max),
	@DonViTienTe float 
AS
BEGIN
	declare @lstNhomDuAn t_tbl_pbv_string, @lstDv t_tbl_string,@iNamKeHoach int;

	insert into @lstDv(sId)
	select
		pbv.iID_DonViQuanLyID
	from
		VDT_KHV_PhanBoVon_DonVi_PheDuyet pbv 
	where pbv.Id in (select * from dbo.splitstring(@lstId))

	select
		@iNamKeHoach = iNamKeHoach
	from
		VDT_KHV_PhanBoVon_DonVi_PheDuyet pbv
	where pbv.Id in (select TOP 1 * from dbo.splitstring(@lstId))

	insert into @lstNhomDuAn(
		sId,
		sMoTa
	)
	select
		distinct
		nda.iID_NhomDuAnID,
		nda.sTenNhomDuAn
	from
		VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet pbvct
	inner join
		VDT_DA_DuAn da
	on
		pbvct.iID_DuAnID = da.iID_DuAnID
	inner join
		VDT_DM_NhomDuAn nda
	on da.iID_NhomDuAnID = nda.iID_NhomDuAnID
	where
		pbvct.iID_PhanBoVon_DonVi_PheDuyet_ID in (select * from dbo.splitstring(@lstId))

	select
		cast(nda.sId as uniqueidentifier) as IdNhomDuAn,
		da.sTenDuAn as STenDuAn,
		da.sDiaDiem as DiaDiemXayDung,
		'' as DiaDiemMoTaiKhoanDuAn,
		cdt.sTenCDT as ChuDauTu,
		'' as MaSoDuAnDauTu,
		'' as MaNganhKinhTe,
		'' as NangLucThietKe,
		(da.sKhoiCong + '-' + da.sKetThuc) as ThoiGianThucHien,
		qddt.sSoQuyetDinh as SSoQuyetDinh,
		qddt.dNgayQuyetDinh as DNgayQuyetDinh,
		qddt.fTongMucDauTuPheDuyet/@DonViTienTe as TongSoVonDauTu,
		(
			select
				isnull(SUM(khnct.fVonBoTriTuNamDenNam/@DonViTienTe), 0)
			from
				VDT_KHV_KeHoach5Nam khn
			inner join
				@lstDv lsdv
			on
				khn.iID_DonViQuanLyID = lsdv.sId
			inner join
				VDT_KHV_KeHoach5Nam_ChiTiet khnct
			on
				khnct.iID_KeHoach5NamID = khn.iID_KeHoach5NamID
			where khn.iGiaiDoanTu <= @iNamKeHoach and khn.iGiaiDoanDen >= @iNamKeHoach
		) as KeHoachVonDauTuGiaiDoan,
		(
			select
				isnull(SUM(pdttct.fGiaTriThanhToanTN/@DonViTienTe), 0) 
				+ isnull(SUM(pdttct.fGiaTriThanhToanNN/@DonViTienTe), 0) - isnull(SUM(pdttct.fGiaTriThuHoiNamNayNN/@DonViTienTe), 0)
			from
				VDT_TT_DeNghiThanhToan dntt
			inner join
				VDT_TT_PheDuyetThanhToan_ChiTiet pdttct
			on pdttct.iID_DeNghiThanhToanID = dntt.iID_DeNghiThanhToanID
			inner join
				VDT_DA_DuAn da
			on dntt.iID_DuAnId = da.iID_DuAnID
			where dntt.iID_DuAnId = pbvdvct.iID_DuAnID
			and da.sKhoiCong >= @iNamKeHoach
		) as VonThanhToanLuyKe,
		cast(0 as float) as TongSoKeHoachVonNam,
		cast(0 as float) as ThuHoiVonDaUngTruoc,
		(
			select
				isnull(SUM(pdttct.fGiaTriThanhToanTN/@DonViTienTe), 0) 
				+ isnull(SUM(pdttct.fGiaTriThanhToanNN/@DonViTienTe), 0)
				- isnull(SUM(pdttct.fGiaTriThuHoiNamNayNN/@DonViTienTe), 0)
			from
				VDT_TT_DeNghiThanhToan dntt
			inner join
				VDT_TT_PheDuyetThanhToan_ChiTiet pdttct
			on pdttct.iID_DeNghiThanhToanID = dntt.iID_DeNghiThanhToanID
			inner join
				VDT_DA_DuAn da
			on dntt.iID_DuAnId = da.iID_DuAnID
			where dntt.iID_DuAnId = pbvdvct.iID_DuAnID
			and dntt.dNgayDeNghi >= cast('01-01-' + cast(@iNamKeHoach as nvarchar(10)) as date) and dntt.dNgayDeNghi <= GETDATE()
		) as VonThucHienTuDauNamDenNay,
		cast(0 as float) as TongSoVonNamDieuChinh,
		(
			select 
				isnull(SUM(pbvct.fGiaTriThuHoi/@DonViTienTe), 0)
			from 
				VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet pbvct
			inner join
				VDT_KHV_PhanBoVon_DonVi_PheDuyet pbv
			on pbvct.iID_PhanBoVon_DonVi_PheDuyet_ID = pbv.Id
			where 
				pbvct.iID_DuAnID = pbvdvct.iID_DuAnID
				and pbvct.iID_LoaiCongTrinh = pbvdvct.iID_LoaiCongTrinh
				and pbv.iNamKeHoach = @iNamKeHoach
				and pbv.bActive = 1
				and pbv.iID_ParentId is not null
		) as ThuHoiVonDaUngTruocDieuChinh,
		(
			select isnull(SUM(pbvct.fGiaTriPhanBo/@DonViTienTe), 0)
			from 
				VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet pbvct
			inner join
				VDT_KHV_PhanBoVon_DonVi_PheDuyet pbv
			on pbvct.iID_PhanBoVon_DonVi_PheDuyet_ID = pbv.Id
			where 
				pbvct.iID_DuAnID = pbvdvct.iID_DuAnID
				and pbvct.iID_LoaiCongTrinh = pbvdvct.iID_LoaiCongTrinh
				and pbv.iNamKeHoach = @iNamKeHoach
				and pbv.bActive = 1
				and pbv.iID_ParentId is not null
		) as TraNoXDCB,
		'' as SGhiChu,
		cast(0 as bit) as IsHangCha,
		2 as Loai
	from
		VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet pbvdvct
	inner join
		VDT_DA_DuAn da
	on pbvdvct.iID_DuAnID = da.iID_DuAnID
	left join
		@lstNhomDuAn nda
	on nda.sId = da.iID_NhomDuAnID
	left join
		VDT_DA_QDDauTu qddt
	on 
		da.iID_DuAnID = qddt.iID_DuAnID
		and qddt.bActive = 1
	left join 
		DM_ChuDauTu cdt
	on da.iID_ChuDauTuID = cdt.ID
	where
		pbvdvct.iID_PhanBoVon_DonVi_PheDuyet_ID in (select * from dbo.splitstring(@lstId))
		and da.iID_MaDonViThucHienDuAnID in (select * from dbo.splitstring(@lstDonVi))
		and pbvdvct.iID_LoaiCongTrinh is not null

	union all
	
	select
		cast(nda.sId as uniqueidentifier) as IdNhomDuAn,
		nda.sMoTa as STenDuAn,
		'' as DiaDiemXayDung,
		'' as DiaDiemMoTaiKhoanDuAn,
		'' as ChuDauTu,
		'' as MaSoDuAnDauTu,
		'' as MaNganhKinhTe,
		'' as NangLucThietKe,
		'' as ThoiGianThucHien,
		'' as SSoQuyetDinh,
		null as DNgayQuyetDinh,
		cast(0 as float) as TongSoVonDauTu,
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

	order by IdNhomDuAn, Loai
END
;
;
