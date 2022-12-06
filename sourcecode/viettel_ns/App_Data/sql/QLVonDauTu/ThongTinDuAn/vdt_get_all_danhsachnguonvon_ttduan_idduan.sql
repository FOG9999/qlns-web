--#DECLARE#--

/*

Lấy danh sách nguồn vốn được tạo ở màn tạo mới thông tin dự án

*/

select duan_nguonvon.Id as iID_DuAn_NguonVon, duan.iID_DuAnID, iID_NguonVonID, sTen as sTenNguonVon, fThanhTien from VDT_DA_DuAn as duan
left join VDT_DA_DuAn_NguonVon as duan_nguonvon on duan.iID_DuAnID = duan_nguonvon.iID_DuAn
left join NS_NguonNganSach as ngansach on duan_nguonvon.iID_NguonVonID =  ngansach.iID_MaNguonNganSach
where duan.iID_DuAnID = @iID_DuAnID