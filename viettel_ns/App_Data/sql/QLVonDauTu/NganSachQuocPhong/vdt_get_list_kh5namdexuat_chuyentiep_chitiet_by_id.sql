
DECLARE @iNamLamViec int				set @iNamLamViec = 0
DECLARE @sTen nvarchar(500)			set @sTen = ''
DECLARE @sTenDonVi nvarchar(500)	set @sTenDonVi = ''
DECLARE @sDiaDiem nvarchar(500)		set @sDiaDiem = ''
DECLARE @iGiaiDoanTu nvarchar(500)	set @iGiaiDoanTu = ''
DECLARE @iGiaiDoanDen nvarchar(500)	set @iGiaiDoanDen = ''
--#DECLARE#--

	-- Insert statements for procedure here
	DECLARE	@IdDonVi nvarchar(1000)
	DECLARE	@NamLamViec int
	DECLARE @GiaiDoanTu int
	SELECT @IdDonVi = iID_MaDonViQuanLy, @NamLamViec = iNamLamViec, @GiaiDoanTu = iGiaiDoanTu FROM VDT_KHV_KeHoach5Nam_DeXuat WHERE iID_KeHoach5Nam_DeXuatID = @iID_KeHoach5Nam_DeXuatID;

	WITH PhanBoVon AS (
		SELECT
			phanbovonChiTiet.iID_DuAnID as iID_DuAnID,
			SUM(ISNULL(phanbovonChiTiet.fCapPhatTaiKhoBac, 0)) AS fCapPhatTaiKhoBac,
			SUM(ISNULL(phanbovonChiTiet.fCapPhatBangLenhChi, 0)) AS fCapPhatBangLenhChi
		FROM VDT_KHV_KeHoachVonNam_DuocDuyet_ChiTiet phanbovonChiTiet
		INNER JOIN VDT_KHV_KeHoachVonNam_DuocDuyet phanbovon
			ON phanbovon.iID_KeHoachVonNam_DuocDuyetID = phanbovonChiTiet.iID_KeHoachVonNam_DuocDuyetID
		INNER JOIN VDT_DA_DuAn duan
			ON duan.iID_DuAnID = phanbovonChiTiet.iID_DuAnID
		WHERE
			phanbovon.iNamKeHoach <= @GiaiDoanTu - 2
			AND phanbovon.iNamKeHoach >= CAST(duan.sKhoiCong AS int)
			AND phanbovon.iNamKeHoach <= CAST(duan.sKetThuc AS int)
		GROUP BY phanbovonChiTiet.iID_DuAnID
	),PhanBoVonDonVi AS (
		SELECT
			phanbovonChiTiet.iID_DuAnID as iID_DuAnID,
			SUM(ISNULL(phanbovonChiTiet.fThanhToan, 0)) AS fThanhToan
		FROM VDT_KHV_KeHoachVonNam_DeXuat_ChiTiet phanbovonChiTiet
		INNER JOIN VDT_KHV_KeHoachVonNam_DeXuat phanbovon
			ON phanbovon.iID_KeHoachVonNamDeXuatID = phanbovonChiTiet.iID_KeHoachVonNamDeXuatID
		INNER JOIN VDT_DA_DuAn duan
			ON duan.iID_DuAnID = phanbovonChiTiet.iID_DuAnID
		WHERE
			phanbovon.iNamKeHoach = @GiaiDoanTu - 1
			--AND phanbovon.iNamKeHoach >= CAST(duan.sKhoiCong AS int)
			--AND phanbovon.iNamKeHoach <= CAST(duan.sKetThuc AS int)
			--AND phanbovon.iID_MaDonViQuanLy = @IdDonVi
		GROUP BY phanbovonChiTiet.iID_DuAnID
	),ThongTinDuAn AS (
		SELECT
			duan.iID_DuAnID AS iIDDuAnID,
			duan.sTenDuAn AS sTen,
			duan.sDiaDiem AS sDiaDiem,
			CAST(duan.sKhoiCong AS int) AS iGiaiDoanTu,
			CAST(duan.sKetThuc AS int) AS iGiaiDoanDen,
			duan.fHanMucDauTu AS fHanMucDauTu,
			donvi.iID_DonVi AS IIdDonViId,
			donvi.iID_MaDonVi AS iIDMaDonVi,
			donvi.sTenDonVi AS STenDonVi,
			(CASE WHEN duanHangMuc.iID_DuAn_HangMucID IS NULL THEN loaiCongTrinh.iID_LoaiCongTrinh ELSE duanHangMuc.iID_LoaiCongTrinhID END) AS iIDLoaiCongTrinhID,
			(CASE WHEN duanHangMuc.iID_DuAn_HangMucID IS NULL THEN loaiCongTrinh.sTenLoaiCongTrinh ELSE duanHangMuc.sTenHangMuc END) AS sTenLoaiCongTrinh,
			nguonNganSach.iID_MaNguonNganSach AS iIDNguonVonID,
			CONCAT(nguonNganSach.iID_MaNguonNganSach, ' - ', nguonNganSach.sTen) as sTenNguonVon,
			ISNULL(phanbovon.fCapPhatBangLenhChi, 0) + ISNULL(phanbovon.fCapPhatTaiKhoBac, 0) AS fVonNSQPLuyKe,
			ISNULL(phanbovonDonVi.fThanhToan, 0) AS fVonNSQP
		FROM VDT_DA_DuAn duan
		LEFT JOIN VDT_DM_DonViThucHienDuAn donvi
			ON duan.iID_DonViThucHienDuAnID  = donvi.iID_DonVi
		LEFT JOIN VDT_DA_DuAn_NguonVon duanNguonvon
			ON duan.iID_DuAnID = duanNguonvon.iID_DuAn
		LEFT JOIN VDT_DM_LoaiCongTrinh loaiCongTrinh
			ON duan.iID_LoaiCongTrinhID = loaiCongTrinh.iID_LoaiCongTrinh
		LEFT JOIN VDT_DA_DuAn_HangMuc duanHangMuc
			ON duan.iID_DuAnID = duanHangMuc.iID_DuAnID AND duanHangMuc.iID_NguonVonID = duanNguonvon.iID_NguonVonID
		LEFT JOIN NS_NguonNganSach nguonNganSach
			ON duanNguonvon.iID_NguonVonID = nguonNganSach.iID_MaNguonNganSach
		LEFT JOIN PhanBoVon phanbovon
			ON phanbovon.iID_DuAnID = duan.iID_DuAnID
		LEFT JOIN PhanBoVonDonVi phanbovonDonVi
			ON phanbovonDonVi.iID_DuAnID = duan.iID_DuAnID
		WHERE
			1=1
			AND duan.sTrangThaiDuAn = 'THUC_HIEN'
			--AND duan.bIsKetThuc IS NULL
			--AND iID_MaDonViThucHienDuAnID = @IdDonVi
	), ThongTinChiTiet AS (
		SELECT
			distinct
			khthChiTiet.*,
			khthChiTietParent.fGiaTriNamThuNhat AS FGiaTriNamThuNhatOrigin,
			khthChiTietParent.fGiaTriNamThuHai	AS FGiaTriNamThuHaiOrigin,
			khthChiTietParent.fGiaTriNamThuBa	AS FGiaTriNamThuBaOrigin,
			khthChiTietParent.fGiaTriNamThuTu	AS FGiaTriNamThuTuOrigin,
			khthChiTietParent.fGiaTriNamThuNam	AS FGiaTriNamThuNamOrigin,
			khth.iLoai
		FROM VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet khthChiTiet
		LEFT JOIN VDT_KHV_KeHoach5Nam_DeXuat_ChiTiet khthChiTietParent
			ON khthChiTiet.iID_KeHoach5Nam_DeXuat_ChiTietID = khthChiTietParent.iID_ParentModified
		INNER JOIN VDT_KHV_KeHoach5Nam_DeXuat khth
			ON khthChiTiet.iID_KeHoach5Nam_DeXuatID = khth.iID_KeHoach5Nam_DeXuatID
		WHERE khth.iID_KeHoach5Nam_DeXuatID = @iID_KeHoach5Nam_DeXuatID
	)

	SELECT
		khthChiTiet.iID_ParentModified,
		khthChiTiet.iID_KeHoach5Nam_DeXuat_ChiTietID,
		khthChiTiet.iID_KeHoach5Nam_DeXuatID,
		isnull(khthChiTiet.fGiaTriKeHoach, 0) as fGiaTriKeHoach,
		khthChiTiet.iID_DonViTienTeID,
		khthChiTiet.fTiGiaDonVi,
		khthChiTiet.fTiGia,
		khthChiTiet.sTrangThai,
		khthChiTiet.sGhiChu,
		isnull(khthChiTiet.fGiaTriNamThuNhat, 0) as fGiaTriNamThuNhat,
		isnull(khthChiTiet.fGiaTriNamThuHai, 0) as fGiaTriNamThuHai,
		isnull(khthChiTiet.fGiaTriNamThuBa, 0) as fGiaTriNamThuBa,
		isnull(khthChiTiet.fGiaTriNamThuTu, 0) as fGiaTriNamThuTu,
		isnull(khthChiTiet.fGiaTriNamThuNam,0) as fGiaTriNamThuNam,
		isnull(khthChiTiet.fGiaTriBoTri,0) as fGiaTriBoTri, 
		cast(0 as float) as fGiaTriNamThuNhatDc,
		cast(0 as float) as fGiaTriNamThuHaiDc,
		cast(0 as float) as fGiaTriNamThuBaDc,
		cast(0 as float) as fGiaTriNamThuTuDc,
		cast(0 as float) as fGiaTriNamThuNamDc,
		cast(0 as float) as fGiaTriBoTriDc,
		isnull(khthChiTiet.fVonDaGiao, 0) as fVonDaGiao,
		isnull(khthChiTiet.fVonBoTriTuNamDenNam,0) as fVonBoTriTuNamDenNam,
		duan.fHanMucDauTu,
		duan.fVonNSQP as fLuyKeNSQPDeNghiBoTri,
		duan.fVonNSQPLuyKe as fLuyKeNSQPDaBoTri,
		duan.iIDNguonVonID as iID_NguonVonID,
		khthChiTiet.iLoai,
		duan.iIDLoaiCongTrinhID as iID_LoaiCongTrinhID,
		duan.IIdDonViId as iID_DonViQuanLyID,
		duan.sTen as sTen,
		khthChiTiet.iIDReference,
		duan.sDiaDiem,
		duan.iGiaiDoanTu,
		duan.iGiaiDoanDen,
		khthChiTiet.iID_ParentID,
		khthChiTiet.sMaOrder,
		khthChiTiet.sSTT,
		10 as iLevel,
		khthChiTiet.iIndexCode,
		cast(0 as bit) as bIsParent,
		ISNULL(khthChiTiet.fGiaTriNamThuNhat, 0) + ISNULL(khthChiTiet.fGiaTriNamThuHai, 0) + ISNULL(khthChiTiet.fGiaTriNamThuBa, 0) + ISNULL(khthChiTiet.fGiaTriNamThuTu, 0) + ISNULL(khthChiTiet.fGiaTriNamThuNam, 0) as fTongSo,
		case 
		when khthChiTiet.iID_NguonVonID = 1
			then
				ISNULL(khthChiTiet.fHanMucDauTu, 0)
			else
				0
		end fTongSoNhuCauNSQP,
		(
			CASE
				WHEN duan.iIDMaDonVi is null THEN ''
				ELSE CONCAT(duan.iIDMaDonVi, ' - ', duan.sTenDonVi)
			END
		) as sDonViThucHienDuAn,
		'' as sDuAnCha,
		duan.sTenLoaiCongTrinh,
		duan.sTenNguonVon as sTenNganSach,
		null as numChild,
		duan.iIDDuAnID as iID_DuAnID,
		(ISNULL(khthChiTiet.fVon5namNganhDX,0) + ISNULL(khthChiTiet.fVonsaunamNganhDX,0)) as fTongVonBoTriNganh,
		(ISNULL(khthChiTiet.fVon5namCTCDX,0) + ISNULL(khthChiTiet.fVonsaunamCTCDexuat,0)) as fTongVonBoTriCuc,
		khthChiTiet.fHanmucNganhDX,
		khthChiTiet.fVon5namNganhDX,
		khthChiTiet.fVonsaunamNganhDX,
		khthChiTiet.fVon5namCTCDX,
		khthChiTiet.fHanmucCucTCDX,
		khthChiTiet.fVonnamthunhatCTC,
		khthChiTiet.fVonsaunamCTCDexuat,
		khthChiTiet.fCucTCDeXuat,
		khthChiTiet.fDuKienBoTriNamThu2
	FROM ThongTinDuAn duan
	left JOIN ThongTinChiTiet khthChiTiet
		ON duan.iIDDuAnID = khthChiTiet.iID_DuAnID
		--AND duan.iIDMaDonVi = khthChiTiet.iID_MaDonVi
		--AND duan.iIDLoaiCongTrinhID = khthChiTiet.iID_LoaiCongTrinhID
		--AND duan.iIDNguonVonID = khthChiTiet.iID_NguonVonID
	WHERE
		1 = 1
		-- and (@sTen is null or khthchitiet.sTen like @sTen)
		-- and (@sTenDonVi is null or duan.STenDonVi like @sTenDonVi)
		-- and (@sDiaDiem is null or khthchitiet.sDiaDiem like @sdiadiem)
		-- and (@iGiaiDoanTu is null or khthchitiet.iGiaiDoanTu like @iGiaiDoanTu)
		-- and (@iGiaiDoanDen is null or khthchitiet.iGiaiDoanDen like @iGiaiDoanDen)
		ORDER BY duan.iIDDuAnID, sTenDonVi, sTenLoaiCongTrinh, sTenNguonVon