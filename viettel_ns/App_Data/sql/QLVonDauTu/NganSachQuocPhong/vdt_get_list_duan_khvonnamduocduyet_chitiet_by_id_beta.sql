DECLARE @phanBoVonId uniqueidentifier set @phanBoVonId = 'e67029fe-aab9-4583-993d-1c545ce70c67'
DECLARE @iIdPhanBoVonDeXuat uniqueidentifier set @iIdPhanBoVonDeXuat = 'b3b906dd-ee94-4c1c-9bcb-0824e58b54bd'
DECLARE @iNamLamViec int = 2022
Declare @EmptyGuid uniqueidentifier
Set @EmptyGuid = '00000000-0000-0000-0000-000000000000'

--DECLARE @sTenDuAn nvarchar(max)
--DECLARE @sLoaiDuAn nvarchar(max) = ''
--DECLARE @sTenDonViThucHienDuAn nvarchar(max)

--select iID_DonViID from VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet
--#DECLARE#--
--#DECLARE#--
SELECT dx.iID_KeHoachVonNamDeXuatID,dx.fThanhToan, ct.iID_DuAnID,ct.iID_DonViID as iID_DonViID, dv.sTen as sTenDonVi INTO #tmpfThanhToan 
FROM VDT_KHV_KeHoachVonNam_DeXuat dx
INNER JOIN VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet ct on ct.iID_KeHoachVonNamDeXuatID = dx.iID_KeHoachVonNamDeXuatID
INNER JOIN NS_DonVi dv on dv.iID_Ma = ct.iID_DonViID
WHERE dx.iID_KeHoachVonNamDeXuatID = @iIdPhanBoVonDeXuat ;


WITH tmpTrangThaiDuAnMoi(iID_KeHoachVonNamDeXuatID,fThanhToan,iID_DuAnID,iID_DonViID,sTenDonVi,sTrangThaiDuAn)
AS
	(
		select *, N'Chuyển tiếp' from #tmpfThanhToan tmp
		where tmp.iID_DuAnID in (SELECT iID_DuAnID FROM VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet )
		union all
		select *, N'Mở mới' from #tmpfThanhToan tmp
		where tmp.iID_DuAnID not in (SELECT iID_DuAnID FROM VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet )
	)

	select * INTO #tmpTrangThaiDuAnMoi from tmpTrangThaiDuAnMoi

	

select iID_DuAnID, MAX(iID_PhanBoVon_DonVi_PheDuyet_ID) as iID_PhanBoVon_DonVi_PheDuyet_ID, MAX(sTenDuAn) as sTenDuAn, MAX(sLoaiDuAn) as sLoaiDuAn, MAX(sTenLoaiCongTrinh) as sTenLoaiCongTrinh, MAX(iID_LoaiCongTrinh) as iID_LoaiCongTrinh,MIN(iID_DonViQuanLyID) as iID_DonViQuanLyID, MIN(sTenDonViThucHienDuAn) as sTenDonViThucHienDuAn, 
MAX(fGiaTriDeNghi) as fGiaTriDeNghi,MAX(fGiaTriPhanBo) as fGiaTriPhanBo, MAX(iID_PhanBoVon_DonVi_PheDuyet_ChiTiet_ID) as iID_PhanBoVon_DonVi_PheDuyet_ChiTiet_ID, MAX(iID_Parent) as iID_Parent, MAX(sGhiChu) as sGhiChu from
(
	select DISTINCT ct.iID_DuAnID, ct.iID_PhanBoVon_DonVi_PheDuyet_ID, da.sTenDuAn, 
		--case 
		--	when da.sTrangThaiDuAn = 'KhoiTao' then N'Mở mới'
		--	when da.sTrangThaiDuAn = 'THUC_HIEN' then N'Chuyển tiếp'
		--	else da.sTrangThaiDuAn
		--end as sLoaiDuAn,
	tmpTrangThai.sTrangThaiDuAn as sLoaiDuAn,
	null as sTenLoaiCongTrinh,
	ct.iID_LoaiCongTrinh,
	dv.iID_Ma as iID_DonViQuanLyID,
	dv.sTen as sTenDonViThucHienDuAn,
	ct.fGiaTriDeNghi as fGiaTriDeNghi,
	ct.fGiaTriPhanBo as fGiaTriPhanBo,
	ct.Id as iID_PhanBoVon_DonVi_PheDuyet_ChiTiet_ID,
	ct.iId_Parent,
	ct.bActive,
	ct.sGhiChu 
	from VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet as ct 
	INNER JOIN VDT_KHV_PhanBoVon_DonVi_PheDuyet pd on pd.Id = ct.iID_PhanBoVon_DonVi_PheDuyet_ID
	INNER JOin VDT_KHV_KeHoachVonNam_DeXuat dx on dx.iID_KeHoachVonNamDeXuatID = pd.iID_VonNamDeXuatID
	INNER join VDT_DA_DuAn as da on ct.iID_DuAnID = da.iID_DuAnID
	INNER JOIN NS_DonVi dv ON dv.iID_MaDonVi = da.iID_MaDonViThucHienDuAnID
	INNER JOIN #tmpTrangThaiDuAnMoi as tmpTrangThai on tmpTrangThai.iID_DuAnID = ct.iID_DuAnID
	--LEFT JOIN #tmpfThanhToan as tmp on tmp.iID_DuAnID = da.iID_DuAnID
	
	Where ct.iID_PhanBoVon_DonVi_PheDuyet_ID = @phanBoVonId 
	AND ct.iID_LoaiCongTrinh is not null 
	--AND dx.iID_KeHoachVonNamDeXuatID = @iIdPhanBoVonDeXuat

	--and dv.iNamLamViec_DonVi = 2022

    union 

	select DISTINCT
	da.iID_DuAnID as iID_DuAnID,
	cast(@phanBoVonId as uniqueidentifier) as iID_PhanBoVon_DonVi_PheDuyet_ID, 
	da.sTenDuAn as sTenDuAn,
	--case 
	--	when da.sTrangThaiDuAn = 'KhoiTao' then N'Mở mới'
	--	when da.sTrangThaiDuAn = 'THUC_HIEN' then N'Chuyển tiếp'
	--	else da.sTrangThaiDuAn
	--end as sLoaiDuAn,
	tmp.sTrangThaiDuAn as sLoaiDuAn,
	lct.sTenLoaiCongTrinh,
	lct.iID_LoaiCongTrinh,
	dv.iID_Ma as iID_DonViQuanLyID,
	dv.sTen as sTenDonViThucHienDuAn,
	tmp.fThanhToan as fGiaTriDeNghi,
	null as fGiaTriPhanBo,
	null as iID_PhanBoVon_DonVi_PheDuyet_ChiTiet_ID,
	null as iID_Parent,
	null as bActive,
	'' as sGhiChu
from
	VDT_DA_DuAn da
	LEFT JOIN NS_DonVi dv ON dv.iID_MaDonVi = da.iID_MaDonViThucHienDuAnID AND dv.iNamLamViec_DonVi = @iNamLamViec
	LEFT JOIN VDT_DA_DuAn_HangMuc dahm ON da.iID_DuAnID = dahm.iID_DuAnID
	left join VDT_KHV_KeHoach5Nam_ChiTiet kh5nct on da.iID_DuAnID = kh5nct.iID_DuAnID
	left join VDT_DM_LoaiCongTrinh lct on da.iID_LoaiCongTrinhID = lct.iID_LoaiCongTrinh or dahm.iID_LoaiCongTrinhID = lct.iID_LoaiCongTrinh
	RIGHT JOIN #tmpTrangThaiDuAnMoi as tmp on tmp.iID_DuAnID = da.iID_DuAnID
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

    union 

	select DISTINCT ct.iID_DuAnID, ct.iID_PhanBoVon_DonVi_PheDuyet_ID,da.sTenDuAn, 
		--case 
		--	when da.sTrangThaiDuAn = 'KhoiTao' then N'Mở mới'
		--	when da.sTrangThaiDuAn = 'THUC_HIEN' then N'Chuyển tiếp'
		--	else da.sTrangThaiDuAn
		--end as sLoaiDuAn,
	tmpTrangThai.sTrangThaiDuAn as sLoaiDuAn,
	lct.sTenLoaiCongTrinh,
	ct.iID_LoaiCongTrinh,
	dv.iID_Ma as iID_DonViQuanLyID,
	dv.sTen as sTenDonViThucHienDuAn,
	ct.fGiaTriDeNghi as fGiaTriDeNghi, 
	ct.fGiaTriPhanBo, 
	ct.Id as iID_PhanBoVon_DonVi_PheDuyet_ChiTiet_ID,
	ct.iId_Parent,
	ct.bActive,
	ct.sGhiChu 
	from VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet as ct 
	INNER JOIN VDT_KHV_PhanBoVon_DonVi_PheDuyet pd on pd.Id = ct.iID_PhanBoVon_DonVi_PheDuyet_ID
	INNER JOin VDT_KHV_KeHoachVonNam_DeXuat dx on dx.iID_KeHoachVonNamDeXuatID = pd.iID_VonNamDeXuatID
	LEFT join VDT_DA_DuAn as da on ct.iID_DuAnID = da.iID_DuAnID
	LEFT JOIN NS_DonVi dv ON dv.iID_MaDonVi = da.iID_MaDonViThucHienDuAnID
	LEFT join VDT_DM_LoaiCongTrinh lct on ct.iID_LoaiCongTrinh = lct.iID_LoaiCongTrinh
	INNER JOIN #tmpTrangThaiDuAnMoi as tmpTrangThai on tmpTrangThai.iID_DuAnID = ct.iID_DuAnID

	Where ct.iID_PhanBoVon_DonVi_PheDuyet_ID = @phanBoVonId 
	AND dx.iID_KeHoachVonNamDeXuatID = @iIdPhanBoVonDeXuat

) as data
where 
	(ISNULL(@sTenDuAn,'')='' OR sTenDuAn LIKE  CONCAT(N'%',@sTenDuAn,N'%'))
	AND (ISNULL(@sTenDonViThucHienDuAn,'')='' OR sTenDonViThucHienDuAn LIKE  CONCAT(N'%',@sTenDonViThucHienDuAn,N'%'))
	AND (ISNULL(@sLoaiDuAn,'')='' OR sLoaiDuAn LIKE  CONCAT(N'%',@sLoaiDuAn,N'%'))

Group by iID_DuAnID

drop table #tmpfThanhToan
drop table #tmpTrangThaiDuAnMoi
