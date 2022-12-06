DECLARE @iID_KeHoachUngID uniqueidentifier set @iID_KeHoachUngID = '90157aa8-e681-44a1-a0e2-ae8400f73da3'

--#DECLARE#--

/*

Lấy all khvu chi tiet theo don khvu id

*/

DECLARE @iIdKeHoachDeXuat uniqueidentifier = (SELECT iID_KeHoachVonUngDeXuatID FROM VDT_KHV_KeHoachVonUng WHERE Id = @iID_KeHoachUngID)

SELECT iID_DuAnID, SUM(ISNULL(fGiaTriDeNghi, 0)) as fGiaTriDeNghi INTO #tmpDeXuat 
FROM VDT_KHV_KeHoachVonUng_DX_ChiTiet WHERE iID_KeHoachUngID = @iIdKeHoachDeXuat
GROUP BY iID_DuAnID

SELECT ml.sXauNoiMa, dt.iID_DuAnID, dt.iID_MucID, dt.iID_TieuMucID, dt.iID_TietMucID, dt.iID_NganhID, 
	da.sTenDuAn, da.sMaDuAn, dt.sTrangThaiDuAnDangKy, dx.fGiaTriDeNghi, da.fTongMucDauTu, dt.fCapPhatTaiKhoBac, dt.fCapPhatBangLenhChi, dt.sGhiChu,
	ml.sLNS, ml.sL, ml.sK, ml.sM, ml.sTM, ml.sTTM, ml.sNG
FROM  VDT_KHV_KeHoachVonUng_ChiTiet as dt
INNER JOIN VDT_DA_DuAn as da on dt.iID_DuAnID = da.iID_DuAnID
LEFT JOIN #tmpDeXuat as dx on dt.iID_DuAnID = dx.iID_DuAnID
LEFT JOIN NS_MucLucNganSach as ml on dt.iID_MucID = ml.iID_MaMucLucNganSach
									OR dt.iID_TieuMucID = ml.iID_MaMucLucNganSach
									OR dt.iID_TietMucID = ml.iID_MaMucLucNganSach
									OR dt.iID_NganhID = ml.iID_MaMucLucNganSach
where dt.iID_KeHoachUngID = @iID_KeHoachUngID

DROP TABLE #tmpDeXuat