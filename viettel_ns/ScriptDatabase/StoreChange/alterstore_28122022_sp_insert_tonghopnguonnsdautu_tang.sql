

ALTER  PROC [dbo].[sp_insert_tonghopnguonnsdautu_tang]
@sLoai nvarchar(100),
@iTypeExecute int,
@uIdQuyetDinh uniqueidentifier,
@iIDQuyetDinhOld uniqueidentifier
AS
BEGIN
	CREATE TABLE #lstMaNguon(sMaNguon nvarchar(100))

	DECLARE @RankDate DATETIME = (SELECT TOP(1) dNgayQuyetDinh FROM VDT_TongHop_NguonNSDauTu WHERE sMaNguon COLLATE DATABASE_DEFAULT = 'QUYET_TOAN' ORDER BY dNgayQuyetDinh DESC)

	IF(@sLoai = 'KHVN')
	BEGIN
		INSERT INTO #lstMaNguon(sMaNguon)
		VALUES('KHVN'), ('101'), ('102')
	END
	ELSE IF(@sLoai = 'KHVU')
	BEGIN
		INSERT INTO #lstMaNguon(sMaNguon)
		VALUES('KHVU'), ('121a'), ('122a')
	END
	ELSE IF(@sLoai = 'QUYET_TOAN')
	BEGIN
		INSERT INTO #lstMaNguon(sMaNguon)
		VALUES('QUYET_TOAN'), ('131'), ('132'), ('211c'), ('212c'), ('301'), ('302'), ('321a'), ('322a')
			, ('000'), ('321b'), ('322b')
	END

	IF(@iTypeExecute in (2,3,4))
	BEGIN 
		IF (@iTypeExecute in (2,3))
		BEGIN
			-- dao nguoc but toan
			INSERT INTO VDT_TongHop_NguonNSDauTu(iID_DuAnID, iID_MaDonViQuanLy, iID_NguonVonID, sSoQuyetDinh, dNgayQuyetDinh, iNamKeHoach, iID_ChungTu, sMaNguon, sMaDich, sMaNguonCha, fGiaTri, bIsLog, iStatus, sMaTienTrinh,
												iID_MucID, iID_TieuMucID, iID_TietMucID, iID_NganhID, LNS, L, K, M, TM, TTM, NG, iID_LoaiCongTrinh)
			--OUTPUT inserted.Id, inserted.sMaDich, inserted.iID_DuAnID, ISNULL(inserted.fGiaTri, 0) INTO #tmp(iId, sMaNguon, iIdDuAnId, fDaThanhToan)
			SELECT tbl.iID_DuAnID, iID_MaDonViQuanLy, iID_NguonVonID, sSoQuyetDinh, dNgayQuyetDinh, iNamKeHoach, iID_ChungTu, tbl.sMaDich, tbl.sMaNguon, tbl.sMaNguonCha, fGiaTri, 1, 2, '300',
				iID_MucID, iID_TieuMucID, iID_TietMucID, iID_NganhID, LNS, L, K, M, TM, TTM, NG, tbl.iID_LoaiCongTrinh
			FROM VDT_TongHop_NguonNSDauTu as tbl
			INNER JOIN #lstMaNguon as dt on tbl.sMaNguon COLLATE DATABASE_DEFAULT = dt.sMaNguon COLLATE DATABASE_DEFAULT
			WHERE tbl.iID_ChungTu = @uIdQuyetDinh 
				AND bIsLog = 0 AND sMaTienTrinh = (CASE WHEN @sLoai = 'QUYET_TOAN' THEN '100' ELSE '200' END)

			-- khoa but toan da update
			UPDATE tbl 
			SET 
				bIsLog = 1
			FROM VDT_TongHop_NguonNSDauTu as tbl
			INNER JOIN #lstMaNguon as dt on tbl.sMaNguon COLLATE DATABASE_DEFAULT = dt.sMaNguon COLLATE DATABASE_DEFAULT
			WHERE tbl.iID_ChungTu = @uIdQuyetDinh
				AND bIsLog = 0 AND sMaTienTrinh in ('100', '200')
		END
		ELSE IF (@iTypeExecute = 4)
		BEGIN
			-- dao nguoc but toan
			INSERT INTO VDT_TongHop_NguonNSDauTu(iID_DuAnID, iID_MaDonViQuanLy, iID_NguonVonID, sSoQuyetDinh, dNgayQuyetDinh, iNamKeHoach, iID_ChungTu, sMaNguon, sMaDich, sMaNguonCha, fGiaTri, bIsLog, iStatus, sMaTienTrinh,
												iID_MucID, iID_TieuMucID, iID_TietMucID, iID_NganhID, LNS, L, K, M, TM, TTM, NG, iID_LoaiCongTrinh)
			--OUTPUT inserted.Id, inserted.sMaDich, inserted.iID_DuAnID, ISNULL(inserted.fGiaTri, 0) INTO #tmp(iId, sMaNguon, iIdDuAnId, fDaThanhToan)
			SELECT tbl.iID_DuAnID, iID_MaDonViQuanLy, iID_NguonVonID, sSoQuyetDinh, dNgayQuyetDinh, iNamKeHoach, iID_ChungTu, tbl.sMaDich, tbl.sMaNguon, tbl.sMaNguonCha, fGiaTri, 1, 2, '300',
					iID_MucID, iID_TieuMucID, iID_TietMucID, iID_NganhID, LNS, L, K, M, TM, TTM, NG, iID_LoaiCongTrinh
			FROM VDT_TongHop_NguonNSDauTu as tbl
			INNER JOIN #lstMaNguon as dt on tbl.sMaNguon COLLATE DATABASE_DEFAULT = dt.sMaNguon COLLATE DATABASE_DEFAULT
			WHERE tbl.iID_ChungTu = @iIDQuyetDinhOld 
				AND bIsLog = 0 AND sMaTienTrinh in ('100', '200')

			---- khoa but toan da update
			UPDATE tbl 
			SET 
				bIsLog = 1
			FROM VDT_TongHop_NguonNSDauTu as tbl
			INNER JOIN #lstMaNguon as dt on tbl.sMaNguon COLLATE DATABASE_DEFAULT = dt.sMaNguon COLLATE DATABASE_DEFAULT
			WHERE tbl.iID_ChungTu = @iIDQuyetDinhOld
				AND bIsLog = 0 AND sMaTienTrinh in ('100', '200')
			--return;
		END

		-- deleted thi khong xu ly nua
		IF(@iTypeExecute = 3)
		BEGIN
			RETURN
		END
	END

	IF(@sLoai = 'KHVN')
	BEGIN
	   select 'xxxxxxx'
		--- insert but toan moi vao 
		INSERT INTO VDT_TongHop_NguonNSDauTu(iID_DuAnID, iID_MaDonViQuanLy, iID_NguonVonID, sSoQuyetDinh, dNgayQuyetDinh, iNamKeHoach, iID_ChungTu, sMaNguon, sMaDich, fGiaTri, iStatus, sMaTienTrinh,
											iID_MucID, iID_TieuMucID, iID_TietMucID, iID_NganhID, LNS, L, K, M, TM, TTM, NG, iID_LoaiCongTrinh)
		SELECT dt.iID_DuAnID, tbl.iID_MaDonViQuanLy, tbl.iID_NguonVonID,
				tbl.sSoQuyetDinh, tbl.dNgayQuyetDinh, tbl.iNamKeHoach, tbl.iID_KeHoachVonNam_DuocDuyetID , dt.sMaNguon, '000', SUM(ISNULL(dt.fGiaTri, 0)), 0, '200',
				iID_MucID, iID_TieuMucID, iID_TietMucID, iID_NganhID, ml.sLNS, ml.sL, ml.sK, ml.sM, ml.sTM, ml.sTTM, ml.sNG, iID_LoaiCongTrinh
		FROM VDT_KHV_KeHoachVonNam_DuocDuyet as tbl
		INNER JOIN (select iID_LoaiCongTrinh, iID_KeHoachVonNam_DuocDuyet_ChiTietID ,iID_KeHoachVonNam_DuocDuyetID,iID_DuAnID, iID_MucID, iID_TieuMucID, iID_TietMucID, iID_NganhID, dt.fGiaTri, 
					(CASE colName WHEN 'fCapPhatTaiKhoBac' THEN '101' WHEN 'fCapPhatBangLenhChi' THEN '102' END) as sMaNguon
					from 
					(select iID_LoaiCongTrinh, iID_KeHoachVonNam_DuocDuyet_ChiTietID ,iID_KeHoachVonNam_DuocDuyetID,iID_DuAnID, iID_MucID, iID_TieuMucID, iID_TietMucID, iID_NganhID , fCapPhatTaiKhoBac,fCapPhatBangLenhChi from VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet) as x
					UNPIVOT
					(fGiaTri FOR colName IN (fCapPhatTaiKhoBac,fCapPhatBangLenhChi)) as dt) as dt on dt.iID_KeHoachVonNam_DuocDuyetID = tbl.iID_KeHoachVonNam_DuocDuyetID
		LEFT JOIN NS_MucLucNganSach as ml on (dt.iID_MucID = ml.iID_MaMucLucNganSach OR dt.iID_TieuMucID = ml.iID_MaMucLucNganSach OR dt.iID_TietMucID = ml.iID_MaMucLucNganSach OR dt.iID_NganhID = ml.iID_MaMucLucNganSach)
		WHERE tbl.iID_KeHoachVonNam_DuocDuyetID = @uIdQuyetDinh
		GROUP BY dt.iID_DuAnID, tbl.iID_MaDonViQuanLy, tbl.iID_NguonVonID, tbl.sSoQuyetDinh, tbl.dNgayQuyetDinh, tbl.iNamKeHoach, tbl.iID_KeHoachVonNam_DuocDuyetID , dt.sMaNguon,
				iID_MucID, iID_TieuMucID, iID_TietMucID, iID_NganhID, ml.sLNS, ml.sL, ml.sK, ml.sM, ml.sTM, ml.sTTM, ml.sNG, iID_LoaiCongTrinh
	END
	ELSE IF(@sLoai = 'KHVU')
	BEGIN
		-- insert but toan moi vao 
		INSERT INTO VDT_TongHop_NguonNSDauTu(iID_DuAnID, iID_MaDonViQuanLy, iID_NguonVonID, sSoQuyetDinh, dNgayQuyetDinh, iNamKeHoach, iID_ChungTu, sMaNguon, sMaDich, fGiaTri, iStatus, sMaTienTrinh,
											iID_MucID, iID_TieuMucID, iID_TietMucID, iID_NganhID, LNS, L, K, M, TM, TTM, NG, iID_LoaiCongTrinh)
		SELECT dt.iID_DuAnID, tbl.iID_MaDonViQuanLy, tbl.iID_NguonVonID,
				tbl.sSoQuyetDinh, tbl.dNgayQuyetDinh, tbl.iNamKeHoach, tbl.Id , dt.sMaNguon , '000', SUM(ISNULL(dt.fGiaTri, 0)), 0, '200',
				iID_MucID, iID_TieuMucID, iID_TietMucID, iID_NganhID, ml.sLNS, ml.sL, ml.sK, ml.sM, ml.sTM, ml.sTTM, ml.sNG, iID_LoaiCongTrinhID
		FROM VDT_KHV_KeHoachVonUng as tbl
		INNER JOIN (select iID_LoaiCongTrinhID,Id,dt.iID_KeHoachUngID,iID_DuAnID, iID_MucID, iID_TieuMucID, iID_TietMucID, iID_NganhID, dt.fGiaTri, 
					(CASE colName WHEN 'fCapPhatTaiKhoBac' THEN '121a' WHEN 'fCapPhatBangLenhChi' THEN '122a' END) as sMaNguon
					from 
					(select iID_LoaiCongTrinhID, Id,iID_KeHoachUngID,iID_DuAnID, iID_MucID, iID_TieuMucID, iID_TietMucID, iID_NganhID, fCapPhatTaiKhoBac,fCapPhatBangLenhChi from VDT_KHV_KeHoachVonUng_ChiTiet) as tbl
					UNPIVOT
					(fGiaTri FOR colName IN (fCapPhatTaiKhoBac,fCapPhatBangLenhChi)) as dt) as dt on dt.iID_KeHoachUngID = tbl.Id
		LEFT JOIN NS_MucLucNganSach as ml on (dt.iID_MucID = ml.iID_MaMucLucNganSach OR dt.iID_TieuMucID = ml.iID_MaMucLucNganSach OR dt.iID_TietMucID = ml.iID_MaMucLucNganSach OR dt.iID_NganhID = ml.iID_MaMucLucNganSach)
		WHERE tbl.Id = @uIdQuyetDinh
		GROUP BY dt.iID_DuAnID, tbl.iID_MaDonViQuanLy, tbl.iID_NguonVonID, tbl.sSoQuyetDinh, tbl.dNgayQuyetDinh, tbl.iNamKeHoach, tbl.Id , dt.sMaNguon,
			iID_MucID, iID_TieuMucID, iID_TietMucID, iID_NganhID, ml.sLNS, ml.sL, ml.sK, ml.sM, ml.sTM, ml.sTTM, ml.sNG, iID_LoaiCongTrinhID
	END
	ELSE IF(@sLoai = 'QUYET_TOAN')
	BEGIN
		-- insert but toan moi vao 
		INSERT INTO VDT_TongHop_NguonNSDauTu(iID_DuAnID, iID_MaDonViQuanLy, iID_NguonVonID, sSoQuyetDinh, dNgayQuyetDinh, iNamKeHoach, iID_ChungTu, sMaNguon, sMaDich, fGiaTri, iStatus, sMaTienTrinh, iID_LoaiCongTrinh)
		SELECT tbl.iID_DuAnID, tbl.iID_MaDonViQuanLy, tbl.iID_NguonVonID, tbl.sSoDeNghi, tbl.dNgayDeNghi, 
			(CASE WHEN tbl.sMaNguon = '211c' OR tbl.sMaNguon = '212c' THEN tbl.iNamKeHoach ELSE tbl.iNamKeHoach + 1 END) as iNamKeHoach
			, tbl.iID_ChungTu, tbl.sMaNguon, tbl.sMaDich, tbl.fGiaTri, tbl.iStatus, '100', iID_LoaiCongTrinh
		FROM 
		(SELECT dt.iID_DuAnID, 
				tbl.iID_MaDonViQuanLy, 
				tbl.iID_NguonVonID,
				tbl.sSoDeNghi, 
				tbl.dNgayDeNghi, 
				tbl.iNamKeHoach as iNamKeHoach, 
				tbl.iID_BCQuyetToanNienDoID as iID_ChungTu, 
				(CASE 
					WHEN colName = 'fGiaTriTamUngDieuChinhGiam' AND tbl.iCoQuanThanhToan = 1 THEN '211c'
					WHEN colName = 'fGiaTriTamUngDieuChinhGiam' AND tbl.iCoQuanThanhToan = 2 THEN '212c'
				END) as sMaNguon, 
				'000' as sMaDich, 
				SUM(ISNULL(dt.fGiaTri, 0)) as fGiaTri, 
				SUM(ISNULL(dt.fGiaTri, 0)) as fSoDu, 
				0 as iStatus,
				dt.iID_LoaiCongTrinh
		FROM VDT_QT_BCQuyetToanNienDo as tbl
		INNER JOIN (select iID_LoaiCongTrinh, iID_BCQuyetToanNienDoChiTiet01ID ,iID_BCQuyetToanNienDo,iID_DuAnID, dt.fGiaTri, colName
					from 
					(select iID_LoaiCongTrinh, iID_BCQuyetToanNienDoChiTiet01ID ,iID_BCQuyetToanNienDo,iID_DuAnID, fGiaTriTamUngDieuChinhGiam from VDT_QT_BCQuyetToanNienDo_ChiTiet_01) as tbl
					UNPIVOT
					(fGiaTri FOR colName IN (fGiaTriTamUngDieuChinhGiam)) as dt) as dt on dt.iID_BCQuyetToanNienDo = tbl.iID_BCQuyetToanNienDoID
		WHERE tbl.iID_BCQuyetToanNienDoID = @uIdQuyetDinh
		GROUP BY dt.iID_DuAnID, tbl.iID_MaDonViQuanLy, tbl.iID_NguonVonID, tbl.sSoDeNghi, tbl.dNgayDeNghi, tbl.iNamKeHoach, tbl.iID_BCQuyetToanNienDoID , dt.colName, tbl.iCoQuanThanhToan, dt.iID_LoaiCongTrinh
		) as tbl


		INSERT INTO VDT_TongHop_NguonNSDauTu(iID_DuAnID, iID_MaDonViQuanLy, iID_NguonVonID, sSoQuyetDinh, dNgayQuyetDinh, iNamKeHoach, iID_ChungTu, sMaNguon, sMaDich, fGiaTri, iStatus, sMaTienTrinh, iID_LoaiCongTrinh)
		SELECT dt.iID_DuAnID, tbl.iID_MaDonViQuanLy, tbl.iID_NguonVonID, tbl.sSoDeNghi, tbl.dNgayDeNghi, (tbl.iNamKeHoach + 1), tbl.iID_BCQuyetToanNienDoID, 
			(CASE WHEN iCoQuanThanhToan = 1 THEN '131' ELSE '132' END), '000', 
			SUM(ISNULL(dt.fKHUngTrcChuaThuHoiTrcNamQuyetToan, 0) - ISNULL(dt.fThuHoiUngTruoc, 0) + ISNULL(dt.fKHUngNamNay, 0) - 
				(ISNULL(dt.fLKThanhToanDenTrcNamQuyetToan_KHUng, 0) - ISNULL(dt.fGiaTriThuHoiTheoGiaiNganThucTe, 0) + ISNULL(dt.fThanhToan_KHUngNamTrcChuyenSang, 0) + ISNULL(dt.fThanhToan_KHUngNamNay, 0))), 
			2, '100', dt.iID_LoaiCongTrinh
		FROM VDT_QT_BCQuyetToanNienDo as tbl
		INNER JOIN VDT_QT_BCQuyetToanNienDo_ChiTiet_01 as dt on tbl.iID_BCQuyetToanNienDoID = dt.iID_BCQuyetToanNienDo
		WHERE tbl.iID_BCQuyetToanNienDoID = @uIdQuyetDinh AND iLoaiThanhToan = 2
		GROUP BY dt.iID_DuAnID, tbl.iID_MaDonViQuanLy, tbl.iID_NguonVonID, tbl.sSoDeNghi, tbl.dNgayDeNghi, tbl.iNamKeHoach, tbl.iID_BCQuyetToanNienDoID, tbl.iCoQuanThanhToan, dt.iID_LoaiCongTrinh
	END
END
