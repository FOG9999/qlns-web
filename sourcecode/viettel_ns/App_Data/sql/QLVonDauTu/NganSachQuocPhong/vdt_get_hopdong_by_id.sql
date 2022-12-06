--#DECLARE#--

/*

Lấy thông tin hợp đồng theo id

*/

select hd.iID_HopDongID,
			hd.dNgayHopDong,
			hd.sNganHang,
			(select ISNULL(sum(fTienHopDong), 0) from VDT_DA_TT_HopDong 
				where iID_HopDongGocID = (select iID_HopDongGocID from VDT_DA_TT_HopDong where iID_HopDongID = @iID_HopDongID)
				and dNgayHopDong <= @dNgayQuyetDinh) as fTongGiaTriHD,
			gt.sTenGoiThau,
			nt.sTenNhaThau,
			nt.sSoTaiKhoan,
			nt.iID_NhaThauID,
			(select ISNULL(sum(fTienTrungThau), 0) 
				from VDT_DA_GoiThau 
				where iID_GoiThauGocID = (select iID_GoiThauGocID 
								from VDT_DA_GoiThau gt 
								INNER JOIN VDT_DA_TT_HopDong hd on gt.iID_GoiThauID = hd.iID_GoiThauID
								where hd.iID_HopDongID = @iID_HopDongID)
							and dNgayLap <= @dNgayQuyetDinh) as fTongTienTrungThau
			
from VDT_DA_TT_HopDong hd
left join VDT_DA_GoiThau gt on hd.iID_GoiThauID = gt.iID_GoiThauID
left join VDT_DM_NhaThau nt on hd.iID_NhaThauThucHienID = nt.iID_NhaThauID
where hd.iID_HopDongID = @iID_HopDongID

