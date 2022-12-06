UPDATE NNS_DuToan SET rTongSoTien  = (
SELECT sum(SoTien) FROM NNS_DuToanChiTiet WHERE iID_DuToan = @iID_DuToan
) WHERE iID_DuToan = @iID_DuToan