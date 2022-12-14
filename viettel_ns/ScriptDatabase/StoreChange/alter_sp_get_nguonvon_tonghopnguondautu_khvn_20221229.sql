
ALTER PROC [dbo].[sp_get_nguonvon_tonghopnguondautu_khvn]
@iNamKeHoach int
AS
BEGIN
	SELECT tbl.iID_TongHop_NguonNSDauTuID, 
		tbl.iID_ChungTu, 
		tbl.iID_DuAnID ,
		tbl.sMaNguon ,
		tbl.sMaDich ,
		tbl.sMaNguonCha ,
		tbl.fGiaTri ,
		tbl.iStatus ,
		tbl.bIsLog ,
		tbl.iId_MaNguonCha ,
		tbl.iNamKeHoach, 
		tbl.iID_MucID as IIDMucID,
		tbl.iID_TieuMucID as IIDTieuMucID,
		tbl.iID_TietMucID as IIDTietMucID,
		tbl.iID_NganhID as IIDNganhID,
		tbl.iID_LoaiCongTrinh as IIDLoaiCongTrinh,
		SUM(ISNULL(dt.fGiaTri, 0)) as fDaChi INTO #tmp
	FROM VDT_TongHop_NguonNSDauTu as tbl
	LEFT JOIN VDT_TongHop_NguonNSDauTu as dt on tbl.iID_ChungTu = dt.iId_MaNguonCha
											AND dt.sMaDich in ('121a', '122a') AND dt.sMaNguon = '000' 
											AND dt.sMaTienTrinh = '200' AND dt.bIsLog = 0 
	WHERE tbl.sMaNguon in ('121a', '122a') 
		AND tbl.sMaDich = '000' 
		AND tbl.sMaTienTrinh = '200' 
		AND tbl.bIsLog = 0
		AND tbl.iNamKeHoach < @iNamKeHoach
	GROUP BY tbl.iID_TongHop_NguonNSDauTuID, 
		tbl.iID_ChungTu, tbl.iID_DuAnID ,tbl.sMaNguon ,tbl.sMaDich ,tbl.sMaNguonCha ,tbl.fGiaTri ,tbl.iStatus ,tbl.bIsLog ,tbl.iId_MaNguonCha ,tbl.iNamKeHoach,
		tbl.iID_MucID, tbl.iID_TieuMucID, tbl.iID_TietMucID, tbl.iID_NganhID, tbl.iID_LoaiCongTrinh

	SELECT tbl.iID_TongHop_NguonNSDauTuID, 
		tbl.iID_ChungTu, 
		tbl.iID_DuAnID ,
		tbl.sMaNguon ,
		tbl.sMaDich ,
		tbl.sMaNguonCha ,
		(ISNULL(tbl.fGiaTri, 0) - ISNULL(tbl.fDaChi, 0)) as fGiaTri,
		tbl.iStatus ,
		tbl.bIsLog ,
		tbl.iId_MaNguonCha ,
		tbl.iNamKeHoach,
		NULL as iThuHoiTUCheDo,
		tbl.IIDMucID,
		tbl.IIDTieuMucID,
		tbl.IIDTietMucID,
		tbl.IIDNganhID,
		tbl.IIDLoaiCongTrinh
	FROM #tmp as tbl
END
