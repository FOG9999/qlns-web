DECLARE @id  uniqueidentifier
SET @id = '00000000-0000-0000-0000-000000000000'

--#DECLARE#--

SELECT gt.*
	,CONCAT(dv.iID_MaDonVi, '-', dv.sTen) AS sTenDonVi
	,da.sTenDuAn
	,khct.sTenNhiemVuChi AS sTenChuongTrinh
	,dmhd.sTenLoaiHopDong
	,htcnt.sTenHinhThuc
	,ptcnt.sTenPhuongThuc
	,nt.sTenNhaThau
	,ltt.sMaTienTe
	,tg.sTenTiGia
FROM NH_DA_GoiThau gt
LEFT JOIN NS_DonVi dv ON gt.iID_MaDonVi = dv.iID_MaDonVi AND gt.iID_DonViID = dv.iID_Ma AND dv.iTrangThai = 1
LEFT JOIN NH_DA_DuAn da ON gt.iID_DuAnID = da.ID
LEFT JOIN NH_KHChiTietBQP_NhiemVuChi khct ON gt.iID_KHCTBQP_ChuongTrinhID = khct.ID
LEFT JOIN NH_DM_LoaiHopDong dmhd ON gt.iID_LoaiHopDongID = dmhd.ID
LEFT JOIN NH_DM_HinhThucChonNhaThau htcnt ON gt.iID_HinhThucChonNhaThauID = htcnt.ID
LEFT JOIN NH_DM_PhuongThucChonNhaThau ptcnt ON gt.iID_PhuongThucChonNhaThauID = ptcnt.ID
LEFT JOIN NH_DM_NhaThau nt ON gt.iID_NhaThauThucHienID = nt.Id
LEFT JOIN NH_DM_LoaiTienTe ltt ON gt.sThanhToanBang = ltt.sMaTienTe
LEFT JOIN NH_DM_TiGia tg ON gt.iID_TiGiaID = tg.ID

WHERE gt.ID = @id