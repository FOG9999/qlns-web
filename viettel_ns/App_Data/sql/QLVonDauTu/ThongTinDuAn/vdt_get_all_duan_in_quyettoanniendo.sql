--#DECLARE#--

/*

Lấy thông tin dự án quyết toán niên độ

*/

Select DISTINCT 
		da.iID_DuAnID,da.sTenDuAn,da.sMaDuAn,
		((Select m.sMa from VDT_DM_PhanCapDuAn m Where m.iID_PhanCapID=da.iID_CapPheDuyetID)) as TenPhanCap,
		(Select Top 1 qddt.sSoQuyetDinh from VDT_DA_QDDauTu qddt WHERE qddt.iID_DuAnID=da.iID_DuAnID
		) as SoQuyetDinhDauTu,
		(				 
			(SELECT ISNULL(SUM(qddtnv.fTienPheDuyet * qddtnv.fTiGia * qddtnv.fTiGiaDonVi),0) FROM VDT_DA_QDDauTu_NguonVon qddtnv JOIN VDT_DA_QDDauTu qddt On qddt.iID_QDDauTuID=qddtnv.iID_QDDauTuID
								WHERE ( qddt.iID_DuAnID=da.iID_DuAnID) AND (@NgayLap IS NULL OR qddt.dNgayQuyetDinh <= @ngayLap)
										AND (@nguonVonId IS NULL OR qddtnv.iID_NguonVonID=@nguonVonId))		
		)as GiaTriDauTu,
		(SELECT ISNULL(fHanMucDauTu,0) FROM VDT_DA_DuAn WHERE iID_DuAnID=da.iID_DuAnID) as HanMucDauTu
								
from VDT_KHV_PhanBoVon pbv join VDT_KHV_PhanBoVon_ChiTiet pbvct ON pbv.iID_PhanBoVonID=pbvct.iID_PhanBoVonID 
						join VDT_DA_DuAn da on pbvct.iID_DuAnID=da.iID_DuAnID	
Where pbv.iNamKeHoach=@year and (@loaiNguonVonID IS NULL OR pbv.iID_LoaiNguonVonID=@loaiNguonVonID )
		and (@donViQuanLyId IS NULL OR pbv.iID_DonViQuanLyID=@donViQuanLyId)
		And (@ngayLap is null or pbv.dNgayQuyetDinh <= @ngayLap)
		AND (@nganhId IS  NULL OR pbvct.iID_NganhID=@nganhId)
		AND pbvct.iID_DuAnID IS not null