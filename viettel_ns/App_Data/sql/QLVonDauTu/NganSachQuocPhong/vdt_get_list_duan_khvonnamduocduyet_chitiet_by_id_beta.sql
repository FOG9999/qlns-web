DECLARE @phanBoVonId uniqueidentifier set @phanBoVonId = null
DECLARE @iIdPhanBoVonDeXuat uniqueidentifier set @iIdPhanBoVonDeXuat = null
DECLARE @iNamLamViec int

--#DECLARE#--
--#DECLARE#--
select iID_DuAnID, MAX(iID_PhanBoVon_DonVi_PheDuyet_ID) as iID_PhanBoVon_DonVi_PheDuyet_ID, MAX(sTenDuAn) as sTenDuAn, MAX(sLoaiDuAn) as sLoaiDuAn, MAX(sTenLoaiCongTrinh) as sTenLoaiCongTrinh, MAX(iID_LoaiCongTrinh) as iID_LoaiCongTrinh,MIN(iID_DonViQuanLyID) as iID_DonViQuanLyID, MIN(sTenDonViThucHienDuAn) as sTenDonViThucHienDuAn, 
MAX(fGiaTriPhanBo) as fGiaTriDeNghi,MAX(fGiaTriPhanBo) as fGiaTriPhanBo, MAX(iID_PhanBoVon_DonVi_PheDuyet_ChiTiet_ID) as iID_PhanBoVon_DonVi_PheDuyet_ChiTiet_ID, MAX(iID_Parent) as iID_Parent, MAX(sGhiChu) as sGhiChu from
(
	select ct.iID_DuAnID, ct.iID_PhanBoVon_DonVi_PheDuyet_ID, da.sTenDuAn, 
		case 
			when da.sTrangThaiDuAn = 'KhoiTao' then N'Mở mới'
			when da.sTrangThaiDuAn = 'THUC_HIEN' then N'Chuyển tiếp'
			else da.sTrangThaiDuAn
		end as sLoaiDuAn,
	null as sTenLoaiCongTrinh,
	ct.iID_LoaiCongTrinh,
	dv.iID_Ma as iID_DonViQuanLyID,
	dv.sTen as sTenDonViThucHienDuAn,
	ct.fGiaTriPhanBo as fGiaTriDeNghi,
	ct.fGiaTriPhanBo as fGiaTriPhanBo,
	ct.Id as iID_PhanBoVon_DonVi_PheDuyet_ChiTiet_ID,
	ct.iId_Parent,
	ct.bActive,
	ct.sGhiChu 
	from VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet as ct left join VDT_DA_DuAn as da on ct.iID_DuAnID = da.iID_DuAnID
	LEFT JOIN NS_DonVi dv ON dv.iID_MaDonVi = da.iID_MaDonViThucHienDuAnID
	Where ct.iID_PhanBoVon_DonVi_PheDuyet_ID = @phanBoVonId and ct.iID_LoaiCongTrinh is not null

    union all

	select
	da.iID_DuAnID as iID_DuAnID,
	cast(@phanBoVonId as uniqueidentifier) as iID_PhanBoVon_DonVi_PheDuyet_ID, 
	da.sTenDuAn as sTenDuAn,
	case 
		when da.sTrangThaiDuAn = 'KhoiTao' then N'Mở mới'
		when da.sTrangThaiDuAn = 'THUC_HIEN' then N'Chuyển tiếp'
		else da.sTrangThaiDuAn
	end as sLoaiDuAn,
	lct.sTenLoaiCongTrinh,
	lct.iID_LoaiCongTrinh,
	dv.iID_Ma as iID_DonViQuanLyID,
	dv.sTen as sTenDonViThucHienDuAn,
	null as fGiaTriDeNghi,
	null as fGiaTriPhanBo,
	null as iID_PhanBoVon_DonVi_PheDuyet_ChiTiet_ID,
	null as iID_Parent,
	null as bActive,
	'' as sGhiChu
from
	VDT_DA_DuAn da
	LEFT JOIN NS_DonVi dv ON dv.iID_MaDonVi = da.iID_MaDonViThucHienDuAnID AND dv.iNamLamViec_DonVi = @iNamLamViec
	LEFT JOIN VDT_DA_DuAn_HangMuc dahm ON da.iID_DuAnID = dahm.iID_DuAnID
left join
	VDT_KHV_KeHoach5Nam_ChiTiet kh5nct
on da.iID_DuAnID = kh5nct.iID_DuAnID
left join
	VDT_DM_LoaiCongTrinh lct
on da.iID_LoaiCongTrinhID = lct.iID_LoaiCongTrinh or dahm.iID_LoaiCongTrinhID = lct.iID_LoaiCongTrinh
where
	da.iID_DuAnID in (
		select 
			khvndxct.iID_DuAnID
		from
			VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet khvndxct
		inner join
			VDT_KHV_KeHoachVonNam_DeXuat khvndx
		on khvndxct.iID_KeHoachVonNamDeXuatID = khvndx.iID_KeHoachVonNamDeXuatID
		where 
			khvndx.iID_KeHoachVonNamDeXuatID = @iIdPhanBoVonDeXuat
	)

    union all

	select ct.iID_DuAnID, ct.iID_PhanBoVon_DonVi_PheDuyet_ID,da.sTenDuAn, 
		case 
			when da.sTrangThaiDuAn = 'KhoiTao' then N'Mở mới'
			when da.sTrangThaiDuAn = 'THUC_HIEN' then N'Chuyển tiếp'
			else da.sTrangThaiDuAn
		end as sLoaiDuAn,
	lct.sTenLoaiCongTrinh,
	ct.iID_LoaiCongTrinh,
	dv.iID_Ma as iID_DonViQuanLyID,
	dv.sTen as sTenDonViThucHienDuAn,
	ct.fGiaTriPhanBo as fGiaTriDeNghi, 
	ct.fGiaTriPhanBo, 
	ct.Id as iID_PhanBoVon_DonVi_PheDuyet_ChiTiet_ID,
	ct.iId_Parent,
	ct.bActive,
	ct.sGhiChu 
	from VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet as ct left join VDT_DA_DuAn as da on ct.iID_DuAnID = da.iID_DuAnID
	LEFT JOIN NS_DonVi dv ON dv.iID_MaDonVi = da.iID_MaDonViThucHienDuAnID
	left join VDT_DM_LoaiCongTrinh lct on ct.iID_LoaiCongTrinh = lct.iID_LoaiCongTrinh
	Where ct.iID_PhanBoVon_DonVi_PheDuyet_ID = @phanBoVonId

) as data
Group by iID_DuAnID



