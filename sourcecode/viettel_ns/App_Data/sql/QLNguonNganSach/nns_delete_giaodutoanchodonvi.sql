DELETE FROM NNS_DuToan WHERE iID_DuToan = @idDuToan;
DELETE FROM NNS_DuToanChiTiet WHERE iID_DuToan = @idDuToan
DELETE FROM NNS_DuToan_NS_DTBS_TLTH WHERE iID_DuToan = @idDuToan
DELETE FROM NNS_DuToan_DotNhan WHERE iID_DuToan = @idDuToan
DELETE FROM NNS_DuToan_NhiemVu WHERE iID_DuToan = @idDuToan