DECLARE @iId uniqueidentifier set @iId= '257d8072-13eb-439a-a143-adeb00906893';
--DECLARE @iNamLamViec int set @iNamLamViec = 2020-->
DECLARE @listIdDuAn varchar(max) set @listIdDuAn = 'DB54D6E2-9B40-4ABB-843F-F7015E92EAF9,8563BD50-136F-4A63-BACB-8B4C7A92F29B '

--#DECLARE#--
SELECT iID_KeHoachUngID INTO #tmp  FROM VDT_KHV_KeHoachVonUng_DX_ChiTiet WHERE iID_KeHoachUngID = @iId


IF ((select count(*) from #tmp) <> 0)
	BEGIN
		SELECT khvuct.*, da.sMaDuAn as sMaDuAn, da.sTenDuAn as sTenDuAn, da.fTongMucDauTu as fTongMucDauTu
		FROM VDT_KHV_KeHoachVonUng_DX_ChiTiet khvuct
		INNER JOIN VDT_DA_DuAn da on da.iID_DuAnID = khvuct.iID_DuAnID
		WHERE khvuct.iID_KeHoachUngID = @iId	

	END
ELSE
	BEGIN
		SELECT da.*, da.sMaDuAn as sMaDuAn, da.sTenDuAn as sTenDuAn, da.fTongMucDauTu as fTongMucDauTu
		FROM  VDT_DA_DuAn da 
		WHERE da.iID_DuAnID in ((select * from dbo.f_split(@listIdDuAn)))
	END