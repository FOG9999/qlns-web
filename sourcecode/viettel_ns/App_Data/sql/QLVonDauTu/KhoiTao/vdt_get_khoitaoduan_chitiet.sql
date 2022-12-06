DECLARE @iID_KhoiTaoID uniqueidentifier set @iID_KhoiTaoID = 'eb53ef73-9a33-4ed7-9ddb-ad8a0119cd54'

--#DECLARE#--
SELECT ktct.iID_KhoiTao_ChiTietID as iId, ktct.*,duan.sTenDuAn, duan.sMaDuAn,chudautu.sTen as sTenChuDauTu, qddt_nv.fTienPheDuyet as fTongMucDauTu, dt_nv.fTienPheDuyet as fGiaTriDuToan,
qddt.sSoQuyetDinh as sSoQDDT, qddt.dNgayQuyetDinh as dNgayPheDuyetQDDT, qddt.sCoQuanPheDuyet as sCoQuanPheDuyetQDDT, qddt.sNguoiKy as sNguoiKyQDDT,
dt.sSoQuyetDinh as sSoQuyetDinhTKDT, dt.dNgayQuyetDinh as dNgayPheDuyetTKDT, dt.sCoQuanPheDuyet as sCoQuanPheDuyetTKDT, dt.sNguoiKy as sNguoiKyTKDT,
 concat(nganh.sM,'-', nganh.sTM, '-', nganh.sTTM, '-', nganh.sNG) as sTenNganh, mlns.sMoTa as sTenLoaiNguonVon, nns.sTen as sTenNguonVon, concat(mlns.sLNS,'|', mlns.iID_MaMucLucNganSach) as iID_LoaiNguonVonIDValue
FROM VDT_KT_KhoiTao kt
INNER JOIN VDT_DA_DuAn duan ON kt.iID_DuAnID = duan.iID_DuAnID
LEFT JOIN NS_DonVi chudautu on duan.iID_ChuDauTuID = chudautu.iID_Ma
INNER JOIN VDT_KT_KhoiTao_ChiTiet ktct ON kt.iID_KhoiTaoID = ktct.iID_KhoiTaoID
INNER JOIN VDT_DA_QDDauTu qddt ON kt.iID_QDDauTuID = qddt.iID_QDDauTuID
LEFT JOIN VDT_DA_DuToan dt ON kt.iID_DuToanID = dt.iID_DuToanID
INNER JOIN VDT_DA_QDDauTu_NguonVon qddt_nv ON qddt_nv.iID_QDDauTuID = kt.iID_QDDauTuID AND qddt_nv.iID_NguonVonID = ktct.iID_NguonVonID
LEFT JOIN VDT_DA_DuToan_Nguonvon dt_nv ON dt_nv.iID_DuToanID = kt.iID_DuToanID AND dt_nv.iID_NguonVonID = ktct.iID_NguonVonID
INNER JOIN NS_MucLucNganSach nganh ON nganh.iID_MaMucLucNganSach = ktct.iID_NganhID
INNER JOIN NS_MucLucNganSach mlns ON mlns.iID_MaMucLucNganSach = ktct.iID_LoaiNguonVonID
INNER JOIN NS_NguonNganSach nns ON nns.iID_MaNguonNganSach = ktct.iID_NguonVonID
WHERE kt.iID_KhoiTaoID = @iID_KhoiTaoID