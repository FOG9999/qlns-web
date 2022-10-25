--#DECLARE#--

/*

Lấy thông tin dự án báo cáo tình hình thực hiện dự án, thông tin chung

*/

select da.sTenDuAn as sTenDuAn,
dv.sTen as sTenDonVi,
qddt.sSoQuyetDinh as sSoQuyetDinh,
qddt.dNgayQuyetDinh as dNgayQuyetDinh,
qddt.fTongMucDauTuPheDuyet as fTongMucDauTuPheDuyet,
concat(qddt.sKhoiCong, '-',qddt.sKetThuc) as sThoiGianThucHien,
(
	select Sum(isnull(pbv_ct.fCapPhatTaiKhoBac,0)) + Sum(isnull(pbv_ct.fCapPhatBangLenhChi,0))		
	from VDT_KHV_PhanBoVon_ChiTiet as pbv_ct
	inner join VDT_KHV_PhanboVon as pbv on pbv_ct.iID_PhanBoVonID = pbv.iID_PhanBoVonID
	where pbv.bActive =1 and da.iID_DuAnID = pbv_ct.iID_DuAnID and iNamKeHoach <= @Nam
	group by pbv_ct.iID_DuAnID
) as fLuyKeVonDaBoTri,
(
	select Sum(isnull(pbv_ct.fCapPhatTaiKhoBac,0)) + Sum(isnull(pbv_ct.fCapPhatBangLenhChi,0))		
	from VDT_KHV_PhanBoVon_ChiTiet as pbv_ct
	inner join VDT_KHV_PhanboVon as pbv on pbv_ct.iID_PhanBoVonID = pbv.iID_PhanBoVonID
	where pbv.bActive =1 and da.iID_DuAnID = pbv_ct.iID_DuAnID and iID_NguonVonID = 1  and iNamKeHoach <= @Nam
	group by pbv_ct.iID_DuAnID
) as fNganSachQuocPhong,
(
	select Sum(isnull(pbv_ct.fCapPhatTaiKhoBac,0))	
	from VDT_KHV_PhanBoVon_ChiTiet as pbv_ct
	inner join VDT_KHV_PhanboVon as pbv on pbv_ct.iID_PhanBoVonID = pbv.iID_PhanBoVonID
	where pbv.bActive =1 and da.iID_DuAnID = pbv_ct.iID_DuAnID  and iNamKeHoach <= @Nam
	group by pbv_ct.iID_DuAnID
) as fLuyKeThanhToanQuaKhoBac,
(
	select Sum(isnull(khvu_ct.fCapPhatTaiKhoBac,0))	
	from VDT_KHV_KeHoachVonUng_ChiTiet as khvu_ct
	inner join VDT_KHV_KeHoachVonUng as khvu on khvu_ct.iID_KeHoachUngID = khvu.Id
	where  da.iID_DuAnID = khvu_ct.iID_DuAnID and iNamKeHoach <= @Nam
	group by khvu_ct.iID_DuAnID
) as fKeHoachUng,
(
	select Sum(qddt_nv.fTienPheDuyet) as fNguonNganSachQuocPhong
	from VDT_DA_QDDauTu_NguonVon as qddt_nv
	inner join VDT_DA_QDDauTu as qddt on qddt_nv.iID_QDDauTuID = qddt.iID_QDDauTuID
	where qddt_nv.iID_NguonVonID = 1 
	group by qddt_nv.iID_QDDauTu_NguonVonID

)as fNguonNganSachQuocPhong

from VDT_DA_DuAn as da
left join VDT_DA_QDDauTu as qddt on da.iID_DuAnID = qddt.iID_DuAnID
left join NS_DonVi as dv on dv.iID_Ma = da.iID_DonViQuanLyID
where da.iID_DuAnID = @iID_DuAn