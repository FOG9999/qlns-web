
--du toan duoc giao--
ALTER TABLE VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet
ADD iID_DuAn_HangMucID uniqueidentifier;


-- von ung de xuat
ALTER TABLE VDT_KHV_KeHoachVonUng_DX_ChiTiet
ADD iID_DuAn_HangMucID uniqueidentifier


ALTER TABLE VDT_KHV_KeHoachVonUng_DX_ChiTiet
ADD iID_LoaiCongTrinhID uniqueidentifier


-- von ung duoc duyet

ALTER TABLE VDT_KHV_KeHoachVonUng_ChiTiet
ADD iID_DuAn_HangMucID uniqueidentifier


ALTER TABLE VDT_KHV_KeHoachVonUng_ChiTiet
ADD iID_LoaiCongTrinhID uniqueidentifier