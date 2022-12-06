
--#DECLARE#--
/*

Lấy danh sách hạng mục chủ trương đầu tư

*/


select	
			hm.iID_ChuTruongDauTu_DM_HangMucID as iID_DuAn_HangMucID,
			hm.iID_ChuTruongDauTu_DM_HangMucID as iID_HangMucID,
			hm.iID_DuAnID ,
			hm.iID_ParentID,
			hm.sMaHangMuc,
			hm.sTenHangMuc,
			lct.sTenLoaiCongTrinh as sTenLoaiCongTrinh,
			hm.fTienHangMuc,
			hm.smaOrder,
			hm.iID_LoaiCongTrinhID,
			cthm.iID_ChuTruongDauTu_HangMucID ,
			cthm.iID_ChuTruongDauTuID
			
			
	from VDT_DA_ChuTruongDauTu_DM_HangMuc hm
		inner join VDT_DA_ChuTruongDauTu_HangMuc cthm ON cthm.iID_HangMucID = hm.iID_ChuTruongDauTu_DM_HangMucID
		left join VDT_DM_LoaiCongTrinh lct ON lct.iID_LoaiCongTrinh = hm.iID_LoaiCongTrinhID
		
	where cthm.iID_ChuTruongDauTuID = @chuTruongId
order by hm.smaOrder