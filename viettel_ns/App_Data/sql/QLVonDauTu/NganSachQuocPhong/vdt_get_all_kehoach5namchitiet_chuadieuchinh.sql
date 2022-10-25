
select da.iID_DuAnID,da.sTenDuAn,
fGiaTriKeHoach
FROM VDT_KHV_KeHoach5Nam_ChiTiet ct
INNER JOIN VDT_DA_DuAn da ON da.iID_DuAnID = ct.iID_DuAnID
where ct.iID_KeHoach5NamID = @iID_KeHoach5NamID