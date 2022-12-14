USE [CTC_DB_TEST]
GO
/****** Object:  StoredProcedure [dbo].[proc_get_all_pheduyet_quyettoan_paging]    Script Date: 01/12/2022 10:31:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROC [dbo].[proc_get_all_pheduyet_quyettoan_paging]
@sSoQuyetDinh nvarchar(50),
@sTenDuAn nvarchar(50),
@dNgayQuyetDinhFrom datetime,
@dNgayQuyetDinhTo datetime,
@fTienQuyetToanPheDuyetFrom float,
@fTienQuyetToanPheDuyetTo float,
@CurrentPage int,
@ItemsPerPage int,
@iToTalItem int OUTPUT
AS
BEGIN

select qt.iID_QuyetToanID, qt.sSoQuyetDinh, qt.dNgayQuyetDinh,
			--qt.fTienQuyetToanPheDuyet,
			ISNULL((select SUM(fGiaTriQuyetToan) from VDT_QT_QuyetToan_ChiTiet where iID_QuyetToanID = qt.iID_QuyetToanID and (iID_ChiPhiId is not null or iID_GoiThauId is not null)), 0) as fTienQuyetToanPheDuyet,
			qt.iID_DuAnID,
			da.sMaDuAn,
			da.sTenDuAn,
			qt.dDateCreate,
			tblTongDauTu.fTongMucDauTuPheDuyet into #TEMP_quyettoan
from VDT_QT_QuyetToan qt
left join VDT_DA_DuAn da on qt.iID_DuAnID = da.iID_DuAnID
left join (
	select iID_DuAnID, ISNULL(sum(fTongMucDauTuPheDuyet), 0) as fTongMucDauTuPheDuyet 
	from VDT_DA_QDDauTu
	group by iID_DuAnID
) as tblTongDauTu on qt.iID_DuAnID = tblTongDauTu.iID_DuAnID
where 1 = 1 
	AND (ISNULL(@sSoQuyetDinh, '') = '' OR qt.sSoQuyetDinh LIKE CONCAT(N'%',@sSoQuyetDinh,N'%')) 
	AND (ISNULL(@sTenDuAn, '') = '' OR da.sTenDuAn LIKE CONCAT(N'%',@sTenDuAn,N'%')) 
	AND (@dNgayQuyetDinhFrom IS NULL OR qt.dNgayQuyetDinh >= @dNgayQuyetDinhFrom)
	AND (@dNgayQuyetDinhTo IS NULL OR qt.dNgayQuyetDinh <= @dNgayQuyetDinhTo);

SET @iToTalItem =  (SELECT COUNT(*) from #TEMP_quyettoan)

SELECT * from #TEMP_quyettoan
WHERE 1=1
	AND (@fTienQuyetToanPheDuyetFrom IS NULL OR #TEMP_quyettoan.fTienQuyetToanPheDuyet >= @fTienQuyetToanPheDuyetFrom)
	AND (@fTienQuyetToanPheDuyetTo IS NULL OR #TEMP_quyettoan.fTienQuyetToanPheDuyet <= @fTienQuyetToanPheDuyetTo)
ORDER BY dDateCreate desc
OFFSET (@ItemsPerPage * (@CurrentPage-1)) ROWS
FETCH NEXT @ItemsPerPage ROWS ONLY
END

