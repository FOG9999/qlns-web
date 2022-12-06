--DECLARE @iID_DeNghiThanhToanID uniqueidentifier SET @iID_DeNghiThanhToanID = '9378B259-DEC2-4881-A44B-ADCE00EB863E'
DECLARE @iID_DeNghiThanhToanID uniqueidentifier
--#DECLARE#--
SELECT dntt.*, hopdong.sSoHopDong, hopdong.fTienHopDong, nhathau.sTenNhaThau, donvi.sTen as sDonViQuanLy, nns.sTen as sNguonVon, duan.sTenDuAn, duan.iID_ChuDauTuID, cdt.sTenCDT as sChuDauTu 
FROM VDT_TT_DeNghiThanhToan dntt
LEFT JOIN NS_DonVi donvi ON dntt.iID_MaDonViQuanLy = donvi.iID_MaDonVi
LEFT JOIN NS_NguonNganSach nns ON dntt.iID_NguonVonID = nns.iID_MaNguonNganSach
LEFT JOIN VDT_DA_DuAn duan ON dntt.iID_DuAnId = duan.iID_DuAnID
LEFT JOIN DM_ChuDauTu cdt ON duan.iID_ChuDauTuID = cdt.ID
LEFT JOIN VDT_DA_TT_HopDong hopdong ON dntt.iID_HopDongId = hopdong.iID_HopDongID
LEFT JOIN VDT_DM_NhaThau nhathau ON hopdong.iID_HopDongID = nhathau.iID_NhaThauID
WHERE dntt.iID_DeNghiThanhToanID = @iID_DeNghiThanhToanID;