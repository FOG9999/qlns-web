DECLARE @id uniqueidentifier SET @id = '00000000-0000-0000-0000-000000000000'

--#DECLARE#--

SELECT hd.*,
	CONCAT(dv.iID_MaDonVi, ' - ', dv.sTen) AS sTenDonVi,
	CONCAT(pb.sTen,' - ',pb.sMoTa) AS sTenBQuanLy,
	nvc.sTenNhiemVuChi AS sTenChuongTrinh,
	CONCAT(N'KHTT ', bqp.iGiaiDoanTu, ' - ', bqp.iGiaiDoanDen, N' -  Số KH:', bqp.sSoKeHoach) AS sSoKeHoachBQP,
	da.sTenDuAn,
	dmhd.sTenLoaiHopDong,
	nt.sTenNhaThau,
	tg.sMaTienTeGoc,
	tg.sTenTiGia
FROM NH_DA_HopDong hd
LEFT JOIN NS_DonVi dv ON hd.iID_MaDonVi = dv.iID_MaDonVi AND hd.iID_DonViID = dv.iID_Ma AND dv.iTrangThai = 1
LEFT JOIN NS_PhongBan pb ON hd.iID_BQuanLyID = pb.iID_MaPhongBan
LEFT JOIN NH_DA_DuAn da ON hd.iID_DuAnID = da.ID
LEFT JOIN NH_KHChiTietBQP_NhiemVuChi nvc ON hd.iID_KHCTBQP_ChuongTrinhID = nvc.ID
LEFT JOIN NH_KHChiTietBQP bqp ON nvc.iID_KHChiTietID = bqp.ID
LEFT JOIN NH_DM_LoaiHopDong dmhd ON hd.iID_LoaiHopDongID = dmhd.ID
LEFT JOIN NH_DM_TiGia tg ON hd.iID_TiGiaID = tg.ID
LEFT JOIN NH_DM_NhaThau nt ON hd.iID_NhaThauThucHienID = nt.ID
WHERE hd.ID = @id