

SELECT distinct da.iID_DuAnID,da.sTenDuAn ,da.sMaDuAn
FROM VDT_DA_DuAn da
inner join VDT_DA_QDDauTu dt ON dt.iID_DuAnID = da.iID_DuAnID
WHERE iID_DonViQuanLyID = @iID_DonViQuanLyID
AND dt.dNgayQuyetDinh <= @dNgayLap