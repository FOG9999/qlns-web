DECLARE @iId uniqueidentifier set @iId= 'd0643a83-e9e4-44bd-83d2-d0cffa0f4173';
--DECLARE @iNamLamViec int set @iNamLamViec = 2020-->
DECLARE @listIdDuAn varchar(max) set @listIdDuAn = '471ccc4d-bd53-4a06-9b51-af7000e3e815'

--#DECLARE#--
SELECT iID_KeHoachUngID INTO #tmp  FROM VDT_KHV_KeHoachVonUng_DX_ChiTiet WHERE iID_KeHoachUngID = @iId


IF ((select count(*) from #tmp) <> 0)
	BEGIN
		SELECT hm.iID_DuAn_HangMucID, khvuct.Id,khvuct.iID_DuAnID,khvuct.iID_DonViQuanLyID,khvuct.iID_KeHoachUngID, khvuct.fGiaTriDeNghiDC,khvuct.fGiaTriDeNghi,
		da.sMaDuAn as sMaDuAn,
		--CASE WHEN hm.sTenHangMuc IS NULL THEN da.sTenDuAn ELSE hm.sTenHangMuc END as sTenDuAn,
		CASE WHEN hm.sTenHangMuc IS NULL THEN da.sTenDuAn ELSE hm.sTenHangMuc END  as sTenDuAn,
		da.fTongMucDauTu as fTongMucDauTu,
		khvuct.sGhiChu,
		hm.iID_LoaiCongTrinhID,
		hm.iID_DuAn_HangMucID

		FROM VDT_KHV_KeHoachVonUng_DX_ChiTiet khvuct
		INNER JOIN VDT_DA_DuAn da on da.iID_DuAnID = khvuct.iID_DuAnID 
		LEFT JOIN VDT_DA_DuAn_HangMuc hm on hm.iID_DuAnID = da.iID_DuAnID and hm.iID_LoaiCongTrinhID = khvuct.iID_LoaiCongTrinhID
		WHERE khvuct.iID_KeHoachUngID = @iId	
	END
ELSE
	BEGIN
		SELECT DISTINCT da.iID_DuAnID,da.sTrangThaiDuAn,da.iID_DonViQuanLyID,da.iID_ChuDauTuID, da.sMaDuAn as sMaDuAn, 
		CASE WHEN hm.sTenHangMuc IS NULL THEN da.sTenDuAn ELSE hm.sTenHangMuc END  as sTenDuAn, da.fTongMucDauTu as fTongMucDauTu,
		hm.iID_LoaiCongTrinhID,
		hm.iID_DuAn_HangMucID
		FROM  VDT_DA_DuAn da 
		LEFT JOIN VDT_DA_DuAn_HangMuc hm on hm.iID_DuAnID = da.iID_DuAnID
		WHERE da.iID_DuAnID in ((select * from dbo.f_split(@listIdDuAn)))
	END
DROP TABLE #tmp;





