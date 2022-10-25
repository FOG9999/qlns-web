
--#DECLARE#--
/*

Lấy danh sách dự nguồn vốn chủ trương đầu tư theo dự án- màn tạo mới phê duyệt dự án

*/


select ns.sTen as sTenNguonVon,
		ctnv.iID_NguonVonID as iID_NguonVonID,
		ctnv.fTienPheDuyet 
		
	from VDT_DA_ChuTruongDauTu_NguonVon ctnv
		inner join NS_NguonNganSach ns ON ns.iID_MaNguonNganSach = ctnv.iID_NguonVonID
		inner join VDT_DA_ChuTruongDauTu ct ON ct.iID_ChuTruongDauTuID = ctnv.iID_ChuTruongDauTuID
	where ct.iID_DuAnID =  @idDuAn and ct.bActive = 1;