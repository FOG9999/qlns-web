
DECLARE @sMaDuAn nvarchar(500)		set @sMaDuAn = null
DECLARE @sTenDuAn nvarchar(500)		set @sTenDuAn = null
DECLARE @iNamLamViec int set @iNamLamViec = 2022
--#DECLARE#--

/*

Lấy danh sách dự án

*/

SELECT duan.iID_DuAnID as IdRow,
		duan.iID_DuAnID,
		duan.iID_DonViThucHienDuAnID as iID_DonViQuanLyID,
		duan.iID_ChuDauTuID,
		duan.iID_DuAnKHTHDeXuatID,
		duan.sMaDuAn,
		duan.sTenDuAn,
		duan.sDiaDiem,
		duan.sKhoiCong,
		duan.sKetThuc,
		duan.fHanMucDauTu,
		duan.dDateCreate,
		(
			CASE
				WHEN dv.iID_MaDonVi is null THEN ''
				ELSE CONCAT(dv.iID_MaDonVi, ' - ', dv.sTen)
			END
		) as sTenDonViQL,
		(
			CASE
				WHEN dmcdt.sId_CDT is null THEN ''
				ELSE CONCAT(dmcdt.sId_CDT, ' - ', dmcdt.sTenCDT)
			END
		) as sTenCDT,
		'' as sTenLoaiCongTrinh,
		'' as sTenNganSach,
		0x0 as iID_LoaiCongTrinhID,
		null as iID_NguonVonID,
		0x0 as iID_DuAn_HangMucID,
		0x0 as iID_ParentID,
		tbl_count_chitiet.numChild,
		1 as iLevel,
		CAST((case when tbl_count_chitiet.numChild > 0 then '1' else '0' end) as bit) as bLaHangCha into #TEMP_DuAn
FROM VDT_DA_DuAn duan
LEFT JOIN NS_DonVi dv on duan.iID_DonViThucHienDuAnID = dv.iID_Ma 
LEFT JOIN DM_ChuDauTu dmcdt on duan.iID_ChuDauTuID = dmcdt.ID
LEFT JOIN (
	select iID_DuAnID, count(iID_DuAnID) as numChild
	from VDT_DA_DuAn_HangMuc
	where iID_DuAnID is not null
	GROUP BY iID_DuAnID
) tbl_count_chitiet on duan.iID_DuAnID = tbl_count_chitiet.iID_DuAnID
where 1 = 1;

SELECT hangmuc.iID_DuAn_HangMucID as IdRow,
		duan.iID_DuAnID,
		duan.iID_DonViThucHienDuAnID as iID_DonViQuanLyID,
		duan.iID_ChuDauTuID,
		duan.iID_DuAnKHTHDeXuatID,
		duan.sMaDuAn,
		duan.sTenDuAn,
		duan.sDiaDiem,
		duan.sKhoiCong,
		duan.sKetThuc,
		hangmuc.fHanMucDauTu,
		duan.dDateCreate,
		(
			CASE
				WHEN dv.iID_MaDonVi is null THEN ''
				ELSE CONCAT(dv.iID_MaDonVi, ' - ', dv.sTen)
			END
		) as sTenDonViQL,
		(
			CASE
				WHEN dmcdt.sId_CDT is null THEN ''
				ELSE CONCAT(dmcdt.sId_CDT, ' - ', dmcdt.sTenCDT)
			END
		) as sTenCDT,
		(
			CASE
				WHEN lct.sMaLoaiCongTrinh is null THEN ''
				ELSE CONCAT(lct.sMaLoaiCongTrinh, ' - ', lct.sTenLoaiCongTrinh)
			END
		) as sTenLoaiCongTrinh,
		(
			CASE
				WHEN nns.iID_MaNguonNganSach is null THEN ''
				ELSE CONCAT(nns.iID_MaNguonNganSach, ' - ', nns.sTen)
			END
		) as sTenNganSach,
		hangmuc.iID_LoaiCongTrinhID,
		hangmuc.iID_NguonVonID,
		hangmuc.iID_DuAn_HangMucID,
		duan.iID_DuAnID as iID_ParentID,
		0 as numChild,
		2 as iLevel,
		CAST('0' as bit) as bLaHangCha into #TEMP_ChiTiet
from VDT_DA_DuAn_HangMuc hangmuc
INNER JOIN VDT_DA_DuAn duan on hangmuc.iID_DuAnID = duan.iID_DuAnID
LEFT JOIN NS_DonVi dv on duan.iID_DonViThucHienDuAnID = dv.iID_Ma and dv.iNamLamViec_DonVi = @iNamLamViec
LEFT JOIN DM_ChuDauTu dmcdt on duan.iID_ChuDauTuID = dmcdt.ID and dmcdt.iNamLamViec = @iNamLamViec
LEFT JOIN VDT_DM_LoaiCongTrinh lct on hangmuc.iID_LoaiCongTrinhID = lct.iID_LoaiCongTrinh
LEFT JOIN NS_NguonNganSach nns on hangmuc.iID_NguonVonID = nns.iID_MaNguonNganSach
where 1 = 1
	AND hangmuc.iID_DuAn_HangMucID NOT IN (
		select dahm.iID_DuAn_HangMucID
		from VDT_DA_DuAn_HangMuc dahm
		INNER JOIN (
			select iID_DuAnID, iID_LoaiCongTrinhID, iID_NguonVonID from VDT_KHV_KeHoach5Nam_ChiTiet
		) as duAnDD on dahm.iID_DuAnID = duAnDD.iID_DuAnID 
						AND dahm.iID_LoaiCongTrinhID = duAnDD.iID_LoaiCongTrinhID
						AND dahm.iID_NguonVonID = duAnDD.iID_NguonVonID
	)
;

SELECT * INTO #TEMP_ALL FROM (
	select * from #TEMP_DuAN tmpDuAn 
	where (tmpDuAn.bLaHangCha = 0 or 
	(tmpDuAn.bLaHangCha = 1 AND EXISTS(select * FROM #TEMP_ChiTiet where(tmpDuAn.iID_DuAnID = #TEMP_ChiTiet.iID_DuAnID))))
	union all
	select * from #TEMP_ChiTiet
) as tempTable;


select *, 
	CAST('0' as bit) as isMap
from #TEMP_ALL
where 1 = 1 
	and (@sMaDuAn is null or sMaDuAn like @sMaDuAn)
	and (@sTenDuAn is null or sTenDuAn like @sTenDuAn)
ORDER BY dDateCreate DESC, iID_DuAnID, bLaHangCha DESC
;

DROP table #TEMP_DuAN;
DROP table #TEMP_ChiTiet;
DROP table #TEMP_ALL;

