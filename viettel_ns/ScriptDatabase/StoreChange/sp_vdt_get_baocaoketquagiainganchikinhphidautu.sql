ALTER PROC [dbo].[sp_vdt_get_baocaoketquagiainganchikinhphidautu]
@iIdMaDonVi nvarchar(100),
@iIdNguonVon int,
@iNamKeHoach int
AS
BEGIN

	CREATE TABLE #tmp
	(
		STT INT IDENTITY(1,1),
		ID uniqueidentifier,
		sNoiDung nvarchar(max),
		fDuToanDonViDeNghiChuyenSangNamSau float,
		fDuToanDonViNamNay float,
		fTongCongDuToanNam float,
		fDuToanDuocThongBaoBQP float,
		fCucTaiChinhThanhToanTrucTiepBQP float,		
		fThanhToanBQP float,
		fTamUngBQP float, 
		fTongCongBQP float, 
		fDatTiLeBQP float, 
		fDuDoanDuocThongBaoKBNN float, 
		fThanhToanKBNN float, 
		fTamUngKBNN float, 
		fTongCongKBNN float, 
		fDatTiLeKBNN float, 
		depth int,
		IDParent uniqueidentifier,
		IsBold int 

	)


	SELECT DISTINCT iID_DuAnID INTO #tmpDuAn
	FROM VDT_KHV_KeHoachVonNam_DuocDuyet as tbl
	INNER JOIN VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet as dt on tbl.iID_KeHoachVonNam_DuocDuyetID = dt.iID_KeHoachVonNam_DuocDuyetID
	WHERE (@iIdMaDonVi = '00000000-0000-0000-0000-000000000000' or tbl.iID_DonViQuanLyID = @iIdMaDonVi) AND tbl.iID_NguonVonID = @iIdNguonVon AND tbl.iNamKeHoach = @iNamKeHoach AND tbl.bActive = 1
	
	-- Get du lieu tung cot
	BEGIN 
		SELECT da.iID_DuAnID,
			SUM((CASE WHEN sMaNguon in ('111', '112') THEN ISNULL(dt.fGiaTri, 0) ELSE 0 END)) as fDuToanDonViDeNghiChuyenSangNamSau,
			SUM((CASE WHEN sMaDich in ('111', '112') THEN ISNULL(dt.fGiaTri, 0) ELSE 0 END)) as fRevertDuToanDonViDeNghiChuyenSangNamSau,
			SUM((CASE WHEN sMaNguon in ('101', '102') THEN ISNULL(dt.fGiaTri, 0) ELSE 0 END)) as fDuToanDonViNamNay,
			SUM((CASE WHEN sMaDich in ('101', '102') THEN ISNULL(dt.fGiaTri, 0) ELSE 0 END)) as fRevertDuToanDonViNamNay,

			SUM((CASE WHEN sMaNguon in ('112', '102') THEN ISNULL(dt.fGiaTri, 0) ELSE 0 END)) as fDuToanDuocThongBaoBQP,
			SUM((CASE WHEN sMaDich in ('112', '102') THEN ISNULL(dt.fGiaTri, 0) ELSE 0 END)) as fRevertDuToanDuocThongBaoBQP,

			SUM((CASE WHEN sMaDich in ('202') AND sMaNguonCha NOT IN ('132','122') THEN ISNULL(dt.fGiaTri, 0) ELSE 0 END)) as fRevertThanhToanBQP,
			SUM((CASE WHEN sMaNguon in ('202') AND sMaNguonCha NOT IN ('132','122') THEN ISNULL(dt.fGiaTri, 0) ELSE 0 END)) as fThanhToanBQP,

			SUM((CASE WHEN sMaDich in ('212a') AND sMaNguonCha NOT IN ('132','122') THEN ISNULL(dt.fGiaTri, 0) ELSE 0 END)) as fRevertTamUngBQP,
			SUM((CASE WHEN sMaNguon in ('212b2') AND sMaNguonCha NOT IN ('132','122') THEN ISNULL(dt.fGiaTri, 0) ELSE 0 END)) as fTamUngBQP,
			SUM((CASE WHEN sMaNguon in ('212a') AND sMaNguonCha NOT IN ('132','122') THEN ISNULL(dt.fGiaTri, 0) ELSE 0 END)) as fTamUngBQP1,
			SUM((CASE WHEN sMaDich in ('212b2') AND sMaNguonCha NOT IN ('132','122') THEN ISNULL(dt.fGiaTri, 0) ELSE 0 END)) as fRevertTamUngBQP1,

			SUM((CASE WHEN sMaNguon in ('111','101') THEN ISNULL(dt.fGiaTri, 0) ELSE 0 END)) as fDuDoanDuocThongBaoKBNN,
			SUM((CASE WHEN sMaDich in ('111','101') THEN ISNULL(dt.fGiaTri, 0) ELSE 0 END)) as fRevertDuDoanDuocThongBaoKBNN,

			SUM((CASE WHEN sMaDich in ('201') AND sMaNguonCha NOT IN ('131','121') THEN ISNULL(dt.fGiaTri, 0) ELSE 0 END)) as fRevertThanhToanKBNN,
			SUM((CASE WHEN sMaNguon in ('201') AND sMaNguonCha NOT IN ('131','122') THEN ISNULL(dt.fGiaTri, 0) ELSE 0 END)) as fThanhToanKBNN, 

			SUM((CASE WHEN sMaDich in ('211a') AND sMaNguonCha NOT IN ('131','122') THEN ISNULL(dt.fGiaTri, 0) ELSE 0 END)) as fRevertTamUngKBNN,
			SUM((CASE WHEN sMaNguon in ('211b2') AND sMaNguonCha NOT IN ('131','122') THEN ISNULL(dt.fGiaTri, 0) ELSE 0 END)) as fTamUngKBNN,
			SUM((CASE WHEN sMaNguon in ('211a') AND sMaNguonCha NOT IN ('131','122') THEN ISNULL(dt.fGiaTri, 0) ELSE 0 END)) as fTamUngKBNN1,
			SUM((CASE WHEN sMaDich in ('211b2') AND sMaNguonCha NOT IN ('131','122') THEN ISNULL(dt.fGiaTri, 0) ELSE 0 END)) as fRevertTamUngKBNN1
			INTO #tmpGiaTri
		FROM #tmpDuAn as da
		INNER JOIN VDT_TongHop_NguonNSDauTu as dt on da.iID_DuAnID = dt.iID_DuAnID
		WHERE  dt.iID_NguonVonID = @iIdNguonVon AND dt.iNamKeHoach = @iNamKeHoach
		GROUP BY da.iID_DuAnID
	END


	-- Tong hop du lieu tung cot 
	BEGIN
		SELECT	
			dt.iID_DuAnID,
			da.sTenDuAn as sTen,
			--tbl.sTen as sTenDVQL,
			---Dự toán
			SUM(ISNULL(dt.fDuToanDonViDeNghiChuyenSangNamSau, 0) - ISNULL(dt.fRevertDuToanDonViDeNghiChuyenSangNamSau, 0)) as fDuToanDonViDeNghiChuyenSangNamSau,
			SUM(ISNULL(dt.fDuToanDonViNamNay, 0) - ISNULL(dt.fRevertDuToanDonViNamNay, 0)) as fDuToanDonViNamNay,
			(SUM(ISNULL(dt.fDuToanDonViDeNghiChuyenSangNamSau, 0) - ISNULL(dt.fRevertDuToanDonViDeNghiChuyenSangNamSau, 0)) + SUM(ISNULL(dt.fDuToanDonViNamNay, 0) - ISNULL(dt.fRevertDuToanDonViNamNay, 0)) ) as fTongCongDuToanNam,
			--Bộ quốc phòng
			SUM(ISNULL(fDuToanDuocThongBaoBQP, 0) - ISNULL(dt.fRevertDuToanDuocThongBaoBQP, 0)) as fDuToanDuocThongBaoBQP,
			0 as fCucTaiChinhThanhToanTrucTiepBQP,
			SUM(ISNULL(dt.fRevertThanhToanBQP, 0) - ISNULL(dt.fThanhToanBQP, 0)) as fThanhToanBQP,
			SUM((ISNULL(dt.fRevertTamUngBQP, 0) - ISNULL(dt.fTamUngBQP, 0)) - (ISNULL(dt.fTamUngBQP1, 0) - ISNULL(dt.fRevertTamUngBQP1, 0))) as fTamUngBQP,
			---fThanhToanBQP + fTamUngBQP
			(SUM(ISNULL(dt.fRevertThanhToanBQP, 0) - ISNULL(dt.fThanhToanBQP, 0)) + SUM((ISNULL(dt.fRevertTamUngBQP, 0) - ISNULL(dt.fTamUngBQP, 0)) - (ISNULL(dt.fTamUngBQP1, 0) - ISNULL(dt.fRevertTamUngBQP1, 0)))) as fTongCongBQP,
			---fTongCongBQP /fDuToanDuocThongBaoBQP
			((SUM(ISNULL(dt.fRevertThanhToanBQP, 0) - ISNULL(dt.fThanhToanBQP, 0)) + SUM((ISNULL(dt.fRevertTamUngBQP, 0) - ISNULL(dt.fTamUngBQP, 0)) - (ISNULL(dt.fTamUngBQP1, 0) - ISNULL(dt.fRevertTamUngBQP1, 0))))/ SUM(ISNULL(fDuToanDuocThongBaoBQP, 0) - ISNULL(dt.fRevertDuToanDuocThongBaoBQP, 0))) as fDatTiLeBQP,

			--Kho bạc nhà nước
			SUM(ISNULL(dt.fDuDoanDuocThongBaoKBNN, 0) - ISNULL(dt.fRevertDuDoanDuocThongBaoKBNN, 0)) as fDuDoanDuocThongBaoKBNN,
			SUM(ISNULL(dt.fRevertThanhToanKBNN, 0) - ISNULL(dt.fThanhToanKBNN, 0)) as fThanhToanKBNN,
			SUM((ISNULL(dt.fRevertTamUngKBNN, 0) - ISNULL(dt.fTamUngKBNN, 0)) - (ISNULL(dt.fTamUngKBNN1, 0) - ISNULL(dt.fRevertTamUngKBNN1, 0))) as fTamUngKBNN,
			---fThanhToanKBNN + fTamUngKBNN
			(SUM(ISNULL(dt.fRevertThanhToanKBNN, 0) - ISNULL(dt.fThanhToanKBNN, 0)) + SUM((ISNULL(dt.fRevertTamUngKBNN, 0) - ISNULL(dt.fTamUngKBNN, 0)) - (ISNULL(dt.fTamUngKBNN1, 0) - ISNULL(dt.fRevertTamUngKBNN1, 0)))) as fTongCongKBNN,
			---fTongCongKBNN/fDuDoanDuocThongBaoKBNN
			((SUM(ISNULL(dt.fRevertThanhToanKBNN, 0) - ISNULL(dt.fThanhToanKBNN, 0)) + SUM((ISNULL(dt.fRevertTamUngKBNN, 0) - ISNULL(dt.fTamUngKBNN, 0)) - (ISNULL(dt.fTamUngKBNN1, 0) - ISNULL(dt.fRevertTamUngKBNN1, 0)))) / SUM(ISNULL(dt.fDuDoanDuocThongBaoKBNN, 0) - ISNULL(dt.fRevertDuDoanDuocThongBaoKBNN, 0))) as fDatTiLeKBNN
		INTO #tmp_giatri_duan
		FROM #tmpGiaTri as dt
		INNER JOIN VDT_DA_DuAn as da on dt.iID_DuAnID = da.iID_DuAnID
		--INNER JOIN NS_DonVi AS tbl on da.iID_MaDonVi = tbl.iID_MaDonVi
		GROUP BY dt.iID_DuAnID, da.sTenDuAn
	END

	--insert dự án
	INSERT INTO #tmp
	( ID, sNoiDung, fDuToanDonViDeNghiChuyenSangNamSau, fDuToanDonViNamNay, fTongCongDuToanNam, fDuToanDuocThongBaoBQP, fCucTaiChinhThanhToanTrucTiepBQP, fThanhToanBQP, fTamUngBQP,fTongCongBQP,
		fDatTiLeBQP, fDuDoanDuocThongBaoKBNN, fThanhToanKBNN, fTamUngKBNN,	fTongCongKBNN, fDatTiLeKBNN, IDParent, IsBold)
	select tp.iID_DuAnID, tp.sTen, fDuToanDonViDeNghiChuyenSangNamSau, fDuToanDonViNamNay, fTongCongDuToanNam, fDuToanDuocThongBaoBQP, fCucTaiChinhThanhToanTrucTiepBQP, fThanhToanBQP, fTamUngBQP,fTongCongBQP,
		fDatTiLeBQP, fDuDoanDuocThongBaoKBNN, fThanhToanKBNN, fTamUngKBNN,	fTongCongKBNN, fDatTiLeKBNN, da.iID_LoaiCongTrinhID, 0
	from #tmp_giatri_duan as tp
	INNER JOIN VDT_DA_DuAn as da on tp.iID_DuAnID = da.iID_DuAnID

	--insert loại công trình
	INSERT INTO #tmp
	( ID, sNoiDung, fDuToanDonViDeNghiChuyenSangNamSau, fDuToanDonViNamNay, fTongCongDuToanNam, fDuToanDuocThongBaoBQP, fCucTaiChinhThanhToanTrucTiepBQP, fThanhToanBQP, fTamUngBQP,fTongCongBQP,
		fDatTiLeBQP, fDuDoanDuocThongBaoKBNN, fThanhToanKBNN, fTamUngKBNN,	fTongCongKBNN, fDatTiLeKBNN, IDParent, IsBold)
	select ct.iID_LoaiCongTrinh as ID, 
		ct.sTenLoaiCongTrinh as sNoiDung,
		SUM(fDuToanDonViDeNghiChuyenSangNamSau) as fDuToanDonViDeNghiChuyenSangNamSau,
		SUM(fDuToanDonViNamNay) as fDuToanDonViNamNay,
		SUM(fTongCongDuToanNam) as fTongCongDuToanNam,
		SUM(fDuToanDuocThongBaoBQP) as fDuToanDuocThongBaoBQP,
		SUM(fCucTaiChinhThanhToanTrucTiepBQP) as fCucTaiChinhThanhToanTrucTiepBQP,
		SUM(fThanhToanBQP) as fThanhToanBQP,
		SUM(fTamUngBQP) as fTamUngBQP,
		SUM(fTongCongBQP) as fTongCongBQP,
		SUM(fDatTiLeBQP) as fDatTiLeBQP,
		SUM(fDuDoanDuocThongBaoKBNN) as fDuDoanDuocThongBaoKBNN,
		SUM(fThanhToanKBNN) as fThanhToanKBNN,
		SUM(fTamUngKBNN) as fTamUngKBNN,
		SUM(fTongCongKBNN) as fTongCongKBNN,
		SUM(fDatTiLeKBNN) as fDatTiLeKBNN,
		NULL as IDParent,
		1 as IsBold
	from VDT_DM_LoaiCongTrinh as ct
	inner join #tmp as tp on ct.iID_LoaiCongTrinh = tp.IDParent
	group by ct.iID_LoaiCongTrinh, ct.sTenLoaiCongTrinh;

	
	WITH #Tree (STT, ID, sNoiDung, fDuToanDonViDeNghiChuyenSangNamSau, fDuToanDonViNamNay, fTongCongDuToanNam, fDuToanDuocThongBaoBQP, fCucTaiChinhThanhToanTrucTiepBQP, fThanhToanBQP, fTamUngBQP,fTongCongBQP,
					fDatTiLeBQP, fDuDoanDuocThongBaoKBNN, fThanhToanKBNN, fTamUngKBNN,	fTongCongKBNN, fDatTiLeKBNN, depth,  IDParent, location,IsBold, Position)
					as (
						select 
							STT,
							ID,
							sNoiDung,
							fDuToanDonViDeNghiChuyenSangNamSau,
							fDuToanDonViNamNay,
							fTongCongDuToanNam,
							fDuToanDuocThongBaoBQP,
							fCucTaiChinhThanhToanTrucTiepBQP,
							fThanhToanBQP,
							fTamUngBQP,
							fTongCongBQP,
							fDatTiLeBQP,
							fDuDoanDuocThongBaoKBNN,
							fThanhToanKBNN,
							fTamUngKBNN,
							fTongCongKBNN,
							fDatTiLeKBNN,
							0 as depth,
							IDParent,
							CAST(STT AS NVARCHAR(MAX)) AS location,
							1 as IsBold,
							CAST(ROW_NUMBER() OVER(ORDER BY temp.sNoiDung) AS NVARCHAR(MAX)) AS Position
					from #tmp as temp
					where IDParent is null
					UNION ALL
						select 
							child.STT,
							child.ID,
							child.sNoiDung,
							child.fDuToanDonViDeNghiChuyenSangNamSau,
							child.fDuToanDonViNamNay,
							child.fTongCongDuToanNam,
							child.fDuToanDuocThongBaoBQP,
							child.fCucTaiChinhThanhToanTrucTiepBQP,
							child.fThanhToanBQP,
							child.fTamUngBQP,
							child.fTongCongBQP,
							child.fDatTiLeBQP,
							child.fDuDoanDuocThongBaoKBNN,
							child.fThanhToanKBNN,
							child.fTamUngKBNN,
							child.fTongCongKBNN,
							child.fDatTiLeKBNN,
							parent.depth + 1,
							child.IDParent,
							CAST(CONCAT(parent.location, '.', child.STT) AS NVARCHAR(MAX)) AS location,
							child.IsBold,
							CONCAT(parent.Position,'.',CAST(ROW_NUMBER() OVER(ORDER BY child.sNoiDung) AS NVARCHAR(MAX))) AS Position
						from #tmp as child 
						inner join #Tree as parent on parent.ID = child.IDParent
					)

	select cast('/' + replace(Position, '.', '/') + '/' as hierarchyid) AS sort, * 
	from #Tree
	order by sort

	DROP TABLE #tmpGiaTri
	DROP TABLE #tmpDuAn
	DROP TABLE #tmp_giatri_duan
	DROP TABLE #tmp
END