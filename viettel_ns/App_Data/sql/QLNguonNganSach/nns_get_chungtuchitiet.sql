DECLARE @iNamLamViec int set @iNamLamViec = 2019

--#DECLARE#--

SELECT * INTO #tmp
FROM 
(
SELECT ct.iID_MaChungTu, ct.iID_MaPhongBan, ct.sTenPhongBan, ct.iSoChungTu, ct.dNgayChungTu, ct.sNoiDung, ct.dNgayTao, sum(ctct.rTuChi + ctct.rChiTaiNganh + ctct.rHangNhap + ctct.rHangMua + ctct.rDuPhong) as SoTien 
FROM DTBS_ChungTuChiTiet ctct
INNER JOIN DTBS_ChungTu ct ON ctct.iID_MaChungTu = ct.iID_MaChungTu
WHERE ct.iID_MaTrangThaiDuyet = 2 AND ct.iBKhac = 0 AND ct.iTrangThai = 1 AND ct.iNamLamViec = @iNamLamViec
AND ct.iID_MaChungTu NOT IN  (select iID_MaChungTu FROM NNS_DuToan_NhiemVu WHERE iID_MaChungTu is not null)
and (select count(iID_MaChungTu_TLTH) from DTBS_ChungTu_TLTHCuc
where iID_MaChungTu_TLTH like concat('%',ct.iID_MaChungTu,'%') and iNamLamViec = @iNamLamViec) > 0
GROUP BY ct.iID_MaChungTu, ct.iID_MaPhongBan, ct.sTenPhongBan, ct.iSoChungTu, ct.dNgayChungTu, ct.sNoiDung,  ct.dNgayTao

UNION ALL

select ct.iID_MaChungTu, ct.iID_MaPhongBan, ct.sTenPhongBan, ct.iSoChungTu, ct.dNgayChungTu, ct.sNoiDung, ct.dNgayTao, 
sum(pc.rTuChi + pc.rChiTaiNganh + pc.rHangNhap + pc.rHangMua + pc.rDuPhong) as SoTien 
from DTBS_ChungTuChiTiet_PhanCap pc
inner join DTBS_ChungTuChiTiet ctct on pc.iID_MaChungTu = ctct.iID_MaChungTuChiTiet
inner join DTBS_ChungTu ct on ctct.iID_MaChungTu = ct.iID_MaChungTu
where ct.iID_MaTrangThaiDuyet = 2 and ct.iBKhac = 0 and ct.iTrangThai = 1 and ct.iNamLamViec = @iNamLamViec
AND ct.iID_MaChungTu NOT IN  (select iID_MaChungTu FROM NNS_DuToan_NhiemVu WHERE iID_MaChungTu is not null)

group by ct.iID_MaChungTu, ct.iID_MaPhongBan, ct.sTenPhongBan, ct.iSoChungTu, ct.dNgayChungTu, ct.sNoiDung, ct.dNgayTao
) as tmp

select iID_MaChungTu, iID_MaPhongBan, sTenPhongBan, iSoChungTu, dNgayChungTu, sNoiDung, dNgayTao, 
sum(SoTien) as SoTien 
FROM #tmp
GROUP BY iID_MaChungTu, iID_MaPhongBan, sTenPhongBan, iSoChungTu, dNgayChungTu, sNoiDung, dNgayTao
ORDER BY dNgayTao DESC

drop table #tmp


