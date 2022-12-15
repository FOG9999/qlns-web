DECLARE @phanBoVonId uniqueidentifier set @phanBoVonId = '55c39019-52a9-478d-a4ab-3fc40d9ed7d0'
DECLARE @iIdPhanBoVonDeXuat uniqueidentifier set @iIdPhanBoVonDeXuat = '3d6a1692-9c92-4601-84ff-7b6ca8a553c5'
DECLARE @sTenDuAn nvarchar(50) set @sTenDuAn=null
DECLARE @sLoaiDuAn nvarchar(50) set @sLoaiDuAn=null
DECLARE @sTenDonViThucHienDuAn nvarchar(50) set @sTenDonViThucHienDuAn=null
DECLARE @iNamLamViec int set @iNamLamViec = 2022

--#DECLARE#--
select * from
(
select
	distinct
	da.iID_DuAnID as iID_DuAnID,
	da.sMaDuAn as sMaDuAn,
	da.sTenDuAn as sTenDuAn,
	dv.sTen as sTenDonViThucHienDuAn,
	null as iID_MucID,
	null as iID_TieuMucID,
	null as iID_TietMucID,
	null as iID_NganhID,
	
	cast(@phanBoVonId as uniqueidentifier) as iID_KeHoachVonNam_DuocDuyetID,
	null as iID_KeHoachVonNam_DuocDuyet_ChiTietID,
	null as iID_Parent,
	cast(1 as int) as iLoaiDuAn,
	N'Khởi công mới' as sLoaiDuAn,
	'' as sGhiChu,
	case 
		when dahm.iID_LoaiCongTrinhID is not null then dahm.iID_LoaiCongTrinhID else da.iID_LoaiCongTrinhID
	end iID_LoaiCongTrinh,
	lct.sTenLoaiCongTrinh as sLoaiCongTrinh,
	
	lct.LNS as LNS,
	lct.L as L,
	lct.K as K,
	lct.M as M,
	lct.TM as TM,
	lct.TTM as TTM,
	lct.NG as NG,
	cast(0 as float) as fCapPhatTaiKhoBac,
	cast(0 as float) as fCapPhatBangLenhChi,
	cast(0 as float) as fGiaTriThuHoiNamTruocKhoBac,
	cast(0 as float) as fGiaTriThuHoiNamTruocLenhChi,

	cast(0 as float) as fCapPhatTaiKhoBacSauDC,
	cast(0 as float) as fCapPhatBangLenhChiSauDC,
	cast(0 as float) as fGiaTriThuHoiNamTruocKhoBacSauDC,
	cast(0 as float) as fGiaTriThuHoiNamTruocLenhChiSauDC,
	CASE
		WHEN khvndxct.fThanhToanDC is not null THEN khvndxct.fThanhToanDC ELSE khvndxct.fThanhToan
	END fGiaTriDeNghi,
	1 as Loai,
	'' as STT
from
	VDT_DA_DuAn da
	LEFT JOIN NS_DonVi dv ON dv.iID_MaDonVi = da.iID_MaDonViThucHienDuAnID AND dv.iNamLamViec_DonVi = @iNamLamViec
left join
	VDT_DA_DuAn_HangMuc dahm
on da.iID_DuAnID = dahm.iID_DuAnID
left join
	VDT_KHV_KeHoach5Nam_ChiTiet kh5nct
on da.iID_DuAnID = kh5nct.iID_DuAnID
left join
	VDT_DM_LoaiCongTrinh lct
on da.iID_LoaiCongTrinhID = lct.iID_LoaiCongTrinh or dahm.iID_LoaiCongTrinhID = lct.iID_LoaiCongTrinh
Left join VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet khvndxct
on da.iID_DuAnID = khvndxct.iID_DuAnID
where
	da.iID_DuAnID in (
		select 
			distinct
			khvndxct.iID_DuAnID
		from
			VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet khvndxct
		inner join
			VDT_KHV_KeHoachVonNam_DeXuat khvndx
		on khvndxct.iID_KeHoachVonNamDeXuatID = khvndx.iID_KeHoachVonNamDeXuatID
		where 
			khvndx.iID_KeHoachVonNamDeXuatID = @iIdPhanBoVonDeXuat
			and khvndxct.iID_DonViID = (SELECT iID_DonViQuanLyID FROM VDT_KHV_KeHoachVonNam_DuocDuyet where iID_KeHoachVonNam_DuocDuyetID = @phanBoVonId)
	)
union all

select
	khvnddct.iID_DuAnID as iID_DuAnID,
	da.sMaDuAn as sMaDuAn,
	da.sTenDuAn as sTenDuAn,
	dv.sTen as sTenDonViThucHienDuAn,
	khvnddct.iID_MucID as iID_MucID,
	khvnddct.iID_TieuMucID as iID_TieuMucID,
	khvnddct.iID_TietMucID as iID_TietMucID,
	khvnddct.iID_NganhID as iID_NganhID,
	
	cast(@phanBoVonId as uniqueidentifier) as iID_KeHoachVonNam_DuocDuyetID,
	khvnddct.iID_KeHoachVonNam_DuocDuyet_ChiTietID as iID_KeHoachVonNam_DuocDuyet_ChiTietID,
	khvnddct.iID_Parent as iID_Parent,
	cast(1 as int) as iLoaiDuAn,
	N'Khởi công mới' as sLoaiDuAn,

	khvnddct.sGhiChu as sGhiChu,
	khvnddct.iID_LoaiCongTrinh as iID_LoaiCongTrinh,
	lct.sTenLoaiCongTrinh as sLoaiCongTrinh,
	
	khvnddct.LNS as LNS,
	khvnddct.L as L,
	khvnddct.K as K,
	khvnddct.M as M,
	khvnddct.TM as TM,
	khvnddct.TTM as TTM,
	khvnddct.NG as NG,
	
	case 
		when khvnddct.iID_Parent is null then khvnddct.fCapPhatTaiKhoBac else khvnddctpr.fCapPhatTaiKhoBac
	end fCapPhatTaiKhoBac,

	case
		when khvnddct.iID_Parent is null then khvnddct.fCapPhatBangLenhChi else khvnddctpr.fCapPhatBangLenhChi
	end fCapPhatBangLenhChi,

	case 
		when khvnddct.iID_Parent is null then khvnddct.fGiaTriThuHoiNamTruocKhoBac else khvnddctpr.fGiaTriThuHoiNamTruocKhoBac
	end fGiaTriThuHoiNamTruocKhoBac,

	case
		when khvnddct.iID_Parent is null then khvnddct.fGiaTriThuHoiNamTruocLenhChi else khvnddctpr.fGiaTriThuHoiNamTruocLenhChi
	end fGiaTriThuHoiNamTruocLenhChi,

	khvnddct.fCapPhatTaiKhoBacDC as fCapPhatTaiKhoBacSauDC,
	khvnddct.fCapPhatBangLenhChiDC as fCapPhatBangLenhChiSauDC,
	khvnddct.fGiaTriThuHoiNamTruocKhoBacDC as fGiaTriThuHoiNamTruocKhoBacSauDC,
	khvnddct.fGiaTriThuHoiNamTruocLenhChiDC as fGiaTriThuHoiNamTruocLenhChiSauDC,
	CASE
		WHEN khvnct.fThanhToanDC is not null THEN khvnct.fThanhToanDC ELSE khvnct.fThanhToan
	END fGiaTriDeNghi,
	2 as Loai,
	'' as STT
from
	VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet khvnddct
left join
	VDT_DA_DuAn da
on khvnddct.iID_DuAnID = da.iID_DuAnID
LEFT JOIN NS_DonVi dv ON dv.iID_MaDonVi = da.iID_MaDonViThucHienDuAnID AND dv.iNamLamViec_DonVi = @iNamLamViec
left join
	VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet khvnddctpr
on khvnddct.iID_Parent = khvnddctpr.iID_KeHoachVonNam_DuocDuyet_ChiTietID
left join
	VDT_DM_LoaiCongTrinh lct
on khvnddct.iID_LoaiCongTrinh = lct.iID_LoaiCongTrinh
LEFT JOIN 
	VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet khvnct
on khvnct.iID_DuAnID = khvnddct.iID_DuAnID AND khvnct.iID_KeHoachVonNamDeXuatID = @iIdPhanBoVonDeXuat
where
	khvnddct.iID_KeHoachVonNam_DuocDuyetID = @phanBoVonId
	and dv.iID_Ma = (SELECT iID_DonViQuanLyID FROM VDT_KHV_KeHoachVonNam_DuocDuyet where iID_KeHoachVonNam_DuocDuyetID = @phanBoVonId)
	
	) as data
where (@sTenDuAn = '' or @sTenDuAn is null or sTenDuAn is null or sTenDuAn like @sTenDuAn)
and (@sLoaiDuAn = '' or @sLoaiDuAn is null or sLoaiDuAn is null or sLoaiDuAn like @sLoaiDuAn)
and (@sTenDonViThucHienDuAn = '' or @sTenDonViThucHienDuAn is null or sTenDonViThucHienDuAn is null or sTenDonViThucHienDuAn like @sTenDonViThucHienDuAn)

