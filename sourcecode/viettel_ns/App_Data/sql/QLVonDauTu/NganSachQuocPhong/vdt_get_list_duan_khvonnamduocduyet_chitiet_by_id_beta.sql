DECLARE @phanBoVonId uniqueidentifier set @phanBoVonId = 'e67029fe-aab9-4583-993d-1c545ce70c67'
DECLARE @iIdPhanBoVonDeXuat uniqueidentifier set @iIdPhanBoVonDeXuat =  'f7ae27df-ac19-4594-8f3a-17e65b1fa84b'--'b3b906dd-ee94-4c1c-9bcb-0824e58b54bd'  --
DECLARE @iNamLamViec int = 2022
Declare @EmptyGuid uniqueidentifier
Set @EmptyGuid = '00000000-0000-0000-0000-000000000000'
--DECLARE @count int

--DECLARE @sTenDuAn nvarchar(max)
--DECLARE @sLoaiDuAn nvarchar(max) = ''
--DECLARE @sTenDonVi nvarchar(max)=''

--select iID_DonViID from VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet
--#DECLARE#--
--#DECLARE#--
SELECT dx.iID_KeHoachVonNamDeXuatID,dx.fThanhToan, ct.iID_DuAnID,ct.iID_DonViID as iID_DonViID, dv.sTen as sTenDonVi INTO #tmpfThanhToan 
FROM VDT_KHV_KeHoachVonNam_DeXuat dx
INNER JOIN VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet ct on ct.iID_KeHoachVonNamDeXuatID = dx.iID_KeHoachVonNamDeXuatID
INNER JOIN NS_DonVi dv on dv.iID_Ma = ct.iID_DonViID
WHERE dx.iID_KeHoachVonNamDeXuatID = @iIdPhanBoVonDeXuat ;

--SET @count = (SELECT count(*) from #tmpfThanhToan);
CREATE TABLE #tmpTrangThaiDuAnMoi(iID_KeHoachVonNamDeXuatID uniqueidentifier, fThanhToan float, iID_DuAnID uniqueidentifier, iID_DonViID uniqueidentifier, sTenDonVi nvarchar(500), sTrangThaiDuAn nVarchar(500) )
	--Truong hop  default chua chon chung tu de xuat tong hop--
	IF((SELECT count(*) from #tmpfThanhToan) > 0)	
		BEGIN
			INSERT INTO  #tmpTrangThaiDuAnMoi(iID_KeHoachVonNamDeXuatID,fThanhToan,iID_DuAnID,iID_DonViID,sTenDonVi,sTrangThaiDuAn)			
					(
						select *, N'Chuyển tiếp' from #tmpfThanhToan tmp
						where tmp.iID_DuAnID in (SELECT iID_DuAnID FROM VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet )
						union all
						select *, N'Mở mới' from #tmpfThanhToan tmp
						where tmp.iID_DuAnID not in (SELECT iID_DuAnID FROM VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet )
					)
		END
	ELSE	
		BEGIN	
			INSERT INTO  #tmpTrangThaiDuAnMoi(iID_KeHoachVonNamDeXuatID,fThanhToan,iID_DuAnID,iID_DonViID,sTenDonVi,sTrangThaiDuAn)
			
				(
					select @iIdPhanBoVonDeXuat, ct.fGiaTriDeNghi ,ct.iID_DuAnID, pd.iID_DonViQuanLyID,dv.sTen , N'Chuyển tiếp' 
					from VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet ct
					inner join VDT_KHV_PhanBoVon_DonVi_PheDuyet pd on pd.Id = ct.iID_PhanBoVon_DonVi_PheDuyet_ID
					inner join VDT_KHV_KeHoachVonNam_DeXuat dx on dx.iID_KeHoachVonNamDeXuatID = pd.iID_VonNamDeXuatID
					inner join NS_DonVi dv on dv.iID_Ma = dx.iID_DonViQuanLyID
					where
						ct.iID_DuAnID in (SELECT iID_DuAnID FROM VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet )
						AND ct.iID_PhanBoVon_DonVi_PheDuyet_ID = @phanBoVonId
					union all
					select  @iIdPhanBoVonDeXuat, ct.fGiaTriDeNghi ,ct.iID_DuAnID, pd.iID_DonViQuanLyID,dv.sTen, N'Mở mới' 
					from VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet ct
					inner join VDT_KHV_PhanBoVon_DonVi_PheDuyet pd on pd.Id = ct.iID_PhanBoVon_DonVi_PheDuyet_ID
					inner join VDT_KHV_KeHoachVonNam_DeXuat dx on dx.iID_KeHoachVonNamDeXuatID = pd.iID_VonNamDeXuatID
					inner join NS_DonVi dv on dv.iID_Ma = dx.iID_DonViQuanLyID
					where 
						ct.iID_DuAnID not in (SELECT iID_DuAnID FROM VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet )
						AND ct.iID_PhanBoVon_DonVi_PheDuyet_ID = @phanBoVonId	
				)						

		END
	
		
select iID_DuAnID, MAX(iID_PhanBoVon_DonVi_PheDuyet_ID) as iID_PhanBoVon_DonVi_PheDuyet_ID, MAX(sTenDuAn) as sTenDuAn, MAX(sLoaiDuAn) as sLoaiDuAn, MAX(sTenLoaiCongTrinh) as sTenLoaiCongTrinh, MAX(iID_LoaiCongTrinh) as iID_LoaiCongTrinh,MIN(iID_DonViQuanLyID) as iID_DonViQuanLyID, MIN(sTenDonViThucHienDuAn) as sTenDonViThucHienDuAn, 
MAX(fGiaTriDeNghi) as fGiaTriDeNghi,MAX(fGiaTriPhanBo) as fGiaTriPhanBo, MAX(iID_PhanBoVon_DonVi_PheDuyet_ChiTiet_ID) as iID_PhanBoVon_DonVi_PheDuyet_ChiTiet_ID, MAX(iID_Parent) as iID_Parent, MAX(sGhiChu) as sGhiChu, MAX(iID_DonViID) as iID_DonViID, MAX(sTenDonVi) as sTenDonVi  from
(
	select DISTINCT ct.iID_DuAnID, ct.iID_PhanBoVon_DonVi_PheDuyet_ID, da.sTenDuAn, 
		--case 
		--	when da.sTrangThaiDuAn = 'KhoiTao' then N'Mở mới'
		--	when da.sTrangThaiDuAn = 'THUC_HIEN' then N'Chuyển tiếp'
		--	else da.sTrangThaiDuAn
		--end as sLoaiDuAn,
	tmpTrangThai.sTrangThaiDuAn as sLoaiDuAn,
	lct.sTenLoaiCongTrinh as sTenLoaiCongTrinh,
	ct.iID_LoaiCongTrinh,
	dv.iID_Ma as iID_DonViQuanLyID,
	dv.sTen as sTenDonViThucHienDuAn,
	ct.fGiaTriDeNghi as fGiaTriDeNghi,
	ct.fGiaTriPhanBo as fGiaTriPhanBo,
	ct.Id as iID_PhanBoVon_DonVi_PheDuyet_ChiTiet_ID,
	ct.iId_Parent,
	ct.bActive,
	ct.sGhiChu ,
	tmpTrangThai.iID_DonViID,
	tmpTrangThai.sTenDonVi
	from VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet as ct 
	INNER JOIN VDT_KHV_PhanBoVon_DonVi_PheDuyet pd on pd.Id = ct.iID_PhanBoVon_DonVi_PheDuyet_ID
	INNER JOin VDT_KHV_KeHoachVonNam_DeXuat dx on dx.iID_KeHoachVonNamDeXuatID = pd.iID_VonNamDeXuatID
	INNER join VDT_DA_DuAn as da on ct.iID_DuAnID = da.iID_DuAnID
	INNER JOIN NS_DonVi dv ON dv.iID_MaDonVi = da.iID_MaDonViThucHienDuAnID
	LEFT JOIN #tmpTrangThaiDuAnMoi as tmpTrangThai on tmpTrangThai.iID_DuAnID = ct.iID_DuAnID
	LEFT join VDT_DM_LoaiCongTrinh lct on ct.iID_LoaiCongTrinh = lct.iID_LoaiCongTrinh
	
	Where ct.iID_PhanBoVon_DonVi_PheDuyet_ID = @phanBoVonId 
	AND ct.iID_LoaiCongTrinh is not null 
	AND dv.iNamLamViec_DonVi = @iNamLamViec

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
	tmp.fThanhToan as fGiaTriPhanBo,
	null as iID_PhanBoVon_DonVi_PheDuyet_ChiTiet_ID,
	null as iID_Parent,
	null as bActive,
	'' as sGhiChu,
	tmp.iID_DonViID,
	tmp.sTenDonVi
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
	ct.sGhiChu,
	tmpTrangThai.iID_DonViID,
	tmpTrangThai.sTenDonVi
	from VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet as ct 
	INNER JOIN VDT_KHV_PhanBoVon_DonVi_PheDuyet pd on pd.Id = ct.iID_PhanBoVon_DonVi_PheDuyet_ID
	INNER JOin VDT_KHV_KeHoachVonNam_DeXuat dx on dx.iID_KeHoachVonNamDeXuatID = pd.iID_VonNamDeXuatID
	LEFT join VDT_DA_DuAn as da on ct.iID_DuAnID = da.iID_DuAnID
	LEFT JOIN NS_DonVi dv ON dv.iID_MaDonVi = da.iID_MaDonViThucHienDuAnID
	LEFT join VDT_DM_LoaiCongTrinh lct on ct.iID_LoaiCongTrinh = lct.iID_LoaiCongTrinh
	LEFT JOIN #tmpTrangThaiDuAnMoi as tmpTrangThai on tmpTrangThai.iID_DuAnID = ct.iID_DuAnID

	Where ct.iID_PhanBoVon_DonVi_PheDuyet_ID = @phanBoVonId 
	AND dx.iID_KeHoachVonNamDeXuatID = @iIdPhanBoVonDeXuat
		AND dv.iNamLamViec_DonVi = @iNamLamViec


) as data
where 
	(ISNULL(@sTenDuAn,'')='' OR sTenDuAn LIKE  CONCAT(N'%',@sTenDuAn,N'%'))
	AND (ISNULL(@sTenDonVi,'')='' OR sTenDonVi LIKE  CONCAT(N'%',@sTenDonVi,N'%'))
	AND (ISNULL(@sLoaiDuAn,'')='' OR sLoaiDuAn LIKE  CONCAT(N'%',@sLoaiDuAn,N'%'))
	AND iID_DuAnID NOT IN (SELECT iID_DuAnID FROM VDT_QT_QuyetToan)

Group by iID_DuAnID
drop table #tmpfThanhToan
drop table #tmpTrangThaiDuAnMoi



