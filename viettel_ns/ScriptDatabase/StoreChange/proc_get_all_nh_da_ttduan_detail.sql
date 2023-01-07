CREATE PROCEDURE [dbo].[proc_get_all_nh_da_ttduan_detail]
	@id uniqueidentifier 
AS
BEGIN
	SELECT DA.*, dv.sTen AS sDonVi, CONCAT(pb.sTen,' - ',pb.sMoTa) AS sBQuanLy,CONCAT(cdt.sId_CDT, ' - ', cdt.sTenCDT) AS sChuDauTu
	,pcpd.sTen AS sPhanCapPheDuyet
	,nvc.iID_KHChiTietID AS iID_BQP
	,CONCAT(N'KHTT ', bqp.iGiaiDoanTu, ' - ', bqp.iGiaiDoanDen, N' -  Số KH:', bqp.sSoKeHoach) AS BQP
	,nvc.sTenNhiemVuChi
	,tg.sTenTiGia
	,DA.sMaNgoaiTeKhac AS sTenMaNgoaiTeKhac
	FROM NH_DA_DuAn DA
	LEFT JOIN NS_DonVi dv ON DA.iID_DonViID = dv.iID_Ma
	LEFT JOIN NS_PhongBan pb ON DA.iID_BQuanLyID = pb.iID_MaPhongBan
	LEFT JOIN DM_ChuDauTu cdt ON DA.iID_ChuDauTuID = cdt.ID
	LEFT JOIN NH_DM_PhanCapPheDuyet pcpd ON DA.iID_CapPheDuyetID = pcpd.ID
	LEFT JOIN NH_DM_TiGia tg ON DA.iID_TiGiaID = tg.ID
	LEFT JOIN NH_KHChiTietBQP_NhiemVuChi nvc ON DA.iID_KHCTBQP_ChuongTrinhID = nvc.ID
	LEFT JOIN NH_KHChiTietBQP bqp ON nvc.iID_KHChiTietID = bqp.ID
	WHERE DA.ID = @id
END