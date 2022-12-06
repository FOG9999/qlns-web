--#DECLARE#--

/* Lấy all đề nghị thanh toán ứng chi tiet theo dnttu id */

-- list Thong tin De nghi thanh toan chi tiet
select dnttu.iID_DeNghiThanhToanID, 
			 dnttu.dNgayDeNghi,
			 dnttu.iID_DonViQuanLyID,
			 dnttu.iID_NhomQuanLyID,
			 dnttuct.iID_DeNghiThanhToan_ChiTietID,
			 dnttuct.iID_HopDongID,
			 dnttuct.iID_DuAnID INTO #tmpDNTT
from VDT_TT_DeNghiThanhToanUng_ChiTiet dnttuct
left join VDT_TT_DeNghiThanhToanUng dnttu on dnttuct.iID_DeNghiThanhToanID = dnttu.iID_DeNghiThanhToanID
 where dnttu.iID_DeNghiThanhToanID = @iID_DeNghiThanhToanID
 
-- list Thong tin hop dong va goi thau
select hd.iID_HopDongID ,hd.iID_HopDongGocID, hd.iID_GoiThauID, gt.iID_GoiThauGocID INTO #tmpHopDong 
from VDT_DA_TT_HopDong hd
left join VDT_DA_GoiThau gt on hd.iID_GoiThauID = gt.iID_GoiThauID
where hd.iID_HopDongID in (select iID_HopDongID from #tmpDNTT)

-- Tong Tien hop dong theo iID_HopDongGocID
select hd.iID_HopDongID ,hd.iID_HopDongGocID, ISNULL(sum(fTienHopDong), 0) as fTongGiaTriHD INTO #tmpTongGTHD
from VDT_DA_TT_HopDong hd
inner JOIN #tmpHopDong tmphd on hd.iID_HopDongGocID = tmphd.iID_HopDongGocID
where hd.dNgayHopDong <= (select top(1) dNgayDeNghi from #tmpDNTT)
GROUP BY hd.iID_HopDongID, hd.iID_HopDongGocID

-- Tinh tong tien trung thau theo iID_GoiThauGocID
select tmphd.iID_HopDongID, tmphd.iID_GoiThauID, ISNULL(sum(gt.fTienTrungThau), 0) as fTongTienTrungThau INTO #tmpTongThau
from #tmpHopDong tmphd 
left join VDT_DA_GoiThau gt on tmphd.iID_GoiThauGocID = gt.iID_GoiThauGocID
and gt.dNgayLap <= (select top(1) dNgayDeNghi from #tmpDNTT)
GROUP BY tmphd.iID_HopDongID, tmphd.iID_GoiThauID

-- Tinh Luy ke KHVU duoc duyet
select tmpDNTT.iID_DeNghiThanhToan_ChiTietID, tmpDNTT.iID_DuAnID, ISNULL(sum(khvuct.fGiaTriUng), 0) as fLKKHVUDuocDuyet INTO #tmpLKKHVUDuocDuyet
from VDT_KHV_KeHoachVonUng_ChiTiet khvuct
left join VDT_KHV_KeHoachVonUng khvu on khvuct.iID_KeHoachUngID = khvu.iID_KeHoachUngID
and khvu.iID_DonViQuanLyID = (select top(1) iID_DonViQuanLyID from #tmpDNTT)
and khvu.iID_NhomQuanLyID = (select top(1) iID_NhomQuanLyID from #tmpDNTT)
and khvu.dNgayQuyetDinh <= (select top(1) dNgayDeNghi from #tmpDNTT)
left join #tmpDNTT tmpDNTT on khvuct.iID_DuAnID = tmpDNTT.iID_DuAnID
GROUP BY tmpDNTT.iID_DeNghiThanhToan_ChiTietID, tmpDNTT.iID_DuAnID

--Lấy lũy kế ứng theo dự án và hợp đồng
select tmpDNTT.iID_DuAnID, 
		ISNULL(sum(dnttuct.fGiaTriThanhToan), 0) as fLKSoVonDaTamUng,
		ISNULL(sum(dnttuct.fGiaTriThuHoiUngNgoaiChiTieu), 0) as fLKThuHoiUng INTO #tmpLuyKeUng
from VDT_TT_DeNghiThanhToanUng_ChiTiet dnttuct
left join VDT_TT_DeNghiThanhToanUng dnttu on dnttuct.iID_DeNghiThanhToanID = dnttu.iID_DeNghiThanhToanID
		and dnttu.iID_DonViQuanLyID = (select top(1) iID_DonViQuanLyID from #tmpDNTT)
		and dnttu.iID_NhomQuanLyID = (select top(1) iID_NhomQuanLyID from #tmpDNTT)
		and dnttu.dNgayDeNghi <= (select top(1) dNgayDeNghi from #tmpDNTT)
left join #tmpDNTT tmpDNTT on dnttuct.iID_DuAnID = tmpDNTT.iID_DuAnID
		and (tmpDNTT.iID_HopDongID is null or dnttuct.iID_HopDongID = tmpDNTT.iID_HopDongID)
GROUP BY tmpDNTT.iID_DuAnID

select dnttuct.iID_DeNghiThanhToan_ChiTietID, 
			dnttuct.iID_DuAnID, 
			dnttuct.iID_HopDongID,
			dnttuct.iID_NhaThauID,
			dnttuct.fGiaTriThanhToan,
			dnttuct.fGiaTriThuHoiUngNgoaiChiTieu,
			da.sTenDuAn,
			dm_nt.sTenNhaThau,
			dm_nt.sSoTaiKhoan,
			hd.sNganHang,
			tmpTongGTHD.fTongGiaTriHD,
			tmpTongThau.fTongTienTrungThau,
			tmpLKKHVUDuocDuyet.fLKKHVUDuocDuyet,
			tmpLuyKeUng.fLKSoVonDaTamUng,
			tmpLuyKeUng.fLKThuHoiUng
from dbo.VDT_TT_DeNghiThanhToanUng_ChiTiet dnttuct
left join VDT_DA_DuAn da on dnttuct.iID_DuAnID = da.iID_DuAnID
left join VDT_DM_NhaThau dm_nt on dnttuct.iID_NhaThauID = dm_nt.iID_NhaThauID
left join VDT_DA_TT_HopDong hd on dnttuct.iID_HopDongID = hd.iID_HopDongID
left join #tmpTongGTHD tmpTongGTHD on dnttuct.iID_HopDongID = tmpTongGTHD.iID_HopDongID
left join #tmpTongThau tmpTongThau on dnttuct.iID_HopDongID = tmpTongThau.iID_HopDongID
left join #tmpLKKHVUDuocDuyet tmpLKKHVUDuocDuyet on dnttuct.iID_DeNghiThanhToan_ChiTietID = tmpLKKHVUDuocDuyet.iID_DeNghiThanhToan_ChiTietID
left join #tmpLuyKeUng tmpLuyKeUng on dnttuct.iID_DuAnID = tmpLuyKeUng.iID_DuAnID
where dnttuct.iID_DeNghiThanhToanID = @iID_DeNghiThanhToanID

drop table #tmpDNTT
drop table #tmpHopDong
drop table #tmpTongGTHD
drop table #tmpTongThau
drop table #tmpLKKHVUDuocDuyet
drop table #tmpLuyKeUng