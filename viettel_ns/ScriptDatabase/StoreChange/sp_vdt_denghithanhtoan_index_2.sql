USE [CTC_DB_TEST]
GO
/****** Object:  StoredProcedure [dbo].[sp_vdt_denghithanhtoan_index_2]    Script Date: 17/11/2022 4:30:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROC [dbo].[sp_vdt_denghithanhtoan_index_2]
	@dNgayDeNghiFrom datetime, @dNgayDeNghiTo datetime,
	@iNamKeHoach int,
	@sSoDeNghi nvarchar(50),
	@YearOfWork int,
	@UserName nvarchar(100),
	@iidDuAn uniqueidentifier,
	@iLoaiThanhToan int,
	@sMaDonVi nvarchar(100),
	@CurrentPage INT,
	@ItemsPerPage INT,
	@isXuatDanhSach BIT,
	@iToTalItem INT OUTPUT 
AS
BEGIN

	DECLARE @CountDonViCha int;
	SELECT 
		@CountDonViCha = count(*) FROM (SELECT * FROM NS_NguoiDung_DonVi WHERE sMaNguoiDung = @UserName AND iNamLamViec = @YearOfWork AND iTrangThai = 1) nddv
	INNER JOIN
		(SELECT * FROM ns_donvi WHERE iNamLamViec_DonVi = @YearOfWork AND iTrangThai = 1 and iCap = 1) dv
	ON dv.iID_MaDonVi = nddv.iID_MaDonVi;

	SELECT DISTINCT iID_DeNghiThanhToanID, tbl.iLoai,
		(CASE WHEN khvn.iID_PhanBoVonID IS NOT NULL THEN khvn.sSoQuyetDinh
			WHEN khvu.Id IS NOT NULL THEN khvu.sSoQuyetDinh
			WHEN qt.iID_BCQuyetToanNienDoID IS NOT NULL THEN qt.sSoDeNghi
		END) as sSoQuyetDinh INTO #tmp
	FROM VDT_TT_DeNghiThanhToan_KHV as tbl
	LEFT JOIN VDT_KHV_PhanBoVon as khvn on tbl.iID_KeHoachVonID = khvn.iID_PhanBoVonID AND tbl.iLoai = 1
	LEFT JOIN VDT_KHV_KeHoachVonUng as khvu on tbl.iID_KeHoachVonID = khvu.Id AND tbl.iLoai = 2
	LEFT JOIN VDT_QT_BCQuyetToanNienDo as qt on tbl.iID_KeHoachVonID = qt.iID_BCQuyetToanNienDoID AND tbl.iLoai in (3,4)


	SELECT 
	  iID_DeNghiThanhToanID,
	  STUFF((
		SELECT '; ' + sSoQuyetDinh
		FROM #tmp 
		WHERE (iID_DeNghiThanhToanID = Results.iID_DeNghiThanhToanID) 
		FOR XML PATH(''),TYPE).value('(./text())[1]','VARCHAR(MAX)')
	  ,1,2,'') AS sKeHoachVon ,
	  STUFF((
		SELECT '; ' + CAST(iLoai as nvarchar(5))
		FROM #tmp 
		WHERE (iID_DeNghiThanhToanID = Results.iID_DeNghiThanhToanID) 
		FOR XML PATH(''),TYPE).value('(./text())[1]','VARCHAR(MAX)')
	  ,1,2,'') AS sLoaiKeHoachVon
	  INTO #tmpKhv
	FROM #tmp Results
	GROUP BY iID_DeNghiThanhToanID

	IF @isXuatDanhSach=0
	BEGIN
		SELECT tbl.*, ns.sTen as sNguonVon, lnv.sMoTa as sLoaiNguonVon, dv.sTen as sTenDonVi, 
			da.sTenDuAn, hd.sSoHopDong, hd.dNgayHopDong, hd.fTienHopDong as fGiaTriHopDong, nt.sMaNhaThau, da.sMaDuAn, khv.sKeHoachVon, khv.sLoaiKeHoachVon, tbl.iID_ChiPhiID as IIdChiPhiId,
			hd.sTenHopDong, pdtt.fGiaTriThanhToanTNDuocDuyet, pdtt.fGiaTriThanhToanNNDuocDuyet
		into #res1
		FROM VDT_TT_DeNghiThanhToan as tbl
		left join 
		(
		select iID_DeNghiThanhToanID,sum(pdtt.fGiaTriThanhToanTN) as fGiaTriThanhToanTNDuocDuyet, sum(pdtt.fGiaTriThanhToanNN) as fGiaTriThanhToanNNDuocDuyet
		from VDT_TT_PheDuyetThanhToan_ChiTiet pdtt
		group by iID_DeNghiThanhToanID
		)
		pdtt on tbl.iID_DeNghiThanhToanID = pdtt.iID_DeNghiThanhToanID
		LEFT JOIN NS_NguonNganSach as ns on tbl.iID_NguonVonID = ns.iID_MaNguonNganSach
		LEFT JOIN NS_MucLucNganSach as lnv on tbl.iID_LoaiNguonVonID = lnv.iID_MaMucLucNganSach
		LEFT JOIN NS_DonVi as dv on tbl.iID_DonViQuanLyID = dv.iID_Ma
		LEFT JOIN VDT_DA_DuAn as da on tbl.iID_DuAnId = da.iID_DuAnID
		LEFT JOIN VDT_DA_TT_HopDong as hd on tbl.iID_HopDongId = hd.iID_HopDongID
		LEFT JOIN VDT_DM_NhaThau as nt on tbl.iID_NhaThauId = nt.iID_NhaThauID
		LEFT JOIN #tmpKhv as khv on tbl.iID_DeNghiThanhToanID = khv.iID_DeNghiThanhToanID
		WHERE 
		(
			( EXISTS (SELECT iID_MaDonViQuanLy as iID_MaDonVi from VDT_TT_DeNghiThanhToan INTERSECT SELECT iID_MaDonVi FROM NS_NguoiDung_DonVi  WHERE sMaNguoiDung = @UserName and iNamLamViec = @YearOfWork)
				AND (@CountDonViCha = 0)
			)
			OR (@CountDonViCha <> 0 AND tbl.bKhoa = 1)
			OR 
			(   EXISTS (SELECT * from f_split(tbl.iID_MaDonViQuanLy) INTERSECT SELECT iID_MaDonVi FROM NS_NguoiDung_DonVi  WHERE sMaNguoiDung = @UserName and iNamLamViec = @YearOfWork)
				AND (@CountDonViCha <> 0)
			)
		) and (tbl.bTongHop is null or tbl.bTongHop != 1)
		AND (@dNgayDeNghiFrom IS NULL OR tbl.dNgayDeNghi >= @dNgayDeNghiFrom)
		AND (@dNgayDeNghiTo IS NULL OR tbl.dNgayDeNghi <= @dNgayDeNghiTo)
		AND (@iNamKeHoach IS NULL OR tbl.iNamKeHoach = @iNamKeHoach)
		AND (@sSoDeNghi IS NULL OR tbl.sSoDeNghi like CONCAT('%', @sSoDeNghi, '%'))
		and (@iidDuAn is null or tbl.iID_DuAnId = @iidDuAn)
		and (@sMaDonVi is null or tbl.iID_MaDonViQuanLy = @sMaDonVi)
		and (@iLoaiThanhToan is null or tbl.iLoaiThanhToan = @iLoaiThanhToan)

		ORDER BY tbl.dDateCreate DESC
		OFFSET ( @ItemsPerPage * ( @CurrentPage - 1 ) ) ROWS FETCH NEXT @ItemsPerPage ROWS ONLY 
	END

	ELSE IF @isXuatDanhSach=1
	BEGIN
		SELECT 
			ROW_NUMBER() OVER(ORDER BY tbl.iID_DeNghiThanhToanID DESC) as iSTT,
			tbl.*, ns.sTen as sNguonVon, lnv.sMoTa as sLoaiNguonVon, dv.sTen as sTenDonVi, 
			da.sTenDuAn, hd.sSoHopDong, hd.dNgayHopDong, hd.fTienHopDong as fGiaTriHopDong, nt.sMaNhaThau, da.sMaDuAn, khv.sKeHoachVon, khv.sLoaiKeHoachVon, tbl.iID_ChiPhiID as IIdChiPhiId,
			hd.sTenHopDong, pdtt.fGiaTriThanhToanTNDuocDuyet, pdtt.fGiaTriThanhToanNNDuocDuyet
		into #res2
		FROM VDT_TT_DeNghiThanhToan as tbl
		left join 
		(
		select iID_DeNghiThanhToanID,sum(pdtt.fGiaTriThanhToanTN) as fGiaTriThanhToanTNDuocDuyet, sum(pdtt.fGiaTriThanhToanNN) as fGiaTriThanhToanNNDuocDuyet
		from VDT_TT_PheDuyetThanhToan_ChiTiet pdtt
		group by iID_DeNghiThanhToanID
		)
		pdtt on tbl.iID_DeNghiThanhToanID = pdtt.iID_DeNghiThanhToanID
		LEFT JOIN NS_NguonNganSach as ns on tbl.iID_NguonVonID = ns.iID_MaNguonNganSach
		LEFT JOIN NS_MucLucNganSach as lnv on tbl.iID_LoaiNguonVonID = lnv.iID_MaMucLucNganSach
		LEFT JOIN NS_DonVi as dv on tbl.iID_DonViQuanLyID = dv.iID_Ma
		LEFT JOIN VDT_DA_DuAn as da on tbl.iID_DuAnId = da.iID_DuAnID
		LEFT JOIN VDT_DA_TT_HopDong as hd on tbl.iID_HopDongId = hd.iID_HopDongID
		LEFT JOIN VDT_DM_NhaThau as nt on tbl.iID_NhaThauId = nt.iID_NhaThauID
		LEFT JOIN #tmpKhv as khv on tbl.iID_DeNghiThanhToanID = khv.iID_DeNghiThanhToanID
		WHERE 
		(
			( EXISTS (SELECT iID_MaDonViQuanLy as iID_MaDonVi from VDT_TT_DeNghiThanhToan INTERSECT SELECT iID_MaDonVi FROM NS_NguoiDung_DonVi  WHERE sMaNguoiDung = @UserName and iNamLamViec = @YearOfWork)
				AND (@CountDonViCha = 0)
			)
			OR (@CountDonViCha <> 0 AND tbl.bKhoa = 1)
			OR 
			(   EXISTS (SELECT * from f_split(tbl.iID_MaDonViQuanLy) INTERSECT SELECT iID_MaDonVi FROM NS_NguoiDung_DonVi  WHERE sMaNguoiDung = @UserName and iNamLamViec = @YearOfWork)
				AND (@CountDonViCha <> 0)
			)
		) and (tbl.bTongHop is null or tbl.bTongHop != 1)
		AND (@dNgayDeNghiFrom IS NULL OR tbl.dNgayDeNghi >= @dNgayDeNghiFrom)
		AND (@dNgayDeNghiTo IS NULL OR tbl.dNgayDeNghi <= @dNgayDeNghiTo)
		AND (@iNamKeHoach IS NULL OR tbl.iNamKeHoach = @iNamKeHoach)
		AND (@sSoDeNghi IS NULL OR tbl.sSoDeNghi like CONCAT('%', @sSoDeNghi, '%'))
		and (@iidDuAn is null or tbl.iID_DuAnId = @iidDuAn)
		and (@sMaDonVi is null or tbl.iID_MaDonViQuanLy = @sMaDonVi)
		and (@iLoaiThanhToan is null or tbl.iLoaiThanhToan = @iLoaiThanhToan)

		ORDER BY tbl.dDateCreate DESC
	END 

	set @iToTalItem = (
		select count(*) from VDT_TT_DeNghiThanhToan tbl
		WHERE 
		(
			( EXISTS (SELECT iID_MaDonViQuanLy as iID_MaDonVi from VDT_TT_DeNghiThanhToan INTERSECT SELECT iID_MaDonVi FROM NS_NguoiDung_DonVi  WHERE sMaNguoiDung = @UserName and iNamLamViec = @YearOfWork)
				AND (@CountDonViCha = 0)
			)
			OR (@CountDonViCha <> 0 AND tbl.bKhoa = 1)
			OR 
			(   EXISTS (SELECT iID_MaDonViQuanLy as iID_MaDonVi from VDT_TT_DeNghiThanhToan INTERSECT SELECT iID_MaDonVi FROM NS_NguoiDung_DonVi  WHERE sMaNguoiDung = @UserName and iNamLamViec = @YearOfWork)
				AND (@CountDonViCha <> 0)
			)
		) and (tbl.bTongHop is null or tbl.bTongHop != 1)
		AND (@dNgayDeNghiFrom IS NULL OR tbl.dNgayDeNghi >= @dNgayDeNghiFrom)
		AND (@dNgayDeNghiTo IS NULL OR tbl.dNgayDeNghi <= @dNgayDeNghiTo)
		AND (@iNamKeHoach IS NULL OR tbl.iNamKeHoach = @iNamKeHoach)
		AND (@sSoDeNghi IS NULL OR tbl.sSoDeNghi like CONCAT('%', @sSoDeNghi, '%'))
		and (@iidDuAn is null or tbl.iID_DuAnId = @iidDuAn)
		and (@sMaDonVi is null or tbl.iID_MaDonViQuanLy = @sMaDonVi)
		and (@iLoaiThanhToan is null or tbl.iLoaiThanhToan = @iLoaiThanhToan)
	)

	IF @isXuatDanhSach=0
	BEGIN
		select * from #res1
		DROP TABLE #res1
	END
	ELSE IF @isXuatDanhSach=1
	BEGIN
		SELECT * FROM #res2
		DROP TABLE #res2
	END
	DROP TABLE #tmp
	
END
;