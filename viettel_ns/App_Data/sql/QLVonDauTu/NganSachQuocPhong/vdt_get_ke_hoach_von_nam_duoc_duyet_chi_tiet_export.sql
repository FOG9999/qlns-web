select
	khvnddct.iID_DuAnID as iID_DuAnID,
	da.sMaDuAn as sMaDuAn,
	da.sTenDuAn as sTenDuAn,
	
	khvnddct.iID_MucID as iID_MucID,
	khvnddct.iID_TieuMucID as iID_TieuMucID,
	khvnddct.iID_TietMucID as iID_TietMucID,
	khvnddct.iID_NganhID as iID_NganhID,
	
	cast(@iiDKeHoachVonNam as uniqueidentifier) as iID_KeHoachVonNam_DuocDuyetID,
	khvnddct.iID_KeHoachVonNam_DuocDuyet_ChiTietID as iID_KeHoachVonNam_DuocDuyet_ChiTietID,
	khvnddct.iID_Parent as iID_Parent,
	cast(1 as int) as ILoaiDuAn,
	N'Khởi công mới' as sLoaiDuAn,

	khvnddct.sGhiChu as sGhiChu,
	khvnddct.iID_LoaiCongTrinh as iID_LoaiCongTrinh,
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

	khvnddct.fCapPhatTaiKhoBac as fCapPhatTaiKhoBacSauDC,
	khvnddct.fCapPhatBangLenhChi as fCapPhatBangLenhChiSauDC,
	khvnddct.fGiaTriThuHoiNamTruocKhoBac as fGiaTriThuHoiNamTruocKhoBacSauDC,
	khvnddct.fGiaTriThuHoiNamTruocLenhChi as fGiaTriThuHoiNamTruocLenhChiSauDC,

	2 as Loai,
	'' as STT
from
	VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet khvnddct
left join
	VDT_DA_DuAn da
on khvnddct.iID_DuAnID = da.iID_DuAnID
left join
	VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet khvnddctpr
on khvnddct.iID_Parent = khvnddctpr.iID_KeHoachVonNam_DuocDuyet_ChiTietID
where
	khvnddct.iID_KeHoachVonNam_DuocDuyetID = @iiDKeHoachVonNam