--DECLARE @phanBoVonId uniqueidentifier set @phanBoVonId = '4169b088-fc21-4e23-8d34-36ae836c59c2'
--DECLARE @iIdPhanBoVonDeXuat uniqueidentifier set @iIdPhanBoVonDeXuat =  '83b0df9b-aeed-4ca4-8007-0068f28f4235'--'b3b906dd-ee94-4c1c-9bcb-0824e58b54bd'  --
--DECLARE @iNamLamViec int = 2022
--Declare @EmptyGuid uniqueidentifier
--Set @EmptyGuid = '00000000-0000-0000-0000-000000000000'
----DECLARE @count int

--DECLARE @sTenDuAn nvarchar(max) = ''
--DECLARE @sLoaiDuAn nvarchar(max) = ''
--DECLARE @sTenDonVi nvarchar(max)=''

----select iID_DonViID from VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet
----#DECLARE#--
----#DECLARE#--
--SELECT dx.iID_KeHoachVonNamDeXuatID,ct.fThanhToan, ct.iID_DuAnID,ct.iID_DonViID as iID_DonViID, dv.sTen as sTenDonVi INTO #tmpfThanhToan 
--FROM VDT_KHV_KeHoachVonNam_DeXuat dx
--INNER JOIN VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet ct on ct.iID_KeHoachVonNamDeXuatID = dx.iID_KeHoachVonNamDeXuatID
--INNER JOIN NS_DonVi dv on dv.iID_Ma = ct.iID_DonViID
--WHERE dx.iID_KeHoachVonNamDeXuatID = @iIdPhanBoVonDeXuat ;

----SET @count = (SELECT count(*) from #tmpfThanhToan);
--CREATE TABLE #tmpTrangThaiDuAnMoi(iID_KeHoachVonNamDeXuatID uniqueidentifier, fThanhToan float, iID_DuAnID uniqueidentifier, iID_DonViID uniqueidentifier, sTenDonVi nvarchar(500), sTrangThaiDuAn nVarchar(500) )
--	--Truong hop  default chua chon chung tu de xuat tong hop--
--	IF((SELECT count(*) from #tmpfThanhToan) > 0)	
--		BEGIN
--			INSERT INTO  #tmpTrangThaiDuAnMoi(iID_KeHoachVonNamDeXuatID,fThanhToan,iID_DuAnID,iID_DonViID,sTenDonVi,sTrangThaiDuAn)			
--					(
--						select *, N'Chuyển tiếp' from #tmpfThanhToan tmp
--						where tmp.iID_DuAnID in (SELECT iID_DuAnID FROM VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet )
--						union all
--						select *, N'Mở mới' from #tmpfThanhToan tmp
--						where tmp.iID_DuAnID not in (SELECT iID_DuAnID FROM VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet )
--						UNION 
--						SELECT pbvdd.iID_VonNamDeXuatID ,khvnct.fThanhToan,ct.iID_DuAnID,khvnct.iID_DonViID,dv.sTen , N'Chuyển tiếp' FROM VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet ct
--						INNER JOIN VDT_KHV_PhanBoVon_DonVi_PheDuyet pbvdd on pbvdd.Id = ct.iID_PhanBoVon_DonVi_PheDuyet_ID
--						INNER JOIN VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet khvnct on khvnct.iID_KeHoachVonNamDeXuatID = pbvdd.iID_VonNamDeXuatID
--						INNER JOIN NS_DonVi dv on dv.iID_Ma = khvnct.iID_DonViID
--						WHERE
--						ct.iID_PhanBoVon_DonVi_PheDuyet_ID = @phanBoVonId
--						AND ct.iID_DuAnID in (SELECT iID_DuAnID FROM VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet )
--						UNION 
--						SELECT pbvdd.iID_VonNamDeXuatID ,khvnct.fThanhToan,ct.iID_DuAnID,khvnct.iID_DonViID ,dv.sTen, N'Mở mới' FROM VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet ct
--						INNER JOIN VDT_KHV_PhanBoVon_DonVi_PheDuyet pbvdd on pbvdd.Id = ct.iID_PhanBoVon_DonVi_PheDuyet_ID
--						INNER JOIN VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet khvnct on khvnct.iID_KeHoachVonNamDeXuatID = pbvdd.iID_VonNamDeXuatID
--						INNER JOIN NS_DonVi dv on dv.iID_Ma = khvnct.iID_DonViID
--						WHERE
--						ct.iID_PhanBoVon_DonVi_PheDuyet_ID = @phanBoVonId
--						AND ct.iID_DuAnID NOT IN (SELECT iID_DuAnID FROM VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet )
--					)
--		END
--	ELSE	
--		BEGIN	
--			INSERT INTO  #tmpTrangThaiDuAnMoi(iID_KeHoachVonNamDeXuatID,fThanhToan,iID_DuAnID,iID_DonViID,sTenDonVi,sTrangThaiDuAn)
			
--				(
--					select @iIdPhanBoVonDeXuat, ct.fGiaTriDeNghi ,ct.iID_DuAnID, pd.iID_DonViQuanLyID,dv.sTen , N'Chuyển tiếp' 
--					from VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet ct
--					inner join VDT_KHV_PhanBoVon_DonVi_PheDuyet pd on pd.Id = ct.iID_PhanBoVon_DonVi_PheDuyet_ID
--					inner join VDT_KHV_KeHoachVonNam_DeXuat dx on dx.iID_KeHoachVonNamDeXuatID = pd.iID_VonNamDeXuatID
--					inner join NS_DonVi dv on dv.iID_Ma = dx.iID_DonViQuanLyID
--					where
--						ct.iID_DuAnID in (SELECT iID_DuAnID FROM VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet )
--						AND ct.iID_PhanBoVon_DonVi_PheDuyet_ID = @phanBoVonId
--					union all
--					select  @iIdPhanBoVonDeXuat, ct.fGiaTriDeNghi ,ct.iID_DuAnID, pd.iID_DonViQuanLyID,dv.sTen, N'Mở mới' 
--					from VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet ct
--					inner join VDT_KHV_PhanBoVon_DonVi_PheDuyet pd on pd.Id = ct.iID_PhanBoVon_DonVi_PheDuyet_ID
--					inner join VDT_KHV_KeHoachVonNam_DeXuat dx on dx.iID_KeHoachVonNamDeXuatID = pd.iID_VonNamDeXuatID
--					inner join NS_DonVi dv on dv.iID_Ma = dx.iID_DonViQuanLyID
--					where 
--						ct.iID_DuAnID not in (SELECT iID_DuAnID FROM VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet )
--						AND ct.iID_PhanBoVon_DonVi_PheDuyet_ID = @phanBoVonId	
--				)						

--		END
----Tinh Gia Tri Dieu Chinh --
--	BEGIN 
--	DECLARE @iID_Parent uniqueidentifier = (SELECT iID_ParentId FROM VDT_KHV_PhanBoVon_DonVi_PheDuyet WHERE Id = @phanBoVonId)
--	CREATE TABLE #tmmFDieuChinh(iID_DuAnID uniqueidentifier,fGiaTriPhanBoDC float )
--	IF(@iID_Parent IS NOT NULL)
--	INSERT INTO #tmmFDieuChinh(iID_DuAnID,fGiaTriPhanBoDC)(
--		SELECT iID_DuAnID,fGiaTriPhanBo   FROM VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet
--		Where iID_PhanBoVon_DonVi_PheDuyet_ID = @iID_Parent)
--	END	
--IF((SELECT COUNT(*) FROM #tmmFDieuChinh) > 0 )
--BEGIN
--	select iID_DuAnID, MAX(iID_PhanBoVon_DonVi_PheDuyet_ID) as iID_PhanBoVon_DonVi_PheDuyet_ID, MAX(sTenDuAn) as sTenDuAn, MAX(sLoaiDuAn) as sLoaiDuAn, MAX(sTenLoaiCongTrinh) as sTenLoaiCongTrinh, MAX(iID_LoaiCongTrinh) as iID_LoaiCongTrinh,MIN(iID_DonViQuanLyID) as iID_DonViQuanLyID, MIN(sTenDonViThucHienDuAn) as sTenDonViThucHienDuAn, 
--	MAX(fGiaTriDeNghi) as fGiaTriDeNghi,MAX(fGiaTriPhanBo) as fGiaTriPhanBo, MAX(iID_PhanBoVon_DonVi_PheDuyet_ChiTiet_ID) as iID_PhanBoVon_DonVi_PheDuyet_ChiTiet_ID, MAX(iID_Parent) as iID_Parent, MAX(sGhiChu) as sGhiChu, MAX(iID_DonViID) as iID_DonViID, MAX(sTenDonVi) as sTenDonVi , MAX(fGiaTriPhanBoDC) as fGiaTriPhanBoDC from
--	(
--		select DISTINCT ct.iID_DuAnID, ct.iID_PhanBoVon_DonVi_PheDuyet_ID, da.sTenDuAn, 
--		tmpTrangThai.sTrangThaiDuAn as sLoaiDuAn,
--		lct.sTenLoaiCongTrinh as sTenLoaiCongTrinh,
--		ct.iID_LoaiCongTrinh,
--		dv.iID_Ma as iID_DonViQuanLyID,
--		dv.sTen as sTenDonViThucHienDuAn,
--		ct.fGiaTriDeNghi as fGiaTriDeNghi,
--		fDieuChinh.fGiaTriPhanBoDC as fGiaTriPhanBo,
--		ct.Id as iID_PhanBoVon_DonVi_PheDuyet_ChiTiet_ID,
--		@iID_Parent as iId_Parent,
--		ct.bActive,
--		ct.sGhiChu ,
--		tmpTrangThai.iID_DonViID,
--		tmpTrangThai.sTenDonVi,
--		ct.fGiaTriPhanBo as fGiaTriPhanBoDC


--		from VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet as ct 
--		INNER JOIN VDT_KHV_PhanBoVon_DonVi_PheDuyet pd on pd.Id = ct.iID_PhanBoVon_DonVi_PheDuyet_ID
--		INNER JOin VDT_KHV_KeHoachVonNam_DeXuat dx on dx.iID_KeHoachVonNamDeXuatID = pd.iID_VonNamDeXuatID
--		INNER join VDT_DA_DuAn as da on ct.iID_DuAnID = da.iID_DuAnID
--		INNER JOIN NS_DonVi dv ON dv.iID_Ma = da.iID_DonViQuanLyID
--		LEFT JOIN #tmpTrangThaiDuAnMoi as tmpTrangThai on tmpTrangThai.iID_DuAnID = ct.iID_DuAnID
--		LEFT join VDT_DM_LoaiCongTrinh lct on ct.iID_LoaiCongTrinh = lct.iID_LoaiCongTrinh
--		LEFT JOIN #tmmFDieuChinh fDieuChinh on fDieuChinh.iID_DuAnID = ct.iID_DuAnID
	
--		Where ct.iID_PhanBoVon_DonVi_PheDuyet_ID = @phanBoVonId 
--		AND ct.iID_LoaiCongTrinh is not null 
--		AND dv.iNamLamViec_DonVi = @iNamLamViec
--		union 
--		select DISTINCT
--		da.iID_DuAnID as iID_DuAnID,
--		cast(@phanBoVonId as uniqueidentifier) as iID_PhanBoVon_DonVi_PheDuyet_ID, 
--		da.sTenDuAn as sTenDuAn,
--		--case 
--		--	when da.sTrangThaiDuAn = 'KhoiTao' then N'Mở mới'
--		--	when da.sTrangThaiDuAn = 'THUC_HIEN' then N'Chuyển tiếp'
--		--	else da.sTrangThaiDuAn
--		--end as sLoaiDuAn,
--		tmp.sTrangThaiDuAn as sLoaiDuAn,
--		lct.sTenLoaiCongTrinh,
--		lct.iID_LoaiCongTrinh,
--		dv.iID_Ma as iID_DonViQuanLyID,
--		dv.sTen as sTenDonViThucHienDuAn,
--		tmp.fThanhToan as fGiaTriDeNghi,
--		fDieuChinh.fGiaTriPhanBoDC as fGiaTriPhanBo,
--		null as iID_PhanBoVon_DonVi_PheDuyet_ChiTiet_ID,
--		@iID_Parent as iId_Parent,
--		null as bActive,
--		'' as sGhiChu,
--		tmp.iID_DonViID,
--		tmp.sTenDonVi,
--		fDieuChinh.fGiaTriPhanBoDC as fGiaTriPhanBoDC
--	from
--		VDT_DA_DuAn da
--		LEFT JOIN NS_DonVi dv ON dv.iID_Ma = da.iID_DonViQuanLyID AND dv.iNamLamViec_DonVi = @iNamLamViec
--		LEFT JOIN VDT_DA_DuAn_HangMuc dahm ON da.iID_DuAnID = dahm.iID_DuAnID
--		left join VDT_KHV_KeHoach5Nam_ChiTiet kh5nct on da.iID_DuAnID = kh5nct.iID_DuAnID
--		left join VDT_DM_LoaiCongTrinh lct on da.iID_LoaiCongTrinhID = lct.iID_LoaiCongTrinh or dahm.iID_LoaiCongTrinhID = lct.iID_LoaiCongTrinh
--		RIGHT JOIN #tmpTrangThaiDuAnMoi as tmp on tmp.iID_DuAnID = da.iID_DuAnID
--		LEFT JOIN #tmmFDieuChinh fDieuChinh on fDieuChinh.iID_DuAnID = tmp.iID_DuAnID
--	where
--		da.iID_DuAnID in (
--			select 
--				khvndxct.iID_DuAnID
--			from
--				VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet khvndxct
--			inner join
--				VDT_KHV_KeHoachVonNam_DeXuat khvndx
--			on khvndxct.iID_KeHoachVonNamDeXuatID = khvndx.iID_KeHoachVonNamDeXuatID
--			where 
--				khvndx.iID_KeHoachVonNamDeXuatID = @iIdPhanBoVonDeXuat
--		)

--		union 

--		select DISTINCT ct.iID_DuAnID, ct.iID_PhanBoVon_DonVi_PheDuyet_ID,da.sTenDuAn, 

--		tmpTrangThai.sTrangThaiDuAn as sLoaiDuAn,
--		lct.sTenLoaiCongTrinh,
--		ct.iID_LoaiCongTrinh,
--		dv.iID_Ma as iID_DonViQuanLyID,
--		dv.sTen as sTenDonViThucHienDuAn,
--		ct.fGiaTriDeNghi as fGiaTriDeNghi, 
--		fDieuChinh.fGiaTriPhanBoDC as fGiaTriPhanBo,
--		ct.Id as iID_PhanBoVon_DonVi_PheDuyet_ChiTiet_ID,
--		@iID_Parent as iId_Parent,
--		ct.bActive,
--		ct.sGhiChu,
--		tmpTrangThai.iID_DonViID,
--		tmpTrangThai.sTenDonVi,
--		ct.fGiaTriPhanBo as fGiaTriPhanBoDC

--		from VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet as ct 
--		INNER JOIN VDT_KHV_PhanBoVon_DonVi_PheDuyet pd on pd.Id = ct.iID_PhanBoVon_DonVi_PheDuyet_ID
--		INNER JOin VDT_KHV_KeHoachVonNam_DeXuat dx on dx.iID_KeHoachVonNamDeXuatID = pd.iID_VonNamDeXuatID
--		LEFT join VDT_DA_DuAn as da on ct.iID_DuAnID = da.iID_DuAnID
--		LEFT JOIN NS_DonVi dv ON dv.iID_Ma = da.iID_DonViQuanLyID
--		LEFT join VDT_DM_LoaiCongTrinh lct on ct.iID_LoaiCongTrinh = lct.iID_LoaiCongTrinh
--		LEFT JOIN #tmpTrangThaiDuAnMoi as tmpTrangThai on tmpTrangThai.iID_DuAnID = ct.iID_DuAnID
--		LEFT JOIN #tmmFDieuChinh fDieuChinh on fDieuChinh.iID_DuAnID = ct.iID_DuAnID
 

--		Where ct.iID_PhanBoVon_DonVi_PheDuyet_ID = @phanBoVonId 
--		AND dx.iID_KeHoachVonNamDeXuatID = @iIdPhanBoVonDeXuat
--			AND dv.iNamLamViec_DonVi = @iNamLamViec


--	) as data
--	where 
--		(ISNULL(@sTenDuAn,'')='' OR sTenDuAn LIKE  CONCAT(N'%',@sTenDuAn,N'%'))
--		AND (ISNULL(@sTenDonVi,'')='' OR sTenDonVi LIKE  CONCAT(N'%',@sTenDonVi,N'%'))
--		AND (ISNULL(@sLoaiDuAn,'')='' OR sLoaiDuAn LIKE  CONCAT(N'%',@sLoaiDuAn,N'%'))
--		AND iID_DuAnID NOT IN (SELECT iID_DuAnID FROM VDT_QT_QuyetToan)

--	Group by iID_DuAnID

--END
--ELSE
--BEGIN
--	select iID_DuAnID, MAX(iID_PhanBoVon_DonVi_PheDuyet_ID) as iID_PhanBoVon_DonVi_PheDuyet_ID, MAX(sTenDuAn) as sTenDuAn, MAX(sLoaiDuAn) as sLoaiDuAn, MAX(sTenLoaiCongTrinh) as sTenLoaiCongTrinh, MAX(iID_LoaiCongTrinh) as iID_LoaiCongTrinh,MIN(iID_DonViQuanLyID) as iID_DonViQuanLyID, MIN(sTenDonViThucHienDuAn) as sTenDonViThucHienDuAn, 
--	MAX(fGiaTriDeNghi) as fGiaTriDeNghi,MAX(fGiaTriPhanBo) as fGiaTriPhanBo, MAX(iID_PhanBoVon_DonVi_PheDuyet_ChiTiet_ID) as iID_PhanBoVon_DonVi_PheDuyet_ChiTiet_ID, MAX(iID_Parent) as iID_Parent, MAX(sGhiChu) as sGhiChu, MAX(iID_DonViID) as iID_DonViID, MAX(sTenDonVi) as sTenDonVi, MAX(fGiaTriPhanBoDC) as fGiaTriPhanBoDC  from
--	(
--		select DISTINCT ct.iID_DuAnID, ct.iID_PhanBoVon_DonVi_PheDuyet_ID, da.sTenDuAn, 
--		tmpTrangThai.sTrangThaiDuAn as sLoaiDuAn,
--		lct.sTenLoaiCongTrinh as sTenLoaiCongTrinh,
--		ct.iID_LoaiCongTrinh,
--		dv.iID_Ma as iID_DonViQuanLyID,
--		dv.sTen as sTenDonViThucHienDuAn,
--		ct.fGiaTriDeNghi as fGiaTriDeNghi,
--		ct.fGiaTriPhanBo as fGiaTriPhanBo,
--		ct.Id as iID_PhanBoVon_DonVi_PheDuyet_ChiTiet_ID,
--		@iID_Parent as iId_Parent,
--		ct.bActive,
--		ct.sGhiChu ,
--		tmpTrangThai.iID_DonViID,
--		tmpTrangThai.sTenDonVi,
--		ct.fGiaTriPhanBo as  fGiaTriPhanBoDC

--		from VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet as ct 
--		INNER JOIN VDT_KHV_PhanBoVon_DonVi_PheDuyet pd on pd.Id = ct.iID_PhanBoVon_DonVi_PheDuyet_ID
--		INNER JOin VDT_KHV_KeHoachVonNam_DeXuat dx on dx.iID_KeHoachVonNamDeXuatID = pd.iID_VonNamDeXuatID
--		INNER join VDT_DA_DuAn as da on ct.iID_DuAnID = da.iID_DuAnID
--		INNER JOIN NS_DonVi dv ON dv.iID_Ma = da.iID_DonViQuanLyID
--		LEFT JOIN #tmpTrangThaiDuAnMoi as tmpTrangThai on tmpTrangThai.iID_DuAnID = ct.iID_DuAnID
--		LEFT join VDT_DM_LoaiCongTrinh lct on ct.iID_LoaiCongTrinh = lct.iID_LoaiCongTrinh
	
--		Where ct.iID_PhanBoVon_DonVi_PheDuyet_ID = @phanBoVonId 
--		AND ct.iID_LoaiCongTrinh is not null 
--		AND dv.iNamLamViec_DonVi = @iNamLamViec
--		union 
--		select DISTINCT
--		da.iID_DuAnID as iID_DuAnID,
--		cast(@phanBoVonId as uniqueidentifier) as iID_PhanBoVon_DonVi_PheDuyet_ID, 
--		da.sTenDuAn as sTenDuAn,
--		--case 
--		--	when da.sTrangThaiDuAn = 'KhoiTao' then N'Mở mới'
--		--	when da.sTrangThaiDuAn = 'THUC_HIEN' then N'Chuyển tiếp'
--		--	else da.sTrangThaiDuAn
--		--end as sLoaiDuAn,
--		tmp.sTrangThaiDuAn as sLoaiDuAn,
--		lct.sTenLoaiCongTrinh,
--		lct.iID_LoaiCongTrinh,
--		dv.iID_Ma as iID_DonViQuanLyID,
--		dv.sTen as sTenDonViThucHienDuAn,
--		tmp.fThanhToan as fGiaTriDeNghi,
--		tmp.fThanhToan as fGiaTriPhanBo,
--		null as iID_PhanBoVon_DonVi_PheDuyet_ChiTiet_ID,
--		@iID_Parent as iId_Parent,
--		null as bActive,
--		'' as sGhiChu,
--		tmp.iID_DonViID,
--		tmp.sTenDonVi,
--		tmp.fThanhToan as  fGiaTriPhanBoDC


--	from
--		VDT_DA_DuAn da
--		LEFT JOIN NS_DonVi dv ON dv.iID_Ma = da.iID_DonViQuanLyID AND dv.iNamLamViec_DonVi = @iNamLamViec
--		LEFT JOIN VDT_DA_DuAn_HangMuc dahm ON da.iID_DuAnID = dahm.iID_DuAnID
--		left join VDT_KHV_KeHoach5Nam_ChiTiet kh5nct on da.iID_DuAnID = kh5nct.iID_DuAnID
--		left join VDT_DM_LoaiCongTrinh lct on da.iID_LoaiCongTrinhID = lct.iID_LoaiCongTrinh or dahm.iID_LoaiCongTrinhID = lct.iID_LoaiCongTrinh
--		RIGHT JOIN #tmpTrangThaiDuAnMoi as tmp on tmp.iID_DuAnID = da.iID_DuAnID
--	where
--		da.iID_DuAnID in (
--			select 
--				khvndxct.iID_DuAnID
--			from
--				VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet khvndxct
--			inner join
--				VDT_KHV_KeHoachVonNam_DeXuat khvndx
--			on khvndxct.iID_KeHoachVonNamDeXuatID = khvndx.iID_KeHoachVonNamDeXuatID
--			where 
--				khvndx.iID_KeHoachVonNamDeXuatID = @iIdPhanBoVonDeXuat
--		)

--		union 

--		select DISTINCT ct.iID_DuAnID, ct.iID_PhanBoVon_DonVi_PheDuyet_ID,da.sTenDuAn, 

--		tmpTrangThai.sTrangThaiDuAn as sLoaiDuAn,
--		lct.sTenLoaiCongTrinh,
--		ct.iID_LoaiCongTrinh,
--		dv.iID_Ma as iID_DonViQuanLyID,
--		dv.sTen as sTenDonViThucHienDuAn,
--		ct.fGiaTriDeNghi as fGiaTriDeNghi, 
--		ct.fGiaTriPhanBo, 
--		ct.Id as iID_PhanBoVon_DonVi_PheDuyet_ChiTiet_ID,
--		@iID_Parent as iId_Parent,
--		ct.bActive,
--		ct.sGhiChu,
--		tmpTrangThai.iID_DonViID,
--		tmpTrangThai.sTenDonVi,
--		ct.fGiaTriPhanBo as  fGiaTriPhanBoDC

--		from VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet as ct 
--		INNER JOIN VDT_KHV_PhanBoVon_DonVi_PheDuyet pd on pd.Id = ct.iID_PhanBoVon_DonVi_PheDuyet_ID
--		INNER JOin VDT_KHV_KeHoachVonNam_DeXuat dx on dx.iID_KeHoachVonNamDeXuatID = pd.iID_VonNamDeXuatID
--		LEFT join VDT_DA_DuAn as da on ct.iID_DuAnID = da.iID_DuAnID
--		LEFT JOIN NS_DonVi dv ON dv.iID_Ma = da.iID_DonViQuanLyID
--		LEFT join VDT_DM_LoaiCongTrinh lct on ct.iID_LoaiCongTrinh = lct.iID_LoaiCongTrinh
--		LEFT JOIN #tmpTrangThaiDuAnMoi as tmpTrangThai on tmpTrangThai.iID_DuAnID = ct.iID_DuAnID

--		Where ct.iID_PhanBoVon_DonVi_PheDuyet_ID = @phanBoVonId 
--		AND dx.iID_KeHoachVonNamDeXuatID = @iIdPhanBoVonDeXuat
--			AND dv.iNamLamViec_DonVi = @iNamLamViec


--	) as data
--	where 
--		(ISNULL(@sTenDuAn,'')='' OR sTenDuAn LIKE  CONCAT(N'%',@sTenDuAn,N'%'))
--		AND (ISNULL(@sTenDonVi,'')='' OR sTenDonVi LIKE  CONCAT(N'%',@sTenDonVi,N'%'))
--		AND (ISNULL(@sLoaiDuAn,'')='' OR sLoaiDuAn LIKE  CONCAT(N'%',@sLoaiDuAn,N'%'))
--		AND iID_DuAnID NOT IN (SELECT iID_DuAnID FROM VDT_QT_QuyetToan)

--	Group by iID_DuAnID

--END

--	drop table #tmpfThanhToan
--	drop table #tmpTrangThaiDuAnMoi
--	drop table #tmmFDieuChinh

-- viet lai ban moi--


DECLARE @phanBoVonId uniqueidentifier set @phanBoVonId = '0ad17969-6fd7-4b44-a3ea-49f875cbb697'
DECLARE @iIdPhanBoVonDeXuat uniqueidentifier set @iIdPhanBoVonDeXuat =  '00000000-0000-0000-0000-000000000000'--'b3b906dd-ee94-4c1c-9bcb-0824e58b54bd'  --
DECLARE @iNamLamViec int = 2022
Declare @EmptyGuid uniqueidentifier
Set @EmptyGuid = '00000000-0000-0000-0000-000000000000'
--DECLARE @count int

DECLARE @sTenDuAn nvarchar(max) = ''
DECLARE @sLoaiDuAn nvarchar(max) = ''
DECLARE @sTenDonVi nvarchar(max)=''
--select iID_DonViID from VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet
--#DECLARE#--
--#DECLARE#--

IF(@iIdPhanBoVonDeXuat = '00000000-0000-0000-0000-000000000000' )
BEGIN
	IF((SELECT COUNT( iID_VonNamDeXuatID) FROM VDT_KHV_PhanBoVon_DonVi_PheDuyet where Id = @phanBoVonId) > 0)
	SELECT @iIdPhanBoVonDeXuat = iID_VonNamDeXuatID FROM VDT_KHV_PhanBoVon_DonVi_PheDuyet where Id = @phanBoVonId
END
SELECT dx.iID_KeHoachVonNamDeXuatID,ct.fThanhToan, ct.iID_DuAnID,ct.iID_DonViID as iID_DonViID, dv.sTen as sTenDonVi,hm.iID_LoaiCongTrinhID as iID_LoaiCongTrinhID INTO #tmpfThanhToan 
FROM VDT_KHV_KeHoachVonNam_DeXuat dx
INNER JOIN VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet ct on ct.iID_KeHoachVonNamDeXuatID = dx.iID_KeHoachVonNamDeXuatID
INNER JOIN NS_DonVi dv on dv.iID_Ma = ct.iID_DonViID
LEFT JOIN VDT_DA_DuAn_HangMuc hm on hm.iID_DuAnID = ct.iID_DuAnID and ct.iID_LoaiCongTrinh = hm.iID_LoaiCongTrinhID and hm.iID_NguonVonID = dx.iID_NguonVonID
WHERE dx.iID_KeHoachVonNamDeXuatID = @iIdPhanBoVonDeXuat ;

DECLARE @idNguonVon int;
SET @idNguonVon = (SELECT iID_NguonVonID from VDT_KHV_KeHoachVonNam_DeXuat where iID_KeHoachVonNamDeXuatID = @iIdPhanBoVonDeXuat)
--drop table #tmpfThanhToan
--SET @count = (SELECT count(*) from #tmpfThanhToan);
--CREATE TABLE #tmpTrangThaiDuAnMoi(iID_KeHoachVonNamDeXuatID uniqueidentifier, fThanhToan float, iID_DuAnID uniqueidentifier, iID_DonViID uniqueidentifier, sTenDonVi nvarchar(500), iID_LoaiCongTrinhID uniqueidentifier, sTrangThaiDuAn nVarchar(500) )
DECLARE @tmpTrangThaiDuAnMoi AS TABLE(iID_KeHoachVonNamDeXuatID uniqueidentifier, fThanhToan float, iID_DuAnID uniqueidentifier, iID_DonViID uniqueidentifier, sTenDonVi nvarchar(500), iID_LoaiCongTrinhID uniqueidentifier, sTrangThaiDuAn nVarchar(500) )
	--Truong hop  default chua chon chung tu de xuat tong hop--
	IF((SELECT count(*) from #tmpfThanhToan) > 0)	
		BEGIN
			INSERT INTO  @tmpTrangThaiDuAnMoi(iID_KeHoachVonNamDeXuatID, fThanhToan, iID_DuAnID, iID_DonViID, sTenDonVi, iID_LoaiCongTrinhID,sTrangThaiDuAn)			
					(
						select tmp.*, N'Chuyển tiếp' from #tmpfThanhToan tmp
						where tmp.iID_DuAnID in (SELECT iID_DuAnID FROM VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet )
						
						union all
						select tmp.*, N'Mở mới' from #tmpfThanhToan tmp
						where tmp.iID_DuAnID not in (SELECT iID_DuAnID FROM VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet )
						
						UNION 
						SELECT pbvdd.iID_VonNamDeXuatID ,khvnct.fThanhToan,ct.iID_DuAnID,khvnct.iID_DonViID,dv.sTen ,hm.iID_LoaiCongTrinhID, N'Chuyển tiếp' FROM VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet ct
						INNER JOIN VDT_KHV_PhanBoVon_DonVi_PheDuyet pbvdd on pbvdd.Id = ct.iID_PhanBoVon_DonVi_PheDuyet_ID
						INNER JOIN VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet khvnct on khvnct.iID_KeHoachVonNamDeXuatID = pbvdd.iID_VonNamDeXuatID
						INNER JOIN NS_DonVi dv on dv.iID_Ma = khvnct.iID_DonViID
						LEFT JOIN  VDT_DA_DuAn_HangMuc hm on hm.iID_DuAnID = ct.iID_DuAnID AND hm.iID_LoaiCongTrinhID = ct.iID_LoaiCongTrinh AND hm.iID_DuAn_HangMucID = ct.iID_DuAn_HangMucID and hm.iID_NguonVonID = @idNguonVon
						WHERE
						ct.iID_PhanBoVon_DonVi_PheDuyet_ID = @phanBoVonId
						AND ct.iID_DuAnID in (SELECT iID_DuAnID FROM VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet )
						AND ct.iID_DuAnID  NOT IN ( SELECT iID_DuAnID FROM @tmpTrangThaiDuAnMoi)
						
						UNION 
						SELECT pbvdd.iID_VonNamDeXuatID ,khvnct.fThanhToan,ct.iID_DuAnID,khvnct.iID_DonViID ,dv.sTen, hm.iID_LoaiCongTrinhID ,N'Mở mới' FROM VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet ct
						INNER JOIN VDT_KHV_PhanBoVon_DonVi_PheDuyet pbvdd on pbvdd.Id = ct.iID_PhanBoVon_DonVi_PheDuyet_ID
						INNER JOIN VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet khvnct on khvnct.iID_KeHoachVonNamDeXuatID = pbvdd.iID_VonNamDeXuatID
						INNER JOIN NS_DonVi dv on dv.iID_Ma = khvnct.iID_DonViID
						LEFT JOIN  VDT_DA_DuAn_HangMuc hm on hm.iID_DuAnID = ct.iID_DuAnID AND hm.iID_LoaiCongTrinhID = ct.iID_LoaiCongTrinh AND hm.iID_DuAn_HangMucID = ct.iID_DuAn_HangMucID and hm.iID_NguonVonID = @idNguonVon
						WHERE
						ct.iID_PhanBoVon_DonVi_PheDuyet_ID = @phanBoVonId
						AND ct.iID_DuAnID NOT IN (SELECT iID_DuAnID FROM VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet )
						AND ct.iID_DuAnID  NOT IN ( SELECT iID_DuAnID FROM @tmpTrangThaiDuAnMoi)

					)
		END
	ELSE	
		BEGIN	
			INSERT INTO  @tmpTrangThaiDuAnMoi(iID_KeHoachVonNamDeXuatID,fThanhToan,iID_DuAnID,iID_DonViID,sTenDonVi,iID_LoaiCongTrinhID,sTrangThaiDuAn)
			
				(
					select @iIdPhanBoVonDeXuat, ct.fGiaTriDeNghi ,ct.iID_DuAnID, pd.iID_DonViQuanLyID,dv.sTen,hm.iID_LoaiCongTrinhID , N'Chuyển tiếp' 
					from VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet ct
					inner join VDT_KHV_PhanBoVon_DonVi_PheDuyet pd on pd.Id = ct.iID_PhanBoVon_DonVi_PheDuyet_ID
					inner join VDT_KHV_KeHoachVonNam_DeXuat dx on dx.iID_KeHoachVonNamDeXuatID = pd.iID_VonNamDeXuatID
					inner join NS_DonVi dv on dv.iID_Ma = dx.iID_DonViQuanLyID
					LEFT JOIN  VDT_DA_DuAn_HangMuc hm on hm.iID_DuAnID = ct.iID_DuAnID and dx.iID_NguonVonID = hm.iID_NguonVonID
					where
						ct.iID_DuAnID in (SELECT iID_DuAnID FROM VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet )
						AND ct.iID_PhanBoVon_DonVi_PheDuyet_ID = @phanBoVonId
					union all
					select  @iIdPhanBoVonDeXuat, ct.fGiaTriDeNghi ,ct.iID_DuAnID, pd.iID_DonViQuanLyID,dv.sTen,hm.iID_LoaiCongTrinhID, N'Mở mới' 
					from VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet ct
					inner join VDT_KHV_PhanBoVon_DonVi_PheDuyet pd on pd.Id = ct.iID_PhanBoVon_DonVi_PheDuyet_ID
					inner join VDT_KHV_KeHoachVonNam_DeXuat dx on dx.iID_KeHoachVonNamDeXuatID = pd.iID_VonNamDeXuatID
					inner join NS_DonVi dv on dv.iID_Ma = dx.iID_DonViQuanLyID
					LEFT JOIN  VDT_DA_DuAn_HangMuc hm on hm.iID_DuAnID = ct.iID_DuAnID and hm.iID_NguonVonID = @idNguonVon

					where 
						ct.iID_DuAnID not in (SELECT iID_DuAnID FROM VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet )
						AND ct.iID_PhanBoVon_DonVi_PheDuyet_ID = @phanBoVonId	
				)						

		END
--Tinh Gia Tri Dieu Chinh --
	BEGIN 
	DECLARE @iID_Parent uniqueidentifier = (SELECT iID_ParentId FROM VDT_KHV_PhanBoVon_DonVi_PheDuyet WHERE Id = @phanBoVonId)
	CREATE TABLE #tmmFDieuChinh(iID_DuAnID uniqueidentifier,fGiaTriPhanBoDC float, iID_LoaiCongTrinh uniqueidentifier )
	IF(@iID_Parent IS NOT NULL)
	INSERT INTO #tmmFDieuChinh(iID_DuAnID,fGiaTriPhanBoDC,iID_LoaiCongTrinh)(
		SELECT iID_DuAnID,fGiaTriPhanBo,iID_LoaiCongTrinh   FROM VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet
		Where iID_PhanBoVon_DonVi_PheDuyet_ID = @iID_Parent)
	END	
--select * FROM #tmmFDieuChinh
--select * FROm #tmpfThanhToan
--select * FROM @tmpTrangThaiDuAnMoi
IF((SELECT COUNT(*) FROM #tmmFDieuChinh) > 0 )
BEGIN
	--select iID_DuAnID, MAX(iID_PhanBoVon_DonVi_PheDuyet_ID) as iID_PhanBoVon_DonVi_PheDuyet_ID, MAX(sTenDuAn) as sTenDuAn, MAX(sLoaiDuAn) as sLoaiDuAn, MAX(sTenLoaiCongTrinh) as sTenLoaiCongTrinh, MAX(iID_LoaiCongTrinh) as iID_LoaiCongTrinh,MIN(iID_DonViQuanLyID) as iID_DonViQuanLyID, MIN(sTenDonViThucHienDuAn) as sTenDonViThucHienDuAn, 
	--MAX(fGiaTriDeNghi) as fGiaTriDeNghi,MAX(fGiaTriPhanBo) as fGiaTriPhanBo, MAX(iID_PhanBoVon_DonVi_PheDuyet_ChiTiet_ID) as iID_PhanBoVon_DonVi_PheDuyet_ChiTiet_ID, MAX(iID_Parent) as iID_Parent, MAX(sGhiChu) as sGhiChu, MAX(iID_DonViID) as iID_DonViID, MAX(sTenDonVi) as sTenDonVi , MAX(fGiaTriPhanBoDC) as fGiaTriPhanBoDC from
	WITH tmp(iID_DuAnID,iID_PhanBoVon_DonVi_PheDuyet_ID,sTenDuAn,sLoaiDuAn,sTenLoaiCongTrinh,iID_LoaiCongTrinh,iID_DonViQuanLyID,sTenDonViThucHienDuAn,fGiaTriDeNghi,fGiaTriPhanBo,iID_PhanBoVon_DonVi_PheDuyet_ChiTiet_ID,iID_Parent,
	bActive,sGhiChu,iID_DonViID,sTenDonVi,fGiaTriPhanBoDC,iID_DuAn_HangMucID)
	AS(
		select DISTINCT ct.iID_DuAnID, ct.iID_PhanBoVon_DonVi_PheDuyet_ID, da.sTenDuAn, 
		tmpTrangThai.sTrangThaiDuAn as sLoaiDuAn,
		lct.sTenLoaiCongTrinh as sTenLoaiCongTrinh,
		ct.iID_LoaiCongTrinh,
		dv.iID_Ma as iID_DonViQuanLyID,
		dv.sTen as sTenDonViThucHienDuAn,
		ct.fGiaTriDeNghi as fGiaTriDeNghi,
		fDieuChinh.fGiaTriPhanBoDC as fGiaTriPhanBo,
		ct.Id as iID_PhanBoVon_DonVi_PheDuyet_ChiTiet_ID,
		@iID_Parent as iID_Parent,
		ct.bActive,
		ct.sGhiChu ,
		tmpTrangThai.iID_DonViID,
		tmpTrangThai.sTenDonVi,
		ct.fGiaTriPhanBo as fGiaTriPhanBoDC,
		hm.iID_DuAn_HangMucID as iID_DuAn_HangMucID


		from VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet as ct 
		INNER JOIN VDT_KHV_PhanBoVon_DonVi_PheDuyet pd on pd.Id = ct.iID_PhanBoVon_DonVi_PheDuyet_ID
		INNER JOin VDT_KHV_KeHoachVonNam_DeXuat dx on dx.iID_KeHoachVonNamDeXuatID = pd.iID_VonNamDeXuatID
		INNER join VDT_DA_DuAn as da on ct.iID_DuAnID = da.iID_DuAnID
		INNER JOIN NS_DonVi dv ON dv.iID_Ma = da.iID_DonViQuanLyID
		LEFT JOIN @tmpTrangThaiDuAnMoi as tmpTrangThai on tmpTrangThai.iID_DuAnID = ct.iID_DuAnID and tmpTrangThai.iID_LoaiCongTrinhID = ct.iID_LoaiCongTrinh
		LEFT join VDT_DM_LoaiCongTrinh lct on ct.iID_LoaiCongTrinh = lct.iID_LoaiCongTrinh and tmpTrangThai.iID_LoaiCongTrinhID = lct.iID_LoaiCongTrinh
		LEFT JOIN #tmmFDieuChinh fDieuChinh on fDieuChinh.iID_DuAnID = ct.iID_DuAnID and fDieuChinh.iID_LoaiCongTrinh =ct.iID_LoaiCongTrinh
		LEFT JOIN VDT_DA_DuAn_HangMuc hm on hm.iID_DuAnID = da.iID_DuAnID and hm.iID_LoaiCongTrinhID = tmpTrangThai.iID_LoaiCongTrinhID and hm.iID_NguonVonID = @idNguonVon
	
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
		fDieuChinh.fGiaTriPhanBoDC as fGiaTriPhanBo,
		null as iID_PhanBoVon_DonVi_PheDuyet_ChiTiet_ID,
		@iID_Parent as iID_Parent,
		null as bActive,
		'' as sGhiChu,
		tmp.iID_DonViID,
		tmp.sTenDonVi,
		fDieuChinh.fGiaTriPhanBoDC as fGiaTriPhanBoDC,
		dahm.iID_DuAn_HangMucID

	from
		VDT_DA_DuAn da
		LEFT JOIN NS_DonVi dv ON dv.iID_Ma = da.iID_DonViQuanLyID AND dv.iNamLamViec_DonVi = @iNamLamViec
		LEFT JOIN VDT_DA_DuAn_HangMuc dahm ON da.iID_DuAnID = dahm.iID_DuAnID and dahm.iID_NguonVonID = @idNguonVon
		left join VDT_KHV_KeHoach5Nam_ChiTiet kh5nct on da.iID_DuAnID = kh5nct.iID_DuAnID
		left join VDT_DM_LoaiCongTrinh lct on da.iID_LoaiCongTrinhID = lct.iID_LoaiCongTrinh or dahm.iID_LoaiCongTrinhID = lct.iID_LoaiCongTrinh
		RIGHT JOIN @tmpTrangThaiDuAnMoi as tmp on tmp.iID_DuAnID = da.iID_DuAnID and dahm.iID_LoaiCongTrinhID = tmp.iID_LoaiCongTrinhID
		LEFT JOIN #tmmFDieuChinh fDieuChinh on fDieuChinh.iID_DuAnID = tmp.iID_DuAnID and fDieuChinh.iID_LoaiCongTrinh = dahm.iID_LoaiCongTrinhID
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
		AND da.iID_DuAnID NOT IN (SELECT ct.iID_DuAnID FROM VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet ct where ct.iID_PhanBoVon_DonVi_PheDuyet_ID = @phanBoVonId )


		union 

		select DISTINCT ct.iID_DuAnID, ct.iID_PhanBoVon_DonVi_PheDuyet_ID,da.sTenDuAn, 

		tmpTrangThai.sTrangThaiDuAn as sLoaiDuAn,
		lct.sTenLoaiCongTrinh,
		ct.iID_LoaiCongTrinh,
		dv.iID_Ma as iID_DonViQuanLyID,
		dv.sTen as sTenDonViThucHienDuAn,
		ct.fGiaTriDeNghi as fGiaTriDeNghi, 
		fDieuChinh.fGiaTriPhanBoDC as fGiaTriPhanBo,
		ct.Id as iID_PhanBoVon_DonVi_PheDuyet_ChiTiet_ID,
		@iID_Parent as iID_Parent,
		ct.bActive,
		ct.sGhiChu,
		tmpTrangThai.iID_DonViID,
		tmpTrangThai.sTenDonVi,
		ct.fGiaTriPhanBo as fGiaTriPhanBoDC,
		hm.iID_DuAn_HangMucID as iID_DuAn_HangMucID

		from VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet as ct 
		INNER JOIN VDT_KHV_PhanBoVon_DonVi_PheDuyet pd on pd.Id = ct.iID_PhanBoVon_DonVi_PheDuyet_ID
		INNER JOin VDT_KHV_KeHoachVonNam_DeXuat dx on dx.iID_KeHoachVonNamDeXuatID = pd.iID_VonNamDeXuatID
		LEFT join VDT_DA_DuAn as da on ct.iID_DuAnID = da.iID_DuAnID
		LEFT JOIN NS_DonVi dv ON dv.iID_Ma = da.iID_DonViQuanLyID
		LEFT JOIN @tmpTrangThaiDuAnMoi as tmpTrangThai on tmpTrangThai.iID_DuAnID = ct.iID_DuAnID and tmpTrangThai.iID_LoaiCongTrinhID = ct.iID_LoaiCongTrinh
		LEFT join VDT_DM_LoaiCongTrinh lct on ct.iID_LoaiCongTrinh = lct.iID_LoaiCongTrinh and ct.iID_LoaiCongTrinh = tmpTrangThai.iID_LoaiCongTrinhID
		LEFT JOIN #tmmFDieuChinh fDieuChinh on fDieuChinh.iID_DuAnID = ct.iID_DuAnID and fDieuChinh.iID_LoaiCongTrinh =ct.iID_LoaiCongTrinh
		LEFT JOIN VDT_DA_DuAn_HangMuc hm on hm.iID_DuAnID = ct.iID_DuAnID and hm.iID_LoaiCongTrinhID = ct.iID_LoaiCongTrinh and hm.iID_NguonVonID = dx.iID_NguonVonID
 

		Where ct.iID_PhanBoVon_DonVi_PheDuyet_ID = @phanBoVonId 
		AND dx.iID_KeHoachVonNamDeXuatID = @iIdPhanBoVonDeXuat
			AND dv.iNamLamViec_DonVi = @iNamLamViec


	) --as data
	select * from tmp where 
		(ISNULL(@sTenDuAn,'')='' OR sTenDuAn LIKE  CONCAT(N'%',@sTenDuAn,N'%'))
		AND (ISNULL(@sTenDonVi,'')='' OR sTenDonVi LIKE  CONCAT(N'%',@sTenDonVi,N'%'))
		AND (ISNULL(@sLoaiDuAn,'')='' OR sLoaiDuAn LIKE  CONCAT(N'%',@sLoaiDuAn,N'%'))
		AND iID_DuAnID NOT IN (SELECT iID_DuAnID FROM VDT_QT_QuyetToan)

	--Group by iID_DuAnID

END
ELSE
BEGIN
	WITH tmpStandar(iID_DuAnID,iID_PhanBoVon_DonVi_PheDuyet_ID,sTenDuAn,sLoaiDuAn,sTenLoaiCongTrinh,iID_LoaiCongTrinh,iID_DonViQuanLyID,sTenDonViThucHienDuAn,fGiaTriDeNghi,fGiaTriPhanBo,iID_PhanBoVon_DonVi_PheDuyet_ChiTiet_ID,iID_Parent,
	bActive,sGhiChu,iID_DonViID,sTenDonVi,fGiaTriPhanBoDC,iID_DuAn_HangMucID)
	AS(
		select DISTINCT ct.iID_DuAnID, ct.iID_PhanBoVon_DonVi_PheDuyet_ID, da.sTenDuAn, 
		tmpTrangThai.sTrangThaiDuAn as sLoaiDuAn,
		lct.sTenLoaiCongTrinh as sTenLoaiCongTrinh,
		ct.iID_LoaiCongTrinh,
		dv.iID_Ma as iID_DonViQuanLyID,
		dv.sTen as sTenDonViThucHienDuAn,
		ct.fGiaTriDeNghi as fGiaTriDeNghi,
		ct.fGiaTriPhanBo as fGiaTriPhanBo,
		ct.Id as iID_PhanBoVon_DonVi_PheDuyet_ChiTiet_ID,
		@iID_Parent as iID_Parent,
		ct.bActive,
		ct.sGhiChu ,
		tmpTrangThai.iID_DonViID,
		tmpTrangThai.sTenDonVi,
		ct.fGiaTriPhanBo as  fGiaTriPhanBoDC,
		hm.iID_DuAn_HangMucID as iID_DuAn_HangMucID

		from VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet as ct 
		INNER JOIN VDT_KHV_PhanBoVon_DonVi_PheDuyet pd on pd.Id = ct.iID_PhanBoVon_DonVi_PheDuyet_ID
		INNER JOin VDT_KHV_KeHoachVonNam_DeXuat dx on dx.iID_KeHoachVonNamDeXuatID = pd.iID_VonNamDeXuatID
		INNER join VDT_DA_DuAn as da on ct.iID_DuAnID = da.iID_DuAnID
		INNER JOIN NS_DonVi dv ON dv.iID_Ma = da.iID_DonViQuanLyID
		LEFT join VDT_DM_LoaiCongTrinh lct on ct.iID_LoaiCongTrinh = lct.iID_LoaiCongTrinh 
		LEFT JOIN @tmpTrangThaiDuAnMoi as tmpTrangThai on tmpTrangThai.iID_DuAnID = ct.iID_DuAnID AND lct.iID_LoaiCongTrinh = tmpTrangThai.iID_LoaiCongTrinhID
		LEFT JOIN VDT_DA_DuAn_HangMuc hm on hm.iID_DuAnID = ct.iID_DuAnID AND hm.iID_LoaiCongTrinhID = ct.iID_LoaiCongTrinh and hm.iID_NguonVonID = dx.iID_NguonVonID
 

	
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
		@iID_Parent as iID_Parent,
		null as bActive,
		'' as sGhiChu,
		tmp.iID_DonViID,
		tmp.sTenDonVi,
		tmp.fThanhToan as  fGiaTriPhanBoDC,
		dahm.iID_DuAn_HangMucID as iID_DuAn_HangMucID
	from
		VDT_DA_DuAn da
		LEFT JOIN NS_DonVi dv ON dv.iID_Ma = da.iID_DonViQuanLyID AND dv.iNamLamViec_DonVi = @iNamLamViec
		LEFT JOIN VDT_DA_DuAn_HangMuc dahm ON da.iID_DuAnID = dahm.iID_DuAnID and dahm.iID_NguonVonID = @idNguonVon
		left join VDT_KHV_KeHoach5Nam_ChiTiet kh5nct on da.iID_DuAnID = kh5nct.iID_DuAnID
		left join VDT_DM_LoaiCongTrinh lct on da.iID_LoaiCongTrinhID = lct.iID_LoaiCongTrinh or dahm.iID_LoaiCongTrinhID = lct.iID_LoaiCongTrinh
		RIGHT JOIN @tmpTrangThaiDuAnMoi as tmp on tmp.iID_DuAnID = da.iID_DuAnID and tmp.iID_LoaiCongTrinhID = dahm.iID_LoaiCongTrinhID
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
		AND da.iID_DuAnID NOT IN (SELECT ct.iID_DuAnID FROM VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet ct where ct.iID_PhanBoVon_DonVi_PheDuyet_ID = @phanBoVonId )

		union 

		select DISTINCT ct.iID_DuAnID, ct.iID_PhanBoVon_DonVi_PheDuyet_ID,da.sTenDuAn, 

		tmpTrangThai.sTrangThaiDuAn as sLoaiDuAn,
		lct.sTenLoaiCongTrinh,
		ct.iID_LoaiCongTrinh,
		dv.iID_Ma as iID_DonViQuanLyID,
		dv.sTen as sTenDonViThucHienDuAn,
		ct.fGiaTriDeNghi as fGiaTriDeNghi, 
		ct.fGiaTriPhanBo, 
		ct.Id as iID_PhanBoVon_DonVi_PheDuyet_ChiTiet_ID,
		@iID_Parent as iID_Parent,
		ct.bActive,
		ct.sGhiChu,
		tmpTrangThai.iID_DonViID,
		tmpTrangThai.sTenDonVi,
		ct.fGiaTriPhanBo as  fGiaTriPhanBoDC,
		hm.iID_DuAn_HangMucID as iID_DuAn_HangMucID

		from VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet as ct 
		INNER JOIN VDT_KHV_PhanBoVon_DonVi_PheDuyet pd on pd.Id = ct.iID_PhanBoVon_DonVi_PheDuyet_ID
		INNER JOin VDT_KHV_KeHoachVonNam_DeXuat dx on dx.iID_KeHoachVonNamDeXuatID = pd.iID_VonNamDeXuatID
		LEFT join VDT_DA_DuAn as da on ct.iID_DuAnID = da.iID_DuAnID
		LEFT JOIN NS_DonVi dv ON dv.iID_Ma = da.iID_DonViQuanLyID
		LEFT join VDT_DM_LoaiCongTrinh lct on ct.iID_LoaiCongTrinh = lct.iID_LoaiCongTrinh
		LEFT JOIN @tmpTrangThaiDuAnMoi as tmpTrangThai on tmpTrangThai.iID_DuAnID = ct.iID_DuAnID and tmpTrangThai.iID_LoaiCongTrinhID = lct.iID_LoaiCongTrinh
		LEFT JOIN VDT_DA_DuAn_HangMuc hm on hm.iID_DuAnID = ct.iID_DuAnID AND  hm.iID_LoaiCongTrinhID =ct.iID_LoaiCongTrinh and hm.iID_NguonVonID = @idNguonVon

		Where ct.iID_PhanBoVon_DonVi_PheDuyet_ID = @phanBoVonId 
		AND dx.iID_KeHoachVonNamDeXuatID = @iIdPhanBoVonDeXuat
			AND dv.iNamLamViec_DonVi = @iNamLamViec


	) 
	select * FROm tmpStandar where 
		(ISNULL(@sTenDuAn,'')='' OR sTenDuAn LIKE  CONCAT(N'%',@sTenDuAn,N'%'))
		AND (ISNULL(@sTenDonVi,'')='' OR sTenDonVi LIKE  CONCAT(N'%',@sTenDonVi,N'%'))
		AND (ISNULL(@sLoaiDuAn,'')='' OR sLoaiDuAn LIKE  CONCAT(N'%',@sLoaiDuAn,N'%'))
		AND iID_DuAnID NOT IN (SELECT iID_DuAnID FROM VDT_QT_QuyetToan)

	--Group by iID_DuAnID

END

	drop table #tmpfThanhToan
	--drop table @tmpTrangThaiDuAnMoi
	 drop table #tmmFDieuChinh



	 --select * FROM VDT_KHV_PhanBoVon_DonVi_ChiTiet_PheDuyet where iID_PhanBoVon_DonVi_PheDuyet_ID = 'a4616e0f-7280-4c28-bb7b-b41a84d73591'


