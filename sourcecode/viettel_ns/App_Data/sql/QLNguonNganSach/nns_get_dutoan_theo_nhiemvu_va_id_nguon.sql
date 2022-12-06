DECLARE @iNamLamViec int SET @iNamLamViec = 2022
DECLARE @dDateFrom datetime set @dDateFrom = null
DECLARE @dDateTo datetime set @dDateTo = null
DECLARE @sSoQuyetDinh nvarchar set @sSoQuyetDinh = '365/QĐ-BQP'
DECLARE @iNguonNganSach uniqueidentifier set @iNguonNganSach = null
DECLARE @dvt int set @dvt = 1
--#DECLARE#--

SELECT dtTheoDV.*, nv.sNhiemVu as GhiChu FROM

(
SELECT dt.dNgayQuyetDinh,dt.sSoQuyetDinh, dt.sMaPhongBan, dt.sTenPhongBan,iID_MaDonVi, TenDonVi, dt.iID_NhiemVu, SUM(dt.SoTien)/@dvt as iTongTien
FROM 
(
	SELECT dt.sSoQuyetDinh, dt.dNgayQuyetDinh, sMaNoiDungChi,sMaPhongBan, concat(sMaPhongBan, ' - ', sTenPhongBan ) as sTenPhongBan, iID_MaDonVi, concat(iID_MaDonVi, ' - ', TenDonVi) as TenDonVi, dtct.iID_NhiemVu, dtct.SoTien
	FROM NNS_DuToanChiTiet as dtct
	INNER JOIN NNS_DuToan as dt on dt.iID_DuToan = dtct.iID_DuToan AND dt.sMaLoaiDuToan <> '001' AND dt.iNamLamViec = @iNamLamViec
									AND (@dDateFrom IS NULL OR @dDateFrom <= dNgayQuyetDinh)
									AND (@dDateTo IS NULL OR @dDateTo >= dNgayQuyetDinh)
) as dt
LEFT JOIN DM_NoiDungChi as ndc on ndc.sMaNoiDungChi = dt.sMaNoiDungChi
WHERE ndc.iTrangThai = 1
AND ndc.iID_NoiDungChi not in (SELECT iID_Parent FROM DM_NoiDungChi WHERE iID_Parent IS NOT NULL AND iTrangThai = 1)
AND (@iNguonNganSach is null or ndc.iID_Nguon = @iNguonNganSach)
		AND ndc.iNamLamViec = @iNamLamViec AND dt.sSoQuyetDinh = @sSoQuyetDinh
GROUP BY dt.sMaPhongBan, dt.sTenPhongBan, dt.iID_NhiemVu,iID_MaDonVi, TenDonVi, dt.dNgayQuyetDinh, dt.sSoQuyetDinh
) dtTheoDV
INNER JOIN NNS_DuToan_NhiemVu nv ON dtTheoDV.iID_NhiemVu = nv.iID_NhiemVu
ORDER BY sMaPhongBan, iID_NhiemVu