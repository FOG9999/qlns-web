declare @iID_KeHoachVonNam_DuocDuyetID uniqueidentifier set @iID_KeHoachVonNam_DuocDuyetID = '241e8cb5-dc96-4601-8187-ae47011a739a'
--#DECLARE#--

UPDATE VDT_KHV_KeHoachVonNam_DuocDuyet
SET fCapPhatTaiKhoBac = (
	select SUM(ISNULL(ctct.fCapPhatTaiKhoBac, 0))
	from VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet ctct
	where ctct.iID_Parent is null
		and ctct.iID_KeHoachVonNam_DuocDuyetID = @iID_KeHoachVonNam_DuocDuyetID
	GROUP BY ctct.iID_KeHoachVonNam_DuocDuyetID
),
fCapPhatBangLenhChi = (
	select SUM(ISNULL(ctct.fCapPhatBangLenhChi, 0))
	from VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet ctct
	where ctct.iID_Parent is null
		and ctct.iID_KeHoachVonNam_DuocDuyetID = @iID_KeHoachVonNam_DuocDuyetID
	GROUP BY ctct.iID_KeHoachVonNam_DuocDuyetID
)
WHERE iID_KeHoachVonNam_DuocDuyetID = @iID_KeHoachVonNam_DuocDuyetID;