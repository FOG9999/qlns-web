DECLARE @iID_KhoiTaoID uniqueidentifier set @iID_KhoiTaoID = '6b6bebbb-16ff-4558-a323-ad8900a874ed'

--#DECLARE#--
SELECT kt.*, duan.sMaDuAn, duan.sTenDuAn, dv.sTen as sTenDonViQuanLy,
pcda.sTen as sTenCapPheDuyet, lct.sTenLoaiCongTrinh, chudautu.sTen as sTenChuDauTu, duan.sKhoiCong, duan.sKetThuc,
qddt.sSoQuyetDinh as sSoQDDT, qddt.dNgayQuyetDinh as dNgayDuyetQDDT, qddt.sCoQuanPheDuyet as sCoQuanPheDuyetQDDT, qddt.sNguoiKy as sNguoiKyQDDT,
dt.sSoQuyetDinh as sSoTKDT, dt.dNgayQuyetDinh as dNgayDuyetTKDT, dt.sCoQuanPheDuyet as sCoQuanPheDuyetTKDT, dt.sNguoiKy as sNguoiKyTKDT,
duan.iID_ChuDauTuID, duan.iID_CapPheDuyetID, duan.iID_LoaiCongTrinhID, duan.iID_DonViQuanLyID, duan.iID_NhomQuanLyID
FROM VDT_KT_KhoiTao kt
INNER JOIN VDT_DA_DuAn duan ON kt.iID_DuAnID = duan.iID_DuAnID
LEFT JOIN VDT_DM_PhanCapDuAn pcda on duan.iID_CapPheDuyetID = pcda.iID_PhanCapID
LEFT JOIN NS_DonVi dv on duan.iID_DonViQuanLyID = dv.iID_Ma
LEFT JOIN NS_DonVi chudautu on duan.iID_ChuDauTuID = chudautu.iID_Ma
LEFT JOIN VDT_DM_LoaiCongTrinh lct on duan.iID_LoaiCongTrinhID = lct.iID_LoaiCongTrinh
INNER JOIN VDT_DA_QDDauTu qddt ON kt.iID_QDDauTuID = qddt.iID_QDDauTuID
LEFT JOIN VDT_DA_DuToan dt ON kt.iID_DuToanID = dt.iID_DuToanID

WHERE kt.iID_KhoiTaoID = @iID_KhoiTaoID