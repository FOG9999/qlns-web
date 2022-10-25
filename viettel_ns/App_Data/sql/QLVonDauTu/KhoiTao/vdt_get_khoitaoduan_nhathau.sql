DECLARE @iID_KhoiTaoID uniqueidentifier set @iID_KhoiTaoID = '27bb48f7-7103-4581-a485-ad8d009b2d60'

--#DECLARE#--
SELECT nhathau.*, ct.iID_NguonVonId, ct.iID_NganhID FROM VDT_KT_KhoiTao_ChiTiet_NhaThau nhathau
INNER JOIN VDT_KT_KhoiTao_ChiTiet ct ON nhathau.iID_KhoiTao_ChiTietID = ct.iID_KhoiTao_ChiTietID
WHERE nhathau.iID_KhoiTaoID = @iID_KhoiTaoID
